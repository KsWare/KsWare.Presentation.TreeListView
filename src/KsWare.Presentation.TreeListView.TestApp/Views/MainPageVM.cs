using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KsWare.Presentation.TreeListView.TestApp.ViewModels;

namespace KsWare.Presentation.TreeListView.TestApp.Views;

internal class MainPageVM   {

	public MainPageVM() {

	}

	public RegistryRootViewModel Registry { get; } = new RegistryRootViewModel();

}
