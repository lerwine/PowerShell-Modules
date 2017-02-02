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
    public class MainWindowVM : DependencyObject
    {
        #region TabIndex Property Members

        public const string DependencyPropertyName_TabIndex = "TabIndex";

        /// <summary>
        /// Identifies the <seealso cref="TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register(DependencyPropertyName_TabIndex, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).TabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as MainWindowVM).TabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).TabIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of currently visible tab
        /// </summary>
        public int TabIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(TabIndexProperty));
                return (int)(Dispatcher.Invoke(new Func<int>(() => TabIndex)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(TabIndexProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => TabIndex = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="TabIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="TabIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="TabIndex"/> property was changed.</param>
        protected virtual void TabIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement MainWindowVM.TabIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="TabIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int TabIndex_CoerceValue(object baseValue)
        {
            int? tabIndex = baseValue as int?;

            if (tabIndex.HasValue)
            {
                if (tabIndex.Value > 2)
                    return 2;

                if (tabIndex > -1)
                    return tabIndex.Value;
            }

            return 0;
        }

        #endregion

        #region DomainCollection Property Members

        public const string PropertyName_DomainCollection = "DomainCollection";

        private static readonly DependencyPropertyKey DomainCollectionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DomainCollection, typeof(ObservableCollection<DomainVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="DomainCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DomainCollectionProperty = DomainCollectionPropertyKey.DependencyProperty;

        /// <summary>
        /// Collection of domains
        /// </summary>
        public ObservableCollection<DomainVM> DomainCollection
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<DomainVM>)(GetValue(DomainCollectionProperty));
                return (ObservableCollection<DomainVM>)(Dispatcher.Invoke(new Func<ObservableCollection<DomainVM>>(() => DomainCollection)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DomainCollectionPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DomainCollection = value));
            }
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(MainWindowVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as MainWindowVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as MainWindowVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as MainWindowVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of selected domain
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
            bool isEnabled = newValue > -1; ;
            DeleteDomainCommand.IsEnabled = isEnabled;
            if (isEnabled)
                SelectedDomain = DomainCollection[newValue];
            else
                SelectedDomain = null;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            if (DomainCollection.Count < 0)
                return -1;

            int? index = baseValue as int?;
            if (!index.HasValue)
                index = SelectedIndex;

            if (index.Value >= DomainCollection.Count)
                return DomainCollection.Count - 1;

            if (index.Value < -1)
                return -1;

            return index.Value;
        }

        #endregion

        #region SelectedDomain Property Members

        public const string PropertyName_SelectedDomain = "SelectedDomain";

        private static readonly DependencyPropertyKey SelectedDomainPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedDomain, typeof(DomainVM), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedDomain"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDomainProperty = SelectedDomainPropertyKey.DependencyProperty;

        /// <summary>
        /// Currently selected domain
        /// </summary>
        public DomainVM SelectedDomain
        {
            get
            {
                if (CheckAccess())
                    return (DomainVM)(GetValue(SelectedDomainProperty));
                return (DomainVM)(Dispatcher.Invoke(new Func<DomainVM>(() => SelectedDomain)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedDomainPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedDomain = value));
            }
        }

        #endregion

        #region Parent Read-only Attached Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Attached property for main window parent
        /// </summary>
        /// <param name="obj">Object from which to retieve the value.</param>
        /// <returns><see cref="MainWindowVM"/> value from <paramref name="obj"/>.</returns>
        public static MainWindowVM GetParent(DependencyObject obj)
        {
            return (MainWindowVM)obj.GetValue(ParentProperty);
        }

        private static void SetParent(DependencyObject obj, MainWindowVM value)
        {
            obj.SetValue(ParentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterAttachedReadOnly(DependencyPropertyName_Parent, typeof(MainWindowVM), typeof(MainWindowVM),
                new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => Parent_PropertyChanged(d, (MainWindowVM)(e.OldValue), (MainWindowVM)(e.NewValue))));

        /// <summary>
        /// Identifies the Parent attached read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = ParentPropertyKey.DependencyProperty;

        private static void Parent_PropertyChanged(DependencyObject d, MainWindowVM oldvalue, MainWindowVM newValue)
        {
            DomainVM vm;
            if (newValue != null && (vm = d as DomainVM) != null)
                vm.SetBrowsersCollection(newValue.Browsers);
        }

        #endregion

        #region PreferredFor Attached Property Members

        public const string DependencyPropertyName_PreferredFor = "PreferredFor";

        /// <summary>
        /// Preferred browser for main window
        /// </summary>
        /// <param name="obj">Object from which to retieve the value.</param>
        /// <returns><see cref="MainWindowVM"/> value from <paramref name="obj"/>.</returns>
        public static MainWindowVM GetPreferredFor(DependencyObject obj)
        {
            return (MainWindowVM)obj.GetValue(PreferredForProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">Object on which to set the value.</param>
        /// <param name="value"><see cref="MainWindowVM"/> value to be set on <paramref name="obj"/>.</param>
        public static void SetPreferredFor(DependencyObject obj, MainWindowVM value)
        {
            obj.SetValue(PreferredForProperty, value);
        }

        /// <summary>
        /// Identifies the PreferredFor attached dependency property.
        /// </summary>
        public static readonly DependencyProperty PreferredForProperty = DependencyProperty.RegisterAttached(DependencyPropertyName_PreferredFor, typeof(MainWindowVM), typeof(MainWindowVM),
                new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => PreferredFor_PropertyChanged(d, (MainWindowVM)(e.OldValue), (MainWindowVM)(e.NewValue))));

        private static void PreferredFor_PropertyChanged(DependencyObject d, MainWindowVM oldvalue, MainWindowVM newValue)
        {
            BrowserVM vm = d as BrowserVM;
            if (vm == null)
                return;

            if (newValue != null)
            {
                foreach (BrowserVM item in newValue.Browsers.Where(b => b != null && !ReferenceEquals(b, vm) && GetPreferredFor(vm) != null))
                    SetPreferredFor(vm, null);
            }
            else if (oldvalue.Browsers.Count > 0 && !oldvalue.Browsers.Where(b => b != null).Select(b => GetPreferredFor(b)).Any(v => v != null && ReferenceEquals(v, oldvalue)))
                SetPreferredFor(oldvalue.Browsers.First(), oldvalue);
        }
        
        #endregion

        #region NewDomain Command Property Members

        private RelayCommand _newDomainCommand = null;

        public RelayCommand NewDomainCommand
        {
            get
            {
                if (_newDomainCommand == null)
                    _newDomainCommand = new RelayCommand(OnNewDomain);

                return _newDomainCommand;
            }
        }

        protected virtual void OnNewDomain(object parameter)
        {
            SelectedDomain = new DomainVM();
            if (SelectedIndex < DomainCollection.Count - 1)
                DomainCollection.Insert(SelectedIndex, SelectedDomain);
            else
                DomainCollection.Add(SelectedDomain);
        }

        #endregion

        #region DeleteDomain Command Property Members

        private RelayCommand _deleteDomainCommand = null;

        public RelayCommand DeleteDomainCommand
        {
            get
            {
                if (_deleteDomainCommand == null)
                    _deleteDomainCommand = new RelayCommand(OnDeleteDomain, false, true);

                return _deleteDomainCommand;
            }
        }

        protected virtual void OnDeleteDomain(object parameter)
        {
            DeleteDomainVisibility = Visibility.Visible;
            int groupCount = SelectedDomain.GroupCollection.Count;
            int credentialCount = SelectedDomain.GroupCollection.Where(g => g != null).Sum(g => g.CredentialCollection.Count(c => c != null));
            if (groupCount == 1)
            {
                if (credentialCount == 1)
                    DeleteDomainConfirmMessage = "There is 1 group containing a total of 1 credential.";
                else
                    DeleteDomainConfirmMessage = String.Format("There is 1 group containing a total of {0} credentials.", credentialCount);
            }
            else if (credentialCount == 1)
                DeleteDomainConfirmMessage = String.Format("There are {0} groups containing a total of 1 credential.", groupCount);
            else
                DeleteDomainConfirmMessage = String.Format("There are {0} groups containing a total of {1} credentials.", groupCount, credentialCount);
        }

        #endregion

        #region DeleteDomainVisibility Property Members

        public const string PropertyName_DeleteDomainVisibility = "DeleteDomainVisibility";

        private static readonly DependencyPropertyKey DeleteDomainVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteDomainVisibility, typeof(Visibility), typeof(MainWindowVM),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <seealso cref="DeleteDomainVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteDomainVisibilityProperty = DeleteDomainVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates whether the Delete Domain confirmation border is visible.
        /// </summary>
        public Visibility DeleteDomainVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(DeleteDomainVisibilityProperty));
                return (Visibility)(Dispatcher.Invoke(new Func<Visibility>(() => DeleteDomainVisibility)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteDomainVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DeleteDomainVisibility = value));
            }
        }

        #endregion

        #region DeleteDomainConfirmMessage Property Members

        public const string PropertyName_DeleteDomainConfirmMessage = "DeleteDomainConfirmMessage";

        private static readonly DependencyPropertyKey DeleteDomainConfirmMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteDomainConfirmMessage, typeof(string), typeof(MainWindowVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="DeleteDomainConfirmMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteDomainConfirmMessageProperty = DeleteDomainConfirmMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Message to show when confirming deletion of domain.
        /// </summary>
        public string DeleteDomainConfirmMessage
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DeleteDomainConfirmMessageProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => DeleteDomainConfirmMessage)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteDomainConfirmMessagePropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DeleteDomainConfirmMessage = value));
            }
        }

        #endregion

        #region DeleteDomainYes Command Property Members

        private RelayCommand _deleteDomainYesCommand = null;

        public RelayCommand DeleteDomainYesCommand
        {
            get
            {
                if (_deleteDomainYesCommand == null)
                    _deleteDomainYesCommand = new RelayCommand(OnDeleteDomainYes);

                return _deleteDomainYesCommand;
            }
        }

        protected virtual void OnDeleteDomainYes(object parameter)
        {
            DomainCollection.RemoveAt(SelectedIndex);
            DeleteDomainVisibility = Visibility.Collapsed;
        }

        #endregion

        #region DeleteDomainNo Command Property Members

        private RelayCommand _deleteDomainNoCommand = null;

        public RelayCommand DeleteDomainNoCommand
        {
            get
            {
                if (_deleteDomainNoCommand == null)
                    _deleteDomainNoCommand = new RelayCommand(OnDeleteDomainNo);

                return _deleteDomainNoCommand;
            }
        }

        protected virtual void OnDeleteDomainNo(object parameter)
        {
            DeleteDomainVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Browsers Property Members

        public const string PropertyName_Browsers = "Browsers";

        private static readonly DependencyPropertyKey BrowsersPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Browsers, typeof(ObservableCollection<BrowserVM>), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Browsers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrowsersProperty = BrowsersPropertyKey.DependencyProperty;

        /// <summary>
        /// List of supported browsers
        /// </summary>
        public ObservableCollection<BrowserVM> Browsers
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<BrowserVM>)(GetValue(BrowsersProperty));
                return (ObservableCollection<BrowserVM>)(Dispatcher.Invoke(new Func<ObservableCollection<BrowserVM>>(() => Browsers)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(BrowsersPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => Browsers = value));
            }
        }

        #endregion

        #region PreferredDefaultBrowser Property Members

        public const string PropertyName_PreferredDefaultBrowser = "PreferredDefaultBrowser";

        private static readonly DependencyPropertyKey PreferredDefaultBrowserPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_PreferredDefaultBrowser, typeof(BrowserVM), typeof(MainWindowVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="PreferredDefaultBrowser"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreferredDefaultBrowserProperty = PreferredDefaultBrowserPropertyKey.DependencyProperty;

        /// <summary>
        /// Preferred default browser
        /// </summary>
        public BrowserVM PreferredDefaultBrowser
        {
            get
            {
                if (CheckAccess())
                    return (BrowserVM)(GetValue(PreferredDefaultBrowserProperty));
                return (BrowserVM)(Dispatcher.Invoke(new Func<BrowserVM>(() => PreferredDefaultBrowser)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(PreferredDefaultBrowserPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => PreferredDefaultBrowser = value));
            }
        }

        #endregion

        public MainWindowVM()
        {
            PreferredDefaultBrowser = new BrowserVM();
            Browsers = new ObservableCollection<BrowserVM>();
            Browsers.CollectionChanged += Browsers_CollectionChanged;
            Browsers.Add(PreferredDefaultBrowser);
            SelectedDomain = new DomainVM();
            DomainCollection = new ObservableCollection<DomainVM>();
            DomainCollection.CollectionChanged += DomainCollection_CollectionChanged;
            DomainCollection.Add(SelectedDomain);
        }

        private void Browsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Browsers.Count == 0)
            {
                PreferredDefaultBrowser = new BrowserVM();
                Browsers.Add(PreferredDefaultBrowser);
            }
            if (PreferredDefaultBrowser == null || !Browsers.Any(b => b != null && ReferenceEquals(b, PreferredDefaultBrowser)))
                PreferredDefaultBrowser = Browsers.FirstOrDefault();

            if (PreferredDefaultBrowser == null)
                return;
            MainWindowVM vm = GetPreferredFor(PreferredDefaultBrowser);
            if (vm == null || !ReferenceEquals(vm, this))
                SetPreferredFor(PreferredDefaultBrowser, this);
        }

        private void DomainCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                DomainVM[] toRemove = e.OldItems.OfType<DomainVM>().Where(o => !DomainCollection.Any(i => i != null && ReferenceEquals(i, o))).ToArray();
                foreach (DomainVM item in toRemove)
                {
                    MainWindowVM vm = GetParent(item);
                    if (vm != null && ReferenceEquals(vm, this))
                        SetParent(item, null);
                }
            }

            foreach (DomainVM item in DomainCollection)
            {
                if (item == null)
                    continue;

                MainWindowVM vm = GetParent(item);
                if (vm != null)
                {
                    if (ReferenceEquals(vm, this))
                        continue;
                    vm.DomainCollection.Remove(item);
                }
                SetParent(item, this);
            }

            if (DomainCollection.Count == 0)
            {
                SelectedDomain = new DomainVM();
                DomainCollection.Add(SelectedDomain);
                return;
            }

            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
                return;
            }

            int index = DomainCollection.IndexOf(SelectedDomain);
            if (index < 0)
            {
                if (SelectedIndex > 0)
                    SelectedIndex -= 1;
                else
                    SelectedDomain = DomainCollection[0];
            }
            else if (SelectedIndex != index)
                SelectedIndex = index;
        }
    }
}
