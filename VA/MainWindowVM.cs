using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Media.Media3D;
using MahApps.Metro.Controls.Dialogs;

namespace VA
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private DataTable _db;
        private string _login;
        private string _pw;
        private string _serverAnswer;
        public event PropertyChangedEventHandler PropertyChanged;
        private Client client;
        private int _reportType;
        private DataWorker dw;

        private bool _isChecked1;
        private bool _isChecked2;
        private bool _isChecked3;
        private bool _isChecked4;
        private bool _isChecked5;
        private bool _isRegistr = false;
        private string acsessType = "u";
        private RelayCommand login;
        private RelayCommand registr;
        private RelayCommand rep;
        private string _param;
        private bool _dbEnable=false;

        public RelayCommand SignIn
        {
            get
            {
                return registr ??
                       (registr = new RelayCommand(obj => LoginUser()));
            }
        }
        public RelayCommand Report
        {
            get
            {
                return rep ??
                       (rep = new RelayCommand(obj => GetReport()));
            }
        }
        public RelayCommand SignUp
        {
            get
            {
                return login ??
                       (login = new RelayCommand(obj => RegisterUser()));
            }
        }

        public bool IsRegistr
        {
            get => _isRegistr;
            set
            {
                _isRegistr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isRegistr)));
            }
        }

        public bool IsChecked1
        {
            get => _isChecked1;
            set
            {
                _isChecked1 = value;
                _reportType = 1;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isChecked1)));
            }
        }

        public bool IsChecked2
        {
            get => _isChecked2;
            set
            {
                _isChecked2 = value;
                _reportType = 2;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isChecked2)));
            }
        }

        public bool IsChecked3
        {
            get => _isChecked3;
            set
            {
                _isChecked3 = value;
                _reportType = 3;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isChecked3)));
            }
        }

        public bool IsChecked4
        {
            get => _isChecked4;
            set
            {
                _isChecked4 = value;
                _reportType = 4;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isChecked4)));
            }
        }

        public bool IsChecked5
        {
            get => _isChecked5;
            set
            {
                _isChecked5 = value;
                _reportType = 5;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_isChecked5)));
            }
        }

        public string Param
        {
            get => _param;
            set
            {
                _param = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Login)));
            }
        }

        public bool dbEnable
        {
            get => _dbEnable;
            set
            {
                _dbEnable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(dbEnable)));
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                if (Login == "Admin")
                    acsessType = "a";
                else
                {
                    acsessType = "u";
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Login)));
            }
        }

        public string Passwd
        {
            get => _pw;
            set
            {
                _pw = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Passwd)));
            }
        }

        public string ServerAnswer
        {
            get => _serverAnswer;
            set
            {
                _serverAnswer = value;
                if (Client.IsSault)
                {
                    Client.ReqestTypeSwither();
                    if (!IsRegistr)
                        client.Login(Login, Passwd + ServerAnswer);
                    else
                    {
                        client.Register(Login, Passwd + ServerAnswer, "u");
                    }

                    
                }
                else if (Client.IsEnter)
                {
                    if (ServerAnswer == "True")
                    {
                        StreamWriter sr = new StreamWriter("acountData.txt");
                        sr.WriteLine(Login);
                        sr.WriteLine(Passwd);
                        sr.Close();
                        Client.ReqestTypeSwither();
                        MessageBox.Show("Enter successful");
                        dbEnable = true;
                    }
                    else
                        MessageBox.Show("Enter faild, wrong login or password");
                }
                else if (ServerAnswer == "done" && Client.IsReport)
                {
                    Client.ReqestTypeSwither();
                }
            }
        }

        public DataTable Db
        {
            get => _db;
            set
            {
                _db = value;
                if (_reportType != 1)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Db)));
                else
                {
                    dw.createTableExel(Db);
                }
                Client.ReqestTypeSwither();
            }
        }

        public MainWindowVM()
        {
            client = Client.сlient(this);
            dw = new DataWorker(this);
        }


        private void LoginUser()
        {
            client.GetSault(Login);
        }

        private void RegisterUser()
        {
            client.GetSault();
        }

        private void GetReport()
        {
            if (IsChecked1)
                client.GetReport("1", acsessType);
            if (IsChecked2)
                client.GetReport("2", "");
            if (IsChecked3)
                client.GetReport("3", Param);
            if (IsChecked4)
                client.GetReport("4", Client.GetMac());
            if (IsChecked5)
                client.GetReport("5", "");
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}