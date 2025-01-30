import { useState, useEffect, memo } from "react";
import { Bell, Pin, PinOff, X, TrendingUp, TrendingDown } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { cn } from "@/lib/utils";
import { useAppSelector } from "@/store/hooks";
import { selectLiveStock } from "@/features/stocks/stocksSlice";
import { selectUserAlerts } from "@/features/alerts/alertsSlice";

interface StockCardProps {
    symbol: string;
    companyName: string;
    currentPrice: number;
    previousClose?: number;
    change?: number;
    changePercent?: number;
    volume?: number;
    isPinned?: boolean;
    onRemove: () => void;
    onTogglePin: () => void;
    onShowChart: (symbol: string) => void;
    onShowAlerts: (symbol: string) => void;
}

export const StockCard = memo(({
    symbol,
    companyName,
    currentPrice,
    previousClose,
    change,
    changePercent,
    volume,
    isPinned,
    onRemove,
    onTogglePin,
    onShowChart,
    onShowAlerts,
}: StockCardProps) => {
    const [lastUpdateTime, setLastUpdateTime] = useState(new Date());
    const [priceFlash, setPriceFlash] = useState<'up' | 'down' | null>(null);
    const [prevPrice, setPrevPrice] = useState(currentPrice);

    const liveStock = useAppSelector(state => selectLiveStock(state, symbol));
    const alerts = useAppSelector(selectUserAlerts);

    useEffect(() => {
        if (liveStock) {
            const newPrice = liveStock.currentPrice;

            // Only flash if the price has actually changed
            if (newPrice !== prevPrice) {
                setPriceFlash(newPrice > prevPrice ? 'up' : 'down');
                setPrevPrice(newPrice);

                // Reset the flash after 1 second
                const timer = setTimeout(() => setPriceFlash(null), 1000);
                return () => clearTimeout(timer);
            }

            setLastUpdateTime(new Date());
        }
    }, [liveStock, prevPrice]);

    const hasActiveAlerts = alerts.some(alert =>
        alert.symbol === symbol && !alert.isTriggered
    );

    const stockData = liveStock || {
        symbol,
        companyName,
        currentPrice,
        previousClose,
        change,
        changePercent,
        volume
    };

    const isPositive = (stockData.changePercent || 0) >= 0;
    const formattedVolume = stockData.volume ?
        stockData.volume > 1000000
            ? `${(stockData.volume / 1000000).toFixed(2)}M`
            : `${(stockData.volume / 1000).toFixed(0)}K`
        : 'N/A';

    return (
        <motion.div
            className={cn(
                "rounded-lg bg-[#111827] border border-[#2a2d3a] p-4 cursor-pointer hover:bg-[#161b2c] transition-all",
                isPinned && "border-[#22c55e]"
            )}
            onClick={() => onShowChart(symbol)}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.2 }}
            whileHover={{ y: -2 }}
            layout
        >
            <div className="flex justify-between items-start">
                <div>
                    <div className="flex items-center gap-2">
                        <h3 className="text-lg font-bold text-white">{symbol}</h3>
                        <button
                            onClick={(e) => {
                                e.stopPropagation();
                                onTogglePin();
                            }}
                            className="text-gray-400 hover:text-[#22c55e] transition-colors"
                            aria-label={isPinned ? "Unpin stock" : "Pin stock"}
                        >
                            {isPinned ? <PinOff size={16} /> : <Pin size={16} />}
                        </button>
                    </div>
                    <AnimatePresence mode="wait">
                        <motion.div
                            key={stockData.currentPrice.toString()}
                            initial={{ opacity: 0, y: -20 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: 20 }}
                            transition={{ duration: 0.3 }}
                            className={cn(
                                "text-xl font-bold mt-1",
                                priceFlash === 'up' ? "text-[#22c55e]" :
                                    priceFlash === 'down' ? "text-[#ef4444]" :
                                        "text-white"
                            )}
                        >
                            ${stockData.currentPrice.toFixed(2)}
                        </motion.div>
                    </AnimatePresence>
                    <div className="flex items-center gap-1 mt-1">
                        <span className={cn(
                            "text-sm",
                            isPositive ? "text-[#22c55e]" : "text-[#ef4444]"
                        )}>
                            {isPositive ? "+" : ""}{stockData.changePercent?.toFixed(2)}% ({isPositive ? "+" : ""}{stockData.change?.toFixed(2)})
                        </span>
                    </div>
                    <div className="text-sm text-gray-400 mt-1">{companyName}</div>
                </div>
                <div className="flex flex-col items-end gap-2">
                    <div className="flex gap-1">
                        <button
                            onClick={(e) => {
                                e.stopPropagation();
                                onShowAlerts(symbol);
                            }}
                            className="text-gray-400 hover:text-white p-2 rounded-full hover:bg-[#1e293b] transition-colors"
                            aria-label="Set price alerts"
                        >
                            <Bell size={16} />
                        </button>
                        <button
                            onClick={(e) => {
                                e.stopPropagation();
                                onRemove();
                            }}
                            className="text-gray-400 hover:text-[#ef4444] p-2 rounded-full hover:bg-[#1e293b] transition-colors"
                            aria-label="Remove from watchlist"
                        >
                            <X size={16} />
                        </button>
                    </div>
                    <div className="mt-1">
                        {isPositive ? (
                            <TrendingUp size={24} className="text-[#22c55e]" />
                        ) : (
                            <TrendingDown size={24} className="text-[#ef4444]" />
                        )}
                    </div>
                </div>
            </div>

            <div className="mt-3 flex justify-between items-center">
                <div className="text-xs text-gray-500">
                    Last updated: {lastUpdateTime.toLocaleTimeString()}
                </div>
                <div className="text-xs text-gray-500">
                    Vol: {formattedVolume}
                </div>
            </div>
        </motion.div>
    );
});

StockCard.displayName = "StockCard";
