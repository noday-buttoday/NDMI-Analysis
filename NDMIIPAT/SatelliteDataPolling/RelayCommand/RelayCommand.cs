using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SatelliteDataPolling
{
    class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <exception cref="ArgumentNullException"><paramref name="execute" /> is <c>null</c>.</exception> 
        public RelayCommand(Action executeAction, Func<bool> canExecuteFunc)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = executeAction;
            this.canExecute = canExecuteFunc;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
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
    }
}
