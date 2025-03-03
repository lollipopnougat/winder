using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Winder.Model
{
    public class FileItem
    {
        private FileAttributes Attributes => attributes;
        public DateTime CreationTime => creationTime;
        public string Extension => parentPath;
        public int FileType => fileType;
        public string FullPath => Path.Join(ParentPath, Name);
        public DateTime LastAccessTime => lastAccessTime;
        public DateTime LastWriteTime => lastWriteTime;
        public long Length => length;
        public string Name => nameNoExtension + extension;
        public string ParentPath => parentPath;

        private FileAttributes attributes;
        private DateTime creationTime;
        private string extension;
        private int fileType;
        private DateTime lastAccessTime;
        private DateTime lastWriteTime;
        private long length;
        private string nameNoExtension;
        private string parentPath;
        private bool hidden;

        public FileItem(FileSystemInfo info)
        {
            if (info is DirectoryInfo dir)
            {
                attributes = dir.Attributes;
                creationTime = dir.CreationTime;
                extension = "";
                fileType = (int)FileItemType.Directory;
                lastAccessTime = dir.LastAccessTime;
                lastWriteTime = dir.LastWriteTime;
                length = -1;
                nameNoExtension = dir.Name;
                parentPath = dir.Parent?.FullName ?? "";
                hidden = (dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            }
            else if (info is FileInfo fileInfo)
            {
                attributes = fileInfo.Attributes;
                creationTime = fileInfo.CreationTime;
                extension = fileInfo.Extension ?? "";
                fileType = (int)GetFileType(fileInfo.Extension);
                lastAccessTime = fileInfo.LastAccessTime;
                lastWriteTime = fileInfo.LastWriteTime;
                length = fileInfo.Length;
                nameNoExtension = string.IsNullOrEmpty(fileInfo.Extension) ? fileInfo.Name : fileInfo.Name.Replace(fileInfo.Extension, "");
                parentPath = fileInfo.DirectoryName ?? "";
                hidden = (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

            }
            else
            {
                throw new InvalidDataException("非法的文件对象");
            }
        }
        private static FileItemType GetFileType(string? extension)
        {
            switch (extension?.Replace(".", "") ?? "")
            {
                case "zip":
                case "7z":
                case "rar":
                case "tar":
                case "gz":
                case "xz":
                case "tgz":
                case "bz2":
                    return FileItemType.ArchiveFile;
                case "jpg":
                case "png":
                case "jpeg":
                case "tif":
                case "tiff":
                case "webp":
                case "gif":
                case "bmp":
                case "svg":
                case "avif":
                    return FileItemType.ImageFile;
                case "mp4":
                case "rm":
                case "rmvb":
                case "mpeg":
                case "ts":
                case "3gp":
                case "mpg":
                case "vob":
                case "mov":
                case "avi":
                case "asf":
                case "wmv":
                case "flv":
                case "f4v":
                case "m4v":
                case "3gpp":
                case "mkv":
                case "webm":
                    return FileItemType.VideoFile;
                case "mp3":
                case "flac":
                case "wav":
                case "ogg":
                case "m4a":
                case "amr":
                case "aac":
                case "ac3":
                case "ape":
                case "midi":
                case "mid":
                case "wma":
                    return FileItemType.AudioFile;
                case "doc":
                case "docx":
                    return FileItemType.WordFile;
                case "ppt":
                case "pptx":
                    return FileItemType.PowerPointFile;
                case "xls":
                case "xlsx":
                    return FileItemType.ExcelFile;
                case "c":
                case "cpp":
                case "java":
                case "js":
                case "css":
                case "py":
                case "vbs":
                case "ps1":
                case "h":
                case "m":
                case "go":
                case "rs":
                case "pl":
                case "php":
                case "sh":
                case "cmd":
                case "kt":
                case "vb":
                case "ps":
                case "xml":
                case "html":
                case "json":
                case "ini":
                case "inf":
                case "toml":
                case "yml":
                    return FileItemType.SourceCodeFile;
                case "dll":
                case "so":
                case "lib":
                case "sys":
                    return FileItemType.libFile;
                case "txt":
                case "exc":
                case "log":
                case "md":
                    return FileItemType.PlainTextFile;
                case "lnk":
                    return FileItemType.LinkFile;
                case "app":
                case "exe":
                case "com":
                    return FileItemType.ExecFile;
                default:
                    return FileItemType.OtherFile;
            }
        }
    }
}
