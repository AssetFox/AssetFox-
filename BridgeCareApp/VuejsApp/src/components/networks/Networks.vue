<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Network</v-subheader>
                        <v-select :items='testNetworks'
                            outline                           
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                    <v-btn style="margin-top: 20px !important; margin-left: 20px !important" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
                        Add Network
                    </v-btn>
                </v-layout>
            </v-flex>
        </v-flex>
        <v-divider />
        <v-flex xs12 class="ghd-constant-header" >
            <v-layout justify-space-between>
                <v-flex xs6>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">
                            Key Attribute
                        </v-subheader>
                        <v-select
                            outline                           
                            class="ghd-select ghd-text-field ghd-text-field-border"
                            :items='testAttributes'>
                        </v-select>  
                    </v-layout>                         
                </v-flex>
                <v-flex xs5>
                <v-layout >
                    <v-select
                        outline 
                        :items="dataSourceSelectValues"  
                        style="margin-top: 18px !important;"                  
                        class="ghd-select ghd-text-field ghd-text-field-border shifted-label"
                        label="Data Source">
                    </v-select>  
                    <v-btn style="margin-top: 20px !important;" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
                        Select All From Source
                    </v-btn>                            
                </v-layout>  
                </v-flex>       
            </v-layout>
        </v-flex>
        <!-- Data source combobox -->
        <v-flex xs12 >
            <v-layout justify-space-between>
                <v-flex xs5 >
                    <v-layout column>
                        <v-layout style="height=12px;padding-bottom:0px;">
                                <v-flex xs12 class="ghd-constant-header" style="height=12px;padding-bottom:0px">
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="padding-top: 14px !important">                             
                                        Spatial Weighting Equation</v-subheader>
                                </v-flex>
                                <v-flex xs1 style="height=12px;padding-bottom:0px;padding-top:0px;">
                                    <v-btn
                                        style="padding-right:20px !important;"
                                        class="edit-icon ghd-control-label"
                                        icon>
                                        <v-icon class="ghd-blue">fas fa-edit</v-icon>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                        <v-text-field outline class="ghd-text-field-border ghd-text-field" />                         
                    </v-layout>
                </v-flex>
                <v-flex xs5>
                    <v-layout column>
                        <div class='priorities-data-table'>
                            <v-layout justify-center>
                                <v-btn flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                                    Add All
                                </v-btn>
                                <v-divider class="investment-divider" inset vertical>
                                </v-divider>
                                <v-btn flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'>
                                    Remove All
                                </v-btn>
                            </v-layout>
                            <v-data-table :headers='budgetPriorityGridHeaders' :items='testAttributeRows'
                                class='v-table__overflow ghd-table' item-key='value' select-all
                                v-model="selectedTestNetworkRows"
                                :must-sort='true'
                                hide-actions
                                :pagination.sync="pagination">
                                <template slot='items' slot-scope='props'>
                                    <td>
                                        <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                                    </td>
                                    <td>{{ props.item.text }}</td> 
                                    <td>Excel</td> 
                                </template>
                            </v-data-table>    
                            <div class="text-xs-center pt-2">
                                <v-pagination class="ghd-pagination ghd-button-text" v-model="pagination.page" :length="pages()"></v-pagination>
                            </div>
                        </div>               
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- The Buttons  -->
        <v-flex xs12 >        
            <v-layout justify-center style="padding-top: 30px !important">
                <v-btn flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Aggregate
                </v-btn>
                <v-btn class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Save
                </v-btn>    
                <v-btn class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Create
                </v-btn>            
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script lang='ts'>
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { SelectItem } from '@/shared/models/vue/select-item';
import Vue from 'vue';
import Component from 'vue-class-component';

@Component({

})
export default class Networks extends Vue {
    budgetPriorityGridHeaders: DataTableHeader[] = [
        { text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '' },
        { text: 'Data Source', value: 'data source', align: 'left', sortable: true, class: '', width: '' },
    ];

    pagination: Pagination = emptyPagination;

    testNetworks: SelectItem[] = [
        {text:"Net 1", value: "Net 1"},
        {text:"Net 2", value: "Net 2"}
    ]

    testAttributes: SelectItem[] = [
        {text:"Att 1", value: "Att 1"},
        {text:"Att 2", value: "Att 2"}
    ]

    testAttributeRows: SelectItem[] =[
        {text:"Att 1", value: "Att 1"},
        {text:"Att 2", value: "Att 2"},
        {text:"Att 3", value: "Att 3"},
        {text:"Att 4", value: "Att 4"},
        {text:"Att 5", value: "Att 5"},
        {text:"Att 6", value: "Att 6"}
    ]

    selectedTestNetworkRows: SelectItem[] = [];

    dataSourceSelectValues: SelectItem[] = [
        {text: 'MS SQL', value: 'MS SQL'},
        {text: 'Excel', value: 'Excel'},
        {text: 'None', value: 'None'}
    ]; 

    pages() {
        this.pagination.totalItems = this.testAttributeRows.length
        if (this.pagination.rowsPerPage == null || this.pagination.totalItems == null) 
            return 0

        return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
      }

}
</script>

<style>
    .shifted-label label{
        font-family: 'Montserrat', sans-serif !important;
        font-size: 14px !important;
        padding-top: 0px !important;
        font-weight: normal;
        top: 10px !important
    }
</style>