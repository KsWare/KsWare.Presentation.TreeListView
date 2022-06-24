using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.TreeListView.Internal.Extensions;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// Class defining a grid view header row presenter.
	/// </summary>
	public class ExtendedGridViewHeaderRowPresenter : GridViewHeaderRowPresenter {

		#region Fields

		/// <summary>
		/// Stores the minimum width for padding header.
		/// </summary>
		private const double PaddingHeaderMinWidth = 2.0;

		/// <summary>
		/// Stores the padding header.
		/// </summary>
		private GridViewColumnHeader _paddingHeader;

		/// <summary>
		/// Stores the indicator diplayed when a header is dragged.
		/// </summary>
		private Separator _dragIndicator;

		#endregion // Fields.

		#region Properties

		/// <summary>
		/// Gets the flag indicating if a column header is dragging.
		/// </summary>
		private bool IsHeaderDragging => this.GetFieldValue<GridViewHeaderRowPresenter, bool>("_isHeaderDragging");

		/// <summary>
		/// Gets the mouse current position when a header is dragged.
		/// </summary>
		private Point DragCurrentPosition => this.GetFieldValue<GridViewHeaderRowPresenter, Point>("_currentPos");

		/// <summary>
		/// Gets the mouse start position when a header is going to be dragged.
		/// </summary>
		private Point DragRelativeStartPosition => this.GetFieldValue<GridViewHeaderRowPresenter, Point>("_relativeStartPos");

		/// <summary>
		/// Gets the index of the column of the header that is going to be dragged.
		/// </summary>
		private int DragStartColumnIndex => this.GetFieldValue<GridViewHeaderRowPresenter, int>("_startColumnIndex");

		/// <summary>
		/// Gets the index of the destination column of the dragged header.
		/// </summary>
		private int DragDestColumnIndex => this.GetFieldValue<GridViewHeaderRowPresenter, int>("_desColumnIndex");

		/// <summary>
		/// Gets the header positions list.
		/// </summary>
		private List<Rect> HeadersPositionList => this.GetPropertyValue<GridViewHeaderRowPresenter, List<Rect>>("HeadersPositionList");

		/// <summary>
		/// Gets the list of currently reached max value of DesiredWidth of cell in the column
		/// </summary>
		private List<double> DesiredWidthList => this.GetPropertyValue<GridViewHeaderRowPresenter, List<double>>("DesiredWidthList");

		/// <summary>
		/// Gets the padding header.
		/// </summary>
		private GridViewColumnHeader PaddingHeader {
			get {
				if (_paddingHeader != null) return _paddingHeader;

				for (var i = 0; i < VisualChildrenCount; ++i) {
					var header = GetVisualChild(i) as GridViewColumnHeader;
					if (header != null) {
						var role =
							(GridViewColumnHeaderRole) header.GetValue(GridViewColumnHeader.RoleProperty);
						if (role == GridViewColumnHeaderRole.Padding) {
							_paddingHeader = header;
							return _paddingHeader;
						}
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the floating header.
		/// </summary>
		private GridViewColumnHeader FloatingHeader {
			get {
				// Do not cache this header as it is modified at each drag.
				for (var i = 0; i < VisualChildrenCount; ++i) {
					if (GetVisualChild(i) is not GridViewColumnHeader header) continue;
					var role = (GridViewColumnHeaderRole) header.GetValue(GridViewColumnHeader.RoleProperty);
					if (role == GridViewColumnHeaderRole.Floating) return header;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the drag indicator.
		/// </summary>
		private Separator DragIndicator {
			get {
				if (_dragIndicator != null) return _dragIndicator;

				for (var i = 0; i < VisualChildrenCount; ++i) {
					if (GetVisualChild(i) is not Separator indicator) continue;
					_dragIndicator = indicator;
					return _dragIndicator;
				}

				return null;
			}
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Override of <seealso cref="FrameworkElement.MeasureOverride" />.
		/// </summary>
		/// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
		/// <returns>The GridViewHeaderRowPresenter's desired size.</returns>
		protected override Size MeasureOverride(Size constraint) {
			var maxHeight = 0.0;
			var accumulatedWidth = 0.0;
			var constraintHeight = constraint.Height;
			var desiredWidthListEnsured = false;

			if (Columns != null) {
				// Measure working headers.
				for (var i = 0; i < Columns.Count; ++i) {
					// Getting the column header visual element.
					var columnHeader = GetVisualChild(GetVisualIndex(i)) as UIElement;
					if (columnHeader == null) {
						continue;
					}

					var childConstraintWidth = Math.Max(0.0, constraint.Width - accumulatedWidth);
					var column = Columns[i] as ExtendedGridViewColumn;

					switch (column.State) {
						case ExtendedGridViewColumnMeasureState.Init:
							if (desiredWidthListEnsured == false) {
								EnsureDesiredWidthList();
								LayoutUpdated += OnLayoutUpdated;
								desiredWidthListEnsured = true;
							}

							columnHeader.Measure(new Size(childConstraintWidth, constraintHeight));
							DesiredWidthList[column.ActualIndex] =
								column.EnsureDesiredWidth(columnHeader.DesiredSize.Width);
							accumulatedWidth += column.DesiredWidth;
							break;
						case ExtendedGridViewColumnMeasureState.Headered:
						case ExtendedGridViewColumnMeasureState.Data:
							//ScrollViewer ParentScrolViewer = this.FindVisualParent<ScrollViewer>();
							//if (ParentScrollViewer.ViewportWidth > 0.0) {
							//    Column.Width = ParentScrollViewer.ViewportWidth / DesiredWidthList.Count;
							//}
							childConstraintWidth = Math.Min(childConstraintWidth, column.DesiredWidth);
							columnHeader.Measure(new Size(childConstraintWidth, constraintHeight));
							accumulatedWidth += column.DesiredWidth;
							break;
						default:
							// ColumnMeasureState.SpecificWidth.
							childConstraintWidth = Math.Min(childConstraintWidth, column.Width);
							columnHeader.Measure(new Size(childConstraintWidth, constraintHeight));
							accumulatedWidth += column.Width;
							break;
					}

					maxHeight = Math.Max(maxHeight, columnHeader.DesiredSize.Height);
				}
			}

			// Measure padding header.
			PaddingHeader.Measure(new Size(0.0, constraintHeight));
			maxHeight = Math.Max(maxHeight, PaddingHeader.DesiredSize.Height);

			// Reserve space for padding header next to the last column.
			accumulatedWidth += PaddingHeaderMinWidth;

			// Measure indicator & floating header in re-ordering.
			if (IsHeaderDragging) {
				// Measure indicator.
				DragIndicator.Measure(constraint);

				// Measure floating header.
				FloatingHeader.Measure(constraint);
			}

			return new Size(accumulatedWidth, maxHeight);
		}

		/// <summary>
		/// Delegate called when the layout is updated.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnLayoutUpdated(object sender, EventArgs eventArgs) {
			// Whether the shared minimum width has been changed since last layout.
			var desiredWidthChanged = false;

			if (Columns != null) {
				foreach (ExtendedGridViewColumn column in Columns) {
					if ((column.State == ExtendedGridViewColumnMeasureState.SpecificWidth)) continue;

					if (column.State == ExtendedGridViewColumnMeasureState.Init) {
						column.State = ExtendedGridViewColumnMeasureState.Headered;
					}

					if (DesiredWidthList == null || column.ActualIndex >= DesiredWidthList.Count) {
						// How can this happen?
						// Between the last measure was called and this update is called, there can be a
						// change done to the ColumnCollection and result in DesiredWidthList out of sync
						// with the column collection. What can we do is end this call asap and the next
						// measure will fix it.
						desiredWidthChanged = true;
						break;
					}

					// TODO Modifying the DesiredWidth if the column is stretched.
					//ScrollViewer ParentScrollViewer = this.FindVisualParent<ScrollViewer>();
					//column.DesiredWidth = ParentScrollViewer.ViewportWidth / DesiredWidthList.Count;
					//desiredWidthChanged = true;

					if (Math.Abs(column.DesiredWidth - DesiredWidthList[column.ActualIndex]) > 0.001) {
						// Update the record because collection operation latter on might
						// need to verified this list again, e.g. insert an 'auto'
						// column, so that we won't trigger unnecessary update due to
						// inconsistency of this column.
						DesiredWidthList[column.ActualIndex] = column.DesiredWidth;

						desiredWidthChanged = true;
					}
				}
			}

			if (desiredWidthChanged) {
				InvalidateMeasure();
			}

			LayoutUpdated -= OnLayoutUpdated;
		}

		/// <summary>
		/// Computes the position of its children inside each child's Margin and calls Arrange on each child.
		/// </summary>
		/// <param name="arrangeSize">Size the GridViewHeaderRowPresenter will assume.</param>
		protected override Size ArrangeOverride(Size arrangeSize) {
			var columns = Columns;

			var accumulatedWidth = 0.0;
			var remainingWidth = arrangeSize.Width;
			Rect rect;

			HeadersPositionList.Clear();

			if (columns != null) {
				// Arrange working headers
				for (var i = 0; i < columns.Count; ++i) {
					var child = GetVisualChild(GetVisualIndex(i)) as UIElement;
					if (child == null) {
						continue;
					}

					var column = columns[i] as ExtendedGridViewColumn;

					// has a given value or 'auto'
					var childArrangeWidth = Math.Min(remainingWidth,
						((column.State == ExtendedGridViewColumnMeasureState.SpecificWidth)
							? column.Width
							: column.DesiredWidth));

					// calculate the header rect
					rect = new Rect(accumulatedWidth, 0.0, childArrangeWidth, arrangeSize.Height);

					// arrange header
					child.Arrange(rect);

					//Store rect in HeadersPositionList as i-th column position
					HeadersPositionList.Add(rect);

					remainingWidth -= childArrangeWidth;
					accumulatedWidth += childArrangeWidth;
				}

				// check width to hide previous header's right half gripper, from the first working header to padding header
				// only happens after column delete, insert, move
				//if (_isColumnChangedOrCreated) {
				//    for (int i = 0; i < columns.Count; ++i) {
				//        GridViewColumnHeader header = children[GetVisualIndex(i)] as GridViewColumnHeader;
				//        header.CheckWidthForPreviousHeaderGripper();
				//    }
				//    _paddingHeader.CheckWidthForPreviousHeaderGripper();
				//    _isColumnChangedOrCreated = false;
				//}
			}

			// Arrange padding header
			Debug.Assert(PaddingHeader != null, "padding header is null");
			rect = new Rect(accumulatedWidth, 0.0, Math.Max(remainingWidth, 0.0), arrangeSize.Height);
			PaddingHeader.Arrange(rect);
			HeadersPositionList.Add(rect);

			// if re-order started, arrange floating header & indicator
			if (IsHeaderDragging) {
				FloatingHeader.Arrange(new Rect(
					new Point(DragCurrentPosition.X - DragRelativeStartPosition.X, 0),
					HeadersPositionList[DragStartColumnIndex].Size));

				var pos = GetHeaderPositionByColumnIndex(DragDestColumnIndex);
				DragIndicator.Arrange(new Rect(pos,
					new Size(DragIndicator.DesiredSize.Width, arrangeSize.Height)));
			}

			return arrangeSize;
		}

		/// <summary>
		/// Map column collection index to header collection index in visual tree.
		/// </summary>
		/// <param name="columnIndex">The column collection index.</param>
		/// <returns>The header collection index.</returns>
		private int GetVisualIndex(int columnIndex) {
			return this.CallMethod<GridViewHeaderRowPresenter, int>("GetVisualIndex", columnIndex);
		}

		/// <summary>
		/// Returns the header position by logic column index.
		/// </summary>
		/// <param name="columnIndex">The column index.</param>
		/// <returns>The corresponding header position.</returns>
		private Point GetHeaderPositionByColumnIndex(int columnIndex) {
			return this.CallMethod<GridViewHeaderRowPresenter, Point>("FindPositionByIndex", columnIndex);
		}

		/// <summary>
		/// Returns the header from the column.
		/// </summary>
		/// <param name="column">The header column.</param>
		/// <returns>The header corresponding to the column.</returns>
		private GridViewColumnHeader GetHeaderByColumn(GridViewColumn column) {
			for (var i = 0; i < VisualChildrenCount; ++i) {
				if (GetVisualChild(i) is GridViewColumnHeader child && child.Column == column) {
					return child;
				}
			}

			return null;
		}

		/// <summary>
		/// Ensures ShareStateList have at least columns.Count items.
		/// </summary>
		private void EnsureDesiredWidthList() {
			this.CallMethod<GridViewHeaderRowPresenter>("EnsureDesiredWidthList");
		}

		#endregion // Methods.

	}

}