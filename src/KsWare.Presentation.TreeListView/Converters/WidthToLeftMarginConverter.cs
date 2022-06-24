using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This class defines a converter used to convert a width into a margin.
	/// </summary>
	public class WidthToLeftMarginConverter : IValueConverter {

		#region Constructors

		/// <summary>
		/// Insitializes a new instance of the <see cref="WidthToLeftMarginConverter"/> class.
		/// </summary>
		public WidthToLeftMarginConverter() {
			InvertMargin = false;
			Margin = 0.0;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the flag indicating if the identation must be inverted.
		/// </summary>
		public bool InvertMargin { get; set; }

		/// <summary>
		/// Gets or sets the margin to remove to the width.
		/// </summary>
		public double Margin { get; set; }

		#endregion // Properties.

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
			var leftThickness = System.Convert.ToDouble(value);
			leftThickness -= Margin;

			if (InvertMargin) {
				leftThickness = -leftThickness;
			}

			return new Thickness(leftThickness, 0, 0, 0);
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