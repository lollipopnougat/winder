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
using Winder.Service;

namespace Winder.Model
{
    public class DirectoryNode
    {
        public ObservableCollection<DirectoryNode> ChildNodes => subNodes;
        public DirectoryNode? ParentNode { get; set; }

        public string Title { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

        private ObservableCollection<DirectoryNode> subNodes;

        public Dictionary<string, DirectoryNode> NodeMap;



        public DirectoryNode(string name, string fullPath, string? title = null, ObservableCollection<DirectoryNode>? subNodes = null)
        {
            Name = name;
            Title = title ?? name;
            FullPath = fullPath;
            this.subNodes = subNodes ?? [];
            NodeMap = [];
            this.subNodes.ToList().ForEach(x => NodeMap.Add(x.Name, x));
            this.subNodes.CollectionChanged += SubNodesCollectionChanged;
        }

        private void SubNodesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is DirectoryNode node)
                        {
                            NodeMap.Add(node.Name, node);
                        }
                    }
                }

            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is DirectoryNode node && NodeMap.ContainsKey(node.Name))
                        {
                            NodeMap.Remove(node.Name);
                        }
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                NodeMap = [];
                this.subNodes.ToList().ForEach(x => NodeMap.Add(x.Name, x));
            }

        }

        public static DirectoryNode BuildNodeTree(string cwd)
        {
            string phyPath = cwd.Replace("\\", "/");
            List<string> pathRoute = [.. phyPath.Split("/")];
            DirectoryNode root = new("", "", "此电脑");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DriveInfo.GetDrives().ToList().ForEach(x =>
                {
                    DirectoryNode mRoot = new(x.Name, x.RootDirectory.FullName, $"{x.VolumeLabel}({x.Name.Replace("\\", "")})");
                    if (pathRoute.Count > 0 && x.Name.Contains(pathRoute[0]))
                    {
                        pathRoute.RemoveAt(0);
                        CreateNode(mRoot, pathRoute);
                    } else
                    {
                        CreateChildNode(mRoot);
                    }
                    root.ChildNodes.Add(mRoot);
                });

            }
            else
            {
                root.FullPath = "/";
                root.Name = "/";
                if (pathRoute.Count > 0 && pathRoute[0] == "")
                {
                    pathRoute.RemoveAt(0);
                }
                CreateNode(root, pathRoute);
            }

            return root;
        }

        public static void CreateNode(DirectoryNode node, List<string> pathRoute)
        {
            if (pathRoute == null || pathRoute.Count == 0)
            {
                return;
            }

            string folder = pathRoute[0];
            while (folder == "" && pathRoute.Count > 0)
            {
                pathRoute.RemoveAt(0);
                folder = pathRoute[0];
            }
            if (folder == "")
            {
                return;
            }
            List<string> folders = [.. Directory.GetDirectories(node.FullPath).Select((x) => Path.GetFileName(x))];
            //List<string> folders = Directory.GetDirectories(Path.Combine(curPath, folder)).Select((x) => Path.GetFileName(x)).ToList();
            folders.Sort((a, b) => StringComparer.CurrentCulture.Compare(a, b));
            folders.ForEach(x =>
            {
                string newPath = Path.Combine(node.FullPath, x);
                DirectoryNode newNode = new(x, newPath);
                if (pathRoute.Count > 0 && x == pathRoute[0])
                {
                    pathRoute.RemoveAt(0);
                    CreateNode(newNode, pathRoute);
                }
                else
                {
                    CreateChildNode(newNode);
                }
                node.ChildNodes.Add(newNode);


            });
        }

        public static void CreateChildNode(DirectoryNode node)
        {
            List<string> folders = FileUtils.GetChildDirectories(node.FullPath); //[.. Directory.GetDirectories(node.FullPath).Select((x) => Path.GetFileName(x))];
            folders.Sort((a, b) => StringComparer.CurrentCulture.Compare(a, b));
            folders.ForEach(x =>
            {
                DirectoryNode newNode = new(x, Path.Combine(node.FullPath, x));
                node.ChildNodes.Add(newNode);
            });
        }
    }
}
