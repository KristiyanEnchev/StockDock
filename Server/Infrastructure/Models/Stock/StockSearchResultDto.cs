namespace Models.Stock
{
    using Domain.Entities.Stock;

    using Models;

    public class StockSearchResultDto : BaseDto<StockSearchResultDto, Stock>
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; 
        public string Region { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public float MatchScore { get; set; } 

        public override void CustomizeMapping(Mapster.TypeAdapterConfig config)
        {
            config.NewConfig<Stock, StockSearchResultDto>()
                .Map(dest => dest.Name, src => src.CompanyName)
                .Map(dest => dest.Type, src => "Equity")  
                .Map(dest => dest.MatchScore, src => 1.0f) 
                .Map(dest => dest.Currency, src => "USD"); 
        }
    }
}
