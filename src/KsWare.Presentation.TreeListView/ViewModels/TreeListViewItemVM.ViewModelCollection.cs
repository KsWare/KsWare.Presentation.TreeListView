using System.Collections.ObjectModel;
using System;

namespace KsWare.Presentation.TreeListView.ViewModels;

public abstract partial class TreeListViewItemVM {

	/// <summary>
	/// This class defines a view model collection implemented to handle the item index in the 
	/// parent view model for performance purpose.
	/// </summary>
	private partial class ViewModelCollection : ObservableCollection<TreeListViewItemVM> {

		#region Fields

		/// <summary>
		/// The collection owner.
		/// </summary>
		private readonly TreeListViewItemVM _owner;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelCollection"/> class.
		/// </summary>
		/// <param name="owner">The owner of the collection.</param>
		public ViewModelCollection(TreeListViewItemVM owner) {
			_owner = owner;
		}

		#endregion // Constructors.


		#region Methods

		/// <summary>
		/// This method clears the items.
		/// </summary>
		protected override void ClearItems() {
			while (Count != 0) RemoveAt(Count - 1);
		}

		/// <summary>
		/// This method inserts an item.
		/// </summary>
		/// <param name="index">The insertion index.</param>
		/// <param name="item">The item to insert.</param>
		protected override void InsertItem(int index, TreeListViewItemVM item) {
			if (item == null) throw new ArgumentNullException(nameof(item));
			if (item.Parent == _owner) return;
			
			// Removing the element from the old parent if any.
			if (item.Parent != null) item.Parent._children.Remove(item);

			// Updating the parenting info.
			item.Parent = _owner;
			item._index = index;
			item.IsVisible = item.Parent.IsVisible;

			// Loading children if not load on demand.
			if (item.LoadItemsOnDemand == false) item.RegisterChildren();

			// Updating the index of the item placed after the added one.
			for (var i = index; index < Count; index++) this[i]._index++;

			// Inserting the item.
			base.InsertItem(index, item);
		}

		/// <summary>
		/// This method removes an item.
		/// </summary>
		/// <param name="itemIndex">The item index.</param>
		protected override void RemoveItem(int itemIndex) {
			// Getting the item.
			var item = this[itemIndex];

			// Invalidating the index.
			item._index = -1;

			// Invalidating the parent.
			item.Parent = null;

			// Updating the index of the item placed after the removed one.
			for (var idx = itemIndex + 1; idx < Count; idx++) {
				this[idx]._index--;
			}

			// Removing it.
			base.RemoveItem(itemIndex);
		}

		/// <summary>
		/// Method called when an item is moved.
		/// </summary>
		/// <param name="oldIndex">The old index.</param>
		/// <param name="newIndex">The new index.</param>
		protected override void MoveItem(int oldIndex, int newIndex) {
			// Moving the item.
			base.MoveItem(oldIndex, newIndex);

			// Updating all the items.
			for (var idx = 0; idx < Count; idx++) {
				this[idx]._index = idx;
			}
		}

		/// <summary>
		/// This method replace an item.
		/// </summary>
		/// <param name="itemIndex">The item of the index to replace.</param>
		/// <param name="item">The item.</param>
		protected override void SetItem(int itemIndex, TreeListViewItemVM item) {
			if (item == null) throw new ArgumentNullException(nameof(item));

			RemoveAt(itemIndex);
			InsertItem(itemIndex, item);
		}

		#endregion // Methods.

	}

}