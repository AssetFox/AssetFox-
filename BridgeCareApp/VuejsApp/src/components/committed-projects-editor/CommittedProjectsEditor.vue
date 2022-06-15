<template>
    <v-layout column>
        <v-layout row>
            <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Get Template</v-btn>
            <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Import Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Exports Projects</v-btn>
            <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Delete All</v-btn>
        </v-layout>
        <v-checkbox class='Montserrat-font-family' label='No Treatments Before Committed Projects' />
        <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Treatment Library</v-subheader>
        <v-select
          class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family sel-style"
        >
        </v-select>
        <v-text-field
          append-icon="fas fa-search"
          class="sel-style"
          lablel="Search"
          outlined
        >
        </v-text-field>
        <v-layout justify-end>
        <p>Commited Projects: {{committedProjectsCount}}</p>
        </v-layout>

        <v-data-table
          :headers="cpGridHeaders"
          :items="cpItems"
          :search="searchItems"
        >
        <template slot="items" slot-scope="props">
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
        </template>
        </v-data-table>    
        <v-btn class="ghd-white-bg ghd-blue ghd-button-text Montserrat-font-family btn-style" outline>Add Committed Project</v-btn>    
        <v-layout justify-center row>
            <v-btn class="ghd-white-bg ghd-blue ghd-button-text Montserrat-font-family" flat>Cancel</v-btn>    
            <v-btn class="ghd-white-bg ghd-blue ghd-button-text Montserrat-font-family" outline>Save</v-btn>    
        </v-layout>
    </v-layout>
    <!-- <div>
    CommittedProjects
    </div> -->
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
            sortable: false,
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
            text: 'Action',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
    ];
    committedProjectsCount: number = 0;
}
</script>
<style scoped>
.sel-style {
    width: 50%;
}
.btn-style {
    width: 500px;
}
</style>