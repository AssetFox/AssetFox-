import { Notification } from '@/shared/models/iAM/notifications';

const state = {
    notifications: [] as Notification[],
    counter: 0,
    totalNotifications: 0,
};

const mutations = {
    addNotificationMutator(state: any, notification: Notification) {
        state.notifications.unshift(notification);
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
    addSuccessNotification({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-check-circle',
            iconColor: 'green',
            active: false,
            shortMessage: payload.message,
            longMessage:
                payload.longMessage || 'No further information provided.',
        };
        commit('addNotificationMutator', notification);
    },
    addWarningNotification({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'yellow',
            active: false,
            shortMessage: payload.message,
            longMessage:
                payload.longMessage || 'No further information provided.',
        };
        commit('addNotificationMutator', notification);
    },
    addErrorNotification({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'red',
            active: false,
            shortMessage: payload.message,
            longMessage:
                payload.longMessage || 'No further information provided.',
        };
        commit('addNotificationMutator', notification);
    },addErrorNotificationWithStackTrace({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'red',
            active: false,
            shortMessage: payload.message,
            longMessage:
                payload.longMessage || 'No further information provided.',
            stackTrace: payload.stackTrace
        };
        commit('addNotificationMutator', notification);
    },
    addInfoNotification({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-info-circle',
            iconColor: 'primary',
            active: false,
            shortMessage: payload.message,
            longMessage:
                payload.longMessage || 'No further information provided.',
        };
        commit('addNotificationMutator', notification);
    },
    addTaskCompletedNotification({ commit }: any, payload: any) {
        let notification: Notification = {
            id: state.totalNotifications + 1,
            icon: 'fas fa-exclamation-circle',
            iconColor: 'green',
            active: false,
            shortMessage: payload.message,
            longMessage: payload.longMessage || 'No further information provided',
        };
        commit('addNotificationMutator', notification);
    },
    removeNotification({ commit }: any, id: number) {
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
