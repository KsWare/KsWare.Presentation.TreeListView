using System.Collections.Generic;
using System.ComponentModel;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// Delegate called when the selection is modified.
	/// </summary>
	/// <param name="sender">The object sender.</param>
	/// <param name="eventArgs">The event arguments.</param>
	public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs eventArgs);

	/// <summary>
	/// Delegate called on the tree view.
	/// </summary>
	/// <param name="sender">The object sender (a tree list view).</param>
	/// <param name="items">The list of items.</param>
	public delegate void TreeViewEventHandler(object sender, IEnumerable<ITreeListViewItemVM> items);

	/// <summary>
	/// Delegate called on the tree view.
	/// </summary>
	/// <param name="sender">The object sender (a tree list view).</param>
	/// <param name="item">The moved item.</param>
	/// <param name="oldIndex">The old index of the item.</param>
	/// <param name="newIndex">THe new index of the item.</param>
	public delegate void TreeViewItemMovedEventHandler(object sender, ITreeListViewItemVM item, int oldIndex,
		int newIndex);

	/// <summary>
	/// Delegate called on the tree view.
	/// </summary>
	/// <param name="sender">The object sender (an item).</param>
	/// <param name="eventArgs">The event arguments.</param>
	public delegate void TreeViewItemEventHandler(object sender, PropertyChangedEventArgs eventArgs);

	/// <summary>
	/// Delegate defining the entry point of a background work executed in a view model.
	/// </summary>
	/// <param name="sender">The object sender.</param>
	/// <param name="parameters">The work parameters.</param>
	public delegate void ParameterizedBackgroundWorkStart(object sender, object parameters);

}