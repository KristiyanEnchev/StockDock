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
    }
});

export const {
} = alertsSlice.actions;
