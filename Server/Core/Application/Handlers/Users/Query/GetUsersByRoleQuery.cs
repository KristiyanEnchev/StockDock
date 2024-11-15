namespace Application.Handlers.Users.Query
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Models.User;

    using Shared;

    public record GetUsersByRoleQuery(string Role) : IRequest<Result<IReadOnlyList<UserDto>>>
    {
        public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, Result<IReadOnlyList<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<GetUsersByRoleQueryHandler> _logger;

            public GetUsersByRoleQueryHandler(
                IUserService userService,
                ILogger<GetUsersByRoleQueryHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var users = await _userService.GetUsersByRoleAsync(request.Role);
                    return Result<IReadOnlyList<UserDto>>.SuccessResult(users);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving all users");
                    throw;
                }
            }
        }
    }
}