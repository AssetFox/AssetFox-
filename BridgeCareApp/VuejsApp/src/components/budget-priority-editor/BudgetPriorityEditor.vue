<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between row >              
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Select Budget Priority Library</v-subheader>
                            <v-select :items='librarySelectItems' 
                                append-icon=$vuetify.icons.ghd-down
                                outline                           
                                v-model='librarySelectItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                            </v-select>    
                             <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario">Based on: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></div>                       
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
                        <v-badge v-show="isShared" style="padding: 10px">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowShareBudgetPriorityLibraryDialog(selectedBudgetPriorityLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
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
                <v-data-table :headers='budgetPriorityGridHeaders' 
                              :items='budgetPriorityGridRows'
                              :pagination.sync="pagination"
                              :must-sort='true'
                              :total-items="totalItems"
                              :rows-per-page-items=[5,10,25]
                              class='v-table__overflow ghd-table' item-key='id' select-all
                              sort-icon=$vuetify.icons.ghd-table-sort                              
                              v-model='selectedBudgetPriorityGridRows' >
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
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsNotUnique(props.item[header.value], currentPriorityList)]" />
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
                                                    <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
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
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
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
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
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
                                @input='checkHasUnsavedChanges()'>
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
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter, Mutation } from 'vuex-class';
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
import CreatePriorityDialog
    from '@/components/budget-priority-editor/budget-priority-editor-dialogs/CreateBudgetPriorityDialog.vue';
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
import ShareBudgetPriorityLibraryDialog from './budget-priority-editor-dialogs/ShareBudgetPriorityLibraryDialog.vue';
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

const ObjectID = require('bson-objectid');

@Component({
    components: {
        CreatePriorityLibraryDialog, CreatePriorityDialog, GeneralCriterionEditorDialog, ConfirmDeleteAlert: Alert, ShareBudgetPriorityLibraryDialog
    },
})
export default class BudgetPriorityEditor extends Vue {
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];
    @State(state => state.budgetPriorityModule.budgetPriorityLibraries) stateBudgetPriorityLibraries: BudgetPriorityLibrary[];
    @State(state => state.budgetPriorityModule.selectedBudgetPriorityLibrary) stateSelectedBudgetPriorityLibrary: BudgetPriorityLibrary;
    @State(state => state.budgetPriorityModule.scenarioBudgetPriorities) stateScenarioBudgetPriorities: BudgetPriority[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.budgetPriorityModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @State(state => state.budgetPriorityModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedBudgetPriorityLibrary') getIsSharedLibraryAction: any;
    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getBudgetPriorityLibraries') getBudgetPriorityLibrariesAction: any;
    @Action('selectBudgetPriorityLibrary') selectBudgetPriorityLibraryAction: any;
    @Action('upsertBudgetPriorityLibrary') upsertBudgetPriorityLibraryAction: any;
    @Action('deleteBudgetPriorityLibrary') deleteBudgetPriorityLibraryAction: any;
    @Action('getScenarioSimpleBudgetDetails') getScenarioSimpleBudgetDetailsAction: any;
    @Action('upsertScenarioBudgetPriorities') upsertScenarioBudgetPrioritiesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @Mutation('budgetPriorityLibraryMutator') budgetPriorityLibraryMutator: any;
    @Mutation('selectedBudgetPriorityLibraryMutator') selectedBudgetPriorityLibraryMutator: any;

    addedRows: BudgetPriority[] = [];
    updatedRowsMap:Map<string, [BudgetPriority, BudgetPriority]> = new Map<string, [BudgetPriority, BudgetPriority]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: BudgetPriority[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: BudgetPriority[] = [];
    initializing: boolean = true;
    currentPriorityList: number[] = [];

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = null;

    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    shareBudgetPriorityLibraryDialogData: ShareBudgetPriorityLibraryDialogData = clone(emptyShareBudgetPriorityLibraryDialogData);
    isShared: boolean = false;

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
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
    createBudgetPriorityLibraryDialogData: CreateBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;  
    disableCrudButtonsResult: boolean = false;
    checkBoxChanged: boolean = false;
    hasLibraryEditPermission: boolean = false;
    hasCreatedLibrary: boolean = false;
    parentLibraryName: string = "None";
    parentLibraryId: string = "";
    parentModifiedFlag: boolean = false;
    scenarioLibraryIsModified: boolean = false;
    loadedParentName: string = "";
    loadedParentId: string = "";
    newLibrarySelection: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getBudgetPriorityLibrariesAction();
            vm.getHasPermittedAccessAction();

            if (to.path.indexOf(ScenarioRoutePaths.BudgetPriority) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }
                vm.hasScenario = true;
                vm.getScenarioSimpleBudgetDetailsAction({ scenarioId: vm.selectedScenarioId }).then(() => {
                    vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                        vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });        
                        vm.initializePages();
                    });                                        
                });             
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
    //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChangedCheckUnsaved(){
        if(this.hasScenario){
            this.onSelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged) {
            this.CheckUnsavedDialog(this.onSelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            });
        }
        this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
        this.newLibrarySelection = true;
        this.librarySelectItemValueAllowedChanged = true;
    }
    onSelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue;
        this.selectBudgetPriorityLibraryAction({ libraryId: this.librarySelectItemValue });
    }

    @Watch('stateSelectedBudgetPriorityLibrary')
    onStateSelectedPriorityLibraryChanged() {
        this.selectedBudgetPriorityLibrary = clone(this.stateSelectedBudgetPriorityLibrary);
    }

    @Watch('selectedBudgetPriorityLibrary')
    onSelectedPriorityLibraryChanged() {
        this.hasSelectedLibrary = this.selectedBudgetPriorityLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }
        
        this.updatedRowsMap.clear();
        this.deletionIds = [];
        this.addedRows = [];
        this.initializing = false;
        if(this.hasSelectedLibrary)
            this.onPaginationChanged();
    }

    @Watch('stateScenarioBudgetPriorities')
    onStateScenarioBudgetPrioritiesChanged() {
        if (this.hasScenario) {
            this.onPaginationChanged();
        }
    }

    @Watch('currentPage')
    onBudgetPrioritiesChanged() {
        if(this.hasScenario){
            const allBudgetPercentagePairsMatchBudgets: boolean = this.currentPage
            .every((budgetPriority: BudgetPriority) => this.hasBudgetPercentagePairsThatMatchBudgets(budgetPriority));
            if (!allBudgetPercentagePairsMatchBudgets) {
                this.syncBudgetPercentagePairsWithBudgets();
                return;
            }
        }
        this.setGridCriteriaColumnWidth();
        this.setGridHeaders();
        this.setGridData();
        this.currentPage.forEach((item) => {
            this.currentPriorityList.push(item.priorityLevel);
        });
        // Get parent name from library id
        this.librarySelectItems.forEach(library => {
            if (library.value === this.parentLibraryId) {
                this.parentLibraryName = library.text;
            }
        });
    }

    @Watch('selectedBudgetPriorityGridRows')
    onSelectedPriorityRowsChanged() {
        this.selectedBudgetPriorityIds = getPropertyValues('id', this.selectedBudgetPriorityGridRows) as string[];
    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
    }

    @Watch('pagination')
    onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        const request: PagingRequest<BudgetPriority>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.librarySelectItemValue !== null ? this.librarySelectItemValue : null,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
                isModified: this.scenarioLibraryIsModified
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            BudgetPriorityService.getScenarioBudgetPriorityPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
        else if(this.hasSelectedLibrary)
             BudgetPriorityService.getLibraryBudgetPriorityPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;

                    if (!isNullOrUndefined(this.selectedBudgetPriorityLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedBudgetPriorityLibrary).then(this.isShared = this.isSharedLibrary);
                    }           
                }
            });     
    }

    @Watch('deletionIds')
    onDeletionIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedRows')
    onAddedRowsChanged(){
        this.checkHasUnsavedChanges();
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

    syncBudgetPercentagePairsWithBudgets() {// this might cause problems
        const budgetPriorities: BudgetPriority[] = clone(this.currentPage);

        if (hasValue(this.stateScenarioSimpleBudgetDetails)) {
            var ids = this.stateScenarioSimpleBudgetDetails.map(_ => _.id);
            budgetPriorities.forEach((budgetPriority: BudgetPriority) => {
                if (!this.hasBudgetPercentagePairsThatMatchBudgets(budgetPriority)) {
                    budgetPriority.budgetPercentagePairs = budgetPriority.budgetPercentagePairs.filter(_ => ids.includes(_.budgetId))
                    var newPairs = this.stateScenarioSimpleBudgetDetails.filter(_ => !budgetPriority.budgetPercentagePairs.some(__ => __.budgetId == _.id)).map((simpleBudgetDetail: SimpleBudgetDetail) => ({
                        id: getNewGuid(),
                        budgetId: simpleBudgetDetail.id,
                        budgetName: simpleBudgetDetail.name,
                        percentage: 100,
                    })) as BudgetPercentagePair[];
                    budgetPriority.budgetPercentagePairs = budgetPriority.budgetPercentagePairs.concat(newPairs)
                    this.onUpdateRow(budgetPriority.id, budgetPriority);
                }
            });
            this.onPaginationChanged();
        }
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
        this.budgetPriorityGridRows = this.currentPage.map((budgetPriority: BudgetPriority) => {
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
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedBudgetPriorityLibrary.owner) == getUserName();
    }

    onShowCreateBudgetPriorityLibraryDialog(createAsNewLibrary: boolean) {
        this.createBudgetPriorityLibraryDialogData = {
            showDialog: true,
            budgetPriorities: createAsNewLibrary ? this.currentPage : [],
        };
    }

    onSubmitCreateBudgetPriorityLibraryDialogResult(budgetPriorityLibrary: BudgetPriorityLibrary) {
        this.createBudgetPriorityLibraryDialogData = clone(emptyCreateBudgetPriorityLibraryDialogData);
        if (!isNil(budgetPriorityLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: budgetPriorityLibrary,    
                isNewLibrary: true,           
                syncModel: {
                    libraryId: budgetPriorityLibrary.budgetPriorities.length == 0 || !this.hasSelectedLibrary ? null : this.selectedBudgetPriorityLibrary.id,
                    rowsForDeletion: budgetPriorityLibrary.budgetPriorities === [] ? [] : this.deletionIds,
                    updateRows: budgetPriorityLibrary.budgetPriorities === [] ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: budgetPriorityLibrary.budgetPriorities === [] ? [] : this.addedRows,
                    isModified: false
                },
                scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            BudgetPriorityService.upsertBudgetPriorityLibrary(upsertRequest).then(() => {
                this.hasCreatedLibrary = true;
                this.librarySelectItemValue = budgetPriorityLibrary.id;
                
                if(budgetPriorityLibrary.budgetPriorities === []){
                    this.clearChanges();
                }

                this.budgetPriorityLibraryMutator(budgetPriorityLibrary);
                this.selectedBudgetPriorityLibraryMutator(budgetPriorityLibrary.id);
                this.addSuccessNotificationAction({message:'Added budget priority library'})
            })
        }
    }

    onAddBudgetPriority(newBudgetPriority: BudgetPriority) {
        this.showCreateBudgetPriorityDialog = false;

        if (!isNil(newBudgetPriority)) {
            if (this.hasScenario && hasValue(this.stateScenarioSimpleBudgetDetails)) {
                newBudgetPriority.budgetPercentagePairs = this.createNewBudgetPercentagePairsFromBudgets();
            }

            this.addedRows.push(newBudgetPriority);
            this.onPaginationChanged()
        }
    }

    onEditBudgetPriority(budgetPriorityGridDatum: BudgetPriorityGridDatum, property: string, value: any) {
        if (any(propEq('id', budgetPriorityGridDatum.id), this.currentPage)) {
            let budgetPriority: BudgetPriority = find(
                propEq('id', budgetPriorityGridDatum.id), this.currentPage,
            ) as BudgetPriority;

            if (property === 'year' && (!hasValue(value) || parseInt(value) === 0)) {
                budgetPriority.year = null;
            } else {
                budgetPriority = setItemPropertyValue(property, value, budgetPriority) as BudgetPriority;
            }
            this.onUpdateRow(budgetPriority.id, clone(budgetPriority))
            this.onPaginationChanged();
        }
    }

    onEditBudgetPercentagePair(budgetPriorityGridDatum: BudgetPriorityGridDatum, budgetName: string, percentage: number) {
        const budgetPriority: BudgetPriority = find(
            propEq('id', budgetPriorityGridDatum.id), this.currentPage,
        ) as BudgetPriority;

        const budgetPercentagePair: BudgetPercentagePair = find(
            propEq('budgetName', budgetName), budgetPriority.budgetPercentagePairs,
        ) as BudgetPercentagePair;

        this.onUpdateRow(budgetPriority.id, {
                ...budgetPriority, budgetPercentagePairs: update(
                    findIndex(propEq('id', budgetPercentagePair.id), budgetPriority.budgetPercentagePairs),
                    setItemPropertyValue('percentage', percentage, budgetPercentagePair) as BudgetPercentagePair,
                    budgetPriority.budgetPercentagePairs,
                ),
            } as BudgetPriority)

        this.onPaginationChanged();
    }

    onShowCriterionLibraryEditorDialog(budgetPriorityGridDatum: BudgetPriorityGridDatum) {
        this.selectedBudgetPriorityForCriteriaEdit = find(
            propEq('id', budgetPriorityGridDatum.id), this.currentPage,
        ) as BudgetPriority;

        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.selectedBudgetPriorityForCriteriaEdit.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        this.criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && this.selectedBudgetPriorityForCriteriaEdit.id !== this.uuidNIL) {
            if(this.selectedBudgetPriorityForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedBudgetPriorityForCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.onUpdateRow(this.selectedBudgetPriorityForCriteriaEdit.id, 
            { ...this.selectedBudgetPriorityForCriteriaEdit, 
            criterionLibrary: {...this.selectedBudgetPriorityForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} })

            this.onPaginationChanged();
        }

        this.selectedBudgetPriorityForCriteriaEdit = clone(emptyBudgetPriority);
    }

    onUpsertScenarioBudgetPriorities() {

        if (this.selectedBudgetPriorityLibrary.id === this.uuidNIL || this.hasUnsavedChanges && this.newLibrarySelection ===false) {this.scenarioLibraryIsModified = true;}
        else { this.scenarioLibraryIsModified = false; }

        BudgetPriorityService.upsertScenarioBudgetPriorities({
            libraryId: this.selectedBudgetPriorityLibrary.id === this.uuidNIL ? null : this.selectedBudgetPriorityLibrary.id,
            rowsForDeletion: this.deletionIds,
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows,
            isModified: this.scenarioLibraryIsModified
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.parentLibraryId = this.librarySelectItemValue ? this.librarySelectItemValue : "";
                this.clearChanges();
                this.librarySelectItemValue = null;
                this.addSuccessNotificationAction({message: "Modified scenario's budget priorities"});
                this.currentPage = sortByProperty("priorityLevel", this.currentPage);
            }           
        });
    }

    onUpsertBudgetPriorityLibrary() {
        const budgetPriorityLibrary: BudgetPriorityLibrary = {
            ...clone(this.selectedBudgetPriorityLibrary),
            budgetPriorities: clone(this.currentPage),
        };
        this.upsertBudgetPriorityLibraryAction(budgetPriorityLibrary);

        const upsertRequest: LibraryUpsertPagingRequest<BudgetPriorityLibrary, BudgetPriority> = {
                library: this.selectedBudgetPriorityLibrary,
                isNewLibrary: false,
                 syncModel: {
                    libraryId: this.selectedBudgetPriorityLibrary.id === this.uuidNIL ? null : this.selectedBudgetPriorityLibrary.id,
                    rowsForDeletion: this.deletionIds,
                    updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: this.addedRows,
                    isModified: false
                 }
                 , scenarioId: null
        }
        BudgetPriorityService.upsertBudgetPriorityLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.budgetPriorityLibraryMutator(this.selectedBudgetPriorityLibrary);
                this.selectedBudgetPriorityLibraryMutator(this.selectedBudgetPriorityLibrary.id);
                this.addSuccessNotificationAction({message: "Updated budget priority library",});
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
        this.parentLibraryName = this.loadedParentName;
        this.parentLibraryId = this.loadedParentId;
    }

    onRemoveBudgetPriorities() {
        this.selectedBudgetPriorityIds.forEach(_ => {
            this.removePriorityLogic(_);
        });

        this.selectedBudgetPriorityIds = [];
        this.onPaginationChanged();
    }

    onRemoveBudgetPriority(id: string){
        this.removePriorityLogic(id)
        this.onPaginationChanged();
    }

    removePriorityLogic(id: string){
        if(isNil(find(propEq('id', id), this.addedRows))){
            this.deletionIds.push(id);
            if(!isNil(this.updatedRowsMap.get(id)))
                this.updatedRowsMap.delete(id)
        }           
        else{          
            this.addedRows = this.addedRows.filter((bp: BudgetPriority) => bp.id !== id)
        }  
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
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid: boolean = rows.every((budgetPriority: BudgetPriority) => {
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

    onUpdateRow(rowId: string, updatedRow: BudgetPriority){
        if(any(propEq('id', rowId), this.addedRows)){
            const index = this.addedRows.findIndex(item => item.id == updatedRow.id)
            this.addedRows[index] = updatedRow;
            return;
        }

        let mapEntry = this.updatedRowsMap.get(rowId)

        if(isNil(mapEntry)){
            const row = this.rowCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
                this.updatedRowsMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            this.updatedRowsMap.delete(rowId)

        this.checkHasUnsavedChanges();
    }

    clearChanges(){
        this.updatedRowsMap.clear();
        this.addedRows = [];
        this.deletionIds = [];
    }

    resetPage(){
        this.pagination.page = 1;
        this.onPaginationChanged();
    }

    checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            this.deletionIds.length > 0 || 
            this.addedRows.length > 0 ||
            this.updatedRowsMap.size > 0 || 
            (this.hasScenario && this.hasSelectedLibrary) ||
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.selectedBudgetPriorityLibrary, this.stateSelectedBudgetPriorityLibrary))
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
    onShowShareBudgetPriorityLibraryDialog(budgetPriorityLibrary: BudgetPriorityLibrary) {
        this.shareBudgetPriorityLibraryDialogData = {
            showDialog:true,
            budgetPriorityLibrary: clone(budgetPriorityLibrary)
        }
    }

    onShareBudgetPriorityLibraryDialogSubmit(budgetPriorityLibraryUsers: BudgetPriorityLibraryUser[]) {
            this.shareBudgetPriorityLibraryDialogData = clone(emptyShareBudgetPriorityLibraryDialogData);

            if (!isNil(budgetPriorityLibraryUsers) && this.selectedBudgetPriorityLibrary.id !== getBlankGuid())
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

                if (!isNullOrUndefined(this.selectedBudgetPriorityLibrary.id) ) {
                            this.getIsSharedLibraryAction(this.selectedBudgetPriorityLibrary).then(this.isShared = this.isSharedLibrary);
                }
                //update budget library sharing
                BudgetPriorityService.upsertOrDeleteBudgetPriorityLibraryUsers(this.selectedBudgetPriorityLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                    {
                        this.resetPage();
                    }
            });
        }
    }
    setParentLibraryName(libraryId: string) {
        if (libraryId === "None") {
            this.parentLibraryName = "None";
            return;
        }
        let foundLibrary: BudgetPriorityLibrary = emptyBudgetPriorityLibrary;
        this.stateBudgetPriorityLibraries.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        this.parentLibraryId = foundLibrary.id;
        this.parentLibraryName = foundLibrary.name;
    }

    initializePages(){
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
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
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            BudgetPriorityService.getScenarioBudgetPriorityPage(this.selectedScenarioId, request).then(response => {
                this.initializing = false
                if(response.data){
                    let data = response.data as PagingPage<BudgetPriority>;
                    this.currentPage = sortByProperty("priorityLevel", data.items);
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
                this.setParentLibraryName(this.currentPage.length > 0 ? this.currentPage[0].libraryId : "None");
                this.loadedParentId = this.currentPage.length > 0 ? this.currentPage[0].libraryId : "";
                this.loadedParentName = this.parentLibraryName; //store original
                this.scenarioLibraryIsModified = this.currentPage.length > 0 ? this.currentPage[0].isModified : false;
            });
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
