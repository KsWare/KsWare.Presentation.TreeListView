using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.ViewModels {

	/// <summary>
	/// This class defines the view model of the tree view without any column.
	/// </summary>
	/// <!-- DPE -->
	internal class PersonRootViewModel : TreeListViewRootItemVM<Person> {

		#region Fields

		/// <summary>
		/// Stores the flag to know if the view model is loaded on demand.
		/// </summary>
		private bool _loadOnDemand;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PersonRootViewModel"/> class.
		/// </summary>
		public PersonRootViewModel() { }

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the model associated to this view model.
		/// </summary>
		public override Person Model {
			get => base.Model;

			set {
				base.Model = value;
				BindChildren("Children", typeof(PersonItemViewModel));
			}
		}

		/// <summary>
		/// Gets the flag indicating if the items are loaded on demand.
		/// </summary>
		protected override bool LoadItemsOnDemand => _loadOnDemand;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Sets the load on demand flag.
		/// </summary>
		/// <param name="flag">The flag to know if the view model is loaded on demand.</param>
		public void SetIsLoadOnDemand(bool flag) {
			_loadOnDemand = flag;
		}

		#endregion // Methods.

	}

}