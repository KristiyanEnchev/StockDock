import { createApi } from '@reduxjs/toolkit/query/react';
import { StockDto, StockPriceHistoryDto } from '@/types/stockTypes';
import { baseQueryWithReauth } from '@/features/auth/baseQueryWithReauth';

export const stocksApi = createApi({
    reducerPath: 'stocksApi',
    baseQuery: baseQueryWithReauth,
    tagTypes: ['Stock', 'PopularStocks', 'StockHistory'],
    endpoints: (builder) => ({
        getStockBySymbol: builder.query<StockDto, string>({
            query: (symbol) => `stocks/${symbol}`,
            providesTags: (_result, _error, symbol) => [{ type: 'Stock', id: symbol }]
        }),
        searchStocks: builder.query<StockDto[], { query: string; sortBy?: string; ascending?: boolean }>({
            query: ({ query, sortBy, ascending = true }) => ({
                url: 'stocks/search',
                params: { query, sortBy, ascending }
            })
        }),
        getPopularStocks: builder.query<StockDto[], number>({
            query: (limit = 10) => ({
                url: 'stocks/popular',
                params: { limit }
            }),
            providesTags: ['PopularStocks']
        }),
        getStockHistory: builder.query<StockPriceHistoryDto[], { symbol: string; from: string; to: string }>({
            query: ({ symbol, from, to }) => ({
                url: `stocks/${symbol}/history`,
                params: { from, to }
            }),
            providesTags: (result, _error, { symbol }) =>
                result
                    ? [{ type: 'StockHistory', id: symbol }]
                    : [],
            keepUnusedDataFor: 300,
            transformResponse: (response: StockPriceHistoryDto[]) => {
                if (!response || !Array.isArray(response) || response.length === 0) {
                    console.warn('Empty or invalid response from getStockHistory API');
                    return [];
                }
                return response;
            },
            onQueryStarted: async (_, { queryFulfilled }) => {
                try {
                    await queryFulfilled;
                } catch (error) {
                    console.error('Error fetching stock history:', error);
                }
            }
        })
    })
});

export const {
    useGetStockBySymbolQuery,
    useSearchStocksQuery,
    useGetPopularStocksQuery,
    useGetStockHistoryQuery,
    usePrefetch
} = stocksApi;
