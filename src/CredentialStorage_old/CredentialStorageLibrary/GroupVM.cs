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
    public class GroupVM : NestedDependencyParent<GroupVM, CredentialVM>, INestedDependencyObject<GroupVM, DomainVM>
    {
        #region Id Property Members

        public const string PropertyName_Id = "Id";

        private static readonly DependencyPropertyKey IdPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Id, typeof(Guid), typeof(GroupVM),
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
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(DependencyPropertyName_Title, typeof(string), typeof(GroupVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as GroupVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as GroupVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as GroupVM).Title_CoerceValue(baseValue)));

        /// <summary>
        /// Title of group
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
            // TODO: Implement GroupVM.Title_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Title"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Title_CoerceValue(object baseValue)
        {
            // TODO: Implement GroupVM.Title_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Notes Property Members

        public const string DependencyPropertyName_Notes = "Notes";

        /// <summary>
        /// Identifies the <seealso cref="Notes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register(DependencyPropertyName_Notes, typeof(string), typeof(GroupVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as GroupVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as GroupVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as GroupVM).Notes_CoerceValue(baseValue)));

        /// <summary>
        /// Notes for group.
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
            // TODO: Implement GroupVM.Notes_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Notes"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Notes_CoerceValue(object baseValue)
        {
            // TODO: Implement GroupVM.Notes_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Parent Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Identifies the <seealso cref="Parent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = DependencyProperty.Register(DependencyPropertyName_Parent, typeof(DomainVM), typeof(GroupVM),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as GroupVM).Parent_PropertyChanged((DomainVM)(e.OldValue), (DomainVM)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as GroupVM).Parent_PropertyChanged((DomainVM)(e.OldValue), (DomainVM)(e.NewValue))));
                }));

        /// <summary>
        /// Parent domain
        /// </summary>
        public DomainVM Parent
        {
            get
            {
                if (CheckAccess())
                    return (DomainVM)(GetValue(ParentProperty));
                return (DomainVM)(Dispatcher.Invoke(new Func<DomainVM>(() => Parent)));
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
        /// <param name="oldValue">The <seealso cref="DomainVM"/> value before the <seealso cref="Parent"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="DomainVM"/> value after the <seealso cref="Parent"/> property was changed.</param>
        protected virtual void Parent_PropertyChanged(DomainVM oldValue, DomainVM newValue)
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
