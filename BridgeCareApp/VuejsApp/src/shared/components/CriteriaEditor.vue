<template>
    <v-layout justify-center column class="criteria-editor-card-text">
        <div>
            <v-layout justify-space-between>
                <v-flex xs6>
                    <v-layout justify-start
                    >
                        <h3 class="ghd-dialog">Output</h3>
                    </v-layout>
                    <v-card class="elevation-0" style="border: 1px solid;">
                        <div class="conjunction-and-messages-container" style="margin-top:4px;">
                            <v-layout
                                :class="{
                                    'justify-space-between': !criteriaEditorData.isLibraryContext,
                                    'justify-start':
                                        criteriaEditorData.isLibraryContext,
                                }"
                            >
                            <v-flex xs2>
                                <v-layout>
                                <v-select
                                    :items="conjunctionSelectListItems"
                                    append-icon=$vuetify.icons.ghd-down
                                    class="ghd-control-border ghd-control-text ghd-select"
                                    v-model="selectedConjunction"
                                >
                                    <template v-slot:selection="{ item }">
                                        <span class="ghd-control-text">{{ item.text }}</span>
                                    </template>
                                    <template v-slot:item="{ item }">
                                        <v-list-item class="ghd-control-text" v-on="on" v-bind="attrs">
                                        <v-list-item-content>
                                            <v-list-item-title>
                                            <v-row no-gutters align="center">
                                            <span>{{ item.text }}</span>
                                            </v-row>
                                            </v-list-item-title>
                                        </v-list-item-content>
                                        </v-list-item>
                                    </template>                                    
                                </v-select>
                                </v-layout>
                            </v-flex>
                                <v-btn
                                    id="CriteriaEditor-addSubCriteria-btn"
                                    @click="onAddSubCriteria"
                                    class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"    
                                    depressed                                
                                    >Add Subcriteria
                                </v-btn>
                            </v-layout>
                        </div>
                        <v-card-text
                            :class="{
                                'clauses-card-dialog':
                                    !criteriaEditorData.isLibraryContext,
                                'clauses-card-library':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                            <div v-for="(clause, index) in subCriteriaClauses">
                                <v-textarea style="padding-left:0px;"
                                    :class="{
                                        'textarea-focused':
                                            index ===
                                            selectedSubCriteriaClauseIndex,
                                        'clause-textarea':
                                            index !=
                                            selectedSubCriteriaClauseIndex,
                                    }"
                                    :value="clause"
                                    @click="
                                        onClickSubCriteriaClauseTextarea(
                                            clause,
                                            index,
                                        )
                                    "
                                    box
                                    class="ghd-control-text"
                                    full-width
                                    no-resize
                                    readonly
                                    rows="3"
                                >
                                    <template slot="append">
                                        <v-btn
                                            @click="onRemoveSubCriteria(index)"
                                            class="ghd-blue"
                                            icon
                                        >
                                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                        </v-btn>
                                    </template>
                                </v-textarea>
                            </div>
                        </v-card-text>
                        <v-card-actions
                            :class="{
                                'validation-actions':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                            <v-layout>
                                <div class="validation-check-btn-container">
                                    <v-btn
                                        id="CriteriaEditor-checkOutput-btn"
                                        :disabled="onDisableCheckOutputButton()"
                                        @click="onCheckCriteria"
                                        class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                                        depressed
                                    >
                                        Check Output
                                    </v-btn>
                                </div>
                                <div class="validation-messages-container">
                                    <p
                                        class="invalid-message"
                                        v-if="invalidCriteriaMessage !== null"
                                    >
                                        <strong>{{
                                            invalidCriteriaMessage
                                        }}</strong>
                                    </p>
                                    <p
                                        id="CriteriaEditor-validOutput-p"
                                        class="valid-message"
                                        v-if="validCriteriaMessage !== null"
                                    >
                                        {{ validCriteriaMessage }}
                                    </p>
                                    <p v-if="checkOutput">
                                        Please click here to check entire rule
                                    </p>
                                </div>
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-flex>
                <v-flex xs6>
                    <v-layout justify-start
                    >
                        <h3 class="ghd-dialog">Criteria Editor</h3>
                    </v-layout>
                    <v-card class="elevation-0" style="border: 1px solid;height:682px;">                  
                        <v-card-text
                            :class="{
                                'criteria-editor-card-dialog':
                                    !criteriaEditorData.isLibraryContext,
                                'criteria-editor-card-library':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                            <v-layout
                                :class="{
                                    'justify-space-between': !criteriaEditorData.isLibraryContext,
                                    'justify-end':
                                        criteriaEditorData.isLibraryContext,
                                }"
                            >                        
                            </v-layout>                     
                            <v-tabs class="ghd-control-text" style="margin-left:4px;margin-right:4px;"
                                v-if="selectedSubCriteriaClauseIndex !== -1"
                            >
                                <v-tab @click="onParseRawSubCriteria" ripple  id="CriteriaEditor-treeView-tab">
                                    Tree View
                                </v-tab>
                                <v-tab @click="onParseSubCriteriaJson" ripple id="CriteriaEditor-rawView-tab">
                                    Raw Criteria
                                </v-tab>
                                <v-tab-item>
                                    <vue-query-builder 
                                        id="CriteriaEditor-criteria-vuequerybuilder"
                                        :labels="queryBuilderLabels"
                                        :maxDepth="25"
                                        :rules="queryBuilderRules"
                                        :styled="true"
                                        v-if="queryBuilderRules.length > 0"
                                        v-model="selectedSubCriteriaClause"
                                    >
                                    </vue-query-builder>
                                </v-tab-item>
                                <v-tab-item>
                                    <v-textarea
                                        id="CriteriaEditor-rawText-vtextarea"
                                        no-resize
                                        outline
                                        rows="23"
                                        v-model="selectedRawSubCriteriaClause"
                                        class="ghd-control-text"
                                    ></v-textarea>
                                </v-tab-item>
                            </v-tabs>
                        </v-card-text>
                        <v-card-actions
                            :class="{
                                'validation-actions':
                                    criteriaEditorData.isLibraryContext,
                            }"
                        >
                            <v-layout column>          
                                <div class="validation-messages-container">
                                    <p
                                        class="invalid-message"
                                        v-if="
                                            invalidSubCriteriaMessage !== null
                                        "
                                    >
                                        <strong>{{
                                            invalidSubCriteriaMessage
                                        }}</strong>
                                    </p>
                                    <p
                                        class="valid-message"
                                        v-if="validSubCriteriaMessage !== null"
                                    >
                                        {{ validSubCriteriaMessage }}
                                    </p>
                                </div>        
                                <div class="validation-check-btn-container" style="height:64px;margin-top:4px;">
                                    <v-btn 
                                        id="CriteriaEditor-updateSubcriteria-btn"
                                        :disabled="
                                            onDisableCheckCriteriaButton()
                                        "
                                        @click="onCheckSubCriteria"
                                        class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                                        depressed
                                    >
                                        Update Subcriteria
                                    </v-btn>
                                </div>                                                         
                            </v-layout>
                        </v-card-actions>
                    </v-card>
                </v-flex>
            </v-layout>
        </div>
        <div class="main-criteria-check-output-container">
            <v-flex
                v-show="!criteriaEditorData.isLibraryContext"
                class="save-cancel-flex"
            >
                <v-layout justify-center wrap>
                    <v-btn
                        id="CriteriaEditor-save-btn"
                        :disabled="cannotSubmit"
                        @click="onSubmitCriteriaEditorResult(true)"
                        class="ara-blue-bg white--text"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        id="CriteriaEditor-cancel-btn"
                        @click="onSubmitCriteriaEditorResult(false)"
                        class="ara-orange-bg white--text"
                        >Cancel</v-btn
                    >
                </v-layout>
            </v-flex>
        </div>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
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
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    criteriaEditorData: CriteriaEditorData
    }>()

let stateAttributes = ref<Attribute[]>(store.state.attributeModule.attributes);
let stateAttributesSelectValues = ref<AttributeSelectValues[]>(store.state.attributeModule.attributesSelectValues);
let stateNetworks = ref<Network[]>(store.state.networkModule.networks);
let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter);

async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
async function getAttributeSelectValuesAction(payload?: any): Promise<any> {await store.dispatch('getAttributeSelectValues');}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}

    let queryBuilderRules: any[] = [];
    let queryBuilderLabels: object = {
        matchType: '',
        matchTypes: [
            { id: 'AND', label: 'AND' },
            { id: 'OR', label: 'OR' },
        ],
        addRule: 'Add Rule',
        removeRule: `<img class='img-general' src="${require("@/assets/icons/trash-ghd-blue.svg")}" style="margin-top:4px;margin-left:4px;"/>`,
        addGroup: 'Add Group',
        removeGroup: `<img class='img-general' src="${require("@/assets/icons/trash-ghd-blue.svg")}"/>`,
        textInputPlaceholder: 'value',
    };

    let cannotSubmit: boolean = true;
    let validCriteriaMessage: string | null = null;
    let invalidCriteriaMessage: string | null = null;
    let validSubCriteriaMessage: string | null = null;
    let invalidSubCriteriaMessage: string | null = null;
    let conjunctionSelectListItems: SelectItem[] = [
        { text: 'OR', value: 'OR' },
        { text: 'AND', value: 'AND' },
    ];
    let selectedConjunction: string = 'OR';
    let subCriteriaClauses: string[] = [];
    let selectedSubCriteriaClauseIndex: number = -1;
    let selectedSubCriteriaClause: Criteria | null = null;
    let selectedRawSubCriteriaClause: string = '';
    let activeTab = 'tree-view';
    let checkOutput: boolean = false;

    onMounted(()=>mounted())
    function mounted() {
        if (hasValue(stateAttributes)) {
            setQueryBuilderRules();
            
        }     
    }

    watch(()=>props.criteriaEditorData,()=>onCriteriaEditorDataChanged())
    function onCriteriaEditorDataChanged() {
        //TODO
        /*const mainCriteria: Criteria = parseCriteriaString(
      this.criteriaEditorData.mergedCriteriaExpression != null ? this.criteriaEditorData.mergedCriteriaExpression : ''
      ) as Criteria;*/
        selectedSubCriteriaClauseIndex = -1;
        const mainCriteria: Criteria = convertCriteriaExpressionToCriteriaObject(
            props.criteriaEditorData.mergedCriteriaExpression != null
                ? props.criteriaEditorData.mergedCriteriaExpression
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
                props.criteriaEditorData.mergedCriteriaExpression,
                mergedCriteriaExpression,
            ) &&
            mainCriteria
        ) {
            if (!hasValue(mainCriteria.logicalOperator)) {
                mainCriteria.logicalOperator = 'OR';
            }

            selectedConjunction = mainCriteria.logicalOperator;

             const andArray = props.criteriaEditorData.mergedCriteriaExpression ? props.criteriaEditorData.mergedCriteriaExpression.split(' AND ') : [];
            const orArray  = props.criteriaEditorData.mergedCriteriaExpression ? props.criteriaEditorData.mergedCriteriaExpression.split(' OR ') : [];

            setSubCriteriaClauses(mainCriteria);
        }
    }

    watch(stateAttributes,()=> onStateAttributesChanged)
    function onStateAttributesChanged() {
        if (hasValue(stateAttributes.value)) {
            setQueryBuilderRules();
        }
    }

    @Watch('subCriteriaClauses')
    onSubCriteriaClausesChanged() {
        resetCriteriaValidationProperties();
    }

    @Watch('selectedSubCriteriaClause')
    onSelectedClauseChanged() {
        resetSubCriteriaValidationProperties();
        if (
            hasValue(selectedSubCriteriaClause) &&
            hasValue(selectedSubCriteriaClause!.children)
        ) {
            let missingAttributes: string[] = [];

            for (
                let index = 0;
                index < selectedSubCriteriaClause!.children!.length;
                index++
            ) {
                missingAttributes = getMissingAttribute(
                    selectedSubCriteriaClause!.children![index].query,
                    missingAttributes,
                );
            }

            if (hasValue(missingAttributes)) {
                getAttributeSelectValuesAction({
                    attributeNames: missingAttributes,
                });
            }
        }
    }

    @Watch('selectedRawSubCriteriaClause')
    onSelectedClauseRawChanged() {
        resetSubCriteriaValidationProperties();
    }

    @Watch('stateAttributesSelectValues')
    onStateAttributesSelectValuesChanged() {
        if (
            hasValue(queryBuilderRules) &&
            hasValue(stateAttributesSelectValues)
        ) {
            const filteredAttributesSelectValues: AttributeSelectValues[] = stateAttributesSelectValues.filter(
                (asv: AttributeSelectValues) => hasValue(asv.values),
            );
            if (hasValue(filteredAttributesSelectValues)) {
                filteredAttributesSelectValues.forEach(
                    (asv: AttributeSelectValues) => {
                        queryBuilderRules = update(
                            findIndex(
                                propEq('id', asv.attribute),
                                queryBuilderRules,
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
                            queryBuilderRules,
                        );
                    },
                );
            }
        }
    }
@Component({
    components: { VueQueryBuilder },
})
export default class CriteriaEditor extends Vue {
    @Prop() criteriaEditorData: CriteriaEditorData;


    @Action('getAttributes') getAttributesAction: any;
    @Action('getAttributeSelectValues') getAttributeSelectValuesAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    

    

    setQueryBuilderRules() {
        queryBuilderRules = stateAttributes.map(
            (attribute: Attribute) => ({
                type: 'text',
                label: attribute.name,
                id: attribute.name,
                operators: ['=', '<>', '<', '<=', '>', '>='],
            }),
        );
    }

    setSubCriteriaClauses(mainCriteria: Criteria) {
        subCriteriaClauses = [];
        if (hasValue(mainCriteria) && hasValue(mainCriteria.children)) {
            mainCriteria.children!.forEach((criteriaType: CriteriaType) => {
                const clause: string = convertCriteriaTypeObjectToCriteriaExpression(
                    criteriaType,
                );
                if (hasValue(clause)) {
                    subCriteriaClauses.push(clause);
                }
            });
        }
    }

    resetCriteriaValidationProperties() {
        validCriteriaMessage = null;
        invalidCriteriaMessage = null;
        cannotSubmit = !isEmpty(
            convertCriteriaObjectToCriteriaExpression(getMainCriteria()),
        );
    }

    resetSubCriteriaValidationProperties() {
        validSubCriteriaMessage = null;
        invalidSubCriteriaMessage = null;
        checkOutput = false;
    }

    resetSubCriteriaSelectedProperties() {
        selectedSubCriteriaClauseIndex = -1;
        selectedSubCriteriaClause = null;
        selectedRawSubCriteriaClause = '';
    }

    onAddSubCriteria() {
        resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            onClickSubCriteriaClauseTextarea(
                '',
                subCriteriaClauses.length,
            );
            subCriteriaClauses.push('');
            selectedSubCriteriaClauseIndex =
                subCriteriaClauses.length - 1;
            selectedSubCriteriaClause = clone(emptyCriteria);
            resetCriteriaValidationProperties();
        });
    }

    onClickSubCriteriaClauseTextarea(
        subCriteriaClause: string,
        subCriteriaClauseIndex: number,
    ) {
        resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            selectedSubCriteriaClauseIndex = subCriteriaClauseIndex;
            // TODO
            //selectedSubCriteriaClause = parseCriteriaString(subCriteriaClause);
            selectedSubCriteriaClause = convertCriteriaExpressionToCriteriaObject(
                subCriteriaClause,
                addErrorNotificationAction,
            );
            if (selectedSubCriteriaClause) {
                if (!hasValue(selectedSubCriteriaClause.logicalOperator)) {
                    selectedSubCriteriaClause.logicalOperator = 'AND';
                }
            } else {
                invalidSubCriteriaMessage =
                    'Unable to parse selected criteria';
            }
        });
    }

    onRemoveSubCriteria(subCriteriaClauseIndex: number) {
        const subCriteriaClause: string = subCriteriaClauses[
            subCriteriaClauseIndex
        ];

        subCriteriaClauses = remove(
            subCriteriaClauseIndex,
            1,
            subCriteriaClauses,
        );

        if (selectedSubCriteriaClauseIndex === subCriteriaClauseIndex) {
            resetSubCriteriaSelectedProperties();
        } else {
            selectedSubCriteriaClauseIndex = findIndex(
                (subCriteriaClause: string) => {
                    const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                        selectedSubCriteriaClause as Criteria,
                    );
                    if (parsedCriteriaJson) {
                        return (
                            subCriteriaClause === parsedCriteriaJson.join('')
                        );
                    }
                    return (
                        subCriteriaClause === selectedRawSubCriteriaClause
                    );
                },
                subCriteriaClauses,
            );
        }

        resetCriteriaValidationProperties();

        if (criteriaEditorData.isLibraryContext) {
            if (!hasValue(subCriteriaClauses)) {
                $emit('submitCriteriaEditorResult', {
                    validated: true,
                    criteria: '',
                });
            } else if (hasValue(subCriteriaClause)) {
                $emit('submitCriteriaEditorResult', {
                    validated: false,
                    criteria: null,
                });
            }
        }
    }

    onParseRawSubCriteria() {
        activeTab = 'tree-view';
        resetSubCriteriaValidationProperties();
        //TODO
        //const parsedRawSubCriteria = parseCriteriaString(selectedRawSubCriteriaClause);
        const parsedRawSubCriteria = convertCriteriaExpressionToCriteriaObject(
            selectedRawSubCriteriaClause,
            addErrorNotificationAction,
        );
        if (parsedRawSubCriteria) {
            selectedSubCriteriaClause = parsedRawSubCriteria;
            if (!hasValue(selectedSubCriteriaClause.logicalOperator)) {
                selectedSubCriteriaClause.logicalOperator = 'OR';
            }
        } else {
            invalidSubCriteriaMessage =
                'The raw criteria string is invalid';
        }
    }

    onParseSubCriteriaJson() {
        activeTab = 'raw-criteria';
        resetSubCriteriaValidationProperties();
        const parsedSubCriteria = convertCriteriaObjectToCriteriaExpression(
            selectedSubCriteriaClause as Criteria,
        );
        if (parsedSubCriteria) {
            selectedRawSubCriteriaClause = parsedSubCriteria.join('');
        } else {
            invalidSubCriteriaMessage = 'The criteria json is invalid';
        }
    }

    onCheckCriteria() {
        checkOutput = false;
        resetSubCriteriaSelectedProperties();

        const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
            getMainCriteria(),
        );
        if (parsedCriteria) {
            if(!isNil($router.currentRoute.query['networkId']))
                criteriaValidationWithCount(parsedCriteria)
            else
                criteriaValidationNoCount(parsedCriteria)
        } else {
            invalidCriteriaMessage = 'Unable to parse criteria';
        }
    }

    private criteriaValidationWithCount(parsedCriteria: string[])
    {
        let networkId = ''
        networkId = $router.currentRoute.query['networkId'].toString()
        const validationParameter = {
                expression: parsedCriteria.join(''),
                currentUserCriteriaFilter: currentUserCriteriaFilter,
                networkId:networkId
            } as ValidationParameter;
        ValidationService.getCriterionValidationResult(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `${result.resultsCount} result(s) returned`;
                    if (result.isValid) {
                        validCriteriaMessage = message;
                        cannotSubmit = false;

                        if (criteriaEditorData.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                $emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                invalidCriteriaMessage =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                invalidCriteriaMessage = message;
                                cannotSubmit = false;
                            } else {
                                invalidCriteriaMessage =
                                    result.validationMessage;
                            }
                        });

                        if (criteriaEditorData.isLibraryContext) {
                            $emit('submitCriteriaEditorResult', {
                                validated: false,
                                criteria: null,
                            });
                        }
                    }
                }
            });
    }

     private criteriaValidationNoCount(parsedCriteria: string[])
    {
        const validationParameter = {
                expression: parsedCriteria.join(''),
                currentUserCriteriaFilter: currentUserCriteriaFilter,
                networkId:getBlankGuid()
            } as ValidationParameter;
        ValidationService.getCriterionValidationResultNoCount(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `Criterion is Valid`;
                    if (result.isValid) {
                        validCriteriaMessage = message;
                        cannotSubmit = false;

                        if (criteriaEditorData.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                $emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                invalidCriteriaMessage =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                invalidCriteriaMessage = message;
                                cannotSubmit = false;
                            } else {
                                invalidCriteriaMessage =
                                    result.validationMessage;
                            }
                        });

                        if (criteriaEditorData.isLibraryContext) {
                            $emit('submitCriteriaEditorResult', {
                                validated: false,
                                criteria: null,
                            });
                        }
                    }
                }
            });
    }

    onCheckSubCriteria() {
        const criteria = getSubCriteriaValueToCheck();

        if (isNil(criteria)) {
            invalidSubCriteriaMessage = 'Unable to parse criteria';
            return;
        }
        if (isEmpty(criteria)) {
            invalidSubCriteriaMessage = 'No criteria to evaluate';
            return;
        }

        subCriteriaClauses = update(
            selectedSubCriteriaClauseIndex,
            criteria,
            subCriteriaClauses,
        );
        resetCriteriaValidationProperties();
        checkOutput = true;
        resetSubCriteriaValidationProperties();

        if (criteriaEditorData.isLibraryContext) {
            $emit('submitCriteriaEditorResult', {
                validated: false,
                criteria: null,
            });
        }
    }

    getSubCriteriaValueToCheck() {
        if (activeTab === 'tree-view') {
            const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                selectedSubCriteriaClause as Criteria,
            );
            if (parsedCriteriaJson) {
                return parsedCriteriaJson.join('');
            }
            return null;
        }
        return selectedRawSubCriteriaClause;
    }

    onSubmitCriteriaEditorResult(submit: boolean) {
        resetSubCriteriaSelectedProperties();
        resetCriteriaValidationProperties();

        if (submit) {
            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                getMainCriteria(),
            );
            if (parsedCriteria) {
                selectedConjunction = 'OR';
                $emit(
                    'submitCriteriaEditorResult',
                    parsedCriteria.join(''),
                );
            } else {
                invalidCriteriaMessage = 'Unable to parse the criteria';
            }
        } else {
            selectedConjunction = 'OR';
            $emit('submitCriteriaEditorResult', null);
        }
    }

    onDisableCheckOutputButton() {
        const mainCriteria: Criteria = getMainCriteria();
        const subCriteriaClausesAreEmpty = subCriteriaClauses.every(
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

    onDisableCheckCriteriaButton() {
        const parsedSelectedSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(
            selectedSubCriteriaClause as Criteria,
        );
        //TODO
        //const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(parseCriteriaString(selectedRawSubCriteriaClause) as Criteria);
        const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(
            convertCriteriaExpressionToCriteriaObject(
                selectedRawSubCriteriaClause,
                addErrorNotificationAction,
            ) as Criteria,
        );

        const disable: boolean =
            selectedSubCriteriaClauseIndex === -1 ||
            (activeTab === 'tree-view' &&
                (parsedSelectedSubCriteriaClause === null ||
                    equals(selectedSubCriteriaClause, emptyCriteria))) ||
            (parsedSelectedSubCriteriaClause &&
                isEmpty(parsedSelectedSubCriteriaClause.join(''))) ||
            (activeTab === 'raw-criteria' &&
                (isEmpty(selectedRawSubCriteriaClause) ||
                    parsedSelectedRawSubCriteriaClause === null ||
                    (parsedSelectedRawSubCriteriaClause &&
                        isEmpty(parsedSelectedRawSubCriteriaClause.join('')))));

        return disable;
    }

    getMainCriteria() {
        const filteredSubCriteria: string[] = subCriteriaClauses.filter(
            (subCriteriaClause: string) => !isEmpty(subCriteriaClause),
        );

        if (hasValue(filteredSubCriteria)) {
            return {
                logicalOperator: selectedConjunction,
                children: subCriteriaClauses
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

    getMissingAttribute(query: any, missingAttributes: string[]) {
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
                    stateAttributesSelectValues,
                ) &&
                missingAttributes.indexOf(criteriaRule.rule) === -1
            ) {
                missingAttributes.push(criteriaRule.rule);
            }
        }

        return missingAttributes;
    }
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
