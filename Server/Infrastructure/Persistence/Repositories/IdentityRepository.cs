namespace Persistence.Repositories
{
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using Domain.Entities;

    using Mapster;

    using Persistence.Context;
    
    using Shared.Interfaces;

    public class IdentityRepository<TEntity> : IIdentityRepository<TEntity>
        where TEntity : BaseIdentityAuditableEntity
    {
       
    }
}