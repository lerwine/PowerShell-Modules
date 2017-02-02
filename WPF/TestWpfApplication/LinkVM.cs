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
    public class LinkVM : DependencyObject
    {
        #region Title Property Members

        public const string DependencyPropertyName_Title = "Title";

        /// <summary>
        /// Identifies the <seealso cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(DependencyPropertyName_Title, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Title_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Title_CoerceValue(baseValue)));

        /// <summary>
        /// Title of link
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
            // TODO: Implement LinkVM.Title_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Title"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Title_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Title_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Url Property Members

        public const string DependencyPropertyName_Url = "Url";

        /// <summary>
        /// Identifies the <seealso cref="Url"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register(DependencyPropertyName_Url, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Url_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Url_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Url_CoerceValue(baseValue)));

        /// <summary>
        /// Url of link
        /// </summary>
        public string Url
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(UrlProperty));
                return (string)(Dispatcher.Invoke(new Func<string>(() => Url)));
            }
            set
            {
                if (CheckAccess())
                    SetValue(UrlProperty, value);
                else
                    Dispatcher.Invoke(new Action(() => Url = value));
            }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="Url"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="string"/> value before the <seealso cref="Url"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="string"/> value after the <seealso cref="Url"/> property was changed.</param>
        protected virtual void Url_PropertyChanged(string oldValue, string newValue)
        {
            // TODO: Implement LinkVM.Url_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Url"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Url_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Url_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Notes Property Members

        public const string DependencyPropertyName_Notes = "Notes";

        /// <summary>
        /// Identifies the <seealso cref="Notes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotesProperty = DependencyProperty.Register(DependencyPropertyName_Notes, typeof(string), typeof(LinkVM),
                new PropertyMetadata("",
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).Notes_PropertyChanged((string)(e.OldValue), (string)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).Notes_CoerceValue(baseValue)));

        /// <summary>
        /// Notes for link
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
            // TODO: Implement LinkVM.Notes_PropertyChanged(string, string)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="Notes"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual string Notes_CoerceValue(object baseValue)
        {
            // TODO: Implement LinkVM.Notes_CoerceValue(DependencyObject, object)
            return (baseValue as string) ?? "";
        }

        #endregion

        #region Browsers Property Members

        private ObservableCollection<BrowserVM> _browsers= null;

        public const string PropertyName_Browsers = "Browsers";

        private static readonly DependencyPropertyKey BrowsersPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_Browsers, typeof(ReadOnlyObservableCollection<BrowserVM>), typeof(LinkVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="Browsers"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BrowsersProperty = BrowsersPropertyKey.DependencyProperty;

        /// <summary>
        /// Browser options
        /// </summary>
        public ReadOnlyObservableCollection<BrowserVM> Browsers
        {
            get
            {
                if (CheckAccess())
                    return (ReadOnlyObservableCollection<BrowserVM>)(GetValue(BrowsersProperty));
                return (ReadOnlyObservableCollection<BrowserVM>)(Dispatcher.Invoke(new Func<ReadOnlyObservableCollection<BrowserVM>>(() => Browsers)));
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

        #region SelectedIndex Property Members

        public const string DependencyPropertyName_SelectedIndex = "SelectedIndex";

        /// <summary>
        /// Identifies the <seealso cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(DependencyPropertyName_SelectedIndex, typeof(int), typeof(LinkVM),
                new PropertyMetadata(-1,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as LinkVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(new Action(() => (d as LinkVM).SelectedIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue))));
                },
                (DependencyObject d, object baseValue) => (d as LinkVM).SelectedIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Index of selected browser
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
            if (newValue < 0)
                SelectedBrowser = null;
            else
                SelectedBrowser = Browsers[newValue];
        }

        /// <summary>
        /// This gets called whenever <seealso cref="SelectedIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int SelectedIndex_CoerceValue(object baseValue)
        {
            int? index = baseValue as int?;
            if (!index.HasValue || index.Value < 0 || index >= Browsers.Count)
                return -1;

            return index.Value;
        }

        #endregion

        #region SelectedBrowser Property Members

        public const string PropertyName_SelectedBrowser = "SelectedBrowser";

        private static readonly DependencyPropertyKey SelectedBrowserPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_SelectedBrowser, typeof(BrowserVM), typeof(LinkVM),
                new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <seealso cref="SelectedBrowser"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBrowserProperty = SelectedBrowserPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public BrowserVM SelectedBrowser
        {
            get
            {
                if (CheckAccess())
                    return (BrowserVM)(GetValue(SelectedBrowserProperty));
                return (BrowserVM)(Dispatcher.Invoke(new Func<BrowserVM>(() => SelectedBrowser)));
            }
            private set
            {
                if (CheckAccess())
                    SetValue(SelectedBrowserPropertyKey, value);
                else
                    Dispatcher.Invoke(new Action(() => SelectedBrowser = value));
            }
        }

        #endregion

        public LinkVM()
        {
            SetBrowsersCollection(new ObservableCollection<BrowserVM>());
        }

        internal void SetBrowsersCollection(ObservableCollection<BrowserVM> browsers)
        {
            if (_browsers != null)
                _browsers.CollectionChanged -= Browsers_CollectionChanged;
            _browsers = browsers;
            Browsers = new ReadOnlyObservableCollection<BrowserVM>(_browsers);
            _browsers.CollectionChanged += Browsers_CollectionChanged;
            OnBrowsersChanged();
        }

        private void Browsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnBrowsersChanged();
        }

        private void OnBrowsersChanged()
        {
            if (SelectedBrowser == null || !_browsers.Any(b => b != null && ReferenceEquals(b, SelectedBrowser)))
            {
                if (_browsers.Count == 0)
                {
                    SelectedIndex = -1;
                    return;
                }

                SelectedBrowser = _browsers.FirstOrDefault(b => b != null && MainWindowVM.GetPreferredFor(b) != null);
                if (SelectedBrowser == null)
                    SelectedBrowser = _browsers.FirstOrDefault();
            }

            SelectedIndex = _browsers.IndexOf(SelectedBrowser);
        }
    }
}