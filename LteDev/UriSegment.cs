using System.ComponentModel;

namespace LteDev
{
    public class UriSegment : INotifyPropertyChanged
    {
        private int _order = 0;
        private string _name = "";

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

		protected internal const string PropertyName_Name = "Name";
		
        [DataObjectField(false, false, false)]
        public string Name
        {
            get { return _name; }
            private set
            {
                string s = (value == null) ? "" : value;
                if (s == _name)
                    return;
                _name = s;
                RaisePropertyChanged(PropertyName_Name);
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
		
		public UriSegment(string name, int order)
		{
			Name = name;
			Order = order;
		}
		
		public UriSegment(string name)
		{
			Name = name;
		}
		
		public UriSegment() { }
    }
}