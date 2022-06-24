using System;
using System.Globalization;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// Class defining a converter used to hide the tooltip if it is not valid.
	/// </summary>
	public class ToolTiConverter : IValueConverter {

		#region Methods

		/// <summary>
		/// Convert from tooltip property to GUI tooltip.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object Convert(object value, Type targetType, object extraParameter, CultureInfo culture) {
			var stringToolTip = value as string;
			if (stringToolTip is string && string.IsNullOrEmpty(stringToolTip)) {
				// Do not display the tooltip if the string to display is empty.
				return null;
			}

			return value;
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