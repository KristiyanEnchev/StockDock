import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import AppRoutes from './routes';
import { useEffect } from 'react';
import { initializeTheme } from './lib/theme';
import { Provider } from 'react-redux';
import { PersistGate } from 'redux-persist/integration/react';
import { store, persistor } from './store';
import SignalRInitializer from './services/SignalRInitializer';
import ConnectionStatus from './components/common/ConnectionStatus';

function App() {
  useEffect(() => {
    initializeTheme();
  }, []);

  return (
    <Provider store={store}>
      <PersistGate loading={null} persistor={persistor}>
        <BrowserRouter>
          <SignalRInitializer />
          <ConnectionStatus />
          <AppRoutes />
          <Toaster
            position="bottom-right"
            toastOptions={{
              duration: 3000,
            }}
          />
        </BrowserRouter>
      </PersistGate>
    </Provider>
  );
}

export default App;
