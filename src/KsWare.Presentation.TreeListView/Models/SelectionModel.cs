using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Models {

	/// <summary>
	/// Class handling the selected items of the <see cref="ExtendedListView"/>
	/// </summary>
	public class SelectionModel {

		#region Fields

		/// <summary>
		/// Stores the parent of the selection model.
		/// </summary>
		private readonly ExtendedListView _parent;

		/// <summary>
		/// Stores the list of the selected items.
		/// </summary>
		private readonly ObservableCollection<ITreeListViewItemVM> _selectedViewModels;

		/// <summary>
		/// Stores the flag indicating if the selection is allowed.
		/// </summary>
		private bool _canSelect;

		#endregion // Fields.

		#region Properties

		/// <summary>
		/// Gets or sets the anchor used to select a set of items.
		/// </summary>
		public ITreeListViewItemVM Anchor { get; private set; }

		/// <summary>
		/// Gets the selected items.
		/// </summary>
		public IEnumerable<ITreeListViewItemVM> SelectedViewModels => _selectedViewModels;

		/// <summary>
		/// Gets the selected objects.
		/// </summary>
		public IEnumerable<object> SelectedObjects => _selectedViewModels.Select(item => item.UntypedOwnedObject).ToArray();

		/// <summary>
		/// Gets or sets the flag indicating the item selection mode.
		/// </summary>
		public TreeSelectionMode SelectionMode {
			get {
				if (_canSelect == false) return TreeSelectionMode.NoSelection;
				return _parent.SelectionMode == System.Windows.Controls.SelectionMode.Extended
					? TreeSelectionMode.MultiSelection
					: TreeSelectionMode.SingleSelection;
			}
			set {
				switch (value) {
					case TreeSelectionMode.NoSelection:
						_canSelect = false;
						break;
					case TreeSelectionMode.SingleSelection:
						_canSelect = true;
						_parent.SelectionMode = System.Windows.Controls.SelectionMode.Single;
						break;
					case TreeSelectionMode.MultiSelection:
						_canSelect = true;
						_parent.SelectionMode = System.Windows.Controls.SelectionMode.Extended;
						break;
				}
			}
		}

		#endregion // Properties.

		#region Events

		/// <summary>
		/// Event raised when the selection is modified.
		/// </summary>
		public event SelectionChangedEventHandler SelectionChanged;

		#endregion // Events.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionModel"/> class.
		/// </summary>
		/// <param name="parent">The model parent.</param>
		public SelectionModel(ExtendedListView parent) {
			_parent = parent;
			_canSelect = true;
			_selectedViewModels = new ObservableCollection<ITreeListViewItemVM>();

			Anchor = null;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Select the given item.
		/// </summary>
		/// <param name="item">The selected item.</param>
		public void Select(ITreeListViewItemVM item) {
			if (SelectionMode != TreeSelectionMode.NoSelection) {
				InternalSelect(item, true);
			}
		}

		/// <summary>
		/// Adds the given to the selection.
		/// </summary>
		/// <param name="item">The item to add to the selection.</param>
		public void AddToSelection(ITreeListViewItemVM item) {
			if (SelectionMode != TreeSelectionMode.NoSelection) {
				InternalAddToSelection(item, true, true);
			}
		}

		/// <summary>
		/// Selects the given items.
		/// </summary>
		/// <param name="items">The selected items.</param>
		public void Select(IEnumerable<ITreeListViewItemVM> items) {
			if (SelectionMode != TreeSelectionMode.NoSelection) {
				InternalSelect(items, true, true);
			}
		}

		/// <summary>
		/// Selects a range of items.
		/// </summary>
		/// <param name="from">The range start view model.</param>
		/// <param name="to">The range stop view model.</param>
		public void SelectRange(ITreeListViewItemVM from, ITreeListViewItemVM to) {
			if (SelectionMode != TreeSelectionMode.NoSelection) {
				InternalSelectRange(from, to, true);
			}
		}

		/// <summary>
		/// Select all the items.
		/// </summary>
		public void SelectAll() {
			if (SelectionMode != TreeSelectionMode.NoSelection) {
				InternalSelectAll(true);
			}
		}

		/// <summary>
		/// Unselect the given item.
		/// </summary>
		/// <param name="item">The item to unselect.</param>
		/// <param name="unselectChildren">Unselect the children as well.</param>
		public void Unselect(ITreeListViewItemVM item, bool unselectChildren) {
			if (unselectChildren) {
				InternalUnselectAndChildren(item, true);
			}
			else {
				InternalUnselect(item, true);
			}
		}

		/// <summary>
		/// Unselect all the selected items.
		/// </summary>
		public void UnselectAll() {
			InternalUnselectAll(true, true);
		}

		/// <summary>
		/// Select the given item.
		/// </summary>
		/// <param name="item">The selected item.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalSelect(ITreeListViewItemVM item, bool notify) {
			if (item.CanBeSelected && (item.IsSelected == false || _selectedViewModels.Count > 1)) {
				// Update.
				var oldSelection = SelectedViewModels.ToArray();
				InternalUnselectAll(true, false);
				InternalAddToSelection(item, true, false);

				// Notification.
				if (notify) {
					NotifySelectionChanged(oldSelection, new ITreeListViewItemVM[] {item});
				}
			}
		}

		/// <summary>
		/// Adds the given to the selection.
		/// </summary>
		/// <param name="item">The item to add to the selection.</param>
		/// <param name="updatePivot">Flag to know if the pivot must be updated.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalAddToSelection(ITreeListViewItemVM item, bool updatePivot, bool notify) {
			if (item.CanBeSelected && item.IsSelected == false) {
				// Updating view model.
				item.IsSelected = true;

				// Updating the selected items list.
				_selectedViewModels.Add(item);

				// Setting the pivot.
				if (updatePivot) {
					Anchor = item;
				}

				// Updating native selection handling.
				if (SelectionMode == TreeSelectionMode.SingleSelection) {
					_parent.SelectedItem = item;
				}
				else {
					_parent.SelectedItems.Add(item);
				}

				// Notification.
				if (notify) {
					NotifySelectionChanged(new ITreeListViewItemVM[] { },
						new ITreeListViewItemVM[] {item});
				}
			}
		}

		/// <summary>
		/// Selects the given items.
		/// </summary>
		/// <param name="items">The selected items.</param>
		/// <param name="updatePivot">Flag to know if the pivot must be updated.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalSelect(IEnumerable<ITreeListViewItemVM> items, bool updatePivot, bool notify) {
			if (SelectionMode != TreeSelectionMode.SingleSelection || items.Count() == 1) {
				var selectableItems = items.Where(item => item.CanBeSelected).ToArray();
				var unselectedItems =
					selectableItems.Where(item => item.IsSelected == false).ToArray();
				if (unselectedItems.Any() || _parent.SelectedItems.Count != selectableItems.Count()) {
					// Update.
					var oldSelection = SelectedViewModels.ToArray();
					InternalUnselectAll(updatePivot, false);
					foreach (var item in items) {
						InternalAddToSelection(item, updatePivot, false);
					}

					// Notification.
					if (notify) {
						var commonItems = oldSelection.Intersect(items).ToArray();
						var itemsToRemove = oldSelection.Except(commonItems).ToArray();
						var itemsToAdd = items.Except(commonItems).ToArray();

						NotifySelectionChanged(itemsToRemove, itemsToAdd);
					}
				}
			}
		}

		/// <summary>
		/// Selects a range of items.
		/// </summary>
		/// <param name="from">The range start view model.</param>
		/// <param name="to">The range stop view model.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalSelectRange(ITreeListViewItemVM from, ITreeListViewItemVM to,
			bool notify) {
			if (SelectionMode != TreeSelectionMode.SingleSelection) {
				// Getting the indexes of the selection range.
				var fromIndex = _parent.Rows.IndexOf(from);
				var toIndex = _parent.Rows.IndexOf(to);
				if (fromIndex > toIndex) {
					// Swap values.
					var temp = fromIndex;
					fromIndex = toIndex;
					toIndex = temp;
				}

				// Building the list of items to select.
				var itemsToSelect = new List<ITreeListViewItemVM>();
				for (var index = fromIndex; index <= toIndex; index++) {
					var item = _parent.Rows.ElementAtOrDefault(index);
					if (item != null) {
						itemsToSelect.Add(item);
					}
				}

				if (itemsToSelect.Any()) {
					// Update.
					InternalSelect(itemsToSelect, false, notify);
				}
			}
		}

		/// <summary>
		/// Select all the items.
		/// </summary>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalSelectAll(bool notify) {
			if (SelectionMode != TreeSelectionMode.SingleSelection) {
				// Updating view model.
				var addedItems = new List<ITreeListViewItemVM>();
				foreach (var item in _parent.ViewModel.Children) {
					addedItems.AddRange(item.SelectAll());
				}

				if (addedItems.Any()) {
					// Updating the selected items list and native selection handling.
					foreach (var item in addedItems) {
						_parent.SelectedItems.Add(item);
						_selectedViewModels.Add(item);
					}

					// Notification.
					if (notify) {
						NotifySelectionChanged(new ITreeListViewItemVM[] { }, addedItems.ToArray());
					}
				}
			}
		}

		/// <summary>
		/// Unselect the given item.
		/// </summary>
		/// <param name="item">The item to unselect.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalUnselect(ITreeListViewItemVM item, bool notify) {
			if (item.IsSelected) {
				// Updating view model.
				item.IsSelected = false;

				// Updating the selected items list.
				_selectedViewModels.Remove(item);

				// Updating the pivot.
				if (Anchor == item) {
					Anchor = null;
				}

				// Updating native selection handling.
				if (SelectionMode == TreeSelectionMode.SingleSelection) {
					_parent.SelectedItem = null;
				}
				else {
					_parent.SelectedItems.Remove(item);
				}

				// Notification.
				if (notify) {
					NotifySelectionChanged(new ITreeListViewItemVM[] {item},
						new ITreeListViewItemVM[] { });
				}
			}
		}

		/// <summary>
		/// Unselect the given item and all its children.
		/// </summary>
		/// <param name="item">The item to unselect.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalUnselectAndChildren(ITreeListViewItemVM item, bool notify) {
			if (item.IsSelected) {
				var oldSelection = SelectedViewModels.ToArray();
				var removedItems = new List<ITreeListViewItemVM>();
				removedItems.AddRange(item.UnSelectAll());

				if (removedItems.Any()) {
					// Updating the selected items list.
					foreach (var i in removedItems) {
						_selectedViewModels.Remove(i);
					}

					// Updating the pivot.
					if (removedItems.Contains(Anchor)) {
						Anchor = null;
					}

					// Updating native selection handling.
					if (SelectionMode == TreeSelectionMode.SingleSelection) {
						_parent.SelectedItem = null;
					}
					else {
						foreach (var i in removedItems) {
							_parent.SelectedItems.Remove(i);
						}
					}

					// Notification.
					if (notify) {
						NotifySelectionChanged(removedItems.ToArray(), new ITreeListViewItemVM[] { });
					}
				}
			}
		}

		/// <summary>
		/// Unselect all the selected items.
		/// </summary>
		/// <param name="cleanPivot">Flag defining if the pivot must be cleaned.</param>
		/// <param name="notify">Flag defining if the notification must be done.</param>
		private void InternalUnselectAll(bool cleanPivot, bool notify) {
			if (SelectedViewModels.Any()) {
				// Updating view model.
				var oldSelection = SelectedViewModels.ToArray();
				foreach (var item in oldSelection) {
					item.UnSelectAll();
				}

				// Updating the selected items list.
				_selectedViewModels.Clear();

				// Updating the pivot.
				if (cleanPivot) {
					Anchor = null;
				}

				// Updating native selection handling.
				if (SelectionMode == TreeSelectionMode.SingleSelection) {
					_parent.SelectedItem = null;
				}
				else {
					_parent.SelectedItems.Clear();
				}

				// Notification.
				if (notify) {
					NotifySelectionChanged(oldSelection, new ITreeListViewItemVM[] { });
				}
			}
		}

		/// <summary>
		/// Notifies a selection modification.
		/// </summary>
		/// <param name="removedItems">The items removed from the selection.</param>
		/// <param name="addedItems">The items added to the selection.</param>
		private void NotifySelectionChanged(ITreeListViewItemVM[] removedItems,
			ITreeListViewItemVM[] addedItems) {
			if (SelectionChanged != null) {
				var args = new SelectionChangedEventArgs(removedItems, addedItems);
				SelectionChanged(this, args);
			}
		}

		#endregion // Methods.

	}

}