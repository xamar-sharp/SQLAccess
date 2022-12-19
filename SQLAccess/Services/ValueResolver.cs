using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace SQLAccess.Services
{
#nullable disable
    public sealed class ValueResolver : IValueResolver
    {
        public object GetValue(FrameworkElement element)
        {
            if (element is TextBox)
            {
                return (element as TextBox).Text;
            }
            else if (element is DatePicker)
            {
                return (element as DatePicker).SelectedDate;
            }
            else if (element is CheckBox)
            {
                return (element as CheckBox).IsChecked;
            }
            throw new NotImplementedException("Resolver error!");
        }
    }
}
