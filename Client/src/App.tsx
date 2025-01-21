import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import AppRoutes from './routes';
import { useEffect } from 'react';
import { initializeTheme } from './lib/theme';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from './store';
import { useAppSelector } from './store/hooks';
import { selectIsAuthenticated, selectToken } from './features/auth/authSlice';
import { initializeSignalR } from './services/signalRService';

function App() {
  useEffect(() => {
    initializeTheme();
  }, []);

  return (
    <Provider store={store}>
      <PersistGate loading={null} persistor={persistor}>
        <BrowserRouter>
          <SignalRInitializer />
          <AppRoutes />
          <Toaster
            position="bottom-right"
            toastOptions={{
              className: 'dark:bg-library-dark-paper dark:text-library-dark-text-primary',
              duration: 3000,
            }}
          />
        </BrowserRouter>
      </PersistGate>
    </Provider>
  );
}

function SignalRInitializer() {
  const token = useAppSelector(selectToken);
  const isAuthenticated = useAppSelector(selectIsAuthenticated);

  useEffect(() => {
    initializeSignalR(token || undefined)
      .then(() => console.log('SignalR connection established'))
      .catch(err => console.error('Failed to initialize SignalR:', err));
  }, [token, isAuthenticated]);

  return null;
}

export default App;
