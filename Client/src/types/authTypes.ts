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
