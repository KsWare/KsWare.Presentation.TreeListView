using System.Windows;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.TestApp.Model;
using KsWare.Presentation.TreeListView.TestApp.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.Views {

	/// <summary>
	/// Class defining the main window.
	/// </summary>
	public partial class MainWindow : Window {

		#region Constructors

		/// <summary>
		/// Initializes an instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow() {
			InitializeComponent();

			// Initializing the mono column tree.
			var rootViewModel = new PersonRootViewModel();
			rootViewModel.SetIsLoadOnDemand(true);
			rootViewModel.Model = Person.CreateFullTestModel();
			MainPage.PersonTreeListView.ViewModel = rootViewModel;

			// Initializing the multi column tree list view.
			MainPage.MultiColumnTreeListView.ViewModel = new RegistryRootViewModel();
			MainPage.MultiColumnTreeListView.SelectionMode = TreeSelectionMode.MultiSelection;
		}

		#endregion // Constructors.

	}

}