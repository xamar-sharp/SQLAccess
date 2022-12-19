using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using System.IO;
namespace SQLAccess.Services
{
    public sealed class DataGridColumnMapper : IDataGridColumnMapper
    {
        public DataGridColumn Map(object value, string propertyBinding)
        {
            var type = value.GetType();
            var binding = new Binding() { Path = new PropertyPath(propertyBinding), Mode = BindingMode.TwoWay };
            if (type == typeof(Boolean))
            {
                return new DataGridCheckBoxColumn() { Binding = binding, Header = propertyBinding };
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime date))
            {
                var markup = "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" DataType=\"{x:Type DatePicker}\"><DatePicker DisplayDate=\"" + "{" + $"Binding Path = {propertyBinding}, Mode=OneWay" +
                    "}" + "\" SelectedDate=\"" + "{" + $"Binding Path = {propertyBinding}, Mode=TwoWay" +
                    "}" + "\"></DatePicker></DataTemplate>";
                return new DataGridTemplateColumn() { CellTemplate = XamlReader.Load(XmlReader.Create(new StringReader(markup))) as DataTemplate, Header = propertyBinding };
            }
            else
            {
                if (Uri.IsWellFormedUriString(value as string, UriKind.Absolute))
                {
                    return new DataGridHyperlinkColumn() { Binding = binding, Header = propertyBinding };
                }
                else
                {
                    return new DataGridTextColumn() { Binding = binding, Header = propertyBinding };
                }
            }
        }
    }
}
