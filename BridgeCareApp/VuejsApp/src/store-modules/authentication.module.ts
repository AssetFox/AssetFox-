import AuthenticationService from '../services/authentication.service';
import { AxiosResponse } from 'axios';
import { UserInfo, UserTokens } from '@/shared/models/iAM/authentication';
import { http2XX } from '@/shared/utils/http-utils';
import {
    checkLDAP,
    parseLDAP,
    regexCheckLDAP,
} from '@/shared/utils/parse-ldap';
import { hasValue } from '@/shared/utils/has-value-util';
import moment from 'moment';
import { isNil } from 'ramda';
import { SecurityTypes } from '@/shared/utils/security-types';

const state = {
    authenticated: false,
    hasRole: false,
    checkedForRole: false,
    isAdmin: false,
    isCWOPA: false,
    username: '',
    refreshing: false,
    securityType: process.env.SECURITY_TYPE,
    pennDotSecurityType: 'ESEC',
    azureSecurityType: 'B2C',
};

const mutations = {
    authenticatedMutator(state: any, status: boolean) {
        state.authenticated = status;
    },
    hasRoleMutator(state: any, status: boolean) {
        state.hasRole = status;
    },
    checkedForRoleMutator(state: any, status: boolean) {
        state.checkedForRole = status;
    },
    isAdminMutator(state: any, status: boolean) {
        state.isAdmin = status;
    },
    isCWOPAMutator(state: any, status: boolean) {
        state.isCWOPA = status;
    },
    usernameMutator(state: any, username: string) {
        state.username = username;
    },
    refreshingMutator(state: any, refreshing: boolean) {
        state.refreshing = refreshing;
    },
};

const actions = {
    async getUserTokens({ commit }: any, code: string) {
        await AuthenticationService.getUserTokens(code).then(
            (response: AxiosResponse) => {
                const expirationInMilliseconds = moment().add(30, 'minutes');
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const userTokens: UserTokens = response.data as UserTokens;
                    localStorage.setItem(
                        'UserTokens',
                        JSON.stringify(userTokens),
                    );
                    localStorage.setItem(
                        'TokenExpiration',
                        expirationInMilliseconds.valueOf().toString(),
                    );
                    commit('authenticatedMutator', true);
                }
            },
        )
        .catch(error => console.log(error));
    },

    async checkBrowserTokens({ dispatch }: any) {
        if (!hasValue(localStorage.getItem('UserTokens'))) {
            throw new Error('Cannot determine user authentication status');
        }

        const storedTokenExpiration: number = parseInt(
            localStorage.getItem('TokenExpiration') as string,
        );
        if (isNaN(storedTokenExpiration)) {
            throw new Error('Cannot determine user authentication status');
        }

        const currentDateTime = moment();
        const tokenExpirationDateTime = moment(storedTokenExpiration);
        const differenceInMinutes = tokenExpirationDateTime.diff(
            currentDateTime,
            'minutes',
        );

        if (differenceInMinutes > 2) {
            return;
        }

        dispatch('refreshTokens');
    },

    async refreshTokens({ commit }: any) {
        if (!hasValue(localStorage.getItem('UserTokens'))) {
            throw new Error('Cannot determine user authentication status');
        } else {
            commit('refreshingMutator', true);
            const userTokens: UserTokens = JSON.parse(
                localStorage.getItem('UserTokens') as string,
            ) as UserTokens;
            await AuthenticationService.refreshTokens(
                userTokens.refresh_token,
            ).then((response: AxiosResponse) => {
                const expirationInMilliseconds = moment().add(30, 'minutes');
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const userTokens: UserTokens = response.data as UserTokens;
                    localStorage.setItem(
                        'UserTokens',
                        JSON.stringify(userTokens),
                    );
                    localStorage.setItem(
                        'TokenExpiration',
                        expirationInMilliseconds.valueOf().toString(),
                    );
                }
            });
            commit('refreshingMutator', false);
        }
    },

    async getUserInfo({ commit, state }: any) {
        if (!hasValue(localStorage.getItem('UserTokens'))) {
            throw new Error('Cannot determine user authentication status');
        } else {
            const userTokens: UserTokens = JSON.parse(
                localStorage.getItem('UserTokens') as string,
            ) as UserTokens;
            await AuthenticationService.getUserInfo(
                userTokens.access_token,
            ).then((response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const userInfo: UserInfo = response.data as UserInfo;
                    localStorage.setItem('UserInfo', JSON.stringify(userInfo));
                    commit('usernameMutator', parseLDAP(userInfo.sub)[0]);

                    const hasRole: boolean = regexCheckLDAP(
                        userInfo.roles,
                        /PD-BAMS-(Administrator|CWOPA|PlanningPartner|DBEngineer)/,
                    );

                    commit('checkedForRoleMutator', hasRole);
                    commit('hasRoleMutator', hasRole);

                    if (hasRole) {
                        commit(
                            'isAdminMutator',
                            checkLDAP(userInfo.roles, 'PD-BAMS-Administrator'),
                        );
                        commit(
                            'isCWOPAMutator',
                            checkLDAP(userInfo.roles, 'PD-BAMS-CWOPA'),
                        );
                    }

                    if (!state.authenticated) {
                        commit('authenticatedMutator', true);
                    }
                } else {
                    if (state.authenticated) {
                        commit('authenticatedMutator', false);
                    }
                    throw new Error(
                        'Cannot determine user authentication status',
                    );
                }
            });
        }
    },

    async logOut({ commit }: any) {
        commit('usernameMutator', '');
        commit('authenticatedMutator', false);
        localStorage.removeItem('UserInfo');
        localStorage.removeItem('TokenExpiration');
        if (hasValue(localStorage.getItem('UserTokens'))) {
            await AuthenticationService.revokeIdToken();

            const userTokens: UserTokens = JSON.parse(
                localStorage.getItem('UserTokens') as string,
            ) as UserTokens;

            localStorage.removeItem('UserTokens');

            await AuthenticationService.revokeToken(
                userTokens.access_token,
                'Access',
            );
            await AuthenticationService.revokeToken(
                userTokens.refresh_token,
                'Refresh',
            );
        }
    },
    async setAzureUserInfo({ commit, dispatch }: any, payload: any) {
        if (payload.status) {
            commit('hasRoleMutator', true);
            commit('checkedForRoleMutator', true);
            commit('isAdminMutator', true);
            commit('usernameMutator', payload.username);
            commit('authenticatedMutator', true);
        } else {
            commit('hasRoleMutator', false);
            commit('checkedForRoleMutator', false);
            commit('isAdminMutator', false);
            commit('usernameMutator', '');
            commit('authenticatedMutator', false);
        }
    },
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
