<template>
    <v-layout class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout column >
                <v-flex xs12>
                    <v-layout>
                        <v-btn @click='OnGetTemplateClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Get Default Template</v-btn>
                            <input
                            id="committedProjectTemplateUpload"
                            type="file"
                            accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                            ref="committedProjectTemplateInput"
                            @change="handleCommittedProjectTemplateUpload"
                            hidden/>
                        <v-btn @click="onUploadCommittedProjectTemplate" style="margin-right: auto;"
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Change Default Template</v-btn>
                        <v-btn @click='showImportExportCommittedProjectsDialog = true' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Import Projects</v-btn>
                        <v-btn @click='OnExportProjectsClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Export Projects</v-btn>
                        <v-btn @click='OnDeleteAllClick' 
                            class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button" outline>Delete All</v-btn>
                    </v-layout>
                </v-flex>
                <v-flex xs12>                   
                    <vlayout>
                             <v-select
                                :items= 'templateSelectItems'
                                append-icon=$vuetify.icons.ghd-down
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
                    </vlayout>
                </v-flex>
                <v-flex xs12>
                    <v-checkbox 
                    id="CommittedProjectsEditor-noTreatmentsBeforeCommittedProjects-ghdcheckbox"
                    class='ghd-checkbox' label='No Treatments Before Committed Projects' v-model='isNoTreatmentBefore' />
                </v-flex>

                <v-flex xs12 class="ghd-constant-header">
                    <v-layout>
                        <v-flex xs6 style="margin-left: 5px">
                            <v-subheader class="ghd-control-label ghd-md-gray"></v-subheader>
                            <v-layout>                                
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
                                style="margin-top: 2px;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline @click="onSearchClick()">Search</v-btn>
                            </v-layout>
                           
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex xs12>
                    <v-layout justify-end class="px-4">
                        <p>Commited Projects: {{totalItems}}</p>
                    </v-layout>
                    
                </v-flex>       
                
                <v-flex xs12 >
                    <v-layout column>
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
                            <template slot="items" slot-scope="props">
                                <td v-for="header in cpGridHeaders">
                                    <div>
                                        <v-combobox v-if="header.value === 'treatment'"
                                                    :items="treatmentSelectItems"
                                                    append-icon=$vuetify.icons.ghd-down
                                                    class="ghd-down-small"
                                                    label="Select a Treatment"
                                                    v-model="props.item.treatment"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"
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
                                            large
                                            lazy
                                            >
                                            <v-text-field v-if="header.value !== 'budget' 
                                                && header.value !== 'year' 
                                                && header.value !== 'keyAttr' 
                                                && header.value !== 'treatment'
                                                && header.value !== 'cost'
                                                && header.value !== 'projectSource'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'budget'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"/>

                                            <v-text-field v-if="header.value === 'keyAttr'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.errors"/>

                                            <v-text-field v-if="header.value === 'year'"
                                                :value="props.item[header.value]"
                                                :mask="'##########'"
                                                :rules="[rules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.yearErrors"/>

                                            <v-text-field v-if="header.value === 'cost'"
                                                :value='formatAsCurrency(props.item[header.value])'
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                                
                                            <template slot="input">
                                                <v-text-field v-if="header.value === 'keyAttr'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

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
                                                    :rules="[rules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty]"/>

                                                <v-text-field v-if="header.value === 'cost'"
                                                    label="Edit"
                                                    single-line
                                                    v-model.number="props.item[header.value]"
                                                    v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                                
                                            </template>
                                        </v-edit-dialog>
                                
                                        <div v-if="header.value === 'actions'">
                                            <v-layout style='flex-wrap:nowrap'>
                                                <v-btn 
                                                    id="CommittedProjectsEditor-deleteCommittedProject-vbtn"
                                                    @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
                                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                                </v-btn>
                                            </v-layout>
                                        </div>                            
                                    </div>
                                </td>
                            </template>
                        </v-data-table>    
                        <v-btn id="CommittedProjectsEditor-addCommittedProject-vbtn" 
                        @click="OnAddCommittedProjectClick" v-if="selectedCommittedProject === ''"
                        class="ghd-white-bg ghd-blue ghd-button btn-style" outline>Add Committed Project</v-btn> 
                    </v-layout>
                </v-flex>

                <v-divider></v-divider>

                <v-flex xs12>
                    <v-layout justify-center>
                        <v-btn 
                        id="CommittedProjectsEditor-cancel-vbtn"
                        @click="onCancelClick" :disabled='!hasUnsavedChanges' class="ghd-white-bg ghd-blue ghd-button-text" flat>Cancel</v-btn>    
                        <v-btn 
                        id="CommittedProjectsEditor-save-vbtn"
                        @click="OnSaveClick" :disabled='!hasUnsavedChanges || disableCrudButtons()' class="ghd-blue-bg ghd-white ghd-button">Save</v-btn>    
                    </v-layout>
                </v-flex> 
            </v-layout>
        </v-flex>
        <v-flex xs8 style="border:1px solid #999999 !important;" v-if="selectedCommittedProject !== ''">
            <v-layout column>
                <v-flex xs12>
                    <v-btn 
                       id="CommittedProjectsEditor-closeSelectedCommitedProject-vbtn"
                       @click="selectedCommittedProject = ''" flat class="ghd-close-button">
                        X
                    </v-btn>
                </v-flex>
              
            </v-layout>
        </v-flex>
        <CommittedProjectsFileUploaderDialog
            :showDialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
            @delete="onDeleteCommittedProjects"
        />
        <Alert
            :dialogData="alertDataForDeletingCommittedProjects"
            @submit="onDeleteCommittedProjectsSubmit"
        />

    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectFillTreatmentReturnValues, emptySectionCommittedProject, SectionCommittedProject, CommittedProjectTemplates, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { Action, Getter, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
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
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { isNullOrUndefined } from 'util';
import { max } from 'moment';
import { stat } from 'fs';
import { Hub } from '@/connectionHub';
import { WorkType } from '@/shared/models/iAM/scenario';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import TreatmentService from '@/services/treatment.service';

const projectSourceMap = new Map<number, string>([
    [0, "None"],
    [1, "iAMPick"],
    [2, "Committed"],
    [3, "SAP"],
    [4, "Project Builder"]
]);

@Component({
    components: {
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        Alert
    },
})
export default class CommittedProjectsEditor extends Vue  {
    searchItems = '';
    dataPerPage = 0;
    totalDataFound = 0;
    librarySelectItemValue: string | null = null;
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    templateSelectItems: string[] = [];
    templateItemSelected: string = "";
    attributeSelectItems: SelectItem[] = [];
    treatmentSelectItems: string[] = [];
    projectSourceOptions: string [] = [];
    budgetSelectItems: SelectItem[] = [];
    categorySelectItems: SelectItem[] = [];
    categories: string[] = [];
    scenarioId: string = getBlankGuid();
    networkId: string = getBlankGuid();
    rules: InputValidationRules = rules;
    network: Network = clone(emptyNetwork);
    selectedLibraryTreatments: Treatment[];
    isAdminTemplateUploaded: Boolean
    fileData: AxiosResponse

    addedRows: SectionCommittedProject[] = [];
    updatedRowsMap:Map<string, [SectionCommittedProject, SectionCommittedProject]> = new Map<string, [SectionCommittedProject, SectionCommittedProject]>();//0: original value | 1: updated value
    deletionIds: string[] = [];
    rowCache: SectionCommittedProject[] = [];
    gridSearchTerm = '';
    currentSearch = '';
    totalItems = 0;
    currentPage: SectionCommittedProjectTableData[] = [];
    isRunning: boolean = true;

    isKeyAttributeValidMap: Map<string, boolean> = new Map<string, boolean>();

    projectPagination: Pagination = clone(emptyPagination);

    @State(state => state.committedProjectsModule.sectionCommittedProjects) stateSectionCommittedProjects: SectionCommittedProject[];
    @State(state => state.committedProjectsModule.committedProjectTemplate) committedProjectTemplate: string;
    @State(state => state.treatmentModule.treatmentLibraries)stateTreatmentLibraries: TreatmentLibrary[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.investmentModule.investmentPlan) stateInvestmentPlan: InvestmentPlan;
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.networkModule.networks) networks: Network[];

    @Action('getCommittedProjects') getCommittedProjects: any;
    @Action('importComittedProjectTemplate') importCommittedProjectTemplate: any;
    @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
    @Action('getScenarioSelectableTreatments') getScenarioSelectableTreatmentsAction: any;
    @Action('getInvestmentPlan') getInvestmentPlanAction: any;
    @Action('getScenarioSimpleBudgetDetails') getScenarioSimpleBudgetDetailsAction:any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('getNetworks') getNetworksAction: any;
    @Action('deleteSpecificCommittedProjects') deleteSpecificCommittedProjectsAction: any;
    @Action('deleteSimulationCommittedProjects') deleteSimulationCommittedProjectsAction: any;
    @Action('upsertCommittedProjects') upsertCommittedProjectsAction: any;

    @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getCurrentUserOrSharedScenario') getCurrentUserOrSharedScenarioAction: any;
    @Action('selectScenario') selectScenarioAction: any;
    @Action('setAlertMessage') setAlertMessageAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;
   

    cpItems: SectionCommittedProjectTableData[] = [];
    selectedCpItems: SectionCommittedProjectTableData[] = [];
    sectionCommittedProjects: SectionCommittedProject[] = [];
    committedProjectsCount: number = 0;
    showImportExportCommittedProjectsDialog: boolean = false;
    selectedCommittedProject: string  = '';
    disableCrudButtonsResult: boolean = true;
    alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    reverseCatMap = clone(treatmentCategoryReverseMap);
    catMap = clone(treatmentCategoryMap);
    
    keyattr: string = '';

    investmentYears: number[] = [];
    lastYear: number = 0;
    firstYear: number = 0;

    isNoTreatmentBefore: boolean = true
    isNoTreatmentBeforeCache: boolean = true
    
    cpGridHeaders: DataTableHeader[] = [
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
        text: 'Project Source', 
        value: 'projectSource',
        align: 'left',
        sortable: false,
        class: '',
        width: '10%'
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

    mounted() {
        this.reverseCatMap.forEach(cat => {
            this.categorySelectItems.push({text: cat, value: cat})        
        })

        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            this.importCompleted,
        );
        this.fetchTreatmentLibrary(this.scenarioId);
    }   

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });

        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            this.importCompleted,
        );

        this.setAlertMessageAction('');
    }
    beforeRouteEnter(to: any, from: any, next:any) {
        next((vm:any) => {
            vm.scenarioId = to.query.scenarioId;
            vm.networkId = to.query.networkId;
            vm.librarySelectItemValue = null;
            vm.templateSelectItems = null;
            
            if (vm.scenarioId === vm.uuidNIL || vm.networkId == vm.uuidNIL) {
                vm.addErrorNotificationAction({
                   message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }
            (async () => { 
                await vm.getNetworksAction();
                await InvestmentService.getScenarioBudgetYears(vm.scenarioId).then(response => {  
                    if(response.data)
                        vm.investmentYears = response.data;
                });
                await ScenarioService.getNoTreatmentBeforeCommitted(vm.scenarioId).then(response => {
                        if(!isNil(response.data)){
                            vm.isNoTreatmentBeforeCache = response.data;
                            vm.isNoTreatmentBefore = response.data;
                        }
                });
                await vm.getScenarioSimpleBudgetDetailsAction({scenarioId: vm.scenarioId});
                await vm.getAttributesAction();
                await vm.getTreatmentLibrariesAction();
                await vm.getCurrentUserOrSharedScenarioAction({simulationId: vm.scenarioId});
                await vm.selectScenarioAction({ scenarioId: vm.scenarioId });
                await ScenarioService.getFastQueuedWorkByDomainIdAndWorkType({domainId: vm.scenarioId, workType: WorkType.ImportCommittedProject}).then(response => {
                    if(response.data){
                        vm.setAlertMessageAction("Committed project import has been added to the work queue")
                    }
                })
                await vm.initializePages()
                    if (vm.scenarioId !== undefined) {                
                            await vm.fetchTreatmentLibrary(vm.scenarioId);
                            await vm.fetchProjectSources();
                        }

                await CommittedProjectsService.getUploadedCommittedProjectTemplates().then(response => {
                    if(!isNil(response.data)){
                            vm.templateSelectItems = response.data;
                        }
           });
            })();                    
        });
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

    fetchProjectSources() {
    CommittedProjectsService.getProjectSources().then((response: AxiosResponse) => {
        if (hasValue(response, 'data')) {
            this.projectSourceOptions = response.data.filter(
                (option: string) => option !== "None" && option !== "iAMPick"
                );
        }
    });
}

    checkExistenceOfAssets(){//todo: refine this
        const uncheckKeys = this.currentPage.map(scp => scp.keyAttr).filter(key => isNil(this.isKeyAttributeValidMap.get(key)))
        if(uncheckKeys.length > 0){
            CommittedProjectsService.validateExistenceOfAssets(uncheckKeys, this.network.id).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    for(let i = 0; i < this.currentPage.length; i++)
                    {
                        const check = response.data[this.currentPage[i].keyAttr]
                        if(!isNil(check)){
                            if(!response.data[this.currentPage[i].keyAttr])
                                this.currentPage[i].errors = [this.keyattr + ' does not exist'];
                            else
                                this.currentPage[i].errors = [];

                            this.isKeyAttributeValidMap.set(this.currentPage[i].keyAttr,response.data[this.currentPage[i].keyAttr] )
                        }
                    }                  
                }
            }); 
        }
        for(let i = 0; i < this.currentPage.length; i++)
        {
            if(!this.isKeyAttributeValidMap.get(this.currentPage[i].keyAttr))
                this.currentPage[i].errors = [this.keyattr + ' does not exist'];
            else
                this.currentPage[i].errors = [];
        }
                          
    }

    checkYear(scp:SectionCommittedProjectTableData){
        if(!hasValue(scp.year))
            scp.yearErrors = ['Value cannot be empty'];
        else if (this.investmentYears.length === 0)
            scp.yearErrors = ['There are no years in the investment settings']
        else if(scp.year.toString().length < 4 || scp.year < 1900)
            scp.yearErrors = ['Invalid Year value'];      
        else
            scp.yearErrors = [];
    }

    checkYears()
    {
        this.currentPage.forEach(scp => {
            this.checkYear(scp);
        })
    }

    updateCommittedProject(row: SectionCommittedProject, value: any, property: string){
        const updatedRow = setItemPropertyValue(
                    property,
                    value,
                    row
                ) as SectionCommittedProject
        this.onUpdateRow(row.id, updatedRow);
    }

    updateCommittedProjects(row: SectionCommittedProject, value: any, property: string){
        const updatedRow = setItemPropertyValue(
                    property,
                    value,
                    row
                ) as SectionCommittedProject
        this.onUpdateRow(row.id, updatedRow);
        this.sectionCommittedProjects = update(
            findIndex(
                propEq('id', row.id),
                this.sectionCommittedProjects,
            ),
            updatedRow,
            this.sectionCommittedProjects,
        );
    }

    updateCommittedProjectTableData(row: SectionCommittedProjectTableData, value: any, property: string ){
        this.currentPage = update(
            findIndex(
                propEq('id', row.id),
                this.currentPage,
            ),
            setItemPropertyValue(
                property,
                value,
                row,
            ) as SectionCommittedProjectTableData,
            this.currentPage,
        );
    }

    onSearchClick(){
        this.currentSearch = this.gridSearchTerm;
        this.resetPage();
    }

    onClearClick(){
        this.gridSearchTerm = '';
        this.onSearchClick();
    }

    onUpdateRow(rowId: string, updatedRow: SectionCommittedProject){
        updatedRow.cost = +updatedRow.cost.toString().replace(/(\$*)(\,*)/g, '')
        if(any(propEq('id', rowId), this.addedRows)){
            const index = this.addedRows.findIndex(item => item.id == updatedRow.id)
            this.addedRows[index] = updatedRow;
            return;
        }

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
        this.projectPagination.page = 1;
        this.onPaginationChanged();
    }

    checkHasUnsavedChanges(){
        const hasUnsavedChanges: boolean = this.committedProjectsAreChanged() || this.isNoTreatmentBeforeCache != this.isNoTreatmentBefore
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    committedProjectsAreChanged() : boolean{
        return  this.deletionIds.length > 0 || 
            this.addedRows.length > 0 ||
            this.updatedRowsMap.size > 0 || (this.hasScenario && this.hasSelectedLibrary)
    }

    importCompleted(data: any){
        var importComp = data.importComp as importCompletion
        if(importComp.id === this.scenarioId && importComp.workType == WorkType.ImportCommittedProject){
            this.projectPagination.page = 1
            this.clearChanges();
            this.onPaginationChanged().then(() => {
                this.setAlertMessageAction('');
            })
        }        
    }

    async fetchTreatmentLibrary(simulationId: string) {
        try {
            const response = await TreatmentService.getTreatmentLibraryBySimulationId(simulationId);

            if (hasValue(response, 'data')) {
                const treatmentLibrary = response.data as TreatmentLibrary;
                this.$store.commit('scenarioTreatmentLibraryMutator', treatmentLibrary);
                this.handleLibrarySelectChange(treatmentLibrary.id);
            }
        } catch (error) {
            this.addErrorNotificationAction({
                message: 'Error fetching treatment library.',
                longMessage: 'There was an issue fetching the treatment library. Please try again.'
            });
        }
    }

handleLibrarySelectChange(libraryId: string) {
    this.selectTreatmentLibraryAction(libraryId);
    this.hasSelectedLibrary = true;
    const library = this.stateTreatmentLibraries.find((o) => o.id === libraryId);

    if (!isNil(library)) {
      this.selectedLibraryTreatments = library.treatments;
      this.onSelectedLibraryTreatmentsChanged();
    } 
}

    async initializePages(){
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
        await CommittedProjectsService.getCommittedProjectsPage(this.scenarioId,request).then(response => {
            this.isRunning = false
            if(response.data){
                let data = response.data as PagingPage<SectionCommittedProject>;
                this.sectionCommittedProjects = data.items;
                this.rowCache = clone(this.sectionCommittedProjects)
                this.totalItems = data.totalItems;
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