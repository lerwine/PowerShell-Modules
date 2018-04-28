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
    public class CredentialVM : NestedDependencyParent<CredentialVM, LinkVM>, INestedDependencyObject<CredentialVM, GroupVM>
    {
        #region Id Property Members

        public const string PropertyName_Id = "Id";

        private static readonly DependencyPropertyKey IdPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Id, typeof(Guid), typeof(CredentialVM),
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
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).Title_CoerceValue(baseValue)));

        /// <summary>
        /// Title of credential.
        /// </summary>
        public string Title
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(TitleProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Title)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(TitleProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Title = value));
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
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).Login_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).Login_CoerceValue(baseValue)));

        /// <summary>
        /// Login account name.
        /// </summary>
        public string Login
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(LoginProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Login)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(LoginProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Login = value));
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

        #region EncryptedPasword Property Members

        public const string DependencyPropertyName_EncryptedPasword = "EncryptedPasword";

        /// <summary>
        /// Identifies the <seealso cref="EncryptedPasword"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EncryptedPaswordProperty = DependencyProperty.Register(DependencyPropertyName_EncryptedPasword, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).EncryptedPasword_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).EncryptedPasword_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).EncryptedPasword_CoerceValue(baseValue)));

        /// <summary>
        /// Encrypted password for credentials
        /// </summary>
        public string EncryptedPasword
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EncryptedPaswordProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => EncryptedPasword)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(EncryptedPaswordProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => EncryptedPasword = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="EncryptedPasword"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="EncryptedPasword"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="EncryptedPasword"/> property was changed.</param>
        protected virtual void EncryptedPasword_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.EncryptedPasword_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="EncryptedPasword"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string EncryptedPasword_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.EncryptedPasword_CoerceValue(DependencyObject, object)
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
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).EncryptedPin_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).EncryptedPin_CoerceValue(baseValue)));

        /// <summary>
        /// Encrypted pin for credentials
        /// </summary>
        public string EncryptedPin
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(EncryptedPinProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => EncryptedPin)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(EncryptedPinProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => EncryptedPin = value));
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

        #region Notes Property Members

        public const string DependencyPropertyName_Notes = "Notes";

        /// <summary>
        /// Identifies the <seealso cref="Notes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register(DependencyPropertyName_Notes, typeof(string), typeof(CredentialVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as CredentialVM).Notes_CoerceValue(baseValue)));

        /// <summary>
        /// Notes for credential.
        /// </summary>
        public string Notes
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NotesProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Notes)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(NotesProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Notes = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Notes"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Notes"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Notes"/> property was changed.</param>
        protected virtual void Notes_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement CredentialVM.Notes_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Notes"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Notes_CoerceValue(object baseValue)
        {
            // TODO: Implement CredentialVM.Notes_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Parent Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Identifies the <seealso cref="Parent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register(DependencyPropertyName_Parent, typeof(GroupVM), typeof(CredentialVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as CredentialVM).Parent_PropertyChanged((GroupVM)(e.OldValue), (GroupVM)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as CredentialVM).Parent_PropertyChanged((GroupVM)(e.OldValue), (GroupVM)(e.NewValue))));
                }));

        /// <summary>
        /// Parent group
        /// </summary>
        public GroupVM Parent
        {
            get
            {
                if (CheckAccess())
                    return (GroupVM)(GetValue(ParentProperty));
                return (GroupVM)(Dispatcher.Invoke(new Func<GroupVM>(() => Parent)));
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
        /// <param name="oldValue">The <seealso cref="GroupVM"/> value before the <seealso cref="Parent"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="GroupVM"/> value after the <seealso cref="Parent"/> property was changed.</param>
        protected virtual void Parent_PropertyChanged(GroupVM oldValue, GroupVM newValue)
        {
            if (oldValue != null)
                oldValue.Items.RemoveItem(this);

            if (newValue != null)
                oldValue.Items.AssertItemAdded(this);
        }

        #endregion

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
