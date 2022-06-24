using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// Defines a delegate used to define a specific view model creation.
	/// </summary>
	/// <param name="ownedObject">The view model owned object.</param>
	public delegate TreeListViewItemVM CreateChildViewModelDelegate(object ownedObject);

	/// <summary>
	/// Sets the order.
	/// </summary>
	public enum SortOrder {

		/// <summary>
		/// Enumeration value indicating the items are sorted in increasing order.
		/// </summary>
		Ascending,

		/// <summary>
		/// Enumeration value indicating the items are sorted in decreasing order.
		/// </summary>
		Descending,

		/// <summary>
		/// Enumeration value indicating the items are unordered.
		/// </summary>
		Unsorted,

	}

	/// <summary>
	/// 
	/// </summary>
	public enum SortKey {

		/// <summary>
		/// The display string is considered as the sort key.
		/// </summary>
		DisplayString,

		/// <summary>
		/// Enumeration value indicating the sort key 
		/// </summary>
		ChildCount,

	}

	/// <summary>
	/// This class defines an abstract tree list view item view model.
	/// </summary>
	public abstract class TreeListViewItemVM : TreeListViewBaseVM, ITreeListViewItemVM {

		#region Fields

		/// <summary>
		/// Stores the children of this item.
		/// </summary>
		private readonly ObservableCollection<TreeListViewItemVM> _children;

		/// <summary>
		/// The parent view model.
		/// </summary>
		private TreeListViewItemVM _parent;

		/// <summary>
		/// Stores the flag indicating if the item is expanded or not.
		/// </summary>
		private bool _isExpanded;

		/// <summary>
		/// This field stores the flag to know if the item is selected.
		/// </summary>
		private bool _isSelected;

		/// <summary>
		/// Gets the index of the node in its parent collection.
		/// </summary>
		private int _index;

		/// <summary>
		/// Stores the decorator width (part where are displayed the expander, icon...).
		/// </summary>
		private double _decoratorWidth;

		/// <summary>
		/// This field stores the key selector.
		/// </summary>
		private Func<ITreeListViewItemVM, ITreeListViewItemVM, object> _comparerKeySelector;

		/// <summary>
		/// This field stores the key comparer.
		/// </summary>
		private IComparer<object> _comparer;

		/// <summary>
		/// Stores the bindings of property changed names between the mViewModel and the viewModel.
		/// e.g. with an entry ["Name", "DisplayString"] when the ViewModel (this) gets notified of the 
		/// mViewModel's "Name" property change, it fires a propertyChangedEvent whose name is "DisplayString".
		/// NOTE : It will only be used if mOwnedObject implements INotifyPropertyChanged.
		/// </summary> 
		private readonly Dictionary<INotifyCollectionChanged, CreateChildViewModelDelegate> _childrenBinding;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListViewItemVM"/> class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		internal TreeListViewItemVM(object ownedObject)
			: base(ownedObject) {
			_isExpanded = false;
			_isSelected = false;
			ChildrenRegistered = false;
			IsInRegistrationMode = false;
			_index = -1;
			(this as ITreeListViewItemVM).ChildrenAreLoaded = false;
			PropertyChanged += NotifyItemViewModelModified;

			// The parent of the children (this) is set here. 
			// An item does not have any parent while it has not been added to a children list.
			_children = new ViewModelCollection(this);
			_children.CollectionChanged += OnChildrenCollectionChanged;

			// Initializing the dictionaries storing the binding between model and view model.
			_childrenBinding = new Dictionary<INotifyCollectionChanged, CreateChildViewModelDelegate>();
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event raised when the children list is modified.
		/// </summary>
		public event NotifyCollectionChangedEventHandler ChildrenCollectionChanged;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Gets the index of the item in the parent list.
		/// </summary>
		public int Index => _index;

		/// <summary>
		/// Gets or sets the parent of this object.
		/// Property defined to implement the IHierarchicalItemViewModel interface.
		/// </summary>
		ITreeListViewItemVM ITreeListViewItemVM.Parent => Parent;

		/// <summary>
		/// Gets the root hierarchical view models.
		/// </summary>
		public TreeListViewItemVM Root {
			get {
				var root = Parent;
				if (root == null) {
					return null;
				}

				while (root.Parent != null) {
					root = root.Parent;
				}

				return root;
			}
		}

		/// <summary>
		/// Gets or sets the parent of the current view model.
		/// </summary>
		public TreeListViewItemVM Parent {
			get => _parent;

			private set {
				if (_parent != value) {
					_parent = value;
					NotifyPropertyChanged("Parent");
				}
			}
		}

		/// <summary>
		/// Gets or sets the parent of this object.
		/// Property defined to implement the IHierarchicalItemViewModel interface.
		/// </summary>
		IEnumerable<ITreeListViewItemVM> ITreeListViewItemVM.Children => Children;

		/// <summary>
		/// Gets the children list.
		/// </summary>
		public IEnumerable<TreeListViewItemVM> Children => _children;

		/// <summary>
		/// Gets the flag indicating if the item has children.
		/// </summary>
		public bool HasChildren {
			get {
				if (ChildrenRegistered) {
					return (_children.Count > 0) && (Children.Any(child => child.IsVisible));
				}

				return HasChildrenLoadedOnDemand;
			}
		}

		/// <summary>
		/// Flag defining if some children will be loaded on demand.
		/// </summary>
		public virtual bool HasChildrenLoadedOnDemand => false;

		/// <summary>
		/// Gets or sets the flag indicating if the item is expanded or not.
		/// </summary>
		public bool IsExpanded {
			get => _isExpanded;
			set {
				if (_isExpanded != value) {
					// Registering the items if it is not done.
					if (value) {
						RegisterChildren();
					}

					// Expanding or collapse this node.
					_isExpanded = value;
					OnIsExpandedChanged(value);
					NotifyPropertyChanged("IsExpanded");
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance can be selected.
		/// </summary>
		public virtual bool CanBeSelected => true;

		/// <summary>
		/// Gets or sets the flag to know if the item is selected.
		/// </summary>
		public bool IsSelected {
			get => _isSelected;
			set {
				if (_isSelected != value && CanBeSelected) {
					_isSelected = value;
					NotifyPropertyChanged("IsSelected");
				}
			}
		}


		/// <summary>
		/// Gets or sets the flag indicating if the children are loaded into the tree.
		/// </summary>
		bool ITreeListViewItemVM.ChildrenAreLoaded { get; set; }

		/// <summary>
		/// Gets or sets the decarator width.
		/// </summary>
		public double DecoratorWidth {
			get => _decoratorWidth;

			set {
				_decoratorWidth = value;
				NotifyPropertyChanged("DecoratorWidth");
			}
		}

		/// <summary>
		/// Gets the list of visible children.
		/// </summary>
		IEnumerable<ITreeListViewItemVM> ITreeListViewItemVM.AllVisibleChildren => AllVisibleChildren;

		/// <summary>
		/// Gets the list of visible children.
		/// </summary>
		private IEnumerable<TreeListViewItemVM> AllVisibleChildren {
			get {
				var viewModel = this;
				while (true) {
					viewModel = viewModel.NextVisibleViewModel;
					if ((viewModel != null)
					    && (viewModel.Level > Level)
					   ) {
						yield return viewModel;
					}
					else {
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets the flag indicating if the children view model have been registered.
		/// </summary>
		/// <remarks>
		/// Children are automatically tagged as registered if the load on demand is not activated.
		/// </remarks>
		protected bool ChildrenRegistered { get; set; }

		/// <summary>
		/// Gets or sets the flag indicating if calling the children modification methods is made during a registration process.
		/// </summary>
		private bool IsInRegistrationMode { get; set; }

		/// <summary>
		/// Gets the flag indicating if the items are loaded on demand.
		/// </summary>
		protected virtual bool LoadItemsOnDemand {
			get {
				if (Parent != null) {
					return Parent.LoadItemsOnDemand;
				}

				return true;
			}
		}

		/// <summary>
		/// Gets the level of the node (depth).
		/// </summary>
		private int Level {
			get {
				if (Parent == null) {
					return -1;
				}

				return Parent.Level + 1;
			}
		}

		/// <summary>
		/// Gets the next visible view model.
		/// </summary>
		private TreeListViewItemVM NextVisibleViewModel {
			get {
				if (IsExpanded && _children.Count > 0) {
					return _children[0];
				}
				else {
					var n = NextViewModel;
					if (n != null) return n;
				}
				return BottomViewModel;
			}
		}

		/// <summary>
		/// Gets the bottom view model.
		/// </summary>
		private TreeListViewItemVM BottomViewModel {
			get {
				if (Parent != null) {
					if (Parent.NextViewModel != null) {
						return Parent.NextViewModel;
					}

					return Parent.BottomViewModel;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the next view model (sibling).
		/// </summary>
		private TreeListViewItemVM NextViewModel {
			get {
				if (Parent != null) {
					if (_index < Parent._children.Count - 1) {
						return Parent._children[_index + 1];
					}
				}

				return null;
			}
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Add a sorter using the provided comparer.
		/// </summary>
		/// <param name="keySelector">The key selector.</param>
		/// <param name="comparer">The key comparer.</param>
		public void SetSorter(Func<ITreeListViewItemVM, ITreeListViewItemVM, object> keySelector,
			IComparer<object> comparer) {
			_comparerKeySelector = keySelector;
			_comparer = comparer;
		}

		/// <summary>
		/// Removes the sorter from the view model.
		/// </summary>
		public void RemoveSorter() {
			_comparerKeySelector = null;
			_comparer = null;
		}

		/// <summary>
		/// Adds a child in the children list.
		/// </summary>
		/// <param name="child">The child to add.</param>
		public virtual void AddChild(ITreeListViewItemVM child) {
			if (child is TreeListViewItemVM c) {
				AddChild(c);
			}
		}

		/// <summary>
		/// Insert a child at the given position.
		/// </summary>
		/// <param name="index">The index where the hild has to be inserted.</param>
		/// <param name="child">The child to insert.</param>
		public void InsertChild(int index, ITreeListViewItemVM child) {
			if (child is TreeListViewItemVM c) {
				InsertChild(index, c);
			}
		}

		/// <summary>
		/// Removes a child from the children list.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		public void RemoveChild(ITreeListViewItemVM child) {
			if (child is TreeListViewItemVM c) {
				RemoveChild(c);
			}
		}

		/// <summary>
		/// Removes the child at the given position.
		/// </summary>
		/// <param name="index">The position of the child to remove.</param>
		public void RemoveChildAt(int index) {
			RemoveChildAtInternal(index);
		}

		/// <summary>
		/// Adds a child to this item.
		/// </summary>
		/// <param name="child">The item to add.</param>
		private void AddChild(TreeListViewItemVM child) {
			if (IsInRegistrationMode) {
				// Adding the children.
				AddOrganizedChild(child);
			}
			else {
				// If never expanded, registers the already defined children.
				RegisterChildren();

				// Adding the children.
				AddOrganizedChild(child);
			}
		}

		/// <summary>
		/// Insert a child to this item.
		/// </summary>
		/// <param name="index">The index where the item has to be inserted.</param>
		/// <param name="child">The item to insert.</param>
		/// <returns>True if the child has been added, false if the index is out of range.</returns>
		private void InsertChild(int index, TreeListViewItemVM child) {
			// Verifying if the item is in the range.
			if
				((index < 0)
				 || (index >= _children.Count)
				) {
				return;
			}

			if (IsInRegistrationMode) {
				// Adding the children.
				InsertOrganizedChild(index, child);
			}
			else {
				// If never expanded, registers the already defined children.
				RegisterChildren();

				// Adding the children.
				InsertOrganizedChild(index, child);
			}
		}

		/// <summary>
		/// Moves a child from the old index to the new index.
		/// </summary>
		/// <param name="oldIndex">The old index of the child.</param>
		/// <param name="newIndex">The new index of the child.</param>
		/// <returns>True if the child has been moved, false if one of the indexes is out of range.</returns>
		public void MoveChild(int oldIndex, int newIndex) {
			// Verifying if the item is in the range.
			if
				((oldIndex < 0)
				 || (oldIndex >= _children.Count)
				 || (_comparerKeySelector != null)
				) {
				return;
			}

			var child = _children.ElementAt(oldIndex);
			if (child != null) {
				_children.Move(oldIndex, newIndex);
				NotifyChildMoved(child, oldIndex, newIndex);
			}
		}

		/// <summary>
		/// Removes a child from this item.
		/// </summary>
		/// <param name="child">The child to remove.</param>
		/// <returns>True if the child has been removed, false otherwise.</returns>
		private bool RemoveChild(TreeListViewItemVM child) {
			// Removing the items from the children list.
			if (_children.Remove(child)) {
				NotifyChildRemoved(child);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Removes the child at the given position.
		/// </summary>
		/// <param name="index">The position of the child to remove.</param>
		/// <returns>The removed item if any, null otherwise.</returns>
		private TreeListViewItemVM RemoveChildAtInternal(int index) {
			var child = _children.ElementAt(index);
			if (child != null && RemoveChild(child)) {
				return child;
			}

			return null;
		}

		/// <summary>
		/// Clears the children list.
		/// </summary>
		/// <param name="dispose">Flag indicating if the children must be disposed or not.</param>
		public void ClearChildren(bool dispose = true) {
			// Clearing the children list.
			while (_children.Count != 0) {
				// Processing Dispose will remove the item from the parent children list as well.
				var item = _children[0];
				item.DisableNotifyPropertyChangedEvent();
				item.ClearChildren(dispose);
				_children.RemoveAt(0);
				item.EnableNotifyPropertyChangedEvent();
				NotifyChildRemoved(item);

				if (dispose) {
					item.Dispose();
				}
			}
		}

		/// <summary>
		/// Add a child to the children list taking in account the comparer.
		/// </summary>
		/// <param name="child">The child to add.</param>
		private void AddOrganizedChild(TreeListViewItemVM child) {
			if (_comparerKeySelector != null) {
				var pair = _children.Select((value, index) => new {value = value, index = index})
					.FirstOrDefault(elt => _comparer.Compare(_comparerKeySelector(this, elt.value),
						_comparerKeySelector(this, child)) > 0);
				if (pair == null) {
					// Propagate visibility to children.
					child.IsVisible = IsVisible;
					_children.Add(child);
					NotifyChildAdded(child);
				}
				else {
					// Propagate visibility to children.
					child.IsVisible = IsVisible;
					_children.Insert(pair.index, child);
					NotifyChildAdded(child);
				}
			}
			else {
				//Propagate visibility to children.
				child.IsVisible = IsVisible;
				_children.Add(child);
				NotifyChildAdded(child);
			}
		}

		/// <summary>
		/// Insert a child to the children list taking in account the comparer.
		/// </summary>
		/// <param name="index">The index where the hild has to be inserted.</param>
		/// <param name="child">The child to insert.</param>
		private void InsertOrganizedChild(int index, TreeListViewItemVM child) {
			if (_comparerKeySelector != null) {
				AddOrganizedChild(child);
			}
			else {
				_children.Insert(index, child);
				NotifyChildAdded(child);
			}
		}

		/// <summary>
		/// Registers the children of this item on demand.
		/// </summary>
		private void RegisterChildren() {
			if (ChildrenRegistered == false) {
				IsInRegistrationMode = true;

				// If a binding has been registered, the synchronization must be done. 
				// The children loading process is no more handled by the user.
				if (_childrenBinding.Count != 0) {
					SynchronizeChildren();
				}
				else {
					InternalRegisterChildren();
				}

				IsInRegistrationMode = false;
				ChildrenRegistered = true;
			}
		}

		/// <summary>
		/// Registers the children into the given list on demand.
		/// </summary>
		protected virtual void InternalRegisterChildren() {
			// Nothing to do.
		}

		/// <summary>
		/// Synchronize the view model children with the owned object binded collection.
		/// </summary>
		private void SynchronizeChildren() {
			foreach (var childrenBinding in _childrenBinding) {
				var modelCollection = childrenBinding.Key as IList;
				SynchronizeChildren(modelCollection);
			}
		}

		/// <summary>
		/// Synchronize the view model children with the owned object binded collection.
		/// </summary>
		/// <param name="collection">The collection to synchronize</param>
		private void SynchronizeChildren(IList collection) {
			var c = collection as INotifyCollectionChanged;
			CreateChildViewModelDelegate creator = null;
			if (c != null) {
				if (_childrenBinding.ContainsKey(c)) {
					creator = _childrenBinding[c];
				}
			}

			if (collection != null && creator != null) {
				// Synchronizing the children.
				foreach (var model in collection) {
					// Creating the view model using the apropriate delegate.
					var itemViewModel = creator(model);
					if (itemViewModel != null) {
						AddChild(itemViewModel);
					}
				}
			}
		}

		/// <summary>
		/// Bind an expression of a property from the owned object to the Children property in the current view model.
		/// </summary>
		/// <param name="modelProperty">The property name of the owned model.</param>
		/// <param name="childViewModelType">The type of the view model to create.</param>
		protected void BindChildren(string modelProperty, Type childViewModelType) {
			// Using the native view model creator delegate.
			BindChildren(modelProperty,
				ownedObject => CreateChildViewModel(childViewModelType, ownedObject));
		}

		/// <summary>
		/// Bind an expression of a property from the owned object to the Children property in the current view model.
		/// </summary>
		/// <param name="modelProperty">The property name of the owned model.</param>
		/// <param name="creationDelegate">Specific child view model creation delegate.</param>
		protected void BindChildren(string modelProperty, CreateChildViewModelDelegate creationDelegate) {
			// Registering the binding.
			var collectionNotifier =
				GetCollectionFromPropertyName(UntypedOwnedObject, modelProperty);
			if (collectionNotifier != null) {
				// Binding the collections.
				_childrenBinding.Add(collectionNotifier, creationDelegate);
				collectionNotifier.CollectionChanged += OnOwnedObjectCollectionChanged;

				// Synchronize it if the item is already expanded.
				if (IsExpanded) {
					SynchronizeChildren(collectionNotifier as IList);
				}
			}
		}

		/// <summary>
		/// Unbind the property binded to the Children view model property.
		/// </summary>
		protected void UnbindChildren() {
			foreach (var childrenBinding in _childrenBinding) {
				// Unregister the notification handler.
				childrenBinding.Key.CollectionChanged -= OnOwnedObjectCollectionChanged;

				// Clearing the children.
				ClearChildren();
			}
			_childrenBinding.Clear();
		}

		/// <summary>
		/// Called each time a binded collection of the owned object gets changed to update the children.
		/// </summary>
		/// <param name="sender">The modified collection.</param>
		/// <param name="e">The event's arguments.</param>
		private void OnOwnedObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			Application.Current.Dispatcher.Invoke(new Action(() => {
				CreateChildViewModelDelegate creator = null;
				if (sender is INotifyCollectionChanged c) {
					if (_childrenBinding.ContainsKey(c)) {
						creator = _childrenBinding[c];
					}
				}

				if (sender is IList list) {
					// Update the binded children.
					switch (e.Action) {
						case NotifyCollectionChangedAction.Add: {
							var doExplicitAdd = true;
							var index = e.NewStartingIndex;
							if (ChildrenRegistered == false) {
								RegisterChildren();

								// Loading children will maybe apply the modifications.
								if (_children.Count == list.Count) {
									doExplicitAdd = false;
								}
							}

							if (doExplicitAdd && creator != null) {
								foreach (var model in e.NewItems) {
									// Creating the view model using the apropriate delegate.
									var itemViewModel = creator(model);
									if (itemViewModel != null) {
										if (index == _children.Count) {
											AddChild(itemViewModel);
										}
										else {
											InsertChild(index, itemViewModel);
										}
									}

									// Increment indices.
									++index;
								}
							}
						}

							break;

						case NotifyCollectionChangedAction.Remove: {
							var modelToRemove = new List<TreeListViewItemVM>();

							foreach (var viewModel in Children) {
								if (e.OldItems.Contains(viewModel.UntypedOwnedObject)) {
									modelToRemove.Add(viewModel);
								}
							}

							// ReSharper disable once UnusedVariable
							foreach (var model in modelToRemove) {
								var removeIndex = _children.IndexOf(model);
								RemoveChildAt(removeIndex);
							}
						}

							break;

						case NotifyCollectionChangedAction.Replace: {
							if (creator != null) {
								var currentItemIndex = 0;
								foreach (var newModel in e.NewItems) {
									// Get old model.
									var oldModel = e.OldItems[currentItemIndex];

									// Create the new Item
									var itemViewModel = creator(newModel);
									if (itemViewModel != null) {
										// Search for the removed item in the collection to update.
										TreeListViewItemVM viewModelToReplace = null;
										foreach (var viewModel in Children) {
											if (viewModel.UntypedOwnedObject == oldModel) {
												viewModelToReplace = viewModel;
												break;
											}
										}

										if (viewModelToReplace != null) {
											// Get the index of the removed item
											var replaceIndex = _children.IndexOf(viewModelToReplace);

											// Replace the old item by the new one
											RemoveChild(viewModelToReplace);
											InsertChild(replaceIndex, itemViewModel);
										}
									}

									// Increment indices.
									++currentItemIndex;
								}
							}
						}

							break;

						case NotifyCollectionChangedAction.Move: {
							var oldIndex = e.OldStartingIndex + e.OldItems.Count - 1;
							while (oldIndex > 0) {
								MoveChild(oldIndex, e.NewStartingIndex);
								oldIndex--;
							}
						}

							break;

						case NotifyCollectionChangedAction.Reset: {
							ClearChildren();
						}

							break;
					}
				}
			}));
		}

		/// <summary>
		/// Creates a new children as ATreeListViewItemViewModel of this item.
		/// </summary>
		/// <param name="modelViewType">The model view type.</param>
		/// <param name="ownedObject">The owned object of the new item.</param>
		/// <returns>The created item.</returns>
		private TreeListViewItemVM CreateChildViewModel(Type modelViewType, object ownedObject) {
			if
				((modelViewType.IsInterface == false)
				 && (modelViewType.IsAbstract == false)
				) {
				var @params = new object[1];
				@params[0] = ownedObject;
				try {
					return Activator.CreateInstance(modelViewType, @params) as TreeListViewItemVM;
				}
				catch
					(Exception /*Ex*/) {
					return null;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the collection from the given object (model or view model) property using reflexion.
		/// </summary>
		/// <remarks>The property can be a property path.</remarks>
		/// <param name="o">The object containing the property.</param>
		/// <param name="propertyName">The property name.</param>
		/// <returns>The collection if any, null otherwise.</returns>
		private INotifyCollectionChanged GetCollectionFromPropertyName(object o, string propertyName) {
			var pi = new List<PropertyInfo>();

			var properties = propertyName.Split(new char[] {'.'});
			pi.Add(o.GetType().GetProperty(properties[0]));
			for (var p = 1; p < properties.Count(); ++p) {
				if (pi.Last() != null) {
					pi.Add(pi.Last().PropertyType.GetProperty(properties[p]));
				}
				else {
					pi.Add(null);
				}
			}

			if
				((pi.Count > 0)
				 && (pi.Last() != null)
				) {
				// Get collection object from propery list
				var obj = o;
				foreach (var pInfo in pi) {
					obj = pInfo.GetValue(obj, null);
				}

				return obj as INotifyCollectionChanged;
			}

			return null;
		}

		/// <summary>
		/// Delegate called if the children collection is modified.
		/// </summary>
		/// <param name="sender">The modified collection.</param>
		/// <param name="eventArgs">The event arguments.</param>
		protected virtual void
			OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs) {
			// Updating the HasChildren property.
			NotifyPropertyChanged("HasChildren");

			// Notifying the user.
			if (ChildrenCollectionChanged != null) {
				ChildrenCollectionChanged(sender, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the visibility is changed.
		/// </summary>
		/// <param name="newValue">The new visibility.</param>
		protected override void OnVisibilityChanged(bool newValue) {
			// Updating the children visibility as well.
			foreach (var child in Children) {
				child.IsVisible = newValue;
			}
		}

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		public new TreeListViewItemVM<TModel> ToGeneric<TModel>() {
			return (this as TreeListViewItemVM<TModel>);
		}

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		ITreeListViewItemVM<TModel> ITreeListViewItemVM.ToGeneric<TModel>() {
			return (this as ITreeListViewItemVM<TModel>);
		}

		/// <summary>
		/// Unregisters from the underlying model.
		/// </summary>
		protected override void UnregisterFromModel() {
			// Dispose the base class.
			base.UnregisterFromModel();

			// Removing this object from the parent children list.
			if (Parent != null) {
				Parent._children.Remove(this);
			}

			// Disposing the children.
			while (_children.Count > 0) {
				_children[0].Dispose();
			}

			// Unbind the children.
			UnbindChildren();
		}

		/// <summary>
		/// Select this item and all the children items.
		/// </summary>
		/// <returns>The new selected items.</returns>
		ITreeListViewItemVM[] ITreeListViewItemVM.SelectAll() {
			var newSelectedItem = new List<ITreeListViewItemVM>();
			if (IsSelected == false) {
				IsSelected = true;
				if (IsSelected) {
					newSelectedItem.Add(this);
				}
			}

			foreach (ITreeListViewItemVM child in AllVisibleChildren) {
				newSelectedItem.AddRange(child.SelectAll());
			}

			return newSelectedItem.ToArray();
		}

		/// <summary>
		/// Unselect this item and all the children items.
		/// </summary>
		/// <returns>The old selected items.</returns>
		ITreeListViewItemVM[] ITreeListViewItemVM.UnSelectAll() {
			var oldSelectedItem = new List<ITreeListViewItemVM>();
			if (IsSelected) {
				IsSelected = false;
				if (IsSelected == false) {
					oldSelectedItem.Add(this);
				}
			}

			foreach (ITreeListViewItemVM child in AllVisibleChildren) {
				oldSelectedItem.AddRange(child.UnSelectAll());
			}

			return oldSelectedItem.ToArray();
		}

		/// <summary>
		/// Check this item and all its children.
		/// </summary>
		/// <returns>The new checked items.</returns>
		ITreeListViewItemVM[] ITreeListViewItemVM.CheckAll() {
			var newCheckedItem = new List<ITreeListViewItemVM>();
			if (IsChecked == false) {
				IsChecked = true;
				if (IsChecked) {
					newCheckedItem.Add(this);
				}
			}

			foreach (ITreeListViewItemVM child in AllVisibleChildren) {
				newCheckedItem.AddRange(child.CheckAll());
			}

			return newCheckedItem.ToArray();
		}

		/// <summary>
		/// Uncheck this item and all its children.
		/// </summary>
		/// <returns>The old checked items.</returns>
		ITreeListViewItemVM[] ITreeListViewItemVM.UncheckAll() {
			var oldCheckedItem = new List<ITreeListViewItemVM>();
			if (IsChecked) {
				IsChecked = false;
				if (IsChecked == false) {
					oldCheckedItem.Add(this);
				}
			}

			foreach (ITreeListViewItemVM child in AllVisibleChildren) {
				oldCheckedItem.AddRange(child.UncheckAll());
			}

			return oldCheckedItem.ToArray();
		}

		/// <summary>
		/// Method to call when new children are added to this view model.
		/// </summary>
		/// <param name="child">The child removed from the children list.</param>
		protected virtual void NotifyChildAdded(ITreeListViewItemVM child) {
			if (Parent != null) {
				Parent.NotifyChildAdded(child);
			}
		}

		/// <summary>
		/// Method to call when children are removed from this view model.
		/// </summary>
		/// <param name="child">The child added to the children list.</param>
		protected virtual void NotifyChildRemoved(ITreeListViewItemVM child) {
			if (Parent != null) {
				Parent.NotifyChildRemoved(child);
			}
		}

		/// <summary>
		/// Method to call when children are moved in this view model.
		/// </summary>
		/// <param name="child">The child added to the children list.</param>
		/// <param name="oldIndex">The old index of the item.</param>
		/// <param name="newIndex">THe new index of the item.</param>
		protected virtual void NotifyChildMoved(ITreeListViewItemVM child, int oldIndex, int newIndex) {
			if (Parent != null) {
				Parent.NotifyChildMoved(child, oldIndex, newIndex);
			}
		}

		/// <summary>
		/// Delegate called when the properties of this view model is modified.
		/// </summary>
		/// <param name="sender">The item view model event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		protected virtual void NotifyItemViewModelModified(object sender, PropertyChangedEventArgs eventArgs) {
			if (Parent != null) {
				Parent.NotifyItemViewModelModified(this, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the expanded state is changed.
		/// </summary>
		/// <param name="newValue">The new expanded state.</param>
		protected virtual void OnIsExpandedChanged(bool newValue) {
			// Nothing to do.
		}

		/// <summary>
		/// Returns the children view models owning the given object.
		/// </summary>
		/// <param name="ownedObject">The analysed owned object.</param>
		/// <param name="comparer">The owned object comparer used when searching for the view models.</param>
		/// <remarks>If the comparer is null, the comparison is made by reference.</remarks>
		/// <returns>The list of view models owning the object.</returns>
		public IEnumerable<ITreeListViewItemVM> GetViewModels(object ownedObject, IComparer comparer = null) {
			var foundViewModels = new List<ITreeListViewItemVM>();

			foreach (ITreeListViewItemVM child in Children) {
				// Checking the current child.
				if (comparer == null) {
					if (child.UntypedOwnedObject == ownedObject) {
						foundViewModels.Add(child);
					}
				}
				else {
					if (comparer.Compare(child.UntypedOwnedObject, ownedObject) == 0) {
						foundViewModels.Add(child);
					}
				}

				// Propagating the request.
				foundViewModels.AddRange(child.GetViewModels(ownedObject, comparer));
			}

			return foundViewModels.ToArray();
		}

		#region Sorter

		/// <summary>
		/// Sets a predefined sorter.
		/// </summary>
		/// <param name="key">The p key.</param>
		/// <param name="sortOrder">The p sort order.</param>
		public void EnableSort(SortKey key, SortOrder sortOrder) {
			if (sortOrder == SortOrder.Unsorted) {
				RemoveSorter();
			}
			else {
				switch (key) {
					case SortKey.DisplayString: {
						SetSorter(GetDisplayStringAsSortKey, new StringComparer(sortOrder));
					}
						break;

					case SortKey.ChildCount: {
						SetSorter(GetChildCountAsSortKey, new IntComparer(sortOrder));
					}
						break;
				}
			}
		}

		/// <summary>
		/// Gets the key sorter.
		/// </summary>
		/// <param name="parentViewModel">The parent view model.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>The display string of the view model.</returns>
		private static object GetDisplayStringAsSortKey(ITreeListViewItemVM parentViewModel,
			ITreeListViewItemVM viewModel) {
			return viewModel.DisplayString;
		}

		/// <summary>
		/// Gets the key sorter.
		/// </summary>
		/// <param name="parentViewModel">The parent view model.</param>
		/// <param name="viewModel">The view model.</param>
		/// <returns>The display string of the view model.</returns>
		private static object GetChildCountAsSortKey(ITreeListViewItemVM parentViewModel,
			ITreeListViewItemVM viewModel) {
			return viewModel.Children.Count();
		}

		/// <summary>
		/// A ascending string comparer.
		/// </summary>
		public class StringComparer : IComparer<object> {

			#region Fields

			/// <summary>
			/// The m order
			/// </summary>
			private readonly int _order = 1;

			#endregion // Fields.

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="StringComparer"/> class.
			/// </summary>
			/// <param name="sorterOrder">The sorter order.</param>
			public StringComparer(SortOrder sorterOrder) {
				if (sorterOrder == SortOrder.Descending) {
					_order = -1;
				}
			}

			#endregion // Constructors.

			#region Methods

			/// <summary>
			/// Compares two strings.
			/// </summary>
			/// <param name="first">The first argument.</param>
			/// <param name="second">The second argument.</param>
			/// <returns>string.Compare</returns>
			int IComparer<object>.Compare(object first, object second) {
				return _order * string.Compare(first.ToString(), second.ToString(),
					StringComparison.InvariantCultureIgnoreCase);
			}

			#endregion // Methods.

		}

		/// <summary>
		/// A ascending int comparer.
		/// </summary>
		public class IntComparer : IComparer<object> {

			#region Fields

			/// <summary>
			/// The m order
			/// </summary>
			private readonly int _order = 1;

			#endregion // Fields.

			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="StringComparer"/> class.
			/// </summary>
			/// <param name="sorterOrder">The sorter order.</param>
			public IntComparer(SortOrder sorterOrder) {
				if (sorterOrder == SortOrder.Descending) {
					_order = -1;
				}
			}

			#endregion // Constructors.

			#region Methods

			/// <summary>
			/// Compares two strings.
			/// </summary>
			/// <param name="first">The first argument.</param>
			/// <param name="second">The second argument.</param>
			/// <returns>string.Compare</returns>
			int IComparer<object>.Compare(object first, object second) {
				if (first is int && second is int) {
					var f = (int) first;
					var s = (int) second;
					return _order * f.CompareTo(s);
				}

				return 0;
			}

			#endregion // Methods.

		}

		#endregion // Sorter.

		#endregion // Methods.

		#region Inner classes

		/// <summary>
		/// This class defines a view model collection implemented to handle the item index in the 
		/// parent view model for performance purpose.
		/// </summary>
		private class ViewModelCollection : ObservableCollection<TreeListViewItemVM> {

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
				while (Count != 0) {
					RemoveAt(Count - 1);
				}
			}

			/// <summary>
			/// This method inserts an item.
			/// </summary>
			/// <param name="index">The insertion index.</param>
			/// <param name="item">The item to insert.</param>
			protected override void InsertItem(int index, TreeListViewItemVM item) {
				if (item == null) {
					throw new ArgumentNullException(nameof(item));
				}

				if (item.Parent != _owner) {
					// Removing the element from the old parent if any.
					if (item.Parent != null) {
						item.Parent._children.Remove(item);
					}

					// Updating the parenting info.
					item.Parent = _owner;
					item._index = index;
					item.IsVisible = item.Parent.IsVisible;

					// Loading children if not load on demand.
					if (item.LoadItemsOnDemand == false) {
						item.RegisterChildren();
					}

					// Updating the index of the item placed after the added one.
					for (var i = index; index < Count; index++) {
						this[i]._index++;
					}

					// Inserting the item.
					base.InsertItem(index, item);
				}
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
				if (item == null) {
					throw new ArgumentNullException(nameof(item));
				}

				RemoveAt(itemIndex);
				InsertItem(itemIndex, item);
			}

			#endregion // Methods.

		}

		#endregion // Inner classes.

	}

	/// <summary>
	/// This class defines a tree list view item view model using genericity to explicitly define the type of the owned object.
	/// </summary>
	/// <typeparam name="TModel">The type of the owned object.</typeparam>
	public abstract class TreeListViewItemVM<TModel> : TreeListViewItemVM, ITreeListViewItemVM<TModel> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListViewItemVM"/> class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		protected TreeListViewItemVM(TModel ownedObject)
			: base(ownedObject) { }

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the owned object.
		/// </summary>
		public TModel OwnedObject => (TModel) UntypedOwnedObject;

		#endregion // Properties.

	}

}