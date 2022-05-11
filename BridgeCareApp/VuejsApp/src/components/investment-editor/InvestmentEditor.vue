<template>
    <v-layout column>
        <v-layout row  >
            <!-- text boxes for scenario only -->
                    <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                        <v-subheader class="ghd-md-gray ghd-control-subheader"><span>First Year of Analysis Period</span></v-subheader>
                        <v-text-field readonly outline
                                      @change='onEditInvestmentPlan("firstYearOfAnalysisPeriod", $event)'
                                      v-model='investmentPlan.firstYearOfAnalysisPeriod' 
                                      class="ghd-text-field-border ghd-text-field"/>
                    </v-flex>
                    <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                        <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Number of Years in Analysis Period</span></v-subheader>
                        <v-text-field readonly outline
                                      @change='onEditInvestmentPlan("numberOfYearsInAnalysisPeriod", $event)'
                                      v-model='investmentPlan.numberOfYearsInAnalysisPeriod'
                                      class="ghd-text-field-border ghd-text-field" />
                    </v-flex>
                    <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                        <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Minimum Project Cost Limit</span></v-subheader>
                        <v-text-field outline id='min-proj-cost-limit-txt'                                      
                                      @change='onEditInvestmentPlan("minimumProjectCostLimit", $event)'
                                      v-model='investmentPlan.minimumProjectCostLimit'
                                      v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: true}"                                      
                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.minimumProjectCostLimit)]"
                                      :disabled="!isAdmin" 
                                      class="ghd-text-field-border ghd-text-field"/>
                    </v-flex>
                    <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                        <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Inflation Rate Percentage</span></v-subheader>
                        <v-text-field outline
                                      v-model='investmentPlan.inflationRatePercentage'
                                      @change='onEditInvestmentPlan("inflationRatePercentage", $event)'
                                      :mask="'###'"
                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0,100])]"
                                      :disabled="!isAdmin" 
                                      class="ghd-text-field-border ghd-text-field"/>
                    </v-flex>
                    <!-- this shows up everywhere -->
                    <v-flex xs4 class="ghd-constant-header">
                        <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Select an Investment library</span></v-subheader>
                        <v-select :items='librarySelectItems'
                              outline 
                              v-model='librarySelectItemValue'
                              class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>
                    </v-flex>
                    <!-- these are only in library -->
                    <v-flex xs4 v-if='!hasScenario' class="ghd-constant-header">
                        <v-layout v-if='hasSelectedLibrary && !hasScenario' row class="header-alignment-padding-center">
                            <div class="header-text-content invest-owner-padding">                            
                                Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                            </div>
                            <v-divider class="owner-shared-divider" inset vertical>
                            </v-divider>
                            <v-checkbox class='sharing header-text-content' label='Shared' v-model='selectedBudgetLibrary.isShared' />
                        </v-layout>
                    </v-flex>
                    <v-flex xs4 v-if='!hasScenario' class="ghd-constant-header"> 
                        <v-layout row align-end class="header-alignment-padding-right">
                            <v-spacer></v-spacer>
                            <v-btn @click='onShowCreateBudgetLibraryDialog(false)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                                Create New Library
                            </v-btn>
                        </v-layout>
                    </v-flex>

        </v-layout>
        <v-divider v-if='hasScenario && hasSelectedLibrary' />
        <v-layout row justify-space-between v-show='hasSelectedLibrary'>
            <v-flex xs4>
                <v-layout row>
                    <v-btn @click='onShowEditBudgetsDialog' 
                    outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                        Edit Budgets
                    </v-btn>
                    <v-text-field :disabled='budgets.length === 0' type="number" min=1 :mask="'##########'" 
                    class="ghd-text-field-border ghd-text-field ghd-left-paired-textbox shrink" 
                    v-bind:class="{ 'ghd-blue-text-field': budgets.length !== 0}"
                    outline v-model.number="range"/>
                    <v-btn :disabled='budgets.length === 0'
                        @click='onSubmitAddBudgetYearRange'
                        class='ghd-right-paired-button ghd-blue ghd-button-text ghd-outline-button-padding ' outline>
                        Add Year(s)
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-spacer></v-spacer>
            <v-flex xs4>
                <v-layout row align-end>
                    <v-spacer></v-spacer>
                    <v-btn :disabled='false' @click='showImportExportInvestmentBudgetsDialog = true; showReminder = false'
                        flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                        Upload
                    </v-btn>
                    <v-divider class="investment-divider" inset vertical>
                    </v-divider>
                    <v-btn :disabled='false' @click='exportInvestmentBudgets()'
                        flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                        Download
                    </v-btn>
                </v-layout>
            </v-flex>
        </v-layout>
        <!-- <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    
                    
                    <v-text-field label='Library Name' v-if='hasSelectedLibrary && !hasScenario'
                                  v-model='selectedBudgetLibrary.name'
                                  :rules="[rules['generalRules'].valueIsNotEmpty]">
                        <template slot='append'>
                            <v-btn @click='librarySelectItemValue = null' class='ara-orange' icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    
                </v-flex>
            </v-layout>
        </v-flex> -->
        <!-- visible on both pages -->
        <!-- <v-divider v-show='hasSelectedLibrary && hasScenario'></v-divider> -->
        <v-flex v-show='hasSelectedLibrary' xs12>
            <!-- <v-layout justify-center>
                <v-flex xs6>
                    <v-layout justify-space-between>
                        
                        <v-btn :disabled='budgets.length === 0'
                               @click='onAddBudgetYear'
                               class='ara-blue-bg white--text'>
                            {{ addYearLabel }}
                        </v-btn>
                        <v-btn
                            :disabled='budgets.length === 0 || budgetYearsGridData.length === 0'
                            @click='onRemoveLatestBudgetYear'
                            class='ara-orange-bg white--text'>
                            {{ deleteYearLabel }}
                        </v-btn>
                        <v-btn :disabled='budgets.length === 0'
                               @click='showSetRangeForAddingBudgetYearsDialog = true'
                               class='ara-blue-bg white--text'>
                            Add Year Range
                        </v-btn>
                        <v-btn
                            :disabled='budgets.length === 0 || budgetYearsGridData.length === 0'
                            @click='showSetRangeForDeletingBudgetYearsDialog = true'
                            class='ara-orange-bg white--text'>
                            Delete Year Range
                        </v-btn>
                        <v-btn :disabled='false' @click='showImportExportInvestmentBudgetsDialog = true'
                               class='ara-blue-bg white--text'>
                            Import/Export
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout> -->
            <!-- datatable -->
                <v-flex >
                   <v-data-table :headers='budgetYearsGridHeaders' :items='budgetYearsGridData'
                                      class='v-table__overflow ghd-table' item-key='year' select-all 
                                      v-model='selectedBudgetYearsGridData' :must-sort='true'>
                            <template slot='items' slot-scope='props'>
                                <td>
                                    <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                                </td>
                                <td v-for='header in budgetYearsGridHeaders'>  
                                    <div v-if="header.value === 'year'">
                                        <span class='sm-txt'>{{ props.item.year }}</span>
                                    </div>       
                                    <div v-if="header.value === 'action'">
                                        <v-btn @click="onRemoveBudgetYear(props.item.year)"  class="ghd-blue" icon>
                                            <v-icon>fas fa-trash</v-icon>
                                        </v-btn>
                                    </div>                           
                                    <div v-if="header.value !== 'year' && header.value !== 'action'">
                                        <v-edit-dialog :return-value.sync='props.item[header.value]'
                                                       @save='onEditBudgetYearValue(props.item.year, header.value, props.item[header.value])'
                                                       large lazy persistent>
                                            <v-text-field readonly single-line class='sm-txt'
                                                          :value='formatAsCurrency(props.item[header.value])'
                                                          :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                            <template slot='input'>
                                                <v-text-field label='Edit' single-line
                                                              v-model.number='props.item[header.value]'
                                                              v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                            </template>
                                        </v-edit-dialog>
                                    </div>
                                    
                                </td>
                            </template>
                        </v-data-table>   
                        <v-btn :disabled='selectedBudgetYears.length === 0' @click='onRemoveBudgetYears'
                            class='ghd-blue ghd-button' flat>
                            Delete Selected
                        </v-btn>  
                </v-flex>
        </v-flex>
        <!-- <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider> -->
        <v-flex v-show='hasSelectedLibrary && !hasScenario' xs12>
            <v-layout justify-center>
                <v-flex>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea no-resize outline rows='4'
                                v-model='selectedBudgetLibrary.description'
                                @input='selectedBudgetLibrary = {...selectedBudgetLibrary, description: $event}'
                                class="ghd-text-field-border">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-center row v-show='hasSelectedLibrary'>
                <v-btn :disabled='!hasUnsavedChanges' @click='onDiscardChanges' flat class='ghd-blue ghd-button-text ghd-button'
                       v-show='hasScenario'>
                    Cancel
                </v-btn>
                <v-btn @click='onShowConfirmDeleteAlert' flat class='ghd-blue ghd-button-text ghd-button' v-show='!hasScenario'
                       :disabled='!hasLibraryEditPermission'>
                    Delete Library
                </v-btn>
                <v-btn :disabled='disableCrudButton()'
                       @click='onShowCreateBudgetLibraryDialog(true)'
                       class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
                    Create as New Library
                </v-btn>
                <v-btn :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                       @click='onUpsertBudgetLibrary()'
                       class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                       v-show='selectedScenarioId === uuidNIL'>
                    Update Library
                </v-btn>
                <v-btn :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                       @click='onUpsertInvestment()'
                       class='ghd-blue-bg white--text ghd-button-text ghd-button'
                       v-show='selectedScenarioId !== uuidNIL'>
                    Save
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onSubmitConfirmDeleteAlertResult' />

        <CreateBudgetLibraryDialog :dialogData='createBudgetLibraryDialogData'
                                   @submit='onSubmitCreateCreateBudgetLibraryDialogResult' />

        <SetRangeForAddingBudgetYearsDialog :showDialog='showSetRangeForAddingBudgetYearsDialog'
                                            :startYear='getNextYear()'
                                            @submit='onSubmitAddBudgetYearRange' />

        <SetRangeForDeletingBudgetYearsDialog :showDialog='showSetRangeForDeletingBudgetYearsDialog'
                                              :endYear='getLatestYear()'
                                              :maxRange='yearsInAnalysisPeriod'
                                              @submit='onSubmitRemoveBudgetYearRange' />

        <EditBudgetsDialog :dialogData='editBudgetsDialogData' @submit='onSubmitEditBudgetsDialogResult' />

        <ImportExportInvestmentBudgetsDialog :showDialog='showImportExportInvestmentBudgetsDialog'
                                             :showReminder='showReminder'
                                             @submit='onSubmitImportExportInvestmentBudgetsDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State, Getter } from 'vuex-class';
import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
import SetRangeForDeletingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForDeletingBudgetYearsDialog.vue';
import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
import {
    Budget,
    BudgetAmount,
    BudgetLibrary,
    BudgetYearsGridData,
    emptyBudgetLibrary,
    emptyInvestmentPlan, InvestmentBudgetFileImport,
    InvestmentPlan,
    emptyBudgetAmount
} from '@/shared/models/iAM/investment';
import { any, append, clone, find, findIndex, groupBy, isNil, keys, propEq, update, contains} from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import moment from 'moment';
import {
    CreateBudgetLibraryDialogData,
    emptyCreateBudgetLibraryDialogData,
} from '@/shared/models/modals/create-budget-library-dialog-data';
import { EditBudgetsDialogData, emptyEditBudgetsDialogData } from '@/shared/models/modals/edit-budgets-dialog';
import { getLastPropertyValue, getPropertyValues } from '@/shared/utils/getter-utils';
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import { hasUnsavedChangesCore, isEqual, sortNonObjectLists } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { sorter } from '@/shared/utils/sorter-utils';
import CreateBudgetLibraryDialog
    from '@/components/investment-editor/investment-editor-dialogs/CreateBudgetLibraryDialog.vue';
import ImportExportInvestmentBudgetsDialog
    from '@/components/investment-editor/investment-editor-dialogs/ImportExportInvestmentBudgetsDialog.vue';
import { ImportExportInvestmentBudgetsDialogResult } from '@/shared/models/modals/import-export-investment-budgets-dialog-result';
import InvestmentService from '@/services/investment.service';
import { AxiosResponse } from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { getUserName } from '@/shared/utils/get-user-info';

@Component({
    components: {
        ImportExportInvestmentBudgetsDialog,
        CreateBudgetLibraryDialog,
        SetRangeForAddingBudgetYearsDialog,
        SetRangeForDeletingBudgetYearsDialog,
        EditBudgetsDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class InvestmentEditor extends Vue {
    @State(state => state.investmentModule.budgetLibraries) stateBudgetLibraries: BudgetLibrary[];
    @State(state => state.investmentModule.selectedBudgetLibrary) stateSelectedBudgetLibrary: BudgetLibrary;
    @State(state => state.investmentModule.investmentPlan) stateInvestmentPlan: InvestmentPlan;
    @State(state => state.investmentModule.scenarioBudgets) stateScenarioBudgets: Budget[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.investmentModule.isSuccessfulImport) isSuccessfulImport: boolean
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;

    @Action('getInvestment') getInvestmentAction: any;
    @Action('getBudgetLibraries') getBudgetLibrariesAction: any;
    @Action('selectBudgetLibrary') selectBudgetLibraryAction: any;
    @Action('upsertInvestment') upsertInvestmentAction: any;
    @Action('upsertBudgetLibrary') upsertBudgetLibraryAction: any;
    @Action('deleteBudgetLibrary') deleteBudgetLibraryAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('importScenarioInvestmentBudgetsFile') importScenarioInvestmentBudgetsFileAction: any;
    @Action('importLibraryInvestmentBudgetsFile') importLibraryInvestmentBudgetsFileAction: any;
    @Action('getCriterionLibraries') getCriterionLibrariesAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    selectedBudgetLibrary: BudgetLibrary = clone(emptyBudgetLibrary);
    investmentPlan: InvestmentPlan = clone(emptyInvestmentPlan);
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = '';
    actionHeader: DataTableHeader = { text: 'Action', value: 'action', align: 'left', sortable: false, class: '', width: ''}
    budgetYearsGridHeaders: DataTableHeader[] = [
        { text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: '' },
        this.actionHeader
    ];
    budgetYearsGridData: BudgetYearsGridData[] = [];
    selectedBudgetYearsGridData: BudgetYearsGridData[] = [];
    selectedBudgetYears: number[] = [];
    createBudgetLibraryDialogData: CreateBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);
    editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
    showSetRangeForAddingBudgetYearsDialog: boolean = false;
    showSetRangeForDeletingBudgetYearsDialog: boolean = false;
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    uuidNIL: string = getBlankGuid();
    rules: InputValidationRules = clone(rules);
    showImportExportInvestmentBudgetsDialog: boolean = false;
    hasScenario: boolean = false;
    hasInvestmentPlanForScenario: boolean = false;
    hasCreatedLibrary: boolean = false;
    budgets: Budget[] = [];
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;
    showReminder: boolean = false;
    range: number = 1;

    get addYearLabel() {
        return 'Add Year (' + this.getNextYear() + ')';
    }

    get deleteYearLabel() {
        const latestYear = this.getLatestYear();
        return latestYear ? 'Delete Year (' + latestYear + ')' : 'Delete Year';
    }

    get yearsInAnalysisPeriod() {
        return this.investmentPlan.numberOfYearsInAnalysisPeriod;
    }

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getBudgetLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.Investment) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getInvestmentAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateBudgetLibraries')
    onStateBudgetLibrariesChanged() {
        this.librarySelectItems = this.stateBudgetLibraries
            .map((library: BudgetLibrary) => ({
                text: library.name,
                value: library.id,
            }));
    }

    @Watch('selectedBudgetYearsGridData')
   onSelectedRowsChanged() {
         this.selectedBudgetYears = getPropertyValues('year', this.selectedBudgetYearsGridData) as number[];
     }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectBudgetLibraryAction(this.librarySelectItemValue);
    }

    @Watch('stateSelectedBudgetLibrary')
    onStateSelectedBudgetLibraryChanged() {
        this.selectedBudgetLibrary = clone(this.stateSelectedBudgetLibrary);
    }

    @Watch('selectedBudgetLibrary', {deep: true})
    onSelectedBudgetLibraryChanged() {
        this.hasSelectedLibrary = this.selectedBudgetLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (this.hasScenario) {
            this.budgets = this.selectedBudgetLibrary.budgets.map((budget: Budget) => ({
                ...budget,
                id: getNewGuid(),
                budgetAmounts: hasValue(budget.budgetAmounts)
                    ? budget.budgetAmounts.map((budgetAmount: BudgetAmount) => ({
                        ...budgetAmount,
                        id: getNewGuid(),
                    }))
                    : [] as BudgetAmount[],
            }));
        } else {
            this.budgets = clone(this.selectedBudgetLibrary.budgets);
        }
    }

    @Watch('stateInvestmentPlan')
    onStateInvestmentPlanChanged() {
        this.cloneStateInvestmentPlan();
        this.hasInvestmentPlanForScenario = true;
    }

    @Watch('stateScenarioBudgets')
    onStateScenarioBudgetsChanged() {
        if (this.hasScenario) {
            this.budgets = clone(this.stateScenarioBudgets);
        }
    }

    @Watch('budgets')
    onScenarioBudgetsChanged() {
        this.setGridHeaders();
        this.setGridData();
        if (this.hasScenario) {
            this.syncInvestmentPlanWithBudgets();
        }
        this.setHasUnsavedChangesFlag();
    }

    @Watch('investmentPlan')
    onInvestmentPlanChanged() {
        this.setHasUnsavedChangesFlag();
    }

    onRemoveBudgetYears() {
        this.budgets.forEach((budget:Budget) =>
        {
            budget.budgetAmounts = budget.budgetAmounts.filter((amount:BudgetAmount) => !contains(amount.year, this.selectedBudgetYears))
        });
        this.onScenarioBudgetsChanged()
    }

    onRemoveBudgetYear(year: number){        
         this.budgets.forEach((budget:Budget) =>
         {
             budget.budgetAmounts = budget.budgetAmounts.filter((amount:BudgetAmount) => amount.year != year)
         });
         this.onScenarioBudgetsChanged()
    }

    cloneStateInvestmentPlan() {
        const investmentPlan: InvestmentPlan = clone(this.stateInvestmentPlan);
        this.investmentPlan = {
            ...investmentPlan,
            id: investmentPlan.id === this.uuidNIL ? getNewGuid() : investmentPlan.id,
        };
    }

    setHasUnsavedChangesFlag() {
        if (this.hasScenario) {
            const budgetsHaveUnsavedChanges: boolean = hasUnsavedChangesCore('', this.budgets, this.stateScenarioBudgets);


            const clonedInvestmentPlan: InvestmentPlan = clone(this.investmentPlan);
            const investmentPlan: InvestmentPlan = {
                ...clonedInvestmentPlan,
                minimumProjectCostLimit: hasValue(clonedInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
            const clonedStateInvestmentPlan: InvestmentPlan = clone(this.stateInvestmentPlan);
            const stateInvestmentPlan: InvestmentPlan = {
                ...clonedStateInvestmentPlan,
                minimumProjectCostLimit: hasValue(clonedStateInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedStateInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
            const investmentPlanHasUnsavedChanges: boolean = hasUnsavedChangesCore('', investmentPlan, stateInvestmentPlan);


            this.setHasUnsavedChangesAction({ value: budgetsHaveUnsavedChanges || investmentPlanHasUnsavedChanges });
        } else if (this.hasSelectedLibrary) {
            const hasUnsavedChanges: boolean = hasUnsavedChangesCore('',
                { ...clone(this.selectedBudgetLibrary), budgets: clone(this.budgets) },
                this.stateSelectedBudgetLibrary);
            this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }

    setGridHeaders() {
        const budgetNames: string[] = sorter(getPropertyValues('name', this.budgets)) as string[];
        const budgetHeaders: DataTableHeader[] = budgetNames
            .map((name: string) => ({
                text: name,
                value: name,
                sortable: true,
                align: 'left',
                class: '',
                width: '',
            }) as DataTableHeader);
        this.budgetYearsGridHeaders = [
            this.budgetYearsGridHeaders[0], 
             ...budgetHeaders,
             this.actionHeader
             ];
    }

    setGridData() {
        this.budgetYearsGridData = [];

        const budgetAmounts: BudgetAmount[] = this.budgets
            .flatMap((budget: Budget) => budget.budgetAmounts);
        const groupBudgetAmountsByYear = groupBy((budgetAmount: BudgetAmount) => budgetAmount.year.toString());
        const groupedBudgetAmounts = groupBudgetAmountsByYear(budgetAmounts);

        keys(groupedBudgetAmounts).forEach((year: any) => {
            const gridDataRow: BudgetYearsGridData = { year: parseInt(year) };

            const budgetAmounts: BudgetAmount[] = groupedBudgetAmounts[year];

            const budgetNames: string[] = sorter(getPropertyValues('name', this.budgets)) as string[];
            for (let i = 0; i < budgetNames.length; i++) {
                const budgetAmount: BudgetAmount = budgetAmounts
                    .find((ba: BudgetAmount) => ba.budgetName === budgetNames[i]) as BudgetAmount;

                gridDataRow[budgetNames[i]] = hasValue(budgetAmount) ? budgetAmount.value : 0;
            }

            this.budgetYearsGridData.push(gridDataRow);
        });
    }

    syncInvestmentPlanWithBudgets() {
        const allBudgetAmounts: BudgetAmount[] = this.budgets
            .flatMap((budget: Budget) => budget.budgetAmounts);
        const allBudgetYears: number[] = sorter(getPropertyValues('year', allBudgetAmounts)) as number[];

        this.investmentPlan.firstYearOfAnalysisPeriod = hasValue(allBudgetYears)
            ? allBudgetYears[0] : this.investmentPlan.firstYearOfAnalysisPeriod;
        this.investmentPlan.numberOfYearsInAnalysisPeriod = hasValue(allBudgetYears)
            ? allBudgetYears.length : 1;
    }

    onShowCreateBudgetLibraryDialog(createAsNewLibrary: boolean) {
        this.createBudgetLibraryDialogData = {
            showDialog: true,
            budgets: createAsNewLibrary ? this.budgets : [],
        };
    }

    onSubmitCreateCreateBudgetLibraryDialogResult(budgetLibrary: BudgetLibrary) {
        this.createBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);

        if (!isNil(budgetLibrary)) {
            this.upsertBudgetLibraryAction(budgetLibrary);
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = budgetLibrary.name;
        }
    }

    getLatestYear(): number {
        const latestYear: number = getLastPropertyValue('year', this.budgetYearsGridData);
        return latestYear;
    }

    getNextYear(): number {
        const latestYear: number = this.getLatestYear();
        const nextYear = hasValue(latestYear) ? latestYear + 1 : moment().year();
        return nextYear;
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedBudgetLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedBudgetLibrary.owner) == getUserName();
    }

    onAddBudgetYear() {
        const nextYear: number = this.getNextYear();

        const budgets: Budget[] = clone(this.budgets);

        budgets.forEach((budget: Budget) => {
            const newBudgetAmount: BudgetAmount = {
                id: getNewGuid(),
                budgetName: budget.name,
                year: nextYear,
                value: 0,
            };
            budget.budgetAmounts = append(newBudgetAmount, budget.budgetAmounts);
        });

        this.budgets = clone(budgets);
    }

    onRemoveLatestBudgetYear() {
        const latestYear: number = this.getLatestYear();

        const budgets: Budget[] = clone(this.budgets);

        budgets.forEach((budget: Budget) => {
            budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                !(budgetAmount.year == latestYear));
        });

        this.budgets = clone(budgets);
    }

    onSubmitAddBudgetYearRange() {
        this.showSetRangeForAddingBudgetYearsDialog = false;

        if (this.range > 0) {
            const latestYear: number = this.getLatestYear();
            const startYear: number = hasValue(latestYear) ? latestYear + 1 : moment().year();
            const endYear = moment().year(startYear).add(this.range, 'years').year();

            const budgets: Budget[] = clone(this.budgets);

            for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                budgets.forEach((budget: Budget) => {
                    budget.budgetAmounts.push({
                        id: getNewGuid(),
                        budgetName: budget.name,
                        year: currentYear,
                        value: 0,
                    });
                });
            }

            this.budgets = clone(budgets);
        }
    }

    onSubmitRemoveBudgetYearRange(range: number) {
        this.showSetRangeForDeletingBudgetYearsDialog = false;

        if (range > 0) {
            const endYear: number = this.getLatestYear();
            const startYear: number = endYear - range + 1;

            const budgets: Budget[] = clone(this.budgets);

            budgets.forEach((budget: Budget) => {
                budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                    !(budgetAmount.year >= startYear && budgetAmount.year <= endYear));
            });

            this.budgets = clone(budgets);
        }
    }

    onShowEditBudgetsDialog() {
        this.editBudgetsDialogData = {
            showDialog: true,
            budgets: this.budgets,
            scenarioId: this.selectedScenarioId,
        };
    }

    onSubmitEditBudgetsDialogResult(budgets: Budget[]) {
        this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        
        const budgetWithBudgetAmounts = budgets.find(b => b.budgetAmounts.length !== 0);
        budgets.forEach((budget: Budget) => {
           if(budget.budgetAmounts.length == 0)
            {                
               const budgetAmounts = hasValue(budgetWithBudgetAmounts) ? budgetWithBudgetAmounts!.budgetAmounts : [];
               budgetAmounts.forEach(budgetAmount => {
                   budget.budgetAmounts.push({
                       ...emptyBudgetAmount,
                       id: getNewGuid(),
                       budgetName: budget.name,
                       year: budgetAmount.year
                   });
               });                              
            } else {
               budget.budgetAmounts.forEach((budgetAmount: BudgetAmount) => budgetAmount.budgetName = budget.name);
           }
        });

        if (!isNil(budgets)) {
            this.budgets = clone(budgets);
        }
    }

    exportInvestmentBudgets()
    {
        const id: string = this.hasScenario ? this.selectedScenarioId : this.selectedBudgetLibrary.id;
        InvestmentService.exportInvestmentBudgets(id, this.hasScenario)
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                const fileInfo: FileInfo = response.data as FileInfo;
                FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
            }
        });
    }
    onSubmitImportExportInvestmentBudgetsDialogResult(result: ImportExportInvestmentBudgetsDialogResult) {
        this.showImportExportInvestmentBudgetsDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {
                

            } else if (hasValue(result.file)) {
                const data: InvestmentBudgetFileImport = {
                    file: result.file,
                    overwriteBudgets: result.overwriteBudgets,
                };

                if (this.hasScenario) {
                    this.importScenarioInvestmentBudgetsFileAction({
                        ...data,
                        id: this.selectedScenarioId,
                        currentUserCriteriaFilter: this.currentUserCriteriaFilter
                    })
                    .then((response: any) => {
                            this.getCriterionLibrariesAction();
                            this.showReminder = this.isSuccessfulImport
                    });
                } else {
                    this.importLibraryInvestmentBudgetsFileAction({
                        ...data,
                        id: this.selectedBudgetLibrary.id,
                        currentUserCriteriaFilter: this.currentUserCriteriaFilter
                    })
                    .then(() => {
                            this.getCriterionLibrariesAction();
                            this.showReminder = this.isSuccessfulImport
                    });
                }

            }
        }
    }

    onEditBudgetYearValue(year: number, budgetName: string, value: number) {
        if (any(propEq('name', budgetName), this.budgets)) {
            const budget: Budget = find(propEq('name', budgetName), this.budgets) as Budget;

            if (any(propEq('year', year), budget.budgetAmounts)) {
                const budgetAmount: BudgetAmount = find(propEq('year', year), budget.budgetAmounts) as BudgetAmount;
                budget.budgetAmounts = update(
                    findIndex(propEq('id', budgetAmount.id), budget.budgetAmounts),
                    {
                        ...budgetAmount,
                        value: hasValue(value)
                            ? parseFloat(value.toString().replace(/(\$*)(\,*)/g, ''))
                            : 0,
                    }, budget.budgetAmounts);

                this.budgets = update(findIndex(propEq('id', budget.id), this.budgets), clone(budget), this.budgets);
            }
        }
    }

    onEditInvestmentPlan(property: string, value: any) {
        this.investmentPlan = setItemPropertyValue(property, value, this.investmentPlan);
    }

    onUpsertInvestment() {
        const investmentPlan: InvestmentPlan = clone(this.investmentPlan);

        this.upsertInvestmentAction({
            investment: {
                scenarioBudgets: clone(this.budgets),
                investmentPlan: {
                    ...investmentPlan,
                    minimumProjectCostLimit: hasValue(investmentPlan.minimumProjectCostLimit)
                        ? parseFloat(investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                        : 1000,
                },
            },
            scenarioId: this.selectedScenarioId,
        })
            .then(() => this.librarySelectItemValue = null);
    }

    onUpsertBudgetLibrary() {
        const budgetLibrary: BudgetLibrary = clone(this.selectedBudgetLibrary);

        this.upsertBudgetLibraryAction({ ...budgetLibrary, budgets: clone(this.budgets) });
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.cloneStateInvestmentPlan();
                this.budgets = clone(this.stateScenarioBudgets);
            }
        });
    }

    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
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
            this.librarySelectItemValue = null;
            this.deleteBudgetLibraryAction(this.selectedBudgetLibrary.id);
        }
    }

    disableCrudButton() {
        const allBudgetDataIsValid: boolean = this.budgets.every((budget: Budget) => {
            return budget.budgetAmounts
                .every((budgetAmount: BudgetAmount) =>
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.year) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
        });

        if (this.hasSelectedLibrary) {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedBudgetLibrary.name) === true &&
                allBudgetDataIsValid);
        } else if (this.hasScenario) {
            const allInvestmentPlanDataIsValid: boolean = this.rules['generalRules'].valueIsNotEmpty(this.investmentPlan.minimumProjectCostLimit) === true &&
                this.rules['investmentRules'].minCostLimitGreaterThanZero(this.investmentPlan.minimumProjectCostLimit) === true &&
                this.rules['generalRules'].valueIsNotEmpty(this.investmentPlan.inflationRatePercentage) === true &&
                this.rules['generalRules'].valueIsWithinRange(this.investmentPlan.inflationRatePercentage, [0, 100]);

            return !(allBudgetDataIsValid && allInvestmentPlanDataIsValid);
        }

        this.disableCrudButtonsResult = !allBudgetDataIsValid;
        return !allBudgetDataIsValid;
    }
}
</script>

<style>
.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}

.budget-year-amount-input {
    border: 1px solid;
    width: 100%;
}



.invest-owner-padding{
    padding-top: 7px;
}

.header-alignment-padding-center{
    padding-top: 54px
}

.header-alignment-padding-right{
    padding-top: 48px
}

.investment-divider{
    height: 22px;
    margin-top: 12px !important;
}
</style>
