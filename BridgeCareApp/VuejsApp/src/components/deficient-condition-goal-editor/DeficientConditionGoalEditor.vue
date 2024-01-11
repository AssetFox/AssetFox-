<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col cols="12">
            <v-row align="center" justify="space-between">
                <v-col cols="auto">
                    <div style="margin-bottom: 10px;">
                        <v-subheader class="ghd-md-gray ghd-control-label">Select a Deficient Condition Goal Library</v-subheader>
                    </div>
                    <v-select
                        id="DeficientConditionGoalEditor-librarySelect-vselect"
                        :items="librarySelectItems"
                        variant="outlined"
                        v-model="librarySelectItemValue"
                        menu-icon=custom:GhdDownSvg
                        item-title="text"
                        item-value="value" 
                        density="compact"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                </v-col>
                <v-col cols="auto">
                    <v-btn v-if="hasScenario" 
                        class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                        @click="importLibrary()"
                        :disabled="importLibraryDisabled"
                        style="margin-right: 5px;"
                    >
                        Import
                    </v-btn>
                </v-col>
                <v-col cols="auto">
                    <v-row v-show="hasSelectedLibrary && ! hasScenario">
                        <div v-if="!hasScenario && hasSelectedLibrary" class="header-text-content owner-padding" align="center"                        >
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <v-btn 
                            id="DeficientConditionGoalEditor-shareLibrary-vbtn" 
                            @click='onShowShareDeficientConditionGoalLibraryDialog(selectedDeficientConditionGoalLibrary)' 
                            class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-left: 10px"
                            variant = "outlined"
                            v-show='!hasScenario && hasSelectedLibrary'>
                        Share Library
                        </v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "auto">
                    <v-btn id="DeficientConditionGoalEditor-createNewLibrary-vbtn" @click="onShowCreateDeficientConditionGoalLibraryDialog(false)"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin: 5px;"
                        v-show="!hasScenario"
                        variant = "outlined">    
                        Create New Library        
                    </v-btn>
                </v-col>
        
                <v-col cols="auto" >
                    <!-- <v-btn v-if="hasScenario" 
                            class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                            @click="importLibrary()"
                            :disabled="importLibraryDisabled"
                            style="margin-right: 5px;"
                        >
                            Import
                        </v-btn> -->
                    <v-btn
                        id="DeficientConditionGoalEditor-addDeficientConditionGoal-vbtn"
                        @click="showCreateDeficientConditionGoalDialog = true"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="hasSelectedLibrary || hasScenario"
                        variant = "outlined"
                        style="margin-right: 5px;">
                        Add Deficient Condition Goal
                    </v-btn>          
                </v-col>
            </v-row>
        </v-col>
    </v-row>
    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'>
        <b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b>
    </div>  
        <v-col cols = "12" v-show="hasSelectedLibrary || hasScenario">
            <div>
                <v-data-table-server
                    id="DeficientConditionGoalEditor-deficientConditionGoals-vdatatable"
                    :headers="deficientConditionGoalGridHeaders"
                    :items="currentPage"  
                    :pagination.sync="pagination"
                    :must-sort='true'
                    sort-asc-icon="custom:GhdTableSortAscSvg"
                    sort-desc-icon="custom:GhdTableSortDescSvg"
                    class="ghd-table v-table__overflow"
                    item-key="id"
                    show-select
                    v-model="selectedGridRows"                
                    :items-length="totalItems"
                    :items-per-page-options="[
                        {value: 5, title: '5'},
                        {value: 10, title: '10'},
                        {value: 25, title: '25'},
                    ]"
                    v-model:sort-by="pagination.sort"
                    v-model:page="pagination.page"
                    v-model:items-per-page="pagination.rowsPerPage"
                    item-value="name"
                    return-object
                    @update:options="onPaginationChanged"
                >
                    <template  v-slot:item="item">
                        <tr>
                        <td>
                            <v-checkbox
                                id="DeficientConditionGoalEditor-selectForDelete-vcheckbox"
                                hide-details
                                primary
                                density="compact"
                                style="margin: 5px;"
                                v-model="selectedGridRows" :value="item.item"
                            ></v-checkbox>
                        </td>
                        <td v-for="header in deficientConditionGoalGridHeaders">
                            <div>
                                <editDialog v-if="header.key !== 'criterionLibrary' && header.key !== 'action'"
                                    v-model:return-value="item.item[header.key]"
                                    @save="onEditDeficientConditionGoalProperty(item.item,header.key,item.item[header.key])"
                                    size="large"
                                    lazy>
                                    <v-text-field v-if="header.key !== 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        density="compact"
                                        variant="underlined"
                                        :model-value="item.item[header.key]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                    <v-text-field v-if="header.key === 'allowedDeficientPercentage'"
                                        readonly
                                        class="sm-txt"
                                        density="compact"
                                        variant="underlined"
                                        :model-value="item.item[header.key]"
                                        :rules="[rules['generalRules'].valueIsNotEmpty,
                                            rules['generalRules'].valueIsWithinRange(item.item[header.key],[0, 100])]"/>

                                    <template v-slot:input>
                                        <v-text-field v-if="header.key === 'name'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalName-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-select v-if="header.key === 'attribute'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalAttribute-vselect"
                                            :items="numericAttributeNames"
                                            menu-icon=custom:GhdDownSvg
                                            label="Select an Attribute"
                                            variant="outlined"
                                            v-model="item.item[header.key]"
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty]">
                                        </v-select>

                                        <v-text-field v-if="header.key === 'deficientLimit'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalLimit-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            type="number"
                                            v-maska:[limitMask]
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                        <v-text-field v-if="header.key === 'allowedDeficientPercentage'"
                                            id="DeficientConditionGoalEditor-editDeficientConditionGoalPercentage-vtextfield"
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model.number="item.item[header.key]"
                                            v-maska:[percentMask]
                                            :rules="[
                                                rules['generalRules'].valueIsNotEmpty,
                                                rules['generalRules'].valueIsWithinRange(
                                                    item.item[header.key],[0, 100])]"/>
                                    </template>
                                </editDialog>
                                
                                <v-row
                                    v-if="header.key === 'criterionLibrary'">
                                    <v-menu>
                                        <template v-slot:activator>
                                            <v-text-field
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="item.item.criterionLibrary.mergedCriteriaExpression"/>
                                        </template>
                                            <v-textarea
                                                :model-value="item.item.criterionLibrary.mergedCriteriaExpression"
                                                no-resize
                                                variant="outlined"
                                                readonly
                                                rows="3"/>
                                    </v-menu>
                                    <v-btn
                                        id="DeficientConditionGoalEditor-editDeficientConditionGoalCriteria-vbtn"
                                        @click="onShowCriterionLibraryEditorDialog(item.item)"
                                        class="ghd-blue"
                                        style="margin-top: 10px;"
                                        flat>
                                        <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-row>
                                <div v-if="header.key === 'action'" style="margin-bottom: 10px;">
                                    <v-btn id="DeficientConditionGoalEditor-deleteDeficientConditionGoal-vbtn" @click="onRemoveSelectedDeficientConditionGoal(item.item.id)"
                                          class="ghd-blue" style="margin-top: 10px;" flat>
                                        <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </div>                               
                            </div>
                        </td>
                    </tr>
                    </template>
                </v-data-table-server> 
                <v-btn
                    id="DeficientConditionGoalEditor-deleteSelected-vbtn"
                    :disabled="selectedDeficientConditionGoalIds.length === 0"
                    @click="onRemoveSelectedDeficientConditionGoals"
                    class='ghd-blue ghd-button'
                    variant="text">

                    Delete Selected
                </v-btn>              
            </div>
        </v-col>
        <v-col v-show="hasSelectedLibrary && !hasScenario">
            <v-row justify-center>
                <v-col>
                    <v-subheader class="ghd-subheader ">Description</v-subheader>
                    <v-textarea
                        class="ghd-text-field-border"
                        no-resize
                        variant="outlined"
                        rows="4"
                        v-model="selectedDeficientConditionGoalLibrary.description"
                        @update:model-value="checkHasUnsavedChanges()"
                    >
                    </v-textarea>
                </v-col>
            </v-row>
        </v-col>
        <v-col v-show="hasSelectedLibrary || hasScenario">
            <v-row justify="center" style="padding-bottom: 40px;">
                <v-btn
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    style="margin-right: 5px;"
                    :disabled="!hasUnsavedChanges"
                    variant = "text">
                    Cancel
                </v-btn>
                <v-btn
                    id="DeficientConditionGoalEditor-deleteLibrary-vbtn"
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    style="margin-right: 5px;"
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                    variant = "text">
                    Delete Library
                </v-btn>    
                <v-btn
                    id="DeficientConditionGoalEditor-createAsNewLibrary-vbtn"
                    @click="onShowCreateDeficientConditionGoalLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    :disabled="disableCrudButtons()"
                    style="margin-right: 5px; margin-left: 5px;"
                    variant = "outlined">
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioDeficientConditionGoals"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="hasScenario"
                    style="margin-left: 5px;"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges">
                    Save
                </v-btn>
                <v-btn
                    id="DeficientConditionGoalEditor-updateLibrary-vbtn"
                    @click="onUpsertDeficientConditionGoalLibrary"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                    style="margin-left: 5px;">
                    Update Library
                </v-btn>               
                       
            </v-row>
        </v-col>
</v-card>
        <Alert
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
    <ConfirmDialog></ConfirmDialog>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, Ref, shallowReactive, ShallowRef, shallowRef, watch } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
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
import { LibraryUser } from '@/shared/models/iAM/user';
import { emptyShareDeficientConditionGoalLibraryDialogData, ShareDeficientConditionGoalLibraryDialogData } from '@/shared/models/modals/share-deficient-condition-goal-data';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import { getUrl } from '@/shared/utils/get-url';

    let store = useStore();
    const confirm = useConfirm();
    const $router = useRouter();
    let stateDeficientConditionGoalLibraries = computed<DeficientConditionGoalLibrary[]>(() => store.state.deficientConditionGoalModule.deficientConditionGoalLibraries);
    let stateSelectedDeficientConditionGoalLibrary = computed<DeficientConditionGoalLibrary>(() => store.state.deficientConditionGoalModule.selectedDeficientConditionGoalLibrary);
    let stateNumericAttributes = computed<Attribute[]>(() => store.state.attributeModule.numericAttributeNames);
    let stateScenarioDeficientConditionGoals = computed<DeficientConditionGoal[]>(() => store.state.deficientConditionGoalModule.scenarioDeficientConditionGoals);
    let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
    let hasPermittedAccess = computed<boolean>(() => store.state.deficientConditionGoalModule.hasPermittedAccess);
    let isSharedLibrary = computed<boolean>(() => store.state.deficientConditionGoalModule.isSharedLibrary);

    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks', payload);}
    async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedDeficientConditionGoalLibrary', payload);}
    async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess', payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}
    async function getDeficientConditionGoalLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getDeficientConditionGoalLibraries', payload);}
    async function selectDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectDeficientConditionGoalLibrary', payload);}
    async function upsertDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertDeficientConditionGoalLibrary', payload);}
    async function deleteDeficientConditionGoalLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteDeficientConditionGoalLibrary', payload);}
    async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes', payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges', payload);}
    async function getScenarioDeficientConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('getScenarioDeficientConditionGoals', payload);}
    async function upsertScenarioDeficientConditionGoalsAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioDeficientConditionGoals', payload);}
    async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification', payload);}
    async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario', payload);}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario', payload);}
    
    function addedOrUpdatedDeficientConditionGoalLibraryMutator(payload: any){store.commit('addedOrUpdatedDeficientConditionGoalLibraryMutator', payload);}
    function selectedDeficientConditionGoalLibraryMutator(payload: any){store.commit('selectedDeficientConditionGoalLibraryMutator', payload);}

    let getNumericAttributesGetter: any = store.getters.getNumericAttributes;
    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    const selectedScenarioId = ref<string>(getBlankGuid());
    let librarySelectItems  = ref<SelectItem[]>([]);
    const selectedDeficientConditionGoalLibrary = ref<DeficientConditionGoalLibrary>(clone(emptyDeficientConditionGoalLibrary));
    let hasSelectedLibrary = ref(false);
    let dateModified = ref<string>();
    let deficientConditionGoalGridHeaders: any[] = [
        {
            title: 'Name',
            key: 'name',
            align: 'start',
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
            width: '12%',
        },
        {
            title: 'Deficient Limit',
            key: 'deficientLimit',
            align: 'left',
            sortable: false,
            class: '',
            width: '8%',
        },
        {
            title: 'Allowed Deficient Percentage',
            key: 'allowedDeficientPercentage',
            align: 'left',
            sortable: false,
            class: '',
            width: '8%',
        },
        {
            title: 'Criteria',
            key: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '30%',
        },
        {
            title: 'Action',
            key: 'action',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        }
    ];
    let numericAttributeNames: string[] = [];
    let selectedGridRows: ShallowRef<DeficientConditionGoal[]> = ref([]);
    let selectedDeficientConditionGoalIds: string[] = [];
    const selectedDeficientConditionGoalForCriteriaEdit = ref< DeficientConditionGoal >(clone(emptyDeficientConditionGoal));
    const showCreateDeficientConditionGoalDialog = ref<boolean>(false);
    const criterionEditorDialogData = ref< GeneralCriterionEditorDialogData >(clone(emptyGeneralCriterionEditorDialogData));
    const createDeficientConditionGoalLibraryDialogData = ref<CreateDeficientConditionGoalLibraryDialogData>(clone(emptyCreateDeficientConditionGoalLibraryDialogData));
    const confirmDeleteAlertData = ref<AlertData>(clone(emptyAlertData));
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario = ref(false);
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let importLibraryDisabled = ref(true);
    let scenarioHasCreatedNew: boolean = false;

    const addedRows = ref<DeficientConditionGoal[]>([]);
    let updatedRowsMap:Map<string, [DeficientConditionGoal, DeficientConditionGoal]> = new Map<string, [DeficientConditionGoal, DeficientConditionGoal]>();//0: original value | 1: updated value
    let deletionIds: string[] = [];
    let rowCache: DeficientConditionGoal[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    //let pagination: ShallowRef<Pagination> = shallowRef(clone(emptyPagination));
    const pagination: Pagination = shallowReactive(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: ShallowRef<DeficientConditionGoal[]> = shallowRef([]);
    let initializing: boolean = true;
    let isShared: boolean = false;

    const shareDeficientConditionGoalLibraryDialogData = ref<ShareDeficientConditionGoalLibraryDialogData>(clone(emptyShareDeficientConditionGoalLibraryDialogData));

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: Ref<string | null> = ref(null);
    let parentLibraryId: string = "";
    let parentLibraryName: string = "None";
    const scenarioLibraryIsModified = ref<boolean>(false);
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let libraryImported: boolean = false;

    const percentMask = { mask: '###' };
    const limitMask = { mask: '##########' };

    created();
    function created() {
    }
    onMounted(async () => {
        librarySelectItemValue.value = null;
        await getDeficientConditionGoalLibrariesAction()
        numericAttributeNames = getPropertyValues('name', getNumericAttributesGetter);
        await getHasPermittedAccessAction()
        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.DeficientConditionGoal) !== -1) {
            selectedScenarioId.value = $router.currentRoute.value.query.scenarioId as string;

            if (selectedScenarioId.value === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                $router.push('/Scenarios/');
            }

            hasScenario.value = true;
            await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId.value})       
            selectScenarioAction({ scenarioId: selectedScenarioId.value });        
            await initializePages();                                      
        }
                
    });
    onBeforeUnmount(() => beforeDestroy); 
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(stateDeficientConditionGoalLibraries, () => onStateDeficientConditionGoalLibrariesChanged())
    function onStateDeficientConditionGoalLibrariesChanged() {
        librarySelectItems.value = stateDeficientConditionGoalLibraries.value.map(
            (library: DeficientConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   //this is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue, () => onLibrarySelectItemValueChangedCheckUnsaved())
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
        librarySelectItemValueAllowedChanged = true;
        librarySelectItems.value.forEach(library => {
            if (library.value === librarySelectItemValue.value) {
                parentLibraryName = library.text;
            }
        });
    }

    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        if(!hasScenario.value || isNil(librarySelectItemValue.value))
            selectDeficientConditionGoalLibraryAction({
                libraryId: librarySelectItemValue.value,
            });
        else if(!isNil(librarySelectItemValue.value) && !scenarioHasCreatedNew)
        {
            importLibraryDisabled.value = false;
        }

        scenarioHasCreatedNew = false;
    }

    watch(stateSelectedDeficientConditionGoalLibrary, () => onStateSelectedDeficientConditionGoalLibraryChanged())
    function onStateSelectedDeficientConditionGoalLibraryChanged() {
        selectedDeficientConditionGoalLibrary.value = clone(
            stateSelectedDeficientConditionGoalLibrary.value,
        );
    }

    watch(selectedDeficientConditionGoalLibrary, () => onSelectedDeficientConditionGoalLibraryChanged())
    function onSelectedDeficientConditionGoalLibraryChanged() {
        if (!isNil(selectedDeficientConditionGoalLibrary.value)) {
            hasSelectedLibrary.value = selectedDeficientConditionGoalLibrary.value.id !== uuidNIL;
            
        }
        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
        }

        clearChanges();
        initializing = false;
        if(hasSelectedLibrary.value)
            onPaginationChanged();
        
        checkHasUnsavedChanges();
    }

    watch(selectedGridRows, () => onSelectedDeficientRowsChanged())
    function onSelectedDeficientRowsChanged() {
        selectedDeficientConditionGoalIds = getPropertyValues('id', selectedGridRows.value,) as string[];
    }

    watch(stateNumericAttributes, () => onStateNumericAttributesChanged())
    function onStateNumericAttributesChanged() {
        numericAttributeNames = getPropertyValues('name', stateNumericAttributes.value);
    }

    watch(stateScenarioDeficientConditionGoals, () => onStateScenarioDeficientConditionGoalsChanged())
    function onStateScenarioDeficientConditionGoalsChanged() {
        if (hasScenario.value) {
            currentPage.value = clone(stateScenarioDeficientConditionGoals.value);
        }
    }

    watch(currentPage, () => onCurrentPageChanged())
    function onCurrentPageChanged() {
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }

    watch(isSharedLibrary, () => onStateSharedAccessChanged())
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }

    watch(pagination, () => onPaginationChanged())
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<DeficientConditionGoal>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: librarySelectItemValue.value !== null && importLibraryDisabled.value ? librarySelectItemValue.value : null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified.value
            },           
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId.value !== uuidNIL)
            await DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(selectedScenarioId.value, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                }
            });
        else if(hasSelectedLibrary.value)
            await DeficientConditionGoalService.getDeficientLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified.value = data.slice(0, 10);
                   }
             }),    
             await DeficientConditionGoalService.getLibraryDeficientConditionGoalPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    if (!isNil(selectedDeficientConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedDeficientConditionGoalLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }      

                }
            });     
    }

    function importLibrary() {
        setParentLibraryName(librarySelectItemValue.value ? librarySelectItemValue.value : "");
        selectDeficientConditionGoalLibraryAction({
            libraryId: librarySelectItemValue.value,
        });
        importLibraryDisabled.value = true;
        scenarioLibraryIsModified.value = false;
        libraryImported = true;

    }

    function getOwnerUserName(): string {
        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedDeficientConditionGoalLibrary.value.owner);
        }
        return getUserName();
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedDeficientConditionGoalLibrary.value.owner) == getUserName();
    }

    function onShowCreateDeficientConditionGoalLibraryDialog(createExistingLibraryAsNew: boolean) {
        createDeficientConditionGoalLibraryDialogData.value = {
            showDialog: true,
            deficientConditionGoals: createExistingLibraryAsNew
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateDeficientConditionGoalLibraryDialogResult(library: DeficientConditionGoalLibrary,) {
        createDeficientConditionGoalLibraryDialogData.value = clone(emptyCreateDeficientConditionGoalLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<DeficientConditionGoalLibrary, DeficientConditionGoal> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.deficientConditionGoals.length == 0 || !hasSelectedLibrary.value? null : selectedDeficientConditionGoalLibrary.value.id,
                    rowsForDeletion: library.deficientConditionGoals.length == 0 ? [] : deletionIds,
                    updateRows: library.deficientConditionGoals.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.deficientConditionGoals.length == 0 ? [] : addedRows.value,
                    isModified: false,
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId.value : null
            }
            DeficientConditionGoalService.upsertDeficientConditionGoalLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = library.id;
                    
                    if(library.deficientConditionGoals.length == 0){
                        clearChanges();
                    }

                    if(hasScenario.value){
                        scenarioHasCreatedNew = true;
                        importLibraryDisabled.value = true;
                    }
                    
                    addedOrUpdatedDeficientConditionGoalLibraryMutator(library);
                    selectedDeficientConditionGoalLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added deficient condition goal library'})
                }               
            })
        }
    }

    function onAddDeficientConditionGoal(newDeficientConditionGoal: DeficientConditionGoal) {
        showCreateDeficientConditionGoalDialog.value = false;

        if (!isNil(newDeficientConditionGoal)) {
            addedRows.value.push(newDeficientConditionGoal);
            onPaginationChanged()
        }
    }

    function onEditDeficientConditionGoalProperty(deficientConditionGoal: DeficientConditionGoal, property: string, value: any) {
        onUpdateRow(deficientConditionGoal.id, clone(deficientConditionGoal))
        onPaginationChanged();
    }

    function onShowCriterionLibraryEditorDialog(deficientConditionGoal: DeficientConditionGoal,) {
        selectedDeficientConditionGoalForCriteriaEdit.value = clone(
            deficientConditionGoal,
        );

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: deficientConditionGoal.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onEditDeficientConditionGoalCriterionLibrary(criterionExpression: string,) {
        criterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedDeficientConditionGoalForCriteriaEdit.value.id !== uuidNIL) {
            if(selectedDeficientConditionGoalForCriteriaEdit.value.criterionLibrary.id === getBlankGuid())
                selectedDeficientConditionGoalForCriteriaEdit.value.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedDeficientConditionGoalForCriteriaEdit.value.id,
             { ...selectedDeficientConditionGoalForCriteriaEdit.value, 
                criterionLibrary: {... selectedDeficientConditionGoalForCriteriaEdit.value.criterionLibrary, mergedCriteriaExpression: criterionExpression}})                
            onPaginationChanged();
        }

        selectedDeficientConditionGoalForCriteriaEdit.value = clone(
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
                addedRows: addedRows.value,
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
        if (selectedDeficientConditionGoalLibrary.value.id === uuidNIL || hasUnsavedChanges.value && libraryImported === false) { scenarioLibraryIsModified.value = true; }
        else { scenarioLibraryIsModified.value = false; }

        DeficientConditionGoalService.upsertScenarioDeficientConditionGoals({
            libraryId: selectedDeficientConditionGoalLibrary.value.id === uuidNIL ? null : selectedDeficientConditionGoalLibrary.value.id,
            rowsForDeletion: deletionIds,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified.value
        }, selectedScenarioId.value).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges();
                librarySelectItemValue.value = null;
                resetPage();
                addSuccessNotificationAction({message: "Modified scenario's deficient condition goals"});
                importLibraryDisabled.value = true;
                libraryImported = false;
            }           
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario.value) {
                clearChanges();
                resetPage();
                importLibraryDisabled.value = true;
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
        if(isNil(find(propEq('id', id), addedRows.value))){
            deletionIds.push(id);
            if(!isNil(updatedRowsMap.get(id)))
                updatedRowsMap.delete(id)
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
            deleteDeficientConditionGoalLibraryAction({
                libraryId: selectedDeficientConditionGoalLibrary.value.id,
            });
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
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

        if (hasSelectedLibrary.value) {
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
        deletionIds = [];
    }

    function resetPage(){
        pagination.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){

        const hasUnsavedChanges: boolean = 
            deletionIds.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedDeficientConditionGoalLibrary.value , stateSelectedDeficientConditionGoalLibrary.value))
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

    function onShowShareDeficientConditionGoalLibraryDialog(deficientConditionGoalLibrary: DeficientConditionGoalLibrary) {
        shareDeficientConditionGoalLibraryDialogData.value = {
            showDialog:true,
            deficientConditionGoalLibrary: clone(deficientConditionGoalLibrary)
        }
    }

    function onShareDeficientConditionGoalDialogSubmit(deficientConditionGoalLibraryUsers: DeficientConditionGoalLibraryUser[]) {
        shareDeficientConditionGoalLibraryDialogData.value = clone(emptyShareDeficientConditionGoalLibraryDialogData);

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
                    if (!isNil(selectedDeficientConditionGoalLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedDeficientConditionGoalLibrary.value).then(() => isShared = isSharedLibrary.value);
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
    async function initializePages(){
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
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId.value !== uuidNIL)
            await DeficientConditionGoalService.getScenarioDeficientConditionGoalPage(selectedScenarioId.value, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<DeficientConditionGoal>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName; //store original
                    scenarioLibraryIsModified.value = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
    }

</script>

<style>
.deficients-data-table {
    height: 340px;
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
