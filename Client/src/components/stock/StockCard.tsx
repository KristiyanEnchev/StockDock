import { useState, useEffect, memo } from "react";
import { Bell, Pin, PinOff, TrendingUp, TrendingDown, AlertCircle } from "lucide-react";
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
    note?: string;
    onRemove: () => void;
    onTogglePin: () => void;
    onUpdateNote: (note: string) => void;
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
    note,
    onRemove,
    onTogglePin,
    onUpdateNote,
    onShowChart,
    onShowAlerts,
}: StockCardProps) => {
    const [isEditingNote, setIsEditingNote] = useState(false);
    const [noteText, setNoteText] = useState(note || "");
    const [showVolumeInfo, setShowVolumeInfo] = useState(false);
    const [lastUpdateTime, setLastUpdateTime] = useState(new Date());

    const liveStock = useAppSelector(state => selectLiveStock(state, symbol));
    const alerts = useAppSelector(selectUserAlerts);

    useEffect(() => {
        setNoteText(note || "");
    }, [note]);

    useEffect(() => {
        if (liveStock) {
            setLastUpdateTime(new Date());
        }
    }, [liveStock]);

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

    const priceUpdateKey = `${symbol}-${stockData.currentPrice}-${stockData.changePercent}`;

    const isPositive = (stockData.changePercent || 0) >= 0;
    const formattedVolume = stockData.volume ?
        stockData.volume > 1000000
            ? `${(stockData.volume / 1000000).toFixed(2)}M`
            : `${(stockData.volume / 1000).toFixed(0)}K`
        : 'N/A';

    const handleNoteSubmit = () => {
        onUpdateNote(noteText);
        setIsEditingNote(false);
    };

    return (
        <motion.div
            className={cn(
                "rounded-xl border bg-[#1a1d1f] border-[#2a2d31] p-4 cursor-pointer hover:bg-[#1f2225] transition-colors relative",
                isPinned && "ring-2 ring-blue-500"
            )}
            onClick={() => onShowChart(symbol)}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.3 }}
            whileHover={{ y: -4 }}
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
                            className="text-gray-400 hover:text-gray-300"
                            aria-label={isPinned ? "Unpin stock" : "Pin stock"}
                        >
                            {isPinned ? <PinOff size={16} /> : <Pin size={16} />}
                        </button>
                        {hasActiveAlerts && (
                            <AlertCircle size={16} className="text-amber-500" />
                        )}
                    </div>
                    <AnimatePresence mode="wait">
                        <motion.div
                            key={priceUpdateKey}
                            initial={{ opacity: 0, y: -10 }}
                            animate={{ opacity: 1, y: 0 }}
                            exit={{ opacity: 0, y: 10 }}
                            transition={{ duration: 0.3 }}
                            className="text-xl font-bold mt-1 text-white"
                        >
                            ${stockData.currentPrice.toFixed(2)}
                        </motion.div>
                    </AnimatePresence>
                    <div className="flex items-center gap-1">
                        {isPositive ? (
                            <TrendingUp size={14} className="text-[#22c55e]" />
                        ) : (
                            <TrendingDown size={14} className="text-rose-500" />
                        )}
                        <span className={cn(
                            "text-sm",
                            isPositive ? "text-[#22c55e]" : "text-rose-500"
                        )}>
                            {stockData.changePercent?.toFixed(2)}% ({isPositive ? "+" : ""}{stockData.change?.toFixed(2)})
                        </span>
                    </div>
                    <div className="text-sm text-gray-400 mt-1">{companyName}</div>
                </div>
                <div className="flex gap-2">
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            onShowAlerts(symbol);
                        }}
                        className="text-gray-400 hover:text-gray-300 p-2 rounded bg-[#2a2d31] hover:bg-[#353a3f] transition-colors"
                        aria-label="Set price alerts"
                    >
                        <Bell size={16} />
                    </button>
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            onRemove();
                        }}
                        className="text-gray-400 hover:text-gray-300 hover:text-rose-500 transition-colors"
                        aria-label="Remove from watchlist"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <line x1="18" y1="6" x2="6" y2="18"></line>
                            <line x1="6" y1="6" x2="18" y2="18"></line>
                        </svg>
                    </button>
                </div>
            </div>

            <div className="mt-3 flex justify-between items-center text-xs text-gray-400">
                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        setShowVolumeInfo(!showVolumeInfo);
                    }}
                    className="hover:text-gray-300 transition-colors"
                >
                    {showVolumeInfo ? 'Vol: ' + formattedVolume : 'Last updated: ' + lastUpdateTime.toLocaleTimeString()}
                </button>
            </div>

            <div className="mt-3">
                {isEditingNote ? (
                    <div onClick={(e) => e.stopPropagation()} className="relative">
                        <textarea
                            value={noteText}
                            onChange={(e) => setNoteText(e.target.value)}
                            className="w-full bg-[#2a2d31] text-white border-0 rounded p-2 text-sm"
                            placeholder="Add a note..."
                            rows={2}
                        />
                        <div className="flex gap-2 mt-2">
                            <button
                                onClick={handleNoteSubmit}
                                className="text-sm bg-blue-500 hover:bg-blue-600 text-white px-2 py-1 rounded transition-colors"
                            >
                                Save
                            </button>
                            <button
                                onClick={() => {
                                    setIsEditingNote(false);
                                    setNoteText(note || "");
                                }}
                                className="text-sm bg-[#2a2d31] hover:bg-[#353a3f] text-white px-2 py-1 rounded transition-colors"
                            >
                                Cancel
                            </button>
                        </div>
                    </div>
                ) : (
                    <div
                        onClick={(e) => {
                            e.stopPropagation();
                            setIsEditingNote(true);
                        }}
                        className="text-sm text-gray-400 hover:text-gray-300 cursor-text min-h-[1.5rem] truncate"
                    >
                        {note || "Click to add a note..."}
                    </div>
                )}
            </div>
        </motion.div>
    );
});

StockCard.displayName = "StockCard";
