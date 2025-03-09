using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Winder.ViewModel
{
    public class DirectoryTreeViewModel: ViewModelBase
    {
        public DirectoryTreeViewModel()
        {
            rootNode = [];

        }

        
        private ObservableCollection<TreeViewItemViewModel> rootNode;

        public ObservableCollection<TreeViewItemViewModel> RootNode => rootNode;

    }
}
