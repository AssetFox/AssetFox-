import { Notification } from '@/shared/models/iAM/notifications';

const state = {
    notifications: [] as Notification[],
    counter: 5,
    totalNotifications: 0,
};

const mutations = {
    addNotificationMutator(state: any, notification: Notification) {
        state.notifications.push(notification);
        state.counter += 1;
        state.totalNotifications += 1;
    },

    removeNotificationMutator(state: any, id: number) {
        state.notifications = state.notifications.filter(
            (x: Notification) => x.id != id,
        );
    },

    counterResetMutator(state: any) {
        state.counter = 0;
    },
};

const actions = {
    addSuccessNotification(
        { commit }: any,
        shortMessage: string,
        longMessage?: string,
    ) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-check-circle',
            iconColor: 'green',
            active: false,
            shortMessage: shortMessage,
            longMessage: longMessage,
        };
        commit('addNotificationMutator', notification);
    },
    addWarningNotification(
        { commit }: any,
        shortMessage: string,
        longMessage?: string,
    ) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'yellow',
            active: false,
            shortMessage: shortMessage,
            longMessage: longMessage || shortMessage,
        };
        commit('addNotificationMutator', notification);
    },
    addErrorNotification(
        { commit }: any,
        shortMessage: string,
        longMessage?: string,
    ) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'red',
            active: false,
            shortMessage: shortMessage,
            longMessage: longMessage || shortMessage,
        };
        commit('addNotificationMutator', notification);
    },
    addInfoNotification(
        { commit }: any,
        shortMessage: string,
        longMessage?: string,
    ) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-info-circle',
            iconColor: 'primary',
            active: false,
            shortMessage: shortMessage,
            longMessage: longMessage || shortMessage,
        };
        commit('addNotificationMutator', notification);
    },
    removeNofitication({ commit }: any, id: number) {
        commit('removeNotificationMutator', id);
    },
    clearNotificationCounter({ commit }: any) {
        commit('counterResetMutator');
    },
};

export default {
    state,
    mutations,
    actions,
};
