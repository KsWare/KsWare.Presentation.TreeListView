using System;
using System.Collections;
using System.Collections.Generic;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// This class defines the arguments of the selection changed event.
	/// </summary>
	public class SelectionChangedEventArgs : EventArgs {

		#region Fields

		/// <summary>
		/// Gets the list of added items compared from the previous selection.
		/// </summary>
		private readonly IEnumerable _addedItems;

		/// <summary>
		/// Gets the list of removed items compared from the previous selection.
		/// </summary>
		private readonly IEnumerable _removedItems;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class.
		/// </summary>
		/// <param name="addedItems">The added items from the previous selection.</param>
		/// <param name="removedItems">The removed items from the previous selection.</param>
		public SelectionChangedEventArgs(IEnumerable removedItems, IEnumerable addedItems) {
			_removedItems = removedItems;
			_addedItems = addedItems;
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the list of added items compared from the previous selection.
		/// </summary>
		public IEnumerable<ITreeListViewItemBaseVM> AddedItems {
			get {
				if
					(_addedItems != null) {
					foreach
						(ITreeListViewItemBaseVM item in _addedItems) {
						yield return item;
					}
				}
			}
		}

		/// <summary>
		/// Gets the list of removed items compared from the previous selection.
		/// </summary>
		public IEnumerable<ITreeListViewItemBaseVM> RemovedItems {
			get {
				if
					(_removedItems != null) {
					foreach
						(ITreeListViewItemBaseVM item in _removedItems) {
						yield return item;
					}
				}
			}
		}

		#endregion // Properties.

	}

}