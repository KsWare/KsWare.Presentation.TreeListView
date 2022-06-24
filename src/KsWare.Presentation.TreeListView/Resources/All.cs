using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace KsWare.Presentation.TreeListView.Resources {

	/// <summary>
	/// Class defining the global resources.
	/// </summary>
	public partial class All : ResourceDictionary {

		#region Fields

		/// <summary>
		/// Stores the unique instance of the singleton.
		/// </summary>
		private static All s_msInstance;

		/// <summary>
		/// Stores the key of the data template binding to source in a cell.
		/// </summary>
		public static string BindToSourceCellDataTemplateKey = "BindToSourceCellDataTemplateKey";

		/// <summary>
		/// Stores the map of the cell data template by display member path.
		/// </summary>
		private readonly Dictionary<string, DataTemplate> _cellDataTemplates;

		#endregion // Fields.

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="All"/> class.
		/// </summary>
		private All() {
			Source = new Uri(@"/KsWare.Presentation.TreeListView;component/Resources/All.xaml", UriKind.Relative);

			_cellDataTemplates = new Dictionary<string, DataTemplate>();
			_cellDataTemplates.Add(BindToSourceCellDataTemplateKey,
				this[BindToSourceCellDataTemplateKey] as DataTemplate);
		}

		#endregion // Constructors.

		#region Properties

		/// <summary>
		/// Gets the unique instance of the singleton.
		/// </summary>
		public static All Instance {
			get {
				if (s_msInstance == null) {
					s_msInstance = new All();
				}

				return s_msInstance;
			}
		}

		#endregion // Properties.

		#region Methods

		/// <summary>
		/// Returns the cell data template displaying the member path in a text block.
		/// </summary>
		/// <param name="displayMemberPath">The display member path.</param>
		/// <returns>The built data template.</returns>
		public DataTemplate GetCellTemplate(string displayMemberPath) {
			// Trying to get it from the cache.
			DataTemplate dataTemplate;
			if (_cellDataTemplates.TryGetValue(displayMemberPath, out dataTemplate)) {
				return dataTemplate;
			}

			if (string.IsNullOrEmpty(displayMemberPath) == false) {
				// Building dynamically the data template.
				var textBlockFactory =
					new FrameworkElementFactory(typeof(System.Windows.Controls.TextBlock));
				var displayMemberBinding = new Binding(displayMemberPath);
				textBlockFactory.SetBinding(System.Windows.Controls.TextBlock.TextProperty, displayMemberBinding);
				dataTemplate = new DataTemplate();
				dataTemplate.VisualTree = textBlockFactory;
				dataTemplate.Seal();

				// Caching the data template.
				_cellDataTemplates.Add(displayMemberPath, dataTemplate);

				return dataTemplate;
			}

			// Getting the bind to source data template.
			return _cellDataTemplates[BindToSourceCellDataTemplateKey];
		}

		#endregion // Methods.

	}

}