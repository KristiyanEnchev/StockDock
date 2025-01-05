export interface StockDto {
    symbol: string;
    companyName: string;
    currentPrice: number;
    previousClose?: number;
    change?: number;
    changePercent?: number;
    volume?: number;
    marketCap?: number;
    peRatio?: number;
    sector?: string;
    industry?: string;
}

export interface StockPriceHistoryDto {
    date: string;
    open: number;
    high: number;
    low: number;
    close: number;
    volume: number;
}
