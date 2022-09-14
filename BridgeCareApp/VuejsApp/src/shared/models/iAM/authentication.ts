export interface UserTokens {
    access_token: string;
    refresh_token: string;
    id_token: string;
    expires_in: number;
    token_type: string;
}

export interface UserInfo {
    sub: string;    
    HasAdminAccess: boolean;
    HasSimulationAccess: boolean;
    InternalRole: string;
    email: string;
}
