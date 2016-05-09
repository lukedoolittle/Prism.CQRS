using System.Threading.Tasks;
using Prism.Commands;

namespace SimpleCQRS.Framework.Contracts
{
    public interface ICommandSender
    {
        Task Send<TCommand>()
            where TCommand : SimpleCommand;

        Task Send<TCommand, TPayload>(TPayload payload)
            where TCommand : SimpleCommand<TPayload>;
    }
}