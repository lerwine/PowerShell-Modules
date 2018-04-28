using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LteDev.WPF
{
    public class ImpliedAndExplicitVM<T> : DependencyObject
    {
        public event CoerceValueCallback OnCoerceValue;

        #region ImpliedValue Property Members

        /// <summary>
        /// Defines the name for the <see cref="ImpliedValue"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ImpliedValue = "ImpliedValue";

        /// <summary>
        /// Identifies the <see cref="ImpliedValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImpliedValueProperty = DependencyProperty.Register(DependencyPropertyName_ImpliedValue, typeof(T), typeof(ImpliedAndExplicitVM<T>),
                new PropertyMetadata(default(T), (d, e) => (d as ImpliedAndExplicitVM<T>).ImpliedValue_PropertyChanged((T)(e.OldValue), (T)(e.NewValue)),
                _OnCoerceValue));

        /// <summary>
        /// Indicates that the property is attached.
        /// </summary>
        public T ImpliedValue
        {
            get { return (T)(GetValue(ImpliedValueProperty)); }
            set { SetValue(ImpliedValueProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="ImpliedValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="T"/> value before the <seealso cref="ImpliedValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="T"/> value after the <seealso cref="ImpliedValue"/> property was changed.</param>
        protected virtual void ImpliedValue_PropertyChanged(T oldValue, T newValue)
        {
            if (UseImplied)
                ImpliedValue = newValue;
        }

        #endregion
        
        #region ExplicitValue Property Members

        /// <summary>
        /// Defines the name for the <see cref="ExplicitValue"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ExplicitValue = "ExplicitValue";

        /// <summary>
        /// Identifies the <see cref="ExplicitValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplicitValueProperty = DependencyProperty.Register(DependencyPropertyName_ExplicitValue, typeof(T), typeof(ImpliedAndExplicitVM<T>),
                new PropertyMetadata(default(T), (d, e) => (d as ImpliedAndExplicitVM<T>).ExplicitValue_PropertyChanged((T)(e.OldValue), (T)(e.NewValue)),
                _OnCoerceValue));

        /// <summary>
        /// Property is read-only
        /// </summary>
        public T ExplicitValue
        {
            get { return (T)(GetValue(ExplicitValueProperty)); }
            set { SetValue(ExplicitValueProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="ExplicitValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="T"/> value before the <seealso cref="ExplicitValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="T"/> value after the <seealso cref="ExplicitValue"/> property was changed.</param>
        protected virtual void ExplicitValue_PropertyChanged(T oldValue, T newValue)
        {
            if (UseExplicit)
                ActualValue = newValue;
        }

        #endregion

        #region ActualValue Property Members

        public event EventHandler ActualValueChanged;

        /// <summary>
        /// Defines the name for the <see cref="ActualValue"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_ActualValue = "ActualValue";

        /// <summary>
        /// Identifies the <see cref="ActualValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register(DependencyPropertyName_ActualValue, typeof(T), typeof(ImpliedAndExplicitVM<T>),
                new PropertyMetadata(default(T), (d, e) => (d as ImpliedAndExplicitVM<T>).ActualValue_PropertyChanged((T)(e.OldValue), (T)(e.NewValue)),
                _OnCoerceValue));

        /// <summary>
        /// Property is read-only
        /// </summary>
        public T ActualValue
        {
            get { return (T)(GetValue(ActualValueProperty)); }
            set { SetValue(ActualValueProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="ActualValue"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="T"/> value before the <seealso cref="ActualValue"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="T"/> value after the <seealso cref="ActualValue"/> property was changed.</param>
        protected virtual void ActualValue_PropertyChanged(T oldValue, T newValue)
        {
            if (UseExplicit)
                ExplicitValue = newValue;
            else
                ImpliedValue = newValue;
            EventHandler actualValueChanged = ActualValueChanged;
            if (actualValueChanged != null)
                actualValueChanged(this, EventArgs.Empty);
        }

        #endregion

        #region UseExplicit Property Members

        /// <summary>
        /// Defines the name for the <see cref="UseExplicit"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_UseExplicit = "UseExplicit";

        /// <summary>
        /// Identifies the <see cref="UseExplicit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseExplicitProperty = DependencyProperty.Register(DependencyPropertyName_UseExplicit, typeof(bool), typeof(ImpliedAndExplicitVM<T>),
                new PropertyMetadata(true, (d, e) => (d as ImpliedAndExplicitVM<T>).UseExplicit_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Property is read-only
        /// </summary>
        public bool UseExplicit
        {
            get { return (bool)(GetValue(UseExplicitProperty)); }
            set { SetValue(UseExplicitProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="UseExplicit"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="UseExplicit"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="UseExplicit"/> property was changed.</param>
        protected virtual void UseExplicit_PropertyChanged(bool oldValue, bool newValue)
        {
            UseImplied = oldValue;
        }

        #endregion

        #region UseImplied Property Members

        /// <summary>
        /// Defines the name for the <see cref="UseImplied"/> dependency property.
        /// </summary>
        public const string DependencyPropertyName_UseImplied = "UseImplied";

        /// <summary>
        /// Identifies the <see cref="UseImplied"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseImpliedProperty = DependencyProperty.Register(DependencyPropertyName_UseImplied, typeof(bool), typeof(ImpliedAndExplicitVM<T>),
                new PropertyMetadata(false, (d, e) => (d as ImpliedAndExplicitVM<T>).UseImplied_PropertyChanged((bool)(e.OldValue), (bool)(e.NewValue))));

        /// <summary>
        /// Indicates that the property is attached.
        /// </summary>
        public bool UseImplied
        {
            get { return (bool)(GetValue(UseImpliedProperty)); }
            set { SetValue(UseImpliedProperty, value); }
        }

        /// <summary>
        /// This gets called after the value associated with the <see cref="UseImplied"/> dependency property has changed.
        /// </summary>
        /// <param name="oldValue">The <seealso cref="bool"/> value before the <seealso cref="UseImplied"/> property was changed.</param>
        /// <param name="newValue">The <seealso cref="bool"/> value after the <seealso cref="UseImplied"/> property was changed.</param>
        protected virtual void UseImplied_PropertyChanged(bool oldValue, bool newValue)
        {
            UseExplicit = oldValue;
            ActualValue = (newValue) ? ImpliedValue : ExplicitValue;
        }

        #endregion
        
        private static object _OnCoerceValue(DependencyObject d, object baseObject)
        {
            CoerceValueCallback coerceValue = ((ImpliedAndExplicitVM<T>)d).OnCoerceValue;
            if (coerceValue != null)
                return coerceValue(d, baseObject);
            return baseObject;
        }

        public ImpliedAndExplicitVM(T value)
        {
            ActualValue = value;
            ImpliedValue = value;
        }

        public ImpliedAndExplicitVM() { }
    }
}