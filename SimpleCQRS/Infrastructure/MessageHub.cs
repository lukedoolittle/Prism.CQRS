using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using SimpleCQRS.Framework.Contracts;

namespace SimpleCQRS.Infrastructure
{
    public class MessageHub : 
        ISubscriptionManager, 
        IEventPublisher, 
        ICommandSender
    {
        private readonly IDictionary<Type, Command> _commands;
        private readonly IEventAggregator _eventAggregator;

        public MessageHub()
        {
            _commands = new Dictionary<Type, Command>();
            _eventAggregator = new EventAggregator();
        }

        public virtual void Register<TCommand>(TCommand command)
            where TCommand : Command
        {
            _commands.Add(typeof(TCommand), command);
        }

        public virtual SubscriptionToken Subscribe<TEvent>(
            Action action,
            ThreadOption threadOptions = ThreadOption.PublisherThread)
            where TEvent : PubSubEvent, new()
        {
            return _eventAggregator
                .GetEvent<TEvent>()
                .Subscribe(
                    action, 
                    threadOptions);
        }
   
        public virtual SubscriptionToken Subscribe<TEvent, TPayload>(
            Action<TPayload> action, 
            ThreadOption threadOptions = ThreadOption.PublisherThread)
            where TEvent : PubSubEvent<TPayload>, new()
        {
            return _eventAggregator
                .GetEvent<TEvent>()
                .Subscribe(
                    action, 
                    threadOptions);
        }

        public virtual Task Send<TCommand>()
            where TCommand : SimpleCommand
        {
            Command command;
            return _commands.TryGetValue(typeof (TCommand), out command) ? 
                _commands[typeof (TCommand)].Execute(null) : 
                Task.FromResult(false);
        }

        public virtual Task Send<TCommand, TPayload>(TPayload payload)
            where TCommand : SimpleCommand<TPayload>
        {
            Command command;
            return _commands.TryGetValue(typeof(TCommand), out command) ?
                _commands[typeof(TCommand)].Execute(payload) :
                Task.FromResult(false);
        }

        public virtual Task Publish<TEvent>()
            where TEvent : PubSubEvent, new()
        {
            return Task.Run(
                () => _eventAggregator
                        .GetEvent<TEvent>()
                        .Publish());
        }

        public virtual Task Publish<TEvent, TPayload>(TPayload payload)
            where TEvent : PubSubEvent<TPayload>, new()
        {
            return Task.Run(
                () => _eventAggregator
                        .GetEvent<TEvent>()
                        .Publish(payload));
        }

        public virtual void UnSubscribe<TEvent>(SubscriptionToken subscription)
            where TEvent : EventBase, new()
        {
            _eventAggregator
                .GetEvent<TEvent>()
                .Unsubscribe(subscription);
        }

        public virtual void UnRegister<TCommand>()
        {
            _commands.Remove(typeof (TCommand));
        }
    }
}
