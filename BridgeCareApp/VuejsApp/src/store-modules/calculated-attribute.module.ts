import CalculatedAttributeService from '@/services/calculated-attribute.service';
import { Attribute } from '@/shared/models/iAM/attribute';
import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
    emptyCalculatedAttributeLibrary
} from '@/shared/models/iAM/calculated-attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { AxiosResponse } from 'axios';
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

const state = {
    calculatedAttributeLibraries: [] as CalculatedAttributeLibrary[],
    selectedCalculatedAttributeLibrary: clone(
        emptyCalculatedAttributeLibrary,
    ) as CalculatedAttributeLibrary,
    scenarioCalculatedAttributes: [] as CalculatedAttribute[],
    selectedLibraryCalculatedAttributes: [] as CalculatedAttribute[],
    calculatedAttributes: [] as Attribute[],
    isSharedLibrary: false,
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
        calculatedAttributes: CalculatedAttribute[],
    ) {
        state.scenarioCalculatedAttributes = clone(calculatedAttributes);
    },
    selectedLibraryCalculatedAttributeMutator(
        state: any,
        calculatedAttributes: CalculatedAttribute[],
    ) {
        state.selectedLibraryCalculatedAttributes = clone(calculatedAttributes);
    },
    calculatedAttributesMutator(state: any, calculatedAtt: Attribute[]) {
        state.calculatedAttributes = clone(calculatedAtt);
    },
    IsSharedLibraryMutator(state: any, status: boolean) {
        state.isSharedLibrary = status;
    }
};

const actions = {
    async getCalculatedAttributes({ commit }: any) {
        await CalculatedAttributeService.getCalculatedAttributes().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'calculatedAttributesMutator',
                        response.data as Attribute[],
                    );
                }
            },
        );
    },
    selectCalculatedAttributeLibrary({ commit }: any, libraryId: string) {
        commit('selectedCalculatedAttributeLibraryMutator', libraryId);
    },
    async getCalculatedAttributeLibraries({ commit }: any) {
        await CalculatedAttributeService.getCalculatedAttributeLibraries().then(
            (response: AxiosResponse<any[]>) => {
                if (hasValue(response, 'data')) {
                    commit(
                        'calculatedAttributeLibrariesMutator',
                        response.data as CalculatedAttributeLibrary[],
                    );
                }
            },
        );
    },
    async getScenarioCalculatedAttribute({ commit }: any, scenarioId: string) {
        await CalculatedAttributeService.getEmptyCalculatedAttributesByScenarioId(
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
    async getSelectedLibraryCalculatedAttributes({ commit }: any, libraryId: string) {
        await CalculatedAttributeService.getEmptyCalculatedAttributesByLibraryId(
            libraryId,
        ).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                commit(
                    'selectedLibraryCalculatedAttributeMutator',
                    response.data as CalculatedAttribute[],
                );
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

                dispatch('addSuccessNotification', {
                    message: 'Deleted calculated attribute library',
                });
            }
        });
    },
    async getIsSharedCalculatedAttributeLibrary({ dispatch, commit }: any, payload: any) {
        await CalculatedAttributeService.getIsSharedCalculatedAttributeLibrary(payload.id).then(
            (response: AxiosResponse) => {
                if (
                hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                commit('IsSharedLibraryMutator', response.data as boolean);
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
