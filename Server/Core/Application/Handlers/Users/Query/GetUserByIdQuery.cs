namespace Application.Handlers.Users.Query
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Models.User;

    using Shared;
    using Shared.Exceptions;

    public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto>>
    {
        public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<GetUserByIdQueryHandler> _logger;

            public GetUserByIdQueryHandler(
                IUserService userService,
                ILogger<GetUserByIdQueryHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userService.GetByIdAsync(request.UserId);
                    return Result<UserDto>.SuccessResult(user);
                }
                catch (CustomException ex)
                {
                    _logger.LogWarning(ex, "User not found {UserId}", request.UserId);
                    return Result<UserDto>.Failure(new List<string> { ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving user {UserId}", request.UserId);
                    throw;
                }
            }
        }
    }
}
