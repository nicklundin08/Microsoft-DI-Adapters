# Microsoft-DI-Adapters
A library for bridging the gap between the build in .NET di framework and other di frameworks

### Microsoft-DI-Adapters.Ninject
```C#
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    //Configure services here
    //services.AddMvc();

    var kernel = new StandardKernel();

    //configure kernel here
   
    //Second parameter only required if you have services bound with the ServiceLifeTime.Scoped
    kernel.Populate(services: services, scopeBindingResolver: RequestScopeBindingResolver);

    return kernel;
}

private IBindingNamedWithOrOnSyntax<object> RequestScopeBindingResolver(ServiceDescriptor serviceDescriptor, IBindingInSyntax<object> bindingSyntax)
{
    //Requires Ninject.Web.Common
    return bindingSyntax.InRequestScope();
}
```
