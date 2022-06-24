using System.ComponentModel;
using System.Windows.Media;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// This interface is the root node of a hierarchical view model.
	/// </summary>
	public interface ITreeListViewRootItemVM : ITreeListViewItemVM {

		#region Events

		/// <summary>
		/// Event called when some items are added.
		/// </summary>
		event TreeViewEventHandler ItemViewModelsAdded;

		/// <summary>
		/// Event called when some items are removed.
		/// </summary>
		event TreeViewEventHandler ItemViewModelsRemoved;

		/// <summary>
		/// Event called when some items are removed.
		/// </summary>
		event TreeViewItemEventHandler ItemViewModelModified;

		/// <summary>
		/// Delegate called when an item is moved.
		/// </summary>
		event TreeViewItemMovedEventHandler ItemViewModelMoved;

		#endregion // Events.

		#region Methods

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		new ITreeListViewRootItemVM<TModel> ToGeneric<TModel>();

		#endregion // Methods.

	}

	/// <summary>
	/// This interface defines the generic root view model interface.
	/// </summary>
	/// <typeparam name="TModel">The type of the owned object.</typeparam>
	public interface ITreeListViewRootItemVM<TModel> : ITreeListViewRootItemVM {

		#region Properties

		/// <summary>
		/// Gets or sets the model associated to this view model.
		/// </summary>
		TModel Model { get; set; }

		#endregion // Properties.

	}

	/// <summary>
	/// This class defines the root of the view model to give to the tree list view.
	/// </summary>
	/// <typeparam name="TModel">The type of the owned object.</typeparam>
	public abstract class TreeListViewRootItemVM<TModel> : TreeListViewItemVM, ITreeListViewRootItemVM<TModel> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListViewRootItemVM{TModel}"/> class.
		/// </summary>
		/// TODO Edit XML Comment Template for #ctor
		protected TreeListViewRootItemVM()
			: base(null) {
			ChildrenRegistered = true;
			IsExpanded = true;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets or sets the model associated to this view model.
		/// </summary>
		public virtual TModel Model {
			get => (TModel) UntypedOwnedObject;

			set {
				UntypedOwnedObject = value;
				NotifyPropertyChanged("Model");
			}
		}

		/// <summary>
		/// Gets the flag indicating if the items are loaded on demand.
		/// </summary>
		protected override bool LoadItemsOnDemand => false;

		/// <summary>
		/// Gets the icon to display in the item.
		/// </summary>
		public sealed override ImageSource IconSource => null;

		#endregion // Properties.

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
		/// Event called when some items are modified.
		/// </summary>
		public event TreeViewItemEventHandler ItemViewModelModified;

		/// <summary>
		/// Delegate called when an item is moved.
		/// </summary>
		public event TreeViewItemMovedEventHandler ItemViewModelMoved;

		#endregion // Events.

		#region Methods

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="UModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		ITreeListViewRootItemVM<TUModel> ITreeListViewRootItemVM.ToGeneric<TUModel>() {
			return (this as ITreeListViewRootItemVM<TUModel>);
		}

		/// <summary>
		/// Method to call when new children are added to this view model.
		/// </summary>
		/// <param name="child">The child removed from the children list.</param>
		protected sealed override void NotifyChildAdded(ITreeListViewItemVM child) {
			if (ItemViewModelsAdded != null) {
				ItemViewModelsAdded(this, new ITreeListViewItemVM[] {child});
			}
		}

		/// <summary>
		/// Method to call when children are removed from this view model.
		/// </summary>
		/// <param name="child">The child added to the children list.</param>
		protected sealed override void NotifyChildRemoved(ITreeListViewItemVM child) {
			if (ItemViewModelsRemoved != null) {
				ItemViewModelsRemoved(this, new ITreeListViewItemVM[] {child});
			}
		}

		/// <summary>
		/// Method to call when children are removed from this view model.
		/// </summary>
		/// <param name="child">The child added to the children list.</param>
		/// <param name="oldIndex">The old index of the item.</param>
		/// <param name="newIndex">THe new index of the item.</param>
		protected sealed override void
			NotifyChildMoved(ITreeListViewItemVM child, int oldIndex, int newIndex) {
			if (ItemViewModelMoved != null) {
				ItemViewModelMoved(this, child, oldIndex, newIndex);
			}
		}

		/// <summary>
		/// Delegate called when the properties of this view model is modified.
		/// </summary>
		/// <param name="sender">The item view model event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		protected sealed override void
			NotifyItemViewModelModified(object sender, PropertyChangedEventArgs eventArgs) {
			if (ItemViewModelModified != null) {
				ItemViewModelModified(sender, eventArgs);
			}
		}

		#endregion // Methods.

	}

}