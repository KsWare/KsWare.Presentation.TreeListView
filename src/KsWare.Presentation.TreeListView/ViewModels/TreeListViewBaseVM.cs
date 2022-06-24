using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace KsWare.Presentation.TreeListView.ViewModels {

	/// <summary>
	/// This class implements a base view model.
	/// </summary>
	public abstract class TreeListViewBaseVM : ITreeListViewBaseVM {

		#region Fields

		/// <summary>
		/// Stores the owned object.
		/// </summary>
		private object _untypedOwnedObject;

		/// <summary>
		/// Stores the flag indicating if the view model is disposed.
		/// </summary>
		private bool _disposed;

		/// <summary>
		/// Stores the visibility of the item.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		/// Stores the flag to know if the item is checked.
		/// </summary>
		private bool _isChecked;

		/// <summary>
		/// Stores the checking enabled state of the view model.
		/// </summary>
		private bool _isCheckingEnabled;

		/// <summary>
		/// Stores the tool tip of the item.
		/// </summary>
		private object _toolTip;

		/// <summary>
		/// Stores the bindings of property changed names between the ViewModel and an object
		/// that implements INotifyPropertyChanged (in most case the Model).
		/// For each INotifyPropertyChanged it gives a dictionary that associate a property name in this object
		/// to a list of properties in the AViewModel inherited object.
		/// e.g. with an entry ["Name", "DisplayString"] when the ViewModel (this) gets notified of the 
		/// Model's "Name" property change, it fires a propertyChangedEvent whose name is "DisplayString".
		/// </summary> 
		private readonly Dictionary<string, HashSet<string>> _propertiesBinding;

		/// <summary>
		/// Stores the flag indicating if the notify property changed event of the view model can be raised or not.
		/// </summary>
		private bool _isNotifyPropertyChangedEnable;

		/// <summary>
		/// Stores the flag indicating if the view model is busy.
		/// </summary>
		private bool _isBusy;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListViewBaseVM"/> class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		protected TreeListViewBaseVM(object ownedObject) {
			UntypedOwnedObject = ownedObject;
			_propertiesBinding = new Dictionary<string, HashSet<string>>();
			_isNotifyPropertyChangedEnable = true;
			_disposed = false;
			_isVisible = true;
			_isCheckingEnabled = true;
			_isChecked = false;
		}

		/// <summary>
		/// Destroys this instance.
		/// This destructor will run only if the Dispose method does not get called.
		/// It gives the base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~TreeListViewBaseVM() {
			Dispose(false);
		}

		#endregion // Constructors.

		#region Events

		/// <summary>
		/// Event raised when a property is modified.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Event raised when the dispose method has been called explicitly, that is not in the finalizer.
		/// </summary>
		public event Action Disposed;

		#endregion // Events.

		#region Properties

		/// <summary>
		/// Gets or sets the string displayed by default in the item in the information part.
		/// </summary>
		public virtual string DisplayString => string.Empty;

		/// <summary>
		/// Gets or sets the owned object.
		/// </summary>
		public object UntypedOwnedObject {
			get => _untypedOwnedObject;

			set {
				var oldObject = _untypedOwnedObject;
				var newObject = value;

				// Specific pre traitment.
				PreviewOwnedObjectChanged(oldObject, newObject);

				// Unregistering the property changed event.
				var oldOwnedObject = _untypedOwnedObject as INotifyPropertyChanged;
				if (oldOwnedObject != null) {
					oldOwnedObject.PropertyChanged -= OnOwnedObjectPropertyChanged;
				}

				_untypedOwnedObject = value;

				// Registering on the property changed event.
				var newOwnedObject = _untypedOwnedObject as INotifyPropertyChanged;
				if (newOwnedObject != null) {
					newOwnedObject.PropertyChanged += OnOwnedObjectPropertyChanged;
				}

				// Specific post traitment.
				OwnedObjectChanged(oldObject, newObject);
			}
		}

		/// <summary>
		/// Gets or sets the flag indicating the visibility of the view model.
		/// </summary>
		public virtual bool IsVisible {
			get => _isVisible;

			set {
				if (_isVisible != value) {
					_isVisible = value;
					OnVisibilityChanged(value);
					NotifyPropertyChanged("IsVisible");
				}
			}
		}

		/// <summary>
		/// Gets or sets the flag indicating if the item is checked or not.
		/// </summary>
		public bool IsChecked {
			get => _isChecked;
			set {
				if (_isChecked != value && IsCheckable && IsCheckingEnabled) {
					_isChecked = value;
					NotifyPropertyChanged(nameof(IsChecked));
				}
			}
		}

		/// <summary>
		/// Gets or sets the flags indicating if the view model checked state can be modified.
		/// </summary>
		public bool IsCheckingEnabled {
			get => _isCheckingEnabled;

			set {
				if (value != _isCheckingEnabled) {
					_isCheckingEnabled = value;
					NotifyPropertyChanged(nameof(IsCheckingEnabled));
				}
			}
		}

		/// <summary>
		/// Gets the flag indicating if the item is checkable.
		/// </summary>
		public virtual bool IsCheckable => false;

		/// <summary>
		/// Gets the ToolTip.
		/// </summary>
		public object ToolTip {
			get => _toolTip;

			set {
				if (_toolTip != value) {
					_toolTip = value;
					NotifyPropertyChanged("ToolTip");
				}
			}
		}

		/// <summary>
		/// Gets the icon to display in the item.
		/// </summary>
		public abstract ImageSource IconSource { get; }

		/// <summary>
		/// Gets the flag indicating if the view model is busy.
		/// </summary>
		public bool IsBusy {
			get => _isBusy;

			private set {
				if (_isBusy != value) {
					_isBusy = value;
					NotifyPropertyChanged("IsBusy");
				}
			}
		}

		/// <summary>
		/// Gets the flag indicating if the view model is disposed.
		/// </summary>
		public bool IsDisposed => _disposed;

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Method called when the owned object is going to be modified.
		/// </summary>
		/// <param name="previousOwnedObject">The previous owned object.</param>
		/// <param name="newOwnedObject">The new owned object.</param>
		protected virtual void PreviewOwnedObjectChanged(object previousOwnedObject, object newOwnedObject) {
			// Nothing to do.
		}

		/// <summary>
		/// Method called when the owned object has been modified.
		/// </summary>
		/// <param name="previousOwnedObject">The previous owned object.</param>
		/// <param name="newOwnedObject">The new owned object.</param>
		protected virtual void OwnedObjectChanged(object previousOwnedObject, object newOwnedObject) {
			// Nothing to do.
		}

		/// <summary>
		/// Delegate called when the visibility is changed.
		/// </summary>
		/// <param name="newValue">The new visibility.</param>
		protected virtual void OnVisibilityChanged(bool newValue) {
			// Nothing to do.
		}

		/// <summary>
		/// Bind an expression of a property from the owned object to an expression of a property in the current view model.
		/// </summary>
		/// <param name="modelProperty">The property name of the origin.</param>
		/// <param name="viewModelProperty">The property name of the destination</param>
		protected void BindProperty(string modelProperty, string viewModelProperty) {
			Debug.Assert(UntypedOwnedObject is INotifyPropertyChanged,
				"The model must implement INotifyPropertyChanged.");

			// Registering the binding.
			if (_propertiesBinding.ContainsKey(modelProperty) == false) {
				_propertiesBinding[modelProperty] = new HashSet<string>();
			}

			_propertiesBinding[modelProperty].Add(viewModelProperty);
		}

		/// <summary>
		/// Unbind an expression of a property from the owned object to an expression of a property in the current view model.
		/// </summary>
		/// <param name="modelProperty">The property name of the origin.</param>
		/// <param name="viewModelProperty">The property name of the destination</param>
		protected void UnbindProperty(string modelProperty, string viewModelProperty) {
			if (_propertiesBinding.ContainsKey(modelProperty)) {
				_propertiesBinding[modelProperty].Remove(viewModelProperty);
			}
		}

		/// <summary>
		/// Called each time a binded property of the owned object gets changed. 
		/// This method can only be called if OwnedObject implements INotifyPropertyChanged.
		/// </summary>
		/// <param name="sender">The modified owned object.</param>
		/// <param name="event">The event arguments.</param>
		private void OnOwnedObjectPropertyChanged(object sender, PropertyChangedEventArgs @event) {
			// Calling internal handler.
			OnOwnedObjectPropertyChangedInternal(@event);

			// Forward if in binding list.
			if (_propertiesBinding.ContainsKey(@event.PropertyName)) {
				foreach (var boundPropertyName in _propertiesBinding[@event.PropertyName]) {
					NotifyPropertyChanged(boundPropertyName);
				}
			}
		}

		/// <summary>
		/// Delegate called when a property of the owned object gets changed.
		/// </summary>
		/// <param name="event">The event arguments.</param>
		protected virtual void OnOwnedObjectPropertyChangedInternal(PropertyChangedEventArgs @event) {
			// Nothing to do by default.
		}

		/// <summary>
		/// Method called when a property is modified to notify the listner.
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		public void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null && _isNotifyPropertyChangedEnable) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Executed a background work for the current view model.
		/// </summary>
		/// <param name="start">The entry point of the work to do.</param>
		/// <param name="parameter">The background work parameter.</param>
		public void DoBackgroundWork(ParameterizedBackgroundWorkStart start, object parameter) {
			// Indicating the tree view is busy.
			var busyTask = new System.Threading.Tasks.Task(() => IsBusy = true);

			// Excuting the background task.
			var backgroundTask = busyTask.ContinueWith((antecedent) => start(this, parameter));

			// The tree view is not busy anymore.
			backgroundTask.ContinueWith((antecedent) => IsBusy = false);

			// Launching the task.
			busyTask.Start();
		}

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>The generic version of the item.</returns>
		public TlvBaseViewModel<TModel> ToGeneric<TModel>() {
			return (this as TlvBaseViewModel<TModel>);
		}

		/// <summary>
		/// Convert the item to the generic version.
		/// </summary>
		/// <typeparam name="TModel">The type of the owned object.</typeparam>
		/// <returns>
		/// The generic version of the item.
		/// </returns>
		ITreeListViewBaseVM<TModel> ITreeListViewBaseVM.ToGeneric<TModel>() {
			return (this as ITreeListViewBaseVM<TModel>);
		}

		/// <summary>
		/// Dispose this view model.
		/// </summary>
		public void Dispose() {
			Dispose(true);

#pragma warning disable 1587
			/// This object will be cleaned up by the Dispose method.
			/// Therefore, GC.SupressFinalize should be called to take this object off the finalization queue 
			/// and prevent finalization code for this object from executing a second time.
#pragma warning restore 1587

			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Unregisters from the underlying model.
		/// </summary>
		protected virtual void UnregisterFromModel() {
			// Invalidating owned object.
			_propertiesBinding.Clear();
			UntypedOwnedObject = null;
			ToolTip = null;
		}

		/// <summary>
		/// Dispose this view model.
		/// </summary>
		/// <param name="disposing">Flag indicating if the owned objects have to be cleaned as well.</param>
		private void Dispose(bool disposing) {
			DisableNotifyPropertyChangedEvent();

			if (_untypedOwnedObject != null && _untypedOwnedObject is INotifyPropertyChanged) {
				(_untypedOwnedObject as INotifyPropertyChanged).PropertyChanged -=
					OnOwnedObjectPropertyChanged;
			}

			if (_disposed == false) {
				// Free other state (managed objects) section.
				if (disposing) {
					NotifyDispose();
					UnregisterFromModel();
				}

				// Free your own state (unmanaged objects) section.

				_disposed = true;
			}


			EnableNotifyPropertyChangedEvent();
		}

		/// <summary>
		/// Notifies a dispose has been made.
		/// </summary>
		private void NotifyDispose() {
			if (Disposed != null) {
				Disposed();
			}
		}

		/// <summary>
		/// Locks the notify property changed event raising.
		/// </summary>
		protected void DisableNotifyPropertyChangedEvent() {
			_isNotifyPropertyChangedEnable = false;
		}

		/// <summary>
		/// Enables the notify property changed event raising.
		/// </summary>
		protected void EnableNotifyPropertyChangedEvent() {
			_isNotifyPropertyChangedEnable = true;
		}

		#endregion // Methods.

	}

	/// <summary>
	/// This class implements a generic view model.
	/// </summary>
	/// <typeparam name="TModel">The type of the owned object.</typeparam>
	public abstract class TlvBaseViewModel<TModel> : TreeListViewBaseVM, ITreeListViewBaseVM<TModel> {

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the AViewModel class.
		/// </summary>
		/// <param name="ownedObject">The owned object.</param>
		protected TlvBaseViewModel(TModel ownedObject)
			: base(ownedObject) { }

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the owned object if any.
		/// </summary>
		public TModel OwnedObject => (TModel) UntypedOwnedObject;

		#endregion // Properties.

	}

}