import { useState, useCallback, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { LineChart, ArrowUpCircle, Lock, Pin, Bell, TrendingUp, BarChart3, Activity, Clock, Zap } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useAppSelector } from '@/store/hooks';
import { selectPopularStocks } from '@/features/stocks/stocksSlice';
import { StockCard } from '@/components/stock/StockCard';
import { WidgetGrid } from '@/components/stock/WidgetGrid';
import { selectIsAuthenticated } from '@/features/auth/authSlice';
import { usePinStocks } from '@/hooks/use-pin-stocks';

const Home = () => {
    const navigate = useNavigate();
    const popularStocks = useAppSelector(selectPopularStocks);
    const isAuthenticated = useAppSelector(selectIsAuthenticated);
    const { pinnedStocks, togglePin } = usePinStocks();
    const [showLoginPrompt, setShowLoginPrompt] = useState(false);
    const [_selectedStock, setSelectedStock] = useState<string | null>(null);

    const handleActionPrompt = () => {
        if (!isAuthenticated) {
            setShowLoginPrompt(true);
            setTimeout(() => setShowLoginPrompt(false), 3000);
        }
    };

    const handleTogglePin = useCallback((symbol: string) => {
        if (!isAuthenticated) {
            handleActionPrompt();
            return;
        }

        togglePin(symbol);
    }, [isAuthenticated, togglePin]);

    const handleShowChart = useCallback((symbol: string) => {
        setSelectedStock(symbol);
    }, []);

    const handleShowAlerts = useCallback((_symbol: string) => {
        if (!isAuthenticated) {
            handleActionPrompt();
            return;
        }
        navigate('/watchlist');
    }, [isAuthenticated, navigate]);

    const handleRemove = useCallback(() => {
        if (!isAuthenticated) {
            handleActionPrompt();
            return;
        }
        navigate('/watchlist');
    }, [isAuthenticated, navigate]);

    const sortedStocks = useMemo(() => {
        const pinnedPopularStocks = popularStocks.filter(stock =>
            pinnedStocks.includes(stock.symbol)
        );

        const nonPinnedPopularStocks = popularStocks.filter(stock =>
            !pinnedStocks.includes(stock.symbol)
        );

        return [...pinnedPopularStocks, ...nonPinnedPopularStocks].slice(0, 6);
    }, [popularStocks, pinnedStocks]);

    return (
        <div className="min-h-screen bg-light-bg dark:bg-dark-bg pt-6 px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                <div className="mb-8">
                    <div className="flex justify-between items-center mb-4">
                        <h2 className="text-xl font-semibold text-light-text dark:text-dark-text">
                            Popular Stocks
                        </h2>
                        <div className="flex items-center gap-2 text-light-text dark:text-dark-text opacity-80">
                            <ArrowUpCircle className="h-4 w-4 text-green-600 dark:text-green-400" />
                            <span className="text-sm">Real-time updates</span>
                        </div>
                    </div>

                    {sortedStocks.length > 0 ? (
                        <WidgetGrid>
                            {sortedStocks.map(stock => {
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
                                        onRemove={handleRemove}
                                        onTogglePin={() => handleTogglePin(stock.symbol)}
                                        onShowChart={() => handleShowChart(stock.symbol)}
                                        onShowAlerts={() => handleShowAlerts(stock.symbol)}
                                    />
                                );
                            })}
                        </WidgetGrid>
                    ) : (
                        <div className="flex justify-center items-center h-64 bg-light-card dark:bg-dark-card rounded-lg">
                            <div className="text-center">
                                <LineChart className="h-10 w-10 text-light-text dark:text-dark-text opacity-40 mx-auto mb-2" />
                                <p className="text-light-text dark:text-dark-text opacity-60">
                                    Loading popular stocks...
                                </p>
                            </div>
                        </div>
                    )}
                </div>

                {isAuthenticated ? (
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
                        <div className="bg-light-card dark:bg-dark-card rounded-lg p-6">
                            <div className="flex items-center justify-between mb-4">
                                <h3 className="text-lg font-semibold text-light-text dark:text-dark-text flex items-center gap-2">
                                    <BarChart3 className="h-5 w-5 text-green-600 dark:text-green-400" />
                                    Market Overview
                                </h3>
                                <span className="text-xs text-light-text dark:text-dark-text opacity-60">
                                    Last updated: {new Date().toLocaleTimeString()}
                                </span>
                            </div>
                            <div className="space-y-4">
                                {marketIndices.map((index) => (
                                    <div key={index.name} className="flex justify-between items-center border-b border-gray-700 pb-2">
                                        <div>
                                            <p className="font-medium text-light-text dark:text-dark-text">{index.name}</p>
                                            <p className="text-sm text-light-text dark:text-dark-text opacity-60">{index.value}</p>
                                        </div>
                                        <div className={`text-sm font-medium ${index.change >= 0 ? 'text-green-500' : 'text-red-500'}`}>
                                            {index.change >= 0 ? '+' : ''}{index.change}%
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <button
                                className="mt-4 text-sm text-green-600 dark:text-green-400 font-medium hover:underline flex items-center gap-1"
                                onClick={() => navigate('/watchlist')}
                            >
                                View more <ArrowUpCircle className="h-3 w-3 rotate-90" />
                            </button>
                        </div>

                        <div className="bg-light-card dark:bg-dark-card rounded-lg p-6">
                            <div className="flex items-center justify-between mb-4">
                                <h3 className="text-lg font-semibold text-light-text dark:text-dark-text flex items-center gap-2">
                                    <Activity className="h-5 w-5 text-green-600 dark:text-green-400" />
                                    Recent Activity
                                </h3>
                            </div>
                            <div className="space-y-4">
                                {recentActivities.map((activity, index) => (
                                    <div key={index} className="flex gap-3 border-b border-gray-700 pb-3">
                                        <div className="mt-1">
                                            <activity.icon className="h-5 w-5 text-green-600 dark:text-green-400" />
                                        </div>
                                        <div>
                                            <p className="text-light-text dark:text-dark-text">{activity.description}</p>
                                            <p className="text-xs text-light-text dark:text-dark-text opacity-60 flex items-center gap-1">
                                                <Clock className="h-3 w-3" /> {activity.time}
                                            </p>
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <button
                                className="mt-4 text-sm text-green-600 dark:text-green-400 font-medium hover:underline flex items-center gap-1"
                                onClick={() => navigate('/profile')}
                            >
                                View all activity <ArrowUpCircle className="h-3 w-3 rotate-90" />
                            </button>
                        </div>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                        {premiumFeatures.map((feature, index) => (
                            <motion.div
                                key={index}
                                className="bg-light-card dark:bg-dark-card rounded-lg p-6 relative overflow-hidden"
                                whileHover={{ y: -5 }}
                                transition={{ duration: 0.2 }}
                            >
                                <div className="absolute top-0 right-0 m-2">
                                    <Lock className="h-4 w-4 text-light-text dark:text-dark-text opacity-40" />
                                </div>
                                <feature.icon className="h-8 w-8 text-green-600 dark:text-green-400 mb-3" />
                                <h3 className="text-lg font-semibold text-light-text dark:text-dark-text mb-2">
                                    {feature.title}
                                </h3>
                                <p className="text-light-text dark:text-dark-text opacity-80 text-sm">
                                    {feature.description}
                                </p>
                                <button
                                    onClick={() => navigate('/login')}
                                    className="mt-4 text-green-600 dark:text-green-400 text-sm font-medium hover:text-green-700 dark:hover:text-green-300 transition"
                                >
                                    Sign up to unlock →
                                </button>
                            </motion.div>
                        ))}
                    </div>
                )}
            </div>

            <AnimatePresence>
                {showLoginPrompt && (
                    <motion.div
                        className="fixed bottom-4 right-4 bg-dark-bg text-dark-text p-4 rounded-lg shadow-lg flex items-center gap-3"
                        initial={{ opacity: 0, y: 50 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: 50 }}
                    >
                        <Lock className="h-5 w-5 text-amber-500" />
                        <p>Please sign in to use this feature</p>
                        <button
                            onClick={() => navigate('/login')}
                            className="ml-2 px-3 py-1 bg-green-600 text-white text-sm rounded"
                        >
                            Sign In
                        </button>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};

const marketIndices = [
    { name: 'S&P 500', value: '4,893.13', change: 0.57 },
    { name: 'Nasdaq', value: '15,628.95', change: 1.24 },
    { name: 'Dow Jones', value: '38,239.66', change: -0.18 },
    { name: 'Russell 2000', value: '2,081.55', change: 0.32 }
];

const recentActivities = [
    {
        description: 'AAPL price alert triggered at $190.00',
        time: '15 minutes ago',
        icon: Bell
    },
    {
        description: 'Added MSFT to your watchlist',
        time: '2 hours ago',
        icon: Pin
    },
    {
        description: 'Set new price alert for NVDA at $950.00',
        time: '1 day ago',
        icon: Bell
    },
    {
        description: 'Market summary: Tech sector up 2.3%',
        time: '2 days ago',
        icon: Zap
    }
];

const premiumFeatures = [
    {
        title: 'Personalized Watchlists',
        description: 'Create and customize watchlists to track your favorite stocks with real-time updates.',
        icon: Pin
    },
    {
        title: 'Price Alerts',
        description: 'Set up alerts to notify you when stocks reach your specified price targets.',
        icon: Bell
    },
    {
        title: 'Advanced Analytics',
        description: 'Get detailed technical analysis and performance metrics for smarter trading decisions.',
        icon: TrendingUp
    }
];

export default Home;
