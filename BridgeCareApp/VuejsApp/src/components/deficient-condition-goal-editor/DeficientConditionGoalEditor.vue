<template>
    <v-row column>
        <v-col cols = "12">
        <v-row justify-space-between>
            <v-col cols = "4" class="ghd-constant-header">
                <v-row column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Deficient Condition Goal Library</v-subheader>
                    <v-select
                        id="DeficientConditionGoalEditor-librarySelect-vselect"
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        variant="outlined"
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-row>
            </v-col>
            <v-col cols = "4" class="ghd-constant-header">
                <div style="padding-top: 15px !important">
                    <v-btn v-if="hasScenario" 
                        class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                        @click="importLibrary()"
                        :disabled="importLibraryDisabled">
                        Import
                    </v-btn>
                </div>
                
                <v-row v-if='hasSelectedLibrary && !hasScenario' style="padding-top: 11px; padding-left: 10px">
                    <div class="header-text-content owner-padding" style="padding-top: 7px;">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                    </div>
                    <v-divider class="owner-shared-divider" vertical
                        v-if='hasSelectedLibrary && selectedScenarioId === uuidNIL'>
                    </v-divider>
                    <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot: badge>
                        <span>Shared</span>
                        </template>
                        </v-badge>
                        <v-btn id="DeficientConditionGoalEditor-shareLibrary-vbtn" @click='onShowShareDeficientConditionGoalLibraryDialog(selectedDeficientConditionGoalLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                    </v-btn>
                </v-row>
            </v-col>
            <v-col cols = "4" class="ghd-constant-header">
                <v-row align-end style="padding-top: 18px !important;">
                    <v-spacer></v-spacer>
                    <v-btn
                        id="DeficientConditionGoalEditor-addDeficientConditionGoal-vbtn"
                        @click="showCreateDeficientConditionGoalDialog = true"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="hasSelectedLibrary || hasScenario"
                        variant = "outlined">
                        Add Deficient Condition Goal
                    </v-btn>
                    <v-btn id="DeficientConditionGoalEditor-createNewLibrary-vbtn" @click="onShowCreateDeficientConditionGoalLibraryDialog(false)"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="!hasScenario"
                        variant = "outlined">    
                        Create New Library        
                    </v-btn>
                </v-row>
            </v-col>
                   
        </v-row>
        </v-col>
        <v-col cols = "12" v-show="hasSelectedLibrary || hasScenario">
            <div class="deficients-data-table">
                <v-data-table
                    id="DeficientConditionGoalEditor-deficientConditionGoals-vdatatable"
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
                    <template slot="items" slot-scope="props" v-slot:item="{item}">
                        <td>
                            <v-checkbox
                                id="DeficientConditionGoalEditor-selectForDelete-vcheckbox"
                                hide-details
                                primary
                                v-model="item.raw.selected"
                            ></v-checkbox>
                        </td>
                        <td v-for="header in deficientConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog v-if="header.value !== 'criterionLibrary' && header.value !== 'action'"
                                    :return-value.sync="item.value[header.value]"
                                    @save="onEditDeficientConditionGoalProperty(item.value,header.value,item.value[header.value])"
                                    size="large"
                                    lazy>
                                    <v-text-field v-if="header.value !== 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        :model-value="item.value[header.value]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                    <v-text-field v-if="header.value === 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        :model-value="item.value[header.value]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty,
                                            rules['generalRules'].valueIsWithinRange(item.value[header.value],[0, 100])]"/>

                                    <template v-slot:input>
                                        <v-text-field v-if="header.value === 'name'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalName-vtextfield"
                                            label="Edit"
                                            single-line
                                            v-model="item.value[header.value]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-select v-if="header.value === 'attribute'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalAttribute-vselect"
                                            :items="numericAttributeNames"
                                            append-icon=$vuetify.icons.ghd-down
                                            label="Select an Attribute"
                                            v-model="item.value[header.value]"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty]">
                                        </v-select>

                                        <v-text-field v-if="header.value === 'deficientLimit'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalLimit-vtextfield"
                                            label="Edit"
                                            single-line
                                            v-model="item.value[header.value]"
                                            :mask="'##########'"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-text-field v-if="header.value === 'allowedDeficientPercentage'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalPercentage-vtextfield"
                                            label="Edit"
                                            single-line
                                            v-model.number="item.value[header.value]"
                                            :mask="'###'"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty,
                                                rules['generalRules'].valueIsWithinRange(
                                                    item.value[header.value],[0, 100])]"/>
                                    </template>
                                </v-edit-dialog>
                                
                                <v-row
                                    v-if="header.value === 'criterionLibrary'"
                                    align-center
                                    style="flex-wrap:nowrap">
                                    <v-menu
                                        location="bottom"
                                        min-height="500px"
                                        min-width="500px">
                                        <template v-slot:activator>
                                            <v-text-field
                                                readonly
                                                class="sm-txt"
                                                :model-value="item.value.criterionLibrary.mergedCriteriaExpression"/>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    :model-value="item.value.criterionLibrary.mergedCriteriaExpression"
                                                    full-width
                                                    no-resize
                                                    variant="outlined"
                                                    readonly
                                                    rows="5"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                                        id="DeficientConditionGoalEditor-editDeficientConditionGoalCriteria-vbtn"
                                        @click="onShowCriterionLibraryEditorDialog(item.value)"
                                        class="ghd-blue"
                                        icon>
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-row>
                                <div v-if="header.value === 'action'">
                                    <v-btn id="DeficientConditionGoalEditor-deleteDeficientConditionGoal-vbtn" @click="onRemoveSelectedDeficientConditionGoal(item.value.id)"  class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </div>                               
                            </div>
                        </td>
                    </template>
                </v-data-table> 
                <v-btn 
                    id="DeficientConditionGoalEditor-deleteSelected-vbtn"
                    :disabled="selectedDeficientConditionGoalIds.length === 0"
                    @click="onRemoveSelectedDeficientConditionGoals"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    variant = "flat">
                    Delete Selected
            </v-btn>              
            </div>
           
        </v-col>
        
        <v-col v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-row justify-center>
                <v-col>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea
                        class="ghd-text-field-border"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedDeficientConditionGoalLibrary.description"
                        @update:model-value="checkHasUnsavedChanges()"
                    >
                    </v-textarea>
                </v-col>
            </v-row>
        </v-col>
        <v-col v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-row justify-center>
                <v-btn
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    :disabled="!hasUnsavedChanges"
                    variant = "flat">
                    Cancel
                </v-btn>
                <v-btn
                    id="DeficientConditionGoalEditor-deleteLibrary-vbtn"
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                    variant = "outlined">
                    Delete Library
                </v-btn>    
                <v-btn
                    id="DeficientConditionGoalEditor-createAsNewLibrary-vbtn"
                    @click="onShowCreateDeficientConditionGoalLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    :disabled="disableCrudButtons()"
                    variant = "outlined">
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioDeficientConditionGoals"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges">
                    Save
                </v-btn>
                <v-btn
                    id="DeficientConditionGoalEditor-updateLibrary-vbtn"
                    @click="onUpsertDeficientConditionGoalLibrary"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges">
                    Update Library
                </v-btn>               
                       
            </v-row>
        </v-col>

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
    </v-row>
</template>

<script setup lang="ts">
import Vue, { onBeforeUnmount, ref, Ref, ShallowRef, shallowRef, watch } from 'vue';
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
    find,
    isNil,
    propEq,
} from 'ramda';
import CreateDeficientConditionGoalDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalDialog.vue';
import {
    CreateDeficientConditionGoalLibraryDialogData,
    emptyCreateDeficientConditionGoalLibraryDialogData,
} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
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
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
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
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    const $router = useRouter();
    let stateDeficientConditionGoalLibraries = shallowRef<DeficientConditionGoalLibrary[]>(store.state.deficientConditionGoalModule.deficientConditionGoalLibraries);
    let stateSelectedDeficientConditionGoalLibrary = shallowRef<DeficientConditionGoalLibrary>(store.state.deficientConditionGoalModule.selectedDeficientConditionGoalLibrary);
    let stateNumericAttributes = shallowRef<Attribute[]>(store.state.attributeModule.numericAttributeNames);
    let stateScenarioDeficientConditionGoals = shallowRef<DeficientConditionGoal[]>(store.state.deficientConditionGoalModule.scenarioDeficientConditionGoals);
    let hasUnsavedChanges = store.state.unsavedChangesFlagModule.hasUnsavedChanges as boolean;
    let hasAdminAccess = (store.state.authenticationModule.hasAdminAccess) as boolean;
    let hasPermittedAccess = (store.state.deficientConditionGoalModule.hasPermittedAccess) as boolean;
    let isSharedLibrary = ref<boolean>(store.state.deficientConditionGoalModule.isSharedLibrary);

    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks');}
    async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedDeficientConditionGoalLibrary');}
    async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess');}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
    async function getDeficientConditionGoalLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getDeficientConditionGoalLibraries');}
    async function selectDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectDeficientConditionGoalLibrary');}
    async function upsertDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertDeficientConditionGoalLibrary');}
    async function deleteDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteDeficientConditionGoalLibrary');}
    async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
    async function getScenarioDeficientConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('getScenarioDeficientConditionGoals');}
    async function upsertScenarioDeficientConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioDeficientConditionGoals');}
    async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification');}
    async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario');}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario');}
    
    function addedOrUpdatedDeficientConditionGoalLibraryMutator(payload: any){store.commit('addedOrUpdatedDeficientConditionGoalLibraryMutator');}
    function selectedDeficientConditionGoalLibraryMutator(payload: any){store.commit('selectedDeficientConditionGoalLibraryMutator');}

    let getNumericAttributesGetter: any = store.getters.getNumericAttributes;
    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    let selectedScenarioId: string = getBlankGuid();
    let librarySelectItems: SelectItem[] = [];
    let selectedDeficientConditionGoalLibrary: ShallowRef<DeficientConditionGoalLibrary> = shallowRef(clone(emptyDeficientConditionGoalLibrary));
    let hasSelectedLibrary: boolean = false;
    let dateModified: string;
    let deficientConditionGoalGridHeaders: DataTableHeader[] = [
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
    let numericAttributeNames: string[] = [];
    let selectedGridRows: ShallowRef<DeficientConditionGoal[]> = ref([]);
    let selectedDeficientConditionGoalIds: string[] = [];
    let selectedDeficientConditionGoalForCriteriaEdit: DeficientConditionGoal = clone(
        emptyDeficientConditionGoal,
    );
    let showCreateDeficientConditionGoalDialog: boolean = false;
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let createDeficientConditionGoalLibraryDialogData: CreateDeficientConditionGoalLibraryDialogData = clone(
        emptyCreateDeficientConditionGoalLibraryDialogData,
    );
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario: boolean = false;
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let importLibraryDisabled: boolean = true;
    let scenarioHasCreatedNew: boolean = false;

    let addedRows: DeficientConditionGoal[] = [];
    let updatedRowsMap:Map<string, [DeficientConditionGoal, DeficientConditionGoal]> = new Map<string, [DeficientConditionGoal, DeficientConditionGoal]>();//0: original value | 1: updated value
    let deletionIds: string[] = [];
    let rowCache: DeficientConditionGoal[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let pagination: ShallowRef<Pagination> = shallowRef(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: ShallowRef<DeficientConditionGoal[]> = ref([]);
    let initializing: boolean = true;
    let isShared: boolean = false;

    let shareDeficientConditionGoalLibraryDialogData: ShareDeficientConditionGoalLibraryDialogData = clone(emptyShareDeficientConditionGoalLibraryDialogData);

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: Ref<string | null> = ref(null);
    let parentLibraryId: string = "";
    let parentLibraryName: string = "None";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let libraryImported: boolean = false;

    created();
    function created() {
        librarySelectItemValue.value = null;
        getDeficientConditionGoalLibrariesAction().then(() => {
            numericAttributeNames = getPropertyValues('name', getNumericAttributesGetter);
            getHasPermittedAccessAction().then(() => {
                if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.DeficientConditionGoal) !== -1) {
                    selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

                    if (selectedScenarioId === uuidNIL) {
                        addErrorNotificationAction({
                            message: 'Found no selected scenario for edit',
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
    onBeforeUnmount(() => beforeDestroy); 
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(stateDeficientConditionGoalLibraries, () => onStateDeficientConditionGoalLibrariesChanged)
    function onStateDeficientConditionGoalLibrariesChanged() {
        librarySelectItems = stateDeficientConditionGoalLibraries.value.map(
            (library: DeficientConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue, () => onLibrarySelectItemValueChangedCheckUnsaved)
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
        librarySelectItemValueAllowedChanged = true;
        librarySelectItems.forEach(library => {
            if (library.value === librarySelectItemValue.value) {
                parentLibraryName = library.text;
            }
        });
    }

    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        if(!hasScenario || isNil(librarySelectItemValue))
            selectDeficientConditionGoalLibraryAction({
                libraryId: librarySelectItemValue,
            });
        else if(!isNil(librarySelectItemValue) && !scenarioHasCreatedNew)
        {
            importLibraryDisabled = false;
        }

        scenarioHasCreatedNew = false;
    }

    watch(stateSelectedDeficientConditionGoalLibrary, () => onStateSelectedDeficientConditionGoalLibraryChanged)
    function onStateSelectedDeficientConditionGoalLibraryChanged() {
        selectedDeficientConditionGoalLibrary.value = clone(
            stateSelectedDeficientConditionGoalLibrary.value,
        );
    }

    watch(selectedDeficientConditionGoalLibrary, () => onSelectedDeficientConditionGoalLibraryChanged)
    function onSelectedDeficientConditionGoalLibraryChanged() {
        if (!isNullOrUndefined(selectedDeficientConditionGoalLibrary)) {
            hasSelectedLibrary = selectedDeficientConditionGoalLibrary.value.id !== uuidNIL;
            
        }
        if (hasSelectedLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }

        clearChanges();
        initializing = false;
        if(hasSelectedLibrary)
            onPaginationChanged();
        
        checkHasUnsavedChanges();
    }

    watch(selectedGridRows, () => onSelectedDeficientRowsChanged)
    function onSelectedDeficientRowsChanged() {
        selectedDeficientConditionGoalIds = getPropertyValues('id', selectedGridRows.value,) as string[];
    }

    watch(stateNumericAttributes, () => onStateNumericAttributesChanged)
    function onStateNumericAttributesChanged() {
        numericAttributeNames = getPropertyValues('name', stateNumericAttributes.value);
    }

    watch(stateScenarioDeficientConditionGoals, () => onStateScenarioDeficientConditionGoalsChanged)
    function onStateScenarioDeficientConditionGoalsChanged() {
        if (hasScenario) {
            currentPage.value = clone(stateScenarioDeficientConditionGoals.value);
        }
    }

    watch(currentPage, () => onCurrentPageChanged)
    function onCurrentPageChanged() {
        librarySelectItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }

    watch(isSharedLibrary, () => onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }

    watch(pagination, () => onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = pagination.value;
        const request: PagingRequest<DeficientConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null && importLibraryDisabled ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds,
                addedRows: addedRows,
                isModified: scenarioLibraryIsModified
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            await DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                }
            });
        else if(hasSelectedLibrary)
            await DeficientConditionGoalService.getDeficientLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified = data.slice(0, 10);
                   }
             }),    
             await DeficientConditionGoalService.getLibraryDeficientConditionGoalPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    if (!isNullOrUndefined(selectedDeficientConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedDeficientConditionGoalLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }      

                }
            });     
    }

    function importLibrary() {
        setParentLibraryName(librarySelectItemValue.value ? librarySelectItemValue.value : "");
        selectDeficientConditionGoalLibraryAction({
            libraryId: librarySelectItemValue,
        });
        importLibraryDisabled = true;
        scenarioLibraryIsModified = false;
        libraryImported = true;

    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedDeficientConditionGoalLibrary.value.owner);
        }
        
        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess || (hasPermittedAccess && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedDeficientConditionGoalLibrary.value.owner) == getUserName();
    }

    function onShowCreateDeficientConditionGoalLibraryDialog(createExistingLibraryAsNew: boolean) {
        createDeficientConditionGoalLibraryDialogData = {
            showDialog: true,
            deficientConditionGoals: createExistingLibraryAsNew
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateDeficientConditionGoalLibraryDialogResult(library: DeficientConditionGoalLibrary,) {
        createDeficientConditionGoalLibraryDialogData = clone(emptyCreateDeficientConditionGoalLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.deficientConditionGoals.length == 0 || !hasSelectedLibrary? null : selectedDeficientConditionGoalLibrary.value.id,
                    rowsForDeletion: library.deficientConditionGoals.length == 0 ? [] : deletionIds,
                    updateRows: library.deficientConditionGoals.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.deficientConditionGoals.length == 0 ? [] : addedRows,
                    isModified: false,
                 },
                 scenarioId: hasScenario ? selectedScenarioId : null
            }
            DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = library.id;
                    
                    if(library.deficientConditionGoals.length == 0){
                        clearChanges();
                    }

                    if(hasScenario){
                        scenarioHasCreatedNew = true;
                        importLibraryDisabled = true;
                    }
                    
                    addedOrUpdatedDeficientConditionGoalLibraryMutator(library);
                    selectedDeficientConditionGoalLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added deficient condition goal library'})
                }               
            })
        }
    }

    function onAddDeficientConditionGoal(newDeficientConditionGoal: DeficientConditionGoal) {
        showCreateDeficientConditionGoalDialog = false;

        if (!isNil(newDeficientConditionGoal)) {
            addedRows.push(newDeficientConditionGoal);
            onPaginationChanged()
        }
    }

    function onEditDeficientConditionGoalProperty(deficientConditionGoal: DeficientConditionGoal, property: string, value: any) {
        onUpdateRow(deficientConditionGoal.id, clone(deficientConditionGoal))
        onPaginationChanged();
    }

    function onShowCriterionLibraryEditorDialog(deficientConditionGoal: DeficientConditionGoal,) {
        selectedDeficientConditionGoalForCriteriaEdit = clone(
            deficientConditionGoal,
        );

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: deficientConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onEditDeficientConditionGoalCriterionLibrary(criterionExpression: string,) {
        criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedDeficientConditionGoalForCriteriaEdit.id !== uuidNIL) {
            if(selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedDeficientConditionGoalForCriteriaEdit.id,
             { ...selectedDeficientConditionGoalForCriteriaEdit, 
                criterionLibrary: {... selectedDeficientConditionGoalForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression}})                
            onPaginationChanged();
        }

        selectedDeficientConditionGoalForCriteriaEdit = clone(
            emptyDeficientConditionGoal,
        );
    }

    function onUpsertDeficientConditionGoalLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal> = {
                library: selectedDeficientConditionGoalLibrary.value,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedDeficientConditionGoalLibrary.value.id === uuidNIL ? null : selectedDeficientConditionGoalLibrary.value.id,
                rowsForDeletion: deletionIds,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                addedRows: addedRows,
                isModified: false
                },
                scenarioId: null
        }
        DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()
                addedOrUpdatedDeficientConditionGoalLibraryMutator(selectedDeficientConditionGoalLibrary.value);
                selectedDeficientConditionGoalLibraryMutator(selectedDeficientConditionGoalLibrary.value.id);
                addSuccessNotificationAction({message: "Updated deficient condition goal library",});
            }
        });
    }

    function onUpsertScenarioDeficientConditionGoals() {
        if (selectedDeficientConditionGoalLibrary.value.id === uuidNIL || hasUnsavedChanges && libraryImported === false) { scenarioLibraryIsModified = true; }
        else { scenarioLibraryIsModified = false; }

        DeficientConditionGoalService.upsertScenarioDeficientConditionGoals({
            libraryId: selectedDeficientConditionGoalLibrary.value.id === uuidNIL ? null : selectedDeficientConditionGoalLibrary.value.id,
            rowsForDeletion: deletionIds,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows,
            isModified: scenarioLibraryIsModified
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges();
                librarySelectItemValue.value = null;
                resetPage();
                addSuccessNotificationAction({message: "Modified scenario's deficient condition goals"});
                importLibraryDisabled = true;
                libraryImported = false;
            }           
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario) {
                clearChanges();
                resetPage();
                importLibraryDisabled = true;
            }
        });
        parentLibraryName = loadedParentName;
        parentLibraryId = loadedParentId;
    }

    function onRemoveSelectedDeficientConditionGoals() {
        selectedDeficientConditionGoalIds.forEach(_ => {
            removeRowLogic(_);
        });

        selectedDeficientConditionGoalIds = [];
        onPaginationChanged();
    }

    function onRemoveSelectedDeficientConditionGoal(id: string){
        removeRowLogic(id);
        onPaginationChanged();
    }

    function removeRowLogic(id: string){
        if(isNil(find(propEq('id', id), addedRows))){
            deletionIds.push(id);
            if(!isNil(updatedRowsMap.get(id)))
                updatedRowsMap.delete(id)
        }           
        else{          
            addedRows = addedRows.filter((row) => row.id !== id)
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
            librarySelectItemValue.value = null;
            deleteDeficientConditionGoalLibraryAction({
                libraryId: selectedDeficientConditionGoalLibrary.value.id,
            });
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
            (deficientGoal: DeficientConditionGoal) => {
                return (
                    rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.name,
                    ) === true &&
                    rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.attribute,
                    ) === true
                );
            },
        );

        if (hasSelectedLibrary) {
            return !(
                rules['generalRules'].valueIsNotEmpty(
                    selectedDeficientConditionGoalLibrary.value.name,
                ) === true &&
                dataIsValid
            );
        }
        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    //paging

    function onUpdateRow(rowId: string, updatedRow: DeficientConditionGoal){
        if(any(propEq('id', rowId), addedRows)){
            const index = addedRows.findIndex(item => item.id == updatedRow.id)
            addedRows[index] = updatedRow;
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
        addedRows = [];
        deletionIds = [];
    }

    function resetPage(){
        pagination.value.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            deletionIds.length > 0 || 
            addedRows.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', stateSelectedDeficientConditionGoalLibrary, selectedDeficientConditionGoalLibrary))
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

    function onShowShareDeficientConditionGoalLibraryDialog(deficientConditionGoalLibrary: DeficientConditionGoalLibrary) {
        shareDeficientConditionGoalLibraryDialogData = {
            showDialog:true,
            deficientConditionGoalLibrary: clone(deficientConditionGoalLibrary)
        }
    }

    function onShareDeficientConditionGoalDialogSubmit(deficientConditionGoalLibraryUsers: DeficientConditionGoalLibraryUser[]) {
        shareDeficientConditionGoalLibraryDialogData = clone(emptyShareDeficientConditionGoalLibraryDialogData);

                if (!isNil(deficientConditionGoalLibraryUsers) && selectedDeficientConditionGoalLibrary.value.id !== getBlankGuid())
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
                    if (!isNullOrUndefined(selectedDeficientConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedDeficientConditionGoalLibrary).then(() => isShared = isSharedLibrary.value);
                    }
                    //update budget library sharing
                    DeficientConditionGoalService.upsertOrDeleteDeficientConditionGoalLibraryUsers(selectedDeficientConditionGoalLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
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
        let foundLibrary: DeficientConditionGoalLibrary = emptyDeficientConditionGoalLibrary;
        stateDeficientConditionGoalLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }
    function initializePages(){
        const request: PagingRequest<DeficientConditionGoal>= {
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
            DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName; //store original
                    scenarioLibraryIsModified = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
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
