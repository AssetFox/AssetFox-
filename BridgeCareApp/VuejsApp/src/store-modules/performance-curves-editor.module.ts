import {emptyPerformanceLibrary, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {any, append, clone, equals, find, findIndex, propEq, reject, update} from 'ramda';
import PerformanceCurvesEditorService from '@/services/performance-curves-editor.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';
import {convertFromMongoToVue} from '@/shared/utils/mongo-model-conversion-utils';
import {http2XX} from '@/shared/utils/http-utils';

const state = {
    performanceCurveLibraries: [] as PerformanceCurveLibrary[],
    scenarioPerformanceCurveLibrary: clone(emptyPerformanceLibrary) as PerformanceCurveLibrary,
    selectedPerformanceCurveLibrary: clone(emptyPerformanceLibrary) as PerformanceCurveLibrary
};

const mutations = {
    performanceCurveLibrariesMutator(state: any, performanceLibraries: PerformanceCurveLibrary[]) {
        state.performanceCurveLibraries = clone(performanceLibraries);
    },
    selectedPerformanceCurveLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.performanceCurveLibraries)) {
            state.selectedPerformanceCurveLibrary = find(propEq('id', libraryId), state.performanceCurveLibraries);
        } else if (state.scenarioPerformanceCurveLibrary.id === libraryId) {
            state.selectedPerformanceCurveLibrary = clone(state.scenarioPerformanceCurveLibrary);
        } else {
            state.selectedPerformanceCurveLibrary = clone(emptyPerformanceLibrary);
        }
    },
    createdPerformanceCurveLibraryMutator(state: any, createdPerformanceLibrary: PerformanceCurveLibrary) {
        state.performanceCurveLibraries = append(createdPerformanceLibrary, state.performanceCurveLibraries);
    },
    updatedPerformanceCurveLibraryMutator(state: any, updatedPerformanceLibrary: PerformanceCurveLibrary) {
        state.performanceCurveLibraries = update(
            findIndex(propEq('id', updatedPerformanceLibrary.id), state.performanceCurveLibraries),
            updatedPerformanceLibrary,
            state.performanceCurveLibraries
        );
    },
    deletedPerformanceCurveLibraryMutator(state: any, deletedPerformanceLibraryId: string) {
        if (any(propEq('id', deletedPerformanceLibraryId), state.performanceCurveLibraries)) {
            state.performanceCurveLibraries = reject(
                (library: PerformanceCurveLibrary) => deletedPerformanceLibraryId === library.id,
                state.performanceCurveLibraries
            );
        }
    },
    scenarioPerformanceCurveLibraryMutator(state: any, scenarioPerformanceLibrary: PerformanceCurveLibrary) {
        state.scenarioPerformanceCurveLibrary = clone(scenarioPerformanceLibrary);
    }
};

const actions = {
    selectPerformanceCurveLibrary({commit}: any, payload: any) {
        commit('selectedPerformanceCurveLibraryMutator', payload.selectedLibraryId);
    },
    async getPerformanceCurveLibraries({commit}: any) {
        await PerformanceCurvesEditorService.getPerformanceCurveLibraries()
            .then((response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    const performanceLibraries: PerformanceCurveLibrary[] = response.data
                        .map((data: any) => convertFromMongoToVue(data));
                    commit('performanceCurveLibrariesMutator', performanceLibraries);
                }
            });
    },
    async createPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurvesEditorService.createPerformanceCurveLibrary(payload.createdPerformanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const createdPerformanceLibrary: PerformanceCurveLibrary = convertFromMongoToVue(response.data);
                    commit('createdPerformanceCurveLibraryMutator', createdPerformanceLibrary);
                    commit('selectedPerformanceCurveLibraryMutator', createdPerformanceLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully created performance library'});
                }
            });
    },
    async updatePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurvesEditorService.updatePerformanceCurveLibrary(payload.updatedPerformanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    const updatedPerformanceLibrary: PerformanceCurveLibrary = convertFromMongoToVue(response.data);
                    commit('updatedPerformanceCurveLibraryMutator', updatedPerformanceLibrary);
                    commit('selectedPerformanceCurveLibraryMutator', updatedPerformanceLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully updated performance library'});
                }
            });
    },
    async deletePerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurvesEditorService.deletePerformanceCurveLibrary(payload.performanceLibrary)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    commit('deletedPerformanceCurveLibraryMutator', payload.performanceLibrary.id);
                    dispatch('setSuccessMessage', {message: 'Successfully deleted performance library'});
                }
            });
    },
    async getScenarioPerformanceLibrary({commit}: any, payload: any) {
        await PerformanceCurvesEditorService.getScenarioPerformanceCurveLibrary(payload.selectedScenarioId)
            .then((response: AxiosResponse<PerformanceCurveLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPerformanceCurveLibraryMutator', response.data);
                    commit('selectedPerformanceCurveLibraryMutator', response.data.id);
                }
            });
    },
    async saveScenarioPerformanceLibrary({dispatch, commit}: any, payload: any) {
        await PerformanceCurvesEditorService
            .saveScenarioPerformanceCurveLibrary(payload.saveScenarioPerformanceLibraryData, payload.objectIdMOngoDBForScenario)
            .then((response: AxiosResponse<PerformanceCurveLibrary>) => {
                if (hasValue(response, 'data')) {
                    commit('scenarioPerformanceCurveLibraryMutator', response.data);
                    dispatch('setSuccessMessage', {message: 'Successfully saved scenario performance library'});
                }
            });
    },
    async socket_performanceLibrary({dispatch, state, commit}: any, payload: any) {
        if (hasValue(payload, 'operationType')) {
            if (hasValue(payload, 'fullDocument')) {
                const performanceLibrary: PerformanceCurveLibrary = convertFromMongoToVue(payload.fullDocument);
                switch (payload.operationType) {
                    case 'update':
                    case 'replace':
                        commit('updatedPerformanceCurveLibraryMutator', performanceLibrary);
                        if (state.selectedPerformanceCurveLibrary.id === performanceLibrary.id &&
                            !equals(state.selectedPerformanceCurveLibrary, performanceLibrary)) {
                            commit('selectedPerformanceCurveLibraryMutator', performanceLibrary.id);
                            dispatch('setInfoMessage',
                                {message: `Performance library '${performanceLibrary.name}' has been changed from another source`});
                        }
                        break;
                    case 'insert':
                        if (!any(propEq('id', performanceLibrary.id), state.performanceCurveLibraries)) {
                            commit('createdPerformanceCurveLibraryMutator', performanceLibrary);
                            dispatch('setInfoMessage',
                                {message: `Performance library '${performanceLibrary.name}' has been created from another source`});
                        }
                }
            } else if (hasValue(payload, 'documentKey')) {
                if (any(propEq('id', payload.documentKey._id), state.performanceCurveLibraries)) {
                    const deletedPerformanceLibrary: PerformanceCurveLibrary = find(
                        propEq('id', payload.documentKey._id), state.performanceCurveLibraries);
                    commit('deletedPerformanceCurveLibraryMutator', payload.documentKey._id);

                    if (!equals(state.scenarioPerformanceCurveLibrary, emptyPerformanceLibrary)) {
                        commit('selectedPerformanceCurveLibraryMutator', state.scenarioPerformanceCurveLibrary.id);
                    } else {
                        commit('selectedPerformanceCurveLibraryMutator', null);
                    }

                    dispatch('setInfoMessage',
                        {message: `Performance library ${deletedPerformanceLibrary.name} has been deleted from another source`});
                }
            }
        }
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
