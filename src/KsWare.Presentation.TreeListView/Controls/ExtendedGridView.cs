using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KsWare.Presentation.TreeListView.Converters;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// Class extending the <see cref="GridView"/> control.
	/// </summary>
	public class ExtendedGridView : GridView {

		#region Fields

		/// <summary>
		/// Stores the flag indicating if the column header is visible.
		/// </summary>
		private bool _showColumnHeaders;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedGridView"/> class.
		/// </summary>
		public ExtendedGridView() {
			ShowColumnHeaders = true;
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event raised when the view is synchronized with the tree list view columns collection.
		/// </summary>
		public event Action Synchronized;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Gets or sets the flag indicating if the column headers are visible or not.
		/// </summary>
		public bool ShowColumnHeaders {
			get => _showColumnHeaders;
			set {
				_showColumnHeaders = value;

				if (value == true) {
					ColumnHeaderContainerStyle = null;
				}
				else {
					ColumnHeaderContainerStyle =
						Resources.All.Instance["CollapsedGridViewColumnHeaderStyle"] as Style;
				}
			}
		}

		/// <summary>
		/// Gets the list view default style key.
		/// </summary>
		protected override object DefaultStyleKey => ExtendedListView.MultiColumnDefaultStyleKey;

		/// <summary>
		/// Gets the item container default style key.
		/// </summary>
		protected override object ItemContainerDefaultStyleKey => TreeListViewItem.MultiColumnDefaultStyleKey;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Updates the grid view columns with the given collection.
		/// </summary>
		/// <param name="collection">The collection to synchronize.</param>
		internal void SynchronizeColumns(TreeListViewColumnCollection collection) {
			// Clearing columns.
			Columns.Clear();

			for (var iter = 0; iter < collection.Count; iter++) {
				// Creating a column for each backuped column.
				var column = ExtendedGridViewColumn.CreateFrom(collection[iter], iter);

				// Indenting the data template used in the first column.
				if (iter == 0) {
					// Extra margin between the end of the decorators part and the beginning of the item data template to remove.
					double extraMargin = 4;

					// Creating the indentation of the data template.
					var templateMarginBinding = new Binding("DecoratorWidth");
					templateMarginBinding.Converter = new WidthToLeftMarginConverter() {Margin = extraMargin};

					// Creating the new cell data template.
					var cellDataTemplate = new DataTemplate();

					// Defining the visual tree.
					var cellFactory = new FrameworkElementFactory(typeof(ContentControl));
					cellFactory.SetBinding(FrameworkElement.MarginProperty, templateMarginBinding);
					cellFactory.SetBinding(ContentControl.ContentProperty,
						new Binding() {BindsDirectlyToSource = true});

					// Trying to get the data template already defined in the first column.
					if (column.CellTemplate != null) {
						cellFactory.SetValue(ContentControl.ContentTemplateProperty, column.CellTemplate);

						// Removing the template from the cell as it is now applied in the ContentControl.
						column.CellTemplate = null;
					}

					// Trying to get the data template selector if any.
					if (column.CellTemplateSelector != null) {
						cellFactory.SetValue(ContentControl.ContentTemplateSelectorProperty,
							column.CellTemplateSelector);

						// Removing the template selector from the cell as it is now applied in the ContentControl.
						column.CellTemplateSelector = null;
					}

					// Trying to get the display path.
					if (column.DisplayMemberBinding != null) {
						cellFactory.SetValue(ContentControl.ContentProperty, column.DisplayMemberBinding);

						// Removing the template selector from the cell as it is now applied in the ContentControl.
						column.DisplayMemberBinding = null;
					}

					// Updating the cell template.
					cellDataTemplate.VisualTree = cellFactory;
					column.CellTemplate = cellDataTemplate;
				}

				// Adding the column.
				Columns.Add(column);

				// Notification.
				if (Synchronized != null) {
					Synchronized();
				}
			}
		}

		#endregion // Methods.

	}

}