import { configureStore, isRejectedWithValue, Middleware, combineReducers } from '@reduxjs/toolkit';
import { persistStore, FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER } from 'redux-persist';
import storage from 'redux-persist/lib/storage';
import toast from 'react-hot-toast';
import persistReducer from 'redux-persist/es/persistReducer';

import themeReducer from '../features/theme/themeSlice';
import authReducer from '../features/auth/authSlice';
import stocksReducer from '../features/stocks/stocksSlice';
import pinnedStocksReducer from '../features/stocks/pinnedStocksSlice';
import alertsReducer from '../features/alerts/alertsSlice';
import signalRReducer from '../features/signalR/signalRSlice';

import { authApi } from '../features/auth/authApi';
import { stocksApi } from '../features/stocks/stocksApi';
import { watchlistApi } from '../features/watchlist/watchlistApi';
import { alertsApi } from '../features/alerts/alertsApi';
import { usersApi } from '@/features/auth/usersApi';

const themePersistConfig = {
    key: 'theme',
    storage,
    whitelist: ['isDark']
};

const authPersistConfig = {
    key: 'auth',
    storage,
    whitelist: ['user', 'token', 'refreshToken', 'refreshTokenExpiryTime']
};

const alertsPersistConfig = {
    key: 'alerts',
    storage,
    whitelist: ['triggeredAlerts']
};

const pinnedStocksPersistConfig = {
    key: 'pinnedStocks',
    storage,
    whitelist: ['pinnedStocks']
};

const rtkQueryErrorLogger: Middleware = () => (next) => (action) => {
    if (isRejectedWithValue(action)) {
        const payload = action.payload as { status?: number; data?: { message?: string } };
        if (payload.status !== 401) {
            console.error('API Error:', payload);
            toast.error(payload.data?.message || 'An error occurred');
        }
    }
    return next(action);
};

const rootReducer = combineReducers({
    auth: persistReducer(authPersistConfig, authReducer),
    theme: persistReducer(themePersistConfig, themeReducer),
    stocks: stocksReducer,
    pinnedStocks: persistReducer(pinnedStocksPersistConfig, pinnedStocksReducer),
    alerts: persistReducer(alertsPersistConfig, alertsReducer),
    signalR: signalRReducer,
    [authApi.reducerPath]: authApi.reducer,
    [stocksApi.reducerPath]: stocksApi.reducer,
    [watchlistApi.reducerPath]: watchlistApi.reducer,
    [alertsApi.reducerPath]: alertsApi.reducer,
    [usersApi.reducerPath]: usersApi.reducer,
});

export type RootState = ReturnType<typeof rootReducer>;

export const store = configureStore({
    reducer: rootReducer,
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({
            serializableCheck: {
                ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
            },
        }).concat(
            authApi.middleware,
            stocksApi.middleware,
            watchlistApi.middleware,
            alertsApi.middleware,
            usersApi.middleware,
            rtkQueryErrorLogger
        ),
    devTools: process.env.NODE_ENV !== 'production',
});

export const persistor = persistStore(store);

export type AppDispatch = typeof store.dispatch;
