namespace Application.Handlers.Watchlist.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;

    using Application.Interfaces.Identity;
    using Application.Interfaces.Watchlist;

    public record RemoveFromWatchlistCommand(string Symbol) : IRequest<Result<bool>>;

    public class RemoveFromWatchlistCommandHandler : IRequestHandler<RemoveFromWatchlistCommand, Result<bool>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IUser _currentUser;
        private readonly ILogger<RemoveFromWatchlistCommandHandler> _logger;

        public RemoveFromWatchlistCommandHandler(
            IWatchlistService watchlistService,
            IUser currentUser,
            ILogger<RemoveFromWatchlistCommandHandler> logger)
        {
            _watchlistService = watchlistService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RemoveFromWatchlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.RemoveFromWatchlistAsync(_currentUser.Id!, request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock {Symbol} from watchlist for user {UserId}",
                    request.Symbol, _currentUser.Id);
                throw;
            }
        }
    }
}