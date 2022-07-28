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
                    <v-checkbox class='ghd-checkbox' label='No Treatments Before Committed Projects' />
                </v-flex>

                <v-flex xs12 class="ghd-constant-header">
                    <v-layout>
                        <v-flex xs6>
                            <v-layout column>
                                <v-subheader class="ghd-control-label ghd-md-gray">Treatment Library</v-subheader>
                                <v-select
                                    outline
                                    class="ghd-select ghd-text-field ghd-text-field-border pa-0"
                                    :items='librarySelectItems' 
                                    v-model='librarySelectItemValue'>
                                </v-select>                       
                            </v-layout>
                        </v-flex>
                        <v-flex xs6>
                            <v-text-field
                                append-icon="fas fa-search"
                                hide-details
                                lablel="Search"
                                placeholder="Search"
                                single-line
                                v-model="searchItems"
                                outline
                                class="ghd-text-field-border ghd-text-field"
                                style="margin-top:17px !important">
                            </v-text-field>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex xs12>
                    <v-layout justify-end class="px-4">
                        <p>Commited Projects: {{committedProjectsCount}}</p>
                    </v-layout>
                </v-flex>       
                
                <v-flex xs12 >
                    <v-layout column>
                        <v-data-table
                        :headers="cpGridHeaders"
                        :items="cpItems"
                        item-key='id'
                        :search="searchItems"
                        v-model="selectedCpItems"
                        class=" fixed-header v-table__overflow">
                            <template slot="items" slot-scope="props">
                                <td v-for="header in cpGridHeaders">
                                    <div>
                                        <v-combobox v-if="header.value === 'treatment'"
                                                    :items="treatmentSelectItems"
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
                                            persistent>
                                            <v-text-field v-if="header.value !== 'budget' && header.value !== 'year' && header.value !== 'brkey' && header.value !== 'treatment'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'budget'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"/>

                                            <v-text-field v-if="header.value === 'brkey'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.errors"/>

                                            <v-text-field v-if="header.value === 'year'"
                                                :value="props.item[header.value]"
                                                :mask="'##########'"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.yearErrors"/>

                                            <template slot="input">
                                                <v-text-field v-if="header.value === 'brkey'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                                <v-select v-if="header.value === 'budget'"
                                                    :items="budgetSelectItems"
                                                    label="Select a Budget"
                                                    v-model="props.item[header.value]">
                                                </v-select>

                                                <v-select v-if="header.value === 'category'"
                                                    :items="categorySelectItems"
                                                    label="Select a Budget"
                                                    v-model="props.item[header.value]">
                                                </v-select>

                                                <v-text-field v-if="header.value === 'year'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :mask="'##########'"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                                <v-text-field v-if="header.value === 'cost'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :mask="'##########'"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                            </template>
                                        </v-edit-dialog>
                                
                                        <div v-if="header.value === 'actions'">
                                            <v-layout style='flex-wrap:nowrap'>
                                                <v-btn @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
                                                    <v-icon>fas fa-trash</v-icon>
                                                </v-btn>
                                                <v-btn
                                                    @click="onSelectCommittedProject(props.item.id)"
                                                    class="ghd-blue"
                                                    icon>
                                                    <v-icon>fas fa-edit</v-icon>
                                                </v-btn>
                                            </v-layout>
                                        </div>                            
                                    </div>
                                </td>
                            </template>
                        </v-data-table>    
                        <v-btn @click="OnAddCommittedProjectClick" v-if="selectedCommittedProject === ''"
                        class="ghd-white-bg ghd-blue ghd-button btn-style" outline>Add Committed Project</v-btn> 
                    </v-layout>
                </v-flex>

                <v-divider></v-divider>

                <v-flex xs12>
                    <v-layout justify-center>
                        <v-btn @click="onCancelClick" :disabled='!hasUnsavedChanges' class="ghd-white-bg ghd-blue ghd-button-text" flat>Cancel</v-btn>    
                        <v-btn @click="OnSaveClick" :disabled='!hasUnsavedChanges || disableCrudButtons()' class="ghd-blue-bg ghd-white ghd-button">Save</v-btn>    
                    </v-layout>
                </v-flex> 
            </v-layout>
        </v-flex>
        <v-flex xs8 style="border:1px solid #999999 !important;" v-if="selectedCommittedProject !== ''">
            <v-layout column>
                <v-flex xs12>
                    <v-btn @click="selectedCommittedProject = ''" flat class="ghd-close-button">
                        X
                    </v-btn>
                </v-flex>
                <v-flex xs12>
                    <v-layout justify-center>
                        <v-btn @click="showCreateCommittedProjectConsequenceDialog = true" 
                            class="ghd-white-bg ghd-blue ghd-button btn-style" outline>Add Conseqence
                        </v-btn> 
                    </v-layout>
                </v-flex>
                <v-flex xs12>
                    <v-data-table
                    :headers="consequenceHeaders"
                    :items="selectedConsequences"
                    item-key='id'
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
                                <v-btn @click="OnDeleteConsequence(props.item.id)"  class="ghd-blue" icon>
                                    <v-icon>fas fa-trash</v-icon>
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
<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectConsequence, emptyCommittedProjectConsequence, emptySectionCommittedProject, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { Action, Getter, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import { getBlankGuid, getNewGuid } from '../../shared/utils/uuid-utils';
import { Treatment, TreatmentCategory, TreatmentConsequence, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { SelectItem } from '@/shared/models/vue/select-item';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Attribute } from '@/shared/models/iAM/attribute';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { AxiosResponse } from 'axios';
import { clone, find, findIndex, isEmpty, isNil, propEq, update } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { http2XX } from '@/shared/utils/http-utils';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import ImportExportCommittedProjectsDialog from './committed-project-editor-dialogs/CommittedProjectsImportDialog.vue';
import CreateConsequenceDialog from './committed-project-editor-dialogs/CreateCommittedProjectConsequenceDialog.vue';
import { Budget, InvestmentPlan } from '@/shared/models/iAM/investment';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import { NIL } from 'uuid';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { Scenario } from '@/shared/models/iAM/scenario';
import ScenarioService from '@/services/scenario.service';
import NetworkService from '@/services/network.service';
import { AsyncComponentFactory } from 'vue/types/options';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { ValidationParameter } from '@/shared/models/iAM/expression-validation';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
@Component({
    components: {
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        CreateConsequenceDialog,
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
    attributeSelectItems: SelectItem[] = [];
    treatmentSelectItems: string[] = [];
    budgetSelectItems: SelectItem[] = [];
    categorySelectItems: SelectItem[] = [];
    categories: string[] = [];
    scenarioId: string = getBlankGuid();
    networkId: string = getBlankGuid();
    rules: InputValidationRules = rules;
    network: Network = clone(emptyNetwork);

    @State(state => state.committedProjectsModule.sectionCommittedProjects) stateSectionCommittedProjects: SectionCommittedProject[];
    @State(state => state.treatmentModule.treatmentLibraries)stateTreatmentLibraries: TreatmentLibrary[];
    selectedLibraryTreatments: Treatment[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.investmentModule.investmentPlan) stateInvestmentPlan: InvestmentPlan;
    @State(state => state.investmentModule.scenarioBudgets) stateScenarioBudgets: Budget[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.networkModule.networks) networks: Network[];

    @Action('getCommittedProjects') getCommittedProjects: any;
    @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
    @Action('getScenarioSelectableTreatments') getScenarioSelectableTreatmentsAction: any;
    @Action('getInvestment') getInvestmentAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('getNetworks') getNetworksAction: any;
    @Action('deleteSpecificCommittedProjects') deleteSpecificCommittedProjectsAction: any;
    @Action('deleteSimulationCommittedProjects') deleteSimulationCommittedProjectsAction: any;
    @Action('upsertCommittedProjects') upsertCommittedProjectsAction: any;

    @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;

    cpItems: SectionCommittedProjectTableData[] = [];
    selectedCpItems: SectionCommittedProjectTableData[] = [];
    sectionCommittedProjects: SectionCommittedProject[] = [];
    selectedConsequences: CommittedProjectConsequence[] = [];
    idsForDeletion: string[] = [];
    committedProjectsCount: number = 0;
    showImportExportCommittedProjectsDialog: boolean = false;
    selectedCommittedProject: string  = '';
    showCreateCommittedProjectConsequenceDialog: boolean = false;
    disableCrudButtonsResult: boolean = true;
    alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    
    brkey_: string = 'BRKEY_'

    cpGridHeaders: DataTableHeader[] = [
        {
            text: 'BRKEY',
            value: 'brkey',
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
    consequenceHeaders: DataTableHeader[] = [
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
            text: '',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: '',
            width: '20%',
        }
    ];
    
    mounted() {
        this.categories = Object.keys(TreatmentCategory).filter((item) => {return isNaN(Number(item));});
        this.categorySelectItems = this.categories.map(
            (cat: string) => ({
                text: cat,
                value: cat
            }),
        );
    }
    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }
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
     
            vm.getNetworksAction().then(() => {
                vm.getCommittedProjects(vm.scenarioId).then(() => {
                    vm.getInvestmentAction(vm.scenarioId);  
                    vm.getTreatmentLibrariesAction();                            
                    vm.getAttributesAction();
                })
            });
                      
        });
    }

    //Watch
    @Watch('networks')
    onStateNetworksChanged(){
        const network = this.networks.find(o => o.id == this.networkId)
        if(!isNil(network)){
            this.network = network;
        }           
    }

    @Watch('stateTreatmentLibraries')
    onStateTreatmentLibrariesChanged() {
        this.librarySelectItems = this.stateTreatmentLibraries.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id
            }),
        );
    }

    @Watch('selectedLibraryTreatments', {deep: true})
    onSelectedLibraryTreatmentsChanged(){
        this.treatmentSelectItems = this.selectedLibraryTreatments.map(
            (treatment: Treatment) => (treatment.name)
        );
    }

    @Watch('stateAttributes')
    onStateAttributesChanged(){
        this.attributeSelectItems = this.stateAttributes.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name
            }),
        );
    }

    @Watch('stateInvestmentPlan')
    onStateInvestmentPlan(){
        this.cpItems.forEach(scp => {
            this.checkYear(scp);
        })
    }

    @Watch('stateScenarioBudgets')
    onStateScenarioBudgetsChanged(){
        this.budgetSelectItems = this.stateScenarioBudgets.map(
            (budget: Budget) => ({
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

    @Watch('librarySelectItemValue')
    onSelectAttributeItemValueChanged() {
        this.selectTreatmentLibraryAction(this.librarySelectItemValue);
        this.hasSelectedLibrary = true;
        const library = this.stateTreatmentLibraries.find(o => o.id == this.librarySelectItemValue)
        if(!isNil(library)){
            this.selectedLibraryTreatments = library.treatments;
            this.onSelectedLibraryTreatmentsChanged()
        }
         
    }

    @Watch('selectedCommittedProject')
    onSelectedCommittedProject(){
        if(!isNil(this.selectedCommittedProject)){
            const selectedProject = find(propEq('id', this.selectedCommittedProject), this.sectionCommittedProjects);
            if(!isNil(selectedProject)){
                this.selectedConsequences = selectedProject.consequences;
            }             
        }
    }

    @Watch('sectionCommittedProjects')
    onSectionCommittedProjectsChanged() {    
        this.committedProjectsCount = this.sectionCommittedProjects.length;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', this.sectionCommittedProjects, this.stateSectionCommittedProjects);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    @Watch('selectedCpItems')
    onSelectedCpItemsChanged(){
        if(this.selectedCpItems.length > 1)
            this.selectedCpItems.splice(0,1);
        if(this.selectedCpItems.length === 1)
            this.selectedCommittedProject = this.selectedCpItems[0].id;
    }

    //Events
    onCancelClick() {
        this.sectionCommittedProjects = clone(this.stateSectionCommittedProjects);
        this.setCpItems();
        this.selectedCommittedProject = '';
        this.selectedCpItems = [];
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

     OnGetTemplateClick(){
        CommittedProjectsService.getCommittedProjectTemplate()
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const fileInfo: FileInfo = response.data as FileInfo;  
                    FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
                }
            });
     }

     OnAddCommittedProjectClick(){
        const newRow: SectionCommittedProject = clone(emptySectionCommittedProject)
        newRow.id = getNewGuid();
        newRow.name = '';
        newRow.locationKeys[this.brkey_] = '';
        newRow.locationKeys['ID'] = getNewGuid();
        newRow.simulationId = this.scenarioId;
        this.sectionCommittedProjects.push(newRow);
        const newCpRow: SectionCommittedProjectTableData = this.cpItemFactory(newRow)
        newCpRow.errors = ['BRKEY does not exist']
        this.cpItems.push(newCpRow);
     }
     
     OnAddConsequenceClick(){
        const newRow: CommittedProjectConsequence = clone(emptyCommittedProjectConsequence)
        newRow.id = getNewGuid();
        newRow.committedProjectId = this.selectedCommittedProject;
        newRow.attribute = ''
        newRow.changeValue = ''
        this.selectedConsequences.push(newRow);
     }

     OnSaveClick(){
        if(this.idsForDeletion.length > 0){
            this.deleteSpecificCommittedProjectsAction(this.idsForDeletion).then(() => {
                this.upsertCommittedProjectsAction(this.sectionCommittedProjects)
                this.idsForDeletion = [];
            })         
        }
        else
            this.upsertCommittedProjectsAction(this.sectionCommittedProjects)     
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
        this.sectionCommittedProjects = this.sectionCommittedProjects.filter((scp: SectionCommittedProject) => scp.id !== id)
        this.cpItems = this.cpItems.filter((scp: SectionCommittedProjectTableData) => scp.id !== id)
        if(!isNil(find(propEq('id', id), this.stateSectionCommittedProjects)))
            this.idsForDeletion.push(id);
     }

      onEditCommittedProjectProperty(scp: SectionCommittedProjectTableData, property: string, value: any) {
       let row = this.sectionCommittedProjects.find(o => o.id === scp.id)
        if(!isNil(row))
        {
            if(property === 'treatment'){
                this.handleTreatmentChange(scp, value, row)                  
            }
            else if(property === 'brkey'){
                this.handleBrkeyChange(row, scp, value);
            }            
            else if(property === 'budget'){
                this.handleBudgetChange(row, scp, value)
            }
            else{
                this.checkYear(scp);
                if(property === 'category')
                    value = this.categories.findIndex((element) => element === scp.category)
                this.sectionCommittedProjects = update(
                findIndex(
                    propEq('id', scp.id),
                    this.sectionCommittedProjects,
                ),
                setItemPropertyValue(
                    property,
                    value,
                    row,
                ) as SectionCommittedProject,
                this.sectionCommittedProjects,
                );
            }    
        }       
    }

    //Consequence Funtions
    OnDeleteConsequence(id: string){
        this.selectedConsequences = this.selectedConsequences.filter((cpc: CommittedProjectConsequence) => cpc.id !== id)
        this.updateSelectedProjectConsequences()
    }

     onAddCommittedProjectConsequenc(newConsequence: CommittedProjectConsequence) {
        this.showCreateCommittedProjectConsequenceDialog = false;     
        if (!isNil(newConsequence)) {
            newConsequence.committedProjectId = this.selectedCommittedProject
            this.selectedConsequences.push(newConsequence);
            this.updateSelectedProjectConsequences();  
        }
    }

    onEditConsequenceProperty(consequence: CommittedProjectConsequence, property: string, value: any) {
        this.selectedConsequences = update(
            findIndex(propEq('id', consequence.id), this.selectedConsequences),
            setItemPropertyValue(property, value, consequence),
            this.selectedConsequences,
        );
        this.updateSelectedProjectConsequences()
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
                ).then((response: AxiosResponse) => {
                    if (
                        hasValue(response, 'status') &&
                        http2XX.test(response.status.toString())
                    ) {
                        this.addSuccessNotificationAction({
                            message: 'Successful upload.',
                            longMessage:
                                'Successfully uploaded committed projects.',
                        });
                        this.getCommittedProjects(this.scenarioId);
                    }
                });
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

    onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        this.alertDataForDeletingCommittedProjects = { ...emptyAlertData };

        if (doDelete) {
            this.deleteSimulationCommittedProjectsAction(this.scenarioId);
            this.sectionCommittedProjects = [];
            this.cpItems = [];
        }
    }

    onSelectCommittedProject(id: string){
        this.selectedCommittedProject = id;
    }

    //Subroutines

    disableCrudButtons() {
        const dataIsValid: boolean = this.sectionCommittedProjects.every(
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
                        scp.locationKeys[this.brkey_]
                    ) == true &&
                    scp.consequences.every(consequence => 
                        this.rules['generalRules'].valueIsNotEmpty(
                        consequence.attribute,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        consequence.changeValue,
                    ) === true)
                );
            },
        );

        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }

    updateSelectedProjectConsequences(){
        let row = this.sectionCommittedProjects.find(o => o.id == this.selectedCommittedProject)
        if(!isNil(row)){
            row.consequences = this.selectedConsequences
            this.sectionCommittedProjects = update(
                findIndex(
                    propEq('id', this.selectedCommittedProject),
                    this.sectionCommittedProjects,
                ),
                setItemPropertyValue(
                    'consequences',
                    this.selectedConsequences,
                    row,
                ) as SectionCommittedProject,
                this.sectionCommittedProjects,
            );
        }
    }

    setCpItems(){
        this.cpItems = this.sectionCommittedProjects.map(o => 
        {          
            const row: SectionCommittedProjectTableData = this.cpItemFactory(o);
            return row
        })
        this.checkBrkeys(0);
    }

    cpItemFactory(scp: SectionCommittedProject): SectionCommittedProjectTableData {
        const budget: Budget = find(
            propEq('id', scp.scenarioBudgetId), this.stateScenarioBudgets,
        ) as Budget;
        const row: SectionCommittedProjectTableData = {
            brkey: scp.locationKeys[this.brkey_],
            year: scp.year,
            cost: scp.cost,
            scenarioBudgetId: scp.scenarioBudgetId? scp.scenarioBudgetId : '',
            budget: budget? budget.name : '',
            treatment: scp.treatment,
            treatmentId: '',
            id: scp.id,
            errors: [],
            yearErrors: [],
            category: TreatmentCategory[scp.category]        
        }
        return row
    }

    handleTreatmentChange(scp: SectionCommittedProjectTableData, treatmentName: string, row: SectionCommittedProject){
        row.treatment = treatmentName
        this.updateCommittedProjects(row, treatmentName, 'treatment')  
        
        CommittedProjectsService.FillTreatmentValues(row, row.locationKeys[this.brkey_])
        .then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                row = response.data
                scp.cost = row.cost;
                scp.category = TreatmentCategory[row.category]
                this.onSelectedCommittedProject();
                this.updateCommittedProjects(row, row.cost, 'cost')  
                this.updateCommittedProjects(row, row.consequences, 'consequences')  
            }                            
        });                                                
    }
    handleBudgetChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, budgetName: string){
        const budget: Budget = find(
            propEq('name', budgetName), this.stateScenarioBudgets,
        ) as Budget;
        if(!isNil(budget)){
            row.scenarioBudgetId = budget.id;
            scp.budget = 'None'           
        }  
        else
            row.scenarioBudgetId = null;
        this.updateCommittedProjects(row, row.scenarioBudgetId, 'scenarioBudgetId')        
    }

    handleBrkeyChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, brkey: string){
        row.locationKeys[this.brkey_] = brkey;
        this.updateCommittedProjects(row, brkey, 'brkey');
        this.checkBrkey(scp, brkey);
    }

    checkBrkey(scp: SectionCommittedProjectTableData, brkey: string){
        CommittedProjectsService.ValidateBRKEY(this.network, brkey).then((response: AxiosResponse) => {
            if (hasValue(response, 'data')) {
                if(!response.data)
                    scp.errors = ['BRKEY does not exist'];
                else
                    scp.errors = [];
            }
        });
    }

    checkBrkeys(index: number){
        if(index < this.cpItems.length)
            CommittedProjectsService.ValidateBRKEY(this.network, this.cpItems[index].brkey).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    if(!response.data)
                        this.cpItems[index].errors = ['BRKEY does not exist'];
                    else
                        this.cpItems[index].errors = [];
                }
                index++;
                this.checkBrkeys(index)
            });
    }

    checkYear(scp:SectionCommittedProjectTableData){
        if(!hasValue(scp.year))
            scp.yearErrors = ['Value cannot be empty'];
        else if(!isNil(this.stateInvestmentPlan) &&(
            scp.year < this.stateInvestmentPlan.firstYearOfAnalysisPeriod 
            || scp.year >= this.stateInvestmentPlan.firstYearOfAnalysisPeriod + this.stateInvestmentPlan.numberOfYearsInAnalysisPeriod))
            scp.yearErrors = ['Year is outside of Analysis period'];
        else
            scp.yearErrors = [];
    }

    updateCommittedProjects(row: SectionCommittedProject, value: any, property: string){
        this.sectionCommittedProjects = update(
            findIndex(
                propEq('id', row.id),
                this.sectionCommittedProjects,
            ),
            setItemPropertyValue(
                property,
                value,
                row,
            ) as SectionCommittedProject,
            this.sectionCommittedProjects,
        );
    }

    updateCommittedProjectTableData(row: SectionCommittedProjectTableData, value: any, property: string ){
        this.cpItems = update(
            findIndex(
                propEq('id', row.id),
                this.cpItems,
            ),
            setItemPropertyValue(
                property,
                value,
                row,
            ) as SectionCommittedProjectTableData,
            this.cpItems,
        );
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

</style>