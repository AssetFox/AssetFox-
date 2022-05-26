<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between row >              
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Select Budget Priority Library</v-subheader>
                            <v-select :items='librarySelectItems' 
                                outline                           
                                v-model='librarySelectItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                            </v-select>                           
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout row v-show='hasSelectedLibrary || hasScenario' class="shared-owner-flex-padding">
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                        </div>
                        <v-divider class="owner-shared-divider" inset vertical
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                        </v-divider>
                        <v-checkbox class='sharing header-text-content' label='Shared'
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'
                            v-model='selectedBudgetPriorityLibrary.shared' />
                    </v-layout>                               
                </v-flex>                               
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout row align-end class="left-buttons-padding">
                        <v-spacer></v-spacer>
                        <v-btn @click='showCreateBudgetPriorityDialog = true' outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show='hasSelectedLibrary || hasScenario'>Add Budget Priority</v-btn>
                        
                        <v-btn @click='onShowCreateBudgetPriorityLibraryDialog(false)' outline
                            v-show='!hasScenario' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>

        </v-flex>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <div class='priorities-data-table'>
                <v-data-table :headers='budgetPriorityGridHeaders' :items='budgetPriorityGridRows'
                              class='v-table__overflow ghd-table' item-key='id' select-all
                              v-model='selectedBudgetPriorityGridRows' :must-sort='true'>
                    <template slot='items' slot-scope='props'>
                        <td>
                            <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                        </td>
                        <td v-for='header in budgetPriorityGridHeaders'>
                            <div v-if="header.value === 'priorityLevel' || header.value === 'year'">
                                <v-edit-dialog
                                    :return-value.sync='props.item[header.value]'
                                    @save='onEditBudgetPriority(props.item, header.value, props.item[header.value])'
                                    large lazy persistent>
                                    <v-text-field v-if="header.value === 'priorityLevel'" readonly single-line
                                                  class='sm-txt'
                                                  :value='props.item[header.value]'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-else readonly single-line class='sm-txt'
                                                  :value='props.item[header.value]' />
                                    <template slot='input'>
                                        <v-text-field v-if="header.value === 'priorityLevel'" label='Edit' single-line
                                                      v-model.number='props.item[header.value]'
                                                      :mask="'##########'"
                                                      :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                        <v-text-field v-else label='Edit' single-line :mask="'####'"
                                                      v-model.number='props.item[header.value]' />
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else-if="header.value === 'criteria'">
                                <v-layout align-center row style='flex-wrap:nowrap'>
                                    <v-menu bottom min-height='500px' min-width='500px'>
                                        <template slot='activator'>
                                            <div v-if='stateScenarioSimpleBudgetDetails.length > 5'>
                                                <v-btn class='ara-blue ghd-button-text' icon>
                                                    <v-icon>fas fa-eye</v-icon>
                                                </v-btn>
                                            </div>
                                            <div v-else class='priority-criteria-output'>
                                                <v-text-field readonly single-line class='sm-txt'
                                                              :value='props.item.criteria' />
                                            </div>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :value='props.item.criteria' full-width no-resize outline
                                                            readonly rows='5' />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click='onShowCriterionLibraryEditorDialog(props.item)' class='ghd-blue'
                                           icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                            <div v-else-if="header.text.endsWith('%')">
                                <v-edit-dialog
                                    :return-value.sync='props.item[header.value]'
                                    @save='onEditBudgetPercentagePair(props.item, header.value, props.item[header.value])'
                                    large lazy persistent>
                                    <v-text-field readonly single-line class='sm-txt' :value='props.item[header.value]'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [0, 100])]" />
                                    <template slot='input'>
                                        <v-text-field :mask="'###'" label='Edit' single-line
                                                      v-model.number='props.item[header.value]'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [0, 100])]" />
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else>
                                <v-btn @click="onRemoveBudgetPriority(props.item.id)"  class="ghd-blue" icon>
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </div>
                        </td>
                    </template>
                </v-data-table>
                <v-btn :disabled='selectedBudgetPriorityIds.length === 0' @click='onRemoveBudgetPriorities'
                    class='ghd-blue ghd-button' flat>
                    Delete Selected
                </v-btn>
            </div>
        </v-flex>
        <v-flex v-show='hasSelectedLibrary && selectedScenarioId === uuidNIL'
                xs12>
            <v-layout justify-center>
                <v-flex >
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea no-resize outline rows='4' class="ghd-text-field-border"
                                v-model='selectedBudgetPriorityLibrary.description'
                                @input='selectedBudgetPriorityLibrary = {...selectedBudgetPriorityLibrary, description: $event}'>
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>           
            <v-layout justify-center row v-show='hasSelectedLibrary || hasScenario'>
                <v-btn  flat @click='onDiscardChanges'
                       v-show='hasScenario' :disabled='!hasUnsavedChanges' class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn @click='onUpsertScenarioBudgetPriorities'
                       class='ghd-blue-bg white--text ghd-button-text ghd-button'
                       v-show='hasScenario' :disabled='disableCrudButtonsResult || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn @click='onShowConfirmDeleteAlert' flat
                       v-show='!hasScenario' :disabled='!hasSelectedLibrary' class='ghd-blue ghd-button-text ghd-button'>
                    Delete Library
                </v-btn>             
                <v-btn @click='onShowCreateBudgetPriorityLibraryDialog(true)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                       :disabled='disableCrudButtons()'>
                    Create as New Library
                </v-btn>
                <v-btn @click='onUpsertBudgetPriorityLibrary'
                       class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                       v-show='!hasScenario' :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'>
                    Update Library
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onSubmitConfirmDeleteAlertResult' />

        <CreatePriorityLibraryDialog :dialogData='createBudgetPriorityLibraryDialogData'
                                     @submit='onSubmitCreateBudgetPriorityLibraryDialogResult' />

        <CreatePriorityDialog :showDialog='showCreateBudgetPriorityDialog' @submit='onAddBudgetPriority' />

        <CriterionLibraryEditorDialog :dialogData='criterionLibraryEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter } from 'vuex-class';
import {
    BudgetPercentagePair,
    BudgetPriority,
    BudgetPriorityGridDatum,
    BudgetPriorityLibrary,
    emptyBudgetPriority,
    emptyBudgetPriorityLibrary,
} from '@/shared/models/iAM/budget-priority';
import CreatePriorityDialog
    from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityDialog.vue';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { any, clone, contains, find, findIndex, isNil, prepend, propEq, update } from 'ramda';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    CreateBudgetPriorityLibraryDialogData,
    emptyCreateBudgetPriorityLibraryDialogData,
} from '@/shared/models/modals/create-budget-priority-library-dialog-data';
import CreatePriorityLibraryDialog
    from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityLibraryDialog.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore, isEqual, sortNonObjectLists } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { getAppliedLibraryId, hasAppliedLibrary } from '@/shared/utils/library-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';

const ObjectID = require('bson-objectid');

@Component({
    components: {
        CreatePriorityLibraryDialog, CreatePriorityDialog, CriterionLibraryEditorDialog, ConfirmDeleteAlert: Alert,
    },
})
export default class BudgetPriorityEditor extends Vue {
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];
    @State(state => state.budgetPriorityModule.budgetPriorityLibraries) stateBudgetPriorityLibraries: BudgetPriorityLibrary[];
    @State(state => state.budgetPriorityModule.selectedBudgetPriorityLibrary) stateSelectedBudgetPriorityLibrary: BudgetPriorityLibrary;
    @State(state => state.budgetPriorityModule.scenarioBudgetPriorities) stateScenarioBudgetPriorities: BudgetPriority[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getBudgetPriorityLibraries') getBudgetPriorityLibrariesAction: any;
    @Action('selectBudgetPriorityLibrary') selectBudgetPriorityLibraryAction: any;
    @Action('upsertBudgetPriorityLibrary') upsertBudgetPriorityLibraryAction: any;
    @Action('deleteBudgetPriorityLibrary') deleteBudgetPriorityLibraryAction: any;
    @Action('getScenarioBudgetPriorities') getScenarioBudgetPrioritiesAction: any;
    @Action('getScenarioSimpleBudgetDetails') getScenarioSimpleBudgetDetailsAction: any;
    @Action('upsertScenarioBudgetPriorities') upsertScenarioBudgetPrioritiesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;

    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = null;
    selectedBudgetPriorityLibrary: BudgetPriorityLibrary = clone(emptyBudgetPriorityLibrary);
    budgetPriorityGridRows: BudgetPriorityGridDatum[] = [];
    actionHeader: DataTableHeader = { text: 'Action', value: '', align: 'left', sortable: false, class: '', width: ''}
    budgetPriorityGridHeaders: DataTableHeader[] = [
        { text: 'Priority', value: 'priorityLevel', align: 'left', sortable: true, class: '', width: '' },
        { text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: '7%' },
        { text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '' },
        this.actionHeader
    ];
    selectedBudgetPriorityGridRows: BudgetPriorityGridDatum[] = [];
    selectedBudgetPriorityIds: string[] = [];
    selectedBudgetPriorityForCriteriaEdit: BudgetPriority = clone(emptyBudgetPriority);
    showCreateBudgetPriorityDialog: boolean = false;
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
    createBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    budgetPriorities: BudgetPriority[] = [];
    disableCrudButtonsResult: boolean = false;
    checkBoxChanged: boolean = false;
    hasLibraryEditPermission: boolean = false;
    hasCreatedLibrary: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getBudgetPriorityLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.BudgetPriority) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioSimpleBudgetDetailsAction({ scenarioId: vm.selectedScenarioId });
                vm.getScenarioBudgetPrioritiesAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateBudgetPriorityLibraries')
    onStateBudgetPriorityLibrariesChanged() {
        this.librarySelectItems = this.stateBudgetPriorityLibraries.map((library: BudgetPriorityLibrary) => ({
            text: library.name,
            value: library.id,
        }));
    }

    @Watch('librarySelectItemValue')
    onSelectItemValueChanged() {
        this.selectBudgetPriorityLibraryAction({ libraryId: this.librarySelectItemValue });
    }

    @Watch('stateSelectedBudgetPriorityLibrary')
    onStateSelectedPriorityLibraryChanged() {
        this.selectedBudgetPriorityLibrary = clone(this.stateSelectedBudgetPriorityLibrary);
    }

    @Watch('selectedBudgetPriorityLibrary', {deep: true})
    onSelectedPriorityLibraryChanged() {
        this.hasSelectedLibrary = this.selectedBudgetPriorityLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (this.hasScenario) {
            this.budgetPriorities = this.selectedBudgetPriorityLibrary.budgetPriorities.map((priority: BudgetPriority) => ({
                ...priority,
                id: getNewGuid(),
            }));
        } else {
            this.budgetPriorities = clone(this.selectedBudgetPriorityLibrary.budgetPriorities);
        }
    }

    @Watch('stateScenarioBudgetPriorities')
    onStateScenarioBudgetPrioritiesChanged() {
        if (this.hasScenario) {
            this.budgetPriorities = clone(this.stateScenarioBudgetPriorities);
        }
    }

    @Watch('budgetPriorities')
    onBudgetPrioritiesChanged() {
        const allBudgetPercentagePairsMatchBudgets: boolean = this.budgetPriorities
            .every((budgetPriority: BudgetPriority) => this.hasBudgetPercentagePairsThatMatchBudgets(budgetPriority));
        if (!allBudgetPercentagePairsMatchBudgets) {
            this.syncBudgetPercentagePairsWithBudgets();
            return;
        }

        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.budgetPriorities, this.stateScenarioBudgetPriorities)
            : hasUnsavedChangesCore('',
                {...clone(this.selectedBudgetPriorityLibrary), budgetPriorities: clone(this.budgetPriorities)},
                this.stateSelectedBudgetPriorityLibrary);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });

        this.setGridCriteriaColumnWidth();
        this.setGridHeaders();
        this.setGridData();
    }

    @Watch('selectedBudgetPriorityGridRows')
    onSelectedPriorityRowsChanged() {
        this.selectedBudgetPriorityIds = getPropertyValues('id', this.selectedBudgetPriorityGridRows) as string[];
    }

    hasBudgetPercentagePairsThatMatchBudgets(budgetPriority: BudgetPriority) {
        if (!hasValue(this.stateScenarioSimpleBudgetDetails)) {
            return true;
        }

        const simpleBudgetDetails: SimpleBudgetDetail[] = budgetPriority.budgetPercentagePairs
            .map((budgetPercentagePair: BudgetPercentagePair) => ({
                id: budgetPercentagePair.budgetId, name: budgetPercentagePair.budgetName,
            })) as SimpleBudgetDetail[];

        return isEqual(sortNonObjectLists(simpleBudgetDetails), sortNonObjectLists(clone(this.stateScenarioSimpleBudgetDetails)));
    }

    syncBudgetPercentagePairsWithBudgets() {
        const budgetPriorities: BudgetPriority[] = clone(this.budgetPriorities);

        if (hasValue(this.stateScenarioSimpleBudgetDetails)) {
            budgetPriorities.forEach((budgetPriority: BudgetPriority) => {
                if (!this.hasBudgetPercentagePairsThatMatchBudgets(budgetPriority)) {
                    budgetPriority.budgetPercentagePairs = this.createNewBudgetPercentagePairsFromBudgets();
                }
            });
        }

        this.budgetPriorities = budgetPriorities;
    }

    createNewBudgetPercentagePairsFromBudgets() {
        return this.stateScenarioSimpleBudgetDetails.map((simpleBudgetDetail: SimpleBudgetDetail) => ({
            id: getNewGuid(),
            budgetId: simpleBudgetDetail.id,
            budgetName: simpleBudgetDetail.name,
            percentage: 100,
        })) as BudgetPercentagePair[];
    }

    setGridCriteriaColumnWidth() {
        let criteriaColumnWidth = '75%';

        if (this.hasScenario) {
            switch (this.stateScenarioSimpleBudgetDetails.length) {
                case 0:
                    criteriaColumnWidth = '75%';
                    break;
                case 1:
                    criteriaColumnWidth = '65%';
                    break;
                case 2:
                    criteriaColumnWidth = '55%';
                    break;
                case 3:
                    criteriaColumnWidth = '45%';
                    break;
                case 4:
                    criteriaColumnWidth = '35%';
                    break;
                case 5:
                    criteriaColumnWidth = '25%';
                    break;
            }
        }

        this.budgetPriorityGridHeaders[2].width = criteriaColumnWidth;
    }

    setGridHeaders() {
        if (this.hasScenario) {
            const budgetNames: string[] = getPropertyValues('name', this.stateScenarioSimpleBudgetDetails) as string[];
            if (hasValue(budgetNames)) {
                const budgetHeaders: DataTableHeader[] = budgetNames.map((budgetName: string) => ({
                    text: `${budgetName} %`,
                    value: budgetName,
                    align: 'left',
                    sortable: true,
                    class: '',
                    width: '',
                }));
                this.budgetPriorityGridHeaders = [
                    this.budgetPriorityGridHeaders[0],
                    this.budgetPriorityGridHeaders[1],
                    this.budgetPriorityGridHeaders[2],
                    ...budgetHeaders,
                    this.actionHeader
                ];
            }
        }
    }

    setGridData() {
        this.budgetPriorityGridRows = this.budgetPriorities.map((budgetPriority: BudgetPriority) => {
            const row: BudgetPriorityGridDatum = {
                id: budgetPriority.id,
                priorityLevel: budgetPriority.priorityLevel.toString(),
                year: hasValue(budgetPriority.year) ? budgetPriority.year!.toString() : '',
                criteria: budgetPriority.criterionLibrary.mergedCriteriaExpression != null ? budgetPriority.criterionLibrary.mergedCriteriaExpression : '',
            };

            if (this.hasScenario && hasValue(budgetPriority.budgetPercentagePairs)) {
                budgetPriority.budgetPercentagePairs.forEach((budgetPercentagePair: BudgetPercentagePair) => {
                    row[budgetPercentagePair.budgetName] = budgetPercentagePair.percentage.toString();
                });
            }

            return row;
        });
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedBudgetPriorityLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedBudgetPriorityLibrary.owner) == getUserName();
    }

    onShowCreateBudgetPriorityLibraryDialog(createAsNewLibrary: boolean) {
        this.createBudgetPriorityLibraryDialogData = {
            showDialog: true,
            budgetPriorities: createAsNewLibrary ? this.budgetPriorities : [],
        };
    }

    onSubmitCreateBudgetPriorityLibraryDialogResult(budgetPriorityLibrary: BudgetPriorityLibrary) {
        this.createBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
        if (!isNil(budgetPriorityLibrary)) {
            this.upsertBudgetPriorityLibraryAction(budgetPriorityLibrary);
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = budgetPriorityLibrary.name;
        }
    }

    onAddBudgetPriority(newBudgetPriority: BudgetPriority) {
        this.showCreateBudgetPriorityDialog = false;

        if (!isNil(newBudgetPriority)) {
            if (this.hasScenario && hasValue(this.stateScenarioSimpleBudgetDetails)) {
                newBudgetPriority.budgetPercentagePairs = this.createNewBudgetPercentagePairsFromBudgets();
            }

            this.budgetPriorities = prepend(newBudgetPriority, this.budgetPriorities);
        }
    }

    onEditBudgetPriority(budgetPriorityGridDatum: BudgetPriorityGridDatum, property: string, value: any) {
        if (any(propEq('id', budgetPriorityGridDatum.id), this.budgetPriorities)) {
            let budgetPriority: BudgetPriority = find(
                propEq('id', budgetPriorityGridDatum.id), this.budgetPriorities,
            ) as BudgetPriority;

            if (property === 'year' && (!hasValue(value) || parseInt(value) === 0)) {
                budgetPriority.year = null;
            } else {
                budgetPriority = setItemPropertyValue(property, value, budgetPriority) as BudgetPriority;
            }

            this.budgetPriorities = update(
                findIndex(propEq('id', budgetPriority.id), this.budgetPriorities),
                budgetPriority,
                this.budgetPriorities,
            );
        }
    }

    onEditBudgetPercentagePair(budgetPriorityGridDatum: BudgetPriorityGridDatum, budgetName: string, percentage: number) {
        const budgetPriority: BudgetPriority = find(
            propEq('id', budgetPriorityGridDatum.id), this.budgetPriorities,
        ) as BudgetPriority;

        const budgetPercentagePair: BudgetPercentagePair = find(
            propEq('budgetName', budgetName), budgetPriority.budgetPercentagePairs,
        ) as BudgetPercentagePair;

        this.budgetPriorities = update(
            findIndex(propEq('id', budgetPriority.id), this.budgetPriorities),
            {
                ...budgetPriority, budgetPercentagePairs: update(
                    findIndex(propEq('id', budgetPercentagePair.id), budgetPriority.budgetPercentagePairs),
                    setItemPropertyValue('percentage', percentage, budgetPercentagePair) as BudgetPercentagePair,
                    budgetPriority.budgetPercentagePairs,
                ),
            } as BudgetPriority,
            this.budgetPriorities,
        );
    }

    onShowCriterionLibraryEditorDialog(budgetPriorityGridDatum: BudgetPriorityGridDatum) {
        this.selectedBudgetPriorityForCriteriaEdit = find(
            propEq('id', budgetPriorityGridDatum.id), this.budgetPriorities,
        ) as BudgetPriority;

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: this.selectedBudgetPriorityForCriteriaEdit.criterionLibrary.id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedBudgetPriorityForCriteriaEdit.id !== this.uuidNIL) {
            this.budgetPriorities = update(
                findIndex(propEq('id', this.selectedBudgetPriorityForCriteriaEdit.id), this.budgetPriorities),
                setItemPropertyValue('criterionLibrary', criterionLibrary, this.selectedBudgetPriorityForCriteriaEdit) as BudgetPriority,
                this.budgetPriorities);
        }

        this.selectedBudgetPriorityForCriteriaEdit = clone(emptyBudgetPriority);
    }

    onUpsertScenarioBudgetPriorities() {
        this.upsertScenarioBudgetPrioritiesAction({
            scenarioBudgetPriorities: this.budgetPriorities,
            scenarioId: this.selectedScenarioId,
        }).then(() => this.librarySelectItemValue = null);
    }

    onUpsertBudgetPriorityLibrary() {
        const budgetPriorityLibrary: BudgetPriorityLibrary = {
            ...clone(this.selectedBudgetPriorityLibrary),
            budgetPriorities: clone(this.budgetPriorities),
        };
        this.upsertBudgetPriorityLibraryAction(budgetPriorityLibrary);
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.budgetPriorities = clone(this.stateScenarioBudgetPriorities);
            }
        });
    }

    onRemoveBudgetPriorities() {
        this.budgetPriorities = this.budgetPriorities
            .filter((budgetPriority: BudgetPriority) => !contains(budgetPriority.id, this.selectedBudgetPriorityIds));
    }

    onRemoveBudgetPriority(id: string){
        this.budgetPriorities = this.budgetPriorities.filter((bp: BudgetPriority) => bp.id !== id)
    }

    onShowConfirmDeleteAlert() {
        this.confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.deleteBudgetPriorityLibraryAction(this.selectedBudgetPriorityLibrary.id)
                .then(() => this.librarySelectItemValue = null);
        }
    }

    disableCrudButtons() {
        const allDataIsValid: boolean = this.budgetPriorities.every((budgetPriority: BudgetPriority) => {
            const priorityIsValid = this.hasBudgetPercentagePairsThatMatchBudgets(budgetPriority);
            const allSubDataIsValid: boolean = this.hasScenario
                ? budgetPriority.budgetPercentagePairs.every((budgetPercentagePair: BudgetPercentagePair) => {
                    return priorityIsValid &&
                        this.rules['generalRules'].valueIsNotEmpty(budgetPercentagePair.percentage) &&
                        this.rules['generalRules'].valueIsWithinRange(budgetPercentagePair.percentage, [0, 100]);
                })
                : true;

            return this.rules['generalRules'].valueIsNotEmpty(budgetPriority.priorityLevel) === true && allSubDataIsValid;
        })

        if (this.hasSelectedLibrary) {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedBudgetPriorityLibrary.name) === true && allDataIsValid);
        }
        this.disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }
}
</script>

<style>
.priorities-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.priorities-data-table .v-menu--inline, .priority-criteria-output {
    width: 100%;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}

.row-padding{
    padding-top: 0px;
    padding-left: 0px;
    padding-right: 0px;
}

.owner-padding{
    padding-top: 9px;
}

.shared-padding{
    padding-top: 10px !important;
}

.shared-owner-flex-padding{
    padding-top: 22px !important;
}

.left-buttons-padding{
    padding-top: 22px;
}
</style>
