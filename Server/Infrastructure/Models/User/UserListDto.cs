namespace Models.User
{
    using Domain.Entities;

    using Mapster;

    public class UserListDto : BaseIdentityAuditableDto<UserListDto, User>, IMapFrom<User>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; } = string.Empty;

        public void Mapping(TypeAdapterConfig config) 
        {
            base.Mapping(config);

            config.NewConfig<User, UserListDto>()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .IgnoreNullValues(true);
        }
    }
}