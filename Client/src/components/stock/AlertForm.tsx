import { useState, useCallback } from "react";
import { useAppSelector } from "@/store/hooks";
import { selectUserAlerts } from "@/features/alerts/alertsSlice";
import { selectLiveStock } from "@/features/stocks/stocksSlice";
import { createAlert, deleteAlert, simulateAlertTrigger } from "@/services/signalRService";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { X, AlertTriangle, ArrowUp, ArrowDown, DollarSign, Bell } from "lucide-react";
import { toast } from "react-hot-toast";
import { AlertType } from "@/types/alertTypes";
import { selectIsAuthenticated } from "@/features/auth/authSlice";
import { useNavigate } from "react-router-dom";

interface AlertFormProps {
    symbol: string;
    showAlertForm: boolean;
    setShowAlertForm: (show: boolean) => void;
}

export const AlertForm = ({ symbol, showAlertForm, setShowAlertForm }: AlertFormProps) => {
    const [alertPrice, setAlertPrice] = useState("");
    const [alertCondition, setAlertCondition] = useState<AlertType>(AlertType.PriceAbove);
    const alerts = useAppSelector(selectUserAlerts);
    const liveStock = useAppSelector(state => selectLiveStock(state, symbol));
    const isAuthenticated = useAppSelector(selectIsAuthenticated);
    const navigate = useNavigate();

    const currentPrice = liveStock?.currentPrice || 0;

    const handleAddAlert = useCallback(async () => {
        if (!isAuthenticated) {
            toast.error("Please sign in to create alerts");
            navigate("/login");
            return;
        }

        const price = Number(alertPrice);
        if (alertPrice && !isNaN(price)) {
            try {
                await createAlert(symbol, alertCondition, price);
                setShowAlertForm(false);
                setAlertPrice("");
                toast.success(`Alert created for ${symbol} when price goes ${alertCondition === AlertType.PriceAbove ? 'above' : 'below'} $${price}`);
            } catch (error) {
                toast.error(`Failed to create alert for ${symbol}`);
            }
        } else {
            toast.error("Please enter a valid price");
        }
    }, [alertPrice, alertCondition, symbol, setShowAlertForm, isAuthenticated, navigate]);

    const handleRemoveAlert = useCallback(async (alertId: string) => {
        if (!isAuthenticated) {
            toast.error("Please sign in to manage alerts");
            navigate("/login");
            return;
        }

        try {
            await deleteAlert(alertId);
            toast.success("Price alert has been removed");
        } catch (error) {
            toast.error("Failed to remove alert");
        }
    }, [isAuthenticated, navigate]);

    const handleTestAlert = useCallback(() => {
        if (!isAuthenticated) {
            toast.error("Please sign in to test alerts");
            navigate("/login");
            return;
        }

        const price = Number(alertPrice);
        if (alertPrice && !isNaN(price)) {
            simulateAlertTrigger(symbol, alertCondition, price);
        } else {
            toast.error("Please enter a valid price for test");
        }
    }, [alertPrice, alertCondition, symbol, isAuthenticated, navigate]);

    const activeAlerts = alerts.filter(alert => alert.symbol === symbol && !alert.isTriggered);

    if (!showAlertForm && activeAlerts.length === 0) {
        return null;
    }

    return (
        <div className="mb-6 p-4 bg-[#2a2d31] rounded-md">
            {showAlertForm && (
                <div className="mb-4">
                    <div className="flex justify-between items-center mb-2">
                        <h3 className="text-sm font-semibold text-gray-300">Create Price Alert</h3>
                        <div className="flex items-center gap-1 text-sm text-gray-400">
                            <DollarSign size={14} />
                            <span>Current: ${currentPrice.toFixed(2)}</span>
                        </div>
                    </div>
                    <div className="flex gap-2 items-center">
                        <div className="flex">
                            <button
                                onClick={() => setAlertCondition(AlertType.PriceAbove)}
                                className={`flex items-center gap-1 px-3 py-1 rounded-l-md text-xs ${alertCondition === AlertType.PriceAbove
                                    ? 'bg-[#22c55e]/20 text-[#22c55e]'
                                    : 'bg-[#3a3d41] text-gray-300'
                                    }`}
                            >
                                <ArrowUp size={12} />
                                Above
                            </button>
                            <button
                                onClick={() => setAlertCondition(AlertType.PriceBelow)}
                                className={`flex items-center gap-1 px-3 py-1 rounded-r-md text-xs ${alertCondition === AlertType.PriceBelow
                                    ? 'bg-[#ef4444]/20 text-[#ef4444]'
                                    : 'bg-[#3a3d41] text-gray-300'
                                    }`}
                            >
                                <ArrowDown size={12} />
                                Below
                            </button>
                        </div>
                        <Input
                            type="number"
                            placeholder="Price"
                            value={alertPrice}
                            onChange={(e) => setAlertPrice(e.target.value)}
                            className="h-7 text-sm bg-[#1a1d1f] border-[#3a3d41] w-24 text-light-text dark:text-dark-text"
                        />
                        <Button
                            onClick={handleAddAlert}
                            className="h-7 px-3 py-0 text-xs bg-[#22c55e] hover:bg-[#22c55e]/80"
                        >
                            Set Alert
                        </Button>

                        {/* {import.meta.env.DEV && ( */}
                        <Button
                            onClick={handleTestAlert}
                            className="h-7 px-3 py-0 text-xs bg-[#3a3d41] hover:bg-[#4a4d51] flex items-center gap-1"
                            title="Test alert trigger (dev only)"
                        >
                            <Bell size={12} />
                            Test
                        </Button>
                        {/* )} */}
                    </div>
                </div>
            )}

            {activeAlerts.length > 0 && (
                <div>
                    <h3 className="text-sm font-semibold mb-2 text-gray-300">Active Alerts</h3>
                    <div className="space-y-2">
                        {activeAlerts.map((alert) => (
                            <div
                                key={alert.id}
                                className="flex justify-between items-center p-2 rounded bg-[#1a1d1f] text-xs text-light-text dark:text-dark-text"
                            >
                                <div className="flex items-center gap-2">
                                    <AlertTriangle
                                        size={14}
                                        className={
                                            alert.type === AlertType.PriceAbove
                                                ? 'text-[#22c55e]'
                                                : 'text-[#ef4444]'
                                        }
                                    />
                                    <span>
                                        {alert.type === AlertType.PriceAbove ? 'Above' : 'Below'} ${alert.threshold}
                                    </span>
                                </div>
                                <button
                                    onClick={() => handleRemoveAlert(alert.id)}
                                    className="text-gray-400 hover:text-white"
                                >
                                    <X size={14} />
                                </button>
                            </div>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};
