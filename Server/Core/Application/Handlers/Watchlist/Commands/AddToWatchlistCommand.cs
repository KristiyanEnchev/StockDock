namespace Application.Handlers.Watchlist.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;

    public record AddToWatchlistCommand(string UserId, CreateWatchlistItemRequest Request)
        : IRequest<Result<UserWatchlistDto>>;

    public class AddToWatchlistCommandHandler
        : IRequestHandler<AddToWatchlistCommand, Result<UserWatchlistDto>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly ILogger<AddToWatchlistCommandHandler> _logger;

        public AddToWatchlistCommandHandler(
            IWatchlistService watchlistService,
            ILogger<AddToWatchlistCommandHandler> logger)
        {
            _watchlistService = watchlistService;
            _logger = logger;
        }

        public async Task<Result<UserWatchlistDto>> Handle(
            AddToWatchlistCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.AddToWatchlistAsync(
                    request.UserId,
                    request.Request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock to watchlist for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}