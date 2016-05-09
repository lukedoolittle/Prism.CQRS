using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SimpleCQRS.Framework.Contracts;
using SimpleCQRS.Infrastructure;

namespace Prism.CQRS.Bootstrapper.Autofac
{
    public class DynamicGenerationMessageHub : MessageHub
    {
        private readonly IContainer _container;
        private readonly List<Type> _genericSubscriptionTypes;
         
        public DynamicGenerationMessageHub(IContainer container)
        {
            _container = container;
            _genericSubscriptionTypes = new List<Type>();
        }

        public override Task Send<TCommand, TPayload>(TPayload payload)
        {
            return base.Send<TCommand, TPayload>(payload);
        }

        public override Task Send<TCommand>()
        {
            return base.Send<TCommand>();
        }

        public override Task Publish<TEvent, TPayload>(TPayload payload)
        {
            //TODO: should we return subscription tokens here for new subscriptions??
            var newHandlerTypes = _container
                .ResolutionTypes<IEventHandler<TPayload>>()
                .Where(t => !_genericSubscriptionTypes.Contains(t))
                .ToList();

            var newHandlers = newHandlerTypes
                .Select(t => (IEventHandler<TPayload>)_container.Resolve(t));

            foreach (var handler in newHandlers)
            {
                this.Subscribe<TEvent, TPayload>(handler.Handle);
            }

            _genericSubscriptionTypes.AddRange(newHandlerTypes);

            return base.Publish<TEvent, TPayload>(payload);
        }
    }
}
