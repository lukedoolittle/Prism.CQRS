using System;
using Prism.Events;
using SimpleCQRS.Framework.Contracts;

namespace Prism.CQRS.Test.SimpleCQRS.Eventing
{
    public abstract class Request { }
    public class TwitterTweet : Request { }

    public interface ISampleAddedEventPayload<out TValue> where TValue : Request { }
    public class SampleAddedEventPayload<T> : ISampleAddedEventPayload<T> where T : Request { }
    public class SampleAddedEvent<TValue> : PubSubEvent<ISampleAddedEventPayload<TValue>> where TValue : Request { }

    public class SampleAddedEventHandler<TRequest> : IEventHandler<ISampleAddedEventPayload<TRequest>>
        where TRequest : Request
    {
        private readonly Action<ISampleAddedEventPayload<TRequest>> _action;

        public SampleAddedEventHandler(Action<ISampleAddedEventPayload<TRequest>> action)
        {
            _action = action;
        }

        public void Handle(ISampleAddedEventPayload<TRequest> @event)
        {
            var type = typeof (TRequest);
            _action(@event);
        }
    }

    public class DummyEventHandler<TRequest> : IEventHandler<ISampleAddedEventPayload<TRequest>>
        where TRequest : Request
    {
        public void Handle(ISampleAddedEventPayload<TRequest> @event)
        {
        }
    }

    public class MyEventPayload
    {
        public string MyPayloadData { get; set; }
    }
    public class MyEvent : PubSubEvent<MyEventPayload> { }
    public class MySimpleEvent : PubSubEvent { }
}
