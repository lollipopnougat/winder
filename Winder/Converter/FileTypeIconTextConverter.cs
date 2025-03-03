using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winder.Model;

namespace Winder.Converter
{
    public class FileTypeIconTextConverter : IValueConverter
    {
        public static readonly FileTypeIconTextConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int sourceValue)
            {
                return InnerFileType(sourceValue);
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);
        }


        public object ConvertBack(object? value, Type targetType,
                                    object? parameter, CultureInfo culture)
        {
            if (value is int sourceValue)
            {
                return InnerFileType(sourceValue);
            }
            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);
        }

        private string InnerFileType(int type)
        {
            switch (type)
            {
                case (int)FileItemType.Directory:
                    return "\xf07b";
                case (int)FileItemType.OtherFile:
                    return "\xf15b";
                case (int)FileItemType.ArchiveFile:
                    return "\xf1c6";
                case (int)FileItemType.ImageFile:
                    return "\xf03e";
                case (int)FileItemType.VideoFile:
                    return "\xf008";
                case (int)FileItemType.AudioFile:
                    return "\xf1c7";
                case (int)FileItemType.WordFile:
                    return "\xf1c2";
                case (int)FileItemType.ExcelFile:
                    return "\xf1c3";
                case (int)FileItemType.PowerPointFile:
                    return "\xf1c4";
                case (int)FileItemType.SourceCodeFile:
                    return "\xf1c9";
                case (int)FileItemType.libFile:
                    return "\xf085";
                case (int)FileItemType.PlainTextFile:
                    return "\xf15c";
                case (int)FileItemType.LinkFile:
                    return "\xf360";
                case (int)FileItemType.hardDriveFile:
                    return "\xf0a0";
                case (int)FileItemType.ExecFile:
                    return "\xf2d0";
                default:
                    return "\xf15b";
            }
        }

    }
}
