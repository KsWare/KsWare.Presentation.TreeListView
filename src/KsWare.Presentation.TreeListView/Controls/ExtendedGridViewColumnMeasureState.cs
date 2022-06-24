namespace KsWare.Presentation.TreeListView.Controls {

	/// <summary>
	/// States of column when doing layout.
	/// </summary>
	internal enum ExtendedGridViewColumnMeasureState {

		/// <summary>
		/// Column width is just initialized and will size to content width.
		/// </summary>
		Init = 0,

		/// <summary>
		/// Column width reach max desired width of header(s) in this column.
		/// </summary>
		Headered = 1,

		/// <summary>
		/// Column width reach max desired width of data row(s) in this column.
		/// </summary>
		Data = 2,

		/// <summary>
		/// Column has a specific value as width.
		/// </summary>
		SpecificWidth = 3,

	}

}