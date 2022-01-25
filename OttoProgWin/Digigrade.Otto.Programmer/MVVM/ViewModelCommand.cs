using System;
using System.Windows.Input;

namespace Digigrade.Otto.Programmer.MVVM
{
    /// <summary>
    /// Tie in for Wpf Command binding
    /// </summary>
    public class ViewModelCommand : ICommand
    {
        private readonly Action? _action;
        private readonly Predicate<object>? _canExecute;
        private readonly Func<bool>? _condition;
        private readonly Action<object>? _execute;

        /// <summary>
        /// Instantiate a new bindable command.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public ViewModelCommand(Action? action, Func<bool>? condition)
        {
            _action = action;
            _condition = condition;
        }

        /// <summary>
        /// Instantiate a new bindable command.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public ViewModelCommand(Action<object>? execute, Predicate<object>? canExecute)
        {
            if (execute == null) { throw new ArgumentNullException("execute"); }
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Returns true if the command is able to execute.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            if (_condition != null) { return _condition(); }
            else if (_canExecute != null && parameter != null) { return _canExecute(parameter); }
            return true;
        }

        /// <summary>
        /// Runs execution if possible.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter)
        {
            if (_execute != null && parameter != null)
            {
                _execute(parameter);
            }
            else if (_action != null)
            {
                _action();
            }
        }
    }
}