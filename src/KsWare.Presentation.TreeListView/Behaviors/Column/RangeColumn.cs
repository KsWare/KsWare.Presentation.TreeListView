using System;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.TreeListView.Behaviors.Column {

	/// <summary>
	/// Class describing a column with a size limited by a width range.
	/// </summary>
	public sealed class RangeColumn : LayoutColumn {

		#region Dependencies

		/// <summary>
		/// Identifies the MinWidth attached property.
		/// </summary>
		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.RegisterAttached("MinWidth", typeof(double), typeof(RangeColumn));

		/// <summary>
		/// Identifies the MaxWidth attached property.
		/// </summary>
		public static readonly DependencyProperty MaxWidthProperty =
			DependencyProperty.RegisterAttached("MaxWidth", typeof(double), typeof(RangeColumn));

		/// <summary>
		/// Identifies the IsFillColumn attached property.
		/// </summary>
		public static readonly DependencyProperty IsFillColumnProperty =
			DependencyProperty.RegisterAttached("IsFillColumn", typeof(bool), typeof(RangeColumn));

		#endregion // Dependencies.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeColumn"/> class.
		/// </summary>
		private RangeColumn() { }

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Gets the minimum width defined in the attached property.
		/// </summary>
		/// <param name="object">The object from witch to get the value.</param>
		/// <returns>The attached value.</returns>
		public static double GetMinWidth(DependencyObject @object) {
			return (double) @object.GetValue(MinWidthProperty);
		}

		/// <summary>
		/// Sets the minimum width in the attached property.
		/// </summary>
		/// <param name="object">The object in witch the value is set.</param>
		/// <param name="minWidth">The width to set.</param>
		public static void SetMinWidth(DependencyObject @object, double minWidth) {
			@object.SetValue(MinWidthProperty, minWidth);
		}

		/// <summary>
		/// Gets the maximum width defined in the attached property.
		/// </summary>
		/// <param name="object">The object from witch to get the value.</param>
		/// <returns>The attached value.</returns>
		public static double GetMaxWidth(DependencyObject @object) {
			return (double) @object.GetValue(MaxWidthProperty);
		}

		/// <summary>
		/// Sets the maximum width in the attached property.
		/// </summary>
		/// <param name="object">The object in witch the value is set.</param>
		/// <param name="maxWidth">The width to set.</param>
		public static void SetMaxWidth(DependencyObject @object, double maxWidth) {
			@object.SetValue(MaxWidthProperty, maxWidth);
		}

		/// <summary>
		/// Gets the fill flag defined in the attached property.
		/// </summary>
		/// <param name="object">The object from witch to get the value.</param>
		/// <returns>The attached value.</returns>
		public static bool GetIsFillColumn(DependencyObject @object) {
			return (bool) @object.GetValue(IsFillColumnProperty);
		}

		/// <summary>
		/// Sets the fill flag in the attached property.
		/// </summary>
		/// <param name="object">The object in witch the value is set.</param>
		/// <param name="isFillColumn">The width to set.</param>
		public static void SetIsFillColumn(DependencyObject @object, bool isFillColumn) {
			@object.SetValue(IsFillColumnProperty, isFillColumn);
		}

		/// <summary>
		/// Returns a flag to know if the given column is a range column.
		/// </summary>
		/// <param name="column">The tested column.</param>
		/// <returns>True if the size has been defined in this column, false otherwise.</returns>
		public static bool IsRangeColumn(GridViewColumn column) {
			if (column == null) {
				return false;
			}

			return (HasPropertyValue(column, MinWidthProperty) || HasPropertyValue(column, MaxWidthProperty) ||
			        HasPropertyValue(column, IsFillColumnProperty));
		}

		/// <summary>
		/// Returns the minimum width for a given column if any.
		/// </summary>
		/// <param name="column">The column to test.</param>
		/// <returns>The minimum width if defined, false otherwise.</returns>
		public static double? GetRangeMinWidth(GridViewColumn column) {
			return GetColumnWidth(column, MinWidthProperty);
		}

		/// <summary>
		/// Returns the maximum width for a given column if any.
		/// </summary>
		/// <param name="column">The column to test.</param>
		/// <returns>The maximum width if defined, false otherwise.</returns>
		public static double? GetRangeMaxWidth(GridViewColumn column) {
			return GetColumnWidth(column, MaxWidthProperty);
		}

		/// <summary>
		/// Returns the fill flag for a given column if any.
		/// </summary>
		/// <param name="column">The column to test.</param>
		/// <returns>The fill flag if defined, false otherwise.</returns>
		public static bool? GetRangeIsFillColumn(GridViewColumn column) {
			if (column == null) {
				throw new ArgumentNullException(nameof(column));
			}

			var value = column.ReadLocalValue(IsFillColumnProperty);
			if (value != null && value.GetType() == IsFillColumnProperty.PropertyType) {
				return (bool) value;
			}

			return null;
		}

		/// <summary>
		/// Applies the range width property on the given column.
		/// </summary>
		/// <param name="column">The column in witch to apply the properties.</param>
		/// <param name="minWidth">The minimum width.</param>
		/// <param name="width">The default width.</param>
		/// <param name="maxWidth">The maximum width.</param>
		/// <returns>The configured column.</returns>
		public static GridViewColumn ApplyWidth(GridViewColumn column, double minWidth, double width,
			double maxWidth) {
			return ApplyWidth(column, minWidth, width, maxWidth, false);
		}

		/// <summary>
		/// Applies the range width property on the given column.
		/// </summary>
		/// <param name="column">The column in witch to apply the properties.</param>
		/// <param name="minWidth">The minimum width.</param>
		/// <param name="width">The default width.</param>
		/// <param name="maxWidth">The maximum width.</param>
		/// <param name="isFillColumn">Flag defining if the column fill the remaining space.</param>
		/// <returns>The configured column.</returns>
		public static GridViewColumn ApplyWidth(GridViewColumn column, double minWidth, double width,
			double maxWidth, bool isFillColumn) {
			SetMinWidth(column, minWidth);
			column.Width = width;
			SetMaxWidth(column, maxWidth);
			SetIsFillColumn(column, isFillColumn);
			return column;
		}

		#endregion // Methods.

	}

}