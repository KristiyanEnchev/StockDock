namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;

    public record GetUserWatchlistQuery(string UserId) : IRequest<Result<IReadOnlyList<UserWatchlistDto>>>;

    public class GetUserWatchlistQueryHandler : IRequestHandler<GetUserWatchlistQuery, Result<IReadOnlyList<UserWatchlistDto>>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly ILogger<GetUserWatchlistQueryHandler> _logger;

        public GetUserWatchlistQueryHandler(
            IWatchlistService watchlistService,
            ILogger<GetUserWatchlistQueryHandler> logger)
        {
            _watchlistService = watchlistService;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<UserWatchlistDto>>> Handle(
            GetUserWatchlistQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.GetUserWatchlistAsync(request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}