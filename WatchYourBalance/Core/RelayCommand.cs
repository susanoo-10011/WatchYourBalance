using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WatchYourBalance.Core
{
    internal class RelayCommand : ICommand // позволяет связать элементы пользовательского интерфейса (кнопки, меню) с методами в коде приложения.
    {
        private readonly Action<object> _execute; // представляет метод, который будет выполнен при вызове команды
        private readonly Func<object, bool> _canExecute; //представляет метод, который определяет, можно ли выполнить команду в текущем состоянии приложения.

        public event EventHandler CanExecuteChanged // какие то команды ретрансляции
        {
            add { CommandManager.RequerySuggested += value; } //
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        //передаются методы, которые будут выполняться при вызове команды и определении ее доступности.
        //Если метод определения доступности не передан, он принимает значение null, что означает, что команда доступна всегда.
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) //Указывает, может ли команда быть выполнена в данный момент.
                                                 //Если значение true, команда доступна для выполнения, если false, то нет
        {
            return _canExecute == null || _canExecute(parameter);
         }

        public void Execute(object parameter) // Выполняет команду, то есть вызывает метод, который был передан в конструктор класса RelayCommand.
        {
            _execute(parameter);
        }
    }
}
