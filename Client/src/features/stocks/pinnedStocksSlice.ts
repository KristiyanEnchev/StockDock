import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '@/store';

interface PinnedStocksState {
    pinnedStocks: string[];
}

const initialState: PinnedStocksState = {
    pinnedStocks: [],
};

export const pinnedStocksSlice = createSlice({
    name: 'pinnedStocks',
    initialState,
    reducers: {
        togglePinStock: (state, action: PayloadAction<string>) => {
            const stockSymbol = action.payload;
            const index = state.pinnedStocks.indexOf(stockSymbol);

            if (index === -1) {
                state.pinnedStocks.push(stockSymbol);
            } else {
                state.pinnedStocks.splice(index, 1);
            }
        },
        pinStock: (state, action: PayloadAction<string>) => {
            const stockSymbol = action.payload;
            if (!state.pinnedStocks.includes(stockSymbol)) {
                state.pinnedStocks.push(stockSymbol);
            }
        },
        unpinStock: (state, action: PayloadAction<string>) => {
            const stockSymbol = action.payload;
            const index = state.pinnedStocks.indexOf(stockSymbol);
            if (index !== -1) {
                state.pinnedStocks.splice(index, 1);
            }
        },
        clearPinnedStocks: (state) => {
            state.pinnedStocks = [];
        },
    },
});

export const { togglePinStock, pinStock, unpinStock, clearPinnedStocks } = pinnedStocksSlice.actions;

export const selectPinnedStocks = (state: RootState) => state.pinnedStocks.pinnedStocks;
export const selectIsPinned = (state: RootState, symbol: string) =>
    state.pinnedStocks.pinnedStocks.includes(symbol);

export default pinnedStocksSlice.reducer;
