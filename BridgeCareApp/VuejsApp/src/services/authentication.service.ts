import {AxiosPromise, AxiosResponse} from 'axios';
import {API, axiosInstance, coreAxiosInstance} from '@/shared/utils/axios-instance';
import {hasValue} from '@/shared/utils/has-value-util';

export default class AuthenticationService {
    static getUserTokens(code: string): AxiosPromise {
        /*return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.get(`/authentication/UserTokens/${code}`)
                .then((response: AxiosResponse<string>) => {
                    if (hasValue(response, 'data')) {
                        return resolve(response);
                    }
                })
                .catch((err: any) => {
                    return resolve(err);
                });
        });*/
        return coreAxiosInstance.get(`${API.Authentication}/UserTokens/${code}`);
    }

    static refreshTokens(refreshToken: string): AxiosPromise {
        /*return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.get(`/authentication/RefreshToken/${refreshToken}`)
                .then((response: AxiosResponse<string>) => {
                    if (hasValue(response, 'data')) {
                        return resolve(response);
                    }
                })
                .catch((err: any) => {
                    return resolve(err);
                });
        });*/
        return coreAxiosInstance.get(`${API.Authentication}/RefreshToken/${refreshToken}`);
    }

    static getUserInfo(token: string): AxiosPromise {
        /*return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.get(`/authentication/UserInfo/${token}`)
                .then((response: AxiosResponse<string>) => {
                    if (hasValue(response, 'data')) {
                        return resolve(response);
                    }
                })
                .catch((err: any) => {
                    return resolve(err);
                });
        });*/
        return coreAxiosInstance.get(`${API.Authentication}/UserInfo/${token}`);
    }

    static revokeToken(token: string, tokenType: string): AxiosPromise {
        /*return new Promise<AxiosResponse>((resolve) => {
            axiosInstance.post(`/authentication/RevokeToken/${tokenType}/${token}`, null)
                .then((response: AxiosResponse<any>) => {
                    return resolve(response);
                })
                .catch((err: any) => {
                    return resolve(err);
                });
        });*/
        return coreAxiosInstance.post(`${API.Authentication}/RevokeToken/${tokenType}/${token}`, null);
    }
}
