<template>
    <v-layout column>
        <v-flex style="margin-top: -20px;">
            <v-layout>
                <v-flex xs6>
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment Library</v-subheader>
                    <v-select
                        id="TreatmentEditor-treatmentLibrary-select"
                        :items='librarySelectItems'
                        append-icon=$vuetify.icons.ghd-down
                        class='ghd-control-border ghd-control-text ghd-control-width-dd ghd-select'
                        label='Select a Treatment Library'
                        outline                        
                        v-model='librarySelectItemValue' 
                    >
                    </v-select>
                    <div class="ghd-md-gray ghd-control-subheader treatment-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>  
                </v-flex>
                <v-flex xs6>                       
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment</v-subheader>
                    <v-select
                    id="TreatmentEditor-treatment-select"
                        :items='treatmentSelectItems'
                        append-icon=$vuetify.icons.ghd-down
                        class='ghd-control-border ghd-control-text ghd-control-width-dd ghd-select'
                        label='Select'
                        outline                        
                        v-model='treatmentSelectItemValue'
                    >
                    </v-select>
                </v-flex>
                <v-flex style="padding-right: 5px">
                    <v-btn
                        @click='showImportTreatmentDialog = true'
                        depressed
                        class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding ghd-margin-top'                        
                        v-show='hasSelectedLibrary'                        
                    >
                        Import Treatment
                    </v-btn>
                </v-flex>
                <v-flex style="padding-right: 5px">
                    <v-btn
                        @click='onShowConfirmDeleteTreatmentAlert'
                        depressed
                        class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding ghd-margin-top'                        
                        v-show='hasSelectedTreatment && !isNoTreatmentSelected'                        
                    >
                        Delete Treatment
                    </v-btn>
                </v-flex>
                <v-flex justify-right align-end style="padding-top: 38px !important;" >
                    <v-btn
                        id="TreatmentEditor-createLibrary-btn"
                        @click='onShowCreateTreatmentLibraryDialog(false)'
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                        v-show="!hasScenario"
                        outline

                    >
                        Create New Library
                    </v-btn>                                                          
                </v-flex>
            </v-layout>

            <v-flex xs6>
                    <v-layout v-if='hasSelectedLibrary && !hasScenario' style="padding-bottom: 50px !important">
                        <div class="ghd-control-label">
                        Owner: <v-label>{{ getOwnerUserName() || '[ No Owner ]' }}</v-label> |    
                        <v-badge v-show="isShared">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowTreatmentLibraryDialog(selectedTreatmentLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                               v-show='!hasScenario'>
                            Share Library
                        </v-btn>

                        </div>  
                    </v-layout>
            </v-flex>


        </v-flex>
        <v-divider style="margin-top:-10px" v-show='hasSelectedLibrary || hasScenario'></v-divider>        
        <div v-show='hasSelectedLibrary || hasScenario' style="width:100%;margin-top:-20px;margin-bottom:-15px;">                
               <v-btn
                    id="TreatmentEditor-addTreatment-btn"
                    @click='showCreateTreatmentDialog = true'
                    depressed
                    class='ghd-white-bg ghd-blue ghd-button-text ghd-text-padding'                              
                    style='float:left;'
                >
                    <span class="ghd-right-padding">Add Treatment</span>
                    <v-icon>fas fa-plus</v-icon>
                </v-btn>                
                <v-btn :disabled='false' @click='OnDownloadTemplateClick()'
                    flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download Template
                </v-btn> 
                <label style='float:right;padding-top:13px;' class="ghd-grey" v-show ='hasSelectedLibrary && !hasScenario'>|</label>
                <v-btn :disabled='false' @click='OnExportTreamentsClick()'
                    flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download
                </v-btn> 
                <label style='float:right;padding-top:13px;' class="ghd-grey" v-show ='hasSelectedLibrary && !hasScenario'>|</label>
                <v-btn :disabled='false' @click='showImportTreatmentsDialog = true'
                    flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Upload
                </v-btn>
            </div>    
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <v-layout>
                <div xs2>
                    <v-flex>
                        <v-list id='TreatmentEditor-Treatment-list' class='treatments-list'>
                            <template v-for='treatmentSelectItem in treatmentSelectItems'>
                                <v-list-tile :key='treatmentSelectItem.value' ripple :class="{'selected-treatment-item': isSelectedTreatmentItem(treatmentSelectItem.value)}"
                                             avatar @click='onSetTreatmentSelectItemValue(treatmentSelectItem.value)'>
                                    <v-list-tile-content>
                                        <span>{{treatmentSelectItem.text}}</span>
                                    </v-list-tile-content>
                                    <v-list-tile-action v-show="treatmentSelectItem.text!='No Treatment'">                                        
                                        <v-btn @click="onShowConfirmDeleteTreatmentAlert" class="ghd-blue" icon>
                                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                        </v-btn>
                                    </v-list-tile-action>
                                </v-list-tile>
                            </template>
                        </v-list>
                    </v-flex>
                </div>
                <div class='treatments-div' xs10>
                    <v-layout column> 
                        <v-flex xs12>               
                            <div v-show='selectedTreatment.id !== uuidNIL'>                                                
                                <v-tabs v-model='activeTab' id='TreatmentEditor-treatmenttabs'>
                                    <v-tab 
                                        :key='index'
                                        @click='activeTab = index'
                                        ripple
                                        v-for='(treatmentTab,
                                        index) in treatmentTabs'
                                    >
                                        {{ treatmentTab }}
                                    </v-tab>
                                    <v-tabs-items v-model='activeTab'>
                                        <v-tab-item>
                                            <v-card style="border:none;">
                                                <v-card-text
                                                    class='card-tab-content'
                                                >
                                                    <TreatmentDetailsTab
                                                        :selectedTreatmentDetails='selectedTreatmentDetails'
                                                        :rules='rules'
                                                        :callFromScenario='hasScenario'
                                                        :callFromLibrary='!hasScenario'
                                                        @onModifyTreatmentDetails='modifySelectedTreatmentDetails'
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-card>
                                                <v-card-text
                                                    class='card-tab-content'
                                                >
                                                    <CostsTab
                                                        :selectedTreatmentCosts='selectedTreatment.costs'
                                                        :callFromScenario='hasScenario'
                                                        :callFromLibrary='!hasScenario'
                                                        @onAddCost='addSelectedTreatmentCost'
                                                        @onModifyCost='modifySelectedTreatmentCost'
                                                        @onRemoveCost='removeSelectedTreatmentCost'
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-card>
                                                <v-card-text
                                                    class='card-tab-content'
                                                >
                                                    <PerformanceFactorTab
                                                        :selectedTreatment='selectedTreatment'
                                                        :selectedTreatmentPerformanceFactors='selectedTreatment.performanceFactors'
                                                        :scenarioId='loadedScenarioId'
                                                        :rules='rules'
                                                        :callFromScenario='hasScenario'
                                                        :callFromLibrary='!hasScenario'
                                                        @onModifyPerformanceFactor='modifySelectedTreatmentPerformanceFactor'
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-card>
                                                <v-card-text
                                                    class='card-tab-content'
                                                >
                                                    <ConsequencesTab
                                                        :selectedTreatmentConsequences='selectedTreatment.consequences'
                                                        :rules='rules'
                                                        :callFromScenario='hasScenario'
                                                        :callFromLibrary='!hasScenario'
                                                        @onAddConsequence='addSelectedTreatmentConsequence'
                                                        @onModifyConsequence='modifySelectedTreatmentConsequence'
                                                        @onRemoveConsequence='removeSelectedTreatmentConsequence'
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-tab-item>
                                        <v-tab-item>
                                            <v-card>
                                                <v-card-text class='card-tab-content'>
                                                    <BudgetsTab :selectedTreatmentBudgets='selectedTreatment.budgetIds'
                                                                :addTreatment='selectedTreatment.addTreatment'
                                                                :fromLibrary='hasSelectedLibrary'
                                                                @onModifyBudgets='modifySelectedTreatmentBudgets' />
                                                </v-card-text>
                                            </v-card>
                                        </v-tab-item>
                                    </v-tabs-items>
                                </v-tabs>
                            </div>                                             
                        </v-flex>                    
                    </v-layout>
                </div>
            </v-layout>
        </v-flex>        
        <v-flex xs12>
            <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
            <v-layout justify-center v-show='hasSelectedLibrary && !hasScenario'>
                <v-flex xs12>
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea                        
                        class='ghd-control-border ghd-control-text'
                        no-resize
                        outline
                        rows='2'
                        v-model='selectedTreatmentLibrary.description'
                        @input='checkHasUnsavedChanges()'
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs9>
            <v-layout justify-center row v-show='(hasSelectedLibrary || hasScenario)'>
                <v-btn :disabled='!hasUnsavedChanges'
                    @click='onDiscardChanges'
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    depressed
                    v-show='hasScenario'
                >
                    Cancel
                </v-btn>
                <v-btn id='TreatmentEditor-deleteLibrary-btn' outline
                    @click='onShowConfirmDeleteAlert'
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    depressed
                    v-show='!hasScenario'
                    :disabled='!hasLibraryEditPermission'
                >
                    Delete Library
                </v-btn>
                <v-btn
                    @click='onShowCreateTreatmentLibraryDialog(true)'
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    outline
                    :disabled='disableCrudButtons()'
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click='onUpsertScenarioTreatments'
                    class='ghd-blue-bg ghd-white ghd-button-text'
                    depressed
                    v-show='hasScenario'
                    :disabled='disableCrudButtonsResult || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn
                    id="TreatmentEditor-updateLibrary-btn"
                    @click='onUpsertTreatmentLibrary'
                    class='ghd-blue-bg ghd-white ghd-button-text  ghd-text-padding'
                    depressed
                    v-show='!hasScenario'
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                >
                    Update Library
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData='confirmBeforeDeleteAlertData'
            @submit='onSubmitConfirmDeleteAlertResult'
        />

        <CreateTreatmentLibraryDialog
            :dialogData='createTreatmentLibraryDialogData'
            @submit='onSubmitCreateTreatmentLibraryDialogResult'
        />

        <ShareTreatmentLibraryDialog :dialogData='shareTreatmentLibraryDialogData' @submit='onShareTreatmentLibraryDialogSubmit' />

        <CreateTreatmentDialog
            :showDialog='showCreateTreatmentDialog'
            @submit='onAddTreatment'
        />

        <ImportNewTreatmentDialog
            :showDialog ='showImportTreatmentDialog'
            @submit='onSubmitNewTreatment'
        />

        <ImportExportTreatmentsDialog :showDialog='showImportTreatmentsDialog'
            @submit='onSubmitImportTreatmentsDialogResult' />

        <ConfirmDeleteTreatmentAlert
            :dialogData='confirmBeforeDeleteTreatmentAlertData'
            @submit='onSubmitConfirmDeleteTreatmentAlertResult'
        />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter, Mutation } from 'vuex-class';
import CreateTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    CreateTreatmentLibraryDialogData,
    emptyCreateTreatmentLibraryDialogData,
} from '@/shared/models/modals/create-treatment-library-dialog-data';
import {
    ShareTreatmentLibraryDialogData,
    emptyShareTreatmentLibraryDialogData,
} from '@/shared/models/modals/share-treatment-library-dialog-data';
import ShareTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/ShareTreatmentLibraryDialog.vue';
import { LibraryUser } from '@/shared/models/iAM/user';
import {
    emptyConsequence,
    emptyTreatment,
    emptyTreatmentDetails,
    emptyTreatmentLibrary,
    SimpleTreatment,
    Treatment,
    TreatmentConsequence,
    TreatmentPerformanceFactor,
    TreatmentCost,
    TreatmentDetails,
    TreatmentLibrary,
    TreatmentLibraryUser,
    TreatmentsFileImport
} from '@/shared/models/iAM/treatment';
import {
    emptyPerformanceCurve,
    PerformanceCurve,
} from '@/shared/models/iAM/performance';
import CreateTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
import {
    any,
    append,
    clone,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
    isEmpty,
} from 'ramda';
import TreatmentDetailsTab from '@/components/treatment-editor/treatment-editor-tabs/TreatmentDetailsTab.vue';
import CostsTab from '@/components/treatment-editor/treatment-editor-tabs/CostsTab.vue';
import PerformanceFactorTab from '@/components/treatment-editor/treatment-editor-tabs/PerformanceFactorTab.vue';
import ConsequencesTab from '@/components/treatment-editor/treatment-editor-tabs/ConsequencesTab.vue';
import BudgetsTab from '@/components/treatment-editor/treatment-editor-tabs/BudgetsTab.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { hasUnsavedChangesCore, isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getUserName } from '@/shared/utils/get-user-info';
import ImportExportTreatmentsDialog from '@/components/treatment-editor/treatment-editor-dialogs/ImportExportTreatmentsDialog.vue';
import ImportNewTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/ImportNewTreatmentDialog.vue';
import { ImportExportTreatmentsDialogResult } from '@/shared/models/modals/import-export-treatments-dialog-result';
import TreatmentService from '@/services/treatment.service';
import { AxiosResponse } from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { LibraryUpsertPagingRequest } from '@/shared/models/iAM/paging';
import { http2XX } from '@/shared/utils/http-utils';
import { watch } from 'fs';
import { isNullOrUndefined } from 'util';
import { Hub } from '@/connectionHub';
import ScenarioService from '@/services/scenario.service';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import { ImportNewTreatmentDialogResult } from '@/shared/models/modals/import-new-treatment-dialog-result';

@Component({
    components: {
        ImportExportTreatmentsDialog,
        ShareTreatmentLibraryDialog,
        BudgetsTab,
        ConsequencesTab,
        CostsTab,
        PerformanceFactorTab,
        TreatmentDetailsTab,
        CreateTreatmentDialog,
        CreateTreatmentLibraryDialog,
        ImportNewTreatmentDialog,
        ConfirmDeleteAlert: Alert,
        ConfirmDeleteTreatmentAlert: Alert
    },
})
export default class TreatmentEditor extends Vue {
    @State(state => state.treatmentModule.treatmentLibraries)
    stateTreatmentLibraries: TreatmentLibrary[];
    @State(state => state.treatmentModule.selectedTreatmentLibrary)
    stateSelectedTreatmentLibrary: TreatmentLibrary;
    @State(state => state.treatmentModule.scenarioSelectableTreatments)
    stateScenarioSelectableTreatments: Treatment[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.treatmentModule.scenarioTreatmentLibrary)
    stateScenarioTreatmentLibrary: TreatmentLibrary;
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.treatmentModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @State(state => state.treatmentModule.simpleScenarioSelectableTreatments) stateSimpleScenarioSelectableTreatments: SimpleTreatment[];
    @State(state => state.treatmentModule.simpleSelectableTreatments) stateSimpleSelectableTreatments: SimpleTreatment[];
    @State(state => state.treatmentModule.isSharedLibrary) isSharedLibrary: boolean;
    @State(state => state.performanceCurveModule.scenarioPerformanceCurves) stateScenarioPerformanceCurves: PerformanceCurve[];

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
    @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
    @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
    @Action('upsertTreatmentLibrary') upsertTreatmentLibraryAction: any;
    @Action('deleteTreatmentLibrary') deleteTreatmentLibraryAction: any;
    @Action('getSimpleScenarioSelectableTreatments') getSimpleScenarioSelectableTreatmentsAction: any;
    @Action('getSimpleSelectableTreatments') getSimpleSelectableTreatmentsAction: any;
    @Action('getTreatmentLibraryBySimulationId') getTreatmentLibraryBySimulationIdAction: any;
    @Action('getScenarioSimpleBudgetDetails')
    getScenarioSimpleBudgetDetailsAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioSelectableTreatments')
    getScenarioSelectableTreatmentsAction: any;
    @Action('upsertScenarioSelectableTreatments')
    upsertScenarioSelectableTreatmentsAction: any;
    @Action('upsertOrDeleteTreatmentLibraryUsers') upsertOrDeleteTreatmentLibraryUsersAction: any;
    @Action('importScenarioTreatmentsFile')
    importScenarioTreatmentsFileAction: any;
    @Action('importLibraryTreatmentsFile')
    importLibraryTreatmentsFileAction: any;
    @Action('deleteTreatment') deleteTreatmentAction: any;
    @Action('deleteScenarioSelectableTreatment') deleteScenarioSelectableTreatmentAction: any;
    @Action('getIsSharedTreatmentLibrary') getIsSharedLibraryAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;
    @Action('getScenarioPerformanceCurves') getScenarioPerformanceCurvesAction: any;
    @Action('setAlertMessage') setAlertMessageAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    @Mutation('addedOrUpdatedTreatmentLibraryMutator') addedOrUpdatedTreatmentLibraryMutator: any;
    @Mutation('selectedTreatmentLibraryMutator') selectedTreatmentLibraryMutator: any;

    selectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
    treatments: Treatment[] = [];
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    treatmentSelectItems: SelectItem[] = [];
    treatmentSelectItemValue: string ="";
    selectedTreatment: Treatment = clone(emptyTreatment);
    selectedTreatmentDetails: TreatmentDetails = clone(emptyTreatmentDetails);
    activeTab: number = 0;
    treatmentTabs: string[] = ['Treatment Details', 'Costs', 'Performance Factor', 'Consequences'];
    createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = clone(
        emptyCreateTreatmentLibraryDialogData,
    );
    showCreateTreatmentDialog: boolean = false;
    showImportTreatmentDialog: boolean = false;
    confirmBeforeDeleteAlertData: AlertData = clone(emptyAlertData);
    hasSelectedTreatment: boolean = false;
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    keepActiveTab: boolean = false;
    hasScenario: boolean = false;
    budgets: SimpleBudgetDetail[] = [];
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;
    showImportTreatmentsDialog: boolean = false;
    confirmBeforeDeleteTreatmentAlertData: AlertData = clone(emptyAlertData);
    isNoTreatmentSelected: boolean = false;
    hasImport: boolean = false;

    addedRows: Treatment[] = [];
    updatedRowsMap:Map<string, [Treatment, Treatment]> = new Map<string, [Treatment, Treatment]>();//0: original value | 1: updated value
    rowCache: Treatment[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    isPageInit = false;
    totalItems = 0;
    currentPage: Treatment[] = [];
    initializing: boolean = true;

    simpleTreatments: SimpleTreatment[] = [];
    isShared: boolean = false;
    treatmentCache: Treatment[] = [];

    unsavedDialogAllowed: boolean = true;
    trueLibrarySelectItemValue: string | null = '';
    librarySelectItemValueAllowedChanged: boolean = true;
    librarySelectItemValue: string | null = '';

    shareTreatmentLibraryDialogData: ShareTreatmentLibraryDialogData = clone(emptyShareTreatmentLibraryDialogData);
    loadedScenarioId: string = '';
    parentLibraryId: string  = this.uuidNIL;
    parentLibraryName: string = 'None';
    scenarioParentLIbrary: string | null = null;
    scenarioLibraryIsModified: boolean = false;
    loadedParentName: string = "";
    loadedParentId: string  = this.uuidNIL;
    newLibrarySelection: boolean = false;
    newTreatment: Treatment = {...emptyTreatment, id: getNewGuid(), addTreatment: false};

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = "";
            vm.getTreatmentLibrariesAction();
            if (to.path.indexOf(ScenarioRoutePaths.Treatment) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;
                vm.loadedScenarioId = vm.selectedScenarioId;
                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }
                vm.hasScenario = true;
                vm.getSimpleScenarioSelectableTreatmentsAction(vm.selectedScenarioId);
                vm.getTreatmentLibraryBySimulationIdAction(vm.selectedScenarioId);
                vm.getScenarioPerformanceCurvesAction(vm.selectedScenarioId);
                vm.treatmentTabs = [...vm.treatmentTabs, 'Budgets'];
                vm.getScenarioSimpleBudgetDetailsAction({ scenarioId: vm.selectedScenarioId, }).then(()=> {
                    vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.selectedScenarioId}).then(() => {         
                        vm.selectScenarioAction({ scenarioId: vm.selectedScenarioId });   
                    });
                });
                ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: vm.selectedScenarioId, workType: WorkType.ImportScenarioTreatment}).then(response => {
                    if(response.data){
                        vm.setAlertMessageAction("A treatment curve has been added to the work queue")
                    }
                })
            }
        });
    }
    mounted() {
            this.$statusHub.$on(
                Hub.BroadcastEventType.BroadcastImportCompletionEvent,
                this.importCompleted,
            );
        } 
    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            this.importCompleted,
        );
        this.setAlertMessageAction('');
    }  

    @Watch('stateScenarioSimpleBudgetDetails')
    onStateScenarioInvestmentLibraryChanged() {
        this.budgets = clone(this.stateScenarioSimpleBudgetDetails);
    }

  
    @Watch('stateTreatmentLibraries')
    onStateTreatmentLibrariesChanged() {
        this.librarySelectItems = this.stateTreatmentLibraries.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id,
            })
        );
    }
    @Watch('stateScenarioPerformanceCurves')
    onStateScenarioPerformanceCurvesChanged() {
    }
    
    @Watch('stateScenarioTreatmentLibrary')
    onStateScenarioTreatmentLibraryChanged() {
        this.setParentLibraryName(this.stateScenarioTreatmentLibrary ? this.stateScenarioTreatmentLibrary.id : "None");
        this.scenarioLibraryIsModified = this.stateScenarioTreatmentLibrary ? this.stateScenarioTreatmentLibrary.isModified : false;
        this.loadedParentId = this.stateScenarioTreatmentLibrary ? this.stateScenarioTreatmentLibrary.id : this.uuidNIL;
        this.loadedParentName = this.parentLibraryName;
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
        this.setParentLibraryName(this.librarySelectItemValue ? this.librarySelectItemValue : this.parentLibraryId);
        this.scenarioLibraryIsModified = false;
        this.newLibrarySelection = true;
    }
    onSelectItemValueChanged() {
        this.trueLibrarySelectItemValue = this.librarySelectItemValue;
        this.selectTreatmentLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
    
        if(!isNil(this.librarySelectItemValue)){
            if (!isEmpty(this.librarySelectItemValue)){
                this.getSimpleSelectableTreatmentsAction(this.librarySelectItemValue);
            }
        }           
    }  

    @Watch('stateSimpleSelectableTreatments')
    onStateSimpleSelectableTreatments() {
        this.simpleTreatments = clone(this.stateSimpleSelectableTreatments);
    }

    @Watch('stateSimpleScenarioSelectableTreatments')
    onStateSimpleScenarioSelectableTreatments(){
        this.simpleTreatments = clone(this.stateSimpleScenarioSelectableTreatments);
    }

    @Watch('stateSelectedTreatmentLibrary')
    onStateSelectedTreatmentLibraryChanged() {
        this.selectedTreatmentLibrary = clone(
            this.stateSelectedTreatmentLibrary
        );
    }
    @Watch('isSharedLibrary')
    onStateSharedAccessChanged() {
        this.isShared = this.isSharedLibrary;
    }

    @Watch('selectedTreatmentLibrary')
    onSelectedTreatmentLibraryChanged() {
        this.hasSelectedLibrary = this.selectedTreatmentLibrary.id !== this.uuidNIL;
        this.getIsSharedLibraryAction(this.selectedTreatmentLibrary).then(this.isShared = this.isSharedLibrary);
        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: this.selectedTreatmentLibrary.id, workType: WorkType.ImportLibraryTreatment}).then(response => {
                if(response.data){
                    this.setAlertMessageAction("A treatment import has been added to the work queue")
                }
                else
                    this.setAlertMessageAction("");
            })
        }

        this.clearChanges();
        if(this.treatmentSelectItemValue !== null && !this.hasScenario)
            this.treatmentCache.push(clone(this.selectedTreatment))
        this.checkHasUnsavedChanges();
    }

    @Watch('simpleTreatments')
    onSimpleTreatments(){
        this.treatmentSelectItems = this.simpleTreatments.map((treatment: SimpleTreatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));

        this.checkHasUnsavedChanges()

        this.treatmentSelectItemValue = "";
    }

    @Watch('treatments')
    onSelectedScenarioTreatmentsChanged() {
        this.treatmentSelectItems = this.treatments.map((treatment: Treatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));       
        this.hasUnsavedChanges = false;
        this.disableCrudButtons();
    }

    @Watch('treatmentSelectItemValue')
    onTreatmentSelectItemValueChanged() {
        if(!isNil(this.treatmentSelectItemValue) && this.treatmentSelectItemValue !== ""){
            var mapEntry = this.updatedRowsMap.get(this.treatmentSelectItemValue);
            var addedRow = this.addedRows.find(_ => _.id == this.treatmentSelectItemValue);
            var treatment = this.treatmentCache.find(_ => _.id === this.treatmentSelectItemValue);

            if(!isNil(mapEntry)){
                this.selectedTreatment = clone(mapEntry[1]);
            }
            else if(!isNil(addedRow)){
                this.selectedTreatment = clone(addedRow);
            }               
            else if(this.hasSelectedLibrary)
                TreatmentService.getSelectedTreatmentById(this.treatmentSelectItemValue).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        this.selectedTreatment = data;
                        if(isNil(this.treatmentCache.find(_ => _.id === data.id)))
                            this.treatmentCache.push(data)
                    }
                })
            else if(!isNil(treatment))
                this.selectedTreatment = clone(treatment);
            else
                TreatmentService.getScenarioSelectedTreatmentById(this.treatmentSelectItemValue).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        this.selectedTreatment = data;
                        if(isNil(this.treatmentCache.find(_ => _.id === data.id))){ this.treatmentCache.push(data); }
                        this.scenarioLibraryIsModified = this.selectedTreatment ? this.selectedTreatment.isModified : false;
                    }
                })
        }
        else
            this.selectedTreatment = clone(emptyTreatment);
       
        if (!this.keepActiveTab) {
            this.activeTab = 0;
        }
        this.keepActiveTab = true;
    }

    @Watch('selectedTreatment')
    onSelectedTreatmentChanged() {
        this.hasSelectedTreatment = this.selectedTreatment.id !== this.uuidNIL;

        this.selectedTreatmentDetails = {
            description: this.selectedTreatment.description,
            shadowForSameTreatment: this.selectedTreatment.shadowForSameTreatment,
            shadowForAnyTreatment: this.selectedTreatment.shadowForAnyTreatment,
            criterionLibrary: this.selectedTreatment.criterionLibrary,
            category: this.selectedTreatment.category,
            assetType: this.selectedTreatment.assetType,
            isUnselectable: this.selectedTreatment.isUnselectable,
        };

        this.isNoTreatmentSelected = this.selectedTreatment.name == 'No Treatment';
    }

    isSelectedTreatmentItem(treatmentId: string | number) {
        return isEqual(this.treatmentSelectItemValue, treatmentId.toString());
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedTreatmentLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedTreatmentLibrary.owner) == getUserName();
    }

    onSetTreatmentSelectItemValue(treatmentId: string | number) {
        if (!isEqual(this.treatmentSelectItemValue, treatmentId.toString())) {
            this.treatmentSelectItemValue = treatmentId.toString();
        }
    }
    
    onShowConfirmDeleteTreatmentAlert() {
        this.confirmBeforeDeleteTreatmentAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onShowTreatmentLibraryDialog(treatmentLibrary: TreatmentLibrary) {
        this.shareTreatmentLibraryDialogData = {
            showDialog: true,
            treatmentLibrary: clone(treatmentLibrary)
        };
    }

    onShareTreatmentLibraryDialogSubmit(treatmentLibraryUsers: TreatmentLibraryUser[]) {
        this.shareTreatmentLibraryDialogData = clone(emptyShareTreatmentLibraryDialogData);
        if (!isNil(treatmentLibraryUsers) && this.selectedTreatmentLibrary.id !== getBlankGuid()) {
            let libraryUserData: LibraryUser[] = [];
                treatmentLibraryUsers.forEach((treatmentLibraryUser, index) =>
                {   
                    //determine access level
                    let libraryUserAccessLevel: number = 0;
                    if (libraryUserAccessLevel == 0 && treatmentLibraryUser.isOwner == true) { libraryUserAccessLevel = 2; }
                    if (libraryUserAccessLevel == 0 && treatmentLibraryUser.canModify == true) { libraryUserAccessLevel = 1; }

                    //create library user object
                    let libraryUser: LibraryUser = {
                        userId: treatmentLibraryUser.userId,
                        userName: treatmentLibraryUser.username,
                        accessLevel: libraryUserAccessLevel
                    }

                    //add library user to an array
                    libraryUserData.push(libraryUser);
                });
                //update budget library sharing
                this.upsertOrDeleteTreatmentLibraryUsersAction({libraryId: this.selectedTreatmentLibrary.id, proposedUsers: libraryUserData});
                this.getIsSharedLibraryAction(this.selectedTreatmentLibrary).then(this.isShared = this.isSharedLibrary);
                this.onUpsertTreatmentLibrary();
        }
    }

    onSubmitConfirmDeleteTreatmentAlertResult(submit: boolean) {
        this.confirmBeforeDeleteTreatmentAlertData = clone(emptyAlertData);

        if (submit) {       
            this.onDeleteTreatment(this.selectedTreatment.id);
        }
    }


    onDeleteTreatment(treatmentId: string | number) {
        if(this.hasScenario)
        {         
            const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), this.simpleTreatments);
            this.deleteScenarioSelectableTreatmentAction({ scenarioSelectableTreatment: this.selectedTreatment, simulationId: this.selectedScenarioId, treatments}).then(() => {
                this.addedRows = this.addedRows.filter(_ => _.id !== treatmentId.toString());
            });
        }
        else
        {
            if (any(propEq('id', treatmentId.toString()), this.simpleTreatments)) {
                const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), this.simpleTreatments);            
                this.deleteTreatmentAction({ treatments: treatments, treatment: this.selectedTreatment, libraryId: this.selectedTreatmentLibrary.id}).then(() => {
                    this.addedRows = this.addedRows.filter(_ => _.id !== treatmentId.toString());
                });
            }            
        }                
    }

    onShowCreateTreatmentLibraryDialog(createAsNewLibrary: boolean) {
        this.createTreatmentLibraryDialogData = {
            showDialog: true,
            selectedTreatmentLibraryTreatments: createAsNewLibrary ? this.simpleTreatments.map(_ => {
                let treatment: Treatment = clone(emptyTreatment);
                treatment.name = _.name;
                treatment.id = _.id
                return treatment
            }) : [],
        };
    }

    onSubmitCreateTreatmentLibraryDialogResult(library: TreatmentLibrary) {
        this.createTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.treatments.length === 0 || !this.hasSelectedLibrary ? null :  this.selectedTreatmentLibrary.id, // setting id required for create as new library
                    rowsForDeletion: [],
                    updateRows: library.treatments.length === 0 ? [] : Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.treatments.length === 0 ? [] : this.addedRows,
                    isModified: false
                 },
                 scenarioId: this.hasScenario ? this.selectedScenarioId : null
            }
            TreatmentService.upsertTreatmentLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.hasCreatedLibrary = true;
                    this.librarySelectItemValue = library.id;
                    
                    if(library.treatments.length === 0){
                        this.clearChanges();
                    }

                    this.addedOrUpdatedTreatmentLibraryMutator(library);
                    this.selectedTreatmentLibraryMutator(library.id);
                    this.addSuccessNotificationAction({message:'Added treatment library'})
                }               
            })
        }
    }

    setParentLibraryName(libraryId: string) {
        if (libraryId === "None" || libraryId === this.uuidNIL) {
            this.parentLibraryName = "None";
            return;
        }
        let foundLibrary: TreatmentLibrary = emptyTreatmentLibrary;
        this.stateTreatmentLibraries.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        this.parentLibraryId = foundLibrary.id;
        this.parentLibraryName = foundLibrary.name;
    }

    onUpsertScenarioTreatments() {

        if (this.selectedTreatmentLibrary.id === this.uuidNIL || this.hasUnsavedChanges && this.newLibrarySelection ===false) {this.scenarioLibraryIsModified = true;}
        else { this.scenarioLibraryIsModified = false; }

        TreatmentService.upsertScenarioSelectedTreatments({
            libraryId: this.selectedTreatmentLibrary.id === this.uuidNIL ? null : this.selectedTreatmentLibrary.id,
            rowsForDeletion: [],
            updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
            addedRows: this.addedRows,
            isModified: this.scenarioLibraryIsModified,
        }, this.selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges();
                if(this.hasSelectedLibrary){
                    this.librarySelectItemValue = null;
                    this.getSimpleScenarioSelectableTreatmentsAction(this.selectedScenarioId)
                }
                this.treatmentCache.push(this.selectedTreatment);
                
                this.addSuccessNotificationAction({message: "Modified scenario's treatments"});   
                
                this.checkHasUnsavedChanges();
            }           
        });
        
    }

    onUpsertTreatmentLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: this.selectedTreatmentLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: this.selectedTreatmentLibrary.id === this.uuidNIL ? null : this.selectedTreatmentLibrary.id,
                rowsForDeletion: this.deletionIds,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                addedRows: this.addedRows,
                isModified: false
                },
                scenarioId: null
        }

        TreatmentService.upsertTreatmentLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                this.clearChanges();              
                this.addedOrUpdatedTreatmentLibraryMutator(this.selectedTreatmentLibrary);
                this.selectedTreatmentLibraryMutator(this.selectedTreatmentLibrary.id);
            }
        });
    }

    onAddTreatment(newTreatment: Treatment) {
        this.showCreateTreatmentDialog = false;

        if (!isNil(newTreatment)) {
            if(this.hasScenario)
                newTreatment.libraryId = this.parentLibraryId
            else 
                newTreatment.libraryId = this.selectedTreatmentLibrary.id
            this.addedRows = append(newTreatment, this.addedRows);
            this.simpleTreatments = append({name: newTreatment.name, id: newTreatment.id}, this.simpleTreatments);
            setTimeout(() => (this.treatmentSelectItemValue = newTreatment.id));
        }
    }

    modifySelectedTreatmentDetails(treatmentDetails: TreatmentDetails) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                description: treatmentDetails.description,
                shadowForAnyTreatment: treatmentDetails.shadowForAnyTreatment,
                shadowForSameTreatment: treatmentDetails.shadowForSameTreatment,
                criterionLibrary: treatmentDetails.criterionLibrary,
                category: treatmentDetails.category,
                assetType: treatmentDetails.assetType,
                isUnselectable: treatmentDetails.isUnselectable,
            });
        }
    }

    addSelectedTreatmentCost(newCost: TreatmentCost) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: prepend(newCost, this.selectedTreatment.costs),
            });
        }
    }

    modifySelectedTreatmentCost(modifiedCost: TreatmentCost) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: update(
                    findIndex(propEq('id', modifiedCost.id), this.selectedTreatment.costs,),
                    modifiedCost,
                    this.selectedTreatment.costs,
                ),
            });
        }
    }

    removeSelectedTreatmentCost(costId: string) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: reject(propEq('id', costId), this.selectedTreatment.costs,),
            });
        }
    }

    modifySelectedTreatmentPerformanceFactor(modifiedPerformanceFactor: TreatmentPerformanceFactor) {
        if (this.hasSelectedTreatment) {
            if (findIndex(propEq('id', modifiedPerformanceFactor.id), this.selectedTreatment.performanceFactors) < 0)
            {
                this.modifySelectedTreatment({
                    ...clone(this.selectedTreatment),
                    performanceFactors: prepend(modifiedPerformanceFactor, this.selectedTreatment.performanceFactors)
                });
            } else {
                this.modifySelectedTreatment({
                    ...clone(this.selectedTreatment),
                    performanceFactors: update(
                        findIndex(propEq('id', modifiedPerformanceFactor.id), this.selectedTreatment.performanceFactors),
                        modifiedPerformanceFactor,
                        this.selectedTreatment.performanceFactors,
                    ),
                });
            }
        }
    }

    addSelectedTreatmentConsequence(newConsequence: TreatmentConsequence) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: prepend(newConsequence, this.selectedTreatment.consequences,),
            });
        }
    }

    modifySelectedTreatmentConsequence(modifiedConsequence: TreatmentConsequence,) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: update(
                    findIndex(propEq('id', modifiedConsequence.id), this.selectedTreatment.consequences,),
                    modifiedConsequence,
                    this.selectedTreatment.consequences,
                ),
            });
        }
    }

    removeSelectedTreatmentConsequence(consequenceId: string) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: reject(propEq('id', consequenceId), this.selectedTreatment.consequences,),
            });
        }
    }

    modifySelectedTreatmentBudgets(simpleBudgetDetails: SimpleBudgetDetail[]) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                budgetIds: getPropertyValues('id', simpleBudgetDetails,) as string[],
            });
        }
    }

    modifySelectedTreatment(treatment: Treatment) {
    this.selectedTreatment = treatment;
    this.onUpdateRow(treatment.id, treatment);
    this.checkHasUnsavedChanges();
}

    onDiscardChanges() {
        this.treatmentSelectItemValue = "";
        this.librarySelectItemValue = "";
        setTimeout(() => {
            if (this.hasScenario) {       
                this.clearChanges();        
                this.simpleTreatments = clone(this.stateSimpleScenarioSelectableTreatments);
            }
        });
        this.parentLibraryName = this.loadedParentName;
        this.parentLibraryId = this.loadedParentId;
    }

    reset(){
        this.treatmentSelectItemValue = "";
        this.librarySelectItemValue = "";
        this.clearChanges();        
        this.simpleTreatments = clone(this.stateSimpleScenarioSelectableTreatments);
    }

    onShowConfirmDeleteAlert() {
        this.confirmBeforeDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmBeforeDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.librarySelectItemValue = "";
            this.deleteTreatmentLibraryAction({ libraryId: this.selectedTreatmentLibrary.id, });            
        }
    }

    disableCrudButtons() {
        const rows = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid: boolean = rows.every((treatment: Treatment) => {
            const allSubDataIsValid: boolean = treatment.consequences.every((consequence: TreatmentConsequence) => {
                    return (this.rules['generalRules'].valueIsNotEmpty(consequence.attribute,) === true &&
                        this.rules['treatmentRules']
                            .hasChangeValueOrEquation(consequence.changeValue, consequence.equation.expression,) === true
                    );
                },
            );

            return allSubDataIsValid && this.rules['generalRules'].valueIsNotEmpty(treatment.name) === true &&
                this.rules['generalRules'].valueIsNotEmpty(treatment.shadowForAnyTreatment) === true &&
                this.rules['generalRules'].valueIsNotEmpty(treatment.shadowForSameTreatment) === true;
        });

        if (this.hasSelectedLibrary) {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedTreatmentLibrary.name) === true &&
                allDataIsValid);
        }

        this.disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    onSubmitNewTreatment(result: ImportNewTreatmentDialogResult){
        this.showImportTreatmentDialog = false;
        
        if(this.hasScenario){
            this.newTreatment.libraryId = this.parentLibraryId
            this.newTreatment.name = result.file.name.slice(0, -5);
            this.addedRows = append(this.newTreatment, this.addedRows);
            this.simpleTreatments = append({name: result.file.name.slice(0, -5), id: this.selectedScenarioId}, this.simpleTreatments);
            setTimeout(() => (this.treatmentSelectItemValue = this.newTreatment.id));
        }
        else{
            this.newTreatment.libraryId = this.selectedTreatmentLibrary.id
            this.newTreatment.name = result.file.name.slice(0, -5);
            this.addedRows = append(this.newTreatment, this.addedRows);
            this.simpleTreatments = append({name: result.file.name.slice(0, -5), id: this.newTreatment.libraryId}, this.simpleTreatments);
            setTimeout(() => (this.treatmentSelectItemValue = this.newTreatment.id));
        }
            
            if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file 
            };
            if (this.hasScenario) {
                TreatmentService.importScenarioTreatments(data.file, this.newTreatment.libraryId, this.hasScenario)
            }
            else{
                TreatmentService.importLibraryTreatments(data.file, this.selectedTreatmentLibrary.id, this.hasScenario)
            }
            this.hasImport = true;
        }

            

    }

     onSubmitImportTreatmentsDialogResult(result: ImportExportTreatmentsDialogResult) {
        this.showImportTreatmentsDialog = false;

        if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file
            };

            if (this.hasScenario) {
                this.importScenarioTreatmentsFileAction({
                    ...data,
                    id: this.selectedScenarioId
                }).then(() => {
                                   
                });
            } else {
                this.importLibraryTreatmentsFileAction({
                    ...data,
                    id: this.selectedTreatmentLibrary.id
                }).then(() => {
                                   
                });;
            }
        }
     }

     OnExportTreamentsClick(){
        const id: string = this.hasScenario ? this.selectedScenarioId : this.selectedTreatmentLibrary.id;
        TreatmentService.exportTreatments(id, this.hasScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

     OnDownloadTemplateClick()
    {
        TreatmentService.downloadTreatmentsTemplate(this.hasScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioTreatment && importComp.id === this.selectedScenarioId ||
            this.hasSelectedLibrary && importComp.workType === WorkType.ImportLibraryTreatment && importComp.id === this.selectedTreatmentLibrary.id){
            this.clearChanges()
            this.getTreatmentLibrariesAction().then(async () => {
                if(this.hasScenario){
                    await this.getSimpleScenarioSelectableTreatmentsAction(this.selectedScenarioId);
                    this.onDiscardChanges();
                }  
                else{
                    await this.getSimpleSelectableTreatmentsAction(this.selectedTreatmentLibrary.id);
                }  
                this.setAlertMessageAction('');             
            })
        }        
    }

    //paging

    onUpdateRow(rowId: string, updatedRow: Treatment){
        if(any(propEq('id', rowId), this.addedRows)){
            const index = this.addedRows.findIndex(item => item.id == updatedRow.id)
            this.addedRows[index] = updatedRow;
            return;
        }
        let mapEntry = this.updatedRowsMap.get(rowId)
        if(isNil(mapEntry)){
            const row = this.treatmentCache.find(r => r.id === rowId);
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
        this.treatmentCache = [];
    }

    checkHasUnsavedChanges(){
    const hasUnsavedChanges: boolean =
        this.addedRows.length > 0 ||
        this.updatedRowsMap.size > 0 ||
        (this.hasScenario && this.hasSelectedLibrary) ||
        (this.hasSelectedLibrary && hasUnsavedChangesCore('', this.stateSelectedTreatmentLibrary, this.selectedTreatmentLibrary)) 
    this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
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
}
</script>

<style>
.treatment-editor-container {
    height: 730px;
    overflow-x: hidden;
    overflow-y: auto;
}

.treatments-div {
    height: 440px;
}

.card-tab-content {
    height: 430px;
    overflow-x: hidden;
    overflow-y: auto;
    border: none;
}

.sharing label {
    padding-top: 0.7em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}

.treatments-list {
    height: 470px;
    width: 400px;
    overflow-y: auto;
}
.treatment-parent {
    padding-bottom: 20px;
}

.selected-treatment-item {
    background: lightblue;
}
</style>
