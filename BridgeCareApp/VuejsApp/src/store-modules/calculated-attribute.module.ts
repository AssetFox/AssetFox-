import CalculatedAttributeService from '@/services/calculated-attribute.service';
import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
} from '@/shared/models/iAM/calculated-attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { AxiosResponse } from 'axios';
import { any, append, clone, find, findIndex, propEq, reject, update } from 'ramda';

const state = {
    calculatedAttributeLibraries: [] as CalculatedAttributeLibrary[],
    selectedCalculatedAttributeLibrary: clone(
        emptyCalculatedAttributeLibrary,
    ) as CalculatedAttributeLibrary,
    scenarioCalculatedAttribute: [] as CalculatedAttribute[],
};

const mutations = {
    calculatedAttributeLibrariesMutator(
        state: any,
        libraries: CalculatedAttributeLibrary[],
    ) {
        state.calculatedAttributeLibraries = clone(libraries);
    },
    calculatedAttributeLibraryMutator(
        state: any,
        library: CalculatedAttributeLibrary,
    ) {
        state.calculatedAttributeLibraries = any(
            propEq('id', library.id),
            state.calculatedAttributeLibraries,
        )
            ? update(
                  findIndex(
                      propEq('id', library.id),
                      state.calculatedAttributeLibraries,
                  ),
                  library,
                  state.calculatedAttributeLibraries,
              )
            : append(library, state.calculatedAttributeLibraries);
    },
    selectedCalculatedAttributeLibraryMutator(state: any, libraryId: string) {
        if (any(propEq('id', libraryId), state.calculatedAttributeLibraries)) {
            state.selectedCalculatedAttributeLibrary = find(
                propEq('id', libraryId),
                state.calculatedAttributeLibraries,
            );
        } else {
            state.selectedCalculatedAttributeLibrary = clone(
                emptyCalculatedAttributeLibrary,
            );
        }
    },
    scenarioCalculatedAttributeMutator(
        state: any,
        calculatedAttribute: CalculatedAttribute[],
    ) {
        state.scenarioCalculatedAttribute = clone(calculatedAttribute);
    },
};

const actions = {
    selectCalculatedAttributeLibrary({ commit }: any, libraryId: string) {
        commit('selectedCalculatedAttributeLibraryMutator', libraryId);
    },
    async getCalculatedAttributeLibraries({ commit }: any) {
        var dummy = [{id: getBlankGuid(), name: '',
        description: '',
        calculatedAttributes: [{
            id:  getBlankGuid(),
    attribute: 'AADTTOTAL',
    name: 'AADTTOTAL',
    shift: true,
        }]}];
        return dummy;
        // await CalculatedAttributeService.getCalculatedAttributeLibraries().then(
        //     (response: AxiosResponse<any[]>) => {
        //         if (hasValue(response, 'data')) {
        //             commit(
        //                 'calculatedAttributeLibrariesMutator',
        //                 response.data as CalculatedAttributeLibrary[],
        //             );
        //         }
        //     },
        // );
    },
    async getScenarioCalculatedAttribute({ commit }: any, scenarioId: string) {
        await CalculatedAttributeService.getScenarioCalculatedAttribute(
            scenarioId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'scenarioCalculatedAttributeMutator',
                    response.data as CalculatedAttribute[],
                );
            }
        });
    },
    async upsertCalculatedAttributeLibrary(
        { dispatch, commit }: any,
        library: CalculatedAttributeLibrary,
    ) {
        await CalculatedAttributeService.upsertCalculatedAttributeLibrary(
            library,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const message: string = any(
                    propEq('id', library.id),
                    state.calculatedAttributeLibraries,
                )
                    ? 'Updated calculated attribute library'
                    : 'Added calculated attribute library';

                commit('calculatedAttributeLibraryMutator', library);
                commit('selectedCalculatedAttributeLibraryMutator', library.id);

                dispatch('setSuccessMessage', { message: message });
            }
        });
    },
    async upsertScenarioCalculatedAttribute(
        { dispatch, commit }: any,
        payload: any,
    ) {
        await CalculatedAttributeService.upsertScenarioCalculatedAttribute(
            payload.scenarioCalculatedAttribute,
            payload.scenarioId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'scenarioCalculatedAttributeMutator',
                    payload.scenarioCalculatedAttribute,
                );
                dispatch('setSuccessMessage', {
                    message: "Modified scenario's calculated attribute",
                });
            }
        });
    },
    async deleteCalculatedAttributeLibrary(
        { dispatch, commit }: any,
        libraryId: string,
    ) {
        await CalculatedAttributeService.deleteCalculatedAttributeLibrary(
            libraryId,
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                const calculatedAttributeLibraries: CalculatedAttributeLibrary[] = reject(
                    propEq('id', libraryId),
                    state.calculatedAttributeLibraries,
                );

                commit(
                    'calculatedAttributeLibrariesMutator',
                    calculatedAttributeLibraries,
                );

                dispatch('setSuccessMessage', {
                    message: 'Deleted calculated attribute library',
                });
            }
        });
    },
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations,
};
