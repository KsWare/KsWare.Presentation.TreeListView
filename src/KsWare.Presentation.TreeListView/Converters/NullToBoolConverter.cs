using System;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This Converter allows to convert from Object nullness to Boolean type.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(object))]
	public class NullToBoolConverter : IValueConverter {

		#region Properties

		/// <summary>
		/// Gets or set the visibility corresponding to "True".
		/// </summary>
		public bool NullBoolean { get; set; }

		/// <summary>
		/// Gets or set the visibility corresponding to "False".
		/// </summary>
		public bool NotNullBoolean { get; set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Initialiazes a new instance of the <see cref="NullToBoolConverter"/> class.
		/// </summary>
		public NullToBoolConverter() {
			NotNullBoolean = false;
			NullBoolean = true;
		}

		/// <summary>
		/// Convert from Visibility to Bool.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object ConvertBack(object value, Type targetType, object extraParameter,
			System.Globalization.CultureInfo culture) {
			return null;
		}

		/// <summary>
		/// Convert from Bool to Visibility.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object Convert(object value, Type targetType, object extraParameter,
			System.Globalization.CultureInfo culture) {
			if (value == null) {
				return NullBoolean;
			}

			return NotNullBoolean;
		}

		#endregion // Methods.

	}

}