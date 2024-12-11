namespace Application.Handlers.Watchlist.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;

    using Application.Interfaces.Identity;
    using Application.Interfaces.Watchlist;

    public record GetUserWatchlistQuery : IRequest<Result<List<UserWatchlistDto>>>;

    public class GetUserWatchlistQueryHandler : IRequestHandler<GetUserWatchlistQuery, Result<List<UserWatchlistDto>>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IUser _currentUser;
        private readonly ILogger<GetUserWatchlistQueryHandler> _logger;

        public GetUserWatchlistQueryHandler(
            IWatchlistService watchlistService,
            IUser currentUser,
            ILogger<GetUserWatchlistQueryHandler> logger)
        {
            _watchlistService = watchlistService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<List<UserWatchlistDto>>> Handle(GetUserWatchlistQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.GetUserWatchlistAsync(_currentUser.Id!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist for user {UserId}", _currentUser.Id);
                throw;
            }
        }
    }
}