import {clone, any, propEq, update, findIndex, append, find, isNil} from 'ramda';
import AttributeService from '@/services/attribute.service';
import {AxiosResponse} from 'axios';
import {Attribute, AttributeSelectValues, AttributeSelectValuesResult, emptyAttribute, RuleDefinition} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { noneDatasource } from '@/shared/models/iAM/data-source';

const state = {
    attributes: [] as Attribute[],
    selectedAttribute: clone(emptyAttribute) as Attribute,
    stringAttributes: [] as Attribute[],
    numericAttributes: [] as Attribute[],
    attributesSelectValues: [] as AttributeSelectValues[],
    checkedSelectAttributes: [] as string[],
    attributeDataSourceTypes: [] as string[],
    aggregationRules: [] as RuleDefinition[],
    aggregationRulesForType: [] as string[]
};

const mutations = {
    selectedAttributeMutator(state: any, attributeId: string) {
        if (any(propEq('id', attributeId), state.attributes)) {
            state.selectedAttribute = find(
                propEq('id', attributeId),
                state.attributes,
            );
        } else {
            state.selectedAttribute = clone(
                emptyAttribute,
            );
        }
    },
    attributesMutator(state: any, attribute: Attribute) {
        state.attributes = any(
            propEq('id', attribute.id),
            state.attributes,
        )
            ? update(
                  findIndex(
                      propEq('id', attribute.id),
                      state.attributes,
                  ),
                  attribute,
                  state.attributes,
              )
            : append(attribute, state.attributes);
        (state.attributes as Attribute[]).sort((one, two) => (one.name.toUpperCase() < two.name.toUpperCase() ? -1 : 1));
    },
    attributesMutatorClone(state: any,attributes: Attribute[]){
        let cleanAttributes = attributes.map( attr => {
            if(isNil(attr.dataSource)) {
                attr.dataSource = clone(noneDatasource);
            }
            return attr;
        });
        state.attributes = clone(cleanAttributes);
    },
    stringAttributesMutator(state: any, stringAttributes: string[]) {
        state.stringAttributes = clone(stringAttributes);
    },
    numericAttributesMutator(state: any, numericAttributes: string[]) {
        state.numericAttributes = clone(numericAttributes);
    },
    attributesSelectValuesMutator(state: any, attributesSelectValues: AttributeSelectValues[]) {
        state.attributesSelectValues = [...state.attributesSelectValues, ...attributesSelectValues];
    },
    checkedSelectAttributeMutator(state: any, checkedSelectAttributes: string[]) {
        state.checkedSelectAttributes = [...state.checkedSelectAttributes, ...checkedSelectAttributes];
    },
    attributeAggregationRulesMutator(state: any, aggregationRules: RuleDefinition[]) {
        state.aggregationRules = clone(aggregationRules);
    },
    attributeAggregationRulesForTypeMutator(state: any, aggregationRulesForType: RuleDefinition[]) {
        state.aggregationRulesForType = clone(aggregationRulesForType.map(ar=>ar.ruleName));
    },
    attributeDataSourceTypesMutator(state: any, attributeDataSourceTypes: string[]) {
        state.attributeDataSourceTypes = clone(attributeDataSourceTypes);
    }
};

const actions = {
    selectAttribute({ commit }: any, attributeId: string) {
        commit('selectedAttributeMutator', attributeId);
    },
    async getAttributes({commit}: any) {
        await AttributeService.getAttributes()
            .then((response: AxiosResponse<Attribute[]>) => {
                if (hasValue(response, 'data')) {
                    commit('attributesMutatorClone', response.data);
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
                    commit('checkedSelectAttributeMutator', payload.attributeNames);

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
    async upsertAttributes(
        { dispatch, commit }: any,
        attributes: Attribute[],
    ) {
        await AttributeService.upsertAttributes(
            attributes
        ).then((response: AxiosResponse) => {
            if (
                hasValue(response, 'status') &&
                http2XX.test(response.status.toString())
            ) {
                commit(
                    'attributesMutatorClone',
                    attributes,
                );
                dispatch('addSuccessNotification', {
                    message: 'Modified attributes',
                });
            }
        });
    },
    async upsertAttribute(
        { dispatch, commit }: any,
        attribute: Attribute
    ) {
        await AttributeService.upsertAttribute(attribute).then(
            (response: AxiosResponse) => {
                if (
                    hasValue(response, 'status') &&
                    http2XX.test(response.status.toString())
                ) {
                    const message: string = any(
                        propEq('id', attribute.id),
                        state.attributes,
                    )
                        ? 'Updated attribute'
                        : 'Added attribute';

                    commit('attributesMutator', attribute);
                    commit('selectedAttributeMutator', attribute.id);
                    commit('stringAttributesMutator', state.attributes
                        .filter((attribute: Attribute) => attribute.type === 'STRING'));
                    commit('numericAttributesMutator', state.attributes
                        .filter((attribute: Attribute) => attribute.type === 'NUMBER'));
                    dispatch('addSuccessNotification', { message: message });
                }
            },
        );
    },    
    async getAttributeAggregationRules({commit}: any) {
        await AttributeService.GetAttributeAggregationRules()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    commit('attributeAggregationRulesMutator', response.data);
                }
            });
    },
    async getAggregationRulesForType({commit}: any, type: String) {        
        if(type == 'NUMBER')
        {
            commit('attributeAggregationRulesForTypeMutator', state.aggregationRules.filter((ruleDefinition: RuleDefinition) => ruleDefinition.isNumeric));
        }
        if(type == 'STRING')
        {
            commit('attributeAggregationRulesForTypeMutator', state.aggregationRules.filter((ruleDefinition: RuleDefinition) => ruleDefinition.isText));
        }
    },
    async getAttributeDataSourceTypes({commit}: any) {
        await AttributeService.GetAttributeDataSourceTypes()
            .then((response: AxiosResponse<Attribute[]>) => {
                if (hasValue(response, 'data')) {
                    commit('attributeDataSourceTypesMutator', response.data);
                }
            });
    }
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
