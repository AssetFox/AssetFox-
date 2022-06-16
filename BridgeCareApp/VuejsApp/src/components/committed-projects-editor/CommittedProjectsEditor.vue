<template>
    <v-layout column>
        <v-layout row>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Get Template</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Import Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Exports Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family" outline>Delete All</v-btn>
        </v-layout>
        <v-checkbox class='Montserrat-font-family' label='No Treatments Before Committed Projects' />
        <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Treatment Library</v-subheader>
        <v-layout class="vl1-style">
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

        <v-data-table
          :headers="cpGridHeaders"
          :items="cpItems"
          :search="searchItems"
        >
        <template slot="items" slot-scope="props">
                                        <td class="Montserrat-font-family btn-style">
                                            {{props.item.brkey}}
                                        </td>
                                        <td class="Montserrat-font-family btn-style">
                                            {{props.item.year}}
                                        </td>
                                        <td class="Montserrat-font-family btn-style">
                                            {{props.item.treatment}}
                                        </td>
                                        <td class="Montserrat-font-family btn-style">
                                            {{props.item.budget}}
                                        </td>
                                        <td class="Montserrat-font-family btn-style">
                                            {{props.item.cost}}
                                        </td>
                                        <td class="Montserrat-font-family btn-style">
                                            <v-icon class="ghd-blue">fas fa-trash</v-icon>
                                        </td>
        </template>
        <template v-slot:footer>
            <td>
                <v-btn class="ghd-white-bg ghd-blue ghd-button Montserrat-font-family btn-style" outline>Add Committed Project</v-btn>    
            </td>
        </template>
        </v-data-table>    


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
import { CommittedProject } from '@/shared/models/iAM/committed-projects';
@Component({
})
export default class CommittedProjectsEditor extends Vue  {
    searchItems = '';
    dataPerPage = 0;
    totalDataFound = 0;
    cpItems: CommittedProject[] = [
        { 
            brkey: 1, 
            year: 2022,
            treatment: "treatment1", 
            budget: "test",
            cost: "$10"
        },
        {
            brkey: 2,
            year: 2022,
            treatment: "treatement2",
            budget: "test2",
            cost: "$20"
        },
    ];
    cpGridHeaders: DataTableHeader[] = [
        {
            text: 'BRKEY',
            value: 'brkey',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Year',
            value: 'year',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Treatment',
            value: 'treatment',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Budget',
            value: 'budget',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Cost',
            value: 'cost',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
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