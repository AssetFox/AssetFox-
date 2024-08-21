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
    hasAdminAccess: false,
    hasSimulationAccess: false,
    username: '',
    refreshing: false,
    securityType: 'B2C',
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
    adminAccessMutator(state: any, status: boolean) {
        state.hasAdminAccess = status;
    },
    simulationAccessMutator(state: any, status: boolean) {
        state.hasSimulationAccess = status;
    },
    usernameMutator(state: any, username: string) {
        state.username = username;
    },
    refreshingMutator(state: any, refreshing: boolean) {
        state.refreshing = refreshing;
    },
};

const actions = {
    async getUserTokens({ commit }: any, payload: any) {
        await AuthenticationService.getUserTokens(payload.code).then(
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
        
        if (differenceInMinutes > 15) {
            return;
        }
       
        await dispatch('refreshTokens');
    },

// TODO check with ESEC not B2C
// On component load, check the token's time stamp, and set a delayed call to refresh the token when less than 5 minutes remain.
// TODO try checking and renwing interval in logOut? Or atleast on each component load in router.ts
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

                    const hasRole: boolean = true; //we can set it to true as at least default role is assigned to user

                    commit('checkedForRoleMutator', hasRole);
                    commit('hasRoleMutator', hasRole);

                    if (hasRole) {
                        commit(
                            'adminAccessMutator',
                            userInfo.hasAdminAccess,
                        );                       
                        commit(
                            'simulationAccessMutator',
                            userInfo.hasSimulationAccess,
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
    async setAzureUserInfo({ commit }: any, payload: any) {
        if (payload.status) {    
            // Check for active status
                var activeStatus = await AuthenticationService.getActiveStatus();
                if(activeStatus.data == true)
                    {
                        await AuthenticationService.getHasAdminAccess().then((response: AxiosResponse) => {
                            let hasAdminAccess: boolean = false;
                            if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                                hasAdminAccess = response.data as boolean;
                            }
                                commit('hasRoleMutator', true);
                                commit('checkedForRoleMutator', true);
                                commit('adminAccessMutator', hasAdminAccess);
                                commit('usernameMutator', payload.username);
                                commit('authenticatedMutator', true);
                                commit('simulationAccessMutator', false);
                        });
                    }
                    else
                    {
                        setCommits({ commit });
                        throw new Error('User is not active');
                    }
        } else {
            setCommits({ commit });
        }
    },
};

async function setCommits({ commit }: any)
{
    commit('hasRoleMutator', false);
    commit('checkedForRoleMutator', false);
    commit('adminAccessMutator', false);
    commit('usernameMutator', '');
    commit('authenticatedMutator', false);
    commit('simulationAccessMutator', false);
}

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
