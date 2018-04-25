using System;
using System.Windows.Input;

namespace WpfExportApp
{
    public class RelayCommand : ICommand
    {
        private Action<object> m_execute;
        private Func<object, bool> m_canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> _execute, Func<object, bool> _canExecute = null)
        {
            m_execute = _execute;
            m_canExecute = _canExecute;
        }

        public bool CanExecute(object _parameter)
        {
            return m_canExecute == null || m_canExecute(_parameter);
        }

        public void Execute(object _parameter)
        {
            m_execute(_parameter);
        }
    }
}
