<template>
    <v-layout column>
        <v-flex xs12>
        <v-layout justify-space-between>
            <v-flex xs4 class="ghd-constant-header">
                <v-layout column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Deficient Condition Goal Library</v-subheader>
                    <v-select
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                </v-layout>
            </v-flex>
            <v-flex xs4 class="ghd-constant-header">
                <div style="padding-top: 15px !important">
                    <v-btn v-if="hasScenario" 
                        class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                        @click="importLibrary()"
                        :disabled="importLibraryDisabled">
                        Import
                    </v-btn>
                </div>
                
                <v-layout v-if='hasSelectedLibrary && !hasScenario' style="padding-top: 11px; padding-left: 10px">
                    <div class="header-text-content owner-padding" style="padding-top: 7px;">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                    </div>
                    <v-divider class="owner-shared-divider" vertical
                        v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                    </v-divider>
                    <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot: badge>
                        <span>Shared</span>
                        </template>
                        </v-badge>
                        <v-btn @click='onShowShareDeficientConditionGoalLibraryDialog(selectedDeficientConditionGoalLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs4 class="ghd-constant-header">
                <v-layout align-end style="padding-top: 18px !important;">
                    <v-spacer></v-spacer>
                    <v-btn
                        @click="showCreateDeficientConditionGoalDialog = true"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="hasSelectedLibrary || hasScenario"
                        outline>
                        Add Deficient Condition Goal
                    </v-btn>
                    <v-btn @click="onShowCreateDeficientConditionGoalLibraryDialog(false)"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="!hasScenario"
                        outline>    
                        Create New Library        
                    </v-btn>
                </v-layout>
            </v-flex>
                   
        </v-layout>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <div class="deficients-data-table">
                <v-data-table
                    :headers="deficientConditionGoalGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :total-items="totalItems"
                    :rows-per-page-items=[5,10,25]
                    sort-icon=$vuetify.icons.ghd-table-sort
                    class=" ghd-table v-table__overflow"
                    item-key="id"
                    select-all
                    v-model="selectedGridRows"
                >
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox
                                hide-details
                                primary
                                v-model="props.selected"
                            ></v-checkbox>
                        </td>
                        <td v-for="header in deficientConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog v-if="header.value !== 'criterionLibrary' && header.value !== 'action'"
                                    :return-value.sync="props.item[header.value]"
                                    @save="onEditDeficientConditionGoalProperty(props.item,header.value,props.item[header.value])"
                                    large
                                    lazy
                                    persistent>
                                    <v-text-field v-if="header.value !== 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                    <v-text-field v-if="header.value === 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty,
                                            rules['generalRules'].valueIsWithinRange(props.item[header.value],[0, 100])]"/>

                                    <template slot="input">
                                        <v-text-field v-if="header.value === 'name'"
                                            label="Edit"
                                            single-line
                                            v-model="props.item[header.value]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-select v-if="header.value === 'attribute'"
                                            :items="numericAttributeNames"
                                            append-icon=$vuetify.icons.ghd-down
                                            label="Select an Attribute"
                                            v-model="props.item[header.value]"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty]">
                                        </v-select>

                                        <v-text-field v-if="header.value === 'deficientLimit'"
                                            label="Edit"
                                            single-line
                                            v-model="props.item[header.value]"
                                            :mask="'##########'"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-text-field v-if="header.value === 'allowedDeficientPercentage'"
                                            label="Edit"
                                            single-line
                                            v-model.number="props.item[header.value]"
                                            :mask="'###'"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty,
                                                rules['generalRules'].valueIsWithinRange(
                                                    props.item[header.value],[0, 100])]"/>
                                    </template>
                                </v-edit-dialog>
                                
                                <v-layout
                                    v-if="header.value === 'criterionLibrary'"
                                    align-center
                                    style="flex-wrap:nowrap">
                                    <v-menu
                                        bottom
                                        min-height="500px"
                                        min-width="500px">
                                        <template slot="activator">
                                            <v-text-field
                                                readonly
                                                class="sm-txt"
                                                :value="props.item.criterionLibrary.mergedCriteriaExpression"/>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    :value="props.item.criterionLibrary.mergedCriteriaExpression"
                                                    full-width
                                                    no-resize
                                                    outline
                                                    readonly
                                                    rows="5"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                                        @click="onShowCriterionLibraryEditorDialog(props.item)"
                                        class="ghd-blue"
                                        icon>
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-layout>
                                <div v-if="header.value === 'action'">
                                    <v-btn @click="onRemoveSelectedDeficientConditionGoal(props.item.id)"  class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </div>                               
                            </div>
                        </td>
                    </template>
                </v-data-table> 
                <v-btn :disabled="selectedDeficientConditionGoalIds.length === 0"
                    @click="onRemoveSelectedDeficientConditionGoals"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    flat>
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
                        v-model="selectedDeficientConditionGoalLibrary.description"
                        @input='checkHasUnsavedChanges()'
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout justify-center>
                <v-btn
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    :disabled="!hasUnsavedChanges"
                    flat>
                    Cancel
                </v-btn>
                <v-btn
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                    flat>
                    Delete Library
                </v-btn>    
                <v-btn
                    @click="onShowCreateDeficientConditionGoalLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    :disabled="disableCrudButtons()"
                    outline>
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioDeficientConditionGoals"
                    class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges">
                    Save
                </v-btn>
                <v-btn
                    @click="onUpsertDeficientConditionGoalLibrary"
                    class='ghd-blue-bg white--text ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges">
                    Update Library
                </v-btn>               
                       
            </v-layout>
        </v-flex>

        <ConfirmBeforeDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateDeficientConditionGoalLibraryDialog
            :dialogData="createDeficientConditionGoalLibraryDialogData"
            @submit="onSubmitCreateDeficientConditionGoalLibraryDialogResult"
        />

        <CreateDeficientConditionGoalDialog
            :showDialog="showCreateDeficientConditionGoalDialog"
            :currentNumberOfDeficientConditionGoals="
                selectedDeficientConditionGoalLibrary.deficientConditionGoals
                    .length
            "
            @submit="onAddDeficientConditionGoal"
        />
        <ShareDeficientConditionGoalLibraryDialog :dialogData="shareDeficientConditionGoalLibraryDialogData"
            @submit="onShareDeficientConditionGoalDialogSubmit" 
        />
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onEditDeficientConditionGoalCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, Mutation, State } from 'vuex-class';
import {
    DeficientConditionGoal,
    DeficientConditionGoalLibrary,
    DeficientConditionGoalLibraryUser,
    emptyDeficientConditionGoal,
    emptyDeficientConditionGoalLibrary,
} from '@/shared/models/iAM/deficient-condition-goal';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
  any,
    clone,
    contains,
    find,
    findIndex,
    isNil,
    prepend,
    propEq,
    update,
} from 'ramda';
import CreateDeficientConditionGoalDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalDialog.vue';
import {
    CreateDeficientConditionGoalLibraryDialogData,
    emptyCreateDeficientConditionGoalLibraryDialogData,
} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import CreateDeficientConditionGoalLibraryDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalLibraryDialog.vue';
import ShareDeficientConditionGoalLibraryDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/ShareDeficientConditionGoalLibraryDialog.vue';
import { Attribute } from '@/shared/models/iAM/attribute';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import DeficientConditionGoalService from '@/services/deficient-condition-goal.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { isNullOrUndefined } from 'util';
import { LibraryUser } from '@/shared/models/iAM/user';
import { emptyShareDeficientConditionGoalLibraryDialogData, ShareDeficientConditionGoalLibraryDialogData } from '@/shared/models/modals/share-deficient-condition-goal-data';

@Component({
    components: {
        CreateDeficientConditionGoalLibraryDialog,
        CreateDeficientConditionGoalDialog,
        ShareDeficientConditionGoalLibraryDialog,
        GeneralCriterionEditorDialog,
        ConfirmBeforeDeleteAlert: Alert,
    },
})
export default class DeficientConditionGoalEditor extends Vue {
    @State(
        state =>
            state.deficientConditionGoalModule.deficientConditionGoalLibraries,
    )
    stateDeficientConditionGoalLibraries: DeficientConditionGoalLibrary[];
    @State(
        state =>
            state.deficientConditionGoalModule
                .selectedDeficientConditionGoalLibrary,
    )
    stateSelectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary;
    @State(state => state.attributeModule.numericAttributeNames)
    stateNumericAttributes: Attribute[];
    @State(
        state =>
            state.deficientConditionGoalModule.scenarioDeficientConditionGoals,
    )
    stateScenarioDeficientConditionGoals: DeficientConditionGoal[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.deficientConditionGoalModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @State(state => state.deficientConditionGoalModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedDeficientConditionGoalLibrary') getIsSharedLibraryAction: any;

    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getDeficientConditionGoalLibraries')
    getDeficientConditionGoalLibrariesAction: any;
    @Action('selectDeficientConditionGoalLibrary')
    selectDeficientConditionGoalLibraryAction: any;
    @Action('upsertDeficientConditionGoalLibrary')
    upsertDeficientConditionGoalLibraryAction: any;
    @Action('deleteDeficientConditionGoalLibrary')
    deleteDeficientConditionGoalLibraryAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioDeficientConditionGoals')
    getScenarioDeficientConditionGoalsAction: any;
    @Action('upsertScenarioDeficientConditionGoals')
    upsertScenarioDeficientConditionGoalsAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Mutation('addedOrUpdatedDeficientConditionGoalLibraryMutator') addedOrUpdatedDeficientConditionGoalLibraryMutator: any;
    @Mutation('selectedDeficientConditionGoalLibraryMutator') selectedDeficientConditionGoalLibraryMutator: any;

    @Getter('getNumericAttributes') getNumericAttributesGetter: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;

    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    selectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = clone(
        emptyDeficientConditionGoalLibrary,
    );
    hasSelectedLibrary: boolean = false;
    deficientConditionGoalGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '15%',
        },
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '12%',
        },
        {
            text: 'Deficient Limit',
            value: 'deficientLimit',
            align: 'left',
            sortable: false,
            class: '',
            width: '8%',
        },
        {
            text: 'Allowed Deficient Percentage',
            value: 'allowedDeficientPercentage',
            align: 'left',
            sortable: false,
            class: '',
            width: '11%',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '40%',
        },
        {
            text: 'Action',
            value: 'action',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        }
    ];
    numericAttributeNames: string[] = [];
    selectedGridRows: DeficientConditionGoal[] = [];
    selectedDeficientConditionGoalIds: string[] = [];
    selectedDeficientConditionGoalForCriteriaEdit: DeficientConditionGoal = clone(
        emptyDeficientConditionGoal,
    );
    showCreateDeficientConditionGoalDialog: boolean = false;
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    createDeficientConditionGoalLibraryDialogData: CreateDeficientConditionGoalLibraryDialogData = clone(
        emptyCreateDeficientConditionGoalLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    currentUrl: string = window.location.href;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;
    importLibraryDisabled: boolean = true;
    scenarioHasCreatedNew: boolean = false;

    addedRows: DeficientConditionGoal[] = [];
    updatedRowsMap:Map<string, [DeficientConditionGoal, DeficientConditionGoal]> = new Map<string, [DeficientConditionGoal, DeficientConditionGoal]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: DeficientConditionGoal[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: DeficientConditionGoal[] = [];
    initializing: boolean = true;
    isShared: boolean = false;

    shareDeficientConditionGoalLibraryDialogData: ShareDeficientConditionGoalLibraryDialogData = clone(emptyShareDeficientConditionGoalLibraryDialogData);

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = null;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getDeficientConditionGoalLibrariesAction().then(() => {
                vm.numericAttributeNames = getPropertyValues('name', vm.getNumericAttributesGetter);
                vm.getHasPermittedAccessAction().then(() => {
                    if (to.path.indexOf(ScenarioRoutePaths.DeficientConditionGoal) !== -1) {
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
                });     
            });       
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateDeficientConditionGoalLibraries')
    onStateDeficientConditionGoalLibrariesChanged() {
        this.librarySelectItems = this.stateDeficientConditionGoalLibraries.map(
            (library: DeficientConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
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
        if(!this.hasScenario || isNil(this.librarySelectItemValue))
            this.selectDeficientConditionGoalLibraryAction({
                libraryId: this.librarySelectItemValue,
            });
        else if(!isNil(this.librarySelectItemValue) && !this.scenarioHasCreatedNew)
        {
            this.importLibraryDisabled = false;
        }

        this.scenarioHasCreatedNew = false;
    }

    @Watch('stateSelectedDeficientConditionGoalLibrary')
    onStateSelectedDeficientConditionGoalLibraryChanged() {
        this.selectedDeficientConditionGoalLibrary = clone(
            this.stateSelectedDeficientConditionGoalLibrary,
        );
    }

    @Watch('selectedDeficientConditionGoalLibrary')
    onSelectedDeficientConditionGoalLibraryChanged() {
        if (!isNullOrUndefined(this.selectedDeficientConditionGoalLibrary)) {
            this.hasSelectedLibrary = this.selectedDeficientConditionGoalLibrary.id !== this.uuidNIL;
            
        }
        if (!isNullOrUndefined(this.selectedDeficientConditionGoalLibrary.id) ) {
            this.getIsSharedLibraryAction(this.selectedDeficientConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
        }    
        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        this.clearChanges();
        this.initializing = false;
        if(this.hasSelectedLibrary)
            this.onPaginationChanged();
        
        this.checkHasUnsavedChanges();
    }

    @Watch('selectedGridRows')
    onSelectedDeficientRowsChanged() {
        this.selectedDeficientConditionGoalIds = getPropertyValues('id', this.selectedGridRows,) as string[];
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
    }

    @Watch('stateScenarioDeficientConditionGoals')
    onStateScenarioDeficientConditionGoalsChanged() {
        if (this.hasScenario) {
            this.currentPage = clone(this.stateScenarioDeficientConditionGoals);
        }
    }

    @Watch('currentPage')
    onCurrentPageChanged() {

    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
        if (!isNullOrUndefined(this.selectDeficientConditionGoalLibrary)) {
            this.selectDeficientConditionGoalLibrary.isShared = this.isShared;
        } 
    }

    @Watch('pagination')
    onPaginationChanged() {
        if(this.initializing)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.pagination;
        if (!isNullOrUndefined(this.selectedDeficientConditionGoalLibrary.id) ) {
            this.getIsSharedLibraryAction(this.selectedDeficientConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
        }

        const request: PagingRequest<DeficientConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.librarySelectItemValue !== null && this.importLibraryDisabled ? this.librarySelectItemValue : null,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
        else if(this.hasSelectedLibrary)
             DeficientConditionGoalService.getLibraryDeficientConditionGoalPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });     
    }

    importLibrary() {
        this.selectDeficientConditionGoalLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
        this.importLibraryDisabled = true;
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedDeficientConditionGoalLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedDeficientConditionGoalLibrary.owner) == getUserName();
    }

    onShowCreateDeficientConditionGoalLibraryDialog(createExistingLibraryAsNew: boolean) {
        this.createDeficientConditionGoalLibraryDialogData = {
            showDialog: true,
            deficientConditionGoals: createExistingLibraryAsNew
                ? this.currentPage
                : [],
        };
    }

    onSubmitCreateDeficientConditionGoalLibraryDialogResult(library: DeficientConditionGoalLibrary,) {
        this.createDeficientConditionGoalLibraryDialogData = clone(emptyCreateDeficientConditionGoalLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.deficientConditionGoals.length == 0 || !this.hasSelectedLibrary? null : this.selectedDeficientConditionGoalLibrary.id,
                    rowsForDeletion: library.deficientConditionGoals === [] ? [] : this.deletionIds,
                    updateRows: library.deficientConditionGoals === [] ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.deficientConditionGoals === [] ? [] : this.addedRows,
                 },
                 scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.hasCreatedLibrary = true;
                    this.librarySelectItemValue = library.id;
                    
                    if(library.deficientConditionGoals === []){
                        this.clearChanges();
                    }

                    if(this.hasScenario){
                        this.scenarioHasCreatedNew = true;
                        this.importLibraryDisabled = true;
                    }
                    
                    this.addedOrUpdatedDeficientConditionGoalLibraryMutator(library);
                    this.selectedDeficientConditionGoalLibraryMutator(library.id);
                    this.addSuccessNotificationAction({message:'Added deficient condition goal library'})
                }               
            })
        }
    }

    onAddDeficientConditionGoal(newDeficientConditionGoal: DeficientConditionGoal) {
        this.showCreateDeficientConditionGoalDialog = false;

        if (!isNil(newDeficientConditionGoal)) {
            this.addedRows.push(newDeficientConditionGoal);
            this.onPaginationChanged()
        }
    }

    onEditDeficientConditionGoalProperty(deficientConditionGoal: DeficientConditionGoal, property: string, value: any) {
        this.onUpdateRow(deficientConditionGoal.id, clone(deficientConditionGoal))
        this.onPaginationChanged();
    }

    onShowCriterionLibraryEditorDialog(deficientConditionGoal: DeficientConditionGoal,) {
        this.selectedDeficientConditionGoalForCriteriaEdit = clone(
            deficientConditionGoal,
        );

        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: deficientConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onEditDeficientConditionGoalCriterionLibrary(criterionExpression: string,) {
        this.criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && this.selectedDeficientConditionGoalForCriteriaEdit.id !== this.uuidNIL) {
            if(this.selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.onUpdateRow(this.selectedDeficientConditionGoalForCriteriaEdit.id,
             { ...this.selectedDeficientConditionGoalForCriteriaEdit, 
                criterionLibrary: {... this.selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression}})                
            this.onPaginationChanged();
        }

        this.selectedDeficientConditionGoalForCriteriaEdit = clone(
            emptyDeficientConditionGoal,
        );
    }

    onUpsertDeficientConditionGoalLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal> = {
                library: this.selectedDeficientConditionGoalLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: this.selectedDeficientConditionGoalLibrary.id === this.uuidNIL ? null : this.selectedDeficientConditionGoalLibrary.id,
                rowsForDeletion: this.deletionIds,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                addedRows: this.addedRows,
                },
                scenarioId: null
        }
        DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.addedOrUpdatedDeficientConditionGoalLibraryMutator(this.selectedDeficientConditionGoalLibrary);
                this.selectedDeficientConditionGoalLibraryMutator(this.selectedDeficientConditionGoalLibrary.id);
                this.addSuccessNotificationAction({message: "Updated deficient condition goal library",});
            }
        });
    }

    onUpsertScenarioDeficientConditionGoals() {
        DeficientConditionGoalService.upsertScenarioDeficientConditionGoals({
            libraryId: this.selectedDeficientConditionGoalLibrary.id === this.uuidNIL ? null : this.selectedDeficientConditionGoalLibrary.id,
            rowsForDeletion: this.deletionIds,
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows           
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges();
                this.librarySelectItemValue = null;
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified scenario's deficient condition goals"});
                this.importLibraryDisabled = true;
            }           
        });
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.clearChanges();
                this.resetPage();
                this.importLibraryDisabled = true;
            }
        });
    }

    onRemoveSelectedDeficientConditionGoals() {
        this.selectedDeficientConditionGoalIds.forEach(_ => {
            this.removeRowLogic(_);
        });

        this.selectedDeficientConditionGoalIds = [];
        this.onPaginationChanged();
    }

    onRemoveSelectedDeficientConditionGoal(id: string){
        this.removeRowLogic(id);
        this.onPaginationChanged();
    }

    removeRowLogic(id: string){
        if(isNil(find(propEq('id', id), this.addedRows))){
            this.deletionIds.push(id);
            if(!isNil(this.updatedRowsMap.get(id)))
                this.updatedRowsMap.delete(id)
        }           
        else{          
            this.addedRows = this.addedRows.filter((row) => row.id !== id)
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
            this.librarySelectItemValue = null;
            this.deleteDeficientConditionGoalLibraryAction({
                libraryId: this.selectedDeficientConditionGoalLibrary.id,
            });
        }
    }

    disableCrudButtons() {
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
            (deficientGoal: DeficientConditionGoal) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedDeficientConditionGoalLibrary.name,
                ) === true &&
                dataIsValid
            );
        }
        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    //paging

    onUpdateRow(rowId: string, updatedRow: DeficientConditionGoal){
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
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.stateSelectedDeficientConditionGoalLibrary, this.selectedDeficientConditionGoalLibrary))
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

    onShowShareDeficientConditionGoalLibraryDialog(deficientConditionGoalLibrary: DeficientConditionGoalLibrary) {
        this.shareDeficientConditionGoalLibraryDialogData = {
            showDialog:true,
            deficientConditionGoalLibrary: clone(deficientConditionGoalLibrary)
        }
    }

    onShareDeficientConditionGoalDialogSubmit(deficientConditionGoalLibraryUsers: DeficientConditionGoalLibraryUser[]) {
        this.shareDeficientConditionGoalLibraryDialogData = clone(emptyShareDeficientConditionGoalLibraryDialogData);

                if (!isNil(deficientConditionGoalLibraryUsers) && this.selectedDeficientConditionGoalLibrary.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    deficientConditionGoalLibraryUsers.forEach((deficientConditionGoalLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && deficientConditionGoalLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && deficientConditionGoalLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: deficientConditionGoalLibraryUser.userId,
                            userName: deficientConditionGoalLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNullOrUndefined(this.selectedDeficientConditionGoalLibrary.id) ) {
                        this.getIsSharedLibraryAction(this.selectedDeficientConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
                    }
                    //update budget library sharing
                    DeficientConditionGoalService.upsertOrDeleteDeficientConditionGoalLibraryUsers(this.selectedDeficientConditionGoalLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                        {
                            this.resetPage();
                        }
                    });
                }
    }


    initializePages(){
        const request: PagingRequest<DeficientConditionGoal>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(this.selectedScenarioId, request).then(response => {
                this.initializing = false
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
    }
}
</script>

<style>
.deficients-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.deficients-data-table .v-menu--inline,
.deficient-criteria-output {
    width: 100%;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
