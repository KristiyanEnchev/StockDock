import { createApi } from '@reduxjs/toolkit/query/react';
import { StockDto } from '@/types/stockTypes';
import { baseQueryWithReauth } from '@/features/auth/baseQueryWithReauth';

export const watchlistApi = createApi({
    reducerPath: 'watchlistApi',
    baseQuery: baseQueryWithReauth,
    tagTypes: ['Watchlist'],
    endpoints: (builder) => ({
        getWatchlist: builder.query<StockDto[], void>({
            query: () => 'watchlist',
            providesTags: ['Watchlist']
        }),
        addToWatchlist: builder.mutation<void, string>({
            query: (symbol) => ({
                url: `watchlist/${symbol}`,
                method: 'POST'
            }),
            invalidatesTags: ['Watchlist']
        }),
        removeFromWatchlist: builder.mutation<void, string>({
            query: (symbol) => ({
                url: `watchlist/${symbol}`,
                method: 'DELETE'
            }),
            invalidatesTags: ['Watchlist']
        })
    })
});

export const {
    useGetWatchlistQuery,
    useAddToWatchlistMutation,
    useRemoveFromWatchlistMutation
} = watchlistApi;
