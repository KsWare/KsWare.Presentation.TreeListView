using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.TreeListView.Behaviors;
using KsWare.Presentation.TreeListView.Internal.Extensions;
using KsWare.Presentation.TreeListView.Models;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// This control implements a tree based on a list view.
	/// </summary>
	/// <remarks>Implemented using the project "http://www.codeproject.com/KB/WPF/wpf_treelistview_control.aspx".</remarks>
	public class ExtendedListView : ListView {

		#region Dependencies

		/// <summary>
		/// Identifies the ViewModel dependency property.
		/// </summary>
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
			typeof(ITreeListViewRootItemVM), typeof(ExtendedListView),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnViewModelChanged)));

		#endregion // Dependencies.

		#region Fields

		/// <summary>
		/// Stores the behavior responsible for resizing the columns of the list view grid view.
		/// </summary>
		public ColumnResizeBehavior ColumnResizeBehavior;

		/// <summary>
		/// Stores the behavior responsible for handling the user selection behavior.
		/// </summary>
		private readonly SelectionBehavior _selectionBehavior;

		///// <summary>
		///// Stores the behavior responsible for handling the user expand behavior.
		///// </summary>
		private readonly ExpandBehavior _expandBehavior;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="ExtendedListView"/> class.
		/// </summary>
		static ExtendedListView() {
			MultiColumnDefaultStyleKey = new ComponentResourceKey(typeof(ExtendedListView), "MultiColumnDefaultStyleKey");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedListView"/> class.
		/// </summary>
		public ExtendedListView() {
			// Creating the rows collection.
			Rows = new RowsCollection(this);

			// Creating the models.
			SelectionModel = new SelectionModel(this);
			CheckModel = new CheckModel();
			ExpandModel = new ExpandModel(this);

			// Creating the behaviors.
			_selectionBehavior = new SelectionBehavior(this);
			_expandBehavior = new ExpandBehavior(this);
			ColumnResizeBehavior = new ColumnResizeBehavior(this);
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event called when some items are added.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelsAdded;

		/// <summary>
		/// Event called when some items are removed.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelsRemoved;

		/// <summary>
		/// Event called when an item is clicked.
		/// </summary>  
		public event TreeViewEventHandler ItemViewModelClicked;

		/// <summary>
		/// Event called when an item is double clicked.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelDoubleClicked;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Stores the selection model.
		/// </summary>
		public SelectionModel SelectionModel { get; private set; }

		/// <summary>
		/// Stores the check model.
		/// </summary>
		public CheckModel CheckModel { get; private set; }

		/// <summary>
		/// Stores the expand model.
		/// </summary>
		public ExpandModel ExpandModel { get; private set; }

		/// <summary>
		/// Gets or sets the rows corresponding to the items displayed in the list view.
		/// </summary>
		internal RowsCollection Rows { get; private set; }

		/// <summary>
		/// Gets or sets the view model of the list.
		/// </summary>
		public ITreeListViewRootItemVM ViewModel {
			get => (ITreeListViewRootItemVM) GetValue(ViewModelProperty);
			set => SetValue(ViewModelProperty, value);
		}

		/// <summary>
		/// Gets the view as extended grid view.
		/// </summary>
		internal new ExtendedGridView View {
			get => base.View as ExtendedGridView;
			set {
				if (base.View == value) return;
				base.View = value;
				if (value != null) ColumnResizeBehavior.Activate();
				else ColumnResizeBehavior.Deactivate();
			}
		}

		/// <summary>
		/// Gets the item style key when the tree is in multi column mode.
		/// </summary>
		public static ResourceKey MultiColumnDefaultStyleKey { get; private set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// This method creates the container for the item.
		/// In this case, the method creates a TreeListViewItem.
		/// </summary>
		/// <returns>a TreeListViewItem.</returns>
		protected override DependencyObject GetContainerForItemOverride() {
			return new TreeListViewItem();
		}

		/// <summary>
		/// This method checks if the item is overriden.
		/// </summary>
		/// <param name="item">The native item.</param>
		/// <returns>True if the item has been overriden.</returns>
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is TreeListViewItem;
		}

		/// <summary>
		/// This delegate is called when the view model is changed.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private static void OnViewModelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs) {
			if (sender is not ExtendedListView extendedListView) return;
			
			// Unloading the old view model.
			if (eventArgs.OldValue is ITreeListViewRootItemVM oldViewModel) {
				// Unregistering from items modification events.
				oldViewModel.ItemViewModelsAdded -= extendedListView.OnItemViewModelsAdded;
				oldViewModel.ItemViewModelsRemoved -= extendedListView.OnItemViewModelsRemoved;
				oldViewModel.ItemViewModelMoved -= extendedListView.OnItemViewModelMoved;

				// Initializing the view model.
				extendedListView.RemoveChildrenItems(oldViewModel, false);
			}

			// Loading the new view model.
			if (eventArgs.NewValue is ITreeListViewRootItemVM newViewModel) {
				// Registering on items modification events.
				newViewModel.ItemViewModelsAdded += extendedListView.OnItemViewModelsAdded;
				newViewModel.ItemViewModelsRemoved += extendedListView.OnItemViewModelsRemoved;
				newViewModel.ItemViewModelMoved += extendedListView.OnItemViewModelMoved;

				// Loading the first level items.
				extendedListView.LoadsChildrenItems(newViewModel);
			}
		}

		/// <summary>
		/// Scrolls the view to show the wanted item to the user.
		/// </summary>
		/// <param name="item">The item to bring.</param>
		/// <param name="select">Flag indicating if the selected item must be selected.</param>
		/// <returns>True if the item is loaded, false otherwise.</returns>
		public bool ScrollIntoView(ITreeListViewItemVM item, bool select) {
			if (item?.Parent == null) return false;

			// Expand its parent to make Item visible.
			ExpandModel.SetIsExpanded(item.Parent, true);

			// Showing the added item.
			ScrollIntoView(item);

			// Selecting the item if asked.
			if (select) SelectionModel.Select(item);

			return true;
		}

		/// <summary>
		/// Loads the children of this item in the control.
		/// </summary>
		/// <param name="viewModel">The view model containing the children.</param>
		internal void LoadsChildrenItems(ITreeListViewItemVM viewModel) {
			if (viewModel.ChildrenAreLoaded) return;
			
			var startIndex = Rows.IndexOf(viewModel);
			Rows.InsertRange(startIndex + 1, viewModel.AllVisibleChildren.ToArray());

			viewModel.ChildrenAreLoaded = true;
			viewModel.AllVisibleChildren.Where(item => item.IsExpanded)
				.ForEach(item => item.ChildrenAreLoaded = true);
		}

		/// <summary>
		/// Removes the children from the rows.
		/// </summary>
		/// <param name="viewModel">The view model containing the rows.</param>
		/// <param name="includeParent">Flag indicating if the parent must be removed has well.</param>
		internal void RemoveChildrenItems(ITreeListViewItemVM viewModel, bool includeParent) {
			if (!viewModel.ChildrenAreLoaded) return;

			var startIndex = Rows.IndexOf(viewModel);
			var count = viewModel.AllVisibleChildren.Count();

			if (includeParent == false) startIndex++; else count++;

			// The item must be in the children list.
			if (startIndex != -1) Rows.RemoveRange(startIndex, count);

			viewModel.ChildrenAreLoaded = false;
			viewModel.AllVisibleChildren.Where(item => item.IsExpanded)
				.ForEach(item => item.ChildrenAreLoaded = false);
		}

		/// <summary>
		/// Delegate called when a click is performed in the tree list view.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnTreeListViewMouseDown(System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the selection.
			_selectionBehavior.OnTreeListViewMouseDown(eventArgs);
		}

		/// <summary>
		/// Delegate called when a key is pressed when the item get the focus.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemKeyUp(ITreeListViewItemVM item, System.Windows.Input.KeyEventArgs eventArgs) {
			// Handling the expand.
			_expandBehavior.OnItemKeyUp(item, eventArgs);
		}

		/// <summary>
		/// Delegate called when the mouse left button is down on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseLeftButtonDown(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the selection.
			_selectionBehavior.OnItemMouseLeftButtonDown(item, eventArgs);
		}

		/// <summary>
		/// Delegate called when the mouse right button is down on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseRightButtonDown(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the selection.
			_selectionBehavior.OnItemMouseRightButtonDown(item, eventArgs);
		}

		/// <summary>
		/// Delegate called when the mouse left button is up on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseLeftButtonUp(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the selection.
			_selectionBehavior.OnItemMouseLeftButtonUp(item, eventArgs);
		}

		/// <summary>
		/// Delegate called when the mouse right button is up on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseRightButtonUp(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the selection.
			_selectionBehavior.OnItemMouseRightButtonUp(item, eventArgs);
		}

		/// <summary>
		/// Delegate called when the mouse clicked on this item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseClicked(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Notification.
			ItemViewModelClicked?.Invoke(this, new ITreeListViewItemVM[] {item});
		}

		/// <summary>
		/// Delegate called when the mouse double clicked on this item.
		/// </summary>
		/// <param name="item">The double clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		internal void OnItemMouseDoubleClicked(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			// Handling the expand.
			_expandBehavior.OnItemMouseDoubleClicked(item, eventArgs);

			// Notification.
			ItemViewModelDoubleClicked?.Invoke(this, new ITreeListViewItemVM[] {item});
		}

		/// <summary>
		/// Delegate called when the expander gets checked on the given item.
		/// </summary>
		/// <param name="item">The checked item.</param>
		internal void OnItemExpanderChecked(ITreeListViewItemVM item) {
			// Handling the expand.
			_expandBehavior.OnItemExpanderChecked(item);
		}

		/// <summary>
		/// Delegate called when the expander gets unchecked on the given item.
		/// </summary>
		/// <param name="item">The unchecked item.</param>
		internal void OnItemExpanderUnchecked(ITreeListViewItemVM item) {
			// Handling the expand.
			_expandBehavior.OnItemExpanderUnchecked(item);
		}

		/// <summary>
		/// Delegate called when items are added in the view model. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="items">The added items.</param>
		private void OnItemViewModelsAdded(object sender, IEnumerable<ITreeListViewItemVM> items) {
			// Updating the node loading in the list view.
			foreach (var item in items) {
				if (item.Parent.IsExpanded == false || !item.Parent.ChildrenAreLoaded) continue;

				// Computing the index of the item in the rows.
				var parentRowIndex = Rows.IndexOf(item.Parent);
				var childIndex = item.Parent.AllVisibleChildren.ToList().IndexOf(item);
				var indexInRows = parentRowIndex + childIndex + 1;

				// Adding the item in the rows.
				if (indexInRows >= Rows.Count) Rows.Add(item);
				else Rows.Insert(indexInRows, item);

				// Recursive call on the visible children.
				if (item.IsExpanded) LoadsChildrenItems(item);
			}

			// Forwarding the notification.
			ItemViewModelsAdded?.Invoke(this, items);
		}

		/// <summary>
		/// Delegate called when items are removed from the view model. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="items">The removed items.</param>
		private void OnItemViewModelsRemoved(object sender, IEnumerable<ITreeListViewItemVM> items) {
			// Updating the node loading in the list view.
			foreach (var item in items) {
				// Removing the items from the selected and toggled list.
				SelectionModel.Unselect(item, true);
				CheckModel.Uncheck(item, true);

				// Removing the items from the tree view if they are displayed.
				if (item.IsExpanded) RemoveChildrenItems(item, true);
				else Rows.Remove(item);
			}

			// Forwarding the notification.
			ItemViewModelsRemoved?.Invoke(this, items);
		}

		/// <summary>
		/// Delegate called when items are removed from the view model. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="item">The removed item.</param>
		/// <param name="oldIndex">The old index of the item.</param>
		/// <param name="newIndex">THe new index of the item.</param>
		private void OnItemViewModelMoved(object sender, ITreeListViewItemVM item, int oldIndex, int newIndex) {
			if (item == null) return;
			
			// Computing the row indexes.
			var oldRowIndex = oldIndex;
			var newRowIndex = newIndex;
			if (item.Parent != null && item.Parent is ITreeListViewRootItemVM == false) {
				var parentRowIndex = Rows.IndexOf(item.Parent);
				oldRowIndex = parentRowIndex + oldIndex + 1;
				newRowIndex = parentRowIndex + newIndex + 1;
			}

			// Removing the items from the tree view if they are displayed.
			if (item.IsExpanded) RemoveChildrenItems(item, true);
			else Rows.Remove(item);

			// Adding the item in the rows.
			if (newRowIndex >= Rows.Count) Rows.Add(item);
			else Rows.Insert(newRowIndex, item);
		}

		#endregion // Methods.

	}

}