import { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import type { RootState } from '../../store';
import { AuthResponse } from '../../types/authTypes';
import { setCredentials, clearAuth } from '../auth/authSlice';
import { baseQuery } from './baseQueryConfig';

export const baseQueryWithReauth: BaseQueryFn<
    string | FetchArgs,
    unknown,
    FetchBaseQueryError
> = async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions);

    if (result.error && result.error.status === 401) {
        const state = api.getState() as RootState;
        const refreshToken = state.auth.refreshToken;
        const userEmail = state.auth.user?.email;

        if (refreshToken && userEmail) {
            const refreshResult = await baseQuery(
                {
                    url: 'identity/refresh',
                    method: 'POST',
                    body: {
                        email: userEmail,
                        refreshToken: refreshToken,
                    },
                },
                api,
                extraOptions
            );

            if (refreshResult.data) {
                api.dispatch(setCredentials(refreshResult.data as AuthResponse));
                result = await baseQuery(args, api, extraOptions);
            } else {
                api.dispatch(clearAuth());
            }
        } else {
            api.dispatch(clearAuth());
        }
    }

    return result;
};
