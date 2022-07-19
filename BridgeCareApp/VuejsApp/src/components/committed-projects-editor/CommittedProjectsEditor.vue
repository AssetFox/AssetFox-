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

                <v-flex xs12>
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
                                style="margin-top:14px !important">
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
                        class=" fixed-header v-table__overflow">
                            <template slot="items" slot-scope="props">
                                <td>
                                    <v-radio-group v-model="selectedCommittedProject">
                                        <v-radio
                                            :key="props.item.id"
                                            :value="props.item.id">
                                        </v-radio>
                                    </v-radio-group>
                                </td>
                                <td>
                                    {{props.item.brkey}}
                                </td>
                                <td>
                                    {{props.item.year}}
                                </td>
                                <td>
                                    {{props.item.treatment}}
                                </td>
                                <td>
                                    {{props.item.budget}}
                                </td>
                                <td>
                                    {{props.item.cost}}
                                </td>
                                <td>
                                    <v-btn @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>    
                        <v-btn @click="OnAddCommittedProjectClick" 
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
        <v-flex xs12 style="border:solid;" v-if="isCommittedProjectSelected">
            <v-layout column>
                <v-flex xs12>
                    <v-btn @click="isCommittedProjectSelected = ''" flat class="ghd-close-button">
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
                                {{props.item.attribute}}
                            </td>
                            <td>
                                {{props.item.changeValue}}
                            </td>
                            <td>
                                <v-btn @click="OnDeleteClick(props.item.id)"  class="ghd-blue" icon>
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
import { CommittedProjectConsequence, emptyCommittedProjectConsequence, emptySectionCommittedProject, SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { Action, Getter, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import { getBlankGuid, getNewGuid } from '../../shared/utils/uuid-utils';
import { Treatment, TreatmentLibrary } from '@/shared/models/iAM/treatment';
import { SelectItem } from '@/shared/models/vue/select-item';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Attribute } from '@/shared/models/iAM/attribute';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { AxiosResponse } from 'axios';
import { clone, find, isNil, propEq } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { http2XX } from '@/shared/utils/http-utils';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import ImportExportCommittedProjectsDialog from './committed-project-editor-dialogs/CommittedProjectsImportDialog.vue';
import CreateConsequenceDialog from './committed-project-editor-dialogs/CreateCommittedProjectConsequenceDialog.vue';
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
    scenarioId: string = getBlankGuid();

    @State(state => state.committedProjectsModule.sectionCommittedProjects) stateSectionCommittedProjects: SectionCommittedProject[];
    @State(state => state.treatmentModule.selectedTreatmentLibrary) stateSelectedTreatmentLibrary: TreatmentLibrary;
    @State(state => state.treatmentModule.treatmentLibraries)stateTreatmentLibraries: TreatmentLibrary[];
    @State(state => state.treatmentModule.scenarioSelectableTreatments) stateScenarioSelectableTreatments: Treatment[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
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

    cpItems: SectionCommittedProjectTableData[] = [];
    sectionCommittedProjects: SectionCommittedProject[] = [];
    selectedConsequences: CommittedProjectConsequence[] = [];
    idsForDeletion: string[] = [];
    deleteAllClicked: boolean = false;
    committedProjectsCount: number = 0;
    showImportExportCommittedProjectsDialog: boolean = false;
    selectedCommittedProject: string  = '';
    isCommittedProjectSelected: boolean = false;
    showCreateCommittedProjectConsequenceDialog: boolean = false;
    
    brkey_: string = 'BRKEY_'

    cpGridHeaders: DataTableHeader[] = [
        {
            text: '',
            value: '',
            align: '',
            sortable: false,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'BRKEY',
            value: 'brkey',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'Year',
            value: 'year',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'Treatment',
            value: 'treatment',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'Budget',
            value: 'budget',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'Cost',
            value: 'cost',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '10%',
        },
        {
            text: 'Actions',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '10%',
        },
    ];
    consequenceHeaders: DataTableHeader[] =[
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '40%',
        },
        {
            text: 'Change',
            value: 'changeValue',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '40%',
        },
        {
            text: '',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: 'header-border',
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
            vm.getTreatmentLibrariesAction();
            if (vm.scenarioId === vm.uuidNIL) {
                vm.addErrorNotificationAction({
                   message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }
            vm.getCommittedProjects(vm.scenarioId);           
        });
    }

    //Watch
    @Watch('stateTreatmentLibraries')
    onStateTreatmentLibrariesChanged() {
        this.librarySelectItems = this.stateTreatmentLibraries.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id.toString(),
            }),
        );
    }

    @Watch('librarySelectItemValue')
    onSelectAttributeItemValueChanged() {
        this.selectTreatmentLibraryAction(this.librarySelectItemValue);
        this.hasSelectedLibrary = true;
        // this.selectDatasourceItemValue = null;
    }

    @Watch('stateSectionCommittedProjects')
    onStateSectionCommittedProjectsChanged(){
        this.sectionCommittedProjects = clone(this.stateSectionCommittedProjects);
    }

    @Watch('selectedCommittedProject')
    onSelectedCommittedProject(){
        if(!isNil(this.selectedCommittedProject)){
            const selectedProject = find(propEq('id', this.selectedCommittedProject), this.sectionCommittedProjects);
            if(!isNil(selectedProject)){
                this.selectedConsequences = selectedProject.consequences;
                this.isCommittedProjectSelected = true;
            }             
        }
    }

    @Watch('sectionCommittedProjects', {deep: true})
    onSectionCommittedProjectsChanged() {
        this.cpItems = this.sectionCommittedProjects.map(o => 
        {
            const row: SectionCommittedProjectTableData = {
                brkey:  o.locationKeys[this.brkey_],
                year: o.year,
                cost: o.cost,
                budget: o.scenarioBudgetId? o.scenarioBudgetId : '',
                treatment: o.treatment,
                id: o.id
            }
            return row
        })

        this.committedProjectsCount = this.sectionCommittedProjects.length;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', this.sectionCommittedProjects, this.stateSectionCommittedProjects);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    //Events
    onCancelClick() {
        this.sectionCommittedProjects = clone(this.stateSectionCommittedProjects);
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
        newRow.name = 'test name';
        newRow.locationKeys[this.brkey_] = 'test key';
        newRow.simulationId = this.scenarioId;
        this.sectionCommittedProjects.push(newRow);
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
     }

     OnDeleteClick(id: string){
        this.sectionCommittedProjects = this.sectionCommittedProjects.filter((scp: SectionCommittedProject) => scp.id !== id)
        if(!isNil(find(propEq('id', id), this.stateSectionCommittedProjects)))
            this.idsForDeletion.push(id);
     }

     OnDeleteConsequence(id: string){
        this.selectedConsequences = this.selectedConsequences.filter((cpc: CommittedProjectConsequence) => cpc.id !== id)
     }

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

    onAddCommittedProjectConsequenc(newConsequence: CommittedProjectConsequence) {
        this.showCreateCommittedProjectConsequenceDialog = false;

        if (!isNil(newConsequence)) {
            this.selectedConsequences.push(newConsequence);
        }
    }

    //Subroutines
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