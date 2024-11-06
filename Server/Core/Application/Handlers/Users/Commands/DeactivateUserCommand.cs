namespace Application.Handlers.Users.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Shared;
    using Shared.Exceptions;

    public record DeactivateUserCommand(string UserId) : IRequest<Result<bool>>
    {
        public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<bool>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<DeactivateUserCommandHandler> _logger;

            public DeactivateUserCommandHandler(
                IUserService userService,
                ILogger<DeactivateUserCommandHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _userService.DeactivateUserAsync(request.UserId);
                    return Result<bool>.SuccessResult(result);
                }
                catch (CustomException ex)
                {
                    _logger.LogWarning(ex, "Failed to deactivate user {UserId}", request.UserId);
                    return Result<bool>.Failure(new List<string> { ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deactivating user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}
