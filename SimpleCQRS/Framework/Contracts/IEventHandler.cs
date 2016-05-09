namespace SimpleCQRS.Framework.Contracts
{
    public interface IEventHandler<in TEvent>
    {
        void Handle(TEvent @event);
    }
}
