namespace OrderService.Mapping;

public interface IMapper<in TSource, out TDestination>
    where TSource : class
    where TDestination : class
{
    public Type ModelType => typeof(TSource);

    public TDestination Map(TSource source);
}