import { useAppSelector } from "@/store/hooks";
import { selectSignalRStatus } from "@/features/signalR/signalRSlice";
import { Wifi, WifiOff, RefreshCw } from "lucide-react";
import { cn } from "@/lib/utils";
import { useState } from "react";
import { initializeSignalR, stopSignalR } from "@/services/signalRService";
import { selectToken, selectIsAuthenticated } from "@/features/auth/authSlice";

const ConnectionStatus = () => {
  const status = useAppSelector(selectSignalRStatus);
  const [showReconnect, setShowReconnect] = useState(false);
  const token = useAppSelector(selectToken);
  const isAuthenticated = useAppSelector(selectIsAuthenticated);

  const handleReconnect = async () => {
    try {
      await stopSignalR();
      await initializeSignalR(isAuthenticated ? token! : undefined);
      setShowReconnect(false);
    } catch (error) {
    }
  };

  return (
    <div
      className={cn(
        "fixed bottom-4 right-4 z-50 flex items-center gap-2 px-3 py-1.5 rounded-full text-xs font-medium transition-all duration-300",
        status === 'connected' && "bg-[#22c55e]/10 text-[#22c55e]",
        status === 'connecting' && "bg-[#eab308]/10 text-[#eab308]",
        status === 'disconnected' && "bg-[#ef4444]/10 text-[#ef4444]"
      )}
      onClick={() => setShowReconnect(!showReconnect)}
    >
      {status === 'connected' && <Wifi size={14} />}
      {status === 'connecting' && <Wifi className="animate-pulse" size={14} />}
      {status === 'disconnected' && <WifiOff size={14} />}
      <span>
        {status === 'connected' && "Connected"}
        {status === 'connecting' && "Connecting..."}
        {status === 'disconnected' && "Disconnected"}
      </span>

      {showReconnect && (
        <button
          onClick={(e) => {
            e.stopPropagation();
            handleReconnect();
          }}
          className="ml-1 p-1 hover:bg-white/10 rounded-full"
          title="Reconnect"
        >
          <RefreshCw size={12} />
        </button>
      )}
    </div>
  );
};

export default ConnectionStatus;
