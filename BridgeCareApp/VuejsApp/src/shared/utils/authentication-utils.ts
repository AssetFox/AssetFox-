import store from '@/store/root-store';
import { SecurityTypes } from '@/shared/utils/security-types';
import { clearRefreshIntervalID } from '@/shared/utils/refresh-interval-id';
import router from '@/router';
import { UnsecuredRoutePathNames } from '@/shared/utils/route-paths';
import AuthenticationService from '@/services/authentication.service';

const isAuthenticatedEsecUser = () => {
    alert('in isAuthenticatedEsecUser');
    return store
        .dispatch('checkBrowserTokens')
        .then(async () => {
            // Check user active status via API
            const activeStatus = await AuthenticationService.getActiveStatus();
            
            if (activeStatus.data = true) {
                // User is active, proceed with fetching user info and other data
                return store.dispatch('getUserInfo').then(() =>
                    store.dispatch('getUserCriteriaFilter').then(() => {
                        // @ts-ignore
                        if (store.state.authenticationModule.authenticated) {
                            return true;
                        } else {
                            throw new Error('Failed to authenticate');
                        }
                    })
                );
            } else {
                // User is not active, handle accordingly
                throw new Error('User is not active');
            }
        })
        .catch((error) => {
            store
                .dispatch('addErrorNotification', {
                    message: 'Authentication Error.',
                    longMessage: error,
                })
                .then(() => {
                    return false;
                });
        });
};

const isAuthenticatedAzureUser = () => {
    return store
        .dispatch('getAzureB2CAccessToken')
        .then(() =>
            store.dispatch('getAzureAccountDetails').then(() => {
                // @ts-ignore
                if (store.state.authenticationModule.authenticated) {
                    return true;
                } else {
                    throw new Error('Failed to authenticate');
                }
            }),
        )
        .catch((error: any) => {
            store
                .dispatch('addErrorNotification', {
                    message: 'Authenticaton Error.',
                    longMessage: error,
                })
                .then(() => {
                    return false;
                });
        });
};

export const isAuthenticatedUser = () => {
    // @ts-ignore
    if (store.state.authenticationModule.securityType === SecurityTypes.esec) {
        return isAuthenticatedEsecUser();
    }

    return isAuthenticatedAzureUser();
};

const onLogout = () => {
    alert('onLogout');
    store.dispatch('logOut').then(() => {
        clearRefreshIntervalID(); 
        // @ts-ignore
        if (window.location.host.toLowerCase().indexOf('penndot.gov') === -1 && store.state.authenticationModule.securityType === SecurityTypes.esec) {
            /*
             * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
             * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
             * to allow the landing page to redirect the browser to the original host.
             */
            window.location.href =
                'http://www.bamssyst.penndot.gov/iAM?host=' +
                encodeURI(window.location.host);
        } else {
            if (
                UnsecuredRoutePathNames.indexOf(router.currentRoute.path) === -1
            ) {
                router.push('/iAM/');
            }
        }
    });
};

const onAzureLogout = () => {
    store.dispatch('azureB2CLogout').then(() => onLogout());
};

export const onHandleLogout = () => {
    if (
        // @ts-ignore
        store.state.authenticationModule.securityType === SecurityTypes.esec
    ) {
        onLogout();
    } else {
        onAzureLogout();
    }
};
