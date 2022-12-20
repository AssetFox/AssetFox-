<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout row style="margin-top:-40px;">
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Select an Investment library</span></v-subheader>
                    <v-select :items='librarySelectItems'
                            append-icon=$vuetify.icons.ghd-down
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
                        <v-checkbox class='sharing header-text-content' label='Shared' v-model='selectedBudgetLibrary.isShared'
                        @change="checkHasUnsavedChanges()" />
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
            <!-- only for scenario -->
            <v-layout row style="margin-top:-40px;">
                <!-- text boxes for scenario only -->
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>First Year of Analysis Period</span></v-subheader>
                    <v-text-field  outline
                                    @change='onEditInvestmentPlan("firstYearOfAnalysisPeriod", $event)'
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                                    :mask="'####'"
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
                                    :disabled="!hasAdminAccess" 
                                    class="ghd-text-field-border ghd-text-field"/>
                </v-flex>
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Inflation Rate Percentage</span></v-subheader>
                    <v-text-field outline
                                    v-model='investmentPlan.inflationRatePercentage'
                                    @change='onEditInvestmentPlan("inflationRatePercentage", $event)'
                                    :mask="'###'"
                                    :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0,100])]"
                                    :disabled="!hasAdminAccess" 
                                    class="ghd-text-field-border ghd-text-field"/>
                </v-flex>
                <v-flex xs4 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-switch style="margin-left:10px;margin-top:50px;"
                        class="ghd-checkbox"
                        label="Allow Funding Carryover"
                        :disabled="!hasAdminAccess"
                        v-model="investmentPlan.shouldAccumulateUnusedBudgetAmounts"
                        @change='onEditInvestmentPlan("shouldAccumulateUnusedBudgetAmounts", $event)'
                    />
                </v-flex>
            </v-layout>
            <v-divider v-if='hasScenario || hasSelectedLibrary' />
            <v-layout row justify-space-between v-show='hasSelectedLibrary || hasScenario'>
                <v-flex xs4>
                    <v-layout row>
                        <v-btn @click='onShowEditBudgetsDialog' 
                        outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Edit Budgets
                        </v-btn>
                        <v-text-field :disabled='currentPage.length === 0' type="number" min=1 :mask="'##########'" 
                        class="ghd-text-field-border ghd-text-field ghd-left-paired-textbox shrink" 
                        v-bind:class="{ 'ghd-blue-text-field': currentPage.length !== 0}"
                        outline v-model.number="range"/>
                        <v-btn :disabled='currentPage.length === 0'
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
                        <v-btn :disabled='false' @click='showImportExportInvestmentBudgetsDialog = true;'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Upload
                        </v-btn>
                        <v-divider class="investment-divider" inset vertical>
                        </v-divider>
                        <v-btn :disabled='false' @click='exportInvestmentBudgets()'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                            </v-divider>
                            <v-btn :disabled='false' @click='OnDownloadTemplateClick()'
                                flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                                Download Template
                            </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>            
        <!-- datatable -->
            <v-flex >
                <v-data-table 
                    :headers='budgetYearsGridHeaders' 
                    :items="budgetYearsGridData"
                    class='v-table__overflow ghd-table' 
                    item-key='year' 
                    select-all 
                    sort-icon=$vuetify.icons.ghd-table-sort
                    v-model='selectedBudgetYearsGridData' 
                    :pagination.sync="pagination"
                    :total-items="totalItems"
                    :rows-per-page-items=[5,10,25]
                    :must-sort='true'>
                    <template slot='items' slot-scope='props'>
                        <td>
                            <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                        </td>
                        <td v-for='header in budgetYearsGridHeaders'>  
                            <div v-if="header.value === 'year'">
                                <span class='sm-txt'>{{ props.item.year + firstYearOfAnalysisPeriodShift}}</span>
                            </div>       
                            <div v-if="header.value === 'action'">
                                <v-btn @click="onRemoveBudgetYear(props.item.year)"  class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </div>                           
                            <div v-if="header.value !== 'year' && header.value !== 'action'">
                                <v-edit-dialog :return-value.sync='props.item.values[header.value]'
                                                @save='onEditBudgetYearValue(props.item.year, header.value, props.item.values[header.value])'
                                                large lazy persistent>
                                    <v-text-field readonly single-line class='sm-txt'
                                                    :value='formatAsCurrency(props.item.values[header.value])'
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <template slot='input'>
                                        <v-text-field label='Edit' single-line
                                                        v-model.number='props.item.values[header.value]'
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
                                @input='checkHasUnsavedChanges()'
                                class="ghd-text-field-border">
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-center row v-show='hasSelectedLibrary || hasScenario'>
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
                       v-show='!hasScenario'>
                    Update Library
                </v-btn>
                <v-btn :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                       @click='onUpsertInvestment()'
                       class='ghd-blue-bg white--text ghd-button-text ghd-button'
                       v-show='hasScenario'>
                    Save
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onSubmitConfirmDeleteAlertResult' />

        <CreateBudgetLibraryDialog :dialogData='createBudgetLibraryDialogData'
                                   :libraryNames='librarySelectItemNames'
                                   @submit='onSubmitCreateCreateBudgetLibraryDialogResult' />

        <SetRangeForAddingBudgetYearsDialog :showDialog='showSetRangeForAddingBudgetYearsDialog'
                                            :startYear='getNextYear()'
                                            @submit='onSubmitAddBudgetYearRange' />

        <SetRangeForDeletingBudgetYearsDialog :showDialog='showSetRangeForDeletingBudgetYearsDialog'
                                              :endYear='lastYear'
                                              :maxRange='yearsInAnalysisPeriod'
                                              @submit='onSubmitRemoveBudgetYearRange' />

        <EditBudgetsDialog :dialogData='editBudgetsDialogData' @submit='onSubmitEditBudgetsDialogResult' />

        <ImportExportInvestmentBudgetsDialog :showDialog='showImportExportInvestmentBudgetsDialog'
                                             @submit='onSubmitImportExportInvestmentBudgetsDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State, Getter, Mutation } from 'vuex-class';
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
    emptyBudgetAmount,
    LibraryInvestmentBudgetFileImport
} from '@/shared/models/iAM/investment';
import { any, append, clone, find, findIndex, groupBy, isNil, keys, propEq, update, contains, sort} from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import moment from 'moment';
import {
    CreateBudgetLibraryDialogData,
    emptyCreateBudgetLibraryDialogData,
} from '@/shared/models/modals/create-budget-library-dialog-data';
import { EditBudgetsDialogData, EmitedBudgetChanges, emptyEditBudgetsDialogData } from '@/shared/models/modals/edit-budgets-dialog';
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
import { InvestmentPagingRequestModel, InvestmentLibraryUpsertPagingRequestModel, InvestmentPagingSyncModel, InvestmentPagingPage } from '@/shared/models/iAM/paging';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { http2XX } from '@/shared/utils/http-utils';
import { BudgetGridRow } from '@/shared/models/iAM/treatment';
import {
    mapToIndexSignature
} from '../../shared/utils/conversion-utils';

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
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;
    @State(state => state.investmentModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
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
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    @Mutation('budgetLibraryMutator') budgetLibraryMutator: any;
    @Mutation('selectedBudgetLibraryMutator') selectedBudgetLibraryMutator: any;
    @Mutation('investmentPlanMutator') investmentPlanMutator: any;

    addedBudgets: Budget[] = [];
    updatedBudgetsMap:Map<string, [Budget, Budget]> = new Map<string, [Budget, Budget]>();//0: original value | 1: updated value
    deletionBudgetIds: string[] = [];
    BudgetCache: Budget[] = [];
    budgetAmountCache: BudgetAmount[] = [];
    updatedBudgetAmountsMaps:Map<string, [BudgetAmount, BudgetAmount]> = new Map<string, [BudgetAmount, BudgetAmount]>();//0: original value | 1: updated value 
    addedBudgetAmounts: Map<string, BudgetAmount[]> = new  Map<string, BudgetAmount[]>();
    deletionYears: number[] = [] 
    updatedBudgetAmounts:  Map<string, BudgetAmount[]> = new  Map<string, BudgetAmount[]>();
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: Budget[] = [];
    lastYear: number = 0;
    firstYear: number = 0;
    initializing: boolean = true;

    originalFirstYear: number = 0
    firstYearOfAnalysisPeriodShift: number = 0;

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = '';

    selectedBudgetLibrary: BudgetLibrary = clone(emptyBudgetLibrary);
    investmentPlan: InvestmentPlan = clone(emptyInvestmentPlan);
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    
    librarySelectItemNames: string[] = [];
    actionHeader: DataTableHeader = { text: 'Action', value: 'action', align: 'left', sortable: false, class: '', width: ''}
    budgetYearsGridHeaders: DataTableHeader[] = [
        { text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: '' },
        this.actionHeader
    ];
    budgetYearsGridData: BudgetYearsGridData[] = []; 
    selectedBudgetYearsGridData: BudgetYearsGridData[] = [];
    selectedBudgetYearsGridDataWithoutShift: BudgetYearsGridData[] = [];  
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
    range: number = 1;
   
    get addYearLabel() {
        return 'Add Year (' + this.getNextYear() + ')';
    }

    get deleteYearLabel() {
        const latestYear = this.lastYear;
        return latestYear ? 'Delete Year (' + latestYear + ')' : 'Delete Year';
    }

    get yearsInAnalysisPeriod() {
        return this.investmentPlan.numberOfYearsInAnalysisPeriod;
    }

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getHasPermittedAccessAction();
            vm.getBudgetLibrariesAction().then(() => {
                if (to.path.indexOf(ScenarioRoutePaths.Investment) !== -1) {
                    vm.selectedScenarioId = to.query.scenarioId;

                    if (vm.selectedScenarioId === vm.uuidNIL) {
                        vm.addErrorNotificationAction({
                            message: 'Found no selected scenario for edit',
                        });
                        vm.$router.push('/Scenarios/');
                    }

                    vm.hasScenario = true;
                    vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                        vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });        
                        vm.initializePages();
                    });                                        
                }
                else
                    vm.initializing = false;
                
            });          
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    // Watchers
    @Watch('pagination')
    async onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        const request: InvestmentPagingRequestModel= {
            page: page,
            rowsPerPage: rowsPerPage,
            pagingSync: {
                libraryId: this.selectedBudgetLibrary.id === this.uuidNIL ? null : this.selectedBudgetLibrary.id,
                updatedBudgets: Array.from(this.updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: this.deletionBudgetIds,
                addedBudgets: this.addedBudgets,
                deletionyears: this.deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( this.updatedBudgetAmounts),
                Investment: !isNil(this.investmentPlan) ? {
                ...this.investmentPlan,
                minimumProjectCostLimit: hasValue(this.investmentPlan.minimumProjectCostLimit)
                    ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
                } : this.investmentPlan,
                addedBudgetAmounts: mapToIndexSignature(this.addedBudgetAmounts),
                firstYearAnalysisBudgetShift: this.firstYearOfAnalysisPeriodShift
            },           
            sortColumn: sortBy === '' ? 'year' : sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            await InvestmentService.getScenarioInvestmentPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    this.firstYear = data.firstYear;
                    this.currentPage = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    this.BudgetCache = clone(this.currentPage);
                    this.BudgetCache.forEach(_ => _.budgetAmounts = []);
                    this.budgetAmountCache = this.currentPage.flatMap(_ => _.budgetAmounts)
                    this.totalItems = data.totalItems;
                    this.investmentPlan = data.investmentPlan;                   
                    this.lastYear = data.lastYear;
                    
                    this.syncInvestmentPlanWithBudgets();
                    
                }
            });
        }            
        else if(this.hasSelectedLibrary)
            await InvestmentService.getLibraryInvestmentPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    this.currentPage = data.items;
                    this.BudgetCache = clone(this.currentPage);
                    this.BudgetCache.forEach(_ => _.budgetAmounts = []);
                    this.budgetAmountCache = this.currentPage.flatMap(_ => _.budgetAmounts)
                    this.totalItems = data.totalItems;
                    this.lastYear = data.lastYear
                }
            });     
    }

    @Watch('deletionBudgetIds')
    onDeletionBudgetIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedBudgets')
    onAddedBudgetsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('deletionyears')
    onDeletionyearsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedBudgetAmounts', {deep: true})
    onAddedBudgetAmountsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('stateBudgetLibraries')
    onStateBudgetLibrariesChanged() {
        this.librarySelectItems = this.stateBudgetLibraries
            .map((library: BudgetLibrary) => ({
                text: library.name,
                value: library.id,
            }));
            // Send names to new library dialog to prevent
            // duplicate names
            this.librarySelectItems.forEach(element => {
                this.librarySelectItemNames.push(element.text);                
            });
    }

    @Watch('selectedBudgetYearsGridData')
   onSelectedRowsChanged() {
        this.selectedBudgetYears = getPropertyValues('year', this.selectedBudgetYearsGridData) as number[];
     }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChangedCheckUnsaved(){
        if(this.hasScenario){
            this.onSelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged)
            this.CheckUnsavedDialog(this.onSelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            })
        this.librarySelectItemValueAllowedChanged = true;
    }
    onSelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue
        this.selectBudgetLibraryAction(this.librarySelectItemValue);
    }

    @Watch('stateSelectedBudgetLibrary')
    onStateSelectedBudgetLibraryChanged() {
        this.selectedBudgetLibrary = clone(this.stateSelectedBudgetLibrary);
    }

    @Watch('selectedBudgetLibrary')
    onSelectedBudgetLibraryChanged() {
        this.hasSelectedLibrary = this.selectedBudgetLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        this.clearChanges()
        this.onPaginationChanged()
    }

    @Watch('stateInvestmentPlan')
    onStateInvestmentPlanChanged() {
        this.cloneStateInvestmentPlan();
        this.hasInvestmentPlanForScenario = true;
        this.checkHasUnsavedChanges();
    }

    @Watch('currentPage')
    onScenarioBudgetsChanged() {
        this.setGridHeaders();
        this.setGridData();
        
        this.checkHasUnsavedChanges()
    }

    @Watch('investmentPlan')
    onInvestmentPlanChanged() {
        this.checkHasUnsavedChanges()
        if(this.hasScenario){
            const firstYear = +this.investmentPlan.firstYearOfAnalysisPeriod;
            const stateFirstYear = +this.stateInvestmentPlan.firstYearOfAnalysisPeriod;
            this.firstYearOfAnalysisPeriodShift = (firstYear - this.originalFirstYear) - (this.firstYear === 0 ? 0 : (this.firstYear - this.originalFirstYear));
        }
            
        if(this.investmentPlan.id === this.uuidNIL)
            this.investmentPlan.id = getNewGuid();
        this.hasInvestmentPlanForScenario = true;
    }

    @Watch('firstYearOfAnalysisPeriodShift')
    onFirstYearOfAnalysisPeriodShiftChanged(){
        this.setGridData();
    }

    onRemoveBudgetYears() {
        this.deletionYears = this.deletionYears.concat(this.selectedBudgetYears)
        let deletedAddYears: number[] = [];
        for(let [key, value] of this.addedBudgetAmounts){
            let val = this.addedBudgetAmounts.get(key)! 
            val = val.filter(v => {
                if(!this.deletionYears.includes(v.year)){                  
                    return true;
                }
 
                deletedAddYears.push(v.year);  

                return false;
            })
            if(val.length == 0)
                this.addedBudgetAmounts.delete(key)
        }
        if(deletedAddYears.length > 0){
            this.selectedBudgetYears = this.selectedBudgetYears.filter(year => !deletedAddYears.includes(year))
            this.deletionYears =  this.deletionYears.concat(this.selectedBudgetYears)
            this.onPaginationChanged();
            return;
        }

        let deletedUpdateIds: string[] = [];

        for(let [key, value] of this.updatedBudgetAmounts){
            let val = this.updatedBudgetAmounts.get(key)!
            const ids = val.filter(v => this.deletionYears.includes(v.year)).map(v => v.id)
            deletedUpdateIds = deletedUpdateIds.concat(ids)
            val = val.filter(v => !this.deletionYears.includes(v.year))
            if(val.length == 0)
                this.updatedBudgetAmounts.delete(key)
        }

        deletedUpdateIds.forEach(id => {
            this.updatedBudgetAmountsMaps.delete(id);
        })

        this.onPaginationChanged();
    }

    onRemoveBudgetYear(year: number){                
        let isyearAdded = false;
        for(let [key, value] of this.addedBudgetAmounts){
            let val = this.addedBudgetAmounts.get(key)! 
            val = val.filter(v => {
                if(v.year !== year){                  
                    return true;
                }
 
                isyearAdded = true; 

                return false;
            })
            if(val.length == 0)
                this.addedBudgetAmounts.delete(key)
            else
            {
                this.addedBudgetAmounts.set(key, val)
            }
        }
        if(isyearAdded){
            this.onPaginationChanged();
            return;
        }
        this.deletionYears.push(year)

        let deleteIds: string[] = [] 
        for(let [key, value] of this.updatedBudgetAmounts){
            let val = this.updatedBudgetAmounts.get(key)!
            const ids = val.filter(v => v.year === year).map(v => v.id)
            deleteIds = deleteIds.concat(ids)
            val = val.filter(v => v.year !== year)
            if(val.length == 0)
                this.updatedBudgetAmounts.delete(key)
        }

        deleteIds.forEach(id => {
            this.updatedBudgetAmountsMaps.delete(id);
        })

        this.onPaginationChanged();
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
            const budgetsHaveUnsavedChanges: boolean = hasUnsavedChangesCore('', this.currentPage, this.stateScenarioBudgets);


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
                { ...clone(this.selectedBudgetLibrary), budgets: clone(this.currentPage) },
                this.stateSelectedBudgetLibrary);
            this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }

    setGridHeaders() {
        const budgetNames: string[] = getPropertyValues('name', this.currentPage) as string[];
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
        if(this.currentPage.length <= 0)
            return;
        for(let i = 0; i < this.currentPage[0].budgetAmounts.length; i++){
            let year = this.currentPage[0].budgetAmounts[i].year
            let values: {[budgetName: string]: number | null} = {}
            for(let o = 0; o < this.currentPage.length; o++){
                values[this.currentPage[o].name] = this.currentPage[o].budgetAmounts[i].value
            }
            this.budgetYearsGridData.push({year, values})
        }        
    }

    syncInvestmentPlanWithBudgets() {//this gets call in on pagination now       
        this.investmentPlan.numberOfYearsInAnalysisPeriod = this.totalItems > 0 ? this.totalItems : 1
    }

    onShowCreateBudgetLibraryDialog(createAsNewLibrary: boolean) {
        this.createBudgetLibraryDialogData = {
            showDialog: true,
            budgets: createAsNewLibrary ? this.currentPage : [],
        };
    }

    onSubmitCreateCreateBudgetLibraryDialogResult(budgetLibrary: BudgetLibrary) {//needs a few things
        this.createBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);

        if (!isNil(budgetLibrary)) {
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = budgetLibrary.id;
            const libraryUpsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
                library: budgetLibrary,
                isNewLibrary: true,
                pagingSync: {
                    libraryId: budgetLibrary.budgets.length === 0 || !this.hasSelectedLibrary ? null : this.selectedBudgetLibrary.id,
                    Investment: this.investmentPlan,
                    budgetsForDeletion: budgetLibrary.budgets === [] ? [] : this.deletionBudgetIds,
                    updatedBudgets: budgetLibrary.budgets === [] ? [] : Array.from(this.updatedBudgetsMap.values()).map(r => r[1]),
                    addedBudgets: budgetLibrary.budgets === [] ? [] : this.addedBudgets,
                    deletionyears: budgetLibrary.budgets === [] ? [] : this.deletionYears,
                    updatedBudgetAmounts: budgetLibrary.budgets === [] ? {} : mapToIndexSignature(this.updatedBudgetAmounts),
                    addedBudgetAmounts: budgetLibrary.budgets === [] ? {} : mapToIndexSignature(this.addedBudgetAmounts),
                    firstYearAnalysisBudgetShift: 0
                },
                scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            // value in v-currency is not parsed back to a number throwing an silent exception between UI and backend.
            const parsedMinimumProjectCostLimit: number = parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''));
            let tempInvesmentPlan: InvestmentPlan | null = libraryUpsertRequest.pagingSync.Investment;
            tempInvesmentPlan? tempInvesmentPlan.minimumProjectCostLimit = parsedMinimumProjectCostLimit : 0;
            libraryUpsertRequest.pagingSync.Investment = tempInvesmentPlan;
            
            InvestmentService.upsertBudgetLibrary(libraryUpsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') &&http2XX.test(response.status.toString())){
                    if(budgetLibrary.budgets === []){
                        this.clearChanges();
                    }

                    this.budgetLibraryMutator(budgetLibrary); // mutation actions
                    this.selectedBudgetLibraryMutator(budgetLibrary.id);
                    this.addSuccessNotificationAction({message:'Added budget library'})
                    this.resetPage();
                }
            });
        }
    }

    getNextYear(): number {
        const latestYear: number = this.lastYear;
        const nextYear = hasValue(latestYear) && latestYear !== 0 ? latestYear + 1 : moment().year();
        return nextYear;
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedBudgetLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedBudgetLibrary.owner) == getUserName();
    }

    onAddBudgetYear() {
        const nextYear: number = this.getNextYear();

        const budgets: Budget[] = clone(this.currentPage);

        budgets.forEach((budget: Budget) => {
            const newBudgetAmount: BudgetAmount = {
                id: getNewGuid(),
                budgetName: budget.name,
                year: nextYear,
                value: 0,
            };
            let amounts = this.addedBudgetAmounts.get(budget.id)
            if(!isNil(amounts))
                amounts.push(newBudgetAmount);
            else
                this.addedBudgetAmounts.set(budget.id, [newBudgetAmount])
        });

        this.onPaginationChanged();
    }

    onRemoveLatestBudgetYear() {
        const latestYear: number = this.lastYear;

        const budgets: Budget[] = clone(this.currentPage);

        budgets.forEach((budget: Budget) => {
            budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                !(budgetAmount.year == latestYear));
        });

        this.currentPage = clone(budgets);
    }

    onSubmitAddBudgetYearRange() {
        this.showSetRangeForAddingBudgetYearsDialog = false;

        if (this.range > 0) {
            const latestYear: number = this.lastYear;
            const startYear: number = hasValue(latestYear) && latestYear !== 0 ? latestYear + 1 : moment().year();
            const endYear = moment().year(startYear).add(this.range, 'years').year();

            const budgets: Budget[] = clone(this.currentPage);

            for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                budgets.forEach((budget: Budget) => {
                    const newBudgetAmount: BudgetAmount = {
                        id: getNewGuid(),
                        budgetName: budget.name,
                        year: currentYear,
                        value: 0,
                    };
                    let amounts = this.addedBudgetAmounts.get(budget.name)
                    if(!isNil(amounts))
                        amounts.push(newBudgetAmount);
                    else
                        this.addedBudgetAmounts.set(budget.name, [newBudgetAmount])
                });
            }
            this.onPaginationChanged();
        }
        
    }

    onSubmitRemoveBudgetYearRange(range: number) {
        this.showSetRangeForDeletingBudgetYearsDialog = false;

        if (range > 0) {
            const endYear: number = this.lastYear;
            const startYear: number = endYear - range + 1;

            const budgets: Budget[] = clone(this.currentPage);

            budgets.forEach((budget: Budget) => {
                budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                    !(budgetAmount.year >= startYear && budgetAmount.year <= endYear));
            });

            this.currentPage = clone(budgets);
        }
    }

    
    onShowEditBudgetsDialog() {

        this.currentPage.sort((b1, b2) => b1.budgetOrder - b2.budgetOrder);
        this.editBudgetsDialogData = {
            showDialog: true,
            budgets: clone(this.BudgetCache),
            scenarioId: this.selectedScenarioId,
        };
    }

    onSubmitEditBudgetsDialogResult(budgetChanges: EmitedBudgetChanges) {
        this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        if(!isNil(budgetChanges)){
            this.addedBudgets = this.addedBudgets.concat(budgetChanges.addedBudgets)
            budgetChanges.addedBudgets.forEach(budget => {
                let amounts: BudgetAmount[] = [];
                if(this.currentPage.length > 0){
                    this.currentPage[0].budgetAmounts.forEach(amount => {
                        amounts.push({
                            id: getNewGuid(),
                            budgetName: budget.name,
                            year: amount.year,
                            value: 0,
                        }) 
                    })
                }
                this.addedBudgetAmounts.set(budget.name, amounts);
            })
            let addedIds = this.addedBudgets.map(b => b.id);            
            budgetChanges.deletionIds.forEach(id => this.removeBudget(id));
            this.deletionBudgetIds = this.deletionBudgetIds.filter(b => !addedIds.includes(b));
            budgetChanges.updatedBudgets.forEach(budget => this.onUpdateBudget(budget.id, budget));
            this.onPaginationChanged();
        }      
    }
    removeBudget(id: string){
        if(any(propEq('id', id), this.addedBudgets)){
            this.addedBudgets = this.addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            this.deletionBudgetIds.push(id);
            const budget = this.currentPage.find(b => b.id === id);
            if(!isNil(budget))
                this.addedBudgetAmounts.delete(budget.name)
        }              
        else if(any(propEq('id', id), Array.from(this.updatedBudgetsMap.values()).map(r => r[1]))){
            this.updatedBudgetsMap.delete(id)
            this.deletionBudgetIds.push(id);
        }          
        else
            this.deletionBudgetIds.push(id);
    }

    OnDownloadTemplateClick()
    {
         InvestmentService.downloadInvestmentBudgetsTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
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
                            this.firstYearOfAnalysisPeriodShift = 0;
                                       
                            this.clearChanges();               
                            this.pagination.page = 1;
                            this.initializePages();
                              
                            this.librarySelectItemValue = null
                    });
                } else {
                    this.importLibraryInvestmentBudgetsFileAction({
                        ...data,
                        id: this.selectedBudgetLibrary.id,
                        currentUserCriteriaFilter: this.currentUserCriteriaFilter
                    })
                    .then(() => {
                            this.getCriterionLibrariesAction();
                            this.librarySelectItemValue = null;
                            this.clearChanges();
                            this.resetPage();
                    });
                }

            }
        }
    }

    onEditBudgetYearValue(year: number, budgetName: string, value: number) {//check this out
        if (any(propEq('name', budgetName), this.currentPage)) {
            const budget: Budget = find(propEq('name', budgetName), this.currentPage) as Budget;

            if (any(propEq('year', year), budget.budgetAmounts)) {
                const budgetAmount: BudgetAmount = find(propEq('year', year), budget.budgetAmounts) as BudgetAmount;
                const updatedRow: BudgetAmount = {
                        ...budgetAmount,
                        value: hasValue(value)
                            ? parseFloat(value.toString().replace(/(\$*)(\,*)/g, ''))
                            : 0,
                    }
                this.onUpdateBudgetAmount(budgetAmount.id, updatedRow)
                this.onPaginationChanged();
            }
        }
    }

    onEditInvestmentPlan(property: string, value: any) {
        this.investmentPlan = setItemPropertyValue(property, value, this.investmentPlan);
    }

    onUpsertInvestment() {
        const investmentPlan: InvestmentPlan = clone(this.investmentPlan);

        const sync: InvestmentPagingSyncModel = {
                libraryId: this.selectedBudgetLibrary.id === this.uuidNIL ? null : this.selectedBudgetLibrary.id,
                updatedBudgets: Array.from(this.updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: this.deletionBudgetIds,
                addedBudgets: this.addedBudgets,
                deletionyears: this.deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( this.updatedBudgetAmounts),
                Investment: {
                ...this.investmentPlan,
                minimumProjectCostLimit: hasValue(this.investmentPlan.minimumProjectCostLimit)
                    ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
                },
                addedBudgetAmounts: mapToIndexSignature(this.addedBudgetAmounts),
                firstYearAnalysisBudgetShift: this.firstYearOfAnalysisPeriodShift 
            }
        InvestmentService.upsertInvestment(sync ,this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.firstYearOfAnalysisPeriodShift = 0;
                this.investmentPlanMutator(this.investmentPlan)                
                this.clearChanges();               
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified investment"});
                this.librarySelectItemValue = null
            }           
        });
    }

    onUpsertBudgetLibrary() {

        const sync: InvestmentPagingSyncModel = {
                libraryId: this.selectedBudgetLibrary.id === this.uuidNIL ? null : this.selectedBudgetLibrary.id,
                updatedBudgets: Array.from(this.updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: this.deletionBudgetIds,
                addedBudgets: this.addedBudgets,
                deletionyears: this.deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( this.updatedBudgetAmounts),
                Investment: null,
                addedBudgetAmounts: mapToIndexSignature(this.addedBudgetAmounts),
                firstYearAnalysisBudgetShift: 0 
            }

         const upsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
                library: this.selectedBudgetLibrary,
                isNewLibrary: false,
                pagingSync: sync,
                scenarioId: null
        }
        InvestmentService.upsertBudgetLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.budgetLibraryMutator(this.selectedBudgetLibrary);
                this.selectedBudgetLibraryMutator(this.selectedBudgetLibrary.id);
                this.addSuccessNotificationAction({message: "Updated budget library",});
            }
        });
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.clearChanges();
                this.resetPage();
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
        const allBudgetDataIsValid: boolean = this.currentPage.every((budget: Budget) => {
            let amountsAreValid = true;
            const addedAmounts = this.addedBudgetAmounts.get(budget.name);
            const updatedAmounts = this.updatedBudgetAmounts.get(budget.name);
            if(!isNil(addedAmounts))
                amountsAreValid = addedAmounts.every((budgetAmount: BudgetAmount) =>
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.year) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
            if(!isNil(updatedAmounts))
                amountsAreValid = amountsAreValid && updatedAmounts.every((budgetAmount: BudgetAmount) =>
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.year) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
            return amountsAreValid
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

    onSearchClick(){
        this.currentSearch = this.gridSearchTerm;
        this.resetPage();
    }

    onUpdateBudget(rowId: string, updatedRow: Budget){
        if(any(propEq('id', rowId), this.addedBudgets))
            return;
        let mapEntry = this.updatedBudgetsMap.get(rowId)
        if(isNil(mapEntry)){
            const row = this.BudgetCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
                this.updatedBudgetsMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            this.updatedBudgetsMap.delete(rowId)

        this.checkHasUnsavedChanges();
    }

    onUpdateBudgetAmount(rowId: string, updatedRow: BudgetAmount){
        if(!isNil(this.addedBudgetAmounts.get(updatedRow.budgetName)))
            if(any(propEq('id', rowId), this.addedBudgetAmounts.get(updatedRow.budgetName)!)){
                let amounts = this.addedBudgetAmounts.get(updatedRow.budgetName)!
                amounts[amounts.findIndex(b => b.id == rowId)] = updatedRow;
            }
                

        let mapEntry = this.updatedBudgetAmountsMaps.get(rowId)

        if(isNil(mapEntry)){
            const row = this.budgetAmountCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row)){
                this.updatedBudgetAmountsMaps.set(rowId, [row , updatedRow])
                if(!isNil(this.updatedBudgetAmounts.get(updatedRow.budgetName)))
                    this.updatedBudgetAmounts.get(updatedRow.budgetName)!.push(updatedRow)
                else
                    this.updatedBudgetAmounts.set(updatedRow.budgetName, [updatedRow])
            }               
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
            let amounts = this.updatedBudgetAmounts.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts[amounts.findIndex(r => r.id == updatedRow.id)] = updatedRow
        }
        else{
            this.updatedBudgetsMap.delete(rowId)
            let amounts = this.updatedBudgetAmounts.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            
        this.checkHasUnsavedChanges();
    }

    clearChanges(){
        this.updatedBudgetsMap.clear();
        this.addedBudgets = [];
        this.deletionBudgetIds = [];
        this.updatedBudgetAmountsMaps.clear();
        this.updatedBudgetAmounts.clear();
        this.addedBudgetAmounts.clear();
        this.deletionYears = [];
        if(this.hasScenario)
            this.investmentPlan = clone(this.stateInvestmentPlan);
    }

    resetPage(){
        this.pagination.page = 1;
        this.onPaginationChanged();
    }

    checkHasUnsavedChanges(){
        const clonedStateInvestmentPlan: InvestmentPlan = clone(this.stateInvestmentPlan);
        const stateInvestmentPlan: InvestmentPlan = {
                ...clonedStateInvestmentPlan,
                inflationRatePercentage: +clonedStateInvestmentPlan.inflationRatePercentage,
                firstYearOfAnalysisPeriod: +clonedStateInvestmentPlan.firstYearOfAnalysisPeriod,
                minimumProjectCostLimit: hasValue(clonedStateInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedStateInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
        const clonedInvestmentPlan: InvestmentPlan = clone(this.investmentPlan);
            const investmentPlan: InvestmentPlan = {
                ...clonedInvestmentPlan,
                inflationRatePercentage: +clonedInvestmentPlan.inflationRatePercentage,
                firstYearOfAnalysisPeriod: +clonedInvestmentPlan.firstYearOfAnalysisPeriod,
                minimumProjectCostLimit: hasValue(clonedInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
        const hasUnsavedChanges: boolean = 
            this.deletionBudgetIds.length > 0 || 
            this.addedBudgets.length > 0 ||
            this.updatedBudgetsMap.size > 0 || 
            this.deletionYears.length > 0 || 
            this.addedBudgetAmounts.size > 0 ||
            this.updatedBudgetAmounts.size > 0 || 
            (this.hasScenario && this.hasSelectedLibrary) ||
            (this.hasScenario && hasUnsavedChangesCore('', investmentPlan, stateInvestmentPlan)) || 
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.selectedBudgetLibrary, this.stateSelectedBudgetLibrary))
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    CheckUnsavedDialog(next: any, otherwise: any) {
        if (this.hasUnsavedChanges && this.unsavedDialogAllowed) {
            // @ts-ignore
            Vue.dialog
                .confirm(
                    'You have unsaved changes. Are you sure you wish to continue?',
                    { reverse: true },
                )
                .then(() => next())
                .catch(() => otherwise())
        } 
        else {
            this.unsavedDialogAllowed = true;
            next();
        }
    };

    initializePages(){
        const request: InvestmentPagingRequestModel= {
            page: 1,
            rowsPerPage: 5,
            pagingSync: {
                libraryId: this.selectedBudgetLibrary.id === this.uuidNIL ? null : this.selectedBudgetLibrary.id,
                updatedBudgets: Array.from(this.updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: this.deletionBudgetIds,
                addedBudgets: this.addedBudgets,
                deletionyears: this.deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( this.updatedBudgetAmounts),
                Investment: null,
                addedBudgetAmounts: mapToIndexSignature(this.addedBudgetAmounts),
                firstYearAnalysisBudgetShift: 0
            },           
            sortColumn: 'year',
            isDescending: false,
            search: ''
        };
        
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            InvestmentService.getScenarioInvestmentPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    this.currentPage = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    this.BudgetCache = clone(this.currentPage);
                    this.BudgetCache.forEach(_ => _.budgetAmounts = []);
                    this.budgetAmountCache = this.currentPage.flatMap(_ => _.budgetAmounts)
                    this.totalItems = data.totalItems;
                    this.investmentPlan = data.investmentPlan;
                    this.investmentPlanMutator(this.investmentPlan)
                    this.syncInvestmentPlanWithBudgets();
                    this.lastYear = data.lastYear;
                    this.firstYear = data.firstYear;
                    this.originalFirstYear = data.firstYear;
                    if(data.firstYear === 0)
                        this.originalFirstYear = moment().year()
                }
                this.initializing = false;
            });
        }            
        else if(this.hasSelectedLibrary)
             InvestmentService.getLibraryInvestmentPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    this.currentPage = data.items;
                    this.BudgetCache = clone(this.currentPage);
                    this.BudgetCache.forEach(_ => _.budgetAmounts = []);
                    this.budgetAmountCache = this.currentPage.flatMap(_ => _.budgetAmounts)
                    this.totalItems = data.totalItems;
                    this.lastYear = data.lastYear;
                }
                this.initializing = false;
            });
        else
            this.initializing = false;
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
