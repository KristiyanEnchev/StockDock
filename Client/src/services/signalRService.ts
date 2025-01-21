import * as signalR from '@microsoft/signalr';
import { store } from '@/store';
import { updateStockPrice, updatePopularStocks } from '@/features/stocks/stocksSlice';
import {
    alertTriggered,
    alertCreated,
    alertDeleted,
    setUserAlerts
} from '@/features/alerts/alertsSlice';
import { toast } from 'react-hot-toast';
import { AlertType } from '@/types/alertTypes';

let connection: signalR.HubConnection | null = null;

export const initializeSignalR = (token?: string): Promise<void> => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        return Promise.resolve();
    }

    const hubConnectionBuilder = new signalR.HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/stock`, {
            ...(token ? { accessTokenFactory: () => token } : {})
        })
        .withAutomaticReconnect()
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
        toast.success(`Alert triggered for ${alert.symbol} - ${alert.type} at ${alert.threshold}`);
    });

    connection.on('ReceiveAlertCreated', (alert) => {
        store.dispatch(alertCreated(alert));
        toast.success(`Alert created for ${alert.symbol}`);
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

    connection.onclose(() => {
        console.log('SignalR connection closed');
    });

    return startConnection();
};

const startConnection = (): Promise<void> => {
    if (!connection) {
        return Promise.reject(new Error('Connection not initialized'));
    }

    return connection.start().catch(err => {
        console.error('Error starting SignalR connection:', err);
        return new Promise<void>((resolve) => {
            setTimeout(() => startConnection().then(resolve), 5000);
        });
    });
};

export const stopConnection = (): Promise<void> => {
    if (!connection) {
        return Promise.resolve();
    }

    return connection.stop().then(() => {
        connection = null;
    });
};

export const subscribeToStock = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('SubscribeToStock', symbol);
    } catch (error) {
        console.error(`Error subscribing to stock ${symbol}:`, error);
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
        console.error(`Error unsubscribing from stock ${symbol}:`, error);
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
        console.error(`Error adding ${symbol} to watchlist:`, error);
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
        console.error(`Error removing ${symbol} from watchlist:`, error);
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
        console.error(`Error creating alert for ${symbol}:`, error);
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
        console.error(`Error deleting alert ${alertId}:`, error);
        throw error;
    }
};

export const getUserAlerts = async (): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        throw new Error('Connection not initialized or not connected');
    }

    try {
        await connection.invoke('GetUserAlerts');
    } catch (error) {
        console.error('Error getting user alerts:', error);
        throw error;
    }
};
