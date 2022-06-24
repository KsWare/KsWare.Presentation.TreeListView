using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using KsWare.Presentation.TreeListView.Behaviors.Column;
using KsWare.Presentation.TreeListView.Controls;

namespace KsWare.Presentation.TreeListView.Behaviors {

	/// <summary>
	/// Class responsible for handling the size of the column of the ListView GridView.
	/// </summary>
	/// <remarks>Implemented using the project "http://www.codeproject.com/Articles/25058/ListView-Layout-Manager".</remarks>
	public class ColumnResizeBehavior {

		#region Fields

		/// <summary>
		/// Stores the delta defining if a range can be considered as zero in pixel.
		/// </summary>
		private const double ZeroWidthRange = 0.1;

		/// <summary>
		/// Stores the managed list view.
		/// </summary>
		private readonly ExtendedListView _listView;

		/// <summary>
		/// Stores the scrol viewer of the list view.
		/// </summary>
		private ScrollViewer _scrollViewer;

		/// <summary>
		/// Stores the flag indicating if the manager is loaded.
		/// </summary>
		private bool _loaded;

		/// <summary>
		/// Stores the flag indicating if the list view is resizing.
		/// </summary>
		private bool _resizing;

		/// <summary>
		/// Stores the default cursor. Used during resizing.
		/// </summary>
		private Cursor _backCursor;

		/// <summary>
		/// Stores the backup horizontal scroll bar visibility.
		/// </summary>
		private ScrollBarVisibility _backupHorizontalScrollBarVisibility;

		/// <summary>
		/// Stores the backup vertical scroll bar visibility.
		/// </summary>
		private ScrollBarVisibility _backupVerticalScrollBarVisibility;

		/// <summary>
		/// Stores the vertical scroll bar visibility.
		/// </summary>
		private readonly ScrollBarVisibility _verticalScrollBarVisibility;

		/// <summary>
		/// Stores the column that need to be resized by a specific process.
		/// </summary>
		private GridViewColumn _autoSizedColumn;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ColumnResizeBehavior"/> class.
		/// </summary>
		/// <param name="listView">The managed list view.</param>
		public ColumnResizeBehavior(ExtendedListView listView) {
			if (listView == null) {
				throw new ArgumentNullException(nameof(listView));
			}

			_verticalScrollBarVisibility = ScrollBarVisibility.Auto;
			_listView = listView;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Activates the behavior.
		/// </summary>
		public void Activate() {
			if (_listView.IsLoaded) {
				Initialize();
			}
			else {
				_listView.Loaded -= new RoutedEventHandler(OnListViewLoaded);
				_listView.Loaded += new RoutedEventHandler(OnListViewLoaded);
			}
		}

		/// <summary>
		/// Dectivates the behavior.
		/// </summary>
		public void Deactivate() {
			if (_loaded == false) {
				return;
			}

			UnregisterEvents(_listView);
			_loaded = false;
		}

		/// <summary>
		/// Initializes the behavior.
		/// </summary>
		public void Initialize() {
			RegisterEvents(_listView);
			InitColumns();
			DoResizeColumns(-1);
			_loaded = true;
		}

		/// <summary>
		/// Delegate called when the managed list view is loaded.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnListViewLoaded(object sender, RoutedEventArgs eventArgs) {
			Initialize();
		}

		/// <summary>
		/// Registers on the wanted events on the dependency object.
		/// </summary>
		/// <param name="object">The object on witch to register the delegates.</param>
		private void RegisterEvents(DependencyObject @object) {
			for (var iter = 0; iter < VisualTreeHelper.GetChildrenCount(@object); iter++) {
				var childVisual = VisualTreeHelper.GetChild(@object, iter) as Visual;
				if (childVisual is Thumb) {
					var column = FindParentColumn(childVisual);
					if (column != null) {
						var thumb = childVisual as Thumb;
						if (FixedColumn.IsFixedColumn(column) || IsFillColumn(column)) {
							// Do not allow resize on the fixed column.
							thumb.IsHitTestVisible = false;
						}
						else {
							// Registering.
							thumb.PreviewMouseMove += new MouseEventHandler(OnThumbPreviewMouseMove);
							thumb.PreviewMouseLeftButtonDown +=
								new MouseButtonEventHandler(OnThumbPreviewMouseLeftButtonDown);
							DependencyPropertyDescriptor
								.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
								.AddValueChanged(column, OnGridColumnWidthChanged);
						}
					}
				}
				else if (childVisual is GridViewColumnHeader) {
					var columnHeader = childVisual as GridViewColumnHeader;
					columnHeader.SizeChanged += new SizeChangedEventHandler(OnGridColumnHeaderSizeChanged);
				}
				else if (_scrollViewer == null && childVisual is ScrollViewer) {
					_scrollViewer = childVisual as ScrollViewer;
					_scrollViewer.ScrollChanged += new ScrollChangedEventHandler(OnScrollViewerScrollChanged);

					// Saving the current scrollbar parameters.
					_backupHorizontalScrollBarVisibility = _scrollViewer.HorizontalScrollBarVisibility;
					_backupVerticalScrollBarVisibility = _scrollViewer.VerticalScrollBarVisibility;

					// Assume we do the regulation of the horizontal scrollbar.
					_scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
					_scrollViewer.VerticalScrollBarVisibility = _verticalScrollBarVisibility;
				}

				RegisterEvents(childVisual);
			}
		}

		/// <summary>
		/// Unregisters from the wanted events of the dependency object.
		/// </summary>
		/// <param name="object">The object on witch to unregister the delegates.</param>
		private void UnregisterEvents(DependencyObject @object) {
			for (var iter = 0; iter < VisualTreeHelper.GetChildrenCount(@object); iter++) {
				var childVisual = VisualTreeHelper.GetChild(@object, iter) as Visual;
				if (childVisual is Thumb) {
					var column = FindParentColumn(childVisual);
					if (column != null) {
						var thumb = childVisual as Thumb;
						if (FixedColumn.IsFixedColumn(column) || IsFillColumn(column)) {
							thumb.IsHitTestVisible = true;
						}
						else {
							thumb.PreviewMouseMove -= new MouseEventHandler(OnThumbPreviewMouseMove);
							thumb.PreviewMouseLeftButtonDown -=
								new MouseButtonEventHandler(OnThumbPreviewMouseLeftButtonDown);
							DependencyPropertyDescriptor
								.FromProperty(GridViewColumn.WidthProperty, typeof(GridViewColumn))
								.RemoveValueChanged(column, OnGridColumnWidthChanged);
						}
					}
				}
				else if (childVisual is GridViewColumnHeader) {
					var columnHeader = childVisual as GridViewColumnHeader;
					columnHeader.SizeChanged -= new SizeChangedEventHandler(OnGridColumnHeaderSizeChanged);
				}
				else if (_scrollViewer == null && childVisual is ScrollViewer) {
					_scrollViewer = childVisual as ScrollViewer;
					_scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(OnScrollViewerScrollChanged);

					// Back applying the scrollbar states.
					_scrollViewer.HorizontalScrollBarVisibility = _backupHorizontalScrollBarVisibility;
					_scrollViewer.VerticalScrollBarVisibility = _backupVerticalScrollBarVisibility;
				}

				UnregisterEvents(childVisual);
			}
		}

		/// <summary>
		/// Initialize the columns by applying there properties.
		/// </summary>
		private void InitColumns() {
			if (_listView.View == null) {
				return;
			}

			foreach (var column in _listView.View.Columns) {
				if (RangeColumn.IsRangeColumn(column) == false) {
					continue;
				}

				var minWidth = RangeColumn.GetRangeMinWidth(column);
				var maxWidth = RangeColumn.GetRangeMaxWidth(column);
				if (minWidth.HasValue == false && maxWidth.HasValue == false) {
					continue;
				}

				var columnHeader = FindColumnHeader(_listView, column);
				if (columnHeader == null) {
					continue;
				}

				var actualWidth = columnHeader.ActualWidth;
				if (minWidth.HasValue) {
					columnHeader.MinWidth = minWidth.Value;
					if (!double.IsInfinity(actualWidth) && actualWidth < columnHeader.MinWidth) {
						column.Width = columnHeader.MinWidth;
					}
				}

				if (maxWidth.HasValue) {
					columnHeader.MaxWidth = maxWidth.Value;
					if (double.IsInfinity(actualWidth) == false && actualWidth > columnHeader.MaxWidth) {
						column.Width = columnHeader.MaxWidth;
					}
				}
			}
		}

		/// <summary>
		/// Tries to find the parent column of the given object.
		/// </summary>
		/// <param name="object">The object to parse.</param>
		/// <returns>The parent column if any.</returns>
		private GridViewColumn FindParentColumn(DependencyObject @object) {
			if (@object == null) {
				return null;
			}

			while (@object != null) {
				var columnHeader = @object as GridViewColumnHeader;
				if (columnHeader != null) {
					return columnHeader.Column;
				}

				@object = VisualTreeHelper.GetParent(@object);
			}

			return null;
		}

		/// <summary>
		/// Tries to find the column header of the given column in the given object.
		/// </summary>
		/// <param name="object">The object to parse.</param>
		/// <param name="column">The header column.</param>
		/// <returns>The parent column if any.</returns>
		private GridViewColumnHeader FindColumnHeader(DependencyObject @object, GridViewColumn column) {
			for (var iter = 0; iter < VisualTreeHelper.GetChildrenCount(@object); iter++) {
				var childVisual = VisualTreeHelper.GetChild(@object, iter) as Visual;
				if (childVisual is GridViewColumnHeader) {
					var columnHeader = childVisual as GridViewColumnHeader;
					if (columnHeader.Column == column) {
						return columnHeader;
					}
				}

				// Recursive call.
				var childColumnHeader = FindColumnHeader(childVisual, column);
				if (childColumnHeader != null) {
					return childColumnHeader;
				}
			}
			return null;
		}

		/// <summary>
		/// Resizes the grid view columns.
		/// </summary>
		/// <param name="columnIndex">The specific index of the resized column, or -1 if the whole control is resized.</param>
		private void DoResizeColumns(int columnIndex) {
			if (_resizing) {
				return;
			}

			_resizing = true;
			try {
				ResizeColumns(columnIndex);
			}
			finally {
				_resizing = false;
			}
		}

		/// <summary>
		/// Resizes the grid view columns.
		/// </summary>
		/// <param name="columnIndex">The specific index of the resized column, or -1 if the whole control is resized.</param>
		private void ResizeColumns(int columnIndex) {
			if (_listView.View == null || _listView.View.Columns.Count == 0) {
				return;
			}

			// Computing the listview width.
			var actualWidth = double.PositiveInfinity;
			if (_scrollViewer != null) {
				actualWidth = _scrollViewer.ViewportWidth;
			}
			if (double.IsInfinity(actualWidth)) {
				actualWidth = _listView.ActualWidth;
			}
			if (double.IsInfinity(actualWidth) || actualWidth <= 0) {
				return;
			}

			// Computing column sizes.
			double resizeableRegionCount = 0;
			double otherColumnsWidth = 0;
			foreach (var column in _listView.View.Columns) {
				if (ProportionalColumn.IsProportionalColumn(column)) {
					var proportionalWidth = ProportionalColumn.GetProportionalWidth(column);
					if (proportionalWidth != null) {
						resizeableRegionCount += proportionalWidth.Value;
					}
				}
				else {
					otherColumnsWidth += column.ActualWidth;
				}
			}

			if (resizeableRegionCount <= 0) {
				// No proportional columns present : commit the regulation to the scroll viewer.
				if (_scrollViewer != null) {
					_scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
				}

				// Searching for the first fill column.
				GridViewColumn fillColumn = null;
				for (var iter = 0; iter < _listView.View.Columns.Count; iter++) {
					var column = _listView.View.Columns[iter];
					if (IsFillColumn(column)) {
						fillColumn = column;
						break;
					}
				}

				if (fillColumn != null) {
					// Applying the width to the fill column taking in account the range.
					var otherColumnsWithoutFillWidth = otherColumnsWidth - fillColumn.ActualWidth;
					var fillWidth = actualWidth - otherColumnsWithoutFillWidth;
					if (fillWidth > 0) {
						var minWidth = RangeColumn.GetRangeMinWidth(fillColumn);
						var maxWidth = RangeColumn.GetRangeMaxWidth(fillColumn);

						var setWidth = (minWidth.HasValue && fillWidth < minWidth.Value) == false;
						if (maxWidth.HasValue && fillWidth > maxWidth.Value) {
							setWidth = false;
						}

						if (setWidth) {
							if (_scrollViewer != null) {
								_scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
							}
							fillColumn.Width = fillWidth;
						}
					}
				}
				return;
			}

			// Some proportional columns have been defined.
			var resizeableColumnsWidth = actualWidth - otherColumnsWidth;
			if (resizeableColumnsWidth <= 0) {
				// Missing space.
				return;
			}

			// Resize proportional columns.
			var resizeableRegionWidth = resizeableColumnsWidth / resizeableRegionCount;
			foreach (var column in _listView.View.Columns) {
				if (ProportionalColumn.IsProportionalColumn(column)) {
					if (columnIndex == -1) {
						// Computing the initial width.
						var proportionalWidth = ProportionalColumn.GetProportionalWidth(column);
						if (proportionalWidth != null) {
							column.Width = proportionalWidth.Value * resizeableRegionWidth;
						}
					}
					else {
						var currentIndex = _listView.View.Columns.IndexOf(column);
						if (columnIndex == currentIndex) {
							// Adapting the ratio so that the column can be resized.
							ProportionalColumn.ApplyWidth(column, column.Width / resizeableRegionWidth);
						}
						else if (currentIndex > columnIndex) {
							// Computing the initial width for the colums after the one resized.
							var proportionalWidth = ProportionalColumn.GetProportionalWidth(column);
							if (proportionalWidth != null) {
								column.Width = proportionalWidth.Value * resizeableRegionWidth;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Applies the bound to the given grid.
		/// </summary>
		/// <param name="column">The column to bound.</param>
		/// <returns>The resize delta after applying the bound.</returns>
		private double SetRangeColumnToBounds(GridViewColumn column) {
			if (RangeColumn.IsRangeColumn(column) == false) {
				// No need to resize.
				return 0;
			}

			var startWidth = column.Width;

			var minWidth = RangeColumn.GetRangeMinWidth(column);
			var maxWidth = RangeColumn.GetRangeMaxWidth(column);

			if ((minWidth.HasValue && maxWidth.HasValue) && (minWidth > maxWidth)) {
				// Invalid case. No resize.
				return 0;
			}

			// Bounding the width.
			if (minWidth.HasValue && column.Width < minWidth.Value) {
				column.Width = minWidth.Value;
			}
			else if (maxWidth.HasValue && column.Width > maxWidth.Value) {
				column.Width = maxWidth.Value;
			}

			return column.Width - startWidth;
		}

		/// <summary>
		/// Returns the flag indicating if the given is a fill column.
		/// </summary>
		/// <param name="column">The column to test.</param>
		/// <returns></returns>
		private bool IsFillColumn(GridViewColumn column) {
			if (column == null) {
				return false;
			}

			if (_listView.View == null || _listView.View.Columns.Count == 0) {
				return false;
			}

			var isFillColumn = RangeColumn.GetRangeIsFillColumn(column);
			return isFillColumn.HasValue && isFillColumn.Value;
		}

		/// <summary>
		/// Delegate called when a mouse is mouving on a thumb.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnThumbPreviewMouseMove(object sender, MouseEventArgs eventArgs) {
			var thumb = sender as Thumb;
			if (thumb == null) {
				return;
			}

			var column = FindParentColumn(thumb);
			if (column == null) {
				return;
			}

			// Suppress column resizing for fixed and range fill columns.
			if (FixedColumn.IsFixedColumn(column) || IsFillColumn(column)) {
				// Cursor is hidden.
				thumb.Cursor = null;
				return;
			}

			// Check range column bounds.
			if (thumb.IsMouseCaptured && RangeColumn.IsRangeColumn(column)) {
				var minWidth = RangeColumn.GetRangeMinWidth(column);
				var maxWidth = RangeColumn.GetRangeMaxWidth(column);

				if ((minWidth.HasValue && maxWidth.HasValue) && (minWidth > maxWidth)) {
					// Invalid case.
					return;
				}

				if (_backCursor == null) {
					// First time = save the resize cursor.
					_backCursor = thumb.Cursor;
				}

				// Updating the cursor.
				if (minWidth.HasValue && column.Width <= minWidth.Value) {
					thumb.Cursor = Cursors.No;
				}
				else if (maxWidth.HasValue && column.Width >= maxWidth.Value) {
					thumb.Cursor = Cursors.No;
				}
				else {
					thumb.Cursor = _backCursor;
				}
			}
		}

		/// <summary>
		/// Delegate called when a mouse left button is down on a thumb.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnThumbPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs) {
			var thumb = sender as Thumb;
			if (thumb == null) {
				return;
			}

			var column = FindParentColumn(thumb);
			if (column == null) {
				return;
			}

			// Suppress column resizing for fixed and range fill columns.
			if (FixedColumn.IsFixedColumn(column) || IsFillColumn(column)) {
				eventArgs.Handled = true;
			}
		}

		/// <summary>
		/// Delegate called when a column width change.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnGridColumnWidthChanged(object sender, EventArgs eventArgs) {
			if (_loaded == false) {
				return;
			}

			var column = sender as GridViewColumn;
			if (column == null) {
				return;
			}

			// Suppress column resizing for fixed columns.
			if (FixedColumn.IsFixedColumn(column)) {
				return;
			}

			// Ensure range column within the bounds.
			if (RangeColumn.IsRangeColumn(column)) {
				// Special case: auto column width - maybe conflicts with min/max range.
				if (column != null && column.Width.Equals(double.NaN)) {
					// Resize is handled by the change header size event.
					_autoSizedColumn = column;
					return;
				}

				// ensure column bounds
				if (Math.Abs(SetRangeColumnToBounds(column) - 0) > ZeroWidthRange) {
					return;
				}
			}

			// Force resize.
			DoResizeColumns(_listView.View.Columns.IndexOf(column));
		}

		/// <summary>
		/// Delegate called when a column header width change.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnGridColumnHeaderSizeChanged(object sender, SizeChangedEventArgs eventArgs) {
			if (_autoSizedColumn == null) {
				return;
			}

			// Handling the resizing of the auto sized column.
			var columnHeader = sender as GridViewColumnHeader;
			if (columnHeader != null && columnHeader.Column == _autoSizedColumn) {
				if (columnHeader.Width.Equals(double.NaN)) {
					// Sync column width. 
					columnHeader.Column.Width = columnHeader.ActualWidth;

					// Force resize.
					DoResizeColumns(_listView.View.Columns.IndexOf(columnHeader.Column));
				}

				_autoSizedColumn = null;
			}
		}

		/// <summary>
		/// Delegate called whenchanges are detected to the scroll position, extent, or viewport size.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs eventArgs) {
			if (_loaded && Math.Abs(eventArgs.ViewportWidthChange - 0) > ZeroWidthRange) {
				DoResizeColumns(-1);
			}
		}

		#endregion // Methods.

	}

}