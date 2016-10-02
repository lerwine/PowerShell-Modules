using System;
using System.Collections.Generic;
#if !PSLEGACY
using System.Linq;
#endif
using System.Text;
#if !PSLEGACY
using System.Threading.Tasks;
#endif
using System.Windows;

namespace IOUtilityCLR
{
    /// <summary>
    /// View model for displayed window.
    /// </summary>
    public class DisplayWindowVM : DependencyObject
    {
        /// <summary>
        /// Default text for window title.
        /// </summary>
        public const string DISPLAYWINDOW_TITLE_DEFAULT = "Window";

        /// <summary>
        /// Default window width.
        /// </summary>
        public const double DISPLAYWINDOW_WIDTH_DEFAULT = 800.0;

        /// <summary>
        /// Default window height.
        /// </summary>
        public const double DISPLAYWINDOW_HEIGHT_DEFAULT = 600.0;
        
        /// <summary>
        /// Defines dependency property for the window title.
        /// </summary>
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register("WindowTitle", typeof(string), typeof(DisplayWindowVM),
            new PropertyMetadata(DISPLAYWINDOW_TITLE_DEFAULT,
            new PropertyChangedCallback(WindowTitle_PropertyChanged), new CoerceValueCallback(WindowTitle_CoerceValue)));

        /// <summary>
        /// Window title
        /// </summary>
        public string WindowTitle
        {
            get { return this.GetValue(DisplayWindowVM.WindowTitleProperty) as string; }
            set { this.SetValue(DisplayWindowVM.WindowTitleProperty, value ?? ""); }
        }

        private static void WindowTitle_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
               (d as DisplayWindowVM).OnWindowTitleChanged(e.OldValue as string, e.NewValue as string);
        }

        private static object WindowTitle_CoerceValue(DependencyObject d, object baseValue)
        {
            return (d as DisplayWindowVM).CoerceWindowTitle(baseValue as string);
        }

        /// <summary>
        /// This gets called when the window title text has changed.
        /// </summary>
        /// <param name="oldTitle">Previous window title.</param>
        /// <param name="newTitle">New window title.</param>
        protected virtual void OnWindowTitleChanged(string oldTitle, string newTitle) { }

        /// <summary>
        /// This gets called when new window title text is being coerced.
        /// </summary>
        /// <param name="baseValue">Text being coerced.</param>
        /// <returns>Coerced window title text.</returns>
        protected virtual string CoerceWindowTitle(string baseValue)
        {
            string s = (baseValue ?? "").Trim();
            if (s.Length == 0)
                return DISPLAYWINDOW_TITLE_DEFAULT;
            return s;
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("WindowWidth", typeof(double), typeof(DisplayWindowVM),
            new PropertyMetadata(DISPLAYWINDOW_WIDTH_DEFAULT,
            new PropertyChangedCallback(WindowWidth_PropertyChanged), new CoerceValueCallback(WindowWidth_CoerceValue)));

        public double WindowWidth
        {
            get { return (double)(this.GetValue(DisplayWindowVM.WidthProperty)); }
            set { this.SetValue(DisplayWindowVM.WidthProperty, (value > 0) ? value : DISPLAYWINDOW_WIDTH_DEFAULT); }
        }

        private static void WindowWidth_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DisplayWindowVM).OnWindowWidthChanged((double)(e.OldValue), (double)(e.NewValue));
        }

        private static object WindowWidth_CoerceValue(DependencyObject d, object baseValue)
        {
            return (d as DisplayWindowVM).CoerceWindowWidth(baseValue as double?);
        }

        protected virtual void OnWindowWidthChanged(double oldWidth, double newWidth) { }

        protected virtual double CoerceWindowWidth(double? baseValue)
        {
            if (baseValue.HasValue && baseValue.Value > 0.0)
                return baseValue.Value;
            return DISPLAYWINDOW_WIDTH_DEFAULT;
        }

        public static readonly DependencyProperty WindowHeightProperty = DependencyProperty.Register("WindowHeight", typeof(double), typeof(DisplayWindowVM),
            new PropertyMetadata(DISPLAYWINDOW_HEIGHT_DEFAULT,
            new PropertyChangedCallback(WindowHeight_PropertyChanged), new CoerceValueCallback(WindowHeight_CoerceValue)));

        public double WindowHeight
        {
            get { return (double)(this.GetValue(DisplayWindowVM.WindowHeightProperty)); }
            set { this.SetValue(DisplayWindowVM.WindowHeightProperty, (value > 0) ? value : DISPLAYWINDOW_HEIGHT_DEFAULT); }
        }

        private static void WindowHeight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DisplayWindowVM).OnWindowHeightChanged((double)(e.OldValue), (double)(e.NewValue));
        }

        private static object WindowHeight_CoerceValue(DependencyObject d, object baseValue)
        {
            return (d as DisplayWindowVM).CoerceWindowHeight(baseValue as double?);
        }

        protected virtual void OnWindowHeightChanged(double oldHeight, double newHeight) { }

        protected virtual double CoerceWindowHeight(double? baseValue)
        {
            if (baseValue.HasValue && baseValue.Value > 0.0)
                return baseValue.Value;
            return DISPLAYWINDOW_WIDTH_DEFAULT;
        }

        public DisplayWindowVM() : base() { }
    }
}
