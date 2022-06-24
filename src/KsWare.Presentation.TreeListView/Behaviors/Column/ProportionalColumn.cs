using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.TreeListView.Behaviors.Column {

	/// <summary>
	/// Class describing a column with a proportional size.
	/// </summary>
	public sealed class ProportionalColumn : LayoutColumn {

		#region Dependencies

		/// <summary>
		/// Identifies the Width attached property.
		/// </summary>
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.RegisterAttached("Width", typeof(double), typeof(ProportionalColumn));

		#endregion // Dependencies.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ProportionalColumn"/> class.
		/// </summary>
		private ProportionalColumn() { }

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Gets the width defined in the attached property.
		/// </summary>
		/// <param name="object">The object from witch to get the value.</param>
		/// <returns>The attached width value.</returns>
		public static double GetWidth(DependencyObject @object) {
			return (double) @object.GetValue(WidthProperty);
		}

		/// <summary>
		/// Sets the width in the attached property.
		/// </summary>
		/// <param name="object">The object in witch the value is set.</param>
		/// <param name="width">The width to set.</param>
		public static void SetWidth(DependencyObject @object, double width) {
			@object.SetValue(WidthProperty, width);
		}

		/// <summary>
		/// Returns a flag to know if the given column is a proportional column.
		/// </summary>
		/// <param name="column">The tested column.</param>
		/// <returns>True if the proportional size has been defined in this column, false otherwise.</returns>
		public static bool IsProportionalColumn(GridViewColumn column) {
			if (column == null) {
				return false;
			}

			return HasPropertyValue(column, WidthProperty);
		}

		/// <summary>
		/// Returns the propertional width for a given column if any.
		/// </summary>
		/// <param name="column">The column to test.</param>
		/// <returns>The proportional width if defined, false otherwise.</returns>
		public static double? GetProportionalWidth(GridViewColumn column) {
			return GetColumnWidth(column, WidthProperty);
		}

		/// <summary>
		/// Applies the proportional width property on the given column.
		/// </summary>
		/// <param name="column">The column in witch to apply the properties.</param>
		/// <param name="width">The proportional width.</param>
		/// <returns>The configured column.</returns>
		public static GridViewColumn ApplyWidth(GridViewColumn column, double width) {
			SetWidth(column, width);
			return column;
		}

		#endregion // Methods.

	}

}