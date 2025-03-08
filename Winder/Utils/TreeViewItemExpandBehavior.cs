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
    public class TreeViewItemBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<ICommand> ExpandedCommandProperty =
            AvaloniaProperty.RegisterAttached<TreeViewItemBehavior, TreeViewItem, ICommand>(
                "ExpandedCommand", default, false, BindingMode.OneTime);
        public static readonly AttachedProperty<ICommand> ItemClickCommandProperty =
            AvaloniaProperty.RegisterAttached<TreeViewItemBehavior, TreeViewItem, ICommand>(
                "ItemClickCommand", default, false, BindingMode.OneTime);

        static TreeViewItemBehavior()
        {
            ExpandedCommandProperty.Changed.AddClassHandler<TreeViewItem>((itemElem, args) =>
            {
                if (args.NewValue is ICommand commandValue)
                {
                    // 添加非空值
                    itemElem.AddHandler(TreeViewItem.ExpandedEvent, OnExpanded);
                }
                else
                {
                    // 删除之前的值
                    itemElem.RemoveHandler(TreeViewItem.ExpandedEvent, OnExpanded);
                }
            });
            ItemClickCommandProperty.Changed.AddClassHandler<TreeViewItem>((item, args) => 
            {
                if (args.NewValue is ICommand commandValue)
                {
                    // 添加非空值
                    item.AddHandler(TreeViewItem.TappedEvent, OnClick);
                }
                else
                {
                    // 删除之前的值
                    item.RemoveHandler(TreeViewItem.TappedEvent, OnClick);
                }
            });
        }

        private static void OnClick(object? sender, TappedEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                if (item.GetValue(ItemClickCommandProperty) is ICommand command)
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

        private static void OnExpanded(object? sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                if (item.GetValue(ExpandedCommandProperty) is ICommand command)
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

        public static void SetExpandedCommand(AvaloniaObject element, ICommand value) =>
            element.SetValue(ExpandedCommandProperty, value);

        public static ICommand GetExpandedCommand(AvaloniaObject element) =>
            element.GetValue(ExpandedCommandProperty);

        public static void SetItemClickCommand(AvaloniaObject element, ICommand value) =>
            element.SetValue(ItemClickCommandProperty, value);

        public static ICommand GetItemClickCommand(AvaloniaObject element) =>
            element.GetValue(ItemClickCommandProperty);
    }
}
