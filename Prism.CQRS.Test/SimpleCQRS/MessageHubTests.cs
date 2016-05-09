using System;
using System.Collections.Concurrent;
using System.Linq;
using Prism.CQRS.Test.SimpleCQRS.Eventing;
using SimpleCQRS.Infrastructure;
using Xunit;

namespace Prism.CQRS.Test.SimpleCQRS
{
    public class MessageHubTests
    {
        [Fact]
        public async void MessageBusSimpleEventHandling()
        {
            var events = new ConcurrentBag<object>();
            var bus = new MessageHub();

            var token = bus.Subscribe<MySimpleEvent>(() => events.Add(new object()));
            await bus.Publish<MySimpleEvent>();

            Assert.Equal(1, events.Count);

            bus.UnSubscribe<MySimpleEvent>(token);
        }

        [Fact]
        public async void MessageBusEventHandling()
        {
            var events = new ConcurrentBag<MyEventPayload>();
            var payload = new MyEventPayload();
            var bus = new MessageHub();

            var token = bus.Subscribe<MyEvent, MyEventPayload>((o) => events.Add(o));
            await bus.Publish<MyEvent, MyEventPayload>(payload);

            Assert.Equal(1, events.Count);
            Assert.Equal(payload, events.First());

            bus.UnSubscribe<MySimpleEvent>(token);
        }

        [Fact]
        public async void MessageBusSimpleCommandHandling()
        {
            var events = new ConcurrentBag<object>();
            var bus = new MessageHub();

            var command = new MySimpleCommand(() => events.Add(new object()));
            bus.Register(command);
            await bus.Send<MySimpleCommand>();

            Assert.Equal(1, events.Count);

            bus.UnRegister<MySimpleCommand>();
        }

        [Fact]
        public async void MessageBusMessageCommandHandling()
        {
            var events = new ConcurrentBag<MyCommandPayload>();
            var payload = new MyCommandPayload();
            var bus = new MessageHub();

            var command = new MyCommand((o) => events.Add(o));
            bus.Register(command);
            await bus.Send<MyCommand, MyCommandPayload>(payload);

            Assert.Equal(1, events.Count);
            Assert.Equal(payload, events.First());

            bus.UnRegister<MyCommand>();
        }

        [Fact]
        public async void SendCommandWithNoHandler()
        {
            var events = new ConcurrentBag<MyCommandPayload>();
            var payload = new MyCommandPayload();
            var bus = new MessageHub();

            await bus.Send<MyCommand, MyCommandPayload>(payload);

            Assert.Equal(0, events.Count);

            bus.UnRegister<MyCommand>();
        }

        [Fact]
        public async void MessageBusGenericEventHandling()
        {
            var events = new ConcurrentBag<object>();
            var payload = new SampleAddedEventPayload<TwitterTweet>();
            var bus = new MessageHub();
            var eventHandler = new SampleAddedEventHandler<TwitterTweet>((o) => events.Add(o));

            var token = bus.Subscribe<SampleAddedEvent<Request>, ISampleAddedEventPayload<Request>>(
                o => eventHandler.Handle((ISampleAddedEventPayload<TwitterTweet>)o));
            await bus.Publish<SampleAddedEvent<Request>, ISampleAddedEventPayload<Request>>(payload);

            Assert.Equal(1, events.Count);
            Assert.Equal(payload, events.First());

            bus.UnSubscribe<SampleAddedEvent<Request>>(token);
        }

        public void MessageHubGenericCommandHandling()
        {
            throw new NotImplementedException();
        }
    }
}
