import {
  acquireTokenConfig,
  azureB2CConfig,
  msalConfig,
  msalPasswordResetConfig,
} from '@/azure-b2c-config';
import * as msal from 'msal';
import { hasValue } from '@/shared/utils/has-value-util';

const state = {
  authenticatedFromAzure: false,
  app: new msal.UserAgentApplication(msalConfig),
  appPasswordReset: new msal.UserAgentApplication(msalPasswordResetConfig)
};

const mutations = {
  authenticatedMutator(state: any, status: boolean) {
    state.authenticatedFromAzure = status;
  }
};

const actions = {
  async azureB2CLogin({dispatch}: any) {
    await state.app.loginPopup()
      .then((token: any) => {
        if (hasValue(token, 'account') && hasValue(token.account, 'name')) {
          dispatch('getAzureB2CAccessToken');
          dispatch('getAzureAccountDetails');
        }
      })
      .catch(async (error: any) => {
        dispatch('getAzureAccountDetails');

        if (hasValue(error, 'errorMessage') && error.errorMessage.indexOf('AADB2C90118') !== -1) {
          await state.appPasswordReset.loginPopup(azureB2CConfig.forgotPasswordAuthority)
            .then(() => {
              dispatch('setSuccessMessage', {message: 'Password has been reset successfully. Please sign-in with your new password.'});
            });
        }
      });
  },
  async azureB2CLogout() {
    await state.app.logout();
    localStorage.removeItem('LoggedInUser');
  },
  async getAzureB2CAccessToken({commit, dispatch}: any, payload: any) {
    await state.app.acquireTokenSilent(acquireTokenConfig)
      .then((result: any) => {
        if (hasValue(result, 'accessToken')) {
          localStorage.setItem('access_token', result.accessToken);
        }
      })
      .catch(async (error: any) => {
        console.log('Silent token acquisition fail. Acquiring token using popup.');
        console.log(`error: ${error}`);

        await state.app.acquireTokenPopup(payload.request)
          .then(tokenResponse => {
            console.log(`access_token: ${tokenResponse}`);
            return tokenResponse;
          });
      });
  },
  async getAzureAccountDetails({dispatch}: any) {
    const accountDetails: any = await state.app.getAccount();
    if (hasValue(accountDetails, 'name')) {
      localStorage.setItem('LoggedInUser', accountDetails.name);
      dispatch('setAzureUserInfo', {
        status: true, username: accountDetails.name
      });
    } else {
      if (hasValue(localStorage.getItem('LoggedInUser'))) {
        localStorage.removeItem('LoggedInUser');
        dispatch('setAzureUserInfo', {
          status: false, username: '',
        });
      }
    }
  }
};

const getters = {};

export default {
  state,
  getters,
  actions,
  mutations,
};