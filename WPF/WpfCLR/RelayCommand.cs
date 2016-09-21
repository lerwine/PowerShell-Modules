using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfCLR
{
    /// <summary>
    /// Command which is used to relay interacive events
    /// </summary>
    /// <typeparam name="TArg">Type of argument to be passed to the command invocation handler</typeparam>
    public class RelayCommand<TArg> : ICommand, INotifyPropertyChanged
    {
        private bool _allowConcurrentInvocations = false;
        private bool _isEnabled = true;
        private int _concurrencyLevel = 0;
        private RelayInvocationHandler<TArg> _invocationhandler = null;

        /// <summary>
        /// Occurs when command object's state changes in a way that affect whether the command should be able to execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Determines whether multiple invocations of this command should be allowed to occur at the same time
        /// </summary>
        public bool AllowConcurrentInvocations
        {
            get { return this._allowConcurrentInvocations; }
            set
            {
                if (value == this._allowConcurrentInvocations)
                    return;

                bool isEnabled = this.IsEnabled;

                this._allowConcurrentInvocations = value;
                this.RaisePropertyChanged("AllowConcurrentInvocations");

                if (isEnabled != this.IsEnabled)
                    this.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Controls whether the command may be invoked
        /// </summary>
        public bool IsEnabled
        {
            get { return this._isEnabled && (this.AllowConcurrentInvocations || this.ConcurrencyLevel == 0); }
            set
            {
                if (this._isEnabled == value)
                    return;

                bool isEnabled = this.IsEnabled;

                this._isEnabled = value;

                if (isEnabled != this.IsEnabled)
                    this.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets the number of conncurent invocations of this command
        /// </summary>
        protected int ConcurrencyLevel
        {
            get { return this._concurrencyLevel; }
            private set
            {
                int i = (value < 0) ? 0 : value;

                if (i == this._concurrencyLevel)
                    return;

                bool isEnabled = this.IsEnabled;

                this._concurrencyLevel = i;
                this.RaisePropertyChanged("IsBeingInvoked");

                if (isEnabled != this.IsEnabled)
                    this.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Determines whether the command is currently being invoked
        /// </summary>
        public bool IsBeingInvoked { get { return this.ConcurrencyLevel > 0; } }

        /// <summary>
        /// Create new relay command.
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        public RelayCommand(RelayInvocationHandler<TArg> invocationhandler)
        {
            this._invocationhandler = invocationhandler;
        }

        /// <summary>
        /// Create new relay command
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        /// <param name="allowConcurrentInvocations">Whether multiple concurrent invocations should be allowed.</param>
        public RelayCommand(RelayInvocationHandler<TArg> invocationhandler, bool allowConcurrentInvocations)
            : this(invocationhandler)
        {
            this.AllowConcurrentInvocations = allowConcurrentInvocations;
        }

        /// <summary>
        /// Create new relay command
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        /// <param name="allowConcurrentInvocations">Whether multiple concurrent invocations should be allowed.</param>
        /// <param name="isDisabled">Whether command is to be intially disabled</param>
        public RelayCommand(RelayInvocationHandler<TArg> invocationhandler, bool allowConcurrentInvocations, bool isDisabled)
            : this(invocationhandler, allowConcurrentInvocations)
        {
            this.IsEnabled = !isDisabled;
        }

        /// <summary>
        /// This gets called when the command object's state changes in a way that affect whether the command should be able to execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            this.RaisePropertyChanged("IsEnabled");

            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This gets called when the value of a property has changed.
        /// </summary>
        /// <param name="args">Arguments to be passed to the PropertyChanged event</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, args);
        }

        /// <summary>
        /// This gets called when it needs to be determined whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Parameter which will be passed to the relay command.</param>
        /// <returns>true if this command can be executed; otherwise false.</returns>
        protected virtual bool OnCanExecute(TArg parameter)
        {
            return this.IsEnabled;
        }

        /// <summary>
        /// This is the method that gets called in order to determine whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Parameter which will be passed to the relay command.</param>
        /// <returns>true if this command can be executed; otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.OnCanExecute((parameter != null && parameter is TArg) ? (TArg)parameter : default(TArg));
        }

        /// <summary>
        /// Occurs when the command is being executed
        /// </summary>
        /// <param name="parameter">Parameter which was passed by the interactive command.</param>
        protected virtual void OnExecute(TArg parameter)
        {
            if (this._invocationhandler != null)
                this._invocationhandler(parameter);
        }

        /// <summary>
        /// This is the method that gets called when the command is to be invoked
        /// </summary>
        /// <param name="parameter">Parameter being passed by the interactive command.</param>
        public void Execute(object parameter)
        {
            // This prevents any overflow, however unlikely
            while (this.ConcurrencyLevel == (Int32.MaxValue >> 1))
                System.Threading.Thread.Sleep(100);

            this.ConcurrencyLevel++;

            try
            {
                this.OnExecute((parameter != null && parameter is TArg) ? (TArg)parameter : default(TArg));
            }
            catch
            {
                throw;
            }
            finally
            {
                this.ConcurrencyLevel--;
            }
        }
    }

    public delegate void RelayInvocationHandler<TArg>(TArg arg);

    public class RelayCommand : RelayCommand<object>
    {
        /// <summary>
        /// Create new relay command.
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        public RelayCommand(RelayInvocationHandler<object> invocationhandler) : base(invocationhandler) { }

        /// <summary>
        /// Create new relay command
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        /// <param name="allowConcurrentInvocations">Whether multiple concurrent invocations should be allowed.</param>
        public RelayCommand(RelayInvocationHandler<object> invocationhandler, bool allowConcurrentInvocations) : base(invocationhandler, allowConcurrentInvocations) { }

        /// <summary>
        /// Create new relay command
        /// </summary>
        /// <param name="invocationhandler">Command invocation handler.</param>
        /// <param name="allowConcurrentInvocations">Whether multiple concurrent invocations should be allowed.</param>
        /// <param name="isEnabled">Whether command is to be intially enabled</param>
        public RelayCommand(RelayInvocationHandler<object> invocationhandler, bool allowConcurrentInvocations, bool isDisabled)
            : base(invocationhandler, allowConcurrentInvocations, isDisabled) { }
    }
}