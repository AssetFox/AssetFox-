<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-row>
        <v-col>
            <v-row align="center" justify="space-between">
                <v-col cols = "auto" class="ghd-constant-header">                   
                    <div style="margin-bottom: 10px;">
                        <v-subheader class="ghd-control-label ghd-md-gray">Remaining Life Limit Library</v-subheader>
                    </div>
                    <v-select id="RemainingLifeLimitEditor-lifeLimitLibrary-select"
                            class="ghd-select ghd-text-field ghd-text-field-border vs-style"
                            :items="selectListItems"
                            item-title="text"
                            item-value="value"
                            append-icon=ghd-down
                            v-model="librarySelectItemValue"
                            variant="outlined"
                            density="compact"
                            >
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>                   
                </v-col>
                <v-col cols = "auto" class="ghd-constant-header">
                    <v-row v-if="hasSelectedLibrary && !hasScenario" style="padding-top: 10px; padding-left: 10px">
                        <div class="header-text-content owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <v-divider vertical 
                            v-if="hasSelectedLibrary && !hasScenario">
                        </v-divider>
                        <v-btn id="RemainingLifeLimitEditor-shareLibrary-vbtn" @click='onShowShareRemainingLifeLimitLibraryDialog(selectedRemainingLifeLimitLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "auto" class="ghd-constant-header">     
                    <v-btn 
                        id="RemainingLifeLimitEditor-addRemainingLifeLimit-btn"
                        class="ghd-white-bg ghd-blue ghd-button"
                        style="margin: 5px;"
                        @click="onShowCreateRemainingLifeLimitDialog"
                        v-show="librarySelectItemValue != null || hasScenario"
                        variant = "outlined">Add Remaining Life Limit
                    </v-btn>
                    <v-btn
                        id="RemainingLifeLimitEditor-createNewLibrary-vbtn"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        style="margin: 5px;"
                        @click="onShowCreateRemainingLifeLimitLibraryDialog(false)"
                        v-show="!hasScenario"
                        variant = "outlined">Create New Library
                    </v-btn>
                </v-col>
            </v-row>
        </v-col>
        <div v-show="librarySelectItemValue != null || hasScenario">
            <v-data-table-server
            id="RemainingLifeLimitEditor-attributes-dataTable"
            :headers="gridHeaders"
            :pagination.sync="pagination"
            :must-sort='true'
            sort-icon=ghd-table-sort
            class="elevation-1 fixed-header v-table__overflow"
            v-model="selectedGridRows"

            :items="currentPage"                      
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
            @update:options="onPaginationChanged"
            >
            <template v-slot:headers="props">
                <tr>
                    <th
                    v-for="header in gridHeaders"
                    :key="header.title"
                    >
                        {{header.title}}
                    </th>
                </tr>
            </template>
                <template v-slot:item="props">
                    <tr :active="props.item.selected" @click="props.item.selected = !props.item.selected">
                        <td>
                            <editDialog
                                v-model:return-value="props.item.attribute"
                                size="large"
                                lazy
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'attribute',
                                        props.item.attribute,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    variant="underlined"
                                    :model-value="props.item.attribute"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template v-slot:input>
                                    <v-select id="RemainingLifeLimitEditor-editAttribute-select"
                                        :items="numericAttributeSelectItems"
                                        item-title="text"
                                        item-value="value"
                                        append-icon=ghd-down
                                        label="Select an Attribute"
                                        variant="outlined"
                                        v-model="props.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </editDialog>
                        </td>
                        <td>
                            <editDialog
                                v-model:return-value="props.item.value"
                                size="large"
                                lazy
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'value',
                                        props.item.value,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    variant="underlined"
                                    :model-value="props.item.value"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template v-slot:input>
                                    <v-text-field
                                        label="Edit"
                                        single-line
                                        :mask="'##########'"
                                        variant="underlined"
                                        v-model.number="props.item.value"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </editDialog>
                        </td>
                        <td v-if="props.item.criterionLibrary.mergedCriteriaExpression != '' && props.item.criterionLibrary.mergedCriteriaExpression != null" >
                            {{ props.item.criterionLibrary.mergedCriteriaExpression}}
                        </td>
                        <td v-else>-
                        </td>
                        <td class="px-0">
                            <v-btn id="RemainingLifeLimitEditor-editCriteria-vbtn" @click="onShowCriterionLibraryEditorDialog(props.item)" flat>
                                <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                            </v-btn>   
                        </td>
                        <td justify-end>
                            <v-btn id="RemainingLifeLimitEditor-deleteAttribute-btn" @click="onRemoveRemainingLifeLimitIcon(props.item)" flat>
                                <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                            </v-btn>                          
                        </td>
                    </tr>
                </template>
                </v-data-table-server>
        </div>
        <v-divider :thickness="4" class="border-opacity-100" ></v-divider>
        <v-col v-show="hasSelectedLibrary && !hasScenario" cols="12">
            <v-subheader class="ghd-subheader">Description</v-subheader>
            <v-textarea
                class="ghd-control-text ghd-control-border"
                v-model="selectedRemainingLifeLimitLibrary.description"
                @update:model-value="checkHasUnsavedChanges()"
                variant="outlined" density="compact"
            >
            </v-textarea>
        </v-col>
        <v-col>
            <v-row v-show="hasSelectedLibrary || hasScenario" style="padding-bottom: 40px;" align-content="center" justify="center">
                <v-btn id="RemainingLifeLimitEditor-cancel-btn" style="margin: 5px;" class="ghd-blue" variant = "outlined" v-show="hasScenario" @click="onDiscardChanges" :disabled="!hasUnsavedChanges">Cancel</v-btn>
                <v-btn id="RemainingLifeLimitEditor-deleteLibrary-btn" style="margin: 5px;" class="ghd-blue" variant = "outlined" v-show="!hasScenario" @click="onShowConfirmDeleteAlert">Delete Library</v-btn>
                <v-btn id="RemainingLifeLimitEditor-createAsNewLibrary-btn" style="margin: 5px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' @click="onShowCreateRemainingLifeLimitLibraryDialog(true)" variant = "outlined">Create as New Library</v-btn>
                <v-btn id="RemainingLifeLimitEditor-save-btn" style="margin: 5px;" class="ghd-blue-bg ghd-white ghd-button" v-show="hasScenario" @click="onUpsertScenarioRemainingLifeLimits" :disabled="disableCrudButton() || !hasUnsavedChanges">Save</v-btn>
                <v-btn id="RemainingLifeLimitEditor-updateLibrary-btn" style="margin: 5px;" class="ghd-blue-bg ghd-white ghd-button" v-show="!hasScenario" :disabled="disableCrudButton() || !hasUnsavedChanges" @click="onUpsertRemainingLifeLimitLibrary">Update Library</v-btn>
            </v-row>
        </v-col>     
        <!-- <ConfirmDeleteAlert -->
        <Alert
          :dialogData="confirmDeleteAlertData"
          @submit="onSubmitConfirmDeleteAlertResult"
        />
        <CreateRemainingLifeLimitLibraryDialog
          :dialogData="createRemainingLifeLimitLibraryDialogData"
          @submit="onSubmitCreateRemainingLifeLimitLibraryDialogResult"
        />
        <CreateRemainingLifeLimitDialog 
          :dialogData="createRemainingLifeLimitDialogData"
          @submit="onAddRemainingLifeLimit"
        />
        <ShareRemainingLifeLimitLibraryDialog :dialogData="shareRemainingLifeLimitLibraryDialogData"
            @submit="onShareRemainingLifeLimitDialogSubmit" 
        />
        <GeneralCriterionEditorDialog 
          :dialogData="criterionEditorDialogData"
          @submit="onEditRemainingLifeLimitCriterionLibrary"
        />
        <ConfirmDialog></ConfirmDialog>
    </v-row>
    </v-card>
</template>

<script setup lang="ts">
import { ref, shallowReactive, shallowRef, ShallowRef, watch, onMounted } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import {
    emptyRemainingLifeLimit,
    emptyRemainingLifeLimitLibrary,
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
    RemainingLifeLimitLibraryUser
} from '@/shared/models/iAM/remaining-life-limit';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { clone, isNil, propEq, any, find } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
import {
    CreateRemainingLifeLimitLibraryDialogData,
    emptyCreateRemainingLifeLimitLibraryDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import CreateRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
import {
    CreateRemainingLifeLimitDialogData,
    emptyCreateRemainingLifeLimitDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
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
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import ShareRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/ShareRemainingLifeLimitLibraryDialog.vue'; 
import RemainingLifeLimitService from '@/services/remaining-life-limit.service';
import { emptyShareRemainingLifeLimitLibraryDialogData, ShareRemainingLifeLimitLibraryDialogData } from '@/shared/models/modals/share-remaining-life-limit-data';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { LibraryUser } from '@/shared/models/iAM/user';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import { computed } from 'vue';
import { onBeforeUnmount } from 'vue';
import { createDecipheriv } from 'crypto';
import { getUrl } from '@/shared/utils/get-url';

    let store = useStore();
    const confirm = useConfirm();
    const $router = useRouter();

    const stateRemainingLifeLimitLibraries = computed<RemainingLifeLimitLibrary[]>(() => store.state.remainingLifeLimitModule.remainingLifeLimitLibraries);
    const stateSelectedRemainingLifeLimitLibrary = computed<RemainingLifeLimitLibrary>(() =>store.state.remainingLifeLimitModule.selectedRemainingLifeLimitLibrary);
    let stateNumericAttributes = computed<Attribute[]>(() =>store.state.attributeModule.numericAttributes);
    let stateScenarioRemainingLifeLimits = computed<RemainingLifeLimit[]>(() =>store.state.remainingLifeLimitModule.scenarioRemainingLifeLimits);
    let hasUnsavedChanges = computed<boolean>(() =>store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let hasAdminAccess = computed<boolean>(() =>store.state.authenticationModule.hasAdminAccess) ;
    let isSharedLibrary = computed<boolean>(() =>store.state.remainingLifeLimitModule.isSharedLibrary);

    async function getIsSharedLibraryAction(payload?: any): Promise<any>{await store.dispatch('getIsSharedRemainingLifeLimitLibrary', payload)}
    async function getRemainingLifeLimitLibrariesAction(payload?: any): Promise<any>{await store.dispatch('getRemainingLifeLimitLibraries', payload)}
    async function upsertRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('upsertRemainingLifeLimitLibrary', payload)}
    async function deleteRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('deleteRemainingLifeLimitLibrary', payload)}
    async function selectRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('selectRemainingLifeLimitLibrary', payload)}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification', payload)}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges', payload)}
    async function getScenarioRemainingLifeLimitsAction(payload?: any): Promise<any>{await store.dispatch('getScenarioRemainingLifeLimits', payload)}
    async function upsertScenarioRemainingLifeLimitsAction(payload?: any): Promise<any>{await store.dispatch('upsertScenarioRemainingLifeLimits', payload)}
    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification', payload)}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch('getCurrentUserOrSharedScenario', payload)}
    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario', payload)}
    
    function addedOrUpdatedRemainingLifeLimselectListItemsrefitLibraryMutator(payload: any){store.commit('addedOrUpdatedRemainingLifeLimitLibraryMutator', payload);}
    function selectedRemainingLifeLimitLibraryMutator(payload: any){store.commit('selectedRemainingLifeLimitLibraryMutator', payload);}

    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    let remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
    let selectedRemainingLifeLimitLibrary: ShallowRef<RemainingLifeLimitLibrary> = shallowRef(clone(
        emptyRemainingLifeLimitLibrary,
    ));
    let selectedGridRows: ShallowRef<RemainingLifeLimit[]> = shallowRef([]);
    let selectedRemainingLifeIds: string[] = [];
    let selectedScenarioId: any = getBlankGuid();
    let selectListItems= ref<SelectItem[]>([]);
    let hasSelectedLibrary = ref<boolean>(false);
    let gridHeaders: any[] = [
        {
            title: 'Remaining Life Attribute',
            key: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Limit',
            key: 'value',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Criteria',
            key: 'criteria',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
        {
            title: '',
            key: '',
            align: 'left',
            sortable: false,
            class:'',
            width: ''
        },
        {
            title: 'Actions',
            key: 'Actions',
            align: 'right',
            sortable: false,
            class: '',
            width: ''
        }
    ];

    let addedRows: ShallowRef<RemainingLifeLimit[]> = shallowRef([]);
    let updatedRowsMap:Map<string, [RemainingLifeLimit, RemainingLifeLimit]> = new Map<string, [RemainingLifeLimit, RemainingLifeLimit]>();//0: original value | 1: updated value
    let deletionIds: ShallowRef<string[]> = shallowRef([]);
    let rowCache: RemainingLifeLimit[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    const pagination: Pagination = shallowReactive(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = ref(0);
    let currentPage: ShallowRef<RemainingLifeLimit[]> = shallowRef([]);
    let initializing: boolean = true;
    let dateModified: string;

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue = ref<string | null>(null);
    let isShared: boolean = false;

    let shareRemainingLifeLimitLibraryDialogData =ref<ShareRemainingLifeLimitLibraryDialogData>(clone(emptyShareRemainingLifeLimitLibraryDialogData));

    let itemsPerPage:number = 5;
    let dataPerPage: number = 0;
    let totalDataFound: number = 5;
    let remainingLifeLimits: RemainingLifeLimit[] = [];
    let numericAttributeSelectItems = ref<SelectItem[]>([]);
    let createRemainingLifeLimitDialogData = ref<CreateRemainingLifeLimitDialogData>(clone(
        emptyCreateRemainingLifeLimitDialogData,
    ));
    let selectedRemainingLifeLimit: RemainingLifeLimit = clone(
        emptyRemainingLifeLimit,
    );
    let criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
    let createRemainingLifeLimitLibraryDialogData = ref<CreateRemainingLifeLimitLibraryDialogData>(clone(emptyCreateRemainingLifeLimitLibraryDialogData));
    let confirmDeleteAlertData = ref<AlertData>(clone(emptyAlertData));
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario = ref<boolean>(false);
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary = ref<boolean>(false);
    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified = ref<boolean>(false);
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection = ref<boolean>(false);

    onMounted(() => {
            librarySelectItemValue.value = null;
            getRemainingLifeLimitLibrariesAction().then(() => {
                if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
                    selectedScenarioId = $router.currentRoute.value.query.scenarioId;
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
            setAttributesSelectListItems();
            
    });

    onBeforeUnmount(() => {
        setHasUnsavedChangesAction({ value: false });
    });

    watch(selectedGridRows, ()=> {
            selectedRemainingLifeIds = getPropertyValues('id', selectedGridRows.value,) as string[];
        });

    watch(stateRemainingLifeLimitLibraries, ()=>  {
        selectListItems.value = stateRemainingLifeLimitLibraries.value.map(
            (remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id,
            }),
        );
    });

    watch(librarySelectItemValue, ()=> {
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
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection.value = true;

    });
    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        selectRemainingLifeLimitLibraryAction({
            libraryId: librarySelectItemValue.value,
        });
    }

    watch(stateSelectedRemainingLifeLimitLibrary,  ()=> {
        selectedRemainingLifeLimitLibrary.value = clone(
            stateSelectedRemainingLifeLimitLibrary.value,
        );
    });

    watch(selectedRemainingLifeLimitLibrary,  ()=> {
        hasSelectedLibrary.value =  selectedRemainingLifeLimitLibrary.value.id !== uuidNIL;
        clearChanges();
        initializing = false;
        if(hasSelectedLibrary.value)
            onPaginationChanged(); 
    });

    watch(stateScenarioRemainingLifeLimits, ()=> {
        if (hasScenario.value) {
            currentPage.value = clone(stateScenarioRemainingLifeLimits.value);
        }
    });

    watch(currentPage,  ()=>  {
           // Get parent name from library id
        selectListItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    });
    
    watch(isSharedLibrary, ()=> {
        isShared = isSharedLibrary.value;
    });
    
    watch(stateNumericAttributes, ()=> {
        setAttributesSelectListItems();
    });

    watch(pagination, onPaginationChanged )
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<RemainingLifeLimit>= {
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
            await RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                }
            });
        else if(hasSelectedLibrary.value)
            await RemainingLifeLimitService.getRemainingLibraryDate(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '').then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      dateModified = data.slice(0, 10);
                   }
             }),     
             RemainingLifeLimitService.getLibraryRemainingLifeLimitPage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    if (!isNil(selectedRemainingLifeLimitLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedRemainingLifeLimitLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }
                }
            });     
    }

    watch(deletionIds,()=>{
        checkHasUnsavedChanges();
    });

    watch(addedRows, ()=> {
        checkHasUnsavedChanges();
    });


    // onMounted(() => {
    //     setAttributesSelectListItems();
    // });

    function setAttributesSelectListItems() {
        if (hasValue(stateNumericAttributes)) {
            numericAttributeSelectItems.value = stateNumericAttributes.value.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }

    function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedRemainingLifeLimitLibrary.value.owner);
        }
        
        return getUserName();
    }
    function onRemoveRemainingLifeLimitIcon(remainingLifeLimit: RemainingLifeLimit) {
        removeRowLogic(remainingLifeLimit.id);
        onPaginationChanged();
    }

    function onRemoveRemainingLifeLimit() {
        selectedRemainingLifeIds.forEach(_ => {
            removeRowLogic(_);
        });

        selectedRemainingLifeIds = [];
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

    function onShowCreateRemainingLifeLimitLibraryDialog(createAsNewLibrary: boolean) {
        createRemainingLifeLimitLibraryDialogData.value = {
            showDialog: true,
            remainingLifeLimits: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateRemainingLifeLimitLibraryDialogResult(library: RemainingLifeLimitLibrary) {
        createRemainingLifeLimitLibraryDialogData.value = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.remainingLifeLimits.length == 0 || !hasSelectedLibrary.value ? null : selectedRemainingLifeLimitLibrary.value.id,
                    rowsForDeletion: library.remainingLifeLimits.length == 0 ? [] : deletionIds.value,
                    updateRows: library.remainingLifeLimits.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.remainingLifeLimits.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary.value = true;
                    librarySelectItemValue.value = library.id;
                    
                    if(library.remainingLifeLimits.length == 0){
                        clearChanges();
                    }

                    addedOrUpdatedRemainingLifeLimselectListItemsrefitLibraryMutator(library);
                    selectedRemainingLifeLimitLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added remaining life limit library'})
                }               
            })
        }
    }

    function onShowCreateRemainingLifeLimitDialog() {
        createRemainingLifeLimitDialogData.value = {
            showDialog: true,
            numericAttributeSelectItems: numericAttributeSelectItems.value
        };
    }

    function onAddRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
        createRemainingLifeLimitDialogData.value = clone(emptyCreateRemainingLifeLimitDialogData);
        if (!isNil(newRemainingLifeLimit)) {
            addedRows.value.push(newRemainingLifeLimit);
            onPaginationChanged()
        }
    }

    function onEditRemainingLifeLimitProperty(remainingLifeLimit: RemainingLifeLimit, property: string, value: any) {
        onUpdateRow(remainingLifeLimit.id, clone(remainingLifeLimit))
        onPaginationChanged();
    }

    function onShowCriterionLibraryEditorDialog(remainingLifeLimit: RemainingLifeLimit) {
        selectedRemainingLifeLimit = remainingLifeLimit;

        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: remainingLifeLimit.criterionLibrary.mergedCriteriaExpression,           
        };
    }

    function onEditRemainingLifeLimitCriterionLibrary(
        criteriaExpression: string | null,
    ) {
        criterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criteriaExpression) && selectedRemainingLifeLimit.id !== uuidNIL) {
            if(selectedRemainingLifeLimit.criterionLibrary.id === getBlankGuid())
                selectedRemainingLifeLimit.criterionLibrary.id = getNewGuid();

            onUpdateRow(selectedRemainingLifeLimit.id, 
            {
                ...selectedRemainingLifeLimit,
                criterionLibrary: {...selectedRemainingLifeLimit.criterionLibrary, mergedCriteriaExpression: criteriaExpression}
            })
                
            onPaginationChanged();
        }

        selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
    }

    function onUpsertRemainingLifeLimitLibrary() {
        const library: RemainingLifeLimitLibrary = {
            ...clone(selectedRemainingLifeLimitLibrary.value),
            remainingLifeLimits: clone(currentPage.value)
        };
        const upsertRequest: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit> = {
                library: selectedRemainingLifeLimitLibrary.value,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedRemainingLifeLimitLibrary.value.id === uuidNIL ? null : selectedRemainingLifeLimitLibrary.value.id,
                rowsForDeletion: deletionIds.value,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                addedRows: addedRows.value,
                isModified: false
                },
                scenarioId: null
        }
        RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()
                addedOrUpdatedRemainingLifeLimselectListItemsrefitLibraryMutator(selectedRemainingLifeLimitLibrary.value);
                selectedRemainingLifeLimitLibraryMutator(selectedRemainingLifeLimitLibrary.value.id)
                addSuccessNotificationAction({message: "Updated remaining life limit library",});               
            }
        });
    }

    function onUpsertScenarioRemainingLifeLimits() {
        if (selectedRemainingLifeLimitLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection.value ===false) {scenarioLibraryIsModified.value = true;}
        else { scenarioLibraryIsModified.value = false; }

        RemainingLifeLimitService.upsertScenarioRemainingLifeLimits({
            libraryId: selectedRemainingLifeLimitLibrary.value.id === uuidNIL ? null : selectedRemainingLifeLimitLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified.value
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges();
                librarySelectItemValue.value = null;
                resetPage();
                addSuccessNotificationAction({message: "Modified scenario's remaining life limits"});
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
        parentLibraryName = loadedParentName;
        parentLibraryId = loadedParentId;
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
            deleteRemainingLifeLimitLibraryAction({
                libraryId: selectedRemainingLifeLimitLibrary.value.id,
            });
        }
    }

    function disableCrudButton() {
        const rows = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rows.every(
            (remainingLife: RemainingLifeLimit) => {
                return (
                    rules['generalRules'].valueIsNotEmpty(
                        remainingLife.value,
                    ) === true &&
                    rules['generalRules'].valueIsNotEmpty(
                        remainingLife.attribute,
                    ) === true
                );
            },
        );

        if (hasSelectedLibrary.value) {
            return !(
                rules['generalRules'].valueIsNotEmpty(
                    selectedRemainingLifeLimitLibrary.value.name,
                ) === true &&
                dataIsValid);
        }

        return !dataIsValid;
    }

    //paging

    function onUpdateRow(rowId: string, updatedRow: RemainingLifeLimit){
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
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedRemainingLifeLimitLibrary.value, stateSelectedRemainingLifeLimitLibrary.value))
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

    function onShowShareRemainingLifeLimitLibraryDialog(remainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        shareRemainingLifeLimitLibraryDialogData.value = {
            showDialog:true,
            remainingLifeLimitLibrary: clone(remainingLifeLimitLibrary)
        }
    }

    function onShareRemainingLifeLimitDialogSubmit(remainingLifeLimitLibraryUsers: RemainingLifeLimitLibraryUser[]) {
        shareRemainingLifeLimitLibraryDialogData.value = clone(emptyShareRemainingLifeLimitLibraryDialogData);

                if (!isNil(remainingLifeLimitLibraryUsers) && selectedRemainingLifeLimitLibrary.value.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    remainingLifeLimitLibraryUsers.forEach((remainingLifeLimitLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && remainingLifeLimitLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && remainingLifeLimitLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: remainingLifeLimitLibraryUser.userId,
                            userName: remainingLifeLimitLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNil(selectedRemainingLifeLimitLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedRemainingLifeLimitLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }
                    //update budget library sharing
                    RemainingLifeLimitService.upsertOrDeleteRemainingLifeLimitLibraryUsers(selectedRemainingLifeLimitLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                        {
                            resetPage();
                        }
                    });
                }
    }

    function setParentLibraryName(libraryId: string) {
        if (libraryId === "") {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: RemainingLifeLimitLibrary = emptyRemainingLifeLimitLibrary;
        stateRemainingLifeLimitLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }

    function initializePages(){
        const request: PagingRequest<RemainingLifeLimit>= {
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
            RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    setParentLibraryName(currentPage.value.length > 0 ? currentPage.value[0].libraryId : "None");
                    loadedParentId = currentPage.value.length > 0 ? currentPage.value[0].libraryId : "";
                    loadedParentName = parentLibraryName; //store original
                    scenarioLibraryIsModified.value = currentPage.value.length > 0 ? currentPage.value[0].isModified : false;
                }
            });
    }
</script>
<style scoped>
.remaininglife-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}
.vs-style {
    width: 100%;
}
</style>