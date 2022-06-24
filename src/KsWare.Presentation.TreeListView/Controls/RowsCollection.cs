using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// Class defining the list view rows collection.
	/// </summary>
	public class RowsCollection : IList<ITreeListViewItemVM> {

		#region Fields

		/// <summary>
		/// Stores the collection owner.
		/// </summary>
		private readonly ExtendedListView _owner;

		/// <summary>
		/// Stores the source collection.
		/// </summary>
		private readonly ObservableCollection<ITreeListViewItemVM> _source;

		/// <summary>
		/// Stores the collection view source.
		/// </summary>
		private readonly CollectionViewSource _viewSource;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RowsCollection"/> class.
		/// </summary>
		/// <param name="owner">The collection owner.</param>
		public RowsCollection(ExtendedListView owner) {
			_owner = owner;

			// Creating the source collection.
			_source = new ObservableCollection<ITreeListViewItemVM>();
			_viewSource = new CollectionViewSource();
			_viewSource.Source = _source;

			// Bind it the owner items source property.
			var itemsSourceBinding = new Binding();
			itemsSourceBinding.Source = _viewSource;
			_owner.SetBinding(ItemsControl.ItemsSourceProperty, itemsSourceBinding);
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the item stores at the given index.
		/// </summary>
		/// <param name="index">The item index.</param>
		/// <returns>The found item.</returns>
		public ITreeListViewItemVM this[int index] {
			get => _source[index];
			set => _source[index] = value;
		}

		/// <summary>
		/// Gets the number of items in the collection.
		/// </summary>
		public int Count => _source.Count;

		/// <summary>
		/// Gets the flag indicating if the collection is read only.
		/// </summary>
		public bool IsReadOnly => false;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Inserts items at a given index.
		/// </summary>
		/// <param name="index">The start index of the insertion.</param>
		/// <param name="collection">The items to add.</param>
		public void InsertRange(int index, IEnumerable<ITreeListViewItemVM> collection) {
			using var refresh = _viewSource.DeferRefresh();
			using var enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext()) {
				Insert(index++, enumerator.Current);
			}
		}

		/// <summary>
		/// Removes a range of items.
		/// </summary>
		/// <param name="index">The start index of the deletion.</param>
		/// <param name="count">The number of elements to remove.</param>
		public void RemoveRange(int index, int count) {
			if (count <= 0) return;
			using var refresh = _viewSource.DeferRefresh();
			var removedCount = 0;
			while (removedCount != count) {
				RemoveAt(index);
				removedCount++;
			}
		}

		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item of interest.</param>
		/// <returns>The item index.</returns>
		public int IndexOf(ITreeListViewItemVM item) {
			return _source.IndexOf(item);
		}

		/// <summary>
		/// Inserts item at a given index.
		/// </summary>
		/// <param name="index">The index of the insertion.</param>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, ITreeListViewItemVM item) {
			_source.Insert(index, item);
		}

		/// <summary>
		/// Removes the item stored at the given index.
		/// </summary>
		/// <param name="index">The index of the item to remove.</param>
		public void RemoveAt(int index) {
			_source.RemoveAt(index);
		}

		/// <summary>
		/// Adds an item to the collection.
		/// </summary>
		/// <param name="item">The added item.</param>
		public void Add(ITreeListViewItemVM item) {
			_source.Add(item);
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear() {
			_source.Clear();
		}

		/// <summary>
		/// Verifies if the item is in the collection.
		/// </summary>
		/// <param name="item">The item to verify.</param>
		/// <returns>True if the item is a part of the collection, false otherwise.</returns>
		public bool Contains(ITreeListViewItemVM item) {
			return _source.Contains(item);
		}

		/// <summary>
		/// Copies the entire collection to a compatible one-dimensional System.Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The target array.</param>
		/// <param name="arrayIndex">The starting index in the array.</param>
		public void CopyTo(ITreeListViewItemVM[] array, int arrayIndex) {
			_source.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the given item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>True if the item has been removed, false otherwise.</returns>
		public bool Remove(ITreeListViewItemVM item) {
			return _source.Remove(item);
		}

		/// <summary>
		/// Returns the collection enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<ITreeListViewItemVM> GetEnumerator() {
			return _source.GetEnumerator();
		}

		/// <summary>
		/// Returns the collection enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion // Methods.

	}

}