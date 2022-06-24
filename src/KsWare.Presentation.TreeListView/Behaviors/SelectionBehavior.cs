using System.Linq;
using System.Windows.Input;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Behaviors {

	/// <summary>
	/// Class handling the user selection behavior of an extended list view.
	/// </summary>
	public class SelectionBehavior {

		#region Fields

		/// <summary>
		/// Stores the handled list view.
		/// </summary>
		private readonly ExtendedListView _parent;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionBehavior"/> class.
		/// </summary>
		/// <param name="parent">The behavior's parent.</param>
		public SelectionBehavior(ExtendedListView parent) {
			_parent = parent;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Delegate called when the mouse left button is down on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemMouseLeftButtonDown(ITreeListViewItemVM item,
			MouseButtonEventArgs eventArgs) {
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
				if (_parent.SelectionModel.SelectionMode != TreeSelectionMode.MultiSelection) return;
				if (!item.CanBeSelected) return;
				if (item.IsSelected == false)
					_parent.SelectionModel.AddToSelection(item);
				else
					_parent.SelectionModel.Unselect(item, false);
			}
			else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
				if (_parent.SelectionModel.SelectionMode != TreeSelectionMode.MultiSelection) return;
				if (!item.CanBeSelected) return;
				if (_parent.SelectionModel.Anchor == null)
					_parent.SelectionModel.Select(item);
				else
					_parent.SelectionModel.SelectRange(_parent.SelectionModel.Anchor, item);
			}
			else {
				// Default behavior.
				if (_parent.SelectionModel.SelectionMode == TreeSelectionMode.SingleSelection) {
					if (item.CanBeSelected) {
						_parent.SelectionModel.Select(item);
					}
				}
				else if (_parent.SelectionModel.SelectionMode == TreeSelectionMode.MultiSelection) {
					if (item.CanBeSelected) {
						_parent.SelectionModel.Select(item);
					}
				}
			}
		}

		/// <summary>
		/// Delegate called when the mouse right button is down on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemMouseRightButtonDown(ITreeListViewItemVM item,
			MouseButtonEventArgs eventArgs) {
			if (item.CanBeSelected) {
				if (item.IsSelected == false) {
					_parent.SelectionModel.Select(item);
				}
			}
			else {
				_parent.SelectionModel.UnselectAll();
			}
		}

		/// <summary>
		/// Delegate called when the mouse left button is up on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemMouseLeftButtonUp(ITreeListViewItemVM item,
			MouseButtonEventArgs eventArgs) {
			if (Keyboard.Modifiers == ModifierKeys.None
			    && item.CanBeSelected) {
				if (item.IsSelected && _parent.SelectionModel.SelectedViewModels.Count() > 1) {
					_parent.SelectionModel.Select(item);
				}
			}
		}

		/// <summary>
		/// Delegate called when the mouse right button is up on an item.
		/// </summary>
		/// <param name="item">The clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemMouseRightButtonUp(ITreeListViewItemVM item,
			MouseButtonEventArgs eventArgs) {
			// Nothing to do.
		}

		/// <summary>
		/// Delegate called when a click is performed in the tree list view.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnTreeListViewMouseDown(MouseButtonEventArgs eventArgs) {
			_parent.SelectionModel.UnselectAll();
		}

		#endregion // Methods.

	}

}