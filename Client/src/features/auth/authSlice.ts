import { createSlice, PayloadAction, createAsyncThunk } from '@reduxjs/toolkit';
import type { RootState } from '../../store';
import { AuthState, AuthResponse } from '../../types/authTypes';
import { parseUserFromToken, verifyToken } from '../../lib/authUtils';
import { AUTH_STORAGE_KEY } from './../../lib/authUtils';
import { authApi } from './authApi';

const getInitialState = (): AuthState => ({
    user: null,
    token: null,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    isAuthenticated: false,
    isLoading: false,
});

const loadState = (): AuthState => {
    try {
        const authState = localStorage.getItem(AUTH_STORAGE_KEY);
        if (authState) {
            const parsedState = JSON.parse(authState);
            if (parsedState.token && !verifyToken(parsedState.token)) {
                return getInitialState();
            }
            return parsedState;
        }
    } catch (e) {
        console.error('Error loading auth state:', e);
    }
    return getInitialState();
};

export const logoutUser = createAsyncThunk(
    'auth/logoutUser',
    async (_, { getState, dispatch }) => {
        const state = getState() as RootState;
        const userEmail = state.auth.user?.email;

        if (userEmail) {
            try {
                await dispatch(authApi.endpoints.logout.initiate(userEmail)).unwrap();
            } catch (error) {
                console.error('Server logout failed:', error);
            }
        }
        dispatch(authSlice.actions.clearAuth());
    }
);

export const authSlice = createSlice({
    name: 'auth',
    initialState: loadState(),
    reducers: {
        setCredentials: (state, action: PayloadAction<AuthResponse>) => {
            const user = parseUserFromToken(action.payload.accessToken);
            if (user) {
                state.user = user;
                state.token = action.payload.accessToken;
                state.refreshToken = action.payload.refreshToken;
                state.refreshTokenExpiryTime = action.payload.refreshTokenExpiryTime;
                state.isAuthenticated = true;
                localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(state));
            }
        },
        clearAuth: (state) => {
            Object.assign(state, getInitialState());
            localStorage.removeItem(AUTH_STORAGE_KEY);
        },
        setAuthLoading: (state, action: PayloadAction<boolean>) => {
            state.isLoading = action.payload;
        },
    },
});

export const { setCredentials, clearAuth, setAuthLoading } = authSlice.actions;

export const selectCurrentUser = (state: RootState) => state.auth.user;
export const selectIsAuthenticated = (state: RootState) => state.auth.isAuthenticated;
export const selectAuthLoading = (state: RootState) => state.auth.isLoading;
export const selectToken = (state: RootState) => state.auth.token;

export default authSlice.reducer;
