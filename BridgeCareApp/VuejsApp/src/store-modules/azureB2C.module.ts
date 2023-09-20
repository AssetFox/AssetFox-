import {
    acquireTokenConfig,
    azureB2CConfig,
    msalConfig,
    msalPasswordResetConfig,
} from '@/azure-b2c-config';
import * as msal from 'msal';
import { hasValue } from '@/shared/utils/has-value-util';
import store from '@/store/root-store';
import router from '@/router';

const state = {
    authenticatedFromAzure: false,
    app: new msal.UserAgentApplication(msalConfig),
    appPasswordReset: new msal.UserAgentApplication(msalPasswordResetConfig),
};

const mutations = {
    authenticatedMutator(state: any, status: boolean) {
        state.authenticatedFromAzure = status;
    },
};

const actions = {
    async azureB2CLogin({ dispatch }: any) {
        await state.app
            .loginPopup()
            .then((authResponse: msal.AuthResponse) => {
                if (
                    hasValue(authResponse.account) &&
                    hasValue(authResponse.account.name)
                ) {
                    dispatch('getAzureB2CAccessToken', authResponse).then(() =>
                        dispatch('getAzureAccountDetails').then(() => {
                            if (
                                // @ts-ignore
                                store.state.authenticationModule.authenticated
                            ) {
                                router.push('/Scenarios/');
                            }
                        }),
                    );
                }
            })
            .catch(async (error: any) => {             
                if(error.response == undefined || error.response.status == 500)
                {                   
                    dispatch('addErrorNotification', {
                        message: 'The authorization system is not available at the moment.',
                        longMessage: error.errorMessage == undefined ? 'Unknown Error' : error.errorMessage
                    });
                }
                
                dispatch('getAzureAccountDetails');

                if (
                    hasValue(error, 'errorMessage') &&
                    error.errorMessage.indexOf('AADB2C90118') !== -1
                ) {
                    await state.appPasswordReset
                        .loginPopup(azureB2CConfig.forgotPasswordAuthority)
                        .then(() => {
                            dispatch('addSuccessNotification', {
                                message: 'Password Reset.',
                                longMessage:
                                    'Password has been reset successfully. Please sign-in with your new password.',
                            });
                        });
                }
            });
    },
    async azureB2CLogout() {     
        localStorage.removeItem('access_token');
        localStorage.removeItem('LoggedInUser');
        state.app.logout();
    },
    async getAzureB2CAccessToken(
        { commit, dispatch }: any,
        authResponse?: msal.AuthResponse,
    ) {
        if (hasValue(authResponse) && hasValue(authResponse!.accessToken)) {
            localStorage.setItem('access_token', authResponse!.accessToken);
        } else {
            await state.app
                .acquireTokenSilent(acquireTokenConfig)
                .then((silentResp: msal.AuthResponse) => {
                    if (hasValue(silentResp.accessToken)) {
                        localStorage.setItem(
                            'access_token',
                            silentResp.accessToken,
                        );
                    }
                })
                .catch(async (error: any) => {
                    console.log(
                        'Silent token acquisition fail. Acquiring token using popup.',
                    );
                    console.log(`error: ${error}`);

                    await state.app
                        .acquireTokenPopup(acquireTokenConfig)
                        .then((popupResp: msal.AuthResponse) => {
                            if (hasValue(popupResp.accessToken)) {
                                localStorage.setItem(
                                    'access_token',
                                    popupResp.accessToken,
                                );
                            } else {
                                throw new Error('Could not authenticate user');
                            }
                        })
                        .catch((error: any) => {
                            throw new Error(error);
                        });
                });
        }
    },
    async getAzureAccountDetails({ dispatch }: any) {
        const accountDetails: msal.Account = await state.app.getAccount();
        if (hasValue(accountDetails.name)) {
            localStorage.setItem('LoggedInUser', accountDetails.name);
            dispatch('setAzureUserInfo', {
                status: true,
                username: accountDetails.name,
            });
        } else {
            localStorage.removeItem('LoggedInUser');
            dispatch('setAzureUserInfo', {
                status: false,
                username: '',
            });
        }
    },
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
