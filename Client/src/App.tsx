import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import AppRoutes from './routes';

function App() {
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
