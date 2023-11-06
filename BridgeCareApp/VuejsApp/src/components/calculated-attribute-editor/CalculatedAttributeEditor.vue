<template>
    <v-row column>
        <!-- top row -->
        <v-col cols = "12">
            <v-row justify-space-between>
                <v-col cols = "5" class="ghd-constant-header" style="margin-right: 10px">
                    <v-row column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Calculated Attribute</v-subheader>
                        <v-select
                                  id="CalculatedAttribute-CalculatedAttribute-select"
                                  :items="librarySelectItems"
                                  append-icon=ghd-down
                                  variant="outlined"
                                  v-model="librarySelectItemValue"
                                  item-title="text" 
                                    item-value="value" 
                                  class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>
                        <div class="ghd-md-gray ghd-control-subheader" v-if="hasScenario"><b>Library Used: {{parentLibraryName}} <span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
                    </v-row>
                </v-col>
                <v-col cols = "7" class="ghd-constant-header" style="margin-right: 10px">
                    <v-row align-end>
                        <v-text-field
                                    id="CalculatedAttribute-search-textField"
                                    prepend-inner-icon=ghd-search
                                    hide-details
                                    lablel="Search"
                                    placeholder="Search Calcultated Attribute"
                                    single-line
                                    v-model="gridSearchTerm"
                                    outline
                                    clearable
                                    @click:clear="onClearClick()"
                                    class="ghd-text-field-border ghd-text-field search-icon-general"
                                    style="margin-top:20px !important">
                        </v-text-field>
                        <v-btn id="CalculatedAttribute-search-btn" style="position: relative; top: 3px; margin-right: 1px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" @click="onSearchClick()">Search</v-btn>
                        <v-btn id="CalculatedAttribute-createNewLibrary-btn"
                            @click="onShowCreateCalculatedAttributeLibraryDialog(false)"
                            class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                            variant = "outlined"
                            v-show="!hasScenario"
                            style="top: 2px !important; position: relative">
                            Create New Library
                        </v-btn>
                    </v-row>
                </v-col>
            </v-row>
        </v-col>
        <v-col cols = "6" class="ghd-constant-header" style="margin-bottom: 15px">
            <v-row v-if='hasSelectedLibrary && !hasScenario' align-center>
                <div class="header-text-content owner-padding">
                     Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ modifiedDate }}
                </div>
                <v-divider class="owner-shared-divider" inset vertical></v-divider>
                <v-badge v-show="isShared" style="padding: 10px">
                    <template v-slot:badge>
                        <span>Shared</span>
                    </template>
                    </v-badge>
                    <v-btn id="CalculatedAttribute-shareLibrary-btn" @click='onShowShareCalculatedAttributeLibraryDialog(selectedCalculatedAttributeLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                v-show='!hasScenario'>
                Share Library
            </v-btn>
        </v-row>
        </v-col>
        <!-- attributes and timing -->
        <v-col cols = "12" v-show="hasSelectedLibrary || hasScenario">
            <v-row justify-space-between>
                <v-col cols = "6">
                <v-row column style="float:left; width: 100%">
                    <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                    <v-select
                        id="CalculatedAttribute-Attribute-select"   
                        :items="attributeSelectItems"
                        append-icon=ghd-down
                        variant="outlined"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        item-title="text" 
                        item-value="value" 
                        v-model="attributeSelectItemValue">
                    </v-select>
                </v-row>
                <v-col cols = "2">
                </v-col>
                </v-col>
                <v-col cols = "6">
                <v-row column style="float:right; width: 100%">
                    <v-subheader class="ghd-md-gray ghd-control-label">Timing</v-subheader>
                    <v-select
                        id="CalculatedAttribute-Timing-select"
                        :items="attributeTimingSelectItems"
                        append-icon=ghd-down
                        variant="outlined"
                        v-model="attributeTimingSelectItemValue"
                        :disabled="!hasAdminAccess"
                        item-title="text" 
                        item-value="value" 
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        v-on:change="setTiming">
                    </v-select>
                </v-row>
                </v-col>
            </v-row>
        </v-col>
        <!-- Default Equation -->
        <v-col cols = "12" v-show="hasSelectedLibrary || hasScenario">
            <v-row justify-center>
                <v-col cols = "6">
                    <v-row column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Default Equation</v-subheader>
                        <v-text-field
                            id="CalculatedAttribute-defaultEquation-textfield"
                            readonly
                            class="sm-txt"
                            v-model="defaultEquation.equation.expression"
                            :disabled="!hasAdminAccess">
                            <template v-slot:append>
                                <v-btn
                                    id="CalculatedAttribute-defaultEquationEditor-btn"
                                    @click="onShowEquationEditorDialogForDefaultEquation()"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="getUrl('assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </v-row>
                </v-col>
            </v-row>
        </v-col>
        <!-- data table -->
        <v-col cols="12" v-show="hasSelectedLibrary || hasScenario">
            <v-data-table-server
                id="CalculatedAttribute-equation-table"    
                :headers="calculatedAttributeGridHeaders"
                :items="selectedGridItem"
                :pagination.sync="pagination"
                class="v-table__overflow ghd-table"
                sort-icon=ghd-table-sort
                item-key="calculatedAttributeLibraryEquationId"                            

                     
                :items-length="totalItems"
                :items-per-page-options="[
                    {value: 5, title: '5'},
                    {value: 10, title: '10'},
                    {value: 25, title: '25'},
                ]"
                v-model:sort-by="pagination.sort"
                v-model:page="pagination.page"
                v-model:items-per-page="pagination.rowsPerPage"
                @update:options="onPaginationChanged"
                >
                <template slot="items" slot-scope="props" v-slot:item="props">
                    <tr>
                    <td class="text-xs-center">
                        <v-text-field
                            id="CalculatedAttribute-equation-textfield"
                            readonly
                            class="sm-txt"
                            :model-value="props.item.equation"
                            :disabled="!hasAdminAccess">
                            <template v-slot:append>
                                <v-btn
                                    id="CalculatedAttribute-editEquation-btn"
                                    @click="onShowEquationEditorDialog(props.item.id)"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="getUrl('assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-text-field id="CalculatedAttribute-EquationCriteria-textfield"
                            readonly
                            class="sm-txt"
                            :model-value="props.item.criteriaExpression"
                            :disabled="!hasAdminAccess">
                            <template v-slot:append>
                                <v-btn
                                    id="CalculatedAttribute-changeEquationCriteria-btn"
                                    @click="onEditCalculatedAttributeCriterionLibrary(props.item.id)"
                                    class="ghd-blue"
                                    icon
                                    v-if="hasAdminAccess">
                                    <img class='img-general img-shift' :src="getUrl('assets/icons/edit.svg')"/>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </td>
                    <td class="text-xs-center">
                        <v-btn
                            id="CalculatedAttribute-RemoveEquation-btn"
                            @click="
                                onRemoveCalculatedAttribute(props.item.id)"
                            class="ghd-blue"
                            icon
                            :disabled="!hasAdminAccess">
                            <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                    </tr>
                </template>
            </v-data-table-server>
            <v-btn
                id="CalculatedAttribute-AddNewEquation-btn"
                @click="onAddCriterionEquationSet()"
                class='ghd-blue ghd-button'
                variant = "outlined"
                v-if="hasAdminAccess"
                :disabled="
                    attributeSelectItemValue == null ||
                    attributeSelectItemValue == ''">
                Add New Equation
            </v-btn>
        </v-col>
        <!-- description -->
        <v-col v-show='hasSelectedLibrary && !hasScenario' cols="12">
            <v-subheader class="ghd-subheader ">Description</v-subheader>
            <v-textarea
                no-resize
                outline
                class="ghd-text-field-border"
                rows="4"
                v-model="selectedCalculatedAttributeLibrary.description"
                @update:model-value="checkHasUnsavedChanges()"/>
        </v-col>
        <!-- buttons -->
        <v-col cols = "12" v-show="hasSelectedLibrary || hasScenario">
            <v-row justify-center v-show='hasSelectedLibrary || hasScenario'>
                <v-btn
                    :disabled="!hasUnsavedChanges"
                    v-if="hasAdminAccess && hasScenario"
                    @click="onDiscardChanges"
                    class='ghd-blue ghd-button-text ghd-button'
                    variant = "flat"
                    v-show="hasSelectedLibrary || hasScenario">
                    Cancel
                </v-btn>
                
                <v-btn id="CalculatedAttribute-deleteLibrary-btn"
                    @click="onShowConfirmDeleteAlert"
                    class='ghd-blue ghd-button-text ghd-button'
                    variant = "flat"
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary">
                    Delete Library
                </v-btn>
                <v-btn id="CalculatedAttribute-createAsNewLibrary-btn"
                    :disabled="disableCrudButton()"
                    v-if="hasAdminAccess"
                    @click="onShowCreateCalculatedAttributeLibraryDialog(true)"
                    variant = "outlined"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
                    Create as New Library
                </v-btn>
                <v-btn id="CalculatedAttribute-updateLibrary-btn"
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertCalculatedAttributeLibrary"
                    class='ghd-blue-bg text-white ghd-button-text ghd-outline-button-padding ghd-button'
                    v-show="!hasScenario">
                    Update Library
                </v-btn>
                <v-btn
                    @click="onUpsertScenarioCalculatedAttribute"
                    class='ghd-blue-bg text-white ghd-button-text ghd-button'
                    v-show="hasScenario && hasAdminAccess"
                    :disabled="disableCrudButton() || !hasUnsavedChanges">
                    Save
                </v-btn> 
            </v-row>
        </v-col>
     
        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />
        <CreateCalculatedAttributeLibraryDialog
            :dialogData="createCalculatedAttributeLibraryDialogData"
            @submit="onSubmitCreateCalculatedAttributeLibraryDialogResult"
        />
        <ShareCalculatedAttributeLibraryDialog :dialogData="shareCalculatedAttributeLibraryDialogData"
            @submit="onShareCalculatedAttributeDialogSubmit" 
        />
        <CreateCalculatedAttributeDialog
            :showDialog="showCreateCalculatedAttributeDialog"
            @submit="onSubmitCreateCalculatedAttributeDialogResult"
        />
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            @submit="onSubmitEquationEditorDialogResult"
        />
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onSubmitCriterionEditorDialogResult"
        />
        <ConfirmDialog></ConfirmDialog>
    </v-row>
</template>
<script lang="ts" setup>
import Vue, { shallowReactive, shallowRef } from 'vue';
import Alert from '@/shared/modals/Alert.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CreateCalculatedAttributeLibraryDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeLibraryDialog.vue';
import CreateCalculatedAttributeDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeDialog.vue';
import ShareCalculatedAttributeLibraryDialog from '@/components/calculated-attribute-editor/calculated-attribute-editor-dialogs/ShareCalculatedAttributeLibraryDialog.vue';
import { emptyShareCalculatedAttributeLibraryDialogData, ShareCalculatedAttributeLibraryDialogData } from '@/shared/models/modals/share-calculated-attribute-data';
import {
    InputValidationRules,
    rules as validationRules
} from '@/shared/utils/input-validation-rules';
import {
  any,
    clone,
    find,
    findIndex,
    isNil,
    propEq,
    update,
} from 'ramda';
import {
    CalculatedAttribute,
    CalculatedAttributeGridModel,
    CalculatedAttributeLibrary,
    CalculatedAttributeLibraryUser,
    CriterionAndEquationSet,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
    emptyCriterionAndEquationSet,
    Timing,
} from '@/shared/models/iAM/calculated-attribute';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import {
    CreateCalculatedAttributeLibraryDialogData,
    emptyCreateCalculatedAttributeLibraryDialogData,
} from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Equation } from '@/shared/models/iAM/equation';
import {
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { CalculatedAttributeLibraryUpsertPagingRequestModel, CalculatedAttributePagingRequestModel, CalculatedAttributePagingSyncModel, calculcatedAttributePagingPageModel} from '@/shared/models/iAM/paging';
import { mapToIndexSignature } from '@/shared/utils/conversion-utils';

import CalculatedAttributeService from '@/services/calculated-attribute.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { LibraryUser } from '@/shared/models/iAM/user';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import mitt from 'mitt';
import { computed } from 'vue';
import { getUrl } from '@/shared/utils/get-url';

let store = useStore();
const confirm = useConfirm();
let stateCalculatedAttributeLibraries = computed<CalculatedAttributeLibrary[]>(() => store.state.calculatedAttributeModule.calculatedAttributeLibraries);
let stateSelectedCalculatedAttributeLibrary = computed<CalculatedAttributeLibrary>(() => store.state.calculatedAttributeModule.selectedCalculatedAttributeLibrary);
let stateScenarioCalculatedAttributes = computed<CalculatedAttribute[]>(() => store.state.calculatedAttributeModule.scenarioCalculatedAttributes);
let stateSelectedLibraryCalculatedAttributes = computed<CalculatedAttribute[]>(() => store.state.calculatedAttributeModule.selectedLibraryCalculatedAttributes);
let stateCalculatedAttributes = computed<Attribute[]>(() => store.state.calculatedAttributeModule.calculatedAttributes);
let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
let isSharedLibrary = computed<boolean>(() => store.state.calculatedAttributeModule.isSharedLibrary);

    async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedCalculatedAttributeLibrary', payload);}
    async function upsertScenarioCalculatedAttributeAction(payload?: any): Promise<any> {await store.dispatch('upsertScenarioCalculatedAttribute', payload);}
    async function deleteCalculatedAttributeLibraryAction(payload?: any): Promise<any> {await store.dispatch('deleteCalculatedAttributeLibrary', payload);}
    async function getCalculatedAttributeLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCalculatedAttributeLibraries', payload);}
    async function getScenarioCalculatedAttributeAction(payload?: any): Promise<any> {await store.dispatch('getScenarioCalculatedAttribute', payload);}
    async function getSelectedLibraryCalculatedAttributesAction(payload?: any): Promise<any> {await store.dispatch('getSelectedLibraryCalculatedAttributes', payload);}
    async function selectCalculatedAttributeLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectCalculatedAttributeLibrary', payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges', payload);}
    async function getCalculatedAttributesAction(payload?: any): Promise<any> {await store.dispatch('getCalculatedAttributes', payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}
    async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification', payload);}
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario', payload);}
    async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario', payload);}
    function selectedCalculatedAttributeLibraryMutator(payload: any){store.commit('selectedCalculatedAttributeLibraryMutator', payload);}
    function calculatedAttributeLibraryMutator(payload: any){store.commit('calculatedAttributeLibraryMutator', payload);}
    let getUserNameByIdGetter: any = store.getters.getUserNameById;

    let updatedCalcAttrMap:Map<string, [CalculatedAttribute, CalculatedAttribute]> = 
        new Map<string, [CalculatedAttribute, CalculatedAttribute]>();//0: original value | 1: updated value
    let addedCalcAttr: CalculatedAttribute[] = [];
    let CalcAttrCache: CalculatedAttribute = emptyCalculatedAttribute;
    let pairsCache: CriterionAndEquationSet[] = [];
    let updatedPairsMaps:Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]> = 
        new Map<string, [CriterionAndEquationSet, CriterionAndEquationSet]>();//0: original value | 1: updated value 
    let addedCalcPairs: Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    let deletionPairsIds: Map<string, string[]> = new Map<string, string[]>();
    let updatedPairs:  Map<string, CriterionAndEquationSet[]> = new  Map<string, CriterionAndEquationSet[]>();
    let defaultEquations: Map<string, CriterionAndEquationSet> = new Map<string, CriterionAndEquationSet>()
    let gridSearchTerm = '';
    let currentSearch = '';
    const pagination: Pagination = shallowReactive(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: CalculatedAttribute = clone(emptyCalculatedAttribute);
    let initializing: boolean = true;
    let uuidNIL: string = getBlankGuid();
    let isShared: boolean = false;
    let modifiedDate: string;

    let shareCalculatedAttributeLibraryDialogData: ShareCalculatedAttributeLibraryDialogData = clone(emptyShareCalculatedAttributeLibraryDialogData);

    let defaultEquation: CriterionAndEquationSet = emptyCriterionAndEquationSet;
    let defaultEquationCache: CriterionAndEquationSet = emptyCriterionAndEquationSet;
    let defaultSelected: boolean = false;

    let hasSelectedLibrary = ref(false);
    let isDefaultBool = shallowRef<boolean>(false); //this exists because isDefault can't be tracked so this bool is tracked for the switch and is then synced with isDefault
    let hasScenario = ref(false);
    let rules: InputValidationRules = validationRules;
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
   let showCreateCalculatedAttributeDialog = false;
    let hasSelectedCalculatedAttribute: boolean = false;
    let selectedCalculatedAttribute: CalculatedAttribute = clone(
        emptyCalculatedAttribute,
    );
    let selectedScenarioId: string = getBlankGuid();
    let librarySelectItems = ref<SelectItem[]>([]);

    let truelibrarySelectItemValue = ref<string>('');
    let librarySelectItemValue = ref<string>('');
    
    let librarySelectItemValueAllowedChanged: boolean = true;

    let unsavedDialogAllowed: boolean = true;

    let attributeSelectItems: SelectItem[] = [];

    let attributeSelectItemValue = ref<string>('');
    let trueAttributeSelectItemValue = ref<string>('');
    let attributeSelectItemValueAllowedChanged: boolean = true;

    let isAttributeSelectedItemValue: boolean = false;
    let isTimingSelectedItemValue: boolean = false;
    let attributeTimingSelectItems: SelectItem[] = [];
    let attributeTimingSelectItemValue= ref('');
    let currentCriteriaEquationSetSelectedId: string | null = '';

    let selectedCalculatedAttributeLibrary = ref(clone(
        emptyCalculatedAttributeLibrary,
    ));
    let createCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = clone(
        emptyCreateCalculatedAttributeLibraryDialogData,
    );
    let equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let calculatedAttributeGridData = shallowRef<CalculatedAttribute[]>([]);
    let activeCalculatedAttributeId: string = getBlankGuid();
    let selectedGridItem  = shallowRef<CalculatedAttributeGridModel[]>([]);
    let selectedAttribute: CalculatedAttribute = clone(emptyCalculatedAttribute)
    let hasCreatedLibrary: boolean = false;

    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;

    let calculatedAttributeGridHeaders: any[] = [
        {
            title: 'Equation',
            key: 'equation',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Criterion',
            key: 'criteriaExpression',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: '',
            key: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];
    
    const $router = useRouter();
    const $emitter = mitt();
    
    beforeRouteEnter();
    function beforeRouteEnter(){
        librarySelectItemValue.value = '';
            attributeSelectItemValue.value = '';

            getCalculatedAttributesAction().then( () => {
                getCalculatedAttributeLibrariesAction().then(() => {
                    setAttributeSelectItems()
                    setAttributeTimingSelectItems();
                    if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.CalculatedAttribute) !== -1) {
                        
                            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;
                        if (selectedScenarioId === uuidNIL) {
                            addErrorNotificationAction({
                                message: 'Unable to identify selected scenario.',
                            });
                            $router.push('/Scenarios/');
                        }

                        hasScenario.value = true;
                        getScenarioCalculatedAttributeAction(selectedScenarioId).then(()=> {
                            getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                                selectScenarioAction({ scenarioId: selectedScenarioId });        
                            });
                        });
                    }
                });                
            });
    }
    onBeforeUnmount(()=>beforeDestroy())
    function beforeDestroy() {
        calculatedAttributeGridData.value = [] as CalculatedAttribute[];
        selectedAttribute = clone(emptyCalculatedAttribute);
    }

    // Watchers
    async function onPaginationChanged() {
        if( initializing)
            return;
         checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } =  pagination;
        const request: CalculatedAttributePagingRequestModel= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId:  selectedCalculatedAttributeLibrary.value.id ===  uuidNIL ? null :  selectedCalculatedAttributeLibrary.value.id,
                updatedCalculatedAttributes: Array.from( updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature( deletionPairsIds),
                updatedPairs: mapToIndexSignature(  updatedPairs),
                addedPairs: mapToIndexSignature( addedCalcPairs),
                addedCalculatedAttributes:  addedCalcAttr,
                defaultEquations: mapToIndexSignature( defaultEquations),
                isModified:  scenarioLibraryIsModified
            },           
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search:  currentSearch,
            attributeId:  stateCalculatedAttributes.value.find(_ => _.name ===  selectedAttribute.attribute)!.id
        };
        if((! hasSelectedLibrary.value &&  hasScenario.value) &&  selectedScenarioId !==  uuidNIL){
            CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage( selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                     currentPage.equations = data.items;
                     currentPage.calculationTiming = data.calculationTiming
                     pairsCache =  currentPage.equations;
                     totalItems = data.totalItems;
                     defaultEquation = data.defaultEquation;
                     selectedGridItem.value =  calculatedAttributeGridModelConverter( currentPage)
                }
            });
        }            
        else if(hasSelectedLibrary.value){
            initializing = true;
            await CalculatedAttributeService.getCalculatedLibraryModifiedDate(selectedCalculatedAttributeLibrary.value.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate = data.slice(0, 10);
                   }
                   initializing = false;
             });

             await CalculatedAttributeService.getLibraryCalculatedAttributePage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                     currentPage.equations = data.items;
                     currentPage.calculationTiming = data.calculationTiming
                     pairsCache =  currentPage.equations;
                     totalItems = data.totalItems;
                     defaultEquation = data.defaultEquation;
                     selectedGridItem.value =  calculatedAttributeGridModelConverter( currentPage);
                    if (!isNil( selectedCalculatedAttributeLibrary.value.id) ) {
                         getIsSharedLibraryAction( selectedCalculatedAttributeLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }
                    initializing = false;
                }
            });     
        }
        
        
    }

    watch(deletionPairsIds,()=> onDeletionPairsIdsChanged())
    function onDeletionPairsIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedCalcPairs,()=> onAddedPairsChanged())
    function onAddedPairsChanged(){
        checkHasUnsavedChanges();
    }

    function onSelectedAttributeChanged(){
        selectedGridItem.value = calculatedAttributeGridModelConverter(currentPage)
    }

    watch(isDefaultBool,() => onIsDefaultBoolChanged())
    function onIsDefaultBoolChanged(){
        selectedCalculatedAttributeLibrary.value.isDefault = isDefaultBool.value;
        checkHasUnsavedChanges()
    }
    watch(stateCalculatedAttributes,()=>onStateCalculatedAttributesChanged())
    function onStateCalculatedAttributesChanged() {
        setAttributeSelectItems();
    }
    watch(currentPage,() => onCurrentPageChanged())
    function onCurrentPageChanged() {
        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
        });
    }
    function setAttributeSelectItems() {
        if (hasValue(stateCalculatedAttributes.value)) {
            attributeSelectItems = stateCalculatedAttributes.value.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );

            attributeSelectItems.forEach(_ => {
                var tempItem: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: _.text,
                    name: _.text,
                    libraryId: getNewGuid(),
                    isModified: false,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                if (calculatedAttributeGridData.value == undefined) {
                    calculatedAttributeGridData.value as CalculatedAttribute[];
                }
                calculatedAttributeGridData.value.push(tempItem);
            });
        }
    }
    function setAttributeTimingSelectItems() {
        attributeTimingSelectItems = [
            { text: 'Pre Deterioration', value: Timing.PreDeterioration },
            { text: 'Post Deterioration', value: Timing.PostDeterioration },
            { text: 'On Demand', value: Timing.OnDemand },
        ];
    }
    watch(stateCalculatedAttributeLibraries, onStateCalculatedAttributeLibrariesChanged)
    function onStateCalculatedAttributeLibrariesChanged() {
        librarySelectItems.value = stateCalculatedAttributeLibraries.value.map(
            (library: CalculatedAttributeLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }
    // is so that a user is asked wether or not to continue when switching libraries after they have made changes
    //but only when in libraries
    watch(librarySelectItemValue,() => onLibrarySelectItemValueChangedCheckUnsaved())
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        defaultEquation.equation.expression = "";
        if(hasScenario.value){
            onLibrarySelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged) {
            CheckUnsavedDialog(onLibrarySelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = truelibrarySelectItemValue.value;               
            });
        }
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        setParentLibraryName(parentLibraryId);
        newLibrarySelection = true;
        librarySelectItemValueAllowedChanged = true;
    }
    function onLibrarySelectItemValueChanged() {
        truelibrarySelectItemValue.value = librarySelectItemValue.value;
        selectCalculatedAttributeLibraryAction(
            librarySelectItemValue.value,
        );
    }

    // is so that users are asked wether or not to continue when switching attributes after making changes
    watch(attributeSelectItemValue,()=> onAttributeSelectItemValueChangedCheckUnsaved())
    function onAttributeSelectItemValueChangedCheckUnsaved(){
        if(attributeSelectItemValueAllowedChanged)
            CheckUnsavedDialog(onAttributeSelectItemValueChanged, () => {
                attributeSelectItemValueAllowedChanged = false;
                attributeSelectItemValue = trueAttributeSelectItemValue;               
            })
        attributeSelectItemValueAllowedChanged = true;
    }

    function onAttributeSelectItemValueChanged() {
        // selection change in calculated attribute multi select
        clearChanges();
        trueAttributeSelectItemValue = attributeSelectItemValue;
        checkHasUnsavedChanges();
        if (
            isNil(attributeSelectItemValue) ||
            attributeSelectItemValue.value == ''
        ) {
            isAttributeSelectedItemValue = false;
            isTimingSelectedItemValue = false;
            selectedAttribute = clone(emptyCalculatedAttribute);
        } else {
            isAttributeSelectedItemValue = true;
            isTimingSelectedItemValue = true;
            var item = calculatedAttributeGridData.value.find(
                _ => _.attribute == attributeSelectItemValue.value,
            );
            if (item != undefined) {
                activeCalculatedAttributeId = item.id;
                selectedAttribute = item;               
                initializePages();
            } else {
                // if the selected Calculated attribute data is not present in the grid
                // Add a new object for it. Because we cannot loop over a object, which is null
                var newAttributeObject: CalculatedAttribute = {
                    id: getNewGuid(),
                    libraryId: getNewGuid(),
                    isModified: false,
                    attribute:  attributeSelectItemValue.value,
                    name:  attributeSelectItemValue.value,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                calculatedAttributeGridData.value.push(newAttributeObject);
                activeCalculatedAttributeId = newAttributeObject.id;
                selectedAttribute = newAttributeObject;
                setTimingsMultiSelect(Timing.OnDemand);
                addedCalcAttr.push(clone(newAttributeObject));
                initializePages();
            }
        }
    }
    watch(attributeTimingSelectItemValue,() => onAttributeTimingSelectItemValue)
    function onAttributeTimingSelectItemValue() {
        // Change in timings select box
        if (
            isNil(attributeTimingSelectItemValue) ||
            attributeTimingSelectItemValue.value == ''
        ) {
            isTimingSelectedItemValue = false;
        } else {
            isTimingSelectedItemValue = true;
            var localTiming = attributeTimingSelectItemValue as unknown as Timing;
            var item = calculatedAttributeGridData.value.find(
                _ => _.attribute == attributeSelectItemValue.value,
            );
            if (item != undefined) {
                CalcAttrCache = clone(item)
                item.calculationTiming = localTiming;
                selectedAttribute = item;
                onUpdateCalcAttr(item.id, clone(item))
                onPaginationChanged();
            }
        }
    }

    watch(stateSelectedCalculatedAttributeLibrary,() => onStateSelectedCalculatedAttributeLibraryChanged())
    function onStateSelectedCalculatedAttributeLibraryChanged() {       
        selectedCalculatedAttributeLibrary.value = clone(stateSelectedCalculatedAttributeLibrary.value)
        isDefaultBool.value = selectedCalculatedAttributeLibrary.value.isDefault;
    }
    watch(stateScenarioCalculatedAttributes,() => onStateScenarioCalculatedAttributeChanged())
    function onStateScenarioCalculatedAttributeChanged() {
        if (hasScenario.value) {
            if (
                !isNil(stateScenarioCalculatedAttributes.value) &&
                stateScenarioCalculatedAttributes.value.length > 0
            ) {
                let test = stateScenarioCalculatedAttributes.value
                activeCalculatedAttributeId = stateScenarioCalculatedAttributes.value[0].id;
            }
            resetGridData();
        }
    }
    watch(calculatedAttributeGridData,() => onCalculatedAttributeGridDataChanged())
    function onCalculatedAttributeGridDataChanged() {
        if (hasAdminAccess.value) {
            checkHasUnsavedChanges();
        }
    }
    watch(selectedCalculatedAttributeLibrary, ()=> onSelectedCalculatedAttributeLibraryChanged())
    function onSelectedCalculatedAttributeLibraryChanged() {
        // change in library multiselect
        if (
            selectedCalculatedAttributeLibrary.value.id !== uuidNIL 
        ) {
            hasSelectedLibrary.value = true;
        } 
        else {
            hasSelectedLibrary.value = false;
        }

        clearChanges();
        if (hasScenario.value && hasSelectedLibrary.value) {
            getSelectedLibraryCalculatedAttributesAction(selectedCalculatedAttributeLibrary.value.id).then(() =>{
                // we need new ids for the object which is assigned to a scenario.
                calculatedAttributeGridData.value = clone(
                    stateSelectedLibraryCalculatedAttributes.value,
                );
                
                // Set the default values in Calculated attribute multi select, if we have data in calculatedAttributeGridData           
                if (
                    calculatedAttributeGridData.value != undefined &&
                    calculatedAttributeGridData.value.length > 0
                ) {
                    setDefaultAttributeOnLoad(
                        calculatedAttributeGridData.value[0],
                    );
                } 
                else {
                    isAttributeSelectedItemValue = false;
                    selectedGridItem.value = [];
                }
                onCalculatedAttributeGridDataChanged();
            })

            
        } else if (hasScenario.value && !hasSelectedLibrary.value) {
            // If a user un select a Library, then reset the grid data from the scenario calculated attribute state
            resetGridData();
            onCalculatedAttributeGridDataChanged();
        } 
        else if (!hasScenario.value && hasSelectedLibrary.value) {
            // If a user is in Lirabry page
            getSelectedLibraryCalculatedAttributesAction(selectedCalculatedAttributeLibrary.value.id).then(() =>{
                calculatedAttributeGridData.value = clone(
                    stateSelectedLibraryCalculatedAttributes.value,
                );
                if (
                    calculatedAttributeGridData.value != undefined &&
                    calculatedAttributeGridData.value.length > 0
                ) {
                    attributeSelectItemValue.value = clone(
                        calculatedAttributeGridData.value[0].attribute,
                    );
                    isAttributeSelectedItemValue = true;

                    setTimingsMultiSelect(
                        calculatedAttributeGridData.value[0].calculationTiming,
                    );
                    activeCalculatedAttributeId = calculatedAttributeGridData.value[0].id;
                    selectedAttribute =
                        calculatedAttributeGridData.value[0] != undefined
                            ? calculatedAttributeGridData.value[0]
                            : selectedCalculatedAttribute;
                } else {
                    isAttributeSelectedItemValue = false;
                    attributeSelectItemValue.value = '';
                    attributeTimingSelectItemValue.value = '';
                     isTimingSelectedItemValue = false;
                     selectedGridItem.value = [];
                }
                 onCalculatedAttributeGridDataChanged();
            })          
        }         
    }
    
    watch(isSharedLibrary,()=> onStateSharedAccessChanged())
    function onStateSharedAccessChanged() {
         isShared =  isSharedLibrary.value;
         if (!isNil( selectedCalculatedAttributeLibrary.value) ) {
            selectCalculatedAttributeLibraryAction(selectedCalculatedAttributeLibrary.value).then(() => isShared = isSharedLibrary.value);
                    }
        //if (!isNullOrUndefined(selectedCalculatedAttributeLibrary)) {

             //selectedCalculatedAttributeLibrary.isShared =  isShared;
        //} 
    }
    function setTiming(selectedItem: number) {
         setTimingsMultiSelect(selectedItem);
    }

    function getOwnerUserName(): string {

        if (! hasCreatedLibrary) {
        return  getUserNameByIdGetter( selectedCalculatedAttributeLibrary.value.owner);
        }
        
        return getUserName();
    }

    function onUpsertScenarioCalculatedAttribute() {

        if ( selectedCalculatedAttributeLibrary.value.id ===  uuidNIL ||  hasUnsavedChanges.value &&  newLibrarySelection ===false) { scenarioLibraryIsModified = true;}
        else {  scenarioLibraryIsModified = false; }

        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId:  selectedCalculatedAttributeLibrary.value.id ===  uuidNIL ? null :  selectedCalculatedAttributeLibrary.value.id,
                updatedCalculatedAttributes: Array.from( updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature( deletionPairsIds),
                updatedPairs: mapToIndexSignature(  updatedPairs),
                addedPairs: mapToIndexSignature( addedCalcPairs) ,
                addedCalculatedAttributes:  addedCalcAttr,
                defaultEquations: mapToIndexSignature( defaultEquations),
                isModified:  scenarioLibraryIsModified
        }
        CalculatedAttributeService.upsertScenarioCalculatedAttribute(syncModel,  selectedScenarioId).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                 parentLibraryId =  librarySelectItemValue.value ?  librarySelectItemValue.value : "";
                 getScenarioCalculatedAttributeAction( selectedScenarioId);
                 clearChanges()
                 resetPage();
                 addSuccessNotificationAction({message: "Modified calculated attrbutes"});
                 librarySelectItemValue.value = ''
            }   
        }))
    }

   function onUpsertCalculatedAttributeLibrary() {
        const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId:  selectedCalculatedAttributeLibrary.value.id ===  uuidNIL ? null :  selectedCalculatedAttributeLibrary.value.id,
                updatedCalculatedAttributes: Array.from( updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature( deletionPairsIds),
                updatedPairs: mapToIndexSignature(  updatedPairs),
                addedPairs: mapToIndexSignature( addedCalcPairs),
                addedCalculatedAttributes:  addedCalcAttr,
                defaultEquations: mapToIndexSignature( defaultEquations),
                isModified: false
        }
        const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
            syncModel: syncModel,
            isNewLibrary: false,
            library:  selectedCalculatedAttributeLibrary.value,
            scenarioId: null
        }
        CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                 clearChanges()
                 resetPage();
                 calculatedAttributeLibraryMutator( selectedCalculatedAttributeLibrary.value)
                 selectedCalculatedAttributeLibraryMutator( selectedCalculatedAttributeLibrary.value.id);
                 addSuccessNotificationAction({message: "Updated calculated attribute library",});
            }   
        }))
    }
    function onShowCreateCalculatedAttributeLibraryDialog(createAsNewLibrary: boolean) {
        var localCalculatedAttributes = [] as CalculatedAttribute[];
        if (createAsNewLibrary) {
            // if library is getting created from a scenario. Assign new Ids
            localCalculatedAttributes = clone( calculatedAttributeGridData.value);
            localCalculatedAttributes.forEach(val => {
                val.id = getNewGuid();
                val.equations.forEach(eq => {
                    eq.id = getNewGuid();
                    if (!isNil(eq.criteriaLibrary)) {
                        eq.criteriaLibrary.id = getNewGuid();
                        eq.criteriaLibrary.isSingleUse = false;
                    }
                    eq.equation.id = getNewGuid();
                });
            });
        }
         createCalculatedAttributeLibraryDialogData = {
            showDialog: true,
            calculatedAttributes: createAsNewLibrary
                ? localCalculatedAttributes
                : ([] as CalculatedAttribute[]),
            attributeSelectItems:  attributeSelectItems,
        };
    }
    function onSubmitCreateCalculatedAttributeLibraryDialogResult(
        calculatedAttributeLibrary: CalculatedAttributeLibrary,
    ) {
         createCalculatedAttributeLibraryDialogData = clone(
            emptyCreateCalculatedAttributeLibraryDialogData,
        );

        if (!isNil(calculatedAttributeLibrary)) {
            const syncModel: CalculatedAttributePagingSyncModel = {
                libraryId: calculatedAttributeLibrary.calculatedAttributes.length === 0 || ! hasSelectedLibrary.value ? null :  selectedCalculatedAttributeLibrary.value.id,
                updatedCalculatedAttributes: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? [] : Array.from( updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature( deletionPairsIds),
                updatedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature(  updatedPairs),
                addedPairs: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature( addedCalcPairs),
                addedCalculatedAttributes: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? [] :  addedCalcAttr,
                defaultEquations: calculatedAttributeLibrary.calculatedAttributes.length === 0 ? {} : mapToIndexSignature( defaultEquations),
                isModified: false
            }
            const request: CalculatedAttributeLibraryUpsertPagingRequestModel = {
                syncModel: syncModel,
                isNewLibrary: true,
                library: calculatedAttributeLibrary,
                scenarioId:  hasScenario.value ?  selectedScenarioId : null
            }
            CalculatedAttributeService.upsertCalculatedAttributeLibrary(request).then(((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                     clearChanges()
                     resetPage();
                     calculatedAttributeLibraryMutator(calculatedAttributeLibrary);
                     selectedCalculatedAttributeLibraryMutator(calculatedAttributeLibrary.id);
                     addSuccessNotificationAction({message: "Updated calculated attribute library",});
                     librarySelectItemValue.value = calculatedAttributeLibrary.id;
                     hasCreatedLibrary = true;
                }   
            }))          
        }
    }
    function onSubmitCreateCalculatedAttributeDialogResult(
        newCalculatedAttribute: CalculatedAttribute[],
    ) {
         showCreateCalculatedAttributeDialog = false;

        if (!isNil(newCalculatedAttribute)) {
             calculatedAttributeGridData.value = clone(newCalculatedAttribute);
        }
    }

    function disableCrudButton() {
        if ( calculatedAttributeGridData.value == undefined) {
            return false;
        }

        if( defaultEquation.id === getBlankGuid() ||  defaultEquation.equation.expression.trim() === '')
            return true;   

        var updatePairs = clone( updatedPairs.get( selectedAttribute.id));
        var addedPairs = clone(addedCalcPairs.get( selectedAttribute.id));
        var equations: CriterionAndEquationSet[] = [];
        if(!isNil(updatePairs))
            equations = updatePairs;
        if(!isNil(addedPairs))
            equations = equations.concat(addedPairs);

        if(equations.length === 0)
            return false;

        var dataIsValid = false
        if(!isNil(equations)){
            if(equations.every(_ =>(!isNil(_.criteriaLibrary) && _.criteriaLibrary.mergedCriteriaExpression!.trim() !== '') && 
            (!isNil(_.equation) && _.equation.expression.trim() !== '')))
                dataIsValid = true;
        }


        return !dataIsValid;
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
             librarySelectItemValue.value = '';
             deleteCalculatedAttributeLibraryAction(
                 selectedCalculatedAttributeLibrary.value.id,
            );
        }
    }
    function onAddCriterionEquationSet() { 
        var newSet = clone(emptyCriterionAndEquationSet);
        newSet.id = getNewGuid();
        newSet.criteriaLibrary.id = getNewGuid();
        newSet.equation.id = getNewGuid();
        newSet.criteriaLibrary.isSingleUse = true;

        let pairs =  addedCalcPairs.get( selectedAttribute.id);
        if(!isNil(pairs)){
            pairs.push(newSet)
        }
        else
             addedCalcPairs.set( selectedAttribute.id, [newSet])
        
        if ( selectedAttribute.equations == undefined) {
             selectedAttribute.equations = [];
             onSelectedAttributeChanged()
        }
         selectedAttribute.equations.push(newSet);
         calculatedAttributeGridData.value = update(
            findIndex(
                propEq('id',  selectedAttribute.id),
                 calculatedAttributeGridData.value,
            ),
            { ... selectedAttribute },
             calculatedAttributeGridData.value,
        );
         onPaginationChanged();
    }
    function onEditCalculatedAttributeCriterionLibrary(criterionEquationSetId: string) {
        var currentCriteria = clone( currentPage.equations.find(
            _ => _.id == criterionEquationSetId,
        )!);
         currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (currentCriteria.criteriaLibrary.id == getBlankGuid()) {
            currentCriteria.criteriaLibrary = {
                id: getNewGuid(),
                name: '',
                mergedCriteriaExpression: '',
                description: '',
                isSingleUse: true,
                isShared: false,
            };
        }
        if (!isNil(currentCriteria)) {
             hasSelectedCalculatedAttribute = true;

             criterionEditorDialogData = {
                showDialog: true,
                CriteriaExpression: currentCriteria.criteriaLibrary.mergedCriteriaExpression,
            };
        }
    }

    function onSubmitCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
         criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        var currItem =  calculatedAttributeGridData.value.find(
            _ => _.id ==  activeCalculatedAttributeId,
        )!;
        if (!isNil(criterionExpression) &&  hasSelectedCalculatedAttribute) {

            if(!isNil(currItem)){
                var set = clone( currentPage.equations.find(_ => _.id ===  currentCriteriaEquationSetSelectedId));
                if(!isNil(set)){
                    if(set.criteriaLibrary.id === getBlankGuid())
                        set.criteriaLibrary.id = getNewGuid();
                    set.criteriaLibrary.mergedCriteriaExpression = criterionExpression;
                     onUpdatePair(set.id, set);
                     onPaginationChanged();
                }
            }
             onSelectedAttributeChanged();
        }

         selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
         hasSelectedCalculatedAttribute = false;
    }
    function onShowEquationEditorDialog(criterionEquationSetId: string) {
        var currentEquation = clone( currentPage.equations.find(
            _ => _.id == criterionEquationSetId,
        ));
         currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (!isNil(currentEquation)) {
             hasSelectedCalculatedAttribute = true;

             equationEditorDialogData = {
                showDialog: true,
                equation: currentEquation.equation,
            };
        }
    }

    function onShowEquationEditorDialogForDefaultEquation() {
         defaultSelected = true;
         equationEditorDialogData = {
            showDialog: true,
            equation:  defaultEquation.equation,
        };
    }
    function onSubmitEquationEditorDialogResult(equation: Equation) {
         equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) &&  hasSelectedCalculatedAttribute) {
            var currItem =  calculatedAttributeGridData.value.find(
                _ => _.id ==  activeCalculatedAttributeId,
            )!;

            if(!isNil(currItem)){
                var pair = clone( currentPage.equations.find(_ => _.id ==  currentCriteriaEquationSetSelectedId));
                if(!isNil(pair)){
                    pair.equation.expression = equation.expression;
                     onUpdatePair(pair.id, pair);
                     onPaginationChanged();
                }
            }
             onSelectedAttributeChanged();
        }
        else if (!isNil(equation) &&  defaultSelected) {

            var defaultPair =  defaultEquation;
            if(!isNil(defaultPair)){
                defaultPair.equation.expression = equation.expression;
                 updatedDefaultEquation(defaultPair);
                 checkHasUnsavedChanges();
            }           
        }
         selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
         hasSelectedCalculatedAttribute = false;
         defaultSelected = false;
    }
    function onRemoveCalculatedAttribute(criterionEquationSetId: string) {
        var currItem =  calculatedAttributeGridData.value.find(
            _ => _.id ==  activeCalculatedAttributeId,
        );
        if(!isNil(currItem)){
            var pair = clone( currentPage.equations.find(_ => _.id === criterionEquationSetId))
            if(!isNil(pair)){
                var addPairs =  addedCalcPairs.get(currItem.id);
                var updatePairs =  updatedPairs.get(currItem.id);
                if(!isNil(addPairs)){
                    if(!isNil(find(propEq('id', criterionEquationSetId), addPairs))){
                        addPairs = addPairs.filter(_ => _.id !== criterionEquationSetId);
                         addedCalcPairs.set(currItem.id, addPairs);
                    }                       
                    else{
                         removePair(currItem.id, criterionEquationSetId);
                    }                  
                }  
                else if (!isNil(updatePairs)){
                    if(!isNil(find(propEq('id', criterionEquationSetId), updatePairs))){
                        updatePairs = updatePairs.filter(_ => _.id !== criterionEquationSetId);
                         updatedPairs.set(currItem.id, updatePairs);
                    }                       
                     removePair(currItem.id, criterionEquationSetId);                        
                }                 
                else
                     removePair(currItem.id, criterionEquationSetId);
                 onPaginationChanged();
            }
        }
        
         onSelectedAttributeChanged()
    }

    function removePair(calcAttriId:string, pairId: string){
        var deletionPair =  deletionPairsIds.get(calcAttriId);
        if(!isNil(deletionPair))
            deletionPair.push(pairId);
        else 
             deletionPairsIds.set(calcAttriId, [pairId]);
    }
    function onDiscardChanges() {
         librarySelectItemValue.value = '';
         selectedCalculatedAttributeLibrary.value = clone(
            emptyCalculatedAttributeLibrary,
        );
        setTimeout(() => {
            if ( hasScenario.value) {
                 resetGridData();
                 clearChanges();
                 resetPage();
            }
        });
         parentLibraryName =  loadedParentName;
         parentLibraryId =  loadedParentId;
    }

    function resetGridData() {
         calculatedAttributeGridData.value = clone(
             stateScenarioCalculatedAttributes.value,
        );

        if ( calculatedAttributeGridData.value != undefined) {
            var currItem =  calculatedAttributeGridData.value.find(
                _ => _.id ==  activeCalculatedAttributeId,
            )!;

            if (currItem != undefined) {
                 attributeSelectItemValue.value = clone(currItem.attribute);
                 isAttributeSelectedItemValue = true;

                 setTimingsMultiSelect(currItem.calculationTiming);
                 selectedAttribute = currItem;
                // Setting up default values for null object, because API is sending it as null.
                 selectedAttribute.equations.forEach(_ => {
                    if (isNil(_.criteriaLibrary)) {
                        _.criteriaLibrary = clone(emptyCriterionLibrary);
                        _.criteriaLibrary.id = getNewGuid();
                        _.criteriaLibrary.isSingleUse = true;
                    }
                });
                 onSelectedAttributeChanged();
            } else if ( calculatedAttributeGridData.value.length > 0) {
                 attributeSelectItemValue.value =  calculatedAttributeGridData.value[0].attribute;
                 isAttributeSelectedItemValue = true;
                 setTimingsMultiSelect(
                     calculatedAttributeGridData.value[0].calculationTiming,
                );
            } else {
                 attributeSelectItemValue.value = '';
                 isAttributeSelectedItemValue = false;
            }
        }
    }
    function setTimingsMultiSelect(selectedItem: number) {
        if (selectedItem == undefined) {
            selectedItem = Timing.OnDemand;
        }
        var localTiming =  attributeTimingSelectItems.find(
            _ => _.value == selectedItem,
        )!.text;
         attributeTimingSelectItemValue.value = selectedItem.toString();
         isTimingSelectedItemValue = true;
    }
    function setDefaultAttributeOnLoad(localCalculatedAttribute: CalculatedAttribute) {
         attributeSelectItemValue.value = clone(
            localCalculatedAttribute.attribute,
        );
         isAttributeSelectedItemValue = true;

         setTimingsMultiSelect(localCalculatedAttribute.calculationTiming);
         activeCalculatedAttributeId = localCalculatedAttribute.id;
         selectedAttribute =
            localCalculatedAttribute != undefined
                ? localCalculatedAttribute
                :  selectedAttribute;
    }

    function calculatedAttributeGridModelConverter(item: CalculatedAttribute): CalculatedAttributeGridModel[]{
        let toReturn: CalculatedAttributeGridModel[] = []
        if(!isNil(item.equations))
        {
            item.equations.forEach(_ =>{
                toReturn.push({
                    id: _.id,
                    equation: isNil(_.equation) ? "" : _.equation.expression,
                    criteriaExpression: isNil(_.criteriaLibrary) || isNil(_.criteriaLibrary.mergedCriteriaExpression) ? "" : _.criteriaLibrary.mergedCriteriaExpression
                    })
            })
        }
        return toReturn
    }

    function onUpdateCalcAttr(rowId: string, updatedRow: CalculatedAttribute){
        var addedrow =  addedCalcAttr.find(_ => _.id === rowId);
        if(!isNil(addedrow)){
            addedrow = updatedRow;
            return;
        }
            
        let mapEntry =  updatedCalcAttrMap.get(rowId)

        if(isNil(mapEntry)){
            const row =  CalcAttrCache;
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row) && row.attribute === updatedRow.attribute)
                 updatedCalcAttrMap.set(rowId, [row , updatedRow])
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
        }
        else
             updatedCalcAttrMap.delete(rowId)

         checkHasUnsavedChanges();
    }

    function onUpdatePair(rowId: string, updatedRow: CriterionAndEquationSet){
        if(!isNil( addedCalcPairs.get( selectedAttribute.id)))
            if(any(propEq('id', rowId),  addedCalcPairs.get( selectedAttribute.id)!)){
                let amounts =  addedCalcPairs.get( selectedAttribute.id)!
                amounts[amounts.findIndex(b => b.id == rowId)] = updatedRow;
                return;
            }               

        let mapEntry =  updatedPairsMaps.get(rowId)

        if(isNil(mapEntry)){
            const row =  pairsCache.find(r => r.id === rowId);
            if(!isNil(row) && hasUnsavedChangesCore('', updatedRow, row)){
                 updatedPairsMaps.set(rowId, [row , updatedRow])
                if(!isNil( updatedPairs.get( selectedAttribute.id)))
                     updatedPairs.get( selectedAttribute.id)!.push(updatedRow)
                else
                     updatedPairs.set( selectedAttribute.id, [updatedRow])
            }               
        }
        else if(hasUnsavedChangesCore('', updatedRow, mapEntry[0])){
            mapEntry[1] = updatedRow;
            let amounts =  updatedPairs.get( selectedAttribute.id)
            if(!isNil(amounts))
                amounts[amounts.findIndex(r => r.id == updatedRow.id)] = updatedRow
        }
        else{
            let amounts =  updatedPairs.get( selectedCalculatedAttribute.id)
            if(!isNil(amounts))
                amounts.splice(amounts.findIndex(r => r.id == updatedRow.id), 1)
        }
            

         checkHasUnsavedChanges();
    }

    function updatedDefaultEquation(defaultEq: CriterionAndEquationSet){
        if(!isNil(defaultEq)){
            var mapEntry =  defaultEquations.get( selectedAttribute.id)
            if(isNil(mapEntry)){
                if(hasUnsavedChangesCore('', defaultEq,  defaultEquationCache)){
                     defaultEquation = clone(defaultEq);
                     defaultEquations.set( selectedAttribute.id, clone(defaultEq));
                }
            }               
            else{               
                if(hasUnsavedChangesCore('', defaultEq,  defaultEquationCache)){
                     defaultEquation = clone(defaultEq);
                    mapEntry = clone(defaultEq);
                }
            }
        }
    }

    function clearChanges(){
         updatedPairs.clear();
         updatedPairsMaps.clear();
         addedCalcPairs.clear();
         deletionPairsIds.clear();
         updatedCalcAttrMap.clear();
         defaultEquations.clear();
        if( addedCalcAttr.length > 0){
            var addedIds =  addedCalcAttr.map(_ => _.id);
             calculatedAttributeGridData.value =  calculatedAttributeGridData.value.filter(_ => !addedIds.includes(_.id))
             addedCalcAttr = [];
        }  
    }

    function resetPage(){
         pagination.page = 1;
         onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
             deletionPairsIds.size > 0 || 
             addedCalcPairs.size > 0 ||
             updatedCalcAttrMap.size > 0 || 
             updatedPairs.size > 0 || 
             addedCalcAttr.length > 0 ||
             defaultEquations.size > 0 ||
            ( hasScenario.value &&  hasSelectedLibrary.value) ||
            ( hasSelectedLibrary.value && hasUnsavedChangesCore('',  selectedCalculatedAttributeLibrary.value,  stateSelectedCalculatedAttributeLibrary.value))
         setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function onSearchClick(){
         currentSearch =  gridSearchTerm;
         resetPage();
    }

    function onClearClick(){
         gridSearchTerm = '';
         onSearchClick();
    }

    function CheckUnsavedDialog(next: any, otherwise: any) {
        const hasUnsavedChanges: boolean = 
             deletionPairsIds.size > 0 || 
             addedCalcPairs.size > 0 ||
             updatedCalcAttrMap.size > 0 || 
             updatedPairs.size > 0 || 
             addedCalcAttr.length > 0 ||
            ( hasSelectedLibrary.value && ! hasScenario.value && hasUnsavedChangesCore('',  selectedCalculatedAttributeLibrary.value,  stateSelectedCalculatedAttributeLibrary.value))
        if (hasUnsavedChanges &&  unsavedDialogAllowed) {

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

    function onShowShareCalculatedAttributeLibraryDialog(calculatedAttributeLibrary: CalculatedAttributeLibrary) {
         shareCalculatedAttributeLibraryDialogData = {
            showDialog:true,
            calculatedAttributeLibrary: clone(calculatedAttributeLibrary)
        }
    }

    function onShareCalculatedAttributeDialogSubmit(calculatedAttributeLibraryUsers: CalculatedAttributeLibraryUser[]) {
         shareCalculatedAttributeLibraryDialogData = clone(emptyShareCalculatedAttributeLibraryDialogData);

                if (!isNil(calculatedAttributeLibraryUsers) &&  selectedCalculatedAttributeLibrary.value.id !== getBlankGuid())
                {
                    let libraryUserData: LibraryUser[] = [];

                    //create library users
                    calculatedAttributeLibraryUsers.forEach((calculatedAttributeLibraryUser, index) =>
                    {   
                        //determine access level
                        let libraryUserAccessLevel: number = 0;
                        if (libraryUserAccessLevel == 0 && calculatedAttributeLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                        if (libraryUserAccessLevel == 0 && calculatedAttributeLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                        //create library user object
                        let libraryUser: LibraryUser = {
                            userId: calculatedAttributeLibraryUser.userId,
                            userName: calculatedAttributeLibraryUser.username,
                            accessLevel: libraryUserAccessLevel
                        }

                        //add library user to an array
                        libraryUserData.push(libraryUser);
                    });
                    if (!isNil( selectedCalculatedAttributeLibrary.value.id) ) {
                         getIsSharedLibraryAction( selectedCalculatedAttributeLibrary.value).then(()=>isShared = isSharedLibrary.value)
                    }
                    //update calculated attribute library sharing
                    CalculatedAttributeService.upsertOrDeleteCalculatedAttributeLibraryUsers( selectedCalculatedAttributeLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
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
        let foundLibrary: CalculatedAttributeLibrary = emptyCalculatedAttributeLibrary;
         stateCalculatedAttributeLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
         parentLibraryId = foundLibrary.id;
         parentLibraryName = foundLibrary.name;
    }

    function initializePages(){
        const request: CalculatedAttributePagingRequestModel= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId:  selectedCalculatedAttributeLibrary.value.id ===  uuidNIL ? null :  selectedCalculatedAttributeLibrary.value.id,
                updatedCalculatedAttributes: Array.from( updatedCalcAttrMap.values()).map(r => r[1]),
                deletedPairs: mapToIndexSignature( deletionPairsIds),
                updatedPairs: mapToIndexSignature(  updatedPairs),
                addedPairs: mapToIndexSignature( addedCalcPairs),
                addedCalculatedAttributes:  addedCalcAttr,
                defaultEquations: mapToIndexSignature( defaultEquations),
                isModified: false
            },           
            sortColumn: '',
            isDescending: false,
            search: '',
            attributeId:  stateCalculatedAttributes.value.find(_ => _.name ===  selectedAttribute.attribute)!.id
        };
        
        if((! hasSelectedLibrary.value &&  hasScenario.value) &&  selectedScenarioId !==  uuidNIL){
            CalculatedAttributeService.getScenarioCalculatedAttrbiutetPage( selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                     currentPage.equations = data.items;
                     currentPage.calculationTiming = data.calculationTiming
                     pairsCache =  currentPage.equations;
                     totalItems = data.totalItems;
                     defaultEquation = data.defaultEquation;
                     defaultEquationCache = clone( defaultEquation);
                     selectedGridItem.value =  calculatedAttributeGridModelConverter( currentPage)
                     setTimingsMultiSelect( currentPage.calculationTiming);

                     setParentLibraryName(data.libraryId);
                     loadedParentId = data.libraryId;
                     loadedParentName =  parentLibraryName; //store original
                     scenarioLibraryIsModified = data.isModified;
                }
                 initializing = false;
            });
        }            
        else if( hasSelectedLibrary.value)
            CalculatedAttributeService.getLibraryCalculatedAttributePage( librarySelectItemValue.value !== null ?  librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as calculcatedAttributePagingPageModel;
                     currentPage.equations = data.items;
                     currentPage.calculationTiming = data.calculationTiming
                     pairsCache =  currentPage.equations;
                     totalItems = data.totalItems;
                     defaultEquation = data.defaultEquation;
                     defaultEquationCache = clone( defaultEquation);
                     selectedGridItem.value =  calculatedAttributeGridModelConverter( currentPage)
                     setTimingsMultiSelect( currentPage.calculationTiming);
                }
                 initializing = false;
            });
        else
             initializing = false;
    }


    
    
</script>
<style>
.sharing {
    padding-top: 0;
    margin: 0;
}
.sharing .v-input__slot{
    top: 5px !important;
}
.sharing .v-label{
    margin-bottom: 0;
    position: relative;
    top: -4px !important;
}
</style>