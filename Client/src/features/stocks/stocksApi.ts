import { createApi } from '@reduxjs/toolkit/query/react';
import { StockDto, StockPriceHistoryDto } from '../../types/stockTypes';
import { baseQueryWithReauth } from '@/features/auth/baseQueryWithReauth';

export const stocksApi = createApi({
    reducerPath: 'stocksApi',
    baseQuery: baseQueryWithReauth,
    tagTypes: ['Stock', 'PopularStocks'],
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
        getStockHistory: builder.query<StockPriceHistoryDto[], { symbol: string; from: Date; to: Date }>({
            query: ({ symbol, from, to }) => ({
                url: `stocks/${symbol}/history`,
                params: { from: from.toISOString(), to: to.toISOString() }
            })
        })
    })
});

export const {
    useGetStockBySymbolQuery,
    useSearchStocksQuery,
    useGetPopularStocksQuery,
    useGetStockHistoryQuery
} = stocksApi;
