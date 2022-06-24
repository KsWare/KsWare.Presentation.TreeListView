using System.Windows;
using System.Windows.Media;

namespace KsWare.Presentation.TreeListView.Internal.Extensions {

	/// <summary>
	/// Class extending the <see cref="System.Windows.DependencyObject"/> class.
	/// </summary>
	public static class DependencyObjectExtensions {

		#region Methods

		/// <summary>
		/// This function is used to look for an ancestor of a given type defined in a DataTemplate in a XAML.
		/// </summary>
		/// <typeparam name="TAncestor">The type of the object requested.</typeparam>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns>The object retrieved if any, NULL otherwise.</returns>
		public static TAncestor FindVisualParent<TAncestor>(this DependencyObject dependencyObject)
			where TAncestor : class {
			var target = dependencyObject;
			do {
				target = VisualTreeHelper.GetParent(target);
			} while (target != null && (target is TAncestor) == false);

			return target as TAncestor;
		}

		#endregion // Methods.

	}

}