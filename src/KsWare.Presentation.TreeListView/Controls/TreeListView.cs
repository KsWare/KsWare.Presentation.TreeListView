using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KsWare.Presentation.TreeListView.Resources;
using KsWare.Presentation.TreeListView.ViewModels;
using SelectionChangedEventArgs = KsWare.Presentation.TreeListView.ViewModels.SelectionChangedEventArgs;
using SelectionChangedEventHandler = KsWare.Presentation.TreeListView.ViewModels.SelectionChangedEventHandler;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// This class defines a tree list view.
	/// </summary>
	[TemplatePart(Name = PartListView, Type = typeof(ExtendedListView))]
	[TemplatePart(Name = PartDefaultMessage, Type = typeof(Label))]
	[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
	public class TreeListView : ItemsControl {

		#region Constants

		/// <summary>
		/// Name of the parts that have to be in the control template.
		/// </summary>
		private const string PartListView = "PART_ListView";

		private const string PartDefaultMessage = "PART_DefaultMessage";

		/// <summary>
		/// Constant containing the default property name displayed in the tree view.
		/// </summary>
		internal const string DefaultDisplayedPropertyName = "DisplayString";

		#endregion


		#region DependencyProperties

		/// <summary>
		/// Identifies the ViewModel dependency property.
		/// </summary>
		public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
			typeof(ITreeListViewRootItemVM), typeof(TreeListView),
			new FrameworkPropertyMetadata(null, (d,e)=>((TreeListView)d).OnViewModelChanged(e)));

		/// <summary>
		/// Identifies the DefaultMessage dependency property.
		/// </summary>
		public static readonly DependencyProperty DefaultMessageProperty = DependencyProperty.Register("DefaultMessage",
			typeof(string), typeof(TreeListView), new FrameworkPropertyMetadata("No message defined.")); // TODO message

		/// <summary>
		/// Identifies the GroupItemDataTemplate dependency property.
		/// </summary>
		public static readonly DependencyProperty GroupItemDataTemplateProperty =
			DependencyProperty.Register("GroupItemDataTemplate", typeof(DataTemplate), typeof(TreeListView),
				new FrameworkPropertyMetadata(All.Instance["GroupItemDefaultDataTemplate"]));

		/// <summary>
		/// Identifies the FirstLevelItemsAsGroup dependency property.
		/// </summary>
		public static readonly DependencyProperty FirstLevelItemsAsGroupProperty =
			DependencyProperty.Register("FirstLevelItemsAsGroup", typeof(bool), typeof(TreeListView),
				new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Identifies the Columns attached dependency property key.
		/// </summary>
		private static readonly DependencyPropertyKey ColumnsPropertyKey =
			DependencyProperty.RegisterAttachedReadOnly("Columns", typeof(TreeListViewColumnCollection),
				typeof(TreeListView), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Identifies the Columns attached dependency property.
		/// </summary>
		private static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		#endregion // DependencyProperties.

		#region Fields

		/// <summary>
		/// Stores the label used to display the default message.
		/// </summary>
		private Label _defaultMessageLabel;

		/// <summary>
		/// Stores the flag indicating the item selection mode when the inner tree list view is not loaded yet.
		/// </summary>
		private TreeSelectionMode _pendingSelectionMode;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="TreeListView"/> class.
		/// </summary>
		static TreeListView() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView),
				new FrameworkPropertyMetadata(typeof(TreeListView)));
			DisplayMemberPathProperty.OverrideMetadata(typeof(TreeListView),
				new FrameworkPropertyMetadata(DefaultDisplayedPropertyName));
			ItemTemplateProperty.OverrideMetadata(typeof(TreeListView),
				new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceItemTemplateAndSelector)));
			ItemTemplateSelectorProperty.OverrideMetadata(typeof(TreeListView),
				new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceItemTemplateAndSelector)));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListView"/> class.
		/// </summary>
		public TreeListView() {
			InnerListView = null;
			SelectionMode = TreeSelectionMode.SingleSelection;
			Columns = new TreeListViewColumnCollection(this);
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event raised when the selection is modified.
		/// </summary>
		public event SelectionChangedEventHandler SelectionChanged;

		/// <summary>
		/// Event raised when an item gets toggled.
		/// </summary>
		public event TreeViewEventHandler ItemsViewModelToggled;

		/// <summary>
		/// Event called when some items are added.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelsAdded;

		/// <summary>
		/// Event called when some items are removed.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelsRemoved;

		/// <summary>
		/// Event called when some items are modified.
		/// </summary>
		public event TreeViewItemEventHandler ItemViewModelModified;

		/// <summary>
		/// [Not implemented] Event called when an item is clicked.
		/// </summary>  
		public event TreeViewEventHandler ItemViewModelClicked {
			add => throw new NotImplementedException();
			remove => throw new NotImplementedException();
		}

		/// <summary>
		/// Event called when an item is double clicked.
		/// </summary>
		public event TreeViewEventHandler ItemViewModelDoubleClicked;

		/// <summary>
		/// Event raised when the view model changed.
		/// </summary>
		public event DependencyPropertyChangedEventHandler ViewModelChanged;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Gets the tree list view id.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Get the tree list view display name. Used as name in the GUI.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Gets or sets the view model of the tree.
		/// </summary>
		public ITreeListViewRootItemVM ViewModel {
			get => (ITreeListViewRootItemVM) GetValue(ViewModelProperty);
			set => SetValue(ViewModelProperty, value);
		}

		/// <summary>
		/// Gets or sets the data template of a group item.
		/// </summary>
		public DataTemplate GroupItemDataTemplate {
			get => (DataTemplate) GetValue(GroupItemDataTemplateProperty);
			set => SetValue(GroupItemDataTemplateProperty, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating if the first level items must be displayed as group.
		/// </summary>
		public bool FirstLevelItemsAsGroup {
			get => (bool) GetValue(FirstLevelItemsAsGroupProperty);
			set => SetValue(FirstLevelItemsAsGroupProperty, value);
		}

		/// <summary>
		/// Gets or sets the default message of the tree.
		/// </summary>
		public string DefaultMessage {
			get => (string) GetValue(DefaultMessageProperty);
			set => SetValue(DefaultMessageProperty, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating the item selection mode.
		/// </summary>
		public TreeSelectionMode SelectionMode {
			get => InnerListView != null ? InnerListView.SelectionModel.SelectionMode : _pendingSelectionMode;
			set {
				if (InnerListView != null) InnerListView.SelectionModel.SelectionMode = value;
				else _pendingSelectionMode = value;
			}
		}

		/// <summary>
		/// Gets the selected items view model.
		/// </summary>
		public IEnumerable<ITreeListViewItemVM> SelectedViewModels => InnerListView != null ? InnerListView.SelectionModel.SelectedViewModels : Array.Empty<ITreeListViewItemVM>();

		/// <summary>
		/// Gets the selected objects.
		/// </summary>
		public IEnumerable<object> SelectedObjects => InnerListView != null ? InnerListView.SelectionModel.SelectedObjects : Array.Empty<object>();

		/// <summary>
		/// Gets the checked items view model.
		/// </summary>
		public IEnumerable<ITreeListViewItemVM> CheckedViewModels => InnerListView != null ? InnerListView.CheckModel.CheckedItemsViewModel : Array.Empty<ITreeListViewItemVM>();

		/// <summary>
		/// Gets or sets the inner list view.
		/// </summary>
		internal ExtendedListView InnerListView { get; private set; }

		/// <summary>
		/// Gets the columns of the tree list view.
		/// </summary>
		public TreeListViewColumnCollection Columns {
			get => GetColumns(this);
			private set => SetValue(ColumnsPropertyKey, value);
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Called when the control template is applied.
		/// </summary>
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			// Getting the parts from the control template.
			InnerListView = GetTemplateChild(PartListView) as ExtendedListView;
			_defaultMessageLabel = GetTemplateChild(PartDefaultMessage) as Label;

			if (InnerListView == null || _defaultMessageLabel == null) {
				throw new Exception("TreeListView control template not correctly defined.");
			}

			// Notifying the columns collection the template is applied.
			Columns.OnParentTreeListViewTemplateApplied();

			// Loading the view model.
			LoadViewModel();

			// Applying the selection option.
			InnerListView.SelectionModel.SelectionMode = _pendingSelectionMode;

			// Registering on the collection changed event for the selected and checked items.
			InnerListView.SelectionModel.SelectionChanged += OnInnerListViewSelectionChanged;
			InnerListView.CheckModel.ItemsViewModelToggled += OnInnerListItemsViewModelToggled;
			InnerListView.ItemViewModelsAdded += OnInnerListViewItemViewModelsAdded;
			InnerListView.ItemViewModelsRemoved += OnInnerListViewItemViewModelsRemoved;
			InnerListView.ItemViewModelDoubleClicked += OnInnerListViewItemViewModelDoubleClicked;
		}

		/// <summary>
		/// Returns the column collection.
		/// </summary>
		/// <param name="control">The tree control.</param>
		/// <returns>The column collection.</returns>
		public static TreeListViewColumnCollection GetColumns(TreeListView control) {
			return (TreeListViewColumnCollection) control.GetValue(ColumnsProperty);
		}

		/// <summary>
		/// This method forces the item to be visible in the viewport.
		/// </summary>
		/// <param name="item">The item to make visible into the viewport.</param>
		public void ScrollToItem(ITreeListViewItemVM item) {
			InnerListView?.ScrollIntoView(item, true);
		}

		/// <summary>
		/// This method selects an item.
		/// </summary>
		/// <param name="item">The item to select.</param>
		public void Select(ITreeListViewItemVM item) {
			InnerListView?.SelectionModel.Select(item);
		}

		/// <summary>
		/// This method selects all the items.
		/// </summary>
		public void SelectAll() {
			InnerListView?.SelectionModel.SelectAll();
		}

		/// <summary>
		/// This method unselects an item.
		/// </summary>
		/// <param name="item">The item to unselect.</param>
		public void Unselect(ITreeListViewItemVM item) {
			InnerListView?.SelectionModel.Unselect(item, false);
		}

		/// <summary>
		/// Unselect all the items in the tree.
		/// </summary>
		public void UnselectAll() {
			InnerListView?.SelectionModel.UnselectAll();
		}

		/// <summary>
		/// Expands the given item.
		/// </summary>
		/// <param name="item">The item to expand.</param>
		public void Expand(ITreeListViewItemVM item) {
			InnerListView?.ExpandModel.SetIsExpanded(item, true);
		}

		/// <summary>
		/// Collapses the given item.
		/// </summary>
		/// <param name="item">The item to expand.</param>
		public void Collapse(ITreeListViewItemVM item) {
			InnerListView?.ExpandModel.SetIsExpanded(item, false);
		}

		/// <summary>
		/// Checks the given item.
		/// </summary>
		/// <param name="item">The item to check.</param>
		public void Check(ITreeListViewItemVM item) {
			InnerListView?.CheckModel.Check(item, false);
		}

		/// <summary>
		/// Unchecks the given item.
		/// </summary>
		/// <param name="item">The item to uncheck.</param>
		public void Uncheck(ITreeListViewItemVM item) {
			InnerListView?.CheckModel.Uncheck(item, false);
		}

		/// <summary>
		/// This delegate is called when the view model is changed.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		private void OnViewModelChanged(DependencyPropertyChangedEventArgs e) {
			if (InnerListView == null) return;
			// Unloading the view model from the inner list.
			InnerListView.ViewModel = null;

			// Invalidating the old view model.
			if (e.OldValue is ITreeListViewRootItemVM oldViewModel) {
				oldViewModel.PropertyChanged -= OnRootViewModelPropertyChanged;
				oldViewModel.ItemViewModelModified -= OnItemViewModelModified;
			}

			// Loading the new view model.
			LoadViewModel();

			// Notifying the user.
			ViewModelChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Loads the view model into the inner list view.
		/// </summary>
		private void LoadViewModel() {
			// Handling the new view model.
			if (ViewModel == null) return;
			// Registering on the view model PropertyChanged event.
			UpdateDefaultMessageVisibility();
			ViewModel.PropertyChanged += OnRootViewModelPropertyChanged;
			ViewModel.ItemViewModelModified += OnItemViewModelModified;

			// Initializing the view model.
			InnerListView.ViewModel = ViewModel;
		}

		/// <summary>
		/// Delegate called when an item view model property is modified.
		/// </summary>
		/// <param name="sender">The modified item view model.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnItemViewModelModified(object sender, PropertyChangedEventArgs eventArgs) {
			ItemViewModelModified?.Invoke(sender, eventArgs);
		}

		/// <summary>
		/// Delegate called when a property is modified on the view model.
		/// </summary>
		/// <param name="sender">The modified view model.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnRootViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs) {
			UpdateDefaultMessageVisibility();
		}

		/// <summary>
		/// Updates the default message visibility.
		/// </summary>
		private void UpdateDefaultMessageVisibility() {
			if (ViewModel != null && ViewModel.Children.Any()) {
				// Hiding the default message.
				_defaultMessageLabel.Visibility = Visibility.Hidden;
			}
			else {
				// Showing the default message.
				_defaultMessageLabel.Visibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// Delegate called when the ItemTemplate or the ItemTemplateSelector property values have to be coerced.
		/// </summary>
		/// <param name="sender">The modified tree list view.</param>
		/// <param name="obj">The object to coerce.</param>
		/// <returns>The coerced object.</returns>
		private static object OnCoerceItemTemplateAndSelector(DependencyObject sender, object obj) {
			if (sender is TreeListView control && obj != null) {
				// ItemTemplate and ItemTemplateSelector properties cannot be set (exception) if the DisplayMemberPath
				// is defined. It is the case as it is the default behaviour of the tree.
				// When using the ItemTemplate or ItemTemplateSelector properties, the DisplayMemberPath is then invalidated.
				control.DisplayMemberPath = string.Empty;
			}

			return obj;
		}

		/// <summary>
		/// Method called when a click is performed in the tree list view.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs eventArgs) {
			base.OnMouseDown(eventArgs);

			// This event is raised when the click is performed in the tree but not in a specific item.
			InnerListView?.OnTreeListViewMouseDown(eventArgs);
		}

		/// <summary>
		/// Delegate called when some items are toggled.
		/// </summary>
		/// <param name="sender">The modified list view.</param>
		/// <param name="viewModels">The toggled items.</param>
		private void OnInnerListItemsViewModelToggled(object sender, IEnumerable<ITreeListViewItemVM> viewModels) {
			ItemsViewModelToggled?.Invoke(this, viewModels);
		}

		/// <summary>
		/// Delegate called when the selection changed on the inner list view.
		/// </summary>
		/// <param name="sender">The modified list view.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnInnerListViewSelectionChanged(object sender, SelectionChangedEventArgs eventArgs) {
			OnSelectionChanged(eventArgs.RemovedItems, eventArgs.AddedItems);
		}

		/// <summary>
		/// Method called when the selection changed.
		/// </summary>
		/// <param name="addedItems">The added items from the previous selection.</param>
		/// <param name="removedItems">The removed items from the previous selection.</param>
		protected virtual void OnSelectionChanged(IEnumerable removedItems, IEnumerable addedItems) {
			// Notifying threw the tree.
			var handler = SelectionChanged;
			if (handler == null) return;
			var selectionEventArgs = new SelectionChangedEventArgs(removedItems, addedItems);
			handler(this, selectionEventArgs);
		}

		/// <summary>
		/// Delegate called when items are added in the view model. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="items">The added items.</param>
		private void OnInnerListViewItemViewModelsAdded(object sender, IEnumerable<ITreeListViewItemVM> items) {
			ItemViewModelsAdded?.Invoke(this, items);
		}

		/// <summary>
		/// Delegate called when items are removed from the view model. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="items">The removed items.</param>
		private void OnInnerListViewItemViewModelsRemoved(object sender, IEnumerable<ITreeListViewItemVM> items) {
			ItemViewModelsRemoved?.Invoke(this, items);
		}

		/// <summary>
		/// Delegate called when items are double clicked. 
		/// </summary>
		/// <param name="sender">The modified root view model.</param>
		/// <param name="items">The removed items.</param>
		private void OnInnerListViewItemViewModelDoubleClicked(object sender, IEnumerable<ITreeListViewItemVM> items) {
			ItemViewModelDoubleClicked?.Invoke(this, items);
		}

		#endregion // Methods.

	}

}