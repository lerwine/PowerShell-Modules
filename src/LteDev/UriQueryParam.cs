using System.ComponentModel;

namespace LteDev
{
    public class UriQueryParam : INotifyPropertyChanged
    {
        private int _order = 0;
        private string _key = "";
        private string _value = "";
        private bool _hasValue = false;

        public event PropertyChangedEventHandler PropertyChanged;

		protected internal const string PropertyName_Order = "Order";
		
        [DataObjectField(true, false, false)]
        public int Order
        {
            get { return _order; }
            set
            {
                if (value == _order)
                    return;
                _order = value;
                RaisePropertyChanged(PropertyName_Order);
            }
        }

		protected internal const string PropertyName_Key = "Key";
		
        [DataObjectField(false, false, false)]
        public string Key
        {
            get { return _key; }
            set
            {
                string s = (value == null) ? "" : value;
                if (s == _key)
                    return;
                _key = s;
                RaisePropertyChanged(PropertyName_Key);
            }
        }

		protected internal const string PropertyName_Value = "Value";
		
        [DataObjectField(false, false, false)]
        public string Value
        {
            get { return _value; }
            set
            {
                string s;
				if (value == null)
				{
					if (!_hasValue)
						return;
					s = "";
				} else
					s = value;
                if (s == _value)
                    return;
                _value = s;
				HasValue = true;
                RaisePropertyChanged(PropertyName_Value);
            }
        }

		protected internal const string PropertyName_HasValue = "HasValue";
		
        [DataObjectField(false, false, false)]
        public bool HasValue
        {
            get { return _hasValue; }
            set
            {
                if (value == _hasValue)
                    return;
				if (!value)
					Value = "";
                _hasValue = value;
                RaisePropertyChanged(PropertyName_HasValue);
            }
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

		public UriQueryParam(string key, string value, int order)
		{
			Key = key;
			Value = value;
			Order = order;
		}
		
		public UriQueryParam(string key, int order)
		{
			Key = key;
			Order = order;
		}
		
		public UriQueryParam(string key, string value)
		{
			Key = key;
			Value = value;
		}
		
		public UriQueryParam(string key)
		{
			Key = key;
		}
		
		public UriQueryParam() { }
    }
}