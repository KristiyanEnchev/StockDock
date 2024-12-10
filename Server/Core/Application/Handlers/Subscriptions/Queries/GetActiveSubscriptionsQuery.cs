namespace Application.Handlers.Subscriptions.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Shared;

    public record GetActiveSubscriptionsQuery(string UserId) : IRequest<Result<IReadOnlyList<string>>>;

    public class GetActiveSubscriptionsQueryHandler
        : IRequestHandler<GetActiveSubscriptionsQuery, Result<IReadOnlyList<string>>>
    {
        private readonly IStockSubscriptionService _subscriptionService;
        private readonly ILogger<GetActiveSubscriptionsQueryHandler> _logger;

        public GetActiveSubscriptionsQueryHandler(
            IStockSubscriptionService subscriptionService,
            ILogger<GetActiveSubscriptionsQueryHandler> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<string>>> Handle(
            GetActiveSubscriptionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                return await _subscriptionService.GetActiveSubscriptionsAsync(request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscriptions for user {UserId}",
                    request.UserId);
                throw;
            }
        }
    }
}
