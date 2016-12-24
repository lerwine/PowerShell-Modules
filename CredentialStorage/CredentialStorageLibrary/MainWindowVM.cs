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
    public class MainWindowVM : NestedDependencyParent<MainWindowVM, DomainVM>
    {
        #region MainWindow Property Members

        public const string PropertyName_MainWindow = "MainWindow";

        private static readonly DependencyPropertyKey MainWindowPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_MainWindow, typeof(MainWindow), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="MainWindow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MainWindowProperty = MainWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Window using this view model.
        /// </summary>
        public MainWindow MainWindow
        {
            get
            {
                if (CheckAccess())
                    return (MainWindow)(GetValue(MainWindowProperty));
                return Dispatcher.Invoke(() => MainWindow);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(MainWindowPropertyKey, value);
                else
                    Dispatcher.Invoke(() => MainWindow = value);
            }
        }

        #endregion

        #region CloseWindow Command Property Members

        private Command.RelayCommand _closeWindowCommand = null;

        public Command.RelayCommand CloseWindowCommand
        {
            get
            {
                if (_closeWindowCommand == null)
                    _closeWindowCommand = new Command.RelayCommand(OnCloseWindow);

                return _closeWindowCommand;
            }
        }

        protected virtual void OnCloseWindow(object parameter)
        {
            // TODO: Implement OnCloseWindow Logic
        }

        #endregion

        #region SelectedTabIndex Property Members

        public const string DependencyPropertyName_SelectedTabIndex = "SelectedTabIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedTabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTabIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedTabIndex, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectedTabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectedTabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).SelectedTabIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Currently selected tab.
        /// </summary>
        /// <remarks>0: Manage Credentials; 1: Manage domains; 2: Manage groups; 3: Manage browser configurations.</remarks>
        public int SelectedTabIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedTabIndexProperty));
                return Dispatcher.Invoke(() => SelectedTabIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedTabIndexProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedTabIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedTabIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectedTabIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectedTabIndex"/> property was changed.</param>
        protected virtual void SelectedTabIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.SelectedTabIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedTabIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedTabIndex_CoerceValue(object baseValue)
        {
            int? index = baseValue as int?;
            if (!index.HasValue || index.Value < 0)
                return 0;

            if (index.Value > 3)
                return 3;

            return index.Value;
        }

        #endregion
        
        #region BrowserConfigCollection Property Members

        public const string PropertyName_BrowserConfigCollection = "BrowserConfigCollection";

        private static readonly DependencyPropertyKey BrowserConfigCollectionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_BrowserConfigCollection, typeof(ObservableCollection<BrowserConfigVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="BrowserConfigCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrowserConfigCollectionProperty = BrowserConfigCollectionPropertyKey.DependencyProperty;

        /// <summary>
        /// Available browser configuration options.
        /// </summary>
        public ObservableCollection<BrowserConfigVM> BrowserConfigCollection
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<BrowserConfigVM>)(GetValue(BrowserConfigCollectionProperty));
                return Dispatcher.Invoke(() => BrowserConfigCollection);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BrowserConfigCollectionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => BrowserConfigCollection = value);
            }
        }
        
        private void BrowserConfigCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SelectedBrowserConfigIndex Property Members

        public const string DependencyPropertyName_SelectedBrowserConfigIndex = "SelectedBrowserConfigIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedBrowserConfigIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrowserConfigIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedBrowserConfigIndex, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectedBrowserConfigIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).SelectedBrowserConfigIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).SelectedBrowserConfigIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of currently selected browser config.
        /// </summary>
        public int SelectedBrowserConfigIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedBrowserConfigIndexProperty));
                return Dispatcher.Invoke(() => SelectedBrowserConfigIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedBrowserConfigIndexProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedBrowserConfigIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedBrowserConfigIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="SelectedBrowserConfigIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="SelectedBrowserConfigIndex"/> property was changed.</param>
        protected virtual void SelectedBrowserConfigIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.SelectedBrowserConfigIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedBrowserConfigIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedBrowserConfigIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement MainWindowVM.SelectedBrowserConfigIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region SelectedBrowserConfig Property Members

        public const string PropertyName_SelectedBrowserConfig = "SelectedBrowserConfig";

        private static readonly DependencyPropertyKey SelectedBrowserConfigPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedBrowserConfig, typeof(BrowserConfigVM), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedBrowserConfig"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrowserConfigProperty = SelectedBrowserConfigPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public BrowserConfigVM SelectedBrowserConfig
        {
            get
            {
                if (CheckAccess())
                    return (BrowserConfigVM)(GetValue(SelectedBrowserConfigProperty));
                return Dispatcher.Invoke(() => SelectedBrowserConfig);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedBrowserConfigPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SelectedBrowserConfig = value);
            }
        }

        #endregion

        #region DefaultBrowserConfigIndex Property Members

        public const string DependencyPropertyName_DefaultBrowserConfigIndex = "DefaultBrowserConfigIndex";

        /// <summary>
        /// Identifies the <seealso cref="DefaultBrowserConfigIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultBrowserConfigIndexProperty = DependencyProperty.Register(DependencyPropertyName_DefaultBrowserConfigIndex, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).DefaultBrowserConfigIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as MainWindowVM).DefaultBrowserConfigIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).DefaultBrowserConfigIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of the default browser config.
        /// </summary>
        public int DefaultBrowserConfigIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(DefaultBrowserConfigIndexProperty));
                return Dispatcher.Invoke(() => DefaultBrowserConfigIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(DefaultBrowserConfigIndexProperty, value);
                else
                    Dispatcher.Invoke(() => DefaultBrowserConfigIndex = value);
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="DefaultBrowserConfigIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="DefaultBrowserConfigIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="DefaultBrowserConfigIndex"/> property was changed.</param>
        protected virtual void DefaultBrowserConfigIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.DefaultBrowserConfigIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="DefaultBrowserConfigIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int DefaultBrowserConfigIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement MainWindowVM.DefaultBrowserConfigIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region DefaultBrowserConfig Property Members

        public const string PropertyName_DefaultBrowserConfig = "DefaultBrowserConfig";

        private static readonly DependencyPropertyKey DefaultBrowserConfigPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DefaultBrowserConfig, typeof(BrowserConfigVM), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="DefaultBrowserConfig"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultBrowserConfigProperty = DefaultBrowserConfigPropertyKey.DependencyProperty;

        /// <summary>
        /// Default browser configuration
        /// </summary>
        public BrowserConfigVM DefaultBrowserConfig
        {
            get
            {
                if (CheckAccess())
                    return (BrowserConfigVM)(GetValue(DefaultBrowserConfigProperty));
                return Dispatcher.Invoke(() => DefaultBrowserConfig);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DefaultBrowserConfigPropertyKey, value);
                else
                    Dispatcher.Invoke(() => DefaultBrowserConfig = value);
            }
        }

        #endregion
        
        public MainWindowVM()
        {
            MainWindow = new MainWindow();
            BrowserConfigCollection = new ObservableCollection<BrowserConfigVM>();
            BrowserConfigCollection.CollectionChanged += BrowserConfigCollection_CollectionChanged;
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
