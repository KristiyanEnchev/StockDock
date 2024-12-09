namespace Application.Handlers.Stocks.Queries
{
    using Microsoft.Extensions.Logging;

    using MediatR;

    using Application.Interfaces;

    using Models.Stock;

    using Shared;

    public record GetTechnicalIndicatorsQuery(string Symbol, string[] Indicators)
        : IRequest<Result<TechnicalIndicatorsDto>>;

    public class GetTechnicalIndicatorsQueryHandler
        : IRequestHandler<GetTechnicalIndicatorsQuery, Result<TechnicalIndicatorsDto>>
    {
        private readonly ITechnicalAnalysisService _technicalAnalysis;
        private readonly ILogger<GetTechnicalIndicatorsQueryHandler> _logger;

        public GetTechnicalIndicatorsQueryHandler(
            ITechnicalAnalysisService technicalAnalysis,
            ILogger<GetTechnicalIndicatorsQueryHandler> logger)
        {
            _technicalAnalysis = technicalAnalysis;
            _logger = logger;
        }

        public async Task<Result<TechnicalIndicatorsDto>> Handle(
            GetTechnicalIndicatorsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var indicators = await _technicalAnalysis
                    .CalculateIndicatorsAsync(request.Symbol, request.Indicators);

                return Result<TechnicalIndicatorsDto>.SuccessResult(indicators);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating indicators for stock {Symbol}", request.Symbol);
                throw;
            }
        }
    }
}