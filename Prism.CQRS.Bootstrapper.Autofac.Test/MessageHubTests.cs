using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCQRS.Domain;
using SimpleCQRS.Infrastructure;
using Xunit;

namespace Prism.CQRS.Bootstrapper.Autofac.Test
{
    [Fact]
    public async void SubscribeToGenericEventAllowsEventToBeHandled()
    {
        object actual = null;
        var subscriptionManager = new MessageHub();
        var handler = new EventHandlerMock<GenericDerived1>((o) => { actual = o; });
        var openGenericType = typeof(EventMock<>);
        var genericTypeParameters = typeof(GenericDerived1);
        var @event = new EventMock<GenericDerived1>();

        subscriptionManager.Subscribe(handler, openGenericType, genericTypeParameters);

        await subscriptionManager.Publish(@event);

        Assert.Equal(@event, actual);
    }

    [Fact]
    public async void PassSubscriptionWithNonGenericParameter()
    {
        var MessageHub = new MessageHub();

        var expected = new Event();
        Event actual = null;

        MessageHub.Subscribe<Event>((e) => actual = e);

        await MessageHub.Publish(expected);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PassOpenSubscriptionInvalidMessageTypeExpectException()
    {
        var MessageHub = new MessageHub();

        var messageType = this.GetType();
        var handlerType = typeof(EventHandlerMock<>);

        Assert.Throws<TypeMismatchException>(() =>
            MessageHub.OpenSubscribe(messageType, handlerType));
    }

    [Fact]
    public void PassOpenSubscriptionInvalidEventHandlerTypeExpectException()
    {
        var MessageHub = new MessageHub();

        var messageType = typeof(EventMock<>);
        var handlerType = this.GetType();

        Assert.Throws<TypeMismatchException>(() =>
            MessageHub.OpenSubscribe(messageType, handlerType));
    }

    [Fact]
    public async Task PassOpenSubscriptionValidEventHandlerAndMessage()
    {
        object actual = null;
        var action = new Action<object>(o => actual = o);
        var handlerFactoryMock = new HandlerFactoryMock(action);
        var MessageHub = new MessageHub(handlerFactoryMock);

        var messageType = typeof(EventMock<>);
        var handlerType = typeof(EventHandlerMock<>);
        var expected = new EventMock<GenericDerived1>();

        MessageHub.OpenSubscribe(messageType, handlerType);

        await MessageHub.Publish(expected);

        var actualActual = (EventMock<GenericDerived1>)actual;

        Assert.Equal(expected, actualActual);
    }

    [Fact]
    public async Task PassOpenSubscriptionValidNonGenericEventHandlerAndMessage()
    {
        object actual = null;
        var action = new Action<object>(o => actual = o);
        var handlerFactoryMock = new HandlerFactoryMock(action);
        var MessageHub = new MessageHub(handlerFactoryMock);

        var messageType = typeof(NonGenericEvent);
        var handlerType = typeof(NonGenericEventHandlerMock);
        var expected = new NonGenericEvent();

        MessageHub.OpenSubscribe(messageType, handlerType);

        await MessageHub.Publish(expected);

        Assert.Equal(expected, actual);
    }
}
