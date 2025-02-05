import { useState, useEffect, useMemo } from "react";
import { Bell, Percent, DollarSign } from "lucide-react";
import { AlertForm } from "./AlertForm";
import { PriceChart } from "./PriceChart";
import { useGetStockHistoryQuery } from "@/features/stocks/stocksApi";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { useAppSelector } from "@/store/hooks";
import { selectLiveStock } from "@/features/stocks/stocksSlice";

const mockDataCache = new Map<string, { date: string; value: number }[]>();

const generateMockData = (symbol: string, days: number = 30) => {
    if (mockDataCache.has(symbol)) {
        return mockDataCache.get(symbol)!;
    }

    const data = [];
    const today = new Date();

    const symbolSum = symbol.split('').reduce((sum, char) => sum + char.charCodeAt(0), 0);
    let basePrice = 100 + (symbolSum % 150);

    for (let i = days; i >= 0; i--) {
        const date = new Date(today);
        date.setDate(date.getDate() - i);
        const dateString = date.toISOString().split('T')[0];

        const dayFactor = i / 10;
        const symbolFactor = symbolSum / 1000;
        const movement = Math.sin(dayFactor + symbolFactor) * 2;

        basePrice = basePrice + movement;

        data.push({
            date: dateString,
            value: parseFloat(basePrice.toFixed(2))
        });
    }

    mockDataCache.set(symbol, data);

    return data;
};

interface ChartDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    symbol: string;
    name: string;
}

export const ChartDialog = ({
    open,
    onOpenChange,
    symbol,
    name
}: ChartDialogProps) => {
    const [showMA, setShowMA] = useState(false);
    const [showAlertForm, setShowAlertForm] = useState(false);
    const [useMockData, setUseMockData] = useState(false);
    const [debugInfo, setDebugInfo] = useState<string>("");
    const [queryParams, setQueryParams] = useState<{ symbol: string; from: string; to: string } | null>(null);

    const liveStock = useAppSelector(state => selectLiveStock(state, symbol));
    const mockData = useMemo(() => generateMockData(symbol), [symbol]);

    useEffect(() => {
        if (open) {
            const toDate = new Date();
            const fromDate = new Date();
            fromDate.setDate(fromDate.getDate() - 30);
            const toDateString = toDate.toISOString();
            const fromDateString = fromDate.toISOString();

            setQueryParams({
                symbol,
                from: fromDateString,
                to: toDateString
            });
        }
    }, [open, symbol]);

    const { data: historyData, isLoading, error } = useGetStockHistoryQuery(
        queryParams || { symbol: "", from: "", to: "" },
        { skip: !open || !queryParams }
    );

    useEffect(() => {
        if (open && queryParams) {
            const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || "Not defined";
            const fullUrl = `${apiBaseUrl}/api/stocks/${symbol}/history?from=${encodeURIComponent(queryParams.from)}&to=${encodeURIComponent(queryParams.to)}`;

            setDebugInfo(`API URL: ${fullUrl}\nSymbol: ${symbol}\nFrom: ${queryParams.from}\nTo: ${queryParams.to}`);

            const timer = setTimeout(() => {
                if (isLoading || (!historyData && !error)) {
                    setUseMockData(true);
                }
            }, 5000);

            return () => clearTimeout(timer);
        }
    }, [open, symbol, queryParams, historyData, isLoading, error]);

    const chartData = historyData?.map(item => ({
        date: new Date(item.timestamp).toISOString().split('T')[0],
        value: item.close
    })) || [];

    const finalChartData = (useMockData || (error && !isLoading)) ? mockData : chartData;
    const enhancedData = finalChartData.map((item, index, arr) => {
        let MA20 = null;
        if (index >= 19) {
            const window = arr.slice(index - 19, index + 1);
            MA20 = window.reduce((sum, point) => sum + point.value, 0) / 20;
        }

        let MA50 = null;
        if (index >= 49) {
            const window = arr.slice(index - 49, index + 1);
            MA50 = window.reduce((sum, point) => sum + point.value, 0) / 50;
        }

        return {
            ...item,
            MA20,
            MA50
        };
    });

    const handleRefresh = () => {
        setUseMockData(false);
        const toDate = new Date();
        const fromDate = new Date();
        fromDate.setDate(fromDate.getDate() - 30);
        const toDateString = toDate.toISOString();
        const fromDateString = fromDate.toISOString();

        setQueryParams({
            symbol,
            from: fromDateString,
            to: toDateString
        });
    };

    const currentPrice = liveStock?.currentPrice || (finalChartData.length > 0 ? finalChartData[finalChartData.length - 1].value : 0);
    const previousClose = liveStock?.previousClose || (finalChartData.length > 1 ? finalChartData[finalChartData.length - 2].value : 0);
    const changePercent = liveStock?.changePercent || (previousClose ? ((currentPrice - previousClose) / previousClose) * 100 : 0);
    const isPositive = changePercent >= 0;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="max-w-[90vw] w-[800px] bg-[#1a1d1f] border-[#2a2d31] text-white max-h-[90vh] overflow-y-auto">
                <DialogHeader>
                    <DialogTitle className="text-white flex justify-between items-center sticky top-0">
                        <div className="flex items-center gap-2">
                            <span>{symbol}</span>
                            <span className="text-gray-400 text-sm font-normal">{name}</span>
                        </div>
                        <div className="flex items-center gap-4">
                            <div className="flex flex-col items-end">
                                <div className="flex items-center gap-1">
                                    <DollarSign size={16} className="text-gray-400" />
                                    <span className="text-lg font-bold">{currentPrice.toFixed(2)}</span>
                                </div>
                                <div className={`flex items-center gap-1 text-sm ${isPositive ? 'text-[#22c55e]' : 'text-[#ef4444]'}`}>
                                    <Percent size={14} />
                                    <span>{isPositive ? '+' : ''}{changePercent.toFixed(2)}%</span>
                                </div>
                            </div>
                            <button
                                onClick={() => setShowAlertForm(!showAlertForm)}
                                className="p-2 rounded-full bg-[#2a2d31] hover:bg-[#3a3d41] transition-colors"
                                title="Set price alert"
                            >
                                <Bell size={16} />
                            </button>
                        </div>
                    </DialogTitle>
                </DialogHeader>

                <AlertForm
                    symbol={symbol}
                    showAlertForm={showAlertForm}
                    setShowAlertForm={setShowAlertForm}
                />

                <div className="mt-4">
                    <div className="flex justify-between items-center mb-4">
                        <div className="flex gap-2 items-center">
                            <button
                                onClick={() => setShowMA(!showMA)}
                                className={`px-3 py-1 rounded-md text-xs ${showMA ? 'bg-[#3a3d41] text-white' : 'bg-[#2a2d31] text-gray-400'
                                    }`}
                            >
                                Moving Averages
                            </button>
                            <button
                                onClick={handleRefresh}
                                className="px-3 py-1 rounded-md text-xs bg-[#2a2d31] text-gray-400 hover:bg-[#3a3d41] hover:text-white"
                                disabled={isLoading}
                            >
                                Refresh
                            </button>
                        </div>
                    </div>

                    <div className="h-[400px] w-full">
                        {isLoading && !useMockData ? (
                            <div className="h-full w-full flex items-center justify-center text-gray-400">
                                Loading chart data...
                            </div>
                        ) : (
                            <PriceChart data={enhancedData} showMA={showMA} symbol={""} />
                        )}
                    </div>

                    {import.meta.env.DEV && error && (
                        <div className="mt-4 p-4 bg-[#2a2d31] rounded-md text-xs font-mono whitespace-pre-wrap text-red-400">
                            <div>Error: {JSON.stringify(error, null, 2)}</div>
                            <div className="mt-2">{debugInfo}</div>
                        </div>
                    )}
                </div>
            </DialogContent>
        </Dialog>
    );
};
