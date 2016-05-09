using System;
using System.Threading.Tasks;
using SimpleCQRS.Framework.Contracts;

namespace SimpleCQRS.Infrastructure
{
    public abstract class Command : System.Windows.Input.ICommand, IMessage, IUnique
    {
        public Guid Id { get; private set; }
        public Guid AggregateId { get; set; }
        public int OriginalVersion { get; set; }

        protected Command()
        {
            Id = Guid.NewGuid();
        }

        public abstract bool CanExecute(object parameter);

        async void System.Windows.Input.ICommand.Execute(object parameter)
        {
            await Execute(parameter);
        }

        public abstract Task Execute(object parameter);

        public abstract event EventHandler CanExecuteChanged;
    }
}
