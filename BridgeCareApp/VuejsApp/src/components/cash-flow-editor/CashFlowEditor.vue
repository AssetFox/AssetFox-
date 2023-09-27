<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Cash Flow Library</v-subheader>
                    <v-select
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        id="CashFlowEditor-SelectLibrary-vselect"
                        variant="outlined"
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'><b>{{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">    
                    <v-layout row v-show='hasSelectedLibrary || hasScenario' style="padding-top: 28px !important">
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content" style="padding-top: 7px !important">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <v-divider class="owner-shared-divider" inset vertical
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                        </v-divider>
                         <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot: badge>
                        <span>Shared</span>
                        </template>
                        </v-badge>
                        <v-btn @click='onShowShareCashFlowRuleLibraryDialog(selectedCashFlowRuleLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                    </v-btn>
                    </v-layout>  
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">                   
                    <v-layout row align-end style="padding-top: 22px !important">
                        <v-spacer></v-spacer>
                        <v-btn @click="showAddCashFlowRuleDialog = true" v-show="hasSelectedLibrary || hasScenario"
                            id="CashFlowEditor-addCashFlowRule-btn" 
                            outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                            Add Cash Flow Rule
                        </v-btn>
                        <v-btn @click="onShowCreateCashFlowRuleLibraryDialog(false)"
                            id="CashFlowEditor-addCashFlowLibrary-btn"
                            outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            v-show="!hasScenario">
                            Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <div class="cash-flow-library-tables">
                <v-data-table
                    id="CashFlowEditor-cashFlowRules-table"
                    :headers="cashFlowRuleGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :total-items="totalItems"
                    :rows-per-page-items=[5,10,25]
                    sort-icon=$vuetify.icons.ghd-table-sort
                    v-model='selectedCashRuleGridRows'
                    class="ghd-table v-table__overflow"
                    item-key="id"
                    select-all>
                    <template v-slot:item="{item}" slot="items" slot-scope="props">
                        <td>
                            <v-checkbox hide-details primary v-model='item.raw.selected'></v-checkbox>
                        </td>
                        <td>
                            <v-edit-dialog
                                :return-value.sync="item.name"
                                large
                                lazy
                                @save="onEditSelectedLibraryListData(item,'description')"
                                >
                                <v-text-field
                                    id="CashFlowEditor-ruleName-text"
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="item.name"
                                    :rules="[inputRules.generalRules.valueIsNotEmpty]"/>
                                <template slot="input">
                                    <v-textarea
                                        label="Description"
                                        no-resize
                                        outline
                                        rows="5"
                                        :rules="[inputRules.generalRules.valueIsNotEmpty]"
                                        v-model="item.name"/>
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-layout align-center style='flex-wrap:nowrap'>
                                <v-menu
                                bottom
                                min-height="500px"
                                min-width="500px">
                                <template slot="activator">
                                    <v-text-field
                                        id="CashFlowEditor-criteria-text"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value="item.criterionLibrary.mergedCriteriaExpression"/>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            :value="
                                                item.criterionLibrary.mergedCriteriaExpression"
                                            full-width
                                            no-resize
                                            outline
                                            readonly
                                            rows="5"/>
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                            <v-btn
                                @click="onEditCashFlowRuleCriterionLibrary(item)"
                                id="CashFlowEditor-editCashFlowRule-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>
                            </v-layout>
                                                   
                        </td>
                        <td>
                            <v-layout style='flex-wrap:nowrap'>
                                <v-btn
                                @click="onDeleteCashFlowRule(item.id)"
                                id="CashFlowEditor-deleteCashFlowRule-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                            </v-btn>
                            <v-btn
                                @click="onSelectCashFlowRule(item.id)"
                                id="CashFlowEditor-editCashFlowRuleDistribution-btn"
                                class="ghd-blue"
                                icon>
                                <img class='img-general' :src="require('@/assets/icons/edit-cash.svg')"/>
                            </v-btn>
                            </v-layout>                          
                        </td>
                    </template>
                </v-data-table>

                <v-btn :disabled='selectedCashRuleGridRows.length === 0' @click='onDeleteSelectedCashFlowRules'
                    class='ghd-blue ghd-button' flat>
                    Delete Selected
                </v-btn>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea
                        class="ghd-text-field-border"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedCashFlowRuleLibrary.description"
                        @input='checkHasUnsavedChanges()'>
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout
                justify-center
                row
                v-show="hasSelectedLibrary || hasScenario">
                <v-btn outline
                    @click="onDeleteCashFlowRuleLibrary"
                    id="CashFlowEditor-deleteLibrary-btn"
                    flat class='ghd-blue ghd-button-text ghd-button'
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission">
                    Delete Library
                </v-btn>   
                <v-btn
                    @click="onDiscardChanges"
                    v-show="hasScenario"
                    :disabled="!hasUnsavedChanges" flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtons()"
                    @click="onShowCreateCashFlowRuleLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline> 
                    Create as New Library
                </v-btn>
                <v-btn
                    id="CashFlowEditor-save-btn"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    @click="onUpsertScenarioCashFlowRules"
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'
                    v-show="hasScenario">
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                    @click="onUpsertCashFlowRuleLibrary"
                    class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario">
                    Update Library
                </v-btn>                                       
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateCashFlowRuleLibraryDialog
            :dialogData="createCashFlowRuleLibraryDialogData"
            @submit="onSubmitCreateCashFlowRuleLibraryDialogResult"
        />

        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onSubmitCriterionLibraryEditorDialogResult"
        />

        <CashFlowRuleEditDialog            
            :showDialog="showRuleEditorDialog"
            :selectedCashFlowRule="selectedCashFlowRule"
            @submit="onSubmitCashFlowRuleEdit"
        />
        <ShareCashFlowRuleLibraryDialog :dialogData="shareCashFlowRuleLibraryDialogData"
            @submit="onShareCashFlowRuleDialogSubmit" 
        />
        <AddCashFlowRuleDialog
            :showDialog="showAddCashFlowRuleDialog"
            @submit="onSubmitAddCashFlowRule"/>
    </v-layout>
</template>

<script lang="ts" setup>
import Vue, { onBeforeUnmount } from 'vue';
import { ref, watch, nextTick, shallowRef, Ref } from 'vue';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    clone,
    find,
    isNil,
    propEq,
    any,
props,
} from 'ramda';
import {
    CashFlowDistributionRule,
    CashFlowRule,
    CashFlowRuleLibrary,
    CashFlowRuleLibraryUser,
    emptyCashFlowRule,
    emptyCashFlowRuleLibrary,
    emptyCashFlowRuleLibraryUsers
} from '@/shared/models/iAM/cash-flow';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { emptyShareCashFlowRuleLibraryDialogData, ShareCashFlowRuleLibraryDialogData } from '@/shared/models/modals/share-cash-flow-rule-data';
import {
    CreateCashFlowRuleLibraryDialogData,
    emptyCreateCashFlowLibraryDialogData,
} from '@/shared/models/modals/create-cash-flow-rule-library-dialog-data';
import CreateCashFlowRuleLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CreateCashFlowRuleLibraryDialog.vue';
import CashFlowRuleEditDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CashFlowRuleEditDialog.vue';
import AddCashFlowRuleDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/AddCashFlowRuleDialog.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import ShareCashFlowRuleLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/ShareCashFlowRuleLibraryDialog.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import CashFlowService from '@/services/cash-flow.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { LibraryUser } from '@/shared/models/iAM/user';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';


let store = useStore();
let stateCashFlowRuleLibraries = ref<CashFlowRuleLibrary[]>(store.state.cashFlowModule.cashFlowRuleLibraries);
let stateSelectedCashRuleFlowLibrary = ref<CashFlowRuleLibrary>(store.state.cashFlowModule.selectedCashFlowRuleLibrary);
let stateScenarioCashFlowRules = ref<CashFlowRule[]>(store.state.cashFlowModule.scenarioCashFlowRules);
let hasUnsavedChanges = ref<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess);
let hasPermittedAccess = ref<boolean>(store.state.cashFlowModule.hasPermittedAccess);
let isSharedLibrary = ref<boolean>(store.state.cashFlowModule.isSharedLibrary);

async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedCashFlowRuleLibrary');}
async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess');}
async function getCashFlowRuleLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCashFlowRuleLibraries');}
async function selectedCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectedCashFlowRuleLibrary');}
async function upsertCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertCashFlowRuleLibrary');}
async function deleteCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteCashFlowRuleLibrary');}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
async function getScenarioCashFlowRulesAction(payload?: any): Promise<any> {await store.dispatch('getScenarioCashFlowRules');}
async function upsertScenarioCashFlowRulesAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioCashFlowRules');}
async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification');}
async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario');}
async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario');}

function cashFlowRuleLibraryMutator(payload: any){store.commit('');}
function selectedCashFlowRuleLibraryMutator(payload: any){store.commit('');}

    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    let gridSearchTerm = '';
    let currentSearch = '';
    let updatedRowsMap:Map<string, [CashFlowRule, CashFlowRule]> = new Map<string, [CashFlowRule, CashFlowRule]>();//0: original value | 1: updated value
    let addedRows = ref<CashFlowRule[]>([]);
    let deletionIds = ref<string[]>([]);
    let rowCache: CashFlowRule[] = [];
    let pagination = ref<Pagination>(clone(emptyPagination));
    let currentPage = ref<CashFlowRule[]>([]);
    let isPageInit = false;
    let totalItems = 0;
    let initializing: boolean = true;
    let isShared: boolean = false;

    let shareCashFlowRuleLibraryDialogData: ShareCashFlowRuleLibraryDialogData = clone(emptyShareCashFlowRuleLibraryDialogData);

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue = shallowRef<string>('');
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue = shallowRef<string>('');

    let hasSelectedLibrary: boolean = false;
    let selectedScenarioId: any = getBlankGuid();
    let librarySelectItems: SelectItem[] = [];
    let selectedCashFlowRuleLibrary = ref<CashFlowRuleLibrary>(clone(emptyCashFlowRuleLibrary));
    let dateModified: string;

    const $router = useRouter();

    const cashFlowRuleGridHeaders: DataTableHeader[] = [
        {
            text: 'Rule Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '25%',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '65%',
        },
        {
            text: 'Action',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
    ];
    let cashFlowRuleGridData = ref<CashFlowRule[]>([]);
    let selectedCashRuleGridRows: CashFlowRule[] = [];
    let cashFlowRuleRadioBtnValue: string = '';
    let selectedCashFlowRule  = ref<CashFlowRule>(clone(emptyCashFlowRule));

    let selectedCashFlowRuleForCriteriaEdit: CashFlowRule = clone(
        emptyCashFlowRule,
    );
    const cashFlowRuleDistributionGridHeaders: DataTableHeader[] = [
        {
            text: 'Duration (yr)',
            value: 'durationInYears',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Cost Ceiling',
            value: 'costCeiling',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Yearly Distribution (%)',
            value: 'yearlyPercentages',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '4.2%',
        },
    ];
    let cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = [];
    let createCashFlowRuleLibraryDialogData: CreateCashFlowRuleLibraryDialogData = clone(
        emptyCreateCashFlowLibraryDialogData,
    );
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let inputRules: InputValidationRules = clone(rules);
    let uuidNIL: string = getBlankGuid();
    let hasScenario: boolean = false;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let showRuleEditorDialog: boolean = false;
    let showAddCashFlowRuleDialog: boolean = false;
    let importLibraryDisabled: boolean = true;
    let scenarioHasCreatedNew: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let libraryImported: boolean = false;

    created();
    function created() {
        librarySelectItemValue.value = "";
        getCashFlowRuleLibrariesAction().then(() => {
            getHasPermittedAccessAction().then(() => {
                if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.CashFlow) !== -1) {
                    selectedScenarioId = $router.currentRoute.value.query.scenarioId;
                    if (selectedScenarioId === uuidNIL) {
                        addErrorNotificationAction({
                            message: 'Unable to identify selected scenario.',
                        });
                        $router.push('/Scenarios/');
                    }

                    hasScenario = true;
                    getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                        selectScenarioAction({ scenarioId: selectedScenarioId });        
                        initializePages();
                    });                                                
                }
            });
        });
    }

    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(stateCashFlowRuleLibraries, () => onStateCashFlowRuleLibrariesChanged)
    function onStateCashFlowRuleLibrariesChanged(){
        librarySelectItems = stateCashFlowRuleLibraries.value.map(
            (library: CashFlowRuleLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

    watch(stateSelectedCashRuleFlowLibrary, () => onStateSelectedCashFlowRuleLibraryChanged)
    function onStateSelectedCashFlowRuleLibraryChanged(){
        selectedCashFlowRuleLibrary = clone(
            stateSelectedCashRuleFlowLibrary,
        );
    }

    watch(stateScenarioCashFlowRules, () => onStateScenarioCashFlowRulesChanged)
    function onStateScenarioCashFlowRulesChanged() {
        if (hasScenario) {
            cashFlowRuleGridData = clone(stateScenarioCashFlowRules);
        }
    }

    watch(selectedCashFlowRule, () => onSelectedSplitTreatmentIdChanged)
    function onSelectedSplitTreatmentIdChanged() {
        cashFlowDistributionRuleGridData = hasValue(
        selectedCashFlowRule.value.cashFlowDistributionRules,
    )
        ? clone(selectedCashFlowRule.value.cashFlowDistributionRules)
        : [];
    }
    watch(pagination, () => onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = pagination.value;
        const request: PagingRequest<CashFlowRule>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null && importLibraryDisabled ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            await CashFlowService.getScenarioCashFlowRulePage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                }
            });
        else if(hasSelectedLibrary)
             CashFlowService.getLibraryCashFlowRulePage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    if (!isNil(selectedCashFlowRuleLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedCashFlowRuleLibrary).then(() =>isShared = isSharedLibrary.value);
                    }

            }
        });     
    }

    watch(currentPage, () => onCurrentPageChanged)
    function onCurrentPageChanged() {
        // Get parent name from library id
        librarySelectItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = "Library Used: " + library.text;
            }
        });
    }

    watch(deletionIds, () => onDeletionIdsChanged)
    function onDeletionIdsChanged() {
        checkHasUnsavedChanges();
    }

    watch(addedRows, () => onAddedRowsChanged)
    function onAddedRowsChanged() {
        checkHasUnsavedChanges();
    }

    function onSelectCashFlowRule(id:string) {
        const cashFlowRule: CashFlowRule = find(
            propEq('id', id),
            currentPage.value,
        ) as CashFlowRule;

        if (hasValue(cashFlowRule)) {
            selectedCashFlowRule.value = clone(cashFlowRule);
        } else {
            selectedCashFlowRule.value = clone(emptyCashFlowRule);
        }

        showRuleEditorDialog = true;
    }

    function onShowCreateCashFlowRuleLibraryDialog(createAsNewLibrary: boolean) {
        createCashFlowRuleLibraryDialogData = {
            showDialog: true,
            cashFlowRules: createAsNewLibrary ? currentPage.value : [],
        };
    }

    function onSubmitCreateCashFlowRuleLibraryDialogResult(
        cashFlowRuleLibrary: CashFlowRuleLibrary,
    ) {
        createCashFlowRuleLibraryDialogData = clone(
            emptyCreateCashFlowLibraryDialogData,
        );

        if (!isNil(cashFlowRuleLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule> = {
                library: cashFlowRuleLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: cashFlowRuleLibrary.cashFlowRules.length == 0 || !hasSelectedLibrary ? null : selectedCashFlowRuleLibrary.value.id,
                    rowsForDeletion: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : deletionIds.value,
                    updateRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario ? selectedScenarioId : null
            }
            CashFlowService.upsertCashFlowRuleLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = cashFlowRuleLibrary.id;
                    
                    if(cashFlowRuleLibrary.cashFlowRules.length == 0){
                        clearChanges();
                    }

                    if(hasScenario){
                        scenarioHasCreatedNew = true;
                        importLibraryDisabled = true;
                    }
                    cashFlowRuleLibraryMutator(cashFlowRuleLibrary);
                    selectedCashFlowRuleLibraryMutator(cashFlowRuleLibrary.id);
                    addSuccessNotificationAction({message:'Added cash flow rule library'})
                }               
            });
        }
    }

    function onSubmitCashFlowRuleEdit(CashFlowDistributionRules:CashFlowDistributionRule[])
    {
        showRuleEditorDialog = false;
        if(!isNil(CashFlowDistributionRules))
        {
            let selectedRule = currentPage.value.find(o => o.id == selectedCashFlowRule.value.id) 
            if(!isNil(selectedRule))
            {
                selectedRule.cashFlowDistributionRules = hasValue(CashFlowDistributionRules) ? clone(CashFlowDistributionRules) : [];  
                onUpdateRow(selectedRule.id, clone(selectedRule))
                onPaginationChanged();
            }                
        }              
    }

    function onAddCashFlowRule() {
        const newCashFlowRule: CashFlowRule = {
            ...emptyCashFlowRule,
            name: `Unnamed Rule ${totalItems + 1}`,
            id: getNewGuid(),
        };
        addedRows.value.push(newCashFlowRule);
        onPaginationChanged()
    }

    function onSubmitAddCashFlowRule(newCashFlowRule: CashFlowRule){
        if(!isNil(newCashFlowRule))
        {
            addedRows.value.push(newCashFlowRule);
            onPaginationChanged()
        }
        showAddCashFlowRuleDialog = false;
    }

    function onDeleteCashFlowRule(cashFlowRuleId: string) {
        removeRowLogic(cashFlowRuleId);
        onPaginationChanged();
    }

    function onDeleteSelectedCashFlowRules() {
        selectedCashRuleGridRows.forEach(_ => {
            removeRowLogic(_.id);
        });
        onPaginationChanged();
    }

    function removeRowLogic(id: string){
        if(isNil(find(propEq('id', id), addedRows.value))){
            deletionIds.value.push(id);
            if(!isNil(updatedRowsMap.get(id)))
                updatedRowsMap.delete(id)
        }           
        else{          
            addedRows.value = addedRows.value.filter((row) => row.id !== id)
        }  
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedCashFlowRuleLibrary.value.owner) == getUserName();
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
            return getUserNameByIdGetter(selectedCashFlowRuleLibrary.value.owner);
        }
        return getUserName();
    }


    function onEditCashFlowRuleCriterionLibrary(cashFlowRule: CashFlowRule) {
        selectedCashFlowRuleForCriteriaEdit = clone(cashFlowRule);

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: selectedCashFlowRuleForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(
        criterionExpression: string,
    ) {
        criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression) && selectedCashFlowRuleForCriteriaEdit.id !== uuidNIL) {
            if(selectedCashFlowRuleForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedCashFlowRuleForCriteriaEdit.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedCashFlowRuleForCriteriaEdit.id, 
            {
                ...selectedCashFlowRuleForCriteriaEdit,
                criterionLibrary: {...selectedCashFlowRuleForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression},
            })
            onPaginationChanged();

            selectedCashFlowRuleForCriteriaEdit = clone(emptyCashFlowRule);
        }
    }

    function onEditSelectedLibraryListData(data: any, property: string) {
        switch (property) {
            case 'description':
                onUpdateRow(data.id, clone(data))
                onPaginationChanged();
                break;
        }
    }

    function onOpenCostCeilingEditDialog(distributionRuleId: string) {
        nextTick(() => {
            const editDialogInputElement: HTMLElement = document.getElementById(
                distributionRuleId,
            ) as HTMLElement;
            if (hasValue(editDialogInputElement)) {
                setTimeout(() => {
                    editDialogInputElement.blur();
                    setTimeout(() => editDialogInputElement.click());
                }, 250);
            }
        });
    }

    function onUpsertScenarioCashFlowRules() {
        if (selectedCashFlowRuleLibrary.value.id === uuidNIL || hasUnsavedChanges && libraryImported === false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        CashFlowService.upsertScenarioCashFlowRules({
            libraryId: selectedCashFlowRuleLibrary.value.id === uuidNIL ? null : selectedCashFlowRuleLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value;
                clearChanges();
                librarySelectItemValue.value = "";
                resetPage();
                addSuccessNotificationAction({message: "Modified scenario's cash flow rules"});
                importLibraryDisabled = true;
                libraryImported = false;
            }           
        });
    }

    function onUpsertCashFlowRuleLibrary() {
        // const cashFlowRuleLibrary: CashFlowRuleLibrary = {
        //     ...clone(selectedCashFlowRuleLibrary),
        //     cashFlowRules: clone(currentPage),
        // };

        const upsertRequest: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule> = {
                library: selectedCashFlowRuleLibrary.value,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedCashFlowRuleLibrary.value.id === uuidNIL ? null : selectedCashFlowRuleLibrary.value.id,
                rowsForDeletion: deletionIds.value,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                addedRows: addedRows.value,
                isModified: false
                },
                scenarioId: null
        }
        CashFlowService.upsertCashFlowRuleLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();
                cashFlowRuleLibraryMutator(selectedCashFlowRuleLibrary);
                selectedCashFlowRuleLibraryMutator(selectedCashFlowRuleLibrary.value.id);
                addSuccessNotificationAction({message: "Updated cash flow rule library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = '';
        parentLibraryName = loadedParentName;
        parentLibraryId = loadedParentId;

        setTimeout(() => {
            if (hasScenario) {
                clearChanges();
                resetPage();
                importLibraryDisabled = true;
            }
        });
    }

    function formatAsCurrency(value: any): any {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid = rows.every(
            (rule: CashFlowRule) => {
                const allSubDataIsValid = rule.cashFlowDistributionRules.every(
                    (
                        distributionRule: CashFlowDistributionRule,
                        index: number,
                    ) => {
                        let isValid: boolean =
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.durationInYears,
                            ) === true &&
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.costCeiling,
                            ) === true &&
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.yearlyPercentages,
                            ) === true &&
                            inputRules[
                                'cashFlowRules'
                            ].doesTotalOfPercentsEqualOneHundred(
                                distributionRule.yearlyPercentages,
                            ) === true;

                        if (index !== 0) {
                            isValid =
                                isValid &&
                                inputRules[
                                    'cashFlowRules'
                                ].isDurationGreaterThanPreviousDuration(
                                    distributionRule,
                                    rule,
                                ) === true &&
                                inputRules[
                                    'cashFlowRules'
                                ].isAmountGreaterThanOrEqualToPreviousAmount(
                                    distributionRule,
                                    rule,
                                ) === true;
                        }

                        return isValid;
                    },
                );

                return (
                    inputRules['generalRules'].valueIsNotEmpty(rule.name) ===
                        true && allSubDataIsValid
                );
            },
        );

        if (!hasScenario && hasSelectedLibrary) {
            return !(
                inputRules['generalRules'].valueIsNotEmpty(
                    selectedCashFlowRuleLibrary.value.name,
                ) === true && allDataIsValid
            );
        }
        disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    function onDeleteCashFlowRuleLibrary() {
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
            librarySelectItemValue.value = '';
            deleteCashFlowRuleLibraryAction(
                selectedCashFlowRuleLibrary.value.id,
            );
        }
    }

    //paging

    function onUpdateRow(rowId: string, updatedRow: CashFlowRule){
        if(any(propEq('id', rowId), addedRows.value)){
            const index = addedRows.value.findIndex(item => item.id == updatedRow.id)
            addedRows.value[index] = updatedRow;
            return;
        }

        let mapEntry = updatedRowsMap.get(rowId)

        if(isNil(mapEntry)){
            const row = rowCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
                updatedRowsMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            updatedRowsMap.delete(rowId)

        checkHasUnsavedChanges();
    }

    function clearChanges(){
        updatedRowsMap.clear();
        addedRows.value = [];
        deletionIds.value = [];
    }

    function resetPage(){
        pagination.value.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', stateSelectedCashRuleFlowLibrary, selectedCashFlowRuleLibrary))
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
        if (libraryId === "") {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: CashFlowRuleLibrary = emptyCashFlowRuleLibrary;
        stateCashFlowRuleLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }

    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue;
        if(!hasScenario || isNil(librarySelectItemValue))
        {    
            selectedCashFlowRuleLibraryAction(librarySelectItemValue);
        }
        else
        {
            if(!isNil(librarySelectItemValue) && !scenarioHasCreatedNew)
            {
                importLibraryDisabled = false;
            }

            scenarioHasCreatedNew = false;
        }

        setParentLibraryName(librarySelectItemValue.value);
        selectedCashFlowRuleLibraryAction(librarySelectItemValue);
        importLibraryDisabled = true;
        scenarioLibraryIsModified = false;
        libraryImported = true;
    }

    watch(librarySelectItemValue, () => onLibrarySelectItemValueChangedCheckUnsaved)
    function onLibrarySelectItemValueChangedCheckUnsaved() {
        if(hasScenario){
            onLibrarySelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged)
            CheckUnsavedDialog(onLibrarySelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue = trueLibrarySelectItemValue;               
            })
        librarySelectItemValueAllowedChanged = true;
        librarySelectItems.forEach(library => {
            if (library.value === librarySelectItemValue.value) {
                parentLibraryName = "Library Used: " + library.text;
            }
        });
    }

    watch(selectedCashFlowRuleLibrary, () => onSelectedCashFlowRuleLibraryChanged)
    function onSelectedCashFlowRuleLibraryChanged() {
        hasSelectedLibrary =
            selectedCashFlowRuleLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }
        initializing = false;

        if(hasSelectedLibrary)
            onPaginationChanged();
    }

    watch(isSharedLibrary, () => onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
        if (!isNil(selectedCashFlowRuleLibrary)) {
            selectedCashFlowRuleLibrary.value.isShared = isShared;
        } 
    }

    function initializePages(){
        const request: PagingRequest<CashFlowRule>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            CashFlowService.getScenarioCashFlowRulePage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value);
                    totalItems = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName; //store original
                    scenarioLibraryIsModified = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
    }

    function onShowShareCashFlowRuleLibraryDialog(cashFlowRuleLibrary: CashFlowRuleLibrary) {
        shareCashFlowRuleLibraryDialogData = {
            showDialog:true,
            cashFlowRuleLibrary: clone(cashFlowRuleLibrary)
        }
    }

    function onShareCashFlowRuleDialogSubmit(cashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[]) {
        shareCashFlowRuleLibraryDialogData = clone(emptyShareCashFlowRuleLibraryDialogData);

        if (!isNil(cashFlowRuleLibraryUsers) && selectedCashFlowRuleLibrary.value.id !== getBlankGuid())
        {
            let libraryUserData: LibraryUser[] = [];

            //create library users
            cashFlowRuleLibraryUsers.forEach((cashFlowRuleLibraryUser, index) =>
            {   
                //determine access level
                let libraryUserAccessLevel: number = 0;
                if (libraryUserAccessLevel == 0 && cashFlowRuleLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                if (libraryUserAccessLevel == 0 && cashFlowRuleLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                //create library user object
                let libraryUser: LibraryUser = {
                    userId: cashFlowRuleLibraryUser.userId,
                    userName: cashFlowRuleLibraryUser.username,
                    accessLevel: libraryUserAccessLevel
                }

                //add library user to an array
                libraryUserData.push(libraryUser);
            });
            if (!isNil(selectedCashFlowRuleLibrary.value.id)) {
                getIsSharedLibraryAction(selectedCashFlowRuleLibrary).then(() => isShared = isSharedLibrary.value);
            }
            //update budget library sharing
            CashFlowService.upsertOrDeleteCashFlowRuleLibraryUsers(selectedCashFlowRuleLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                {
                    resetPage();
                }
            });
        }
    }
</script>

<style>
.cash-flow-library-tables {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.cash-flow-library-tables .v-menu--inline {
    width: 100%;
}

.cash-flow-library-tables .v-menu__activator a,
.cash-flow-library-tables .v-menu--inline input {
    width: 100%;
}

.cash-flow-radio-group .v-input--radio-group__input {
    padding-top: 25px;
}

.output {
    border-bottom: 1px solid;
}

.cash-flow-library-card {
    height: 330px;
    overflow-y: auto;
    overflow-x: hidden;
}

.invalid-input {
    color: red;
}

.amount-div {
    width: 208px;
}

.split-treatment-limit-currency-input {
    border: 1px solid;
    width: 100%;
}

.split-treatment-limit-amount-rule-span {
    font-size: 0.8em;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
