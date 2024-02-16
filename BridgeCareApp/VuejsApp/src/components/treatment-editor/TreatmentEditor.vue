<template>
    <v-card height="1000px" class="elevation-0 vcard-main-layout" style="margin-top: -20px;">    
    <v-row style="margin-top: 5px;">
        <v-col>
            <v-select
                id="TreatmentEditor-treatmentLibrary-select"
                :items='librarySelectItems'
                menu-icon=custom:GhdDownSvg
                class='ghd-control-border ghd-control-text ghd-control-width-dd ghd-select'
                label='Select a Treatment Library'
                variant="outlined"
                density="compact"
                item-title="text"
                item-value="value"
                v-model='librarySelectItemValue' 
            >
            </v-select>
            <div class="ghd-md-gray ghd-control-subheader treatment-parent" v-if='hasScenario'><b>Library Used: {{parentLibraryName}}<span v-if="scenarioLibraryIsModified">&nbsp;(Modified)</span></b></div>
        </v-col>
        <v-col>
            <v-select
            id="TreatmentEditor-treatment-select"
                :items='treatmentSelectItems'
                menu-icon=custom:GhdDownSvg
                class='ghd-control-border ghd-control-text ghd-control-width-dd ghd-select'
                label='Select a Treatment'
                variant="outlined"
                density="compact"
                item-title="text"
                item-value="value"
                v-model='treatmentSelectItemValue'
                v-show='hasSelectedLibrary || hasScenario'
            >
            </v-select>
        </v-col>
        <v-col class="ghd-blue ghd-button-text ghd-text-padding" v-if='hasSelectedLibrary' style="border-style: solid;border-width: 2px; border-color: lightgray;margin-right: 5px;margin-bottom: 50px;">Treatments<br>
            <v-btn :disabled='false' @click='OnDownloadTemplateClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download Template
                </v-btn> 
                <v-btn :disabled='false' @click='OnExportTreamentsClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download
                </v-btn> 
                <v-btn :disabled='false' @click='showImportTreatmentsDialog = true'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Upload
            </v-btn>
        </v-col>        
        <v-col class="ghd-blue ghd-button-text ghd-text-padding" v-if='hasSelectedLibrary' style="border-style: solid;border-width: 2px; border-color: lightgray;margin-bottom: 50px;">Supersede<br>
            <v-btn :disabled='false' @click='OnDownloadSupersedeTemplateClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download Template
                </v-btn> 
                <v-btn :disabled='false' @click='OnExportSupersedeClick()'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Download
                </v-btn>
                <v-btn :disabled='false' @click='showImportSupersedeDialog = true'
                variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                    style='float:right;'
                    >
                    Upload
            </v-btn>
        </v-col>
    </v-row><br>    
    <v-row>
        <v-col style="padding-right: 5px;margin-top: -50px;">                       
            <v-btn
                @click='showImportTreatmentDialog = true'
                variant = "outlined"
                style="margin-right: 5px;"
                class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding ghd-margin-top'                        
                v-show='hasSelectedLibrary'                        
            >
                Import Treatment
            </v-btn>
            <v-btn
                @click='onShowConfirmDeleteTreatmentAlert'
                variant = "outlined"
                style="margin-right: 5px; margin-left: 5px;"
                class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding ghd-margin-top'                        
                v-show='hasSelectedTreatment && !isNoTreatmentSelected'                        
            >
                Delete Treatment
            </v-btn>
            <v-btn
                id="TreatmentEditor-createLibrary-btn"
                @click='onShowCreateTreatmentLibraryDialog(false)'
                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-margin-top'
                style="margin-left: 5px;"
                v-show="!hasScenario"
                variant = "outlined"
            >
                Create New Library
            </v-btn>                                                          
        </v-col>
    </v-row>
        <v-col cols="auto">
            <v-row v-if='hasSelectedLibrary && !hasScenario' style="margin-top: 10px; !important">
                <div class="header-text-content owner-padding">
                    Owner: {{ getOwnerUserName() || '[ No Owner ]' }} | Date Modified: {{ modifiedDate }}   
                    <v-btn @click='onShowTreatmentLibraryDialog(selectedTreatmentLibrary)'
                        style=" margin-left: 10px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        v-show='!hasScenario'>
                        Share Library
                    </v-btn>
                </div>  
            </v-row>
        </v-col>
        <v-divider style="margin-top:10px" v-show='hasSelectedLibrary || hasScenario'></v-divider>        
        <v-row>
            <div v-show='hasSelectedLibrary || hasScenario' style="width:100%;margin-top:0px;margin-bottom:-15px; margin-left: 20px;">                
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
            </div>    
            <div>
                <v-col>
                    <v-list class='treatments-list'>
                        <template v-for='treatmentSelectItem in treatmentSelectItems' :key='treatmentSelectItem.value'>
                            <v-list-item ripple :class="{'selected-treatment-item': isSelectedTreatmentItem(treatmentSelectItem.value)}"
                                        avatar @click='onSetTreatmentSelectItemValue(treatmentSelectItem.value)'>
                                <v-list-item-content class ="item-content">
                                    <span>{{treatmentSelectItem.text}}</span>
                                    <v-btn flat style="margin-left: 10px; background-color: transparent;" v-show="treatmentSelectItem.text!='No Treatment'" @click="onShowConfirmDeleteTreatmentAlert" class="ghd-blue">
                                        <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </v-list-item-content>
                            </v-list-item>
                        </template>
                    </v-list>
                </v-col>
            </div>
            <v-col >               
                <div v-show='selectedTreatment.id !== uuidNILRef'>                                                
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
                    </v-tabs>
                    <v-window v-model='activeTab'> 
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
                                <SupersedeTab
                                        :selectedTreatmentSupersedeRules='selectedTreatment.supersedeRules'
                                        :treatmentSelectItems='treatmentSelectItems'
                                        :rules='rules'
                                        :callFromScenario='hasScenario'
                                        :callFromLibrary='!hasScenario'                                        
                                        @onAddSupersedeRule='addSelectedTreatmentSupersedeRule'
                                        @onModifySupersedeRule='modifySelectedTreatmentSupersedeRule'
                                        @onRemoveSupersedeRule='removeSelectedTreatmentSupersedeRule'
                                    />                                    
                                </v-card-text>
                            </v-card>
                        </v-window-item>
                        <v-window-item>
                            <v-card>
                                <v-card-text
                                    class='card-tab-content'
                                >
                                <BudgetsTab :selectedTreatmentBudgets='selectedTreatment.budgetIds'
                                                :addTreatment='selectedTreatment.addTreatment'
                                                :fromLibrary='hasSelectedLibrary'
                                                @onModifyBudgets='modifySelectedTreatmentBudgets' />
                                </v-card-text>
                            </v-card>
                        </v-window-item>
                        <v-window-item>
                            <v-card>
                                <v-card-text class='card-tab-content'>
                                    
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
                    </v-window>
                </div>                                             
            </v-col>                    
        </v-row>
        <v-col cols="12">
            <v-row justify="center" v-show='hasSelectedLibrary && !hasScenario'>
                <v-col>
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea                        
                        class='ghd-control-border ghd-control-text'
                        no-resize
                        variant="outlined"
                        rows='2'
                        v-model='selectedTreatmentLibrary.description'
                        @update:model-value="checkHasUnsavedChanges()"
                    />
                </v-col>
            </v-row>
            <v-divider
            v-show="(hasSelectedLibrary || hasScenario) && selectedTreatment.name !== ''"
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
            <v-row style="padding-bottom: 100px;" justify="center" v-show="(hasSelectedLibrary || hasScenario) ">
                <v-col cols="6">
                    <v-btn :disabled='!hasUnsavedChanges'
                        @click='onDiscardChanges'
                        class='ghd-white-bg ghd-blue ghd-button-text'
                        variant = "flat"
                        v-show='hasScenario'
                        style="margin-right: 5px;"
                    >
                        Cancel
                    </v-btn>
                    <v-btn id='TreatmentEditor-deleteLibrary-btn' variant = "text"
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
                        style="margin-left: 5px;margin-right: 5px;"
                        :disabled='disableCrudButtons()'
                    >
                        Create as New Library
                    </v-btn>
                    <v-btn justify-center
                        @click='onUpsertScenarioTreatments'
                        id="TreatmentEditor-SaveScenarioTreatments-btn"
                        class='ghd-blue-bg ghd-white ghd-button-text'
                        variant = "flat"
                        v-show='hasScenario'
                        style="margin-left: 5px;"
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
                </v-col>
            </v-row>
    </v-col>
    
</v-card>
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

    <ConfirmDeleteAlert
        :dialogData='confirmBeforeDeleteTreatmentAlertData'
        @submit='onSubmitConfirmDeleteTreatmentAlertResult'
    />
    <ConfirmDialog></ConfirmDialog>    
    <ImportSupersedeDialog :showDialog='showImportSupersedeDialog'
        @submit='onSubmitImportSupersedeDialogResult' />
</template>

<script setup lang='ts'>
import { ShallowRef, computed, inject, shallowRef, watch } from 'vue';
import { ref, onMounted, onBeforeUnmount, Ref} from 'vue';
import { useStore } from 'vuex';
import CreateTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
import { SelectItem } from '@/shared/models/vue/select-item';
import ConfirmDeleteAlert from '@/shared/modals/Alert.vue';
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
    TreatmentsFileImport,
    SupersedeFileImport,
    TreatmentSupersedeRule
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
import SupersedeTab from '@/components/treatment-editor/treatment-editor-tabs/SupersedeTab.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {
    InputValidationRules,
    rules as validationRules
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import {hasUnsavedChangesCore, isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getUserName } from '@/shared/utils/get-user-info';
import ImportExportTreatmentsDialog from '@/components/treatment-editor/treatment-editor-dialogs/ImportExportTreatmentsDialog.vue';
import ImportSupersedeDialog from '@/components/treatment-editor/treatment-editor-dialogs/ImportSupersedeDialog.vue';
import ImportNewTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/ImportNewTreatmentDialog.vue';
import { ImportExportTreatmentsDialogResult } from '@/shared/models/modals/import-export-treatments-dialog-result';
import { ImportSupersedeDialogResult }from '@/shared/models/modals/import-supersede-dialog-result';
import TreatmentService from '@/services/treatment.service';
import { AxiosResponse } from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { LibraryUpsertPagingRequest } from '@/shared/models/iAM/paging';
import { http2XX } from '@/shared/utils/http-utils';
import { Hub } from '@/connectionHub';
import ScenarioService from '@/services/scenario.service';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import { ImportNewTreatmentDialogResult } from '@/shared/models/modals/import-new-treatment-dialog-result';
import { useRouter } from 'vue-router';
import mitt, { Emitter, EventType } from 'mitt';
import { useConfirm } from 'primevue/useconfirm';
import ConfirmDialog from 'primevue/confirmdialog';
import { getUrl } from '@/shared/utils/get-url';

    const emit = defineEmits(['submit'])    
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>
    const $router = useRouter();
    const confirm = useConfirm();
    let store = useStore();
    let stateTreatmentLibraries = computed<TreatmentLibrary[]>(() => store.state.treatmentModule.treatmentLibraries);
    let stateSelectedTreatmentLibrary = computed<TreatmentLibrary>(() => store.state.treatmentModule.selectedTreatmentLibrary);
    let stateScenarioSelectableTreatment = computed<Treatment[]>(() => store.state.treatmentModule.scenarioSelectableTreatments);
    let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let stateScenarioTreatmentLibrary= computed<TreatmentLibrary>(() => store.state.treatmentModule.scenarioTreatmentLibrary);
    let stateScenarioSimpleBudgetDetails = computed<SimpleBudgetDetail[]>(() => store.state.investmentModule.scenarioSimpleBudgetDetails);
    let hasAdminAccess= computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
    let hasPermittedAccess= computed<boolean>(() => store.state.treatmentModule.hasPermittedAccess);
    let stateSimpleScenarioSelectableTreatments= computed<SimpleTreatment[]>(() => store.state.treatmentModule.simpleScenarioSelectableTreatments);
    let stateSimpleSelectableTreatments= computed<SimpleTreatment[]>(() => store.state.treatmentModule.simpleSelectableTreatments);
    let isSharedLibrary= computed<boolean>(() => store.state.treatmentModule.isSharedLibrary);
    let stateScenarioPerformanceCurves= computed<PerformanceCurve[]>(() => store.state.performanceCurveModule.scenarioPerformanceCurves);
    let getUserNameByIdGetter: any = store.getters.getUserNameById;


 function addSuccessNotificationAction(payload?: any): void {
     store.dispatch('addSuccessNotification', payload);
}
 function addWarningNotificationAction(payload?: any): void {
     store.dispatch('addWarningNotification', payload);
}

 function addErrorNotificationAction(payload?: any): void {
   store.dispatch('addErrorNotification', payload);
}

 function addInfoNotificationAction(payload?: any): void {
   store.dispatch('addInfoNotification', payload);
}

 async function getTreatmentLibrariesAction(payload?: any): Promise<any> {
  await store.dispatch('getTreatmentLibraries', payload);
}

async function selectTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('selectTreatmentLibrary', payload);
}

async function upsertTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('upsertTreatmentLibrary', payload);
}

async function deleteTreatmentLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('deleteTreatmentLibrary', payload);
}

async function getSimpleScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getSimpleScenarioSelectableTreatments', payload);
}

async function getSimpleSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getSimpleSelectableTreatments', payload);
}

async function getTreatmentLibraryBySimulationIdAction(payload?: any): Promise<any> {
  await store.dispatch('getTreatmentLibraryBySimulationId', payload);
}

async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioSimpleBudgetDetails', payload);
}

function setHasUnsavedChangesAction(payload?: any): void {
  store.dispatch('setHasUnsavedChanges', payload);
}

async function getScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioSelectableTreatments', payload);
}

async function upsertScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {
  await store.dispatch('upsertScenarioSelectableTreatments', payload);
}

async function upsertOrDeleteTreatmentLibraryUsersAction(payload?: any): Promise<any> {
  await store.dispatch('upsertOrDeleteTreatmentLibraryUsers', payload);
}

async function importScenarioTreatmentsFileAction(payload?: any): Promise<any> {
  await store.dispatch('importScenarioTreatmentsFile', payload);
}

async function importLibraryTreatmentsFileAction(payload?: any): Promise<any> {
  await store.dispatch('importLibraryTreatmentsFile', payload);
}

async function importScenarioTreatmentSupersedeRulesFileAction(payload?: any): Promise<any> {
  await store.dispatch('importScenarioTreatmentSupersedeRulesFile', payload);
}

async function importLibraryTreatmentSupersedeRulesFileAction(payload?: any): Promise<any> {
  await store.dispatch('importLibraryTreatmentSupersedeRulesFile', payload);
}

async function deleteTreatmentAction(payload?: any): Promise<any> {
  await store.dispatch('deleteTreatment', payload);
}

async function deleteScenarioSelectableTreatmentAction(payload?: any): Promise<any> {
  await store.dispatch('deleteScenarioSelectableTreatment', payload);
}

async function getIsSharedLibraryAction(payload?: any): Promise<any> {
  await store.dispatch('getIsSharedTreatmentLibrary', payload);
}

async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> {
  await store.dispatch('getCurrentUserOrSharedScenario', payload);
}

async function selectScenarioAction(payload?: any): Promise<any> {
  await store.dispatch('selectScenario', payload);
}

async function getScenarioPerformanceCurvesAction(payload?: any): Promise<any> {
  await store.dispatch('getScenarioPerformanceCurves', payload);
}

 function setAlertMessageAction(payload?: any): void {
   store.dispatch('setAlertMessage', payload);
}


async function addedOrUpdatedTreatmentLibraryMutator(payload?: any): Promise<any> {
  await store.commit('addedOrUpdatedTreatmentLibraryMutator', payload);
}

async function selectedTreatmentLibraryMutator(payload?: any): Promise<any> {
  await store.commit('selectedTreatmentLibraryMutator', payload);
}


    let selectedTreatmentLibrary = shallowRef(clone(emptyTreatmentLibrary));
    let treatments = shallowRef<Treatment[]>([]);
    let selectedScenarioId: string = getBlankGuid();
    let hasSelectedLibrary = ref(false);
    let librarySelectItems = shallowRef<SelectItem[]>([]);
    let treatmentSelectItems = shallowRef<SelectItem[]>([]);
    let treatmentSelectItemValue = ref("");
    let selectedTreatment = ref(clone(emptyTreatment));
    let selectedTreatmentDetails: TreatmentDetails = clone(emptyTreatmentDetails);
    let activeTab = ref(0);
    let treatmentTabs: string[] = ['Treatment Details', 'Costs', 'Consequences', 'Supersede'];
    const createTreatmentLibraryDialogData = ref<CreateTreatmentLibraryDialogData>(clone(emptyCreateTreatmentLibraryDialogData));
    let showCreateTreatmentDialog = ref(false);
    const showImportTreatmentDialog = ref<boolean>(false);
    let confirmBeforeDeleteAlertData = ref(clone(emptyAlertData));
    let hasSelectedTreatment = ref(false);
    let rules: InputValidationRules = clone(validationRules);
    let uuidNIL: string = getBlankGuid();
    let uuidNILRef = ref(getBlankGuid());
    let keepActiveTab: boolean = false;
    let hasScenario = ref(false);
    let budgets: SimpleBudgetDetail[] = [];
    let hasCreatedLibrary: boolean = false;
    let disableCrudButtonsResult: boolean = false;
    let hasLibraryEditPermission: boolean | Promise<boolean> = false;
    const showImportTreatmentsDialog = ref<boolean>(false);
    const showImportSupersedeDialog = ref<boolean>(false);
    const confirmBeforeDeleteTreatmentAlertData = ref<AlertData>(clone(emptyAlertData));
    let isNoTreatmentSelected = ref(false);
    let hasImport: boolean = false;
    let modifiedDate = ref<string>('');

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

    let simpleTreatments  = shallowRef<SimpleTreatment[]>([]);
    let isShared: boolean = false;
    let treatmentCache: Treatment[] = [];

    let unsavedDialogAllowed: boolean = true;
    let trueLibrarySelectItemValue: string = '';
    let librarySelectItemValueAllowedChanged: boolean = true;
    let librarySelectItemValue  = ref<string>('');

    const shareTreatmentLibraryDialogData = ref<ShareTreatmentLibraryDialogData>(clone(emptyShareTreatmentLibraryDialogData));
    let loadedScenarioId: string = '';
    let parentLibraryId: string  = uuidNIL;
    let parentLibraryName = ref('None');
    let scenarioParentLIbrary: string | null = null;
    let scenarioLibraryIsModified: boolean = false;
    let loadedParentName: string = "";
    let loadedParentId: string  = uuidNIL;
    let newLibrarySelection: boolean = false;
    let newTreatment: Treatment = {...emptyTreatment, id: getNewGuid(), addTreatment: false};

    
    beforeRouteEnter();
    async function beforeRouteEnter() {
        librarySelectItemValue.value = "";
        await getTreatmentLibrariesAction();
        if ($router.currentRoute.value.path.indexOf(ScenarioRoutePaths.Treatment) !== -1) {
            selectedScenarioId = $router.currentRoute.value.query.scenarioId as string;
            loadedScenarioId = selectedScenarioId;
            if (selectedScenarioId === uuidNIL) {
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                $router.push('/Scenarios/');
            }
            hasScenario.value = true;
            await getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId);
            await getTreatmentLibraryBySimulationIdAction(selectedScenarioId);
            await getScenarioPerformanceCurvesAction(selectedScenarioId);
            
            treatmentTabs = [...treatmentTabs, 'Budgets', 'Performance Factor'];
            await getScenarioSimpleBudgetDetailsAction({ scenarioId: selectedScenarioId, })
            await getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId})
            selectScenarioAction({ scenarioId: selectedScenarioId });   
              
            await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedScenarioId, workType: WorkType.ImportScenarioTreatment}).then(response => {
                if(response.data){
                    setAlertMessageAction("A treatment curve has been added to the work queue")
                }
            })
        }
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

    watch(stateScenarioSimpleBudgetDetails,  () => onStateScenarioInvestmentLibraryChanged())
    function onStateScenarioInvestmentLibraryChanged() {
        budgets = clone(stateScenarioSimpleBudgetDetails.value);
    }

    
   watch(stateTreatmentLibraries, () => onStateTreatmentLibrariesChanged())
   function onStateTreatmentLibrariesChanged() {
        librarySelectItems.value = stateTreatmentLibraries.value.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id,
            })
        );
    }

    watch(stateScenarioPerformanceCurves, () => onStateScenarioPerformanceCurvesChanged())
    function onStateScenarioPerformanceCurvesChanged() {

    }
    
    watch(stateScenarioTreatmentLibrary, () => onStateScenarioTreatmentLibraryChanged())
    function onStateScenarioTreatmentLibraryChanged() {

        setParentLibraryName(stateScenarioTreatmentLibrary.value ? stateScenarioTreatmentLibrary.value.id : "None");
        scenarioLibraryIsModified = stateScenarioTreatmentLibrary.value ? stateScenarioTreatmentLibrary.value.isModified : false;
        loadedParentId = stateScenarioTreatmentLibrary.value ? stateScenarioTreatmentLibrary.value.id : uuidNIL;
        loadedParentName = parentLibraryName.value;
    }

    watch(librarySelectItemValue, () => onLibrarySelectItemValueChangedCheckUnsaved())
    function onLibrarySelectItemValueChangedCheckUnsaved(){
        if(hasScenario.value){
            onSelectItemValueChanged();
            unsavedDialogAllowed = false;
        }           
        else if(librarySelectItemValueAllowedChanged)
            CheckUnsavedDialog(onSelectItemValueChanged, () => {
                librarySelectItemValueAllowedChanged = false;
                librarySelectItemValue.value = trueLibrarySelectItemValue;               
            })
        librarySelectItemValueAllowedChanged = true;
        setParentLibraryName(librarySelectItemValue.value ? librarySelectItemValue.value : parentLibraryId);
        scenarioLibraryIsModified = false;
        newLibrarySelection = true;
    }

    function onSelectItemValueChanged() {
        trueLibrarySelectItemValue = librarySelectItemValue.value;
        selectTreatmentLibraryAction({
            libraryId: librarySelectItemValue.value,
        });
    
        if(!isNil(librarySelectItemValue.value)){
            if (!isEmpty(librarySelectItemValue.value)){
                getSimpleSelectableTreatmentsAction(librarySelectItemValue.value);
            }
        }           
    }  

    watch(stateSimpleSelectableTreatments, () => onStateSimpleSelectableTreatments())
    function onStateSimpleSelectableTreatments() {
        simpleTreatments.value = clone(stateSimpleSelectableTreatments.value);
    }

    watch(stateSimpleScenarioSelectableTreatments, () => onStateSimpleScenarioSelectableTreatments())
    function onStateSimpleScenarioSelectableTreatments(){
        simpleTreatments.value = clone(stateSimpleScenarioSelectableTreatments.value);
    }

    watch(stateSelectedTreatmentLibrary, () => onStateSelectedTreatmentLibraryChanged())
    function onStateSelectedTreatmentLibraryChanged() {
        selectedTreatmentLibrary.value = clone(
            stateSelectedTreatmentLibrary.value
        );
    }

    watch(isSharedLibrary, () => onStateSharedAccessChanged())
    function onStateSharedAccessChanged() {
        isShared = isSharedLibrary.value;
    }

    watch(selectedTreatmentLibrary, () =>  {
        hasSelectedLibrary.value = selectedTreatmentLibrary.value.id !== uuidNIL;
        getIsSharedLibraryAction(selectedTreatmentLibrary.value).then(() => isShared = isSharedLibrary.value);
        if (hasSelectedLibrary.value) {
            checkLibraryEditPermission();
            hasCreatedLibrary = false;
            getDateModified();
            ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: selectedTreatmentLibrary.value.id, workType: WorkType.ImportLibraryTreatment}).then(response => {
                if(response.data){
                    setAlertMessageAction("A treatment import has been added to the work queue")
                }
                else
                    setAlertMessageAction("");
            })
        }

        clearChanges();
        if(treatmentSelectItemValue.value !== null && !hasScenario.value)
            treatmentCache.push(clone(selectedTreatment.value))
        checkHasUnsavedChanges();
    })

    watch(simpleTreatments, () => onSimpleTreatments())
    function onSimpleTreatments(){
        treatmentSelectItems.value = simpleTreatments.value.map((treatment: SimpleTreatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));

        checkHasUnsavedChanges()

        treatmentSelectItemValue.value = "";
    }

    watch(treatments, () => onSelectedScenarioTreatmentsChanged())
   function onSelectedScenarioTreatmentsChanged() {
        treatmentSelectItems.value = treatments.value.map((treatment: Treatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));       
        setHasUnsavedChangesAction(false)
        disableCrudButtons();
    }

    watch(treatmentSelectItemValue, () => onTreatmentSelectItemValueChanged())
    function onTreatmentSelectItemValueChanged() {
        if(!isNil(treatmentSelectItemValue.value) && treatmentSelectItemValue.value !== ""){
            var mapEntry = updatedRowsMap.get(treatmentSelectItemValue.value);
            var addedRow = addedRows.find(_ => _.id == treatmentSelectItemValue.value);
            var treatment = treatmentCache.find(_ => _.id === treatmentSelectItemValue.value);

            if(!isNil(mapEntry)){
                selectedTreatment.value = clone(mapEntry[1]);
            }
            else if(!isNil(addedRow)){
                selectedTreatment.value = clone(addedRow);
            }               
            else if(hasSelectedLibrary.value)
                TreatmentService.getSelectedTreatmentById(treatmentSelectItemValue.value).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        selectedTreatment.value = data;
                        if(isNil(treatmentCache.find(_ => _.id === data.id)))
                            treatmentCache.push(data)
                    }
                })
            
            else if(!isNil(treatment))
                selectedTreatment.value = clone(treatment);
            else{              
                TreatmentService.getScenarioSelectedTreatmentById(treatmentSelectItemValue.value).then((response: AxiosResponse) => {
                    if(hasValue(response, 'data')) {
                        var data = response.data as Treatment;
                        selectedTreatment.value = data;
                        if(isNil(treatmentCache.find(_ => _.id === data.id))){ treatmentCache.push(data); }
                        scenarioLibraryIsModified = selectedTreatment.value ? selectedTreatment.value.isModified : false;
                    }
                })
            }
                 
        }
        else
            selectedTreatment.value = clone(emptyTreatment);
        if (!keepActiveTab) {
            activeTab.value = 0;
        }
        keepActiveTab = true;
    }

    watch(selectedTreatment, () => onSelectedTreatmentChanged())
    function onSelectedTreatmentChanged() {
        hasSelectedTreatment.value = selectedTreatment.value.id !== uuidNIL;

        selectedTreatmentDetails = {
            description: selectedTreatment.value.description,
            shadowForSameTreatment: selectedTreatment.value.shadowForSameTreatment,
            shadowForAnyTreatment: selectedTreatment.value.shadowForAnyTreatment,
            criterionLibrary: selectedTreatment.value.criterionLibrary,
            category: selectedTreatment.value.category,
            assetType: selectedTreatment.value.assetType,
            isUnselectable: selectedTreatment.value.isUnselectable,
        };

        isNoTreatmentSelected.value = selectedTreatment.value.name == 'No Treatment';
    }

    function isSelectedTreatmentItem(treatmentId: string | number) {
        return isEqual(treatmentSelectItemValue.value, treatmentId.toString());
    }

     function getOwnerUserName(): string {

        if (!hasCreatedLibrary) {
            let username:any = getUserNameByIdGetter(selectedTreatmentLibrary.value.owner)
        return username ;
        }
        
        return getUserName();
    }
     function getDateModified() {
        if(hasSelectedLibrary.value) 
        { 
              TreatmentService.getTreatmentLibraryModifiedDate(selectedTreatmentLibrary.value.id).then(response => {
                  if (hasValue(response, 'status') && http2XX.test(response.status.toString()) && response.data)
                   {
                      var data = response.data as string;
                      modifiedDate.value = data.slice(0, 10);
                   }
             });
        }
        return modifiedDate.value;
    }

   function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess.value || (hasPermittedAccess.value && checkUserIsLibraryOwner());
    }

    async function checkUserIsLibraryOwner() {
        return await getUserNameByIdGetter(selectedTreatmentLibrary.value.owner) == getUserName();
    }

    function clearChanges(){
        updatedRowsMap.clear();
        addedRows = [];
        treatmentCache = [];
    }

    function onSetTreatmentSelectItemValue(treatmentId: string | number) {
        if (!isEqual(treatmentSelectItemValue.value, treatmentId.toString())) {
            treatmentSelectItemValue.value = treatmentId.toString();
        }
    }
    
    function onShowConfirmDeleteTreatmentAlert() {
        confirmBeforeDeleteTreatmentAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function  onShowTreatmentLibraryDialog(treatmentLibrary: TreatmentLibrary) {
        shareTreatmentLibraryDialogData.value = {
            showDialog: true,
            treatmentLibrary: clone(treatmentLibrary)
        };
    }

    function onShareTreatmentLibraryDialogSubmit(treatmentLibraryUsers: TreatmentLibraryUser[]) {
        shareTreatmentLibraryDialogData.value = clone(emptyShareTreatmentLibraryDialogData);
        if (!isNil(treatmentLibraryUsers) && selectedTreatmentLibrary.value.id !== getBlankGuid()) {
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
                upsertOrDeleteTreatmentLibraryUsersAction({libraryId: selectedTreatmentLibrary.value.id, proposedUsers: libraryUserData});
                getIsSharedLibraryAction(selectedTreatmentLibrary.value).then(() => isShared = isSharedLibrary.value);
                onUpsertTreatmentLibrary();
        }
    }

    function onSubmitConfirmDeleteTreatmentAlertResult(submit: boolean) {
        confirmBeforeDeleteTreatmentAlertData.value = clone(emptyAlertData);

        if (submit) {       
            onDeleteTreatment(selectedTreatment.value.id);
        }
    }


    function onDeleteTreatment(treatmentId: string | number) {
        if(hasScenario.value)
        {         
            const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), simpleTreatments.value);
            deleteScenarioSelectableTreatmentAction({ scenarioSelectableTreatment: selectedTreatment.value, simulationId: selectedScenarioId, treatments}).then(() => {
                addedRows = addedRows.filter(_ => _.id !== treatmentId.toString());
            });
        }
        else
        {
            if (any(propEq('id', treatmentId.toString()), simpleTreatments.value)) {
                const treatments : SimpleTreatment[] = reject(propEq('id', treatmentId.toString()), simpleTreatments.value);            
                deleteTreatmentAction({ treatments: treatments, treatment: selectedTreatment.value, libraryId: selectedTreatmentLibrary.value.id}).then(() => {
                    addedRows = addedRows.filter(_ => _.id !== treatmentId.toString());
                });
            }            
        }                
    }

    function onShowCreateTreatmentLibraryDialog(createAsNewLibrary: boolean) {
        createTreatmentLibraryDialogData.value = {
            showDialog: true,
            selectedTreatmentLibraryTreatments: createAsNewLibrary ? simpleTreatments.value.map(_ => {
                let treatment: Treatment = clone(emptyTreatment);
                treatment.name = _.name;
                treatment.id = _.id
                return treatment
            }) : [],
        };
    }

    function  onSubmitCreateTreatmentLibraryDialogResult(library: TreatmentLibrary) {
        createTreatmentLibraryDialogData.value = clone(emptyCreateTreatmentLibraryDialogData,);

        if (!isNil(library)) {
            const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: library,    
                isNewLibrary: true,           
                 syncModel: {
                    libraryId: library.treatments.length === 0 || !hasSelectedLibrary.value ? null :  selectedTreatmentLibrary.value.id, // setting id required for create as new library
                    rowsForDeletion: [],
                    updateRows: library.treatments.length === 0 ? [] : Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: library.treatments.length === 0 ? [] : addedRows,
                    isModified: false
                 },
                 scenarioId: hasScenario.value ? selectedScenarioId : null
            }
            TreatmentService.upsertTreatmentLibrary(upsertRequest).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    hasCreatedLibrary = true;
                    librarySelectItemValue.value = library.id;
                    
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
            parentLibraryName.value = "None";
            return;
        }
        let foundLibrary: TreatmentLibrary = emptyTreatmentLibrary;
        stateTreatmentLibraries.value.forEach(library => {
            if (library.id === libraryId ) {
                foundLibrary = clone(library);
            }
        });
        parentLibraryId = foundLibrary.id;
        parentLibraryName.value = foundLibrary.name;
    }

    function onUpsertScenarioTreatments() {

        if (selectedTreatmentLibrary.value.id === uuidNIL || hasUnsavedChanges.value && newLibrarySelection ===false) {scenarioLibraryIsModified = true;}
        else { scenarioLibraryIsModified = false; }

        TreatmentService.upsertScenarioSelectedTreatments({
            libraryId: selectedTreatmentLibrary.value.id === uuidNIL ? null : selectedTreatmentLibrary.value.id,
            rowsForDeletion: [],
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows,
            isModified: scenarioLibraryIsModified,
        }, selectedScenarioId).then((response: AxiosResponse) => {
            if (hasValue(response, 'status') && http2XX.test(response.status.toString())){
                clearChanges();
                if(hasSelectedLibrary.value){
                    librarySelectItemValue.value = "";
                    getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId)
                }
                treatmentCache.push(selectedTreatment.value);
                
                addSuccessNotificationAction({message: "Modified scenario's treatments"});   
                
                checkHasUnsavedChanges();
            }           
        });
        
    }

    function onUpsertTreatmentLibrary() {
        const upsertRequest: LibraryUpsertPagingRequest<TreatmentLibrary, Treatment> = {
                library: selectedTreatmentLibrary.value,
                isNewLibrary: false,
                syncModel: {
                libraryId: selectedTreatmentLibrary.value.id === uuidNIL ? null : selectedTreatmentLibrary.value.id,
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
                addedOrUpdatedTreatmentLibraryMutator(selectedTreatmentLibrary.value);
                selectedTreatmentLibraryMutator(selectedTreatmentLibrary.value.id);
            }
        });
    }

    function onAddTreatment(newTreatment: Treatment) {
        showCreateTreatmentDialog.value = false;

        if (!isNil(newTreatment)) {
            if(hasScenario.value)
                newTreatment.libraryId = parentLibraryId
            else 
                newTreatment.libraryId = selectedTreatmentLibrary.value.id
            addedRows = append(newTreatment, addedRows);
            simpleTreatments.value = append({name: newTreatment.name, id: newTreatment.id}, simpleTreatments.value);
            setTimeout(() => (treatmentSelectItemValue.value = newTreatment.id));
        }
    }

    function modifySelectedTreatmentDetails(treatmentDetails: TreatmentDetails) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
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
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                costs: prepend(newCost, selectedTreatment.value.costs),
            });
        }
    }

    function  modifySelectedTreatmentCost(modifiedCost: TreatmentCost) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                costs: update(
                    findIndex(propEq('id', modifiedCost.id), selectedTreatment.value.costs,),
                    modifiedCost,
                    selectedTreatment.value.costs,
                ),
            });
        }
    }

    function removeSelectedTreatmentCost(costId: string) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                costs: reject(propEq('id', costId), selectedTreatment.value.costs,),
            });
        }
    }

    function modifySelectedTreatmentPerformanceFactor(modifiedPerformanceFactor: TreatmentPerformanceFactor) {
        if (hasSelectedTreatment.value) {
            if (findIndex(propEq('id', modifiedPerformanceFactor.id), selectedTreatment.value.performanceFactors) < 0)
            {
                modifySelectedTreatment({
                    ...clone(selectedTreatment.value),
                    performanceFactors: prepend(modifiedPerformanceFactor, selectedTreatment.value.performanceFactors)
                });
            } else {
                modifySelectedTreatment({
                    ...clone(selectedTreatment.value),
                    performanceFactors: update(
                        findIndex(propEq('id', modifiedPerformanceFactor.id), selectedTreatment.value.performanceFactors),
                        modifiedPerformanceFactor,
                        selectedTreatment.value.performanceFactors,
                    ),
                });
            }
        }
    }

    function addSelectedTreatmentConsequence(newConsequence: TreatmentConsequence) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                consequences: prepend(newConsequence, selectedTreatment.value.consequences,),
            });
        }
    }

    function  modifySelectedTreatmentConsequence(modifiedConsequence: TreatmentConsequence,) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                consequences: update(
                    findIndex(propEq('id', modifiedConsequence.id), selectedTreatment.value.consequences,),
                    modifiedConsequence,
                    selectedTreatment.value.consequences,
                ),
            });
        }
    }

    function removeSelectedTreatmentConsequence(consequenceId: string) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                consequences: reject(propEq('id', consequenceId), selectedTreatment.value.consequences,),
            });
        }
    }

    function modifySelectedTreatmentBudgets(simpleBudgetDetails: SimpleBudgetDetail[]) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                budgetIds: getPropertyValues('id', simpleBudgetDetails,) as string[],
            });
        }
    }

    function addSelectedTreatmentSupersedeRule(newSupersedeRule: TreatmentSupersedeRule) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                supersedeRules: prepend(newSupersedeRule, selectedTreatment.value.supersedeRules,),
            });
        }
    }

    function  modifySelectedTreatmentSupersedeRule(modifiedSupersedeRule: TreatmentSupersedeRule,) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                supersedeRules: update(
                    findIndex(propEq('id', modifiedSupersedeRule.id), selectedTreatment.value.supersedeRules,),
                    modifiedSupersedeRule,
                    selectedTreatment.value.supersedeRules,
                ),
            });
        }
    }

    function removeSelectedTreatmentSupersedeRule(supersedeRuleId: string) {
        if (hasSelectedTreatment.value) {
            modifySelectedTreatment({
                ...clone(selectedTreatment.value),
                supersedeRules: reject(propEq('id', supersedeRuleId), selectedTreatment.value.supersedeRules,),
            });
        }
    }

    function modifySelectedTreatment(treatment: Treatment) {
        selectedTreatment.value = treatment;

        onUpdateRow(treatment.id, treatment);
        checkHasUnsavedChanges();
    }

    function onDiscardChanges() {
        treatmentSelectItemValue.value = "";
        librarySelectItemValue.value = "";
        setTimeout(() => {
            if (hasScenario.value) {       
                clearChanges();        
                simpleTreatments.value = clone(stateSimpleScenarioSelectableTreatments.value);
            }
        });
        parentLibraryName.value = loadedParentName;
        parentLibraryId = loadedParentId;
    }

    function reset(){
        treatmentSelectItemValue.value = "";
        librarySelectItemValue.value = "";
        clearChanges();        
        simpleTreatments.value = clone(stateSimpleScenarioSelectableTreatments.value);
    }

    function onShowConfirmDeleteAlert() {
        confirmBeforeDeleteAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function onSubmitConfirmDeleteAlertResult(submit: boolean) {
        confirmBeforeDeleteAlertData.value = clone(emptyAlertData);

        if (submit) {
            librarySelectItemValue.value = "";
            deleteTreatmentLibraryAction({ libraryId: selectedTreatmentLibrary.value.id, });            
        }
    }

    function disableCrudButtons() { // TODO for supersedes?
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

        if (hasSelectedLibrary.value) {
            return !(rules['generalRules'].valueIsNotEmpty(selectedTreatmentLibrary.value.name) === true &&
                allDataIsValid);
        }

        disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    function onSubmitNewTreatment(result: ImportNewTreatmentDialogResult){
        showImportTreatmentDialog.value = false;
        if (result === null) return;

        if(hasScenario.value){
            newTreatment.libraryId = parentLibraryId
            newTreatment.name = result.file.name.slice(0, -5);
            addedRows = append(newTreatment, addedRows);
            simpleTreatments.value = append({name: result.file.name.slice(0, -5), id: selectedScenarioId}, simpleTreatments.value);
            setTimeout(() => (treatmentSelectItemValue.value = newTreatment.id));
        }
        else{
            newTreatment.libraryId = selectedTreatmentLibrary.value.id
            newTreatment.name = result.file.name.slice(0, -5);
            addedRows = append(newTreatment, addedRows);
            simpleTreatments.value = append({name: result.file.name.slice(0, -5), id: newTreatment.libraryId}, simpleTreatments.value);
            setTimeout(() => (treatmentSelectItemValue.value = newTreatment.id));
        }
            
            if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file 
            };
            if (hasScenario.value) {
                TreatmentService.importScenarioTreatments(data.file, newTreatment.libraryId, hasScenario.value)
            }
            else{
                TreatmentService.importLibraryTreatments(data.file, selectedTreatmentLibrary.value.id, hasScenario.value)
            }
            hasImport = true;
        }
    }

     function onSubmitImportTreatmentsDialogResult(result: ImportExportTreatmentsDialogResult) {
        showImportTreatmentsDialog.value = false;

        if (hasValue(result) && hasValue(result.file)) {
            const data: TreatmentsFileImport = {
                file: result.file
            };

            if (hasScenario.value) {
                importScenarioTreatmentsFileAction({
                    ...data,
                    id: selectedScenarioId
                }).then(() => {
                                   
                });
            } else {
                importLibraryTreatmentsFileAction({
                    ...data,
                    id: selectedTreatmentLibrary.value.id
                }).then(() => {
                                   
                });;
            }
        }
     }

    function OnExportTreamentsClick(){
        const id: string = hasScenario.value ? selectedScenarioId : selectedTreatmentLibrary.value.id;
        TreatmentService.exportTreatments(id, hasScenario.value)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

     function OnDownloadTemplateClick()
    {
        TreatmentService.downloadTreatmentsTemplate(hasScenario.value)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }

    function OnExportSupersedeClick(){
        const id: string = hasScenario.value ? selectedScenarioId : selectedTreatmentLibrary.value.id;
        TreatmentService.exportSupersedeRules(id, hasScenario.value)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

    function OnDownloadSupersedeTemplateClick()
    {
        TreatmentService.downloadSupersedeRulesTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
    }
    
    function onSubmitImportSupersedeDialogResult(result: ImportSupersedeDialogResult) {        
        showImportSupersedeDialog.value = false;

        if (hasValue(result) && hasValue(result.file)) {
            const data: SupersedeFileImport = {
                file: result.file
            };

            if (hasScenario.value) {
                importScenarioTreatmentSupersedeRulesFileAction({
                    ...data,
                    id: selectedScenarioId
                }).then(() => {                                 
                });
            } else {
                importLibraryTreatmentSupersedeRulesFileAction({
                    ...data,
                    id: selectedTreatmentLibrary.value.id
                }).then(() => {                                   
                });
            }
        }
     }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if( importComp.workType === WorkType.ImportScenarioTreatment && importComp.id === selectedScenarioId ||
            hasSelectedLibrary.value && importComp.workType === WorkType.ImportLibraryTreatment && importComp.id === selectedTreatmentLibrary.value.id){
            clearChanges()
            getTreatmentLibrariesAction().then(async () => {
                if(hasScenario.value){
                    await getSimpleScenarioSelectableTreatmentsAction(selectedScenarioId);
                    onDiscardChanges();
                }  
                else{
                    await getSimpleSelectableTreatmentsAction(selectedTreatmentLibrary.value.id);
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
            (hasScenario.value && hasSelectedLibrary.value) ||
            (hasSelectedLibrary.value && hasUnsavedChangesCore('', selectedTreatmentLibrary.value, stateSelectedTreatmentLibrary.value))
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
.item-content{
    display: flex;
    justify-content: space-between;
    align-items: center;
}
</style>