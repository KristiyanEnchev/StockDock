import { useState, useEffect, memo } from "react";
import { Bell, Pin, X, TrendingUp, TrendingDown } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { cn } from "@/lib/utils";
import { useAppSelector } from "@/store/hooks";
import { selectLiveStock } from "@/features/stocks/stocksSlice";
import { selectUserAlerts } from "@/features/alerts/alertsSlice";
import { ChartDialog } from "./ChartDialog";

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
    isPinned = false,
    onRemove,
    onTogglePin,
    onShowChart,
    onShowAlerts,
}: StockCardProps) => {
    const [lastUpdateTime, setLastUpdateTime] = useState(new Date());
    const [priceFlash, setPriceFlash] = useState<'up' | 'down' | null>(null);
    const [prevPrice, setPrevPrice] = useState(currentPrice);
    const [showChartDialog, setShowChartDialog] = useState(false);
    const [pinAnimation, setPinAnimation] = useState(false);

    const liveStock = useAppSelector(state => selectLiveStock(state, symbol));
    const alerts = useAppSelector(selectUserAlerts);

    useEffect(() => {
        if (liveStock) {
            const newPrice = liveStock.currentPrice;

            if (newPrice !== prevPrice) {
                setPriceFlash(newPrice > prevPrice ? 'up' : 'down');
                setPrevPrice(newPrice);

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

    const handleCardClick = () => {
        setShowChartDialog(true);
        onShowChart(symbol);
    };

    const handleTogglePin = (e: React.MouseEvent) => {
        e.stopPropagation();
        setPinAnimation(true);
        setTimeout(() => setPinAnimation(false), 300);
        onTogglePin();
    };

    return (
        <>
            <motion.div
                className={cn(
                    "rounded-lg bg-[#111827] border border-[#2a2d3a] p-4 cursor-pointer hover:bg-[#161b2c] transition-all",
                    isPinned && "border-[#22c55e] shadow-[0_0_10px_rgba(34,197,94,0.3)]"
                )}
                onClick={handleCardClick}
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
                            <motion.button
                                onClick={handleTogglePin}
                                className={cn(
                                    "transition-colors",
                                    isPinned
                                        ? "text-[#22c55e] hover:text-[#ef4444]"
                                        : "text-gray-400 hover:text-[#22c55e]"
                                )}
                                aria-label={isPinned ? "Unpin stock" : "Pin stock"}
                                animate={pinAnimation ? { scale: [1, 1.5, 1] } : {}}
                                transition={{ duration: 0.3 }}
                            >
                                {isPinned ? <Pin size={16} /> : <Pin size={16} />}
                            </motion.button>
                            {hasActiveAlerts && (
                                <span className="text-[#eab308]">
                                    <Bell size={14} />
                                </span>
                            )}
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
                                className={cn(
                                    "p-2 rounded-full hover:bg-[#1e293b] transition-colors",
                                    hasActiveAlerts ? "text-[#eab308]" : "text-gray-400 hover:text-white"
                                )}
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
                {isPinned && (
                    <div className="absolute -top-2 -right-2">
                        <motion.div
                            className="bg-[#22c55e] rounded-full p-1"
                            initial={{ scale: 0 }}
                            animate={{ scale: 1 }}
                            exit={{ scale: 0 }}
                        >
                            <Pin size={12} className="text-white" />
                        </motion.div>
                    </div>
                )}
            </motion.div>

            <ChartDialog
                open={showChartDialog}
                onOpenChange={setShowChartDialog}
                symbol={symbol}
                name={companyName}
            />
        </>
    );
});

StockCard.displayName = "StockCard";
