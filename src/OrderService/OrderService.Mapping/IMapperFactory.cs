namespace OrderService.Mapping;

public interface IMapperFactory
{
    IMapper<TSource, TDestination> GetMapper<TSource, TDestination>()
        where TSource : class
        where TDestination : class;
}