using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Data;
using Microsoft.Data.SqlClient;
using SQLAccess.Services;
using ReactiveUI;
namespace SQLAccess.ViewModels
{
#nullable disable
    public sealed class ConnectViewModel : ReactiveObject
    {
        private string _server;
        private string _database;
        private string _login;
        private string _password;
        private readonly ISpeechLogger _logger;
        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }
        public string Database
        {
            get => _database;
            set => this.RaiseAndSetIfChanged(ref _database, value);
        }

        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
        internal static bool IsAuthenticated = false;
        public string ConnectionString { get => $"Server={_server};Database={_database};User ID={_login};Password={_password};TrustServerCertificate=TRUE"; }
        public ICommand ConnectCommand { get; set; }
        public ConnectViewModel(ISpeechLogger logger)
        {
            _logger = logger;   
            ConnectCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (App.Connection is not null)
                {
                    await App.Connection.DisposeAsync();
                    IsAuthenticated = false;
                }
                App.Connection = new SqlConnection(ConnectionString);
                try
                {
                    await App.Connection.OpenAsync();
                    IsAuthenticated = true;
                    _logger.SpeakAsync("Успешное соединение!", false, true);
                }
                catch(Exception ex)
                {
                    App.HandleError("Проблема соединения", _logger);
                }
            }, this.WhenAny(c => c.Server, c => c.Database, c => Login, c => c.Password, (server, db, login, password) =>
            {
                return !(string.IsNullOrWhiteSpace(server.Value) || string.IsNullOrWhiteSpace(db.Value) || string.IsNullOrWhiteSpace(login.Value) || string.IsNullOrWhiteSpace(password.Value));
            }));
        }
    }
}
