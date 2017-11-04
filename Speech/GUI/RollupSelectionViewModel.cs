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
    /// View Model for RollupSelectionWindow.xaml
    /// </summary>
    public class RollupSelectionViewModel : DependencyObject
    {
        public event EventHandler OnTerminalCommand;

        private static ObservableCollection<NewFileTypeViewModel> _newFileTypeOptions = null;

        public RollupSelectionViewModel(IEnumerable<FileInfo> files)
        {
            if (_newFileTypeOptions == null)
            {
                _newFileTypeOptions = new ObservableCollection<NewFileTypeViewModel>();
                _newFileTypeOptions.Add(new NewFileTypeViewModel(false));
                _newFileTypeOptions.Add(new NewFileTypeViewModel(true));
            }
            NewFileTypeOptions = new ReadOnlyObservableCollection<NewFileTypeViewModel>(_newFileTypeOptions);
            SelectedNewFileType = _newFileTypeOptions[0];

            ObservableCollection<RollupItemViewModel> rollupItems = ObservableCollection<RollupItemViewModel>();
            RollupItems = new ReadOnlyObservableCollection<RollupItemViewModel>(rollupItems);
            if (files == null)
                return;
            foreach (IGrouping<FileInfo> r in files.Where(f => f != null).GroupBy(f => f.FullName.ToLower()).Select(g => g.First()).GroupBy(f => f.Name.ToLower()))
            {
                FileInfo textFile = r.FirstOrDefault(f => String.Equals(f.Extension, ".txt", StringComparison.InvariantCultureIgnoreCase));
                FileInfo xmlFile = r.FirstOrDefault(f => String.Equals(f.Extension, ".xml", StringComparison.InvariantCultureIgnoreCase));
                string name;

                if (xmlFile == null)
                {
                    if (textFile == null)
                        continue;
                    name = textFile.Name;
                }
                else
                    name = xmlFile.Name;
                rollupItems.Add(new RollupItemViewModel(Path.GetFileNameWithoutExtension(name), textFile != null, xmlFile != null, r.FirstOrDefault(f => String.Equals(f.Extension, ".wav", StringComparison.InvariantCultureIgnoreCase)) != null));
            }
        }

        #region RollupItems Property Members

        public const string PropertyName_RollupItems = "RollupItems";

        private static readonly DependencyPropertyKey RollupItemsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_RollupItems, typeof(ReadOnlyObservableCollection<RollupItemViewModel>), typeof(RollupSelectionViewModel),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="RollupItems"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RollupItemsProperty = RollupItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Collection of rolled up file items.
        /// </summary>
        public ReadOnlyObservableCollection<RollupItemViewModel> RollupItems
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<RollupItemViewModel>)(GetValue(RollupItemsProperty));
                return (ReadOnlyObservableCollection<RollupItemViewModel>)(Dispatcher.Invoke(new Func<ReadOnlyObservableCollection<RollupItemViewModel>>(() => RollupItems)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(RollupItemsPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => RollupItems = value));
            }
        }

        #endregion

        #region SelectedRollupItem Property Members

        public const string PropertyName_SelectedRollupItem = "SelectedRollupItem";

        /// <summary>
        /// Identifies the <seealso cref="SelectedRollupItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedRollupItemProperty = DependencyProperty.Register(PropertyName_SelectedRollupItem, typeof(RollupItemViewModel), typeof(RollupSelectionViewModel),
                new PropertyMetadata(null,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as RollupSelectionViewModel).SelectedRollupItem_PropertyChanged((RollupItemViewModel)(e.OldValue), (RollupItemViewModel)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as RollupSelectionViewModel).SelectedRollupItem_PropertyChanged((RollupItemViewModel)(e.OldValue), (RollupItemViewModel)(e.NewValue))));
                }));

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="SelectedRollupItem"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="RollupItemViewModel"/> value before the <seealso cref="SelectedRollupItem"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="RollupItemViewModel"/> value after the <seealso cref="SelectedRollupItem"/> property was changed.</param>
        protected virtual void SelectedRollupItem_PropertyChanged(RollupItemViewModel oldValue, RollupItemViewModel newValue)
        {
            if (newValue == null)
            {
                EditTextCommand.Enabled = false;
                EditXmlCommand.Enabled = false;
                GenerateXmlCommand.Enabled = false;
                SpeakCommand.Enabled = false;
                GenerateAudioCommand.Enabled = false;
                return;
            }

            if (newValue.HasText)
            {
                EditTextCommand.Enabled = true;
                EditXmlCommand.Enabled = true;
                GenerateXmlCommand.Enabled = true;
                SpeakCommand.Enabled = newValue.HasXml;
                GenerateAudioCommand.Enabled = true;
                return;
            }

            EditTextCommand.Enabled = false;
            GenerateXmlCommand.Enabled = false;

            if (newValue.HasXml)
            {
                EditXmlCommand.Enabled = true;
                SpeakCommand.Enabled = true;
                GenerateAudioCommand.Enabled = true;
            }
            else
            {
                EditXmlCommand.Enabled = false;
                SpeakCommand.Enabled = false;
                GenerateAudioCommand.Enabled = false;
            }
        }

        /// <summary>
        /// Currently selected rolled up file name.
        /// </summary>
        public RollupItemViewModel SelectedRollupItem
        {
            get
            {
                if (CheckAccess())
                    return (RollupItemViewModel)(GetValue(SelectedRollupItemProperty));
                return (RollupItemViewModel)(Dispatcher.Invoke(new Func<RollupItemViewModel>(() => SelectedRollupItem)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedRollupItemProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedRollupItem = value));
            }
        }

        #endregion

        #region NewFileTypeOptions Property Members

        public const string PropertyName_NewFileTypeOptions = "NewFileTypeOptions";

        private static readonly DependencyPropertyKey NewFileTypeOptionsPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_NewFileTypeOptions, typeof(ReadOnlyObservableCollection<NewFileTypeViewModel>), typeof(RollupSelectionViewModel),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="NewFileTypeOptions"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NewFileTypeOptionsProperty = NewFileTypeOptionsPropertyKey.DependencyProperty;

        /// <summary>
        /// Collection of domains
        /// </summary>
        public ReadOnlyObservableCollection<NewFileTypeViewModel> NewFileTypeOptions
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<NewFileTypeViewModel>)(GetValue(NewFileTypeOptionsProperty));
                return (ReadOnlyObservableCollection<NewFileTypeViewModel>)(Dispatcher.Invoke(new Func<ReadOnlyObservableCollection<NewFileTypeViewModel>>(() => NewFileTypeOptions)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(NewFileTypeOptionsPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => NewFileTypeOptions = value));
            }
        }

        #endregion

        #region SelectedNewFileType Property Members

        public const string PropertyName_SelectedNewFileType = "SelectedNewFileType";

        /// <summary>
        /// Identifies the <seealso cref="SelectedNewFileType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedNewFileTypeProperty = DependencyProperty.Register(PropertyName_SelectedNewFileType, typeof(NewFileTypeViewModel), typeof(RollupSelectionViewModel),
                new PropertyMetadata(null));

        /// <summary>
        /// Currently selected new file type
        /// </summary>
        public NewFileTypeViewModel SelectedNewFileType
        {
            get
            {
                if (CheckAccess())
                    return (NewFileTypeViewModel)(GetValue(SelectedNewFileTypeProperty));
                return (NewFileTypeViewModel)(Dispatcher.Invoke(new Func<NewFileTypeViewModel>(() => SelectedNewFileType)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(SelectedNewFileTypeProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedNewFileType = value));
            }
        }

        #endregion

        #region NextAction Property Members

        public const string PropertyDirectoryName_HasText = "NextAction";

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyDirectoryName_HasText, typeof(NextActionType), typeof(RollupItemViewModel),
                new PropertyMetadata(NextActionType.None));

        /// <summary>
        /// Identifies the <seealso cref="NextAction"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Next action
        /// </summary>
        public NextActionType NextAction
        {
            get
            {
                if (CheckAccess())
                    return (NextActionType)(GetValue(HasTextProperty));
                return (NextActionType)(Dispatcher.Invoke(new Func<NextActionType>(() => NextAction)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(HasTextPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => NextAction = value));
            }
        }

        #endregion

        #region NewFile Command Property Members

        private RelayCommand _newFileCommand = null;

        public RelayCommand NewFileCommand
        {
            get
            {
                if (_newFileCommand == null)
                    _newFileCommand = new RelayCommand(OnNewFile, false, true);

                return _newFileCommand;
            }
        }

        protected virtual void OnNewFile(object parameter)
        {
            NextAction = NextActionType.NewFile;
            RaiseTerminalCommand();
        }

        #endregion

        #region EditText Command Property Members

        private RelayCommand _editTextCommand = null;

        public RelayCommand EditTextCommand
        {
            get
            {
                if (_editTextCommand == null)
                    _editTextCommand = new RelayCommand(OnEditText, false, true);

                return _editTextCommand;
            }
        }

        protected virtual void OnEditText(object parameter)
        {
            NextAction = NextActionType.EditText;
            RaiseTerminalCommand();
        }

        #endregion

        #region EditXml Command Property Members

        private RelayCommand _editXmlCommand = null;

        public RelayCommand EditXmlCommand
        {
            get
            {
                if (_editXmlCommand == null)
                    _editXmlCommand = new RelayCommand(OnEditXml, false, true);

                return _editXmlCommand;
            }
        }

        protected virtual void OnEditXml(object parameter)
        {
            NextAction = NextActionType.EditXml;
            RaiseTerminalCommand();
        }

        #endregion

        #region GenerateXml Command Property Members

        private RelayCommand _generateXmlCommand = null;

        public RelayCommand GenerateXmlCommand
        {
            get
            {
                if (_generateXmlCommand == null)
                    _generateXmlCommand = new RelayCommand(OnGenerateXml, false, true);

                return _generateXmlCommand;
            }
        }

        protected virtual void OnGenerateXml(object parameter)
        {
            NextAction = NextActionType.GenerateXml;
            RaiseTerminalCommand();
        }

        #endregion

        #region Speak Command Property Members

        private RelayCommand _speakCommand = null;

        public RelayCommand SpeakCommand
        {
            get
            {
                if (_speakCommand == null)
                    _speakCommand = new RelayCommand(OnSpeak, false, true);

                return _speakCommand;
            }
        }

        protected virtual void OnSpeak(object parameter)
        {
            NextAction = NextActionType.Speak;
            RaiseTerminalCommand();
        }

        #endregion

        #region GenerateAudio Command Property Members

        private RelayCommand _generateAudioCommand = null;

        public RelayCommand GenerateAudioCommand
        {
            get
            {
                if (_generateAudioCommand == null)
                    _generateAudioCommand = new RelayCommand(OnGenerateAudio, false, true);

                return _generateAudioCommand;
            }
        }

        protected virtual void OnGenerateAudio(object parameter)
        {
            NextAction = NextActionType.GenerateAudio;
            RaiseTerminalCommand();
        }

        #endregion

        #region Exit Command Property Members

        private RelayCommand _exitCommand = null;

        public RelayCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(OnExit, false, true);

                return _exitCommand;
            }
        }

        protected virtual void OnExit(object parameter)
        {
            NextAction = NextActionType.Exit;
            RaiseTerminalCommand();
        }

        #endregion

        protected void RaiseTerminalCommand()
        {
            OnTerminalCommand ??.Invoke(this, EventArgs.Empty);
        }
    }
}