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
                if(int.TryParse(value.ToString(),out int res))
                {
                    return $"{res}";
                }
                return $"'{value as string}'";// even Guid!!
            }
            throw new NotImplementedException("Value formatter!");
        }
    }
}
