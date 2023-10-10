<template>
    <v-row column>
        <v-flex xs12>
            <v-row row style="margin-top:-40px;">
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Select an Investment library</span></v-subheader>
                    <v-select 
                        id="InvestmentEditor-investmentLibrary-select"
                        :items='librarySelectItems'
                        append-icon=$vuetify.icons.ghd-down
                        variant="outlined"
                        v-model='librarySelectItemValue'
                        class="ghd-select ghd-text-field ghd-text-field-border budget-parent">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader" v-if="hasScenario"><b>Library Used: {{parentLibraryName}} <span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
                </v-flex>

                <!-- these are only in library -->
                <v-flex xs4 v-if='!hasScenario' class="ghd-constant-header">
                    <v-row v-if='hasSelectedLibrary && !hasScenario' row class="header-alignment-padding-center">
                        <div class="header-text-content invest-owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                        </div>
                        <div class="header-text-content invest-owner-padding">
                            Date Modified: {{ modifiedDate }}
                        </div>
                        <v-btn id="InvestmentEditor-ShareLibrary-vbtn" @click='onShowShareBudgetLibraryDialog(selectedBudgetLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                               v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>
                </v-flex>
                <v-flex xs4 v-if='!hasScenario' class="ghd-constant-header">
                    <v-row row align-end justify-end class="header-alignment-padding-right">
                        <v-spacer></v-spacer>
                        <v-btn id="InvestmentEditor-CreateNewLibrary-vbtn" @click='onShowCreateBudgetLibraryDialog(false)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="!hasScenario"
                        variant = "outlined">
                            Create New Library
                        </v-btn>
                    </v-row>
                </v-flex>
            </v-row>
            <!-- only for scenario -->
            <v-row row style="margin-top:-20px;">
                <!-- text boxes for scenario only -->
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>First Year of Analysis Period</span></v-subheader>
                    <v-text-field id="InvestmentEditor-firstYearAnalysisPeriod-textField"
                                  outline
                                  @change='onEditInvestmentPlan("firstYearOfAnalysisPeriod", $event)'
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"
                                  :mask="'####'"
                                  v-model='investmentPlan.firstYearOfAnalysisPeriod' 
                                  class="ghd-text-field-border ghd-text-field"/>
                </v-flex>
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Number of Years in Analysis Period</span></v-subheader>
                    <v-text-field id="InvestmentEditor-numberYearsAnalysisPeriod-textField"
                                  readonly outline
                                  @change='onEditInvestmentPlan("numberOfYearsInAnalysisPeriod", $event)'
                                  v-model='investmentPlan.numberOfYearsInAnalysisPeriod'
                                  class="ghd-text-field-border ghd-text-field" />
                </v-flex>
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Minimum Project Cost Limit</span></v-subheader>
                    <v-text-field outline 
                                  id='InvestmentEditor-minimumProjectCostLimit-textField'
                                  @change='onEditInvestmentPlan("minimumProjectCostLimit", $event)'
                                  v-model='investmentPlan.minimumProjectCostLimit'
                                  v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: true}"
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.minimumProjectCostLimit)]"
                                  :disabled="!hasAdminAccess"
                                  class="ghd-text-field-border ghd-text-field" />
                </v-flex>
                <v-flex xs2 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-subheader"><span>Inflation Rate Percentage</span></v-subheader>
                    <v-text-field id="InvestmentEditor-inflationRatePercentage-textField"
                                  outline
                                  v-model='investmentPlan.inflationRatePercentage'
                                  @change='onEditInvestmentPlan("inflationRatePercentage", $event)'
                                  :mask="'###'"
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0,100])]"
                                  :disabled="!hasAdminAccess"
                                  class="ghd-text-field-border ghd-text-field" />
                </v-flex>
                <v-flex xs4 v-if='hasInvestmentPlanForScenario' class="ghd-constant-header">
                    <v-switch id="InvestmentEditor-allowFundingCarryover-switch"
                              style="margin-left:10px;margin-top:50px;"
                              class="ghd-checkbox"
                              label="Allow Funding Carryover"
                              :disabled="!hasAdminAccess"
                              v-model="investmentPlan.shouldAccumulateUnusedBudgetAmounts"
                              @change='onEditInvestmentPlan("shouldAccumulateUnusedBudgetAmounts", $event)' />
                </v-flex>
            </v-row>
            <v-divider v-if='hasScenario || hasSelectedLibrary' />
            <v-row row justify-space-between v-show='hasSelectedLibrary || hasScenario'>
                <v-flex xs4>
                    <v-row row>
                        <v-btn id="InvestmentEditor-editBudgets-btn"
                            @click='onShowEditBudgetsDialog'
                            variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Edit Budgets
                        </v-btn>
                        <v-text-field id="InvestmentEditor-numberYearsToAdd-textField"
                                      :disabled='currentPage.length === 0' type="number" min=1 :mask="'##########'"
                                      class="ghd-text-field-border ghd-text-field ghd-left-paired-textbox shrink"
                                      v-bind:class="{ 'ghd-blue-text-field': currentPage.length !== 0}"
                                      outline v-model.number="range" />
                        <v-btn id="InvestmentEditor-addBudgetYearRange-btn"
                               :disabled='currentPage.length === 0'
                               @click='onSubmitAddBudgetYearRange'
                               class='ghd-right-paired-button ghd-blue ghd-button-text ghd-outline-button-padding ' variant = "outlined">
                            Add Year(s)
                        </v-btn>
                    </v-row>
                    <v-row row>
                        <div class = "ghd-md-gray ghd-control-subheader" style="margin-left:2% !important;"> 
                            Number of Budgets: {{ currentPage.length }}
                        </div>
                    </v-row>
                </v-flex>
                <v-spacer></v-spacer>
                <v-flex xs4>
                    <v-row row align-end>
                        <v-spacer></v-spacer>
                        <v-btn id="InvestmentEditor-upload-btn"
                            :disabled='false' @click='showImportExportInvestmentBudgetsDialog = true;'
                            variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Upload
                        </v-btn>
                        <v-divider class="investment-divider" inset vertical>
                        </v-divider>
                        <v-btn id="InvestmentEditor-download-btn"
                               :disabled='false' @click='exportInvestmentBudgets()'
                               variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                        </v-divider>
                        <v-btn id="InvestmentEditor-downloadTemplate-btn"
                               :disabled='false' @click='OnDownloadTemplateClick()'
                               variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download Template
                        </v-btn>
                    </v-row>
                </v-flex>
            </v-row>
        </v-flex>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>            
        <!-- datatable -->
            <v-flex >
                <v-data-table 
                    id="InvestmentEditor-investmentsDataTable-dataTable"
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
                    <template slot='items' slot-scope='props' v-slot:item="{item}">
                        <td>
                            <v-checkbox hide-details primary v-model='item.raw.selected'></v-checkbox>
                        </td>
                        <td v-for='header in budgetYearsGridHeaders'>
                            <div v-if="header.value === 'year'">
                                <span class='sm-txt'>{{ item.value.year + firstYearOfAnalysisPeriodShift}}</span>
                            </div>       
                            <div v-if="header.value === 'action'">
                                <v-btn @click="onRemoveBudgetYear(item.value.year)" class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')" />
                                </v-btn>
                            </div>
                            <div v-if="header.value !== 'year' && header.value !== 'action'">
                                <v-edit-dialog :return-value.sync='item.value.values[header.value]'
                                               @save='onEditBudgetYearValue(item.value.year, header.value, item.value.values[header.value])'
                                               size="large" lazy>
                                    <v-text-field readonly single-line class='sm-txt'
                                                  :model-value='formatAsCurrency(item.value.values[header.value])'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <template v-slot:input>
                                        <v-text-field label='Edit' single-line
                                                      v-model.number='item.value.values[header.value]'
                                                      v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                      :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    </template>
                                </v-edit-dialog>
                            </div>

                        </td>
                    </template>
                </v-data-table>
                <v-btn id="InvestmentEditor-deleteSelected-btn"
                       :disabled='selectedBudgetYears.length === 0' @click='onRemoveBudgetYears'
                       class='ghd-blue ghd-button' variant = "flat">
                    Delete Selected
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex v-show='hasSelectedLibrary && !hasScenario' xs12>
            <v-row justify-center>
                <v-flex>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea no-resize outline rows='4'
                                v-model='selectedBudgetLibrary.description'
                                @update:model-value="checkHasUnsavedChanges()"
                                class="ghd-text-field-border">
                    </v-textarea>
                </v-flex>
            </v-row>
        </v-flex>
        <v-flex xs12>
            <v-row justify-center row v-show='hasSelectedLibrary || hasScenario'>
                <v-btn id="InvestmentEditor-cancel-btn"
                       :disabled='!hasUnsavedChanges' @click='onDiscardChanges' variant = "flat" class='ghd-blue ghd-button-text ghd-button'
                       v-show='hasScenario'>
                    Cancel
                </v-btn>
                <v-btn outline id="InvestmentEditor-deleteLibrary-btn"
                       @click='onShowConfirmDeleteAlert' variant = "flat" class='ghd-blue ghd-button-text ghd-button' v-show='!hasScenario'
                       :disabled='!hasLibraryEditPermission'>
                    Delete Library
                </v-btn>
                <v-btn id="InvestmentEditor-createAsNewLibrary-btn"
                       :disabled='disableCrudButton()'
                       @click='onShowCreateBudgetLibraryDialog(true)'
                       class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Create as New Library
                </v-btn>
                <v-btn id="InvestmentEditor-updateLibrary-btn"
                       :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                       @click='onUpsertBudgetLibrary()'
                       class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                       v-show='!hasScenario'>
                    Update Library
                </v-btn>
                <v-btn id="InvestmentEditor-save-btn"
                       :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                       @click='onUpsertInvestment()'
                       class='ghd-blue-bg text-white ghd-button-text ghd-button'
                       v-show='hasScenario'>
                    Save
                </v-btn>
            </v-row>
        </v-flex>

        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onSubmitConfirmDeleteAlertResult' />

        <CreateBudgetLibraryDialog :dialogData='createBudgetLibraryDialogData'
                                   :libraryNames='librarySelectItemNames'
                                   @submit='onSubmitCreateCreateBudgetLibraryDialogResult' />

        <ShareBudgetLibraryDialog :dialogData="shareBudgetLibraryDialogData"
                                  @submit="onShareBudgetLibraryDialogSubmit" />

        <SetRangeForAddingBudgetYearsDialog :showDialog='showSetRangeForAddingBudgetYearsDialog'
                                            :startYear='getNextYear()'
                                            @submit='onSubmitAddBudgetYearRange' />

        <SetRangeForDeletingBudgetYearsDialog :showDialog='showSetRangeForDeletingBudgetYearsDialog'
                                              :endYear='lastYear'
                                              :maxRange='yearsInAnalysisPeriod()'
                                              @submit='onSubmitRemoveBudgetYearRange' />

        <EditBudgetsDialog :dialogData='editBudgetsDialogData' @submit='onSubmitEditBudgetsDialogResult' />

        <ImportExportInvestmentBudgetsDialog :showDialog='showImportExportInvestmentBudgetsDialog'
                                             @submit='onSubmitImportExportInvestmentBudgetsDialogResult' />
    </v-row>
</template>

<script lang='ts' setup>
import Vue, { shallowRef } from 'vue';
import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
import SetRangeForDeletingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForDeletingBudgetYearsDialog.vue';
import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
import {
    Budget,
    BudgetAmount,
    BudgetLibrary,
    BudgetLibraryUser,
    BudgetYearsGridData,
    emptyBudgetLibrary,
    emptyInvestmentPlan, InvestmentBudgetFileImport,
    InvestmentPlan
} from '@/shared/models/iAM/investment';
import { any, clone, find, isNil, propEq } from 'ramda';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import moment from 'moment';
import {
    CreateBudgetLibraryDialogData,
    emptyCreateBudgetLibraryDialogData,
} from '@/shared/models/modals/create-budget-library-dialog-data';

import {
    ShareBudgetLibraryDialogData,
    emptyShareBudgetLibraryDialogData,
} from '@/shared/models/modals/share-budget-library-dialog-data';

import { EditBudgetsDialogData, EmitedBudgetChanges, emptyEditBudgetsDialogData } from '@/shared/models/modals/edit-budgets-dialog';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import CreateBudgetLibraryDialog
    from '@/components/investment-editor/investment-editor-dialogs/CreateBudgetLibraryDialog.vue';
import ShareBudgetLibraryDialog from '@/components/investment-editor/investment-editor-dialogs/ShareBudgetLibraryDialog.vue';
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
import { LibraryUser } from '@/shared/models/iAM/user';
import {
    mapToIndexSignature
} from '../../shared/utils/conversion-utils';
import { isNullOrUndefined } from 'util';
import { Hub } from '@/connectionHub';
import ScenarioService from '@/services/scenario.service';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import mitt from 'mitt';

let store = useStore();
const emit = defineEmits(['submit'])
const $vuetify = inject('$vuetify') as any
    const $router = useRouter();
    const $statusHub = inject('$statusHub') as any
    const $config = inject('$config') as any
    const $emitter = mitt()
let stateBudgetLibraries = ref<BudgetLibrary[]>(store.state.investmentModule.budgetLibraries);
let stateSelectedBudgetLibrary = ref<BudgetLibrary>(store.state.investmentModule.selectedBudgetLibrary)
let stateInvestmentPlan = ref<InvestmentPlan>(store.state.investmentModule.investmentPlan);
let stateScenarioBudgets = ref<Budget[]>(store.state.investmentModule.scenarioBudgets)
let hasUnsavedChanges = ref<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess)
let isSuccessfulImport = ref<boolean>(store.state.investmentModule.isSuccessfulImport);
let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter)
let hasPermittedAccess = ref<boolean>(store.state.investmentModule.hasPermittedAccess);

async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess');}
async function getInvestmentAction(payload?: any): Promise<any> {await store.dispatch('getInvestment');}
async function getBudgetLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getBudgetLibraries');}
async function selectBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectBudgetLibrary');}
async function upsertInvestmentAction(payload?: any): Promise<any> {await store.dispatch('upsertInvestment');}
async function upsertBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertBudgetLibrary');}
async function deleteBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteBudgetLibrary');}
async function upsertOrDeleteBudgetLibraryUsersAction(payload: any): Promise<any> {await store.dispatch('upsertOrDeleteBudgetLibraryUsers');}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
async function importScenarioInvestmentBudgetsFileAction(payload?: any): Promise<any> {await store.dispatch('importScenarioInvestmentBudgetsFile');}
async function importLibraryInvestmentBudgetsFileAction(payload?: any): Promise<any> {await store.dispatch('importLibraryInvestmentBudgetsFile');}
async function getCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCriterionLibraries');}
async function setAlertMessageAction(payload?: any): Promise<any> {await store.dispatch('setAlertMessage');}
async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification');}
    
let getModifiedDate = store.getters.getLibraryDateModified;
let getUserNameByIdGetter = store.getters.getUserNameById;

function budgetLibraryMutator(payload:any){store.commit('budgetLibraryMutator');}
function selectedBudgetLibraryMutator(payload:any){store.commit('selectedBudgetLibraryMutator');}
function investmentPlanMutator(payload:any){store.commit('investmentPlanMutator');}
function isSuccessfulImportMutator(payload:any){store.commit('isSuccessfulImportMutator');}
    let modifiedDate: string;

    let addedBudgets: Budget[] = [];
    let updatedBudgetsMap:Map<string, [Budget, Budget]> = new Map<string, [Budget, Budget]>();//0: original value | 1: updated value
    let deletionBudgetIds: string[] = [];
    let BudgetCache: Budget[] = [];
    let budgetAmountCache: BudgetAmount[] = [];
    let updatedBudgetAmountsMaps:Map<string, [BudgetAmount, BudgetAmount]> = new Map<string, [BudgetAmount, BudgetAmount]>();//0: original value | 1: updated value 
    let addedBudgetAmounts: Map<string, BudgetAmount[]> = new  Map<string, BudgetAmount[]>();
    let deletionYears: number[] = [] 
    let updatedBudgetAmounts:  Map<string, BudgetAmount[]> = new  Map<string, BudgetAmount[]>();
    let gridSearchTerm = '';
    let currentSearch = '';
    let pagination: Pagination = clone(emptyPagination);
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: Budget[] = [];
    let lastYear: number = 0;
    let firstYear: number = 0;
    let initializing: boolean = true;

    let selectedBudgetLibrary: BudgetLibrary = clone(emptyBudgetLibrary);
    let investmentPlan: InvestmentPlan = clone(emptyInvestmentPlan);
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let librarySelectItemNames: string[] = [];
    let librarySelectItemValue = shallowRef<string|null>('');
    let actionHeader: DataTableHeader = { text: 'Action', value: 'action', align: 'left', sortable: false, class: '', width: '' }
    let budgetYearsGridHeaders: DataTableHeader[] = [
        { text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: '' },
        actionHeader
    ];
    let budgetYearsGridData: BudgetYearsGridData[] = [];
    let selectedBudgetYearsGridData: BudgetYearsGridData[] = [];
    let selectedBudgetYears: number[] = [];

    let createBudgetLibraryDialogData: CreateBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);
    let shareBudgetLibraryDialogData: ShareBudgetLibraryDialogData = clone(emptyShareBudgetLibraryDialogData);
    let editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
    let showSetRangeForAddingBudgetYearsDialog: boolean = false;
    let showSetRangeForDeletingBudgetYearsDialog: boolean = false;
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let uuidNIL: string = getBlankGuid();
    let rules: InputValidationRules = validationRules;
    let showImportExportInvestmentBudgetsDialog: boolean = false;
    let hasScenario: boolean = false;
    let hasInvestmentPlanForScenario: boolean = false;
    let hasCreatedLibrary: boolean = false;
    let budgets: Budget[] = [];
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let showReminder: boolean = false;
    let range: number = 1;
    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;

    function addYearLabel() {
        return 'Add Year (' + getNextYear() + ')';
    }
    let originalFirstYear: number = 0
    let firstYearOfAnalysisPeriodShift = shallowRef<number>(0);


    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue = shallowRef<string|null>('');
    let librarySelectItemValueAllowedChanged: boolean = true;

    function deleteYearLabel() {
            const latestYear = lastYear;
            return latestYear ? 'Delete Year (' + latestYear + ')' : 'Delete Year';
        }

    function yearsInAnalysisPeriod() {
            return investmentPlan.numberOfYearsInAnalysisPeriod;
        }
        beforeRouteEnter()
        function beforeRouteEnter() {
            (() => {
                (async () => { 
                    librarySelectItemValue.value = '';
                    await getHasPermittedAccessAction();
                    await getBudgetLibrariesAction()
                    if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.Investment) !== -1) {
                        selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

                        if (selectedScenarioId === uuidNIL) {
                            addErrorNotificationAction({
                                message: 'Found no selected scenario for edit',
                            });
                            $router.push('/Scenarios/');
                        }

                        hasScenario = true;
                        ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioInvestment}).then(response => {
                            if(response.data){
                                setAlertMessageAction("An investment import has been added to the work queue")
                            }
                        })
                        await initializePages();
                    }
                    else
                        initializing = false;               
                })();                    
            });
        }
    onMounted(()=>mounted)
    function mounted() {
        $emitter.on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }  
    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
        $emitter.off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }
        

    // Watchers
    watch(pagination,()=>onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = pagination;
        const request: InvestmentPagingRequestModel= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: selectedBudgetLibrary.id === uuidNIL ? null : selectedBudgetLibrary.id,
                updatedBudgets: Array.from(updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: deletionBudgetIds,
                addedBudgets: addedBudgets,
                deletionyears: deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( updatedBudgetAmounts),
                Investment: !isNil(investmentPlan) ? {
                ...investmentPlan,
                minimumProjectCostLimit: hasValue(investmentPlan.minimumProjectCostLimit)
                    ? parseFloat(investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
                } : investmentPlan,
                addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts),
                firstYearAnalysisBudgetShift: firstYearOfAnalysisPeriodShift.value,
                isModified: scenarioLibraryIsModified
            },           
            sortColumn: sortBy === '' ? 'year' : sortBy,
            isDescending: descending != null ? descending : false,
            search: currentSearch
        };
        
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL){
            await InvestmentService.getScenarioInvestmentPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    firstYear = data.firstYear;
                    currentPage = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    BudgetCache = clone(currentPage);
                    BudgetCache.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache = currentPage.flatMap(_ => _.budgetAmounts)
                    totalItems = data.totalItems;
                    investmentPlan = data.investmentPlan;                   
                    lastYear = data.lastYear;
                    
                    syncInvestmentPlanWithBudgets();
                    
                }
            });
        }            
        else if(hasSelectedLibrary){
            if(librarySelectItemValue === null)
                return;
                await InvestmentService.getBudgetLibraryModifiedDate(selectedBudgetLibrary.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate = data.slice(0, 10);
                   }
             });

            await InvestmentService.getLibraryInvestmentPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage = data.items;
                    BudgetCache = clone(currentPage);
                    BudgetCache.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache = currentPage.flatMap(_ => _.budgetAmounts)
                    totalItems = data.totalItems;
                    lastYear = data.lastYear
                }
            }); 
        }                
    }
   

    watch(deletionBudgetIds,()=> onDeletionBudgetIdsChanged)
    function onDeletionBudgetIdsChanged() {
        checkHasUnsavedChanges();
    }

    watch(addedBudgets,()=>onAddedBudgetsChanged)
    function onAddedBudgetsChanged() {
        checkHasUnsavedChanges();
    }

    watch(deletionYears,()=> onDeletionyearsChanged)
    function onDeletionyearsChanged() {
        checkHasUnsavedChanges();
    }

    watch(addedBudgetAmounts,()=>onAddedBudgetAmountsChanged)
    function onAddedBudgetAmountsChanged() {
        checkHasUnsavedChanges();
    }

    watch(stateBudgetLibraries,()=> onStateBudgetLibrariesChanged)
    function onStateBudgetLibrariesChanged() {
        librarySelectItems = stateBudgetLibraries.value
            .map((library: BudgetLibrary) => ({
                text: library.name,
                value: library.id,
            }));
        librarySelectItemNames = librarySelectItems.map((library: SelectItem) => library.text)
    }

    watch(selectedBudgetYearsGridData,()=> onSelectedRowsChanged)
    function onSelectedRowsChanged() {
        selectedBudgetYears = getPropertyValues('year', selectedBudgetYearsGridData) as number[];
    }

    watch(librarySelectItemValue,()=> onLibrarySelectItemValueChangedCheckUnsaved)
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario){
            onSelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged){
            CheckUnsavedDialog(onSelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = trueLibrarySelectItemValue.value;               
            });
        }
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        scenarioLibraryIsModified = false;
        librarySelectItemValueAllowedChanged = true;
    }
    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue
        selectBudgetLibraryAction(librarySelectItemValue);
    }

    watch(stateSelectedBudgetLibrary,()=>onStateSelectedBudgetLibraryChanged)
    function onStateSelectedBudgetLibraryChanged() {
        selectedBudgetLibrary = clone(stateSelectedBudgetLibrary.value);
    }

    watch(selectedBudgetLibrary,()=>onSelectedBudgetLibraryChanged)
    function onSelectedBudgetLibraryChanged() {
        hasSelectedLibrary = selectedBudgetLibrary.id !== uuidNIL;

        if (hasSelectedLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedBudgetLibrary.id, workType: WorkType.ImportLibraryInvestment}).then(response => {
                if(response.data){
                    setAlertMessageAction("An investment import has been added to the work queue")
                }
                else
                    setAlertMessageAction("");
            })
        }

        clearChanges()
        onPaginationChanged()
    }

    watch(stateInvestmentPlan,()=>onStateInvestmentPlanChanged)
    function onStateInvestmentPlanChanged() {
        cloneStateInvestmentPlan();
        hasInvestmentPlanForScenario = true;
        checkHasUnsavedChanges();
    }

    watch(currentPage,()=> onScenarioBudgetsChanged)
    function onScenarioBudgetsChanged() {
        setGridHeaders();
        setGridData();
        
        checkHasUnsavedChanges();

        // Get parent name from library id
        librarySelectItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }

    watch(investmentPlan,()=> onInvestmentPlanChanged)
    function onInvestmentPlanChanged() {
        checkHasUnsavedChanges()
        if(hasScenario){
            const firstYear = +investmentPlan.firstYearOfAnalysisPeriod;
            const stateFirstYear = +stateInvestmentPlan.value.firstYearOfAnalysisPeriod;
            firstYearOfAnalysisPeriodShift.value = (firstYear - originalFirstYear) - (firstYear === 0 ? 0 : (firstYear - originalFirstYear));
        }
            
        if(investmentPlan.id === uuidNIL)
            investmentPlan.id = getNewGuid();
        hasInvestmentPlanForScenario = true;
    }

    watch(firstYearOfAnalysisPeriodShift,()=>onFirstYearOfAnalysisPeriodShiftChanged)
    function onFirstYearOfAnalysisPeriodShiftChanged(){
        setGridData();
    }

    function onRemoveBudgetYears() {
        deletionYears = deletionYears.concat(selectedBudgetYears)
        let deletedAddYears: number[] = [];
        for(let [key, value] of addedBudgetAmounts){
            let val = addedBudgetAmounts.get(key)! 
            val = val.filter(v => {
                if(!deletionYears.includes(v.year)){                  
                    return true;
                }

                deletedAddYears.push(v.year);  
                return false;
            })
            if (val.length == 0)
                addedBudgetAmounts.delete(key)
        }
        if (deletedAddYears.length > 0) {
            selectedBudgetYears = selectedBudgetYears.filter(year => !deletedAddYears.includes(year))
            deletionYears = deletionYears.concat(selectedBudgetYears)
            onPaginationChanged();
            return;
        }

        let deletedUpdateIds: string[] = [];

        for (let [key, value] of updatedBudgetAmounts) {
            let val = updatedBudgetAmounts.get(key)!
            const ids = val.filter(v => deletionYears.includes(v.year)).map(v => v.id)
            deletedUpdateIds = deletedUpdateIds.concat(ids)
            val = val.filter(v => !deletionYears.includes(v.year))
            if (val.length == 0)
                updatedBudgetAmounts.delete(key)
        }

        deletedUpdateIds.forEach(id => {
            updatedBudgetAmountsMaps.delete(id);
        })

        onPaginationChanged();
    }

    function onRemoveBudgetYear(year: number) {
        let isyearAdded = false;
        for (let [key, value] of addedBudgetAmounts) {
            let val = addedBudgetAmounts.get(key)!
            val = val.filter(v => {
                if (v.year !== year) {
                    return true;
                }

                isyearAdded = true;

                return false;
            })
            if(val.length == 0)
                addedBudgetAmounts.delete(key)
            else
            {
                addedBudgetAmounts.set(key, val)
            }
        }
        if(isyearAdded){
            onPaginationChanged();
            return;
        }
        deletionYears.push(year)

        let deleteIds: string[] = []
        for (let [key, value] of updatedBudgetAmounts) {
            let val = updatedBudgetAmounts.get(key)!
            const ids = val.filter(v => v.year === year).map(v => v.id)
            deleteIds = deleteIds.concat(ids)
            val = val.filter(v => v.year !== year)
            if (val.length == 0)
                updatedBudgetAmounts.delete(key)
        }

        deleteIds.forEach(id => {
            updatedBudgetAmountsMaps.delete(id);
        })

        onPaginationChanged();
    }

    function cloneStateInvestmentPlan() {
        let investmentPlan: InvestmentPlan = clone(stateInvestmentPlan.value);
        investmentPlan = {
            ...investmentPlan,
            id: investmentPlan.id === uuidNIL ? getNewGuid() : investmentPlan.id,
        };
    }

    function setHasUnsavedChangesFlag() {
        if (hasScenario) {
            const budgetsHaveUnsavedChanges: boolean = hasUnsavedChangesCore('', currentPage, stateScenarioBudgets);


            const clonedInvestmentPlan: InvestmentPlan = clone(investmentPlan);
            const NewInvestmentPlan: InvestmentPlan = {
                ...clonedInvestmentPlan,
                minimumProjectCostLimit: hasValue(clonedInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
            const clonedStateInvestmentPlan: InvestmentPlan = clone(stateInvestmentPlan.value);
            const NewStateInvestmentPlan: InvestmentPlan = {
                ...clonedStateInvestmentPlan,
                minimumProjectCostLimit: hasValue(clonedStateInvestmentPlan.minimumProjectCostLimit)
                    ? parseFloat(clonedStateInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            };
            const investmentPlanHasUnsavedChanges: boolean = hasUnsavedChangesCore('', NewInvestmentPlan, NewStateInvestmentPlan);


            setHasUnsavedChangesAction({ value: budgetsHaveUnsavedChanges || investmentPlanHasUnsavedChanges });
        } else if (hasSelectedLibrary) {
            const hasUnsavedChanges: boolean = hasUnsavedChangesCore('',
                { ...clone(selectedBudgetLibrary), budgets: clone(currentPage) },
                stateSelectedBudgetLibrary);
            setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }

    function setGridHeaders() {
        const budgetNames: string[] = getPropertyValues('name', currentPage) as string[];
        const budgetHeaders: DataTableHeader[] = budgetNames
            .map((name: string) => ({
                text: name,
                value: name,
                sortable: true,
                align: 'left',
                class: '',
                width: '',
            }) as DataTableHeader);
        budgetYearsGridHeaders = [
            budgetYearsGridHeaders[0],
            ...budgetHeaders,
            actionHeader
        ];
    }

    function setGridData() {
        budgetYearsGridData = [];
        if(currentPage.length <= 0)
            return;
        for(let i = 0; i < currentPage[0].budgetAmounts.length; i++){
            let year = currentPage[0].budgetAmounts[i].year
            let values: {[budgetName: string]: number | null} = {}
            for(let o = 0; o < currentPage.length; o++){
                values[currentPage[o].name] = currentPage[o].budgetAmounts[i].value
            }
            budgetYearsGridData.push({year, values})
        }        
    }

    function syncInvestmentPlanWithBudgets() {//gets call in on pagination now       
        investmentPlan.numberOfYearsInAnalysisPeriod = totalItems > 0 ? totalItems : 1
    }

    function onShareBudgetLibraryDialogSubmit(budgetLibraryUsers: BudgetLibraryUser[]) {
        shareBudgetLibraryDialogData = clone(emptyShareBudgetLibraryDialogData);

        if (!isNil(budgetLibraryUsers) && selectedBudgetLibrary.id !== getBlankGuid())
        {
            let libraryUserData: LibraryUser[] = [];

            //create library users
            budgetLibraryUsers.forEach((budgetLibraryUser, index) =>
            {   
                //determine access level
                let libraryUserAccessLevel: number = 0;
                if (libraryUserAccessLevel == 0 && budgetLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                if (libraryUserAccessLevel == 0 && budgetLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                //create library user object
                let libraryUser: LibraryUser = {
                    userId: budgetLibraryUser.userId,
                    userName: budgetLibraryUser.username,
                    accessLevel: libraryUserAccessLevel
                }

                //add library user to an array
                libraryUserData.push(libraryUser);
            });

            upsertOrDeleteBudgetLibraryUsersAction((selectedBudgetLibrary.id, libraryUserData));

            //update budget library sharing
            InvestmentService.upsertOrDeleteBudgetLibraryUsers(selectedBudgetLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                {
                    addSuccessNotificationAction({ message: 'Shared budget library' })
                    resetPage();
                }
            })
        }
    }

    function onShowCreateBudgetLibraryDialog(createAsNewLibrary: boolean) {
        createBudgetLibraryDialogData = {
            showDialog: true,
            budgets: createAsNewLibrary ? currentPage : [],
        };
    }

    function onSubmitCreateCreateBudgetLibraryDialogResult(budgetLibrary: BudgetLibrary) {//needs a few things
        createBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);

    if (!isNil(budgetLibrary)) {
        hasCreatedLibrary = true;
        librarySelectItemValue.value = budgetLibrary.id;
        const libraryUpsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
            library: budgetLibrary,
            isNewLibrary: true,
            syncModel: {
                libraryId: budgetLibrary.budgets.length === 0 || !hasSelectedLibrary ? null : selectedBudgetLibrary.id,
                Investment: investmentPlan,
                budgetsForDeletion: budgetLibrary.budgets.length === 0 ? [] : deletionBudgetIds,
                updatedBudgets: budgetLibrary.budgets.length === 0 ? [] : Array.from(updatedBudgetsMap.values()).map(r => r[1]),
                addedBudgets: budgetLibrary.budgets.length === 0 ? [] : addedBudgets,
                deletionyears: budgetLibrary.budgets.length === 0 ? [] : deletionYears,
                updatedBudgetAmounts: budgetLibrary.budgets.length === 0 ? {} : mapToIndexSignature(updatedBudgetAmounts),
                addedBudgetAmounts: budgetLibrary.budgets.length === 0 ? {} : mapToIndexSignature(addedBudgetAmounts),
                firstYearAnalysisBudgetShift: 0,
                isModified: false
            },
            scenarioId: hasScenario ? selectedScenarioId : null
        }
        // value in v-currency is not parsed back to a number throwing an silent exception between UI and backend.
        const parsedMinimumProjectCostLimit: number = parseFloat(investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''));
        let tempInvesmentPlan: InvestmentPlan | null = libraryUpsertRequest.syncModel.Investment;
        tempInvesmentPlan? tempInvesmentPlan.minimumProjectCostLimit = parsedMinimumProjectCostLimit : 0;
        libraryUpsertRequest.syncModel.Investment = tempInvesmentPlan;
        
        InvestmentService.upsertBudgetLibrary(libraryUpsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') &&http2XX.test(response.status.toString())){
                if(budgetLibrary.budgets.length === 0){
                    clearChanges();
                }

                    budgetLibraryMutator(budgetLibrary); // mutation actions
                    selectedBudgetLibraryMutator(budgetLibrary.id);
                    addSuccessNotificationAction({ message: 'Added budget library' })
                    resetPage();
                }
            })
        }
    }

    function getNextYear(): number {
        const latestYear: number = lastYear;
        const nextYear = hasValue(latestYear) && latestYear !== 0 ? latestYear + 1 : moment().year();
        return nextYear;
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
            return getUserNameByIdGetter(selectedBudgetLibrary.owner);
        }

        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedBudgetLibrary.owner) == getUserName();
    }

    function onAddBudgetYear() {
        const nextYear: number = getNextYear();

        const budgets: Budget[] = clone(currentPage);

        budgets.forEach((budget: Budget) => {
            const newBudgetAmount: BudgetAmount = {
                id: getNewGuid(),
                budgetName: budget.name,
                year: nextYear,
                value: 0,
            };
            let amounts = addedBudgetAmounts.get(budget.id)
            if (!isNil(amounts))
                amounts.push(newBudgetAmount);
            else
                addedBudgetAmounts.set(budget.id, [newBudgetAmount])
        });

        onPaginationChanged();
    }

    function onRemoveLatestBudgetYear() {
        const latestYear: number = lastYear;

        const budgets: Budget[] = clone(currentPage);

        budgets.forEach((budget: Budget) => {
            budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                !(budgetAmount.year == latestYear));
        });

        currentPage = clone(budgets);
    }

    function onShowShareBudgetLibraryDialog(budgetLibrary: BudgetLibrary) { 
            shareBudgetLibraryDialogData = 
            { 
                showDialog: true, 
                budgetLibrary: clone(budgetLibrary), 
            }; 
        } 

    function onSubmitAddBudgetYearRange() {
        showSetRangeForAddingBudgetYearsDialog = false;

    if (range > 0) {
        const latestYear: number = lastYear;
        const startYear: number = hasValue(latestYear) && latestYear !== 0 ? latestYear + 1 : moment().year();
        const endYear = moment().year(startYear).add(range, 'years').year();

            const budgets: Budget[] = clone(currentPage);

            for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                budgets.forEach((budget: Budget) => {
                    const newBudgetAmount: BudgetAmount = {
                        id: getNewGuid(),
                        budgetName: budget.name,
                        year: currentYear,
                        value: 0,
                    };
                    let amounts = addedBudgetAmounts.get(budget.name)
                    if (!isNil(amounts))
                        amounts.push(newBudgetAmount);
                    else
                        addedBudgetAmounts.set(budget.name, [newBudgetAmount])
                });
            }
            onPaginationChanged();
        }

    }

    function onSubmitRemoveBudgetYearRange(range: number) {
        showSetRangeForDeletingBudgetYearsDialog = false;

        if (range > 0) {
            const endYear: number = lastYear;
            const startYear: number = endYear - range + 1;

            const budgets: Budget[] = clone(currentPage);

            budgets.forEach((budget: Budget) => {
                budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                    !(budgetAmount.year >= startYear && budgetAmount.year <= endYear));
            });

            currentPage = clone(budgets);
        }
    }


    function onShowEditBudgetsDialog() {

        currentPage.sort((b1, b2) => b1.budgetOrder - b2.budgetOrder);
        editBudgetsDialogData = {
            showDialog: true,
            budgets: clone(BudgetCache),
            scenarioId: selectedScenarioId,
        };
    }

    function onSubmitEditBudgetsDialogResult(budgetChanges: EmitedBudgetChanges) {        
        editBudgetsDialogData = clone(emptyEditBudgetsDialogData);
        if(!isNil(budgetChanges)){
            addedBudgets = addedBudgets.concat(budgetChanges.addedBudgets);
            budgetChanges.addedBudgets.forEach(budget => {
                let amounts: BudgetAmount[] = [];
                if(currentPage.length > 0){
                    currentPage[0].budgetAmounts.forEach(amount => {
                        amounts.push({
                            id: getNewGuid(),
                            budgetName: budget.name,
                            year: amount.year,
                            value: 0,
                        }) 
                    })
                }
                addedBudgetAmounts.set(budget.name, amounts);
            });          
            let addedIds = addedBudgets.map(b => b.id);            
            budgetChanges.deletionIds.forEach(id => removeBudget(id));
            deletionBudgetIds = deletionBudgetIds.filter(b => !addedIds.includes(b));
            budgetChanges.updatedBudgets.forEach(budget => onUpdateBudget(budget.id, budget));                       
            
            onPaginationChanged();
        }      
    }
    function removeBudget(id: string){
        if(any(propEq('id', id), addedBudgets)){
            addedBudgets = addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            deletionBudgetIds.push(id);
            const budget = currentPage.find(b => b.id === id);
            if(!isNil(budget))
                addedBudgetAmounts.delete(budget.name)
        }              
        else if(any(propEq('id', id), Array.from(updatedBudgetsMap.values()).map(r => r[1]))){
            updatedBudgetsMap.delete(id)
            deletionBudgetIds.push(id);
        }          
        else
            deletionBudgetIds.push(id);
    }

    function OnDownloadTemplateClick() {
        InvestmentService.downloadInvestmentBudgetsTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    function exportInvestmentBudgets() {
        const id: string = hasScenario ? selectedScenarioId : selectedBudgetLibrary.id;
        InvestmentService.exportInvestmentBudgets(id, hasScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }
    function onSubmitImportExportInvestmentBudgetsDialogResult(result: ImportExportInvestmentBudgetsDialogResult) {
        showImportExportInvestmentBudgetsDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {


            } else if (hasValue(result.file)) {
                const data: InvestmentBudgetFileImport = {
                    file: result.file,
                    overwriteBudgets: result.overwriteBudgets,
                };

            if (hasScenario) {
                importScenarioInvestmentBudgetsFileAction({
                    ...data,
                    id: selectedScenarioId,
                    currentUserCriteriaFilter: currentUserCriteriaFilter
                })
                .then((response: any) => {
                        setAlertMessageAction("Investment Budgets import has been added to the work queue.");
                });
            } else {
                importLibraryInvestmentBudgetsFileAction({
                    ...data,
                    id: selectedBudgetLibrary.id,
                    currentUserCriteriaFilter: currentUserCriteriaFilter
                })
                .then(() => {
                        setAlertMessageAction("Investment Budgets import has been added to the work queue.");                     
                });
            }

            }
        }
    }

    function onEditBudgetYearValue(year: number, budgetName: string, value: number) {//check out
        if (any(propEq('name', budgetName), currentPage)) {
            const budget: Budget = find(propEq('name', budgetName), currentPage) as Budget;

            if (any(propEq('year', year), budget.budgetAmounts)) {
                const budgetAmount: BudgetAmount = find(propEq('year', year), budget.budgetAmounts) as BudgetAmount;
                const updatedRow: BudgetAmount = {
                    ...budgetAmount,
                    value: hasValue(value)
                        ? parseFloat(value.toString().replace(/(\$*)(\,*)/g, ''))
                        : 0,
                }
                onUpdateBudgetAmount(budgetAmount.id, updatedRow)
                onPaginationChanged();
            }
        }
    }

    function onEditInvestmentPlan(property: string, value: any) {
        investmentPlan = setItemPropertyValue(property, value, investmentPlan);
    }

    function onUpsertInvestment() {
        const investmentPlanUpsert: InvestmentPlan = clone(investmentPlan);

        if (selectedBudgetLibrary.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        const sync: InvestmentPagingSyncModel = {
            libraryId: selectedBudgetLibrary.id === uuidNIL ? null : selectedBudgetLibrary.id,
            updatedBudgets: Array.from(updatedBudgetsMap.values()).map(r => r[1]),
            budgetsForDeletion: deletionBudgetIds,
            addedBudgets: addedBudgets,
            deletionyears: deletionYears,
            updatedBudgetAmounts: mapToIndexSignature(updatedBudgetAmounts),
            Investment: {
                ...investmentPlan,
                minimumProjectCostLimit: hasValue(investmentPlan.minimumProjectCostLimit)
                    ? parseFloat(investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            },
            addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts),
            firstYearAnalysisBudgetShift: firstYearOfAnalysisPeriodShift.value,
            isModified: scenarioLibraryIsModified,
        }
        InvestmentService.upsertInvestment(sync ,selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value: "";
                firstYearOfAnalysisPeriodShift.value = 0;
                investmentPlanMutator(investmentPlanUpsert)
                clearChanges();                                            
                addSuccessNotificationAction({message: "Modified investment"});
                librarySelectItemValue.value = null;
            }           
        });
    }

    function onUpsertBudgetLibrary() {
        const sync: InvestmentPagingSyncModel = {
            libraryId: selectedBudgetLibrary.id === uuidNIL ? null : selectedBudgetLibrary.id,
            updatedBudgets: Array.from(updatedBudgetsMap.values()).map(r => r[1]),
            budgetsForDeletion: deletionBudgetIds,
            addedBudgets: addedBudgets,
            deletionyears: deletionYears ,
            updatedBudgetAmounts: mapToIndexSignature( updatedBudgetAmounts),
            Investment: null,
            addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts),
            firstYearAnalysisBudgetShift: 0,
            isModified: false
        }

        const upsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
            library: selectedBudgetLibrary,
            isNewLibrary: false,
            syncModel: sync,
            scenarioId: null
        }
        InvestmentService.upsertBudgetLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()
                budgetLibraryMutator(selectedBudgetLibrary);
                selectedBudgetLibraryMutator(selectedBudgetLibrary.id);
                addSuccessNotificationAction({message: "Updated budget library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario) {
                clearChanges();
                resetPage();
            }
        });
        parentLibraryName = loadedParentName;
        parentLibraryId = loadedParentId;
    }

    function formatAsCurrency(value: any): any {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
    }

    function onShowConfirmDeleteAlert() {
        confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function onSubmitConfirmDeleteAlertResult(submit: boolean) {
        confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            librarySelectItemValue.value = null;
            deleteBudgetLibraryAction(selectedBudgetLibrary.id);
        }
    }

    function disableCrudButton() {
        const allBudgetDataIsValid: boolean = currentPage.every((budget: Budget) => {
            let amountsAreValid = true;
            const addedAmounts = addedBudgetAmounts.get(budget.name);
            const updatedAmounts = updatedBudgetAmounts.get(budget.name);
            if (!isNil(addedAmounts))
                amountsAreValid = addedAmounts.every((budgetAmount: BudgetAmount) =>
                    rules['generalRules'].valueIsNotEmpty(budgetAmount.year) === true &&
                    rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
            if (!isNil(updatedAmounts))
                amountsAreValid = amountsAreValid && updatedAmounts.every((budgetAmount: BudgetAmount) =>
                    rules['generalRules'].valueIsNotEmpty(budgetAmount.year) === true &&
                    rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
            return amountsAreValid
        });

        if (hasSelectedLibrary) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedBudgetLibrary.name) === true &&
                allBudgetDataIsValid);
        } else if (hasScenario) {
            const allInvestmentPlanDataIsValid: boolean = rules['generalRules'].valueIsNotEmpty(investmentPlan.minimumProjectCostLimit) === true &&
                rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.minimumProjectCostLimit) === true &&
                rules['generalRules'].valueIsNotEmpty(investmentPlan.inflationRatePercentage) === true &&
                rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0, 100]);

            return !(allBudgetDataIsValid && allInvestmentPlanDataIsValid);
        }

        disableCrudButtonsResult = !allBudgetDataIsValid;
        return !allBudgetDataIsValid;
    }

    function onSearchClick() {
        currentSearch = gridSearchTerm;
        resetPage();
    }

    function onUpdateBudget(rowId: string, updatedRow: Budget){        
    if(any(propEq('id', rowId), addedBudgets))
    {            
        addedBudgets[addedBudgets.findIndex((b => b.id == rowId))] = updatedRow;
        return;
    }
    let mapEntry = updatedBudgetsMap.get(rowId)
    if(isNil(mapEntry)){
        const row = BudgetCache.find(r => r.id === rowId);
        if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
            updatedBudgetsMap.set(rowId, [row , updatedRow])
    }
    else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
        mapEntry[1] = updatedRow;
    }
    else
        updatedBudgetsMap.delete(rowId)

        checkHasUnsavedChanges();
    }

    function onUpdateBudgetAmount(rowId: string, updatedRow: BudgetAmount) {
        if (!isNil(addedBudgetAmounts.get(updatedRow.budgetName)))
            if (any(propEq('id', rowId), addedBudgetAmounts.get(updatedRow.budgetName)!)) {
                let amounts = addedBudgetAmounts.get(updatedRow.budgetName)!
                amounts[amounts.findIndex(b => b.id == rowId)] = updatedRow;
            }


        let mapEntry = updatedBudgetAmountsMaps.get(rowId)

        if(isNil(mapEntry)){
            const row = budgetAmountCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row)){
                updatedBudgetAmountsMaps.set(rowId, [row , updatedRow])
                if(!isNil(updatedBudgetAmounts.get(updatedRow.budgetName)))
                    updatedBudgetAmounts.get(updatedRow.budgetName)!.push(updatedRow)
                else
                    updatedBudgetAmounts.set(updatedRow.budgetName, [updatedRow])
            }               
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
            let amounts = updatedBudgetAmounts.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts[amounts.findIndex(r => r.id == updatedRow.id)] = updatedRow
        }
        else{
            updatedBudgetsMap.delete(rowId)
            let amounts = updatedBudgetAmounts.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            
        checkHasUnsavedChanges();
    }

    function clearChanges() {
        updatedBudgetsMap.clear();
        addedBudgets = [];
        deletionBudgetIds = [];
        updatedBudgetAmountsMaps.clear();
        updatedBudgetAmounts.clear();
        addedBudgetAmounts.clear();
        deletionYears = [];
        if (hasScenario)
            investmentPlan = clone(stateInvestmentPlan.value);
    }

    function resetPage() {
        pagination.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges() {
        const clonedStateInvestmentPlan: InvestmentPlan = clone(stateInvestmentPlan.value);
        const CheckStateInvestmentPlan: InvestmentPlan = {
            ...clonedStateInvestmentPlan,
            inflationRatePercentage: +clonedStateInvestmentPlan.inflationRatePercentage,
            firstYearOfAnalysisPeriod: +clonedStateInvestmentPlan.firstYearOfAnalysisPeriod,
            minimumProjectCostLimit: hasValue(clonedStateInvestmentPlan.minimumProjectCostLimit)
                ? parseFloat(clonedStateInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                : 0,
        };
        const clonedInvestmentPlan: InvestmentPlan = clone(investmentPlan);
        const CheckInvestmentPlan: InvestmentPlan = {
            ...clonedInvestmentPlan,
            inflationRatePercentage: +clonedInvestmentPlan.inflationRatePercentage,
            firstYearOfAnalysisPeriod: +clonedInvestmentPlan.firstYearOfAnalysisPeriod,
            minimumProjectCostLimit: hasValue(clonedInvestmentPlan.minimumProjectCostLimit)
                ? parseFloat(clonedInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                : 0,
        };
        const hasUnsavedChanges: boolean = 
            deletionBudgetIds.length > 0 || 
            addedBudgets.length > 0 ||
            updatedBudgetsMap.size > 0 || 
            deletionYears.length > 0 || 
            addedBudgetAmounts.size > 0 ||
            updatedBudgetAmounts.size > 0 || 
            (hasScenario && hasSelectedLibrary) ||
            (hasScenario && hasUnsavedChangesCore('', CheckInvestmentPlan, CheckStateInvestmentPlan)) || 
            (hasSelectedLibrary && hasUnsavedChangesCore('', selectedBudgetLibrary, stateSelectedBudgetLibrary))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        if (hasUnsavedChanges && unsavedDialogAllowed) {
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
            unsavedDialogAllowed = true;
            next();
        }
    };

    function setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: BudgetLibrary = emptyBudgetLibrary;
        stateBudgetLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioInvestment && importComp.id === selectedScenarioId ||
            hasSelectedLibrary && importComp.workType === WorkType.ImportLibraryInvestment && importComp.id === selectedBudgetLibrary.id){
            clearChanges()
            pagination.page = 1
            initializePages().then(async () => {
                setAlertMessageAction('');
                isSuccessfulImportMutator(true);
                await getBudgetLibrariesAction()
                if(hasScenario){                
                    investmentPlanMutator(investmentPlan);
                    firstYearOfAnalysisPeriodShift.value = 0;
                }
                else{

                }
            })
        }        
    }

    async function initializePages(){
        const request: InvestmentPagingRequestModel= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: selectedBudgetLibrary.id === uuidNIL ? null : selectedBudgetLibrary.id,
                updatedBudgets: Array.from(updatedBudgetsMap.values()).map(r => r[1]),
                budgetsForDeletion: deletionBudgetIds,
                addedBudgets: addedBudgets,
                deletionyears: deletionYears ,
                updatedBudgetAmounts: mapToIndexSignature( updatedBudgetAmounts),
                Investment: null,
                addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts),
                firstYearAnalysisBudgetShift: 0,
                isModified: false
            },           
            sortColumn: 'year',
            isDescending: false,
            search: ''
        };
        
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL){
            await InvestmentService.getScenarioInvestmentPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    BudgetCache = clone(currentPage);
                    BudgetCache.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache = currentPage.flatMap(_ => _.budgetAmounts)
                    totalItems = data.totalItems;
                    investmentPlan = data.investmentPlan;
                    investmentPlanMutator(investmentPlan)
                    syncInvestmentPlanWithBudgets();
                    lastYear = data.lastYear;
                    firstYear = data.firstYear;
                    originalFirstYear = data.firstYear;
                    if(data.firstYear === 0)
                        originalFirstYear = moment().year()
                }
                setParentLibraryName(currentPage.length > 0 ? currentPage[0].libraryId : "None");
                loadedParentId = currentPage.length > 0 ? currentPage[0].libraryId : "";
                loadedParentName = parentLibraryName; //store original
                scenarioLibraryIsModified = currentPage.length > 0 ? currentPage[0].isModified : false;
                initializing = false;
            });
        }            
        else if(hasSelectedLibrary)
                await InvestmentService.getLibraryInvestmentPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage = data.items;
                    BudgetCache = clone(currentPage);
                    BudgetCache.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache = currentPage.flatMap(_ => _.budgetAmounts)
                    totalItems = data.totalItems;
                    lastYear = data.lastYear;
                }
                initializing = false;
            });
        else
            initializing = false;
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

    .invest-owner-padding {
        padding-top: 7px;
    }

    .header-alignment-padding-center {
        padding-top: 54px
    }

    .header-alignment-padding-right {
        padding-top: 48px
    }

    .investment-divider {
        height: 22px;
        margin-top: 12px !important;
    }
</style>
