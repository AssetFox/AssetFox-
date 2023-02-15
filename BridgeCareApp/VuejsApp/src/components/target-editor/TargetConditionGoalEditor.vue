<template>
    <v-layout column>
        <v-flex xs12>
           <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout column>
                        <v-subheader class="ghd-control-label ghd-md-gray">Target Condition Goal Library</v-subheader>
                        <v-select
                            class="ghd-select ghd-text-field ghd-text-field-border"
                            :items="librarySelectItems"
                            append-icon=$vuetify.icons.ghd-down
                            outline
                            v-model="librarySelectItemValue"
                            outlined
                        >
                        </v-select>
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout v-if="hasSelectedLibrary && ! hasScenario" style="padding-top: 10px; padding-left: 10px">
                        <div v-if="hasSelectedLibrary && !hasScenario" class="header-text-content owner-padding" style="padding-top: 7px;">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                        </div>
                        <v-divider vertical 
                            class="owner-shared-divider"
                            v-if="hasSelectedLibrary && !hasScenario"
                        >
                        </v-divider>
                        <v-badge v-show="isShared" style="padding: 10px">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowShareTargetConditionGoalLibraryDialog(selectedTargetConditionGoalLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                    <v-layout align-end style="padding-top: 18px !important;">
                        <v-spacer></v-spacer>
                        <v-btn outline
                            @click="showCreateTargetConditionGoalDialog = true"
                            class="ghd-control-border ghd-blue"
                            v-show="hasSelectedLibrary || hasScenario" 
                        >Add Target Condition Goal</v-btn>
                        <v-btn outline
                            @click="onShowCreateTargetConditionGoalLibraryDialog(false)"
                            class="ghd-control-border ghd-blue"
                            v-show="!hasScenario"
                        >
                        Create New Library
                        </v-btn>
                    </v-layout>
                </v-flex>
           </v-layout>
        </v-flex>
        <!-- </v-flex> -->
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <div class="targets-data-table">
                <v-data-table
                    :headers="targetConditionGoalGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :total-items="totalItems"
                    :rows-per-page-items=[5,10,25]
                    sort-icon=$vuetify.icons.ghd-table-sort                                    
                    class="elevation-1 fixed-header v-table__overflow"
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
                        <td v-for="header in targetConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog
                                    v-if="header.value !== 'criterionLibrary'"
                                    :return-value.sync="
                                        props.item[header.value]
                                    "
                                    @save="
                                        onEditTargetConditionGoalProperty(
                                            props.item,
                                            header.value,
                                            props.item[header.value],
                                        )
                                    "
                                    large
                                    lazy
                                    persistent
                                >
                                    <v-text-field
                                        v-if="header.value === 'year'"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                    />
                                    <v-text-field
                                        v-else
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                    <v-card-actions
                                        v-if="header.value === 'actions'"
                                        label="Actions"
                                    >
                                        <v-btn                                       
                                            @click="onShowCriterionLibraryEditorDialog(props.item)"
                                            class="ghd-blue"
                                            icon
                                        >
                                            <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                        </v-btn>
                                        <v-btn
                                            @click="onRemoveTargetConditionGoalsIcon(props.item)"
                                            class="ghd-blue"
                                            icon
                                        >
                                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                        </v-btn>
                                    </v-card-actions>

                                    <template slot="input">
                                        <v-select
                                            v-if="header.value === 'attribute'"
                                            :items="numericAttributeNames"
                                            append-icon=$vuetify.icons.ghd-down
                                            label="Select an Attribute"
                                            v-model="props.item.attribute"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <v-text-field
                                            v-if="header.value === 'year'"
                                            label="Edit"
                                            single-line
                                            :mask="'####'"
                                            v-model.number="
                                                props.item[header.value]
                                            "
                                        />
                                        <v-text-field
                                            v-if="header.value === 'target'"
                                            label="Edit"
                                            single-line
                                            :mask="'##########'"
                                            v-model.number="
                                                props.item[header.value]
                                            "
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <v-text-field
                                            v-if="header.value === 'name'"
                                            label="Edit"
                                            single-line
                                            v-model="props.item[header.value]"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                    </template>
                                </v-edit-dialog>
                                <v-layout
                                    v-else
                                    align-center
                                    row
                                    style="flex-wrap:nowrap"
                                >
                                    <v-menu
                                        bottom
                                        min-height="500px"
                                        min-width="500px"
                                    >
                                        <template slot="activator">
                                            <v-text-field
                                                readonly
                                                class="sm-txt"
                                                :value="
                                                    props.item.criterionLibrary
                                                        .mergedCriteriaExpression
                                                "
                                            />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    :value="
                                                        props.item
                                                            .criterionLibrary
                                                            .mergedCriteriaExpression
                                                    "
                                                    full-width
                                                    no-resize
                                                    outline
                                                    readonly
                                                    rows="5"
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                </v-layout>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <v-layout justify-start align-center v-show="hasSelectedLibrary || hasScenario">
            <v-btn flat right
                class="ghd-control-label ghd-blue"
                @click="onRemoveTargetConditionGoals"
            > Delete Selected 
            </v-btn>
        </v-layout>

        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
            <v-textarea
                class="ghd-control-text ghd-control-border"
                outline
                v-model="selectedTargetConditionGoalLibrary.description"
                @input='checkHasUnsavedChanges()'
            >
            </v-textarea>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout justify-center row>
                <v-btn flat
                    @click="onShowConfirmDeleteAlert"
                    class="ghd-white-bg ghd-blue"
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary"
                >
                    Delete Library
                </v-btn>
                <v-btn :disabled='!hasUnsavedChanges' flat
                    @click="onDiscardChanges"
                    class="ghd-white-bg ghd-blue"
                    v-show="hasScenario"
                >
                    Cancel
                </v-btn>
                <v-btn outline
                    @click="onShowCreateTargetConditionGoalLibraryDialog(true)"
                    class="ghd-control-border ghd-blue"
                    :disabled="disableCrudButtons()"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioTargetConditionGoals"
                    class="ghd-blue-bg ghd-white"
                    v-show="hasScenario"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                >
                    Save
                </v-btn>
                <v-btn
                    @click="onUpsertTargetConditionGoalLibrary"
                    class="ghd-blue-bg ghd-white"
                    v-show="!hasScenario"
                    :disabled="disableCrudButtons() || !hasUnsavedChanges || !hasLibraryEditPermission"
                >
                    Update Library
                </v-btn>
            </v-layout>
        </v-flex>
    
        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateTargetConditionGoalLibraryDialog
            :dialogData="createTargetConditionGoalLibraryDialogData"
            @submit="onSubmitCreateTargetConditionGoalLibraryDialogResult"
        />

        <CreateTargetConditionGoalDialog
            :showDialog="showCreateTargetConditionGoalDialog"
            :currentNumberOfTargetConditionGoals="
                selectedTargetConditionGoalLibrary.targetConditionGoals.length
            "
            @submit="onAddTargetConditionGoal"
        />

        <ShareTargetConditionGoalLibraryDialog :dialogData="shareTargetConditionGoalLibraryDialogData"
            @submit="onShareTargetConditionGoalDialogSubmit" 
        />

        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onEditTargetConditionGoalCriterionLibrary"
        />
    </v-layout>
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, Mutation, State } from 'vuex-class';
import {
    emptyTargetConditionGoal,
    emptyTargetConditionGoalLibrary,
    TargetConditionGoal,
    TargetConditionGoalLibrary,
    TargetConditionGoalLibraryUser
} from '@/shared/models/iAM/target-condition-goal';
import {
  any,
    clone,
    contains,
    find,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import {
    ShareTargetConditionGoalLibraryDialogData,
    emptyShareTargetConditionGoalLibraryDialogData
} from '@/shared/models/modals/share-target-condition-goals-data';
import ShareTargetConditionGoalLibraryDialog from '@/components/target-editor/target-editor-dialogs/ShareTargetConditionGoalLibraryDialog.vue';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import CreateTargetConditionGoalDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalDialog.vue';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import {
    CreateTargetConditionGoalLibraryDialogData,
    emptyCreateTargetConditionGoalLibraryDialogData,
} from '@/shared/models/modals/create-target-condition-goal-library-dialog-data';
import CreateTargetConditionGoalLibraryDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalLibraryDialog.vue';
import { Attribute } from '@/shared/models/iAM/attribute';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import {
    CriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import {LibraryUser} from '@/shared/models/iAM/user'
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import TargetConditionGoalService from '@/services/target-condition-goal.service';
import { AxiosResponse } from 'axios';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { isNullOrUndefined } from 'util';

@Component({
    components: {
        GeneralCriterionEditorDialog,
        CreateTargetConditionGoalLibraryDialog,
        ShareTargetConditionGoalLibraryDialog,
        CreateTargetConditionGoalDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class TargetConditionGoalEditor extends Vue {
    @State(
        state => state.targetConditionGoalModule.targetConditionGoalLibraries,
    )
    stateTargetConditionGoalLibraries: TargetConditionGoalLibrary[];
    @State(
        state => state.targetConditionGoalModule.selectedTargetConditionGoalLibrary,
    )
    stateSelectedTargetConditionLibrary: TargetConditionGoalLibrary;
    @State(state => state.attributeModule.numericAttributeNames)
    stateNumericAttributes: Attribute[];
    @State(
        state => state.targetConditionGoalModule.scenarioTargetConditionGoals,
    )
    stateScenarioTargetConditionGoals: TargetConditionGoal[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.targetConditionGoalModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @State(state => state.targetConditionGoalModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedLibrary') getIsSharedLibraryAction: any;

    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getTargetConditionGoalLibraries') getTargetConditionGoalLibrariesAction: any;
    @Action('selectTargetConditionGoalLibrary') selectTargetConditionGoalLibraryAction: any;
    @Action('upsertTargetConditionGoalLibrary') upsertTargetConditionGoalLibraryAction: any;
    @Action('deleteTargetConditionGoalLibrary') deleteTargetConditionGoalLibraryAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioTargetConditionGoals') getScenarioTargetConditionGoalsAction: any;
    @Action('upsertScenarioTargetConditionGoals') upsertScenarioTargetConditionGoalsAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    @Mutation('addedOrUpdatedTargetConditionGoalLibraryMutator') addedOrUpdatedTargetConditionGoalLibraryMutator: any;
    @Mutation('selectedTargetConditionGoalLibraryMutator') selectedTargetConditionGoalLibraryMutator: any;

    @Getter('getNumericAttributes') getNumericAttributesGetter: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;

    addedRows: TargetConditionGoal[] = [];
    updatedRowsMap:Map<string, [TargetConditionGoal, TargetConditionGoal]> = new Map<string, [TargetConditionGoal, TargetConditionGoal]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: TargetConditionGoal[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    pagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: TargetConditionGoal[] = [];
    initializing: boolean = true;

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = null;

    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    shareTargetConditionGoalLibraryDialogData: ShareTargetConditionGoalLibraryDialogData = clone(emptyShareTargetConditionGoalLibraryDialogData);
    isShared: boolean = false;
    selectedTargetConditionGoalLibrary: TargetConditionGoalLibrary = clone(
        emptyTargetConditionGoalLibrary,
    );
    hasSelectedLibrary: boolean = false;
    targetConditionGoalGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Target',
            value: 'target',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Year',
            value: 'year',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
        {
            text: 'Actions',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        }
    ];
    numericAttributeNames: string[] = [];
    selectedGridRows: TargetConditionGoal[] = [];
    selectedTargetConditionGoalIds: string[] = [];
    selectedTargetConditionGoalForCriteriaEdit: TargetConditionGoal = clone(
        emptyTargetConditionGoal,
    );
    showCreateTargetConditionGoalDialog: boolean = false;
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    createTargetConditionGoalLibraryDialogData: CreateTargetConditionGoalLibraryDialogData = clone(
        emptyCreateTargetConditionGoalLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    currentUrl: string = window.location.href;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getTargetConditionGoalLibrariesAction();
            vm.numericAttributeNames = getPropertyValues('name', vm.getNumericAttributesGetter);
            vm.getHasPermittedAccessAction().then(() => {
                if (to.path.indexOf(ScenarioRoutePaths.TargetConditionGoal) !== -1) {
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
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateTargetConditionGoalLibraries')
    onStateTargetConditionGoalLibrariesChanged() {
        this.librarySelectItems = this.stateTargetConditionGoalLibraries.map(
            (library: TargetConditionGoalLibrary) => ({
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
            this.onLibrarySelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged)
            this.CheckUnsavedDialog(this.onLibrarySelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            })
        this.librarySelectItemValueAllowedChanged = true;
    }
    onLibrarySelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue
        this.selectTargetConditionGoalLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
    }

    @Watch('stateSelectedTargetConditionLibrary')
    onStateSelectedTargetConditionGoalLibraryChanged() {
        this.selectedTargetConditionGoalLibrary = clone(
            this.stateSelectedTargetConditionLibrary,
        );
    }

    @Watch('selectedTargetConditionGoalLibrary')
    onSelectedTargetConditionGoalLibraryChanged() {
        this.hasSelectedLibrary = this.selectedTargetConditionGoalLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        this.numericAttributeNames = getPropertyValues('name', this.getNumericAttributesGetter);

        this.clearChanges();
        this.initializing = false;
        if(this.hasSelectedLibrary)
            this.onPaginationChanged();

        if (!isNullOrUndefined(this.selectedTargetConditionGoalLibrary.id) ) {
            this.getIsSharedLibraryAction(this.selectedTargetConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
        }
    }

    @Watch('selectedGridRows')
    onSelectedGridRowsChanged() {
        this.selectedTargetConditionGoalIds = getPropertyValues('id', this.selectedGridRows,) as string[];
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
    }

    @Watch('stateScenarioTargetConditionGoals')
    onStateScenarioTargetConditionGoalsChanged() {
        if (this.hasScenario) {
            this.currentPage = clone(this.stateScenarioTargetConditionGoals);
        }
    }

    @Watch('currentPage')
    onCurrentPageChanged() {
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
        if (!isNullOrUndefined(this.selectedTargetConditionGoalLibrary.id) ) {
            this.getIsSharedLibraryAction(this.selectedTargetConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
        }

        const request: PagingRequest<TargetConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.librarySelectItemValue !== null ? this.librarySelectItemValue : null,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL)
            TargetConditionGoalService.getScenarioTargetConditionGoalPage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
        else if(this.hasSelectedLibrary)
             TargetConditionGoalService.getLibraryTargetConditionGoalPage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
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

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedTargetConditionGoalLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedTargetConditionGoalLibrary.owner) == getUserName();
    }

    onShowCreateTargetConditionGoalLibraryDialog(createAsNewLibrary: boolean) {
        this.createTargetConditionGoalLibraryDialogData = {
            showDialog: true,
            targetConditionGoals: createAsNewLibrary
                ? this.currentPage
                : [],
        };
    }

    onSubmitCreateTargetConditionGoalLibraryDialogResult(library: TargetConditionGoalLibrary) {
        this.createTargetConditionGoalLibraryDialogData = clone(emptyCreateTargetConditionGoalLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.targetConditionGoals.length == 0 || !this.hasSelectedLibrary ? null : this.selectedTargetConditionGoalLibrary.id,
                    rowsForDeletion: library.targetConditionGoals === [] ? [] : this.deletionIds,
                    updateRows: library.targetConditionGoals === [] ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.targetConditionGoals === [] ? [] : this.addedRows,
                 },
                 scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.hasCreatedLibrary = true;
                    this.librarySelectItemValue = library.id;
                    
                    if(library.targetConditionGoals === []){
                        this.clearChanges();
                    }

                    this.addedOrUpdatedTargetConditionGoalLibraryMutator(library);
                    this.selectedTargetConditionGoalLibraryMutator(library.id);
                    this.addSuccessNotificationAction({message:'Added target condition goal library'})
                }               
            })
        }
    }

    onAddTargetConditionGoal(newTargetConditionGoal: TargetConditionGoal) {
        this.showCreateTargetConditionGoalDialog = false;

        if (!isNil(newTargetConditionGoal)) {
            this.addedRows.push(newTargetConditionGoal);
            this.onPaginationChanged()
        }
    }

    onEditTargetConditionGoalProperty(
        targetConditionGoal: TargetConditionGoal,
        property: string,
        value: any,
    ) {
        // this.currentPage = update(
        //     findIndex(
        //         propEq('id', targetConditionGoal.id),
        //         this.currentPage,
        //     ),
        //     setItemPropertyValue(
        //         property,
        //         value,
        //         targetConditionGoal,
        //     ) as TargetConditionGoal,
        //     this.currentPage,
        // );

        this.onUpdateRow(targetConditionGoal.id, clone(targetConditionGoal))
        this.onPaginationChanged();
    }

    onShowCriterionLibraryEditorDialog(targetConditionGoal: TargetConditionGoal) {
        this.selectedTargetConditionGoalForCriteriaEdit = clone(
            targetConditionGoal,
        );

        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: targetConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onEditTargetConditionGoalCriterionLibrary(criterionExpression: string,) {
        this.criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && this.selectedTargetConditionGoalForCriteriaEdit.id !== this.uuidNIL) {
            if(this.selectedTargetConditionGoalForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedTargetConditionGoalForCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.onUpdateRow(this.selectedTargetConditionGoalForCriteriaEdit.id, 
            { ...this.selectedTargetConditionGoalForCriteriaEdit, 
            criterionLibrary: {...this.selectedTargetConditionGoalForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} })
            this.onPaginationChanged();
        }

        this.selectedTargetConditionGoalForCriteriaEdit = clone(emptyTargetConditionGoal);
    }

    onUpsertTargetConditionGoalLibrary() {
        const targetConditionGoalLibrary: TargetConditionGoalLibrary = {
            ...clone(this.selectedTargetConditionGoalLibrary),
            targetConditionGoals: clone(this.currentPage),
        };

        const upsertRequest: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal> = {
                library: this.selectedTargetConditionGoalLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: this.selectedTargetConditionGoalLibrary.id === this.uuidNIL ? null : this.selectedTargetConditionGoalLibrary.id,
                rowsForDeletion: this.deletionIds,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                addedRows: this.addedRows
                },
                scenarioId: null
        }
        TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.addedOrUpdatedTargetConditionGoalLibraryMutator(this.selectedTargetConditionGoalLibrary);
                this.selectedTargetConditionGoalLibraryMutator(this.selectedTargetConditionGoalLibrary.id);
                this.addSuccessNotificationAction({message: "Updated target condition goal library",});
            }
        });
    }

    onUpsertScenarioTargetConditionGoals() {
        TargetConditionGoalService.upsertScenarioTargetConditionGoals({
            libraryId: this.selectedTargetConditionGoalLibrary.id === this.uuidNIL ? null : this.selectedTargetConditionGoalLibrary.id,
            rowsForDeletion: this.deletionIds,
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows           
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges();
                this.librarySelectItemValue = null;
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified scenario's target condition goals"});
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

    onRemoveTargetConditionGoals() {
        this.selectedTargetConditionGoalIds.forEach(_ => {
            this.removeRowLogic(_);
        });

        this.selectedTargetConditionGoalIds = [];
        this.onPaginationChanged();
    }
    onRemoveTargetConditionGoalsIcon(targetConditionGoal: TargetConditionGoal) {
        this.removeRowLogic(targetConditionGoal.id);
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
            this.deleteTargetConditionGoalLibraryAction({
                libraryId: this.selectedTargetConditionGoalLibrary.id,
            });
        }
    }

    disableCrudButtons() {
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
            (targetGoal: TargetConditionGoal) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        targetGoal.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        targetGoal.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedTargetConditionGoalLibrary.name,
                ) === true &&
                dataIsValid
            );
        }

        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    //paging

    onUpdateRow(rowId: string, updatedRow: TargetConditionGoal){
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
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.stateSelectedTargetConditionLibrary, this.selectedTargetConditionGoalLibrary))
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
        const request: PagingRequest<TargetConditionGoal>= {
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
            TargetConditionGoalService.getScenarioTargetConditionGoalPage(this.selectedScenarioId, request).then(response => {
                this.initializing = false
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
    }

    onShowShareTargetConditionGoalLibraryDialog(targetConditionGoalLibrary: TargetConditionGoalLibrary) {
        this.shareTargetConditionGoalLibraryDialogData = {
            showDialog:true,
            targetConditionGoalLibrary: clone(targetConditionGoalLibrary)
        }
    }

    onShareTargetConditionGoalDialogSubmit(targetConditionGoalLibraryUsers: TargetConditionGoalLibraryUser[]) {
            this.shareTargetConditionGoalLibraryDialogData = clone(emptyShareTargetConditionGoalLibraryDialogData);

            if (!isNil(targetConditionGoalLibraryUsers) && this.selectedTargetConditionGoalLibrary.id !== getBlankGuid())
            {
                let libraryUserData: LibraryUser[] = [];

                //create library users
                targetConditionGoalLibraryUsers.forEach((targetConditionGoalLibraryUser, index) =>
                {   
                    //determine access level
                    let libraryUserAccessLevel: number = 0;
                    if (libraryUserAccessLevel == 0 && targetConditionGoalLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                    if (libraryUserAccessLevel == 0 && targetConditionGoalLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                    //create library user object
                    let libraryUser: LibraryUser = {
                        userId: targetConditionGoalLibraryUser.userId,
                        userName: targetConditionGoalLibraryUser.username,
                        accessLevel: libraryUserAccessLevel
                    }

                    //add library user to an array
                    libraryUserData.push(libraryUser);
                });

                if (!isNullOrUndefined(this.selectedTargetConditionGoalLibrary.id) ) {
                            this.getIsSharedLibraryAction(this.selectedTargetConditionGoalLibrary).then(this.isShared = this.isSharedLibrary);
                }
                //update budget library sharing
                TargetConditionGoalService.upsertOrDeleteTargetConditionGoalLibraryUsers(this.selectedTargetConditionGoalLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                    {
                        this.resetPage();
                    }
            });
        }
    }





}
</script>

<style>
.targets-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.targets-data-table .v-menu--inline,
.target-criteria-output {
    width: 100%;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    padding-left: 10;
    margin: 10;
}
</style>
