using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Winder.Model;
using Winder.Service;
using Winder.Storage;

namespace Winder.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            TitleText = $"~ - Winder";
            PathBoxText = "~";
            fileItems = [];
            Nodes = [];
            store = Store.GenNewStore();
            SetAddressPath();
            BuildTreeView();

        }

        [ObservableProperty]
        private string? titleText;

        [ObservableProperty]
        private string? cwd;

        [ObservableProperty]
        private string? pathBoxText;

        [ObservableProperty]
        private bool? upBtnEnabled;

        private Store store;


        private ObservableCollection<FileItem> SelectedItems { get; } = [];

        [ObservableProperty]
        public ObservableCollection<FileItemViewModel> fileItems;

        [ObservableProperty]
        private ObservableCollection<DirectoryNode>? nodes;

        [RelayCommand]
        private void SetAddressPath()
        {
            if (!string.IsNullOrEmpty(PathBoxText))
            {
                var file = FileUtils.getFileInfo(PathBoxText);
                if (file is DirectoryInfo dir)
                {
                    Cwd = dir.FullName;
                    TitleText = $"{dir.Name} - Winder";
                    SetListView(dir.FullName);
                    PathBoxText = dir.FullName;
                    var dirPath = Path.GetDirectoryName(Cwd);
                    UpBtnEnabled = dirPath != null;
                }
                else if (file is FileInfo fil)
                {
                    try
                    {
                        FileUtils.StartFile(fil.FullName);
                        PathBoxText = Cwd;
                    }
                    catch (Exception ex)
                    {

                    }
                }



            }

        }

        private void BuildTreeView()
        {
            List<string> paths = [.. Cwd.Replace("\\", "/").Split("/")];
            Nodes.Add(DirectoryNode.BuildNodeTree(Cwd));

        }
        [RelayCommand]
        private void UpBtnClick()
        {
            var dir = Path.GetDirectoryName(Cwd);
            if (dir != null)
            {
                PathBoxText = dir;
                SetAddressPath();
            }
        }

        private void SetListView(string cwd)
        {

            if (!string.IsNullOrEmpty(cwd) && Directory.Exists(cwd))
            {
                DirectoryInfo dir = new(cwd);
                List<DirectoryInfo> dirs = [.. dir.GetDirectories()];
                dirs.Sort((a, b) => StringComparer.CurrentCulture.Compare(a.Name,b.Name));
                List<FileInfo> files = [.. dir.GetFiles()];
                files.Sort((a, b) => StringComparer.CurrentCulture.Compare(a.Name, b.Name));
                FileItems.Clear();
                dirs.ForEach(dirInfo => FileItems.Add(FileItemViewModel.CreateViewModel(dirInfo)));
                files.ForEach(file => FileItems.Add(FileItemViewModel.CreateViewModel(file)));
                Debug.WriteLine($"count = {FileItems.Count}");
            }
        }
        [RelayCommand]
        private void RightClickItem()
        {

        }

        [RelayCommand]
        public void DoubleClick(object? param)
        {
            if (param is FileItemViewModel item)
            {
                if (item.FileType == (int)FileItemType.Directory)
                {
                    PathBoxText = item.FullPath;
                    SetAddressPath();
                } else
                {
                    try
                    {
                        FileUtils.StartFile(item.FullPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"double click {ex}");
                    }
                }
            }
            Debug.WriteLine($"double click");
        }



    }
}
