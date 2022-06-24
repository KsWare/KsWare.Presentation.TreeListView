using System;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This converter allows to convert from integer to boolean type.
	/// </summary>
	[ValueConversion(typeof(int), typeof(bool))]
	public class IntegerToBoolConverter : IValueConverter {

		#region Properties

		/// <summary>
		/// Gets or sets the boolean equals to 0.
		/// </summary>
		public bool ZeroValue { get; set; }

		#endregion // Properties.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegerToBoolConverter"/> class.
		/// </summary>
		public IntegerToBoolConverter() {
			ZeroValue = false;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Convert from boolean to integer.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object ConvertBack(object value, Type targetType, object extraParameter,
			System.Globalization.CultureInfo culture) {
			return Binding.DoNothing;
		}

		/// <summary>
		/// Convert from integer to boolean.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object Convert(object value, Type targetType, object extraParameter,
			System.Globalization.CultureInfo culture) {
			// Checks if the value is valid.
			if ((value == null) || value is int == false) return false;

			var i = (int) value;
			if (i == 0) return ZeroValue;

			return ZeroValue == false;
		}

		#endregion // Methods.

	}

}