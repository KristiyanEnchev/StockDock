import { ArrowUpRight, ArrowDownRight } from "lucide-react";
import { useState, memo, useMemo, useCallback } from "react";
import toast from "react-hot-toast";
import { useAppSelector } from "@/store/hooks";
import { selectPopularStocks } from "@/features/stocks/stocksSlice";
import { selectIsAuthenticated } from "@/features/auth/authSlice";

interface PopularStock {
    symbol: string;
    name: string;
    change: number;
}

interface PopularStocksSidebarProps {
    onAddStock: (stock: PopularStock) => void;
    addedStocks: string[];
    title?: string;
    className?: string;
}

export const PopularStocksSidebar = memo(({
    onAddStock,
    addedStocks,
    title = "Popular Stocks",
    className = ""
}: PopularStocksSidebarProps) => {
    const [isCollapsed, setIsCollapsed] = useState(false);
    const popularStocks = useAppSelector(selectPopularStocks);
    const isAuthenticated = useAppSelector(selectIsAuthenticated);

    const displayStocks = useMemo(() => popularStocks.map(stock => ({
        symbol: stock.symbol,
        name: stock.companyName || stock.symbol,
        change: stock.changePercent || 0
    })), [popularStocks]);

    const handleAddStock = useCallback((stock: PopularStock) => {
        if (!isAuthenticated) {
            toast.error("Please log in to add stocks to your watchlist");
            return;
        }

        if (addedStocks.includes(stock.symbol)) {
            toast.error(`${stock.symbol} is already in your watchlist`);
            return;
        }
        onAddStock(stock);
        toast.success(`${stock.symbol} has been added to your watchlist`);
    }, [onAddStock, addedStocks, isAuthenticated]);

    return (
        <div className={`h-full bg-[#1A1F2C] border-r border-[#2a2d31] rounded-lg transition-all duration-300 ${isCollapsed ? "w-14" : "w-full"} ${className}`}>
            <div className="p-2 border-b border-[#2a2d31] flex justify-between items-center">
                <h2 className={`text-sm font-medium text-white ${isCollapsed ? "hidden" : "block"}`}>
                    {title}
                </h2>
                <button
                    onClick={() => setIsCollapsed(!isCollapsed)}
                    className="text-gray-400 hover:text-white transition-colors text-xs p-1 hover:bg-[#2a2d31] rounded"
                >
                    {isCollapsed ? "→" : "←"}
                </button>
            </div>
            <div className="overflow-y-auto max-h-[calc(100vh-12rem)] scrollbar-thin scrollbar-thumb-[#2a2d31] scrollbar-track-transparent">
                {displayStocks.length > 0 ? (
                    displayStocks.map((stock) => (
                        <button
                            key={stock.symbol}
                            onClick={() => handleAddStock(stock)}
                            className={`w-full py-2 px-3 flex items-center gap-2 hover:bg-[#2a2d31] transition-colors border-b border-[#2a2d31]/50 ${addedStocks.includes(stock.symbol) ? "opacity-50" : ""
                                }`}
                        >
                            {stock.change >= 0 ? (
                                <ArrowUpRight className="text-green-500 shrink-0" size={16} />
                            ) : (
                                <ArrowDownRight className="text-red-500 shrink-0" size={16} />
                            )}
                            <div className={`flex-1 text-left ${isCollapsed ? "hidden" : "block"}`}>
                                <div className="text-sm font-medium text-white leading-none">{stock.symbol}</div>
                                <div className="text-xs text-gray-400 truncate mt-0.5">{stock.name}</div>
                            </div>
                            {!isCollapsed && addedStocks.includes(stock.symbol) && (
                                <span className="text-xs text-green-500 px-1.5 py-0.5 bg-green-500/10 rounded">Added</span>
                            )}
                        </button>
                    ))
                ) : (
                    <div className="p-3 text-center text-gray-400 text-sm">
                        No stocks available
                    </div>
                )}
            </div>
        </div>
    );
});

PopularStocksSidebar.displayName = "PopularStocksSidebar";
