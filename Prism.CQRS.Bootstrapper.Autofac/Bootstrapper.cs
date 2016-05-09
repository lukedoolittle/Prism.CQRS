using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.ResolveAnything;
using SimpleCQRS.Framework;
using SimpleCQRS.Framework.Contracts;
using Module = Autofac.Module;

namespace Prism.CQRS.Bootstrapper.Autofac
{
    public abstract class Bootstrapper
    {
        private readonly List<Action<IContainer>> _startupTasks;
        private readonly ContainerBuilder _builder;

        protected Bootstrapper()
        {
            _startupTasks = new List<Action<IContainer>>();
            _builder = new ContainerBuilder();
        }

        protected virtual IContainer Run(bool useDefaultConfiguration = true)
        {
            if (useDefaultConfiguration)
            {
                ProvideDefaultConfiguration();
            }

            var resolver = _builder.Build();

            RunStartupTasks(resolver);

            return resolver;
        }

        public Bootstrapper AddModule<TModule>()
            where TModule : Module, new()
        {
            _builder.RegisterModule<TModule>();

            return this;
        }

        protected Bootstrapper AddAllModules(AssemblyCatalog catalog)
        {
            _builder.RegisterAssemblyModules(catalog.ToArray());

            return this;
        }

        public Bootstrapper AddStartupTask<TTask>()
            where TTask : IStartupTask
        {
            _startupTasks.Add(resolver => resolver.Resolve<TTask>().Execute());

            return this;
        }

        protected Bootstrapper AddAllStartupTasks(AssemblyCatalog catalog)
        {
            var startupTaskTypes = catalog
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsAssignableTo<IStartupTask>());

            foreach (var startupTaskType in startupTaskTypes)
            {
                _startupTasks.Add(resolver =>
                    ((IStartupTask)resolver.Resolve(startupTaskType))
                    .Execute());
            }

            return this;
        }

        protected abstract AssemblyCatalog ProvideCatalog();

        protected virtual void ProvideDefaultConfiguration()
        {
            var catalog = ProvideCatalog();
            this.AddAllModules(catalog).AddAllStartupTasks(catalog);
            _builder.RegisterAssemblyGenericInterfaceImplementors(
                catalog, 
                typeof(IEventHandler<>));
            _builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }

        private void RunStartupTasks(IContainer resolver)
        {
            foreach (var task in _startupTasks)
            {
                task(resolver);
            }
        }
    }
}
