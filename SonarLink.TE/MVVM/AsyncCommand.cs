// https://johnthiriet.com/mvvm-going-async-with-async-command/

using Microsoft.VisualStudio.Threading;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SonarLink.TE.MVVM
{
    /// <summary>
    /// ICommand implementation suitable for 'async' operations
    /// </summary>
    /// <remarks>Reference: https://johnthiriet.com/mvvm-going-async-with-async-command/</remarks>
    public class AsyncCommand : ICommand
    {
        /// <summary>
        /// Command execution state (running, not running)
        /// </summary>
        private bool _isExecuting = false;

        /// <summary>
        /// Async execute operation
        /// </summary>
        private readonly Func<object, Task> _execute;

        /// <summary>
        /// Predicate determining if the operation can be executed
        /// </summary>
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Action handling exceptional cases caused during command execution
        /// </summary>
        private readonly Action<AggregateException> _onFault;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Async execute operation</param>
        /// <param name="canExecute">Predicate determining if the operation can be executed</param>
        /// <param name="onFault">Action handling exceptional cases caused during command execution</param>
        public AsyncCommand(
            Func<object, Task> execute,
            Func<object, bool> canExecute = null,
            Action<AggregateException> onFault = null
        )
        {
            _execute = execute;
            _canExecute = canExecute;
            _onFault = onFault;
        }

        /// <summary>
        /// Executes the async operation
        /// </summary>
        /// <param name="parameter">Execution parameter</param>
        /// <returns>Awaitable task representing the operation in progress</returns>
        public async Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter))
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();

                try
                {
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Triggers the <c>CanExecuteChanged</c> event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        /// <inheritdoc />
        /// <see cref="ICommand.CanExecuteChanged"/>
        public event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        /// <see cref="ICommand.CanExecute"/>
        public bool CanExecute(object parameter)
        {
            return !_isExecuting && ((_canExecute == null) || _canExecute(parameter));
        }

        /// <inheritdoc />
        /// <see cref="ICommand.Execute"/>
        public void Execute(object parameter)
        {
            ExecuteAsync(parameter).ContinueWith(task =>
            {
                if (task.IsFaulted && (_onFault != null))
                {
                    _onFault(task.Exception);
                }

            }, TaskScheduler.Default).Forget();
        }

        #endregion
    }
}
