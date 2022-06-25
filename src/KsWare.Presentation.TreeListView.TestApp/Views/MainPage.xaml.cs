using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.TestApp.ViewModels;
using System.Windows.Controls;

namespace KsWare.Presentation.TreeListView.TestApp.Views {

	/// <summary>
	/// This class defines the panel of this plugin.
	/// </summary>
	/// <!-- DPE -->
	public partial class MainPage : UserControl {

		/// <summary>
		/// Initializes a new instance of the Panel class.
		/// </summary>
		public MainPage() {
			InitializeComponent();
			DataContext = new MainPageVM();


			// Initializing the mono column tree.
			var rootViewModel = new PersonRootViewModel();
			rootViewModel.SetIsLoadOnDemand(true);
			rootViewModel.Model = Person.CreateFullTestModel();
			PersonTreeListView.ViewModel = rootViewModel;

			// Initializing the multi column tree list view.
			MultiColumnTreeListView.ViewModel = new RegistryRootViewModel();
			MultiColumnTreeListView.SelectionMode = TreeSelectionMode.MultiSelection;

			PersonTestPanel.TreeListViewToTest = PersonTreeListView;
			RegisterTestPanel.TreeListViewToTest = MultiColumnTreeListView;
		}

	}

}