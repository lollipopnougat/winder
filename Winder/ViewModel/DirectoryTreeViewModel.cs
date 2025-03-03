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

        public void BuildNodeTree(string cwd)
        {
            string phyPath = cwd.Replace("\\", "/");
            pathRoute = phyPath.Split("/").ToList();
            string rootPath = "/";
            DirectoryNode root = new(rootPath);
            CreateNode(root, rootPath);
            

        }
        private void CreateNode(DirectoryNode node, string curPath)
        {
            if (pathRoute == null || pathRoute.Count == 0)
            {
                return;
            }
            DirectoryNode? nextNode = null;
            string folder = pathRoute[0];
            pathRoute.RemoveAt(0);
            List<string> folders = Directory.GetDirectories(Path.Combine(curPath, folder)).Select((x) => Path.GetFileName(x)).ToList();
            folders.Sort();
            folders.ForEach(x =>
            {
                DirectoryNode newNode = new(x);
                if (pathRoute?.Count > 0 && pathRoute[0] == x) 
                {
                    nextNode = newNode;
                }
                node.ChildNodes.Add(newNode);
                newNode.ParentNode = node;
            });
            if (nextNode != null) 
            {
                CreateNode(nextNode, Path.Combine(curPath, nextNode.Name));
            }
            
            
        }
        private ObservableCollection<DirectoryNode> rootNode;

        public ObservableCollection<DirectoryNode> RootNode => rootNode;

    }
}
