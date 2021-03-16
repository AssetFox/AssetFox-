import {User} from '@/shared/models/iAM/user';
import {clone, findIndex, propEq, reject, update} from 'ramda';
import UserService from '@/services/user.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    users: [] as User[]
};

const mutations = {
    usersMutator(state: any, users: User[]) {
        state.users = clone(users);
    },
    updatedUserMutator(state: any, user: User) {
        state.users = update(
            findIndex(propEq('id', user.id), state.users),
            user, state.users
        );
    },
    deletedUserMutator(state: any, id: string) {
        state.users = reject(propEq('id', id), state.users);
    }
};

const actions = {
    async getAllUsers({commit}: any) {
        await UserService.getAllUsers()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('usersMutator', response.data as User[]);
                }
            });
    },
    async updateUser({commit, dispatch}: any, payload: any) {
        await UserService.updateUser(payload.user)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('updatedUserMutator', payload.user);
                    dispatch('setSuccessMessage', {message: 'Updated user'});
                }
            });
    },
    async deleteUser({commit, dispatch}: any, payload: any) {
        await UserService.deleteUser(payload.userId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedUserMutator', payload.userId);
                    dispatch('setSuccessMessage', {message: 'Deleted user'});
                }
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
