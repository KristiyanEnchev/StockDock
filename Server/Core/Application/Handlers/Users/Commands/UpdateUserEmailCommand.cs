namespace Application.Handlers.Users.Commands
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Shared;
    using Shared.Exceptions;

    using Models.User;

    public record UpdateUserEmailCommand(string UserId, string NewEmail, string CurrentPassword) 
        : IRequest<Result<string>>
    {
        public class UpdateUserEmailCommandHandler : IRequestHandler<UpdateUserEmailCommand, Result<string>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<UpdateUserEmailCommandHandler> _logger;

            public UpdateUserEmailCommandHandler(
                IUserService userService,
                ILogger<UpdateUserEmailCommandHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<string>> Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var updateEmailRequest = new UpdateEmailRequest
                    {
                        CurrentPassword = request.CurrentPassword,
                        NewEmail = request.NewEmail
                    };

                    return await _userService.UpdateEmailAsync(request.UserId, updateEmailRequest);
                }
                catch (CustomException ex)
                {
                    _logger.LogWarning(ex, "Failed to update Email for user {UserId}", request.UserId);
                    return Result<string>.Failure(new List<string> { ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Email for user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}