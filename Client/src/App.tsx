import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import AppRoutes from './routes';
import { useAppDispatch } from './store/hooks';
import { useEffect } from 'react';
import { setTheme } from './features/theme/themeSlice';
import { initializeTheme } from './lib/theme';

function App() {
  const dispatch = useAppDispatch();

  useEffect(() => {
    const isDark = initializeTheme();
    dispatch(setTheme(isDark));
  }, [dispatch]);

  return (
    <BrowserRouter>
      <AppRoutes />
      <Toaster
        position="bottom-right"
        toastOptions={{
          className: 'dark:bg-library-dark-paper dark:text-library-dark-text-primary',
          duration: 3000,
        }}
      />
    </BrowserRouter>
  );
}

export default App;
