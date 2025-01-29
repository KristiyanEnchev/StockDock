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

export const initializeSignalR = async (token?: string): Promise<void> => {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        console.log('SignalR already connected, reusing connection');
        return Promise.resolve();
    }

    console.log(`Initializing SignalR with auth token: ${token ? 'Present' : 'Not present'}`);

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
        console.log(`Received stock update for ${stock.symbol}: $${stock.currentPrice}`);
        store.dispatch(updateStockPrice(stock));
    });

    connection.on('ReceivePopularStocksUpdate', (stocks) => {
        console.log(`Received popular stocks update: ${stocks.length} stocks`);
        store.dispatch(updatePopularStocks(stocks));
    });

    connection.onclose((error) => {
        console.log('SignalR connection closed', error);
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

    connection.onreconnecting((error) => {
        console.log('SignalR reconnecting:', error);
    });

    connection.onreconnected((connectionId) => {
        console.log('SignalR reconnected with connection ID:', connectionId);
    });

    return startConnection();
};

const startConnection = (): Promise<void> => {
    if (!connection) {
        return Promise.reject(new Error('Connection not initialized'));
    }

    console.log('Starting SignalR connection...');
    return connection.start()
        .then(() => {
            console.log('SignalR connection started successfully');
        })
        .catch(err => {
            console.error('Error starting SignalR connection:', err);
            return new Promise<void>((resolve, reject) => {
                setTimeout(() => {
                    console.log('Retrying SignalR connection...');
                    startConnection()
                        .then(resolve)
                        .catch(reject);
                }, 5000);
            });
        });
};

export const stopConnection = async (): Promise<void> => {
    if (!connection) {
        return Promise.resolve();
    }

    console.log('Stopping SignalR connection...');
    try {
        await connection.stop();
        console.log('SignalR connection stopped successfully');
        connection = null;
        return Promise.resolve();
    } catch (error) {
        console.error('Error stopping SignalR connection:', error);
        connection = null;
        return Promise.reject(error);
    }
};

export const subscribeToStock = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        console.warn(`Cannot subscribe to ${symbol} - connection not active`);
        throw new Error('SignalR connection not established');
    }

    try {
        console.log(`Subscribing to stock ${symbol}...`);
        await connection.invoke('SubscribeToStock', symbol);
        console.log(`Successfully subscribed to stock ${symbol}`);
    } catch (error) {
        console.error(`Error subscribing to stock ${symbol}:`, error);
        throw error;
    }
};

export const unsubscribeFromStock = async (symbol: string): Promise<void> => {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        console.warn(`Cannot unsubscribe from ${symbol} - connection not active`);
        return;
    }

    try {
        console.log(`Unsubscribing from stock ${symbol}...`);
        await connection.invoke('UnsubscribeFromStock', symbol);
        console.log(`Successfully unsubscribed from stock ${symbol}`);
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
