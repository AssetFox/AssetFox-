import AuthenticationService from '../services/authentication.service';
import {AxiosResponse} from 'axios';
import {UserInformation} from '@/shared/models/iAM/user-information';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    loginFailed: true,
    userName: '',
    userId: '',
    userAccessToken: '',
    userIdToken: '',
    userRoles: [] as Array<string>
};

const mutations = {
    loginMutator(state: any, status: boolean) {
        state.loginFailed = status;
    },
    userNameMutator(state: any, userName: string) {
        state.userName = userName;
    },
    userIdMutator(state: any, userId: string) {
        state.userId = userId;
    },
    userIdTokenMutator(state: any, userIdToken: string) {
        state.userIdToken = userIdToken;
    },
    userAccessTokenMutator(state: any, userAccessToken: string) {
        state.userAccessToken = userAccessToken;
    }
};

const actions = {
    async authenticateUser({commit}: any) {
        return await AuthenticationService.authenticateUser()
            .then((response: AxiosResponse<UserInformation>) => {
                if (hasValue(response, 'data')) {
                    commit('loginMutator', false);
                    commit('userNameMutator', response.data.name);
                    commit('userIdMutator', response.data.id);
                }
            });
    },

    async getUserTokens({commit}: any, code: string) {
        return await AuthenticationService.getUserTokens(code)
            .then((response: AxiosResponse<string>) => {
                const jsonResponse: any = JSON.parse(response.data);
                commit('loginMutator', false);
                commit('userIdTokenMutator', jsonResponse.id_token);
                commit('userAccessTokenMutator', jsonResponse.access_token);
            });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
