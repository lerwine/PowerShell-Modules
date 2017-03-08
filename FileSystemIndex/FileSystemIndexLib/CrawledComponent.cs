using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Erwine.Leonard.T.GDIPlus
{
    public abstract class CrawledComponent<TKey> : ICrawledComponent<TKey>
		where TKey : IComparable
    {
        private TKey _key;
		private ICrawlComponentContainer<TKey> _parent;
		private static IEqualityComparer<TKey> _keyComparer;
		private ComponentPropertyDictionary _propertyDictionary;
		
		protected const string PropertyName_Properties = "Properties";
		
		public virtual ComponentPropertyDictionary Properties
		{
			if ((_propertyDictionary = CoerceProperties(null)) == null)
				_propertyDictionary = new ComponentPropertyDictionary();
			_propertyDictionary.Initialize(this);
			get { return _propertyDictionary; }
			set
			{
				ComponentPropertyDictionary oldValue = _propertyDictionary;
				ComponentPropertyDictionary newValue = value;
				if (newValue = null && (newValue = CoerceProperties(newValue)) == null)
					newValue = new ComponentPropertyDictionary();
				else
				{
					if (ReferenceEquals(oldValue, newValue))
						return;
					if (newValue = CoerceProperties(newValue)) == null)
						newValue = new ComponentPropertyDictionary();
					else if (ReferenceEquals(oldValue, newValue))
						return;
				}
				RaisePropertyChanging(PropertyName_Properties, oldValue, newValue);
				_propertyDictionary = newValue;
				RaisePropertyChanged(PropertyName_Properties, oldValue, newValue);
			}
		}
		
		protected virtual ComponentPropertyDictionary CoerceProperties(ComponentPropertyDictionary properties) { return properties; }
		
		static CrawledComponent() { _keyComparer = EqualityComparer<TKey>.Default; }
		
		protected CrawledComponent()
		{
            throw new NotImplementedException();
		}
		
		protected CrawledComponent(CrawledComponent<TKey> toClone, ICrawlComponentContainer<TKey> parent) : this((toClone == null) ? default(TKey) : toClone.Key, parent) { }
		
		protected CrawledComponent(TKey key, ICrawlComponentContainer<TKey> parent)
		{
		    _key = key;
		    _parent = parent;
		}
		
        protected abstract CrawledComponent<TKey> CreateClone(ICrawlComponentContainer<TKey> parent);
		
		#region ICrawledComponent<TKey> Implementation
		
		protected virtual TKey Key
		{
			get { return _key; }
			set
			{
				TKey oldValue = _key;
				if (_keyComparer.Equals(value, oldValue))
					return;
				OnKeyChanging(oldValue, value);
				_key = value;
				OnKeyChanged(oldValue, value);
			}
		}
		
		protected virtual TKey CoerceKey(TKey value) { return value; }
		
		protected virtual void OnKeyChanging(TKey oldValue, TKey newValue) { }
		
		protected virtual void OnKeyChanged(TKey oldValue, TKey newValue) { }
		
		TKey ICrawledComponent<TKey>.Key
		{
			get { return Key; }
			set { Key = value; }
		}
		
		protected ICrawlComponentContainer<TKey> Parent
		{
			get { return _parent; }
			set
			{
				ICrawlComponentContainer<TKey> oldValue = _parent;
                if (ReferenceEquals(value, oldValue))
                    return;
				OnParentChanging(oldValue, value);;
				_parent = value;
				OnParentChanged(oldValue, value);
			}
		}
		
		protected virtual ICrawlComponentContainer<TKey> CoerceParent(ICrawlComponentContainer<TKey> parent) { return parent; }
		
		protected void OnParentChanging(ICrawlComponentContainer<TKey> oldValue, ICrawlComponentContainer<TKey> newValue) { }
		
		protected void OnParentChanged(ICrawlComponentContainer<TKey> oldValue, ICrawlComponentContainer<TKey> newValue) { }
		
		ICrawlComponentContainer<TKey> ICrawledComponent<TKey>.Parent
		{
			get { return Parent; }
			set { Parent = value; }
		}
		
        ICrawledComponent<TKey> ICrawledComponent<TKey>.Clone(ICrawlComponentContainer<TKey> parent) { return CreateClone(parent); }
        ICrawledComponent<TKey> ICrawledComponent<TKey>.Clone() { return CreateClone(Parent); }
		
		#region ICloneable Implementation
		
        object ICloneable.Clone() { return CreateClone(Parent); }
		
		#endregion
		
		#region INotifyPropertyChanging Implementation
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		protected void RaisePropertyChanging(string propertyName)
		{
			PropertyChangingEventArgs args = new PropertyChangingEventArgs(propertyName);
			try { OnPropertyChanging(args, null, null); }
			finally
			{
				try { OnPropertyChanging(args); }
				finally
				{
					PropertyChangingEventHandler propertyChanging = PropertyChanging;
					if (propertyChanging != null)
						propertyChanging(this, args);
				}
			}
		}
		
		protected void RaisePropertyChanging(string propertyName, object oldValue, object newValue)
		{
			PropertyChangingEventArgs args = new PropertyChangingEventArgs(propertyName);
			try { OnPropertyChanging(args, oldValue, newValue); }
			finally
			{
				try { OnPropertyChanging(args); }
				finally
				{
					PropertyChangingEventHandler propertyChanging = PropertyChanging;
					if (propertyChanging != null)
						propertyChanging(this, args);
				}
			}
		}
		
		protected virtual void OnPropertyChanging(PropertyChangingEventArgs args) { }
		
		protected virtual void OnPropertyChanging(PropertyChangingEventArgs args, object oldValue, object newValue) { }
		
		#endregion
		
		#region INotifyPropertyChanged Implementation
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
			try { OnPropertyChanged(args, null, null); }
			finally
			{
				try { OnPropertyChanged(args); }
				finally
				{
					PropertyChangedEventHandler propertyChanged = PropertyChanged;
					if (propertyChanged != null)
						propertyChanged(this, args);
				}
			}
		}
		
		protected void RaisePropertyChanged(string propertyName, object oldValue, object newValue)
		{
			PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
			try { OnPropertyChanged(args, oldValue, newValue); }
			finally
			{
				try { OnPropertyChanged(args); }
				finally
				{
					PropertyChangedEventHandler propertyChanged = PropertyChanged;
					if (propertyChanged != null)
						propertyChanged(this, args);
				}
			}
		}
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }
		
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args, object oldValue, object newValue) { }
		
		#endregion
		
		#endregion
	}
}