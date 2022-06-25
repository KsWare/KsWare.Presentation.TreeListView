using System.Collections.Generic;

namespace KsWare.Presentation.TreeListView.ViewModels;

partial class TreeListViewItemVM {

	/// <summary>
	/// A ascending int comparer.
	/// </summary>
	public class IntComparer : IComparer<object> {

		/// <summary>
		/// The m order
		/// </summary>
		private readonly int _order = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringComparer"/> class.
		/// </summary>
		/// <param name="sortOrder">The sort order.</param>
		public IntComparer(SortOrder sortOrder) {
			if (sortOrder == SortOrder.Descending) _order = -1;
		}

		/// <summary>
		/// Compares two strings.
		/// </summary>
		/// <param name="a">The first argument.</param>
		/// <param name="b">The second argument.</param>
		/// <returns>string.Compare</returns>
		int IComparer<object>.Compare(object a, object b) {
			if (a is not int intA || b is not int intB) return 0;
			return _order * intA.CompareTo(intB);

		}

	}

}