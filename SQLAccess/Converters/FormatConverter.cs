using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Data;
namespace SQLAccess.Converters
{
    public sealed class FormatConverter : IValueConverter
    {
        public object Convert(object path, Type target, object param, CultureInfo info)
        {
            var unformat = ViewModels.ConnectViewModel.IsAuthenticated;
            if (target == typeof(ImageSource))
            {
                Uri uri = new Uri(unformat ? "success.png" : "fail.png", UriKind.Relative);
                return new BitmapImage(uri);
            }
            else
            {
                return unformat ? "Подключено" : "Нет соединения";
            }
        }
        public object ConvertBack(object path, Type target, object param, CultureInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
