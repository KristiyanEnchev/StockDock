﻿namespace Application.Interfaces.Identity
{
    using Shared;

    using Models.User;

    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(string id);
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<IReadOnlyList<UserDto>> GetUsersByRoleAsync(string role);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ReactivateUserAsync(string userId);

        Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<Result<string>> UpdateEmailAsync(string userId, UpdateEmailRequest request);
    }
}