<template>
    <v-layout column>
      <v-flex xs12>
        <v-layout justify-space-between>
          <v-flex xs3 class="ghd-constant-header">
              <v-layout column>
                  <v-subheader class="ghd-control-label ghd-md-gray">Remaining Life Limit Library</v-subheader>
                  <v-select id="RemainingLifeLimitEditor-lifeLimitLibrary-select"
                            class="ghd-select ghd-text-field ghd-text-field-border vs-style"
                            :items="selectListItems"
                            append-icon=$vuetify.icons.ghd-down
                            v-model="librarySelectItemValue"
                            outline
                            >
                  </v-select>
                  <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
              </v-layout>
          </v-flex>
          <v-flex xs4 class="ghd-constant-header">
                    <v-layout v-if="hasSelectedLibrary && !hasScenario" style="padding-top: 18px; padding-left: 5px" align-center>
                        <div class="header-text-content owner-padding">
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ dateModified }}
                        </div>
                        <v-divider  vertical 
                            v-if="hasSelectedLibrary && !hasScenario">
                        </v-divider>
                        <v-badge v-show="isShared" style="padding: 7px">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowShareRemainingLifeLimitLibraryDialog(selectedRemainingLifeLimitLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                    </v-layout>
                </v-flex>
                <v-flex xs4 class="ghd-constant-header">
                <v-layout justify-end align-end style="padding-top: 18px !important;">
                    <div>
                        <v-btn id="RemainingLifeLimitEditor-addRemainingLifeLimit-btn" class="ghd-white-bg ghd-blue ghd-button" @click="onShowCreateRemainingLifeLimitDialog" v-show="librarySelectItemValue != null || hasScenario" variant = "outlined">Add Remaining Life Limit</v-btn>
                        <v-btn class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style ="ri"  @click="onShowCreateRemainingLifeLimitLibraryDialog(false)" v-show="!hasScenario" variant = "outlined">Create New Library</v-btn>
                    </div>
                </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <div v-show="librarySelectItemValue != null || hasScenario">
            <v-data-table
            id="RemainingLifeLimitEditor-attributes-dataTable"
            :headers="gridHeaders"
            :items="currentPage"  
            :pagination.sync="pagination"
            :must-sort='true'
            :total-items="totalItems"
            :rows-per-page-items=[5,10,25]
            sort-icon=$vuetify.icons.ghd-table-sort
            class="elevation-1 fixed-header v-table__overflow"
            v-model="selectedGridRows"
            >
                <template v-slot:headers="props">
                    <tr>
                        <th
                          v-for="header in props.headers"
                          :key="header.text"
                        >
                            {{header.text}}
                        </th>
                    </tr>
                </template>
                <template v-slot:items="props">
                    <tr :active="props.selected" @click="props.selected = !props.selected">
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.attribute"
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
                                    :value="props.item.attribute"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-select
                                        :items="numericAttributeSelectItems"
                                        append-icon=$vuetify.icons.ghd-down
                                        label="Select an Attribute"
                                        outline
                                        v-model="props.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.value"
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
                                    :value="props.item.value"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-text-field
                                        label="Edit"
                                        single-line
                                        :mask="'##########'"
                                        v-model.number="props.item.value"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td v-if="props.item.criterionLibrary.mergedCriteriaExpression != '' && props.item.criterionLibrary.mergedCriteriaExpression != null" >
                            {{ props.item.criterionLibrary.mergedCriteriaExpression}}
                        </td>
                        <td v-else>-
                        </td>
                        <td class="px-0">
                            <v-btn @click="onShowCriterionLibraryEditorDialog(props.item)" icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>   
                        </td>
                        <td justify-end>
                            <v-btn @click="onRemoveRemainingLifeLimitIcon(props.item)" icon>
                                <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                            </v-btn>                          
                        </td>
                    </tr>
                </template>
                </v-data-table>
                <v-layout justify-start align-center class="pa-2">
                </v-layout>
                <v-divider></v-divider>
                <v-flex v-show="!hasScenario" xs12 class="px-0">
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea
                        class="ghd-control-text ghd-control-border"
                        v-model="selectedRemainingLifeLimitLibrary.description"
                        @update:model-value="checkHasUnsavedChanges()"
                        outline
                    >
                    </v-textarea>
                </v-flex>
                <v-layout justify-center row>
                    <v-btn id="RemainingLifeLimitEditor-cancel-btn" class="ghd-blue" variant = "outlined" v-show="hasScenario" @click="onDiscardChanges" :disabled="!hasUnsavedChanges">Cancel</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-deleteLibrary-btn" class="ghd-blue" variant = "outlined" v-show="!hasScenario" @click="onShowConfirmDeleteAlert">Delete Library</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-createAsNewLibrary-btn" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' @click="onShowCreateRemainingLifeLimitLibraryDialog(true)" variant = "outlined">Create as New Library</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-save-btn" class="ghd-blue-bg ghd-white ghd-button" v-show="hasScenario" @click="onUpsertScenarioRemainingLifeLimits" :disabled="disableCrudButton() || !hasUnsavedChanges">Save</v-btn>
                    <v-btn id="RemainingLifeLimitEditor-updateLibrary-btn" class="ghd-blue-bg ghd-white ghd-button" v-show="!hasScenario" :disabled="disableCrudButton() || !hasUnsavedChanges" @click="onUpsertRemainingLifeLimitLibrary">Update Library</v-btn>
                </v-layout>
        </div>

        <ConfirmDeleteAlert 
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
    </v-layout>
</template>

<script setup lang="ts">
import Vue, { Ref, ref, shallowReactive, shallowRef, ShallowRef, watch } from 'vue';
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
import { isNullOrUndefined } from 'util';
import { LibraryUser } from '@/shared/models/iAM/user';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    const $router = useRouter();

    const stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = shallowReactive(store.state.remainingLifeLimitModule.remainingLifeLimitLibraries);
    const stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = shallowReactive(store.state.remainingLifeLimitModule.selectedRemainingLifeLimitLibrary);
    let stateNumericAttributes: Attribute[] = shallowReactive(store.state.attributeModule.numericAttributes);
    let stateScenarioRemainingLifeLimits: RemainingLifeLimit[] = shallowReactive(store.state.remainingLifeLimitModule.scenarioRemainingLifeLimits);
    let hasUnsavedChanges: boolean = (store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let hasAdminAccess: boolean = (store.state.authenticationModule.hasAdminAccess) ;
    let isSharedLibrary: Ref<boolean> = ref(store.state.remainingLifeLimitModule.isSharedLibrary);

    async function getIsSharedLibraryAction(payload?: any): Promise<any>{await store.dispatch('getIsSharedRemainingLifeLimitLibrary')}
    async function getRemainingLifeLimitLibrariesAction(payload?: any): Promise<any>{await store.dispatch('getRemainingLifeLimitLibraries')}
    async function upsertRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('upsertRemainingLifeLimitLibrary')}
    async function deleteRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('deleteRemainingLifeLimitLibrary')}
    async function selectRemainingLifeLimitLibraryAction(payload?: any): Promise<any>{await store.dispatch('selectRemainingLifeLimitLibrary')}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification')}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges')}
    async function getScenarioRemainingLifeLimitsAction(payload?: any): Promise<any>{await store.dispatch('getScenarioRemainingLifeLimits')}
    async function upsertScenarioRemainingLifeLimitsAction(payload?: any): Promise<any>{await store.dispatch('upsertScenarioRemainingLifeLimits')}
    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification')}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch('getCurrentUserOrSharedScenario')}
    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario')}
    
    function addedOrUpdatedRemainingLifeLimitLibraryMutator(payload: any){store.commit('addedOrUpdatedRemainingLifeLimitLibraryMutator');}
    function selectedRemainingLifeLimitLibraryMutator(payload: any){store.commit('selectedRemainingLifeLimitLibraryMutator');}

    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    let remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
    let selectedRemainingLifeLimitLibrary: ShallowRef<RemainingLifeLimitLibrary> = shallowRef(clone(
        emptyRemainingLifeLimitLibrary,
    ));
    let selectedGridRows: ShallowRef<RemainingLifeLimit[]> = shallowRef([]);
    let selectedRemainingLifeIds: string[] = [];
    let selectedScenarioId: string = getBlankGuid();
    let selectListItems: SelectItem[] = [];
    let hasSelectedLibrary: boolean = false;
    let gridHeaders: DataTableHeader[] = [
        {
            text: 'Remaining Life Attribute',
            value: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Limit',
            value: 'value',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Criteria',
            value: 'criteria',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class:'',
            width: ''
        },
        {
            text: 'Actions',
            value: 'Actions',
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
    let totalItems = 0;
    let currentPage: ShallowRef<RemainingLifeLimit[]> = shallowRef([]);
    let initializing: boolean = true;
    let dateModified: string;

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: Ref<string | null> = ref(null);
    let isShared: boolean = false;

    let shareRemainingLifeLimitLibraryDialogData: ShareRemainingLifeLimitLibraryDialogData = clone(emptyShareRemainingLifeLimitLibraryDialogData);

    let itemsPerPage:number = 5;
    let dataPerPage: number = 0;
    let totalDataFound: number = 5;
    let remainingLifeLimits: RemainingLifeLimit[] = [];
    let numericAttributeSelectItems: SelectItem[] = [];
    let createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(
        emptyCreateRemainingLifeLimitDialogData,
    );
    let selectedRemainingLifeLimit: RemainingLifeLimit = clone(
        emptyRemainingLifeLimit,
    );
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
        emptyCreateRemainingLifeLimitLibraryDialogData,
    );
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let hasScenario: boolean = false;
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;

    function beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getRemainingLifeLimitLibrariesAction().then(() => {
                if (to.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
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

    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(selectedGridRows, onSelectedGridRowsChanged )
    function onSelectedGridRowsChanged() {
            selectedRemainingLifeIds = getPropertyValues('id', selectedGridRows.value,) as string[];
        }

    watch(stateRemainingLifeLimitLibraries, onStateRemainingLifeLimitLibrariesChanged )
    function onStateRemainingLifeLimitLibrariesChanged() {
        selectListItems = stateRemainingLifeLimitLibraries.map(
            (remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id,
            }),
        );
    }

    watch(librarySelectItemValue, onLibrarySelectItemValueChangedCheckUnsaved )
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario){
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
        newLibrarySelection = true;

    }
    function onLibrarySelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        selectRemainingLifeLimitLibraryAction({
            libraryId: librarySelectItemValue.value,
        });
    }

    watch(stateSelectedRemainingLifeLimitLibrary,  onStateSelectedRemainingLifeLimitLibraryChanged)
    function onStateSelectedRemainingLifeLimitLibraryChanged() {
        selectedRemainingLifeLimitLibrary.value = clone(
            stateSelectedRemainingLifeLimitLibrary,
        );
    }

    watch(selectedRemainingLifeLimitLibrary,  onSelectedRemainingLifeLimitLibraryChanged)
    function onSelectedRemainingLifeLimitLibraryChanged() {
        hasSelectedLibrary =  selectedRemainingLifeLimitLibrary.value.id !== uuidNIL;
        clearChanges();
        initializing = false;
        if(hasSelectedLibrary)
            onPaginationChanged();
    }

    watch(stateScenarioRemainingLifeLimits, onStateScenarioRemainingLifeLimitsChanged )
    function onStateScenarioRemainingLifeLimitsChanged() {
        if (hasScenario) {
            currentPage.value = clone(stateScenarioRemainingLifeLimits);
        }
    }

    watch(currentPage,  onGridDataChanged)
    function onGridDataChanged() {
           // Get parent name from library id
        selectListItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }
    
    watch(isSharedLibrary, onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }
    
    watch(stateNumericAttributes, onStateNumericAttributesChanged )
    function onStateNumericAttributesChanged() {
        setAttributesSelectListItems();
    }

    watch(pagination, onPaginationChanged )
    async function onPaginationChanged() {
        if(initializing)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = pagination;
        const request: PagingRequest<RemainingLifeLimit>= {
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
            await RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                }
            });
        else if(hasSelectedLibrary)
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
                    totalItems = data.totalItems;
                    if (!isNullOrUndefined(selectedRemainingLifeLimitLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedRemainingLifeLimitLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }
                }
            });     
    }

    watch(deletionIds,onDeletionIdsChanged  )
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows, onAddedRowsChanged )
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }

    function mounted() {
        setAttributesSelectListItems();
    }

    function setAttributesSelectListItems() {
        if (hasValue(stateNumericAttributes)) {
            numericAttributeSelectItems = stateNumericAttributes.map(
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
        createRemainingLifeLimitLibraryDialogData = {
            showDialog: true,
            remainingLifeLimits: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreateRemainingLifeLimitLibraryDialogResult(library: RemainingLifeLimitLibrary) {
        createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<RemainingLifeLimitLibrary, RemainingLifeLimit> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.remainingLifeLimits.length == 0 || !hasSelectedLibrary ? null : selectedRemainingLifeLimitLibrary.value.id,
                    rowsForDeletion: library.remainingLifeLimits.length == 0 ? [] : deletionIds.value,
                    updateRows: library.remainingLifeLimits.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.remainingLifeLimits.length == 0 ? [] : addedRows.value,
                    isModified: false
                 },
                 scenarioId: hasScenario ? selectedScenarioId : null
            }
            RemainingLifeLimitService.upsertRemainingLifeLimitLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = library.id;
                    
                    if(library.remainingLifeLimits.length == 0){
                        clearChanges();
                    }

                    addedOrUpdatedRemainingLifeLimitLibraryMutator(library);
                    selectedRemainingLifeLimitLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added remaining life limit library'})
                }               
            })
        }
    }

    function onShowCreateRemainingLifeLimitDialog() {
        createRemainingLifeLimitDialogData = {
            showDialog: true,
            numericAttributeSelectItems: numericAttributeSelectItems,
        };
    }

    function onAddRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
        createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);
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

        criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: remainingLifeLimit.criterionLibrary.mergedCriteriaExpression,           
        };
    }

    function onEditRemainingLifeLimitCriterionLibrary(
        criteriaExpression: string | null,
    ) {
        criterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

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
                addedOrUpdatedRemainingLifeLimitLibraryMutator(selectedRemainingLifeLimitLibrary.value);
                selectedRemainingLifeLimitLibraryMutator(selectedRemainingLifeLimitLibrary.value.id)
                addSuccessNotificationAction({message: "Updated remaining life limit library",});               
            }
        });
    }

    function onUpsertScenarioRemainingLifeLimits() {
        if (selectedRemainingLifeLimitLibrary.value.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        RemainingLifeLimitService.upsertScenarioRemainingLifeLimits({
            libraryId: selectedRemainingLifeLimitLibrary.value.id === uuidNIL ? null : selectedRemainingLifeLimitLibrary.value.id,
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
                addSuccessNotificationAction({message: "Modified scenario's remaining life limits"});
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

        if (hasSelectedLibrary) {
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
            (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', stateSelectedRemainingLifeLimitLibrary, selectedRemainingLifeLimitLibrary))
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

    function onShowShareRemainingLifeLimitLibraryDialog(remainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
        shareRemainingLifeLimitLibraryDialogData = {
            showDialog:true,
            remainingLifeLimitLibrary: clone(remainingLifeLimitLibrary)
        }
    }

    function onShareRemainingLifeLimitDialogSubmit(remainingLifeLimitLibraryUsers: RemainingLifeLimitLibraryUser[]) {
        shareRemainingLifeLimitLibraryDialogData = clone(emptyShareRemainingLifeLimitLibraryDialogData);

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
                    if (!isNullOrUndefined(selectedRemainingLifeLimitLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedRemainingLifeLimitLibrary).then(() => isShared = isSharedLibrary.value);
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
        stateRemainingLifeLimitLibraries.forEach(library => {
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
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            RemainingLifeLimitService.getScenarioRemainingLifeLimitPage(selectedScenarioId, request).then(response => {
                initializing = false
                if(response.data){
                    let data = response.data as PagingPage<RemainingLifeLimit>;
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
<style scoped>
.vs-style {
    width: 100%;
}
</style>