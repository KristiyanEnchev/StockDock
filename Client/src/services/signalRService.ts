import * as signalR from '@microsoft/signalr';
import { store } from '@/store';
import { updateStockPrice, updatePopularStocks } from '@/features/stocks/stocksSlice';
import {
    alertTriggered,
    alertCreated,
    alertDeleted,
    setUserAlerts
} from '@/features/alerts/alertsSlice';
import { setSignalRStatus } from '@/features/signalR/signalRSlice';
import { toast } from 'react-hot-toast';
import { AlertDto, AlertType } from '@/types/alertTypes';
let connection: signalR.HubConnection | null = null;
let connectionStatus: 'disconnected' | 'connecting' | 'connected' = 'disconnected';

export const getConnectionStatus = () => connectionStatus;

export const initializeSignalR = async (token?: string): Promise<void> => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        return Promise.resolve();
    }

    connectionStatus = 'connecting';
    store.dispatch(setSignalRStatus('connecting'));

    const hubConnectionBuilder = new signalR.HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/stock`, {
            ...(token ? {
                accessTokenFactory: () => token,
                headers: {
                    "Authorization": `Bearer ${token}`
                },
                transport: signalR.HttpTransportType.WebSockets
            } : {})
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 20000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection = hubConnectionBuilder;

    connection.on('ReceiveStockPriceUpdate', (stock) => {
        store.dispatch(updateStockPrice(stock));
    });

    connection.on('ReceivePopularStocksUpdate', (stocks) => {
        store.dispatch(updatePopularStocks(stocks));
    });

    connection.on('ReceiveAlertTriggered', (alert) => {
        store.dispatch(alertTriggered(alert));
        toast.success(`Alert triggered: ${alert.symbol} is now ${alert.type === AlertType.PriceAbove ? 'above' : 'below'} $${alert.threshold}`, {
            duration: 6000,
            icon: '🔔'
        });
    });

    connection.on('ReceiveAlertCreated', (alert) => {
        store.dispatch(alertCreated(alert));
    });

    connection.on('ReceiveAlertDeleted', (alertId) => {
        store.dispatch(alertDeleted(alertId));
    });

    connection.on('ReceiveUserAlerts', (alerts) => {
        store.dispatch(setUserAlerts(alerts));
    });

    connection.on('ReceiveError', (message) => {
        toast.error(message);
    });

    connection.onclose((_error) => {
        connectionStatus = 'disconnected';
        store.dispatch(setSignalRStatus('disconnected'));
    });

    try {
        await connection.start();
        connectionStatus = 'connected';
        store.dispatch(setSignalRStatus('connected'));

        if (token) {
            try {
                await connection.invoke('GetUserAlerts');
            } catch (err) {
            }
        }
    } catch (err) {
        connectionStatus = 'disconnected';
        store.dispatch(setSignalRStatus('disconnected'));
        throw err;
    }
};

export const stopSignalR = async (): Promise<void> => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        try {
            await connection.stop();
            connectionStatus = 'disconnected';
            store.dispatch(setSignalRStatus('disconnected'));
        } catch (err) {
            throw err;
        }
    }
};

export const subscribeToStock = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('SubscribeToStock', symbol);
    } catch (error) {
        throw error;
    }
};

export const unsubscribeFromStock = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('UnsubscribeFromStock', symbol);
    } catch (error) {
        throw error;
    }
};

export const addToWatchlist = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('AddToWatchlist', symbol);
    } catch (error) {
        throw error;
    }
};

export const removeFromWatchlist = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('RemoveFromWatchlist', symbol);
    } catch (error) {
        throw error;
    }
};

export const createAlert = async (symbol: string, type: AlertType, threshold: number): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('CreateAlert', symbol, type, threshold);
    } catch (error) {
        throw error;
    }
};

export const deleteAlert = async (alertId: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('DeleteAlert', alertId);
    } catch (error) {
        throw error;
    }
};

export const simulateAlertTrigger = (symbol: string, type: AlertType, threshold: number): void => {
    const state = store.getState();
    const userId = state.auth.user?.id || 'test-user';

    const alert: AlertDto = {
        id: `test-${Date.now()}`,
        userId: userId,
        stockId: `stock-${symbol}`,
        symbol,
        type,
        threshold,
        isTriggered: true,
        createdAt: new Date().toISOString(),
        lastTriggeredAt: new Date().toISOString()
    };

    store.dispatch(alertTriggered(alert));
    toast.success(`Alert triggered: ${symbol} is now ${type === AlertType.PriceAbove ? 'above' : 'below'} $${threshold}`, {
        duration: 6000,
        icon: '🔔'
    });
};
