namespace Application.Handlers.Stocks.Commands
{
    using Microsoft.Extensions.Logging;

    using Application.Interfaces;

    using MediatR;

    using Shared;

    public record UnsubscribeFromStockCommand(string UserId, string Symbol) : IRequest<Result<bool>>;

    public class UnsubscribeFromStockCommandHandler : IRequestHandler<UnsubscribeFromStockCommand, Result<bool>>
    {
        private readonly IStockSubscriptionService _subscriptionService;
        private readonly ILogger<UnsubscribeFromStockCommandHandler> _logger;

        public UnsubscribeFromStockCommandHandler(
            IStockSubscriptionService subscriptionService,
            ILogger<UnsubscribeFromStockCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UnsubscribeFromStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _subscriptionService.UnsubscribeAsync(request.UserId, request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing user {UserId} from stock {Symbol}",
                    request.UserId, request.Symbol);
                throw;
            }
        }
    }
}