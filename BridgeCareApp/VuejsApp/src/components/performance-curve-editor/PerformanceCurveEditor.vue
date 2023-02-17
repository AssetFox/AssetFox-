<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout column>
                <v-layout justify-left style="height:96px">
                    <v-flex xs5>
                        <v-subheader class="ghd-control-label ghd-md-gray">Deterioration Model Library</v-subheader>
                        <v-select
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
                                <v-list-item-content>
                                    <v-list-item-title>
                                    <v-row no-gutters align="center">
                                    <span>{{ item.text }}</span>
                                    </v-row>
                                    </v-list-item-title>
                                </v-list-item-content>
                                </v-list-item>
                            </template>
                        </v-select>
                    </v-flex>
                    <v-flex xs2 v-show="hasScenario"></v-flex>
                    <v-flex xs5 v-show="hasSelectedLibrary || hasScenario">                     
                        <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                        <v-layout>
                        
                        <v-text-field
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
                        <v-btn style="margin-top: 2px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline @click="onSearchClick()">Search</v-btn>
                        </v-layout>
                    </v-flex>
                    <v-flex xs5 v-show="!(hasSelectedLibrary || hasScenario)">
                    </v-flex>                    
                    <v-flex xs2 v-show='!hasScenario'>
                        <v-subheader class="ghd-control-label ghd-md-gray"> </v-subheader>
                        <v-layout row align-end>
                            <v-btn @click='onShowCreatePerformanceCurveLibraryDialog(false)'
                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                                style="margin-top:0px;"
                                outline                               
                            >
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
                                Owner: {{ getOwnerUserName() || '[ No Owner ]' }} |
                            <v-badge v-show="isShared">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                            </v-badge>
                            <v-btn @click='onShowSharePerformanceCurveLibraryDialog(selectedPerformanceCurveLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                                v-show='!hasScenario'>
                                Share Library
                            </v-btn>
                            </div>
                            <!-- <v-divider v-if='hasSelectedLibrary && !hasScenario' class="owner-shared-divider" style="margin-left:10px;" inset vertical>
                            </v-divider>      -->                  
                            <!-- <v-switch style="margin-left:10px;margin-top:4px;"
                                class="sharing ghd-checkbox"
                                label="Shared"
                                v-if="hasSelectedLibrary && !hasScenario"
                                v-model="selectedPerformanceCurveLibrary.isShared"
                                @change="checkHasUnsavedChanges()"
                            />                -->
                    </v-layout>
                </v-flex>
                <v-flex xs9 v-show="hasScenario">
                </v-flex>
                <v-flex xs2 v-show="hasScenario || hasSelectedLibrary">
                    <v-layout row align-end style="margin-top:-4px;height:40px;">
                        <v-btn :disabled='false' @click='showImportExportPerformanceCurvesDialog = true'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Upload
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                        </v-divider>
                        <v-btn :disabled='false' @click='exportPerformanceCurves()'
                            flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                            Download
                        </v-btn>
                        <v-divider class="upload-download-divider" inset vertical>
                        </v-divider>
                        <v-btn :disabled='false' @click='OnDownloadTemplateClick()'
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
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-checkbox class="ghd-checkbox"
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
                                        persistent
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
                                        persistent
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
                                            <v-btn class="ghd-blue" icon>
                                                <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')">
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
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
                                    <v-btn
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
                                            <v-btn class="ghd-blue" flat icon>
                                                <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')">
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
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
                                    <v-btn
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
                                    <v-btn
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
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    @click="onDiscardChanges"
                    class="ghd-white-bg ghd-blue ghd-button-text"
                    depressed
                    v-show="hasScenario"
                >
                    Cancel
                </v-btn>
                <v-btn
                    @click="onShowConfirmDeleteAlert"
                    class="ghd-white-bg ghd-blue ghd-button-text"
                    depressed
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                >
                    Delete Library
                </v-btn>                
                <v-btn
                    :disabled="disableCrudButtons()"
                    @click="onShowCreatePerformanceCurveLibraryDialog(true)"
                    class="ghd-blue ghd-white-bg ghd-button-text ghd-button-border ghd-outline-button-padding"
                    depressed                    
                    outlined
                >
                    Create as New Library
                </v-btn>
               <v-btn
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                    @click='onUpsertPerformanceCurveLibrary'
                    class="ghd-blue-bg ghd-white ghd-button-text ghd-button-border ghd-outline-button-padding"
                    depressed
                    outlined
                    v-show='!hasScenario'
                >
                    Update Library
                </v-btn>
                <v-btn :disabled='disableCrudButtonsResult || !hasUnsavedChanges'
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

<script lang="ts">
import Vue from 'vue';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State, Getter, Mutation } from 'vuex-class';
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
    contains,
    find,
    findIndex,
    isNil,
    propEq,
    reject,
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
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { LibraryUser } from '@/shared/models/iAM/user';
import { PerformanceCurveLibraryUser } from '@/shared/models/iAM/performance.ts';
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

@Component({
    components: {
        ImportExportPerformanceCurvesDialog,
        CreatePerformanceCurveLibraryDialog,
        CreatePerformanceCurveDialog,
        EquationEditorDialog,
        GeneralCriterionEditorDialog,
        ConfirmDeleteAlert: Alert,
        SharePerformanceCurveLibraryDialog,
    },
})
export default class PerformanceCurveEditor extends Vue {
    @State(state => state.performanceCurveModule.performanceCurveLibraries)
    statePerformanceCurveLibraries: PerformanceCurveLibrary[];
    @State(
        state => state.performanceCurveModule.selectedPerformanceCurveLibrary,
    )
    stateSelectedPerformanceCurveLibrary: PerformanceCurveLibrary;
    @State(state => state.performanceCurveModule.scenarioPerformanceCurves)
    stateScenarioPerformanceCurves: PerformanceCurve[];
    @State(state => state.attributeModule.numericAttributes)
    stateNumericAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;
    @State(state => state.performanceCurveModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @State(state => state.performanceCurveModule.isSharedLibrary) isSharedLibrary: boolean;
    @Action('getIsSharedPerformanceCurveLibrary') getIsSharedLibraryAction: any;
    
    @Action('getPerformanceCurveLibraries')
    getPerformanceCurveLibrariesAction: any;
    @Action('selectPerformanceCurveLibrary')
    selectPerformanceCurveLibraryAction: any;
    @Action('deletePerformanceCurveLibrary')
    deletePerformanceCurveLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('updatePerformanceCurvesCriterionLibraries')
    updatePerformanceCurveCriterionLibrariesAction: any;
    
    @Action('upsertOrDeletePerformanceCurveLibraryUsers') upsertOrDeletePerformanceCurveLibraryUsersAction: any;

    @Action('importScenarioPerformanceCurvesFile')
    importScenarioPerformanceCurvesFileAction: any;
    @Action('importLibraryPerformanceCurvesFile')
    importLibraryPerformanceCurvesFileAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;
    
    @Getter('getUserNameById') getUserNameByIdGetter: any;

    @Mutation('performanceCurveLibraryMutator') performanceCurveLibraryMutator: any;
    @Mutation('selectedPerformanceCurveLibraryMutator') selectedPerformanceCurveLibraryMutator: any;

    addedRows: PerformanceCurve[] = [];
    updatedRowsMap:Map<string, [PerformanceCurve, PerformanceCurve]> = new Map<string, [PerformanceCurve, PerformanceCurve]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: PerformanceCurve[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    selectedPerformanceCurveLibrary: PerformanceCurveLibrary = clone(
        emptyPerformanceCurveLibrary,
    );
    performancePagination: Pagination = clone(emptyPagination);
    isPageInit = false;
    totalItems = 0;
    currentPage: PerformanceCurve[] = [];
    isRunning: boolean = true;
    isShared: boolean = false;
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    hasScenario: boolean = false;
    librarySelectItems: SelectItem[] = [];
    
    performanceCurveGridHeaders: DataTableHeader[] = [
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
    performanceCurveGridData: PerformanceCurve[] = [];
    attributeSelectItems: SelectItem[] = [];
    selectedPerformanceCurve: PerformanceCurve = clone(emptyPerformanceCurve);
    hasSelectedPerformanceCurve: boolean = false;

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = ''
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = '';

    selectedPerformanceEquations: PerformanceCurve[] = [];
    selectedPerformanceEquationIds: string[] = [];

    createPerformanceCurveLibraryDialogData: CreatePerformanceCurveLibraryDialogData = clone(
        emptyCreatePerformanceLibraryDialogData,
    );
    equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    showCreatePerformanceCurveDialog = false;
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = clone(rules);
    uuidNIL: string = getBlankGuid();
    currentUrl: string = window.location.href;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;
    showImportExportPerformanceCurvesDialog: boolean = false;    

    sharePerformanceCurveLibraryDialogData: SharePerformanceCurveLibraryDialogData = clone(emptySharePerformanceCurveLibraryDialogData);

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;           
            vm.getPerformanceCurveLibrariesAction().then(() => {
                vm.getHasPermittedAccessAction().then(() => {
                    if (to.path.indexOf(ScenarioRoutePaths.PerformanceCurve) !== -1) {
                        vm.selectedScenarioId = to.query.scenarioId;

                        if (vm.selectedScenarioId === vm.uuidNIL) {
                            vm.addErrorNotificationAction({
                                message: 'Unable to identify selected scenario.',
                            });
                            vm.$router.push('/Scenarios/');
                        }

                        vm.hasScenario = true;
                        vm.initializePages();

                        vm.hasScenario = true;
                        vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                            vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });        
                        });
                    }

                    
                });
            });          
        });
    }

    mounted() {
        this.setAttributeSelectItems();
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('performancePagination')
    onPaginationChanged() {
        if(this.isRunning)
            return;
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.performancePagination;

        if (!isNullOrUndefined(this.selectedPerformanceCurveLibrary.id) ) {
            this.getIsSharedLibraryAction(this.selectedPerformanceCurveLibrary).then(this.isShared = this.isSharedLibrary);
        }
        const request: PagingRequest<PerformanceCurve>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: this.selectedPerformanceCurveLibrary.id === this.uuidNIL ? null : this.selectedPerformanceCurveLibrary.id,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if((!this.hasSelectedLibrary || this.hasScenario) && this.selectedScenarioId !== this.uuidNIL){
            this.isRunning = true;
            PerformanceCurveService.getPerformanceCurvePage(this.selectedScenarioId, request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                    this.isRunning = false;
                }
            });
        }          
        else if(this.hasSelectedLibrary){
            this.isRunning = true;
            PerformanceCurveService.GetLibraryPerformanceCurvePage(this.librarySelectItemValue !== null ? this.librarySelectItemValue : '', request).then(response => {
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                    this.isRunning = false;
                }
            });  
        }
    }

    @Watch('selectedPerformanceEquations')
    onSelectedPerformanceEquationsChanged() {
        this.selectedPerformanceEquationIds = getPropertyValues('id', this.selectedPerformanceEquations) as string[];
    } 
    
    onRemovePerformanceEquations() {
        this.deletionIds = this.deletionIds.concat(this.selectedPerformanceEquationIds);
        this.selectedPerformanceEquations = [];
        this.onPaginationChanged();
    }    

    @Watch('statePerformanceCurveLibraries')
    onStatePerformanceCurveLibrariesChanged() {
        this.librarySelectItems = this.statePerformanceCurveLibraries.map(
            (library: PerformanceCurveLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

   @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChangedCheckUnsaved(){
        if(this.hasScenario){
            this.onSelectItemValueChanged();
            this.unsavedDialogAllowed = false;
        }           
        else if(this.librarySelectItemValueAllowedChanged)
            this.CheckUnsavedDialog(this.onSelectItemValueChanged, () => {
                this.librarySelectItemValueAllowedChanged = false;
                this.librarySelectItemValue = this.trueLibrarySelectItemValue;               
            })
        this.librarySelectItemValueAllowedChanged = true;
    }
    onSelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue
        this.selectPerformanceCurveLibraryAction(this.librarySelectItemValue);
    }

    @Watch('stateSelectedPerformanceCurveLibrary')
    onStateSelectedPerformanceCurveLibraryChanged() {
        this.selectedPerformanceCurveLibrary = clone(
            this.stateSelectedPerformanceCurveLibrary,
        );
    }

    @Watch('selectedPerformanceCurveLibrary')
    onSelectedPerformanceCurveLibraryChanged() { 
        this.hasSelectedLibrary =
            this.selectedPerformanceCurveLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        this.updatedRowsMap.clear();
        this.deletionIds = [];
        this.addedRows = [];
        this.isRunning = false;
        this.onPaginationChanged();
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributeSelectItems();
    }

    @Watch('stateScenarioPerformanceCurves')
    onStateScenarioPerformanceCurvesChanged() {
        if (
            this.hasScenario
        ) {
            this.onPaginationChanged();
        }
    }

    @Watch('deletionIds')
    onDeletionIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedRows')
    onAddedRowsChanged(){
        this.checkHasUnsavedChanges();
    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
    }
    checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            this.deletionIds.length > 0 || 
            this.addedRows.length > 0 ||
            this.updatedRowsMap.size > 0 || (this.hasScenario && this.hasSelectedLibrary) ||
            (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.selectedPerformanceCurveLibrary, this.stateSelectedPerformanceCurveLibrary))
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    setAttributeSelectItems() {
        if (hasValue(this.stateNumericAttributes)) {
            this.attributeSelectItems = this.stateNumericAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedPerformanceCurveLibrary.owner) == getUserName();
    }

    getOwnerUserName(): string {
        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedPerformanceCurveLibrary.owner);
        }
        return getUserName();
    }

    onShowCreatePerformanceCurveLibraryDialog(createAsNewLibrary: boolean) { 
        this.createPerformanceCurveLibraryDialogData = {
            showDialog: true,
            performanceCurves: createAsNewLibrary
                ? this.currentPage
                : [],
        };
    }

    onSubmitCreatePerformanceCurveLibraryDialogResult(
        performanceCurveLibrary: PerformanceCurveLibrary,
    ) {
        this.createPerformanceCurveLibraryDialogData = clone(
            emptyCreatePerformanceLibraryDialogData,
        );

        if (!isNil(performanceCurveLibrary)) {
            const upsertRequest: LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve> = {
                library: performanceCurveLibrary,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: performanceCurveLibrary.performanceCurves.length == 0 || !this.hasSelectedLibrary ? null : this.selectedPerformanceCurveLibrary.id,
                    rowsForDeletion: performanceCurveLibrary.performanceCurves === [] ? [] : this.deletionIds,
                    updateRows: performanceCurveLibrary.performanceCurves === [] ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: performanceCurveLibrary.performanceCurves === [] ? [] : this.addedRows,
                 },
                scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            PerformanceCurveService.UpsertPerformanceCurveLibrary(upsertRequest).then(() => {
                this.hasCreatedLibrary = true;
                this.librarySelectItemValue = performanceCurveLibrary.id;
                
                if(performanceCurveLibrary.performanceCurves === []){
                    this.clearChanges();
                }

                this.performanceCurveLibraryMutator(performanceCurveLibrary);
                this.selectedPerformanceCurveLibraryMutator(performanceCurveLibrary.id);
                this.addSuccessNotificationAction({message:'Added deterioration model library'})
            })
        }
    }

    onSubmitCreatePerformanceCurveDialogResult( 
        newPerformanceCurve: PerformanceCurve,
    ) {
        this.showCreatePerformanceCurveDialog = false;

        if (!isNil(newPerformanceCurve)) {
            this.addedRows = prepend(
                newPerformanceCurve,
                this.addedRows,
            );
            this.onPaginationChanged();
        }
    }

    onEditPerformanceCurveProperty(id: string, property: string, value: any) {
        if (any(propEq('id', id), this.currentPage)) { 
            const performanceCurve: PerformanceCurve = find(
                propEq('id', id),
                this.currentPage,
            ) as PerformanceCurve;
            this.onUpdateRow(id, performanceCurve);
            this.onPaginationChanged();
        }
    }

    onShowEquationEditorDialog(performanceCurveId: string) {
        this.selectedPerformanceCurve = find(
            propEq('id', performanceCurveId),
            this.currentPage,
        ) as PerformanceCurve;

        if (!isNil(this.selectedPerformanceCurve)) {
            this.hasSelectedPerformanceCurve = true;

            this.equationEditorDialogData = {
                showDialog: true,
                equation: this.selectedPerformanceCurve.equation,
            };
        }
    }

    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedPerformanceCurve) {
            this.onUpdateRow(this.selectedPerformanceCurve.id, { ...this.selectedPerformanceCurve, equation: equation })
            this.currentPage = update(
                findIndex(
                    propEq('id', this.selectedPerformanceCurve.id),
                    this.currentPage,
                ),
                { ...this.selectedPerformanceCurve, equation: equation },
                this.currentPage,
            );
        }

        this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
        this.hasSelectedPerformanceCurve = false;
    }

    onEditPerformanceCurveCriterionLibrary(performanceCurveId: string) {
        this.selectedPerformanceCurve = find(
            propEq('id', performanceCurveId),
            this.currentPage,
        ) as PerformanceCurve;

        if (!isNil(this.selectedPerformanceCurve)) {
            this.hasSelectedPerformanceCurve = true;

            this.criterionEditorDialogData = {
                showDialog: true,
                CriteriaExpression: this.selectedPerformanceCurve.criterionLibrary.mergedCriteriaExpression
            };
        }
    }

    onSubmitCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionExpression) && this.hasSelectedPerformanceCurve) {
            if(this.selectedPerformanceCurve.criterionLibrary.id === getBlankGuid())
                this.selectedPerformanceCurve.criterionLibrary.id = getNewGuid();
            this.onUpdateRow(this.selectedPerformanceCurve.id, { ...this.selectedPerformanceCurve, 
            criterionLibrary: {...this.selectedPerformanceCurve.criterionLibrary, mergedCriteriaExpression: criterionExpression} })
            this.currentPage = update(
                findIndex(
                    propEq('id', this.selectedPerformanceCurve.id),
                    this.currentPage,
                ),
                {
                    ...this.selectedPerformanceCurve,
                    criterionLibrary: {...this.selectedPerformanceCurve.criterionLibrary, mergedCriteriaExpression: criterionExpression},
                },
                this.currentPage,
            );
        }

        this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
        this.hasSelectedPerformanceCurve = false;
    }

    onRemovePerformanceCurve(performanceCurveId: string) {
        this.deletionIds.push(performanceCurveId);
        this.onPaginationChanged();
    }

    onUpsertScenarioPerformanceCurves() {
        PerformanceCurveService.UpsertScenarioPerformanceCurves({
            libraryId: this.selectedPerformanceCurveLibrary.id === this.uuidNIL ? null : this.selectedPerformanceCurveLibrary.id,
            rowsForDeletion: this.deletionIds,
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows           
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.addSuccessNotificationAction({message: "Modified scenario's deterioration models"});
                this.librarySelectItemValue = null
            }           
        });
    }

    onUpsertPerformanceCurveLibrary() { // need to do upsert things
        const upsertRequest: LibraryUpsertPagingRequest<PerformanceCurveLibrary, PerformanceCurve> = {
                library: this.selectedPerformanceCurveLibrary,
                isNewLibrary: false,
                 syncModel: {
                    libraryId: this.selectedPerformanceCurveLibrary.id === this.uuidNIL ? null : this.selectedPerformanceCurveLibrary.id,
                    rowsForDeletion: this.deletionIds,
                    updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: this.addedRows
                 },
                 scenarioId: null
        }
        PerformanceCurveService.UpsertPerformanceCurveLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges()
                this.resetPage();
                this.performanceCurveLibraryMutator(this.selectedPerformanceCurveLibrary);
                this.selectedPerformanceCurveLibraryMutator(this.selectedPerformanceCurveLibrary.id);
                this.addSuccessNotificationAction({message: "Updated deterioration model library",});
            }
        });
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.deletionIds = [];
                this.addedRows = [];
                this.updatedRowsMap.clear();
                this.resetPage();
            }
        });
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
            this.deletePerformanceCurveLibraryAction(
                this.selectedPerformanceCurveLibrary.id,
            );
        }
    }

    disableCrudButtons() {
        const rowChanges = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rowChanges.every(
            (performanceCurve: PerformanceCurve) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        performanceCurve.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        performanceCurve.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedPerformanceCurveLibrary.name,
                ) === true &&
                dataIsValid);
        }

        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    OnDownloadTemplateClick()
    {
        PerformanceCurveService.downloadPerformanceCurvesTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    exportPerformanceCurves() {
        const id: string = this.hasScenario ? this.selectedScenarioId : this.selectedPerformanceCurveLibrary.id;
                PerformanceCurveService.exportPerformanceCurves(id, this.hasScenario)
                    .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                            const fileInfo: FileInfo = response.data as FileInfo;
                            FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                    });
    }

    onSubmitImportExportPerformanceCurvesDialogResult(result: ImportExportPerformanceCurvesDialogResult) {
        this.showImportExportPerformanceCurvesDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {

            }
            else
            if (hasValue(result.file)) {
                const data: PerformanceCurvesFileImport = {
                    file: result.file
                };

                if (this.hasScenario) {
                    this.importScenarioPerformanceCurvesFileAction({
                        ...data,
                        id: this.selectedScenarioId,
                        currentUserCriteriaFilter: this.currentUserCriteriaFilter
                    }).then(() => {
                        this.onDiscardChanges();
                    });
                } else {
                    this.importLibraryPerformanceCurvesFileAction({
                        ...data,
                        id: this.selectedPerformanceCurveLibrary.id,
                        currentUserCriteriaFilter: this.currentUserCriteriaFilter
                    }).then(() => {
                        this.onDiscardChanges();
                    });
                }

            }
        }
    }
    onShowSharePerformanceCurveLibraryDialog(performanceCurveLibrary: PerformanceCurveLibrary)
    {
        this.sharePerformanceCurveLibraryDialogData =
        {
            showDialog: true,
            performanceCurveLibrary: clone(performanceCurveLibrary),
        };
    }
    onSharePerformanceCurveLibraryDialogSubmit(performanceCurveLibraryUsers: PerformanceCurveLibraryUser[]) {
        this.sharePerformanceCurveLibraryDialogData = clone(emptySharePerformanceCurveLibraryDialogData);

        if (!isNil(performanceCurveLibraryUsers) && this.selectedPerformanceCurveLibrary.id !== getBlankGuid())
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
            //this.selectedPerformanceCurveLibrary.isShared = this.sharePerformanceCurveLibraryDialogData.performanceCurveLibrary.isShared;
            if (!isNullOrUndefined(this.selectedPerformanceCurveLibrary.id) ) {
                this.getIsSharedLibraryAction(this.selectedPerformanceCurveLibrary).then(this.isShared = this.isSharedLibrary);
            }
            //update performance curve library sharing
            PerformanceCurveService.upsertOrDeletePerformanceCurveLibraryUsers(this.selectedPerformanceCurveLibrary.id, libraryUserData).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString()))
                {
                    this.resetPage();
                }
            });
        }
    }
    onSearchClick(){
        this.currentSearch = this.gridSearchTerm;
        this.resetPage();
    }

    onClearClick(){
        this.gridSearchTerm = '';
        this.onSearchClick();
    }

    onUpdateRow(rowId: string, updatedRow: PerformanceCurve){
        if(any(propEq('id', rowId), this.addedRows))
            return;

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
        this.performancePagination.page = 1;
        this.onPaginationChanged();
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
        const request: PagingRequest<PerformanceCurve>= {
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
            PerformanceCurveService.getPerformanceCurvePage(this.selectedScenarioId, request).then(response => {
                this.isRunning = false
                if(response.data){
                    let data = response.data as PagingPage<PerformanceCurve>;
                    this.currentPage = data.items;
                    this.rowCache = clone(this.currentPage)
                    this.totalItems = data.totalItems;
                }
            });
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
