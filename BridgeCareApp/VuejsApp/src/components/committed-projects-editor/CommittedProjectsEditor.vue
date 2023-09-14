<template>
    <v-layout class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout column >
                <v-flex xs12>
                    <v-layout>
                        <v-btn @click='OnGetTemplateClick' 
                            class="ghd-white-bg ghd-blue ghd-button" outline>Get Template</v-btn>
                        <v-btn @click='showImportExportCommittedProjectsDialog = true' 
                            class="ghd-white-bg ghd-blue ghd-button" outline>Import Projects</v-btn>
                        <v-btn @click='OnExportProjectsClick' 
                            class="ghd-white-bg ghd-blue ghd-button" outline>Export Projects</v-btn>
                        <v-btn @click='OnDeleteAllClick' 
                            class="ghd-white-bg ghd-blue ghd-button" outline>Delete All</v-btn>
                    </v-layout>
                </v-flex>

                <v-flex xs12>
                    <v-checkbox 
                    id="CommittedProjectsEditor-noTreatmentsBeforeCommittedProjects-ghdcheckbox"
                    class='ghd-checkbox' label='No Treatments Before Committed Projects' v-model='isNoTreatmentBefore' />
                </v-flex>

                <v-flex xs12 class="ghd-constant-header">
                    <v-layout>
                        <v-flex xs6>
                            <v-layout column>
                                <v-subheader class="ghd-control-label ghd-md-gray">Treatment Library</v-subheader>
                                <v-select
                                    id="CommittedProjectsEditor-treatmentLibrary-vSelect"
                                    outline
                                    append-icon=$vuetify.icons.ghd-down
                                    class="ghd-select ghd-text-field ghd-text-field-border pa-0"
                                    :items='librarySelectItems' 
                                    v-model='librarySelectItemValue'>
                                </v-select>                       
                            </v-layout>
                        </v-flex>
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
                                                && header.value !== 'performanceFactor'
                                                && header.value !== 'cost'"
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
                                                :rules="[rules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [firstYear, lastYear])]"
                                                :error-messages="props.item.yearErrors"/>

                                            <v-text-field v-if="header.value === 'cost'"
                                                :value='formatAsCurrency(props.item[header.value])'
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                            <v-text-field v-if="header.value === 'performanceFactor'"
                                                :value='parseFloat(props.item[header.value])'
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
                                                    :rules="[rules['committedProjectRules'].hasInvestmentYears([firstYear, lastYear]), rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [firstYear, lastYear])]"/>

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
                                                <v-btn
                                                    id="CommittedProjectsEditor-editCommittedProject-vbtn"
                                                    @click="onSelectCommittedProject(props.item.id)"
                                                    class="ghd-blue"
                                                    icon>
                                                    <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
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
                <v-flex xs12>
                    <v-layout justify-center>
                        <v-btn 
                            id="CommittedProjectsEditor-addConsequence-vbtn"
                            @click="showCreateCommittedProjectConsequenceDialog = true" 
                            class="ghd-white-bg ghd-blue ghd-button btn-style" outline>Add Conseqence
                        </v-btn> 
                    </v-layout>
                </v-flex>
                <v-flex xs12>
                    <v-data-table
                    id="CommittedProjectsEditor-consequences-vDataTable"
                    :headers="consequenceHeaders"
                    :items="selectedConsequences"
                    item-key='id'
                    sort-icon=$vuetify.icons.ghd-table-sort
                    class=" fixed-header v-table__overflow">
                        <template slot="items" slot-scope="props">
                            <td>
                                
                                <v-edit-dialog
                                :return-value.sync="props.item.attribute"
                                large
                                lazy
                                persistent
                                @save="onEditConsequenceProperty(props.item,'attribute',props.item.attribute) ">
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.attribute"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"/>
                                <template slot="input">
                                    <v-select
                                        :items="attributeSelectItems"
                                        append-icon=$vuetify.icons.ghd-down
                                        label="Select an Attribute"
                                        outline
                                        v-model="props.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]" />
                                </template>
                            </v-edit-dialog>
                            </td>
                            <td>
                                <v-edit-dialog
                                    :return-value.sync="props.item.changeValue"
                                    @save="onEditConsequenceProperty(props.item,'changeValue',props.item.changeValue) "
                                    large
                                    lazy
                                    persistent>
                                    <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.changeValue"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"/>
                                    <template slot="input">
                                        <v-text-field
                                            label="Change value"
                                            single-line
                                            v-model="props.item.changeValue"
                                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                    </template>
                                </v-edit-dialog>
                            </td>
                            <td>
                                <v-edit-dialog
                                :return-value.sync="props.item.performanceFactor"
                                large
                                lazy
                                persistent
                                @save="onEditConsequenceProperty(props.item,'performanceFactor',props.item.performanceFactor)">
                                <v-text-field
                                    readonly 
                                    single-line
                                    class="sm-text"
                                    :value='props.item.performanceFactor'
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template slot="input">
                                    <v-text-field
                                        label=""
                                        single-line
                                        maxlength="5"
                                        v-model="props.item.performanceFactor"
                                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                </template>    
                                </v-edit-dialog>
                            </td>
                            <td>
                                <v-btn 
                                    id="CommittedProjectsEditor-deleteConsequence-vbtn"
                                    @click="OnDeleteConsequence(props.item.id)"  class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </td>
                        </template>
                    </v-data-table>    
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

        <CreateConsequenceDialog :showDialog='showCreateCommittedProjectConsequenceDialog' @submit='onAddCommittedProjectConsequenc' />
    </v-layout>
</template>
<script lang="ts" setup>
import { watch, ref, inject, onBeforeUnmount } from 'vue'
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectConsequence, CommittedProjectFillTreatmentReturnValues, emptyCommittedProjectConsequence, emptySectionCommittedProject, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
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
import CreateConsequenceDialog from './committed-project-editor-dialogs/CreateCommittedProjectConsequenceDialog.vue';
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

    let store = useStore();
    const $statusHub = inject('$statusHub') as any
    created();
    let searchItems = '';
    let dataPerPage = 0;
    let totalDataFound = 0;
    let librarySelectItemValue: string | null = null;
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

    let addedRows: SectionCommittedProject[] = [];
    let updatedRowsMap:Map<string, [SectionCommittedProject, SectionCommittedProject]> = new Map<string, [SectionCommittedProject, SectionCommittedProject]>();//0: original value | 1: updated value
    let deletionIds: string[] = [];
    let rowCache: SectionCommittedProject[] = [];
    let gridSearchTerm = '';
    let currentSearch = '';
    let totalItems = 0;
    let currentPage: SectionCommittedProjectTableData[] = [];
    let isRunning: boolean = true;

    let selectedLibraryTreatments: Treatment[];
    let isKeyAttributeValidMap: Map<string, boolean> = new Map<string, boolean>();

    let projectPagination: Pagination = clone(emptyPagination);

    let stateSectionCommittedProjects = ref<SectionCommittedProject[]>(store.state.committedProjectsModule.sectionCommittedProjects);
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

    async function selectTreatmentLibraryAction(payload?: any): Promise<any> { await store.dispatch('selectTreatmentLibrary'); }
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> { await store.dispatch('setHasUnsavedChanges'); }
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification'); }
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification'); } 
    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any> { await store.dispatch('getCurrentUserOrSharedScenario'); }
    async function selectScenarioAction(payload?: any): Promise<any> { await store.dispatch('selectScenario'); }
    async function setAlertMessageAction(payload?: any): Promise<any> { await store.dispatch('setAlertMessage'); }

    let getUserNameByIdGetter: any = store.getters.getUserNameByIdGetter;

    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;
    
    let cpItems: SectionCommittedProjectTableData[] = [];
    let selectedCpItems: SectionCommittedProjectTableData[] = [];
    let sectionCommittedProjects: SectionCommittedProject[] = [];
    let selectedConsequences: CommittedProjectConsequence[] = [];
    let committedProjectsCount: number = 0;
    let showImportExportCommittedProjectsDialog: boolean = false;
    let selectedCommittedProject: string  = '';
    let showCreateCommittedProjectConsequenceDialog: boolean = false;
    let disableCrudButtonsResult: boolean = true;
    let alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    let reverseCatMap = clone(treatmentCategoryReverseMap);
    let catMap = clone(treatmentCategoryMap);
    
    let keyattr: string = '';

    let investmentYears: number[] = [];
    let lastYear: number = 0;
    let firstYear: number = 0;

    let isNoTreatmentBefore: boolean = true
    let isNoTreatmentBeforeCache: boolean = true
    
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
    const consequenceHeaders: DataTableHeader[] = [
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '40%',
        },
        {
            text: 'Change',
            value: 'changeValue',
            align: 'left',
            sortable: false,
            class: '',
            width: '40%',
        },
        {
            text: 'Factor',
            value: 'performanceFactor',
            align: 'left',
            sortable: false,
            class: '',
            width: '15%',
        },
        {
            text: '',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '20%',
        }
    ];

    function created() {
        reverseCatMap.forEach(cat => {
            categorySelectItems.push({text: cat, value: cat})        
        })

        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
    }
    // mounted() {
    //     this.reverseCatMap.forEach(cat => {
    //         this.categorySelectItems.push({text: cat, value: cat})        
    //     })

    //     this.$statusHub.$on(
    //         Hub.BroadcastEventType.BroadcastImportCompletionEvent,
    //         this.importCompleted,
    //     );
    // }   

    onBeforeUnmount(() => beforeDestroy())
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });

        $statusHub.$off(
            Hub.BroadcastEventType.BroadcastImportCompletionEvent,
            importCompleted,
        );
        setAlertMessageAction('');
    }

    // beforeDestroy() {
    //     this.setHasUnsavedChangesAction({ value: false });

    //     this.$statusHub.$off(
    //         Hub.BroadcastEventType.BroadcastImportCompletionEvent,
    //         this.importCompleted,
    //     );

    //     this.setAlertMessageAction('');
    // }
    beforeRouteEnter(to: any, from: any, next:any) {
        next((vm:any) => {
            vm.scenarioId = to.query.scenarioId;
            vm.networkId = to.query.networkId;
            vm.librarySelectItemValue = null;
            
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
            })();                    
        });
    }

    //Watch
    watch(isNoTreatmentBefore, () =>onIsNoTreatmentBeforeChanged)
    function onIsNoTreatmentBeforeChanged(){
        checkHasUnsavedChanges();
    }

    watch(investmentYears, () => onInvestmentYearsChanged)
    function onInvestmentYearsChanged(){
        //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Math/min
        if (investmentYears.length > 0) {
            lastYear = Math.max(...investmentYears);
            firstYear = Math.min(...investmentYears);
        }
    }

    watch(networks, () => onStateNetworksChanged)
    function onStateNetworksChanged(){
        const network = networks.find(o => o.id == networkId)
        if(!isNil(network)){
            network = network;
        }           
    }

    watch(stateTreatmentLibraries, () => onStateTreatmentLibrariesChanged)
    function onStateTreatmentLibrariesChanged() {
        librarySelectItems = stateTreatmentLibraries.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id
            }),
        );
    }

    //@Watch('selectedLibraryTreatments', {deep: true})
    watch(selectedLibraryTreatments, () => onSelectedLibraryTreatmentsChanged)
    function onSelectedLibraryTreatmentsChanged(){
        treatmentSelectItems = selectedLibraryTreatments.map(
            (treatment: Treatment) => (treatment.name)
        );
    }

    watch(stateAttributes, () => onStateAttributesChanged)
    function onStateAttributesChanged(){
        attributeSelectItems = stateAttributes.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }),
        );
        let keyAttr = stateAttributes.find(_ => _.id == network.keyAttribute)
        if(!isNil(keyAttr)){
            keyattr = keyAttr.name;
            cpGridHeaders[0].text = keyattr;
        }
            
    }

    watch(stateScenarioSimpleBudgetDetails, () => onStateScenarioSimpleBudgetDetailsChanged)
    function onStateScenarioSimpleBudgetDetailsChanged(){
        budgetSelectItems = stateScenarioSimpleBudgetDetails.map(
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
            sectionCommittedProjects = clone(stateSectionCommittedProjects);
            setCpItems();
    }

    watch(librarySelectItemValue, () => onSelectAttributeItemValueChanged)
    function onSelectAttributeItemValueChanged() {
        selectTreatmentLibraryAction(librarySelectItemValue);
        hasSelectedLibrary = true;
        const library = stateTreatmentLibraries.find(o => o.id == librarySelectItemValue)
        if(!isNil(library)){
            selectedLibraryTreatments = library.treatments;
            onSelectedLibraryTreatmentsChanged()
        }        
    }

    watch(selectedCommittedProject, () => onSelectedCommittedProject)
    function onSelectedCommittedProject(){
        if(!isNil(selectedCommittedProject)){
            const selectedProject = find(propEq('id', selectedCommittedProject), sectionCommittedProjects);
            if(!isNil(selectedProject)){
                selectedConsequences = selectedProject.consequences;
            }             
        }
    }

    watch(sectionCommittedProjects, () => onSectionCommittedProjectsChanged)
    function onSectionCommittedProjectsChanged() {  
        setCpItems();  
    }

    watch(selectedCpItems, () => onSelectedCpItemsChanged)
    function onSelectedCpItemsChanged(){
        if(selectedCpItems.length > 1)
            selectedCpItems.splice(0,1);
        if(selectedCpItems.length === 1)
            selectedCommittedProject = selectedCpItems[0].id;
    }

    watch(projectPagination, () => onPaginationChanged)
    async function onPaginationChanged() {
        if(isRunning)
            return;
        isRunning = true
        checkHasUnsavedChanges();
        const { sortBy, descending, page, rowsPerPage } = projectPagination;

        const request: PagingRequest<SectionCommittedProject>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                rowsForDeletion: deletionIds,
                addedRows: addedRows,
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
                    sectionCommittedProjects = data.items;
                    rowCache = clone(sectionCommittedProjects)
                    totalItems = data.totalItems;
                    const row = data.items.find(scp => scp.id == selectedCommittedProject);

                    // Updated existing data with no factor set to 1.2
                    sectionCommittedProjects.forEach(element => {
                        if (element.consequences !=null){
                            element.consequences.forEach(consequence => {
                            if (consequence.performanceFactor === 0) {
                                consequence.performanceFactor = 1.2;
                                updateCommittedProject(row ? row : emptySectionCommittedProject, "1.2", "performanceFactor");
                            }
                        });
                        }
                        
                    });
                    if(isNil(row)) {
                        selectedCommittedProject = '';
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
        selectedCommittedProject = '';
        selectedCpItems = [];
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
        CommittedProjectsService.getCommittedProjectTemplate(networkId)
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;  
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
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
        addedRows.push(newRow)
        onPaginationChanged();
     }
     
     function OnAddConsequenceClick(){
        const newRow: CommittedProjectConsequence = clone(emptyCommittedProjectConsequence)
        newRow.id = getNewGuid();
        newRow.committedProjectId = selectedCommittedProject;
        newRow.attribute = '';
        newRow.changeValue = '';
        newRow.performanceFactor = 1.2;
        selectedConsequences.push(newRow);
     }

     function OnSaveClick(){
        const upsertRequest = {
                    libraryId: null,
                    rowsForDeletion: deletionIds,
                    updateRows: Array.from(updatedRowsMap.values()).map(r => r[1]),
                    addedRows: addedRows,
                    isModified: false    
                }
        if(!committedProjectsAreChanged())
        {
            updateNoTreatment();
        }
        else if(deletionIds.length > 0){
            CommittedProjectsService.deleteSpecificCommittedProjects(deletionIds).then((response: AxiosResponse) => {
                if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                    deletionIds = [];
                    addSuccessNotificationAction({message:'Deleted committed projects'})              
                }
                CommittedProjectsService.upsertCommittedProjects(scenarioId, upsertRequest).then((response: AxiosResponse) => {
                    if(hasValue(response, 'status') && http2XX.test(response.status.toString())){
                        addSuccessNotificationAction({message:'Committed Projects Updated Successfully'}) 
                        addedRows = [];
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
                    addedRows = [];
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
        if(isNil(addedRows.find(_ => _.id === id)))
            deletionIds.push(id);
        else
            addedRows = addedRows.filter((scp: SectionCommittedProject) => scp.id !== id)

        onPaginationChanged();
     }

      function onEditCommittedProjectProperty(scp: SectionCommittedProjectTableData, property: string, value: any) {
       let row = sectionCommittedProjects.find(o => o.id === scp.id)
        if(!isNil(row))
        {
            if(property === 'treatment'){
                handleTreatmentChange(scp, value, row)             
            }
            else if(property === 'keyAttr'){
                handleKeyAttrChange(row, scp, value);               
            }
            else if(property === 'performanceFactor') {
                handleFactorChange(row, scp, value);
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

    //Consequence Funtions
    function OnDeleteConsequence(id: string){
        selectedConsequences = selectedConsequences.filter((cpc: CommittedProjectConsequence) => cpc.id !== id)
        updateSelectedProjectConsequences()
    }

     function onAddCommittedProjectConsequenc(newConsequence: CommittedProjectConsequence) {
        showCreateCommittedProjectConsequenceDialog = false;     
        if (!isNil(newConsequence)) {
            newConsequence.committedProjectId = selectedCommittedProject;
            selectedConsequences.push(newConsequence);
            updateSelectedProjectConsequences();  
        }
    }

    function onEditConsequenceProperty(consequence: CommittedProjectConsequence, property: string, value: any) {
        selectedConsequences = update(
            findIndex(propEq('id', consequence.id), selectedConsequences),
            setItemPropertyValue(property, value, consequence),
            selectedConsequences,
        );
        updateSelectedProjectConsequences()
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

    function onSelectCommittedProject(id: string){
        selectedCommittedProject = id;
    }

    //Subroutines
    function formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }

        return null;
    }
    function disableCrudButtons() {
        const rowChanges = addedRows.concat(Array.from(updatedRowsMap.values()).map(r => r[1]));
        const dataIsValid: boolean = rowChanges.every(
            (scp: SectionCommittedProject) => {
                if (isNil( scp.consequences )) scp.consequences = [];
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
                    scp.consequences.every(consequence => 
                        inputRules['generalRules'].valueIsNotEmpty(
                        consequence.attribute,
                    ) === true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        consequence.changeValue,
                    ) === true &&
                    inputRules['generalRules'].valueIsNotEmpty(
                        consequence.performanceFactor,
                    ) === true ) &&
                    inputRules['generalRules'].valueIsWithinRange(
                        scp.year, [firstYear, lastYear],
                    ) === true
                );
            },
        );
        disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    function updateSelectedProjectConsequences(){
        let row = sectionCommittedProjects.find(o => o.id == selectedCommittedProject)
        if(!isNil(row)){
            row.consequences = selectedConsequences;
            updateCommittedProjects(row, selectedConsequences, 'consequences')
        }
    }

    function setCpItems(){
        currentPage = sectionCommittedProjects.map(o => 
        {          
            const row: SectionCommittedProjectTableData = cpItemFactory(o);
            return row
        })
        checkExistenceOfAssets();
        checkYears();
    }

    function cpItemFactory(scp: SectionCommittedProject): SectionCommittedProjectTableData {
        const budget: SimpleBudgetDetail = find(
            propEq('id', scp.scenarioBudgetId), stateScenarioSimpleBudgetDetails,
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

    function handleTreatmentChange(scp: SectionCommittedProjectTableData, treatmentName: string, row: SectionCommittedProject){
        row.treatment = treatmentName;
        updateCommittedProject(row, treatmentName, 'treatment')  
        CommittedProjectsService.FillTreatmentValues({
            committedProjectId: row.id,
            treatmentLibraryId: librarySelectItemValue ? librarySelectItemValue : getBlankGuid(),
            treatmentName: treatmentName,
            KeyAttributeValue: row.locationKeys[keyattr],
            networkId: networkId
        })
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                var values = response.data as CommittedProjectFillTreatmentReturnValues
                row.cost = values.treatmentCost;
                row.consequences = values.validTreatmentConsequences;
                row.category = values.treatmentCategory;
                scp.cost = row.cost;
                let cat = reverseCatMap.get(row.category);
                if(!isNil(cat))
                    scp.category = cat;           
                updateCommittedProject(row, row.cost, 'cost')  
                updateCommittedProject(row, row.consequences, 'consequences')  
                onSelectedCommittedProject();
                onPaginationChanged();
            }                            
        });                                                
    }
    function handleBudgetChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, budgetName: string){
        const budget: SimpleBudgetDetail = find(
            propEq('name', budgetName), stateScenarioSimpleBudgetDetails,
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
        else if (investmentYears.length === 0)
            scp.yearErrors = ['There are no years in the investment settings']
        else if(scp.year < firstYear )
            scp.yearErrors = ['Year is outside of Analysis period'];      
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
        sectionCommittedProjects = update(
            findIndex(
                propEq('id', row.id),
                sectionCommittedProjects,
            ),
            updatedRow,
            sectionCommittedProjects,
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
        if(any(propEq('id', rowId), addedRows)){
            const index = addedRows.findIndex(item => item.id == updatedRow.id)
            addedRows[index] = updatedRow;
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
        addedRows = [];
        deletionIds = [];
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
        return  deletionIds.length > 0 || 
            addedRows.length > 0 ||
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
                sectionCommittedProjects = data.items;
                rowCache = clone(sectionCommittedProjects)
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