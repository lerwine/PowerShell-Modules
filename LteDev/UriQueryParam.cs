namespace LteDev
{
    public class UriQueryParam
    {
        public string Key
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

        public string Value
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

        public bool HasValue
        {
            get { return _hasErrors; }
            set
            {
                if (value == _hasErrors)
                    return;
                _hasErrors = value;
                RaisePropertyChanged("HasErrors");
            }
        }

    }
}