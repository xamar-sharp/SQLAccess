using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
namespace SQLAccess
{
    public static class FrameworkElementExtensions
    {
        public static void AddToolTip(this FrameworkElement element, string hint)
        {
            element.ToolTip = new ToolTip() { Content = hint };
        }
        public static void AddContextAction(this FrameworkElement element, string textCommand, string iconName, ICommand command, Func<Task<object>> commandParameter)
        {
            element.ContextMenu = new ContextMenu() { Items = { new MenuItem() { Header = textCommand, Icon = new BitmapImage(new Uri(iconName, UriKind.Relative)), Command = command,CommandParameter = commandParameter } } };
        }
    }
}
