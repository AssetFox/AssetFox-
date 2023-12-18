const state = {
    processCounter: 0
};

const mutations = {
    incrementProcessCounterMutator(state: any){
        state.processCounter++
    },
    decrementProcessCounterMutator(state: any){
        state.processCounter--
    },
    setProcessCounterMutator(state: any, count: number){
        state.processCounter = count;
    }
};

const actions = {
    incrementProcessCounter({commit}: any){
        commit('incrementProcessCounterMutator');
    },
    decrementProcessCounter({commit}: any){
        commit('decrementProcessCounterMutator');
    },
    setProcessCounter({commit}: any, count: number){
        commit('setProcessCounterMutator', count)
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
