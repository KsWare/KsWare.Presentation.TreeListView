using System.Collections.Generic;
using System.Linq;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Models {

	/// <summary>
	/// Class handling the checked items in the ExtendedListView.
	/// </summary>
	public class CheckModel {

		#region Fields

		/// <summary>
		/// Stores the list of the selected items.
		/// </summary>
		private readonly List<ITreeListViewItemVM> _checkedItemsViewModel;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckModel"/> class.
		/// </summary>
		public CheckModel() {
			_checkedItemsViewModel = new List<ITreeListViewItemVM>();
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event raised when an item gets toggled.
		/// </summary>
		public event TreeViewEventHandler ItemsViewModelToggled;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Gets the checked items.
		/// </summary>
		public IEnumerable<ITreeListViewItemVM> CheckedItemsViewModel => _checkedItemsViewModel;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Check the item.
		/// </summary>
		/// <param name="item">The item to check.</param>
		/// <param name="checkChildren">Flag defining if the children have to be checked as well.</param>
		public void Check(ITreeListViewItemVM item, bool checkChildren) {
			if ((item.IsChecked || !item.IsCheckable || !item.IsCheckingEnabled) && !checkChildren) return;
			// Update.
			ITreeListViewItemVM[] checkedItems;
			if (checkChildren) {
				checkedItems = item.CheckAll();
			}
			else {
				item.IsChecked = true;
				checkedItems = new ITreeListViewItemVM[] {item};
			}

			if (checkedItems.Any()) {
				_checkedItemsViewModel.AddRange(checkedItems);

				// Notification.
				NotifyItemsToggled(checkedItems);
			}
		}

		/// <summary>
		/// Uncheck the item.
		/// </summary>
		/// <param name="item">The item to uncheck.</param>
		/// <param name="uncheckChildren">Flag defining if the children have to be unchecked as well.</param>
		public void Uncheck(ITreeListViewItemVM item, bool uncheckChildren) {
			if ((!item.IsChecked || !item.IsCheckable || !item.IsCheckingEnabled) && !uncheckChildren) return;
			// Update.
			ITreeListViewItemVM[] uncheckedItems;
			if (uncheckChildren) {
				uncheckedItems = item.UncheckAll();
			}
			else {
				item.IsChecked = false;
				uncheckedItems = new ITreeListViewItemVM[] {item};
			}

			if (uncheckedItems.Any()) {
				foreach (var i in uncheckedItems) {
					_checkedItemsViewModel.Remove(i);
				}

				// Notification.
				NotifyItemsToggled(uncheckedItems);
			}
		}

		/// <summary>
		/// Notifies a check modification.
		/// </summary>
		/// <param name="toggledItem">The toogled item.</param>
		private void NotifyItemToggled(ITreeListViewItemVM toggledItem) {
			if (ItemsViewModelToggled != null) {
				ItemsViewModelToggled(this, new ITreeListViewItemVM[] {toggledItem});
			}
		}

		/// <summary>
		/// Notifies a check modification.
		/// </summary>
		/// <param name="toggledItem">The toogled item.</param>
		private void NotifyItemsToggled(ITreeListViewItemVM[] toggledItem) {
			if (ItemsViewModelToggled != null) {
				ItemsViewModelToggled(this, toggledItem);
			}
		}

		#endregion // Methods.

	}

}