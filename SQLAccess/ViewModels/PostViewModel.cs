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
                    var res = await command.ExecuteNonQueryAsync();
                    if(res == -1)
                    {
                        res = 0;
                        _logger.SpeakAsync("Неверная команда, используйте команду INSERT!", false, true);
                    }
                    else if(res != 0)
                    {
                        _logger.SpeakAsync("Элементы успешно вставлены в таблицу!", false, true);
                    }
                    else
                    {
                        _logger.SpeakAsync("Ни один эдемент не был добавлен в таблицу!!", false, true);
                    }
                    AddedItems = res;
                    LastExecuteDate = DateTime.Now;
                }
                catch (Exception ex)
                {
                    App.HandleError("Вставка данных!", _logger);
                }
            }, this.WhenAny(e => e.Query, (q) => !string.IsNullOrEmpty(q.Value)));
        }
    }
}
