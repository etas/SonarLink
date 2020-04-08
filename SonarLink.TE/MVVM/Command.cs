// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.Windows.Input;

namespace SonarLink.TE.MVVM
{
    /// <summary>
    /// Command to relay its functionality to other
    /// objects by invoking delegates.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>
        /// The execution logic.
        /// </summary>
        private Action<object> _execute;

        /// <summary>
        /// The execution status logic.
        /// </summary>
        private Predicate<object> _canExecute;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public Command(Action<object> execute) : this(execute, null) { }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #region ICommand

        /// <inheritdoc />
        /// <see cref="ICommand.CanExecuteChanged"/>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <inheritdoc />
        /// <see cref="ICommand.CanExecute"/>
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) ? true : _canExecute(parameter);
        }

        /// <inheritdoc />
        /// <see cref="ICommand.Execute"/>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
