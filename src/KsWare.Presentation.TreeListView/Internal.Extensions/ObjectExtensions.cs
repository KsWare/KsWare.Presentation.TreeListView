using System.Linq.Expressions;
using System;
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
		/// <param name="obj">The reflected object.</param>
		/// <param name="propertyName">The property name.</param>
		/// <returns>The property value.</returns>
		public static TValue GetPropertyValue<TObject, TValue>(this TObject obj, string propertyName) {
			return (TValue) typeof(TObject).InvokeMember(propertyName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance, null,
				obj, new object[] { });
		}

		/// <summary>
		/// Sets the value of the property of a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TValue">The type of the property value.</typeparam>
		/// <param name="obj">The reflected object.</param>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The new property value.</param>
		public static void SetPropertyValue<TObject, TValue>(this TObject obj, string propertyName, TValue value) {
			// typeof(TObject).InvokeMember(propertyName,
			// 	BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance,
			//	null, obj, new object[] {value});

			var type = typeof(TObject);
			var pi = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			pi.SetValue(obj,value);
		}

		/// <summary>
		/// Returns the value of the field of a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TValue">The type of the property value.</typeparam>
		/// <param name="obj">The reflected object.</param>
		/// <param name="fieldName">The field name.</param>
		/// <returns>The property value.</returns>
		public static TValue GetFieldValue<TObject, TValue>(this TObject obj, string fieldName) {
			return (TValue) typeof(TObject).InvokeMember(fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null,
				obj, new object[] { });
		}

		/// <summary>
		/// Calls a method on a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <typeparam name="TReturnValue">The type of the returned value.</typeparam>
		/// <param name="obj">The reflected object.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="parameters">The method parameters.</param>
		/// <returns>The method returned value.</returns>
		public static TReturnValue CallMethod<TObject, TReturnValue>(this TObject obj, string methodName,
			params object[] parameters) {
			return (TReturnValue) typeof(TObject).InvokeMember(methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null,
				obj, parameters);
		}

		/// <summary>
		/// Calls a method on a given object using reflexion.
		/// </summary>
		/// <typeparam name="TObject">The type of the reflected object.</typeparam>
		/// <param name="obj">The reflected object.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="parameters">The method parameters.</param>
		public static void CallMethod<TObject>(this TObject obj, string methodName, params object[] parameters) {
			typeof(TObject).InvokeMember(methodName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null,
				obj, parameters);
		}

		#endregion // Properties.

		public static Func<T,R> GetFieldAccessor<T,R>(string fieldName) { 
			var param = Expression.Parameter (typeof(T),"arg");  
			var member = Expression.Field(param, fieldName);   
			var lambda = Expression.Lambda(typeof(Func<T,R>), member, param);   
			var compiled = (Func<T,R>)lambda.Compile(); 
			return compiled; 
		}

		public static Func<T,R> GetPropertyAccessor<T,R>(string propertyName) { 
			var param = Expression.Parameter (typeof(T),"arg");  
			var member = Expression.Property(param, propertyName);   
			var lambda = Expression.Lambda(typeof(Func<T,R>), member, param);   
			var compiled = (Func<T,R>)lambda.Compile(); 
			return compiled; 
		}

	}

}