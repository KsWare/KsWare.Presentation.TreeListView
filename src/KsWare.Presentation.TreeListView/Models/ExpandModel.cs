using System.Linq;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Models {

	/// <summary>
	/// Class handling the expand behavior of the <see cref="ExtendedListView"/>.
	/// </summary>
	public class ExpandModel {

		#region Fields

		/// <summary>
		/// Stores the parent of the selection model.
		/// </summary>
		private readonly ExtendedListView _parent;

		/// <summary>
		/// Stores the watchdog preventing for reentrancy.
		/// </summary>
		private bool _isProcessingExpand;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpandModel"/> class.
		/// </summary>
		/// <param name="parent">The model parent.</param>
		public ExpandModel(ExtendedListView parent) {
			_parent = parent;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// This method expands a node.
		/// </summary>
		/// <param name="item">The target node.</param>
		/// <param name="value">True to set the node as expanded.</param>
		public void SetIsExpanded(ITreeListViewItemVM item, bool value) {
			if (!BeginProcessingExpand()) return;
			if (item.HasChildren) {
				if (value) {
					item.IsExpanded = value;
					_parent.LoadsChildrenItems(item);
				}
				else {
					// When collapsed, if any child is selected, then all the item are unselected and the collapsed item is selected.
					if (_parent.SelectionModel.SelectedViewModels.Any(selectedItem =>
						    item.AllVisibleChildren.Contains(selectedItem))) {
						_parent.SelectionModel.Select(item);
					}

					_parent.DroChildrenItems(item, false);
					item.IsExpanded = value;
				}
			}

			EndProcessingExpand();
		}

		/// <summary>
		/// Toggles the expand state of the given item.
		/// </summary>
		/// <param name="item">The item to toggle expand.</param>
		public void ToggleExpand(ITreeListViewItemVM item) {
			if (!item.HasChildren) return;
			SetIsExpanded(item, item.IsExpanded == false);
		}

		/// <summary>
		/// Allows the expand process by checking the reentrancy. 
		/// </summary>
		/// <returns>True if the expand can be processed, false otherwise.</returns>
		private bool BeginProcessingExpand() {
			if (_isProcessingExpand) return false;
			_isProcessingExpand = true;
			return true;

		}

		/// <summary>
		/// Notifies the end of the expand processing.
		/// </summary>
		private void EndProcessingExpand() {
			_isProcessingExpand = false;
		}

		#endregion // Methods.

	}

}