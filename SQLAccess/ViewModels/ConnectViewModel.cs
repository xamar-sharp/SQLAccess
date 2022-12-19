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
        public static bool IsAuthenticated = false;
        public string ConnectionString { get => $"Server={_server};Database={_database};User ID={_login};Password={_password};TrustServerCertificate=TRUE"; }
        public ICommand ConnectCommand { get; set; }
        public ConnectViewModel(ISpeechLogger logger,Window dialog)
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
                    dialog.DialogResult = true;
                    dialog.Hide();
                    _logger.SpeakAsync("Успешное соединение!", false, false);
                }
                catch(Exception ex)
                {
                    App.HandleError("Проблема соединения", _logger);
                    dialog.DialogResult = false;
                }
            }, this.WhenAny<ConnectViewModel,bool,string,string,string,string>(c => c.Server, c => c.Database, c => c.Login, c => c.Password, (server, db, login, password) =>
            {
                return !(string.IsNullOrEmpty(server.GetValue()) || string.IsNullOrEmpty(db.GetValue()) || string.IsNullOrEmpty(login.GetValue()) || string.IsNullOrEmpty(password.GetValue()));
            }));
        }
    }
}
