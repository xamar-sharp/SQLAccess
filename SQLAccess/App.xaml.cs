using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Speech;
using SQLAccess.Services;
using Microsoft.Data.SqlClient;
namespace SQLAccess
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static SqlConnection Connection;
        internal static void HandleError(string details, ISpeechLogger logger)
        {
            var msg = $"Произошла ошибка!Детали ошибки: {details}";
            logger.SpeakAsync(msg, false, false);
            MessageBox.Show(msg,"Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }
    }
}
