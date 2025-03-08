using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using Avalonia.Data;
using Avalonia.Input;

namespace Winder.Utils
{
    public class DataGridRowDTBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<ICommand> DoubleTappedCommandProperty =
            AvaloniaProperty.RegisterAttached<DataGridRowDTBehavior, DataGridRow, ICommand>(
                "DoubleTappedCommand", default, false, BindingMode.OneTime);

        static DataGridRowDTBehavior()
        {
            DoubleTappedCommandProperty.Changed.AddClassHandler<DataGridRow>((itemElem, args) =>
            {
                if (args.NewValue is ICommand commandValue)
                {
                    // 添加非空值
                    itemElem.AddHandler(DataGridRow.DoubleTappedEvent, OnDClick);
                }
                else
                {
                    // 删除之前的值
                    itemElem.RemoveHandler(DataGridRow.DoubleTappedEvent, OnDClick);
                }
            });
            
        }

        private static void OnDClick(object? sender, TappedEventArgs e)
        {
            if (sender is DataGridRow item)
            {
                if (item.GetValue(DoubleTappedCommandProperty) is ICommand command)
                {
                    var parameter = item.DataContext; // 传递 DataContext 作为参数
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }

            }
            e.Handled = true;
        }

        public static void SetDoubleTappedCommand(AvaloniaObject element, ICommand value) =>
            element.SetValue(DoubleTappedCommandProperty, value);

        public static ICommand GetDoubleTappedCommand(AvaloniaObject element) =>
            element.GetValue(DoubleTappedCommandProperty);

    }
}
