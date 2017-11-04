using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Speech.GUI
{
    /// <summary>
    /// View Model for new file type selection.
    /// </summary>
    public class NewFileTypeViewModel : DependencyObject
    {
        public NewFileTypeViewModel(bool isXml)
        {
            IsXml = isXml;
            DisplayText = (isXml) ? "Xml File" : "Text File";
        }

        #region DisplayText Property Members

        public const string PropertyName_DisplayText = "DisplayText";

        private static readonly DependencyPropertyKey DisplayTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DisplayText, typeof(string), typeof(NewFileTypeViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="DisplayText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Display text for new file type.
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DisplayTextProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => DisplayText)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DisplayTextPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DisplayText = value));
            }
        }

        #endregion

        #region IsXml Property Members

        public const string PropertyName_IsXml = "IsXml";

        private static readonly DependencyPropertyKey IsXmlPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_IsXml, typeof(bool), typeof(NewFileTypeViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="IsXml"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsXmlProperty = IsXmlPropertyKey.DependencyProperty;

        /// <summary>
        /// True if new file is XML; otherwise, false if new file is plain text.
        /// </summary>
        public bool IsXml
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(IsXmlProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => IsXml)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(IsXmlPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => IsXml = value));
            }
        }

        #endregion
    }
}