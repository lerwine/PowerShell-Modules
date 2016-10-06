using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TestWpfApplication
{
    public class GroupVM : DependencyObject
    {
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
                        d.Dispatcher.Invoke(() => (d as GroupVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
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
                        d.Dispatcher.Invoke(() => (d as GroupVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as GroupVM).Notes_CoerceValue(baseValue)));

        /// <summary>
        /// Notes for group
        /// </summary>
        public string Notes
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(NotesProperty));
                return Dispatcher.Invoke(() => Notes);
            }
            set
            {
                if (CheckAccess())
                    SetValue(NotesProperty, value);
                else
                    Dispatcher.Invoke(() => Notes = value);
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

        #region CredentialCollection Property Members

        public const string PropertyName_CredentialCollection = "CredentialCollection";

        private static readonly DependencyPropertyKey CredentialCollectionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_CredentialCollection, typeof(ObservableCollection<CredentialVM>), typeof(GroupVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="CredentialCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CredentialCollectionProperty = CredentialCollectionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<CredentialVM> CredentialCollection
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<CredentialVM>)(GetValue(CredentialCollectionProperty));
                return Dispatcher.Invoke(() => CredentialCollection);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(CredentialCollectionPropertyKey, value);
                else
                    Dispatcher.Invoke(() => CredentialCollection = value);
            }
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(GroupVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as GroupVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as GroupVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as GroupVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of selected credential
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (CheckAccess())
                    return (int)(GetValue(SelectedIndexProperty));
                return Dispatcher.Invoke(() => SelectedIndex);
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedIndexProperty, value);
                else
                    Dispatcher.Invoke(() => SelectedIndex = value);
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
            DeleteCredentialCommand.IsEnabled = isEnabled;
            if (isEnabled)
                SelectedCredential = CredentialCollection[newValue];
            else
                SelectedCredential = null;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            if (CredentialCollection.Count < 0)
                return -1;

            int? index = baseValue as int?;
            if (!index.HasValue)
                index = SelectedIndex;

            if (index.Value >= CredentialCollection.Count)
                return CredentialCollection.Count - 1;

            if (index.Value < -1)
                return -1;

            return index.Value;
        }

        #endregion

        #region SelectedCredential Property Members

        public const string PropertyName_SelectedCredential = "SelectedCredential";

        private static readonly DependencyPropertyKey SelectedCredentialPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedCredential, typeof(CredentialVM), typeof(GroupVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedCredential"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCredentialProperty = SelectedCredentialPropertyKey.DependencyProperty;

        /// <summary>
        /// Currently selected credential
        /// </summary>
        public CredentialVM SelectedCredential
        {
            get
            {
                if (CheckAccess())
                    return (CredentialVM)(GetValue(SelectedCredentialProperty));
                return Dispatcher.Invoke(() => SelectedCredential);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedCredentialPropertyKey, value);
                else
                    Dispatcher.Invoke(() => SelectedCredential = value);
            }
        }

        #endregion

        #region Parent Read-only Attached Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Parent group for credential.
        /// </summary>
        /// <param name="obj">Object from which to retieve the value.</param>
        /// <returns><see cref="GroupVM"/> value from <paramref name="obj"/>.</returns>
        public static GroupVM GetParent(DependencyObject obj)
        {
            return (GroupVM)obj.GetValue(ParentProperty);
        }

        private static void SetParent(DependencyObject obj, GroupVM value)
        {
            obj.SetValue(ParentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterAttachedReadOnly(DependencyPropertyName_Parent, typeof(GroupVM), typeof(GroupVM),
                new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => Parent_PropertyChanged(d, (GroupVM)(e.OldValue), (GroupVM)(e.NewValue))));

        /// <summary>
        /// Identifies the Parent attached read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = ParentPropertyKey.DependencyProperty;

        private static void Parent_PropertyChanged(DependencyObject d, GroupVM oldvalue, GroupVM newValue)
        {
            CredentialVM vm;
            if (newValue != null && (vm = d as CredentialVM) != null)
                vm.SetBrowsersCollection(newValue._browsers);
        }

        #endregion

        #region NewCredential Command Property Members

        private RelayCommand _newCredentialCommand = null;

        public RelayCommand NewCredentialCommand
        {
            get
            {
                if (_newCredentialCommand == null)
                    _newCredentialCommand = new RelayCommand(OnNewCredential);

                return _newCredentialCommand;
            }
        }

        protected virtual void OnNewCredential(object parameter)
        {
            SelectedCredential = new CredentialVM();
            if (SelectedIndex < CredentialCollection.Count - 1)
                CredentialCollection.Insert(SelectedIndex, SelectedCredential);
            else
                CredentialCollection.Add(SelectedCredential);
        }

        #endregion

        #region DeleteCredential Command Property Members

        private RelayCommand _deleteCredentialCommand = null;

        public RelayCommand DeleteCredentialCommand
        {
            get
            {
                if (_deleteCredentialCommand == null)
                    _deleteCredentialCommand = new RelayCommand(OnDeleteCredential, false, true);

                return _deleteCredentialCommand;
            }
        }

        protected virtual void OnDeleteCredential(object parameter)
        {
            DeleteCredentialVisibility = Visibility.Visible;
        }

        #endregion

        #region DeleteCredentialVisibility Property Members

        public const string PropertyName_DeleteCredentialVisibility = "DeleteCredentialVisibility";

        private static readonly DependencyPropertyKey DeleteCredentialVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteCredentialVisibility, typeof(Visibility), typeof(GroupVM),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <seealso cref="DeleteCredentialVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteCredentialVisibilityProperty = DeleteCredentialVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Shows or hides delete credential box
        /// </summary>
        public Visibility DeleteCredentialVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(DeleteCredentialVisibilityProperty));
                return Dispatcher.Invoke(() => DeleteCredentialVisibility);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteCredentialVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(() => DeleteCredentialVisibility = value);
            }
        }

        #endregion
        
        #region DeleteCredentialYes Command Property Members

        private RelayCommand _deleteCredentialYesCommand = null;

        public RelayCommand DeleteCredentialYesCommand
        {
            get
            {
                if (_deleteCredentialYesCommand == null)
                    _deleteCredentialYesCommand = new RelayCommand(OnDeleteCredentialYes);

                return _deleteCredentialYesCommand;
            }
        }

        protected virtual void OnDeleteCredentialYes(object parameter)
        {
            CredentialCollection.RemoveAt(SelectedIndex);
            DeleteCredentialVisibility = Visibility.Collapsed;
        }

        #endregion

        #region DeleteCredentialNo Command Property Members

        private RelayCommand _deleteCredentialNoCommand = null;

        public RelayCommand DeleteCredentialNoCommand
        {
            get
            {
                if (_deleteCredentialNoCommand == null)
                    _deleteCredentialNoCommand = new RelayCommand(OnDeleteCredentialNo);

                return _deleteCredentialNoCommand;
            }
        }

        protected virtual void OnDeleteCredentialNo(object parameter)
        {
            DeleteCredentialVisibility = Visibility.Collapsed;
        }

        #endregion

        public GroupVM()
        {
            CredentialCollection = new ObservableCollection<CredentialVM>();
            CredentialCollection.CollectionChanged += CredentialCollection_CollectionChanged;
        }

        private void CredentialCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                CredentialVM[] toRemove = e.OldItems.OfType<CredentialVM>().Where(o => !CredentialCollection.Any(i => i != null && ReferenceEquals(i, o))).ToArray();
                foreach (CredentialVM item in toRemove)
                {
                    GroupVM vm = GetParent(item);
                    if (vm != null && ReferenceEquals(vm, this))
                        SetParent(item, null);
                }
            }

            foreach (CredentialVM item in CredentialCollection)
            {
                if (item == null)
                    continue;

                GroupVM vm = GetParent(item);
                if (vm != null)
                {
                    if (ReferenceEquals(vm, this))
                        continue;
                    vm.CredentialCollection.Remove(item);
                }
                SetParent(item, this);
            }

            if (SelectedIndex < 0 || CredentialCollection.Count == 0)
            {
                SelectedIndex = -1;
                return;
            }

            int index = CredentialCollection.IndexOf(SelectedCredential);
            if (index < 0)
            {
                if (SelectedIndex > 0)
                    SelectedIndex -= 1;
                else
                    SelectedCredential = CredentialCollection[0];
            }
            else if (SelectedIndex != index)
                SelectedIndex = index;
        }

        private ObservableCollection<BrowserVM> _browsers = new ObservableCollection<BrowserVM>();

        internal void SetBrowsersCollection(ObservableCollection<BrowserVM> browsers)
        {
            _browsers = browsers;
            foreach (CredentialVM credential in CredentialCollection.Where(c => c != null))
                credential.SetBrowsersCollection(browsers);
        }
    }
}