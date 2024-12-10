﻿namespace Infrastructure.Services.Identity
{
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    using Mapster;

    using Domain.Entities;

    using Models.User;

    using Shared;
    using Shared.Exceptions;
    using Shared.Interfaces;
    using Application.Interfaces.Identity;

    public class UserService : IUserService
    {
        private readonly IIdentityRepository<User> _userRepository;
        private readonly ITransactionHelper _transactionHelper;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IIdentityRepository<User> userRepository,
            UserManager<User> userManager,
            ILogger<UserService> logger,
            ITransactionHelper transactionHelper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
            _transactionHelper = transactionHelper;
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

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.AsTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId)
                    ?? throw new CustomException($"User with ID {userId} not found.");

                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ReactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.AsTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId)
                    ?? throw new CustomException($"User with ID {userId} not found.");

                user.IsActive = true;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reactivating user {UserId}", userId);
                throw;
            }
        }

        public async Task<IReadOnlyList<UserDto>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                return usersInRole.Adapt<IReadOnlyList<UserDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users by role {Role}", role);
                throw;
            }
        }

        public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new CustomException($"User with ID {userId} not found.");

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!result.Succeeded)
                {
                    return Result<string>.Failure(result.Errors.Select(e => e.Description).ToList());
                }

                return Result<string>.SuccessResult("Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Result<string>> UpdateEmailAsync(string userId, UpdateEmailRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new CustomException($"User with ID {userId} not found.");

                if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
                {
                    return Result<string>.Failure(new List<string> { "Current password is incorrect." });
                }

                if (await _userManager.FindByEmailAsync(request.NewEmail) != null)
                {
                    return Result<string>.Failure(new List<string> { "Email is already in use." });
                }

                await using var transaction = await _transactionHelper.BeginTransactionAsync();
                try
                {
                    user.Email = request.NewEmail;
                    user.UserName = request.NewEmail;
                    user.EmailConfirmed = false;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        return Result<string>.Failure(result.Errors.Select(e => e.Description).ToList());
                    }

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await transaction.CommitAsync();

                    return Result<string>.SuccessResult("Email update initiated. Please check your email to confirm the change.");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating email for user {UserId}", userId);
                throw;
            }
        }
    }
}