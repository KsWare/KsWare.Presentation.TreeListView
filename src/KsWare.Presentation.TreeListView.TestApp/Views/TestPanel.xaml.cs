using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.Internal.Extensions;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.Views {

	/// <summary>
	/// Class defining the panel used to tes the tree list view.
	/// </summary>
	public partial class TestPanel : UserControl {

		#region Fields

		/// <summary>
		/// Gets or sets the tree list view to test.
		/// </summary>
		private Controls.TreeListView _treeListViewToTest;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TestPanel"/> class.
		/// </summary>
		public TestPanel() {
			InitializeComponent();

			AddColumnButton.Click += OnAddColumnButtonClick;
			RemoveColumnButton.Click += OnRemoveColumnButtonClick;
			UnselectItemsButton.Click += OnUnselectItemsButtonClick;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the tree list view to test.
		/// </summary>
		public Controls.TreeListView TreeListViewToTest {
			get => _treeListViewToTest;

			set {
				_treeListViewToTest = value;
				_treeListViewToTest.Loaded += OnTreeListViewToTestLoaded;
			}
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Method called when the tree to test is loaded.
		/// </summary>
		/// <param name="sender">The tree sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnTreeListViewToTestLoaded(object sender, RoutedEventArgs eventArgs) {
			UpdateAvailableColumns();
			ColumnsListBox.ItemsSource = TreeListViewToTest.Columns;
			SelectedItemsListBox.ItemsSource = TreeListViewToTest.SelectedViewModels;
		}

		private void UpdateAvailableColumns() {
			//TODO continue here
			if (TreeListViewToTest.ItemsSource != null) {
				var type = TreeListViewToTest.ItemsSource.GetType();
			} else if (TreeListViewToTest.Items != null) {
				var type = TreeListViewToTest.Items.GetType();
			}

			if (TreeListViewToTest.Name == "PersonTreeListView") {

			}else if (TreeListViewToTest.Name == "MultiColumnTreeListView") {

			}
		}

		/// <summary>
		/// Method called when the add column button is clicked.
		/// </summary>
		/// <param name="sender">The button sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnAddColumnButtonClick(object sender, RoutedEventArgs eventArgs) {
			if (string.IsNullOrEmpty(NewColumnPropertyName.Text)) return;
			TreeListViewToTest.Columns.Add(new TreeListViewColumn {
				Header = NewColumnPropertyName.Text,
				DisplayMemberPath = NewColumnPropertyName.Text
			});
		}

		/// <summary>
		/// Method called when the remove column button is clicked.
		/// </summary>
		/// <param name="sender">The button sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnRemoveColumnButtonClick(object sender, RoutedEventArgs eventArgs) {
			if (ColumnsListBox.SelectedValue is not TreeListViewColumn column) return;
			TreeListViewToTest.Columns.Remove(column);
		}

		/// <summary>
		/// Method called when the unselect items button is clicked.
		/// </summary>
		/// <param name="sender">The button sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnUnselectItemsButtonClick(object sender, RoutedEventArgs eventArgs) {
			foreach (var item in SelectedItemsListBox.SelectedItems
				         .OfType<ITreeListViewItemVM>().ToList()) {
				TreeListViewToTest.Unselect(item);
			}
		}

		#endregion // Methods.

	}

}