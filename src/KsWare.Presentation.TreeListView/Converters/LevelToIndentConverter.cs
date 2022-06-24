using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This class indent the given item by computing the left margin.
	/// </summary>
	[ValueConversion(typeof(ITreeListViewItemVM), typeof(Thickness))]
	internal class LevelToIndentConverter : IValueConverter {

		#region Constants

		/// <summary>
		/// Constant used to set an indentation size (in pixels).
		/// </summary>
		private const double IndentSize = 18.0;

		#endregion // Constants.

		#region Methods

		/// <summary>
		/// Convert from Level to Tickness.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object Convert(object value, Type targetType, object extraParameter, CultureInfo culture) {
			var level = 0;
			var itemToIndent = value as ITreeListViewItemVM;
			if
				(itemToIndent != null) {
				var currentItem = itemToIndent;
				while
					(currentItem.Parent != null) {
					currentItem = currentItem.Parent;
					level++;
				}
			}

			return new Thickness(level * IndentSize, 0, 0, 0);
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object ConvertBack(object value, Type targetType, object extraParameter, CultureInfo culture) {
			return Binding.DoNothing;
		}

		#endregion // Methods.

	}

}