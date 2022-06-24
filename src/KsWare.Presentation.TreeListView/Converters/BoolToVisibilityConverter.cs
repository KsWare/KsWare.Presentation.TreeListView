using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// Converter used to convert a boolean to visibility.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	internal class BoolToVisibilityConverter : IValueConverter {

		#region Properties

		/// <summary>
		/// Gets or sets the visibility value if the boolean is true.
		/// </summary>
		public Visibility TrueValue { get; set; }

		/// <summary>
		/// Gets or sets the visibility value if the boolean is false.
		/// </summary>
		public Visibility FalseValue { get; set; }

		#endregion // Properties.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
		/// </summary>
		public BoolToVisibilityConverter() {
			TrueValue = Visibility.Visible;
			FalseValue = Visibility.Hidden;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Convert from boolean to Visibility.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="extraParameter">The extra parameter to use (not used by the Converter).</param>
		/// <param name="culture">The culture to use (not used by the Converter).</param>
		/// <returns>The value converted.</returns>
		public object Convert(object value, Type targetType, object extraParameter, CultureInfo culture) {
			var b = System.Convert.ToBoolean(value);
			return b ? TrueValue : FalseValue;
		}

		/// <summary>
		/// Convert from Visibility to Boolean (Not supported).
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