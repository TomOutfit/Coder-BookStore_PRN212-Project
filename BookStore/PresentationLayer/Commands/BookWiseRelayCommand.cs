using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PresentationLayer.Commands
{
    /// <summary>
    /// Implements ICommand for parameterless and parameterized UI commands.
    /// Supports action delegation and execution conditions with thread-safe updates.
    /// </summary>
    public class BookWiseRelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public BookWiseRelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (Application.Current?.Dispatcher is Dispatcher dispatcher)
            {
                dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested(), DispatcherPriority.Normal);
            }
        }
    }
}