import { useState, useCallback, useEffect, useMemo } from 'react';
import { useGetWatchlistQuery, useRemoveFromWatchlistMutation, useAddToWatchlistMutation } from '@/features/watchlist/watchlistApi';
import { PopularStocksSidebar } from '@/components/sidebar/Sidebar';
import { StockCard } from '@/components/stock/StockCard';
import { WidgetGrid } from '@/components/stock/WidgetGrid';
import { subscribeToStock, unsubscribeFromStock } from '@/services/signalRService';
import { AlertForm } from '@/components/stock/AlertForm';
import { useAppSelector } from '@/store/hooks';
import { selectIsAuthenticated } from '@/features/auth/authSlice';
import { usePinStocks } from '@/hooks/use-pin-stocks';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-hot-toast';
import { LineChart } from 'lucide-react';

const Watchlist = () => {
    const { data: watchlistStocks, isLoading, error } = useGetWatchlistQuery();
    const [removeFromWatchlistMutation] = useRemoveFromWatchlistMutation();
    const [addToWatchlistMutation] = useAddToWatchlistMutation();
    const isAuthenticated = useAppSelector(selectIsAuthenticated);
    const { pinnedStocks, togglePin } = usePinStocks();
    const navigate = useNavigate();

    const [selectedStock, setSelectedStock] = useState<string | null>(null);
    const [showAlertForm, setShowAlertForm] = useState(false);
    const [addedStocks, setAddedStocks] = useState<string[]>([]);

    useEffect(() => {
        if (watchlistStocks) {
            setAddedStocks(watchlistStocks.map(stock => stock.symbol));
        }
    }, [watchlistStocks]);

    const handleAddStock = useCallback(async (stock: { symbol: string; name: string; change: number }) => {
        if (!isAuthenticated) {
            toast.error('Please log in to add stocks to your watchlist');
            navigate('/login');
            return;
        }

        try {
            await addToWatchlistMutation(stock.symbol);
            await subscribeToStock(stock.symbol);
            setAddedStocks(prev => [...prev, stock.symbol]);
            toast.success(`${stock.symbol} added to your watchlist`);
        } catch (error) {
            toast.error(`Failed to add ${stock.symbol} to watchlist`);
            console.error('Error adding to watchlist:', error);
        }
    }, [isAuthenticated, navigate, addToWatchlistMutation]);

    const handleRemoveStock = useCallback(async (symbol: string) => {
        try {
            await removeFromWatchlistMutation(symbol);
            await unsubscribeFromStock(symbol);
            setAddedStocks(prev => prev.filter(s => s !== symbol));
            toast.success(`${symbol} removed from your watchlist`);

            if (selectedStock === symbol) {
                setSelectedStock(null);
                setShowAlertForm(false);
            }
        } catch (error) {
            toast.error(`Failed to remove ${symbol} from watchlist`);
            console.error('Error removing from watchlist:', error);
        }
    }, [removeFromWatchlistMutation, selectedStock]);

    const handleTogglePin = useCallback((symbol: string) => {
        togglePin(symbol);
    }, [togglePin]);

    const handleShowChart = useCallback((symbol: string) => {
        setSelectedStock(symbol);
    }, []);

    const handleShowAlerts = useCallback((symbol: string) => {
        setSelectedStock(symbol);
        setShowAlertForm(true);
    }, []);

    const sortedWatchlistStocks = useMemo(() => {
        if (!watchlistStocks) return [];

        const pinnedWatchlistStocks = watchlistStocks.filter(stock =>
            pinnedStocks.includes(stock.symbol)
        );

        const nonPinnedWatchlistStocks = watchlistStocks.filter(stock =>
            !pinnedStocks.includes(stock.symbol)
        );

        return [...pinnedWatchlistStocks, ...nonPinnedWatchlistStocks];
    }, [watchlistStocks, pinnedStocks]);

    if (!isAuthenticated) {
        return <></>;
    }

    return (
        <div className="min-h-screen bg-light-bg dark:bg-dark-bg pt-6 px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                <div className="flex flex-col md:flex-row gap-6">
                    <div className="md:w-64 flex-shrink-0">
                        <PopularStocksSidebar
                            onAddStock={handleAddStock}
                            addedStocks={addedStocks}
                        />
                    </div>

                    <div className="flex-1">
                        <div className="mb-6">
                            <h1 className="text-2xl font-bold text-light-text dark:text-dark-text mb-4">
                                My Watchlist
                            </h1>

                            {isLoading ? (
                                <div className="flex justify-center items-center h-64 bg-light-card dark:bg-dark-card rounded-lg">
                                    <div className="text-center">
                                        <LineChart className="h-10 w-10 text-light-text dark:text-dark-text opacity-40 mx-auto mb-2" />
                                        <p className="text-light-text dark:text-dark-text opacity-60">
                                            Loading your watchlist...
                                        </p>
                                    </div>
                                </div>
                            ) : error ? (
                                <div className="p-4 bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400 rounded-lg">
                                    <p>Error loading watchlist. Please try again later.</p>
                                </div>
                            ) : sortedWatchlistStocks.length > 0 ? (
                                <div>
                                    {selectedStock && showAlertForm && (
                                        <AlertForm
                                            symbol={selectedStock}
                                            showAlertForm={showAlertForm}
                                            setShowAlertForm={setShowAlertForm}
                                        />
                                    )}

                                    <WidgetGrid>
                                        {sortedWatchlistStocks.map(stock => {
                                            const isPinned = pinnedStocks.includes(stock.symbol);
                                            return (
                                                <StockCard
                                                    key={stock.symbol}
                                                    symbol={stock.symbol}
                                                    companyName={stock.companyName}
                                                    currentPrice={stock.currentPrice}
                                                    previousClose={stock.previousClose}
                                                    change={stock.change}
                                                    changePercent={stock.changePercent}
                                                    volume={stock.volume}
                                                    isPinned={isPinned}
                                                    onRemove={() => handleRemoveStock(stock.symbol)}
                                                    onTogglePin={() => handleTogglePin(stock.symbol)}
                                                    onShowChart={() => handleShowChart(stock.symbol)}
                                                    onShowAlerts={() => handleShowAlerts(stock.symbol)}
                                                />
                                            );
                                        })}
                                    </WidgetGrid>
                                </div>
                            ) : (
                                <div className="flex flex-col items-center justify-center h-64 bg-light-card dark:bg-dark-card rounded-lg p-6">
                                    <LineChart className="h-12 w-12 text-light-text dark:text-dark-text opacity-40 mb-4" />
                                    <p className="text-light-text dark:text-dark-text text-lg mb-2">
                                        Your watchlist is empty
                                    </p>
                                    <p className="text-light-text dark:text-dark-text opacity-60 text-center mb-4">
                                        Add stocks from the sidebar to start tracking them in real-time
                                    </p>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Watchlist;
