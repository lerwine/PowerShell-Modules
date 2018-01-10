using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteDev
{
    public class ParsedUrl : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
    {
        public const string ErrorMessage_NoHost = "Host not specified";

        private string _scheme = "https";
        private string _userName = null;
        private string _password = null;
        private string _host = "";
        private string _port = null;
        private string _path = "";
        private BindingList<UriQueryParam> _query = new BindingList<UriQueryParam>();
        private string _fragment = null;
        private UriBuilder _builder = new UriBuilder("https://localhost");
        private Dictionary<string, string> _columnErrors = new Dictionary<string, string>();
        private string _error = "";
        private bool _hasErrors = false;

        public string Scheme
        {
            get { return _scheme; }
            private set
            {
                string s = (value == null) ? "" : value;
                if (s == _scheme)
                    return;
                _scheme = s;
                RaisePropertyChanged("Scheme");
            }
        }

        public string UserName
        {
            get { return _userName; }
            private set
            {
                string s = _userName;
                if ((value == null) ? s == null : s != null && s == value)
                    return;
                _userName = s;
                RaisePropertyChanged("UserName");
            }
        }

        public string Password
        {
            get { return _password; }
            private set
            {
                string s = _password;
                if ((value == null) ? s == null : s != null && s == value)
                    return;
                _password = s;
                RaisePropertyChanged("Password");
            }
        }

        public string Host
        {
            get { return _host; }
            private set
            {
                string s = (value == null) ? "" : value;
                if (s == _host)
                    return;
                _host = s;
                RaisePropertyChanged("Host");
            }
        }

        public string Port
        {
            get { return _port; }
            private set
            {
                string s = _port;
                if ((value == null) ? s == null : s != null && s == value)
                    return;
                _port = s;
                RaisePropertyChanged("Port");
            }
        }

        public string Path
        {
            get { return _path; }
            private set
            {
                string s = (value == null) ? "" : value;
                if (s == _path)
                    return;
                _path = s;
                RaisePropertyChanged("Path");
            }
        }

        public BindingList<UriQueryParam> Query { get { return _query; } }

        public string Fragment
        {
            get { return _fragment; }
            private set
            {
                string s = _fragment;
                if ((value == null) ? s == null : s != null && s == value)
                    return;
                _fragment = s;
                RaisePropertyChanged("Fragment");
            }
        }

        public UriBuilder Builder { get { return _builder; } }

        public string Error
        {
            get { return _error; }
            private set
            {
                string s = (value == null) ? "" : value;
                if (s == _error)
                    return;
                _error = s;
                RaisePropertyChanged("Error");
            }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            private set
            {
                if (value == _hasErrors)
                    return;
                _hasErrors = value;
                RaisePropertyChanged("HasErrors");
            }
        }

        public string this[string columnName]
        {
            get { return _columnErrors[columnName]; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            string s = _columnErrors[propertyName];
            if (String.IsNullOrEmpty(s))
                return new string[0];
            return new string[] { s };
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            try { OnPropertyChanged(args); }
            finally
            {
                PropertyChangedEventHandler propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                    propertyChanged(this, args);
            }
        }
    }
}
