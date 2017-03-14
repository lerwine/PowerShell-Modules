using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace FileSystemIndexLib
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class CrawledComponent<TKey> : ICrawledComponent<TKey>
        where TKey : IComparable
    {
        private TKey _key;
        private ICrawlComponentContainer<TKey> _parent;
        private static IEqualityComparer<TKey> _keyComparer;
        private ComponentPropertyDictionary<TKey> _propertyDictionary;
        
        /// <summary>
        /// 
        /// </summary>
        protected const string PropertyName_Properties = "Properties";
        
        /// <summary>
        /// 
        /// </summary>
        public virtual ComponentPropertyDictionary<TKey> Properties
        {
            get { return _propertyDictionary; }
            set
            {
                ComponentPropertyDictionary<TKey> oldValue = _propertyDictionary;
                ComponentPropertyDictionary<TKey> newValue = value;
                if (newValue == null && (newValue = CoerceProperties(newValue)) == null)
                    newValue = new ComponentPropertyDictionary<TKey>();
                else
                {
                    if (ReferenceEquals(oldValue, newValue))
                        return;
                    if ((newValue = CoerceProperties(newValue)) == null)
                        newValue = new ComponentPropertyDictionary<TKey>();
                    else if (ReferenceEquals(oldValue, newValue))
                        return;
                }
                RaisePropertyChanging(PropertyName_Properties, oldValue, newValue);
                _propertyDictionary = newValue;
                RaisePropertyChanged(PropertyName_Properties, oldValue, newValue);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected virtual ComponentPropertyDictionary<TKey> CoerceProperties(ComponentPropertyDictionary<TKey> properties) { return properties; }
        
        static CrawledComponent() { _keyComparer = EqualityComparer<TKey>.Default; }

        /// <summary>
        /// 
        /// </summary>
        protected CrawledComponent()
        {
            if ((_propertyDictionary = CoerceProperties(null)) == null)
                _propertyDictionary = new ComponentPropertyDictionary<TKey>();
            _propertyDictionary.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toClone"></param>
        /// <param name="parent"></param>
        protected CrawledComponent(CrawledComponent<TKey> toClone, ICrawlComponentContainer<TKey> parent) : this((toClone == null) ? default(TKey) : toClone.Key, parent) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        protected CrawledComponent(TKey key, ICrawlComponentContainer<TKey> parent)
        {
            _key = key;
            _parent = parent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual TKey CoerceKey(TKey value) { return value; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnKeyChanging(TKey oldValue, TKey newValue) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnKeyChanged(TKey oldValue, TKey newValue) { }
        
        TKey ICrawledComponent<TKey>.Key
        {
            get { return Key; }
            set { Key = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        protected virtual ICrawlComponentContainer<TKey> CoerceParent(ICrawlComponentContainer<TKey> parent) { return parent; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected void OnParentChanging(ICrawlComponentContainer<TKey> oldValue, ICrawlComponentContainer<TKey> newValue) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs args) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs args, object oldValue, object newValue) { }
        
        #endregion
        
        #region INotifyPropertyChanged Implementation
        
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args, object oldValue, object newValue) { }
        
        #endregion
        
        #endregion
    }
}