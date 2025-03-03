using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExCSS;
using Winder.Model;
using Winder.Service;

namespace Winder.ViewModel
{
    public partial class FileItemViewModel : ViewModelBase
    {
        public FileItemViewModel(FileItem fileItem)
        {
            this.creationTime = fileItem.CreationTime.ToString("g");
            this.fileType = fileItem.FileType;
            this.name = fileItem.Name;
            this.lastAccessTime = fileItem.LastAccessTime.ToString("g");
            this.lastWriteTime = fileItem.LastWriteTime.ToString("g");
            this.length = $"{fileItem.Length / 1024}KB";
            this.FullPath = fileItem.FullPath;
        }

        //private FileAttributes attributes;
        [ObservableProperty]
        private string creationTime;
        [ObservableProperty]
        private int fileType;
        [ObservableProperty]
        private string lastAccessTime;
        [ObservableProperty]
        private string lastWriteTime;
        [ObservableProperty]
        private string length;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private bool hidden;

        public string FullPath;

        public static FileItemViewModel CreateViewModel(FileSystemInfo file)
        {
            return new FileItemViewModel(new FileItem(file));
        }

        

    }
}
