namespace Application.Handlers.Subscriptions.Commands
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Shared;
    using Application.Interfaces.Subscription;

    public record SubscribeToStockCommand(string UserId, string Symbol) : IRequest<Result<bool>>;

    public class SubscribeToStockCommandHandler : IRequestHandler<SubscribeToStockCommand, Result<bool>>
    {
        private readonly IStockSubscriptionService _subscriptionService;
        private readonly ILogger<SubscribeToStockCommandHandler> _logger;

        public SubscribeToStockCommandHandler(
            IStockSubscriptionService subscriptionService,
            ILogger<SubscribeToStockCommandHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(SubscribeToStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await _subscriptionService.SubscribeAsync(request.UserId, request.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing user {UserId} to stock {Symbol}",
                    request.UserId, request.Symbol);
                throw;
            }
        }
    }
}
