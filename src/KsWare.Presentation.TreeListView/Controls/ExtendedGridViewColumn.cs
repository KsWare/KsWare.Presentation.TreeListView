using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.TreeListView.Behaviors.Column;
using KsWare.Presentation.TreeListView.Internal.Extensions;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// Class extending the <see cref="GridViewColumn"/> control.
	/// </summary>
	public class ExtendedGridViewColumn : GridViewColumn {

		#region Fields

		/// <summary>
		/// Stores the column width.
		/// </summary>
		private GridLength _width;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedGridViewColumn"/> class.
		/// </summary>
		private ExtendedGridViewColumn() { }

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the column width.
		/// </summary>
		public GridLength GridLength {
			get => _width;
			set {
				_width = value;
				if (_width.GridUnitType == GridUnitType.Pixel) {
					RangeColumn.ApplyWidth(this, 0, _width.Value, double.MaxValue);
				}
				else if (_width.GridUnitType == GridUnitType.Star) {
					ProportionalColumn.ApplyWidth(this, _width.Value);
				}
			}
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Creates a GUI column from the given properties.
		/// </summary>
		/// <param name="column">The column properties.</param>
		/// <param name="index">The column index.</param>
		/// <returns>The created column.</returns>
		public static ExtendedGridViewColumn CreateFrom(TreeListViewColumn column, int index) {
			var c = new ExtendedGridViewColumn();
			c.Header = column.Header;
			c.GridLength = new GridLength(column.Width, column.Stretch ? GridUnitType.Star : GridUnitType.Pixel);
			
			if (column.TemplateSelector != null)
				c.CellTemplateSelector = column.TemplateSelector;
			else 
				c.CellTemplate = Resources.All.Instance.GetCellTemplate(column.DisplayMemberPath);

			return c;
		}

		/// <summary>
		/// Ensures final column desired width is no less than the given value.
		/// </summary>
		internal double EnsureDesiredWidth(double width) {
			return this.CallMethod<GridViewColumn, double>("EnsureWidth", width);
		}

		#endregion // Methods.

		#region Properties

		/// <summary>
		/// Gets or sets the rendering state of the column.
		/// </summary>
		internal ExtendedGridViewColumnMeasureState State {
			get => this.GetPropertyValue<GridViewColumn, ExtendedGridViewColumnMeasureState>(nameof(State));
			set => this.SetPropertyValue<GridViewColumn, int>(nameof(State), (int) value);
		}

		/// <summary>
		/// Gets or sets the index of the column in the grid.
		/// </summary>
		/// <remarks>
		/// Perf optimization. To avoid re-search index again and again by every GridViewRowPresenter, add an index here.
		/// </remarks>
		internal int ActualIndex => this.GetPropertyValue<GridViewColumn, int>(nameof(ActualIndex));

		/// <summary>
		/// Gets or sets the width of the column if the Width property is NaN.
		/// </summary>
		internal double DesiredWidth {
			get => this.GetPropertyValue<GridViewColumn, double>(nameof(DesiredWidth));
			set => this.SetPropertyValue<GridViewColumn, double>(nameof(DesiredWidth), value);
		}

		#endregion // Properties.

	}

}