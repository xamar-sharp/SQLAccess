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
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using SQLAccess.Services;
using ReactiveUI;
namespace SQLAccess.ViewModels
{
#nullable disable
    public sealed class SelectViewModel : ReactiveObject
    {
        private string _query;
        private ObservableCollection<dynamic> _results = new ObservableCollection<dynamic>(new List<dynamic>(2));
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
        public ObservableCollection<dynamic> Results
        {
            get => _results;
            set
            {
                this.RaiseAndSetIfChanged(ref _results, value);
            }
        }
        public object SelectedRow { get; set; }
        public ICommand QueryCommand { get; set; }
        public ICommand PushCommand { get; set; }
        public ICommand DropRegisterCommand { get; set; }
        internal SelectViewModel AddUpdateChange(string rowId, string columnName, object value)
        {
            string formatValue = _formatter.FormatSql(value);
            _updateQueries.Add($"UPDATE {MainWindow.CurrentQueryTable} SET {columnName} = {formatValue} WHERE {MainWindow.CurrentPrimaryColumn} = {(Guid.TryParse(rowId, out Guid res) ? $"'{res}'" : Convert.ToInt64(rowId))}");
            return this;
        }
        internal SelectViewModel AddDropChange(string rowId)
        {
            _updateQueries.Add($"DELETE FROM {MainWindow.CurrentQueryTable} WHERE {MainWindow.CurrentPrimaryColumn} = {(Guid.TryParse(rowId, out Guid res) ? $"'{res}'" : Convert.ToInt64(rowId))}");
            return this;
        }
        private string FindTableName(string query)
        {
            int start = query.IndexOf("FROM");
            var sub = query.Substring(start);
            var sec = sub.IndexOf(" ");
            int realStart = start + sec + 1;
            var end = realStart + query.Substring(realStart).IndexOf(" ");
            return query[realStart..end];
        }
        public SelectViewModel(DataGrid target,TextBlock tableName, IValueFormatter formatter, ISpeechLogger logger, IDataGridColumnMapper mapper)
        {
            _formatter = formatter;
            _logger = logger;
            _mapper = mapper;
            DropRegisterCommand = ReactiveCommand.CreateFromTask<Func<Task<Object>>>(async (obj) =>
            {
                var id = await obj();
                try
                {
                    Results.Remove(SelectedRow);
                    AddDropChange(id.ToString());
                }
                catch (Exception ex)
                {
                    App.HandleError("Удаление строки", _logger);
                }
            }, this.WhenAny(e => e.Results, e => e.SelectedRow, (res, sel) => true));
            QueryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    _updateQueries.Clear();
                    Results.Clear();
                    if(!Query.EndsWith(" "))
                    {
                        Query += " ";
                    }
                    SqlCommand command = new SqlCommand(Query, App.Connection);
                    using var reader = await command.ExecuteReaderAsync();
                    target.Columns?.Clear();
                    bool read = false;
                    var coll = reader.GetColumnSchema();
                    var tab = reader.GetSchemaTable();
                    MainWindow.CurrentQueryTable = FindTableName(Query);
                    tableName.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
                    MainWindow.CurrentPrimaryColumn = tab.PrimaryKey.FirstOrDefault()?.ColumnName ?? coll.FirstOrDefault(cl => cl.ColumnName.Contains("Id")).ColumnName;
                    while (await reader.ReadAsync())
                    {
                        dynamic previous = new ExpandoObject();
                        for (int columnIndex = 0; columnIndex < reader.FieldCount; columnIndex++)
                        {
                            if (!read)
                            {
                                target.Columns.Add(_mapper.Map(reader[columnIndex], coll[columnIndex].ColumnName));
                            }
                            (previous as IDictionary<string, object>).Add(new KeyValuePair<string, object>(coll[columnIndex].ColumnName, reader[columnIndex]));
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
            }, this.WhenAny(e => e.Query, (e) => !string.IsNullOrEmpty(e.Value)));
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
                catch (Exception ex)
                {
                    App.HandleError("Применение изменений", _logger);
                }
            });
        }
    }
}
