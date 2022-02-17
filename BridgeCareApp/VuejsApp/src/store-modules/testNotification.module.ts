const actions = {
    loadNotifications({ dispatch }: any) {
        dispatch('addSuccessNotification', {
            message: 'Test Success Notification.',
        });
        dispatch('addSuccessNotification', {
            message: 'Test Long Form Success Notification.',
            longMessage:
                'This is a long form message displayed in an inline block.',
        });
        dispatch('addWarningNotification', {
            message: 'Test Warning Notification.',
        });
        dispatch('addWarningNotification', {
            message: 'Test Long Form Warning Notification.',
            longMessage:
                'This is a long form message displayed in an inline block.',
        });
        dispatch('addErrorNotification', {
            message: 'Test Error Notification.',
        });
        dispatch('addErrorNotification', {
            message: 'Test Long Form Error Notification.',
            longMessage:
                'This is a long form message displayed in an inline block.',
        });
        dispatch('addInfoNotification', {
            message: 'Test Info Notification.',
        });
        dispatch('addInfoNotification', {
            message: 'Test Long Form Info Notification.',
            longMessage:
                'This is a long form message displayed in an inline block.',
        });
    },
};

export default { actions };
