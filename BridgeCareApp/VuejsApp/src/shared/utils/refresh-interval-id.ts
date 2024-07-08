import { hasValue } from '@/shared/utils/has-value-util';
import store from '@/store/root-store';
import { onHandleLogout } from '@/shared/utils/authentication-utils';

let refreshIntervalID: any | null = null;

export const clearRefreshIntervalID = () => {
    if (hasValue(refreshIntervalID)) {
        window.clearInterval(refreshIntervalID);
    }
};

export const setRefreshIntervalID = () => {
    alert("in setRefreshIntervalID");
    if (!hasValue(refreshIntervalID)) {
        refreshIntervalID = window.setInterval(checkBrowserTokens, 30000);
    }
};

const checkBrowserTokens = () => {
    store.dispatch('checkBrowserTokens').catch((err: any) => {
        alert(checkBrowserTokens + " " + err);
        store
            .dispatch('addErrorNotification', {
                message: 'Browser token error',
                longMessage: err.errorMessage,
            })
            .then(() => onHandleLogout());
    });
};
