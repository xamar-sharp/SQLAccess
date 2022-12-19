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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQLAccess.ViewModels;
using SQLAccess.Services;
#nullable disable
namespace SQLAccess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CurrentQueryTable = "Table";
        internal static string CurrentPrimaryColumn = "Id";
        private readonly IValueResolver _resolver;
        public SelectViewModel SelectModel { get; set; }
        public PostViewModel PostModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            _resolver = new ValueResolver();
            SelectModel = new SelectViewModel(output, new ValueFormatter(), new SpeechLogger(), new DataGridColumnMapper());
            PostModel = new PostViewModel(new SpeechLogger());
            output.AddContextAction("Удалить строку", "drop.png", SelectModel.DropCommand, async () =>
            {
                var index = output.SelectedIndex;
                return await Task.Run(() =>
                {
                    var obj = SelectModel.Results[index];
                    var dict = (IDictionary<string, object>)obj;
                    return dict[MainWindow.CurrentPrimaryColumn];
                });
            });
            this.DataContext = this;
        }
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var obj = SelectModel.Results[e.Row.GetIndex()];
                var dict = (IDictionary<string, object>)obj;
                var id = dict[MainWindow.CurrentPrimaryColumn];
                SelectModel.AddUpdateChange(id.ToString(), ((e.Column as DataGridBoundColumn).Binding as Binding).Path.Path, _resolver.GetValue(e.EditingElement));
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConnectionDialog();
            if (dialog.ShowDialog() == true)
            {
                statusImage.GetBindingExpression(Image.SourceProperty).UpdateTarget();
                statusText.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var dict = output.SelectedItem as IDictionary<string, object>;
            var id = dict[MainWindow.CurrentPrimaryColumn];
            var value = (sender as DatePicker).SelectedDate;
            SelectModel.AddUpdateChange(id.ToString(), dict.Keys.FirstOrDefault(key => (DateTime)dict[key] == value.Value), value);
        }

    }
}
