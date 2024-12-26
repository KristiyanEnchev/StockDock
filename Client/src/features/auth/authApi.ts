import { createApi } from '@reduxjs/toolkit/query/react';
import { AuthResponse, LoginRequest, RegisterRequest } from '../../types/authTypes';
import { baseQuery } from './baseQueryConfig';

export const authApi = createApi({
    reducerPath: 'authApi',
    baseQuery,
    endpoints: (builder) => ({
        login: builder.mutation<AuthResponse, LoginRequest>({
            query: (credentials) => ({
                url: 'identity/login',
                method: 'POST',
                body: credentials,
            }),
        }),
        register: builder.mutation<AuthResponse, RegisterRequest>({
            query: (userData) => ({
                url: 'identity/register',
                method: 'POST',
                body: userData,
            }),
        }),
        logout: builder.mutation<void, string>({
            query: (email) => ({
                url: 'identity/logout',
                method: 'POST',
                body: { email },
            }),
        }),
        refresh: builder.mutation<AuthResponse, { email: string; refreshToken: string }>({
            query: (refreshData) => ({
                url: 'identity/refresh',
                method: 'POST',
                body: refreshData,
            }),
        }),
    }),
});

export const {
    useLoginMutation,
    useRegisterMutation,
    useLogoutMutation,
    useRefreshMutation,
} = authApi;
