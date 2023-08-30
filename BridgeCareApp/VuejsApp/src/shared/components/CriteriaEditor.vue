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
<v-textarea
    v-if="isAndConjunction"
    style="padding-left:0px;"
    :class="getClassForTextarea(-1)"
    :value="getValueForTextarea(-1)"
    @click="onClickSubCriteriaClauseTextarea(subCriteriaClauses.join(' '), -1)"
    box
    class="ghd-control-text"
    full-width
    no-resize
    readonly
    rows="3"
>
    <template slot="append">
        <v-btn
            @click="onRemoveSubCriteria(selectedSubCriteriaClauseIndex)"
            class="ghd-blue"
            icon
        >
            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
        </v-btn>
    </template>
</v-textarea>
<div v-else>
    <div v-for="(clause, index) in subCriteriaClauses" :key="index">
        <v-textarea style="padding-left:0px;"
            :class="getClassForTextarea(index)"
            :value="getValueForTextarea(index)"
            @click="onClickSubCriteriaClauseTextarea(clause, index)"
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

@Component({
    components: { VueQueryBuilder },
})
export default class CriteriaEditor extends Vue {
    @Prop() criteriaEditorData: CriteriaEditorData;

    @State(state => state.attributeModule.attributes)
    stateAttributes: Attribute[];
    @State(state => state.attributeModule.attributesSelectValues)
    stateAttributesSelectValues: AttributeSelectValues[];
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.userModule.currentUserCriteriaFilter)
    currentUserCriteriaFilter: UserCriteriaFilter;

    @Action('getAttributes') getAttributesAction: any;
    @Action('getAttributeSelectValues') getAttributeSelectValuesAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    queryBuilderRules: any[] = [];
    queryBuilderLabels: object = {
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

    cannotSubmit: boolean = true;
    validCriteriaMessage: string | null = null;
    invalidCriteriaMessage: string | null = null;
    validSubCriteriaMessage: string | null = null;
    invalidSubCriteriaMessage: string | null = null;
    conjunctionSelectListItems: SelectItem[] = [
        { text: 'OR', value: 'OR' },
        { text: 'AND', value: 'AND' },
    ];
    selectedConjunction: string = 'OR';
    subCriteriaClauses: string[] = [];
    selectedSubCriteriaClauseIndex: number = -1;
    selectedSubCriteriaClause: Criteria | null = null;
    selectedRawSubCriteriaClause: string = '';
    activeTab = 'tree-view';
    checkOutput: boolean = false;

    mounted() {
        if (hasValue(this.stateAttributes)) {
            this.setQueryBuilderRules();
            
        }     
    }

    @Watch('criteriaEditorData')
    onCriteriaEditorDataChanged() {
        //TODO
        /*const mainCriteria: Criteria = parseCriteriaString(
      this.criteriaEditorData.mergedCriteriaExpression != null ? this.criteriaEditorData.mergedCriteriaExpression : ''
      ) as Criteria;*/
        this.selectedSubCriteriaClauseIndex = -1;
        const mainCriteria: Criteria = convertCriteriaExpressionToCriteriaObject(
            this.criteriaEditorData.mergedCriteriaExpression != null
                ? this.criteriaEditorData.mergedCriteriaExpression
                : '',
            this.addErrorNotificationAction,
        ) as Criteria;
        const parsedSubCriteria:
            | string[]
            | null = convertCriteriaObjectToCriteriaExpression(
            this.getMainCriteria(),
        );
        const mergedCriteriaExpression: string | null = hasValue(
            parsedSubCriteria,
        )
            ? parsedSubCriteria!.join('')
            : null;

        if (
            !equals(
                this.criteriaEditorData.mergedCriteriaExpression,
                mergedCriteriaExpression,
            ) &&
            mainCriteria
        ) {
            if (!hasValue(mainCriteria.logicalOperator)) {
                mainCriteria.logicalOperator = 'OR';
            }

            this.selectedConjunction = mainCriteria.logicalOperator;

            this.andArray = this.criteriaEditorData.mergedCriteriaExpression ? this.criteriaEditorData.mergedCriteriaExpression.split(' AND ') : [];
            this.orArray  = this.criteriaEditorData.mergedCriteriaExpression ? this.criteriaEditorData.mergedCriteriaExpression.split(' OR ') : [];

            this.setSubCriteriaClauses(mainCriteria);
        }
    }

    @Watch('stateAttributes')
    onStateAttributesChanged() {
        if (hasValue(this.stateAttributes)) {
            this.setQueryBuilderRules();
        }
    }

    @Watch('subCriteriaClauses')
    onSubCriteriaClausesChanged() {
        this.resetCriteriaValidationProperties();
    }

    @Watch('selectedSubCriteriaClause')
    onSelectedClauseChanged() {
        this.resetSubCriteriaValidationProperties();
        if (
            hasValue(this.selectedSubCriteriaClause) &&
            hasValue(this.selectedSubCriteriaClause!.children)
        ) {
            let missingAttributes: string[] = [];

            for (
                let index = 0;
                index < this.selectedSubCriteriaClause!.children!.length;
                index++
            ) {
                missingAttributes = this.getMissingAttribute(
                    this.selectedSubCriteriaClause!.children![index].query,
                    missingAttributes,
                );
            }

            if (hasValue(missingAttributes)) {
                this.getAttributeSelectValuesAction({
                    attributeNames: missingAttributes,
                });
            }
        }
    }

    @Watch('selectedRawSubCriteriaClause')
    onSelectedClauseRawChanged() {
        this.resetSubCriteriaValidationProperties();
    }

    @Watch('stateAttributesSelectValues')
    onStateAttributesSelectValuesChanged() {
        if (
            hasValue(this.queryBuilderRules) &&
            hasValue(this.stateAttributesSelectValues)
        ) {
            const filteredAttributesSelectValues: AttributeSelectValues[] = this.stateAttributesSelectValues.filter(
                (asv: AttributeSelectValues) => hasValue(asv.values),
            );
            if (hasValue(filteredAttributesSelectValues)) {
                filteredAttributesSelectValues.forEach(
                    (asv: AttributeSelectValues) => {
                        this.queryBuilderRules = update(
                            findIndex(
                                propEq('id', asv.attribute),
                                this.queryBuilderRules,
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
                            this.queryBuilderRules,
                        );
                    },
                );
            }
        }
    }

    get isAndConjunction() {
        return this.selectedConjunction === 'AND';
    }

    getClassForTextarea(index: number) {
        if (this.isAndConjunction) {
            return {
                'textarea-focused': this.selectedSubCriteriaClauseIndex === -1,
                'clause-textarea': this.selectedSubCriteriaClauseIndex !== -1
            };
        } else {
            return {
                'textarea-focused': index === this.selectedSubCriteriaClauseIndex,
                'clause-textarea': index !== this.selectedSubCriteriaClauseIndex
            };
        }
    }

    getValueForTextarea(index: number) {
        return this.isAndConjunction ? this.subCriteriaClauses.join(' ') : this.subCriteriaClauses[index];
    }

    setQueryBuilderRules() {
        this.queryBuilderRules = this.stateAttributes.map(
            (attribute: Attribute) => ({
                type: 'text',
                label: attribute.name,
                id: attribute.name,
                operators: ['=', '<>', '<', '<=', '>', '>='],
            }),
        );
    }

    setSubCriteriaClauses(mainCriteria: Criteria) {
        this.subCriteriaClauses = [];
        if (hasValue(mainCriteria) && hasValue(mainCriteria.children)) {
            mainCriteria.children!.forEach((criteriaType: CriteriaType) => {
                const clause: string = convertCriteriaTypeObjectToCriteriaExpression(
                    criteriaType,
                );
                if (hasValue(clause)) {
                    this.subCriteriaClauses.push(clause);
                }
            });
        }
    }

    resetCriteriaValidationProperties() {
        this.validCriteriaMessage = null;
        this.invalidCriteriaMessage = null;
        this.cannotSubmit = !isEmpty(
            convertCriteriaObjectToCriteriaExpression(this.getMainCriteria()),
        );
    }

    resetSubCriteriaValidationProperties() {
        this.validSubCriteriaMessage = null;
        this.invalidSubCriteriaMessage = null;
        this.checkOutput = false;
    }

    resetSubCriteriaSelectedProperties() {
        this.selectedSubCriteriaClauseIndex = -1;
        this.selectedSubCriteriaClause = null;
        this.selectedRawSubCriteriaClause = '';
    }

    onAddSubCriteria() {
        this.resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            this.onClickSubCriteriaClauseTextarea(
                '',
                this.subCriteriaClauses.length,
            );
            this.subCriteriaClauses.push('');
            this.selectedSubCriteriaClauseIndex =
                this.subCriteriaClauses.length - 1;
            this.selectedSubCriteriaClause = clone(emptyCriteria);
            this.resetCriteriaValidationProperties();
        });
    }

    onClickSubCriteriaClauseTextarea(
        subCriteriaClause: string,
        subCriteriaClauseIndex: number,
    ) {
        this.resetSubCriteriaSelectedProperties();
        setTimeout(() => {
            this.selectedSubCriteriaClauseIndex = subCriteriaClauseIndex;
            // TODO
            //this.selectedSubCriteriaClause = parseCriteriaString(subCriteriaClause);
            this.selectedSubCriteriaClause = convertCriteriaExpressionToCriteriaObject(
                subCriteriaClause,
                this.addErrorNotificationAction,
            );
            if (this.selectedSubCriteriaClause) {
                if (!hasValue(this.selectedSubCriteriaClause.logicalOperator)) {
                    this.selectedSubCriteriaClause.logicalOperator = 'AND';
                }
            } else {
                this.invalidSubCriteriaMessage =
                    'Unable to parse selected criteria';
            }
        });
    }

    onRemoveSubCriteria(subCriteriaClauseIndex: number) {
        const subCriteriaClause: string = this.subCriteriaClauses[
            subCriteriaClauseIndex
        ];

        this.subCriteriaClauses = remove(
            subCriteriaClauseIndex,
            1,
            this.subCriteriaClauses,
        );

        if (this.selectedSubCriteriaClauseIndex === subCriteriaClauseIndex) {
            this.resetSubCriteriaSelectedProperties();
        } else {
            this.selectedSubCriteriaClauseIndex = findIndex(
                (subCriteriaClause: string) => {
                    const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                        this.selectedSubCriteriaClause as Criteria,
                    );
                    if (parsedCriteriaJson) {
                        return (
                            subCriteriaClause === parsedCriteriaJson.join('')
                        );
                    }
                    return (
                        subCriteriaClause === this.selectedRawSubCriteriaClause
                    );
                },
                this.subCriteriaClauses,
            );
        }

        this.resetCriteriaValidationProperties();

        if (this.criteriaEditorData.isLibraryContext) {
            if (!hasValue(this.subCriteriaClauses)) {
                this.$emit('submitCriteriaEditorResult', {
                    validated: true,
                    criteria: '',
                });
            } else if (hasValue(subCriteriaClause)) {
                this.$emit('submitCriteriaEditorResult', {
                    validated: false,
                    criteria: null,
                });
            }
        }
    }

    onParseRawSubCriteria() {
        this.activeTab = 'tree-view';
        this.resetSubCriteriaValidationProperties();
        //TODO
        //const parsedRawSubCriteria = parseCriteriaString(this.selectedRawSubCriteriaClause);
        const parsedRawSubCriteria = convertCriteriaExpressionToCriteriaObject(
            this.selectedRawSubCriteriaClause,
            this.addErrorNotificationAction,
        );
        if (parsedRawSubCriteria) {
            this.selectedSubCriteriaClause = parsedRawSubCriteria;
            if (!hasValue(this.selectedSubCriteriaClause.logicalOperator)) {
                this.selectedSubCriteriaClause.logicalOperator = 'OR';
            }
        } else {
            this.invalidSubCriteriaMessage =
                'The raw criteria string is invalid';
        }
    }

    onParseSubCriteriaJson() {
        this.activeTab = 'raw-criteria';
        this.resetSubCriteriaValidationProperties();
        const parsedSubCriteria = convertCriteriaObjectToCriteriaExpression(
            this.selectedSubCriteriaClause as Criteria,
        );
        if (parsedSubCriteria) {
            this.selectedRawSubCriteriaClause = parsedSubCriteria.join('');
        } else {
            this.invalidSubCriteriaMessage = 'The criteria json is invalid';
        }
    }

    onCheckCriteria() {
        this.checkOutput = false;
        this.resetSubCriteriaSelectedProperties();

        const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
            this.getMainCriteria(),
        );
        if (parsedCriteria) {
            if(!isNil(this.$router.currentRoute.query['networkId']))
                this.criteriaValidationWithCount(parsedCriteria)
            else
                this.criteriaValidationNoCount(parsedCriteria)
        } else {
            this.invalidCriteriaMessage = 'Unable to parse criteria';
        }
    }

    private criteriaValidationWithCount(parsedCriteria: string[])
    {
        let networkId = ''
        networkId = this.$router.currentRoute.query['networkId'].toString()
        const validationParameter = {
                expression: parsedCriteria.join(''),
                currentUserCriteriaFilter: this.currentUserCriteriaFilter,
                networkId:networkId
            } as ValidationParameter;
        ValidationService.getCriterionValidationResult(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `${result.resultsCount} result(s) returned`;
                    if (result.isValid) {
                        this.validCriteriaMessage = message;
                        this.cannotSubmit = false;

                        if (this.criteriaEditorData.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                this.getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                this.$emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                this.invalidCriteriaMessage =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        this.resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                this.invalidCriteriaMessage = message;
                                this.cannotSubmit = false;
                            } else {
                                this.invalidCriteriaMessage =
                                    result.validationMessage;
                            }
                        });

                        if (this.criteriaEditorData.isLibraryContext) {
                            this.$emit('submitCriteriaEditorResult', {
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
                currentUserCriteriaFilter: this.currentUserCriteriaFilter,
                networkId:getBlankGuid()
            } as ValidationParameter;
        ValidationService.getCriterionValidationResultNoCount(
                validationParameter,
            ).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const result: CriterionValidationResult = response.data as CriterionValidationResult;
                    const message = `Criterion is Valid`;
                    if (result.isValid) {
                        this.validCriteriaMessage = message;
                        this.cannotSubmit = false;

                        if (this.criteriaEditorData.isLibraryContext) {
                            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                                this.getMainCriteria(),
                            );
                            if (parsedCriteria) {
                                this.$emit('submitCriteriaEditorResult', {
                                    validated: true,
                                    criteria: parsedCriteria.join(''),
                                });
                            } else {
                                this.invalidCriteriaMessage =
                                    'Unable to parse the criteria';
                            }
                        }
                    } else {
                        this.resetCriteriaValidationProperties();
                        setTimeout(() => {
                            if (result.resultsCount === 0) {
                                this.invalidCriteriaMessage = message;
                                this.cannotSubmit = false;
                            } else {
                                this.invalidCriteriaMessage =
                                    result.validationMessage;
                            }
                        });

                        if (this.criteriaEditorData.isLibraryContext) {
                            this.$emit('submitCriteriaEditorResult', {
                                validated: false,
                                criteria: null,
                            });
                        }
                    }
                }
            });
    }

    onCheckSubCriteria() {
        const criteria = this.getSubCriteriaValueToCheck();

        if (isNil(criteria)) {
            this.invalidSubCriteriaMessage = 'Unable to parse criteria';
            return;
        }
        if (isEmpty(criteria)) {
            this.invalidSubCriteriaMessage = 'No criteria to evaluate';
            return;
        }
        
        this.subCriteriaClauses = update(
            this.selectedSubCriteriaClauseIndex,
            criteria,
            this.subCriteriaClauses,
        );
        this.resetCriteriaValidationProperties();
        this.checkOutput = true;
        this.resetSubCriteriaValidationProperties();

        if (this.criteriaEditorData.isLibraryContext) {
            this.$emit('submitCriteriaEditorResult', {
                validated: false,
                criteria: null,
            });
        }
    }

    getSubCriteriaValueToCheck() {
        if (this.activeTab === 'tree-view') {
            const parsedCriteriaJson = convertCriteriaObjectToCriteriaExpression(
                this.selectedSubCriteriaClause as Criteria,
            );
            if (parsedCriteriaJson) {
                return parsedCriteriaJson.join('');
            }
            return null;
        }
        return this.selectedRawSubCriteriaClause;
    }

    onSubmitCriteriaEditorResult(submit: boolean) {
        this.resetSubCriteriaSelectedProperties();
        this.resetCriteriaValidationProperties();

        if (submit) {
            const parsedCriteria = convertCriteriaObjectToCriteriaExpression(
                this.getMainCriteria(),
            );
            if (parsedCriteria) {
                this.selectedConjunction = 'OR';
                this.$emit(
                    'submitCriteriaEditorResult',
                    parsedCriteria.join(''),
                );
            } else {
                this.invalidCriteriaMessage = 'Unable to parse the criteria';
            }
        } else {
            this.selectedConjunction = 'OR';
            this.$emit('submitCriteriaEditorResult', null);
        }
    }

    onDisableCheckOutputButton() {
        const mainCriteria: Criteria = this.getMainCriteria();
        const subCriteriaClausesAreEmpty = this.subCriteriaClauses.every(
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
            this.selectedSubCriteriaClause as Criteria,
        );
        //TODO
        //const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(parseCriteriaString(this.selectedRawSubCriteriaClause) as Criteria);
        const parsedSelectedRawSubCriteriaClause = convertCriteriaObjectToCriteriaExpression(
            convertCriteriaExpressionToCriteriaObject(
                this.selectedRawSubCriteriaClause,
                this.addErrorNotificationAction,
            ) as Criteria,
        );

        const disable: boolean =
            this.selectedSubCriteriaClauseIndex === -1 ||
            (this.activeTab === 'tree-view' &&
                (parsedSelectedSubCriteriaClause === null ||
                    equals(this.selectedSubCriteriaClause, emptyCriteria))) ||
            (parsedSelectedSubCriteriaClause &&
                isEmpty(parsedSelectedSubCriteriaClause.join(''))) ||
            (this.activeTab === 'raw-criteria' &&
                (isEmpty(this.selectedRawSubCriteriaClause) ||
                    parsedSelectedRawSubCriteriaClause === null ||
                    (parsedSelectedRawSubCriteriaClause &&
                        isEmpty(parsedSelectedRawSubCriteriaClause.join('')))));

        return disable;
    }

    getMainCriteria() {
        const filteredSubCriteria: string[] = this.subCriteriaClauses.filter(
            (subCriteriaClause: string) => !isEmpty(subCriteriaClause),
        );

        if (hasValue(filteredSubCriteria)) {
            return {
                logicalOperator: this.selectedConjunction,
                children: this.subCriteriaClauses
                    .filter(
                        (subCriteriaClause: string) =>
                            !isEmpty(subCriteriaClause),
                    )
                    .map((subCriteriaClause: string) => {
                        //TODO
                        //const parsedSubCriteriaClause: Criteria = parseCriteriaString(subCriteriaClause) as Criteria;
                        const parsedSubCriteriaClause: Criteria = convertCriteriaExpressionToCriteriaObject(
                            subCriteriaClause,
                            this.addErrorNotificationAction,
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
                    missingAttributes = this.getMissingAttribute(
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
                    this.stateAttributesSelectValues,
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
