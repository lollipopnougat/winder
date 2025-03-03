using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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


        public static List<string> GetRemovableDeviceID()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                List<string> deviceIDs = [];
                ManagementObjectSearcher query = new("SELECT * From Win32_LogicalDisk");
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection.Cast<ManagementObject>())
                {

                    switch (int.Parse(mo["DriveType"]?.ToString() ?? "-1"))
                    {
                        case (int)DriveType.Removable:   //可以移动磁盘     
                            {
                                //MessageBox.Show("可以移动磁盘");
                                var tmp = mo["DeviceID"].ToString();
                                if (tmp != null)
                                {
                                    deviceIDs.Add(tmp);
                                }
                                break;
                            }
                        case (int)DriveType.Fixed:   //本地磁盘     
                            {
                                //MessageBox.Show("本地磁盘");
                                var tmp = mo["DeviceID"].ToString();
                                if (tmp != null)
                                {
                                    deviceIDs.Add(tmp);
                                }
                                break;
                            }
                        case (int)DriveType.CDRom:   //CD   rom   drives     
                            {
                                //MessageBox.Show("CD   rom   drives ");
                                break;
                            }
                        case (int)DriveType.Network:   //网络驱动   
                            {
                                //MessageBox.Show("网络驱动器 ");
                                break;
                            }
                        case (int)DriveType.Ram:
                            {
                                //MessageBox.Show("驱动器是一个 RAM 磁盘 ");
                                break;
                            }
                        case (int)DriveType.NoRootDirectory:
                            {
                                //MessageBox.Show("驱动器没有根目录 ");
                                break;
                            }
                        default:   //defalut   to   folder     
                            {
                                //MessageBox.Show("驱动器类型未知 ");
                                break;
                            }
                    }

                }
                return deviceIDs;

            } else
            {
                return ["/"];
            }
        }

    }
}