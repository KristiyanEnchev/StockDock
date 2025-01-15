import { createSlice } from '@reduxjs/toolkit';
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
    }
});

export const {
} = alertsSlice.actions;
