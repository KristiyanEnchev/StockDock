namespace Application.Handlers.Users.Query
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Models.User;

    using Shared;

    public record GetAllUsersQuery : IRequest<Result<IReadOnlyList<UserDto>>>
    {
        public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IReadOnlyList<UserDto>>>
        {
            private readonly IUserService _userService;
            private readonly ILogger<GetAllUsersQueryHandler> _logger;

            public GetAllUsersQueryHandler(
                IUserService userService,
                ILogger<GetAllUsersQueryHandler> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var users = await _userService.GetAllAsync();
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