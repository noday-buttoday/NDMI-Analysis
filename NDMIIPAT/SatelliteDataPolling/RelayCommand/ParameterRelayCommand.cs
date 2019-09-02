using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SatelliteDataPolling
{
    public class ParameterRelayCommand: ICommand
    {
        public ParameterRelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public ParameterRelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
        
        /// <summary>
        ///     Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        ///     Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }



        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;
    }
}
