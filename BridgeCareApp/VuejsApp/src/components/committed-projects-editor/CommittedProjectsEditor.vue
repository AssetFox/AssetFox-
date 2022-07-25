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
                                        <v-edit-dialog v-if="header.value !== 'actions' && header.value !== 'selection'"
                                            :return-value.sync="props.item[header.value]"
                                            @save="onEditCommittedProjectProperty(props.item,header.value,props.item[header.value])"
                                            large
                                            lazy
                                            persistent>
                                            <v-text-field v-if="header.value !== 'budget' && header.value !== 'treatment' && header.value !== 'brkey'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'budget'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item.budget"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'treatment'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item.treatment"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                            <v-text-field v-if="header.value === 'brkey'"
                                                readonly
                                                class="sm-txt"
                                                :value="props.item[header.value]"
                                                :rules="[rules['generalRules'].valueIsNotEmpty]"
                                                :error-messages="props.item.errors"/>

                                            <template slot="input">
                                                <v-text-field v-if="header.value === 'brkey'"
                                                    label="Edit"
                                                    single-line
                                                    v-model="props.item[header.value]"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                                                <v-select v-if="header.value === 'treatment'"
                                                    :items="treatmentSelectItems"
                                                    label="Select a Treatment"
                                                    v-model="props.item.treatmentId"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]">
                                                </v-select>

                                                <v-select v-if="header.value === 'budget'"
                                                    :items="budgetSelectItems"
                                                    label="Select a Budget"
                                                    v-model="props.item.scenarioBudgetId"
                                                    :rules="[rules['generalRules'].valueIsNotEmpty]">
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
                                            <v-btn @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </div>  
                                        <div v-if="header.value === 'selection'">
                                            <v-checkbox
                                                hide-details
                                                primary
                                                v-model="props.selected">
                                            </v-checkbox>
                                        </div>                            
                                    </div>
                                </td>
                            </template>
                        </v-data-table>    
                        <v-btn @click="OnAddCommittedProjectClick" v-if="selectedCpItems.length < 1"
                        class="ghd-white-bg ghd-blue ghd-button btn-style" outline>Add Committed Project</v-btn> 
                    </v-layout>
                </v-flex>

                <v-divider></v-divider>

                <v-flex xs12>
                    <v-layout justify-center>
                        <v-btn @click="onCancelClick" :disabled='!hasUnsavedChanges' class="ghd-white-bg ghd-blue ghd-button-text" flat>Cancel</v-btn>    
                        <v-btn @click="OnSaveClick" :disabled='!hasUnsavedChanges' class="ghd-blue-bg ghd-white ghd-button">Save</v-btn>    
                    </v-layout>
                </v-flex> 
            </v-layout>
        </v-flex>
        <v-flex xs8 style="border:1px solid #999999 !important;" v-if="selectedCpItems.length === 1">
            <v-layout column>
                <v-flex xs12>
                    <v-btn @click="selectedCpItems.pop()" flat class="ghd-close-button">
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

        <CreateConsequenceDialog :showDialog='showCreateCommittedProjectConsequenceDialog' @submit='onAddCommittedProjectConsequenc' />
    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { CommittedProjectConsequence, emptyCommittedProjectConsequence, emptySectionCommittedProject, GetValidTreatmentConsequenceParameters, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { Action, Getter, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import { getBlankGuid, getNewGuid } from '../../shared/utils/uuid-utils';
import { Treatment, TreatmentConsequence, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { SelectItem } from '@/shared/models/vue/select-item';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Attribute } from '@/shared/models/iAM/attribute';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { AxiosResponse } from 'axios';
import { clone, find, findIndex, isNil, propEq, update } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { http2XX } from '@/shared/utils/http-utils';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import ImportExportCommittedProjectsDialog from './committed-project-editor-dialogs/CommittedProjectsImportDialog.vue';
import CreateConsequenceDialog from './committed-project-editor-dialogs/CreateCommittedProjectConsequenceDialog.vue';
import { Budget } from '@/shared/models/iAM/investment';
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
@Component({
    components: {
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        CreateConsequenceDialog
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
    treatmentSelectItems: SelectItem[] = [];
    budgetSelectItems: SelectItem[] = [];
    scenarioId: string = getBlankGuid();
    rules: InputValidationRules = rules;
    network: Network = clone(emptyNetwork);

    @State(state => state.committedProjectsModule.sectionCommittedProjects) stateSectionCommittedProjects: SectionCommittedProject[];
    @State(state => state.treatmentModule.treatmentLibraries)stateTreatmentLibraries: TreatmentLibrary[];
    selectedLibraryTreatments: Treatment[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.investmentModule.scenarioBudgets) stateScenarioBudgets: Budget[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;

    @Action('getCommittedProjects') getCommittedProjects: any;
    @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
    @Action('getScenarioSelectableTreatments') getScenarioSelectableTreatmentsAction: any;
    @Action('getInvestment') getInvestmentAction: any;
    @Action('getAttributes') getAttributesAction: any;
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
    deleteAllClicked: boolean = false;
    committedProjectsCount: number = 0;
    showImportExportCommittedProjectsDialog: boolean = false;
    selectedCommittedProject: string  = '';
    showCreateCommittedProjectConsequenceDialog: boolean = false;
    disableCrudButtonsResult: boolean = true;
    
    brkey_: string = 'BRKEY_'

    cpGridHeaders: DataTableHeader[] = [
        {
            text: '',
            value: 'selection',
            align: '',
            sortable: false,
            class: '',
            width: '5%',
        },
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
    consequenceHeaders: DataTableHeader[] =[
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
    
    onmounted() {
    }
    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }
    beforeRouteEnter(to: any, from: any, next:any) {
        next((vm:any) => {
            vm.scenarioId = to.query.scenarioId;
            vm.librarySelectItemValue = null;
            
            if (vm.scenarioId === vm.uuidNIL) {
                vm.addErrorNotificationAction({
                   message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }

            vm.getTreatmentLibrariesAction();
            vm.getCommittedProjects(vm.scenarioId);      
            vm.getInvestmentAction(vm.scenarioId);     
            vm.getAttributesAction();
            ScenarioService.getScenarios().then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    const scenarios = response.data as Scenario[]
                    const scenario = scenarios.find(s => s.id == vm.scenarioId)
                    if(!isNil(scenario))
                        NetworkService.getNetworks().then((response: AxiosResponse) =>{
                            if(hasValue(response, 'data')){
                                const networks = response.data as Network[]
                                const network = networks.find(n => n.id == scenario.networkId)
                                if(!isNil(network))
                                    vm.network = network;
                            }
                        })
                    
                }
            })
        });
    }

    //Watch
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
            (treatment: Treatment) => ({
                text: treatment.name,
                value: treatment.id
            }),
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

    @Watch('stateScenarioBudgets')
    onStateScenarioBudgetsChanged(){
        this.budgetSelectItems = this.stateScenarioBudgets.map(
            (budget: Budget) => ({
                text: budget.name,
                value: budget.id
            }),
        );
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

    @Watch('cpItems')
    onCpItemsChanged(){
        var foo = this.cpItems;
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
        this.cpItems.push(this.cpItemFactory(newRow));
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
            this.deleteSpecificCommittedProjectsAction(this.idsForDeletion)
            this.idsForDeletion = [];
        }
        if(this.deleteAllClicked){
            this.deleteSimulationCommittedProjectsAction(this.scenarioId);
            this.deleteAllClicked = false;
        }
        
        this.upsertCommittedProjectsAction(this.sectionCommittedProjects)
        
     }

     OnDeleteAllClick(){
        this.deleteAllClicked = true;
        this.sectionCommittedProjects = [];
        this.cpItems = [];
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
                this.handleTreatmentChange(scp, scp.treatmentId, row)                  
            }
            else if(property == 'brkey'){
                this.handleBrkeyChange(row, scp, value);
            }            
            else if(property === 'budget'){
                this.handleBudgetChange(row, scp)
            }
            else{
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
            const budget = this.stateScenarioBudgets.find(b => b.id === o.scenarioBudgetId);
            const row: SectionCommittedProjectTableData = this.cpItemFactory(o);
            return row
        })
    }

    cpItemFactory(scp: SectionCommittedProject): SectionCommittedProjectTableData {
        const budget = this.stateScenarioBudgets.find(b => b.id === scp.scenarioBudgetId)
            const row: SectionCommittedProjectTableData = {
                brkey:  scp.locationKeys[this.brkey_],
                year: scp.year,
                cost: scp.cost,
                scenarioBudgetId: scp.scenarioBudgetId? scp.scenarioBudgetId : '',
                budget: budget? budget.name : '',
                treatment: scp.treatment,
                treatmentId: '',
                id: scp.id,
                errors: []              
            }
            return row
    }

    handleTreatmentChange(scp: SectionCommittedProjectTableData, treatmentId: string, row: SectionCommittedProject){
        const treatment = this.selectedLibraryTreatments.find(o => o.id == treatmentId)
        if(!isNil(treatment)){
            scp.treatment = treatment.name;
            this.updateCommittedProjects(row, treatment.name, 'treatment')  
            const parameters: GetValidTreatmentConsequenceParameters = 
            {
                consequences: treatment.consequences, 
                ValidationParameters: {
                    expression: '', 
                    currentUserCriteriaFilter: this.currentUserCriteriaFilter,
                    networkId: this.network.id
                } as ValidationParameter}
            CommittedProjectsService.GetValidConsequences(parameters, row.locationKeys[this.brkey_])
            .then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    row.consequences = [];
                    const consequences = (response.data as TreatmentConsequence[]).map(con => {
                        const consequence: CommittedProjectConsequence = {
                                id: getNewGuid(),
                                committedProjectId: row.id,
                                attribute: con.attribute,
                                changeValue: con.changeValue
                            }
                            return consequence
                        })
                        row.consequences = consequences;
                        this.updateCommittedProjects(row, consequences, 'consequences');
                        this.onSelectedCommittedProject();
                }
                CommittedProjectsService.GetTreatmetCost(row, row.locationKeys[this.brkey__])
                .then((response: AxiosResponse) => {
                    if (hasValue(response, 'data')) {
                        row.cost = response.data;
                        this.updateCommittedProjects(row, response.data, 'cost')  
                    }
                });                   
            });                                                
        }
    }
    handleBudgetChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData){
        const budget = this.stateScenarioBudgets.find(o => o.id == scp.scenarioBudgetId)
        if(!isNil(budget)){
            scp.budget = budget.name;
            this.updateCommittedProjectTableData(scp, budget.name, 'budget')
            row.scenarioBudgetId = scp.scenarioBudgetId
            this.updateCommittedProjects(row, scp.scenarioBudgetId, 'scenarioBudgetId')  
        }        
    }

    handleBrkeyChange(row: SectionCommittedProject, scp: SectionCommittedProjectTableData, brkey: string){
        row.locationKeys[this.brkey_] = brkey;
        this.updateCommittedProjects(row, brkey, 'brkey')
        CommittedProjectsService.ValidateBRKEY(this.network, brkey).then((response: AxiosResponse) => {
                if (hasValue(response, 'data')) {
                    if(!response.data)
                        scp.errors = ['BRKEY does not exist'];
                    else
                        scp.errors = [];
                }
            });
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