using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
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
            MyAutoCompletePath = this.AutoCompletePath;
            store = Store.GenNewStore();
            SetAddressPathFromUI();
            BuildTreeView();

        }


        [ObservableProperty]
        public Func<string?, CancellationToken, Task<IEnumerable<object>>> myAutoCompletePath;

        [ObservableProperty]
        private string? titleText;

        [ObservableProperty]
        private string? cwd;

        [ObservableProperty]
        private string? pathBoxText;

        [ObservableProperty]
        private bool? upBtnEnabled;


        [ObservableProperty]
        private bool canGoBack;

        [ObservableProperty]
        private bool canGoForward;

        [ObservableProperty]
        private bool canPaste;

        [ObservableProperty]
        private string statusText;

        [ObservableProperty]
        private bool isError;

        [ObservableProperty]
        private string errorMsg;


        private readonly Store store;

        [ObservableProperty]
        private ObservableCollection<FileItemViewModel> selectedItems;

        [ObservableProperty]
        public ObservableCollection<FileItemViewModel> fileItems;

        [ObservableProperty]
        private ObservableCollection<TreeViewItemViewModel>? nodes;

        [ObservableProperty]
        private TreeViewItemViewModel selectedNode;

        [RelayCommand]
        private void SetAddressPathFromUI()
        {
            if (!string.IsNullOrEmpty(PathBoxText))
            {
                var file = FileUtils.getFileInfo(PathBoxText);
                if (file is DirectoryInfo dir)
                {
                    SetCurrentWorkingDirectory(dir.FullName);
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

        private void SetCurrentWorkingDirectory(string dirFullName, string? title = null, bool noHis = false)
        {
            Cwd = dirFullName;
            var titleStr = title ?? Path.GetFileName(dirFullName) ?? dirFullName;
            TitleText = $"{titleStr} - Winder";
            SetListView(dirFullName);
            PathBoxText = dirFullName;
            var dirPath = Path.GetDirectoryName(dirFullName);
            UpBtnEnabled = dirPath != null;
            if (!noHis)
            {
                store.addHistory(dirFullName);

            }
            CanGoBack = store.CanBack;
            CanGoForward = store.CanForward;
        }

        private void BuildTreeView()
        {
            List<string> paths = [.. Cwd.Replace("\\", "/").Split("/")];
            Nodes.Add(TreeViewItemViewModel.BuildNodeTree(Cwd));

        }
        [RelayCommand]
        private void UpBtnClick()
        {
            var dir = Path.GetDirectoryName(Cwd);
            if (dir != null)
            {
                SetCurrentWorkingDirectory(dir);
            }
        }

        [RelayCommand]
        private void RefreshList()
        {
            SetListView(Cwd);
        }
        private void SetListView(string cwd)
        {

            if (!string.IsNullOrEmpty(cwd) && Directory.Exists(cwd))
            {
                IsError = false;
                try
                {
                    DirectoryInfo dir = new(cwd);
                    List<DirectoryInfo> dirs = [.. dir.GetDirectories()];
                    dirs.Sort((a, b) => StringComparer.CurrentCulture.Compare(a.Name, b.Name));
                    List<FileInfo> files = [.. dir.GetFiles()];
                    files.Sort((a, b) => StringComparer.CurrentCulture.Compare(a.Name, b.Name));
                    FileItems.Clear();
                    bool isRoot = FileUtils.IsRootDirectory(cwd);
                    dirs.ForEach(dirInfo =>
                    {
                        if (!(isRoot && FileUtils.windowsSystemDirs.Contains(dirInfo.Name.ToLower())))
                        {
                            FileItems.Add(FileItemViewModel.CreateViewModel(dirInfo));
                        }
                    });
                    files.ForEach(file => FileItems.Add(FileItemViewModel.CreateViewModel(file)));
                    StatusText = $"{FileItems.Count}个项目";
                }
                catch (UnauthorizedAccessException)
                {
                    IsError = true;
                    ErrorMsg = $"暂无权限访问{cwd}";
                    StatusText = $"暂无权限访问{cwd}";
                }
                catch (Exception err)
                {
                    IsError = true;
                    ErrorMsg = $"出错啦: {err.Message}";
                    StatusText = $"错误: {err.Message}";
                }


            }
        }
        [RelayCommand]
        private void ClickItem(object? param)
        {
            Debug.WriteLine($"ev {param?.GetType().FullName}");
        }

        private static bool IsRightClick(PointerPressedEventArgs e)
        {
            return e.GetCurrentPoint(e.Source as Avalonia.Visual).Properties.IsLeftButtonPressed;
        }

        [RelayCommand]
        public void DoubleClick(object? param)
        {
            if (param is FileItemViewModel item)
            {
                if (item.FileType == (int)FileItemType.Directory)
                {
                    PathBoxText = item.FullPath;
                    SetAddressPathFromUI();
                }
                else
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
        }

        [RelayCommand]
        public void BackBtnClick(object? sender)
        {
            var backDir = store.getBack();
            if (backDir != null)
            {
                SetCurrentWorkingDirectory(backDir, noHis: true);
            }
        }


        [RelayCommand]
        public void ForwardBtnClick(object? param)
        {
            var forwardDir = store.getForward();
            if (forwardDir != null)
            {
                SetCurrentWorkingDirectory(forwardDir, noHis: true);
            }

        }

        private async Task<IEnumerable<object>> AutoCompletePath(string? text, CancellationToken token)
        {
            try
            {
                var res = await Task.Run(() =>
                {
                    List<string> rt = [];
                    if (text == null)
                    {

                    }
                    else if (Directory.Exists(text))
                    {
                        DirectoryInfo dir = new(text);
                        List<DirectoryInfo> dirs = [.. dir.GetDirectories()];
                        rt = [.. dirs.Select(x => x.FullName)];
                    }
                    else if (Directory.Exists(Path.GetDirectoryName(text)))
                    {
                        var parent = Path.GetDirectoryName(text);
                        if (parent != null)
                        {
                            DirectoryInfo dir = new(parent);
                            List<DirectoryInfo> dirs = [.. dir.GetDirectories()];
                            string prefix = Path.GetFileName(text);
                            rt = [.. dirs.Where(x => x.Name.StartsWith(prefix)).Select(x => x.FullName)];
                        }

                    }
                    return rt;
                }, token);
            }
            catch (Exception)
            {

            }
            return [];
        }

        [RelayCommand]
        public void CopyFileClick(object? param)
        {
            if (param is FileItemViewModel item)
            {
                store.IsCutPaste = false;
                store.CopyFile = item;
                CanPaste = true;
            }
        }

        [RelayCommand]
        public void CutFileClick(object? param)
        {
            if (param is FileItemViewModel item)
            {
                store.IsCutPaste = true;
                store.CopyFile = item;
                CanPaste = true;
            }
        }

        [RelayCommand]
        public async Task PasteFileClick(object? param)
        {
            if (store.CopyFile != null)
            {

                if (store.IsCutPaste)
                {
                    StatusText = "移动文件中...";
                    store.IsCutPaste = false;
                    await FileUtils.MoveFileAsync(store.CopyFile.FullPath, Cwd);
                    RefreshList();
                }
                else
                {
                    StatusText = "复制文件中...";
                    await FileUtils.CopyFileAsync(store.CopyFile.FullPath, Cwd);
                    RefreshList();
                }
            }
        }
        [RelayCommand]
        public async Task RemoveFileClick(object? param)
        {
            if (param is FileItemViewModel item)
            {
                StatusText = "删除文件中...";
                await Emik.Rubbish.MoveAsync(item.FullPath);
                RefreshList();
            }
        }

        [RelayCommand]
        public void ClickTreeView(object? param)
        {
            if (param is TreeViewItemViewModel item)
            {
                if (item == SelectedNode && item.FullPath != "" && Cwd != item.FullPath)
                {
                    SetCurrentWorkingDirectory(item.FullPath);
                }
            }
        }
        [RelayCommand]
        public void ExpandNode(object? param)
        {
            if (param is TreeViewItemViewModel item)
            {
                item.SyncNodeWithFS();
            }
        }


    }
}
