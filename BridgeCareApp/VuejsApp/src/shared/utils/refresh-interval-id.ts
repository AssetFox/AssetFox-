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
    if (!hasValue(refreshIntervalID)) {
        refreshIntervalID = window.setInterval(checkBrowserTokens, 30000);
    }
};

const checkBrowserTokens = () => {
    store.dispatch('checkBrowserTokens').catch((err: any) => {
        store
            .dispatch('setErrorMessage', {
                message: err.errorMessage,
            })
            .then(() => onHandleLogout());
    });
};
