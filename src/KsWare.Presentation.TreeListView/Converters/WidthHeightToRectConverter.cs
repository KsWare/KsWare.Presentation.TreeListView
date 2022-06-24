using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This class convert a width and height into a rectangle.
	/// </summary>
	public class WidthHeightToRectConverter : IMultiValueConverter {

		#region Constructors

		/// <summary>
		/// Insitializes a new instance of the <see cref="WidthHeightToRectConverter"/> class.
		/// </summary>
		public WidthHeightToRectConverter() {
			Margin = new Thickness();
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the margin to remove to the rectangle.
		/// </summary>
		public Thickness Margin { get; set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Convert from width and height to Rect.
		/// </summary>
		/// <param name="values">The width and the height.</param>
		/// <param name="targetType">The target type (rect).</param>
		/// <param name="parameter">The converters additional parameters.</param>
		/// <param name="culture">The converter culture to use.</param>
		/// <returns>The converted rect.</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var width = (double) values[0];
			var height = (double) values[1];

			// Building the rectangle.
			var rect = new Rect();
			rect.Width = width;
			rect.Height = height;

			// Applying the margin.
			rect.X += Margin.Left;
			rect.Width = Math.Max(0.0, rect.Width - Margin.Left);
			rect.Y += Margin.Top;
			rect.Height = Math.Max(0.0, rect.Height - Margin.Top);
			rect.Width = Math.Max(0.0, rect.Width - Margin.Right);
			rect.Height = Math.Max(0.0, rect.Height - Margin.Bottom);

			// To be sure the rendering wont be affected.
			rect.X -= 1;
			rect.Y -= 1;
			rect.Height += 1;
			rect.Width += 1;

			return rect;
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		/// <param name="values">The value to convert.</param>
		/// <param name="targetTypes">The target types.</param>
		/// <param name="parameter">The converters additional parameters.</param>
		/// <param name="culture">The converter culture to use.</param>
		/// <returns>Binding.DoNothing</returns>
		public object[] ConvertBack(object values, Type[] targetTypes, object parameter, CultureInfo culture) {
			return new object[] {Binding.DoNothing};
		}

		#endregion // Methods.

	}

}