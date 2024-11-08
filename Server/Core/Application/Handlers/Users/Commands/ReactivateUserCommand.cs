namespace Application.Handlers.Users.Commands
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Shared;
    using Shared.Exceptions;

    public record ReactivateUserCommand(string UserId) : IRequest<Result<bool>>
    {
        public class ReactivateUserCommandHandler : IRequestHandler<ReactivateUserCommand, Result<bool>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<ReactivateUserCommandHandler> _logger;

            public ReactivateUserCommandHandler(
                IUserService userService,
                ILogger<ReactivateUserCommandHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<bool>> Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _userService.ReactivateUserAsync(request.UserId);
                    return Result<bool>.SuccessResult(result);
                }
                catch (CustomException ex)
                {
                    _logger.LogWarning(ex, "Failed to reactivate user {UserId}", request.UserId);
                    return Result<bool>.Failure(new List<string> { ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reactivate user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}