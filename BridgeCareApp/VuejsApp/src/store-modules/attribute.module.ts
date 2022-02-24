import {clone, any, propEq, update, findIndex, append} from 'ramda';
import AttributeService from '@/services/attribute.service';
import {AxiosResponse} from 'axios';
import {Attribute, AttributeSelectValues, AttributeSelectValuesResult} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    attributes: [] as Attribute[],
    stringAttributes: [] as Attribute[],
    numericAttributes: [] as Attribute[],
    attributesSelectValues: [] as AttributeSelectValues[]
};

const mutations = {
    attributesMutator(state: any, attributes: string[]) {
        state.attributes = clone(attributes);
    },
    stringAttributesMutator(state: any, stringAttributes: string[]) {
        state.stringAttributes = clone(stringAttributes);
    },
    numericAttributesMutator(state: any, numericAttributes: string[]) {
        state.numericAttributes = clone(numericAttributes);
    },
    attributesSelectValuesMutator(state: any, attributesSelectValues: AttributeSelectValues[]) {
        state.attributesSelectValues = [...state.attributesSelectValues, ...attributesSelectValues];
    }
};

const actions = {
    async getAttributes({commit}: any) {
        await AttributeService.getAttributes()
            .then((response: AxiosResponse<Attribute[]>) => {
                if (hasValue(response, 'data')) {
                    commit('attributesMutator', response.data);
                    commit('stringAttributesMutator', response.data
                        .filter((attribute: Attribute) => attribute.type === 'STRING'));
                    commit('numericAttributesMutator', response.data
                        .filter((attribute: Attribute) => attribute.type === 'NUMBER'));
                }
            });
    },
    async getAttributeSelectValues({commit, dispatch}: any, payload: any) {
        await AttributeService.getAttributeSelectValues(payload.attributeNames)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const results: AttributeSelectValuesResult[] = response.data as AttributeSelectValuesResult[];
                    const attributesSelectValues: AttributeSelectValues[] = [];
                    const warningMessages: string[] = [];

                    results.forEach((result: AttributeSelectValuesResult) => {
                        attributesSelectValues.push({attribute: result.attribute, values: result.values});

                        if (result.resultType.toLowerCase() === 'warning') {
                            warningMessages.push(result.resultMessage);
                        }
                    });

                    commit('attributesSelectValuesMutator', attributesSelectValues);

                    if (hasValue(warningMessages)) {
                    dispatch('addWarningNotification', {
                        message: 'Attributes selected warning.',
                        longMessage:
                            warningMessages.length === 1
                                ? warningMessages[0]
                                : warningMessages.join('<br>'),
                    });
                }
            }
        });
    },
};

const getters = {
    getNumericAttributes: (state: any) => {
        return state.numericAttributes;
    },
};

export default {
    state,
    getters,
    actions,
    mutations,
};
