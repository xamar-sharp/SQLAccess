using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLAccess.Services
{
#nullable disable
    public sealed class ValueFormatter : IValueFormatter
    {
        public string FormatSql(object value)
        {
            Type type = value.GetType();
            if(type == typeof(Boolean))
            {
                return ((bool)value).ToString();
            }
            else if(type == typeof(DateTime))
            {
                return $"'{((DateTime)value)}'";
            }
            else if (type==typeof(string)){
                return $"'{value}'";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
