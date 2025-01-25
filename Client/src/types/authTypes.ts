export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    refreshTokenExpiryTime: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface DecodedToken {
    Id: string;
    Email: string;
    FirstName: string;
    LastName: string;
    Username: string;
    ip: string;
    roles: string;
    exp: number;
    iss: string;
    aud: string;
}

export interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    username: string;
    roles: string[];
}

export interface AuthState {
    user: User | null;
    token: string | null;
    refreshToken: string | null;
    refreshTokenExpiryTime: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
}

export interface Result<T> {
    success: boolean;
    data: T;
    errors: string[] | null;
    message?: string;
}
