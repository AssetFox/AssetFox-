<template>
    <v-card height="1000px" class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col cols="12">
            <v-row justify="space-between">
                <v-col cols = "auto" class="ghd-constant-header">
                    <div style="margin-bottom: 10px;">
                        <v-subheader class="ghd-control-label ghd-md-gray"><span>Select an Investment library</span></v-subheader><!--class="ghd-md-gray ghd-control-subheader"-->
                    </div>
                        <v-select 
                        id="InvestmentEditor-investmentLibrary-select"
                        :items='librarySelectItems'
                        item-title="text"
                        item-value="value"
                        v-model='librarySelectItemValue'
                        class="ghd-select ghd-text-field ghd-text-field-border vs-style"
                        variant="outlined"
                        density="compact">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader" v-if="hasScenario"><b>Library Used: {{parentLibraryName}} <span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
                </v-col>

                <!-- these are only in library -->
                <v-col cols = "auto" v-if='!hasScenario' class="ghd-constant-header">
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
                </v-col>
                <v-col cols = "auto" v-if='!hasScenario' class="ghd-constant-header">
                    <v-row align-end justify-end class="header-alignment-padding-right">
                        <!-- <v-spacer></v-spacer> -->
                        <v-btn id="InvestmentEditor-CreateNewLibrary-vbtn" @click='onShowCreateBudgetLibraryDialog(false)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="!hasScenario"
                        variant = "outlined">
                            Create New Library
                        </v-btn>
                    </v-row>
                </v-col>
            </v-row>

            <!-- only for scenario -->
            <v-row style="margin-top:80px;" v-if='hasInvestmentPlanForScenario' align="end">
                <!-- text boxes for scenario only -->
                <v-col cols = "2" class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label"><span>First Year of Analysis Period</span></v-subheader>
                    <v-text-field id="InvestmentEditor-firstYearAnalysisPeriod-textField"
                                  variant="outlined"
                                  @change='onEditInvestmentPlan("firstYearOfAnalysisPeriod", $event)'
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"
                                  :mask="'####'"
                                  v-model='investmentPlan.firstYearOfAnalysisPeriod' 
                                  class="ghd-text-field-border ghd-text-field"/>
                </v-col>
                <v-col cols = "2" class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label"><span>Number of Years in Analysis Period</span></v-subheader>
                    <v-text-field id="InvestmentEditor-numberYearsAnalysisPeriod-textField"
                                  readonly variant="outlined"
                                  @change='onEditInvestmentPlan("numberOfYearsInAnalysisPeriod", $event)'
                                  v-model='investmentPlan.numberOfYearsInAnalysisPeriod'
                                  class="ghd-text-field-border ghd-text-field" />
                </v-col>
                <v-col cols = "2" class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label"><span>Minimum Project Cost Limit</span></v-subheader>
                    <currencyTextbox outline 
                                  id='InvestmentEditor-minimumProjectCostLimit-textField'
                                  @change='onEditInvestmentPlan("minimumProjectCostLimit", $event)'
                                  v-model='investmentPlan.minimumProjectCostLimit'
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.minimumProjectCostLimit)]"
                                  :disabled="!hasAdminAccess"
                                  variant="outlined"
                                  class="ghd-text-field-border ghd-text-field" />
                </v-col>
                <v-col cols = "2" class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label"><span>Inflation Rate Percentage</span></v-subheader>
                    <v-text-field id="InvestmentEditor-inflationRatePercentage-textField"
                                  variant="outlined"
                                  v-model='investmentPlan.inflationRatePercentage'
                                  @change='onEditInvestmentPlan("inflationRatePercentage", $event)'
                                  :mask="'###'"
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0,100])]"
                                  :disabled="!hasAdminAccess"
                                  class="ghd-text-field-border ghd-text-field" />
                </v-col>
                <v-col cols = "4" class="ghd-constant-header">
                    <v-switch id="InvestmentEditor-allowFundingCarryover-switch"
                              style="margin-left:10px;margin-top:50px;"
                              class="ghd-checkbox"
                              label="Allow Funding Carryover"
                              :disabled="!hasAdminAccess"
                              v-model="investmentPlan.shouldAccumulateUnusedBudgetAmounts"
                              @change='onEditInvestmentPlan("shouldAccumulateUnusedBudgetAmounts", $event)' />
                </v-col>
            </v-row>
            <v-divider :thickness="2" class="border-opacity-100" v-show ='hasScenario || hasSelectedLibrary' />

            <v-row justify-space-between v-show='hasSelectedLibrary || hasScenario'>
                <v-col cols = "6">
                    <v-row>
                        <v-col>
                        <v-btn id="InvestmentEditor-editBudgets-btn"
                            @click='onShowEditBudgetsDialog'
                            variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Edit Budgets
                        </v-btn>
                        </v-col>
                        <v-col cols="2">
                        <v-text-field id="InvestmentEditor-numberYearsToAdd-textField"
                                      :disabled='currentPage.length === 0' type="number" min=1 :mask="'##########'"
                                      class="ghd-text-field-border ghd-text-field"
                                      v-bind:class="{ 'ghd-blue-text-field': currentPage.length !== 0}"
                                      variant="outlined" density="compact" v-model.number="range" />
                        </v-col>
                        <v-col>
                        <v-btn id="InvestmentEditor-addBudgetYearRange-btn"
                               :disabled='currentPage.length === 0'
                               @click='onSubmitAddBudgetYearRange'
                               class='ghd-right-paired-button ghd-blue ghd-button-text ghd-outline-button-padding ' variant = "outlined">
                            Add Year(s)
                        </v-btn>
                        </v-col>
                    </v-row>
                    <v-row>
                        <div class = "ghd-md-gray ghd-control-subheader" style="margin-left:2% !important;"> 
                            Number of Budgets: {{ currentPage.length }}
                        </div>
                    </v-row>
                </v-col>
                <v-spacer></v-spacer>
                <v-col cols = "5">
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
                </v-col>
            </v-row>
        </v-col>

        <v-col v-show='hasSelectedLibrary || hasScenario'>             
                <v-data-table-server
                    id="InvestmentEditor-investmentsDataTable-dataTable"
                    :headers='budgetYearsGridHeaders' 
                    :items="budgetYearsGridData"                                       
                    :pagination.sync="pagination"                                   
                    :must-sort='true'
                    :items-length="totalItems"
                    :rows-per-page-items=[5,10,25]
                    :items-per-page-options="[
                        {value: 5, title: '5'},
                        {value: 10, title: '10'},
                        {value: 25, title: '25'},
                    ]"
                    item-key='year'
                    class='v-table__overflow ghd-table'        
                    show-select 
                    return-object
                    sort-icon=ghd-table-sort
                    v-model='selectedBudgetYearsGridData' 
                    v-model:sort-by="pagination.sort"
                    v-model:page="pagination.page"
                    v-model:items-per-page="pagination.rowsPerPage"
                    @update:options="onPaginationChanged"
                    >
                    <template v-slot:item="{item}">
                    <tr>
                        <td>
                            <v-checkbox hide-details primary v-model="selectedBudgetYearsGridData" :value="item"></v-checkbox>
                        </td>
                        <td v-for='header in budgetYearsGridHeaders'>
                            <div v-if="header.key === 'year'">
                                <span class='sm-txt'>{{ item.year + firstYearOfAnalysisPeriodShift}}</span>
                            </div>       
                           
                            <div v-if="header.key !== 'year' && header.key !== 'action'">
                                <editDialog :return-value.sync='item[header.key]'
                                    @save='onEditBudgetYearValue(item.year, header.key, item[header.key])'
                                    size="large" lazy>
                                    <currencyTextbox readonly single-line class='sm-txt'
                                        variant="underlined"
                                        :model-value='item[header.key]'
                                        :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <template v-slot:input>
                                        <currencyTextbox label='Edit' single-line
                                            v-model.number='item[header.key]'                                             
                                            :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    </template>
                                </editDialog>
                            </div>
                            <div v-if="header.key === 'action'">
                                <v-btn id="InvestmentEditor-removeYear-btn" @click="onRemoveBudgetYear(item.year)" class="ghd-blue" flat>
                                    <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')" />
                                </v-btn>
                            </div>
                        </td>
                    </tr>
                    </template>
                </v-data-table-server>
                <v-btn id="InvestmentEditor-deleteSelected-btn"
                        :disabled='selectedBudgetYears.length === 0' @click='onRemoveBudgetYears'
                        class='ghd-blue ghd-button' variant = "text">
                    Delete Selected
                </v-btn>
        </v-col>
    
        <v-col v-show='hasSelectedLibrary && !hasScenario' cols = "12">
            <v-row justify-center>
                <v-col>
                    <v-subheader class="ghd-label">Description</v-subheader>
                    <v-textarea no-resize rows='4'
                                v-model='selectedBudgetLibrary.description'
                                @update:model-value="checkHasUnsavedChanges()"
                                variant="outlined"
                                class="ghd-control-text ghd-control-border" density="compact">
                    </v-textarea>
                </v-col>
            </v-row>
        </v-col>
        <v-col cols="12">          
            <v-row style="padding-bottom: 40px;" v-show='hasSelectedLibrary || hasScenario' justify="center">
                <v-spacer></v-spacer>
                    <v-btn id="InvestmentEditor-cancel-btn"
                        :disabled='!hasUnsavedChanges' @click='onDiscardChanges' variant = "flat" class='ghd-blue ghd-button-text ghd-button'
                        v-show='hasScenario'>
                        Cancel
                    </v-btn>
                    <v-btn outline id="InvestmentEditor-deleteLibrary-btn"
                        @click='onShowConfirmDeleteAlert' variant = "text" class='ghd-blue ghd-button-text ghd-button' v-show='!hasScenario'
                        :disabled='!hasLibraryEditPermission'>
                        Delete Library
                    </v-btn>
                    <v-btn id="InvestmentEditor-createAsNewLibrary-btn"
                        :disabled='disableCrudButton()'
                        @click='onShowCreateBudgetLibraryDialog(true)'
                        style="margin-left: 5px;"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                        Create as New Library
                    </v-btn>
                    <v-btn id="InvestmentEditor-updateLibrary-btn"
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                        @click='onUpsertBudgetLibrary()'
                        style="margin-left: 5px;"
                        class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show='!hasScenario'>
                        Update Library
                    </v-btn>
                    <v-btn id="InvestmentEditor-save-btn"
                        :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                        @click='onUpsertInvestment()'
                        style="margin-left: 5px;"
                        class='ghd-blue-bg text-white ghd-button-text ghd-button'
                        v-show='hasScenario'>
                        Save
                    </v-btn>
                <v-spacer></v-spacer>
            </v-row>
        </v-col>

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
</v-card>
    <ConfirmDialog></ConfirmDialog>
</template>

<script setup lang='ts'>
import { shallowRef } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
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
import ConfirmDeleteAlert from '@/shared/modals/Alert.vue';
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
import {inject, reactive, ref, shallowReactive, onMounted, onBeforeUnmount, computed, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import mitt from 'mitt';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import { getUrl } from '@/shared/utils/get-url';
import  currencyTextbox  from '@/shared/components/CurrencyTextbox.vue';

let store = useStore();
const confirm = useConfirm();
const emit = defineEmits(['submit'])
const $router = useRouter();
const $emitter = mitt()
      
const stateBudgetLibraries = computed<BudgetLibrary[]>(() => store.state.investmentModule.budgetLibraries) ;  

const stateSelectedBudgetLibrary = computed<BudgetLibrary>(() => store.state.investmentModule.selectedBudgetLibrary);
let stateInvestmentPlan = computed<InvestmentPlan>(() => store.state.investmentModule.investmentPlan);
let stateScenarioBudgets = ref<Budget[]>(store.state.investmentModule.scenarioBudgets);
const hasUnsavedChanges = computed<boolean>(() => (store.state.unsavedChangesFlagModule.hasUnsavedChanges));
const hasAdminAccess = computed<boolean>(()=> (store.state.authenticationModule.hasAdminAccess)); 

let isSuccessfulImport = ref<boolean>(store.state.investmentModule.isSuccessfulImport);
let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter);
let hasPermittedAccess = computed<boolean>(() => store.state.investmentModule.hasPermittedAccess);

async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess', payload);}
async function getInvestmentAction(payload?: any): Promise<any> {await store.dispatch('getInvestment', payload);}
async function getBudgetLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getBudgetLibraries', payload);}
async function selectBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectBudgetLibrary', payload);}
async function upsertInvestmentAction(payload?: any): Promise<any> {await store.dispatch('upsertInvestment', payload);}
async function upsertBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertBudgetLibrary', payload);}
async function deleteBudgetLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteBudgetLibrary', payload);}
async function upsertOrDeleteBudgetLibraryUsersAction(payload: any): Promise<any> {await store.dispatch('upsertOrDeleteBudgetLibraryUsers', payload);}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}
async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges', payload);}
async function importScenarioInvestmentBudgetsFileAction(payload?: any): Promise<any> {await store.dispatch('importScenarioInvestmentBudgetsFile', payload);}
async function importLibraryInvestmentBudgetsFileAction(payload?: any): Promise<any> {await store.dispatch('importLibraryInvestmentBudgetsFile', payload);}
async function getCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCriterionLibraries', payload);}
async function setAlertMessageAction(payload?: any): Promise<any> {await store.dispatch('setAlertMessage', payload);}
async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification', payload);}
    
let getModifiedDate = store.getters.getLibraryDateModified;
let getUserNameByIdGetter = store.getters.getUserNameById;

function budgetLibraryMutator(payload:any){store.commit('budgetLibraryMutator', payload);}
function selectedBudgetLibraryMutator(payload:any){store.commit('selectedBudgetLibraryMutator', payload);}
function investmentPlanMutator(payload:any){store.commit('investmentPlanMutator', payload);}
function isSuccessfulImportMutator(payload:any){store.commit('isSuccessfulImportMutator', payload);}
    let modifiedDate: string;

    let addedBudgets = ref<Budget[]>([]);
    let updatedBudgetsMap = ref<Map<string, [Budget, Budget]>>(new Map<string, [Budget, Budget]>());//0: original value | 1: updated value
    let deletionBudgetIds = ref<string[]>([]);
    let BudgetCache = ref<Budget[]>([]);
    let budgetAmountCache = ref<BudgetAmount[]>([]);
    let updatedBudgetAmountsMaps = ref<Map<string, [BudgetAmount, BudgetAmount]>>(new Map<string, [BudgetAmount, BudgetAmount]>());//0: original value | 1: updated value 
    let addedBudgetAmounts = ref<Map<string, BudgetAmount[]>>(new  Map<string, BudgetAmount[]>());
    let deletionYears = ref<number[]>([]);
    let updatedBudgetAmounts = ref<Map<string, BudgetAmount[]>>(new  Map<string, BudgetAmount[]>());
    let gridSearchTerm = '';
    let currentSearch = '';
    //let pagination: Pagination = clone(emptyPagination);
    const pagination: Pagination = shallowReactive(clone(emptyPagination));
    
    let isPageInit = false;
    let totalItems = ref<number>(0);
    let currentPage = ref<Budget[]>([]);
    let lastYear: number = 0;
    let firstYear: number = 0;
    let initializing: boolean = true;

    const selectedBudgetLibrary = ref<BudgetLibrary>(clone(emptyBudgetLibrary));
    const investmentPlan = ref<InvestmentPlan>(clone(emptyInvestmentPlan));
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary = ref<boolean>(false);

    const librarySelectItems = ref<SelectItem[]>([]);
    const librarySelectItemNames = ref<string[]>([]);
    let librarySelectItemValue = ref<string|null>('');

    let actionHeader: any = { title: 'Action', key: 'action', align: 'left', sortable: false, class: '', width: '' }
    let budgetYearsGridHeaders = ref<any[]>([
        { title: 'Year', key: 'year', sortable: true, align: 'left', class: '', width: '' },
        actionHeader
    ]);
    let budgetYearsGridData = ref<BudgetYearsGridData[]>([]);
    let selectedBudgetYearsGridData = ref<BudgetYearsGridData[]>([]);
    let selectedBudgetYears = ref<number[]>([]);

    let createBudgetLibraryDialogData = ref<CreateBudgetLibraryDialogData>(clone(emptyCreateBudgetLibraryDialogData));
    
    let shareBudgetLibraryDialogData = ref<ShareBudgetLibraryDialogData>(clone(emptyShareBudgetLibraryDialogData));
    let editBudgetsDialogData = ref<EditBudgetsDialogData>(clone(emptyEditBudgetsDialogData));

    let showSetRangeForAddingBudgetYearsDialog: boolean = false;
    let showSetRangeForDeletingBudgetYearsDialog: boolean = false;
    let confirmDeleteAlertData = ref<AlertData>(clone(emptyAlertData));
    let uuidNIL: string = getBlankGuid();
    let rules: InputValidationRules = validationRules;
    let showImportExportInvestmentBudgetsDialog = ref<boolean>(false);
    let hasScenario = ref<boolean>(false);
    let hasInvestmentPlanForScenario = ref<boolean>(false);
    let hasCreatedLibrary: boolean = false;
    let budgets: Budget[] = [];
    let disableCrudButtonsResult = ref<boolean>(false);
    let hasLibraryEditPermission = ref<boolean>(false);
    let showReminder: boolean = false;
    let range: number = 1;
    let parentLibraryName = ref<string>("None");
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified = ref<boolean>(false);
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;

    let originalFirstYear: number = 0
    let firstYearOfAnalysisPeriodShift = shallowRef<number>(0);

    let unsavedDialogAllowed = ref<boolean>(true);
    let trueLibrarySelectItemValue = ref<string|null>('');
    let librarySelectItemValueAllowedChanged: boolean = true;

    function addYearLabel() {
        return 'Add Year (' + getNextYear() + ')';
    }

    function deleteYearLabel() {
            const latestYear = lastYear;
            return latestYear ? 'Delete Year (' + latestYear + ')' : 'Delete Year';
        }

    function yearsInAnalysisPeriod() {
            return investmentPlan.value.numberOfYearsInAnalysisPeriod;
        }
    
    // REPLACE with created() ?
    //function beforeRouteEnter() {
    // created();
    // async function created() {
    //     (() => {
    //         (async () => { 
    //             librarySelectItemValue.value = '';
    //             await getHasPermittedAccessAction();
    //             await getBudgetLibrariesAction()
        
    //             if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.Investment) !== -1) {
    //                 selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

    //                 if (selectedScenarioId === uuidNIL) {
    //                     addErrorNotificationAction({
    //                         message: 'Found no selected scenario for edit',
    //                     });
    //                     $router.push('/Scenarios/');
    //                 }

    //                 hasScenario.value = true;
    //                 ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioInvestment}).then(response => {
    //                     if(response.data){
    //                         setAlertMessageAction("An investment import has been added to the work queue")
    //                     }
    //                 })
    //                 await initializePages();
    //             }
    //             else
    //                 initializing = false;               
    //         })();                    
    //     });
    // }

    onMounted(async () => {
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

            hasScenario.value = true;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioInvestment}).then(response => {
                if(response.data){
                    setAlertMessageAction("An investment import has been added to the work queue")
                }
            })
                await initializePages();
        }
        else
            initializing = false;        

        $emitter.on(
                Hub.BroadcastEventType.BroadcastImportCompletionEvent,
                importCompleted,
        );
    });

    onBeforeUnmount(() =>  {
        setHasUnsavedChangesAction({ value: false });
        $emitter.off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    });
        
    // Watchers
    watch(pagination, ()=> onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: InvestmentPagingRequestModel= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: selectedBudgetLibrary.value.id === uuidNIL ? null : selectedBudgetLibrary.value.id,
                updatedBudgets: Array.from(updatedBudgetsMap.value.values()).map(r => r[1]),
                budgetsForDeletion: deletionBudgetIds.value,
                addedBudgets: addedBudgets.value,
                deletionyears: deletionYears.value ,
                updatedBudgetAmounts: mapToIndexSignature( updatedBudgetAmounts.value),
                Investment: !isNil(investmentPlan) ? {
                ...investmentPlan.value,
                minimumProjectCostLimit: hasValue(investmentPlan.value.minimumProjectCostLimit)
                    ? parseFloat(investmentPlan.value.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
                } : investmentPlan,
                addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts.value),
                firstYearAnalysisBudgetShift: firstYearOfAnalysisPeriodShift.value,
                isModified: scenarioLibraryIsModified.value
            },           
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch
        };
        
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL){
            await InvestmentService.getScenarioInvestmentPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    firstYear = data.firstYear;
                    currentPage.value = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    BudgetCache.value = clone(currentPage.value);
                    BudgetCache.value.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache.value = currentPage.value.flatMap(_ => _.budgetAmounts)
                    totalItems.value = data.totalItems;
                    investmentPlan.value = data.investmentPlan;                   
                    lastYear = data.lastYear;
                    
                    syncInvestmentPlanWithBudgets();
                    
                }
            });
        }            
        else if(hasSelectedLibrary.value){
            if(librarySelectItemValue === null)
                return;
                await InvestmentService.getBudgetLibraryModifiedDate(selectedBudgetLibrary.value.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate = data.slice(0, 10);
                   }
             });

            await InvestmentService.getLibraryInvestmentPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage.value = data.items;
                    BudgetCache.value = clone(currentPage.value);
                    BudgetCache.value.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache.value = currentPage.value.flatMap(_ => _.budgetAmounts)
                    totalItems.value = data.totalItems;
                    lastYear = data.lastYear
                }
            }); 
        }                
    }
   

    watch(deletionBudgetIds, () => {
        checkHasUnsavedChanges();
    });

    watch(addedBudgets, () => {
        checkHasUnsavedChanges();
    });

    watch(deletionYears, () => {
        checkHasUnsavedChanges();
    });

    watch(addedBudgetAmounts, () => {
        checkHasUnsavedChanges();
    });

    // watch(stateBudgetLibraries,()=> onStateBudgetLibrariesChanged)
    // function onStateBudgetLibrariesChanged() {
    //     librarySelectItems = stateBudgetLibraries.value
    //         .map((library: BudgetLibrary) => ({
    //             text: library.name,
    //             value: library.id,
    //         }));
    //     librarySelectItemNames = librarySelectItems.map((library: SelectItem) => library.text)
    // }

    let libraryItems =  ref<BudgetLibrary[]>([]);
    
    watch(stateBudgetLibraries, ()=> {
        libraryItems.value = clone(stateBudgetLibraries.value);
        librarySelectItems.value=[];
        librarySelectItemNames.value =[];

        libraryItems.value.forEach(_=> {
            librarySelectItems.value.push({text:_.name,value:_.id})
            librarySelectItemNames.value.push(_.name)
        })
    });

    watch(selectedBudgetYearsGridData,()=> {
        selectedBudgetYears.value = getPropertyValues('year', selectedBudgetYearsGridData.value) as number[];
    });

    watch(librarySelectItemValue,() => {
        hasSelectedLibrary.value = true;

        if(hasScenario.value){
            onSelectItemValueChanged();
            unsavedDialogAllowed.value = false;
        }           
        else if(librarySelectItemValueAllowedChanged){
            CheckUnsavedDialog(onSelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                onSelectItemValueChanged();
                librarySelectItemValue.value = trueLibrarySelectItemValue.value;              
            });
        }
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        scenarioLibraryIsModified.value = false;
        librarySelectItemValueAllowedChanged = true;       
    });

    function onSelectItemValueChanged() {      
        trueLibrarySelectItemValue = librarySelectItemValue
        selectBudgetLibraryAction(librarySelectItemValue.value);
    }

    watch(stateSelectedBudgetLibrary,() => {
        selectedBudgetLibrary.value = clone(stateSelectedBudgetLibrary.value);
    });

    watch(selectedBudgetLibrary,()=> {     
        hasSelectedLibrary.value = selectedBudgetLibrary.value.id !== uuidNIL;
        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedBudgetLibrary.value.id, workType: WorkType.ImportLibraryInvestment}).then(response => {
                if(response.data){
                    setAlertMessageAction("An investment import has been added to the work queue")
                }
                else
                    setAlertMessageAction("");
            })
        }

        clearChanges()
       onPaginationChanged()
    });

    watch(stateInvestmentPlan,() => {
        cloneStateInvestmentPlan();
        hasInvestmentPlanForScenario.value = true;
        checkHasUnsavedChanges();
    });

    watch(currentPage, () => {
        setGridHeaders();
        setGridData();
        
        checkHasUnsavedChanges();

        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName.value = library.text;
            }
        });
    });

    watch(investmentPlan,() => {
        checkHasUnsavedChanges()
        if(hasScenario.value){
            const firstYear = +investmentPlan.value.firstYearOfAnalysisPeriod;
            const stateFirstYear = +stateInvestmentPlan.value.firstYearOfAnalysisPeriod;
            firstYearOfAnalysisPeriodShift.value = (firstYear - originalFirstYear) - (firstYear === 0 ? 0 : (firstYear - originalFirstYear));
        }
            
        if(investmentPlan.value.id === uuidNIL)
            investmentPlan.value.id = getNewGuid();
        hasInvestmentPlanForScenario.value = true;
    });

    watch(firstYearOfAnalysisPeriodShift,() => {
        setGridData();
    });

    function onRemoveBudgetYears() {
        deletionYears.value = deletionYears.value.concat(selectedBudgetYears.value)
        let deletedAddYears: number[] = [];
        for(let [key, value] of addedBudgetAmounts.value){
            let val = addedBudgetAmounts.value.get(key)! 
            val = val.filter(v => {
                if(!deletionYears.value.includes(v.year)){                  
                    return true;
                }

                deletedAddYears.push(v.year);  
                return false;
            })
            if (val.length == 0)
                addedBudgetAmounts.value.delete(key)
        }
        if (deletedAddYears.length > 0) {
            selectedBudgetYears.value = selectedBudgetYears.value.filter(year => !deletedAddYears.includes(year))
            deletionYears.value = deletionYears.value.concat(selectedBudgetYears.value)
            onPaginationChanged();
            return;
        }

        let deletedUpdateIds: string[] = [];

        for (let [key, value] of updatedBudgetAmounts.value) {
            let val = updatedBudgetAmounts.value.get(key)!
            const ids = val.filter(v => deletionYears.value.includes(v.year)).map(v => v.id)
            deletedUpdateIds = deletedUpdateIds.concat(ids)
            val = val.filter(v => !deletionYears.value.includes(v.year))
            if (val.length == 0)
                updatedBudgetAmounts.value.delete(key)
        }

        deletedUpdateIds.forEach(id => {
            updatedBudgetAmountsMaps.value.delete(id);
        })

        onPaginationChanged();
    }

    function onRemoveBudgetYear(year: number) {
        let isyearAdded = false;
        for (let [key, value] of addedBudgetAmounts.value) {
            let val = addedBudgetAmounts.value.get(key)!
            val = val.filter(v => {
                if (v.year !== year) {
                    return true;
                }

                isyearAdded = true;

                return false;
            })
            if(val.length == 0)
                addedBudgetAmounts.value.delete(key)
            else
            {
                addedBudgetAmounts.value.set(key, val)
            }
        }
        if(isyearAdded){
            onPaginationChanged();
            return;
        }
        deletionYears.value.push(year)

        let deleteIds: string[] = []
        for (let [key, value] of updatedBudgetAmounts.value) {
            let val = updatedBudgetAmounts.value.get(key)!
            const ids = val.filter(v => v.year === year).map(v => v.id)
            deleteIds = deleteIds.concat(ids)
            val = val.filter(v => v.year !== year)
            if (val.length == 0)
                updatedBudgetAmounts.value.delete(key)
        }

        deleteIds.forEach(id => {
            updatedBudgetAmountsMaps.value.delete(id);
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
        if (hasScenario.value) {
            const budgetsHaveUnsavedChanges: boolean = hasUnsavedChangesCore('', currentPage.value, stateScenarioBudgets.value);


            const clonedInvestmentPlan: InvestmentPlan = clone(investmentPlan.value);
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
        } else if (hasSelectedLibrary.value) {
            const hasUnsavedChanges: boolean = hasUnsavedChangesCore('',
                { ...clone(selectedBudgetLibrary.value), budgets: clone(currentPage.value) },
                stateSelectedBudgetLibrary.value);
            setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }

    function setGridHeaders() {
        const budgetNames: string[] = getPropertyValues('name', currentPage.value) as string[];
        const budgetHeaders: any[] = budgetNames
            .map((name: string) => ({
                title: name,
                key: name,
                sortable: true,
                align: 'left',
                class: '',
                width: '',
            }) as any);
        budgetYearsGridHeaders.value = [
            budgetYearsGridHeaders.value[0],
            ...budgetHeaders,
            actionHeader
        ];
    }

    function setGridData() {
        budgetYearsGridData.value = [];
        if(currentPage.value.length <= 0)
            return;
        for(let i = 0; i < currentPage.value[0].budgetAmounts.length; i++){
            let year = currentPage.value[0].budgetAmounts[i].year
            let values: {[budgetName: string]: number | null} = {}
            for(let o = 0; o < currentPage.value.length; o++){
                values[currentPage.value[o].name] = currentPage.value[o].budgetAmounts[i].value
            }
            budgetYearsGridData.value.push({year, values})
        }        
    }

    function syncInvestmentPlanWithBudgets() {//gets call in on pagination now       
        investmentPlan.value.numberOfYearsInAnalysisPeriod = totalItems.value > 0 ? totalItems.value : 1
    }

    function onShareBudgetLibraryDialogSubmit(budgetLibraryUsers: BudgetLibraryUser[]) {
        shareBudgetLibraryDialogData.value = clone(emptyShareBudgetLibraryDialogData);

        if (!isNil(budgetLibraryUsers) && selectedBudgetLibrary.value.id !== getBlankGuid())
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

            upsertOrDeleteBudgetLibraryUsersAction((selectedBudgetLibrary.value.id, libraryUserData));

            //update budget library sharing
            InvestmentService.upsertOrDeleteBudgetLibraryUsers(selectedBudgetLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                {
                    addSuccessNotificationAction({ message: 'Shared budget library' })
                    resetPage();
                }
            })
        }
    }

    function onShowCreateBudgetLibraryDialog(createAsNewLibrary: boolean) {
        createBudgetLibraryDialogData.value = {
            showDialog: true,
            budgets: createAsNewLibrary ? currentPage.value : [],
        };
    }

    function onSubmitCreateCreateBudgetLibraryDialogResult(budgetLibrary:BudgetLibrary) {//needs a few things        
        createBudgetLibraryDialogData.value = clone(emptyCreateBudgetLibraryDialogData);

    if (!isNil(budgetLibrary)) {
        hasCreatedLibrary = true;
        librarySelectItemValue.value = budgetLibrary.id;
        const libraryUpsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
            library: budgetLibrary,
            isNewLibrary: true,
            syncModel: {
                libraryId: budgetLibrary.budgets.length === 0 || !hasSelectedLibrary.value ? null : selectedBudgetLibrary.value.id,
                Investment: investmentPlan.value,
                budgetsForDeletion: budgetLibrary.budgets.length === 0 ? [] : deletionBudgetIds.value,
                updatedBudgets: budgetLibrary.budgets.length === 0 ? [] : Array.from(updatedBudgetsMap.value.values()).map(r => r[1]),
                addedBudgets: budgetLibrary.budgets.length === 0 ? [] : addedBudgets.value,
                deletionyears: budgetLibrary.budgets.length === 0 ? [] : deletionYears.value,
                updatedBudgetAmounts: budgetLibrary.budgets.length === 0 ? {} : mapToIndexSignature(updatedBudgetAmounts.value),
                addedBudgetAmounts: budgetLibrary.budgets.length === 0 ? {} : mapToIndexSignature(addedBudgetAmounts.value),
                firstYearAnalysisBudgetShift: 0,
                isModified: false
            },
            scenarioId: hasScenario.value ? selectedScenarioId : null
        }
        // value in v-currency is not parsed back to a number throwing an silent exception between UI and backend.
        const parsedMinimumProjectCostLimit: number = parseFloat(investmentPlan.value.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''));
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
            //return getUserNameByIdGetter(selectedBudgetLibrary.owner);
        }

        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission.value = hasAdminAccess || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedBudgetLibrary.value.owner) == getUserName();
    }

    function onAddBudgetYear() {
        const nextYear: number = getNextYear();

        const budgets: Budget[] = clone(currentPage.value);

        budgets.forEach((budget: Budget) => {
            const newBudgetAmount: BudgetAmount = {
                id: getNewGuid(),
                budgetName: budget.name,
                year: nextYear,
                value: 0,
            };
            let amounts = addedBudgetAmounts.value.get(budget.id)
            if (!isNil(amounts))
                amounts.push(newBudgetAmount);
            else
                addedBudgetAmounts.value.set(budget.id, [newBudgetAmount])
        });

        onPaginationChanged();
    }

    function onRemoveLatestBudgetYear() {
        const latestYear: number = lastYear;

        const budgets: Budget[] = clone(currentPage.value);

        budgets.forEach((budget: Budget) => {
            budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                !(budgetAmount.year == latestYear));
        });

        currentPage.value = clone(budgets);
    }

    function onShowShareBudgetLibraryDialog(budgetLibrary: BudgetLibrary) { 
            shareBudgetLibraryDialogData.value = 
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

            const budgets: Budget[] = clone(currentPage.value);

            for (let currentYear = startYear; currentYear < endYear; currentYear++) {
                budgets.forEach((budget: Budget) => {
                    const newBudgetAmount: BudgetAmount = {
                        id: getNewGuid(),
                        budgetName: budget.name,
                        year: currentYear,
                        value: 0,
                    };
                    let amounts = addedBudgetAmounts.value.get(budget.name)
                    if (!isNil(amounts))
                        amounts.push(newBudgetAmount);
                    else
                        addedBudgetAmounts.value.set(budget.name, [newBudgetAmount])
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

            const budgets: Budget[] = clone(currentPage.value);

            budgets.forEach((budget: Budget) => {
                budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
                    !(budgetAmount.year >= startYear && budgetAmount.year <= endYear));
            });

            currentPage.value = clone(budgets);
        }
    }


    function onShowEditBudgetsDialog() {

        currentPage.value.sort((b1, b2) => b1.budgetOrder - b2.budgetOrder);
        editBudgetsDialogData.value = {
            showDialog: true,
            budgets: clone(BudgetCache.value),
            scenarioId: selectedScenarioId,
        };
    }

    function onSubmitEditBudgetsDialogResult(budgetChanges: EmitedBudgetChanges) {        
        editBudgetsDialogData.value = clone(emptyEditBudgetsDialogData);
        if(!isNil(budgetChanges)){
            addedBudgets.value = addedBudgets.value.concat(budgetChanges.addedBudgets);
            budgetChanges.addedBudgets.forEach(budget => {
                let amounts: BudgetAmount[] = [];
                if(currentPage.value.length > 0){
                    currentPage.value[0].budgetAmounts.forEach(amount => {
                        amounts.push({
                            id: getNewGuid(),
                            budgetName: budget.name,
                            year: amount.year,
                            value: 0,
                        }) 
                    })
                }
                addedBudgetAmounts.value.set(budget.name, amounts);
            });          
            let addedIds = addedBudgets.value.map(b => b.id);            
            budgetChanges.deletionIds.forEach(id => removeBudget(id));
            deletionBudgetIds.value = deletionBudgetIds.value.filter(b => !addedIds.includes(b));
            budgetChanges.updatedBudgets.forEach(budget => onUpdateBudget(budget.id, budget));                       
            
            onPaginationChanged();
        }      
    }
    function removeBudget(id: string){
        if(any(propEq('id', id), addedBudgets.value)){
            addedBudgets.value = addedBudgets.value.filter((addBudge: Budget) => addBudge.id != id);
            deletionBudgetIds.value.push(id);
            const budget = currentPage.value.find(b => b.id === id);
            if(!isNil(budget))
                addedBudgetAmounts.value.delete(budget.name)
        }              
        else if(any(propEq('id', id), Array.from(updatedBudgetsMap.value.values()).map(r => r[1]))){
            updatedBudgetsMap.value.delete(id)
            deletionBudgetIds.value.push(id);
        }          
        else
            deletionBudgetIds.value.push(id);
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
        const id: string = hasScenario.value ? selectedScenarioId : selectedBudgetLibrary.value.id;
        InvestmentService.exportInvestmentBudgets(id, hasScenario.value)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }
    function onSubmitImportExportInvestmentBudgetsDialogResult(result: ImportExportInvestmentBudgetsDialogResult) {
        showImportExportInvestmentBudgetsDialog.value = false;

        if (hasValue(result)) {
            if (result.isExport) {


            } else if (hasValue(result.file)) {
                const data: InvestmentBudgetFileImport = {
                    file: result.file,
                    overwriteBudgets: result.overwriteBudgets,
                };

            if (hasScenario.value) {
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
                    id: selectedBudgetLibrary.value.id,
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
        if (any(propEq('name', budgetName), currentPage.value)) {
            console.log("YES")
            const budget: Budget = find(propEq('name', budgetName), currentPage.value) as Budget;

            if (any(propEq('year', year), budget.budgetAmounts)) {
                console.log("YES2")
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
        investmentPlan.value = setItemPropertyValue(property, value, investmentPlan.value);
    }

    function onUpsertInvestment() {
        const investmentPlanUpsert: InvestmentPlan = clone(investmentPlan.value);

        if (selectedBudgetLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection ===false) {scenarioLibraryIsModified.value = true;}
        else { scenarioLibraryIsModified.value = false; }

        const sync: InvestmentPagingSyncModel = {
            libraryId: selectedBudgetLibrary.value.id === uuidNIL ? null : selectedBudgetLibrary.value.id,
            updatedBudgets: Array.from(updatedBudgetsMap.value.values()).map(r => r[1]),
            budgetsForDeletion: deletionBudgetIds.value,
            addedBudgets: addedBudgets.value,
            deletionyears: deletionYears.value,
            updatedBudgetAmounts: mapToIndexSignature(updatedBudgetAmounts.value),
            Investment: {
                ...investmentPlan.value,
                minimumProjectCostLimit: hasValue(investmentPlan.value.minimumProjectCostLimit)
                    ? parseFloat(investmentPlan.value.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                    : 0,
            },
            addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts.value),
            firstYearAnalysisBudgetShift: firstYearOfAnalysisPeriodShift.value,
            isModified: scenarioLibraryIsModified.value,
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
            libraryId: selectedBudgetLibrary.value.id === uuidNIL ? null : selectedBudgetLibrary.value.id,
            updatedBudgets: Array.from(updatedBudgetsMap.value.values()).map(r => r[1]),
            budgetsForDeletion: deletionBudgetIds.value,
            addedBudgets: addedBudgets.value,
            deletionyears: deletionYears.value ,
            updatedBudgetAmounts: mapToIndexSignature(updatedBudgetAmounts.value),
            Investment: null,
            addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts.value),
            firstYearAnalysisBudgetShift: 0,
            isModified: false
        }

        const upsertRequest: InvestmentLibraryUpsertPagingRequestModel = {
            library: selectedBudgetLibrary.value,
            isNewLibrary: false,
            syncModel: sync,
            scenarioId: null
        }
        InvestmentService.upsertBudgetLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()
                budgetLibraryMutator(selectedBudgetLibrary.value);
                selectedBudgetLibraryMutator(selectedBudgetLibrary.value.id);
                addSuccessNotificationAction({message: "Updated budget library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario.value) {
                clearChanges();
                resetPage();
            }
        });
        parentLibraryName.value = loadedParentName;
        parentLibraryId = loadedParentId;
    }

    function formatAsCurrency(value: any): any {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
    }

    function onShowConfirmDeleteAlert() {
        confirmDeleteAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function onSubmitConfirmDeleteAlertResult(submit: boolean) {
        confirmDeleteAlertData.value = clone(emptyAlertData);

        if (submit) {
            librarySelectItemValue.value = null;
            deleteBudgetLibraryAction(selectedBudgetLibrary.value.id);
        }
    }

    function disableCrudButton() {
        const allBudgetDataIsValid: boolean = currentPage.value.every((budget: Budget) => {
            let amountsAreValid = true;
            const addedAmounts = addedBudgetAmounts.value.get(budget.name);
            const updatedAmounts = updatedBudgetAmounts.value.get(budget.name);
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

        if (hasSelectedLibrary.value) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedBudgetLibrary.value.name) === true &&
                allBudgetDataIsValid);
        } else if (hasScenario.value) {
            const allInvestmentPlanDataIsValid: boolean = rules['generalRules'].valueIsNotEmpty(investmentPlan.value.minimumProjectCostLimit) === true &&
                rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.value.minimumProjectCostLimit) === true &&
                rules['generalRules'].valueIsNotEmpty(investmentPlan.value.inflationRatePercentage) === true &&
                rules['generalRules'].valueIsWithinRange(investmentPlan.value.inflationRatePercentage, [0, 100]);

            return !(allBudgetDataIsValid && allInvestmentPlanDataIsValid);
        }

        disableCrudButtonsResult.value = !allBudgetDataIsValid;
        return !allBudgetDataIsValid;
    }

    function onSearchClick() {
        currentSearch = gridSearchTerm;
        resetPage();
    }

    function onUpdateBudget(rowId: string, updatedRow: Budget){        
    if(any(propEq('id', rowId), addedBudgets.value))
    {            
        addedBudgets.value[addedBudgets.value.findIndex((b => b.id == rowId))] = updatedRow;
        return;
    }
    let mapEntry = updatedBudgetsMap.value.get(rowId)
    if(isNil(mapEntry)){
        const row = BudgetCache.value.find(r => r.id === rowId);
        if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
            updatedBudgetsMap.value.set(rowId, [row , updatedRow])
    }
    else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
        mapEntry[1] = updatedRow;
    }
    else
        updatedBudgetsMap.value.delete(rowId)

        checkHasUnsavedChanges();
    }

    function onUpdateBudgetAmount(rowId: string, updatedRow: BudgetAmount) {
        if (!isNil(addedBudgetAmounts.value.get(updatedRow.budgetName)))
            if (any(propEq('id', rowId), addedBudgetAmounts.value.get(updatedRow.budgetName)!)) {
                let amounts = addedBudgetAmounts.value.get(updatedRow.budgetName)!
                amounts[amounts.findIndex(b => b.id == rowId)] = updatedRow;
            }


        let mapEntry = updatedBudgetAmountsMaps.value.get(rowId)

        if(isNil(mapEntry)){
            const row = budgetAmountCache.value.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row)){
                updatedBudgetAmountsMaps.value.set(rowId, [row , updatedRow])
                if(!isNil(updatedBudgetAmounts.value.get(updatedRow.budgetName)))
                    updatedBudgetAmounts.value.get(updatedRow.budgetName)!.push(updatedRow)
                else
                    updatedBudgetAmounts.value.set(updatedRow.budgetName, [updatedRow])
            }               
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
            let amounts = updatedBudgetAmounts.value.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts[amounts.findIndex(r => r.id == updatedRow.id)] = updatedRow
        }
        else{
            updatedBudgetsMap.value.delete(rowId)
            let amounts = updatedBudgetAmounts.value.get(updatedRow.budgetName)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            
        checkHasUnsavedChanges();
    }

    function clearChanges() {
        updatedBudgetsMap.value.clear();
        addedBudgets.value = [];
        deletionBudgetIds.value = [];
        updatedBudgetAmountsMaps.value.clear();
        updatedBudgetAmounts.value.clear();
        addedBudgetAmounts.value.clear();
        deletionYears.value = [];
        if (hasScenario.value)
            investmentPlan.value = clone(stateInvestmentPlan.value);
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
        const clonedInvestmentPlan: InvestmentPlan = clone(investmentPlan.value);
        const CheckInvestmentPlan: InvestmentPlan = {
            ...clonedInvestmentPlan,
            inflationRatePercentage: +clonedInvestmentPlan.inflationRatePercentage,
            firstYearOfAnalysisPeriod: +clonedInvestmentPlan.firstYearOfAnalysisPeriod,
            minimumProjectCostLimit: hasValue(clonedInvestmentPlan.minimumProjectCostLimit)
                ? parseFloat(clonedInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                : 0,
        };
        const hasUnsavedChanges: boolean = 
            deletionBudgetIds.value.length > 0 || 
            addedBudgets.value.length > 0 ||
            updatedBudgetsMap.value.size > 0 || 
            deletionYears.value.length > 0 || 
            addedBudgetAmounts.value.size > 0 ||
            updatedBudgetAmounts.value.size > 0 || 
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasScenario.value && hasUnsavedChangesCore('', CheckInvestmentPlan, CheckStateInvestmentPlan)) || 
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedBudgetLibrary.value, stateSelectedBudgetLibrary.value))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        if (hasUnsavedChanges.value && unsavedDialogAllowed.value) {
            confirm.require({
                message: "You have unsaved changes. Are you sure you wish to continue?",
                header: "Unsaved Changes",
                icon: 'pi pi-question-circle',
                accept: ()=>next(),
                reject: ()=>otherwise()
            });
        } 
        else {
            unsavedDialogAllowed.value = true;
            next();
        }
    };

    function setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            parentLibraryName.value = "None";
            return;
        }
        let foundLibrary: BudgetLibrary = emptyBudgetLibrary;
        stateBudgetLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioInvestment && importComp.id === selectedScenarioId ||
            hasSelectedLibrary.value && importComp.workType === WorkType.ImportLibraryInvestment && importComp.id === selectedBudgetLibrary.value.id){
            clearChanges()
            pagination.page = 1
            initializePages().then(async () => {
                setAlertMessageAction('');
                isSuccessfulImportMutator(true);
                await getBudgetLibrariesAction()
                if(hasScenario.value){                
                    investmentPlanMutator(investmentPlan.value);
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
                libraryId: selectedBudgetLibrary.value.id === uuidNIL ? null : selectedBudgetLibrary.value.id,
                updatedBudgets: Array.from(updatedBudgetsMap.value.values()).map(r => r[1]),
                budgetsForDeletion: deletionBudgetIds.value,
                addedBudgets: addedBudgets.value,
                deletionyears: deletionYears.value ,
                updatedBudgetAmounts: mapToIndexSignature( updatedBudgetAmounts.value),
                Investment: null,
                addedBudgetAmounts: mapToIndexSignature(addedBudgetAmounts.value),
                firstYearAnalysisBudgetShift: 0,
                isModified: false
            },           
            sortColumn: 'year',
            isDescending: false,
            search: ''
        };
        
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL){
            await InvestmentService.getScenarioInvestmentPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage.value = data.items.sort((a, b) => a.budgetOrder - b.budgetOrder);
                    BudgetCache.value = clone(currentPage.value);
                    BudgetCache.value.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache.value = currentPage.value.flatMap(_ => _.budgetAmounts)
                    totalItems.value = data.totalItems;
                    investmentPlan.value = data.investmentPlan;
                    investmentPlanMutator(investmentPlan.value)
                    syncInvestmentPlanWithBudgets();
                    lastYear = data.lastYear;
                    firstYear = data.firstYear;
                    originalFirstYear = data.firstYear;
                    if(data.firstYear === 0)
                        originalFirstYear = moment().year()
                }
                setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                loadedParentName = parentLibraryName.value; //store original
                scenarioLibraryIsModified.value = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                initializing = false;
            });
        }            
        else if(hasSelectedLibrary.value)
                await InvestmentService.getLibraryInvestmentPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as InvestmentPagingPage;
                    currentPage.value = data.items;
                    BudgetCache.value = clone(currentPage.value);
                    BudgetCache.value.forEach(_ => _.budgetAmounts = []);
                    budgetAmountCache.value = currentPage.value.flatMap(_ => _.budgetAmounts)
                    totalItems.value = data.totalItems;
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