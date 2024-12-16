import { configureStore, isRejectedWithValue, Middleware, combineReducers } from '@reduxjs/toolkit';
import { persistStore, FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER } from 'redux-persist';
import toast from 'react-hot-toast';

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
            rtkQueryErrorLogger
        ),
    devTools: process.env.NODE_ENV !== 'production',
});

export const persistor = persistStore(store);

export type AppDispatch = typeof store.dispatch;
