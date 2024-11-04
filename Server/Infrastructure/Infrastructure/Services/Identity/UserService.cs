namespace Infrastructure.Services.Identity
{
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Identity;

    using Mapster;

    using Application.Interfaces;

    using Domain.Entities;

    using Models.User;

    using Shared;
    using Shared.Exceptions;
    using Shared.Interfaces;

    public class UserService : IUserService
    {
        private readonly IIdentityRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IIdentityRepository<User> userRepository,
            UserManager<User> userManager,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserDto> GetByIdAsync(string id)
        {
            try
            {
                return await _userRepository.GetByIdAsync<UserDto>(id)
                    ?? throw new CustomException($"User with ID {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user by id {Id}", id);
                throw;
            }
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync<UserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                throw;
            }
        }

       
    }
}