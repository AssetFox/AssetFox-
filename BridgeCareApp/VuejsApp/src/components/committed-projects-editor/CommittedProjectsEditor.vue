<template>
    <v-row class="Montserrat-font-family">
        <v-col cols = "12">
            <v-row column >
                <v-col cols = "12">
                    <v-row>
                        <v-btn @click='OnGetTemplateClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" variant = "outlined">Get Template</v-btn>
                            <input
                            id="committedProjectTemplateUpload"
                            type="file"
                            accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                            ref="committedProjectTemplateInput"
                            @change="handleCommittedProjectTemplateUpload"
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
                    <v-checkbox 
                    id="CommittedProjectsEditor-noTreatmentsBeforeCommittedProjects-ghdcheckbox"
                    class='ghd-checkbox' label='No Treatments Before Committed Projects' v-model='isNoTreatmentBefore' />
                </v-col>

                <v-col cols = "12" class="ghd-constant-header">
                    <v-row>
                        <v-col cols = "6" style="margin-left: 5px">
                            <v-subheader class="ghd-control-label ghd-md-gray"></v-subheader>
                            <v-row>                                
                                <v-text-field
                                    id="CommittedProjectsEditor-search-vtextfield"
                                    prepend-inner-icon=$vuetify.icons.ghd-search
                                    hide-details
                                    lablel="Search"
                                    placeholder="Search"
                                    single-line
                                    v-model="gridSearchTerm"
                                    outline
                                    clearable
                                    @click:clear="onClearClick()"
                                    class="ghd-text-field-border ghd-text-field search-icon-general">
                                </v-text-field>
                                <v-btn 
                                id="CommittedProjectsEditor-performSearch-vbtn"
                                style="margin-top: 2px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" @click="onSearchClick()">Search</v-btn>
                            </v-row>
                           
                        </v-col>
                    </v-row>
                </v-col>
                <v-col cols = "12">
                    <v-row justify-end class="px-4">
                        <p>Commited Projects: {{totalItems}}</p>
                    </v-row>
                    
                </v-col>       
                
                <v-col cols = "12">
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
                        <v-btn id="CommittedProjectsEditor-addCommittedProject-vbtn" 
                        @click="OnAddCommittedProjectClick" v-if="selectedCommittedProject === ''"
                        class="ghd-white-bg ghd-blue ghd-button btn-style" variant = "outlined">Add Committed Project</v-btn> 
                    </v-row>
                </v-col>

                <v-divider></v-divider>

                <v-col cols = "12">
                    <v-row justify-center>
                        <v-btn 
                        id="CommittedProjectsEditor-cancel-vbtn"
                        @click="onCancelClick" :disabled='!hasUnsavedChanges' class="ghd-white-bg ghd-blue ghd-button-text" variant = "flat">Cancel</v-btn>    
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
<script lang="ts" setup>
import { useRouter } from 'vue-router';
import { watch, ref, inject, onBeforeUnmount, shallowRef } from 'vue'
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

    let store = useStore();
    const $router = useRouter();
    const $statusHub = inject('$statusHub') as any
    const $emitter = mitt()
    created();

    let searchItems = '';
    let dataPerPage = 0;
    let totalDataFound = 0;
    let hasScenario: boolean = false;
    let librarySelectItemValue = ref<string>('');
    let hasSelectedLibrary: boolean = false;
    let librarySelectItems: SelectItem[] = [];
    let attributeSelectItems: SelectItem[] = [];
    let treatmentSelectItems: string[] = [];
    let budgetSelectItems: SelectItem[] = [];
    let categorySelectItems: SelectItem[] = [];
    let categories: string[] = [];
    let scenarioId: string = getBlankGuid();
    let networkId: string = getBlankGuid();
    let inputRules: InputValidationRules = rules;
    let network: Network = clone(emptyNetwork);
    let isAdminTemplateUploaded: Boolean;

    const uuidNIL: string = getBlankGuid();
    let addedRows = shallowRef<SectionCommittedProject[]>([]);
    let updatedRowsMap:Map<string, [SectionCommittedProject, SectionCommittedProject]> = new Map<string, [SectionCommittedProject, SectionCommittedProject]>();//0: original value | 1: updated value
    let deletionIds = shallowRef<string[]>([]);
    let rowCache: SectionCommittedProject[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let totalItems = 0;
    let currentPage: SectionCommittedProjectTableData[] = [];
    let isRunning: boolean = true;

    let selectedLibraryTreatments = ref<Treatment[]>([]);
    let isKeyAttributeValidMap: Map<string, boolean> = new Map<string, boolean>();

    let projectPagination = shallowRef<Pagination>(clone(emptyPagination));

    let currentUserCriteriaFilter = ref<UserCriteriaFilter>(store.state.userModule.currentUserCriteriaFilter);
    let stateSectionCommittedProjects = ref<SectionCommittedProject[]>(store.state.committedProjectsModule.sectionCommittedProjects);
    let committedProjectTemplate = ref<string>(store.state.committedProjectsModule.committedProjectTemplate);
    let stateTreatmentLibraries = ref<TreatmentLibrary[]>(store.state.treatmentModule.treatmentLibraries);
    let stateAttributes = ref<Attribute[]>(store.state.attributeModule.attributes);
    let stateInvestmentPlan = ref<InvestmentPlan>(store.state.investmentModule.investmentPlan);
    let stateScenarioSimpleBudgetDetails = ref<SimpleBudgetDetail[]>(store.state.investmentModule.scenarioSimpleBudgetDetails);
    let hasUnsavedChanges = ref<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let networks = ref<Network[]>(store.state.networkModule.networks);

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
        reverseCatMap.forEach(cat => {
            categorySelectItems.push({text: cat, value: cat})        
        });    

        $emitter.on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
        scenarioId = $router.currentRoute.value.query.scenarioId as string;
        networkId = $router.currentRoute.value.query.networkId as string;
        
        if (scenarioId === uuidNIL || networkId == uuidNIL) {
            addErrorNotificationAction({
               message: 'Found no selected scenario for edit',
            });
            $router.push('/Scenarios/');
        }
        (async () => { 
            hasScenario = true;
            await getNetworksAction();
            await InvestmentService.getScenarioBudgetYears(scenarioId).then(response => {  
                if(response.data)
                    investmentYears = response.data;
            });
            await ScenarioService.getNoTreatmentBeforeCommitted(scenarioId).then(response => {
                    if(!isNil(response.data)){
                        isNoTreatmentBeforeCache = response.data;
                        isNoTreatmentBefore = response.data;
                    }
            });
            await getScenarioSimpleBudgetDetailsAction({scenarioId: scenarioId});
            await getAttributesAction();
            await getTreatmentLibrariesAction();
            await getCurrentUserOrSharedScenarioAction({simulationId: scenarioId});
            await selectScenarioAction({ scenarioId: scenarioId });
            await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: scenarioId, workType: WorkType.ImportCommittedProject}).then(response => {
                if(response.data){
                    setAlertMessageAction("Committed project import has been added to the work queue")
                }
            })
            await initializePages()
        })();                    
    }

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
    watch(isNoTreatmentBefore, () =>onIsNoTreatmentBeforeChanged)
    function onIsNoTreatmentBeforeChanged(){
        checkHasUnsavedChanges();
    }

    watch(investmentYears, () => onInvestmentYearsChanged)
    function onInvestmentYearsChanged(){
        //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/min
        if (investmentYears.value.length > 0) {
            lastYear = Math.max(...investmentYears.value);
            firstYear = Math.min(...investmentYears.value);
        }
    }

    watch(networks, () => onStateNetworksChanged)
    function onStateNetworksChanged(){
        const net = networks.value.find(o => o.id == networkId)
        if(!isNil(net)){
            network = net;
        }           
    }

    watch(stateTreatmentLibraries, () => onStateTreatmentLibrariesChanged)
    function onStateTreatmentLibrariesChanged() {
        librarySelectItems = stateTreatmentLibraries.value.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id
            }),
        );
    }

    watch(selectedLibraryTreatments, () => onSelectedLibraryTreatmentsChanged)
    function onSelectedLibraryTreatmentsChanged(){
        treatmentSelectItems = selectedLibraryTreatments.value.map(
            (treatment: Treatment) => (treatment.name)
        );
    }

    watch(stateAttributes, () => onStateAttributesChanged)
    function onStateAttributesChanged(){
        attributeSelectItems = stateAttributes.value.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }),
        );
        let keyAttr = stateAttributes.value.find(_ => _.id == network.keyAttribute)
        if(!isNil(keyAttr)){
            keyattr = keyAttr.name;
            cpGridHeaders[0].text = keyattr;
        }
            
    }

    watch(stateScenarioSimpleBudgetDetails, () => onStateScenarioSimpleBudgetDetailsChanged)
    function onStateScenarioSimpleBudgetDetailsChanged(){
        budgetSelectItems = stateScenarioSimpleBudgetDetails.value.map(
            (budget: SimpleBudgetDetail) => ({
                text: budget.name,
                value: budget.name
            }),
        );
        budgetSelectItems.push({
            text: 'None',
            value: ''
        });
    }

    watch(stateSectionCommittedProjects, () => onStateSectionCommittedProjectsChanged)
    function onStateSectionCommittedProjectsChanged(){
            sectionCommittedProjects.value = clone(stateSectionCommittedProjects.value);
            setCpItems();
    }

    watch(sectionCommittedProjects, () => onSectionCommittedProjectsChanged)
    function onSectionCommittedProjectsChanged() {  
        setCpItems();  
    }

    watch(selectedCpItems, () => onSelectedCpItemsChanged)
    function onSelectedCpItemsChanged(){
        if(selectedCpItems.value.length > 1)
            selectedCpItems.value.splice(0,1);
        if(selectedCpItems.value.length === 1)
            selectedCommittedProject.value = selectedCpItems.value[0].id;
    }

    watch(projectPagination, () => onPaginationChanged)
    async function onPaginationChanged() {
        if(isRunning)
            return;
        isRunning = true
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = projectPagination.value;

        const request: PagingRequest<SectionCommittedProject>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds.value,
                addedRows: addedRows.value,
                isModified: false
            },           
            sortColumn: sortBy,
            isDescending: descending != null ? descending : false,
            search: currentSearch
        };
        if(scenarioId !== uuidNIL)
            CommittedProjectsService.getCommittedProjectsPage(scenarioId, request).then(response => {
                if(response.data){
                    isRunning = false;
                    let data = response.data as PagingPage<SectionCommittedProject>;
                    sectionCommittedProjects.value = data.items;
                    rowCache = clone(sectionCommittedProjects.value)
                    totalItems = data.totalItems;
                    const row = data.items.find(scp => scp.id == selectedCommittedProject.value);

                    if(isNil(row)) {
                        selectedCommittedProject.value = '';
                    }
                } 
            }); 
        else
            isRunning = false;
    }

    watch(deletionIds, () => onDeletionIdsChanged)
    function onDeletionIdsChanged(){
        checkHasUnsavedChanges();
    }

    watch(addedRows, () => onAddedRowsChanged)
    function onAddedRowsChanged(){
        checkHasUnsavedChanges();
    }

    //Events
    function onCancelClick() {
        clearChanges()
        selectedCommittedProject.value = '';
        selectedCpItems.value = [];
        isNoTreatmentBefore = isNoTreatmentBeforeCache
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

     function OnGetTemplateClick(){
        CommittedProjectsService.getUploadedCommittedProjectTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    FileDownload(convertBase64ToArrayBuffer(response.data), 'Committed Project Template', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
                    isAdminTemplateUploaded = true;
                }
            });

            if(isAdminTemplateUploaded = false){
                 CommittedProjectsService.getCommittedProjectTemplate(networkId)
                 .then((response: AxiosResponse) => {
                        if (hasValue(response, 'data')) {
                          const fileInfo: FileInfo = response.data as FileInfo;  
                          FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                        }
                });
            }
     }

     function OnAddCommittedProjectClick(){
        const newRow: SectionCommittedProject = clone(emptySectionCommittedProject)
        newRow.id = getNewGuid();
        newRow.name = '';
        newRow.locationKeys[keyattr] = '';
        newRow.locationKeys['ID'] = getNewGuid();
        newRow.simulationId = scenarioId;
        addedRows.value.push(newRow)
        onPaginationChanged();
     }

     function OnSaveClick(){
        const upsertRequest = {
                    libraryId: null,
                    rowsForDeletion: deletionIds.value,
                    updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: addedRows.value,
                    isModified: false    
                }
        if(!committedProjectsAreChanged())
        {
            updateNoTreatment();
        }
        else if(deletionIds.value.length > 0){
            CommittedProjectsService.deleteSpecificCommittedProjects(deletionIds.value).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    deletionIds.value = [];
                    addSuccessNotificationAction({message:'Deleted committed projects'})              
                }
                CommittedProjectsService.upsertCommittedProjects(scenarioId, upsertRequest).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        addSuccessNotificationAction({message:'Committed Projects Updated Successfully'}) 
                        addedRows.value = [];
                        updatedRowsMap.clear();
                    }
                    if(isNoTreatmentBefore != isNoTreatmentBeforeCache)
                        updateNoTreatment()
                    else
                        resetPage()
                })
            })         
        }
        else
            CommittedProjectsService.upsertCommittedProjects(scenarioId, upsertRequest).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    addSuccessNotificationAction({message:'Committed Projects Updated Successfully'}) 
                    addedRows.value = [];
                    updatedRowsMap.clear();
                }
                if(isNoTreatmentBefore != isNoTreatmentBeforeCache)
                    updateNoTreatment()
                else
                    resetPage()
            })   
     }

     function updateNoTreatment(){
        if(isNoTreatmentBefore)
                ScenarioService.setNoTreatmentBeforeCommitted(scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        isNoTreatmentBeforeCache = isNoTreatmentBefore
                    }
                    resetPage()
                })
            else
                ScenarioService.removeNoTreatmentBeforeCommitted(scenarioId).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        isNoTreatmentBeforeCache = isNoTreatmentBefore
                    }
                    resetPage()
                })
     }

     function OnDeleteAllClick(){
        alertDataForDeletingCommittedProjects = {
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
       let row = sectionCommittedProjects.value.find(o => o.id === scp.id)
        if(!isNil(row))
        {
            if(property === 'keyAttr'){
                handleKeyAttrChange(row, scp, value);               
            }
            else if(property === 'budget'){
                handleBudgetChange(row, scp, value)
            }
            else{
                if(property === 'category')
                    value = catMap.get(value);
                updateCommittedProject(row, value, property)
                onPaginationChanged()
            }
        }
    }

    //Dialog functions
    function onSubmitImportExportCommittedProjectsDialogResult(
        result: ImportExportCommittedProjectsDialogResult,
    ) {
        showImportExportCommittedProjectsDialog = false;

        if (hasValue(result)) {         
            if (hasValue(result.file)) {
                CommittedProjectsService.importCommittedProjects(
                    result.file,
                    result.applyNoTreatment,
                    scenarioId,
                ).then((response: any) =>{
                    setAlertMessageAction("Committed project import has been added to the work queue")
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
        alertDataForDeletingCommittedProjects = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }   

    function onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        alertDataForDeletingCommittedProjects = { ...emptyAlertData };

        if (doDelete) {
            deleteSimulationCommittedProjectsAction(scenarioId);
            CommittedProjectsService.deleteSimulationCommittedProjects(scenarioId).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    onCancelClick();
                }
            })
        }
    }

    //Subroutines
    function formatAsCurrency(value: any): any {
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
                    inputRules['generalRules'].valueIsNotEmpty(
                        scp.simulationId,
                    ) === true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        scp.year,
                    ) === true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        scp.cost,
                    ) === true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        scp.treatment
                    ) == true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        scp.locationKeys[keyattr]
                    ) == true &&
                    inputRules['generalRules'].valueIsWithinRange(
                        scp.year, [firstYear, lastYear],
                    ) === true
                );
            },
        );
        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    function setCpItems(){
        currentPage = sectionCommittedProjects.value.map(o => 
        {          
            const row: SectionCommittedProjectTableData = cpItemFactory(o);
            return row
        })
        checkExistenceOfAssets();
        checkYears();
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
            yearErrors: [],
            category: value
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

    function checkExistenceOfAssets(){//todo: refine this
        const uncheckKeys = currentPage.map(scp => scp.keyAttr).filter(key => isNil(isKeyAttributeValidMap.get(key)))
        if(uncheckKeys.length > 0){
            CommittedProjectsService.validateExistenceOfAssets(uncheckKeys, network.id).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    for(let i = 0; i < currentPage.length; i++)
                    {
                        const check = response.data[currentPage[i].keyAttr]
                        if(!isNil(check)){
                            if(!response.data[currentPage[i].keyAttr])
                                currentPage[i].errors = [keyattr + ' does not exist'];
                            else
                                currentPage[i].errors = [];

                            isKeyAttributeValidMap.set(currentPage[i].keyAttr,response.data[currentPage[i].keyAttr] )
                        }
                    }                  
                }
            }); 
        }
        for(let i = 0; i < currentPage.length; i++)
        {
            if(!isKeyAttributeValidMap.get(currentPage[i].keyAttr))
                currentPage[i].errors = [keyattr + ' does not exist'];
            else
                currentPage[i].errors = [];
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
        currentPage.forEach(scp => {
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
        currentPage = update(
            findIndex(
                propEq('id', row.id),
                currentPage,
            ),
            setItemPropertyValue(
                property,
                value,
                row,
            ) as SectionCommittedProjectTableData,
            currentPage,
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
        projectPagination.value.page = 1;
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
            projectPagination.value.page = 1
            clearChanges();
            onPaginationChanged().then(() => {
                setAlertMessageAction('');
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
                rowCache = clone(sectionCommittedProjects.value)
                totalItems = data.totalItems;
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