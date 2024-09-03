// store/modules/config.ts

export interface ConfigState {
  config: Config | null;
}

export interface Config {
  authorizationEndpoint: string;
  esecScope: string;
  clientId: string;
  redirectUri: string;
  securityType: string;
}

const state = {
    config: null,
  };

const mutations = {
    setConfigMutator(state: any, config: Config) {
      state.config = config;
    },
  };

const actions =  {
    async loadConfig({ commit }: any) {
      try {
        const response = await fetch('/config.json');
        if (!response.ok) {
          throw new Error('Failed to load config.json');
        }
        const config: Config = await response.json();
        commit('setConfigMutator', config);
      } catch (error) {
        console.error('Error loading config:', error);
        throw error;
      }
    },
  };

const getters = {
    getConfig(state: any): Config | null {
      return state.config;
    },
  };

  export default {
    state,
    actions,
    mutations,
    getters
}
