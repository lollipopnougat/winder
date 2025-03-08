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
            this.length = GetFileLengthStr(fileItem.Length);
            this.FullPath = fileItem.FullPath;
            this.FileTypeStr = fileItem.FileTypeStr;
        }

        private static string GetFileLengthStr(long len)
        {
            if (len == -1)
            {
                return "";
            }
            long kB = len / 1024;
            long mB = kB / 1024;
            long gB = mB / 1024;
            if (kB == 0)
            {
                return $"{len}B";
            } else if (mB == 0)
            {
                return $"{kB}KB";
            } else if (gB == 0)
            {
                return $"{mB}MB";
            } else
            {
                return $"{gB}GB";
            }
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

        [ObservableProperty]
        private string fileTypeStr;

        public string FullPath;

        public static FileItemViewModel CreateViewModel(FileSystemInfo file)
        {
            return new FileItemViewModel(new FileItem(file));
        }

        

    }
}
