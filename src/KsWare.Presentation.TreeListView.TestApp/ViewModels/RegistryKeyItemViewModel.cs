using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.ViewModels;
using Microsoft.Win32;

namespace KsWare.Presentation.TreeListView.TestApp.ViewModels {

	/// <summary>
	/// This class defines a view model for the registry key model.
	/// </summary>
	/// <!-- DPE -->
	internal class RegistryKeyItemViewModel : TreeListViewItemVM<RegistryKey> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistryKeyItemViewModel"/> class.
		/// </summary>
		public RegistryKeyItemViewModel(RegistryKey ownedObject)
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
		public string Kind => null;

		/// <summary>
		/// Gets the registry element data.
		/// </summary>
		public object Data => null;

		/// <summary>
		/// Gets the icon to display in the item.
		/// </summary>
		public override ImageSource IconSource =>
			new BitmapImage(new Uri(@"/KsWare.Presentation.TreeListView.TestApp;component/Resources/Folder.png",
				UriKind.Relative));

		/// <summary>
		/// Gets the flag indicating if the item has children.
		/// </summary>
		public override bool HasChildrenLoadedOnDemand => true;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Registers the children of this item on demand.
		/// </summary>
		protected override void InternalRegisterChildren() {
			if
				(Children.Count() == 0) {
				foreach
					(var name in OwnedObject.GetSubKeyNames()) {
					RegistryKey subKey = null;
					try {
						subKey = OwnedObject.OpenSubKey(name);
					}
					catch { }
					if
						(subKey != null) {
						AddChild(new RegistryKeyItemViewModel(subKey));
					}
				}

				foreach
					(var name in OwnedObject.GetValueNames()) {
					var regValue = new RegistryValue() {
						Name = name,
						Data = OwnedObject.GetValue(name),
						Kind = OwnedObject.GetValueKind(name)
					};

					AddChild(new RegistryValueItemViewModel(regValue));
				}
			}
		}

		#endregion // Methods.

	}

}