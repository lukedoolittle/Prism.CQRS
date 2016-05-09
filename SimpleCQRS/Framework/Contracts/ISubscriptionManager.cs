using System;
using Prism.Events;
using SimpleCQRS.Infrastructure;

namespace SimpleCQRS.Framework.Contracts
{
    public interface ISubscriptionManager
    {
        void Register<TCommand>(TCommand command)
            where TCommand : Command;

        SubscriptionToken Subscribe<TEvent>(
            Action action,
            ThreadOption threadOptions = ThreadOption.PublisherThread)
            where TEvent : PubSubEvent, new();

        SubscriptionToken Subscribe<TEvent, TPayload>(
            Action<TPayload> action,
            ThreadOption threadOptions = ThreadOption.PublisherThread)
            where TEvent : PubSubEvent<TPayload>, new();

        void UnSubscribe<TEvent>(SubscriptionToken subscription)
            where TEvent : EventBase, new();

        void UnRegister<TCommand>();
    }
}
