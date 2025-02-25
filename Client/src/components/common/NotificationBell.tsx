import { useEffect, useRef, useState } from "react";
import { Bell, X } from "lucide-react";
import { useAppSelector, useAppDispatch } from "@/store/hooks";
import { selectTriggeredAlerts, removeTriggeredAlert } from "@/features/alerts/alertsSlice";
import { AlertType } from "@/types/alertTypes";
import { selectIsAuthenticated } from "@/features/auth/authSlice";

export function NotificationBell() {
    const dispatch = useAppDispatch();
    const triggeredAlerts = useAppSelector(selectTriggeredAlerts);
    const isAuthenticated = useAppSelector(selectIsAuthenticated);
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const handleRemoveAlert = (alertId: string) => {
        dispatch(removeTriggeredAlert(alertId));
    };

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsDropdownOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    if (!isAuthenticated) {
        return null;
    }

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                className="relative flex items-center justify-center w-10 h-10 text-light-text dark:text-dark-text hover:text-green-600 dark:hover:text-green-400 transition"
            >
                <Bell className="w-5 h-5" />
                {triggeredAlerts.length > 0 && (
                    <span className="absolute top-1 right-1 flex h-4 w-4 items-center justify-center rounded-full bg-green-600 text-xs text-white">
                        {triggeredAlerts.length}
                    </span>
                )}
            </button>

            {isDropdownOpen && (
                <div className="absolute right-0 mt-2 w-80 rounded-md shadow-lg bg-light-card dark:bg-dark-card border border-light-card dark:border-dark-card z-50">
                    <div className="p-3 border-b border-light-bg dark:border-dark-bg flex justify-between items-center">
                        <div className="flex items-center gap-2">
                            <Bell size={16} className="text-green-600 dark:text-green-400" />
                            <span className="font-medium text-light-text dark:text-dark-text">Recent Alerts</span>
                            {triggeredAlerts.length > 0 && (
                                <span className="bg-green-600 text-white text-xs px-2 py-0.5 rounded-full">
                                    {triggeredAlerts.length}
                                </span>
                            )}
                        </div>
                    </div>

                    <div className="max-h-60 overflow-y-auto">
                        {triggeredAlerts.length > 0 ? (
                            <div className="p-2">
                                {triggeredAlerts.map(alert => (
                                    <div
                                        key={alert.id}
                                        className="p-2 mb-2 bg-light-bg dark:bg-dark-bg rounded flex items-start justify-between"
                                    >
                                        <div>
                                            <div className="flex items-center gap-1 mb-1">
                                                <span className="font-medium text-light-text dark:text-dark-text">{alert.symbol}</span>
                                                <span className={`text-xs px-1.5 py-0.5 rounded ${alert.type === AlertType.PriceAbove
                                                        ? 'bg-green-600/20 text-green-600 dark:text-green-400'
                                                        : 'bg-red-600/20 text-red-600 dark:text-red-400'
                                                    }`}>
                                                    {alert.type === AlertType.PriceAbove ? 'Above' : 'Below'} ${alert.threshold}
                                                </span>
                                            </div>
                                            <div className="text-xs text-gray-500 dark:text-gray-400">
                                                Triggered: {alert.lastTriggeredAt ? new Date(alert.lastTriggeredAt).toLocaleString() : 'N/A'}
                                            </div>
                                        </div>
                                        <button
                                            className="text-gray-500 hover:text-gray-700 dark:hover:text-gray-300 p-1"
                                            onClick={() => handleRemoveAlert(alert.id)}
                                        >
                                            <X size={14} />
                                        </button>
                                    </div>
                                ))}
                            </div>
                        ) : (
                            <div className="p-4 text-center text-gray-500 dark:text-gray-400">
                                No alerts to display
                            </div>
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}
