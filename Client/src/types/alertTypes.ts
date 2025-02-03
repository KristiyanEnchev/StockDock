export enum AlertType {
    PriceAbove = 0,
    PriceBelow = 1,
    PercentageChange = 2
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
