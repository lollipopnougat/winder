using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Winder.Model
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode> ChildNodes => subNodes;
        public DirectoryNode? ParentNode { get; set; }

        public string Title { get; }
        public string Name { get; }

        private ObservableCollection<DirectoryNode> subNodes;

        public Dictionary<string, DirectoryNode> NodeMap;


        private static List<string> pathRoute = [];


        public DirectoryNode(string title, string? name = null, ObservableCollection<DirectoryNode>? subNodes = null)
        {
            Title = title;
            Name = name ?? title;
            this.subNodes = subNodes ?? [];
            NodeMap = [];
            this.subNodes.ToList().ForEach(x => NodeMap.Add(x.Name, x));
            this.subNodes.CollectionChanged += SubNodesCollectionChanged;
        }

        private void SubNodesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NodeMap = [];
            this.subNodes.ToList().ForEach(x => NodeMap.Add(x.Name, x));
        }

        public static DirectoryNode BuildNodeTree(string cwd)
        {
            string phyPath = cwd.Replace("\\", "/");
            pathRoute = [.. phyPath.Split("/")];
            string rootPath = "/";
            DirectoryNode root = new DirectoryNode(rootPath, "此电脑");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Service.FileUtils.GetRemovableDeviceID().ForEach(x =>
                {
                    DirectoryNode mRoot = new($"{x}/", "此电脑");
                    CreateNode(mRoot, $"{x}/");
                    root.ChildNodes.Add(mRoot);
                });

            } else
            {
                CreateNode(root, rootPath);
            }
            
            return root;
        }

        private static void CreateNode(DirectoryNode node, string curPath)
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
    }
}
