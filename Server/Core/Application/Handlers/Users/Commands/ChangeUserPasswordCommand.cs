namespace Application.Handlers.Users.Commands
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Models.User;

    using Shared;
    using Shared.Exceptions;

    public record ChangeUserPasswordCommand(string UserId, string CurrentPassword, string NewPassword)
        : IRequest<Result<string>>
    {
        public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, Result<string>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<ChangeUserPasswordCommandHandler> _logger;

            public ChangeUserPasswordCommandHandler(
                IUserService userService,
                ILogger<ChangeUserPasswordCommandHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var changePasswordRequest = new ChangePasswordRequest
                    {
                        CurrentPassword = request.CurrentPassword,
                        NewPassword = request.NewPassword
                    };

                    return await _userService.ChangePasswordAsync(request.UserId, changePasswordRequest);
                }
                catch (CustomException ex)
                {
                    _logger.LogWarning(ex, "Failed to change password for user {UserId}", request.UserId);
                    return Result<string>.Failure(new List<string> { ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error changing password for user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}
