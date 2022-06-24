using System.Linq;
using KsWare.Presentation.TreeListView.Controls;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Behaviors {

	/// <summary>
	/// Class handling the user selection behavior of an extended list view.
	/// </summary>
	public class ExpandBehavior {

		#region Fields

		/// <summary>
		/// Stores the handled list view.
		/// </summary>
		private readonly ExtendedListView _parent;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ExpandBehavior"/> class.
		/// </summary>
		/// <param name="parent">The behavior's parent.</param>
		public ExpandBehavior(ExtendedListView parent) {
			_parent = parent;
		}

		#endregion // Constructors.

		#region Methods

		/// <summary>
		/// Delegate called when a key is pressed when the item get the focus.
		/// </summary>
		/// <param name="item">The key up item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemKeyUp(ITreeListViewItemVM item, System.Windows.Input.KeyEventArgs eventArgs) {
			if (eventArgs.KeyboardDevice.Modifiers != System.Windows.Input.ModifierKeys.None) return;
			switch (eventArgs.Key) {
				// Handling the selection when right arrow key is pressed.
				case System.Windows.Input.Key.Right: {
					if (item == null || !item.HasChildren) return;
					if (item.IsExpanded) {
						// If not expanded, just expand it.
						_parent.ExpandModel.SetIsExpanded(item, true);
					}
					else {
						// Otherwise, selecting the first child if any.
						var firstChild = item.Children.FirstOrDefault();
						if (firstChild != null) {
							_parent.ScrollIntoView(firstChild, true);
						}
					}
					break;
				}
				// Handling the selection when left arrow key is pressed.
				case System.Windows.Input.Key.Left when _parent.SelectionModel.SelectedViewModels.Count() > 1: {
					// Selecting the first element with no selected parent.
					var foundItem =
						_parent.SelectionModel.SelectedViewModels.FirstOrDefault(item =>
							(item.Parent == null) || (item.Parent.IsSelected == false));
					if (foundItem != null) {
						_parent.ScrollIntoView(foundItem, true);
					}
					break;
				}
				case System.Windows.Input.Key.Left: {
					if (item == null) return;
					// This item is the only one selected.
					if (item.IsExpanded) {
						_parent.ExpandModel.SetIsExpanded(item, false);
					}
					else if (item.Parent != null && item.Parent is ITreeListViewRootItemVM == false) {
						_parent.ScrollIntoView(item.Parent, true);
					}
					break;
				}
			}
		}

		/// <summary>
		/// Delegate called when the item expander gets checked.
		/// </summary>
		/// <param name="item">The checked item.</param>
		public void OnItemExpanderChecked(ITreeListViewItemVM item) {
			_parent.ExpandModel.SetIsExpanded(item, true);
		}

		/// <summary>
		/// Delegate called when the item expander gets unchecked.
		/// </summary>
		/// <param name="item">The unchecked item.</param>
		public void OnItemExpanderUnchecked(ITreeListViewItemVM item) {
			_parent.ExpandModel.SetIsExpanded(item, false);
		}

		/// <summary>
		/// Delegate called when the mouse double clicked on this item.
		/// </summary>
		/// <param name="item">The double clicked item.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void OnItemMouseDoubleClicked(ITreeListViewItemVM item,
			System.Windows.Input.MouseButtonEventArgs eventArgs) {
			_parent.ExpandModel.ToggleExpand(item);
		}

		#endregion // Methods.

	}

}