using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using KsWare.Presentation.TreeListView.Converters;
using KsWare.Presentation.TreeListView.Internal.Extensions;
using KsWare.Presentation.TreeListView.ViewModels;

namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// This object represents a tree list view item.
	/// </summary>
	[TemplatePart(Name = PartDecoratorsContainer, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = PartExpander, Type = typeof(ToggleButton))]
	[TemplatePart(Name = PartCheckBox, Type = typeof(CheckBox))]
	[TemplatePart(Name = PartIcon, Type = typeof(Image))]
	public class TreeListViewItem : ListViewItem {

		#region Dependencies

		/// <summary>
		/// Identifies the IsExpanded dependency property.
		/// </summary>
		public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded",
			typeof(bool), typeof(TreeListViewItem), new FrameworkPropertyMetadata(false));

		/// <summary>
		/// Identifies the Group dependency property.
		/// </summary>
		public static readonly DependencyProperty GroupProperty = DependencyProperty.Register("Group", typeof(object),
			typeof(TreeListViewItem), new FrameworkPropertyMetadata(string.Empty));

		/// <summary>
		/// Identifies the GrouDataTemplate dependency property.
		/// </summary>
		public static readonly DependencyProperty GroupDataTemplateProperty =
			DependencyProperty.Register("GroupDataTemplate", typeof(DataTemplate),
				typeof(TreeListViewItem), new FrameworkPropertyMetadata(null));

		/// <summary>
		/// Identifies the IsGroup dependency property.
		/// </summary>
		public static readonly DependencyProperty IsGroupProperty = DependencyProperty.Register("IsGroup", typeof(bool),
			typeof(TreeListViewItem), new FrameworkPropertyMetadata(false));

		#endregion // Dependencies.

		#region Fields

		/// <summary>
		/// Name of the parts that have to be in the control template.
		/// </summary>
		private const string PartDecoratorsContainer = "PART_DecoratorsContainer";

		private const string PartExpander = "PART_Expander";
		private const string PartCheckBox = "PART_CheckBox";
		private const string PartIcon = "PART_Icon";

		/// <summary>
		/// Stores the control containing the items decorators.
		/// </summary>
		private FrameworkElement _decoratorsContainer;

		/// <summary>
		/// Stores the object responsible for expending the item.
		/// </summary>
		private ToggleButton _expander;

		/// <summary>
		/// Stores the check box of the item.
		/// </summary>
		private CheckBox _checkBox;

		/// <summary>
		/// Stores the image responsible for displaying the icon of the item.
		/// </summary>
		private Image _image;

		/// <summary>
		/// Stores the parent grid view ensuring the item can be unregister from Columns.CollectionChanged when removed.
		/// </summary>
		private ExtendedGridView _parentGridView;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="TreeListViewItem"/> class.
		/// </summary>
		static TreeListViewItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListViewItem),
				new FrameworkPropertyMetadata(typeof(TreeListViewItem)));
			MultiColumnDefaultStyleKey =
				new ComponentResourceKey(typeof(TreeListViewItem), "MultiColumnDefaultStyleKey");
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the parent list view.
		/// </summary>
		internal TreeListView ParentTreeListView => this.FindVisualParent<TreeListView>();

		/// <summary>
		/// Gets or sets the view model attached to this item.
		/// </summary>
		public ITreeListViewItemVM ViewModel => Content as ITreeListViewItemVM;

		/// <summary>
		/// Gets or sets the flag indicating if the item is expanded or not.
		/// </summary>
		public bool IsExpanded {
			get => (bool) GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		/// <summary>
		/// Gets or sets the group object.
		/// </summary>
		public object Group {
			get => GetValue(GroupProperty);
			set => SetValue(GroupProperty, value);
		}

		/// <summary>
		/// Gets or sets the group data template.
		/// </summary>
		public DataTemplate GrouDataTemplate {
			get => (DataTemplate) GetValue(GroupDataTemplateProperty);
			set => SetValue(GroupDataTemplateProperty, value);
		}

		/// <summary>
		/// Gets or sets the flag indicating if the item is a group or not.
		/// </summary>
		public bool IsGroup {
			get => (bool) GetValue(IsGroupProperty);
			set => SetValue(IsGroupProperty, value);
		}

		/// <summary>
		/// Gets the item style key when the tree is in multi column mode.
		/// </summary>
		public static ResourceKey MultiColumnDefaultStyleKey { get; private set; }

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Called when the control template is applied.
		/// </summary>
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();

			// Getting the parts from the control template.
			_decoratorsContainer = GetTemplateChild(PartDecoratorsContainer) as FrameworkElement;
			_expander = GetTemplateChild(PartExpander) as ToggleButton;
			_checkBox = GetTemplateChild(PartCheckBox) as CheckBox;
			_image = GetTemplateChild(PartIcon) as Image;

			if
				((_expander == null)
				 || (_decoratorsContainer == null)
				 || (_checkBox == null)
				 || (_image == null)
				) {
				throw new Exception("TreeListViewItem control template not correctly defined.");
			}

			// Updating the decorator (icon + expander) clip region.
			_decoratorsContainer.Clip = GetDecoratorsCliRegion();

			// Updating the bindings.
			UpdateBindings(null, ViewModel);
		}

		/// <summary>
		/// Method called when the control content changed.
		/// </summary>
		/// <param name="oldContent">The previous content.</param>
		/// <param name="newContent">The new content.</param>
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);

			// Updating the bindings.
			UpdateBindings(oldContent as ITreeListViewItemVM, newContent as ITreeListViewItemVM);
		}

		/// <summary>
		/// Registers to the grid columns event.
		/// </summary>
		private void RegisterToGridColumnsEvents() {
			if (ParentTreeListView == null || ParentTreeListView.InnerListView == null) {
				return;
			}

			// Registers on the column changed event to update the decorator clip region.
			_parentGridView = ParentTreeListView.InnerListView.View;
			if (_parentGridView != null) {
				// Assure you remove a previously registered delegate if passing twice with non null model.
				_parentGridView.Synchronized -= OnGridViewSynchronized;
				_parentGridView.Synchronized += OnGridViewSynchronized;
			}
		}

		/// <summary>
		/// Registers to the container events.
		/// </summary>
		private void RegisterToContainerEvents() {
			if (_decoratorsContainer != null) {
				// Assure you remove a previously registered delegate if passing twice with non null model.
				_decoratorsContainer.SizeChanged -= OnDecoratorsContainerSizeChanged;
				_decoratorsContainer.SizeChanged += OnDecoratorsContainerSizeChanged;
			}
		}

		/// <summary>
		/// Registers on the expander events.
		/// </summary>
		private void RegisterToExpanderEvents() {
			if (_expander != null) {
				_expander.Checked -= OnExpanderChecked;
				_expander.Unchecked -= OnExpanderUnchecked;
				_expander.Checked += OnExpanderChecked;
				_expander.Unchecked += OnExpanderUnchecked;
			}
		}

		/// <summary>
		/// Registers to the check box events.
		/// </summary>
		private void RegisterToCheckBoxEvents() {
			if (_expander != null) {
				_checkBox.Checked -= OnCheckBoxChecked;
				_checkBox.Unchecked -= OnCheckBoxUnchecked;
				_checkBox.Checked += OnCheckBoxChecked;
				_checkBox.Unchecked += OnCheckBoxUnchecked;
			}
		}

		/// <summary>
		/// Unregisters from the grid columns events.
		/// </summary>
		private void UnregisterFromGridColumnsEvents() {
			if (_parentGridView != null) {
				_parentGridView.Synchronized -= OnGridViewSynchronized;
				_parentGridView = null;
			}
		}

		/// <summary>
		/// Unregisters to the container event.
		/// </summary>
		private void UnregisterFromContainerEvents() {
			if (_decoratorsContainer != null) {
				_decoratorsContainer.SizeChanged -= OnDecoratorsContainerSizeChanged;
			}
		}

		/// <summary>
		/// Unregisters from the expander events.
		/// </summary>
		private void UnregisterFromExpanderEvents() {
			if (_expander != null) {
				_expander.Checked -= OnExpanderChecked;
				_expander.Unchecked -= OnExpanderUnchecked;
			}
		}

		/// <summary>
		/// Unregisters from the check box events.
		/// </summary>
		private void UnregisterFromCheckBoxEvents() {
			if (_checkBox != null) {
				_checkBox.Checked -= OnCheckBoxChecked;
				_checkBox.Unchecked -= OnCheckBoxUnchecked;
			}
		}

		/// <summary>
		/// Updates the bindings of the view.
		/// </summary>
		/// <param name="oldViewModel">The old view model.</param>
		/// <param name="newViewModel">The new view model.</param>
		private void UpdateBindings(ITreeListViewItemVM oldViewModel,
			ITreeListViewItemVM newViewModel) {
			if (oldViewModel != null) {
				// Unregistering from the events.
				UnregisterFromExpanderEvents();
				UnregisterFromCheckBoxEvents();
				UnregisterFromContainerEvents();
				UnregisterFromGridColumnsEvents();

				// Clear reference to group property.
				Group = null;
			}

			if (newViewModel != null && newViewModel.IsDisposed == false) {
				// Registering to uncontextual events.
				RegisterToGridColumnsEvents();
				RegisterToContainerEvents();

				// Binding the IsSelected property.
				var isSelectedBinding = new Binding("IsSelected");
				isSelectedBinding.Source = newViewModel;
				isSelectedBinding.Mode = BindingMode.OneWay;
				SetBinding(IsSelectedProperty, isSelectedBinding);

				// Binding the Tooltip property.
				var tooltiBinding = new Binding("ToolTip");
				tooltiBinding.Source = newViewModel;
				tooltiBinding.Converter = new ToolTiConverter();
				SetBinding(ToolTipProperty, tooltiBinding);

				// Binding the FirstLevelItemsAsGroup property.
				var isGrouBinding = new MultiBinding();
				var viewModelBinding = new Binding() {BindsDirectlyToSource = true};
				viewModelBinding.Source = newViewModel;
				isGrouBinding.Bindings.Add(viewModelBinding);
				var firstLevelItemAsGrouBinding = new Binding("FirstLevelItemsAsGroup");
				firstLevelItemAsGrouBinding.Source = ParentTreeListView;
				isGrouBinding.Bindings.Add(firstLevelItemAsGrouBinding);
				isGrouBinding.Converter = new ItemToIsGroupConverter();
				SetBinding(IsGroupProperty, isGrouBinding);

				// Binding the group item data template.
				var groupItemDataTemplateBinding = new Binding("GroupItemDataTemplate");
				groupItemDataTemplateBinding.Source = ParentTreeListView;
				SetBinding(GroupDataTemplateProperty, groupItemDataTemplateBinding);

				// View model defines the group model if the item is displayed as a group.
				Group = newViewModel;

				if (_expander != null) {
					// Binding the visibility of the expander.
					var expanderVisibilityBinding = new Binding("HasChildren");
					expanderVisibilityBinding.Source = newViewModel;
					expanderVisibilityBinding.Converter = new BoolToVisibilityConverter();
					_expander.SetBinding(VisibilityProperty, expanderVisibilityBinding);

					// Binding the indentation of the expander.
					var expanderMarginBinding = new Binding("Parent");
					expanderMarginBinding.Source = newViewModel;
					expanderMarginBinding.Converter = new LevelToIndentConverter();
					_expander.SetBinding(MarginProperty, expanderMarginBinding);

					// Binding the expanded state.
					RegisterToExpanderEvents();
					var expanderIsCheckedBinding = new Binding("IsExpanded");
					expanderIsCheckedBinding.Source = newViewModel;
					expanderIsCheckedBinding.Mode = BindingMode.OneWay;
					_expander.SetBinding(ToggleButton.IsCheckedProperty, expanderIsCheckedBinding);
				}

				if (_image != null) {
					// Binding the icon source.
					var imageSourceBinding = new Binding("IconSource");
					imageSourceBinding.Source = newViewModel;
					_image.SetBinding(Image.SourceProperty, imageSourceBinding);

					// Binding the icon visibility.
					var imageVisibilityBinding = new Binding("IconSource");
					imageVisibilityBinding.Source = newViewModel;
					imageVisibilityBinding.Converter = new NullToVisibilityConverter()
						{NullValue = Visibility.Collapsed};
					_image.SetBinding(VisibilityProperty, imageVisibilityBinding);
				}

				if (_checkBox != null) {
					// Binding the checked state.
					var checkBoxIsCheckedBinding = new Binding("IsChecked");
					checkBoxIsCheckedBinding.Source = newViewModel;
					checkBoxIsCheckedBinding.Mode = BindingMode.OneWay;
					_checkBox.SetBinding(ToggleButton.IsCheckedProperty, checkBoxIsCheckedBinding);
					RegisterToCheckBoxEvents();

					// Binding the checkable state.
					var checkBoxVisibilityBinding = new Binding("IsCheckable");
					checkBoxVisibilityBinding.Source = newViewModel;
					checkBoxVisibilityBinding.Converter = new BooleanToVisibilityConverter();
					_checkBox.SetBinding(VisibilityProperty, checkBoxVisibilityBinding);

					// Binding the checking state.
					var checkBoxIsEnabledBinding = new Binding("IsCheckingEnabled");
					checkBoxIsEnabledBinding.Source = newViewModel;
					_checkBox.SetBinding(IsEnabledProperty, checkBoxIsEnabledBinding);
				}
			}
		}

		/// <summary>
		/// Delegate called when the columns collection of the grid view is modified.
		/// </summary>
		private void OnGridViewSynchronized() {
			// Updating the expander clip region.
			_decoratorsContainer.Clip = GetDecoratorsCliRegion();
		}

		/// <summary>
		/// Retrieves the region used to clip the decorator container.
		/// </summary>
		private Geometry GetDecoratorsCliRegion() {
			// Blank space between the end of the column and the actual end of the clip region of the column.
			double blankSpaceMarginInPixels = 6;

			RectangleGeometry clip = null;
			var parent = ParentTreeListView;
			if (parent != null) {
				if ((parent.InnerListView.View != null)
				    && (parent.InnerListView.View.Columns.Count > 0)
				   ) {
					var firstColumn = parent.InnerListView.View.Columns[0];
					clip = new RectangleGeometry();

					var xBinding = new Binding("ActualWidth");
					xBinding.Source = firstColumn;
					var yBinding = new Binding("ActualHeight");
					yBinding.Source = parent.InnerListView;
					var rectBinding = new MultiBinding();
					rectBinding.Bindings.Add(xBinding);
					rectBinding.Bindings.Add(yBinding);
					rectBinding.Converter = new WidthHeightToRectConverter()
						{Margin = new Thickness(0, 0, blankSpaceMarginInPixels, 0)};

					BindingOperations.SetBinding(clip, RectangleGeometry.RectProperty, rectBinding);
				}
			}

			return clip;
		}

		/// <summary>
		/// Delegate called when the size of the decorators container changed.
		/// </summary>
		/// <param name="sender">The modified container.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnDecoratorsContainerSizeChanged(object sender, SizeChangedEventArgs eventArgs) {
			ViewModel.DecoratorWidth = eventArgs.NewSize.Width;
		}

		/// <summary>
		/// Delegate called when a key is pressed when the item get the focus.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnKeyUp(KeyEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemKeyUp(ViewModel, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the mouse left button is down.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseLeftButtonDown(ViewModel, eventArgs);
			}

			// Focused when clicked.
			Focus();

			eventArgs.Handled = true;
		}

		/// <summary>
		/// Delegate called when the mouse right button is down.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseRightButtonDown(ViewModel, eventArgs);
			}

			// Focused when clicked.
			Focus();

			eventArgs.Handled = true;
		}

		/// <summary>
		/// Delegate called when the mouse left button is up.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseLeftButtonUp(ViewModel, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the mouse right button is up.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseRightButtonUp(MouseButtonEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseRightButtonUp(ViewModel, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the mouse clicked on this item.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseUp(MouseButtonEventArgs eventArgs) {
			base.OnMouseUp(eventArgs);

			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseClicked(ViewModel, eventArgs);
			}
		}

		/// <summary>
		/// Delegate called when the mouse double clicked on this item.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnMouseDoubleClick(MouseButtonEventArgs eventArgs) {
			base.OnMouseDoubleClick(eventArgs);

			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemMouseDoubleClicked(ViewModel, eventArgs);
			}

			eventArgs.Handled = true;
		}

		/// <summary>
		/// Delegate called the check box is checked.
		/// </summary>
		/// <param name="sender">The check box sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnCheckBoxChecked(object sender, RoutedEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.CheckModel.Check(ViewModel, false);
			}
		}

		/// <summary>
		/// Delegate called the check box is unchecked.
		/// </summary>
		/// <param name="sender">The check box sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnCheckBoxUnchecked(object sender, RoutedEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.CheckModel.Uncheck(ViewModel, false);
			}
		}

		/// <summary>
		/// Delegate called when the expander gets checked.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnExpanderChecked(object sender, RoutedEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemExpanderChecked(ViewModel);
			}
		}

		/// <summary>
		/// Delegate called when the expander gets unchecked.
		/// </summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void OnExpanderUnchecked(object sender, RoutedEventArgs eventArgs) {
			var parent = ParentTreeListView;
			if (parent != null) {
				parent.InnerListView.OnItemExpanderUnchecked(ViewModel);
			}
		}

		#endregion // Methods.

	}

}