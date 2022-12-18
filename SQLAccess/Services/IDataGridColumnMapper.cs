using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace SQLAccess.Services
{
    public interface IDataGridColumnMapper
    {
        DataGridColumn Map(object value, string headerForTemplate);
    }
}
