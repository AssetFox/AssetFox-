<template>
    <v-layout column>
        <v-layout >
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Get Template</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Import Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Exports Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Delete All</v-btn>
        </v-layout>
        <v-checkbox class='Montserrat-font-family ghd-checkbox' label='No Treatments Before Committed Projects' />
        <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Treatment Library</v-subheader>
        <v-layout class="vl1-style" align-center>
            <v-flex xs3 d-flex>
         <v-select
           outline
           class="ghd-select ghd-text-field ghd-text-field-border pa-0 Montserrat-font-family"
         >
         </v-select>
            </v-flex>
            <v-flex xs3>
         <v-text-field
           prepend-inner-icon="fas fa-search"
           label="Search"
           solo
           outline
         >
         </v-text-field>
            </v-flex>
        </v-layout>
        <v-layout justify-end class="px-4">
        <p>Commited Projects: {{committedProjectsCount}}</p>
        </v-layout>
        
        <v-layout>
            <v-data-table
            :headers="cpGridHeaders"
            :items="cpItems"
            :search="searchItems"
            class=" fixed-header v-table__overflow">
                <template slot="items" slot-scope="props">
                    <td>
                        <v-checkbox
                        hide-details
                        primary
                        ></v-checkbox>
                    </td>
                    <td class="Montserrat-font-family ">
                        {{props.item.brkey}}
                    </td>
                    <td class="Montserrat-font-family ">
                        {{props.item.year}}
                    </td>
                    <td class="Montserrat-font-family ">
                        {{props.item.treatment}}
                    </td>
                    <td class="Montserrat-font-family ">
                        {{props.item.budget}}
                    </td>
                    <td class="Montserrat-font-family ">
                        {{props.item.cost}}
                    </td>
                    <td class="Montserrat-font-family ">
                        <v-icon class="ghd-blue">fas fa-trash</v-icon>
                    </td>
                </template>
            </v-data-table>    
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family btn-style" outline>Add Committed Project</v-btn> 
        </v-layout>

        <v-layout justify-start align-center>
            <v-text class="ghd-control-text Montserrat-font-family" v-if="totalDataFound > 0">Showing {{ dataPerPage }} of {{ totalDataFound }} results</v-text>
            <v-text class="ghd-control-text Montserrat-font-family" v-else>No results found!</v-text>
            <v-divider vertical class="mx-3"/>
            <v-btn flat right
                class="ghd-control-label ghd-blue"
            > Delete Selected 
            </v-btn>
        </v-layout>
<v-divider></v-divider>
        <v-layout justify-center row>
            <v-btn class="ghd-white-bg ghd-blue ghd-button-text Montserrat-font-family" flat>Cancel</v-btn>    
            <v-btn class="ghd-blue-bg ghd-white ghd-button Montserrat-font-family">Save</v-btn>    
        </v-layout>
    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue'
import Component from 'vue-class-component';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { SectionCommittedProject, SectionCommittedProjectTableData } from '@/shared/models/iAM/committed-projects';
import { Action, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import { getBlankGuid } from '../../shared/utils/uuid-utils';
@Component({
})
export default class CommittedProjectsEditor extends Vue  {
    searchItems = '';
    dataPerPage = 0;
    totalDataFound = 0;

    @State(state => state.committedProjectsModule.sectionCommittedProjects) sectionCommittedProjects: SectionCommittedProject[];
    @Action('getCommittedProjects') getCommittedProjects: any;

    scenarioId: string = getBlankGuid();
    cpItems: SectionCommittedProjectTableData[] = [];

    onmounted() {
    }
    beforeRouteEnter(to: any, from: any, next:any) {
        next((vm:any) => {
            vm.selectedScenarioId = to.query.scenarioId;
            if (vm.selectedScenarioId === vm.uuidNIL) {
                vm.addErrorNotificationAction({
                   message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }

            // TODO: We need a simulation ID here not scenario ID, where does this come from?

            // const simulationId = '17143BA8-6275-4C43-A261-1EC0306A46A8';
            // vm.getCommittedProjects(simulationId);
            vm.getCommittedProjects(vm.selectedScenarioId);
            
        });
    }

    @Watch('sectionCommittedProjects')
    onSectionCommittedProjectsChanged() {

        // TODO:
        // push the data into table format
        // budget, treatment, brkey need to be resolved.
        this.sectionCommittedProjects.forEach((value, index) => {
            this.cpItems.push({
                brkey: value.locationKeys.values.toString(),
                year: value.year,
                cost: value.cost,
                budget: value.scenarioBudgetId? value.scenarioBudgetId : '',
                treatment: value.treatment
            });
        })
    }
    // Sample Data
    // cpItems: CommittedProject[] = [
    //     { 
    //         brkey: 1, 
    //         year: 2022,
    //         treatment: "treatment1", 
    //         budget: "test",
    //         cost: "$10"
    //     },
    //     {
    //         brkey: 2,
    //         year: 2022,
    //         treatment: "treatement2",
    //         budget: "test2",
    //         cost: "$20"
    //     },
    // ];
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
    committedProjectsCount: number = 0;
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