using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LteDev
{
    [DataObject()]
    public class ParsedUrl : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
    {
        public const string ErrorMessage_NoHost = "Host not specified";

        public class SegmentsList : BindingList<UriSegment>
        {
            protected override void ClearItems()
            {
                base.ClearItems();
            }

            protected override void InsertItem(int index, UriSegment item)
            {
                base.InsertItem(index, item);
            }

            protected override void OnListChanged(ListChangedEventArgs e)
            {
                base.OnListChanged(e);
            }
            
            protected override void SetItem(int index, UriSegment item)
            {
                base.SetItem(index, item);
            }

            protected override object AddNewCore()
            {
                return base.AddNewCore();
            }
        }

        public class QueryItemList : BindingList<UriQueryParam>
        {
            private ListSortDirection _sortDirection = ListSortDirection.Ascending;
            private PropertyDescriptor _sortProperty;
            private PropertyDescriptor _orderPropertyDescriptor;
            private PropertyDescriptor _keyPropertyDescriptor;
            private PropertyDescriptor _valuePropertyDescriptor;
            private PropertyDescriptor _hasValuePropertyDescriptor;

            protected override ListSortDirection SortDirectionCore { get { return _sortDirection; } }

            protected override PropertyDescriptor SortPropertyCore { get { return _sortProperty; } }

            public QueryItemList(IList<UriQueryParam> list)
                : base(list)
            {
                Initialize();
            }

            public QueryItemList()
                : base()
            {
                Initialize();
            }

            private void Initialize()
            {
                PropertyDescriptorCollection pc = TypeDescriptor.GetProperties(this);
                _orderPropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == "Order");
                _keyPropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == "Key");
                _valuePropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == "Value");
                _hasValuePropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == "HasValue");
                ApplySortCore(_orderPropertyDescriptor, ListSortDirection.Ascending);
            }
            
            protected override void ClearItems()
            {
                base.ClearItems();
            }

            protected override void InsertItem(int index, UriQueryParam item)
            {
                base.InsertItem(index, item);
            }

            protected override void OnListChanged(ListChangedEventArgs e)
            {
                base.OnListChanged(e);
            }

            protected override void SetItem(int index, UriQueryParam item)
            {
                base.SetItem(index, item);
            }

            protected override object AddNewCore()
            {
                return base.AddNewCore();
            }

            protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
            {
                if (prop == null || (_sortProperty != null && prop.Name == _sortProperty.Name && _sortDirection == direction))
                    return;
                List<UriQueryParam> sorted;
                if (prop.Name == _orderPropertyDescriptor.Name)
                {
                    _sortProperty = _orderPropertyDescriptor;
                    if (direction == ListSortDirection.Ascending)
                        sorted = this.OrderBy(p => p.Order).ToList();
                    else
                        sorted = this.OrderByDescending(p => p.Order).ToList();
                }
            }

            protected override int FindCore(PropertyDescriptor prop, object key)
            {
                return base.FindCore(prop, key);
            }

            protected override bool IsSortedCore { get { return true; } }

            protected override void RemoveSortCore()
            {
                base.RemoveSortCore();
            }

            protected override bool SupportsChangeNotificationCore { get { return true; } }

            protected override bool SupportsSearchingCore { get { return true; } }

            protected override bool SupportsSortingCore {  get { return true; } }
        }

        private string _scheme = "";
        private string _userName = null;
        private string _password = null;
        private string _host = "";
        private string _port = null;
        private string _path = "";
        private BindingList<UriSegment> _segments = new BindingList<UriSegment>();
        private BindingList<UriQueryParam> _query = new BindingList<UriQueryParam>();
        private string _fragment = null;
        private UriBuilder _builder = new UriBuilder("https://localhost");
        private Dictionary<string, string> _columnErrors = new Dictionary<string, string>();
        private string _error = "";
        private bool _hasErrors = false;

        [DataObjectField(false, false, false)]
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

        [DataObjectField(false, false, true)]
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

        [DataObjectField(false, false, true)]
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

        [DataObjectField(false, false, false)]
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

        [DataObjectField(false, false, true)]
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

        [DataObjectField(false, false, false)]
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
        
        [DataObjectField(false, false, true)]
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

        [DataObjectField(false, false, false)]
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

        [DataObjectField(false, false, false)]
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

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public BindingList<UriQueryParam> GetQuery() { return _query; }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public BindingList<UriSegment> GetSegments() { return _segments; }

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
