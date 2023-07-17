const state = {
    alertMessage: ''
};

const mutations = {
    alertMessageMutator(state: any, message: string) {
        state.alertMessage = message;
    }
};

const actions = {
    setAlertMessage({commit}: any, message: string) {
        commit('alertMessageMutator', message);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};