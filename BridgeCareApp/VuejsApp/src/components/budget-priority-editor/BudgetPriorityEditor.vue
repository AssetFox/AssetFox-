<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col cols="12">
            <v-row align="center" justify="space-between">              
                <v-col cols ="auto">   
                    <div style="margin-bottom: 10px;">              
                        <v-subheader class="ghd-md-gray ghd-control-label">Budget Priority Library</v-subheader>
                    </div> 
                    <v-select id="BudgetPriorityEditor-library-vselect"
                        :items='librarySelectItems'  
                        menu-icon=custom:GhdDownSvg                          
                        item-title="text"
                        item-value="value" 
                        v-model='librarySelectItemValue'
                        append-icon=ghd-down
                        variant="outlined"
                        density="compact"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>    
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>                       
                </v-col>
                <v-col cols = "auto">
                    <v-row v-show='hasSelectedLibrary || hasScenario'>
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content owner-padding" style="margin-top:6px;">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <!-- <v-divider class="owner-shared-divider" vertical
                            v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                        </v-divider> -->
                        <v-btn id="BudgetPriorityEditor-shareLibrary-vbtn" @click='onShowShareBudgetPriorityLibraryDialog(selectedBudgetPriorityLibrary)'
                             style="margin: 10px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>                               
                </v-col>
                <v-col cols = "auto">
                        <!-- <v-spacer></v-spacer> -->
                        <v-btn id="BudgetPriorityEditor-addBudgetPriority-vbtn" @click='showCreateBudgetPriorityDialog = true' style="margin: 5px;" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show='hasSelectedLibrary || hasScenario'>Add Budget Priority</v-btn>
                        
                        <v-btn id="BudgetPriorityEditor-createNewLibrary-vbtn" @click='onShowCreateBudgetPriorityLibraryDialog(false)' style="margin: 5px;" variant = "outlined"
                            v-show='!hasScenario' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' > 
                            Create New Library
                        </v-btn>
                </v-col>
            </v-row>
        </v-col>

        <v-col v-show='hasSelectedLibrary || hasScenario' cols="12">
                <v-data-table-server 
                    id = "BudgetPriority-priorities-vdatatable"
                    :headers='budgetPriorityGridHeaders' 
                    :items='budgetPriorityGridRows'
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :rows-per-page-items=[5,10,25]
                    :items-length="totalItems"
                    :items-per-page-options="[
                        {value: 5, title: '5'},
                        {value: 10, title: '10'},
                        {value: 25, title: '25'},
                    ]"
                    item-key="id"
                    show-select
                    return-object 
                    class='v-table__overflow ghd-table' :item-value="'id'"
                    sort-asc-icon="custom:GhdTableSortAscSvg"
                    sort-desc-icon="custom:GhdTableSortDescSvg"                           
                    v-model='selectedBudgetPriorityGridRows'                                       
                    v-model:sort-by="pagination.sort"
                    v-model:page="pagination.page"
                    v-model:items-per-page="pagination.rowsPerPage"
                    @update:options="onPaginationChanged">

                    <template v-slot:item="item">
                        <tr>
                            <td>
                                <v-checkbox id="BudgetPriorityEditor-deleteBudgetPriority-vcheckbox" hide-details primary  v-model="selectedBudgetPriorityGridRows" :value="item.item"></v-checkbox>
                            </td>
                            <td v-for='header in budgetPriorityGridHeaders'>
                                <div v-if="header.key === 'priorityLevel' || header.key === 'year'">
                                    <editDialog
                                        v-model:return-value='item.item[header.key]'
                                        @save='onEditBudgetPriority(item.item, header.key, item.item[header.key])'
                                        size="large" lazy>
                                        <v-text-field v-if="header.key === 'priorityLevel'" readonly single-line
                                            variant="underlined"
                                            class='sm-txt'
                                            :model-value="item.item[header.key]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]" 
                                            />
                                        <v-text-field style="width: 75px;"
                                            v-else readonly single-line class='sm-txt'
                                            variant="underlined"
                                            :model-value='item.item[header.key]' 
                                        />
                                        <template v-slot:input>
                                            <v-text-field v-if="header.key === 'priorityLevel'" label='Edit' single-line
                                                        v-model.number='item.item[header.key]'
                                                        :mask="'##########'"
                                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-else label='Edit' single-line :mask="'####'"
                                                        v-model.number='item.item[header.key]' />
                                        </template>
                                    </editDialog>
                                </div>
                                <div v-else-if="header.key === 'criteria'">
                                    <v-row>
                                        <v-menu >
                                            <template v-slot:activator="{ props }">
                                                    <div style="display: flex; align-items: center;" v-if='stateScenarioSimpleBudgetDetails.length > 5'>
                                                        <v-btn class='ara-blue ghd-button-text' v-bind="props" flat>
                                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')"/>
                                                        </v-btn>
                                                    </div>
                                                    <div v-else class='priority-criteria-output' style="width: 1000px;">
                                                        <v-text-field readonly single-line class='sm-txt' variant="underlined"
                                                                    :model-value='item.item.criteria' />                                        
                                                    </div>
                                            </template>
                                            <v-card>
                                                <v-card-text>
                                                    <v-textarea :model-value='item.item.criteria' full-width no-resize outline
                                                                readonly rows='5' />
                                                </v-card-text>
                                            </v-card>
                                        </v-menu>
                                    </v-row>
                                </div>
                                <div v-else-if="header.title.endsWith('%')">
                                    <editDialog
                                        v-model:return-value='item.item[header.key]'
                                        @save='onEditBudgetPercentagePair(item.item, header.key, item.item[header.key])'
                                        size="large" lazy>
                                        <v-text-field readonly single-line class='sm-txt' :model-value="item.item[header.key]" variant="underlined"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(item.item[header.key], [0, 100])]" />
                                        <template v-slot:input>
                                            <v-text-field :mask="'###'" label='Edit' single-line
                                                        v-model="item.item[header.key]"
                                                        :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(item.item[header.key], [0, 100])]" />
                                        </template>
                                    </editDialog>
                                </div>
                                <div v-else>
                                    <v-btn id="BudgetPriorityEditor-editCriteria-vbtn" @click='onShowCriterionLibraryEditorDialog(item.item)' flat>
                                            <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                                        </v-btn>
                                    <v-btn  id="BudgetPriorityEditor-deleteBudgetPriority-btn" @click="onRemoveBudgetPriority(item.item.id)" flat>
                                        <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </div>
                            </td>
                        </tr>
                    </template>
                </v-data-table-server>
            <v-btn flat
                :disabled='selectedBudgetPriorityIds.length === 0'
                @click='onRemoveBudgetPriorities'
                class='ghd-blue ghd-button'
                variant="text">
                Delete Selected
            </v-btn>
        </v-col>
        <v-divider
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
        <v-col cols="12" v-show='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
            <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
            <v-textarea class="ghd-control-text ghd-control-border" variant="outlined" density="compact"
                        v-model='selectedBudgetPriorityLibrary.description'
                        @update:model-value="checkHasUnsavedChanges()">
            </v-textarea>
        </v-col>
        <v-col cols = "12">   
            <div v-if="!hasUniquePriorityLevels()" class="priorities-error">
                Each item must have a unique priority level.
            </div>        
            <v-row style="padding-bottom: 40px;" justify="center" v-show='hasSelectedLibrary || hasScenario'>
                <v-btn  variant = "flat" @click='onDiscardChanges'
                       v-show='hasScenario' :disabled='!hasUnsavedChanges' style="margin: 5px;" class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn id="BudgetPriorityEditor-createAsNewLibrary-vbtn" @click='onShowCreateBudgetPriorityLibraryDialog(true)' style="margin: 5px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                       :disabled='disableCrudButtons()'>
                    Create as New Library
                </v-btn>
                <v-btn @click='onUpsertScenarioBudgetPriorities'
                       class='ghd-blue-bg text-white ghd-button-text ghd-button' style="margin: 5px;"
                       v-show='hasScenario' :disabled='disableCrudButtonsResult || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn id="BudgetPriorityEditor-deleteLibrary-vbtn"  @click='onShowConfirmDeleteAlert' variant = "outlined" style="margin: 5px;"
                       v-show='!hasScenario' :disabled='!hasSelectedLibrary' class='ghd-blue ghd-button-text ghd-button'>
                    Delete Library
                </v-btn>             
                <v-btn id="BudgetPriorityEditor-updateLibrary-vbtn" @click='onUpsertBudgetPriorityLibrary'
                       class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button' style="margin: 5px;"
                       v-show='!hasScenario' :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'>
                    Update Library
                </v-btn>
            </v-row>
        </v-col>
        <ShareBudgetPriorityLibraryDialog 
            :dialogData='shareBudgetPriorityLibraryDialogData' 
            @submit='onShareBudgetPriorityLibraryDialogSubmit'
        />
        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onSubmitConfirmDeleteAlertResult' />

        <CreatePriorityLibraryDialog :dialogData='createBudgetPriorityLibraryDialogData'
                                     @submit='onSubmitCreateBudgetPriorityLibraryDialogResult' />

        <CreatePriorityDialog :showDialog='showCreateBudgetPriorityDialog' @submit='onAddBudgetPriority' />

        <GeneralCriterionEditorDialog :dialogData='criterionEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
        <ConfirmDialog></ConfirmDialog>
    </v-row>
</v-card>
</template>

<script setup lang='ts'>
    import { ref, watch, shallowReactive, onBeforeUnmount, ShallowRef, shallowRef, computed } from 'vue';
    import editDialog from '@/shared/modals/Edit-Dialog.vue'
    import {
        BudgetPercentagePair,
        BudgetPriority,
        BudgetPriorityGridDatum,
        BudgetPriorityLibrary,
        BudgetPriorityLibraryUser,
        emptyBudgetPriority,
        emptyBudgetPriorityLibrary,
        emptyBudgetPriorityLibraryUsers
    } from '@/shared/models/iAM/budget-priority';
    import { any, clone, contains, find, findIndex, isNil, prepend, propEq, update } from 'ramda';
        import { hasValue } from '@/shared/utils/has-value-util';
    import { getPropertyValues } from '@/shared/utils/getter-utils';
    import { setItemPropertyValue } from '@/shared/utils/setter-utils';
    import { SelectItem } from '@/shared/models/vue/select-item';
    import {
        CreateBudgetPriorityLibraryDialogData,
        emptyCreateBudgetPriorityLibraryDialogData,
    } from '@/shared/models/modals/create-budget-priority-library-dialog-data';
    import {
        ShareBudgetPriorityLibraryDialogData,
        emptyShareBudgetPriorityLibraryDialogData
    } from '@/shared/models/modals/share-budget-priority-library-dialog-data';   
    import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
    import ConfirmDeleteAlert from '@/shared/modals/Alert.vue';
    import { hasUnsavedChangesCore, isEqual, sortNonObjectLists } from '@/shared/utils/has-unsaved-changes-helper';
    import { InputValidationRules, rules as validationRules } from '@/shared/utils/input-validation-rules';
    import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
    import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
    import { getAppliedLibraryId, hasAppliedLibrary } from '@/shared/utils/library-utils';
    import { CriterionLibrary } from '@/shared/models/iAM/criteria';
    import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
    import { getUserName } from '@/shared/utils/get-user-info';
    import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
    import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
    import BudgetPriorityService from '@/services/budget-priority.service';
    import { AxiosResponse } from 'axios';
    import { http2XX } from '@/shared/utils/http-utils';
    import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
    import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
    import { sortByProperty } from '../../shared/utils/sorter-utils';
    import {LibraryUser} from '@/shared/models/iAM/user'
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import CreatePriorityDialog from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityDialog.vue'
    import CreatePriorityLibraryDialog  from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityLibraryDialog.vue'
    import ShareBudgetPriorityLibraryDialog  from '@/components/budget-priority-editor/budget-priority-editor-dialogs/ShareBudgetPriorityLibraryDialog.vue'
    import { useConfirm } from 'primevue/useconfirm';
    import ConfirmDialog from 'primevue/confirmdialog';
import { getUrl } from '@/shared/utils/get-url';
import { faL } from '@fortawesome/free-solid-svg-icons';

    let store = useStore();
    const confirm = useConfirm();
    const $router = useRouter();
    let stateScenarioSimpleBudgetDetails = computed<SimpleBudgetDetail[]>(() => store.state.investmentModule.scenarioSimpleBudgetDetails);
    let stateBudgetPriorityLibraries = computed<BudgetPriorityLibrary[]>(() => store.state.budgetPriorityModule.budgetPriorityLibraries);
    let stateSelectedBudgetPriorityLibrary = computed<BudgetPriorityLibrary>(() => store.state.budgetPriorityModule.selectedBudgetPriorityLibrary);
    let stateScenarioBudgetPriorities = computed<BudgetPriority[]>(() => store.state.budgetPriorityModule.scenarioBudgetPriorities);
    let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
    let hasPermittedAccess = computed<boolean>(() => store.state.budgetPriorityModule.hasPermittedAccess);
    let isSharedLibrary = computed<boolean>(() => store.state.budgetPriorityModule.isSharedLibrary);
    async function getIsSharedLibraryAction(payload?: any): Promise<any>{await store.dispatch('getIsSharedBudgetPriorityLibrary', payload)}
    async function getHasPermittedAccessAction(payload?: any): Promise<any>{await store.dispatch('getHasPermittedAccess', payload)}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification', payload) }
    async function getBudgetPriorityLibrariesAction(payload?: any): Promise<any>{await store.dispatch('getBudgetPriorityLibraries', payload)}
    async function selectBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('selectBudgetPriorityLibrary', payload)}
    async function upsertBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('upsertBudgetPriorityLibrary', payload) }
    async function deleteBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('deleteBudgetPriorityLibrary', payload)}
    async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any>{await store.dispatch('getScenarioSimpleBudgetDetails', payload)} 
    async function upsertScenarioBudgetPrioritiesAction(payload?: any): Promise<any>{await store.dispatch('upsertScenarioBudgetPriorities', payload) }
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges', payload) }
    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification', payload)}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch( 'getCurrentUserOrSharedScenario', payload)}
    function selectScenarioAction(payload?: any): void { store.dispatch('selectScenario', payload)}
    
    let getUserNameByIdGetter: any = store.getters.getUserNameById;
    function budgetPriorityLibraryMutator(payload: any){store.commit('budgetPriorityLibraryMutator', payload);}
    function selectedBudgetPriorityLibraryMutator(payload: any){store.commit('selectedBudgetPriorityLibraryMutator', payload);}
    
    let addedRows: ShallowRef<BudgetPriority[]> = shallowRef([]);
    let updatedRowsMap:Map<string, [BudgetPriority, BudgetPriority]> = new Map<string, [BudgetPriority, BudgetPriority]>();//0: original value | 1: updated value
    let deletionIds: ShallowRef<string[]> = shallowRef([]);
    let rowCache: BudgetPriority[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    //const pagination: ShallowRef<Pagination> = shallowRef(clone(emptyPagination));
    const pagination: Pagination = shallowReactive(clone(emptyPagination));

    let isPageInit = false;
    let totalItems = ref(0);
    let currentPage: ShallowRef<BudgetPriority[]> = shallowRef([]);
    let initializing: boolean = true;
    let currentPriorityList: number[] = [];

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = '';
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue = ref<string | null>(null);

    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary = ref(false);
    let librarySelectItems  = ref<SelectItem[]>([]);
    let shareBudgetPriorityLibraryDialogData = ref<ShareBudgetPriorityLibraryDialogData>(clone(emptyShareBudgetPriorityLibraryDialogData));
    let isShared = ref<boolean>(false);
    let dateModified = ref<string>();

    let selectedBudgetPriorityLibrary = shallowRef(clone(emptyBudgetPriorityLibrary));
    let budgetPriorityGridRows = ref<BudgetPriorityGridDatum[]>([]);
    let actionHeader: any = { title: 'Action', key: '', align: 'left', sortable: false, class: '', width: ''}
    let budgetPriorityGridHeaders: any[] = [
        { title: 'Priority', key: 'priorityLevel', align: 'left', sortable: true, class: '', width: '5%' },
        { title: 'Year', key: 'year', align: 'left', sortable: false, class: '', width: '5%' },
        { title: 'Criteria', key: 'criteria', align: 'left', sortable: false, class: '', width: '5%' },
        actionHeader
    ];
    let selectedBudgetPriorityGridRows = ref<BudgetPriorityGridDatum[]>([]);
    let selectedBudgetPriorityIds: string[] = [];
    let selectedBudgetPriorityForCriteriaEdit: BudgetPriority = clone(emptyBudgetPriority);
    let showCreateBudgetPriorityDialog = ref<boolean>(false);
    let criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
    let createBudgetPriorityLibraryDialogData = ref<CreateBudgetPriorityLibraryDialogData>(clone(emptyCreateBudgetPriorityLibraryDialogData));
    let confirmDeleteAlertData = ref<AlertData>(clone(emptyAlertData));
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario = ref(false);  
    let disableCrudButtonsResult: boolean = false;
    let checkBoxChanged: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let hasCreatedLibrary: boolean = false;
    let parentLibraryName = ref<string>("None");
    let parentLibraryId: string = "";
    let parentModifiedFlag: boolean = false;
    let scenarioLibraryIsModified = ref<boolean>(false);
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;
    
    created();
    async function created() {
        librarySelectItemValue.value = null;
        await getBudgetPriorityLibrariesAction();
        await getHasPermittedAccessAction();

        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.BudgetPriority) !== -1) {
            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                $router.push('/Scenarios/');
            }
            hasScenario.value = true;
            await getScenarioSimpleBudgetDetailsAction({ scenarioId: selectedScenarioId })
            await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId})
            selectScenarioAction({ scenarioId: selectedScenarioId });        
            await initializePages();             
        }
    }
    onBeforeUnmount(() => onBeforeUnmountFunc())
    function onBeforeUnmountFunc() {
        setHasUnsavedChangesAction({ value: false });
    }
    watch(stateBudgetPriorityLibraries, onStateBudgetPriorityLibrariesChanged)
    function onStateBudgetPriorityLibrariesChanged() {
        librarySelectItems.value = stateBudgetPriorityLibraries.value.map((library: BudgetPriorityLibrary) => ({
            text: library.name,
            value: library.id,
        }));
    }
    //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue, onLibrarySelectItemValueChangedCheckUnsaved)
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario.value){
            onSelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged) {
            CheckUnsavedDialog(onSelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = trueLibrarySelectItemValue;               
            });
        }

        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        librarySelectItemValueAllowedChanged = true;
    }
    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value;
        selectBudgetPriorityLibraryAction({ libraryId: librarySelectItemValue.value });
    }
    watch(stateSelectedBudgetPriorityLibrary, onStateSelectedPriorityLibraryChanged)
    function onStateSelectedPriorityLibraryChanged() {
        selectedBudgetPriorityLibrary.value = clone(stateSelectedBudgetPriorityLibrary.value);
    }
    watch(selectedBudgetPriorityLibrary, onSelectedPriorityLibraryChanged)
    function onSelectedPriorityLibraryChanged() {
        hasSelectedLibrary.value = selectedBudgetPriorityLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }
        updatedRowsMap.clear();
        deletionIds.value = [];
        addedRows.value = [];
        initializing = false;
        if(hasSelectedLibrary.value)
            onPaginationChanged();
    }
    watch(stateScenarioBudgetPriorities, onStateScenarioBudgetPrioritiesChanged)
    function onStateScenarioBudgetPrioritiesChanged() {
        if (hasScenario.value) {
            onPaginationChanged();
        }
    }

    watch(currentPage, onBudgetPrioritiesChanged)
    function onBudgetPrioritiesChanged() {
        
        setGridData();
        currentPage.value.forEach((item) => {
            currentPriorityList.push(item.priorityLevel);
        });
        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName.value = library.text;
            }
        });
    }

    watch(selectedBudgetPriorityGridRows, onSelectedPriorityRowsChanged)
    function onSelectedPriorityRowsChanged() {
        selectedBudgetPriorityIds = getPropertyValues('id', selectedBudgetPriorityGridRows.value) as string[];
    }

    watch(isSharedLibrary, onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared.value = isSharedLibrary.value;
    }

    watch(pagination, () => onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<BudgetPriority>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified.value
            },           
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL)
            BudgetPriorityService.getScenarioBudgetPriorityPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    populateEmptyBudgetPercentagePairs(currentPage.value);
                }
            });
        else if(hasSelectedLibrary.value)
             await BudgetPriorityService.getBudgetPriorityLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified.value = data.slice(0, 10);
                   }
             }),
             await BudgetPriorityService.getLibraryBudgetPriorityPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;

                    if (!isNil(selectedBudgetPriorityLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedBudgetPriorityLibrary.value).then(() => isShared.value = isSharedLibrary.value);
                    }           
                }
            });     
    }

    watch(deletionIds, onDeletionIdsChanged)
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows, onAddedRowsChanged)
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }

    watch(stateScenarioSimpleBudgetDetails, () => {
        setGridCriteriaColumnWidth();
        setGridHeaders();
    })

    function hasBudgetPercentagePairsThatMatchBudgets(budgetPriority: BudgetPriority) {
        if (!hasValue(stateScenarioSimpleBudgetDetails.value)) {
            return true;
        }

        const simpleBudgetDetails: SimpleBudgetDetail[] = budgetPriority.budgetPercentagePairs
            .map((budgetPercentagePair: BudgetPercentagePair) => ({
                id: budgetPercentagePair.budgetId, name: budgetPercentagePair.budgetName,
            })) as SimpleBudgetDetail[];

        return isEqual(sortNonObjectLists(simpleBudgetDetails), sortNonObjectLists(clone(stateScenarioSimpleBudgetDetails.value)));
    }

    function createNewBudgetPercentagePairsFromBudgets() {
        return stateScenarioSimpleBudgetDetails.value.map((simpleBudgetDetail: SimpleBudgetDetail) => ({
            id: getNewGuid(),
            budgetId: simpleBudgetDetail.id,
            budgetName: simpleBudgetDetail.name,
            percentage: 100,
        })) as BudgetPercentagePair[];
    }

    function setGridCriteriaColumnWidth() {
        let criteriaColumnWidth = '75%';

        if (hasScenario.value) {
            switch (stateScenarioSimpleBudgetDetails.value.length) {
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
                default:
                    criteriaColumnWidth = '10%';
            }
        }

        budgetPriorityGridHeaders[2].width = criteriaColumnWidth;
    }

    function setGridHeaders() {
        if (hasScenario.value) {
            const budgetNames: string[] = getPropertyValues('name', stateScenarioSimpleBudgetDetails.value) as string[];
            if (hasValue(budgetNames)) {
                const budgetHeaders: any[] = budgetNames.map((budgetName: string) => ({
                    title: `${budgetName} %`,
                    key: budgetName,
                    align: 'left',
                    sortable: true,
                    class: '',
                    width: '',
                }));
                budgetPriorityGridHeaders = [
                    budgetPriorityGridHeaders[0],
                    budgetPriorityGridHeaders[1],
                    budgetPriorityGridHeaders[2],
                    ...budgetHeaders,
                    actionHeader
                ];
            }
        }
    }

    function setGridData() {
        budgetPriorityGridRows.value = currentPage.value.map((budgetPriority: BudgetPriority) => {
            const row: BudgetPriorityGridDatum = {
                id: budgetPriority.id,
                priorityLevel: budgetPriority.priorityLevel.toString(),
                year: hasValue(budgetPriority.year) ? budgetPriority.year!.toString() : '',
                criteria: budgetPriority.criterionLibrary.mergedCriteriaExpression != null ? budgetPriority.criterionLibrary.mergedCriteriaExpression : '',
            };

            if (hasScenario.value && hasValue(budgetPriority.budgetPercentagePairs)) {
                budgetPriority.budgetPercentagePairs.forEach((budgetPercentagePair: BudgetPercentagePair) => {
                    row[budgetPercentagePair.budgetName] = budgetPercentagePair.percentage.toString();
                });
            }

            return row;
        });
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedBudgetPriorityLibrary.value.owner);
        }
        
        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedBudgetPriorityLibrary.value.owner) == getUserName();
    }

    function onShowCreateBudgetPriorityLibraryDialog(createAsNewLibrary: boolean) {     
        createBudgetPriorityLibraryDialogData.value = {
            showDialog: true,
            budgetPriorities: createAsNewLibrary ? currentPage.value : [],
        };
    }

    function onSubmitCreateBudgetPriorityLibraryDialogResult(budgetPriorityLibrary: BudgetPriorityLibrary) {
        createBudgetPriorityLibraryDialogData.value = clone(emptyCreateBudgetPriorityLibraryDialogData);
        if (!isNil(budgetPriorityLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: budgetPriorityLibrary,    
                isNewLibrary: true,           
                syncModel: {
                    libraryId: budgetPriorityLibrary.budgetPriorities.length == 0 || !hasSelectedLibrary.value ? null : selectedBudgetPriorityLibrary.value.id,
                    rowsForDeletion: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : deletionIds.value,
                    updateRows: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : addedRows.value,
                    isModified: false
                },
                scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            BudgetPriorityService.upsertBudgetPriorityLibrary(upsertRequest).then(() => {
                hasCreatedLibrary = true;
                librarySelectItemValue.value = budgetPriorityLibrary.id;
                
                if(budgetPriorityLibrary.budgetPriorities.length == 0){
                    clearChanges();
                }

                budgetPriorityLibraryMutator(budgetPriorityLibrary);
                selectedBudgetPriorityLibraryMutator(budgetPriorityLibrary.id);                
                addSuccessNotificationAction({message:'Added budget priority library'})
            })
        }
    }

    function onAddBudgetPriority(newBudgetPriority: BudgetPriority) {
        showCreateBudgetPriorityDialog.value = false;

        if (!isNil(newBudgetPriority)) {
            if (hasScenario.value && hasValue(stateScenarioSimpleBudgetDetails)) {
                newBudgetPriority.budgetPercentagePairs = createNewBudgetPercentagePairsFromBudgets();
            }

            addedRows.value.push(newBudgetPriority);
            onPaginationChanged()
        }
    }

    function onEditBudgetPriority(budgetPriorityGridDatum: BudgetPriorityGridDatum, property: string, value: any) {
        if (any(propEq('id', budgetPriorityGridDatum.id), currentPage.value)) {
            let budgetPriority: BudgetPriority = find(
                propEq('id', budgetPriorityGridDatum.id), currentPage.value,
            ) as BudgetPriority;

            if (property === 'year' && (!hasValue(value) || parseInt(value) === 0)) {
                budgetPriority.year = null;
            } else {
                budgetPriority = setItemPropertyValue(property, value, budgetPriority) as BudgetPriority;
            }
            onUpdateRow(budgetPriority.id, clone(budgetPriority))
            onPaginationChanged();
        }
    }

    function onEditBudgetPercentagePair(budgetPriorityGridDatum: BudgetPriorityGridDatum, budgetName: string, percentage: number) {
        const budgetPriority: BudgetPriority = find(
            propEq('id', budgetPriorityGridDatum.id), currentPage.value,
        ) as BudgetPriority;

        const budgetPercentagePair: BudgetPercentagePair = find(
            propEq('budgetName', budgetName), budgetPriority.budgetPercentagePairs,
        ) as BudgetPercentagePair;

        onUpdateRow(budgetPriority.id, {
                ...budgetPriority, budgetPercentagePairs: update(
                    findIndex(propEq('id', budgetPercentagePair.id), budgetPriority.budgetPercentagePairs),
                    setItemPropertyValue('percentage', percentage, budgetPercentagePair) as BudgetPercentagePair,
                    budgetPriority.budgetPercentagePairs,
                ),
            } as BudgetPriority)

        onPaginationChanged();
    }

    function onShowCriterionLibraryEditorDialog(budgetPriorityGridDatum: BudgetPriorityGridDatum) {
        selectedBudgetPriorityForCriteriaEdit = find(
            propEq('id', budgetPriorityGridDatum.id), currentPage.value,
        ) as BudgetPriority;

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: selectedBudgetPriorityForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        criterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedBudgetPriorityForCriteriaEdit.id !== uuidNIL) {
            if(selectedBudgetPriorityForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedBudgetPriorityForCriteriaEdit.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedBudgetPriorityForCriteriaEdit.id, 
            { ...selectedBudgetPriorityForCriteriaEdit, 
            criterionLibrary: {...selectedBudgetPriorityForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} })

            onPaginationChanged();
        }

        selectedBudgetPriorityForCriteriaEdit = clone(emptyBudgetPriority);
    }

    function hasUniquePriorityLevels() {
        const levels = new Set();
        for (let item of budgetPriorityGridRows.value) {
            if (levels.has(item.priorityLevel)) {
                return false;
            }
            levels.add(item.priorityLevel);
        }
        return true;
    }

    async function onUpsertScenarioBudgetPriorities() {
        if (selectedBudgetPriorityLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection ===false) {scenarioLibraryIsModified.value = true;}
        else { scenarioLibraryIsModified.value = false; }

        let response = await BudgetPriorityService.upsertScenarioBudgetPriorities({
            libraryId: selectedBudgetPriorityLibrary.value.id === uuidNIL ? null : selectedBudgetPriorityLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified.value
        }, selectedScenarioId)
        if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
            parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
            clearChanges();
            librarySelectItemValue.value = null;
            addSuccessNotificationAction({message: "Modified scenario's budget priorities"});
            currentPage.value = sortByProperty("priorityLevel", currentPage.value);
            await onPaginationChanged();
        }           
    }

    function onUpsertBudgetPriorityLibrary() {
        const budgetPriorityLibrary: BudgetPriorityLibrary = {
            ...clone(selectedBudgetPriorityLibrary.value),
            budgetPriorities: clone(currentPage.value),
        };
        upsertBudgetPriorityLibraryAction(budgetPriorityLibrary);

        const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: selectedBudgetPriorityLibrary.value,
                isNewLibrary: false,
                 syncModel: {
                    libraryId: selectedBudgetPriorityLibrary.value.id === uuidNIL ? null : selectedBudgetPriorityLibrary.value.id,
                    rowsForDeletion: deletionIds.value,
                    updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: addedRows.value,
                    isModified: false
                 }
                 , scenarioId: null
        }
        BudgetPriorityService.upsertBudgetPriorityLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()               
                budgetPriorityLibraryMutator(selectedBudgetPriorityLibrary.value);
                selectedBudgetPriorityLibraryMutator(selectedBudgetPriorityLibrary.value.id);                
                addSuccessNotificationAction({message: "Updated budget priority library",});
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

    function onRemoveBudgetPriorities() {
        selectedBudgetPriorityIds.forEach(_ => {
            removePriorityLogic(_);
        });

        selectedBudgetPriorityIds = [];
        onPaginationChanged();
    }

    function onRemoveBudgetPriority(id: string){
        removePriorityLogic(id)
        onPaginationChanged();
    }

    function removePriorityLogic(id: string){
        if(isNil(find(propEq('id', id), addedRows.value))){
            deletionIds.value.push(id);
            if(!isNil(updatedRowsMap.get(id)))
                updatedRowsMap.delete(id)
        }           
        else{          
            addedRows.value = addedRows.value.filter((bp: BudgetPriority) => bp.id !== id)
        }  
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
            deleteBudgetPriorityLibraryAction(selectedBudgetPriorityLibrary.value.id)
                .then(() => librarySelectItemValue.value = null);
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid: boolean = rows.every((budgetPriority: BudgetPriority) => {
            const priorityIsValid = hasBudgetPercentagePairsThatMatchBudgets(budgetPriority);
            const allSubDataIsValid: boolean = hasScenario.value
                ? budgetPriority.budgetPercentagePairs.every((budgetPercentagePair: BudgetPercentagePair) => {
                    return priorityIsValid &&
                        rules['generalRules'].valueIsNotEmpty(budgetPercentagePair.percentage) &&
                        rules['generalRules'].valueIsWithinRange(budgetPercentagePair.percentage, [0, 100]);
                })
                : true;

            return rules['generalRules'].valueIsNotEmpty(budgetPriority.priorityLevel) === true && allSubDataIsValid;
        })

        if (hasSelectedLibrary.value) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedBudgetPriorityLibrary.value.name) === true && allDataIsValid);
        }
        disableCrudButtonsResult = !allDataIsValid && hasUniquePriorityLevels();
        return !allDataIsValid;
    }

    function onUpdateRow(rowId: string, updatedRow: BudgetPriority){
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
        pagination.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedBudgetPriorityLibrary.value, stateSelectedBudgetPriorityLibrary.value))
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
    function onShowShareBudgetPriorityLibraryDialog(budgetPriorityLibrary: BudgetPriorityLibrary) {
        shareBudgetPriorityLibraryDialogData.value = {
            showDialog:true,
            budgetPriorityLibrary: clone(budgetPriorityLibrary)
        }
    }

    function onShareBudgetPriorityLibraryDialogSubmit(budgetPriorityLibraryUsers: BudgetPriorityLibraryUser[]) {
            shareBudgetPriorityLibraryDialogData.value = clone(emptyShareBudgetPriorityLibraryDialogData);

            if (!isNil(budgetPriorityLibraryUsers) && selectedBudgetPriorityLibrary.value.id !== getBlankGuid())
            {
                let libraryUserData: LibraryUser[] = [];

                //create library users
                budgetPriorityLibraryUsers.forEach((budgetPriorityLibraryUser, index) =>
                {   
                    //determine access level
                    let libraryUserAccessLevel: number = 0;
                    if (libraryUserAccessLevel == 0 && budgetPriorityLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                    if (libraryUserAccessLevel == 0 && budgetPriorityLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                    //create library user object
                    let libraryUser: LibraryUser = {
                        userId: budgetPriorityLibraryUser.userId,
                        userName: budgetPriorityLibraryUser.username,
                        accessLevel: libraryUserAccessLevel
                    }

                    //add library user to an array
                    libraryUserData.push(libraryUser);
                });

                if (!isNil(selectedBudgetPriorityLibrary.value.id) ) {
                            getIsSharedLibraryAction(selectedBudgetPriorityLibrary.value).then(() => isShared.value = isSharedLibrary.value);
                }
                //update budget library sharing
                BudgetPriorityService.upsertOrDeleteBudgetPriorityLibraryUsers(selectedBudgetPriorityLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                    {
                        resetPage();
                    }
            });
        }
    }
    function setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            parentLibraryName.value = "None";
            return;
        }
        let foundLibrary: BudgetPriorityLibrary = emptyBudgetPriorityLibrary;
        stateBudgetPriorityLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }
    function populateEmptyBudgetPercentagePairs(budgetPriorites: BudgetPriority[]) {
        budgetPriorites.forEach(item => {
            if (item.budgetPercentagePairs.length === 0) {
                item.budgetPercentagePairs = createNewBudgetPercentagePairsFromBudgets();
            }
        });
    }
    async function initializePages(){
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<BudgetPriority>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL)
            await BudgetPriorityService.getScenarioBudgetPriorityPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = sortByProperty("priorityLevel", data.items);
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;

                    populateEmptyBudgetPercentagePairs(currentPage.value);
                }
                setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                loadedParentName = parentLibraryName.value; //store original
                scenarioLibraryIsModified.value = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
            });
    }
</script>

<style>
.priorities-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.priorities-error {
    color: red;
    text-align: center;
    margin-bottom: 10px;
}

.priorities-data-table .v-menu--inline, .priority-criteria-output {
    width: 100%;
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
