using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ninject;
using Ninject.Syntax;
using System;

namespace MicrosoftDIAdapters.Ninject
{
    /// <summary>
    /// Applies the scope binding to the syntax
    /// </summary>
    /// <param name="serviceDescriptor"></param>
    /// <param name="bindingSyntax"></param>
    /// <returns></returns>
    public delegate IBindingNamedWithOrOnSyntax<object> ScopedBindingResolver(ServiceDescriptor serviceDescriptor, IBindingInSyntax<object> bindingSyntax);

    public static class NinjectAdapter
    {
        public static IServiceCollection AddNinjectServiceScopeFactory(this IServiceCollection services) => services.AddTransient<IServiceScopeFactory, ServiceScopeFactory>();

        /// <summary>
        /// Populates the kernel with the supplied services.
        /// If <see cref="ServiceLifetime.Scoped"/> services are added, the caller must pass a <see cref="ScopedBindingResolver"/> delegate
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="services"></param>
        /// <param name="scopeBindingResolver">Used to apply scope for scoped service descriptors <see cref="ServiceLifetime.Scoped"/>. If not specified, will throw an exception</param>
        public static void Populate(this IKernel kernel, IServiceCollection services, ScopedBindingResolver scopeBindingResolver = null)
        {
            if (scopeBindingResolver == null)
                scopeBindingResolver = (serviceDescriptor, syntax) => throw new Exception(
                    $"Scoped service descriptors are not supported by default. To support scoped bindings, please supply a {typeof(ScopedBindingResolver)} delegate");

            services.TryAddSingleton<IServiceProvider>(kernel);

            services.TryAddSingleton<IServiceScopeFactory, ServiceScopeFactory>();

            foreach (var serviceDescriptor in services)
            {
                kernel
                    .Bind(serviceDescriptor.ServiceType)
                    .To(serviceDescriptor)
                    .InScope(serviceDescriptor, scopeBindingResolver);
            }
        }

        private static IBindingInSyntax<object> To(this IBindingToSyntax<object> syntax, ServiceDescriptor s)
        {
            if (s.ImplementationType != null)
                return syntax.To(s.ImplementationType);

            if (s.ImplementationFactory != null)
                return syntax.ToMethod(ctx => s.ImplementationFactory(ctx.Kernel));

            if (s.ImplementationInstance != null)
                return syntax.ToConstant(s.ImplementationInstance);

            throw new Exception("Invalid service descriptor binding");
        }

        private static IBindingNamedWithOrOnSyntax<object> InScope(this IBindingInSyntax<object> syntax, ServiceDescriptor s, ScopedBindingResolver scopeBindingResolver)
        {
            switch (s.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    return syntax.InSingletonScope();
                case ServiceLifetime.Scoped:
                    return scopeBindingResolver(s, syntax);
                case ServiceLifetime.Transient:
                    return syntax.InTransientScope();

            }

            throw new Exception("Invalid service descriptor binding");
        }
    }
}
