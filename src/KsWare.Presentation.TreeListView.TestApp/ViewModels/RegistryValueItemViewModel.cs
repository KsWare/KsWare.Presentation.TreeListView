using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.ViewModels {

	/// <summary>
	/// This class defines a view model for the registry value model.
	/// </summary>
	/// <!-- DPE -->
	internal class RegistryValueItemViewModel : TreeListViewItemVM<RegistryValue> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistryValueItemViewModel"/> class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		public RegistryValueItemViewModel(RegistryValue ownedObject)
			: base(ownedObject) {
			ToolTip = Name;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the person name.
		/// </summary>
		public override string DisplayString => OwnedObject.Name;

		/// <summary>
		/// Gets the registry element name.
		/// </summary>
		public string Name => OwnedObject.Name;

		/// <summary>
		/// Gets the registry element kind.
		/// </summary>
		public object Kind => OwnedObject.Kind;

		/// <summary>
		/// Gets the registry element data.
		/// </summary>
		public object Data => OwnedObject.Data;

		/// <summary>
		/// Gets the icon to display in the item.
		/// </summary>
		public override ImageSource IconSource {
			get {
				if (OwnedObject.Kind == Microsoft.Win32.RegistryValueKind.String) {
					return new BitmapImage(new Uri(@"/KsWare.Presentation.TreeListView.TestApp;component/Resources/DataString.png",
						UriKind.Relative));
				}
				else {
					return new BitmapImage(new Uri(@"/KsWare.Presentation.TreeListView.TestApp;component/Resources/Data.png",
						UriKind.Relative));
				}
			}
		}

		#endregion // Properties.

	}

}