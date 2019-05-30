using FluentAssertions;
using Microsoft.Extensions.Adapters.Ninject;
using Microsoft.Extensions.Adapters.Test.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using System;
using Xunit;

namespace Microsoft.Extensions.Adapters.Test.Ninject
{
    public class NinjectAdapterTests
    {
        private readonly IServiceProvider serviceProvider;

        public NinjectAdapterTests()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ISingletonDependency, SingletonDependency>();

            services.AddTransient<ITransientDependency, TransientDependency>();

            var kernel = new StandardKernel();

            kernel.Populate(services);

            serviceProvider = kernel;
        }

        [Fact]
        public void ShouldReuseSingletonDependencies()
        {
            var instance1 = serviceProvider.GetRequiredService<ISingletonDependency>();

            var instance2 = serviceProvider.GetRequiredService<ISingletonDependency>();

            instance1.Equals(instance2).Should().BeTrue();

        }

        [Fact]
        public void ShouldReuseSingletonDependenciesInScope()
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var instance1 = scope.ServiceProvider.GetRequiredService<ISingletonDependency>();

                var instance2 = scope.ServiceProvider.GetRequiredService<ISingletonDependency>();

                instance1.Equals(instance2).Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldNotReuseTransientDependencies()
        {
            var instance1 = serviceProvider.GetRequiredService<ITransientDependency>();

            var instance2 = serviceProvider.GetRequiredService<ITransientDependency>();

            instance1.Equals(instance2).Should().BeFalse();
        }

        [Fact]
        public void ShouldNotReuseTransientsDependenciesInScope()
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var instance1 = scope.ServiceProvider.GetRequiredService<ITransientDependency>();

                var instance2 = scope.ServiceProvider.GetRequiredService<ITransientDependency>();

                instance1.Equals(instance2).Should().BeFalse();
            }
        }
    }
}
