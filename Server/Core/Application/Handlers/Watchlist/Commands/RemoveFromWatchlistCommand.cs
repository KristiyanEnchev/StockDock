namespace Application.Handlers.Watchlist.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;
    using Application.Interfaces.Watchlist;

    public record RemoveFromWatchlistCommand(string UserId, string Symbol) : IRequest<Result<bool>>;

    public class RemoveFromWatchlistCommandHandler
        : IRequestHandler<RemoveFromWatchlistCommand, Result<bool>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly ILogger<RemoveFromWatchlistCommandHandler> _logger;

        public RemoveFromWatchlistCommandHandler(
            IWatchlistService watchlistService,
            ILogger<RemoveFromWatchlistCommandHandler> logger)
        {
            _watchlistService = watchlistService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            RemoveFromWatchlistCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.RemoveFromWatchlistAsync(
                    request.UserId,
                    request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock from watchlist for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}