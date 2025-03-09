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

namespace Winder.ViewModel
{
    public class TreeViewItemViewModel
    {
        public ObservableCollection<TreeViewItemViewModel> ChildNodes => subNodes;
        public TreeViewItemViewModel? ParentNode { get; set; }

        public string Title { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

        public bool IsFixed { get; set; }

        private ObservableCollection<TreeViewItemViewModel> subNodes;

        public Dictionary<string, TreeViewItemViewModel> NodeMap;



        public TreeViewItemViewModel(string name, string fullPath, string? title = null, ObservableCollection<TreeViewItemViewModel>? subNodes = null)
        {
            Name = name;
            Title = title ?? name;
            FullPath = fullPath;
            this.subNodes = subNodes ?? [];
            NodeMap = [];
            IsFixed = false;
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
                        if (item is TreeViewItemViewModel node)
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
                        if (item is TreeViewItemViewModel node && NodeMap.ContainsKey(node.Name))
                        {
                            NodeMap.Remove(node.Name);
                        }
                    }
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                NodeMap = [];
                subNodes.ToList().ForEach(x => NodeMap.Add(x.Name, x));
            }

        }

        public static List<TreeViewItemViewModel> BuildNodeTree(string cwd)
        {
            List<TreeViewItemViewModel> rt = [];
            string phyPath = cwd.Replace("\\", "/");
            List<string> pathRoute = [.. phyPath.Split("/")];
            TreeViewItemViewModel root = new("", "", "此电脑");
            root.IsFixed = true;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                DriveInfo.GetDrives().ToList().ForEach(x =>
                {
                    TreeViewItemViewModel mRoot = new(x.Name, x.RootDirectory.FullName, $"{x.VolumeLabel}({x.Name.Replace("\\", "")})");
                    if (pathRoute.Count > 0 && x.Name.Contains(pathRoute[0]))
                    {
                        pathRoute.RemoveAt(0);
                        CreateNode(mRoot, pathRoute);
                    }
                    else
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
            rt.Add(root);
            TreeViewItemViewModel userNode = new(Environment.UserName, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{Environment.UserName}的Home");
            CreateChildNode(userNode);
            rt.Add(userNode);
            return rt;
        }

        public static void CreateNode(TreeViewItemViewModel node, List<string> pathRoute)
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
            List<string> folders = FileUtils.GetChildDirectoriesSafe(node.FullPath);
            bool isRoot = FileUtils.IsRootDirectory(node.FullPath);
            folders.Sort((a, b) => StringComparer.CurrentCulture.Compare(a, b));
            folders.ForEach(x =>
            {
                if (!(isRoot && FileUtils.windowsSystemDirs.Contains(x.ToLower())))
                {
                    string newPath = Path.Combine(node.FullPath, x);
                    TreeViewItemViewModel newNode = new(x, newPath);
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
                }
            });
        }

        public static void CreateChildNode(TreeViewItemViewModel node)
        {
            List<string> folders = FileUtils.GetChildDirectoriesSafe(node.FullPath);
            bool isRoot = FileUtils.IsRootDirectory(node.FullPath);
            folders.Sort((a, b) => StringComparer.CurrentCulture.Compare(a, b));
            folders.ForEach(x =>
            {
                if (!(isRoot && FileUtils.windowsSystemDirs.Contains(x.ToLower())))
                {
                    TreeViewItemViewModel newNode = new(x, Path.Combine(node.FullPath, x));
                    node.ChildNodes.Add(newNode);
                }
            });
        }


        public void SyncNodeWithFS()
        {
            SyncChildNodeWithFs(this, 1);
        }

        public static void SyncChildNodeWithFs(TreeViewItemViewModel node, int depth)
        {
            if (node.IsFixed)
            {
                foreach (TreeViewItemViewModel item in node.ChildNodes)
                {
                    item.SyncNodeWithFS();
                }
                return;
            }
            List<string> folders = FileUtils.GetChildDirectoriesSafe(node.FullPath);
            bool isRoot = FileUtils.IsRootDirectory(node.FullPath);
            folders.Sort((a, b) => StringComparer.CurrentCulture.Compare(a, b));
            List<TreeViewItemViewModel> nodes = [];
            folders.ForEach(x =>
            {
                if (!(isRoot && FileUtils.windowsSystemDirs.Contains(x.ToLower())))
                {

                    if (node.NodeMap.TryGetValue(x, out TreeViewItemViewModel? tNode))
                    {
                        if (depth > 0)
                        {
                            SyncChildNodeWithFs(tNode, depth - 1);
                        }
                        nodes.Add(tNode);
                    }
                    else
                    {
                        TreeViewItemViewModel nNode = new(x, Path.Combine(node.FullPath, x));
                        if (depth > 0)
                        {
                            SyncChildNodeWithFs(nNode, depth - 1);
                        }
                        nodes.Add(nNode);
                    }
                }
            });
            node.ChildNodes.Clear();
            nodes.ForEach(x => node.ChildNodes.Add(x));
        }
    }
}
