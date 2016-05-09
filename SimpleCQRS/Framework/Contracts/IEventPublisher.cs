using System.Threading.Tasks;
using Prism.Events;

namespace SimpleCQRS.Framework.Contracts
{
    public interface IEventPublisher
    {
        Task Publish<TEvent>()
            where TEvent : PubSubEvent, new();

        Task Publish<TEvent, TPayload>(TPayload payload)
            where TEvent : PubSubEvent<TPayload>, new();
    }
}