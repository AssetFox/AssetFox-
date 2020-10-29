const state = {
    successMessage: '',
    warningMessage: '',
    errorMessage: '',
    infoMessage: '',
};

const mutations = {
    successMessageMutator(state: any, message: string) {
        state.successMessage = message;
    },
    warningMessageMutator(state: any, message: string) {
        state.warningMessage = message;
    },
    errorMessageMutator(state: any, message: string) {
        state.errorMessage = message;
    },
    infoMessageMutator(state: any, message: string) {
        state.infoMessage = message;
    }
};

const actions = {
    setSuccessMessage({commit}: any, payload: any) {
        commit('successMessageMutator', payload.message);
    },
    setWarningMessage({commit}: any, payload: any) {
        commit('warningMessageMutator', payload.message);
    },
    setErrorMessage({commit}: any, payload: any) {
        commit('errorMessageMutator', payload.message);
    },
    setInfoMessage({commit}: any, payload: any) {
        commit('infoMessageMutator', payload.message);
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
