using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CredentialStorageLibrary
{
    public abstract class NestedDependencyParent<TImplemented, TItem> : DependencyObject, INestedDependencyParent<TImplemented, TItem>
        where TItem : DependencyObject, INestedDependencyObject<TItem, TImplemented>, new()
        where TImplemented : NestedDependencyParent<TImplemented, TItem>
    {
        #region Items Property Members

        public const string PropertyName_Items = "Items";

        private static readonly DependencyPropertyKey ItemsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Items, typeof(NestedDependencyObjectCollection<TImplemented, TItem>), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Items collection
        /// </summary>
        public NestedDependencyObjectCollection<TImplemented, TItem> Items
        {
            get
            {
                if (CheckAccess())
                    return (NestedDependencyObjectCollection<TImplemented, TItem>)(GetValue(ItemsProperty));
                return (NestedDependencyObjectCollection<TImplemented, TItem>)(Dispatcher.Invoke(new Func<NestedDependencyObjectCollection<TImplemented, TItem>>(() => Items)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ItemsPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => Items = value));
            }
        }

        #endregion

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as NestedDependencyParent<TImplemented, TItem>).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as NestedDependencyParent<TImplemented, TItem>).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as NestedDependencyParent<TImplemented, TItem>).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// 
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
            // TODO: Implement NestedDependencyParent<TImplemented, TItem>.SelectedIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement NestedDependencyParent<TImplemented, TItem>.SelectedIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region SelectedItem Property Members

        public const string PropertyName_SelectedItem = "SelectedItem";

        private static readonly DependencyPropertyKey SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedItem, typeof(TItem), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TItem SelectedItem
        {
            get
            {
                if (CheckAccess())
                    return (TItem)(GetValue(SelectedItemProperty));
                return (TItem)(Dispatcher.Invoke(new Func<TItem>(() => SelectedItem)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedItemPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedItem = value));
            }
        }

        #endregion

        #region EditingItem Property Members

        public const string PropertyName_EditingItem = "EditingItem";

        private static readonly DependencyPropertyKey EditingItemPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_EditingItem, typeof(TItem), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="EditingItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EditingItemProperty = EditingItemPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public TItem EditingItem
        {
            get
            {
                if (CheckAccess())
                    return (TItem)(GetValue(EditingItemProperty));
                return (TItem)(Dispatcher.Invoke(new Func<TItem>(() => EditingItem)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(EditingItemPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => EditingItem = value));
            }
        }

        #endregion

        #region DeleteConfirmVisibility Property Members

        public const string PropertyName_DeleteConfirmVisibility = "DeleteConfirmVisibility";

        private static readonly DependencyPropertyKey DeleteConfirmVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_DeleteConfirmVisibility, typeof(Visibility), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <seealso cref="DeleteConfirmVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeleteConfirmVisibilityProperty = DeleteConfirmVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public Visibility DeleteConfirmVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(DeleteConfirmVisibilityProperty));
                return (Visibility)(Dispatcher.Invoke(new Func<Visibility>(() => DeleteConfirmVisibility)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(DeleteConfirmVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => DeleteConfirmVisibility = value));
            }
        }

        #endregion

        #region EditControlsVisibility Property Members

        public const string PropertyName_EditControlsVisibility = "EditControlsVisibility";

        private static readonly DependencyPropertyKey EditControlsVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_EditControlsVisibility, typeof(Visibility), typeof(NestedDependencyParent<TImplemented, TItem>),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <seealso cref="EditControlsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EditControlsVisibilityProperty = EditControlsVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public Visibility EditControlsVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(EditControlsVisibilityProperty));
                return (Visibility)(Dispatcher.Invoke(new Func<Visibility>(() => EditControlsVisibility)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(EditControlsVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => EditControlsVisibility = value));
            }
        }

        #endregion

        #region EditItem Command Property Members

        private Command.RelayCommand _editItemCommand = null;

        public Command.RelayCommand EditItemCommand
        {
            get
            {
                if (_editItemCommand == null)
                    _editItemCommand = new Command.RelayCommand(OnEditItem);

                return _editItemCommand;
            }
        }

        protected virtual void OnEditItem(object parameter)
        {
            // TODO: Implement OnEditItem Logic
        }

        #endregion

        #region EditSave Command Property Members

        private Command.RelayCommand _editSaveCommand = null;

        public Command.RelayCommand EditSaveCommand
        {
            get
            {
                if (_editSaveCommand == null)
                    _editSaveCommand = new Command.RelayCommand(OnEditSave);

                return _editSaveCommand;
            }
        }

        protected virtual void OnEditSave(object parameter)
        {
            // TODO: Implement OnEditSave Logic
        }

        #endregion

        #region EditCancel Command Property Members

        private Command.RelayCommand _editCancelCommand = null;

        public Command.RelayCommand EditCancelCommand
        {
            get
            {
                if (_editCancelCommand == null)
                    _editCancelCommand = new Command.RelayCommand(OnEditCancel);

                return _editCancelCommand;
            }
        }

        protected virtual void OnEditCancel(object parameter)
        {
            // TODO: Implement OnEditCancel Logic
        }

        #endregion

        #region NewItem Command Property Members

        private Command.RelayCommand _newItemCommand = null;

        public Command.RelayCommand NewItemCommand
        {
            get
            {
                if (_newItemCommand == null)
                    _newItemCommand = new Command.RelayCommand(OnNewItem);

                return _newItemCommand;
            }
        }

        protected virtual void OnNewItem(object parameter)
        {
            // TODO: Implement OnNewItem Logic
        }

        #endregion

        #region DeleteItem Command Property Members

        private Command.RelayCommand _deleteItemCommand = null;

        public Command.RelayCommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                    _deleteItemCommand = new Command.RelayCommand(OnDeleteItem);

                return _deleteItemCommand;
            }
        }

        protected virtual void OnDeleteItem(object parameter)
        {
            // TODO: Implement OnDeleteItem Logic
        }

        #endregion

        #region DeleteYes Command Property Members

        private Command.RelayCommand _deleteYesCommand = null;

        public Command.RelayCommand DeleteYesCommand
        {
            get
            {
                if (_deleteYesCommand == null)
                    _deleteYesCommand = new Command.RelayCommand(OnDeleteYes);

                return _deleteYesCommand;
            }
        }

        protected virtual void OnDeleteYes(object parameter)
        {
            // TODO: Implement OnDeleteYes Logic
        }

        #endregion

        #region DeleteNo Command Property Members

        private Command.RelayCommand _deleteNoCommand = null;

        public Command.RelayCommand DeleteNoCommand
        {
            get
            {
                if (_deleteNoCommand == null)
                    _deleteNoCommand = new Command.RelayCommand(OnDeleteNo);

                return _deleteNoCommand;
            }
        }

        protected virtual void OnDeleteNo(object parameter)
        {
            // TODO: Implement OnDeleteNo Logic
        }

        #endregion
        
        public NestedDependencyParent()
        {
            Items = new NestedDependencyObjectCollection<TImplemented, TItem>(this as TImplemented);
            EditingItem = new TItem();
        }
    }
}
