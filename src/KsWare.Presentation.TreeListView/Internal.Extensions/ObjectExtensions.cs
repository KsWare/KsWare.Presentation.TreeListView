using System.Reflection;

namespace KsWare.Presentation.TreeListView.Internal.Extensions {

	/// <summary>
	/// Class extending the <see cref="System.Object"/> class.
	/// </summary>
	public static class ObjectExtensions {

		#region Properties

		/// <summary>
		/// Returns the value of the property of a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TValue">The type of the property value.</typeparam>
		/// <param name="object">The reflected object.</param>
		/// <param name="propertyName">The property name.</param>
		/// <returns>The property value.</returns>
		public static TValue GetPropertyValue<TObject, TValue>(this TObject @object, string propertyName) {
			return (TValue) typeof(TObject).InvokeMember(propertyName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance, null,
				@object, new object[] { });
		}

		/// <summary>
		/// Sets the value of the property of a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TValue">The type of the property value.</typeparam>
		/// <param name="object">The reflected object.</param>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The new property value.</param>
		public static void
			SetPropertyValue<TObject, TValue>(this TObject @object, string propertyName, TValue value) {
			typeof(TObject).InvokeMember(propertyName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null,
				@object, new object[] {value});
		}

		/// <summary>
		/// Returns the value of the field of a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TValue">The type of the property value.</typeparam>
		/// <param name="object">The reflected object.</param>
		/// <param name="fieldName">The field name.</param>
		/// <returns>The property value.</returns>
		public static TValue GetFieldValue<TObject, TValue>(this TObject @object, string fieldName) {
			return (TValue) typeof(TObject).InvokeMember(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null,
				@object, new object[] { });
		}

		/// <summary>
		/// Calls a method on a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TReturnValue">The type of the returned value.</typeparam>
		/// <param name="object">The reflected object.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="parameters">The method parameters.</param>
		/// <returns>The method returned value.</returns>
		public static TReturnValue CallMethod<TObject, TReturnValue>(this TObject @object, string methodName,
			params object[] parameters) {
			return (TReturnValue) typeof(TObject).InvokeMember(methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null,
				@object, parameters);
		}

		/// <summary>
		/// Calls a method on a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <param name="object">The reflected object.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="parameters">The method parameters.</param>
		public static void CallMethod<TObject>(this TObject @object, string methodName, params object[] parameters) {
			typeof(TObject).InvokeMember(methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null,
				@object, parameters);
		}

		#endregion // Properties.

	}

}