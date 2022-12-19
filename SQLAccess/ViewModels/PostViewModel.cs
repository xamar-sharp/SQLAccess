using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using SQLAccess.Services;
using ReactiveUI;
namespace SQLAccess.ViewModels
{
#nullable disable
    public sealed class PostViewModel : ReactiveObject
    {
        private string _query;
        private DateTime _date = DateTime.MinValue;
        private int _added = 0;
        private readonly ISpeechLogger _logger;
        public string Query
        {
            get => _query;
            set
            {
                this.RaiseAndSetIfChanged(ref _query, value);
            }
        }
        public DateTime LastExecuteDate
        {
            get => _date;
            set
            {
                this.RaiseAndSetIfChanged(ref _date, value);
            }
        }
        public int AddedItems
        {
            get => _added; set
            {
                this.RaiseAndSetIfChanged(ref _added, value);
            }
        }
        public ICommand ExecuteCommand { get; set; }
        public PostViewModel(ISpeechLogger logger)
        {
            _logger = logger;
            ExecuteCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    SqlCommand command = new SqlCommand(Query, App.Connection);
                    AddedItems = await command.ExecuteNonQueryAsync();
                    LastExecuteDate = DateTime.Now;
                    _logger.SpeakAsync("Отправка данных успешно выполнена!", false, true);
                }
                catch (Exception ex)
                {
                    App.HandleError("Вставка данных!", _logger);
                }
            }, this.WhenAny(e => e.Query, (q) => !string.IsNullOrEmpty(q.Value)));
        }
    }
}
