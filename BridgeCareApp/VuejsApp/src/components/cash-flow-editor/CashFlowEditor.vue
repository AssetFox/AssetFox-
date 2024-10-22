<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col cols="12">
            <v-row align="center" justify="space-between">
                <v-col cols = "auto" >
                    <div style="margin-bottom: 10px;">
                        <v-subheader class="ghd-md-gray ghd-control-label">Select a Cash Flow Library</v-subheader>
                    </div>
                    <v-select
                        :items="sortAlphabetically(librarySelectItems)"
                        id="CashFlowEditor-SelectLibrary-vselect"
                        variant="outlined"
                        v-model="librarySelectItemValue"
                        menu-icon=custom:GhdDownSvg
                        item-title="text"
                        item-value="value"
                        class="ghd-select ghd-text-field ghd-text-field-border" density="compact">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader-library-used budget-parent" v-if='hasScenario'><b>{{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-col>
                <v-col cols = "auto" >    
                    <v-row v-show='hasSelectedLibrary || hasScenario'>
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content" style="padding-top: 7px">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <!-- <v-divider class="owner-shared-divider" inset vertical
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                        </v-divider> -->
                        <ShareLibraryButton 
                            @shareLibrary="onShowShareCashFlowRuleLibraryDialog(selectedCashFlowRuleLibrary)"
                            :show="!hasScenario"
                        />
                    </v-row>  
                </v-col>
                <v-col cols = "auto" class="ghd-constant-header">   
                    <v-btn @click="showAddCashFlowRuleDialog = true" v-show="hasSelectedLibrary || hasScenario"
                        id="CashFlowEditor-addCashFlowRule-btn" 
                        variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                        Add Cash Flow Rule
                    </v-btn>
                    <CreateNewLibraryButton 
                        @createNewLibrary="onShowCreateCashFlowRuleLibraryDialog(false)"
                        :show="!hasScenario"
                        style="margin-left: 10px"
                    />
                </v-col>
            </v-row>
        </v-col>
        <v-col v-show="hasSelectedLibrary || hasScenario" cols="12">
                <v-data-table-server
                    id="CashFlowEditor-cashFlowRules-table"
                    :headers="cashFlowRuleGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    sort-asc-icon="custom:GhdTableSortAscSvg"
                    sort-desc-icon="custom:GhdTableSortDescSvg"
                    v-model='selectedCashRuleGridRows'
                    class="ghd-table v-table__overflow"
                    item-key="id"
                    return-object
                    show-select               
                    :items-length="totalItems"
                    :rows-per-page-items=[5,10,25]
                    :items-per-page-options="[
                        {value: 5, title: '5'},
                        {value: 10, title: '10'},
                        {value: 25, title: '25'},
                    ]"
                    v-model:sort-by="pagination.sort"
                    v-model:page="pagination.page"
                    v-model:items-per-page="pagination.rowsPerPage"
                    @update:options="onPaginationChanged">
                    <template v-slot:item="item" slot="items" slot-scope="props">
                        <tr>
                        <td>
                            <v-checkbox hide-details primary v-model="selectedCashRuleGridRows" :value="item.item"></v-checkbox>
                        </td>
                        <td>
                            <editDialog
                                v-model:return-value="item.item.name"
                                size="large"
                                lazy
                                @save="onEditSelectedLibraryListData(item.item,'description')"
                                >
                                <v-text-field
                                    id="CashFlowEditor-ruleName-text"
                                    readonly
                                    variant="underlined"
                                    single-line
                                    class="sm-txt"
                                    :model-value="item.item.name"
                                    :rules="[inputRules.generalRules.valueIsNotEmpty]"/>
                                <template v-slot:input>
                                    <v-textarea
                                    id="CashFlowEditor-editRuleName-textarea"
                                        label="Description"
                                        no-resize
                                        outline
                                        rows="5"
                                        variant="outlined"
                                        :rules="[inputRules.generalRules.valueIsNotEmpty]"
                                        v-model="item.item.name"/>
                                </template>
                            </editDialog>
                        </td>
                        <td>
                            <v-row align-center style='flex-wrap:nowrap'>
                                <v-menu
                                location="bottom">
                                <template v-slot:activator>
                                    <v-text-field
                                    variant="underlined"
                                        id="CashFlowEditor-criteria-text"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :model-value="item.item.criterionLibrary.mergedCriteriaExpression"/>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            :model-value="
                                                item.item.criterionLibrary.mergedCriteriaExpression"
                                            full-width
                                            no-resize
                                            outline
                                            readonly
                                            rows="5"
                                            style = "min-width: 500px;min-height: 205px;"/>
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                            <v-btn
                                @click="onEditCashFlowRuleCriterionLibrary(item.item)"
                                id="CashFlowEditor-editCashFlowRule-btn"
                                class="ghd-green"
                                style="margin-top: 12px;"
                                flat
                                icon>
                                <EditSvg />
                            </v-btn>
                            </v-row>
                        </td>
                        <td>
                            <v-row>
                                <v-btn
                                @click="onDeleteCashFlowRule(item.item.id)"
                                id="CashFlowEditor-deleteCashFlowRule-btn"
                                class="ghd-red"
                                flat
                                icon>
                                <TrashCanSvg />
                            </v-btn>
                            <v-btn
                                @click="onSelectCashFlowRule(item.item.id)"
                                id="CashFlowEditor-editCashFlowRuleDistribution-btn"
                                class="ghd-blue"
                                variant="flat"
                                icon>
                                <img class='img-general' :src="getUrl('assets/icons/edit-cash.svg')"/>
                            </v-btn>
                            </v-row>                          
                        </td>
                        </tr>
                    </template>
                </v-data-table-server>
                <DeleteSelectedButton 
                    @deleteSelected="onDeleteSelectedCashFlowRules"
                    :disabled='selectedCashRuleGridRows.length === 0'
                />
        </v-col>
        <v-col v-show="hasSelectedLibrary && !hasScenario" cols="12">
            <v-row justify-center>
                <v-col>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea
                        class="ghd-text-field-border"
                        no-resize
                        variant="outlined"
                        rows="4"
                        v-model="selectedCashFlowRuleLibrary.description"
                        @update:model-value="checkHasUnsavedChanges()">

                    </v-textarea>
                </v-col>
            </v-row>
        </v-col>
        <v-col cols = "12">
            <v-row justify="center" style="padding-bottom: 40px;" v-show="hasSelectedLibrary || hasScenario">
                <DeleteLibraryButton 
                    @deleteLibrary="onDeleteCashFlowRuleLibrary"
                    :disabled="!hasLibraryEditPermission"
                    :show="!hasScenario"
                />
                <CancelButton 
                    @cancel="onDiscardChanges"
                    :disabled="!hasUnsavedChanges"
                    :show="hasScenario"
                />
                <CreateAsNewLibraryButton 
                    @createAsNewLibrary="onShowCreateCashFlowRuleLibraryDialog(true)"
                    :disabled="disableCrudButtons()"
                />
                <SaveButton 
                    @save="onUpsertScenarioCashFlowRules"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    :show="hasScenario"
                />
                <UpdateLibraryButton 
                    @updateLibrary="onUpsertCashFlowRuleLibrary"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                    :show="!hasScenario"
                />                                     
            </v-row>
        </v-col>

        <Alert
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
        <ConfirmDialog></ConfirmDialog>
    </v-row>
    </v-card>
</template>

<script lang="ts" setup>
import { watch, nextTick, shallowRef, onBeforeUnmount, ref, computed, onMounted, onBeforeMount} from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
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
import ConfirmDialog from 'primevue/confirmdialog';
import { getUrl } from '@/shared/utils/get-url';
import { useConfirm } from 'primevue/useconfirm';
import { sortSelectItemsAlphabetically } from '@/shared/utils/sorter-utils'
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';
import { inject } from 'vue';
import mitt, { Emitter, EventType } from 'mitt';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import CreateAsNewLibraryButton from '@/shared/components/buttons/CreateAsNewLibraryButton.vue';
import UpdateLibraryButton from '@/shared/components/buttons/UpdateLibraryButton.vue';
import DeleteLibraryButton from '@/shared/components/buttons/DeleteLibraryButton.vue';
import CreateNewLibraryButton from '@/shared/components/buttons/CreateNewLibraryButton.vue';
import ShareLibraryButton from '@/shared/components/buttons/ShareLibraryButton.vue';
import DeleteSelectedButton from '@/shared/components/buttons/DeleteSelectedButton.vue';


let store = useStore();
const confirm = useConfirm();
const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>
// const stateSimulationReportNames = computed<string[]>(() => store.state.adminDataModule.simulationReportNames);

const stateCashFlowRuleLibraries = computed<CashFlowRuleLibrary[]>(() => store.state.cashFlowModule.cashFlowRuleLibraries);
let stateSelectedCashRuleFlowLibrary = computed<CashFlowRuleLibrary>(() => store.state.cashFlowModule.selectedCashFlowRuleLibrary);
let stateScenarioCashFlowRules = computed<CashFlowRule[]>(() => store.state.cashFlowModule.scenarioCashFlowRules);
let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
let hasPermittedAccess = computed<boolean>(() => store.state.cashFlowModule.hasPermittedAccess);
let isSharedLibrary = computed<boolean>(() => store.state.cashFlowModule.isSharedLibrary);

async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedCashFlowRuleLibrary', payload);}
async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess', payload);}
async function getCashFlowRuleLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCashFlowRuleLibraries', payload);}
async function selectedCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectCashFlowRuleLibrary', payload);}
async function upsertCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertCashFlowRuleLibrary', payload);}
async function deleteCashFlowRuleLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteCashFlowRuleLibrary', payload);}
function addErrorNotificationAction(payload?: any) { store.dispatch('addErrorNotification', payload);}
function setHasUnsavedChangesAction(payload?: any) { store.dispatch('setHasUnsavedChanges', payload);}
async function getScenarioCashFlowRulesAction(payload?: any): Promise<any> {await store.dispatch('getScenarioCashFlowRules', payload);}
async function upsertScenarioCashFlowRulesAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioCashFlowRules', payload);}
function addSuccessNotificationAction(payload?: any) { store.dispatch('addSuccessNotification', payload);}
async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario', payload);}
 function selectScenarioAction(payload?: any) { store.dispatch('selectScenario', payload);}

function cashFlowRuleLibraryMutator(payload: any){store.commit('cashFlowRuleLibraryMutator', payload);}
function selectedCashFlowRuleLibraryMutator(payload: any){store.commit('selectedCashFlowRuleLibraryMutator', payload);}

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
    let totalItems = ref(0);
    let initializing: boolean = true;
    let isShared: boolean = false;

    let shareCashFlowRuleLibraryDialogData = ref(clone(emptyShareCashFlowRuleLibraryDialogData));

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue = '';
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue = ref<string>('');

    let hasSelectedLibrary = ref(false);
    let selectedScenarioId: any = getBlankGuid();
    let librarySelectItems  = ref<SelectItem[]>([]);
    let selectedCashFlowRuleLibrary = ref<CashFlowRuleLibrary>(clone(emptyCashFlowRuleLibrary));
    let dateModified = ref<string>();

    const $router = useRouter();

    const cashFlowRuleGridHeaders: any[] = [
        {
            title: 'Rule Name',
            key: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '25%',
        },
        {
            title: 'Criteria',
            key: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '65%',
        },
        {
            title: 'Action',
            key: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
    ];
    let cashFlowRuleGridData = ref<CashFlowRule[]>([]);
    let selectedCashRuleGridRows = ref<CashFlowRule[]>([]);
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
    let createCashFlowRuleLibraryDialogData = ref(clone(
        emptyCreateCashFlowLibraryDialogData,
    ));
    let criterionEditorDialogData = ref(clone(
        emptyGeneralCriterionEditorDialogData,
    ));
    let confirmDeleteAlertData = ref(clone(emptyAlertData));
    let inputRules: InputValidationRules = clone(rules);
    let uuidNIL: string = getBlankGuid();
    let hasScenario = ref(false);
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let showRuleEditorDialog = ref(false);
    let showAddCashFlowRuleDialog= ref(false);
    let importLibraryDisabled: boolean = true;
    let scenarioHasCreatedNew: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let parentLibraryName = ref('None');
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let libraryImported: boolean = false;

    created();
    async function created() {
        librarySelectItemValue.value = "";
        await getCashFlowRuleLibrariesAction()
        await getHasPermittedAccessAction()
        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.CashFlow) !== -1) {
            selectedScenarioId = $router.currentRoute.value.query.scenarioId;
            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Unable to identify selected scenario.',
                });
                $router.push('/Scenarios/');
                return;
            }

            hasScenario.value = true;
            await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId})     
            selectScenarioAction({ scenarioId: selectedScenarioId });        
            await initializePages();                                   
        }
    }


    onBeforeUnmount(() => {
        setHasUnsavedChangesAction({ value: false });
    });

    watch(stateCashFlowRuleLibraries, () => {
        librarySelectItems.value = stateCashFlowRuleLibraries.value.map(
            (library: CashFlowRuleLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    });

    watch(stateSelectedCashRuleFlowLibrary, () => {
        selectedCashFlowRuleLibrary.value = clone(
            stateSelectedCashRuleFlowLibrary.value,
        );
    });

    watch(stateScenarioCashFlowRules, () => {
        if (hasScenario.value) {
            cashFlowRuleGridData.value = clone(stateScenarioCashFlowRules.value);
        }
    });

    watch(selectedCashFlowRule, () => {
        cashFlowDistributionRuleGridData = hasValue(
        selectedCashFlowRule.value.cashFlowDistributionRules,
    )
        ? clone(selectedCashFlowRule.value.cashFlowDistributionRules)
        : [];
    });

    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();

        const { sort, descending, page, rowsPerPage } = pagination.value;
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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL)
             CashFlowService.getScenarioCashFlowRulePage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                }
            });
        else if(hasSelectedLibrary.value)
            await CashFlowService.getCashLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified.value = data.slice(0, 10);
                   }
             }),
             await CashFlowService.getLibraryCashFlowRulePage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    if (!isNil(selectedCashFlowRuleLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedCashFlowRuleLibrary.value).then(() =>isShared = isSharedLibrary.value);
                    }
            }
        });     
    }

    watch(currentPage, () => onCurrentPageChanged())
    function onCurrentPageChanged() {
        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName.value = "Library Used: " + library.text;
            }
        });
    }

    watch(deletionIds, () => onDeletionIdsChanged())
    function onDeletionIdsChanged() {
        checkHasUnsavedChanges();
    }

    watch(addedRows, () => onAddedRowsChanged())
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

        showRuleEditorDialog.value = true;
    }

    function onShowCreateCashFlowRuleLibraryDialog(createAsNewLibrary: boolean) {
        createCashFlowRuleLibraryDialogData.value = {
            showDialog: true,
            cashFlowRules: createAsNewLibrary ? currentPage.value : [],
        };
    }

    function sortAlphabetically(items: SelectItem[]) {
        return sortSelectItemsAlphabetically(items);
    }

    function onSubmitCreateCashFlowRuleLibraryDialogResult(
        cashFlowRuleLibrary: CashFlowRuleLibrary,
    ) {
        createCashFlowRuleLibraryDialogData.value = clone(
            emptyCreateCashFlowLibraryDialogData,
        );

        if (!isNil(cashFlowRuleLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<CashFlowRuleLibrary, CashFlowRule> = {
                library: cashFlowRuleLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: cashFlowRuleLibrary.cashFlowRules.length == 0 || !hasSelectedLibrary.value ? null : selectedCashFlowRuleLibrary.value.id,
                    rowsForDeletion: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : deletionIds.value,
                    updateRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: cashFlowRuleLibrary.cashFlowRules.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            CashFlowService.upsertCashFlowRuleLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    
                    if(cashFlowRuleLibrary.cashFlowRules.length == 0){
                        clearChanges();
                    }
                    cashFlowRuleLibraryMutator(cashFlowRuleLibrary);
                    if(hasScenario.value){
                        scenarioHasCreatedNew = true;
                        importLibraryDisabled = true;
                    }                   
                    else
                        librarySelectItemValue.value = cashFlowRuleLibrary.id;
                    addSuccessNotificationAction({message:'Added cash flow rule library'})
                }               
            });
        }
    }

    function onSubmitCashFlowRuleEdit(CashFlowDistributionRules:CashFlowDistributionRule[])
    {
        showRuleEditorDialog.value = false;
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
            name: `Unnamed Rule ${totalItems.value + 1}`,
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
        showAddCashFlowRuleDialog.value = false;
    }

    function onDeleteCashFlowRule(cashFlowRuleId: string) {
        removeRowLogic(cashFlowRuleId);
        onPaginationChanged();
    }

    function onDeleteSelectedCashFlowRules() {
        selectedCashRuleGridRows.value.forEach(_ => {
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

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: selectedCashFlowRuleForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(
        criterionExpression: string,
    ) {
        criterionEditorDialogData.value = clone(
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

    async function onUpsertScenarioCashFlowRules() {
        if (selectedCashFlowRuleLibrary.value.id === uuidNIL || hasUnsavedChanges.value && libraryImported === false) {scenarioLibraryIsModified = true;}
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
        $emitter.emit('CashFlowUpdated');
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
                cashFlowRuleLibraryMutator(selectedCashFlowRuleLibrary.value);
                selectedCashFlowRuleLibraryMutator(selectedCashFlowRuleLibrary.value.id);
                addSuccessNotificationAction({message: "Updated cash flow rule library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = '';
        parentLibraryName.value = loadedParentName;
        parentLibraryId = loadedParentId;

        setTimeout(() => {
            if (hasScenario.value) {
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

        if (!hasScenario.value && hasSelectedLibrary.value) {
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
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedCashFlowRuleLibrary.value,  stateSelectedCashRuleFlowLibrary.value))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        if (hasUnsavedChanges.value && unsavedDialogAllowed) {

            confirm.require({
                message: "You have unsaved changes. Are you sure you wish to continue?",
                acceptLabel: "Continue",
                rejectLabel: "Close",
                accept: ()=>next(),
                reject: ()=>otherwise()
            });
        } 
        else {
            unsavedDialogAllowed = true;
            next();
        }
    };

    function setParentLibraryName(libraryId: string) {
        if (libraryId === "" || libraryId === uuidNIL) {
            parentLibraryName.value = "None";
            return;
        }
        let foundLibrary: CashFlowRuleLibrary = emptyCashFlowRuleLibrary;
        stateCashFlowRuleLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value;
        if(!hasScenario.value || !isNil(librarySelectItemValue.value))
        {    
            selectedCashFlowRuleLibraryAction(librarySelectItemValue.value);
        }
        else
        {
            if(!isNil(librarySelectItemValue.value) && !scenarioHasCreatedNew)
            {
                importLibraryDisabled = false;
            }

            scenarioHasCreatedNew = false;
        }

        setParentLibraryName(librarySelectItemValue.value);
        selectedCashFlowRuleLibraryAction(librarySelectItemValue.value);
        importLibraryDisabled = true;
        scenarioLibraryIsModified = false;
        libraryImported = true;
    }

    watch(librarySelectItemValue, () => onLibrarySelectItemValueChangedCheckUnsaved())
    function onLibrarySelectItemValueChangedCheckUnsaved() {
        if(hasScenario.value){
            onLibrarySelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged)
            CheckUnsavedDialog(onLibrarySelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = trueLibrarySelectItemValue;               
            })
        librarySelectItemValueAllowedChanged = true;
        librarySelectItems.value.forEach(library => {
            if (library.value === librarySelectItemValue.value) {
                parentLibraryName.value = "Library Used: " + library.text;
            }
        });
    }

    watch(selectedCashFlowRuleLibrary, () => onSelectedCashFlowRuleLibraryChanged())
    function onSelectedCashFlowRuleLibraryChanged() {
        hasSelectedLibrary.value =
            selectedCashFlowRuleLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }
        initializing = false;
        clearChanges();

        if(hasSelectedLibrary.value)
            onPaginationChanged();
    }

    watch(isSharedLibrary, () => onStateSharedAccessChanged())
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
        if (!isNil(selectedCashFlowRuleLibrary.value)) {
            selectedCashFlowRuleLibrary.value.isShared = isShared;
        } 
    }

    async function initializePages(){
        
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
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL) {
            await CashFlowService.getScenarioCashFlowRulePage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<CashFlowRule>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value);
                    totalItems.value = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName.value; //store original
                    scenarioLibraryIsModified = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
        }
    }

    function onShowShareCashFlowRuleLibraryDialog(cashFlowRuleLibrary: CashFlowRuleLibrary) {
        shareCashFlowRuleLibraryDialogData.value = {
            showDialog:true,
            cashFlowRuleLibrary: clone(cashFlowRuleLibrary)
        }
    }

    function onShareCashFlowRuleDialogSubmit(cashFlowRuleLibraryUsers: CashFlowRuleLibraryUser[]) {
        shareCashFlowRuleLibraryDialogData.value = clone(emptyShareCashFlowRuleLibraryDialogData);

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
                getIsSharedLibraryAction(selectedCashFlowRuleLibrary.value).then(() => isShared = isSharedLibrary.value);
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
