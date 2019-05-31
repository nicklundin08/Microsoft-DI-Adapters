using Microsoft.Extensions.DependencyInjection;
using System;

namespace MicrosoftDIAdapters.Ninject
{
    public class ServiceScope : IServiceScope
    {
        public ServiceScope(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public void Dispose()
        {
        }

        public IServiceProvider ServiceProvider { get; }
    }

    public class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceScopeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return new ServiceScope(_serviceProvider);
        }
    }
}
