using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SQLAccess.ViewModels;
using SQLAccess.Services;
namespace SQLAccess
{
    /// <summary>
    /// Логика взаимодействия для ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        public ConnectViewModel Model { get; set; }
        public ConnectionDialog()
        {
            InitializeComponent();
            this.DataContext = Model = new ConnectViewModel(new SpeechLogger(), this);
        }
    }
}
