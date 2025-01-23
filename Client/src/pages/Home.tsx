import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { LineChart, ArrowUpCircle, Lock, Pin, Bell, TrendingUp } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useAppSelector } from '@/store/hooks';
import { selectPopularStocks } from '@/features/stocks/stocksSlice';
import { StockCard } from '@/components/stock/StockCard';
import { WidgetGrid } from '@/components/stock/WidgetGrid';

const Home = () => {
    const navigate = useNavigate();
    const popularStocks = useAppSelector(selectPopularStocks);
    const [showLoginPrompt, setShowLoginPrompt] = useState(false);

    const handleActionPrompt = () => {
        setShowLoginPrompt(true);
        setTimeout(() => setShowLoginPrompt(false), 3000);
    };

    const emptyHandler = () => handleActionPrompt();
    const displayStocks = popularStocks.slice(0, 6);

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


                </div>

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
