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
    }
});

export const {
} = alertsSlice.actions;
