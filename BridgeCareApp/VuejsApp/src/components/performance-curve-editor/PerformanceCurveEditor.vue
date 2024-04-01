<template>
    <v-card class="elevation-0 vcard-main-layout" >
        <v-row>
            <v-col cols = "auto">
                <div style="margin-bottom: 10px;">
                    <v-subheader class="ghd-control-label ghd-md-gray">Deterioration Model Library</v-subheader>
                </div>
                <v-select
                    id="PerformanceCurveEditor-library-select"
                    class="ghd-control-border ghd-control-text ghd-select "
                    :items="librarySelectItems"
                    menu-icon=custom:GhdDownSvg
                    variant="outlined"
                    v-model="librarySelectItemValue"
                    item-title="text" 
                    item-value="value" 
                    density="compact"
                >
                </v-select>
                <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}}                    
                    <span v-if="scenarioLibraryIsModified">&nbsp;&nbsp;{{modifiedStatus}}</span></b>              
                </div>

            </v-col>
            <v-spacer/>
            <v-col style="padding-top: 50px;" cols = "5" v-show="hasSelectedLibrary || hasScenario">                     
                <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                <v-row align="center">
                
                <v-text-field
                    id="PerformanceCurveEditor-searchDeteriorationEquations-textField"
                    class="ghd-text-field-border ghd-text-field search-icon-general"
                    style="margin-top:0px;"
                    prepend-inner-icon=custom:GhdSearchSvg
                    hide-details
                    placeholder="Search Deterioration Equations"
                    single-line
                    variant="outlined"
                    density="compact"
                    clearable
                    @click:clear="onClearClick()"
                    v-model="gridSearchTerm"
                >
                </v-text-field>
                <v-btn id="PerformanceCurveEditor-search-button"  class='m-2 ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" @click="onSearchClick()">Search</v-btn>
                </v-row>
            </v-col>
            <v-spacer cols = "auto" v-show="!(hasSelectedLibrary || hasScenario)"/>                
            <v-col cols = "auto" v-show='!hasScenario'>
                <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                <v-row align="end" justify="end">
                    <v-btn
                        id="PerformanceCurveEditor-createNewLibrary-button"
                        @click='onShowCreatePerformanceCurveLibraryDialog(false)'
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        style="margin-top: 45px;"
                        variant = "outlined">
                        Create New Library
                    </v-btn>
                </v-row>
            </v-col>                    
        </v-row>
        <v-row style="height:48px;">
            <v-col cols = "auto" v-show="!hasScenario">
                <v-row>
                        <div style="margin-top:6px;"
                            v-if='hasSelectedLibrary && !hasScenario'
                            class="header-text-content owner-padding"
                        > 
                            Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ modifiedDate }}
                        <v-btn
                            id="PerformanceCurveEditor-shareLibrary-button"
                            @click='onShowSharePerformanceCurveLibraryDialog(selectedPerformanceCurveLibrary)'
                            style="margin-left: 10px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                            v-show='!hasScenario'>
                            Share Library
                        </v-btn>
                        </div>
                </v-row>
            </v-col>
            <!-- <v-spacer  v-show="hasScenario"/> -->
            <v-spacer></v-spacer>
            <v-col cols = "auto" v-show="hasScenario || hasSelectedLibrary">
                <v-row row align="end" style="margin-top:-4px;height:40px;">
                    <v-btn
                        id="PerformanceCurveEditor-upload-button"
                        :disabled='false' @click='showImportExportPerformanceCurvesDialog = true'
                        variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                        Upload
                    </v-btn>
                    <v-divider class="upload-download-divider" inset vertical>
                    </v-divider>
                    <v-btn
                        id="PerformanceCurveEditor-download-button"
                        :disabled='false' @click='exportPerformanceCurves()'
                        variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                        Download
                    </v-btn>
                    <v-divider class="upload-download-divider" inset vertical>
                    </v-divider>
                    <v-btn
                        id="PerformanceCurveEditor-downloadTemplate-button"
                        :disabled='false' @click='OnDownloadTemplateClick()'
                        variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                        Download Template
                    </v-btn>
                </v-row>            
            </v-col>
        </v-row>
        <v-row class="data-table" justify="start" v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-col cols = "12">
                <!-- <v-card class="elevation-0"> -->
                    <v-data-table-server
                        id="PerformanceCurveEditor-deteriorationModels-datatable"                    
                        show-select
                        class='v-table__overflow ghd-table'
                        item-key="id"
                        :headers="performanceCurveGridHeaders"
                        :pagination.sync="performancePagination"
                        :must-sort='true'
                        sort-asc-icon="custom:GhdTableSortAscSvg"
                        sort-desc-icon="custom:GhdTableSortDescSvg"
                        v-model="selectedPerformanceEquations"
                        return-object
                        :items="currentPage"                      
                        :items-length="totalItems"
                        :items-per-page-options="[
                            {value: 5, title: '5'},
                            {value: 10, title: '10'},
                            {value: 25, title: '25'},
                        ]"
                        v-model:sort-by="performancePagination.sort"
                        v-model:page="performancePagination.page"
                        v-model:items-per-page="performancePagination.rowsPerPage"                          
                        @update:options="onPaginationChanged"                           
                    >
                        <template slot="items" slot-scope="props" v-slot:item="item">
                            <tr>
                            <td>
                                <v-checkbox id="PerformanceCurveEditor-deleteModel-vcheckbox" class="ghd-checkbox"
                                    hide-details
                                    primary
                                    v-model="selectedPerformanceEquations" :value="item.item"
                                >
                                </v-checkbox>
                            </td>                                
                            <td class="text-xs-left">
                                <editDialog
                                    v-model:return-value="item.item.name"
                                    @save="
                                        onEditPerformanceCurveProperty(
                                            item.item.id,
                                            'name',
                                            item.item.name,
                                        )
                                    "
                                    size="large"
                                    lazy
                                >
                                    <v-text-field
                                        readonly
                                        single-line
                                        variant="underlined"
                                        class="sm-txt equation-name-text-field-output"
                                        :model-value="item.item.name"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                    <template v-slot:input>
                                        <v-text-field
                                            label="Edit"
                                            single-line
                                            variant="underlined"
                                            v-model="item.item.name"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                    </template>
                                </editDialog>
                            </td>
                            <td class="text-xs-left">
                                <editDialog
                                    v-model:return-value="
                                        item.item.attribute
                                    "
                                    @save="
                                        onEditPerformanceCurveProperty(
                                            item.item.id,
                                            'attribute',
                                            item.item.attribute,
                                        )
                                    "
                                    size="large"
                                    lazy
                                >
                                    <v-text-field
                                        readonly
                                        single-line
                                        variant="underlined"
                                        class="sm-txt attribute-text-field-output"
                                        :model-value="item.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                    <template v-slot:input>
                                        <v-select
                                            :items="attributeSelectItems"
                                            menu-icon=custom:GhdDownSvg
                                            label="Edit"
                                            variant="outlined"
                                            v-model="item.item.attribute"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                            item-title="text" 
                                            item-value="value" 
                                        />
                                    </template>
                                </editDialog>
                            </td>
                            <td class="text-xs-left">
                                <v-menu
                                    location="left"
                                    v-show="item.item.equation.expression !== ''"
                                >
                                    <template v-slot:activator="{ props }">
                                        <v-btn id="PerformanceCurveEditor-checkEquationEye-vbtn" v-bind="props" class="ghd-blue" icon variant="flat">
                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')">
                                        </v-btn>
                                    </template>
                                    <v-card>
                                        <v-card-text>
                                            <v-textarea
                                                id="PerformanceCurveEditor-checkEquation-vtextarea"
                                                class="sm-txt Montserrat-font-family"
                                                :model-value="
                                                    item.item.equation
                                                        .expression
                                                "
                                                full-width
                                                no-resize
                                                outline
                                                readonly
                                                rows="5"
                                                style = "min-width: 500px;min-height: 205px;"
                                            />
                                        </v-card-text>
                                    </v-card>
                                </v-menu>
                                <v-btn id="PerformanceCurveEditor-editEquation-vbtn"
                                    @click="onShowEquationEditorDialog(item.item.id) "
                                    class="ghd-blue"
                                    variant="flat"
                                    icon
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')">
                                </v-btn>
                            </td>
                            <td class="text-xs-left">
                                <v-menu                                    
                                    location="right"
                                    v-show="
                                        item.item.criterionLibrary
                                            .mergedCriteriaExpression !== ''
                                    "                                    
                                >
                                    <template v-slot:activator="{ props }">
                                        <v-btn v-bind="props" id="PerformanceCurveEditor-checkCriteriaEye-vbtn" class="ghd-blue" variant = "flat" icon>
                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')">
                                        </v-btn>
                                    </template>
                                    <v-card>
                                        <v-card-text>
                                            <v-textarea
                                                id="PerformanceCurveEditor-checkCriteria-vtextarea"
                                                class="sm-txt Montserrat-font-family"
                                                :model-value="
                                                    item.item
                                                        .criterionLibrary
                                                        .mergedCriteriaExpression
                                                "
                                                full-width
                                                no-resize
                                                outline
                                                readonly
                                                rows="5"
                                                style = "min-width: 500px;min-height: 205px;"
                                            />
                                        </v-card-text>
                                    </v-card>
                                </v-menu>
                                <v-btn id="PerformanceCurveEditor-editCriteria-vbtn"
                                    @click="onEditPerformanceCurveCriterionLibrary(item.item.id)"
                                    variant="flat"
                                    class="ghd-blue"
                                    icon
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')">
                                </v-btn>
                            </td>
                            <td class="text-xs-left">
                                <v-btn id="PerformanceCurveEditor-deleteModel-vbtn"
                                    @click="onRemovePerformanceCurve(item.item.id)"
                                    variant="flat"
                                    class="ghd-blue"
                                    icon
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </td>
                        </tr>
                        </template>
                        <!-- <template v-slot:body.append-inner>
                        <v-btn>Append button</v-btn>
                        </template>                                -->
                    </v-data-table-server>
                    <v-btn style="margin-top:-84px"
                        id="PerformanceCurveEditor-deleteSelected-button"
                        :disabled='selectedPerformanceEquationIds.length === 0 || (!hasLibraryEditPermission && !hasScenario)'
                        @click='onRemovePerformanceEquations'
                        class='ghd-blue' variant = "text"
                    >
                        Delete Selected
                    </v-btn>                        
                <!-- </v-card> -->
            </v-col>
        </v-row>
        <v-row class="header-height" justify="start" style="margin-bottom: 15px;" v-show="hasSelectedLibrary || hasScenario">
            <v-col cols = "3">
                <v-btn
                    id="PerformanceCurveEditor-addDeteriorationModel-button"
                    @click="showCreatePerformanceCurveDialog = true"
                    class="ghd-blue ghd-white-bg ghd-button-text ghd-button-border ghd-outline-button-padding"                                  
                    variant = "outlined"
                >
                    Add Deterioration Model
                </v-btn>
            </v-col>
        </v-row>        
        <v-divider
            v-show="hasSelectedLibrary || hasScenario"
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
        <v-row justify="center"  v-show="hasSelectedLibrary && !hasScenario">
            <v-col cols = "12">
                <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>                    
                <v-textarea
                    class="ghd-control-text ghd-control-border"
                    no-resize
                    variant="outlined"
                    rows="4"
                    v-model="selectedPerformanceCurveLibrary.description"
                    @update:model-value="checkHasUnsavedChanges()"
                />
            </v-col>
        </v-row>
        <v-row style="padding-bottom: 40px;"
            justify="center"
            row
            v-show='hasSelectedLibrary || hasScenario'
        >
            <v-btn
                id="PerformanceCurveEditor-cancel-button"
                :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                @click="onDiscardChanges"
                class="m-2 ghd-white-bg ghd-blue ghd-button-text"
                variant = "flat"
                v-show="hasScenario"
            >
                Cancel
            </v-btn>
            <v-btn
                id="PerformanceCurveEditor-deleteLibrary-button"
                @click="onShowConfirmDeleteAlert"
                class="m-2 ghd-white-bg ghd-blue ghd-button-text"
                variant = "flat"
                v-show="!hasScenario"
                :disabled="!hasLibraryEditPermission"
            >
                Delete Library
            </v-btn>                
            <v-btn
                id="PerformanceCurveEditor-createAsNewLibrary-button"
                :disabled="disableCrudButtons()"
                @click="onShowCreatePerformanceCurveLibraryDialog(true)"
                class='m-2 ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                variant = "outlined"                  
            >
                Create as New Library
            </v-btn>
            <v-btn
                id="PerformanceCurveEditor-updateLibrary-button"
                :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                @click='onUpsertPerformanceCurveLibrary'
                class="m-2 ghd-blue-bg ghd-white ghd-button-text ghd-button-border ghd-outline-button-padding"
                variant = "outlined"
                v-show='!hasScenario'
            >
                Update Library
            </v-btn>
            <v-btn
                id="PerformanceCurveEditor-save-button"
                :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                @click='onUpsertScenarioPerformanceCurves'
                class="ghd-blue-bg ghd-white ghd-button-text m-2"
                variant = "flat"
                v-show='hasScenario'
            >
                Save
            </v-btn>
        </v-row>

        <Alert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreatePerformanceCurveLibraryDialog
            :dialogData="createPerformanceCurveLibraryDialogData"
            @submit="onSubmitCreatePerformanceCurveLibraryDialogResult"
        />

        <SharePerformanceCurveLibraryDialog 
            :dialogData='sharePerformanceCurveLibraryDialogData' 
            @submit='onSharePerformanceCurveLibraryDialogSubmit'
        />

        <CreatePerformanceCurveDialog
            :showDialog="showCreatePerformanceCurveDialog"
            @submit="onSubmitCreatePerformanceCurveDialogResult"
        />

        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            :isFromPerformanceCurveEditor=true
            @submit="onSubmitEquationEditorDialogResult"
        />

        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onSubmitCriterionEditorDialogResult"
        />
        <ImportExportPerformanceCurvesDialog :showDialog='showImportExportPerformanceCurvesDialog'
            @submit='onSubmitImportExportPerformanceCurvesDialogResult' />
    </v-card>
    <ConfirmDialog></ConfirmDialog>
</template>

<script  lang="ts" setup>
import Vue from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import CreatePerformanceCurveLibraryDialog from './performance-curve-editor-dialogs/CreatePerformanceCurveLibraryDialog.vue';
import CreatePerformanceCurveDialog from './performance-curve-editor-dialogs/CreatePerformanceCurveDialog.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import {
    emptyPerformanceCurve,
    emptyPerformanceCurveLibrary,
    PerformanceCurve,
    PerformanceCurveLibrary,
    PerformanceCurvesFileImport
} from '@/shared/models/iAM/performance';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
    any,
    prepend,
    clone,
    find,
    findIndex,
    isNil,
    propEq,
    update,
    fromPairs,
props,
} from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import {
    CreatePerformanceCurveLibraryDialogData,
    emptyCreatePerformanceLibraryDialogData,
} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {
    SharePerformanceCurveLibraryDialogData,
    emptySharePerformanceCurveLibraryDialogData
} from '@/shared/models/modals/share-performance-curve-library-dialog-data';
import SharePerformanceCurveLibraryDialog from './performance-curve-editor-dialogs/SharePerformanceCurveLibraryDialog.vue';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Attribute } from '@/shared/models/iAM/attribute';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { LibraryUser } from '@/shared/models/iAM/user';
import { PerformanceCurveLibraryUser } from '@/shared/models/iAM/performance';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';
import ImportExportPerformanceCurvesDialog from '@/components/performance-curve-editor/performance-curve-editor-dialogs/ImportExportPerformanceCurvesDialog.vue';
import { ImportExportPerformanceCurvesDialogResult } from '@/shared/models/modals/import-export-performance-curves-dialog-result';
import PerformanceCurveService from '@/services/performance-curve.service';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { AxiosResponse } from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { LibraryUpsertPagingRequest, PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import { http2XX } from '@/shared/utils/http-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { Hub } from '@/connectionHub';
import ScenarioService from '@/services/scenario.service';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref, shallowRef, ShallowRef} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import mitt, { Emitter, EventType } from 'mitt';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import { computed } from 'vue';
import { getUrl } from '@/shared/utils/get-url';
import { nextTick } from 'process';

const emit = defineEmits(['submit'])
let store = useStore();
const confirm = useConfirm();

let statePerformanceCurveLibraries = computed<PerformanceCurveLibrary[]>(() => store.state.performanceCurveModule.performanceCurveLibraries);
let stateSelectedPerformanceCurveLibrary = computed<PerformanceCurveLibrary>(() => store.state.performanceCurveModule.selectedPerformanceCurveLibrary);
let stateScenarioPerformanceCurves = computed<PerformanceCurveLibrary[]>(() => store.state.performanceCurveModule.scenarioPerformanceCurves);
let stateNumericAttributes = computed<Attribute[]>(() => store.state.attributeModule.numericAttributes);
let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter);
let hasPermittedAccess = computed<boolean>(() => store.state.performanceCurveModule.hasPermittedAccess);
let isSharedLibrary = computed<boolean>(() => store.state.performanceCurveModule.isSharedLibrary);

async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess', payload);}
async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedPerformanceCurveLibrary', payload);}
async function getPerformanceCurveLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getPerformanceCurveLibraries', payload);}
function selectPerformanceCurveLibraryAction(payload?: any) { store.dispatch('selectPerformanceCurveLibrary', payload);}
async function deletePerformanceCurveLibraryAction(payload?: any): Promise<any> {await store.dispatch('deletePerformanceCurveLibrary', payload);}
function addErrorNotificationAction(payload?: any) { store.dispatch('addErrorNotification', payload);}
function setHasUnsavedChangesAction(payload?: any) { store.dispatch('setHasUnsavedChanges', payload);}
async function updatePerformanceCurveCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('updatePerformanceCurvesCriterionLibraries', payload);}
async function upsertOrDeletePerformanceCurveLibraryUsersAction(payload?: any): Promise<any> {await store.dispatch('upsertOrDeletePerformanceCurveLibraryUsers', payload);}
async function importScenarioPerformanceCurvesFileAction(payload?: any): Promise<any> {await store.dispatch('importScenarioPerformanceCurvesFile', payload);}
async function importLibraryPerformanceCurvesFileAction(payload?: any): Promise<any> {await store.dispatch('importLibraryPerformanceCurvesFile', payload);}
function addSuccessNotificationAction(payload?: any) { store.dispatch('addSuccessNotification', payload);}
async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario', payload);}
function selectScenarioAction(payload?: any){ store.dispatch('selectScenario', payload);}
function setAlertMessageAction(payload?: any) { store.dispatch('setAlertMessage', payload);}

let getUserNameByIdGetter: any = store.getters.getUserNameById
function performanceCurveLibraryMutator(payload:any){store.commit('performanceCurveLibraryMutator', payload);}
function selectedPerformanceCurveLibraryMutator(payload:any){store.commit('selectedPerformanceCurveLibraryMutator', payload);}

    let addedRows: ShallowRef<PerformanceCurve[]> = ref([]);
    let updatedRowsMap:Map<string, [PerformanceCurve, PerformanceCurve]> = new Map<string, [PerformanceCurve, PerformanceCurve]>();//0: original value | 1: updated value
    let deletionIds: ShallowRef<string[]> = ref([]);
    let rowCache: PerformanceCurve[] = [];
    let gridSearchTerm = ref('');
    let currentSearch = '';
    let selectedPerformanceCurveLibrary: ShallowRef<PerformanceCurveLibrary> = shallowRef(clone(
        emptyPerformanceCurveLibrary,
    ));
    let performancePagination  = ref(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = ref(0);
    let currentPage  = ref<PerformanceCurve[]>([]);
    let isRunning: boolean = true;
    let isShared: boolean = false;
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary = ref(false);
    let hasScenario = ref(false);
    let librarySelectItems  = ref<SelectItem[]>([]);
    let modifiedDate = ref<string>(''); 
    
    let performanceCurveGridHeaders: any[] = [
        {
            title: 'Name',
            key: 'name',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Attribute',
            key: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            title: 'Equation',
            key: 'equation',
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
            width: '',
        },
        {
            title: 'Actions',
            key: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];
    let performanceCurveGridData: PerformanceCurve[] = [];
    let attributeSelectItems: SelectItem[] = [];
    let selectedPerformanceCurve: PerformanceCurve = clone(emptyPerformanceCurve);
    let hasSelectedPerformanceCurve: boolean = false;
    const $router = useRouter();    
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>
    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: Ref<string | null> = ref(null);

    let selectedPerformanceEquations: Ref<PerformanceCurve[]> = ref([]);
    let selectedPerformanceEquationIds: string[] = [];

    let createPerformanceCurveLibraryDialogData = ref(clone(
        emptyCreatePerformanceLibraryDialogData,
    ));
    let equationEditorDialogData = ref(clone(
        emptyEquationEditorDialogData,
    ));
    let criterionEditorDialogData = ref(clone(
        emptyGeneralCriterionEditorDialogData,
    ));
    let showCreatePerformanceCurveDialog = ref(false);
    let confirmDeleteAlertData = ref(clone(emptyAlertData));
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let showImportExportPerformanceCurvesDialog = ref(false);    

    let sharePerformanceCurveLibraryDialogData = ref(clone(emptySharePerformanceCurveLibraryDialogData));

    let parentLibraryName = ref('None');
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;
    let modifiedStatus : string = "";

    created();
    async function created() {
        librarySelectItemValue.value= null;           
        await getPerformanceCurveLibrariesAction()
        await getHasPermittedAccessAction()
        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.PerformanceCurve) !== -1) {
            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Unable to identify selected scenario.',
                });
                $router.push('/Scenarios/');
            }
            hasScenario.value = true;
            await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioPerformanceCurve}).then(async response => {
                if(response.data){
                    setAlertMessageAction("A performance curve import has been added to the work queue")
                }
                await initializePages()
                hasScenario.value = true;
                await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId})   
                selectScenarioAction({ scenarioId: selectedScenarioId });        
            })
        }
    }
    onMounted(()=>mounted())
    function mounted() {
        setAttributeSelectItems();
        
        $emitter.on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }
    onBeforeUnmount(()=>beforeDestroy())
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });

        $emitter.off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );

        setAlertMessageAction('');
    }

    async function onPaginationChanged() {
        if(isRunning)
            return;
        checkHasUnsavedChanges();
        const { sort, descending, page, rowsPerPage } = performancePagination.value;
        const request: PagingRequest<PerformanceCurve>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: selectedPerformanceCurveLibrary.value.id === uuidNIL ? null : selectedPerformanceCurveLibrary.value.id,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: scenarioLibraryIsModified
            },           
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL){
            isRunning = true;

            await PerformanceCurveService.getPerformanceCurvePage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    isRunning = false;
                }
            });
        }          
        else if(hasSelectedLibrary.value){
            isRunning = true;
            await PerformanceCurveService.getPerformanceLibraryModifiedDate(selectedPerformanceCurveLibrary.value.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate.value = data.slice(0, 10);
                   }
             });

            await PerformanceCurveService.GetLibraryPerformanceCurvePage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems.value = data.totalItems;
                    isRunning = false;
                    if (!isNil(selectedPerformanceCurveLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedPerformanceCurveLibrary.value).then(()=>isShared = isSharedLibrary.value);
                    }
                }
            });  
        }
    }

    watch(selectedPerformanceEquations,()=>onSelectedPerformanceEquationsChanged())
    function onSelectedPerformanceEquationsChanged() {
        selectedPerformanceEquationIds = getPropertyValues('id', selectedPerformanceEquations.value) as string[];
    } 
    
    function onRemovePerformanceEquations() {
        deletionIds.value = deletionIds.value.concat(selectedPerformanceEquationIds);
        selectedPerformanceEquations.value = [];
        onPaginationChanged();
        modifiedStatus = " (Modified)";
    }    

    watch(statePerformanceCurveLibraries,()=>onStatePerformanceCurveLibrariesChanged())
    function onStatePerformanceCurveLibrariesChanged() {
        librarySelectItems.value = statePerformanceCurveLibraries.value.map(
            (library: PerformanceCurveLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   watch(librarySelectItemValue,()=>onLibrarySelectItemValueChangedCheckUnsaved())
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
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        scenarioLibraryIsModified = false;
        librarySelectItemValueAllowedChanged = true;
    }
    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        selectPerformanceCurveLibraryAction(librarySelectItemValue.value);
    }

    watch(stateSelectedPerformanceCurveLibrary,()=> onStateSelectedPerformanceCurveLibraryChanged())
    function onStateSelectedPerformanceCurveLibraryChanged() {
        selectedPerformanceCurveLibrary.value = clone(
            stateSelectedPerformanceCurveLibrary.value,
        );
    }

    watch(selectedPerformanceCurveLibrary,()=> onSelectedPerformanceCurveLibraryChanged())
    function onSelectedPerformanceCurveLibraryChanged() { 
        hasSelectedLibrary.value =
            selectedPerformanceCurveLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedPerformanceCurveLibrary.value.id, workType: WorkType.ImportLibraryPerformanceCurve}).then(response => {
                if(response.data){
                    setAlertMessageAction("A performance curve import has been added to the work queue")
                }
                else
                    setAlertMessageAction("");
            })
        }

        updatedRowsMap.clear();
        deletionIds.value = [];
        addedRows.value = [];
        isRunning = false;
        onPaginationChanged();
    }

    watch(stateNumericAttributes,()=>onStateNumericAttributesChanged())
    function onStateNumericAttributesChanged() {
        setAttributeSelectItems();
    }

    watch(stateScenarioPerformanceCurves,()=> onStateScenarioPerformanceCurvesChanged())
    function onStateScenarioPerformanceCurvesChanged() {
        if (
            hasScenario.value
        ) {
            onPaginationChanged();
        }
    }

    watch(deletionIds,()=>onDeletionIdsChanged())
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows,()=>onAddedRowsChanged())
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }
    watch(currentPage,()=>onCurrentPageChanged())
    function onCurrentPageChanged() {
        // Get parent name from library id
        librarySelectItems.value.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName.value = library.text;
            }
            if(parentLibraryName.value == ""){
                parentLibraryName.value = "None";
            }
        });
    }
    watch(isSharedLibrary,()=>onStateSharedAccessChanged())
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }
    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedPerformanceCurveLibrary.value, stateSelectedPerformanceCurveLibrary.value))
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function setAttributeSelectItems() {
        if (hasValue(stateNumericAttributes)) {
            attributeSelectItems = stateNumericAttributes.value.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess && checkUserIsLibraryOwner());
    }

    function checkUserIsLibraryOwner() {
        return getUserNameByIdGetter(selectedPerformanceCurveLibrary.value.owner) == getUserName();
    }

    function getOwnerUserName(): string {
        if (!hasCreatedLibrary) {
        return getUserNameByIdGetter(selectedPerformanceCurveLibrary.value.owner);
        }
        return getUserName();
    }

    function onShowCreatePerformanceCurveLibraryDialog(createAsNewLibrary: boolean) { 
        createPerformanceCurveLibraryDialogData.value = {
            showDialog: true,
            performanceCurves: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreatePerformanceCurveLibraryDialogResult(
        performanceCurveLibrary: PerformanceCurveLibrary,
    ) {
        createPerformanceCurveLibraryDialogData.value = clone(
            emptyCreatePerformanceLibraryDialogData,
        );

        if (!isNil(performanceCurveLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve> = {
                library: performanceCurveLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: performanceCurveLibrary.performanceCurves.length == 0 || !hasSelectedLibrary.value ? null : selectedPerformanceCurveLibrary.value.id,
                    rowsForDeletion: performanceCurveLibrary.performanceCurves.length == 0 ? [] : deletionIds.value,
                    updateRows: performanceCurveLibrary.performanceCurves.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: performanceCurveLibrary.performanceCurves.length == 0 ? [] : addedRows.value,
                    isModified: scenarioLibraryIsModified
                 },
                scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            PerformanceCurveService.UpsertPerformanceCurveLibrary(upsertRequest).then(() => {
                hasCreatedLibrary = true;
                if(!hasScenario.value)
                    librarySelectItemValue.value = performanceCurveLibrary.id;
                
                if(performanceCurveLibrary.performanceCurves.length == 0){
                    clearChanges();
                }

                performanceCurveLibraryMutator(performanceCurveLibrary);
                addSuccessNotificationAction({message:'Added deterioration model library'})
            })
        }
    }

    function onSubmitCreatePerformanceCurveDialogResult( 
        newPerformanceCurve: PerformanceCurve,
    ) {
        showCreatePerformanceCurveDialog.value = false;

        if (!isNil(newPerformanceCurve)) {
            addedRows.value = prepend(
                newPerformanceCurve,
                addedRows.value,
            );
            onPaginationChanged();
        }
    }

    function onEditPerformanceCurveProperty(id: string, property: string, value: any) {
        if (any(propEq('id', id), currentPage.value)) { 
            const performanceCurve: PerformanceCurve = find(
                propEq('id', id),
                currentPage.value,
            ) as PerformanceCurve;
            onUpdateRow(id, performanceCurve);
            onPaginationChanged();
        }
    }

    function onShowEquationEditorDialog(performanceCurveId: string) {
        selectedPerformanceCurve = find(
            propEq('id', performanceCurveId),
            currentPage.value,
        ) as PerformanceCurve;
        var selectedAttr = find(
            propEq('name', selectedPerformanceCurve.attribute),
            stateNumericAttributes.value,
        ) as Attribute;

        if (!isNil(selectedPerformanceCurve) && !isNil(selectedAttr)) {
            hasSelectedPerformanceCurve = true;

            equationEditorDialogData.value = {
                showDialog: true,
                equation: selectedPerformanceCurve.equation,
                isAscending: selectedAttr.isAscending
            };
        }
    }

    function onSubmitEquationEditorDialogResult(equation: Equation) {
        equationEditorDialogData.value = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && hasSelectedPerformanceCurve) {
            onUpdateRow(selectedPerformanceCurve.id, { ...selectedPerformanceCurve, equation: equation })
            currentPage.value = update(
                findIndex(
                    propEq('id', selectedPerformanceCurve.id),
                    currentPage.value,
                ),
                { ...selectedPerformanceCurve, equation: equation },
                currentPage.value,
            );
        }
        selectedPerformanceCurve = clone(emptyPerformanceCurve);
        hasSelectedPerformanceCurve = false;
    }

    function onEditPerformanceCurveCriterionLibrary(performanceCurveId: string) {
        selectedPerformanceCurve = find(
            propEq('id', performanceCurveId),
            currentPage.value,
        ) as PerformanceCurve;

        if (!isNil(selectedPerformanceCurve)) {
            hasSelectedPerformanceCurve = true;

            criterionEditorDialogData.value = {
                showDialog: true,
                CriteriaExpression: selectedPerformanceCurve.criterionLibrary.mergedCriteriaExpression
            };
        }
    }

    function onSubmitCriterionEditorDialogResult(criterionExpression: string) {
        criterionEditorDialogData.value = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression) && hasSelectedPerformanceCurve) {
            if(selectedPerformanceCurve.criterionLibrary.id === getBlankGuid())
                selectedPerformanceCurve.criterionLibrary.id = getNewGuid();
            onUpdateRow(selectedPerformanceCurve.id, { ...selectedPerformanceCurve, 
            criterionLibrary: {...selectedPerformanceCurve.criterionLibrary, mergedCriteriaExpression: criterionExpression} })
            currentPage.value = update(
                findIndex(
                    propEq('id', selectedPerformanceCurve.id),
                    currentPage.value,
                ),
                {
                    ...selectedPerformanceCurve,
                    criterionLibrary: {...selectedPerformanceCurve.criterionLibrary, mergedCriteriaExpression: criterionExpression},
                },
                currentPage.value,
            );
        }

        selectedPerformanceCurve = clone(emptyPerformanceCurve);
        hasSelectedPerformanceCurve = false;
    }

    function onRemovePerformanceCurve(performanceCurveId: string) {
        deletionIds.value.push(performanceCurveId);
        onPaginationChanged();
    }

    function onUpsertScenarioPerformanceCurves() {

        if (selectedPerformanceCurveLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        PerformanceCurveService.UpsertScenarioPerformanceCurves({
            libraryId: selectedPerformanceCurveLibrary.value.id === uuidNIL ? null : selectedPerformanceCurveLibrary.value.id,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: scenarioLibraryIsModified
        }, selectedScenarioId).then(async (response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
                clearChanges()
                performancePagination.value.page = 1;
                await onPaginationChanged();
                addSuccessNotificationAction({message: "Modified scenario's deterioration models"});
                librarySelectItemValue.value = null
            }           
        });
    }

    function onUpsertPerformanceCurveLibrary() { 
        const upsertRequest: LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve> = {
                library: selectedPerformanceCurveLibrary.value,
                isNewLibrary: false,
                 syncModel: {
                    libraryId: selectedPerformanceCurveLibrary.value.id === uuidNIL ? null : selectedPerformanceCurveLibrary.value.id,
                    rowsForDeletion: deletionIds.value,
                    updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: addedRows.value,
                    isModified: scenarioLibraryIsModified
                 },
                 scenarioId: null
        }
        PerformanceCurveService.UpsertPerformanceCurveLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges()
                performanceCurveLibraryMutator(selectedPerformanceCurveLibrary.value);
                selectedPerformanceCurveLibraryMutator(selectedPerformanceCurveLibrary.value.id);
                addSuccessNotificationAction({message: "Updated deterioration model library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario.value) {
                deletionIds.value = [];
                addedRows.value = [];
                updatedRowsMap.clear();
                resetPage();
            }
        });
        parentLibraryName.value = loadedParentName;
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
            deletePerformanceCurveLibraryAction(
                selectedPerformanceCurveLibrary.value.id,
            );
        }
    }

    function disableCrudButtons() {
        const rowChanges = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rowChanges.every(
            (performanceCurve: PerformanceCurve) => {
                return (
                    rules['generalRules'].valueIsNotEmpty(
                        performanceCurve.name,
                    ) === true &&
                    rules['generalRules'].valueIsNotEmpty(
                        performanceCurve.attribute,
                    ) === true
                );
            },
        );

        if (hasSelectedLibrary.value) {
            return !(
                rules['generalRules'].valueIsNotEmpty(
                    selectedPerformanceCurveLibrary.value.name,
                ) === true &&
                dataIsValid);
        }

        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    function OnDownloadTemplateClick()
    {
        PerformanceCurveService.downloadPerformanceCurvesTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    function exportPerformanceCurves() {
        const id: string = hasScenario.value ? selectedScenarioId : selectedPerformanceCurveLibrary.value.id;
                PerformanceCurveService.exportPerformanceCurves(id, hasScenario.value)
                    .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                            const fileInfo: FileInfo = response.data as FileInfo;
                            FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                    });
    }

    function onSubmitImportExportPerformanceCurvesDialogResult(result: ImportExportPerformanceCurvesDialogResult) {
        showImportExportPerformanceCurvesDialog.value = false;

        if (hasValue(result)) {
            if (result.isExport) {

            }
            else
            if (hasValue(result.file)) {
                const data: PerformanceCurvesFileImport = {
                    file: result.file
                };

                if (hasScenario.value) {
                    importScenarioPerformanceCurvesFileAction({
                        ...data,
                        id: selectedScenarioId,
                        currentUserCriteriaFilter: currentUserCriteriaFilter
                    }).then(() => {
                    });
                } else {
                    importLibraryPerformanceCurvesFileAction({
                        ...data,
                        id: selectedPerformanceCurveLibrary.value.id,
                        currentUserCriteriaFilter: currentUserCriteriaFilter
                    }).then(() => {
                    });
                }

            }
        }
    }
    function onShowSharePerformanceCurveLibraryDialog(performanceCurveLibrary: PerformanceCurveLibrary)
    {
        sharePerformanceCurveLibraryDialogData.value =
        {
            showDialog: true,
            performanceCurveLibrary: clone(performanceCurveLibrary),
        };
    }
    function onSharePerformanceCurveLibraryDialogSubmit(performanceCurveLibraryUsers: PerformanceCurveLibraryUser[]) {
        sharePerformanceCurveLibraryDialogData.value = clone(emptySharePerformanceCurveLibraryDialogData);

        if (!isNil(performanceCurveLibraryUsers) && selectedPerformanceCurveLibrary.value.id !== getBlankGuid())
        {
            let libraryUserData: LibraryUser[] = [];

            //create library users
            performanceCurveLibraryUsers.forEach((performanceCurveLibraryUser, index) =>
            {   
                //determine access level
                let libraryUserAccessLevel: number = 0;
                if (libraryUserAccessLevel == 0 && performanceCurveLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                if (libraryUserAccessLevel == 0 && performanceCurveLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                //create library user object
                let libraryUser: LibraryUser = {
                    userId: performanceCurveLibraryUser.userId,
                    userName: performanceCurveLibraryUser.username,
                    accessLevel: libraryUserAccessLevel
                }

                //add library user to an array
                libraryUserData.push(libraryUser);
            });
            if (!isNil(selectedPerformanceCurveLibrary.value.id) ) {
                getIsSharedLibraryAction(selectedPerformanceCurveLibrary.value).then(()=> isShared = isSharedLibrary.value);
            }
            //update performance curve library sharing
            PerformanceCurveService.upsertOrDeletePerformanceCurveLibraryUsers(selectedPerformanceCurveLibrary.value.id, libraryUserData).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                {
                    resetPage();
                }
            });
        }
    }
    function onSearchClick(){
        currentSearch = gridSearchTerm.value;
        resetPage();
    }

    function onClearClick(){
        gridSearchTerm.value = '';
        onSearchClick();
    }

    function onUpdateRow(rowId: string, updatedRow: PerformanceCurve){
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
        performancePagination.value.page = 1;
        onPaginationChanged();
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

    function setParentLibraryName(libraryId: string) {
         if (libraryId === "None" || libraryId === uuidNIL) {
            parentLibraryName.value = "None";
            return;
        }
        let foundLibrary: PerformanceCurveLibrary = emptyPerformanceCurveLibrary;
        statePerformanceCurveLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioPerformanceCurve && importComp.id === selectedScenarioId ||
            hasSelectedLibrary.value && importComp.workType === WorkType.ImportLibraryPerformanceCurve && importComp.id === selectedPerformanceCurveLibrary.value.id){
            clearChanges()
            performancePagination.value.page = 1
            onPaginationChanged().then(() => {
                setAlertMessageAction('');
            })
        }        
    }

    async function initializePages(){
        const request: PagingRequest<PerformanceCurve>= {
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
        if((!hasSelectedLibrary.value || hasScenario.value) && selectedScenarioId !== uuidNIL){
            let response = await PerformanceCurveService.getPerformanceCurvePage(selectedScenarioId, request);
            isRunning = false
            if(response.data) {
                let data = response.data as PagingPage<PerformanceCurve>;
                currentPage.value = data.items;
                rowCache = clone(currentPage.value)
                totalItems.value = data.totalItems;
                let currentPageLength = currentPage.value.length;
                setParentLibraryName(currentPageLength > 0 ? currentPage.value[0].libraryId : "None");
                loadedParentId = currentPageLength > 0 ? currentPage.value[0].libraryId : "";
                loadedParentName = parentLibraryName.value; //store original
                scenarioLibraryIsModified = currentPageLength > 0 ? currentPage.value[0].isModified : false;
            
            }
        }
    }
</script>

<style>
.equation-name-text-field-output {
    margin-left: 10px;
}

.attribute-text-field-output {
    margin-left: 15px;
}

.header-height {
    height: 45px;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
