﻿namespace Persistence.Context
{
    using System.Reflection;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

    using Application.Interfaces.Identity;

    using Domain.Interfaces;
    using Domain.Entities.Base;
    using Domain.Entities.Identity;
    using Domain.Entities.Stock;

    public class ApplicationDbContext : IdentityDbContext<User, UserRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IUser _user;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher dispatcher, IUser user)
            : base(options)
        {
            _dispatcher = dispatcher;
            _user = user;
        }

        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<StockPriceHistory> StockPriceHistories => Set<StockPriceHistory>();
        public DbSet<UserWatchlist> UserWatchlists => Set<UserWatchlist>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplySoftDelete(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.ApplySnakeCaseNamingConvention();

            DisableCascadeDelete(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await ApplyAuditInfoAsync();
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEvents();
            return result;
        }

        private async Task ApplyAuditInfoAsync()
        {
            var entries = ChangeTracker.Entries<IAuditableEntity>()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);

            var userId = _user.Id;
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = currentTime;
                    entry.Entity.CreatedBy = userId;
                }

                entry.Entity.UpdatedDate = currentTime;
                entry.Entity.UpdatedBy = userId;

                if (entry.Entity is ISoftDelete softDelete &&
                    entry.Property(nameof(ISoftDelete.IsDeleted)).IsModified &&
                    softDelete.IsDeleted)
                {
                    softDelete.DeletedDate = currentTime;
                    softDelete.DeletedBy = userId;
                }
            }
        }

        private void ApplySoftDelete(ModelBuilder modelBuilder) 
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var deletedCheck = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                            Expression.Constant(false)
                        ),
                        parameter
                    );
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(deletedCheck);
                }
            }
        }

        private async Task DispatchDomainEvents()
        {
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            if (_dispatcher != null && entitiesWithEvents.Any())
            {
                await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);
            }
        }

        public static void DisableCascadeDelete(ModelBuilder builder)
        {
            var entityTypes = builder.Model.GetEntityTypes().ToList();
            var foreignKeys = entityTypes
            .SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
            foreach (var foreignKey in foreignKeys)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void ApplySnakeCaseNamingConvention(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var foreignKey in entity.GetForeignKeys())
                {
                    foreignKey.SetConstraintName(foreignKey.GetConstraintName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
                }
            }
        }

        private static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}