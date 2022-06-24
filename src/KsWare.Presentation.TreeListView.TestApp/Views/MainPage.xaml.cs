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

			PersonTestPanel.TreeListViewToTest = PersonTreeListView;
			RegisterTestPanel.TreeListViewToTest = MultiColumnTreeListView;
		}



	}

}