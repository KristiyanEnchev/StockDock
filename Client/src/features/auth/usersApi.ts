import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { RootState } from '@/store';
import { Result } from '@/types/authTypes';

interface UserDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    isActive: boolean;
    roles: string[];
}

interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

interface UpdateEmailRequest {
    newEmail: string;
    currentPassword: string;
}

export const usersApi = createApi({
    reducerPath: 'usersApi',
    baseQuery: fetchBaseQuery({
        baseUrl: `${import.meta.env.VITE_API_BASE_URL}/api`,
        prepareHeaders: (headers, { getState }) => {
            const token = (getState() as RootState).auth.token;

            if (token) {
                headers.set('authorization', `Bearer ${token}`);
            }

            return headers;
        },
    }),
    tagTypes: ['User'],
    endpoints: (builder) => ({
        getUserById: builder.query<Result<UserDto>, string>({
            query: (id) => `/users/${id}`,
            providesTags: (_result, _error, id) => [{ type: 'User', id }],
        }),

        changePassword: builder.mutation<Result<string>, { id: string; request: ChangePasswordRequest }>({
            query: ({ id, request }) => ({
                url: `/users/${id}/change-password`,
                method: 'POST',
                body: request,
            }),
            invalidatesTags: (_result, _error, { id }) => [{ type: 'User', id }],
        }),

        updateEmail: builder.mutation<Result<string>, { id: string; request: UpdateEmailRequest }>({
            query: ({ id, request }) => ({
                url: `/users/${id}/update-email`,
                method: 'POST',
                body: request,
            }),
            invalidatesTags: (_result, _error, { id }) => [{ type: 'User', id }],
        }),

        getAllUsers: builder.query<Result<UserDto[]>, void>({
            query: () => '/users',
            providesTags: (result) =>
                result
                    ? [
                        ...result.data.map(({ id }) => ({ type: 'User' as const, id })),
                        { type: 'User', id: 'LIST' },
                    ]
                    : [{ type: 'User', id: 'LIST' }],
        }),

        getUsersByRole: builder.query<Result<UserDto[]>, string>({
            query: (role) => `/users/by-role/${role}`,
            providesTags: [{ type: 'User', id: 'LIST' }],
        }),

        deactivateUser: builder.mutation<Result<boolean>, string>({
            query: (id) => ({
                url: `/users/${id}/deactivate`,
                method: 'POST',
            }),
            invalidatesTags: (_result, _error, id) => [
                { type: 'User', id },
                { type: 'User', id: 'LIST' },
            ],
        }),

        reactivateUser: builder.mutation<Result<boolean>, string>({
            query: (id) => ({
                url: `/users/${id}/reactivate`,
                method: 'POST',
            }),
            invalidatesTags: (_result, _error, id) => [
                { type: 'User', id },
                { type: 'User', id: 'LIST' },
            ],
        }),
    }),
});

export const {
    useGetUserByIdQuery,
    useChangePasswordMutation,
    useUpdateEmailMutation,
    useGetAllUsersQuery,
    useGetUsersByRoleQuery,
    useDeactivateUserMutation,
    useReactivateUserMutation,
} = usersApi;
