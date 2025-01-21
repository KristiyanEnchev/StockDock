import { useEffect } from 'react';
import { useAppSelector } from '@/store/hooks';
import { selectIsAuthenticated, selectToken } from '@/features/auth/authSlice';
import { initializeSignalR, stopConnection } from '@/services/signalRService';

function SignalRInitializer() {
    const token = useAppSelector(selectToken);
    const isAuthenticated = useAppSelector(selectIsAuthenticated);

    useEffect(() => {
        const setupSignalR = async () => {
            try {
                await stopConnection();
                await initializeSignalR(isAuthenticated ? token! : undefined);
                console.log('SignalR connection established');
            } catch (err) {
                console.error('Failed to initialize SignalR:', err);
            }
        };

        setupSignalR();

        return () => {
            stopConnection()
                .catch(err => console.error('Error stopping SignalR connection:', err));
        };
    }, [token, isAuthenticated]);

    return null;
}

export default SignalRInitializer;
