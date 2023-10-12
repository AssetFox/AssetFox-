<template>
    <v-row column class="Montserrat-font-family">
        <v-col cols = "12">
            <v-row justify-space-between row >              
                <v-col cols ="4" class="ghd-constant-header">
                    <v-row column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Select Budget Priority Library</v-subheader>
                            <v-select id="BudgetPriorityEditor-library-vselect"
                                :items='librarySelectItems' 
                                append-icon=$vuetify.icons.ghd-down
                                variant="outlined"
                                v-model='librarySelectItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                            </v-select>    
                             <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>                       
                    </v-row>
                </v-col>
                <v-col cols = "4" class="ghd-constant-header">
                    <v-row row v-show='hasSelectedLibrary || hasScenario' class="shared-owner-flex-padding">
                        <div v-if='hasSelectedLibrary && !hasScenario' class="header-text-content owner-padding">
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
                        <v-btn id="BudgetPriorityEditor-shareLibrary-vbtn" @click='onShowShareBudgetPriorityLibraryDialog(selectedBudgetPriorityLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>                               
                </v-col>
                <v-col cols = "4" class="ghd-constant-header">
                    <v-row row align-end class="left-buttons-padding">
                        <v-spacer></v-spacer>
                        <v-btn id="BudgetPriorityEditor-addBudgetPriority-vbtn" @click='showCreateBudgetPriorityDialog = true' variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show='hasSelectedLibrary || hasScenario'>Add Budget Priority</v-btn>
                        
                        <v-btn id="BudgetPriorityEditor-createNewLibrary-vbtn" @click='onShowCreateBudgetPriorityLibraryDialog(false)' variant = "outlined"
                            v-show='!hasScenario' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' > 
                            Create New Library
                        </v-btn>
                    </v-row>
                </v-col>
            </v-row>

        </v-col>
        <v-col v-show='hasSelectedLibrary || hasScenario' cols="12">
            <div class='priorities-data-table'>
                <v-data-table :header='budgetPriorityGridHeaders' 
                              :items='budgetPriorityGridRows'
                              :pagination.sync="pagination"
                              :must-sort='true'
                              :total-items="totalItems"
                              :rows-per-page-items=[5,10,25]
                              id = "BudgetPriority-priorities-vdatatable"
                              class='v-table__overflow ghd-table' item-key='id' select-all
                              sort-icon=$vuetify.icons.ghd-table-sort                              
                              v-model='selectedBudgetPriorityGridRows' >
                    <template v-slot:item="{item}">
                        <td>
                            <v-checkbox id="BudgetPriorityEditor-deleteBudgetPriority-vcheckbox" hide-details primary v-model='item.raw.selected'></v-checkbox>
                        </td>
                        <td v-for='header in budgetPriorityGridHeaders'>
                            <div v-if="header.value === 'priorityLevel' || header.value === 'year'">
                                <v-edit-dialog
                                    :return-value.sync='item.value[header.value]'
                                    @save='onEditBudgetPriority(item.value, header.value, item.value[header.value])'
                                    size="large" lazy>
                                    <v-text-field v-if="header.value === 'priorityLevel'" readonly single-line
                                                  class='sm-txt'
                                                  :model-value="item.value[header.value]"
                                                  :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-else readonly single-line class='sm-txt'
                                                  :model-value='item.value[header.value]' />
                                    <template v-slot:input>
                                        <v-text-field v-if="header.value === 'priorityLevel'" label='Edit' single-line
                                                      v-model.number='item.value[header.value]'
                                                      :mask="'##########'"
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsNotUnique(item.value[header.value], currentPriorityList)]" />
                                        <v-text-field v-else label='Edit' single-line :mask="'####'"
                                                      v-model.number='item.value[header.value]' />
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else-if="header.value === 'criteria'">
                                <v-row align-center row style='flex-wrap:nowrap'>
                                    <v-menu bottom min-height='500px' min-width='500px'>
                                        <template v-slot:activator>
                                            <div v-if='stateScenarioSimpleBudgetDetails.length > 5'>
                                                <v-btn class='ara-blue ghd-button-text' icon>
                                                    <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
                                                </v-btn>
                                            </div>
                                            <div v-else class='priority-criteria-output'>
                                                <v-text-field readonly single-line class='sm-txt'
                                                              :model-value='item.value.criteria' />
                                            </div>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea :model-value='item.value.criteria' full-width no-resize outline
                                                            readonly rows='5' />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn id="BudgetPriorityEditor-editCriteria-vbtn" @click='onShowCriterionLibraryEditorDialog(item.value)' class='ghd-blue'
                                           icon>
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-row>
                            </div>
                            <div v-else-if="header.text.endsWith('%')">
                                <v-edit-dialog
                                    :return-value.sync='item.value[header.value]'
                                    @save='onEditBudgetPercentagePair(item.value, header.value, item.value[header.value])'
                                    size="large" lazy>
                                    <v-text-field readonly single-line class='sm-txt' :model-value='item.value[header.value]'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(item.value[header.value], [0, 100])]" />
                                    <template v-slot:input>
                                        <v-text-field :mask="'###'" label='Edit' single-line
                                                      :model-value.number="item.value[header.value]"
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(item.value[header.value], [0, 100])]" />
                                    </template>
                                </v-edit-dialog>
                            </div>
                            <div v-else>
                                <v-btn @click="onRemoveBudgetPriority(item.value.id)"  class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </div>
                        </td>
                    </template>
                </v-data-table>
                <v-btn :disabled='selectedBudgetPriorityIds.length === 0' @click='onRemoveBudgetPriorities'
                    class='ghd-blue ghd-button' variant = "flat">
                    Delete Selected
                </v-btn>
            </div>
        </v-col>
        <v-col v-show='hasSelectedLibrary && selectedScenarioId === uuidNIL'
                xs12>
            <v-row justify-center>
                <v-col >
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea no-resize outline rows='4' class="ghd-text-field-border"
                                v-model='selectedBudgetPriorityLibrary.description'
                                @update:model-value="checkHasUnsavedChanges()">
                    </v-textarea>
                </v-col>
            </v-row>
        </v-col>
        <v-col cols = "12">           
            <v-row justify-center row v-show='hasSelectedLibrary || hasScenario'>
                <v-btn  variant = "flat" @click='onDiscardChanges'
                       v-show='hasScenario' :disabled='!hasUnsavedChanges' class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn id="BudgetPriorityEditor-createAsNewLibrary-vbtn" @click='onShowCreateBudgetPriorityLibraryDialog(true)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                       :disabled='disableCrudButtons()'>
                    Create as New Library
                </v-btn>
                <v-btn @click='onUpsertScenarioBudgetPriorities'
                       class='ghd-blue-bg text-white ghd-button-text ghd-button'
                       v-show='hasScenario' :disabled='disableCrudButtonsResult || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn id="BudgetPriorityEditor-deleteLibrary-vbtn"  @click='onShowConfirmDeleteAlert' variant = "outlined"
                       v-show='!hasScenario' :disabled='!hasSelectedLibrary' class='ghd-blue ghd-button-text ghd-button'>
                    Delete Library
                </v-btn>             
                <v-btn id="BudgetPriorityEditor-updateLibrary-vbtn" @click='onUpsertBudgetPriorityLibrary'
                       class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
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
    </v-row>
</template>

<script setup lang='ts'>
    import Vue, { Ref, ref, watch, onBeforeUnmount, ShallowRef, shallowRef } from 'vue';
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
    import { DataTableHeader } from '@/shared/models/vue/data-table-header';
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
    import { isNullOrUndefined } from 'util';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import CreatePriorityDialog from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityDialog.vue'
    import CreatePriorityLibraryDialog  from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityLibraryDialog.vue'
    import ShareBudgetPriorityLibraryDialog  from '@/components/budget-priority-editor/budget-priority-editor-dialogs/ShareBudgetPriorityLibraryDialog.vue'

    const ObjectID = require('bson-objectid');
    let store = useStore();
    const $router = useRouter();
    let stateScenarioSimpleBudgetDetails = shallowRef<SimpleBudgetDetail[]>(store.state.investmentModule.scenarioSimpleBudgetDetails);
    let stateBudgetPriorityLibraries = shallowRef<BudgetPriorityLibrary[]>(store.state.budgetPriorityModule.budgetPriorityLibraries);
    let stateSelectedBudgetPriorityLibrary = shallowRef<BudgetPriorityLibrary>(store.state.budgetPriorityModule.selectedBudgetPriorityLibrary);
    let stateScenarioBudgetPriorities = shallowRef<BudgetPriority[]>(store.state.budgetPriorityModule.scenarioBudgetPriorities);
    let hasUnsavedChanges = shallowRef<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let hasAdminAccess = shallowRef<boolean>(store.state.authenticationModule.hasAdminAccess);
    let hasPermittedAccess = shallowRef<boolean>(store.state.budgetPriorityModule.hasPermittedAccess);
    let isSharedLibrary = shallowRef<boolean>(store.state.budgetPriorityModule.isSharedLibrary);
    async function getIsSharedLibraryAction(payload?: any): Promise<any>{await store.dispatch('getIsSharedBudgetPriorityLibrary')}
    async function getHasPermittedAccessAction(payload?: any): Promise<any>{await store.dispatch('getHasPermittedAccess')}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification') }
    async function getBudgetPriorityLibrariesAction(payload?: any): Promise<any>{await store.dispatch('getBudgetPriorityLibraries')}
    async function selectBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('selectBudgetPriorityLibrary')}
    async function upsertBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('upsertBudgetPriorityLibrary') }
    async function deleteBudgetPriorityLibraryAction(payload?: any): Promise<any>{await store.dispatch('deleteBudgetPriorityLibrary')}
    async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any>{await store.dispatch('getScenarioSimpleBudgetDetails')} 
    async function upsertScenarioBudgetPrioritiesAction(payload?: any): Promise<any>{await store.dispatch('upsertScenarioBudgetPriorities') }
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges') }
    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification')}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch( 'getCurrentUserOrSharedScenario')}
    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario')}
    
    let getUserNameByIdGetter: any = store.getters.getUserNameById;
    function budgetPriorityLibraryMutator(payload: any){store.commit('budgetPriorityLibraryMutator');}
    function selectedBudgetPriorityLibraryMutator(payload: any){store.commit('selectedBudgetPriorityLibraryMutator');}
    
    let addedRows: ShallowRef<BudgetPriority[]> = shallowRef([]);
    let updatedRowsMap:Map<string, [BudgetPriority, BudgetPriority]> = new Map<string, [BudgetPriority, BudgetPriority]>();//0: original value | 1: updated value
    let deletionIds: ShallowRef<string[]> = shallowRef([]);
    let rowCache: BudgetPriority[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let pagination: ShallowRef<Pagination> = shallowRef(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: ShallowRef<BudgetPriority[]> = shallowRef([]);
    let initializing: boolean = true;
    let currentPriorityList: number[] = [];

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: ShallowRef<string | null> = shallowRef(null);

    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let shareBudgetPriorityLibraryDialogData: ShareBudgetPriorityLibraryDialogData = clone(emptyShareBudgetPriorityLibraryDialogData);
    let isShared: boolean = false;
    let dateModified: string;

    let selectedBudgetPriorityLibrary: BudgetPriorityLibrary = clone(emptyBudgetPriorityLibrary);
    let budgetPriorityGridRows: BudgetPriorityGridDatum[] = [];
    let actionHeader: DataTableHeader = { text: 'Action', value: '', align: 'left', sortable: false, class: '', width: ''}
    let budgetPriorityGridHeaders: DataTableHeader[] = [
        { text: 'Priority', value: 'priorityLevel', align: 'left', sortable: true, class: '', width: '' },
        { text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: '7%' },
        { text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '' },
        actionHeader
    ];
    let selectedBudgetPriorityGridRows: ShallowRef<BudgetPriorityGridDatum[]> = shallowRef([]);
    let selectedBudgetPriorityIds: string[] = [];
    let selectedBudgetPriorityForCriteriaEdit: BudgetPriority = clone(emptyBudgetPriority);
    let showCreateBudgetPriorityDialog: boolean = false;
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
    let createBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario: boolean = false;  
    let disableCrudButtonsResult: boolean = false;
    let checkBoxChanged: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let hasCreatedLibrary: boolean = false;
    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let parentModifiedFlag: boolean = false;
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;
    
    created()
    function created() {
        librarySelectItemValue.value = null;
        getBudgetPriorityLibrariesAction();
        getHasPermittedAccessAction();

        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.BudgetPriority) !== -1) {
            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                $router.push('/Scenarios/');
            }
            hasScenario = true;
            getScenarioSimpleBudgetDetailsAction({ scenarioId: selectedScenarioId }).then(() => {
                getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                    selectScenarioAction({ scenarioId: selectedScenarioId });        
                    initializePages();
                });                                        
            });             
        }

    }
    onBeforeUnmount(() => onBeforeUnmountFunc)
    function onBeforeUnmountFunc() {
        setHasUnsavedChangesAction({ value: false });
    }
    watch(stateBudgetPriorityLibraries, onStateBudgetPriorityLibrariesChanged)
    function onStateBudgetPriorityLibrariesChanged() {
        librarySelectItems = stateBudgetPriorityLibraries.value.map((library: BudgetPriorityLibrary) => ({
            text: library.name,
            value: library.id,
        }));
    }
    //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue, onLibrarySelectItemValueChangedCheckUnsaved )
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario){
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
        selectBudgetPriorityLibraryAction({ libraryId: librarySelectItemValue });
    }
    watch(stateSelectedBudgetPriorityLibrary, onStateSelectedPriorityLibraryChanged)
    function onStateSelectedPriorityLibraryChanged() {
        selectedBudgetPriorityLibrary = clone(stateSelectedBudgetPriorityLibrary.value);
    }
    watch(selectedBudgetPriorityLibrary, onSelectedPriorityLibraryChanged)
    function onSelectedPriorityLibraryChanged() {
        hasSelectedLibrary = selectedBudgetPriorityLibrary.id !== uuidNIL;

        if (hasSelectedLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }
        updatedRowsMap.clear();
        deletionIds.value = [];
        addedRows.value = [];
        initializing = false;
        if(hasSelectedLibrary)
            onPaginationChanged();
    }
    watch(stateScenarioBudgetPriorities, onStateScenarioBudgetPrioritiesChanged)
    function onStateScenarioBudgetPrioritiesChanged() {
        if (hasScenario) {
            onPaginationChanged();
        }
    }

    watch(currentPage, onBudgetPrioritiesChanged )
    function onBudgetPrioritiesChanged() {
        setGridCriteriaColumnWidth();
        setGridHeaders();
        setGridData();
        currentPage.value.forEach((item) => {
            currentPriorityList.push(item.priorityLevel);
        });
        // Get parent name from library id
        librarySelectItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }

    watch(selectedBudgetPriorityGridRows, onSelectedPriorityRowsChanged )
    function onSelectedPriorityRowsChanged() {
        selectedBudgetPriorityIds = getPropertyValues('id', selectedBudgetPriorityGridRows.value) as string[];
    }

    watch(isSharedLibrary, onStateSharedAccessChanged )
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }

    watch(pagination, onPaginationChanged )
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = pagination.value;
        const request: PagingRequest<BudgetPriority>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null ? librarySelectItemValue.value : null,
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
            BudgetPriorityService.getScenarioBudgetPriorityPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    populateEmptyBudgetPercentagePairs(currentPage.value);
                }
            });
        else if(hasSelectedLibrary)
             await BudgetPriorityService.getBudgetPriorityLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified = data.slice(0, 10);
                   }
             }),
             await BudgetPriorityService.getLibraryBudgetPriorityPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;

                    if (!isNullOrUndefined(selectedBudgetPriorityLibrary.id) ) {
                        getIsSharedLibraryAction(selectedBudgetPriorityLibrary).then(() => isShared = isSharedLibrary.value);
                    }           
                }
            });     
    }

    watch(deletionIds, onDeletionIdsChanged )
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows, onAddedRowsChanged)
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }

    function hasBudgetPercentagePairsThatMatchBudgets(budgetPriority: BudgetPriority) {
        if (!hasValue(stateScenarioSimpleBudgetDetails)) {
            return true;
        }

        const simpleBudgetDetails: SimpleBudgetDetail[] = budgetPriority.budgetPercentagePairs
            .map((budgetPercentagePair: BudgetPercentagePair) => ({
                id: budgetPercentagePair.budgetId, name: budgetPercentagePair.budgetName,
            })) as SimpleBudgetDetail[];

        return isEqual(sortNonObjectLists(simpleBudgetDetails), sortNonObjectLists(clone(stateScenarioSimpleBudgetDetails)));
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

        if (hasScenario) {
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
            }
        }

        budgetPriorityGridHeaders[2].width = criteriaColumnWidth;
    }

    function setGridHeaders() {
        if (hasScenario) {
            const budgetNames: string[] = getPropertyValues('name', stateScenarioSimpleBudgetDetails.value) as string[];
            if (hasValue(budgetNames)) {
                const budgetHeaders: DataTableHeader[] = budgetNames.map((budgetName: string) => ({
                    text: `${budgetName} %`,
                    value: budgetName,
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
        budgetPriorityGridRows = currentPage.value.map((budgetPriority: BudgetPriority) => {
            const row: BudgetPriorityGridDatum = {
                id: budgetPriority.id,
                priorityLevel: budgetPriority.priorityLevel.toString(),
                year: hasValue(budgetPriority.year) ? budgetPriority.year!.toString() : '',
                criteria: budgetPriority.criterionLibrary.mergedCriteriaExpression != null ? budgetPriority.criterionLibrary.mergedCriteriaExpression : '',
            };

            if (hasScenario && hasValue(budgetPriority.budgetPercentagePairs)) {
                budgetPriority.budgetPercentagePairs.forEach((budgetPercentagePair: BudgetPercentagePair) => {
                    row[budgetPercentagePair.budgetName] = budgetPercentagePair.percentage.toString();
                });
            }

            return row;
        });
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedBudgetPriorityLibrary.owner);
        }
        
        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedBudgetPriorityLibrary.owner) == getUserName();
    }

    function onShowCreateBudgetPriorityLibraryDialog(createAsNewLibrary: boolean) {
        createBudgetPriorityLibraryDialogData = {
            showDialog: true,
            budgetPriorities: createAsNewLibrary ? currentPage.value : [],
        };
    }

    function onSubmitCreateBudgetPriorityLibraryDialogResult(budgetPriorityLibrary: BudgetPriorityLibrary) {
        createBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
        if (!isNil(budgetPriorityLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: budgetPriorityLibrary,    
                isNewLibrary: true,           
                syncModel: {
                    libraryId: budgetPriorityLibrary.budgetPriorities.length == 0 || !hasSelectedLibrary ? null : selectedBudgetPriorityLibrary.id,
                    rowsForDeletion: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : deletionIds.value,
                    updateRows: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: budgetPriorityLibrary.budgetPriorities.length == 0 ? [] : addedRows.value,
                    isModified: false
                },
                scenarioId: hasScenario ? selectedScenarioId : null
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
        showCreateBudgetPriorityDialog = false;

        if (!isNil(newBudgetPriority)) {
            if (hasScenario && hasValue(stateScenarioSimpleBudgetDetails)) {
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

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: selectedBudgetPriorityForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

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

    function onUpsertScenarioBudgetPriorities() {

        if (selectedBudgetPriorityLibrary.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        BudgetPriorityService.upsertScenarioBudgetPriorities({
            libraryId: selectedBudgetPriorityLibrary.id === uuidNIL ? null : selectedBudgetPriorityLibrary.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges();
                librarySelectItemValue.value = null;
                addSuccessNotificationAction({message: "Modified scenario's budget priorities"});
                currentPage.value = sortByProperty("priorityLevel", currentPage.value);
                onPaginationChanged();
            }           
        });
    }

    function onUpsertBudgetPriorityLibrary() {
        const budgetPriorityLibrary: BudgetPriorityLibrary = {
            ...clone(selectedBudgetPriorityLibrary),
            budgetPriorities: clone(currentPage.value),
        };
        upsertBudgetPriorityLibraryAction(budgetPriorityLibrary);

        const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: selectedBudgetPriorityLibrary,
                isNewLibrary: false,
                 syncModel: {
                    libraryId: selectedBudgetPriorityLibrary.id === uuidNIL ? null : selectedBudgetPriorityLibrary.id,
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
                budgetPriorityLibraryMutator(selectedBudgetPriorityLibrary);
                selectedBudgetPriorityLibraryMutator(selectedBudgetPriorityLibrary.id);                
                addSuccessNotificationAction({message: "Updated budget priority library",});
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
            deleteBudgetPriorityLibraryAction(selectedBudgetPriorityLibrary.id)
                .then(() => librarySelectItemValue.value = null);
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid: boolean = rows.every((budgetPriority: BudgetPriority) => {
            const priorityIsValid = hasBudgetPercentagePairsThatMatchBudgets(budgetPriority);
            const allSubDataIsValid: boolean = hasScenario
                ? budgetPriority.budgetPercentagePairs.every((budgetPercentagePair: BudgetPercentagePair) => {
                    return priorityIsValid &&
                        rules['generalRules'].valueIsNotEmpty(budgetPercentagePair.percentage) &&
                        rules['generalRules'].valueIsWithinRange(budgetPercentagePair.percentage, [0, 100]);
                })
                : true;

            return rules['generalRules'].valueIsNotEmpty(budgetPriority.priorityLevel) === true && allSubDataIsValid;
        })

        if (hasSelectedLibrary) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedBudgetPriorityLibrary.name) === true && allDataIsValid);
        }
        disableCrudButtonsResult = !allDataIsValid;
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
        pagination.value.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', selectedBudgetPriorityLibrary, stateSelectedBudgetPriorityLibrary))
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
    function onShowShareBudgetPriorityLibraryDialog(budgetPriorityLibrary: BudgetPriorityLibrary) {
        shareBudgetPriorityLibraryDialogData = {
            showDialog:true,
            budgetPriorityLibrary: clone(budgetPriorityLibrary)
        }
    }

    function onShareBudgetPriorityLibraryDialogSubmit(budgetPriorityLibraryUsers: BudgetPriorityLibraryUser[]) {
            shareBudgetPriorityLibraryDialogData = clone(emptyShareBudgetPriorityLibraryDialogData);

            if (!isNil(budgetPriorityLibraryUsers) && selectedBudgetPriorityLibrary.id !== getBlankGuid())
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

                if (!isNullOrUndefined(selectedBudgetPriorityLibrary.id) ) {
                            getIsSharedLibraryAction(selectedBudgetPriorityLibrary).then(() => isShared = isSharedLibrary.value);
                }
                //update budget library sharing
                BudgetPriorityService.upsertOrDeleteBudgetPriorityLibraryUsers(selectedBudgetPriorityLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                    {
                        resetPage();
                    }
            });
        }
    }
    function setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: BudgetPriorityLibrary = emptyBudgetPriorityLibrary;
        stateBudgetPriorityLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }
    function populateEmptyBudgetPercentagePairs(budgetPriorites: BudgetPriority[]) {
        budgetPriorites.forEach(item => {
            if (item.budgetPercentagePairs.length === 0) {
                item.budgetPercentagePairs = createNewBudgetPercentagePairsFromBudgets();
            }
        });
    }
    function initializePages(){
        const { sortBy, descending, page, rowsPerPage } = pagination.value;
        const request: PagingRequest<BudgetPriority>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: ''
        };
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            BudgetPriorityService.getScenarioBudgetPriorityPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    currentPage.value = sortByProperty("priorityLevel", data.items);
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;

                    populateEmptyBudgetPercentagePairs(currentPage.value);
                }
                setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                loadedParentName = parentLibraryName; //store original
                scenarioLibraryIsModified = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
            });
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
