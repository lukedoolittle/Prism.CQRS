using System;
using System.Threading.Tasks;
using SimpleCQRS.Infrastructure;

namespace Prism.Commands
{
    public abstract class SimpleCommandBase : Command
    {
        private readonly DelegateCommandBase _command;

        protected SimpleCommandBase(DelegateCommandBase command)
        {
            _command = command;
            _command.CanExecuteChanged += OnCanExecuteChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return _command.CanExecute(parameter);
        }

        public override Task Execute(object parameter)
        {
            return _command.Execute(parameter);
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }

        public override event EventHandler CanExecuteChanged;

    }
    public class SimpleCommand : SimpleCommandBase
    {
        public SimpleCommand(Action action) : 
            base(new DelegateCommand(action))
        {
        }
    }

    public class SimpleCommand<T> : SimpleCommandBase
    {
        public SimpleCommand(Action<T> action) : 
            base(new DelegateCommand<T>(action))
        {
        }
    }
}
