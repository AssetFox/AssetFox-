<template>
    <v-card class="elevation-0 vcard-main-layout">
    <v-div>
        <v-row class="Montserrat-font-family" justify="space-between">
            <v-col>
                <v-btn @click='OnGetTemplateClick' 
                    class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant="outlined" style="margin: 10px;">Get Default Template</v-btn>
                    <input
                    id="committedProjectTemplateUpload"
                    type="file"
                    @change="handleCommittedProjectTemplateUpload"
                    accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                    ref="committedProjectTemplateInput"
                    hidden/>
            </v-col>
            <v-col>
                <v-btn @click='showImportExportCommittedProjectsDialog = true' 
                    class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" style="margin: 5px;" variant="outlined">Import Projects</v-btn>
                <v-btn @click='OnExportProjectsClick' 
                    class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" style="margin: 5px;" variant="outlined">Export Projects</v-btn>
                <v-btn @click='OnDeleteAllClick' 
                    class="ghd-red ghd-button-text ghd-outline-button-padding ghd-button" style="margin: 5px;" variant="outlined">Delete All</v-btn>
            </v-col>
        </v-row>
        <v-col>                   
            <v-checkbox 
                    id="CommittedProjectsEditor-noTreatmentsBeforeCommittedProjects-ghdcheckbox"
                    label='No Treatments Before Committed Projects' v-model='isNoTreatmentBefore' />
        </v-col>
        <v-col cols="3" class="ghd-constant-header">
            <v-row>
                <v-subheader class="ghd-control-label ghd-md-gray"></v-subheader>
                <v-row align="center" style="padding: 10px;">                                
                    <v-text-field
                        id="CommittedProjectsEditor-search-vtextfield"
                        hide-details
                        lablel="Search"
                        placeholder="Search"
                        single-line
                        v-model="gridSearchTerm"
                        prepend-inner-icon=custom:GhdSearchSvg
                        variant="outlined"
                        density="compact"
                        clearable
                        @click:clear="onClearClick()"
                        style="margin-left: 15px;"
                        class="ghd-text-field-border ghd-text-field search-icon-general">
                    </v-text-field>
                    <v-btn 
                    id="CommittedProjectsEditor-performSearch-vbtn"
                    style="margin-top: 2px; margin-left: 5px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" @click="onSearchClick()">Search</v-btn>
                </v-row>
                        
                </v-row>
        </v-col>
        <v-row justify="end" class="px-4">
                        <p>Commited Projects: {{totalItems}}</p>
        </v-row>
        <v-col>
                <p-card style="padding: 10px; width: auto;">
                    <v-row>
                        <v-data-table-server
                        id="CommittedProjectsEditor-committedProjects-vdatatable"
                        :headers="cpGridHeaders"
                        :items-length="totalItems"
                        :items="currentPage"
                        item-key='id'
                        sort-asc-icon="custom:GhdTableSortAscSvg"
                        sort-desc-icon="custom:GhdTableSortDescSvg"
                        :pagination.sync="projectPagination"
                        :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                        v-model:sort-by="projectPagination.sort"
                        v-model:page="projectPagination.page"
                        v-model:items-per-page="projectPagination.rowsPerPage"
                        item-value="name"
                        v-model="selectedCpItems"
                        @update:options="onPaginationChanged"
                        class=" fixed-header v-table__overflow">
                            <template v-slot:item="item">
                                <tr>
                                
                                <td v-for="header in cpGridHeaders">
                                    <div>
                                        <v-select v-if="header.key === 'treatment'"
                                            :items="treatmentSelectItems"
                                            menu-icon=custom:GhdDownSvg
                                            class="ghd-down-small"
                                            density="compact"
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :rules="[inputRules['generalRules'].valueIsNotEmpty]"
                                            :style="getTreatmentStyle(item.item[header.key])"
                                            :error-messages="item.item.treatmentErrors"
                                            @update:model-value="onEditCommittedProjectProperty(item.item,header.key,item.item[header.key])">
                                        </v-select>
                                        <v-select
                                            v-else-if="header.key === 'projectSource'"
                                            :items="projectSourceOptions"
                                            menu-icon=custom:GhdDownSvg
                                            class="ghd-down-small"
                                            density="compact"
                                            variant="underlined"
                                            v-model="item.item[header.key]"
                                            :style="getProjectSourceStyle(item.item[header.key])"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                                            :error-messages="item.item.projectSourceErrors"
                                            @update:model-value="onEditCommittedProjectProperty(item.item, header.key, item.item.projectSource)"
                                            
                                        ></v-select>
                                        <editDialog v-if="header.key !== 'actions' && header.key !== 'selection'"
                                            v-model:return-value="item.item[header.key]"
                                            @save="onEditCommittedProjectProperty(item.item,header.key,item.item[header.key])"
                                            size="large"
                                            lazy
                                            >
                                            <v-text-field v-if="header.key !== 'budget' 
                                                && header.key !== 'year' 
                                                && header.key !== 'keyAttr' 
                                                && header.key !== 'treatment'
                                                && header.key !== 'cost'
                                                && header.key !== 'projectSource'
                                                && header.key !== 'category'
                                                && header.key !== 'projectId'"
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="item.item[header.key]"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.key === 'category'"
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="reverseCatMap.get(item.item[header.key])"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.key === 'budget'"
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="item.item[header.key]"/>

                                            <v-text-field v-if="header.key === 'keyAttr'"
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="item.item[header.key]"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="item.item.errors"/>

                                            <v-text-field v-if="header.key === 'year'"
                                                :model-value="item.item[header.key]"
                                                v-maska:[yearMask]
                                                density="compact"
                                                variant="underlined"
                                                :style="getYearStyle(item.item[header.key])"
                                                :rules="[inputRules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), inputRules['generalRules'].valueIsNotEmpty, inputRules['generalRules'].valueIsWithinRange(item.item[header.key], [firstYear, lastYear])]"
                                                :error-messages="item.item.yearErrors"/>
                                                
                                            <currencyTextbox v-if="header.key === 'cost'"
                                                :model-value='item.item[header.key]'
                                                density="compact"
                                                variant="underlined"
                                                :style="getCostStyle(item.item[header.key])"
                                                :error-messages="item.item.costErrors"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>

                                            <v-text-field v-if="header.key === 'projectId'"
                                                readonly
                                                class="sm-txt"
                                                density="compact"
                                                variant="underlined"
                                                :model-value="item.item[header.key]"/>

                                            <template v-slot:input>
                                                <v-text-field v-if="header.key === 'keyAttr'"
                                                    label="Edit"
                                                    single-line
                                                    variant="underlined"
                                                    v-model="item.item[header.key]"
                                                    :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>

                                                <v-select v-if="header.key === 'budget'"
                                                    :items="budgetSelectItems"
                                                    menu-icon=custom:GhdDownSvg
                                                    label="Select a Budget"
                                                    variant="outlined"
                                                    item-title="text"  
                                                    item-value="value"
                                                    v-model="item.item[header.key]">
                                                </v-select>

                                                <v-select v-if="header.key === 'category'"
                                                    :items="categorySelectItems"
                                                    menu-icon=custom:GhdDownSvg
                                                    label="Select a Category"
                                                    variant="outlined"
                                                    item-title="text"  
                                                    item-value="value"
                                                    v-model="item.item[header.key]">
                                                </v-select>
                                                
                                                <v-text-field v-if="header.key === 'year'"
                                                    label="Edit"
                                                    single-line
                                                    variant = "underlined"
                                                    v-model="item.item[header.key]"
                                                    v-maska:[yearMask]
                                                    :rules="[inputRules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(item.item[header.key], [firstYear, lastYear])]"/>

                                                <currencyTextbox v-if="header.key === 'cost'"
                                                    label="Edit"
                                                    single-line
                                                    v-model.number="item.item[header.key]"
                                                    :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>
                                                
                                                <v-text-field v-if="header.key === 'projectId'"
                                                    label="Edit"
                                                    single-line
                                                    variant="underlined"
                                                    v-model="item.item[header.key]"/>

                                            </template>
                                        </editDialog>
                                
                                        <div v-if="header.key === 'actions'">
                                            <v-row justify="center">
                                                <v-btn 
                                                    id="CommittedProjectsEditor-deleteCommittedProject-vbtn"
                                                    @click="OnDeleteClick(item.item.id)"  class="ghd-red" flat icon>
                                                    <TrashCanSvg />
                                                </v-btn>
                                            </v-row>
                                        </div>                            
                                    </div>
                                </td>
                            </tr>
                            </template>
                        </v-data-table-server>    
                    </v-row>
            </p-card>
        </v-col>
        <v-divider></v-divider>
        <v-btn id="CommittedProjectsEditor-addCommittedProject-vbtn" 
                        @click="OnAddCommittedProjectClick" v-if="selectedCommittedProject === ''"
                        class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" style="margin:10px" variant = "outlined"
                > Add Committed Project
        </v-btn> 
        <v-divider
            :thickness="2"
            class="border-opacity-100"
        ></v-divider>
        <v-col>
            <v-row justify="center" style="padding-bottom: 80px;">
                <CancelButton 
                    @cancel="onCancelClick"
                    :disabled="!hasUnsavedChanges"
                />
                <SaveButton 
                    @save="OnSaveClick"
                    :disabled="!hasUnsavedChanges || disableCrudButtons()"
                />
            </v-row>
        </v-col> 
        <v-row justify="center">
            <p v-if="importedProjectTreatmentBoolean" style="color: red; padding-bottom: 20px;">The following treatments have not been found in the treatment list: {{ missingTreatments.join(', ') }}</p>
        </v-row>
        <v-col cols = "8" style="border:1px solid #999999 !important;" v-if="selectedCommittedProject !== ''">
            <v-row column>
                <v-col cols = "12">
                    <v-btn 
                       id="CommittedProjectsEditor-closeSelectedCommitedProject-vbtn"
                       @click="selectedCommittedProject = ''" variant = "flat" class="ghd-close-button">
                        X
                    </v-btn>
                </v-col>
            </v-row>
        </v-col>
        <ImportExportCommittedProjectsDialog
            :show-dialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
        />
        <Alert
            :dialogData="alertDataForDeletingCommittedProjects"
            @submit="onDeleteCommittedProjectsSubmit"
        />
        <ConfirmDialog></ConfirmDialog>
    </v-div>
    <SuccessfulUploadDialog 
        v-model="showSuccessPopup"
        message="Successfully uploaded committed projects."
    />
    <!-- <v-dialog v-model="showSuccessPopup" max-width="400px">
        <v-card>
            <v-card-text class="text-center">
                Successfully uploaded committed projects.
            </v-card-text>
            <v-card-actions>
                <v-row justify="center" class="w-100">
                    <v-btn color="primary" variant="text" 
                    class='ghd-white-bg ghd-blue ghd-button-text' @click="showSuccessPopup = false">OK</v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog> -->
</v-card>
</template>
<script setup lang="ts">
import { useRouter } from 'vue-router';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import { watch, ref, shallowReactive, onMounted, onBeforeUnmount, shallowRef, computed, inject } from 'vue'
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectFillTreatmentReturnValues, emptySectionCommittedProject, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { getBlankGuid, getNewGuid } from '../../shared/utils/uuid-utils';
import { emptyTreatmentLibrary, SimpleTreatment, Treatment, treatmentCategoryMap, treatmentCategoryReverseMap, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { SelectItem } from '@/shared/models/vue/select-item';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Attribute } from '@/shared/models/iAM/attribute';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { AxiosResponse } from 'axios';
import { any, clone, find, findIndex, isNil, propEq, update } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { http2XX } from '@/shared/utils/http-utils';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import ImportExportCommittedProjectsDialog from './committed-project-editor-dialogs/CommittedProjectsImportDialog.vue';
import { InvestmentPlan, SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import ScenarioService from '@/services/scenario.service';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import InvestmentService from '@/services/investment.service';
// import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { max } from 'moment';
import { Hub } from '@/connectionHub';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import { storeKey, useStore } from 'vuex';
import { createDecipheriv } from 'crypto';
import mitt, { Emitter, EventType } from 'mitt';
import Dialog from 'primevue/dialog';
import Column from 'primevue/column';
import TreatmentService from '@/services/treatment.service';
import { getUrl } from '@/shared/utils/get-url';
import  currencyTextbox  from '@/shared/components/CurrencyTextbox.vue';
import ConfirmDialog from 'primevue/confirmdialog';
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import SuccessfulUploadDialog from '@/shared/components/dialogs/SuccessfulUploadDialog.vue';


    let store = useStore();
    const $router = useRouter();    
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>
    created();

    let searchItems = '';
    let dataPerPage = 0;
    let totalDataFound = 0;
    let hasScenario: boolean = false;
    let librarySelectItemValue = ref<string>("");
    const templateSelectItems = ref<string[]>([]);
    const templateItemSelected = ref<string>("");
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let attributeSelectItems: SelectItem[] = [];
    const treatmentSelectItems = ref< string[] >([]);
    const projectSourceOptions = ref< string [] >([]);
    const budgetSelectItems = ref< SelectItem[] >([]);
    const categorySelectItems = ref<SelectItem[]>([]);
    let importedProjectTreatmentName = ref<string>("");
    let importedProjectTreatmentBoolean = ref(false);
    let categories: string[] = [];
    const missingTreatments = ref< string[] >([]);
    const missingTreatmentsValue = ref< string[] >([]);
    const invalidProjectSources = ref<number[]>([]);
    const invalidCosts = ref<number[]>([]);
    const invalidYears = ref<number[]>([]);
    const invalidTreatments = ref<string[]>([]);
    let allImportedTreatments;
    let scenarioId: string = getBlankGuid();
    let networkId: string = getBlankGuid();
    let inputRules: InputValidationRules = rules;
    let network: Network = clone(emptyNetwork);
    let isAdminTemplateUploaded: Boolean;
    let fileData: AxiosResponse;
    
    const projectSourceMap = new Map<number, string>([
        [0, "None"],
        [1, "iAMPick"],
        [2, "Committed"],
        [3, "MPMS"],
        [4, "SAP"],
        [5, "ProjectBuilder"]
    ]);

    const showSuccessPopup = ref(false);

    const uuidNIL: string = getBlankGuid();
    let addedRows = ref<SectionCommittedProject[]>([]);
    let updatedRowsMap:Map<string, [SectionCommittedProject, SectionCommittedProject]> = new Map<string, [SectionCommittedProject, SectionCommittedProject]>();//0: original value | 1: updated value
    let deletionIds = ref<string[]>([]);
    const rowCache = ref<SectionCommittedProject[]>([]);
    const gridSearchTerm = ref('');
    const currentSearch = ref('');
    let totalItems = ref(0);
    const currentPage = ref<SectionCommittedProjectTableData[]>([]);
    let isRunning: boolean = true;

    const selectedLibraryTreatments = ref<SimpleTreatment[]>([]);
    let isKeyAttributeValidMap: Map<string, boolean> = new Map<string, boolean>();

    let projectPagination = shallowReactive<Pagination>(clone(emptyPagination));

    const stateSectionCommittedProjects = computed<SectionCommittedProject[]>(() => store.state.committedProjectsModule.sectionCommittedProjects);
    const stateTreatmentLibraries = computed<TreatmentLibrary[]>(() =>store.state.treatmentModule.treatmentLibraries);
    const stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    const stateScenarioSimpleBudgetDetails = computed<SimpleBudgetDetail[]>(() =>store.state.investmentModule.scenarioSimpleBudgetDetails);
    const hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    const networks = computed<Network[]>(() => store.state.networkModule.networks);
    const selectedStateTreatmentLibrary = computed<TreatmentLibrary>(() => store.state.treatmentModule.selectedTreatmentLibrary);
    const stateSimpleScenarioSelectableTreatments = computed<SimpleTreatment[]>(() => store.state.treatmentModule.simpleScenarioSelectableTreatments);

    async function getTreatmentLibrariesAction(payload?: any): Promise<any> { await store.dispatch('getTreatmentLibraries', payload); }
    async function getSimpleScenarioSelectableTreatmentsAction(payload?: any): Promise<any> { await store.dispatch('getSimpleScenarioSelectableTreatments', payload); }
    async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any> { await store.dispatch('getScenarioSimpleBudgetDetails', payload); }
    async function getAttributesAction(payload?: any): Promise<any> { await store.dispatch('getAttributes', payload); }
    async function getNetworksAction(payload?: any): Promise<any> { await store.dispatch('getNetworks', payload); }
    async function deleteSimulationCommittedProjectsAction(payload?: any): Promise<any> { await store.dispatch('deleteSimulationCommittedProjects', payload); }
    function selectTreatmentLibraryAction(payload?: any){ store.dispatch('selectTreatmentLibrary', payload); }
    function setHasUnsavedChangesAction(payload?: any) { store.dispatch('setHasUnsavedChanges', payload); }
    function addSuccessNotificationAction(payload?: any) {  store.dispatch('addSuccessNotification', payload); }
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification', payload); } 
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> { await store.dispatch('getCurrentUserOrSharedScenario', payload); }
    function selectScenarioAction(payload?: any){  store.dispatch('selectScenario', payload); }
    function setAlertMessageAction(payload?: any){ store.dispatch('setAlertMessage', payload); }
    
    let getUserNameByIdGetter: any = store.getters.getUserNameByIdGetter;
    
    let cpItems: SectionCommittedProjectTableData[] = [];
    let selectedCpItems = ref<SectionCommittedProjectTableData[]>([]);
    let sectionCommittedProjects = ref<SectionCommittedProject[]>([]);
    let committedProjectsCount: number = 0;
    const showImportExportCommittedProjectsDialog = ref< boolean > (false);
    let selectedCommittedProject = ref<string>('');
    let showCreateCommittedProjectConsequenceDialog: boolean = false;
    let disableCrudButtonsResult: boolean = true;
    const alertDataForDeletingCommittedProjects = ref<AlertData>({ ...emptyAlertData });
    let reverseCatMap = clone(treatmentCategoryReverseMap);
    let catMap = clone(treatmentCategoryMap);
    
    let keyattr: string = '';

    let lastYear: number = 0;
    let firstYear: number = 0;

    let investmentYears = ref<number[]>([]);
    let isNoTreatmentBefore = ref<boolean>(true);
    let isNoTreatmentBeforeCache = ref<boolean>(true);

    const yearMask = { mask: '####' };
    
    const cpGridHeaders = ref<any[]>([
        {
            title: '',
            key: 'keyAttr',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            title: 'Year',
            key: 'year',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            title: 'Treatment',
            key: 'treatment',
            align: 'left',
            sortable: true,
            class: '',
            width: '15%',
        },
        {
            title: 'Category',
            key: 'category',
            align: 'left',
            sortable: true,
            class: '',
            width: '15%',
        },
        {
            title: 'Budget',
            key: 'budget',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            title: 'Cost',
            key: 'cost',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        { 
            title: 'Project Source', 
            key: 'projectSource',
            align: 'left',
            sortable: false,
            class: '',
            width: '15%'
        },
        { 
            title: 'Project Id', 
            key: 'projectId',
            align: 'left',
            sortable: false,
            class: '',
            width: '15%'
        },
        {
            title: 'Actions',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
    ]);

    function created() {
        $emitter.on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }
    onMounted(async() => {
        scenarioId = $router.currentRoute.value.query.scenarioId as string;
        networkId = $router.currentRoute.value.query.networkId as string;
        
        if (scenarioId === uuidNIL || networkId == uuidNIL) {
            addErrorNotificationAction({
               message: 'Found no selected scenario for edit',
            });
            $router.push('/Scenarios/');
        }
        let i = 0
        reverseCatMap.forEach(cat => {
            categorySelectItems.value.push({text: cat, value: i})     
            i++   
        });
        await getSimpleScenarioSelectableTreatmentsAction(scenarioId)
        hasScenario = true;
        await getNetworksAction();
        await InvestmentService.getScenarioBudgetYears(scenarioId).then(response => {  
            if(response.data) {
                if (response.data.length === 0) {
                    investmentYears.value = [];
                } else {
                    investmentYears.value = response.data;
                }
            }
        });
        await ScenarioService.getNoTreatmentBeforeCommitted(scenarioId).then(response => {
            if(!isNil(response.data)){
                isNoTreatmentBeforeCache.value = response.data;
                isNoTreatmentBefore.value = response.data;
            }
        });
        await getScenarioSimpleBudgetDetailsAction({scenarioId: scenarioId});
        await getAttributesAction();
        
        await getCurrentUserOrSharedScenarioAction({simulationId: scenarioId});
        await selectScenarioAction({ scenarioId: scenarioId });
        await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: scenarioId, workType: WorkType.ImportCommittedProject}).then(response => {
            if(response.data){
                setAlertMessageAction("Committed project import has been added to the work queue");
            }
        });
        await initializePages();
        if (scenarioId !== undefined) {  
            await fetchProjectSources();
        }

        await CommittedProjectsService.getUploadedCommittedProjectTemplates().then(response => {
            if(!isNil(response.data)){
                    templateSelectItems.value = response.data;
            }
        });   
        onPaginationChanged();         
    });
    onBeforeUnmount(() => beforeDestroy())
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });

        $emitter.off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
        setAlertMessageAction('');
    }

        //Watch
        watch(isNoTreatmentBefore, () => {
            checkHasUnsavedChanges();
        });

    watch(investmentYears, (newYears) => {
        if (newYears.length > 0) {
            lastYear = Math.max(...newYears);
            firstYear = Math.min(...newYears);
        }
    });

    watch(networks, (newNetworks) => {
        network = newNetworks.find(o => o.id === networkId) || clone(emptyNetwork);
    });


    watch(stateAttributes, () => {
        attributeSelectItems = stateAttributes.value.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }),
        );
        let keyAttr = stateAttributes.value.find(_ => _.id == network.keyAttribute);
        if(!isNil(keyAttr)){
            keyattr = keyAttr.name;
            cpGridHeaders.value[0].title = keyattr;
        }
    });

    watch(selectedLibraryTreatments, onSelectedLibraryTreatmentsChanged)
    function onSelectedLibraryTreatmentsChanged() {
        treatmentSelectItems.value = selectedLibraryTreatments.value.map(
            (treatment: SimpleTreatment) => (treatment.name)
        );
    };

    watch(stateSimpleScenarioSelectableTreatments, () => {
        treatmentSelectItems.value = stateSimpleScenarioSelectableTreatments.value.map(
            (treatment: SimpleTreatment) => (treatment.name)
        );
    });

    watch(stateScenarioSimpleBudgetDetails, () => {
        budgetSelectItems.value = stateScenarioSimpleBudgetDetails.value.map(
            (budget: SimpleBudgetDetail) => ({
                text: budget.name,
                value: budget.name
            }),
        );
    });

    watch(stateSectionCommittedProjects, () => {
        sectionCommittedProjects.value = clone(stateSectionCommittedProjects.value);
        setCpItems();
    });

    watch(sectionCommittedProjects, () => {
        setCpItems();  
    });

    watch(selectedCpItems, () => {
        if(selectedCpItems.value.length > 1)
           selectedCpItems.value.splice(0,1);
        if(selectedCpItems.value.length === 1)
           selectedCommittedProject.value = selectedCpItems.value[0].id;
    },{ immediate: true });

    async function onPaginationChanged() {
        if (isRunning) {
            return;
        }

        resetValidationArrays();

        isRunning = true
        checkHasUnsavedChanges();

        const request = createPaginationRequest();

        if (scenarioId !== uuidNIL) {
            try {
                const response = await CommittedProjectsService.getCommittedProjectsPage(scenarioId, request);

                if (response.data) {
                    const data = response.data as PagingPage<SectionCommittedProject>;
                    sectionCommittedProjects.value = data.items;
                    rowCache.value = clone(sectionCommittedProjects.value);
                    totalItems.value = data.totalItems;
                    allImportedTreatments = data.items;

                    // Check imported treatments and other validations
                    validateImportedData(data.items);

                    // Sort missing treatments if any
                    if (missingTreatments.value.length > 0) {
                        importedProjectTreatmentBoolean.value = true;
                        missingTreatmentsValue.value = missingTreatments.value;

                        data.items = sortMissingTreatments(data.items);
                    } else {
                        importedProjectTreatmentBoolean.value = false;
                    }

                    const row = data.items.find(scp => scp.id == selectedCommittedProject.value);
                    if (isNil(row)) {
                        selectedCommittedProject.value = '';
                    }
                }
            } catch (error) {
                console.error('Error fetching committed projects:', error);
            } finally {
                isRunning = false;
            }
        } else {
            isRunning = false;
        }
    }

    function createPaginationRequest(): PagingRequest<SectionCommittedProject> {
        const { sort, descending, page, rowsPerPage } = projectPagination;

        return {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: false
            },
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: descending != null ? descending : false,
            search: currentSearch.value
        };
    }

    function resetValidationArrays() {
        const arraysToReset = [
            missingTreatments, 
            invalidProjectSources, 
            invalidCosts, 
            invalidYears, 
            invalidTreatments
        ];
        arraysToReset.forEach(arr => arr.value = []);
    }

    function validateImportedData(items: SectionCommittedProject[]) {
        const treatmentSet = new Set(treatmentSelectItems.value); // Optimize includes check
        items.forEach(item => {
            importedProjectTreatmentName.value = item.treatment;

            if (!treatmentSet.has(importedProjectTreatmentName.value)) {
                missingTreatments.value.push(importedProjectTreatmentName.value);
            }

            if (!validProjectSource(item.projectSource)) {
                invalidProjectSources.value.push(item.projectSource);
            }

            if (!validCost(item.cost)) {
                invalidCosts.value.push(item.cost);
            }

            if (!item.year || item.year < firstYear || item.year > lastYear) {
                invalidYears.value.push(item.year);
            }
        });
    }

    function sortMissingTreatments(items: SectionCommittedProject[]) {
        return items.sort((a, b) => {
            const isInMissingTreatmentsA = missingTreatments.value.includes(a.treatment);
            const isInMissingTreatmentsB = missingTreatments.value.includes(b.treatment);

            if (isInMissingTreatmentsA && !isInMissingTreatmentsB) {
                return -1; // a should come first
            } else if (!isInMissingTreatmentsA && isInMissingTreatmentsB) {
                return 1; // b should come first
            } else {
                return 0; // no change in order
            }
        });
    }

     watch(deletionIds, () => {
        checkHasUnsavedChanges();
     });

    watch(addedRows, () =>{
        checkHasUnsavedChanges();
    });

    //Events
    function onCancelClick() {

        clearChanges()
        selectedCommittedProject.value = '';
        selectedCpItems.value = [];
        isNoTreatmentBefore.value = isNoTreatmentBeforeCache.value
        resetPage();
    }

    function OnExportProjectsClick(){
        CommittedProjectsService.exportCommittedProjects(scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
        }

    async function OnGetTemplateClick(){
       await CommittedProjectsService.getUploadedCommittedProjectTemplate()
            .then((response: AxiosResponse) => {
                    if(response.data.toString() != ""){
                        FileDownload(convertBase64ToArrayBuffer(response.data), 'Committed Project Template', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                        isAdminTemplateUploaded = true;
                    }
                    else{
                         CommittedProjectsService.getCommittedProjectTemplate(networkId)
                            .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                          const fileInfo: FileInfo = response.data as FileInfo;  
                          FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                });
                    }
            });
        }

     function OnAddCommittedProjectClick(){
        const newRow: SectionCommittedProject = clone(emptySectionCommittedProject)
        newRow.id = getNewGuid();
        newRow.name = '';
        newRow.locationKeys[keyattr] = '';
        newRow.locationKeys['ID'] = getNewGuid();
        newRow.simulationId = scenarioId;
        newRow.projectSource = 'None';
        newRow.projectId = '';
        addedRows.value.push(newRow)
        onPaginationChanged();   
     }

     function OnSaveClick() {
        const upsertRequest = {
            libraryId: null,
            rowsForDeletion: deletionIds.value,
            updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
            addedRows: addedRows.value,
            isModified: false    
        };

        if (!committedProjectsAreChanged()) {
            updateNoTreatment();
            return;
        }

        const performUpsert = () => {
            CommittedProjectsService.upsertCommittedProjects(scenarioId, upsertRequest).then(handleUpsertResponse);
        };

        if (deletionIds.value.length > 0) {
            CommittedProjectsService.deleteSpecificCommittedProjects(deletionIds.value).then((response: AxiosResponse) => {
                if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                    deletionIds.value = [];
                    $emitter.emit('AllCommittedProjectsDeleted');
                    addSuccessNotificationAction({ message: 'Deleted committed projects' });
                }
                performUpsert();
            });
        } else {
            performUpsert();
        }
    }

     function handleUpsertResponse(response: AxiosResponse) {
        if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
            addSuccessNotificationAction({ message: 'Committed Projects Updated Successfully' });
            console.log(selectedCpItems.value);
            console.log(totalItems.value);
            if(totalItems.value !== 0)
            {
                $emitter.emit('CommittedProjectsUpdated');
            }
            addedRows.value = [];
            updatedRowsMap.clear();
            if (isNoTreatmentBefore.value !== isNoTreatmentBeforeCache.value) {
                updateNoTreatment();
            } else {
                resetPage();
            }
        }
    }

    function handleTreatmentChange(scp: SectionCommittedProjectTableData, treatmentName: string, row: SectionCommittedProject){
        row.treatment = treatmentName;
        updateCommittedProject(row, treatmentName, 'treatment')  
        var treatment = clone(stateSimpleScenarioSelectableTreatments.value.find(_ => _.name === treatmentName))
        if(isNil(treatment))
            return;
        CommittedProjectsService.FillTreatmentValues({
            committedProjectId: row.id,
            treatmentId: treatment.id,
            KeyAttributeValue: row.locationKeys[keyattr],
            networkId: networkId
        })
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                var values = response.data as CommittedProjectFillTreatmentReturnValues;
                row.cost = values.treatmentCost;
                row.category = values.treatmentCategory;
                scp.cost = row.cost;
                let cat = reverseCatMap.get(row.category);
                if (!isNil(cat))
                    scp.category = row.category;
                updateCommittedProject(row, row.cost, 'cost')  
                onPaginationChanged();
            }
        });
    }

     function updateNoTreatment(){
        if(isNoTreatmentBefore.value)
                ScenarioService.setNoTreatmentBeforeCommitted(scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        isNoTreatmentBeforeCache.value = isNoTreatmentBefore.value
                    }
                    resetPage()
                });
            else
                ScenarioService.removeNoTreatmentBeforeCommitted(scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        isNoTreatmentBeforeCache.value = isNoTreatmentBefore.value
                    }
                    resetPage()
                });
     }

     function OnDeleteAllClick(){
        alertDataForDeletingCommittedProjects.value = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
     }

     function OnDeleteClick(id: string){
        if(isNil(addedRows.value.find(_ => _.id === id)))
            deletionIds.value.push(id);
        else
            addedRows.value = addedRows.value.filter((scp: SectionCommittedProject) => scp.id !== id)

        onPaginationChanged();
     }

     function onEditCommittedProjectProperty(scp: SectionCommittedProjectTableData, property: string, value: any) {
        let row = sectionCommittedProjects.value.find(o => o.id === scp.id);
        if (!isNil(row)) {
            switch (property) {
                case 'treatment':
                    handleTreatmentChange(scp, value, row);
                    break;
                case 'keyAttr':
                    handleKeyAttrChange(row, scp, value);
                    break;
                case 'budget':
                    handleBudgetChange(row, scp, value);
                    break;
                case 'projectId':
                    handleprojectIdChange(row, scp, value);
                    break;
                case 'projectSource':
                    handleProjectSourceChange(row, scp, value);
                    break;
                default:
                    updateCommittedProject(row, value, property);
                    onPaginationChanged();
                    break;
            }
        }
    }

    //Dialog functions
    function onSubmitImportExportCommittedProjectsDialogResult(
        result: ImportExportCommittedProjectsDialogResult,
    ) {
        showImportExportCommittedProjectsDialog.value = false;

        if (hasValue(result)) {         
            if (hasValue(result.file)) {
                CommittedProjectsService.importCommittedProjects(
                    result.file,
                    scenarioId,
                    ).then((response: any) =>{
                        setAlertMessageAction("Committed project import has been added to the work queue");                        
                    })
            } else {
                addErrorNotificationAction({
                    message: 'No file selected.',
                    longMessage:
                        'No file selected to upload the committed projects.',
                });
            }          
        }
    }

    function onDeleteCommittedProjects() {
        alertDataForDeletingCommittedProjects.value = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }

/*     function handleAddCommittedProjectTemplateUpload(event: { target: { files: any[]; }; }){
             const file = event.target.files[0];
             CommittedProjectsService.addCommittedProjectTemplate(file).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    addSuccessNotificationAction({message:'Uploaded Template'})      
                }
            });
    }

    function onAddSelectedTemplate(){
        document.getElementById("addCommittedProjectTemplate")?.click();
    }
 */    
    function onDownloadSelectedTemplate(){
        CommittedProjectsService.getSelectedCommittedProjectTemplate(templateItemSelected.value)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    FileDownload(convertBase64ToArrayBuffer(response.data), templateItemSelected.value, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                    isAdminTemplateUploaded = true;
                }
            });
    }

    function onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        alertDataForDeletingCommittedProjects.value = { ...emptyAlertData };

        if (doDelete) {
            deleteSimulationCommittedProjectsAction(scenarioId);
            $emitter.emit('AllCommittedProjectsDeleted');
            CommittedProjectsService.deleteSimulationCommittedProjects(scenarioId).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    onCancelClick();
                }
            })
        }
    }

    //Subroutines
    function formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
    }
    function disableCrudButtons() {
    const rowChanges = addedRows.value.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rowChanges.every(
        (scp: SectionCommittedProject) => {
            return (
                rules['generalRules'].valueIsNotEmpty(
                    scp.simulationId,
                ) === true &&
                rules['generalRules'].valueIsNotEmpty(
                    scp.year,
                ) === true &&
                rules['generalRules'].valueIsNotEmpty(
                    scp.cost,
                ) === true &&
                rules['generalRules'].valueIsNotEmpty(
                    scp.treatment
                ) == true &&
                rules['generalRules'].valueIsNotEmpty(
                    scp.locationKeys[keyattr]
                ) == true &&
                rules['generalRules'].valueIsWithinRange(
                    scp.year, [firstYear, lastYear],
                ) === true
                
            );
        });
        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
}


    function setCpItems() {
        currentPage.value = sectionCommittedProjects.value.map(cpItemFactory);
        checkExistenceOfAssets();
        validateCommittedProjects();
    }


    function cpItemFactory(scp: SectionCommittedProject): SectionCommittedProjectTableData {
        const budget: SimpleBudgetDetail = find(
            propEq('id', scp.scenarioBudgetId), stateScenarioSimpleBudgetDetails.value,
        ) as SimpleBudgetDetail;
        let cat = reverseCatMap.get(scp.category);
        let value = '';
        if(!isNil(cat))
            value = cat;
        const row: SectionCommittedProjectTableData = {
            keyAttr: scp.locationKeys[keyattr],
            year: scp.year,
            cost: scp.cost,
            scenarioBudgetId: scp.scenarioBudgetId? scp.scenarioBudgetId : '',
            budget: budget? budget.name : '',
            treatment: scp.treatment,
            treatmentId: '',
            id: scp.id,
            errors: [],
            projectSourceErrors: [],
            treatmentErrors: [],
            costErrors: [],
            budgetErrors: [],
            yearErrors: [],
            category: scp.category,
            projectSource: projectSourceMap.get(+scp.projectSource) || scp.projectSource,
            projectId: scp.projectId
        }
        return row
    }

    function handleBudgetChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, budgetName: string){
        const budget: SimpleBudgetDetail = find(
            propEq('name', budgetName), stateScenarioSimpleBudgetDetails.value,
        ) as SimpleBudgetDetail;
        if(!isNil(budget)){
            row.scenarioBudgetId = budget.id;
            scp.budget = 'None'           
        }  
        else
            row.scenarioBudgetId = null;
        updateCommittedProject(row, row.scenarioBudgetId, 'scenarioBudgetId') 
        onPaginationChanged();       
    }

    function handleKeyAttrChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, keyAttr: string){
        row.locationKeys[keyattr] = keyAttr;
        updateCommittedProject(row, keyAttr, 'keyAttr');
        onPaginationChanged();
    }

    function handleFactorChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, factor: number) {
        updateCommittedProject(row, factor, 'performanceFactor');
        onPaginationChanged();
    }

    function handleProjectSourceChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, projectSource: string) {
        row.projectSource = projectSource;
        updateCommittedProject(row, projectSource, 'projectSource');
        onPaginationChanged();
    }

    function handleprojectIdChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, projectId: string) {
        row.projectId = projectId;
        updateCommittedProject(row, projectId, 'projectId');
        onPaginationChanged();
    }

    function onUploadCommittedProjectTemplate(){
      document.getElementById("committedProjectTemplateUpload")?.click();
   }

   function handleCommittedProjectTemplateUpload(event: { target: { files: any[]; }; }){
      const file = event.target.files[0];
      CommittedProjectsService.importCommittedProjectTemplate(file).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    addSuccessNotificationAction({message:'Updated Default Template'})      
                }
            });
   }

   function checkAssetExistence(scp: SectionCommittedProjectTableData, keyAttr: string){
        CommittedProjectsService.validateAssetExistence(network, keyAttr).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                if(!response.data)
                    scp.errors = [keyattr + ' does not exist'];
                else
                    scp.errors = [];
            }
        });
    }

    function fetchProjectSources() {
    CommittedProjectsService.getProjectSources().then((response: AxiosResponse) => {
        if (hasValue(response, 'data')) {

            projectSourceOptions.value = response.data.filter(
                (option: string) => option !== "None" && option !== "ProjectPick"
                );
        }
    });
}

    function checkExistenceOfAssets(){//todo: refine this
        const uncheckKeys = currentPage.value.map(scp => scp.keyAttr).filter(key => isNil(isKeyAttributeValidMap.get(key)))
        if(uncheckKeys.length > 0){
            CommittedProjectsService.validateExistenceOfAssets(uncheckKeys, network.id).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    for(let i = 0; i < currentPage.value.length; i++)
                    {
                        const check = response.data[currentPage.value[i].keyAttr]
                        if(!isNil(check)){
                            if(!response.data[currentPage.value[i].keyAttr])
                                currentPage.value[i].errors = [keyattr + ' does not exist'];
                            else
                                currentPage.value[i].errors = [];

                            isKeyAttributeValidMap.set(currentPage.value[i].keyAttr,response.data[currentPage.value[i].keyAttr] )
                        }
                    }                  
                }
            }); 
        }
        for(let i = 0; i < currentPage.value.length; i++)
        {
            if(!isKeyAttributeValidMap.get(currentPage.value[i].keyAttr))
                currentPage.value[i].errors = [keyattr + ' does not exist'];
            else
                currentPage.value[i].errors = [];
        }
                          
    }

    //Add red boxes round missing treatments
    const getTreatmentStyle = (treatment: string) => {
        const isInMissingTreatments = missingTreatmentsValue.value.includes(treatment);
        return isInMissingTreatments ? { border: '1px solid red', padding: '3px' } : {};
    };

    const getYearStyle = (year: number) => {
        const isInInvalidYears = invalidYears.value.includes(year);
        return isInInvalidYears ? { border: '1px solid red', padding: '3px' } : {};
    };

    const getProjectSourceStyle = (projectSource: string) => {
        let source = getProjectSourceKeyFromValueMap(projectSourceMap, projectSource);
        const isInInvalidProjectSources = typeof source === 'number' && invalidProjectSources.value.includes(source);
        return isInInvalidProjectSources ? { border: '1px solid red', padding: '3px' } : {};
    };
    function getProjectSourceKeyFromValueMap(map: Map<number, string>, value: string) {
        for (let [key, mapValue] of map.entries()) {
            if (mapValue === value) {
                return key;
            }
        }
        return undefined;
    }

    const getCostStyle = (cost: number) => {
        const isInInvalidCosts = invalidCosts.value.includes(cost);
        return isInInvalidCosts ? { border: '1px solid red', padding: '3px' } : {};
    };

    function validYear(year: number) {

    }

    function validTreatmentName(treatment: string) {
        return treatmentSelectItems.value.includes(treatment);
    }

    function validProjectSource(source: number | string) {
        return source !== 0 && source !== 'None'
    }

    function validCost(cost: number) {
        return cost !== -1.0 || cost !== -1;
    }

    function validateCommittedProjects() {
        currentPage.value.forEach(scp => {

            const errors = {
                yearErrors: checkYear(scp),
                treatmentErrors: checkTreatment(scp.treatment),
                costErrors: checkCost(scp.cost),
                projectSourceErrors: checkProjectSource(scp.projectSource),
            };

            scp.yearErrors = errors.yearErrors;
            scp.treatmentErrors = errors.treatmentErrors;
            scp.costErrors = errors.costErrors;
            scp.projectSourceErrors = errors.projectSourceErrors;
        });
    }

    function checkYear(scp: SectionCommittedProjectTableData) {
        const errors = [];

        if (!hasValue(scp.year)) {
            errors.push('Year cannot be empty');
            return errors;
        }

        const yearStr = scp.year.toString();
        if (yearStr.length < 4 || scp.year < 1900) {
            errors.push('Invalid year value');
        }

        if (investmentYears.value.length > 0) {
            if (scp.year < firstYear || scp.year > lastYear) {
                errors.push(`Year must be within ${firstYear} - ${lastYear}`);
            }
        } else {
            errors.push('There are no years in the investment settings');
        }

        return errors;
    }

    function checkTreatment(treatment: string) {
        const errors = [];
        if (!validTreatmentName(treatment)) {
            errors.push('Select a valid treatment');
        }
        return errors;
    }

    function checkCost(cost: number) {
        const errors = [];
        if (!validCost(cost)) {
            errors.push('Fix project cost');
        }
        return errors;
    }

    function checkProjectSource(source: string) {
        const errors = [];
        if (!validProjectSource(source)) {
            errors.push('Select a valid project source');
        }
        return errors;
    }

    function updateCommittedProject(row: SectionCommittedProject, value: any, property: string){
        const updatedRow = setItemPropertyValue(
                    property,
                    value,
                    row
                ) as SectionCommittedProject
        onUpdateRow(row.id, updatedRow);
    }

    function updateCommittedProjects(row: SectionCommittedProject, value: any, property: string){
        const updatedRow = setItemPropertyValue(
                    property,
                    value,
                    row
                ) as SectionCommittedProject
        onUpdateRow(row.id, updatedRow);
        sectionCommittedProjects.value = update(
            findIndex(
                propEq('id', row.id),
                sectionCommittedProjects.value,
            ),
            updatedRow,
            sectionCommittedProjects.value,
        );
    }

    function updateCommittedProjectTableData(row: SectionCommittedProjectTableData, value: any, property: string ){
        currentPage.value = update(
            findIndex(
                propEq('id', row.id),
                currentPage.value,
            ),
            setItemPropertyValue(
                property,
                value,
                row,
            ) as SectionCommittedProjectTableData,
            currentPage.value,
        );
    }

    function onSearchClick(){
        currentSearch.value = gridSearchTerm.value;
        resetPage();
    }

    function onClearClick(){
        gridSearchTerm.value = '';
        onSearchClick();
    }

    function onUpdateRow(rowId: string, updatedRow: SectionCommittedProject){
        updatedRow.cost = +updatedRow.cost.toString().replace(/(\$*)(\,*)/g, '')
        if(any(propEq('id', rowId), addedRows.value)){
            const index = addedRows.value.findIndex(item => item.id == updatedRow.id)
            addedRows.value[index] = updatedRow;
            return;
        }

        let mapEntry = updatedRowsMap.get(rowId)

        if(isNil(mapEntry)){
            const row = rowCache.value.find(r => r.id === rowId);
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
        projectPagination.page = 1;
        onPaginationChanged();
    }

    function checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = committedProjectsAreChanged() || isNoTreatmentBeforeCache.value != isNoTreatmentBefore.value
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function committedProjectsAreChanged() : boolean{
        return  deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if(importComp.id === scenarioId && importComp.workType == WorkType.ImportCommittedProject){
            projectPagination.page = 1
            clearChanges();
            onPaginationChanged().then(() => {
                setAlertMessageAction('');
            })
            showSuccessPopup.value = true; 
        } 
    }

    async function fetchTreatmentLibrary(simulationId: string) {
        try {
            await TreatmentService.getTreatmentLibraryBySimulationId(simulationId).then(response => {
                if (hasValue(response, 'data')) {
                    const treatmentLibrary = response.data as TreatmentLibrary;
                    store.commit('scenarioTreatmentLibraryMutator', treatmentLibrary);
                    handleLibrarySelectChange(treatmentLibrary.id);
                }
            });
            
        } catch (error) {
            addErrorNotificationAction({
                message: 'Error fetching treatment library.',
                longMessage: 'There was an issue fetching the treatment library. Please try again.'
            });
        }
    }

    async function handleLibrarySelectChange(libraryId: string) {
        selectTreatmentLibraryAction(libraryId);
        hasSelectedLibrary = true;
        const library = stateTreatmentLibraries.value.find((o) => o.id === libraryId);
        librarySelectItemValue.value = libraryId;

        if (!isNil(library)) {
            await TreatmentService.getSimpleTreatmentsByLibraryId(libraryId).then(response => {
                if (hasValue(response, 'data')) {
                    let treatments = response.data as SimpleTreatment[]
                    selectedLibraryTreatments.value = clone(treatments);
                    onSelectedLibraryTreatmentsChanged();
                }
            })         
        } 
    }   

    async function initializePages(){
        const request: PagingRequest<SectionCommittedProject>= {
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
        await CommittedProjectsService.getCommittedProjectsPage(scenarioId,request).then(response => {
            isRunning = false
            if(response.data){
                let data = response.data as PagingPage<SectionCommittedProject>;
                sectionCommittedProjects.value = data.items;
                rowCache.value = clone(sectionCommittedProjects.value)
                totalItems.value = data.totalItems;
            }
        }); 
    }
</script>
<style scoped>
.sel-style {
    width: auto;
    height: 56px;
    padding: 20px;
}
.btn-style {
    width: 300px;
    border-radius: 5px;
}
.header-border {
  border-bottom: 2px solid black;
}
.vl1-style {
justify-content: space-between;
}

.ghd-down-small svg{
    width: 12px;
}

.ghd-down-small .v-input__icon{
    position: relative;
    top: 2px;
}

</style>