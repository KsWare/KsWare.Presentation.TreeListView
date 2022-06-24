using System;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.TreeListView.Behaviors.Column {

	/// <summary>
	/// Class defining the base class for the column descriptors used by the layout manager.
	/// </summary>
	public abstract class LayoutColumn {

		#region Methods

		/// <summary>
		/// Verifies if the given dependency property is attached to the given grid view column and if the property value is defined.
		/// </summary>
		/// <param name="column">The tested column.</param>
		/// <param name="property">The dependency property.</param>
		/// <returns>True if the dependency property is attached to the given grid view column and if the property value is defined, false otherwise.</returns>
		protected static bool HasPropertyValue(GridViewColumn column, DependencyProperty property) {
			if (column == null) {
				throw new ArgumentNullException(nameof(column));
			}

			var value = column.ReadLocalValue(property);
			if (value != null && value.GetType() == property.PropertyType) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the column width if the dependency property is attached to the column.
		/// </summary>
		/// <param name="column">The tested column.</param>
		/// <param name="property">The width dependency property.</param>
		/// <returns>The width value if the width has been defined using the given dependency property.</returns>
		protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty property) {
			if (HasPropertyValue(column, property)) {
				return Convert.ToDouble(column.ReadLocalValue(property));
			}

			return null;
		}

		#endregion // Methods.

	}

}