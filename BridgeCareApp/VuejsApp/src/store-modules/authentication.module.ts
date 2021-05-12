import AuthenticationService from '../services/authentication.service';
import { AxiosResponse } from 'axios';
import { UserInfo, UserTokens } from '@/shared/models/iAM/authentication';
import { http2XX } from '@/shared/utils/http-utils';
import { checkLDAP, parseLDAP, regexCheckLDAP } from '@/shared/utils/parse-ldap';
import { hasValue } from '@/shared/utils/has-value-util';
import moment from 'moment';

const state = {
  authenticated: false,
  hasRole: false,
  checkedForRole: false,
  isAdmin: false,
  isCWOPA: false,
  username: '',
  refreshing: false,
  securityType: 'B2C',
  pennDotSecurityType: 'ESEC',
  azureSecurityType: 'B2C'
};

const mutations = {
  authenticatedMutator(state: any, status: boolean) {
    state.authenticated = status;
  },
  hasRoleMutator(state: any, status: boolean) {
    state.hasRole = status;
  },
  checkedForRoleMutator(state: any, status: boolean) {
    state.checkedForRole = status;
  },
  isAdminMutator(state: any, status: boolean) {
    state.isAdmin = status;
  },
  isCWOPAMutator(state: any, status: boolean) {
    state.isCWOPA = status;
  },
  usernameMutator(state: any, username: string) {
    state.username = username;
  },
  refreshingMutator(state: any, refreshing: boolean) {
    state.refreshing = refreshing;
  },
};

const actions = {
  async getUserTokens({ commit }: any, code: string) {
    await AuthenticationService.getUserTokens(code)
      .then((response: AxiosResponse) => {
        const expirationInMilliseconds = moment().add(30, 'minutes');
        if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
          const userTokens: UserTokens = response.data as UserTokens;
          localStorage.setItem('UserTokens', JSON.stringify(userTokens));
          localStorage.setItem('TokenExpiration', expirationInMilliseconds.valueOf().toString());
          commit('authenticatedMutator', true);
        }
      });
  },

  async checkBrowserTokens({ commit, dispatch, state }: any, code: string) {
    const storedTokenExpiration: number = parseInt(localStorage.getItem('TokenExpiration') as string);
    if (isNaN(storedTokenExpiration)) {
      return;
    }

    const currentDateTime = moment();
    const tokenExpirationDateTime = moment(storedTokenExpiration);
    const differenceInMinutes = tokenExpirationDateTime.diff(currentDateTime, 'minutes');

    if (differenceInMinutes > 2) {
      return;
    } else if (differenceInMinutes <= 2) {
        dispatch('refreshTokens');
    } else if (state.authenticated) {
      dispatch('logOut');
    }
  },

  async refreshTokens({ commit, dispatch }: any) {
    if (!localStorage.getItem('UserTokens')) {
      dispatch('logOut');
    } else {
      commit('refreshingMutator', true);
      const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
      await AuthenticationService.refreshTokens(userTokens.refresh_token)
        .then((response: AxiosResponse) => {
          const expirationInMilliseconds = moment().add(30, 'minutes');
          if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
            const userTokens: UserTokens = response.data as UserTokens;
            localStorage.setItem('UserTokens', JSON.stringify(userTokens));
            localStorage.setItem('TokenExpiration', expirationInMilliseconds.valueOf().toString());
          }
        });
      commit('refreshingMutator', false);
    }
  },

  async getUserInfo({ commit, dispatch, state }: any) {
    if (!localStorage.getItem('UserTokens')) {
      dispatch('logOut');
    } else {
      const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
      await AuthenticationService.getUserInfo(userTokens.access_token)
        .then((response: AxiosResponse) => {
          if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
            const userInfo: UserInfo = response.data as UserInfo;
            localStorage.setItem('UserInfo', JSON.stringify(userInfo));
            const username: string = parseLDAP(userInfo.sub)[0];
            commit('hasRoleMutator', regexCheckLDAP(userInfo.roles, /PD-BAMS-(Administrator|CWOPA|PlanningPartner|DBEngineer)/));

            if (state.hasRole) {
              commit('isAdminMutator', checkLDAP(userInfo.roles, 'PD-BAMS-Administrator'));
              commit('isCWOPAMutator', checkLDAP(userInfo.roles, 'PD-BAMS-CWOPA'));
            }

            commit('checkedForRoleMutator', true);
            commit('usernameMutator', username);

            if (!state.authenticated) {
              commit('authenticatedMutator', true);
            }
          } else {
            dispatch('logOut');
          }
        });
    }
  },

  async logOut({ commit }: any) {
    if (!localStorage.getItem('UserTokens')) {
      commit('usernameMutator', '');
      commit('authenticatedMutator', false);
    } else {
      localStorage.removeItem('UserInfo');
      const userTokens: UserTokens = JSON.parse(localStorage.getItem('UserTokens') as string) as UserTokens;
      localStorage.removeItem('UserTokens');
      localStorage.removeItem('TokenExpiration');
      AuthenticationService.revokeToken(userTokens.access_token, 'Access');
      AuthenticationService.revokeToken(userTokens.refresh_token, 'Refresh');
      commit('usernameMutator', '');
      commit('authenticatedMutator', false);
    }
  },
  async setAzureUserInfo({ commit, dispatch }: any, payload: any) {
    if (payload.status) {
      commit('hasRoleMutator', true);
      commit('checkedForRoleMutator', true);
      commit('isAdminMutator', true);
      commit('usernameMutator', payload.username);
    } else {
      commit('hasRoleMutator', false);
      commit('checkedForRoleMutator', false);
      commit('isAdminMutator', false);
      commit('usernameMutator', '');
      dispatch('azureB2CLogout');
    }

    if (state.authenticated !== payload.status) {
      commit('authenticatedMutator', payload.status);
    }
  },
  async checkAzureB2CBrowserTokens({commit, dispatch}: any) {
    const storedTokenExpiration: number = Number(localStorage.getItem('TokenExpiration') as string);
    if (isNaN(storedTokenExpiration)) {
      return;
    }

    if (storedTokenExpiration > Date.now()) {
      if (state.authenticated) {
        return;
      }
      commit('authenticatedMutator', true);
    } else if (state.authenticated) {
      dispatch('logOut');
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
