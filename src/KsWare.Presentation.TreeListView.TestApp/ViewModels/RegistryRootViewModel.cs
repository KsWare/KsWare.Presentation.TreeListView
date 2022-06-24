using KsWare.Presentation.TreeListView.ViewModels;
using Microsoft.Win32;

namespace KsWare.Presentation.TreeListView.TestApp.ViewModels {

	/// <summary>
	/// This class defines the view model of the multi column tree view.
	/// </summary>
	/// <!-- DPE -->
	internal class RegistryRootViewModel : TreeListViewRootItemVM<object> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistryRootViewModel"/> class.
		/// </summary>
		public RegistryRootViewModel() {
			AddChild(new RegistryKeyItemViewModel(Registry.ClassesRoot));
			AddChild(new RegistryKeyItemViewModel(Registry.CurrentUser));
			AddChild(new RegistryKeyItemViewModel(Registry.LocalMachine));
			AddChild(new RegistryKeyItemViewModel(Registry.Users));
			AddChild(new RegistryKeyItemViewModel(Registry.CurrentConfig));
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the flag indicating if the items are loaded on demand.
		/// </summary>
		protected override bool LoadItemsOnDemand => true;

		#endregion // Properties.

	}

}