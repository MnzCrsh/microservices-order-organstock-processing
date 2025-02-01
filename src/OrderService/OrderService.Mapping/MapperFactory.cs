using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Mapping;

public class MapperFactory(IServiceProvider serviceProvider) : IMapperFactory
{
    public IMapper<TSource, TDestination> GetMapper<TSource, TDestination>() where TSource : class where TDestination : class
    {
        return serviceProvider.GetService<IMapper<TSource, TDestination>>() ??
               throw new InvalidOperationException
                   ($"Mapper is not registered for type[{typeof(TSource).FullName} to {typeof(TDestination).FullName}].");
    }
}