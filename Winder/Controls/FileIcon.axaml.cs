using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using Winder.Model;

namespace Winder.Controls;

public partial class FileIcon : UserControl
{
    public FileIcon()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<int> FileTypeProperty =
        AvaloniaProperty.Register<FileIcon, int>(nameof(FileType), defaultValue: 0);

    
    public int FileType
    {
        get => GetValue(FileTypeProperty);
        set
        {
            SetValue(FileTypeProperty, value);
        }
    }

}