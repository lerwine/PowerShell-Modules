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
    public class DomainVM : DependencyObject
    {
        #region Title Property Members

        public const string DependencyPropertyName_Title = "Title";

        /// <summary>
        /// Identifies the <seealso cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(DependencyPropertyName_Title, typeof(string), typeof(DomainVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DomainVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as DomainVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as DomainVM).Title_CoerceValue(baseValue)));

        /// <summary>
        /// Title of domain
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
            // TODO: Implement DomainVM.Title_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Title"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Title_CoerceValue(object baseValue)
        {
            // TODO: Implement DomainVM.Title_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Notes Property Members

        public const string DependencyPropertyName_Notes = "Notes";

        /// <summary>
        /// Identifies the <seealso cref="Notes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register(DependencyPropertyName_Notes, typeof(string), typeof(DomainVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DomainVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as DomainVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as DomainVM).Notes_CoerceValue(baseValue)));

        /// <summary>
        /// Domain notes
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
            // TODO: Implement DomainVM.Notes_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Notes"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Notes_CoerceValue(object baseValue)
        {
            // TODO: Implement DomainVM.Notes_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region GroupCollection Property Members

        public const string PropertyName_GroupCollection = "GroupCollection";

        private static readonly DependencyPropertyKey GroupCollectionPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_GroupCollection, typeof(ObservableCollection<GroupVM>), typeof(DomainVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="GroupCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupCollectionProperty = GroupCollectionPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GroupVM> GroupCollection
        {
            get
            {
                if (CheckAccess())
                    return (ObservableCollection<GroupVM>)(GetValue(GroupCollectionProperty));
                return (ObservableCollection<GroupVM>)(Dispatcher.Invoke(new Func<ObservableCollection<GroupVM>>(() => GroupCollection)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(GroupCollectionPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => GroupCollection = value));
            }
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(DomainVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as DomainVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as DomainVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as DomainVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of selected group.
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
            DeleteGroupCommand.IsEnabled = isEnabled;
            if (isEnabled)
                SelectedGroup = GroupCollection[newValue];
            else
                SelectedGroup = null;
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            if (GroupCollection.Count < 0)
                return -1;

            int? index = baseValue as int?;
            if (!index.HasValue)
                index = SelectedIndex;

            if (index.Value >= GroupCollection.Count)
                return GroupCollection.Count - 1;

            if (index.Value < -1)
                return -1;

            return index.Value;
        }

        #endregion

        #region SelectedGroup Property Members

        public const string PropertyName_SelectedGroup = "SelectedGroup";

        private static readonly DependencyPropertyKey SelectedGroupPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedGroup, typeof(GroupVM), typeof(DomainVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedGroup"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedGroupProperty = SelectedGroupPropertyKey.DependencyProperty;

        /// <summary>
        /// Currently selected group
        /// </summary>
        public GroupVM SelectedGroup
        {
            get
            {
                if (CheckAccess())
                    return (GroupVM)(GetValue(SelectedGroupProperty));
                return (GroupVM)(Dispatcher.Invoke(new Func<GroupVM>(() => SelectedGroup)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedGroupPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedGroup = value));
            }
        }

        #endregion

        #region Parent Read-only Attached Property Members

        public const string DependencyPropertyName_Parent = "Parent";

        /// <summary>
        /// Parent domain
        /// </summary>
        /// <param name="obj">Object from which to retieve the value.</param>
        /// <returns><see cref="DomainVM"/> value from <paramref name="obj"/>.</returns>
        public static DomainVM GetParent(DependencyObject obj)
        {
            return (DomainVM)obj.GetValue(ParentProperty);
        }

        private static void SetParent(DependencyObject obj, DomainVM value)
        {
            obj.SetValue(ParentPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ParentPropertyKey = DependencyProperty.RegisterAttachedReadOnly(DependencyPropertyName_Parent, typeof(DomainVM), typeof(DomainVM),
                new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) => Parent_PropertyChanged(d, (DomainVM)(e.OldValue), (DomainVM)(e.NewValue))));

        /// <summary>
        /// Identifies the Parent attached read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentProperty = ParentPropertyKey.DependencyProperty;

        private static void Parent_PropertyChanged(DependencyObject d, DomainVM oldvalue, DomainVM newValue)
        {
            GroupVM vm;
            if (newValue != null && (vm = d as GroupVM) != null)
                vm.SetBrowsersCollection(newValue._browsers);
        }

        #endregion

        #region DeleteGroupVisibility Property Members

        public const string PropertyName_DeleteGroupVisibility = "DeleteGroupVisibility";

        private static readonly DependencyPropertyKey DeleteGroupVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteGroupVisibility, typeof(Visibility), typeof(DomainVM),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <seealso cref="DeleteGroupVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteGroupVisibilityProperty = DeleteGroupVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Controls whether delete group box is visible
        /// </summary>
        public Visibility DeleteGroupVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(DeleteGroupVisibilityProperty));
                return (Visibility)(Dispatcher.Invoke(new Func<Visibility>(() => DeleteGroupVisibility)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteGroupVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DeleteGroupVisibility = value));
            }
        }

        #endregion

        #region DeleteGroupConfirmMessage Property Members

        public const string PropertyName_DeleteGroupConfirmMessage = "DeleteGroupConfirmMessage";

        private static readonly DependencyPropertyKey DeleteGroupConfirmMessagePropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteGroupConfirmMessage, typeof(string), typeof(DomainVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="DeleteGroupConfirmMessage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteGroupConfirmMessageProperty = DeleteGroupConfirmMessagePropertyKey.DependencyProperty;

        /// <summary>
        /// Message to display for confirming group delete.
        /// </summary>
        public string DeleteGroupConfirmMessage
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(DeleteGroupConfirmMessageProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => DeleteGroupConfirmMessage)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteGroupConfirmMessagePropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DeleteGroupConfirmMessage = value));
            }
        }

        #endregion

        #region NewGroup Command Property Members

        private RelayCommand _newGroupCommand = null;

        public RelayCommand NewGroupCommand
        {
            get
            {
                if (_newGroupCommand == null)
                    _newGroupCommand = new RelayCommand(OnNewGroup);

                return _newGroupCommand;
            }
        }

        protected virtual void OnNewGroup(object parameter)
        {
            SelectedGroup = new GroupVM();
            if (SelectedIndex < GroupCollection.Count - 1)
                GroupCollection.Insert(SelectedIndex, SelectedGroup);
            else
                GroupCollection.Add(SelectedGroup);
        }

        #endregion

        #region DeleteGroup Command Property Members

        private RelayCommand _deleteGroupCommand = null;

        public RelayCommand DeleteGroupCommand
        {
            get
            {
                if (_deleteGroupCommand == null)
                    _deleteGroupCommand = new RelayCommand(OnDeleteGroup, false, true);

                return _deleteGroupCommand;
            }
        }

        protected virtual void OnDeleteGroup(object parameter)
        {
            DeleteGroupVisibility = Visibility.Visible;
            int credentialCount = SelectedGroup.CredentialCollection.Count;
            if (credentialCount == 1)
                DeleteGroupConfirmMessage = "This group contains 1 credential.";
            else
                DeleteGroupConfirmMessage = String.Format("This group contains {1} credentials.", credentialCount);
        }

        #endregion

        #region DeleteGroupYes Command Property Members

        private RelayCommand _deleteGroupYesCommand = null;

        public RelayCommand DeleteGroupYesCommand
        {
            get
            {
                if (_deleteGroupYesCommand == null)
                    _deleteGroupYesCommand = new RelayCommand(OnDeleteGroupYes);

                return _deleteGroupYesCommand;
            }
        }

        protected virtual void OnDeleteGroupYes(object parameter)
        {
            GroupCollection.RemoveAt(SelectedIndex);
            DeleteGroupVisibility = Visibility.Collapsed;
        }

        #endregion

        #region DeleteGroupNo Command Property Members

        private RelayCommand _deleteGroupNoCommand = null;

        public RelayCommand DeleteGroupNoCommand
        {
            get
            {
                if (_deleteGroupNoCommand == null)
                    _deleteGroupNoCommand = new RelayCommand(OnDeleteGroupNo);

                return _deleteGroupNoCommand;
            }
        }

        protected virtual void OnDeleteGroupNo(object parameter)
        {
            DeleteGroupVisibility = Visibility.Collapsed;
        }

        #endregion

        public DomainVM()
        {
            SelectedGroup = new GroupVM();
            GroupCollection = new ObservableCollection<GroupVM>();
            GroupCollection.CollectionChanged += GroupCollection_CollectionChanged;
            GroupCollection.Add(SelectedGroup);
        }
        
        private void GroupCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                GroupVM[] toRemove = e.OldItems.OfType<GroupVM>().Where(o => !GroupCollection.Any(i => i != null && ReferenceEquals(i, o))).ToArray();
                foreach (GroupVM item in toRemove)
                {
                    DomainVM vm = GetParent(item);
                    if (vm != null && ReferenceEquals(vm, this))
                        SetParent(item, null);
                }
            }

            foreach (GroupVM item in GroupCollection)
            {
                if (item == null)
                    continue;

                DomainVM vm = GetParent(item);
                if (vm != null)
                {
                    if (ReferenceEquals(vm, this))
                        continue;
                    vm.GroupCollection.Remove(item);
                }
                SetParent(item, this);
            }

            if (GroupCollection.Count == 0)
            {
                SelectedGroup = new GroupVM();
                GroupCollection.Add(SelectedGroup);
                return;
            }

            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
                return;
            }

            int index = GroupCollection.IndexOf(SelectedGroup);
            if (index < 0)
            {
                if (SelectedIndex > 0)
                    SelectedIndex -= 1;
                else
                    SelectedGroup = GroupCollection[0];
            }
            else if (SelectedIndex != index)
                SelectedIndex = index;
        }

        private ObservableCollection<BrowserVM> _browsers = new ObservableCollection<BrowserVM>();

        internal void SetBrowsersCollection(ObservableCollection<BrowserVM> browsers)
        {
            foreach (GroupVM item in GroupCollection.Where(g => g != null))
                item.SetBrowsersCollection(browsers);
            _browsers = browsers;
        }
    }
}