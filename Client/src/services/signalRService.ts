import * as signalR from '@microsoft/signalr';

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
