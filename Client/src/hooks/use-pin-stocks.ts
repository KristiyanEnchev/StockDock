import { useCallback } from 'react';
import { useAppSelector, useAppDispatch } from '@/store/hooks';
import {
    selectPinnedStocks,
    togglePinStock,
    pinStock,
    unpinStock
} from '@/features/stocks/pinnedStocksSlice';
import { toast } from 'react-hot-toast';

export const usePinStocks = () => {
    const dispatch = useAppDispatch();
    const pinnedStocks = useAppSelector(selectPinnedStocks);

    const isPinned = useCallback((symbol: string) => {
        return pinnedStocks.includes(symbol);
    }, [pinnedStocks]);

    const togglePin = useCallback((symbol: string) => {
        dispatch(togglePinStock(symbol));
        const isCurrentlyPinned = pinnedStocks.includes(symbol);
        toast.success(`${symbol} ${isCurrentlyPinned ? 'unpinned' : 'pinned'} successfully`);
        return !isCurrentlyPinned;
    }, [dispatch, pinnedStocks]);

    const addPin = useCallback((symbol: string) => {
        if (!pinnedStocks.includes(symbol)) {
            dispatch(pinStock(symbol));
            toast.success(`${symbol} pinned successfully`);
            return true;
        }
        return false;
    }, [dispatch, pinnedStocks]);

    const removePin = useCallback((symbol: string) => {
        if (pinnedStocks.includes(symbol)) {
            dispatch(unpinStock(symbol));
            toast.success(`${symbol} unpinned successfully`);
            return true;
        }
        return false;
    }, [dispatch, pinnedStocks]);

    return {
        pinnedStocks,
        isPinned,
        togglePin,
        addPin,
        removePin
    };
};
