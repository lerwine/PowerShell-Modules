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
    public class BrowserConfigVM : DependencyObject
    {

        #region Id Property Members

        public const string DependencyPropertyName_Id = "Id";

        /// <summary>
        /// Identifies the <seealso cref="Id"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(DependencyPropertyName_Id, typeof(Guid), typeof(BrowserConfigVM),
                new PropertyMetadata(default(Guid),
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as BrowserConfigVM).Id_PropertyChanged((Guid)(e.OldValue), (Guid)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as BrowserConfigVM).Id_PropertyChanged((Guid)(e.OldValue), (Guid)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as BrowserConfigVM).Id_CoerceValue(baseValue)));

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                if (CheckAccess())
                    return (Guid)(GetValue(IdProperty));
                return Dispatcher.Invoke(() => Id);
            }
            set
            {
                if (CheckAccess())
                    SetValue(IdProperty, value);
                else
                    Dispatcher.Invoke(() => Id = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Id"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="Guid"/> value before the <seealso cref="Id"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="Guid"/> value after the <seealso cref="Id"/> property was changed.</param>
        protected virtual void Id_PropertyChanged(Guid oldValue, Guid newValue)
        {
            // TODO: Implement BrowserConfigVM.Id_PropertyChanged(Guid, Guid)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Id"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual Guid Id_CoerceValue(object baseValue)
        {
            // TODO: Implement BrowserConfigVM.Id_CoerceValue(DependencyObject, object)
            return (Guid)baseValue;
        }

        #endregion

        #region DisplayText Property Members

        public const string DependencyPropertyName_DisplayText = "DisplayText";

        /// <summary>
        /// Identifies the <seealso cref="DisplayText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(DependencyPropertyName_DisplayText, typeof(string), typeof(BrowserConfigVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as BrowserConfigVM).DisplayText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as BrowserConfigVM).DisplayText_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as BrowserConfigVM).DisplayText_CoerceValue(baseValue)));

        /// <summary>
        /// Display text for browser config
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayTextProperty));
                return Dispatcher.Invoke(() => DisplayText);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DisplayTextProperty, value);
                else
                    Dispatcher.Invoke(() => DisplayText = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DisplayText"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="DisplayText"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="DisplayText"/> property was changed.</param>
        protected virtual void DisplayText_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement BrowserConfigVM.DisplayText_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DisplayText"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string DisplayText_CoerceValue(object baseValue)
        {
            // TODO: Implement BrowserConfigVM.DisplayText_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
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
