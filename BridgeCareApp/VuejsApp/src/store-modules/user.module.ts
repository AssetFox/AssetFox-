import { User, UserNewsAccessDate, emptyUser } from '@/shared/models/iAM/user';
import { clone, findIndex, propEq, reject, update } from 'ramda';
import UserService from '@/services/user.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import {
    emptyUserCriteriaFilter,
    UserCriteriaFilter,
} from '@/shared/models/iAM/user-criteria-filter';
import store from '@/store/root-store';

const state = {
    users: [] as User[],
    usersCriteriaFilter: [] as UserCriteriaFilter[],
    currentUserCriteriaFilter: clone(
        emptyUserCriteriaFilter,
    ) as UserCriteriaFilter,
    currentUser: clone(emptyUser),
    checkedForCriteria: false,
};

const mutations = {
    usersMutator(state: any, users: User[]) {
        state.users = clone(users);
    },
    deletedUserMutator(state: any, id: string) {
        // this is to delete the user entry from criteria filter. Because the user is getting deleted from the system
        state.usersCriteriaFilter = reject(
            propEq('userId', id),
            state.usersCriteriaFilter,
        );

        state.users = reject(propEq('id', id), state.users);
    },
    currentUserMutator(state: any, currentUser: User) {
        state.currentUser = currentUser;
    },
    usersCriteriaFilterMutator(state: any, usersFilter: UserCriteriaFilter[]) {
        state.usersCriteriaFilter = clone(usersFilter);
    },
    currentUserCriteriaFilterMutator(
        state: any,
        currentUsersFilter: UserCriteriaFilter,
    ) {
        state.currentUserCriteriaFilter = clone(currentUsersFilter);
    },
    updatedUserCriteriaFilterMutator(
        state: any,
        userFilter: UserCriteriaFilter,
    ) {
        var criteriaIndex = findIndex(
            propEq('userId', userFilter.userId),
            state.usersCriteriaFilter,
        );
        if (criteriaIndex != -1) {
            state.usersCriteriaFilter[criteriaIndex].criteria =
                userFilter.criteria;
            state.usersCriteriaFilter[criteriaIndex].hasAccess =
                userFilter.hasAccess;
            state.usersCriteriaFilter[criteriaIndex].hasCriteria =
                userFilter.hasCriteria;
        } else {
            state.usersCriteriaFilter.push(userFilter);
        }

        var index = findIndex(propEq('id', userFilter.userId), state.users);
        if (index != -1) {
            var data = state.users[index];
            data.hasInventoryAccess = userFilter.hasAccess;
            state.users = update(index, data, state.users);
        }
    },
    revokeUsersCriteriaFilterMutator(state: any, id: string) {
        var index = findIndex(
            propEq('criteriaId', id),
            state.usersCriteriaFilter,
        );
        var currUserFilter = state.usersCriteriaFilter[index];

        state.usersCriteriaFilter = reject(
            propEq('criteriaId', id),
            state.usersCriteriaFilter,
        );

        var index = findIndex(propEq('id', currUserFilter.userId), state.users);
        if (index != -1) {
            var data = state.users[index];
            data.hasInventoryAccess = false;
            state.users = update(index, data, state.users);
        }
    },
    checkedForCriteriaMutator(state: any, checked: boolean) {
        state.checkedForCriteria = checked;
    }
};

const actions = {
    async getAllUsers({ commit }: any) {
        await UserService.getAllUsers().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('usersMutator', response.data as User[]);
                }
            },
        );
    },

    async deleteUser({ commit, dispatch }: any, payload: any) {
        await UserService.deleteUser(payload.userId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    commit('deletedUserMutator', payload.userId);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted user',
                    });
                }
            },
        );
    },
    async getUserCriteriaFilter({ commit, dispatch }: any) {
            // @ts-ignore
            if (!store.state.userModule.checkedForCriteria) {
            const message =
                'You do not have access to any bridge data. \
                Please contact an administrator to gain access to the data you need.';

            await UserService.getUserCriteriaFilterData().then(
                (response: AxiosResponse) => {
                    if (hasValue(response, 'data')) {
                        commit('checkedForCriteriaMutator', true);
                        const userCriteriaFilter: UserCriteriaFilter = response.data as UserCriteriaFilter;
                        commit(
                            'currentUserCriteriaFilterMutator',
                            userCriteriaFilter,
                        );
                        if (
                            !userCriteriaFilter.hasAccess &&
                            // @ts-ignore
                            !store.state.authenticationModule.hasAdminAccess
                        ) {
                            dispatch('addInfoNotification', {
                                message: 'Access Denied.',
                                longMessage: message,
                            });
                        }
                    }
                },
            );
        }
    },

    async getAllUserCriteriaFilter({ commit }: any) {
        await UserService.getAllUsersCriteriaFilterData().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'usersCriteriaFilterMutator',
                        response.data as UserCriteriaFilter[],
                    );
                }
            },
        );
    },
    async updateUserCriteriaFilter({ commit, dispatch }: any, payload: any) {
        await UserService.updateUserCriteriaFilterData(
            payload.userCriteriaFilter,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'updatedUserCriteriaFilterMutator',
                    payload.userCriteriaFilter,
                );
                dispatch('addSuccessNotification', {
                    message: 'Updated user criteria filter',
                });
            }
        });
    },
    async revokeUserCriteriaFilter({ commit, dispatch }: any, payload: any) {
        await UserService.revokeUserCriteriaFilterData(
            payload.userCriteriaId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'revokeUsersCriteriaFilterMutator',
                    payload.userCriteriaId,
                );
                dispatch('addSuccessNotification', {
                    message: 'Deleted user criteria filter',
                });
            }
        });
    },
    async updateUserLastNewsAccessDate({dispatch}: any, payload: any) {
        let data: UserNewsAccessDate = {
            id: payload.id,
            lastNewsAccessDate: payload.accessDate
        }

        await UserService.updateLastNewsAccessDate(
            data
        ).then((response: AxiosResponse) => {
            if (
                !(hasValue(response, 'status') &&
                http2XX.test(response.status.toString()))
            ) {
                dispatch('addErrorNotification', {
                    message: 'Failed to update news notification to server.',
                });
            }
        });
    },
    async getCurrentUserByUserName ({dispatch, commit}: any, userName: string) {
        await UserService.getUserByUserName(userName).then(
        (response: AxiosResponse<any>) => {
            if (
                hasValue(response, 'data') &&
                http2XX.test(response.status.toString())
            ) {
                commit('currentUserMutator', response.data as User);
            }
            else {
                dispatch('addErrorNotification', {
                    message: 'Failed to get current user from server.',
                });
            }     
        },)
    },
};

const getters = {
    getUserNameById: (state: any) => (id: string) => {
        var userIndex = findIndex(
            propEq('id', id),
            state.users,
        );

        return state.users[userIndex].username;
    },
    getIdByUserName: (state: any) => (username: string) => {
        var userIndex = findIndex(
            propEq('username', username),
            state.users,
        );

        return state.users[userIndex].id;
    },
};

export default {
    state,
    getters,
    actions,
    mutations,
};
