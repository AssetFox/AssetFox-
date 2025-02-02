import {
    emptyTreatmentLibrary,
    SimpleTreatment,
    Treatment,
    TreatmentLibrary,
} from '@/shared/models/iAM/treatment';
import {
    any,
    append,
    clone,
    find,
    findIndex,
    propEq,
    reject,
    update,
} from 'ramda';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import TreatmentService from '@/services/treatment.service';
import { stat } from 'fs';
import { stringify } from 'querystring';
import { LibraryUser } from '@/shared/models/iAM/user';
import { name } from 'msal/lib-commonjs/packageMetadata';

const state = {
    treatmentLibraries: [] as TreatmentLibrary[],
    selectedTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary,
    scenarioSelectableTreatments: [] as Treatment[],
    scenarioTreatmentLibrary: clone(emptyTreatmentLibrary) as TreatmentLibrary,
    simpleScenarioSelectableTreatments: [] as SimpleTreatment[],
    simpleSelectableTreatments: [] as SimpleTreatment[],
    hasPermittedAccess: false,
    hasOwnerAccess: false,
    hasViewAccess: false,
    isSharedLibrary: false
};

const mutations = {
    treatmentLibrariesMutator(state: any, libraries: TreatmentLibrary[]) {
        state.treatmentLibraries = clone(libraries);
    },
    selectedTreatmentLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.treatmentLibraries)) {
            state.selectedTreatmentLibrary = find(
                propEq('id', libraryId),
                state.treatmentLibraries,
            );
        } else {
            state.selectedTreatmentLibrary = clone(emptyTreatmentLibrary);
        }
    },
    addedOrUpdatedTreatmentLibraryMutator(
        state: any,
        library: TreatmentLibrary,
    ) {
        state.treatmentLibraries = any(
            propEq('id', library.id),
            state.treatmentLibraries,
        )
            ? update(
                  findIndex(propEq('id', library.id), state.treatmentLibraries),
                  library,
                  state.treatmentLibraries,
              )
            : append(library, state.treatmentLibraries);
    },
    deletedTreatmentLibraryMutator(state: any, deletedLibraryId: string) {
        if (any(propEq('id', deletedLibraryId), state.treatmentLibraries)) {
            state.treatmentLibraries = reject(
                (library: TreatmentLibrary) => deletedLibraryId === library.id,
                state.treatmentLibraries,
            );
        }
    },
    scenarioSelectableTreatmentsMutator(
        state: any,
        selectableTreatments: Treatment[],
    ) {
        state.scenarioSelectableTreatments = clone(selectableTreatments);
    },
    simpleScenarioSelectableTreatmentsMutator(
        state: any,
        selectableTreatments: SimpleTreatment[],
    ) {
        state.simpleScenarioSelectableTreatments = clone(selectableTreatments);
    },
    simpleSelectableTreatmentsMutator(
        state: any,
        selectableTreatments: SimpleTreatment[],
    ) {
        state.simpleSelectableTreatments = clone(selectableTreatments);
    },
    scenarioTreatmentLibraryMutator(
        state: any,
        treatmentLibrary: Treatment
    ) {
        state.scenarioTreatmentLibrary = clone(treatmentLibrary);
    },
    PermittedAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    ViewAccessMutator(state: any, status: boolean) {
        state.hasPermittedAccess = status;
    },
    OwnerAccessMutator(state: any, status: boolean) {
        state.hasOwnerAccess = status;
    },
    IsSharedLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    },
    setIsTreatmentSet(state: any, isTreatmentSet: boolean) {
        state.isTreatmentSet = isTreatmentSet;
    },
};

const actions = {
    selectTreatmentLibrary({ commit }: any, payload: any) {
        commit('selectedTreatmentLibraryMutator', payload.libraryId);
    },
    async getTreatmentLibraries({ commit }: any) {
        await TreatmentService.getTreatmentLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'treatmentLibrariesMutator',
                        response.data as TreatmentLibrary[],
                    );
                }
            },
        );
    },
    async upsertTreatmentLibrary({ dispatch, commit }: any, payload: any) {
        await TreatmentService.upsertTreatmentLibrary(payload.library).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const message: string = any(
                        propEq('id', payload.library.id),
                        state.treatmentLibraries,
                    )
                        ? 'Updated treatment library'
                        : 'Added treatment library';
                    commit(
                        'addedOrUpdatedTreatmentLibraryMutator',
                        payload.library,
                    );
                    commit(
                        'selectedTreatmentLibraryMutator',
                        payload.library.id,
                    );
                    dispatch('addSuccessNotification', { message: message });
                }
            },
        );
    },
    async getScenarioSelectableTreatments({ commit }: any, scenario: { scenarioId: string }) {
        const scenarioId = scenario.scenarioId;
    
        const response = await TreatmentService.getScenarioSelectedTreatments(scenarioId);
        
        if (hasValue(response, 'data')) {
            commit(
                'scenarioSelectableTreatmentsMutator',
                response.data as Treatment[],
            );
        }
    
        return response.data;
    },
    async getSimpleScenarioSelectableTreatments({ commit }: any, scenarioId: string) {
        await TreatmentService.getSimpleTreatmentsByScenarioId(scenarioId).then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'simpleScenarioSelectableTreatmentsMutator',
                        response.data as SimpleTreatment[],
                    );

                    const isTreatmentSet = response.data.length === 0;
                    commit('setIsTreatmentSet', isTreatmentSet);
                }
            },
        );
    },
    async getSimpleSelectableTreatments({ commit }: any, libraryId: string) {
        await TreatmentService.getSimpleTreatmentsByLibraryId(libraryId).then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'simpleSelectableTreatmentsMutator',
                        response.data as SimpleTreatment[],
                    );
                }
            },
        );
    },
    async getTreatmentLibraryBySimulationId({ commit }: any, simulationId: string) {
        await TreatmentService.getTreatmentLibraryBySimulationId(simulationId).then(
            (response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'scenarioTreatmentLibraryMutator',
                        response.data as TreatmentLibrary
                    );
                }
            }
        );
    },
    async upsertScenarioSelectableTreatments(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await TreatmentService.upsertScenarioSelectedTreatments(
            payload.scenarioSelectableTreatments,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioSelectableTreatmentsMutator',
                    payload.scenarioSelectableTreatments,
                );
                dispatch('addSuccessNotification', {
                    message: 'Modified scenario treatments',
                });
            }
        });
    },
    async deleteTreatmentLibrary(
        { dispatch, commit, state }: any,
        payload: any,
    ) {
        await TreatmentService.deleteTreatmentLibrary(payload.libraryId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    commit('deletedTreatmentLibraryMutator', payload.libraryId);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted treatment library',
                    });
                }
            },
        );
    },
    async importScenarioTreatmentsFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await TreatmentService.importTreatments(
            payload.file,
            payload.id,
            true
        ).then((response: AxiosResponse) => {
            dispatch('setAlertMessage', "A treatment import has been added to the work queue");
        });
    },
    async importLibraryTreatmentsFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await TreatmentService.importTreatments(
            payload.file,
            payload.id,
            false
        ).then((response: AxiosResponse) => {
            dispatch('setAlertMessage', "A treatment import has been added to the work queue");
        });
    },
    async importScenarioTreatmentSupersedeRulesFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await TreatmentService.importSupersedeRules(
            payload.file,
            payload.id,
            true
        ).then((response: AxiosResponse) => {
            dispatch('setAlertMessage', "A supersede import has been added to the work queue");
        });
    },
    async importLibraryTreatmentSupersedeRulesFile(
        { commit, dispatch }: any,
        payload: any,
    ) {
        await TreatmentService.importSupersedeRules(
            payload.file,
            payload.id,
            false
        ).then((response: AxiosResponse) => {
            dispatch('setAlertMessage', "A supersede import has been added to the work queue");
        });
    },
    async deleteTreatment(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await TreatmentService.deleteTreatment(payload.treatment, payload.libraryId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    commit('simpleSelectableTreatmentsMutator', payload.treatments);
                    commit('selectedTreatmentLibraryMutator', payload.libraryId);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted treatment',
                    });
                }
            },
        );
    },
    async deleteScenarioSelectableTreatment(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await TreatmentService.deleteScenarioSelectableTreatment(payload.scenarioSelectableTreatment, payload.simulationId).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    commit('simpleScenarioSelectableTreatmentsMutator', payload.treatments);
                    dispatch('addSuccessNotification', {
                        message: 'Deleted scenario treatment',
                    });
                }
            },
        );
    },
    async upsertOrDeleteTreatmentLibraryUsers({dispatch, commit}: any, payload: any) 
    {
        await TreatmentService.upsertOrDeleteTreatmentLibraryUsers(payload.libraryId, payload.proposedUsers)
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
                ) {
                }
        });
    },
    async getHasPermittedAccess({ commit }: any)
    {
        await TreatmentService.getHasPermittedAccess()
        .then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const hasPermittedAccess: boolean = response.data as boolean;
                commit('PermittedAccessMutator', hasPermittedAccess);
            }
        });
    },
    async getIsSharedTreatmentLibrary({ dispatch, commit }: any, payload: any) {
        await TreatmentService.getIsSharedLibrary(payload.id).then(
            (response: AxiosResponse) => {
                if (
                hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                commit('IsSharedLibraryMutator', response.data as boolean);
            }
            });
        }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
