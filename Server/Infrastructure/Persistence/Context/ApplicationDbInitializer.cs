namespace Persistence.Context
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Shared.Interfaces;
    using Domain.Entities.Identity;

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly ITransactionHelper _transactionHelper;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<UserRole> roleManager,
            ITransactionHelper transactionHelper)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _transactionHelper = transactionHelper;
        }

        public async Task InitialiseAsync()
        {
            if (!await _context.Database.CanConnectAsync())
            {
                _logger.LogInformation("Database does not exist. Creating and applying migrations...");
                await _context.Database.MigrateAsync();
                return;
            }

            using var transaction = await _transactionHelper.BeginTransactionAsync();
            try
            {
                var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying {Count} pending migrations: {Migrations}",
                        pendingMigrations.Count,
                        string.Join(", ", pendingMigrations));
                    await _context.Database.MigrateAsync();
                }
                else
                {
                    _logger.LogInformation("No pending migrations. Database is up to date.");
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SeedAsync()
        {
            if (!await _context.Database.CanConnectAsync())
            {
                _logger.LogError("Cannot seed database because it does not exist or cannot be accessed.");
                return;
            }

            using var transaction = await _transactionHelper.BeginTransactionAsync();
            try
            {
                if (!await _context.Set<UserRole>().AnyAsync())
                {
                    await TrySeedAsync();
                    _logger.LogInformation("Seeding completed successfully");
                }
                else
                {
                    _logger.LogInformation("Skipping seeding as data already exists");
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            var administratorRole = new UserRole("Administrator")
            {
                Description = "Admin Role"
            };

            var userRole = new UserRole("User")
            {
                Description = "User Role"
            };

            if (!_roleManager.Roles.Any(r => r.Name == administratorRole.Name))
            {
                await _roleManager.CreateAsync(administratorRole);
            }

            if (!_roleManager.Roles.Any(r => r.Name == userRole.Name))
            {
                await _roleManager.CreateAsync(userRole);
            }

            var adminEmail = "admin@admin.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedBy = "Initial Seed",
                    EmailConfirmed = true
                };

                var createAdminResult = await _userManager.CreateAsync(adminUser, "123456");
                if (createAdminResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(adminUser, new[] { administratorRole.Name });
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
                }
            }

            var userEmail = "user@example.com";
            var standardUser = await _userManager.FindByEmailAsync(userEmail);
            if (standardUser == null)
            {
                standardUser = new User
                {
                    Id = "58efac42-e31d-4039-bfe1-76672c615dd5",
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "User",
                    LastName = "One",
                    IsActive = true,
                    CreatedBy = "Initial Seed",
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(standardUser, "123456");
                if (createUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(standardUser, new[] { userRole.Name });
                }
                else
                {
                    _logger.LogError("Failed to create standard user: {Errors}", string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}