import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AlertDto } from '../../types/alertTypes';

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
        alertsLoading: (state) => {
            state.loading = true;
            state.error = null;
        },
        alertError: (state, action: PayloadAction<string>) => {
            state.loading = false;
            state.error = action.payload;
        },
    }
});

export const {
} = alertsSlice.actions;
