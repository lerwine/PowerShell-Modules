using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestWpfApplication
{
    public class CredentialVM : DependencyObject
    {
        #region Title Property Members

        public const string DependencyPropertyName_Title = "Title";

        /// <summary>
        /// Identifies the <seealso cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(DependencyPropertyName_Title, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as CredentialVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).Title_CoerceValue(baseValue)));

        /// <summary>
        /// Title of credential
        /// </summary>
        public string Title
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(TitleProperty));
                return Dispatcher.Invoke(() => Title);
            }
            set
            {
                if (CheckAccess())
                    SetValue(TitleProperty, value);
                else
                    Dispatcher.Invoke(() => Title = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Title"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Title"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Title"/> property was changed.</param>
        protected virtual void Title_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.Title_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Title"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Title_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.Title_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Login Property Members

        public const string DependencyPropertyName_Login = "Login";

        /// <summary>
        /// Identifies the <seealso cref="Login"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoginProperty = DependencyProperty.Register(DependencyPropertyName_Login, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).Login_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as CredentialVM).Login_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).Login_CoerceValue(baseValue)));

        /// <summary>
        /// Login for credential
        /// </summary>
        public string Login
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LoginProperty));
                return Dispatcher.Invoke(() => Login);
            }
            set
            {
                if (CheckAccess())
                    SetValue(LoginProperty, value);
                else
                    Dispatcher.Invoke(() => Login = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Login"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Login"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Login"/> property was changed.</param>
        protected virtual void Login_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.Login_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Login"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Login_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.Login_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region EncryptedPassword Property Members

        public const string DependencyPropertyName_EncryptedPassword = "EncryptedPassword";

        /// <summary>
        /// Identifies the <seealso cref="EncryptedPassword"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EncryptedPasswordProperty = DependencyProperty.Register(DependencyPropertyName_EncryptedPassword, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).EncryptedPassword_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as CredentialVM).EncryptedPassword_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).EncryptedPassword_CoerceValue(baseValue)));

        /// <summary>
        /// Encrypted password for credential
        /// </summary>
        public string EncryptedPassword
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EncryptedPasswordProperty));
                return Dispatcher.Invoke(() => EncryptedPassword);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EncryptedPasswordProperty, value);
                else
                    Dispatcher.Invoke(() => EncryptedPassword = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="EncryptedPassword"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="EncryptedPassword"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="EncryptedPassword"/> property was changed.</param>
        protected virtual void EncryptedPassword_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.EncryptedPassword_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="EncryptedPassword"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string EncryptedPassword_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.EncryptedPassword_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region EncryptedPin Property Members

        public const string DependencyPropertyName_EncryptedPin = "EncryptedPin";

        /// <summary>
        /// Identifies the <seealso cref="EncryptedPin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EncryptedPinProperty = DependencyProperty.Register(DependencyPropertyName_EncryptedPin, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).EncryptedPin_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as CredentialVM).EncryptedPin_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).EncryptedPin_CoerceValue(baseValue)));

        /// <summary>
        /// Encrypted pin for credential
        /// </summary>
        public string EncryptedPin
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EncryptedPinProperty));
                return Dispatcher.Invoke(() => EncryptedPin);
            }
            set
            {
                if (CheckAccess())
                    SetValue(EncryptedPinProperty, value);
                else
                    Dispatcher.Invoke(() => EncryptedPin = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="EncryptedPin"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="EncryptedPin"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="EncryptedPin"/> property was changed.</param>
        protected virtual void EncryptedPin_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.EncryptedPin_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="EncryptedPin"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string EncryptedPin_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.EncryptedPin_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Links Property Members

        public const string PropertyName_Links = "Links";

        private static readonly DependencyPropertyKey LinksPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Links, typeof(ObservableCollection<LinkVM>), typeof(CredentialVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Links"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LinksProperty = LinksPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<LinkVM> Links
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<LinkVM>)(GetValue(LinksProperty));
                return Dispatcher.Invoke(() => Links);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(LinksPropertyKey, value);
                else
                    Dispatcher.Invoke(() => Links = value);
            }
        }

        #endregion

        #region Parent Read-only Attached Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Parent for LinkVM
        /// </summary>
        /// <param name="obj">Object from which to retieve the value.</param>
        /// <returns><see cref="CredentialVM"/> value from <paramref name="obj"/>.</returns>
        public static CredentialVM GetParent(DependencyObject obj)
        {
            return (CredentialVM)obj.GetValue(ParentProperty);
        }

        private static void SetParent(DependencyObject obj, CredentialVM value)
        {
            obj.SetValue(ParentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterAttachedReadOnly(DependencyPropertyName_Parent, typeof(CredentialVM), typeof(CredentialVM),
                new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => Parent_PropertyChanged(d, (CredentialVM)(e.OldValue), (CredentialVM)(e.NewValue))));

        /// <summary>
        /// Identifies the Parent attached read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = ParentPropertyKey.DependencyProperty;

        private static void Parent_PropertyChanged(DependencyObject d, CredentialVM oldvalue, CredentialVM newValue)
        {
            LinkVM vm;
            if (newValue != null && (vm = d as LinkVM) != null)
                vm.SetBrowsersCollection(newValue._browsers);
        }

        #endregion

        public CredentialVM()
        {
            Links = new ObservableCollection<LinkVM>();
            Links.CollectionChanged += Links_CollectionChanged;
        }

        private void Links_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                LinkVM[] toRemove = e.OldItems.OfType<LinkVM>().Where(o => !Links.Any(i => i != null && ReferenceEquals(i, o))).ToArray();
                foreach (LinkVM item in toRemove)
                {
                    CredentialVM vm = GetParent(item);
                    if (vm != null && ReferenceEquals(vm, this))
                        SetParent(item, null);
                }
            }

            foreach (LinkVM item in Links)
            {
                if (item == null)
                    continue;

                CredentialVM vm = GetParent(item);
                if (vm != null)
                {
                    if (ReferenceEquals(vm, this))
                        continue;
                    vm.Links.Remove(item);
                }
                SetParent(item, this);
            }
        }

        private ObservableCollection<BrowserVM> _browsers = new ObservableCollection<BrowserVM>();

        internal void SetBrowsersCollection(ObservableCollection<BrowserVM> browsers)
        {
            _browsers = browsers;
            foreach (LinkVM link in Links.Where(l => l != null))
                link.SetBrowsersCollection(browsers);
        }
    }
}