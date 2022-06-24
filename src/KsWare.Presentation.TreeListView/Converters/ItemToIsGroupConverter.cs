using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Converters {

	/// <summary>
	/// This class defines a converter which defines if the tree item is a group or not.
	/// </summary>
	internal class ItemToIsGroupConverter : IMultiValueConverter {

		#region Methods

		/// <summary>
		/// Converts the tree configuration to the item group state.
		/// </summary>
		/// <param name="values">The values defining the tree configuration.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="parameter">The additional parameters.</param>
		/// <param name="culture">The culture to use during the conversion.</param>
		/// <returns>The returned item group state.</returns>
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			Debug.Assert(values.Count() == 2);

			try {
#pragma warning disable 1587
				/// To be a group, an item must have the root as its parent view model, and the FirstLevelItemAsGroup property to true.
				/// First parameter is the view model, second is the FirstLevelItemAsGroup property.
#pragma warning restore 1587
				var viewModel = values[0] as ITreeListViewItemVM;
				var firstLevelItemAsGroup = System.Convert.ToBoolean(values[1]);
				if
					((viewModel != null)
					 && (viewModel.Parent is ITreeListViewRootItemVM)
					 && (firstLevelItemAsGroup)
					) {
					return true;
				}

				return false;
			}
			catch {
				return Binding.DoNothing;
			}
		}

		/// <summary>
		/// Converts the item group state to the tree configuration.
		/// </summary>
		/// <param name="value">The item group state.</param>
		/// <param name="targetType">The target type.</param>
		/// <param name="parameter">The additional parameters.</param>
		/// <param name="culture">The culture to use during the conversion.</param>
		/// <returns>Returns nothing.</returns>
		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) {
			return new object[] {Binding.DoNothing};
		}

		#endregion // Methods.

	}

}