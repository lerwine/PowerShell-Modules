using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfControlLibrary1
{
    public class PasswordManagerVM : DependencyObject
    {
        #region TabIndex Property Members

        public const string DependencyPropertyName_TabIndex = "TabIndex";

        /// <summary>
        /// Identifies the <seealso cref="TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty = DependencyProperty.Register(DependencyPropertyName_TabIndex, typeof(int), typeof(PasswordManagerVM),
                new PropertyMetadata(0,
                (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
                {
                    if (d.CheckAccess())
                        (d as PasswordManagerVM).TabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue));
                    else
                        d.Dispatcher.Invoke(() => (d as PasswordManagerVM).TabIndex_PropertyChanged((int)(e.OldValue), (int)(e.NewValue)));
                },
                (DependencyObject d, object baseValue) => (d as PasswordManagerVM).TabIndex_CoerceValue(baseValue)));

        /// <summary>
        /// Selected tab index
        /// </summary>
        public int TabIndex
        {
            get { return (int)(GetValue(TabIndexProperty)); }
            set { SetValue(TabIndexProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <seealso cref="TabIndex"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="int"/> value before the <seealso cref="TabIndex"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="int"/> value after the <seealso cref="TabIndex"/> property was changed.</param>
        protected virtual void TabIndex_PropertyChanged(int oldValue, int newValue)
        {
            // TODO: Implement PasswordManagerVM.TabIndex_PropertyChanged(int, int)
        }

        /// <summary>
        /// This gets called whenever <seealso cref="TabIndex"/> is being re-evaluated, or coercion is specifically requested.
        /// </summary>
        /// <param name="baseValue">The new value of the property, prior to any coercion attempt.</param>
        /// <returns>The coerced value.</returns>
        public virtual int TabIndex_CoerceValue(object baseValue)
        {
            // TODO: Implement PasswordManagerVM.TabIndex_CoerceValue(DependencyObject, object)
            return (int)baseValue;
        }

        #endregion

        #region ErrorBorderVisibility Property Members

        public const string PropertyName_ErrorBorderVisibility = "ErrorBorderVisibility";

        private static readonly DependencyPropertyKey ErrorBorderVisibilityPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ErrorBorderVisibility, typeof(Visibility), typeof(PasswordManagerVM),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <seealso cref="ErrorBorderVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorBorderVisibilityProperty = ErrorBorderVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public Visibility ErrorBorderVisibility
        {
            get
            {
                if (CheckAccess())
                    return (Visibility)(GetValue(ErrorBorderVisibilityProperty));
                return Dispatcher.Invoke(() => ErrorBorderVisibility);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ErrorBorderVisibilityPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ErrorBorderVisibility = value);
            }
        }

        #endregion

        #region ErrorMessageText Property Members

        public const string PropertyName_ErrorMessageText = "ErrorMessageText";

        private static readonly DependencyPropertyKey ErrorMessageTextPropertyKey = DependencyProperty.RegisterReadOnly(PropertyName_ErrorMessageText, typeof(string), typeof(PasswordManagerVM),
                new PropertyMetadata(""));

        /// <summary>
        /// Identifies the <seealso cref="ErrorMessageText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorMessageTextProperty = ErrorMessageTextPropertyKey.DependencyProperty;

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessageText
        {
            get
            {
                if (CheckAccess())
                    return (string)(GetValue(ErrorMessageTextProperty));
                return Dispatcher.Invoke(() => ErrorMessageText);
            }
            private set
            {
                if (CheckAccess())
                    SetValue(ErrorMessageTextPropertyKey, value);
                else
                    Dispatcher.Invoke(() => ErrorMessageText = value);
            }
        }

        #endregion

    }
}
