import {User} from '@/shared/models/iAM/user';
import {clone, findIndex, propEq, reject, update} from 'ramda';
import UserService from '@/services/user.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {http2XX} from '@/shared/utils/http-utils';
import { emptyUserCriteriaFilter, UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';

const state = {
    users: [] as User[],
    usersCriteriaFilter: [] as UserCriteriaFilter[],
    currentUserCriteriaFilter: clone(emptyUserCriteriaFilter) as UserCriteriaFilter
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
    },
/////////////////////
    usersCriteriaFilterMutator(state: any, usersFilter: UserCriteriaFilter[]) {
        state.usersCriteriaFilter = clone(usersFilter);
    },
    currentUserCriteriaFilterMutator(state: any, currentUsersFilter: UserCriteriaFilter){
        state.currentUserCriteriaFilter = clone(currentUsersFilter);
    },
    updatedusersCriteriaFilterMutator(state: any, userFilter: UserCriteriaFilter) {

        var criteriaIndex = findIndex(propEq('userId', userFilter.userId), state.usersCriteriaFilter);
        if(criteriaIndex != -1){
            state.usersCriteriaFilter[criteriaIndex].criteria = userFilter.criteria;
            state.usersCriteriaFilter[criteriaIndex].hasAccess = userFilter.hasAccess;
            state.usersCriteriaFilter[criteriaIndex].hasCriteria = userFilter.hasCriteria;
        }
        else{
            state.usersCriteriaFilter.push(userFilter);
        }

        var index = findIndex(propEq('id', userFilter.userId), state.users);
        if(index != -1){
            var data = state.users[index];
            data.hasInventoryAccess = userFilter.hasAccess;
            state.users = update(index,
                data, state.users
            );
        }
    },
    deletedusersCriteriaFilterMutator(state: any, id: string) {

        var index = findIndex(propEq('criteriaId', id), state.usersCriteriaFilter);
        var currUserFilter = state.usersCriteriaFilter[index];

        state.usersCriteriaFilter = reject(propEq('criteriaId', id), state.usersCriteriaFilter);

        var index = findIndex(propEq('id', currUserFilter.userId), state.users);
        if(index != -1){
            var data = state.users[index];
            data.hasInventoryAccess = false;
            state.users = update(index,
                data, state.users
            );
        }
    },
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
    },
//////////////////////////////
    async getUserCriteriaFilter({commit, dispatch}: any) {
        const message = 'You do not have access to any bridge data. \
        Please contact an administrator to gain access to the data you need.';
        await UserService.getUserCriteriaFilterData()
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    if (!response.data.hasAccess) {
                        dispatch('setInfoMessage', {message});
                    }
                    commit('currentUserCriteriaFilterMutator', response.data as UserCriteriaFilter);
                    commit('checkedForRoleMutator', response.data.hasAccess);
                }
            });
    },

    async getAllUserCriteriaFilter({commit}: any) {
        await UserService.getAllUsersCriteriaFilterData()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit('usersCriteriaFilterMutator', response.data as UserCriteriaFilter[]);
                }
            });
    },
    async updateUserCriteriaFilter({commit, dispatch}: any, payload: any) {
        await UserService.updateUserCriteriaFilterData(payload.userCriteriaFilter)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('updatedusersCriteriaFilterMutator', payload.userCriteriaFilter);
                    dispatch('setSuccessMessage', {message: 'Updated user criteria filter'});
                }
            });
    },
    async deleteUserCriteriaFilter({commit, dispatch}: any, payload: any) {
        await UserService.deleteUserCriteriaFilterData(payload.userCriteriaId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedusersCriteriaFilterMutator', payload.userCriteriaId);
                    dispatch('setSuccessMessage', {message: 'Deleted user criteria filter'});
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
