<template>
    <v-row class="Montserrat-font-family">
        <v-col cols = "12">
            <v-row column >
                <v-col cols = "6">
                    <v-row>
                        <v-btn @click='OnGetTemplateClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Get Template</v-btn>
                            <input
                            id="committedProjectTemplateUpload"
                            type="file"
                            accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                            hidden/>
                        <v-btn @click="onUploadCommittedProjectTemplate"
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Upload Committed Project Template</v-btn>
                        <v-btn @click='showImportExportCommittedProjectsDialog = true' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Import Projects</v-btn>
                        <v-btn @click='OnExportProjectsClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Export Projects</v-btn>
                        <v-btn @click='OnDeleteAllClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Delete All</v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "12">                   
                    <v-row>
                        <v-select
                           :items= 'templateSelectItems'
                           class='ghd-control-border ghd-control-text ghd-select'
                           label='Select a Template'
                           style="width: 20% !important;"
                           v-model="templateItemSelected"
                           outline>
                        </v-select>
                        <v-btn @click='onDownloadSelectedTemplate' 
                        class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Download Selected Template</v-btn>
                        <input
                        id="addCommittedProjectTemplate"
                        type="file"
                        accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                        ref="committedProjectTemplateInput"
                        @change="handleAddCommittedProjectTemplateUpload"
                        hidden/>
                        <v-btn @click='onAddSelectedTemplate'  
                        class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Upload New Template</v-btn>
                    </v-row>
                </v-col>
                <v-col cols = "12">
                    <div style="padding:10px">
                    <v-checkbox 
                    id="CommittedProjectsEditor-noTreatmentsBeforeCommittedProjects-ghdcheckbox"
                    label='No Treatments Before Committed Projects' v-model='isNoTreatmentBefore' />
                </div>
                </v-col>

                <v-col cols = "6" class="ghd-constant-header">
                    <v-row>
                        <v-col cols = "6" style="margin-left: 5px">
                            <v-subheader class="ghd-control-label ghd-md-gray"></v-subheader>
                            <v-row align="center" style="padding: 10px;">                                
                                <v-text-field
                                    id="CommittedProjectsEditor-search-vtextfield"
                                    hide-details
                                    lablel="Search"
                                    placeholder="Search"
                                    single-line
                                    v-model="gridSearchTerm"
                                    variant="outlined"
                                    clearable
                                    @click:clear="onClearClick()"
                                    class="ghd-text-field-border ghd-text-field search-icon-general">
                                </v-text-field>
                                <v-btn 
                                id="CommittedProjectsEditor-performSearch-vbtn"
                                style="margin-top: 2px; margin-left: 5px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" @click="onSearchClick()">Search</v-btn>
                            </v-row>
                           
                        </v-col>
                    </v-row>
                </v-col>
                <v-col cols = "12">
                    <v-row justify-end class="px-4">
                        <p>Commited Projects: {{totalItems}}</p>
                    </v-row>
                    
                </v-col>       
                <v-col>
                    <DataTable
                        striped-rows
                        :rows="5"
                        :rows-per-page-options="[5,10,25]"
                        id="CommittedProjectsEditor-committedProjects-vdatatable"
                        class="fixed-header ghd-table v-table__overflow"
                        :value="currentPage"
                        paginator
                        paginator-template="RowsPerPageDropdown FirstPageLink PrevPageLink CurrentPageReport NextPageLink LastPageLink"
                        currentPageReportTemplate="{first} to {last} of {totalRecords}"
                        selection-mode="single"
                        table-style="min-width: 50rem"
                        >
                        <Column header="Key Attr" field="Year"></Column>
                        <Column header="Year" field="Year"></Column>
                        <Column header="Treatment" field="Year"></Column>
                        <Column header="Category" field="Year"></Column>
                        <Column header="Budget" field="Year"></Column>
                        <Column header="Cost" field="Year"></Column>
                        <Column header="Project Source" field="Year"></Column>
                        <Column header="Actions" field="Year"></Column>

                    </DataTable>
                </v-col>
                <!-- <v-col cols = "12">
                    <v-row column>
                        <v-data-table
                        id="CommittedProjectsEditor-committedProjects-vdatatable"
                        :headers="cpGridHeaders"
                        :items="currentPage"
                        sort-icon=$vuetify.icons.ghd-table-sort
                        item-key='id'
                        :pagination.sync="projectPagination"
                        :total-items="totalItems"
                        :rows-per-page-items=[5,10,25]
                        v-model="selectedCpItems"
                        class=" fixed-header v-table__overflow">
                            <template slot="items" slot-scope="props" v-slot:item="props">
                                <td v-for="header in cpGridHeaders">
                                    <div>
                                        <v-combobox v-if="header.value === 'treatment'"
                                                    :items="treatmentSelectItems"
                                                    append-icon=$vuetify.icons.ghd-down
                                                    class="ghd-down-small"
                                                    label="Select a Treatment"
                                                    v-model="props.item.treatment"
                                                    :rules="[inputRules['generalRules'].valueIsNotEmpty]"
                                                    @change="onEditCommittedProjectProperty(props.item,header.value,props.item[header.value])">
                                            
                                        </v-combobox>
                                        <v-combobox
                                            v-else-if="header.value === 'projectSource'"
                                            :items="projectSourceOptions"
                                            append-icon="$vuetify.icons.ghd-down"
                                            class="ghd-down-small"
                                            label="Select Project Source"
                                            v-model="props.item.projectSource"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                                            @change="onEditCommittedProjectProperty(props.item, header.value, props.item.projectSource)"
                                            
                                        ></v-combobox>
                                        <v-edit-dialog v-if="header.value !== 'actions' && header.value !== 'selection'"
                                            :return-value.sync="props.item[header.value]"
                                            @save="onEditCommittedProjectProperty(props.item,header.value,props.item[header.value])"
                                            size="large"
                                            lazy
                                            >
                                            <v-text-field v-if="header.value !== 'budget' 
                                                && header.value !== 'year' 
                                                && header.value !== 'keyAttr' 
                                                && header.value !== 'treatment'
                                                && header.value !== 'cost'"
                                                readonly
                                                class="sm-txt"
                                                :model-value="props.item[header.value]"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'budget'"
                                                readonly
                                                class="sm-txt"
                                                :model-value="props.item[header.value]"/>

                                            <v-text-field v-if="header.value === 'keyAttr'"
                                                readonly
                                                class="sm-txt"
                                                :model-value="props.item[header.value]"
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.errors"/>

                                            <v-text-field v-if="header.value === 'year'"
                                                :model-value="props.item[header.value]"
                                                :mask="'##########'"
                                                :rules="[inputRules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), inputRules['generalRules'].valueIsNotEmpty, inputRules['generalRules'].valueIsWithinRange(props.item[header.value], [firstYear, lastYear])]"
                                                :error-messages="props.item.yearErrors"/>

                                            <v-text-field v-if="header.value === 'cost'"
                                                :model-value='formatAsCurrency(props.item[header.value])'
                                                :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>

                                            <template v-slot:input>
                                                <v-text-field v-if="header.value === 'keyAttr'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>

                                                <v-select v-if="header.value === 'budget'"
                                                    :items="budgetSelectItems"
                                                    append-icon=$vuetify.icons.ghd-down
                                                    label="Select a Budget"
                                                    v-model="props.item[header.value]">
                                                </v-select>

                                                <v-select v-if="header.value === 'category'"
                                                    :items="categorySelectItems"
                                                    append-icon=$vuetify.icons.ghd-down
                                                    label="Select a Budget"
                                                    v-model="props.item[header.value]">
                                                </v-select>
                                                
                                                <v-text-field v-if="header.value === 'year'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :mask="'##########'"
                                                    :rules="[inputRules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [firstYear, lastYear])]"/>

                                                <v-text-field v-if="header.value === 'cost'"
                                                    label="Edit"
                                                    single-line
                                                    v-model.number="props.item[header.value]"
                                                    v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                    :rules="[inputRules['generalRules'].valueIsNotEmpty]"/>

                                            </template>
                                        </v-edit-dialog>
                                
                                        <div v-if="header.value === 'actions'">
                                            <v-row style='flex-wrap:nowrap'>
                                                <v-btn 
                                                    id="CommittedProjectsEditor-deleteCommittedProject-vbtn"
                                                    @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
                                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                                </v-btn>
                                            </v-row>
                                        </div>                            
                                    </div>
                                </td>
                            </template>
                        </v-data-table>    
                    </v-row>
                </v-col> -->
                <v-divider></v-divider>
                <v-btn id="CommittedProjectsEditor-addCommittedProject-vbtn" 
                        @click="OnAddCommittedProjectClick" v-if="selectedCommittedProject === ''"
                        class="ghd-white-bg ghd-blue ghd-button btn-style" variant = "outlined">Add Committed Project</v-btn> 

                <v-col>
                    <v-row justify="end">
                        <v-btn 
                        id="CommittedProjectsEditor-cancel-vbtn"
                        @click="onCancelClick" :disabled='!hasUnsavedChanges' class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>    
                        <div style="padding:5px"></div>

                        <v-btn 
                        id="CommittedProjectsEditor-save-vbtn"
                        @click="OnSaveClick" :disabled='!hasUnsavedChanges || disableCrudButtons()' class="ghd-blue-bg ghd-white ghd-button">Save</v-btn>    
                    </v-row>
                </v-col> 
            </v-row>
        </v-col>
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
        <CommittedProjectsFileUploaderDialog :is="ImportExportCommittedProjectsDialog"
            :showDialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
            @delete="onDeleteCommittedProjects"
        />
        <Alert
            :dialogData="alertDataForDeletingCommittedProjects"
            @submit="onDeleteCommittedProjectsSubmit"
        />

    </v-row>
</template>
<script setup lang="ts">
import { useRouter } from 'vue-router';
import { watch, ref, onMounted, onBeforeUnmount, shallowRef, computed } from 'vue'
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectFillTreatmentReturnValues, emptySectionCommittedProject, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { getBlankGuid, getNewGuid } from '../../shared/utils/uuid-utils';
import { Treatment, treatmentCategoryMap, treatmentCategoryReverseMap, TreatmentLibrary } from '@/shared/models/iAM/treatment';
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
import mitt from 'mitt';
import Dialog from 'primevue/dialog';
import Column from 'primevue/column';

    let store = useStore();
    const $router = useRouter();    
    const $emitter = mitt()
    created();

    let searchItems = '';
    let dataPerPage = 0;
    let totalDataFound = 0;
    let hasScenario: boolean = false;
    let librarySelectItemValue = ref<string>("");
    let templateSelectItems: string[] = [];
    let templateItemSelected: string = "";
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let attributeSelectItems: SelectItem[] = [];
    let treatmentSelectItems: string[] = [];
    let projectSourceOptions: string [] = [];
    let budgetSelectItems: SelectItem[] = [];
    let categorySelectItems: SelectItem[] = [];
    let categories: string[] = [];
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
        [3, "SAP"],
        [4, "ProjectBuilder"]
    ]);

    const uuidNIL: string = getBlankGuid();
    let addedRows = shallowRef<SectionCommittedProject[]>([]);
    let updatedRowsMap:Map<string, [SectionCommittedProject, SectionCommittedProject]> = new Map<string, [SectionCommittedProject, SectionCommittedProject]>();//0: original value | 1: updated value
    let deletionIds = shallowRef<string[]>([]);
    const rowCache = ref<SectionCommittedProject[]>([]);
    let gridSearchTerm = '';
    let currentSearch = '';
    let totalItems = 0;
    const currentPage = ref<SectionCommittedProjectTableData[]>([]);
    let isRunning: boolean = true;

    let selectedLibraryTreatments = ref<Treatment[]>([]);
    let isKeyAttributeValidMap: Map<string, boolean> = new Map<string, boolean>();

    let projectPagination = shallowRef<Pagination>(clone(emptyPagination));

    const currentUserCriteriaFilter = computed<UserCriteriaFilter>(() => store.state.userModule.currentUserCriteriaFilter);
    const stateSectionCommittedProjects = computed<SectionCommittedProject[]>(() => store.state.committedProjectsModule.sectionCommittedProjects);
    const committedProjectTemplate = computed<string>(() =>store.state.committedProjectsModule.committedProjectTemplate);
    const stateTreatmentLibraries = computed<TreatmentLibrary[]>(() =>store.state.treatmentModule.treatmentLibraries);
    const stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    const stateInvestmentPlan = computed<InvestmentPlan>(() => store.state.investmentModule.investmentPlan);
    const stateScenarioSimpleBudgetDetails = computed<SimpleBudgetDetail[]>(() =>store.state.investmentModule.scenarioSimpleBudgetDetails);
    const hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    const networks = computed<Network[]>(() => store.state.networkModule.networks);


    async function getCommittedProjects(payload?: any): Promise<any> { await store.dispatch('getCommittedProjects'); }
    async function getTreatmentLibrariesAction(payload?: any): Promise<any> { await store.dispatch('getTreatmentLibraries'); }
    async function getScenarioSelectableTreatmentsAction(payload?: any): Promise<any> { await store.dispatch('getScenarioSelectableTreatments'); }
    async function getInvestmentPlanAction(payload?: any): Promise<any> { await store.dispatch('getInvestmentPlan'); }
    async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any> { await store.dispatch('getScenarioSimpleBudgetDetails'); }
    async function getAttributesAction(payload?: any): Promise<any> { await store.dispatch('getAttributes'); }
    async function getNetworksAction(payload?: any): Promise<any> { await store.dispatch('getNetworks'); }
    async function deleteSpecificCommittedProjectsAction(payload?: any): Promise<any> { await store.dispatch('deleteSpecificCommittedProjects'); }
    async function deleteSimulationCommittedProjectsAction(payload?: any): Promise<any> { await store.dispatch('deleteSimulationCommittedProjects'); }
    async function upsertCommittedProjectsAction(payload?: any): Promise<any> { await store.dispatch('upsertCommittedProjects'); }

        // TODO:Remove this one?
        async function importCommittedProjectTemplate(payload?: any): Promise<any> { await store.dispatch('importComittedProjectTemplate');}

        async function selectTreatmentLibraryAction(payload?: any): Promise<any> { await store.dispatch('selectTreatmentLibrary'); }
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> { await store.dispatch('setHasUnsavedChanges'); }
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification'); }
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification'); } 
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> { await store.dispatch('getCurrentUserOrSharedScenario'); }
    async function selectScenarioAction(payload?: any): Promise<any> { await store.dispatch('selectScenario'); }
    async function setAlertMessageAction(payload?: any): Promise<any> { await store.dispatch('setAlertMessage'); }
    
    let getUserNameByIdGetter: any = store.getters.getUserNameByIdGetter;
    
    let cpItems: SectionCommittedProjectTableData[] = [];
    let selectedCpItems = ref<SectionCommittedProjectTableData[]>([]);
    let sectionCommittedProjects = shallowRef<SectionCommittedProject[]>([]);
    let committedProjectsCount: number = 0;
    let showImportExportCommittedProjectsDialog: boolean = false;
    let selectedCommittedProject = ref<string>('');
    let showCreateCommittedProjectConsequenceDialog: boolean = false;
    let disableCrudButtonsResult: boolean = true;
    let alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    let reverseCatMap = clone(treatmentCategoryReverseMap);
    let catMap = clone(treatmentCategoryMap);
    
    let keyattr: string = '';

    let lastYear: number = 0;
    let firstYear: number = 0;

    let investmentYears = ref<number[]>([]);
    let isNoTreatmentBefore = ref<boolean>(true);
    let isNoTreatmentBeforeCache = ref<boolean>(true);
    
    const cpGridHeaders: DataTableHeader[] = [
        {
            text: '',
            value: 'keyAttr',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Year',
            value: 'year',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Treatment',
            value: 'treatment',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Category',
            value: 'category',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Budget',
            value: 'budget',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Cost',
            value: 'cost',
            align: 'left',
            sortable: true,
            class: '',
            width: '10%',
        },
        {
            text: 'Actions',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '10%',
        },
    ];

    function created() {
        // reverseCatMap.forEach(cat => {
        //     categorySelectItems.push({text: cat, value: cat})        
        // });    

        // $emitter.on(
        //     Hub.BroadcastEventType.BroadcastImportCompletionEvent,
        //     importCompleted,
        // );
        // scenarioId = $router.currentRoute.value.query.scenarioId as string;
        // networkId = $router.currentRoute.value.query.networkId as string;
        
        // if (scenarioId === uuidNIL || networkId == uuidNIL) {
        //     addErrorNotificationAction({
        //        message: 'Found no selected scenario for edit',
        //     });
        //     $router.push('/Scenarios/');
        // }
    }
    onMounted(() => {
        scenarioId = "26C23F48-9B5C-4AEC-BDE0-0778AA512E10";

        (async () => { 
            hasScenario = true;
            await getNetworksAction();
            // await InvestmentService.getScenarioBudgetYears(scenarioId).then(response => {  
            //     if(response.data)
            //         investmentYears = response.data;
            // });
            // await ScenarioService.getNoTreatmentBeforeCommitted(scenarioId).then(response => {
            //         if(!isNil(response.data)){
            //             isNoTreatmentBeforeCache = response.data;
            //             isNoTreatmentBefore = response.data;
            //         }
            // });
            // await getScenarioSimpleBudgetDetailsAction({scenarioId: scenarioId});
            // await getAttributesAction();
            // await getTreatmentLibrariesAction();
            // await getCurrentUserOrSharedScenarioAction({simulationId: scenarioId});
            // await selectScenarioAction({ scenarioId: scenarioId });
            // // await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: scenarioId, workType: WorkType.ImportCommittedProject}).then(response => {
            //     if(response.data){
            //         setAlertMessageAction("Committed project import has been added to the work queue")
            //     }
            // })
            initializePages();
        //     if (vm.scenarioId !== undefined) {                
        //                     await vm.fetchTreatmentLibrary(vm.scenarioId);
        //                     await vm.fetchProjectSources();
        //                 }
        //         await CommittedProjectsService.getUploadedCommittedProjectTemplates().then(response => {
        //             if(!isNil(response.data)){
        //                     vm.templateSelectItems = response.data;
        //                 }
        //    });            
        })();                    

    });
    onBeforeUnmount(() => beforeDestroy())
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });

        // $emitter.off(
        //     Hub.BroadcastEventType.BroadcastImportCompletionEvent,
        //     importCompleted,
        // );
        setAlertMessageAction('');
    }

        //Watch
        @Watch('isNoTreatmentBefore')
    onIsNoTreatmentBeforeChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('investmentYears')
    onInvestmentYearsChanged(){
        //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/min
        if (this.investmentYears.length > 0) {
            this.lastYear = Math.max(...this.investmentYears);
            this.firstYear = Math.min(...this.investmentYears);
        }
    }

    @Watch('networks')
    onStateNetworksChanged(){
        const network = this.networks.find(o => o.id == this.networkId)
        if(!isNil(network)){
            this.network = network;
        }           
    }

    @Watch('stateAttributes')
    onStateAttributesChanged(){
        this.attributeSelectItems = this.stateAttributes.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }),
        );
        let keyAttr = this.stateAttributes.find(_ => _.id == this.network.keyAttribute)
        if(!isNil(keyAttr)){
            this.keyattr = keyAttr.name;
            this.cpGridHeaders[0].text = this.keyattr;
        }
            
    }

    @Watch('selectedLibraryTreatments', {deep: true})
    onSelectedLibraryTreatmentsChanged(){
        this.treatmentSelectItems = this.selectedLibraryTreatments.map(
            (treatment: Treatment) => (treatment.name)
        );
    }

    @Watch('stateScenarioSimpleBudgetDetails')
    onStateScenarioSimpleBudgetDetailsChanged(){
        this.budgetSelectItems = this.stateScenarioSimpleBudgetDetails.map(
            (budget: SimpleBudgetDetail) => ({
                text: budget.name,
                value: budget.name
            }),
        );
        this.budgetSelectItems.push({
            text: 'None',
            value: ''
        });
    }

    @Watch('stateSectionCommittedProjects')
        onStateSectionCommittedProjectsChanged(){
            this.sectionCommittedProjects = clone(this.stateSectionCommittedProjects);
            this.setCpItems();
    }

    @Watch('sectionCommittedProjects')
    onSectionCommittedProjectsChanged() {  
        this.setCpItems();  
    }

    @Watch('selectedCpItems')
    onSelectedCpItemsChanged(){
        if(this.selectedCpItems.length > 1)
            this.selectedCpItems.splice(0,1);
        if(this.selectedCpItems.length === 1)
            this.selectedCommittedProject = this.selectedCpItems[0].id;
    }

    @Watch('projectPagination')
    async onPaginationChanged() {
        if(this.isRunning)
            return;
        this.isRunning = true
        this.checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = this.projectPagination;

        const request: PagingRequest<SectionCommittedProject>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: this.deletionIds,
                addedRows: this.addedRows,
                isModified: false
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: this.currentSearch
        };
        if(this.scenarioId !== this.uuidNIL)
            CommittedProjectsService.getCommittedProjectsPage(this.scenarioId, request).then(response => {
                if(response.data){
                    this.isRunning = false;
                    let data = response.data as PagingPage<SectionCommittedProject>;
                    this.sectionCommittedProjects = data.items;
                    this.rowCache = clone(this.sectionCommittedProjects)
                    this.totalItems = data.totalItems;
                    const row = data.items.find(scp => scp.id == this.selectedCommittedProject);

                    if(isNil(row)) {
                        this.selectedCommittedProject = '';
                    }
                } 
            }); 
        else
            this.isRunning = false;
    }

     @Watch('deletionIds')
    onDeletionIdsChanged(){
        this.checkHasUnsavedChanges();
    }

    @Watch('addedRows')
    onAddedRowsChanged(){
        this.checkHasUnsavedChanges();
    }

    //Events
    onCancelClick() {
        this.clearChanges()
        this.selectedCommittedProject = '';
        this.selectedCpItems = [];
        this.isNoTreatmentBefore = this.isNoTreatmentBeforeCache
        this.resetPage();
    }

    OnExportProjectsClick(){
        CommittedProjectsService.exportCommittedProjects(this.scenarioId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

      async OnGetTemplateClick(){
       await CommittedProjectsService.getUploadedCommittedProjectTemplate()
            .then((response: AxiosResponse) => {
                    if(response.data.toString() != ""){
                        FileDownload(convertBase64ToArrayBuffer(response.data), 'Committed Project Template', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                        this.isAdminTemplateUploaded = true;
                    }
                    else{
                         CommittedProjectsService.getCommittedProjectTemplate(this.networkId)
                            .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                          const fileInfo: FileInfo = response.data as FileInfo;  
                          FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                });
                    }
            });

    
     }

     OnAddCommittedProjectClick(){
        const newRow: SectionCommittedProject = clone(emptySectionCommittedProject)
        newRow.id = getNewGuid();
        newRow.name = '';
        newRow.locationKeys[this.keyattr] = '';
        newRow.locationKeys['ID'] = getNewGuid();
        newRow.simulationId = this.scenarioId;
        newRow.projectSource = 'None';
        this.addedRows.push(newRow)
        this.onPaginationChanged();   
     }

     OnSaveClick(){
        const upsertRequest = {
                    libraryId: null,
                    rowsForDeletion: this.deletionIds,
                    updateRows: Array.from(this.updatedRowsMap.values()).map(r => r[1]),
                    addedRows: this.addedRows,
                    isModified: false    
                }
        if(!this.committedProjectsAreChanged())
        {
            this.updateNoTreatment();
        }
        else if(this.deletionIds.length > 0){
            CommittedProjectsService.deleteSpecificCommittedProjects(this.deletionIds).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.deletionIds = [];
                    this.addSuccessNotificationAction({message:'Deleted committed projects'})              
                }
                CommittedProjectsService.upsertCommittedProjects(this.scenarioId, upsertRequest).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        this.addSuccessNotificationAction({message:'Committed Projects Updated Successfully'}) 
                        this.addedRows = [];
                        this.updatedRowsMap.clear();
                    }
                    if(this.isNoTreatmentBefore != this.isNoTreatmentBeforeCache)
                        this.updateNoTreatment()
                    else
                        this.resetPage()
                })
            })         
        }
        else
            CommittedProjectsService.upsertCommittedProjects(this.scenarioId, upsertRequest).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.addSuccessNotificationAction({message:'Committed Projects Updated Successfully'}) 
                    this.addedRows = [];
                    this.updatedRowsMap.clear();
                }
                if(this.isNoTreatmentBefore != this.isNoTreatmentBeforeCache)
                        this.updateNoTreatment()
                else
                    this.resetPage()
            })   
     }

    handleTreatmentChange(scp: SectionCommittedProjectTableData, treatmentName: string, row: SectionCommittedProject){
    row.treatment = treatmentName;
    this.updateCommittedProject(row, treatmentName, 'treatment')  
    CommittedProjectsService.FillTreatmentValues({
        committedProjectId: row.id,
        treatmentLibraryId: this.librarySelectItemValue ? this.librarySelectItemValue : getBlankGuid(),
        treatmentName: treatmentName,
        KeyAttributeValue: row.locationKeys[this.keyattr],
        networkId: this.networkId
    })
    .then((response: AxiosResponse) => {
        if (hasValue(response, 'data')) {
            var values = response.data as CommittedProjectFillTreatmentReturnValues;
            row.cost = values.treatmentCost;
            row.category = values.treatmentCategory;
            scp.cost = row.cost;
            let cat = this.reverseCatMap.get(row.category);
            if (!isNil(cat))
                scp.category = cat;
            this.updateCommittedProject(row, row.cost, 'cost')  
            this.onPaginationChanged();
        }
    });
}

     updateNoTreatment(){
        if(this.isNoTreatmentBefore)
                ScenarioService.setNoTreatmentBeforeCommitted(this.scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        this.isNoTreatmentBeforeCache = this.isNoTreatmentBefore
                    }
                    this.resetPage()
                })
            else
                ScenarioService.removeNoTreatmentBeforeCommitted(this.scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        this.isNoTreatmentBeforeCache = this.isNoTreatmentBefore
                    }
                    this.resetPage()
                })
     }

     OnDeleteAllClick(){
        this.alertDataForDeletingCommittedProjects = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
     }

     OnDeleteClick(id: string){
        if(isNil(this.addedRows.find(_ => _.id === id)))
            this.deletionIds.push(id);
        else
            this.addedRows = this.addedRows.filter((scp: SectionCommittedProject) => scp.id !== id)

        this.onPaginationChanged();
     }

      onEditCommittedProjectProperty(scp: SectionCommittedProjectTableData, property: string, value: any) {
       let row = this.sectionCommittedProjects.find(o => o.id === scp.id)
        if(!isNil(row))
        {
            if(property === 'treatment'){
                this.handleTreatmentChange(scp, value, row)             
            }
            else if(property === 'keyAttr'){
                this.handleKeyAttrChange(row, scp, value);               
            }
            else if(property === 'budget'){
                this.handleBudgetChange(row, scp, value)
            }
            else if(property === 'projectSource') {
            this.handleProjectSourceChange(row, scp, value)
            }
            else{
                if(property === 'category')
                    value = this.catMap.get(value);
                this.updateCommittedProject(row, value, property)
                this.onPaginationChanged()
            }
        }
    }

    //Dialog functions
    onSubmitImportExportCommittedProjectsDialogResult(
        result: ImportExportCommittedProjectsDialogResult,
    ) {
        this.showImportExportCommittedProjectsDialog = false;

        if (hasValue(result)) {         
            if (hasValue(result.file)) {
                CommittedProjectsService.importCommittedProjects(
                    result.file,
                    result.applyNoTreatment,
                    this.scenarioId,
                ).then((response: any) =>{
                    this.setAlertMessageAction("Committed project import has been added to the work queue")
                })
            } else {
                this.addErrorNotificationAction({
                    message: 'No file selected.',
                    longMessage:
                        'No file selected to upload the committed projects.',
                });
            }          
        }
    }

    onDeleteCommittedProjects() {
        this.alertDataForDeletingCommittedProjects = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }

    handleAddCommittedProjectTemplateUpload(event: { target: { files: any[]; }; }){
             const file = event.target.files[0];
             CommittedProjectsService.addCommittedProjectTemplate(file).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.addSuccessNotificationAction({message:'Uploaded Template'})      
                }
            });
    }

    onAddSelectedTemplate(){
        document.getElementById("addCommittedProjectTemplate")?.click();
    }
    
    onDownloadSelectedTemplate(){
        CommittedProjectsService.getSelectedCommittedProjectTemplate(this.templateItemSelected)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    FileDownload(convertBase64ToArrayBuffer(response.data), this.templateItemSelected, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                    this.isAdminTemplateUploaded = true;
                }
            });
    }

    onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        this.alertDataForDeletingCommittedProjects = { ...emptyAlertData };

        if (doDelete) {
            this.deleteSimulationCommittedProjectsAction(this.scenarioId);
            CommittedProjectsService.deleteSimulationCommittedProjects(this.scenarioId).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.onCancelClick();
                }
            })
        }
    }

    //Subroutines
    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
    }
    disableCrudButtons() {
    const rowChanges = this.addedRows.concat(Array.from(this.updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rowChanges.every(
        (scp: SectionCommittedProject) => {
            return (
                this.rules['generalRules'].valueIsNotEmpty(
                    scp.simulationId,
                ) === true &&
                this.rules['generalRules'].valueIsNotEmpty(
                    scp.year,
                ) === true &&
                this.rules['generalRules'].valueIsNotEmpty(
                    scp.cost,
                ) === true &&
                this.rules['generalRules'].valueIsNotEmpty(
                    scp.treatment
                ) == true &&
                this.rules['generalRules'].valueIsNotEmpty(
                    scp.locationKeys[this.keyattr]
                ) == true &&
                this.rules['generalRules'].valueIsWithinRange(
                    scp.year, [this.firstYear, this.lastYear],
                ) === true &&
                scp.projectSource !== ""
                
            );
        },
    );
        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
}


    setCpItems(){
        this.currentPage = this.sectionCommittedProjects.map(o => 
        {          
            const row: SectionCommittedProjectTableData = this.cpItemFactory(o);
            return row
        })
        this.checkExistenceOfAssets();
        this.checkYears();
    }

    cpItemFactory(scp: SectionCommittedProject): SectionCommittedProjectTableData {
        const budget: SimpleBudgetDetail = find(
            propEq('id', scp.scenarioBudgetId), this.stateScenarioSimpleBudgetDetails,
        ) as SimpleBudgetDetail;
        let cat = this.reverseCatMap.get(scp.category);
        let value = '';
        if(!isNil(cat))
            value = cat;
        const row: SectionCommittedProjectTableData = {
            keyAttr: scp.locationKeys[this.keyattr],
            year: scp.year,
            cost: scp.cost,
            scenarioBudgetId: scp.scenarioBudgetId? scp.scenarioBudgetId : '',
            budget: budget? budget.name : '',
            treatment: scp.treatment,
            treatmentId: '',
            id: scp.id,
            errors: [],
            yearErrors: [],
            category: value,
            projectSource: projectSourceMap.get(+scp.projectSource) || scp.projectSource
        }
        return row
    }

    handleBudgetChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, budgetName: string){
        const budget: SimpleBudgetDetail = find(
            propEq('name', budgetName), this.stateScenarioSimpleBudgetDetails,
        ) as SimpleBudgetDetail;
        if(!isNil(budget)){
            row.scenarioBudgetId = budget.id;
            scp.budget = 'None'           
        }  
        else
            row.scenarioBudgetId = null;
        this.updateCommittedProject(row, row.scenarioBudgetId, 'scenarioBudgetId') 
        this.onPaginationChanged();       
    }

    handleKeyAttrChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, keyAttr: string){
        row.locationKeys[this.keyattr] = keyAttr;
        this.updateCommittedProject(row, keyAttr, 'keyAttr');
        this.onPaginationChanged();
    }

    handleFactorChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, factor: number) {
        this.updateCommittedProject(row, factor, 'performanceFactor');
        this.onPaginationChanged();
    }

    handleProjectSourceChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, projectSource: string) {
        row.projectSource = projectSource;
    this.updateCommittedProject(row, projectSource, 'projectSource');
    this.onPaginationChanged();
    }

    onUploadCommittedProjectTemplate(){
      document.getElementById("committedProjectTemplateUpload")?.click();
   }

    handleCommittedProjectTemplateUpload(event: { target: { files: any[]; }; }){
      const file = event.target.files[0];
      CommittedProjectsService.importCommittedProjectTemplate(file).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    this.addSuccessNotificationAction({message:'Updated Default Template'})      
                }
            });
   }

    checkAssetExistence(scp: SectionCommittedProjectTableData, keyAttr: string){
        CommittedProjectsService.validateAssetExistence(this.network, keyAttr).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                if(!response.data)
                    scp.errors = [this.keyattr + ' does not exist'];
                else
                    scp.errors = [];
            }
        });
    }

    function fetchProjectSources() {
    CommittedProjectsService.getProjectSources().then((response: AxiosResponse) => {
        if (hasValue(response, 'data')) {
            projectSourceOptions = response.data.filter((option: string) => option !== "None");
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

    function checkYear(scp:SectionCommittedProjectTableData){
        if(!hasValue(scp.year))
            scp.yearErrors = ['Value cannot be empty'];
        else if (investmentYears.value.length === 0)
            scp.yearErrors = ['There are no years in the investment settings']
        else if(scp.year.toString().length < 4 || scp.year < 1900)
            scp.yearErrors = ['Invalid Year value'];      
        else
            scp.yearErrors = [];
    }

    function checkYears()
    {
        currentPage.value.forEach(scp => {
            checkYear(scp);
        })
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
        currentSearch = gridSearchTerm;
        resetPage();
    }

    function onClearClick(){
        gridSearchTerm = '';
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
        const hasUnsavedChanges: boolean = committedProjectsAreChanged() || isNoTreatmentBeforeCache != isNoTreatmentBefore
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function committedProjectsAreChanged() : boolean{
        return  deletionIds.value.length > 0 || 
            addedRows.value.length > 0 ||
            updatedRowsMap.size > 0 || (hasScenario && hasSelectedLibrary)
    }

    function importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if(importComp.id === scenarioId && importComp.workType == WorkType.ImportCommittedProject){
            projectPagination.page = 1
            clearChanges();
            onPaginationChanged().then(() => {
                setAlertMessageAction('');
            })
        }        
    }

    async function fetchTreatmentLibrary(simulationId: string) {
        try {
            const response = await TreatmentService.getTreatmentLibraryBySimulationId(simulationId);

            if (hasValue(response, 'data')) {
                const treatmentLibrary = response.data as TreatmentLibrary;
                store.commit('scenarioTreatmentLibraryMutator', treatmentLibrary);
                handleLibrarySelectChange(treatmentLibrary.id);
            }
        } catch (error) {
            addErrorNotificationAction({
                message: 'Error fetching treatment library.',
                longMessage: 'There was an issue fetching the treatment library. Please try again.'
            });
        }
    }

    function handleLibrarySelectChange(libraryId: string) {
        selectTreatmentLibraryAction(libraryId);
        hasSelectedLibrary = true;
    const library = stateTreatmentLibraries.value.find((o) => o.id === libraryId);

    if (!isNil(library)) {
      selectedLibraryTreatments.value = library.treatments;
      onSelectedLibraryTreatmentsChanged();
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
                totalItems = data.totalItems;
            }
        }); 
    }
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