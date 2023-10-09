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
                        variant="outlined"
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
                        variant="outlined"
                        v-model='treatmentSelectItemValue'
                    >
                    </v-select>
                </v-flex>
                <v-flex style="padding-right: 5px">
                    <v-btn
                        @click='showImportTreatmentDialog = true'
                        variant = "flat"
                        class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding ghd-margin-top'                        
                        v-show='hasSelectedLibrary'                        
                    >
                        Import Treatment
                    </v-btn>
                </v-flex>
                <v-flex style="padding-right: 5px">
                    <v-btn
                        @click='onShowConfirmDeleteTreatmentAlert'
                        variant = "flat"
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
                        variant = "outlined"

                    >
                        Create New Library
                    </v-btn>                                                          
                </v-flex>
            </v-layout>

            <v-flex xs6>
                    <v-layout v-if='hasSelectedLibrary && !hasScenario' style="padding-bottom: 50px !important">
                        <div class="ghd-control-label">
                        Owner: <v-label>{{ getOwnerUserName() || '[ No Owner ]' }}</v-label> | Date Modified: {{ modifiedDate }}   
                        <v-badge v-show="isShared">
                            <template v-slot: badge>
                                <span>Shared</span>
                            </template>
                        </v-badge>
                        <v-btn @click='onShowTreatmentLibraryDialog(selectedTreatmentLibrary)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
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
                    variant = "flat"
                    class='ghd-white-bg ghd-blue ghd-button-text ghd-text-padding'                              
                    style='float:left;'
                >
                    <span class="ghd-right-padding">Add Treatment</span>
                    <v-icon>fas fa-plus</v-icon>
                </v-btn>                
                <v-btn :disabled='false' @click='OnDownloadTemplateClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download Template
                </v-btn> 
                <label style='float:right;padding-top:13px;' class="ghd-grey" v-show ='hasSelectedLibrary && !hasScenario'>|</label>
                <v-btn :disabled='false' @click='OnExportTreamentsClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download
                </v-btn> 
                <label style='float:right;padding-top:13px;' class="ghd-grey" v-show ='hasSelectedLibrary && !hasScenario'>|</label>
                <v-btn :disabled='false' @click='showImportTreatmentsDialog = true'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Upload
                </v-btn>
            </div>    
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <v-layout>
                <div xs2>
                    <v-flex>
                        <v-list class='treatments-list'>
                            <template v-for='treatmentSelectItem in treatmentSelectItems' :key='treatmentSelectItem.value'>
                                <v-list-tile ripple :class="{'selected-treatment-item': isSelectedTreatmentItem(treatmentSelectItem.value)}"
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
                                        <v-window-item>
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
                                        </v-window-item>
                                        <v-window-item>
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
                                        </v-window-item>
                                        <v-window-item>
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
                                        </v-window-item>
                                        <v-window-item>
                                            <v-card>
                                                <v-card-text
                                                    class='card-tab-content'
                                                >
                                                    <PerformanceFactorTab
                                                        :selectedTreatmentPerformanceFactors='selectedTreatment.performanceFactors'
                                                        :selectedTreatment='selectedTreatment'
                                                        :scenarioId='loadedScenarioId'
                                                        :rules='rules'
                                                        :callFromScenario='hasScenario'
                                                        :callFromLibrary='!hasScenario'
                                                        @onModifyPerformanceFactor='modifySelectedTreatmentPerformanceFactor'
                                                    />
                                                </v-card-text>
                                            </v-card>
                                        </v-window-item>
                                        <v-window-item>
                                            <v-card>
                                                <v-card-text class='card-tab-content'>
                                                    <BudgetsTab :selectedTreatmentBudgets='selectedTreatment.budgetIds'
                                                                :addTreatment='selectedTreatment.addTreatment'
                                                                :fromLibrary='hasSelectedLibrary'
                                                                @onModifyBudgets='modifySelectedTreatmentBudgets' />
                                                </v-card-text>
                                            </v-card>
                                        </v-window-item>
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
                        @update:model-value="checkHasUnsavedChanges()"
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs9>
            <v-layout justify-center row v-show='(hasSelectedLibrary || hasScenario)'>
                <v-btn :disabled='!hasUnsavedChanges'
                    @click='onDiscardChanges'
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    variant = "flat"
                    v-show='hasScenario'
                >
                    Cancel
                </v-btn>
                <v-btn id='TreatmentEditor-deleteLibrary-btn' variant = "outline"
                    @click='onShowConfirmDeleteAlert'
                    class='ghd-white-bg ghd-blue ghd-button-text'

                    v-show='!hasScenario'
                    :disabled='!hasLibraryEditPermission'
                >
                    Delete Library
                </v-btn>
                <v-btn
                    @click='onShowCreateTreatmentLibraryDialog(true)'
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                    variant = "outlined"
                    :disabled='disableCrudButtons()'
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click='onUpsertScenarioTreatments'
                    class='ghd-blue-bg ghd-white ghd-button-text'
                    variant = "flat"
                    v-show='hasScenario'
                    :disabled='disableCrudButtonsResult || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn
                    id="TreatmentEditor-updateLibrary-btn"
                    @click='onUpsertTreatmentLibrary'
                    class='ghd-blue-bg ghd-white ghd-button-text  ghd-text-padding'
                    variant = "flat"
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

<script lang='ts' setup>
import Vue, { ShallowRef, shallowRef } from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, Ref} from 'vue';
import { useStore } from 'vuex';
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
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import {hasUnsavedChangesCore, isEqual } from '@/shared/utils/has-unsaved-changes-helper';
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
import { useRouter } from 'vue-router';
import mitt from 'mitt';

    const emit = defineEmits(['submit'])
    const $statusHub = inject('$statusHub') as any
    const $emitter = mitt()
    const $router = useRouter();
    let store = useStore();
    let stateTreatmentLibraries = ref<TreatmentLibrary[]>(store.state.attributeModule.attributes);
    let stateSelectedTreatmentLibrary = ref<TreatmentLibrary>(store.state.treatmentModule.selectedTreatmentLibrary);
    let stateScenarioSelectableTreatment = ref<Treatment[]>(store.state.treatmentModule.scenarioSelectableTreatments);
    let hasUnsavedChanges = ref<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let stateScenarioTreatmentLibrary= ref<TreatmentLibrary>(store.state.treatmentModule.scenarioTreatmentLibrary);
    let stateScenarioSimpleBudgetDetails = ref<SimpleBudgetDetail[]>(store.state.investmentModule.scenarioSimpleBudgetDetails);
    let hasAdminAccess= ref<boolean>(store.state.authenticationModule.hasAdminAccess);
    let hasPermittedAccess= ref<boolean>(store.state.treatmentModule.hasPermittedAccess);
    let stateSimpleScenarioSelectableTreatments= ref<SimpleTreatment[]>(store.state.treatmentModule.simpleScenarioSelectableTreatmentss);
    let stateSimpleSelectableTreatments= ref<SimpleTreatment[]>(store.state.treatmentModule.simpleSelectableTreatments);
    let isSharedLibrary= ref<boolean>(store.state.treatmentModule.isSharedLibrary);
    let stateScenarioPerformanceCurves= ref<PerformanceCurve[]>(store.state.performanceCurveModule.scenarioPerformanceCurves);

    async function addSuccessNotificationAction(payload?: any): Promise<any> {
        await store.dispatch('addSuccessNotification');
}
    async function addWarningNotificationAction(payload?: any): Promise<any> {
  await store.dispatch('addWarningNotification');
}

async function addErrorNotificationAction(payload?: any): Promise<any> {
  await store.dispatch('addErrorNotification');
}

async function addInfoNotificationAction(payload?: any): Promise<any> {
  await store.dispatch('addInfoNotification');
}

async function getTreatmentLibrariesAction(payload?: any): Promise<any> {
  await store.dispatch('getTreatmentLibraries');
}

async function selectTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('selectTreatmentLibrary');
}

async function upsertTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('upsertTreatmentLibrary');
}

async function deleteTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('deleteTreatmentLibrary');
}

async function getSimpleScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getSimpleScenarioSelectableTreatments');
}

async function getSimpleSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getSimpleSelectableTreatments');
}

async function getTreatmentLibraryBySimulationIdAction(payload?: any): Promise<any> {
  await store.dispatch('getTreatmentLibraryBySimulationId');
}

async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioSimpleBudgetDetails');
}

async function setHasUnsavedChangesAction(payload?: any): Promise<any> {
  await store.dispatch('setHasUnsavedChanges');
}

async function getScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioSelectableTreatments');
}

async function upsertScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('upsertScenarioSelectableTreatments');
}

async function upsertOrDeleteTreatmentLibraryUsersAction(payload?: any): Promise<any> {
  await store.dispatch('upsertOrDeleteTreatmentLibraryUsers');
}

async function importScenarioTreatmentsFileAction(payload?: any): Promise<any> {
  await store.dispatch('importScenarioTreatmentsFile');
}

async function importLibraryTreatmentsFileAction(payload?: any): Promise<any> {
  await store.dispatch('importLibraryTreatmentsFile');
}

async function deleteTreatmentAction(payload?: any): Promise<any> {
  await store.dispatch('deleteTreatment');
}

async function deleteScenarioSelectableTreatmentAction(payload?: any): Promise<any> {
  await store.dispatch('deleteScenarioSelectableTreatment');
}

async function getIsSharedLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('getIsSharedTreatmentLibrary');
}

async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {
  await store.dispatch('getCurrentUserOrSharedScenario');
}

async function selectScenarioAction(payload?: any): Promise<any> {
  await store.dispatch('selectScenario');
}

async function getScenarioPerformanceCurvesAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioPerformanceCurves');
}

async function setAlertMessageAction(payload?: any): Promise<any> {
  await store.dispatch('setAlertMessage');
}

async function getUserNameByIdGetter(payload?: any): Promise<any> {
  return await store.getters.getUserNameById(payload);
}

async function addedOrUpdatedTreatmentLibraryMutator(payload?: any): Promise<any> {
  await store.commit('addedOrUpdatedTreatmentLibraryMutator', payload);
}

async function selectedTreatmentLibraryMutator(payload?: any): Promise<any> {
  await store.commit('selectedTreatmentLibraryMutator', payload);
}


    let selectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
    let treatments: Treatment[] = [];
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let treatmentSelectItems: SelectItem[] = [];
    let treatmentSelectItemValue: string ="";
    let selectedTreatment: Treatment = clone(emptyTreatment);
    let selectedTreatmentDetails: TreatmentDetails = clone(emptyTreatmentDetails);
    let activeTab: number = 0;
    let treatmentTabs: string[] = ['Treatment Details', 'Costs', 'Performance Factor', 'Consequences'];
    let createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = clone(
        emptyCreateTreatmentLibraryDialogData,
    );
    let showCreateTreatmentDialog: boolean = false;
    let showImportTreatmentDialog: boolean = false;
    let confirmBeforeDeleteAlertData: AlertData = clone(emptyAlertData);
    let hasSelectedTreatment: boolean = false;
    let rules: InputValidationRules = shallowRef<InputValidationRules>();
    let uuidNIL: string = getBlankGuid();
    let keepActiveTab: boolean = false;
    let hasScenario: boolean = false;
    let budgets: SimpleBudgetDetail[] = [];
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean | Promise<boolean> = false;
    let showImportTreatmentsDialog: boolean = false;
    let confirmBeforeDeleteTreatmentAlertData: AlertData = clone(emptyAlertData);
    let isNoTreatmentSelected: boolean = false;
    let hasImport: boolean = false;
    let modifiedDate: string = '';

    let deletionIds: ShallowRef<string[]> = ref([]);
    let addedRows: Treatment[] = [];
    let updatedRowsMap:Map<string, [Treatment, Treatment]> = new Map<string, [Treatment, Treatment]>();//0: original value | 1: updated value
    let rowCache: Treatment[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let isPageInit = false;
    let totalItems = 0;
    let currentPage: Treatment[] = [];
    let initializing: boolean = true;

    let simpleTreatments: SimpleTreatment[] = [];
    let isShared: boolean = false;
    let treatmentCache: Treatment[] = [];

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string | null = '';
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue: string | null = '';

    let shareTreatmentLibraryDialogData: ShareTreatmentLibraryDialogData = clone(emptyShareTreatmentLibraryDialogData);
    let loadedScenarioId: string = '';
    let parentLibraryId: string  = uuidNIL;
    let parentLibraryName: string = 'None';
    let scenarioParentLIbrary: string | null = null;
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string  = uuidNIL;
    let newLibrarySelection: boolean = false;
    let newTreatment: Treatment = {...emptyTreatment, id: getNewGuid(), addTreatment: false};

    
    beforeRouteEnter();
    function beforeRouteEnter() {
        (() => {
            librarySelectItemValue = "";
            getTreatmentLibrariesAction();
            if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.Treatment) !== -1) {
                selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;
                loadedScenarioId = selectedScenarioId;
                if (selectedScenarioId === uuidNIL) {
                    addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    $router.push('/Scenarios/');
                }
                hasScenario = true;
                getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId);
                getTreatmentLibraryBySimulationIdAction(selectedScenarioId);
                getScenarioPerformanceCurvesAction(selectedScenarioId);
                treatmentTabs = [...treatmentTabs, 'Budgets'];
                getScenarioSimpleBudgetDetailsAction({ scenarioId: selectedScenarioId, }).then(()=> {
                    getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                        selectScenarioAction({ scenarioId: selectedScenarioId });   
                    });
                });
                ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioTreatment}).then(response => {
                    if(response.data){
                        setAlertMessageAction("A treatment curve has been added to the work queue")
                    }
                })
            }
        });
    }

    onMounted(() => mounted());
    function mounted() {
            $emitter.on(
                Hub.BroadcastEventType.BroadcastImportCompletionEvent,
                importCompleted,
            );
        } 
    onBeforeUnmount(() => beforeDestroy());
     function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
        $emitter.off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
        setAlertMessageAction('');
    }  

    watch('stateScenarioSimpleBudgetDetails',  () => onStateScenarioInvestmentLibraryChanged)
    function onStateScenarioInvestmentLibraryChanged() {
        budgets = clone(stateScenarioSimpleBudgetDetails.value);
    }

    
   watch('stateTreatmentLibraries', () => onStateTreatmentLibrariesChanged)
   function onStateTreatmentLibrariesChanged() {
        librarySelectItems = stateTreatmentLibraries.value.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id,
            })
        );
    }

    watch('stateScenarioPerformanceCurves', () => onStateScenarioPerformanceCurvesChanged)
    function onStateScenarioPerformanceCurvesChanged() {

    }
    
    watch('stateScenarioTreatmentLibrary', () => onStateScenarioTreatmentLibraryChanged)
    function onStateScenarioTreatmentLibraryChanged() {
        setParentLibraryName(stateScenarioTreatmentLibrary ? stateScenarioTreatmentLibrary.value.id : "None");
        scenarioLibraryIsModified = stateScenarioTreatmentLibrary ? stateScenarioTreatmentLibrary.value.isModified : false;
        loadedParentId = stateScenarioTreatmentLibrary ? stateScenarioTreatmentLibrary.value.id : uuidNIL;
        loadedParentName = parentLibraryName;
    }

    watch('librarySelectItemValue', () => onLibrarySelectItemValueChangedCheckUnsaved)
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario){
            onSelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged)
            CheckUnsavedDialog(onSelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue = trueLibrarySelectItemValue;               
            })
        librarySelectItemValueAllowedChanged = true;
        setParentLibraryName(librarySelectItemValue ? librarySelectItemValue : parentLibraryId);
        scenarioLibraryIsModified = false;
        newLibrarySelection = true;
    }
    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue;
        selectTreatmentLibraryAction({
            libraryId: librarySelectItemValue,
        });
    
        if(!isNil(librarySelectItemValue)){
            if (!isEmpty(librarySelectItemValue)){
                getSimpleSelectableTreatmentsAction(librarySelectItemValue);
            }
        }           
    }  

    watch('stateSimpleSelectableTreatments', () => onStateSimpleSelectableTreatments)
    function onStateSimpleSelectableTreatments() {
        simpleTreatments = clone(stateSimpleSelectableTreatments.value);
    }

    watch('stateSimpleScenarioSelectableTreatments', () => onStateSimpleScenarioSelectableTreatments)
    function onStateSimpleScenarioSelectableTreatments(){
        simpleTreatments = clone(stateSimpleScenarioSelectableTreatments.value);
    }

    watch('stateSelectedTreatmentLibrary', () => onStateSelectedTreatmentLibraryChanged)
    function onStateSelectedTreatmentLibraryChanged() {
        selectedTreatmentLibrary = clone(
            stateSelectedTreatmentLibrary.value
        );
    }

    watch('isSharedLibrary', () => onStateSharedAccessChanged)
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }

    watch('selectedTreatmentLibrary', () => onSelectedTreatmentLibraryChanged)
    function onSelectedTreatmentLibraryChanged() {
        hasSelectedLibrary = selectedTreatmentLibrary.id !== uuidNIL;
        getIsSharedLibraryAction(selectedTreatmentLibrary).then(() => isShared = isSharedLibrary.value);
        if (hasSelectedLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedTreatmentLibrary.id, workType: WorkType.ImportLibraryTreatment}).then(response => {
                if(response.data){
                    setAlertMessageAction("A treatment import has been added to the work queue")
                }
                else
                    setAlertMessageAction("");
            })
        }

        clearChanges();
        if(treatmentSelectItemValue !== null && !hasScenario)
            treatmentCache.push(clone(selectedTreatment))
        checkHasUnsavedChanges();
    }

    watch('simpleTreatments', onSimpleTreatments)
    function onSimpleTreatments(){
        treatmentSelectItems = simpleTreatments.map((treatment: SimpleTreatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));

        checkHasUnsavedChanges()

        treatmentSelectItemValue = "";
    }

    watch('treatments', () => onSelectedScenarioTreatmentsChanged)
   function onSelectedScenarioTreatmentsChanged() {
        treatmentSelectItems = treatments.map((treatment: Treatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));       
        hasUnsavedChanges.value = false;
        disableCrudButtons();
    }

    watch('treatmentSelectItemValue', () => onTreatmentSelectItemValueChanged)
    function onTreatmentSelectItemValueChanged() {
        if(!isNil(treatmentSelectItemValue) && treatmentSelectItemValue !== ""){
            var mapEntry = updatedRowsMap.get(treatmentSelectItemValue);
            var addedRow = addedRows.find(_ => _.id == treatmentSelectItemValue);
            var treatment = treatmentCache.find(_ => _.id === treatmentSelectItemValue);

            if(!isNil(mapEntry)){
                selectedTreatment = clone(mapEntry[1]);
            }
            else if(!isNil(addedRow)){
                selectedTreatment = clone(addedRow);
            }               
            else if(hasSelectedLibrary)
                TreatmentService.getSelectedTreatmentById(treatmentSelectItemValue).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        selectedTreatment = data;
                        if(isNil(treatmentCache.find(_ => _.id === data.id)))
                            treatmentCache.push(data)
                    }
                })
            }

            
            else if(!isNil(treatment))
                selectedTreatment = clone(treatment);
            else
                 selectedTreatment = clone(emptyTreatment);
                 if (!keepActiveTab) {
                     activeTab = 0;
                    }
                keepActiveTab = true;
                TreatmentService.getScenarioSelectedTreatmentById(treatmentSelectItemValue).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        selectedTreatment = data;
                        if(isNil(treatmentCache.find(_ => _.id === data.id))){ treatmentCache.push(data); }
                        scenarioLibraryIsModified = selectedTreatment ? selectedTreatment.isModified : false;
                    }
                })
        }
   
    

    watch('selectedTreatment', () => onSelectedTreatmentChanged())
    function onSelectedTreatmentChanged() {
        hasSelectedTreatment = selectedTreatment.id !== uuidNIL;

        selectedTreatmentDetails = {
            description: selectedTreatment.description,
            shadowForSameTreatment: selectedTreatment.shadowForSameTreatment,
            shadowForAnyTreatment: selectedTreatment.shadowForAnyTreatment,
            criterionLibrary: selectedTreatment.criterionLibrary,
            category: selectedTreatment.category,
            assetType: selectedTreatment.assetType,
            isUnselectable: selectedTreatment.isUnselectable,
        };

        isNoTreatmentSelected = selectedTreatment.name == 'No Treatment';
    }

    function isSelectedTreatmentItem(treatmentId: string | number) {
        return isEqual(treatmentSelectItemValue, treatmentId.toString());
    }

    async function getOwnerUserName(): Promise<string> {

        if (!hasCreatedLibrary) {
        return await getUserNameByIdGetter(selectedTreatmentLibrary.owner);
        }
        
        return getUserName();
    }

   function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    async function checkUserIsLibraryOwner() {
        return await getUserNameByIdGetter(selectedTreatmentLibrary.owner) == getUserName();
    }

    function clearChanges(){
        updatedRowsMap.clear();
        addedRows = [];
        treatmentCache = [];
    }

    function onSetTreatmentSelectItemValue(treatmentId: string | number) {
        if (!isEqual(treatmentSelectItemValue, treatmentId.toString())) {
            treatmentSelectItemValue = treatmentId.toString();
        }
    }
    
    function onShowConfirmDeleteTreatmentAlert() {
        confirmBeforeDeleteTreatmentAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function  onShowTreatmentLibraryDialog(treatmentLibrary: TreatmentLibrary) {
        shareTreatmentLibraryDialogData = {
            showDialog: true,
            treatmentLibrary: clone(treatmentLibrary)
        };
    }

    function onShareTreatmentLibraryDialogSubmit(treatmentLibraryUsers: TreatmentLibraryUser[]) {
        shareTreatmentLibraryDialogData = clone(emptyShareTreatmentLibraryDialogData);
        if (!isNil(treatmentLibraryUsers) && selectedTreatmentLibrary.id !== getBlankGuid()) {
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
                upsertOrDeleteTreatmentLibraryUsersAction({libraryId: selectedTreatmentLibrary.id, proposedUsers: libraryUserData});
                getIsSharedLibraryAction(selectedTreatmentLibrary).then(() => isShared = isSharedLibrary.value);
                onUpsertTreatmentLibrary();
        }
    }

    function onSubmitConfirmDeleteTreatmentAlertResult(submit: boolean) {
        confirmBeforeDeleteTreatmentAlertData = clone(emptyAlertData);

        if (submit) {       
            onDeleteTreatment(selectedTreatment.id);
        }
    }


    function onDeleteTreatment(treatmentId: string | number) {
        if(hasScenario)
        {         
            const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), simpleTreatments);
            deleteScenarioSelectableTreatmentAction({ scenarioSelectableTreatment: selectedTreatment, simulationId: selectedScenarioId, treatments}).then(() => {
                addedRows = addedRows.filter(_ => _.id !== treatmentId.toString());
            });
        }
        else
        {
            if (any(propEq('id', treatmentId.toString()), simpleTreatments)) {
                const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), simpleTreatments);            
                deleteTreatmentAction({ treatments: treatments, treatment: selectedTreatment, libraryId: selectedTreatmentLibrary.id}).then(() => {
                    addedRows = addedRows.filter(_ => _.id !== treatmentId.toString());
                });
            }            
        }                
    }

    function onShowCreateTreatmentLibraryDialog(createAsNewLibrary: boolean) {
        createTreatmentLibraryDialogData = {
            showDialog: true,
            selectedTreatmentLibraryTreatments: createAsNewLibrary ? simpleTreatments.map(_ => {
                let treatment: Treatment = clone(emptyTreatment);
                treatment.name = _.name;
                treatment.id = _.id
                return treatment
            }) : [],
        };
    }

    function  onSubmitCreateTreatmentLibraryDialogResult(library: TreatmentLibrary) {
        createTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.treatments.length === 0 || !hasSelectedLibrary ? null :  selectedTreatmentLibrary.id, // setting id required for create as new library
                    rowsForDeletion: [],
                    updateRows: library.treatments.length === 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.treatments.length === 0 ? [] : addedRows,
                    isModified: false
                 },
                 scenarioId: hasScenario ? selectedScenarioId : null
            }
            TreatmentService.upsertTreatmentLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue = library.id;
                    
                    if(library.treatments.length === 0){
                        clearChanges();
                    }

                    addedOrUpdatedTreatmentLibraryMutator(library);
                    selectedTreatmentLibraryMutator(library.id);
                    addSuccessNotificationAction({message:'Added treatment library'})
                }               
            })
        }
    }

    function setParentLibraryName(libraryId: string) {
        if (libraryId === "None" || libraryId === uuidNIL) {
            parentLibraryName = "None";
            return;
        }
        let foundLibrary: TreatmentLibrary = emptyTreatmentLibrary;
        stateTreatmentLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName = foundLibrary.name;
    }

    function onUpsertScenarioTreatments() {

        if (selectedTreatmentLibrary.id === uuidNIL || hasUnsavedChanges && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        TreatmentService.upsertScenarioSelectedTreatments({
            libraryId: selectedTreatmentLibrary.id === uuidNIL ? null : selectedTreatmentLibrary.id,
            rowsForDeletion: [],
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows,
            isModified: scenarioLibraryIsModified,
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();
                if(hasSelectedLibrary){
                    librarySelectItemValue = null;
                    getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId)
                }
                treatmentCache.push(selectedTreatment);
                
                addSuccessNotificationAction({message: "Modified scenario's treatments"});   
                
                checkHasUnsavedChanges();
            }           
        });
        
    }

    function onUpsertTreatmentLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: selectedTreatmentLibrary,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedTreatmentLibrary.id === uuidNIL ? null : selectedTreatmentLibrary.id,
                rowsForDeletion: deletionIds.value,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                addedRows: addedRows,
                isModified: false
                },
                scenarioId: null
        }

        TreatmentService.upsertTreatmentLibrary(upsertRequest).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();              
                addedOrUpdatedTreatmentLibraryMutator(selectedTreatmentLibrary);
                selectedTreatmentLibraryMutator(selectedTreatmentLibrary.id);
            }
        });
    }

    function onAddTreatment(newTreatment: Treatment) {
        showCreateTreatmentDialog = false;

        if (!isNil(newTreatment)) {
            if(hasScenario)
                newTreatment.libraryId = parentLibraryId
            else 
                newTreatment.libraryId = selectedTreatmentLibrary.id
            addedRows = append(newTreatment, addedRows);
            simpleTreatments = append({name: newTreatment.name, id: newTreatment.id}, simpleTreatments);
            setTimeout(() => (treatmentSelectItemValue = newTreatment.id));
        }
    }

    function modifySelectedTreatmentDetails(treatmentDetails: TreatmentDetails) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
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

    function addSelectedTreatmentCost(newCost: TreatmentCost) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                costs: prepend(newCost, selectedTreatment.costs),
            });
        }
    }

    function  modifySelectedTreatmentCost(modifiedCost: TreatmentCost) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                costs: update(
                    findIndex(propEq('id', modifiedCost.id), selectedTreatment.costs,),
                    modifiedCost,
                    selectedTreatment.costs,
                ),
            });
        }
    }

    function removeSelectedTreatmentCost(costId: string) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                costs: reject(propEq('id', costId), selectedTreatment.costs,),
            });
        }
    }

    function modifySelectedTreatmentPerformanceFactor(modifiedPerformanceFactor: TreatmentPerformanceFactor) {
        if (hasSelectedTreatment) {
            if (findIndex(propEq('id', modifiedPerformanceFactor.id), selectedTreatment.performanceFactors) < 0)
            {
                modifySelectedTreatment({
                    ...clone(selectedTreatment),
                    performanceFactors: prepend(modifiedPerformanceFactor, selectedTreatment.performanceFactors)
                });
            } else {
                modifySelectedTreatment({
                    ...clone(selectedTreatment),
                    performanceFactors: update(
                        findIndex(propEq('id', modifiedPerformanceFactor.id), selectedTreatment.performanceFactors),
                        modifiedPerformanceFactor,
                        selectedTreatment.performanceFactors,
                    ),
                });
            }
        }
    }

    function addSelectedTreatmentConsequence(newConsequence: TreatmentConsequence) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                consequences: prepend(newConsequence, selectedTreatment.consequences,),
            });
        }
    }

    function  modifySelectedTreatmentConsequence(modifiedConsequence: TreatmentConsequence,) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                consequences: update(
                    findIndex(propEq('id', modifiedConsequence.id), selectedTreatment.consequences,),
                    modifiedConsequence,
                    selectedTreatment.consequences,
                ),
            });
        }
    }

    function removeSelectedTreatmentConsequence(consequenceId: string) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                consequences: reject(propEq('id', consequenceId), selectedTreatment.consequences,),
            });
        }
    }

    function modifySelectedTreatmentBudgets(simpleBudgetDetails: SimpleBudgetDetail[]) {
        if (hasSelectedTreatment) {
            modifySelectedTreatment({
                ...clone(selectedTreatment),
                budgetIds: getPropertyValues('id', simpleBudgetDetails,) as string[],
            });
        }
    }

    function modifySelectedTreatment(treatment: Treatment) {
        selectedTreatment = treatment;

        onUpdateRow(treatment.id, treatment);
        checkHasUnsavedChanges();
    }

    function onDiscardChanges() {
        treatmentSelectItemValue = "";
        librarySelectItemValue = "";
        setTimeout(() => {
            if (hasScenario) {       
                clearChanges();        
                simpleTreatments = clone(stateSimpleScenarioSelectableTreatments.value);
            }
        });
        parentLibraryName = loadedParentName;
        parentLibraryId = loadedParentId;
    }

    function reset(){
        treatmentSelectItemValue = "";
        librarySelectItemValue = "";
        clearChanges();        
        simpleTreatments = clone(stateSimpleScenarioSelectableTreatments.value);
    }

    function onShowConfirmDeleteAlert() {
        confirmBeforeDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function onSubmitConfirmDeleteAlertResult(submit: boolean) {
        confirmBeforeDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            librarySelectItemValue = "";
            deleteTreatmentLibraryAction({ libraryId: selectedTreatmentLibrary.id, });            
        }
    }

    function disableCrudButtons() {
        const rows = addedRows.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const allDataIsValid: boolean = rows.every((treatment: Treatment) => {
            const allSubDataIsValid: boolean = treatment.consequences.every((consequence: TreatmentConsequence) => {
                    return (rules['generalRules'].valueIsNotEmpty(consequence.attribute,) === true &&
                        rules['treatmentRules']
                            .hasChangeValueOrEquation(consequence.changeValue, consequence.equation.expression,) === true
                    );
                },
            );

            return allSubDataIsValid && rules['generalRules'].valueIsNotEmpty(treatment.name) === true &&
                rules['generalRules'].valueIsNotEmpty(treatment.shadowForAnyTreatment) === true &&
                rules['generalRules'].valueIsNotEmpty(treatment.shadowForSameTreatment) === true;
        });

        if (hasSelectedLibrary) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedTreatmentLibrary.name) === true &&
                allDataIsValid);
        }

        disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    function onSubmitNewTreatment(result: ImportNewTreatmentDialogResult){
        showImportTreatmentDialog = false;
        
        if(hasScenario){
            newTreatment.libraryId = parentLibraryId
            newTreatment.name = result.file.name.slice(0, -5);
            addedRows = append(newTreatment, addedRows);
            simpleTreatments = append({name: result.file.name.slice(0, -5), id: selectedScenarioId}, simpleTreatments);
            setTimeout(() => (treatmentSelectItemValue = newTreatment.id));
        }
        else{
            newTreatment.libraryId = selectedTreatmentLibrary.id
            newTreatment.name = result.file.name.slice(0, -5);
            addedRows = append(newTreatment, addedRows);
            simpleTreatments = append({name: result.file.name.slice(0, -5), id: newTreatment.libraryId}, simpleTreatments);
            setTimeout(() => (treatmentSelectItemValue = newTreatment.id));
        }
            
            if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file 
            };
            if (hasScenario) {
                TreatmentService.importScenarioTreatments(data.file, newTreatment.libraryId, hasScenario)
            }
            else{
                TreatmentService.importLibraryTreatments(data.file, selectedTreatmentLibrary.id, hasScenario)
            }
            hasImport = true;
        }
    }

     function onSubmitImportTreatmentsDialogResult(result: ImportExportTreatmentsDialogResult) {
        showImportTreatmentsDialog = false;

        if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file
            };

            if (hasScenario) {
                importScenarioTreatmentsFileAction({
                    ...data,
                    id: selectedScenarioId
                }).then(() => {
                                   
                });
            } else {
                importLibraryTreatmentsFileAction({
                    ...data,
                    id: selectedTreatmentLibrary.id
                }).then(() => {
                                   
                });;
            }
        }
     }

    function OnExportTreamentsClick(){
        const id: string = hasScenario ? selectedScenarioId : selectedTreatmentLibrary.id;
        TreatmentService.exportTreatments(id, hasScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

     function OnDownloadTemplateClick()
    {
        TreatmentService.downloadTreatmentsTemplate(hasScenario)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioTreatment && importComp.id === selectedScenarioId ||
            hasSelectedLibrary && importComp.workType === WorkType.ImportLibraryTreatment && importComp.id === selectedTreatmentLibrary.id){
            clearChanges()
            getTreatmentLibrariesAction().then(async () => {
                if(hasScenario){
                    await getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId);
                    onDiscardChanges();
                }  
                else{
                    await getSimpleSelectableTreatmentsAction(selectedTreatmentLibrary.id);
                }  
                setAlertMessageAction('');             
            })
        }        
    }

    //paging

    function onUpdateRow(rowId: string, updatedRow: Treatment){
        if(any(propEq('id', rowId), addedRows)){
            const index = addedRows.findIndex(item => item.id == updatedRow.id)
            addedRows[index] = updatedRow;
            return;
        }
        let mapEntry = updatedRowsMap.get(rowId)
        if(isNil(mapEntry)){
            const row = treatmentCache.find(r => r.id === rowId);
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

    function learChanges(){
        updatedRowsMap.clear();
        addedRows = [];
        treatmentCache = [];
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = 
            addedRows.length > 0 ||
            updatedRowsMap.size > 0 || 
            (hasScenario && hasSelectedLibrary) ||
            (hasSelectedLibrary && hasUnsavedChangesCore('', stateSelectedTreatmentLibrary, selectedTreatmentLibrary))
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
