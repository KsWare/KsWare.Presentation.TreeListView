using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// Class defining a set of <see cref="TreeListViewColumn"/>.
	/// </summary>
	public sealed class TreeListViewColumnCollection : ObservableCollection<TreeListViewColumn> {

		#region Fields

		/// <summary>
		/// Stores the collection owner.
		/// </summary>
		private readonly TreeListView _owner;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListView"/> class.
		/// </summary>
		/// <param name="owner">The collection owner.</param>
		internal TreeListViewColumnCollection(TreeListView owner) {
			_owner = owner;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		private ExtendedGridView View {
			get => _owner.InnerListView?.View;

			set {
				if (_owner.InnerListView == null) return;

				if (_owner.InnerListView.View != null)
					_owner.InnerListView.View.Columns.CollectionChanged -= OnGridViewColumnsCollectionChanged;

				_owner.InnerListView.View = value;

				if (_owner.InnerListView.View != null)
					_owner.InnerListView.View.Columns.CollectionChanged += OnGridViewColumnsCollectionChanged;
			}
		}

		#endregion // Properties.


		#region Methods

		/// <summary>
		/// Inserts an item at the given index.
		/// </summary>
		/// <param name="index">The index of the item.</param>
		/// <param name="item">The item to insert.</param>
		protected override void InsertItem(int index, TreeListViewColumn item) {
			if (_owner.InnerListView != null && Count == 0) {
				// The first column is added.
				View = new ExtendedGridView();
			}

			// Calling base method.
			base.InsertItem(index, item);

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		/// <summary>
		/// Moves an item from the first to the second index position.
		/// </summary>
		/// <param name="oldIndex">The old position.</param>
		/// <param name="newIndex">The new position.</param>
		protected override void MoveItem(int oldIndex, int newIndex) {
			// Calling base method.
			base.MoveItem(oldIndex, newIndex);

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		/// <summary>
		/// Removes the item stores at the given position.
		/// </summary>
		/// <param name="index">The posiiton of the item to remove.</param>
		protected override void RemoveItem(int index) {
			if (_owner.InnerListView != null && Count == 1) {
				// The last column is removed.
				View = null;
			}

			// Calling base method.
			base.RemoveItem(index);

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		/// <summary>
		/// Replaces the item stored at the given position.
		/// </summary>
		/// <param name="index">The index of the item to replace.</param>
		/// <param name="item">The new item.</param>
		protected override void SetItem(int index, TreeListViewColumn item) {
			// Calling base method.
			base.SetItem(index, item);

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		protected override void ClearItems() {
			// Calling base method.
			base.ClearItems();

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		/// <summary>
		/// Synchronizes the grid view columns.
		/// </summary>
		private void SynchronizeGridViewColumns() {
			if (View == null) return;

			View.Columns.CollectionChanged -= OnGridViewColumnsCollectionChanged;
			View.SynchronizeColumns(this);
			View.Columns.CollectionChanged += OnGridViewColumnsCollectionChanged;
		}

		/// <summary>
		/// Delegate called when the columns collection of the grid view is modified.
		/// </summary>
		/// <param name="sender">The modified grid view.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnGridViewColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs) {
			switch (eventArgs.Action) {
				case NotifyCollectionChangedAction.Add: break;// Nothing to do.
				case NotifyCollectionChangedAction.Remove: break;// Nothing to do.
				case NotifyCollectionChangedAction.Replace: break;// Nothing to do.
				case NotifyCollectionChangedAction.Move:
					Application.Current.Dispatcher.BeginInvoke(new Action(() => {
						// Updating this list.
						Move(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex);
					}));
					break;
				case NotifyCollectionChangedAction.Reset: break;// Nothing to do.
			}
		}

		/// <summary>
		/// Method called when the parent tree list view template is applied.
		/// </summary>
		internal void OnParentTreeListViewTemplateApplied() {
			if (Count <= 0) return;
			
			// Setting the grid view.
			View = new ExtendedGridView();

			// Synchronizing the grid view.
			SynchronizeGridViewColumns();
		}

		#endregion // Methods.

	}

}