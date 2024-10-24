namespace Shared.Mappings
{
    using Mapster;

    public interface IMapFrom<T>
    {
        void Mapping(TypeAdapterConfig config);
    }
}