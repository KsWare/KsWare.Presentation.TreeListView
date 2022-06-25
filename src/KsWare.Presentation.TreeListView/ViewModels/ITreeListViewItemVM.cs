using System;
using System.Collections;
using System.Collections.Generic;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// This interface defines an item in a hierarchical view model.
	/// </summary>
	public interface ITreeListViewItemVM : ITreeListViewItemBaseVM {

		#region Properties

		/// <summary>
		/// Gets or sets the parent of this object.
		/// </summary>
		ITreeListViewItemVM Parent { get; }

		/// <summary>
		/// Gets the logical children items of this object.
		/// </summary>
		IEnumerable<ITreeListViewItemVM> Children { get; }

		/// <summary>
		/// Gets the visible children of this item.
		/// </summary>
		IEnumerable<ITreeListViewItemVM> AllVisibleChildren { get; }

		/// <summary>
		/// Gets or sets the flag indicating if the children are loaded into the tree.
		/// </summary>
		bool ChildrenAreLoaded { get; set; }

		/// <summary>
		/// Gets the flag indcating if the item has children. Can be used for load on demand implementation.
		/// </summary>
		bool HasChildren { get; }

		/// <summary>
		/// Gets or sets the flag indicating whether the item is expanded. Mainly used to display the children.
		/// </summary>
		bool IsExpanded { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance can be selected.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance can be selected; otherwise, <c>false</c>.
		/// </value>
		bool CanBeSelected { get; }

		/// <summary>
		/// Gets or sets the flag indicating whether the item is selected or not.
		/// </summary>
		bool IsSelected { get; set; }

		/// <summary>
		/// Gets or sets the width of the decorator part of the item (includes the expander, the icon and the check box).
		/// </summary>
		double DecoratorWidth { get; set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Adds a child in the children list.
		/// </summary>
		/// <param name="child">The child to add.</param>
		void AddChild(ITreeListViewItemVM child);

		/// <summary>
		/// Insert a child at the given position.
		/// </summary>
		/// <param name="index">The index where the hild has to be inserted.</param>
		/// <param name="child">The child to insert.</param>
		void InsertChild(int index, ITreeListViewItemVM child);

		/// <summary>
		/// Removes a child from the children list.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		void RemoveChild(ITreeListViewItemVM child);

		/// <summary>
		/// Removes the child at the given position.
		/// </summary>
		/// <param name="index">The position of the child to remove.</param>
		void RemoveChildAt(int index);

		/// <summary>
		/// Clears the children list.
		/// </summary>
		/// <param name="dispose">Flag indicating if the children must be disposed or not.</param>
		void ClearChildren(bool dispose);

		/// <summary>
		/// Add a sorter using the provided comparer.
		/// </summary>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="comparer">The key comparer.</param>
		void SetSorter(Func<ITreeListViewItemVM, ITreeListViewItemVM, object> keySelector,
			IComparer<object> comparer);

		/// <summary>
		/// Removes the sorter from the view model.
		/// </summary>
		void RemoveSorter();

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		new ITreeListViewItemVM<TModel> ToGeneric<TModel>();

		/// <summary>
		/// Select all the loaded children of this item.
		/// </summary>
		/// <returns>Returns the items selected during this process.</returns>
		/// <remarks>Items that were selected before this call are not a part of the resulting list.</remarks>
		ITreeListViewItemVM[] SelectAll();

		/// <summary>
		/// Unelect all the loaded children of this item.
		/// </summary>
		/// <returns>Returns the items unselected during this process.</returns>
		/// <remarks>Items that were unselected before this call are not a part of the resulting list.</remarks>
		ITreeListViewItemVM[] UnSelectAll();

		/// <summary>
		/// Check all the loaded children of this item.
		/// </summary>
		/// <returns>Returns the items checked during this process.</returns>
		/// <remarks>Items that were checked before this call are not a part of the resulting list.</remarks>
		ITreeListViewItemVM[] CheckAll();

		/// <summary>
		/// Uncheck all the loaded children of this item.
		/// </summary>
		/// <returns>Returns the items unchecked during this process.</returns>
		/// <remarks>Items that were unchecked before this call are not a part of the resulting list.</remarks>
		ITreeListViewItemVM[] UncheckAll();

		/// <summary>
		/// Returns the children view models owning the given object.
		/// </summary>
		/// <param name="ownedObject">The analyzed owned object.</param>
		/// <param name="comparer">The owned object comparer used when searching for the view models.</param>
		/// <remarks>If the comparer is null, the comparison is made by reference.</remarks>
		/// <returns>The list of view models owning the object.</returns>
		IEnumerable<ITreeListViewItemVM> GetViewModels(object ownedObject, IComparer comparer = null);

		#endregion // Methods.

	}

	/// <summary>
	/// This interface defines a generic item in a hierarchical view model.
	/// </summary>
	/// <typeparam name="TModel">The type of the owned object.</typeparam>
	public interface ITreeListViewItemVM<TModel> : ITreeListViewItemVM, ITreeListViewItemBaseVM<TModel> { }

}