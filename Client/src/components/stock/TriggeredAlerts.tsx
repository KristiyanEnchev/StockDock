import { useAppSelector } from "@/store/hooks";
import { selectTriggeredAlerts } from "@/features/alerts/alertsSlice";
import { AlertType } from "@/types/alertTypes";
import { Bell, ChevronUp, ChevronDown, X } from "lucide-react";
import { useState } from "react";

export const TriggeredAlerts = () => {
    const triggeredAlerts = useAppSelector(selectTriggeredAlerts);
    const [isCollapsed, setIsCollapsed] = useState(false);

    if (triggeredAlerts.length === 0) {
        return null;
    }

    return (
        <div className="fixed bottom-4 right-4 z-50 w-80 bg-[#1a1d1f] border border-[#2a2d31] rounded-md shadow-lg overflow-hidden">
            <div
                className="flex justify-between items-center p-3 bg-[#2a2d31] cursor-pointer"
                onClick={() => setIsCollapsed(!isCollapsed)}
            >
                <div className="flex items-center gap-2">
                    <Bell size={16} className="text-[#22c55e]" />
                    <span className="font-medium text-white">Recent Alerts</span>
                    <span className="bg-[#22c55e] text-black text-xs px-2 py-0.5 rounded-full">
                        {triggeredAlerts.length}
                    </span>
                </div>
                <button className="text-gray-400 hover:text-white">
                    {isCollapsed ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
                </button>
            </div>

            {!isCollapsed && (
                <div className="max-h-60 overflow-y-auto p-2">
                    {triggeredAlerts.map(alert => (
                        <div
                            key={alert.id}
                            className="p-2 mb-2 bg-[#2a2d31] rounded flex items-start justify-between"
                        >
                            <div>
                                <div className="flex items-center gap-1 mb-1">
                                    <span className="font-medium text-white">{alert.symbol}</span>
                                    <span className={`text-xs px-1.5 py-0.5 rounded ${alert.type === AlertType.PriceAbove
                                            ? 'bg-[#22c55e]/20 text-[#22c55e]'
                                            : 'bg-[#ef4444]/20 text-[#ef4444]'
                                        }`}>
                                        {alert.type === AlertType.PriceAbove ? 'Above' : 'Below'} ${alert.threshold}
                                    </span>
                                </div>
                                <div className="text-xs text-gray-400">
                                    Triggered: {alert.lastTriggeredAt ? new Date(alert.lastTriggeredAt).toLocaleString() : 'N/A'}
                                </div>
                            </div>
                            <button className="text-gray-500 hover:text-gray-300 p-1">
                                <X size={14} />
                            </button>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};
