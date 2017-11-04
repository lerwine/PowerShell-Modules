using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
    public class RollupItemViewModel : DependencyObject
    {
        public const string Extension_Text = ".txt";
        public const string Extension_Xml = ".xml";
        public const string Extension_Wav = ".wav";

        public RollupItemViewModel(FileInfo refFile)
        {
            if (refFile == null)
            {
                Name = "";
                DirectoryName = "";
                return;
            }

            Name = Path.GetFileNameWithoutExtension(refFile);
            DirectoryName = refFile.DirectoryName;

            if (String.Equals(refFile.Extension, Extension_Text, StringComparison.InvariantCultureIgnoreCase))
            {
                Refresh(refFile, new FileInfo(Path.Combine(DirectoryName, Name + Extension_Xml)),
                    new FileInfo(Path.Combine(DirectoryName, Name + Extension_Wav)));
                return;
            }

            if (String.Equals(refFile.Extension, Extension_Xml, StringComparison.InvariantCultureIgnoreCase))
            {
                Refresh(new FileInfo(Path.Combine(DirectoryName, Name + Extension_Text)),
                    refFile, new FileInfo(Path.Combine(DirectoryName, Name + Extension_Wav)));
            }
            else
            {
                Refresh(new FileInfo(Path.Combine(DirectoryName, Name + Extension_Text)),
                    new FileInfo(Path.Combine(DirectoryName, Name + Extension_Xml)),
                    (String.Equals(refFile.Extension, Extension_Wav, StringComparison.InvariantCultureIgnoreCase)) ? refFile : new FileInfo(Path.Combine(DirectoryName, Name + Extension_Wav)));
            }
        }

        public void Refresh()
        {
            if (Name.Length == 0)
                return;

            Refresh(new FileInfo(Path.Combine(DirectoryName, Name + Extension_Text)),
                new FileInfo(Path.Combine(DirectoryName, Name + Extension_Xml)),
                new FileInfo(Path.Combine(DirectoryName, Name + Extension_Wav)));
        }

        private void Refresh(FileInfo textFile, FileInfo xmlFile, FileInfo wavFile)
        {
            HasText = textFile.Exists;
            TextIsNewer = textFile.Exists && (!xmlFile.Exists || xmlFile.LastWriteTime < textFile.LastWriteTime);
            HasXml = xmlFile.Exists;
            XmlIsNewer = xmlFile.Exists && (!wavFile.Exists || wavFile.LastWriteTime < xmlFile.LastWriteTime);
            HasWav = wavFile.Exists;
        }

        #region Name Property Members

        public const string PropertyName_Name = "Name";

        private static readonly DependencyPropertyKey NamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Name, typeof(string), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty = NamePropertyKey.DependencyProperty;

        /// <summary>
        /// Display text for new file type.
        /// </summary>
        public string Name
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NameProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Name)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(NamePropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => Name = value));
            }
        }

        #endregion

        #region DirectoryName Property Members

        public const string PropertyDirectoryName_DirectoryName = "DirectoryName";

        private static readonly DependencyPropertyKey DirectoryNamePropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_DirectoryName, typeof(string), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="DirectoryName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectoryNameProperty = DirectoryNamePropertyKey.DependencyProperty;

        /// <summary>
        /// Display text for new file type.
        /// </summary>
        public string DirectoryName
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DirectoryNameProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => DirectoryName)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DirectoryNamePropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DirectoryName = value));
            }
        }

        #endregion

        #region HasText Property Members

        public const string PropertyDirectoryName_HasText = "HasText";

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_HasText, typeof(bool), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="HasText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        /// <summary>
        /// True if a TXT file exists; otherwise, false.
        /// </summary>
        public bool HasText
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(HasTextProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => HasText)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(HasTextPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => HasText = value));
            }
        }

        #endregion

        #region HasXml Property Members

        public const string PropertyDirectoryName_HasXml = "HasXml";

        private static readonly DependencyPropertyKey HasXmlPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_HasXml, typeof(bool), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="HasXml"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasXmlProperty = HasXmlPropertyKey.DependencyProperty;

        /// <summary>
        /// True if an XML file exists; otherwise, false.
        /// </summary>
        public bool HasXml
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(HasXmlProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => HasXml)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(HasXmlPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => HasXml = value));
            }
        }

        #endregion

        #region HasWav Property Members

        public const string PropertyDirectoryName_HasWav = "HasWav";

        private static readonly DependencyPropertyKey HasWavPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_HasWav, typeof(bool), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="HasWav"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasWavProperty = HasWavPropertyKey.DependencyProperty;

        /// <summary>
        /// True if a WAV file exists; otherwise, false.
        /// </summary>
        public bool HasWav
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(HasWavProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => HasWav)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(HasWavPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => HasWav = value));
            }
        }

        #endregion

        #region TextIsNewer Property Members

        public const string PropertyDirectoryName_TextIsNewer = "TextIsNewer";

        private static readonly DependencyPropertyKey TextIsNewerPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_TextIsNewer, typeof(bool), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="TextIsNewer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextIsNewerProperty = TextIsNewerPropertyKey.DependencyProperty;

        /// <summary>
        /// True if a WAV file exists; otherwise, false.
        /// </summary>
        public bool TextIsNewer
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(TextIsNewerProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => TextIsNewer)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(TextIsNewerPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => TextIsNewer = value));
            }
        }

        #endregion

        #region XmlIsNewer Property Members

        public const string PropertyDirectoryName_XmlIsNewer = "XmlIsNewer";

        private static readonly DependencyPropertyKey XmlIsNewerPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_XmlIsNewer, typeof(bool), typeof(RollupItemViewModel),
                new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <seealso cref="XmlIsNewer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XmlIsNewerProperty = XmlIsNewerPropertyKey.DependencyProperty;

        /// <summary>
        /// True if a WAV file exists; otherwise, false.
        /// </summary>
        public bool XmlIsNewer
        {
            get
            {
                if (CheckAccess())
                    return (bool)(GetValue(XmlIsNewerProperty));
                return (bool)(Dispatcher.Invoke(new Func<bool>(() => XmlIsNewer)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(XmlIsNewerPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => XmlIsNewer = value));
            }
        }

        #endregion
    }
}