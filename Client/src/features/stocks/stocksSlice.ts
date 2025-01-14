import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { StockDto } from '@/types/stockTypes';
import { RootState } from '@/store';

interface StocksState {
    liveStocks: Record<string, StockDto>;
    popularStocks: StockDto[];
}

const initialState: StocksState = {
    liveStocks: {},
    popularStocks: []
};

export const stocksSlice = createSlice({
    name: 'stocks',
    initialState,
    reducers: {
        updateStockPrice: (state, action: PayloadAction<StockDto>) => {
            const stock = action.payload;
            state.liveStocks[stock.symbol] = {
                ...state.liveStocks[stock.symbol],
                ...stock
            };
        },
        updatePopularStocks: (state, action: PayloadAction<StockDto[]>) => {
            state.popularStocks = action.payload;
            action.payload.forEach(stock => {
                state.liveStocks[stock.symbol] = {
                    ...state.liveStocks[stock.symbol],
                    ...stock
                };
            });
        },
        clearStocks: (state) => {
            state.liveStocks = {};
            state.popularStocks = [];
        }
    }
});

export const { updateStockPrice, updatePopularStocks, clearStocks } = stocksSlice.actions;

export const selectLiveStock = (state: RootState, symbol: string) => state.stocks.liveStocks[symbol];
export const selectAllLiveStocks = (state: RootState) => Object.values(state.stocks.liveStocks);
export const selectPopularStocks = (state: RootState) => state.stocks.popularStocks;

export default stocksSlice.reducer;
