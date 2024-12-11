namespace Application.Handlers.Watchlist.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Models.Stock;

    using Shared;

    using Application.Interfaces.Identity;
    using Application.Interfaces.Watchlist;

    public record AddToWatchlistCommand(string Symbol) : IRequest<Result<UserWatchlistDto>>;

    public class AddToWatchlistCommandHandler : IRequestHandler<AddToWatchlistCommand, Result<UserWatchlistDto>>
    {
        private readonly IWatchlistService _watchlistService;
        private readonly IUser _currentUser;
        private readonly ILogger<AddToWatchlistCommandHandler> _logger;

        public AddToWatchlistCommandHandler(
            IWatchlistService watchlistService,
            IUser currentUser,
            ILogger<AddToWatchlistCommandHandler> logger)
        {
            _watchlistService = watchlistService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<UserWatchlistDto>> Handle(AddToWatchlistCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _watchlistService.AddToWatchlistAsync(_currentUser.Id!, request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock {Symbol} to watchlist for user {UserId}",
                    request.Symbol, _currentUser.Id);
                throw;
            }
        }
    }
}