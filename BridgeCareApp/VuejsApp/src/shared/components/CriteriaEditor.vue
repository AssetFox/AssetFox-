<template>
    <v-row>
        <v-col>
            <h3 class="ghd-dialog">Output</h3>
            <v-card class="elevation-0" style="border: 1px solid;" width="600px">
                        <div class="conjunction-and-messages-container" >
                            <v-row
                                :class="{
                                    'justify-space-between': !criteriaEditorData.isLibraryContext,
                                    'justify-start':
                                        criteriaEditorData.isLibraryContext,
                                }"
                            >
                            <v-col cols = "4">
                                <v-row style="padding: 20px;">
                                <v-select
                                    :items="conjunctionSelectListItems"
                                    class="ghd-control-border ghd-control-text ghd-select"
                                    v-model="selectedConjunction"
                                    item-title="text"
                                    item-value="value"
                                    density="compact"
                                    variant="outlined"
                                >
                                </v-select>
                                </v-row>
                            </v-col>
                            <div style="padding:20px">
                                <v-btn
                                    id="CriteriaEditor-addSubCriteria-btn"
                                    @click="onAddSubCriteria"
                                    class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                                    variant = "flat"
                                    >Add Subcriteria
                                </v-btn>
                            </div>
                            </v-row>
                        </div>
                        <v-card-text
                            :class="{
                                'clauses-card-dialog':
                                    !criteriaEditorData.isLibraryContext,
                                'clauses-card-library':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                        <v-textarea
                            v-if="isAndConjunction()"
                            style="padding-left:0px;"
                            variant="outlined"
                            :value="getValueForTextarea(-1)"
                            @click="onClickSubCriteriaClauseTextarea(subCriteriaClauses.join(' '), -1)"
                            class="ghd-control-text"
                            full-width
                            no-resize
                            readonly
                            rows="3"
                        >
                            <template v-slot:append>
                                <v-btn
                                    @click="onRemoveSubCriteria(selectedSubCriteriaClauseIndex)"
                                    class="ghd-blue"
                                    flat
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </template>
                        </v-textarea>
                        <div v-else>
                            <div v-for="(clause, index) in subCriteriaClauses" :key="index">
                                <v-textarea style="padding-left:0px;"
                                    :value="getValueForTextarea(index)"
                                    variant="outlined"
                                    @click="onClickSubCriteriaClauseTextarea(clause, index)"
                                    class="ghd-control-text"
                                    full-width
                                    no-resize
                                    readonly
                                    rows="3"
                                >
                                    <template v-slot:append>
                                        <v-btn id="CriteriaEditor-removeSubCriteria-btn"
                                            @click="onRemoveSubCriteria(index)"
                                            class="ghd-blue"
                                            flat
                                        >
                                            <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                        </v-btn>
                                    </template>
                                </v-textarea>
                            </div>
                        </div>
                    </v-card-text>
                    <v-card-actions :class="{'validation-actions':criteriaEditorData.isLibraryContext,}">
                        <v-row>
                            <div class="validation-check-btn-container">
                                <v-btn
                                    id="CriteriaEditor-checkOutput-btn"
                                    :disabled="onDisableCheckOutputButton()"
                                    @click="onCheckCriteria"
                                    class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                                    variant = "flat"
                                >
                                    Check Output
                                </v-btn>
                            </div>
                            <div style="padding: 10px;">
                                <p class="invalid-message" v-if="invalidCriteriaMessage !== null">
                                    <strong>{{ invalidCriteriaMessage }}</strong>
                                </p>
                                <p id="CriteriaEditor-validOutput-p" class="valid-message" v-if="validCriteriaMessage !== null">
                                    <strong>{{ validCriteriaMessage }}</strong>
                                </p>
                                <p v-if="checkOutput">
                                    Please click here to check entire rule
                                </p>
                            </div>
                        </v-row>
                    </v-card-actions>
            </v-card>
        </v-col>
        <v-col>
            <h3 class="ghd-dialog">Criteria Editor</h3>
            <v-card class="elevation-0" style="border: 1px solid;" width="600px">
                        <v-card-text
                            :class="{
                                'criteria-editor-card-dialog':
                                    !criteriaEditorData.isLibraryContext,
                                'criteria-editor-card-library':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                            <v-row
                                :class="{
                                    'justify-space-between': !criteriaEditorData.isLibraryContext,
                                    'justify-end':
                                        criteriaEditorData.isLibraryContext,
                                }"
                            >
                            </v-row>
                            <v-tabs class="ghd-control-text" style="margin-left:4px;margin-right:4px;"
                                v-if="selectedSubCriteriaClauseIndex !== -1" v-model="tab"
                            >
                                <v-tab @click="onParseRawSubCriteria" id="CriteriaEditor-treeView-tab" value="tree">
                                    Tree View
                                </v-tab>
                                <v-tab @click="onParseSubCriteriaJson" id="CriteriaEditor-rawView-tab" value="raw">
                                    Raw Criteria
                                </v-tab>
                           </v-tabs>
                            <v-window v-model="tab">
                                <v-window-item value="tree">
                                    <!-- <vue-query-builder
                                        id="CriteriaEditor-criteria-vuequerybuilder"
                                        :labels="queryBuilderLabels"
                                        :maxDepth="25"
                                        :rules="queryBuilderRules"
                                        
                                        v-if="queryBuilderRules.length > 0"
                                        v-model="selectedSubCriteriaClause"
                                    >
                                    </vue-query-builder> -->
                                </v-window-item>
                                <v-window-item value="raw">
                                    <v-textarea
                                        id="CriteriaEditor-rawText-vtextarea"
                                        no-resize
                                        variant="outlined"
                                        rows="20"
                                        v-model="selectedRawSubCriteriaClause"
                                        class="ghd-control-text"
                                        
                                    ></v-textarea>
                                </v-window-item>
                            </v-window>

                        </v-card-text>
                        <v-card-actions :class="{ 'validation-actions':criteriaEditorData.isLibraryContext, }">
                            <v-row>
                                <div class="validation-check-btn-container">
                                    <v-btn
                                        id="CriteriaEditor-updateSubcriteria-btn"
                                        @click="onCheckSubCriteria"
                                        class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                                        variant = "flat"
                                    >
                                        Update Subcriteria
                                    </v-btn>
                                </div>
                                <div>
                                    <p class="invalid-message" v-if="invalidSubCriteriaMessage !== null">
                                        <strong>{{
                                            invalidSubCriteriaMessage
                                        }}</strong>
                                    </p>
                                    <p class="valid-message" v-if="validSubCriteriaMessage !== null">
                                        {{ validSubCriteriaMessage }}
                                    </p>
                                </div>       
                            </v-row>
                        </v-card-actions>
            </v-card>
        </v-col>
    </v-row>
    <div class="main-criteria-check-output-container">
        <v-col
            v-show="!criteriaEditorData.isLibraryContext"
            class="save-cancel-flex"
        >
            <v-row justify-center wrap>
                <v-btn
                    id="CriteriaEditor-save-btn"
                    :disabled="cannotSubmit"
                    @click="onSubmitCriteriaEditorResult(true)"
                    class="ara-blue-bg text-white"
                >
                    Save
                </v-btn>
                <v-btn
                    id="CriteriaEditor-cancel-btn"
                    @click="onSubmitCriteriaEditorResult(false)"
                    class="ara-orange-bg text-white"
                    >Cancel</v-btn>
            </v-row>
        </v-col>
    </div>
</template>

<script lang="ts" setup>
import VueQueryBuilder from "vue-query-builder/src/VueQueryBuilder.vue";
import {
    Criteria,
    CriteriaEditorData,
    CriteriaRule,
    CriteriaType,
    emptyCriteria,
} from '../models/iAM/criteria';
import {
    convertCriteriaObjectToCriteriaExpression,
    convertCriteriaTypeObjectToCriteriaExpression,
    convertCriteriaExpressionToCriteriaObject,
} from '../utils/criteria-editor-parsers';
import { hasValue } from '../utils/has-value-util';
import {
    any,
    clone,
    equals,
    findIndex,
    isEmpty,
    isNil,
    propEq,
    remove,
    update,
} from 'ramda';
import {
    Attribute,
    AttributeSelectValues,
} from '@/shared/models/iAM/attribute';
import { AxiosResponse } from 'axios';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Network } from '@/shared/models/iAM/network';
import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import ValidationService from '@/services/validation.service';
import {
    CriterionValidationResult,
    ValidationParameter,
} from '@/shared/models/iAM/expression-validation';
import { UserCriteriaFilter } from '../models/iAM/user-criteria-filter';
import { getBlankGuid } from '../utils/uuid-utils';
import { ref, onMounted, computed, toRefs, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { getUrl } from "../utils/get-url";

let store = useStore();
const $router = useRouter();
const emit = defineEmits(['submit','submitCriteriaEditorResult'])
const props = defineProps<{
    criteriaEditorData: CriteriaEditorData
    }>()
const { criteriaEditorData } = toRefs(props);
const stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
const stateAttributesSelectValues = computed<AttributeSelectValues[]>(() => store.state.attributeModule.attributesSelectValues);
const stateNetworks = computed<Network[]>(() => store.state.networkModule.networks);
const currentUserCriteriaFilter = computed<UserCriteriaFilter>(() => store.state.userModule.currentUserCriteriaFilter);

async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes', payload);}
async function getAttributeSelectValuesAction(payload?: any): Promise<any> {await store.dispatch('getAttributeSelectValues',payload);}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification',payload);}
const tab = ref<any>(null);
    const queryBuilderRules = ref<any[]>([]);
    let queryBuilderLabels: object = {
        matchType: '',
        matchTypes: [
            { id: 'AND', label: 'AND' },
            { id: 'OR', label: 'OR' },
        ],
        addRule: 'Add Rule',
        removeRule: `<img class='img-general' src="${getUrl("assets/icons/trash-ghd-blue.svg")}" style="margin-top:4px;margin-left:4px;"/>`,
        addGroup: 'Add Group',
        removeGroup: `<img class='img-general' src="${getUrl("assets/icons/trash-ghd-blue.svg")}"/>`,
        textInputPlaceholder: 'value',
    };
    const cannotSubmit = ref<boolean>(true);
    const validCriteriaMessage = ref<string | null>(null);
    const invalidCriteriaMessage = ref<string | null>(null);
    const validSubCriteriaMessage = ref<string | null>(null);
    const invalidSubCriteriaMessage = ref<string | null>(null);
    const conjunctionSelectListItems = ref<SelectItem[]>([
        { text: 'OR', value: 'OR' },
        { text: 'AND', value: 'AND' },
    ]);
    const selectedConjunction = ref<string>('OR');
    const subCriteriaClauses= ref<string[]>([]);
    const selectedSubCriteriaClauseIndex = ref<number>(-1);
    const selectedSubCriteriaClause= ref<Criteria |null>(emptyCriteria);
    const selectedRawSubCriteriaClause = ref<string>('');
    let activeTab = 'tree-view';
    const checkOutput = ref<boolean>(false);

    onMounted(()=> {
        const mainCriteria: Criteria = convertCriteriaExpressionToCriteriaObject(
            criteriaEditorData.value.mergedCriteriaExpression != null
                ? criteriaEditorData.value.mergedCriteriaExpression
                : '',
            addErrorNotificationAction,
        ) as Criteria;

        selectedConjunction.value = mainCriteria.logicalOperator;
        setSubCriteriaClauses(mainCriteria);

        if (hasValue(stateAttributes)) {
            setQueryBuilderRules();
        }
    });

    // doesn't seem like excecuted at all 
    watch(criteriaEditorData,() => {
        //TODO
        /*const mainCriteria: Criteria = parseCriteriaString(
      criteriaEditorData.mergedCriteriaExpression != null ? criteriaEditorData.mergedCriteriaExpression : ''
      ) as Criteria;*/
      
        selectedSubCriteriaClauseIndex.value = -1;
        const mainCriteria: Criteria = convertCriteriaExpressionToCriteriaObject(
            criteriaEditorData.value.mergedCriteriaExpression != null
                ? criteriaEditorData.value.mergedCriteriaExpression
                : '',
            addErrorNotificationAction,
        ) as Criteria;
        const parsedSubCriteria:
            | string[]
            | null = convertCriteriaObjectToCriteriaExpression(
            getMainCriteria(),
        );
        const mergedCriteriaExpression: string | null = hasValue(
            parsedSubCriteria,
        )
            ? parsedSubCriteria!.join('')
            : null;

        if (
            !equals(
                criteriaEditorData.value.mergedCriteriaExpression,
                mergedCriteriaExpression,
            ) &&
            mainCriteria
        ) {
            if (!hasValue(mainCriteria.logicalOperator)) {
                mainCriteria.logicalOperator = 'OR';
            }

            selectedConjunction.value = mainCriteria.logicalOperator;

             const andArray = criteriaEditorData.value.mergedCriteriaExpression ? criteriaEditorData.value.mergedCriteriaExpression.split(' AND ') : [];
            const orArray  = criteriaEditorData.value.mergedCriteriaExpression ? criteriaEditorData.value.mergedCriteriaExpression.split(' OR ') : [];

            setSubCriteriaClauses(mainCriteria);
        }
    });

    watch(stateAttributes,()=> {
        if (hasValue(stateAttributes.value)) {
            setQueryBuilderRules();
        }
    });

    watch(subCriteriaClauses,()=> {
        resetCriteriaValidationProperties();
    });

    watch(selectedSubCriteriaClause,()=> {
        resetSubCriteriaValidationProperties();
        if (
            hasValue(selectedSubCriteriaClause.value) &&
            hasValue(selectedSubCriteriaClause.value!.children)
        ) {
            let missingAttributes: string[] = [];

            for (
                let index = 0;
                index < selectedSubCriteriaClause.value!.children!.length;
                index++
            ) {
                missingAttributes = getMissingAttribute(
                    selectedSubCriteriaClause.value!.children![index].query,
                    missingAttributes,
                );
            }

            if (hasValue(missingAttributes)) {
                getAttributeSelectValuesAction({
                    attributeNames: missingAttributes,
                });
            }
        }
    });

    watch(selectedRawSubCriteriaClause,()=> {
        resetSubCriteriaValidationProperties();
    });

    watch(stateAttributesSelectValues,()=> {
        if (
            hasValue(queryBuilderRules.value) &&
            hasValue(stateAttributesSelectValues.value)
        ) {
            const filteredAttributesSelectValues: AttributeSelectValues[] = stateAttributesSelectValues.value.filter(
                (asv: AttributeSelectValues) => hasValue(asv.values),
            );
            if (hasValue(filteredAttributesSelectValues)) {
                filteredAttributesSelectValues.forEach(
                    (asv: AttributeSelectValues) => {
                        queryBuilderRules.value = update(
                            findIndex(
                                propEq('id', asv.attribute),
                                queryBuilderRules.value,
                            ),
                            {
                                type: 'select',
                                id: asv.attribute,
                                label: asv.attribute,
                                operators: ['=', '<>', '<', '<=', '>', '>='],
                                choices: asv.values.map((value: string) => ({
                                    label: value,
                                    value: value,
                                })),
                            },
                            queryBuilderRules.value,
                        );
                    },
                );
            }
        }
    })

    function isAndConjunction() {
        return selectedConjunction.value === 'AND';
    }

    function getClassForTextarea(index: number) {
        if (isAndConjunction()) {
            return {
                'textarea-focused': selectedSubCriteriaClauseIndex.value === -1,
                'clause-textarea': selectedSubCriteriaClauseIndex.value !== -1
            };
        } else {
            return {
                'textarea-focused': index === selectedSubCriteriaClauseIndex.value,
                'clause-textarea': index !== selectedSubCriteriaClauseIndex.value
            };
        }
    }

    function getValueForTextarea(index: number) {
        return isAndConjunction() ? subCriteriaClauses.value.join(' AND ') : subCriteriaClauses.value[index];
    }

    function setQueryBuilderRules() {
        queryBuilderRules.value = stateAttributes.value.map(
            (attribute: Attribute) => ({
                type: 'text',
                label: attribute.name,
                id: attribute.name,
                operators: ['=', '<>', '<', '<=', '>', '>='],
            }),
        );
    }

    function setSubCriteriaClauses(mainCriteria: Criteria) {
        subCriteriaClauses.value = [];
        if (hasValue(mainCriteria) && hasValue(mainCriteria.children)) {
            mainCriteria.children!.forEach((criteriaType: CriteriaType) => {
                const clause: string = convertCriteriaTypeObjectToCriteriaExpression(
                    criteriaType,
                );
                if (hasValue(clause)) {
                    subCriteriaClauses.value.push(clause);
                }
            });
        }
    }

    function resetCriteriaValidationProperties() {
        validCriteriaMessage.value = null;
        invalidCriteriaMessage.value = null;
        cannotSubmit.value = !isEmpty(
            convertCriteriaObjectToCriteriaExpression(getMainCriteria()),
        );
    }

    function resetSubCriteriaValidationProperties() {
        validSubCriteriaMessage.value = null;
        invalidSubCriteriaMessage.value = null;
        checkOutput.value = false;
    }

    function resetSubCriteriaSelectedProperties() {
        selectedSubCriteriaClauseIndex.value = -1;
        selectedSubCriteriaClause.value = null;
        selectedRawSubCriteriaClause.value = '';
    }

    function onAddSubCriteria() {
        resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            onClickSubCriteriaClauseTextarea(
                '',
                subCriteriaClauses.value.length,
            );
            subCriteriaClauses.value.push('');
            selectedSubCriteriaClauseIndex.value =
                subCriteriaClauses.value.length - 1;
            selectedSubCriteriaClause.value = clone(emptyCriteria);
            resetCriteriaValidationProperties();
        });
    }

    function onClickSubCriteriaClauseTextarea(
        subCriteriaClause: string,
        subCriteriaClauseIndex: number,
    ) {
        resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            selectedSubCriteriaClauseIndex.value = subCriteriaClauseIndex;
            // TODO
            //selectedSubCriteriaClause = parseCriteriaString(subCriteriaClause);
            selectedSubCriteriaClause.value = convertCriteriaExpressionToCriteriaObject(
                subCriteriaClause,
                addErrorNotificationAction,
            );
            if (selectedSubCriteriaClause.value) {
                if (!hasValue(selectedSubCriteriaClause.value?.logicalOperator)) {
                    selectedSubCriteriaClause.value!.logicalOperator = 'AND';
                }
            } else {
                invalidSubCriteriaMessage.value =
                    'Unable to parse selected criteria';
            }
        });
    }

    function onRemoveSubCriteria(subCriteriaClauseIndex: number) {
        const subCriteriaClause: string = subCriteriaClauses.value[
            subCriteriaClauseIndex
        ];

        subCriteriaClauses.value = remove(
            subCriteriaClauseIndex,
            1,
            subCriteriaClauses.value,
        );

        if (selectedSubCriteriaClauseIndex.value === subCriteriaClauseIndex) {
            resetSubCriteriaSelectedProperties();
        } else {
            selectedSubCriteriaClauseIndex.value = findIndex(
                (subCriteriaClause: string) => {
                    const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                        selectedSubCriteriaClause.value as Criteria,
                    );
                    if (parsedCriteriaJson) {
                        return (
                            subCriteriaClause === parsedCriteriaJson.join('')
                        );
                    }
                    return (
                        subCriteriaClause === selectedRawSubCriteriaClause.value
                    );
                },
                subCriteriaClauses.value,
            );
        }

        resetCriteriaValidationProperties();

        if (criteriaEditorData.value.isLibraryContext) {
            if (!hasValue(subCriteriaClauses)) {
                emit('submitCriteriaEditorResult', {
                    validated: true,
                    criteria: '',
                });
            } else if (hasValue(subCriteriaClause)) {
                emit('submitCriteriaEditorResult', {
                    validated: false,
                    criteria: null,
                });
            }
        }
    }

    function onParseRawSubCriteria() {
        activeTab = 'tree-view';
        resetSubCriteriaValidationProperties();
        //TODO
        //const parsedRawSubCriteria = parseCriteriaString(selectedRawSubCriteriaClause);
        const parsedRawSubCriteria = convertCriteriaExpressionToCriteriaObject(
            selectedRawSubCriteriaClause.value,
            addErrorNotificationAction,
        );
        if (parsedRawSubCriteria) {
            selectedSubCriteriaClause.value = parsedRawSubCriteria;
            if (!hasValue(selectedSubCriteriaClause.value.logicalOperator)) {
                selectedSubCriteriaClause.value.logicalOperator = 'OR';
            }
        } else {
            invalidSubCriteriaMessage.value =
                'The raw criteria string is invalid';
        }
    }

    function onParseSubCriteriaJson() {
        activeTab = 'raw-criteria';
        resetSubCriteriaValidationProperties();
        const parsedSubCriteria = convertCriteriaObjectToCriteriaExpression(
            selectedSubCriteriaClause.value as Criteria,
        );
        if (parsedSubCriteria) {
            selectedRawSubCriteriaClause.value = parsedSubCriteria.join('');
        } else {
            invalidSubCriteriaMessage.value = 'The criteria json is invalid';
        }
    }

    function onCheckCriteria() {
        checkOutput.value = false;
        resetSubCriteriaSelectedProperties();

        const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
            getMainCriteria(),
        );
        if (parsedCriteria) {
            if(!isNil($router.currentRoute.value.query['networkId']))
                criteriaValidationWithCount(parsedCriteria)
            else
                criteriaValidationNoCount(parsedCriteria)
        } else {
            invalidCriteriaMessage.value = 'Unable to parse criteria';
        }
    }

    function  criteriaValidationWithCount(parsedCriteria: string[])
    {
        let networkId = ''
        networkId = $router.currentRoute.value.query['networkId'] as string
        const validationParameter = {
                expression: parsedCriteria.join(''),
                currentUserCriteriaFilter: currentUserCriteriaFilter.value,
                networkId:networkId
            } as ValidationParameter;
        ValidationService.getCriterionValidationResult(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `${result.resultsCount} result(s) returned`;
                    if (result.isValid) {
                        validCriteriaMessage.value = message;
                        cannotSubmit.value = false;

                        if (criteriaEditorData.value.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                invalidCriteriaMessage.value =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                invalidCriteriaMessage.value = message;
                                cannotSubmit.value = false;
                            } else {
                                invalidCriteriaMessage.value =
                                    result.validationMessage;
                            }
                        });

                        if (criteriaEditorData.value.isLibraryContext) {
                            emit('submitCriteriaEditorResult', {
                                validated: false,
                                criteria: null,
                            });
                        }
                    }
                }
            });
    }

     function criteriaValidationNoCount(parsedCriteria: string[])
    {
        const validationParameter = {
                expression: parsedCriteria.join(''),
                currentUserCriteriaFilter: currentUserCriteriaFilter.value,
                networkId:getBlankGuid()
            } as ValidationParameter;
        ValidationService.getCriterionValidationResultNoCount(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `Criterion is Valid`;
                    if (result.isValid) {
                        validCriteriaMessage.value = message;
                        cannotSubmit.value = false;

                        if (criteriaEditorData.value.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                invalidCriteriaMessage.value =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                invalidCriteriaMessage.value = message;
                                cannotSubmit.value = false;
                            } else {
                                invalidCriteriaMessage.value =
                                    result.validationMessage;
                            }
                        });

                        if (criteriaEditorData.value.isLibraryContext) {
                            emit('submitCriteriaEditorResult', {
                                validated: false,
                                criteria: null,
                            });
                        }
                    }
                }
            });
    }

    function onCheckSubCriteria() {
        const criteria = getSubCriteriaValueToCheck();

        if (isNil(criteria)) {
            invalidSubCriteriaMessage.value = 'Unable to parse criteria';
            return;
        }
        if (isEmpty(criteria)) {
            invalidSubCriteriaMessage.value = 'No criteria to evaluate';
            return;
        }

        subCriteriaClauses.value= update(
            selectedSubCriteriaClauseIndex.value,
            criteria as any,
            subCriteriaClauses.value,
        );
        resetCriteriaValidationProperties();
        checkOutput.value = true;
        resetSubCriteriaValidationProperties();

        if (criteriaEditorData.value.isLibraryContext) {
            emit('submitCriteriaEditorResult', {
                validated: false,
                criteria: null,
            });
        }
    }

    function getSubCriteriaValueToCheck() {
        if (activeTab === 'tree-view') {
            const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                selectedSubCriteriaClause.value as Criteria,
            );
            if (parsedCriteriaJson) {
                return parsedCriteriaJson.join('');
            }
            return null;
        }
        return selectedRawSubCriteriaClause.value;
    }

    function onSubmitCriteriaEditorResult(submit: boolean) {
        resetSubCriteriaSelectedProperties();
        resetCriteriaValidationProperties();

        if (submit) {
            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                getMainCriteria(),
            );
            if (parsedCriteria) {
                selectedConjunction.value = 'OR';
                emit(
                    'submitCriteriaEditorResult',
                    parsedCriteria.join(''),
                );
            } else {
                invalidCriteriaMessage.value = 'Unable to parse the criteria';
            }
        } else {
            selectedConjunction.value = 'OR';
            emit('submitCriteriaEditorResult', null);
        }
    }

    function onDisableCheckOutputButton() {
        const mainCriteria: Criteria = getMainCriteria();
        const subCriteriaClausesAreEmpty = subCriteriaClauses.value.every(
            (subCriteriaClause: string) => isEmpty(subCriteriaClause),
        );

        const disable: boolean =
            !mainCriteria ||
            (isEqual(mainCriteria, emptyCriteria) &&
                subCriteriaClausesAreEmpty) ||
            (!hasValue(mainCriteria.children) && subCriteriaClausesAreEmpty) ||
            isEmpty(convertCriteriaObjectToCriteriaExpression(mainCriteria));

        return disable;
    }

    function onDisableCheckCriteriaButton() {
        const parsedSelectedSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(
            selectedSubCriteriaClause.value as Criteria,
        );
        //TODO
        //const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(parseCriteriaString(selectedRawSubCriteriaClause) as Criteria);
        const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(
            convertCriteriaExpressionToCriteriaObject(
                selectedRawSubCriteriaClause.value,
                addErrorNotificationAction,
            ) as Criteria,
        );

        const disable: boolean =
            selectedSubCriteriaClauseIndex.value === -1 ||
            (activeTab === 'tree-view' &&
                (parsedSelectedSubCriteriaClause === null ||
                    equals(selectedSubCriteriaClause.value, emptyCriteria))) ||
            (parsedSelectedSubCriteriaClause &&
                isEmpty(parsedSelectedSubCriteriaClause.join(''))) ||
            (activeTab === 'raw-criteria' &&
                (isEmpty(selectedRawSubCriteriaClause.value) ||
                    parsedSelectedRawSubCriteriaClause == null ||
                    (parsedSelectedRawSubCriteriaClause &&
                        isEmpty(parsedSelectedRawSubCriteriaClause.join('')))));

        return disable;
    }

    function getMainCriteria() {
        const filteredSubCriteria: string[] = subCriteriaClauses.value.filter(
            (subCriteriaClause: string) => !isEmpty(subCriteriaClause),
        );

        if (hasValue(filteredSubCriteria)) {
            return {
                logicalOperator: selectedConjunction.value,
                children: subCriteriaClauses.value
                    .filter(
                        (subCriteriaClause: string) =>
                            !isEmpty(subCriteriaClause),
                    )
                    .map((subCriteriaClause: string) => {
                        //TODO
                        //const parsedSubCriteriaClause: Criteria = parseCriteriaString(subCriteriaClause) as Criteria;
                        const parsedSubCriteriaClause: Criteria = convertCriteriaExpressionToCriteriaObject(
                            subCriteriaClause,
                            addErrorNotificationAction,
                        ) as Criteria;
                        if (
                            hasValue(parsedSubCriteriaClause) &&
                            parsedSubCriteriaClause.children!.length === 1
                        ) {
                            return parsedSubCriteriaClause.children![0];
                        }
                        return {
                            type: 'query-builder-group',
                            query: {
                                logicalOperator:
                                    parsedSubCriteriaClause.logicalOperator,
                                children: parsedSubCriteriaClause.children,
                            },
                        };
                    }),
            };
        }

        return clone(emptyCriteria);
    }

    function getMissingAttribute(query: any, missingAttributes: string[]) {
        if (query.hasOwnProperty('children')) {
            const criteria: Criteria = query as Criteria;
            if (hasValue(criteria.children)) {
                criteria.children!.forEach((child: CriteriaType) => {
                    missingAttributes = getMissingAttribute(
                        child.query,
                        missingAttributes,
                    );
                });
            }
        } else {
            const criteriaRule: CriteriaRule = query as CriteriaRule;
            if (
                !any(
                    propEq('attribute', criteriaRule.rule),
                    stateAttributesSelectValues.value,
                ) &&
                missingAttributes.indexOf(criteriaRule.rule) === -1
            ) {
                missingAttributes.push(criteriaRule.rule);
            }
        }

        return missingAttributes;
    }

</script>

<style>
.invalid-message {
    color: red;
}

.valid-message {
    color: green;
}

.clauses-card-dialog {
    height: 500px;
    max-height: calc(100vh - 400px);
    overflow-y: auto;
}

.clauses-card-library {
    height: 537px;
    overflow-y: auto;
}

.criteria-editor-card-dialog {
    height: 568px;
    max-height: calc(100vh - 332px);
    overflow-y: auto;
}

.criteria-editor-card-library {
    height: 618px;
    overflow-y: auto;
}


.clause-textarea .v-input__slot {
    background-color: #ffffff !important;
}

.textarea-focused .v-input__slot {
    background-color: #ffffff !important;
}

.clause-textarea textarea {
    border: 1px solid #999999;
    margin-left:-12px;
    padding-left:12px;
    border-radius: 4px;
}


.textarea-focused textarea {
    background-color: #ffffff !important;
    margin-left:-12px;
    padding-left:12px;
    border: 2px solid #2A578D;
    border-radius: 4px;
}

.conjunction-and-messages-container {
    padding-left: 20px;
}

.conjunction-select-list {
    width: 100px;
}


.save-cancel-flex {
    margin-top: 20px;
}

.validation-actions {
    height: 75px;
}

.validation-check-btn-container {
    margin-left: 10px;
}

.validation-messages-container {
    margin-left: 5px;
}

/* Force drop-down arrow on vue-query-view selects */
select.form-control {
    -webkit-appearance:auto !important;
}

button.btn.btn-default {
    border-radius: 5px;
    border: 1px solid #99999980 !important;
    padding-left: 5px;
    padding-right: 5px;
    background-color: #FFFFFF !important;
    color: #2A578D !important;
    font-weight: 600 !important;
}

</style>
