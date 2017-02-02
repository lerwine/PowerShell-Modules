using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace CredentialStorageLibrary
{
    public class LinkVM : DependencyObject, INestedDependencyObject<LinkVM, CredentialVM>
    {
        #region Id Property Members

        public const string PropertyName_Id = "Id";

        private static readonly DependencyPropertyKey IdPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Id, typeof(Guid), typeof(LinkVM),
                new PropertyMetadata(default(Guid)));

        /// <summary>
        /// Identifies the <seealso cref="Id"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty = IdPropertyKey.DependencyProperty;

        /// <summary>
        /// Unique Identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                if (CheckAccess())
                    return (Guid)(GetValue(IdProperty));
                return (Guid)(Dispatcher.Invoke(new Func<Guid>(() => Id)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(IdPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => Id = value));
            }
        }

        #endregion

        #region DisplayUriText Property Members

        public const string PropertyName_DisplayUriText = "DisplayUriText";

        private static readonly DependencyPropertyKey DisplayUriTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DisplayUriText, typeof(string), typeof(LinkVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="DisplayUriText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayUriTextProperty = DisplayUriTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Display text for full URI
        /// </summary>
        public string DisplayUriText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayUriTextProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => DisplayUriText)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DisplayUriTextPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DisplayUriText = value));
            }
        }

        #endregion

        #region Scheme Property Members

        public const string DependencyPropertyName_Scheme = "Scheme";

        /// <summary>
        /// Identifies the <seealso cref="Scheme"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SchemeProperty = DependencyProperty.Register(DependencyPropertyName_Scheme, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Scheme_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Scheme_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Scheme_CoerceValue(baseValue)));

        /// <summary>
        /// Scheme of link URI.
        /// </summary>
        public string Scheme
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(SchemeProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Scheme)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(SchemeProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Scheme = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Scheme"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Scheme"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Scheme"/> property was changed.</param>
        protected virtual void Scheme_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Scheme_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Scheme"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Scheme_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Scheme_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region IncludeCredentialsInUri Property Members

        public const string DependencyPropertyName_IncludeCredentialsInUri = "IncludeCredentialsInUri";

        /// <summary>
        /// Identifies the <seealso cref="IncludeCredentialsInUri"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncludeCredentialsInUriProperty = DependencyProperty.Register(DependencyPropertyName_IncludeCredentialsInUri, typeof(bool), typeof(LinkVM),
                new PropertyMetadata(false,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).IncludeCredentialsInUri_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).IncludeCredentialsInUri_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).IncludeCredentialsInUri_CoerceValue(baseValue)));

        /// <summary>
        /// Whether to include credentials when building URI.
        /// </summary>
        public bool IncludeCredentialsInUri
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IncludeCredentialsInUriProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => IncludeCredentialsInUri)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(IncludeCredentialsInUriProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => IncludeCredentialsInUri = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="IncludeCredentialsInUri"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="IncludeCredentialsInUri"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="IncludeCredentialsInUri"/> property was changed.</param>
        protected virtual void IncludeCredentialsInUri_PropertyChanged(bool oldValue, bool newValue)
        {
            // TODO: Implement LinkVM.IncludeCredentialsInUri_PropertyChanged(bool, bool)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="IncludeCredentialsInUri"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual bool IncludeCredentialsInUri_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.IncludeCredentialsInUri_CoerceValue(DependencyObject, object)
            return (bool)baseValue;
        }

        #endregion

        #region Host Property Members

        public const string DependencyPropertyName_Host = "Host";

        /// <summary>
        /// Identifies the <seealso cref="Host"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HostProperty = DependencyProperty.Register(DependencyPropertyName_Host, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Host_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Host_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Host_CoerceValue(baseValue)));

        /// <summary>
        /// Host of link URI
        /// </summary>
        public string Host
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(HostProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Host)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(HostProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Host = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Host"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Host"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Host"/> property was changed.</param>
        protected virtual void Host_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Host_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Host"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Host_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Host_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Port Property Members

        public const string DependencyPropertyName_Port = "Port";

        /// <summary>
        /// Identifies the <seealso cref="Port"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PortProperty = DependencyProperty.Register(DependencyPropertyName_Port, typeof(int), typeof(LinkVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Port_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Port_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Port_CoerceValue(baseValue)));

        /// <summary>
        /// Port of URI or -1 for default.
        /// </summary>
        public int Port
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(PortProperty));
                return (int)(Dispatcher.Invoke(new Func<int>(() => Port)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(PortProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Port = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Port"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="Port"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="Port"/> property was changed.</param>
        protected virtual void Port_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement LinkVM.Port_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Port"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int Port_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Port_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region Path Property Members

        public const string DependencyPropertyName_Path = "Path";

        /// <summary>
        /// Identifies the <seealso cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(DependencyPropertyName_Path, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Path_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Path_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Path_CoerceValue(baseValue)));

        /// <summary>
        /// Path of Link URI
        /// </summary>
        public string Path
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(PathProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Path)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(PathProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Path = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Path"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Path"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Path"/> property was changed.</param>
        protected virtual void Path_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Path_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Path"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Path_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Path_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Query Property Members

        public const string DependencyPropertyName_Query = "Query";

        /// <summary>
        /// Identifies the <seealso cref="Query"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(DependencyPropertyName_Query, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Query_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Query_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Query_CoerceValue(baseValue)));

        /// <summary>
        /// Query portion of Link URI
        /// </summary>
        public string Query
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(QueryProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Query)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(QueryProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Query = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Query"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Query"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Query"/> property was changed.</param>
        protected virtual void Query_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Query_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Query"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Query_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Query_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Fragment Property Members

        public const string DependencyPropertyName_Fragment = "Fragment";

        /// <summary>
        /// Identifies the <seealso cref="Fragment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FragmentProperty = DependencyProperty.Register(DependencyPropertyName_Fragment, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Fragment_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Fragment_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Fragment_CoerceValue(baseValue)));

        /// <summary>
        /// Fragment portion of link URI
        /// </summary>
        public string Fragment
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(FragmentProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Fragment)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(FragmentProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Fragment = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Fragment"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Fragment"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Fragment"/> property was changed.</param>
        protected virtual void Fragment_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Fragment_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Fragment"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Fragment_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Fragment_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Parent Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Identifies the <seealso cref="Parent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register(DependencyPropertyName_Parent, typeof(CredentialVM), typeof(LinkVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Parent_PropertyChanged((CredentialVM)(e.OldValue), (CredentialVM)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Parent_PropertyChanged((CredentialVM)(e.OldValue), (CredentialVM)(e.NewValue))));
                }));

        /// <summary>
        /// Parent credential
        /// </summary>
        public CredentialVM Parent
        {
            get
            {
                if (CheckAccess())
                    return (CredentialVM)(GetValue(ParentProperty));
                return (CredentialVM)(Dispatcher.Invoke(new Func<CredentialVM>(() => Parent)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(ParentProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Parent = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Parent"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="CredentialVM"/> value before the <seealso cref="Parent"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="CredentialVM"/> value after the <seealso cref="Parent"/> property was changed.</param>
        protected virtual void Parent_PropertyChanged(CredentialVM oldValue, CredentialVM newValue)
        {
            if (oldValue != null)
                oldValue.Items.RemoveItem(this);

            if (newValue != null)
                oldValue.Items.AssertItemAdded(this);
        }

        #endregion

        #region BrowserOptions Property Members

        public const string PropertyName_BrowserOptions = "BrowserOptions";

        private static readonly DependencyPropertyKey BrowserOptionsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BrowserOptions, typeof(ObservableCollection<BrowserConfigVM>), typeof(LinkVM),
                new PropertyMetadata(0));

        /// <summary>
        /// Identifies the <seealso cref="BrowserOptions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrowserOptionsProperty = BrowserOptionsPropertyKey.DependencyProperty;

        /// <summary>
        /// List of options for browser to open URI
        /// </summary>
        public ObservableCollection<BrowserConfigVM> BrowserOptions
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<BrowserConfigVM>)(GetValue(BrowserOptionsProperty));
                return (ObservableCollection<BrowserConfigVM>)(Dispatcher.Invoke(new Func<ObservableCollection<BrowserConfigVM>>(() => BrowserOptions)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BrowserOptionsPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => BrowserOptions = value));
            }
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(LinkVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of selected browoser config option.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedIndexProperty));
                return (int)(Dispatcher.Invoke(new Func<int>(() => SelectedIndex)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedIndexProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedIndex = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectedIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectedIndex"/> property was changed.</param>
        protected virtual void SelectedIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement LinkVM.SelectedIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.SelectedIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region SelectedBrowserConfig Property Members

        public const string PropertyName_SelectedBrowserConfig = "SelectedBrowserConfig";

        private static readonly DependencyPropertyKey SelectedBrowserConfigPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedBrowserConfig, typeof(BrowserConfigVM), typeof(LinkVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedBrowserConfig"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrowserConfigProperty = SelectedBrowserConfigPropertyKey.DependencyProperty;

        /// <summary>
        /// Selected browser config option.
        /// </summary>
        public BrowserConfigVM SelectedBrowserConfig
        {
            get
            {
                if (CheckAccess())
                    return (BrowserConfigVM)(GetValue(SelectedBrowserConfigProperty));
                return (BrowserConfigVM)(Dispatcher.Invoke(new Func<BrowserConfigVM>(() => SelectedBrowserConfig)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedBrowserConfigPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedBrowserConfig = value));
            }
        }

        #endregion

        public UriBuilder ToUriBuilder()
        {
            throw new NotImplementedException();
        }

        public Uri ToUri()
        {
            throw new NotImplementedException();
        }

        public void Load(XmlReader xmlReader)
        {
            throw new NotImplementedException();
        }

        public void Save(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
