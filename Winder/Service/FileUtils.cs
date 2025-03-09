using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Winder.Service
{
    class FileUtils
    {
        public static bool IsFile(string path)
        {
            return File.Exists(path);
        }

        public static bool IsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public static FileSystemInfo? getFileInfo(string text)
        {
            string path = text == "~" ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) : text;
            if (IsDirectory(path))
            {
                return new DirectoryInfo(path);
            }
            else if (IsFile(path))
            {
                return new FileInfo(path);
            }
            else
            {
                return null;
            }
        }

        // 处理文件路径中的特殊字符（如空格）
        private static string EscapeFilePath(string path)
        {
            return $"\"{path}\"";
        }

        public static void StartFile(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows：使用 ShellExecute 打开文件
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // Linux：使用 xdg-open 命令
                Process.Start("xdg-open", EscapeFilePath(path));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS：使用 open 命令
                Process.Start("open", EscapeFilePath(path));
            }
            else
            {
                throw new PlatformNotSupportedException("不支持的操作系统。");
            }
        }

        public static readonly HashSet<string> windowsSystemDirs = ["recovery", "system volume information", "$recycle.bin", "perflogs", "documents and settings", "config.msi"];
        public static bool IsRootDirectory(string path)
        {
            return Path.GetPathRoot(path) == path;
        }

        public static List<string> GetChildDirectoriesSafe(string path)
        {
            List<string> rt = [];
            try
            {
                rt.AddRange(Directory.GetDirectories(path).Select(x => Path.GetFileName(x)));
            }
            catch (Exception err)
            {
                Debug.WriteLine($"err {err}");
            }
            return rt;
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            if (!Directory.Exists(destinationDir))
            {
                if (File.Exists(destinationDir))
                {
                    throw new ArgumentException($"Destination is a file: ${destinationDir}");
                }
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public static async Task CopyFileAsync(string source, string destination)
        {
            await Task.Run(() =>
            {
                bool isCopyDir = Directory.Exists(source);
                if (!isCopyDir && !File.Exists(source))
                {
                    throw new FileNotFoundException($"Source file not found: {source}");
                }
                if (isCopyDir)
                {
                    CopyDirectory(source, destination, true);
                }
                else
                {
                    string fileName = Path.GetFileName(source);
                    string target = Path.Combine(destination, fileName);
                    if (File.Exists(target))
                    {
                        string fileNameNoExt = Path.GetFileNameWithoutExtension(fileName);
                        string ext = Path.GetExtension(fileName);
                        target = $"{Path.Combine(destination, fileNameNoExt)}-副本{ext}";
                    }
                    File.Copy(source, target);
                }
            });
        }

        public static async Task<bool> MoveFileAsync(string source, string destination, bool isRename)
        {
            return await Task.Run<bool>(() =>
            {
                try
                {
                    bool isDir = Directory.Exists(source);
                    string fileName = Path.GetFileName(source);
                    if (isDir)
                    {
                        if (isRename)
                        {
                            Directory.Move(source, destination);
                        }
                        else
                        {
                            Directory.Move(source, Path.Combine(destination, fileName));
                        }
                    }
                    else
                    {
                        if (isRename)
                        {
                            File.Move(source, destination);
                        }
                        else
                        {
                            File.Move(source, Path.Combine(destination, fileName));
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            });


        }


        public static async Task<bool> RenameFile(string path, string newName)
        {
            var dirPath = Path.GetDirectoryName(path);
            if (dirPath != null)
            {
                return await MoveFileAsync(path, Path.Combine(dirPath, newName), true);
            }
            return false;
        }
    }
}