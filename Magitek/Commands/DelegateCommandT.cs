using System;
using System.Windows.Input;

namespace Magitek.Commands
{
    /// <summary>
    ///     A command that calls the specified delegate when the command is executed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
    {
        private readonly Func<T, bool> _canExecuteMethod;
        private readonly Action<T> _executeMethod;
        private bool _isExecuting;

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod = null)
        {
            if ((executeMethod == null) && (canExecuteMethod == null))
            {
                throw new ArgumentNullException(nameof(executeMethod), @"Execute Method cannot be null");
            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return !_isExecuting && CanExecute((T) parameter);
        }

        void ICommand.Execute(object parameter)
        {
            _isExecuting = true;
            try
            {
                RaiseCanExecuteChanged();
                Execute((T) parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(T parameter)
        {
            return _canExecuteMethod == null || _canExecuteMethod(parameter);
        }

        private void Execute(T parameter)
        {
            _executeMethod(parameter);
        }
    }
}