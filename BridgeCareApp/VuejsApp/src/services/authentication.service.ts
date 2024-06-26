import {AxiosPromise} from 'axios';
import {API, coreAxiosInstance} from '@/shared/utils/axios-instance';

export default class AuthenticationService {
    static getUserTokens(code: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Authentication}/UserTokens/${code}`);
    }

    static refreshTokens(refreshToken: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Authentication}/RefreshToken/${refreshToken}`);
    }

    static getUserInfo(token: string): AxiosPromise {
        return coreAxiosInstance.get(`${API.Authentication}/UserInfo/${token}`);
    }

    static revokeToken(token: string, tokenType: string): AxiosPromise {
        return coreAxiosInstance.post(`${API.Authentication}/RevokeToken/${tokenType}/${token}`, null);
    }

    static revokeIdToken(): AxiosPromise {
        return coreAxiosInstance.post(`${API.Authentication}/RevokeToken/Id`, null);
    }

    static getHasAdminAccess(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Authentication}/GetHasAdminAccess`);
    }

    static getActiveStatus(): AxiosPromise {
        return coreAxiosInstance.get(`${API.Authentication}/GetActiveStatus`);
    }

}
