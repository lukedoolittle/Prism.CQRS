using Prism.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SimpleCQRS.Infrastructure;

namespace Prism.Commands
{
    /// <summary>
    /// The CompositeCommand composes one or more ICommands.
    /// </summary>
    public class CompositeCommand : Command
    {
        private readonly List<Command> _registeredCommands = new List<Command>();
        private readonly bool _monitorCommandActivity;
        private readonly EventHandler _onRegisteredCommandCanExecuteChangedHandler;

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeCommand"/>.
        /// </summary>
        public CompositeCommand()
        {
            this._onRegisteredCommandCanExecuteChangedHandler = new EventHandler(this.OnRegisteredCommandCanExecuteChanged);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompositeCommand"/>.
        /// </summary>
        /// <param name="monitorCommandActivity">Indicates when the command activity is going to be monitored.</param>
        public CompositeCommand(bool monitorCommandActivity)
            : this()
        {
            this._monitorCommandActivity = monitorCommandActivity;
        }

        /// <summary>
        /// Adds a command to the collection and signs up for the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        ///  <remarks>
        /// If this command is set to monitor command activity, and <paramref name="command"/> 
        /// implements the <see cref="IActiveAwareCommand"/> interface, this method will subscribe to its
        /// <see cref="IActiveAwareCommand.IsActiveChanged"/> event.
        /// </remarks>
        /// <param name="command">The command to register.</param>
        public virtual void RegisterCommand(Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command == this)
            {
                throw new ArgumentException(Resources.CannotRegisterCompositeCommandInItself);
            }

            lock (this._registeredCommands)
            {
                if (this._registeredCommands.Contains(command))
                {
                    throw new InvalidOperationException(Resources.CannotRegisterSameCommandTwice);
                }
                this._registeredCommands.Add(command);
            }

            command.CanExecuteChanged += this._onRegisteredCommandCanExecuteChangedHandler;
            this.OnCanExecuteChanged();

            if (this._monitorCommandActivity)
            {
                var activeAwareCommand = command as IActiveAware;
                if (activeAwareCommand != null)
                {
                    activeAwareCommand.IsActiveChanged += this.Command_IsActiveChanged;
                }
            }
        }

        /// <summary>
        /// Removes a command from the collection and removes itself from the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        /// <param name="command">The command to unregister.</param>
        public virtual void UnregisterCommand(Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            bool removed;
            lock (this._registeredCommands)
            {
                removed = this._registeredCommands.Remove(command);
            }

            if (removed)
            {
                command.CanExecuteChanged -= this._onRegisteredCommandCanExecuteChangedHandler;
                this.OnCanExecuteChanged();

                if (this._monitorCommandActivity)
                {
                    var activeAwareCommand = command as IActiveAware;
                    if (activeAwareCommand != null)
                    {
                        activeAwareCommand.IsActiveChanged -= this.Command_IsActiveChanged;
                    }
                }
            }
        }

        private void OnRegisteredCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }


        /// <summary>
        /// Forwards <see cref="ICommand.CanExecute"/> to the registered commands and returns
        /// <see langword="true" /> if all of the commands return <see langword="true" />.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to <see langword="null" />.
        /// </param>
        /// <returns><see langword="true" /> if all of the commands return <see langword="true" />; otherwise, <see langword="false" />.</returns>
        public override bool CanExecute(object parameter)
        {
            bool hasEnabledCommandsThatShouldBeExecuted = false;

            Command[] commandList;
            lock (this._registeredCommands)
            {
                commandList = this._registeredCommands.ToArray();
            }
            foreach (var command in commandList)
            {
                if (this.ShouldExecute(command))
                {
                    if (!command.CanExecute(parameter))
                    {
                        return false;
                    }

                    hasEnabledCommandsThatShouldBeExecuted = true;
                }
            }

            return hasEnabledCommandsThatShouldBeExecuted;
        }

        /// <summary>
        /// Occurs when any of the registered commands raise <see cref="ICommand.CanExecuteChanged"/>.
        /// </summary>
        public override event EventHandler CanExecuteChanged;

        /// <summary>
        /// Forwards <see cref="ICommand.Execute"/> to the registered commands.
        /// </summary>
        /// <param name="parameter">Data used by the command.
        /// If the command does not require data to be passed, this object can be set to <see langword="null" />.
        /// </param>
        public override async Task Execute(object parameter)
        {
            Queue<Command> commands;
            lock (this._registeredCommands)
            {
                commands = new Queue<Command>(this._registeredCommands.Where(this.ShouldExecute).ToList());
            }

            var executingCommands = new List<Task>();
            while (commands.Count > 0)
            {
                var command = commands.Dequeue();
                executingCommands.Add(command.Execute(parameter));
            }

            await Task.WhenAll(executingCommands);
        }

        /// <summary>
        /// Evaluates if a command should execute.
        /// </summary>
        /// <param name="command">The command to evaluate.</param>
        /// <returns>A <see cref="bool"/> value indicating whether the command should be used 
        /// when evaluating <see cref="CompositeCommand.CanExecute"/> and <see cref="CompositeCommand.Execute"/>.</returns>
        /// <remarks>
        /// If this command is set to monitor command activity, and <paramref name="command"/>
        /// implements the <see cref="IActiveAwareCommand"/> interface, 
        /// this method will return <see langword="false" /> if the command's <see cref="IActiveAwareCommand.IsActive"/> 
        /// property is <see langword="false" />; otherwise it always returns <see langword="true" />.</remarks>
        protected virtual bool ShouldExecute(Command command)
        {
            var activeAwareCommand = command as IActiveAware;

            if (this._monitorCommandActivity && activeAwareCommand != null)
            {
                return activeAwareCommand.IsActive;
            }

            return true;
        }

        /// <summary>
        /// Gets the list of all the registered commands.
        /// </summary>
        /// <value>A list of registered commands.</value>
        /// <remarks>This returns a copy of the commands subscribed to the CompositeCommand.</remarks>
        public IList<Command> RegisteredCommands
        {
            get
            {
                IList<Command> commandList;
                lock (this._registeredCommands)
                {
                    commandList = this._registeredCommands.ToList();
                }

                return commandList;
            }
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> on the UI thread so every 
        /// command invoker can requery <see cref="ICommand.CanExecute"/> to check if the
        /// <see cref="CompositeCommand"/> can execute.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handler for IsActiveChanged events of registered commands.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">EventArgs to pass to the event.</param>
        private void Command_IsActiveChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }
    }
}