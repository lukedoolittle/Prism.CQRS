using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using SimpleCQRS.Domain;
using SimpleCQRS.Framework.Contracts;

namespace SimpleCQRS.Framework
{
    public class EventAction<TEvent, TPayload> : EventAction
        where TEvent : PubSubEvent<TPayload>, new()
    {
        public EventAction(Event @event)
        {
            Event = @event;
            PublishAction = ((publisher, o) => publisher.Publish<TEvent, TPayload>((TPayload)o));
        }
    }

    public class EventAction
    {
        public Action<IEventPublisher, object> PublishAction { get; set; }
        public Event Event { get; set; }
    }
}
