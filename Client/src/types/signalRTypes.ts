import { StockDto } from './stockTypes';
import { AlertDto } from './alertTypes';

export interface IStockHub {
    ReceiveStockPriceUpdate: (stock: StockDto) => void;
    ReceivePopularStocksUpdate: (stocks: StockDto[]) => void;
    ReceiveAlertTriggered: (alert: AlertDto) => void;
    ReceiveAlertCreated: (alert: AlertDto) => void;
    ReceiveAlertDeleted: (alertId: string) => void;
    ReceiveUserAlerts: (alerts: AlertDto[]) => void;
    ReceiveError: (message: string) => void;
}
