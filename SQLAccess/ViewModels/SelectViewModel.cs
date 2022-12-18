using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Dynamic;
using Microsoft.Data.SqlClient;
using SQLAccess.Services;
using ReactiveUI;
namespace SQLAccess.ViewModels
{
#nullable disable
    public sealed class SelectViewModel : ReactiveObject
    {
        private string _query;
        private IList _results;
        private List<string> _updateQueries = new List<string>(4);
        private string _sumUpdateQuery { get => this._updateQueries.Aggregate((first, sec) => first + ";" + sec); }
        private readonly IValueFormatter _formatter;
        private readonly ISpeechLogger _logger;
        private readonly IDataGridColumnMapper _mapper;
        public string Query
        {
            get => _query;
            set
            {
                this.RaiseAndSetIfChanged(ref _query, value);
            }
        }
        public IList Results
        {
            get => _results;
            set
            {
                this.RaiseAndSetIfChanged(ref _results, value);
            }
        }
        public object SelectedRow { get; set; }//One way to source binding (SelectedItem property У DataGrid)
        public ICommand QueryCommand { get; set; }
        public ICommand PushCommand { get; set; }
        public ICommand DropCommand { get; set; }
        internal SelectViewModel AddUpdateChange(Guid rowId, string columnName, object value)
        {
            string formatValue = _formatter.FormatSql(value);
            _updateQueries.Add($"UPDATE {MainWindow.CurrentQueryTable} SET {columnName} = {formatValue} WHERE Id = '{rowId}'");
            return this;
        }
        internal SelectViewModel AddDropChange(Guid rowId)
        {
            _updateQueries.Add($"DELETE FROM {MainWindow.CurrentQueryTable} WHERE Id = '{rowId}'");
            return this;
        }
        internal string ComputeQueryTable()
        {
            var start = Query.LastIndexOf("FROM") + 5;
            var end = Query.Substring(start).IndexOf(" ");
            return Query.Substring(start, end - start);
        }
        public SelectViewModel(DataGrid target, IValueFormatter formatter, ISpeechLogger logger, IDataGridColumnMapper mapper)
        {
            _formatter = formatter;
            _logger = logger;
            _mapper = mapper;
            DropCommand = ReactiveCommand.Create<object>((obj) =>
            {
                try
                {
                    Results.Remove(SelectedRow);
                    AddDropChange(Guid.Parse(obj.ToString()));
                    _logger.SpeakAsync("Одна строка была удалена!", false, true);
                }
                catch (Exception ex)
                {
                    App.HandleError("Удаление строки", _logger);
                }
            }, this.WhenAny(e => e.Results, (res) => res.Value is not null && res.Value.Count > 0 && SelectedRow is not null && ConnectViewModel.IsAuthenticated));
            QueryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    _updateQueries.Clear();
                    Results.Clear();
                    MainWindow.CurrentQueryTable = ComputeQueryTable();
                    SqlCommand command = new SqlCommand(Query, App.Connection);
                    using var reader = await command.ExecuteReaderAsync();
                    target.AutoGenerateColumns = false;
                    target.Columns.Clear();
                    bool read = false;
                    while (await reader.ReadAsync())
                    {
                        dynamic previous = new ExpandoObject();
                        var coll = reader.GetColumnSchema();
                        for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
                        {
                            if (!read)
                            {
                                target.Columns.Add(_mapper.Map(reader[columnIndex], coll[columnIndex].ColumnName));
                            }
                            previous.Add(new KeyValuePair<string, object>(coll[columnIndex].ColumnName, reader[columnIndex]));
                        }
                        read = true;
                        Results.Add(previous);
                    }
                    var msg = "Данные были успешно получены!";
                    _logger.SpeakAsync(msg, false, true);
                }
                catch (Exception ex)
                {
                    App.HandleError("Запрос данных", _logger);
                }
            }, this.WhenAny(e => e.Query, (e) => !string.IsNullOrEmpty(e.Value) && ConnectViewModel.IsAuthenticated));
            PushCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                int sum = 0;
                try
                {
                    foreach (var command in _updateQueries.Select(e => new SqlCommand(e, App.Connection)))
                    {
                        sum += await command.ExecuteNonQueryAsync();
                    }
                    var msg = $"Было изменено {sum} строк!";
                    _logger.SpeakAsync(msg, false, true);
                }
                catch(Exception ex)
                {
                    App.HandleError("Применение изменений", _logger);
                }
            }, this.WhenAny(e => e._updateQueries, queries => queries.Value.Count > 0 && ConnectViewModel.IsAuthenticated));
        }
    }
}
