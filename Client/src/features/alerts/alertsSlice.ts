import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AlertDto } from '@/types/alertTypes';
import { RootState } from '@/store';

interface AlertsState {
    userAlerts: AlertDto[];
    triggeredAlerts: AlertDto[];
    loading: boolean;
    error: string | null;
}

const initialState: AlertsState = {
    userAlerts: [],
    triggeredAlerts: [],
    loading: false,
    error: null
};

export const alertsSlice = createSlice({
    name: 'alerts',
    initialState,
    reducers: {
        setUserAlerts: (state, action: PayloadAction<AlertDto[]>) => {
            state.userAlerts = action.payload;
            state.loading = false;
            state.error = null;
        },
        alertTriggered: (state, action: PayloadAction<AlertDto>) => {
            const index = state.userAlerts.findIndex(alert => alert.id === action.payload.id);
            if (index !== -1) {
                state.userAlerts[index] = action.payload;
            }
            if (!state.triggeredAlerts.some(alert => alert.id === action.payload.id)) {
                state.triggeredAlerts.unshift(action.payload);
                if (state.triggeredAlerts.length > 50) {
                    state.triggeredAlerts.pop();
                }
            }
        },
        alertCreated: (state, action: PayloadAction<AlertDto>) => {
            state.userAlerts.push(action.payload);
            state.loading = false;
            state.error = null;
        },
        alertDeleted: (state, action: PayloadAction<string>) => {
            state.userAlerts = state.userAlerts.filter(alert => alert.id !== action.payload);
            state.loading = false;
            state.error = null;
        },
        removeTriggeredAlert: (state, action: PayloadAction<string>) => {
            state.triggeredAlerts = state.triggeredAlerts.filter(alert => alert.id !== action.payload);
        },
        alertsLoading: (state) => {
            state.loading = true;
            state.error = null;
        },
        alertError: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.error = action.payload;
        },
        clearTriggeredAlerts: (state) => {
            state.triggeredAlerts = [];
        },
        clearAlerts: (state) => {
            state.userAlerts = [];
            state.triggeredAlerts = [];
            state.loading = false;
            state.error = null;
        }
    }
});

export const {
    setUserAlerts,
    alertTriggered,
    alertCreated,
    alertDeleted,
    removeTriggeredAlert,
    alertsLoading,
    alertError,
    clearTriggeredAlerts,
    clearAlerts
} = alertsSlice.actions;

export const selectUserAlerts = (state: RootState) => state.alerts.userAlerts;
export const selectTriggeredAlerts = (state: RootState) => state.alerts.triggeredAlerts;
export const selectAlertsLoading = (state: RootState) => state.alerts.loading;
export const selectAlertsError = (state: RootState) => state.alerts.error;

export default alertsSlice.reducer;
