using System;
using System.Collections.Generic;

namespace KsWare.Presentation.TreeListView.Internal.Extensions {

	/// <summary>
	/// Class extending the <see cref="System.Collections.IEnumerable"/> class.
	/// </summary>
	public static class EnumerableExtensions {

		#region Methods

		/// <summary>
		/// Method applying a function to all elements in an enumerable object such as a list.
		/// </summary>
		/// <typeparam name="TElement">The type of element</typeparam>
		/// <param name="enumerable">the source</param>
		/// <param name="action">The action to apply on all those elements</param>
		public static void ForEach<TElement>(this IEnumerable<TElement> enumerable, Action<TElement> action) {
			if (enumerable == null || action == null) {
				throw new NullReferenceException();
			}

			// Process.
			foreach (var element in enumerable) {
				action(element);
			}
		}

		#endregion // Methods.

	}

}