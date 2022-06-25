using System;
using System.Collections.Generic;

namespace KsWare.Presentation.TreeListView.ViewModels;

public abstract partial class TreeListViewItemVM {
	
	/// <summary>
	/// A ascending string comparer.
	/// </summary>
	public class StringComparer : IComparer<object> {

		/// <summary>
		/// The order
		/// </summary>
		private readonly int _order = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringComparer"/> class.
		/// </summary>
		/// <param name="sortOrder">The sort order.</param>
		public StringComparer(SortOrder sortOrder) {
			if (sortOrder == SortOrder.Descending) _order = -1;
		}

		/// <summary>
		/// Compares two strings.
		/// </summary>
		/// <param name="a">The first argument.</param>
		/// <param name="b">The second argument.</param>
		/// <returns>string.Compare</returns>
		int IComparer<object>.Compare(object a, object b) {
			return _order * string.Compare(a?.ToString(), b?.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}
	}

}