<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout column>
                <v-layout justify-left style="height:96px">
                    <v-flex xs5>
                        <v-subheader class="ghd-control-label ghd-md-gray">Deterioration Model Library</v-subheader>
                        <v-select
                            id="PerformanceCurveEditor-library-select"
                            class="ghd-control-border ghd-control-text ghd-select"
                            :items="librarySelectItems"
                            append-icon=$vuetify.icons.ghd-down
                            outline
                            v-model="librarySelectItemValue"
                        >
                            <template v-slot:selection="{ item }">
                                <span class="ghd-control-text">{{ item.text }}</span>
                            </template>
                            <template v-slot:item="{ item }">
                                <v-list-item v-on="on" v-bind="attrs">
                                    <v-list-item-title>
                                    <v-row no-gutters align="center">
                                    <span>{{ item.text }}</span>
                                    </v-row>
                                    </v-list-item-title>
                                </v-list-item>
                            </template>
                        </v-select>
                        <div class="ghd-md-gray ghd-control-subheader budget-parent" v-if="hasScenario"><b>Library Used: {{parentLibraryName}} 
                            
                            <span v-if="scenarioLibraryIsModified">&nbsp;&nbsp;{{modifiedStatus}}</span></b>
                        
                        </div>

                    </v-flex>
                    <v-flex xs2 v-show="hasScenario"></v-flex>
                    <v-flex xs5 v-show="hasSelectedLibrary || hasScenario">                     
                        <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                        <v-layout>
                        
                        <v-text-field
                            id="PerformanceCurveEditor-searchDeteriorationEquations-textField"
                            class="ghd-text-field-border ghd-text-field search-icon-general"
                            style="margin-top:0px;"
                            prepend-inner-icon=$vuetify.icons.ghd-search
                            hide-details
                            label="Search Deterioration Equations"
                            placeholder="Search Deterioration Equations"
                            single-line
                            outline
                            clearable
                            @click:clear="onClearClick()"
                            v-model="gridSearchTerm"
                        >
                        </v-text-field>
                        <v-btn id="PerformanceCurveEditor-search-button" style="margin-top: 2px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline @click="onSearchClick()">Search</v-btn>
                        </v-layout>
                    </v-flex>
                    <v-flex xs5 v-show="!(hasSelectedLibrary || hasScenario)">
                    </v-flex>                    
                    <v-flex xs2 v-show='!hasScenario'>
                        <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                        <v-layout row align-end justify-end>
                            <v-btn
                                id="PerformanceCurveEditor-createNewLibrary-button"
                                @click='onShowCreatePerformanceCurveLibraryDialog(false)'
                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                                outline>
                                Create New Library
                            </v-btn>
                        </v-layout>
                    </v-flex>                    
                </v-layout>
            </v-layout>            
        </v-flex>
        <v-flex>
            <v-layout row style="height:48px;">
                <v-flex xs9 v-show="!hasScenario">
                    <v-layout row>
                            <div style="margin-top:6px;"
                                v-if='hasSelectedLibrary && !hasScenario'
                                class="ghd-control-label ghd-md-gray"
                            > 
                                Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ modifiedDate }}
                            <v-badge v-show="isShared">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                            </v-badge>
                            <v-btn
                                id="PerformanceCurveEditor-shareLibrary-button"
                                @click='onShowSharePerformanceCurveLibraryDialog(selectedPerformanceCurveLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                                v-show='!hasScenario'>
                                Share Library
                            </v-btn>
                            </div>
                    </v-layout>
                </v-flex>
                <v-flex xs9 v-show="hasScenario">
                </v-flex>
                <v-flex xs2 v-show="hasScenario || hasSelectedLibrary">
                    <v-layout row align-end style="margin-top:-4px;height:40px;">
                        <v-btn
                            id="PerformanceCurveEditor-upload-button"
                            :disabled='false' @click='showImportExportPerformanceCurvesDialog = true'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Upload
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                        </v-divider>
                        <v-btn
                            id="PerformanceCurveEditor-download-button"
                            :disabled='false' @click='exportPerformanceCurves()'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                        </v-divider>
                        <v-btn
                            id="PerformanceCurveEditor-downloadTemplate-button"
                            :disabled='false' @click='OnDownloadTemplateClick()'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download Template
                        </v-btn>
                    </v-layout>            
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout class="data-table" justify-left>
                <v-flex xs12>
                    <v-card class="elevation-0">
                        <v-data-table
                            id="PerformanceCurveEditor-deteriorationModels-datatable"
                            :headers="performanceCurveGridHeaders"
                            :items="currentPage"                       
                            :pagination.sync="performancePagination"
                            :total-items="totalItems"
                            :rows-per-page-items=[5,10,25]
                            sort-icon=$vuetify.icons.ghd-table-sort
                            select-all
                            v-model='selectedPerformanceEquations'
                            class="fixed-header ghd-table v-table__overflow"
                            item-key="id"
                        >
                            <template slot="items" slot-scope="props" v-slot:items="props">
                                <td>
                                    <v-checkbox id="PerformanceCurveEditor-deleteModel-vcheckbox" class="ghd-checkbox"
                                        hide-details
                                        primary
                                        v-model='props.selected'
                                    >
                                    </v-checkbox>
                                </td>                                
                                <td class="text-xs-left">
                                    <v-edit-dialog
                                        :return-value.sync="props.item.name"
                                        @save="
                                            onEditPerformanceCurveProperty(
                                                props.item.id,
                                                'name',
                                                props.item.name,
                                            )
                                        "
                                        large
                                        lazy
                                    >
                                        <v-text-field
                                            readonly
                                            single-line
                                            class="sm-txt equation-name-text-field-output"
                                            :value="props.item.name"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <template slot="input">
                                            <v-text-field
                                                label="Edit"
                                                single-line
                                                v-model="props.item.name"
                                                :rules="[
                                                    rules['generalRules']
                                                        .valueIsNotEmpty,
                                                ]"
                                            />
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-left">
                                    <v-edit-dialog
                                        :return-value.sync="
                                            props.item.attribute
                                        "
                                        @save="
                                            onEditPerformanceCurveProperty(
                                                props.item.id,
                                                'attribute',
                                                props.item.attribute,
                                            )
                                        "
                                        large
                                        lazy
                                    >
                                        <v-text-field
                                            readonly
                                            single-line
                                            class="sm-txt attribute-text-field-output"
                                            :value="props.item.attribute"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <template slot="input">
                                            <v-select
                                                :items="attributeSelectItems"
                                                append-icon=$vuetify.icons.ghd-down
                                                label="Edit"
                                                v-model="props.item.attribute"
                                                :rules="[
                                                    rules['generalRules']
                                                        .valueIsNotEmpty,
                                                ]"
                                            />
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class="text-xs-left">
                                    <v-menu
                                        left
                                        min-height="500px"
                                        min-width="500px"
                                        v-show="
                                            props.item.equation.expression !==
                                                ''
                                        "
                                    >
                                        <template slot="activator">
                                            <v-btn id="PerformanceCurveEditor-checkEquationEye-vbtn" class="ghd-blue" icon>
                                                <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')">
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    id="PerformanceCurveEditor-checkEquation-vtextarea"
                                                    class="sm-txt Montserrat-font-family"
                                                    :value="
                                                        props.item.equation
                                                            .expression
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
                                    <v-btn id="PerformanceCurveEditor-editEquation-vbtn"
                                        @click="
                                            onShowEquationEditorDialog(
                                                props.item.id,
                                            )
                                        "
                                        class="ghd-blue"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')">
                                    </v-btn>
                                </td>
                                <td class="text-xs-left">
                                    <v-menu
                                        min-height="500px"
                                        min-width="500px"
                                        right
                                        v-show="
                                            props.item.criterionLibrary
                                                .mergedCriteriaExpression !== ''
                                        "
                                    >
                                        <template slot="activator">
                                            <v-btn id="PerformanceCurveEditor-checkCriteriaEye-vbtn" class="ghd-blue" flat icon>
                                                <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')">
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    id="PerformanceCurveEditor-checkCriteria-vtextarea"
                                                    class="sm-txt Montserrat-font-family"
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
                                    <v-btn id="PerformanceCurveEditor-editCriteria-vbtn"
                                        @click="
                                            onEditPerformanceCurveCriterionLibrary(
                                                props.item.id,
                                            )
                                        "
                                        class="ghd-blue"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')">
                                    </v-btn>
                                </td>
                                <td class="text-xs-left">
                                    <v-btn id="PerformanceCurveEditor-deleteModel-vbtn"
                                        @click="
                                            onRemovePerformanceCurve(
                                                props.item.id,
                                            )
                                        "
                                        class="ghd-blue"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </td>
                            </template>
                            <template v-slot:body.append>
                            <v-btn>Append button</v-btn>
                            </template>                               
                        </v-data-table>
                        <v-btn style="margin-top:-84px"
                            id="PerformanceCurveEditor-deleteSelected-button"
                            :disabled='selectedPerformanceEquationIds.length === 0 || (!hasLibraryEditPermission && !hasScenario)'
                            @click='onRemovePerformanceEquations'
                            class='ghd-blue' flat
                        >
                            Delete Selected
                        </v-btn>                        
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
            <v-layout class="header-height" justify-left v-show="hasSelectedLibrary || hasScenario">
                <v-flex xs3>
                    <v-btn
                        id="PerformanceCurveEditor-addDeteriorationModel-button"
                        @click="showCreatePerformanceCurveDialog = true"
                        class="ghd-blue ghd-white-bg ghd-button-text ghd-button-border ghd-outline-button-padding"
                        depressed                
                        outlined
                    >
                        Add Deterioration Model
                    </v-btn>
                </v-flex>
            </v-layout>        
        <v-divider v-show="hasSelectedLibrary || hasScenario"></v-divider>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex xs12>
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>                    
                    <v-textarea
                        class="ghd-control-text ghd-control-border"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedPerformanceCurveLibrary.description"
                        @input='checkHasUnsavedChanges()'
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout
                justify-center
                row
                v-show='hasSelectedLibrary || hasScenario'
            >
                <v-btn
                    id="PerformanceCurveEditor-cancel-button"
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    @click="onDiscardChanges"
                    class="ghd-white-bg ghd-blue ghd-button-text"
                    depressed
                    v-show="hasScenario"
                >
                    Cancel
                </v-btn>
                <v-btn outline
                    id="PerformanceCurveEditor-deleteLibrary-button"
                    @click="onShowConfirmDeleteAlert"
                    class="ghd-white-bg ghd-blue ghd-button-text"
                    depressed
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                >
                    Delete Library
                </v-btn>                
                <v-btn
                    id="PerformanceCurveEditor-createAsNewLibrary-button"
                    :disabled="disableCrudButtons()"
                    @click="onShowCreatePerformanceCurveLibraryDialog(true)"
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    outline                  
                >
                    Create as New Library
                </v-btn>
               <v-btn
                    id="PerformanceCurveEditor-updateLibrary-button"
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                    @click='onUpsertPerformanceCurveLibrary'
                    class="ghd-blue-bg ghd-white ghd-button-text ghd-button-border ghd-outline-button-padding"
                    depressed
                    outlined
                    v-show='!hasScenario'
                >
                    Update Library
                </v-btn>
                <v-btn
                    id="PerformanceCurveEditor-save-button"
                    :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
                    @click='onUpsertScenarioPerformanceCurves'
                    class="ghd-blue-bg ghd-white ghd-button-text"
                    depressed
                    v-show='hasScenario'
                >
                    Save
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
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
    </v-layout>
</template>

<script  lang="ts" setup>
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
import { isNullOrUndefined } from 'util';
import { Hub } from '@/connectionHub';
import ScenarioService from '@/services/scenario.service';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref, shallowRef, ShallowRef} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
let store = useStore();

let statePerformanceCurveLibraries = ref<PerformanceCurveLibrary[]>(store.state.performanceCurveModule.performanceCurveLibraries);
let stateSelectedPerformanceCurveLibrary = ref<PerformanceCurveLibrary>(store.state.performanceCurveModule.selectedPerformanceCurveLibrary);
let stateScenarioPerformanceCurves = ref<PerformanceCurveLibrary[]>(store.state.performanceCurveModule.scenarioPerformanceCurves);
let stateNumericAttributes = ref<Attribute[]>(store.state.attributeModule.numericAttributes);
let hasUnsavedChanges = ref<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess);
let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter);
let hasPermittedAccess = ref<boolean>(store.state.performanceCurveModule.hasPermittedAccess);
let isSharedLibrary = ref<boolean>(store.state.performanceCurveModule.isSharedLibrary);

async function getHasPermittedAccessAction(payload?: any): Promise<any> {await store.dispatch('getHasPermittedAccess');}
async function getIsSharedLibraryAction(payload?: any): Promise<any> {await store.dispatch('getIsSharedPerformanceCurveLibrary');}
async function getPerformanceCurveLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getPerformanceCurveLibraries');}
async function selectPerformanceCurveLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectPerformanceCurveLibrary');}
async function deletePerformanceCurveLibraryAction(payload?: any): Promise<any> {await store.dispatch('deletePerformanceCurveLibrary');}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
async function updatePerformanceCurveCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('updatePerformanceCurvesCriterionLibraries');}
async function upsertOrDeletePerformanceCurveLibraryUsersAction(payload?: any): Promise<any> {await store.dispatch('upsertOrDeletePerformanceCurveLibraryUsers');}
async function importScenarioPerformanceCurvesFileAction(payload?: any): Promise<any> {await store.dispatch('importScenarioPerformanceCurvesFile');}
async function importLibraryPerformanceCurvesFileAction(payload?: any): Promise<any> {await store.dispatch('importLibraryPerformanceCurvesFileAction');}
async function addSuccessNotificationAction(payload?: any): Promise<any> {await store.dispatch('addSuccessNotification');}
async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {await store.dispatch('getCurrentUserOrSharedScenario');}
async function selectScenarioAction(payload?: any): Promise<any> {await store.dispatch('selectScenario');}
async function setAlertMessageAction(payload?: any): Promise<any> {await store.dispatch('setAlertMessage');}

let getUserNameByIdGetter: any = store.getters.getUserNameById
function performanceCurveLibraryMutator(payload:any){store.commit('performanceCurveLibraryMutator');}
function selectedPerformanceCurveLibraryMutator(payload:any){store.commit('selectedPerformanceCurveLibraryMutator');}

    let addedRows: ShallowRef<PerformanceCurve[]> = ref([]);
    let updatedRowsMap:Map<string, [PerformanceCurve, PerformanceCurve]> = new Map<string, [PerformanceCurve, PerformanceCurve]>();//0: original value | 1: updated value
    let deletionIds: ShallowRef<string[]> = ref([]);
    let rowCache: PerformanceCurve[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let selectedPerformanceCurveLibrary: ShallowRef<PerformanceCurveLibrary> = shallowRef(clone(
        emptyPerformanceCurveLibrary,
    ));
    let performancePagination: ShallowRef<Pagination> = shallowRef(clone(emptyPagination));
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: ShallowRef<PerformanceCurve[]> = ref([]);
    let isRunning: boolean = true;
    let isShared: boolean = false;
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary: boolean = false;
    let hasScenario: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let modifiedDate: string; 
    
    let performanceCurveGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '',
        },
        {
            text: 'Equation',
            value: 'equation',
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
            width: '',
        },
        {
            text: 'Actions',
            value: '',
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
    const $statusHub = inject('$statusHub') as any
    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = ''
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: Ref<string | null> = ref(null);

    let selectedPerformanceEquations: ShallowRef<PerformanceCurve[]> = ref([]);
    let selectedPerformanceEquationIds: string[] = [];

    let createPerformanceCurveLibraryDialogData: CreatePerformanceCurveLibraryDialogData = clone(
        emptyCreatePerformanceLibraryDialogData,
    );
    let equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let showCreatePerformanceCurveDialog = false;
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let rules: InputValidationRules = validationRules;
    let uuidNIL: string = getBlankGuid();
    let currentUrl: string = window.location.href;
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean = false;
    let showImportExportPerformanceCurvesDialog: boolean = false;    

    let sharePerformanceCurveLibraryDialogData: SharePerformanceCurveLibraryDialogData = clone(emptySharePerformanceCurveLibraryDialogData);

    let parentLibraryName: string = "None";
    let parentLibraryId: string = "";
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string = "";
    let newLibrarySelection: boolean = false;
    let modifiedStatus : string = "";

    beforeRouteEnter();
    function beforeRouteEnter() {
        (() => {
            librarySelectItemValue.value= null;           
            getPerformanceCurveLibrariesAction().then(() => {
                getHasPermittedAccessAction().then(() => {
                    if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.PerformanceCurve) !== -1) {
                        selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;

                        if (selectedScenarioId === uuidNIL) {
                            addErrorNotificationAction({
                                message: 'Unable to identify selected scenario.',
                            });
                            $router.push('/Scenarios/');
                        }

                        hasScenario = true;
                        ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioPerformanceCurve}).then(response => {
                            if(response.data){
                                setAlertMessageAction("A performance curve import has been added to the work queue")
                            }
                            initializePages().then(() =>{
                                hasScenario = true;
                                getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                                    selectScenarioAction({ scenarioId: selectedScenarioId });        
                            });
                        });
                            
                        })

                    }

                    
                });
            });          
        });
    }
    onMounted(()=>mounted())
    function mounted() {
        setAttributeSelectItems();
        
        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }
    onBeforeUnmount(()=>beforeDestroy())
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });

        $statusHub.$off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );

        setAlertMessageAction('');
    }

    watch(performancePagination,()=> onPaginationChanged())
    async function onPaginationChanged() {
        if(isRunning)
            return;
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = performancePagination.value;
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
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: currentSearch
        };
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL){
            isRunning = true;
            await PerformanceCurveService.getPerformanceCurvePage(selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    isRunning = false;
                }
            });
        }          
        else if(hasSelectedLibrary){
            isRunning = true;

            await PerformanceCurveService.getPerformanceLibraryModifiedDate(selectedPerformanceCurveLibrary.value.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate = data.slice(0, 10);
                   }
             });

            await PerformanceCurveService.GetLibraryPerformanceCurvePage(librarySelectItemValue.value !== null ? librarySelectItemValue.value : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    currentPage.value = data.items;
                    rowCache = clone(currentPage.value)
                    totalItems = data.totalItems;
                    isRunning = false;
                    if (!isNullOrUndefined(selectedPerformanceCurveLibrary.value.id) ) {
                        getIsSharedLibraryAction(selectedPerformanceCurveLibrary).then(()=>isShared = isSharedLibrary.value);
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
        librarySelectItems = statePerformanceCurveLibraries.value.map(
            (library: PerformanceCurveLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   watch(librarySelectItemValue,()=>onLibrarySelectItemValueChangedCheckUnsaved())
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
        parentLibraryId = librarySelectItemValue.value ? librarySelectItemValue.value : "";
        newLibrarySelection = true;
        scenarioLibraryIsModified = false;
        librarySelectItemValueAllowedChanged = true;
    }
    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value
        selectPerformanceCurveLibraryAction(librarySelectItemValue);
    }

    watch(stateSelectedPerformanceCurveLibrary,()=> onStateSelectedPerformanceCurveLibraryChanged())
    function onStateSelectedPerformanceCurveLibraryChanged() {
        selectedPerformanceCurveLibrary.value = clone(
            stateSelectedPerformanceCurveLibrary.value,
        );
    }

    watch(selectedPerformanceCurveLibrary,()=> onSelectedPerformanceCurveLibraryChanged())
    function onSelectedPerformanceCurveLibraryChanged() { 
        hasSelectedLibrary =
            selectedPerformanceCurveLibrary.value.id !== uuidNIL;

        if (hasSelectedLibrary) {
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
            hasScenario
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
        librarySelectItems.forEach(library => {
            if (library.value === parentLibraryId) {
                parentLibraryName = library.text;
            }
            if(parentLibraryName == ""){
                parentLibraryName = "None";
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
            updatedRowsMap.size > 0 || (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', selectedPerformanceCurveLibrary, stateSelectedPerformanceCurveLibrary))
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
        createPerformanceCurveLibraryDialogData = {
            showDialog: true,
            performanceCurves: createAsNewLibrary
                ? currentPage.value
                : [],
        };
    }

    function onSubmitCreatePerformanceCurveLibraryDialogResult(
        performanceCurveLibrary: PerformanceCurveLibrary,
    ) {
        createPerformanceCurveLibraryDialogData = clone(
            emptyCreatePerformanceLibraryDialogData,
        );

        if (!isNil(performanceCurveLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve> = {
                library: performanceCurveLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: performanceCurveLibrary.performanceCurves.length == 0 || !hasSelectedLibrary ? null : selectedPerformanceCurveLibrary.value.id,
                    rowsForDeletion: performanceCurveLibrary.performanceCurves.length == 0 ? [] : deletionIds.value,
                    updateRows: performanceCurveLibrary.performanceCurves.length == 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: performanceCurveLibrary.performanceCurves.length == 0 ? [] : addedRows.value,
                    isModified: scenarioLibraryIsModified
                 },
                scenarioId: hasScenario ? selectedScenarioId : null
            }
            PerformanceCurveService.UpsertPerformanceCurveLibrary(upsertRequest).then(() => {
                hasCreatedLibrary = true;
                librarySelectItemValue.value = performanceCurveLibrary.id;
                
                if(performanceCurveLibrary.performanceCurves.length == 0){
                    clearChanges();
                }

                performanceCurveLibraryMutator(performanceCurveLibrary);
                selectedPerformanceCurveLibraryMutator(performanceCurveLibrary.id);
                addSuccessNotificationAction({message:'Added deterioration model library'})
            })
        }
    }

    function onSubmitCreatePerformanceCurveDialogResult( 
        newPerformanceCurve: PerformanceCurve,
    ) {
        showCreatePerformanceCurveDialog = false;

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

        if (!isNil(selectedPerformanceCurve)) {
            hasSelectedPerformanceCurve = true;

            equationEditorDialogData = {
                showDialog: true,
                equation: selectedPerformanceCurve.equation,
            };
        }
    }

    function onSubmitEquationEditorDialogResult(equation: Equation) {
        equationEditorDialogData = clone(emptyEquationEditorDialogData);

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

            criterionEditorDialogData = {
                showDialog: true,
                CriteriaExpression: selectedPerformanceCurve.criterionLibrary.mergedCriteriaExpression
            };
        }
    }

    function onSubmitCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        criterionEditorDialogData = clone(
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

        if (selectedPerformanceCurveLibrary.value.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
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
                performanceCurveLibraryMutator(selectedPerformanceCurveLibrary);
                selectedPerformanceCurveLibraryMutator(selectedPerformanceCurveLibrary.value.id);
                addSuccessNotificationAction({message: "Updated deterioration model library",});
            }
        });
    }

    function onDiscardChanges() {
        librarySelectItemValue.value = null;
        setTimeout(() => {
            if (hasScenario) {
                deletionIds.value = [];
                addedRows.value = [];
                updatedRowsMap.clear();
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

        if (hasSelectedLibrary) {
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
        const id: string = hasScenario ? selectedScenarioId : selectedPerformanceCurveLibrary.value.id;
                PerformanceCurveService.exportPerformanceCurves(id, hasScenario)
                    .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                            const fileInfo: FileInfo = response.data as FileInfo;
                            FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                    });
    }

    function onSubmitImportExportPerformanceCurvesDialogResult(result: ImportExportPerformanceCurvesDialogResult) {
        showImportExportPerformanceCurvesDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {

            }
            else
            if (hasValue(result.file)) {
                const data: PerformanceCurvesFileImport = {
                    file: result.file
                };

                if (hasScenario) {
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
        sharePerformanceCurveLibraryDialogData =
        {
            showDialog: true,
            performanceCurveLibrary: clone(performanceCurveLibrary),
        };
    }
    function onSharePerformanceCurveLibraryDialogSubmit(performanceCurveLibraryUsers: PerformanceCurveLibraryUser[]) {
        sharePerformanceCurveLibraryDialogData = clone(emptySharePerformanceCurveLibraryDialogData);

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
            if (!isNullOrUndefined(selectedPerformanceCurveLibrary.value.id) ) {
                getIsSharedLibraryAction(selectedPerformanceCurveLibrary).then(()=> isShared = isSharedLibrary.value);
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
        currentSearch = gridSearchTerm;
        resetPage();
    }

    function onClearClick(){
        gridSearchTerm = '';
        onSearchClick();
    }

    function onUpdateRow(rowId: string, updatedRow: PerformanceCurve){
        if(any(propEq('id', rowId), addedRows.value))
            return;

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

    function setParentLibraryName(libraryId: string) {
         if (libraryId === "None") {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: PerformanceCurveLibrary = emptyPerformanceCurveLibrary;
        statePerformanceCurveLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioPerformanceCurve && importComp.id === selectedScenarioId ||
            hasSelectedLibrary && importComp.workType === WorkType.ImportLibraryPerformanceCurve && importComp.id === selectedPerformanceCurveLibrary.value.id){
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
        if((!hasSelectedLibrary || hasScenario) && selectedScenarioId !== uuidNIL)
            await PerformanceCurveService.getPerformanceCurvePage(selectedScenarioId, request).then(response => {
                isRunning = false
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
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
