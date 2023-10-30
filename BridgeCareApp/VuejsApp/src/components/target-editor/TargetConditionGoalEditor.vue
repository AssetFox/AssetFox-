<template>
    <v-row column>
        <v-col cols = "12">
           <v-row justify-space-between>
                <v-col cols = "6" class="ghd-constant-header">
                        <v-subheader class="ghd-control-label ghd-md-gray">Target Condition Goal Library</v-subheader>
                        <v-select
                            id="TargetConditionGoalEditor-SelectLibrary-select"
                            class="ghd-select ghd-text-field ghd-text-field-border"
                            :items="librarySelectItems"
                            item-title="text"
                            item-value="value"
                            v-model="librarySelectItemValue"
                            variant="outlined"
                            density="compact"
                        >
                        </v-select>
                </v-col>
                <v-col align-self="center">
                <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-col>
                <v-col cols = "12" class="ghd-constant-header">
                    <v-row v-if="hasSelectedLibrary && ! hasScenario" style="padding-top: 10px; padding-left: 10px">
                        <div v-if="hasSelectedLibrary && !hasScenario" class="header-text-content owner-padding" style="padding-top: 7px;">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
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
                        <v-btn id="TargetConditionGoalEditor-ShareLibrary-btn" @click='onShowShareTargetConditionGoalLibraryDialog(selectedTargetConditionGoalLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "12" class="ghd-constant-header">
                    <v-row justify-end align-end style="padding: 10px !important;">
                        <v-spacer></v-spacer>
                        <v-btn variant = "outlined"
                            id="TargetConditionGoalEditor-addTargetConditionGoal-btn"
                            @click="showCreateTargetConditionGoalDialog = true"
                            class="ghd-control-border ghd-blue"
                            v-show="hasSelectedLibrary || hasScenario" 
                        >Add Target Condition Goal</v-btn>
                        <v-btn 
                            id="TargetConditionGoalEditor=CreateLibrary-btn"
                            @click="onShowCreateTargetConditionGoalLibraryDialog(false)"
                            class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            v-show="!hasScenario"
                            variant = "outlined"
                        >
                        Create New Library
                        </v-btn>
                    </v-row>
                </v-col>
           </v-row>
        </v-col>
        <div class="targets-data-table" style="height: auto;">
                <v-data-table-server
                    id="TargetConditionGoalEditor-targetConditionGoals-vdatatable"
                    :headers="targetConditionGoalGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    :items-length="totalItems"
                    :rows-per-page-items=[5,10,25]
                    :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                    :v-model:sort-by="pagination.sort"
                    :v-model:page="pagination.page"
                    :v-model:items="pagination.rowsPerPage"
                    @update:options="onPaginationChanged"
                    class="elevation-1 fixed-header v-table__overflow"
                    item-key="id"
                    show-select
                    v-model="selectedGridRows"
                >
                    <template slot="items" slot-scope="props" v-slot:item="{item}">
                        <td>
                            <v-checkbox
                                id="TargetConditionGoalEditor-selectForDelete-vcheckbox"
                                hide-details
                                primary
                                v-model="item.raw.selected"
                            ></v-checkbox>
                        </td>
                        <td v-for="header in targetConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog
                                    v-if="header.value !== 'criterionLibrary' && header.value !== 'actions'"
                                    :return-value.sync="item.value[header.value]"
                                    @save="
                                        onEditTargetConditionGoalProperty(
                                            item.value,
                                            header.value,
                                            item.value[header.value])"
                                    size="large"
                                    lazy>
                                    <v-text-field
                                        v-if="header.value === 'year'"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :model-value="item.value[header.value]"/>
                                    <v-text-field
                                        v-else
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :model-value="item.value[header.value]"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty]"/>

                                    <template v-slot:input>
                                        <v-select
                                            id="TargetConditionGoalEditor-editTargetConditionGoalAttribute-vselect"
                                            v-if="header.value === 'attribute'"
                                            :items="numericAttributeNames"
                                            label="Select an Attribute"
                                            v-model="item.value.attribute"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty]"/>
                                        <v-text-field
                                            id="TargetConditionGoalEditor-editTargetConditionGoalYear-vtextfield"
                                            v-if="header.value === 'year'"
                                            label="Edit"
                                            single-line
                                            :mask="'####'"
                                            v-model.number="item.value[header.value]"/>
                                        <v-text-field
                                            id="TargetConditionGoalEditor-editTargetConditionGoalTarget-vtextfield"
                                            v-if="header.value === 'target'"
                                            label="Edit"
                                            single-line
                                            :mask="'##########'"
                                            v-model.number="item.value[header.value]"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty]"/>
                                        <v-text-field
                                            id="TargetConditionGoalEditor-editTargetConditionGoalName-vtextfield"
                                            v-if="header.value === 'name'"
                                            label="Edit"
                                            single-line
                                            v-model="item.value[header.value]"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty]"/>
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
                                                    outline
                                                    readonly
                                                    rows="5"/>
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                                        id="TargetConditionGoalEditor-editTargetConditionGoalCriteria-vbtn"
                                        @click="onShowCriterionLibraryEditorDialog(item.value)"
                                        class="ghd-blue"
                                        icon>
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-row>
                                <div v-if="header.value === 'actions'">
                                    <v-btn id="TargetConditionGoalEditor-deleteTargetConditionGoal-vbtn" @click="onRemoveTargetConditionGoalsIcon(item.value)"  class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </div> 
                            </div>
                        </td>
                    </template>
                </v-data-table-server>
            </div>
            <v-row v-show="hasSelectedLibrary || hasScenario">
            <v-col>
            <v-btn flat
                id="TargetConditionGoalEditor-deleteSelected-vbtn"
                class="ghd-control-label ghd-blue"
                @click="onRemoveTargetConditionGoals"> 
                Delete Selected 
            </v-btn>
        </v-col>
        </v-row>

            <v-divider
                :thickness="4"
                class="border-opacity-100"
            ></v-divider>

        <v-row>
            <v-col v-show="hasSelectedLibrary && !hasScenario">
                <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                <v-textarea
                    class="ghd-control-text ghd-control-border"
                    variant="outlined"
                    v-model="selectedTargetConditionGoalLibrary.description"
                    @update:model-value="checkHasUnsavedChanges()">
                </v-textarea>
            </v-col>
            <v-row style="margin: 20px;" justify="end">
            <v-col cols="6" v-show="hasSelectedLibrary || hasScenario" style="padding: 20px;">
                <v-row justify-center row>
                    <v-btn variant = "outlined"
                        id="TargetConditionGoalEditor-deleteLibrary-btn"
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
                    <v-btn flat
                        id="TargetConditionGoalEditor-CreateAsNewLibrary-btn"
                        @click="onShowCreateTargetConditionGoalLibraryDialog(true)"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
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
                        id="TargetConditionGoalEditor-UpdateLibrary-btn"
                        @click="onUpsertTargetConditionGoalLibrary"
                        class="ghd-blue-bg ghd-white"
                        v-show="!hasScenario"
                        :disabled="disableCrudButtons() || !hasUnsavedChanges || !hasLibraryEditPermission"
                    >
                        Update Library
                    </v-btn>
                </v-row>
            </v-col>
        </v-row>
    </v-row>
    
        <ConfirmDeleteAlert :is="Alert"
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
        <ConfirmDialog></ConfirmDialog>
    </v-row>
</template>

<script setup lang="ts">
import { onBeforeUnmount, ref, computed, onMounted, watch } from 'vue'; 
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
    find,
    isNil,
    propEq,
    isEmpty,
} from 'ramda';
import {
    ShareTargetConditionGoalLibraryDialogData,
    emptyShareTargetConditionGoalLibraryDialogData
} from '@/shared/models/modals/share-target-condition-goals-data';
import ShareTargetConditionGoalLibraryDialog from '@/components/target-editor/target-editor-dialogs/ShareTargetConditionGoalLibraryDialog.vue';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CreateTargetConditionGoalDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalDialog.vue';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
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
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
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
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';

    let store = useStore();
    const $router = useRouter(); 
    const confirm = useConfirm();

    const stateTargetConditionGoalLibraries = computed<TargetConditionGoalLibrary[]>(() =>store.state.targetConditionGoalModule.targetConditionGoalLibraries);
    const stateSelectedTargetConditionLibrary = computed<TargetConditionGoalLibrary>(() =>store.state.targetConditionGoalModule.selectedTargetConditionGoalLibrary) 
    const stateNumericAttributes = computed<Attribute[]>(() =>store.state.attributeModule.numericAttributeNames);
    const stateScenarioTargetConditionGoals = computed<TargetConditionGoal[]>(() =>store.state.targetConditionGoalModule.scenarioTargetConditionGoals);
    const hasUnsavedChanges = computed<boolean>(() =>store.state.unsavedChangesFlagModule.hasUnsavedChanges); 
    const hasAdminAccess = computed<boolean>(() =>store.state.authenticationModule.hasAdminAccess);
    const hasPermittedAccess = computed<boolean>(() =>store.state.targetConditionGoalModule.hasPermittedAccess);
    const isSharedLibrary = computed<boolean>(() =>store.state.targetConditionGoalModule.isSharedLibrary);

    async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedTargetConditionGoalLibrary',payload);} 
    async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess',payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification',payload);}
    async function getTargetConditionGoalLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getTargetConditionGoalLibraries',payload);} 
    async function selectTargetConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectTargetConditionGoalLibrary',payload);}
    async function upsertTargetConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertTargetConditionGoalLibrary',payload);}
    async function deleteTargetConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteTargetConditionGoalLibrary',payload);}
    async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes',payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges',payload);}
    async function getScenarioTargetConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('getScenarioTargetConditionGoals',payload);}
    async function upsertScenarioTargetConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioTargetConditionGoals',payload);}
    async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification',payload);}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario',payload);}
    async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario',payload);}

    function addedOrUpdatedTargetConditionGoalLibraryMutator(payload:any){store.commit('addedOrUpdatedTargetConditionGoalLibraryMutator',payload);} 
    function selectedTargetConditionGoalLibraryMutator(payload:any){store.commit('selectedTargetConditionGoalLibraryMutator',payload);} 
   
    let getNumericAttributesGetter = store.getters.getNumericAttributes;
    let getUserNameByIdGetter = store.getters.getUserNameById;

    const addedRows = ref<TargetConditionGoal[]>([]);
    let updatedRowsMap:Map<string, [TargetConditionGoal, TargetConditionGoal]> = new Map<string, [TargetConditionGoal, TargetConditionGoal]>();//0: original value | 1: updated value
    const deletionIds = ref<string[]>([]);
    const rowCache = ref<TargetConditionGoal[]>([]);

    let gridSearchTerm = '';
    let currentSearch = ref<string>('');
    const pagination = ref<Pagination>(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = ref(0);
    const currentPage = ref<TargetConditionGoal[]>([]);
    let initializing: boolean = true;
    let dateModified: string;

    let unsavedDialogAllowed: boolean = true;
    const trueLibrarySelectItemValue = ref<string|null>(''); 
    let librarySelectItemValueAllowedChanged: boolean = true;
    const librarySelectItemValue = ref<string|null>(null); 

    let selectedScenarioId: string = getBlankGuid();
    const librarySelectItems = ref<SelectItem[]>([]);
    let shareTargetConditionGoalLibraryDialogData: ShareTargetConditionGoalLibraryDialogData = clone(emptyShareTargetConditionGoalLibraryDialogData);
    let isShared: boolean = false;   
    let selectedTargetConditionGoalLibrary = ref<TargetConditionGoalLibrary>(clone(emptyTargetConditionGoalLibrary,));

    const hasSelectedLibrary = ref<boolean>(false);
    let targetConditionGoalGridHeaders: any[] = [
        {
            title: 'Name',
            key: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Attribute',
            key: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Target',
            key: 'target',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Year',
            key: 'year',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            title: 'Criteria',
            key: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
        {
            title: 'Actions',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        }
    ];
    let numericAttributeNames: string[] = [];
    const selectedGridRows= ref<TargetConditionGoal[]>([]);
    let selectedTargetConditionGoalIds: string[] = [];
    let selectedTargetConditionGoalForCriteriaEdit: TargetConditionGoal = clone(
        emptyTargetConditionGoal,
    );
    let showCreateTargetConditionGoalDialog: boolean = false;
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let createTargetConditionGoalLibraryDialogData: CreateTargetConditionGoalLibraryDialogData = clone(
        emptyCreateTargetConditionGoalLibraryDialogData,
    );
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let rules: InputValidationRules = validationRules; 
    let uuidNIL: string = getBlankGuid();
    const hasScenario = ref<boolean>(false);
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let parentLibraryId: string = "";
    const parentLibraryName = ref<string>("None");
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;

   //beforeRouteEnter();  
    created();
    function created(){
            // librarySelectItemValue.value = null;
            // getTargetConditionGoalLibrariesAction();
            // numericAttributeNames = getPropertyValues('name', getNumericAttributesGetter);
            // getHasPermittedAccessAction().then(() => {
            //     if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.TargetConditionGoal) !== -1) { 
            //         //selectedScenarioId = to.query.scenarioId;
            //         selectedScenarioId = $router.currentRoute.value.query.scenarioId as string; 

            //         if (selectedScenarioId === uuidNIL) {
            //             addErrorNotificationAction({
            //                 message: 'Found no selected scenario for edit',
            //             });
            //             $router.push('/Scenarios/');
            //         }

            //         hasScenario = true;
            //         getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
            //             selectScenarioAction({ scenarioId: selectedScenarioId });        
            //             initializePages();
            //         });                                        
            //     }
            // });            
    }

    onMounted(() => {
        librarySelectItemValue.value = null;
        getTargetConditionGoalLibrariesAction();
        numericAttributeNames = getPropertyValues('name', getNumericAttributesGetter);
        getHasPermittedAccessAction();
        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.TargetConditionGoal) !== -1) { 
            //selectedScenarioId = to.query.scenarioId;
            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string; 
            
            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                $router.push('/Scenarios/');
            }

            hasScenario.value = true;
            getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                selectScenarioAction({ scenarioId: selectedScenarioId });        
                initializePages();
            });                                        
        }
    });            
    onBeforeUnmount(()=> beforeDestroy());
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(hasPermittedAccess, () => {
    });
    watch(stateTargetConditionGoalLibraries,()=> {
        librarySelectItems.value = stateTargetConditionGoalLibraries.value.map(
            (library: TargetConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    });

    //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue,()=> onLibrarySelectItemValueChangedCheckUnsaved)
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario.value){
            onLibrarySelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged) {
            CheckUnsavedDialog(onLibrarySelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = trueLibrarySelectItemValue.value;               
            });
        }
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        librarySelectItemValueAllowedChanged = true;
    }

    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue.value = librarySelectItemValue.value
        selectTargetConditionGoalLibraryAction({
            libraryId: librarySelectItemValue,
        });
    }

    watch(stateSelectedTargetConditionLibrary,()=> onStateSelectedTargetConditionGoalLibraryChanged)
    function onStateSelectedTargetConditionGoalLibraryChanged() {
        selectedTargetConditionGoalLibrary = clone(stateSelectedTargetConditionLibrary,);
    }

    watch(selectedTargetConditionGoalLibrary,()=> onSelectedTargetConditionGoalLibraryChanged)
    function onSelectedTargetConditionGoalLibraryChanged() {
        hasSelectedLibrary.value = selectedTargetConditionGoalLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }

        numericAttributeNames = getPropertyValues('name', getNumericAttributesGetter);

        clearChanges();
        initializing = false;
        if(hasSelectedLibrary.value)
            onPaginationChanged();
    }

    watch(selectedGridRows,()=> onSelectedGridRowsChanged)
    function onSelectedGridRowsChanged() {
        selectedTargetConditionGoalIds = getPropertyValues('id', selectedGridRows.value,) as string[];
    }

    watch(stateNumericAttributes,()=> onStateNumericAttributesChanged)
    function onStateNumericAttributesChanged() {
        numericAttributeNames = getPropertyValues('name', stateNumericAttributes.value);
    }

    watch(stateScenarioTargetConditionGoals,()=> onStateScenarioTargetConditionGoalsChanged)
    function onStateScenarioTargetConditionGoalsChanged() {
        if (hasScenario.value) {
            currentPage.value = clone(stateScenarioTargetConditionGoals.value);
        }
    }

    watch(currentPage,()=> {

        // Get parent name from library id
        librarySelectItems.value.forEach(library => {pagination
            if (library.value === parentLibraryId) {
                parentLibraryName.value = library.text;
            }
        });
    });
    
    watch(isSharedLibrary,()=> onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }
    
    watch(pagination,()=> onPaginationChanged)
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = pagination.value;
        const request: PagingRequest<TargetConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified
            },   
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch.value        
            // sortColumn: sortBy,
            // isDescending: descending != null ? descending : false,
            // search: currentSearch
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL)
            await TargetConditionGoalService.getScenarioTargetConditionGoalPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    currentPage.value = data.items;
                    rowCache.value = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                }
            });
        else if(hasSelectedLibrary.value)
            await TargetConditionGoalService.getTargetLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified = data.slice(0, 10);
                   }
             }),
             await TargetConditionGoalService.getLibraryTargetConditionGoalPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    currentPage.value = data.items;
                    rowCache.value = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    if (!isNil(selectedTargetConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedTargetConditionGoalLibrary).then(() => isShared = isSharedLibrary.value);
                        
                    }
                }
            });     
    }

    watch(deletionIds,()=> onDeletionIdsChanged)
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows,()=> onAddedRowsChanged)
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedTargetConditionGoalLibrary.value.owner);
        }
        
        return getUserName();
    }

    function checkLibraryEditPermission() {
        setHasUnsavedChangesAction(hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner()))
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedTargetConditionGoalLibrary.value.owner) == getUserName();
    }

    function onShowCreateTargetConditionGoalLibraryDialog(createAsNewLibrary: boolean) {
        createTargetConditionGoalLibraryDialogData = {
            showDialog: true,
            targetConditionGoals: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateTargetConditionGoalLibraryDialogResult(library: TargetConditionGoalLibrary) {
        createTargetConditionGoalLibraryDialogData = clone(emptyCreateTargetConditionGoalLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.targetConditionGoals.length == 0 || !hasSelectedLibrary.value ? null : selectedTargetConditionGoalLibrary.value.id,
                    rowsForDeletion: library.targetConditionGoals.length == 0 ? [] : deletionIds.value,
                    updateRows: library.targetConditionGoals.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.targetConditionGoals.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = library.id;
                    
                    if(library.targetConditionGoals.length == 0){
                        clearChanges();
                    }

                    addedOrUpdatedTargetConditionGoalLibraryMutator(library);
                    selectedTargetConditionGoalLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added target condition goal library'})
                }               
            })
        }
    }

    function onAddTargetConditionGoal(newTargetConditionGoal: TargetConditionGoal) {
        showCreateTargetConditionGoalDialog = false;
        newTargetConditionGoal.libraryId = selectedTargetConditionGoalLibrary.value.id;
        if (!isNil(newTargetConditionGoal)) {
            addedRows.value.push(newTargetConditionGoal);
            onPaginationChanged()
        }
    }

    function onEditTargetConditionGoalProperty(
        targetConditionGoal: TargetConditionGoal,
        property: string,
        value: any,
    ) {
        onUpdateRow(targetConditionGoal.id, clone(targetConditionGoal))
        onPaginationChanged();
    }

    function onShowCriterionLibraryEditorDialog(targetConditionGoal: TargetConditionGoal) {
        selectedTargetConditionGoalForCriteriaEdit = clone(
            targetConditionGoal,
        );

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: targetConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onEditTargetConditionGoalCriterionLibrary(criterionExpression: string,) {
        criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedTargetConditionGoalForCriteriaEdit.id !== uuidNIL) {
            if(selectedTargetConditionGoalForCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedTargetConditionGoalForCriteriaEdit.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedTargetConditionGoalForCriteriaEdit.id, 
            { ...selectedTargetConditionGoalForCriteriaEdit, 
            criterionLibrary: {...selectedTargetConditionGoalForCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} })
            onPaginationChanged();
        }

        selectedTargetConditionGoalForCriteriaEdit = clone(emptyTargetConditionGoal);
    }

    function onUpsertTargetConditionGoalLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal> = {
                library: selectedTargetConditionGoalLibrary.value,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedTargetConditionGoalLibrary.value.id === uuidNIL ? null : selectedTargetConditionGoalLibrary.value.id,
                rowsForDeletion: deletionIds.value,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                addedRows: addedRows.value,
                isModified: false
                },
                scenarioId: null
        }
        TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();
                addedOrUpdatedTargetConditionGoalLibraryMutator(selectedTargetConditionGoalLibrary.value.id);
                selectedTargetConditionGoalLibraryMutator(selectedTargetConditionGoalLibrary.value.id);
                addSuccessNotificationAction({message: "Updated target condition goal library",});
            }
        });
    }

    function onUpsertScenarioTargetConditionGoals() {
        if (selectedTargetConditionGoalLibrary.value.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        TargetConditionGoalService.upsertScenarioTargetConditionGoals({
            libraryId: selectedTargetConditionGoalLibrary.value.id === uuidNIL ? null : selectedTargetConditionGoalLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified     
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges();
                librarySelectItemValue.value = null;
                resetPage();
                addSuccessNotificationAction({message: "Modified scenario's target condition goals"});
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

    function onRemoveTargetConditionGoals() {
        selectedTargetConditionGoalIds.forEach(_ => {
            removeRowLogic(_);
        });

        selectedTargetConditionGoalIds = [];
        onPaginationChanged();
    }
    function onRemoveTargetConditionGoalsIcon(targetConditionGoal: TargetConditionGoal) {
        removeRowLogic(targetConditionGoal.id);
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
            deleteTargetConditionGoalLibraryAction({
                libraryId: selectedTargetConditionGoalLibrary.value.id,
            });
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
            (targetGoal: TargetConditionGoal) => {
                return (
                    rules['generalRules'].valueIsNotEmpty(
                        targetGoal.name,
                    ) === true &&
                    rules['generalRules'].valueIsNotEmpty(
                        targetGoal.attribute,
                    ) === true
                );
            },
        );

        if (hasSelectedLibrary.value) {
            return !(
                rules['generalRules'].valueIsNotEmpty(
                    selectedTargetConditionGoalLibrary.value.name,
                ) === true &&
                dataIsValid
            );
        }

        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    //paging

    function onUpdateRow(rowId: string, updatedRow: TargetConditionGoal){
        if(any(propEq('id', rowId), addedRows.value)){
            const index = addedRows.value.findIndex(item => item.id == updatedRow.id)
            addedRows.value[index] = updatedRow;
            return;
        }

        let mapEntry = updatedRowsMap.get(rowId)

        if(isNil(mapEntry)){
            const row = rowCache.value.find(r => r.id === rowId);
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
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', stateSelectedTargetConditionLibrary, selectedTargetConditionGoalLibrary))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        if (hasUnsavedChanges && unsavedDialogAllowed) {
            
            confirm.require({
                message: "You have unsaved changes. Are you sure you wish to continue?",
                header: "Unsaved Changes",
                icon: 'pi pi-question-circle',
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
        let foundLibrary: TargetConditionGoalLibrary = emptyTargetConditionGoalLibrary;
        stateTargetConditionGoalLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    function initializePages(){
        const request: PagingRequest<TargetConditionGoal>= {
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
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL)
            TargetConditionGoalService.getScenarioTargetConditionGoalPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    currentPage.value = data.items;
                    rowCache.value = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName.value; //store original
                    scenarioLibraryIsModified = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
    }

    function onShowShareTargetConditionGoalLibraryDialog(targetConditionGoalLibrary: TargetConditionGoalLibrary) {
        shareTargetConditionGoalLibraryDialogData = {
            showDialog:true,
            targetConditionGoalLibrary: clone(targetConditionGoalLibrary)
        }
    }

    function onShareTargetConditionGoalDialogSubmit(targetConditionGoalLibraryUsers: TargetConditionGoalLibraryUser[]) {
            shareTargetConditionGoalLibraryDialogData = clone(emptyShareTargetConditionGoalLibraryDialogData);

            if (!isNil(targetConditionGoalLibraryUsers) && selectedTargetConditionGoalLibrary.value.id !== getBlankGuid())
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

                if (!isNil(selectedTargetConditionGoalLibrary.value.id) ) {
                            getIsSharedLibraryAction(selectedTargetConditionGoalLibrary).then(() => isShared = isSharedLibrary.value);
                }
                //update budget library sharing
                TargetConditionGoalService.upsertOrDeleteTargetConditionGoalLibraryUsers(selectedTargetConditionGoalLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                    if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                    {
                        resetPage();
                    }
            });
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
