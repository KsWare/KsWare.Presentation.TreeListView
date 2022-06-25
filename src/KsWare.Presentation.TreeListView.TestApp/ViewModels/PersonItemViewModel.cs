using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.ViewModels {

	/// <summary>
	/// This class defines a view model for the Person model.
	/// </summary>
	/// <!-- DPE -->
	internal class PersonItemViewModel : TreeListViewItemVM<Person> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PersonItemViewModel"/> class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		public PersonItemViewModel(Person ownedObject)
			: base(ownedObject) {
			ToolTip = "Custom tooltip test";
			BindChildren("Children", typeof(PersonItemViewModel));
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the person name.
		/// </summary>
		public override string DisplayString => OwnedObject.Name; // used for tree item w/o columns

		/// <summary>
		/// Gets the person id.
		/// </summary>
		public string Id => OwnedObject.Id.ToString();

		/// <summary>
		/// Gets the person name.
		/// </summary>
		[Column]
		public string Name => OwnedObject.Name;

		/// <summary>
		/// Gets the person address.
		/// </summary>
		[Column]
		public string Address => OwnedObject.Address;

		/// <summary>
		/// Gets the flag indicating if the item has children.
		/// </summary>
		public override bool HasChildrenLoadedOnDemand => OwnedObject.Children.Any();

		/// <summary>
		/// Gets the icon to display in the item.
		/// </summary>
		public override ImageSource IconSource =>
			new BitmapImage(new Uri(@"/KsWare.Presentation.TreeListView.TestApp;component/Resources/Person.png",
				UriKind.Relative));

		/// <summary>
		/// Gets the flag indicating if the item is checkable.
		/// </summary>
		public override bool IsCheckable => true;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Returns the current object as string.
		/// </summary>
		/// <returns>The string description of the object.</returns>
		public override string ToString() {
			return OwnedObject.ToString();
		}

		#endregion // Methods.

	}

}