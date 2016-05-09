using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;

namespace Prism.CQRS.Bootstrapper.Autofac
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterAssemblyGenericInterfaceImplementors(
            this ContainerBuilder instance, 
            IEnumerable<Assembly> assemblies, 
            Type openGenericInterface)
        {
            var types = assemblies.Select(a => a.ExportedTypes.AsEnumerable()).SelectMany(a => a);

            var handlerTypes = types.Where(
            t =>
                t.GetTypeInfo()
                    .ImplementedInterfaces.Any(
                        i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface));

            foreach (var type in handlerTypes)
            {
                if (type.GetTypeInfo().IsGenericType)
                {
                    instance.RegisterGeneric(type).As(openGenericInterface);
                }
                else
                {
                    instance.RegisterType(type).AsImplementedInterfaces();
                }
            }

            return instance;
        }

        public static IEnumerable<Type> ResolutionTypes<T>(this IContainer instance)
        {
            var registrations = instance.ComponentRegistry
                .RegistrationsFor(new TypedService(typeof (T)));

            return registrations
                .Select(registration => registration.Activator)
                .OfType<ReflectionActivator>()
                .Select(activator => activator.LimitType)
                .ToList();
        } 
    }
}
