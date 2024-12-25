import { jwtDecode } from "jwt-decode";
import { DecodedToken, User, AuthState } from "../types/authTypes";

export const AUTH_STORAGE_KEY = "auth";

export const parseUserFromToken = (token: string): User | null => {
    try {
        const decoded: DecodedToken = jwtDecode(token);
        return {
            id: decoded.Id,
            email: decoded.Email,
            firstName: decoded.FirstName,
            lastName: decoded.LastName,
            username: decoded.Username,
            roles: decoded.roles.split(',').map(role => role.trim()),
        };
    } catch (error) {
        console.error("Invalid token format:", error);
        return null;
    }
};

export const getInitialState = (): AuthState => ({
    user: null,
    token: null,
    refreshToken: null,
    refreshTokenExpiryTime: null,
    isAuthenticated: false,
    isLoading: false,
});

export const verifyToken = (token: string): boolean => {
    try {
        const decoded: DecodedToken = jwtDecode(token);
        const currentTime = Date.now() / 1000;
        return decoded.exp > currentTime;
    } catch {
        return false;
    }
};
