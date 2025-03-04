using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        public static List<string> GetChildDirectories(string path)
        {
            List<string> rt = [];
            try
            {
                rt.AddRange(Directory.GetDirectories(path).Select(x => Path.GetFileName(x)));
            } catch (Exception err)
            {
                Debug.WriteLine($"err {err}");
            }
            return rt;
        }
    }
}