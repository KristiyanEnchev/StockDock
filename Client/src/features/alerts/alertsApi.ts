import { createApi } from '@reduxjs/toolkit/query/react';
import { AlertDto, CreateStockAlertRequest } from '@/types/alertTypes';
import { baseQueryWithReauth } from '@/features/auth/baseQueryWithReauth';

export const alertsApi = createApi({
    reducerPath: 'alertsApi',
    baseQuery: baseQueryWithReauth,
    tagTypes: ['Alerts'],
    endpoints: (builder) => ({
        getAlerts: builder.query<AlertDto[], void>({
            query: () => 'alerts',
            providesTags: ['Alerts']
        }),
        createAlert: builder.mutation<void, CreateStockAlertRequest>({
            query: (alert) => ({
                url: 'alerts',
                method: 'POST',
                body: alert
            }),
            invalidatesTags: ['Alerts']
        }),
        deleteAlert: builder.mutation<void, string>({
            query: (alertId) => ({
                url: `alerts/${alertId}`,
                method: 'DELETE'
            }),
            invalidatesTags: ['Alerts']
        })
    })
});

export const {
    useGetAlertsQuery,
    useCreateAlertMutation,
    useDeleteAlertMutation
} = alertsApi;
