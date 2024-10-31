﻿namespace Models.User
{
    using Domain.Entities;

    using Mapster;

    public class UserDto : BaseIdentityAuditableDto<UserDto, User>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; } = string.Empty;

        public override void CustomizeMapping(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
                .IgnoreNullValues(true);
        }
    }
}
