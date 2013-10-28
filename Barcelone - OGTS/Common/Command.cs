using System.Windows.Input;
using System;

namespace Barcelone___OGTS.Common
{
    /// <summary>
    /// The Command class - an ICommand that can fire a function.
    /// </summary>
    public class Command : ICommand
    {
        #region Attributes
        protected Action action = null;
        protected Action<object> parameterizedAction = null;
        protected readonly Predicate<object> _canExecute;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Occurs when can execute is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event CancelCommandEventHandler Executing;

        /// <summary>
        /// Occurs when the command executed.
        /// </summary>
        public event CommandEventHandler Executed;
        #endregion

        #region ICommand Members
        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        void ICommand.Execute(object parameter)
        {
            this.DoExecute(parameter);

        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public Command(Action action, Predicate<object> canExecute)
        {
            this.action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">The parameterized action.</param>
        /// <param name="canExecute">if set to <c>true</c> [can execute].</param>
        public Command(Action<object> parameterizedAction, Predicate<object> canExecute)
        {
            //  Set the action.
            this.parameterizedAction = parameterizedAction;
            this._canExecute = canExecute;
        }
        #endregion

        #region Execution Methods
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(object param)
        {
            //  Invoke the executing command, allowing the command to be cancelled.
            CancelCommandEventArgs args = new CancelCommandEventArgs() { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  If the event has been cancelled, bail now.
            if (args.Cancel)
                return;

            //  Call the action or the parameterized action, whichever has been set.
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs() { Parameter = param });
        }

        protected void InvokeAction(object param)
        {
            Action theAction = action;
            Action<object> theParameterizedAction = parameterizedAction;
            if (theAction != null)
                theAction();
            else if (theParameterizedAction != null)
                theParameterizedAction(param);
        }

        protected void InvokeExecuted(CommandEventArgs args)
        {
            CommandEventHandler executed = Executed;

            //  Call the executed event.
            if (executed != null)
                executed(this, args);
        }

        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            CancelCommandEventHandler executing = Executing;

            //  Call the executed event.
            if (executing != null)
                executing(this, args);
        }

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        #endregion
    }

    #region Delegates
    /// <summary>
    /// The CommandEventHandler delegate.
    /// </summary>
    public delegate void CommandEventHandler(object sender, CommandEventArgs args);

    /// <summary>
    /// The CancelCommandEvent delegate.
    /// </summary>
    public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);
    #endregion

    #region Classes
    /// <summary>
    /// CommandEventArgs - simply holds the command parameter.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        public object Parameter { get; set; }
    }

    /// <summary>
    /// CancelCommandEventArgs - just like above but allows the event to 
    /// be cancelled.
    /// </summary>
    public class CancelCommandEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
    #endregion
}