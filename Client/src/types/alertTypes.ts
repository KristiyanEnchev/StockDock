export enum AlertType {
    PriceAbove = 'PriceAbove',
    PriceBelow = 'PriceBelow',
    PercentageChange = 'PercentageChange'
}

export interface AlertDto {
    id: string;
    userId: string;
    stockId: string;
    symbol: string;
    type: AlertType;
    threshold: number;
    isTriggered: boolean;
    lastTriggeredAt?: string;
    createdAt: string;
}

export interface CreateStockAlertRequest {
    symbol: string;
    type: AlertType;
    threshold: number;
}
