using System.Collections.Generic;
using Prism.Events;
using SimpleCQRS.Framework.Contracts;

namespace Prism.CQRS.Bootstrapper.Autofac.Test
{
    public abstract class Request { }
    public class RequestOne : Request { }
    public class RequestTwo : Request { }

    public class SomeEventPayload<T> where T : Request { }
    public class AnotherEventPayload<T> where T : Request { }
    public class AThirdEventPayload { }

    public class SomeEvent<TRequest> : PubSubEvent<SomeEventPayload<TRequest>> where TRequest : Request { }
    public class AThirdEvent : PubSubEvent<AThirdEventPayload> { }

    public class SomeEventHandler<TRequest> : IEventHandler<SomeEventPayload<TRequest>>
        where TRequest : Request
    {
        public static List<object> HandledEvents = new List<object>();
         
        public void Handle(SomeEventPayload<TRequest> @event)
        {
            HandledEvents.Add(@event);
        }
    }

    public class AnotherEventHandler<TRequest> : IEventHandler<SomeEventPayload<TRequest>>
    where TRequest : Request
    {
        public static List<object> HandledEvents = new List<object>();

        public void Handle(SomeEventPayload<TRequest> @event)
        {
            HandledEvents.Add(@event);
        }
    }

    public class AThirdEventHandler : IEventHandler<AThirdEventPayload>
    {
        public static List<object> HandledEvents = new List<object>();

        public void Handle(AThirdEventPayload @event)
        {
            HandledEvents.Add(@event);
        }
    }
}
