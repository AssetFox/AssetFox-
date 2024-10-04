<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col cols="12">
            <v-row align="center" justify="space-between">
                <v-col cols="auto">
                    <div style="margin-bottom: 10px;">
                        <v-subheader class="ghd-control-label ghd-md-gray">Target Condition Goal Library</v-subheader>
                    </div>
                    <v-select
                        id="TargetConditionGoalEditor-SelectLibrary-select"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items="sortAlphabetically(librarySelectItems)"
                        menu-icon=custom:GhdDownSvg
                        item-title="text"
                        item-value="value"
                        v-model="librarySelectItemValue"
                        variant="outlined"
                        density="compact"
                    />
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-col>
                <v-col cols = "auto">
                    <v-row v-show="hasSelectedLibrary && ! hasScenario">
                        <div v-if="hasSelectedLibrary && !hasScenario" class="header-text-content owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <!-- <v-divider vertical 
                            class="owner-shared-divider"
                            v-if="hasSelectedLibrary && !hasScenario"
                        >
                        </v-divider> -->
                        <v-btn @click='onShowShareTargetConditionGoalLibraryDialog(selectedTargetConditionGoalLibrary)'
                            style="margin-left: 10px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "auto">
                    <v-btn 
                        id="TargetConditionGoalEditor=CreateLibrary-btn"
                        @click="onShowCreateTargetConditionGoalLibraryDialog(false)"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        style="margin:5px"
                        v-show="!hasScenario"
                        variant = "outlined"
                        >
                        Create New Library
                    </v-btn>
                </v-col>
                <v-col cols="auto">
                    <v-btn variant = "outlined"
                            id="TargetConditionGoalEditor-addTargetConditionGoal-btn"
                            @click="showCreateTargetConditionGoalDialog = true"
                            class="ghd-control-border ghd-blue"
                            style="margin: 5px;"
                            v-show="hasSelectedLibrary || hasScenario" 
                        >Add Target Condition Goal</v-btn>              
                </v-col>
            </v-row>
        </v-col>
        <v-col v-show='hasSelectedLibrary || hasScenario' cols="12">
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
                    v-model:sort-by="pagination.sort"
                    v-model:page="pagination.page"
                    v-model:items-per-page="pagination.rowsPerPage"
                    @update:options="onPaginationChanged"
                    class='v-table__overflow ghd-table'
                    sort-asc-icon="custom:GhdTableSortAscSvg"
                    sort-desc-icon="custom:GhdTableSortDescSvg"
                    item-key="id"
                    show-select
                    return-object
                    v-model="selectedGridRows"
                >
                    <template v-slot:item="item">
                        <tr>
                            <td>
                            <v-checkbox 
                                id="TargetConditionGoalEditor-selectForDelete-vcheckbox"
                                hide-details
                                v-model="selectedGridRows" :value="item.item"
                            ></v-checkbox>
                            </td>
                            <td v-for="header in targetConditionGoalGridHeaders">
                                <editDialog v-if="header.key !== 'criterionLibrary' && header.key !== 'actions'"
                                    v-model:return-value.sync="item.item[header.key]"
                                    @save="onEditTargetConditionGoalProperty(item.item,header.key,item.item[header.key])"
                                    size="large"
                                    lazy>
                                    <v-text-field v-if="header.key !== 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        density="compact"
                                        variant="underlined"
                                        :model-value="item.item[header.key]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                      <template v-slot:input>
                                        <v-text-field v-if="header.key === 'name'"
                                            id="TargetConditionGoalEditor-editTargetConditionGoalName-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                        <v-select v-if="header.key === 'attribute'"
                                            id="TargetConditionGoalEditor-editTargetConditionGoalAttribute-vselect"
                                            :items="numericAttributeNames"
                                            menu-icon=custom:GhdDownSvg
                                            label="Select an Attribute"
                                            variant="outlined"
                                            v-model="item.item[header.key]"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty]">
                                        </v-select>
                                        <v-text-field v-if="header.key === 'target'"
                                            id="TargetConditionGoalEditor-editTargetConditionGoalTarget-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                        <v-text-field v-if="header.key === 'year'"
                                            id="TargetConditionGoalEditor-editTargetConditionGoalYear-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                      </template>
                                </editDialog>
                                <v-row v-if="header.key === 'criterionLibrary'" style='flex-wrap:nowrap'>    
                                    <v-menu>
                                        <template v-slot:activator>
                                            <v-text-field                                              
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                v-model="item.item.criterionLibrary.mergedCriteriaExpression"/>
                                        </template>
                                    </v-menu>
                                    <v-btn
                                        id="TargetConditionGoalEditor-editTargetConditionGoalCriteria-vbtn"
                                        @click="onShowCriterionLibraryEditorDialog(item.item)"
                                        class="ghd-green" style="margin-top: 10px;"
                                        flat
                                        icon>
                                        <EditSvg />
                                    </v-btn>
                                </v-row>    
                                <div v-if="header.key === 'actions'">
                                    <v-btn 
                                        id="TargetConditionGoalEditor-deleteTargetConditionGoal-vbtn" 
                                        @click="onRemoveTargetConditionGoalsIcon(item.item)"  
                                        class="ghd-red" style="margin-top: 10px;"
                                        flat
                                        icon>
                                            <TrashCanSvg />
                                    </v-btn>
                                </div>
                            </td>
                        </tr>
                    </template>
                </v-data-table-server>
        </v-col>
        <v-divider
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
        <v-col cols="12">
            <v-btn flat
                v-show="hasSelectedLibrary || hasScenario"
                :disabled="selectedTargetConditionGoalIds.length === 0"
                variant="text"
                id="TargetConditionGoalEditor-deleteSelected-vbtn"
                class='ghd-red ghd-button'
                @click="onRemoveTargetConditionGoals"> 
                Delete Selected 
            </v-btn>
        </v-col>
        <v-col cols="12">
                <v-subheader v-show="hasSelectedLibrary && !hasScenario" class="ghd-control-label ghd-md-gray">Description</v-subheader>
                <v-textarea
                    v-show="hasSelectedLibrary && !hasScenario"
                    class="ghd-control-text ghd-control-border"
                    variant="outlined"
                    density="compact"
                    v-model="selectedTargetConditionGoalLibrary.description"
                    @update:model-value="checkHasUnsavedChanges()">
                </v-textarea>

        </v-col>
        <v-col>
            <v-row v-show="hasSelectedLibrary || hasScenario" style="padding-bottom: 40px;" justify="center">
                <v-btn flat
                    id="TargetConditionGoalEditor-deleteLibrary-btn"
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary"
                >
                    Delete Library
                </v-btn>
                <v-btn :disabled='!hasUnsavedChanges' flat
                    @click="onDiscardChanges"
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    style="margin-right: 5px;"
                    v-show="hasScenario"
                    variant="text"
                >
                    Cancel
                </v-btn>
                <v-btn
                    variant="outlined"
                    id="TargetConditionGoalEditor-CreateAsNewLibrary-btn"
                    @click="onShowCreateTargetConditionGoalLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    style="margin-left: 5px; margin-right: 5px;"
                    :disabled="disableCrudButtons()"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioTargetConditionGoals"
                    class='ghd-blue-bg ghd-white ghd-button-text'
                    v-show="hasScenario"
                    style="margin-left: 5px;"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                >
                    Save
                </v-btn>
                <v-btn
                    id="TargetConditionGoalEditor-UpdateLibrary-btn"
                    @click="onUpsertTargetConditionGoalLibrary"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    style="margin-left: 5px;"
                    v-show="!hasScenario"
                    :disabled="disableCrudButtons() || !hasUnsavedChanges || !hasLibraryEditPermission"
                >
                    Update Library
                </v-btn>
            </v-row>
        </v-col>
        <Alert
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
    </v-card>
</template>

<script setup lang="ts">
import { onBeforeUnmount, ref, shallowReactive, computed, onMounted, watch } from 'vue'; 
import editDialog from '@/shared/modals/Edit-Dialog.vue'
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
import { getUrl } from '@/shared/utils/get-url';
import { sortSelectItemsAlphabetically } from '@/shared/utils/sorter-utils'
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';

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
    function addErrorNotificationAction(payload?: any) { store.dispatch('addErrorNotification',payload);}
    async function getTargetConditionGoalLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getTargetConditionGoalLibraries',payload);} 
    function selectTargetConditionGoalLibraryAction(payload?: any) { store.dispatch('selectTargetConditionGoalLibrary',payload);}
    async function upsertTargetConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertTargetConditionGoalLibrary',payload);}
    async function deleteTargetConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteTargetConditionGoalLibrary',payload);}
    async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes',payload);}
    function setHasUnsavedChangesAction(payload?: any){ store.dispatch('setHasUnsavedChanges',payload);}
    async function getScenarioTargetConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('getScenarioTargetConditionGoals',payload);}
    async function upsertScenarioTargetConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioTargetConditionGoals',payload);}
    function addSuccessNotificationAction(payload?: any) { store.dispatch('addSuccessNotification',payload);}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario',payload);}
    function selectScenarioAction(payload?: any) { store.dispatch('selectScenario',payload);}

    function addedOrUpdatedTargetConditionGoalLibraryMutator(payload:any){store.commit('addedOrUpdatedTargetConditionGoalLibraryMutator',payload);} 
    function selectedTargetConditionGoalLibraryMutator(payload:any){store.commit('selectedTargetConditionGoalLibraryMutator',payload);} 
   
    let getNumericAttributesGetter = store.getters.getNumericAttributes;
    let getUserNameByIdGetter = store.getters.getUserNameById;

    const addedRows = ref<TargetConditionGoal[]>([]);
    const updatedRowsMap = ref<Map<string, [TargetConditionGoal, TargetConditionGoal]>>(new Map<string, [TargetConditionGoal, TargetConditionGoal]>());//0: original value | 1: updated value
    const deletionIds = ref<string[]>([]);
    const rowCache = ref<TargetConditionGoal[]>([]);

    let gridSearchTerm = '';
    let currentSearch = ref<string>('');
    //const pagination = ref<Pagination>(clone(emptyPagination));
    const pagination: Pagination = shallowReactive(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = ref(0);
    const currentPage = ref<TargetConditionGoal[]>([]);
    let initializing: boolean = true;
    let dateModified = ref<string>();

    const unsavedDialogAllowed = ref<boolean>(true);
    const trueLibrarySelectItemValue = ref<string|null>(''); 
    let librarySelectItemValueAllowedChanged: boolean = true;
    const librarySelectItemValue = ref<string|null>(null); 

    let selectedScenarioId: string = getBlankGuid();
    const librarySelectItems = ref<SelectItem[]>([]);
    const shareTargetConditionGoalLibraryDialogData = ref<ShareTargetConditionGoalLibraryDialogData>(clone(emptyShareTargetConditionGoalLibraryDialogData));
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
            width: '30%',
        },
        {
            title: 'Attribute',
            key: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
        {
            title: 'Target',
            key: 'target',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
        {
            title: 'Year',
            key: 'year',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
        {
            title: 'Criteria',
            key: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '40%',
        },
        {
            title: 'Actions',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        }
    ];
    const numericAttributeNames = ref<string[]>([]);
    let selectedGridRows= ref<TargetConditionGoal[]>([]);
    const selectedTargetConditionGoalIds = ref<string[]>([]);
    let selectedTargetConditionGoalForCriteriaEdit: TargetConditionGoal = clone(
        emptyTargetConditionGoal,
    );
    const showCreateTargetConditionGoalDialog = ref<boolean>(false);
    const criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
    const createTargetConditionGoalLibraryDialogData = ref<CreateTargetConditionGoalLibraryDialogData>(clone(emptyCreateTargetConditionGoalLibraryDialogData));
    const confirmDeleteAlertData = ref<AlertData>(clone(emptyAlertData));
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
    }

    onMounted(async () => {
        librarySelectItemValue.value = null;
        await getTargetConditionGoalLibrariesAction();
        numericAttributeNames.value = getPropertyValues('name', getNumericAttributesGetter);
        await getHasPermittedAccessAction();
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
            await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId})
            selectScenarioAction({ scenarioId: selectedScenarioId });     
            await initializePages();                                   
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
    watch(librarySelectItemValue,()=> {

        if(hasScenario.value){
            onLibrarySelectItemValueChanged();
            unsavedDialogAllowed.value = false;
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
    });

    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue.value = librarySelectItemValue.value;
        selectTargetConditionGoalLibraryAction({
            libraryId: librarySelectItemValue.value,
        });
    }

    watch(stateSelectedTargetConditionLibrary,()=> {
        selectedTargetConditionGoalLibrary.value = clone(stateSelectedTargetConditionLibrary.value);
    });

    watch(selectedTargetConditionGoalLibrary,()=> {
        hasSelectedLibrary.value = selectedTargetConditionGoalLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }

        numericAttributeNames.value = getPropertyValues('name', getNumericAttributesGetter);

        clearChanges();
        initializing = false;
        checkHasUnsavedChanges();
        if(hasSelectedLibrary.value)
            onPaginationChanged();
    })

    watch(selectedGridRows,()=> {
        selectedTargetConditionGoalIds.value = getPropertyValues('id', selectedGridRows.value,) as string[];
    });

    watch(stateNumericAttributes,()=> onStateNumericAttributesChanged)
    function onStateNumericAttributesChanged() {
        numericAttributeNames.value = getPropertyValues('name', stateNumericAttributes.value);
    }

    watch(stateScenarioTargetConditionGoals,()=> onStateScenarioTargetConditionGoalsChanged)
    function onStateScenarioTargetConditionGoalsChanged() {
        if (hasScenario.value) {
            currentPage.value = clone(stateScenarioTargetConditionGoals.value);
        }
    }

    watch(currentPage,()=> {

        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
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
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<TargetConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.value.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified
            },   
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch.value        
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
                      dateModified.value = data.slice(0, 10);
                   }
             }),
             await TargetConditionGoalService.getLibraryTargetConditionGoalPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<TargetConditionGoal>;
                    currentPage.value = data.items;
                    rowCache.value = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    if (!isNil(selectedTargetConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedTargetConditionGoalLibrary.value).then(() => isShared = isSharedLibrary.value);
                        
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
        createTargetConditionGoalLibraryDialogData.value = {
            showDialog: true,
            targetConditionGoals: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateTargetConditionGoalLibraryDialogResult(library: TargetConditionGoalLibrary) {
        createTargetConditionGoalLibraryDialogData.value = clone(emptyCreateTargetConditionGoalLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TargetConditionGoalLibrary, TargetConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.targetConditionGoals.length == 0 || !hasSelectedLibrary.value ? null : selectedTargetConditionGoalLibrary.value.id,
                    rowsForDeletion: library.targetConditionGoals.length == 0 ? [] : deletionIds.value,
                    updateRows: library.targetConditionGoals.length == 0 ? [] : Array.from(updatedRowsMap.value.values()).map(r => r[1]),
                    addedRows: library.targetConditionGoals.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    if(!hasScenario.value)
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
        showCreateTargetConditionGoalDialog.value = false;
        if (!isNil(newTargetConditionGoal)) {
            newTargetConditionGoal.libraryId = selectedTargetConditionGoalLibrary.value.id;

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

    function sortAlphabetically(items: SelectItem[]) {
        return sortSelectItemsAlphabetically(items);
    }

    function onShowCriterionLibraryEditorDialog(targetConditionGoal: TargetConditionGoal) {
        selectedTargetConditionGoalForCriteriaEdit = clone(
            targetConditionGoal,
        );

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: targetConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onEditTargetConditionGoalCriterionLibrary(criterionExpression: string,) {
        criterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

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
                updateRows: Array.from(updatedRowsMap.value.values()).map(r => r[1]),
                addedRows: addedRows.value,
                isModified: false
                },
                scenarioId: null
        }
        TargetConditionGoalService.upsertTargetConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();
                addedOrUpdatedTargetConditionGoalLibraryMutator(selectedTargetConditionGoalLibrary.value);
                selectedTargetConditionGoalLibraryMutator(selectedTargetConditionGoalLibrary.value.id);
                addSuccessNotificationAction({message: "Updated target condition goal library",});
            }
        });
    }

    function onUpsertScenarioTargetConditionGoals() {
        if (selectedTargetConditionGoalLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        TargetConditionGoalService.upsertScenarioTargetConditionGoals({
            libraryId: selectedTargetConditionGoalLibrary.value.id === uuidNIL ? null : selectedTargetConditionGoalLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.value.values()).map(r => r[1]),
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
        selectedTargetConditionGoalIds.value.forEach(_ => {
            removeRowLogic(_);
        });

        selectedTargetConditionGoalIds.value = [];
        onPaginationChanged();
    }
    function onRemoveTargetConditionGoalsIcon(targetConditionGoal: TargetConditionGoal) {
        removeRowLogic(targetConditionGoal.id);
        onPaginationChanged();
    }

    function removeRowLogic(id: string){
        if(isNil(find(propEq('id', id), addedRows.value))){
            deletionIds.value.push(id);
            if(!isNil(updatedRowsMap.value.get(id)))
                updatedRowsMap.value.delete(id)
        }           
        else{          
            addedRows.value = addedRows.value.filter((row) => row.id !== id)
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
            librarySelectItemValue.value = null;
            deleteTargetConditionGoalLibraryAction({
                libraryId: selectedTargetConditionGoalLibrary.value.id,
            });
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.value.values()).map(r => r[1]));
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

        let mapEntry = updatedRowsMap.value.get(rowId)

        if(isNil(mapEntry)){
            const row = rowCache.value.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row))
                updatedRowsMap.value.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
            updatedRowsMap.value.delete(rowId)

        checkHasUnsavedChanges();
    }

    function clearChanges(){
        updatedRowsMap.value.clear();
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
            updatedRowsMap.value.size > 0 || 
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedTargetConditionGoalLibrary.value, stateSelectedTargetConditionLibrary.value))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        if (hasUnsavedChanges.value && unsavedDialogAllowed.value) {
            
            confirm.require({
                message: "You have unsaved changes. Are you sure you wish to continue?",
                acceptLabel: "Continue",
                rejectLabel: "Close",
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
        let foundLibrary: TargetConditionGoalLibrary = emptyTargetConditionGoalLibrary;
        stateTargetConditionGoalLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    async function initializePages(){
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
            await TargetConditionGoalService.getScenarioTargetConditionGoalPage(selectedScenarioId, request).then(response => {
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
        shareTargetConditionGoalLibraryDialogData.value = {
            showDialog:true,
            targetConditionGoalLibrary: clone(targetConditionGoalLibrary)
        }
    }

    function onShareTargetConditionGoalDialogSubmit(targetConditionGoalLibraryUsers: TargetConditionGoalLibraryUser[]) {
            shareTargetConditionGoalLibraryDialogData.value = clone(emptyShareTargetConditionGoalLibraryDialogData);

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
                            getIsSharedLibraryAction(selectedTargetConditionGoalLibrary.value).then(() => isShared = isSharedLibrary.value);
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
