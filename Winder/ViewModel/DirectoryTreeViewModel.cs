using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Winder.Model;
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
        private List<string>? pathRoute;

        
        private ObservableCollection<DirectoryNode> rootNode;

        public ObservableCollection<DirectoryNode> RootNode => rootNode;

    }
}
