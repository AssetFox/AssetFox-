import { Notification } from "@/shared/models/iAM/notifications";

const state = {
    notifications: [] as Notification[],
    counter: 0
};

const mutations = {
    notificationMessagesMutator(state: any, message: Notification) {
        state.notifications.push(message);
        state.counter += 1;
    },

    counterResetMutator(state: any) {
        state.counter = 0;
    }
};

const actions = {
    addNotificationMessage({commit}: any, payload: any) {
        commit('notificationMessagesMutator', payload.message);
    },
    clearNotificationCounter({commit}: any) {
        commit('counterResetMutator');
    },
};

export default {
    state,
    mutations,
    actions
};