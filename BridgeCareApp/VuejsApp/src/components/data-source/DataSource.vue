<template>
    <v-layout column class="Montserrat-font-family ma-0">
            <v-layout align-center class="vl-style">
                <v-flex xs12>
                <v-layout column>
                    <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Data Source</v-subheader>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family"
                      :items="dsItems"
                      v-model="sourceTypeItem"
                      outline
                      outlined
                    >
                    </v-select>
                </v-layout>
                </v-flex>
                <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Add Data Source</v-btn>
            </v-layout>
            <v-divider></v-divider>
        <v-layout column>
            <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Source Type</v-subheader>
            <v-select
              class="ghd-select ghd-text-field ghd-text-field-border ds-style Montserrat-font-family"
              :items="dsTypeItems"
              v-model="dataSourceTypeItem"
              outline
              outlined
            >
            </v-select>
        </v-layout>
        <v-layout column class="cs-style">
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">FileName</v-subheader>
            <v-layout class="txt-style" row>
                <v-text-field
                    v-show="showExcel"
                    class="ghd-control-text ghd-control-border Montserrat-font-family"
                    outlined
                ></v-text-field>
                <v-btn v-show="showExcel" class="ghd-white-bg ghd-blue Montserrat-font-family" outline>Add File</v-btn>
            </v-layout>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Location Column</v-subheader>
            <v-select
              v-show="showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
            >
            </v-select>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Date Column</v-subheader>
            <v-select
              v-show="showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
            >
            </v-select>
        </v-layout>
        <v-layout column>
            <v-subheader v-show="showMssql" class="ghd-control-label ghd-md-gray Montserrat-font-family">Connection String</v-subheader>
            <v-layout justify-start>
                    <v-flex xs8>
                        <v-textarea
                          class="ghd-control-border Montserrat-font-family"
                          v-show="showMssql"
                          label="Description"
                          no-resize
                          outline
                        >
                        </v-textarea>
                        <p class="p-success Montserrat-font-family">Connection Successful - Lorem ipsum dolor sit amet</p>
                        <p class="p-fail Montserrat-font-family">Connnection Failed - Lorem ipsum dolor sit amet</p>
                        <p class="p-success Montserrat-font-family">Success! {{assetNumber}} Number of Assets Added</p>
                        <p class="p-fail Montserrat-font-family">Error! {{invalidColumn}} Column is invalid</p>
                    </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-center> 
            <v-flex xs6>
                <v-btn class="ghd-white-bg ghd-blue" flat>Cancel</v-btn>
                <v-btn v-show="showMssql" class="ghd-blue-bg ghd-white ghd-button-text">Test</v-btn>
                <v-btn v-show="showMssql" class="ghd-blue-bg ghd-white ghd-button-text">Save</v-btn>
                <v-btn v-show="showExcel" class="ghd-blue-bg ghd-white ghd-button-text">Load</v-btn>
            </v-flex>
        </v-layout>
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import {
    clone,
    contains,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import {Datasource, DataSourceType} from '@/shared/models/iAM/data-source';

@Component({

})
export default class DataSource extends Vue {
    
    @State(state => state.datasourceModule.dataSources) dataSources: Datasource[];
    @State(state => state.datasourceModule.dataSourceTypes) dataSourceTypes: string[];
    @Action('getDataSources') getDataSourcesAction: any;
    @Action('getDataSourceTypes') getDataSourceTypesAction: any;
    dsTypeItems: string[] = [];
    dsItems: any;
    assetNumber: number = 0;
    invalidColumn: string = '';
    sourceTypeItem: string | null = '';
    dataSourceTypeItem: string | null = '';
    datasourceNames: string[] = [];
    showMssql: boolean = false;
    showExcel: boolean = false;
    mounted() {
        this.getDataSourcesAction();
        this.getDataSourceTypesAction();
    }
    @Watch('dataSources')
        onGetDataSources() {
            this.dsItems = clone(this.dataSources);
            this.dsItems = this.dataSources.map(
                (ds: Datasource) => ({
                    text: ds.name,
                    value: ds.name
                }),
            );
        }
    @Watch('dataSourceTypes')
        onGetDataSourceTypes() {
            this.dsTypeItems = clone(this.dataSourceTypes);
        }
    @Watch('dataSourceTypeItem')
        onSourceTypeChanged() {
            if (this.dataSourceTypeItem==="SQL") {
                this.showMssql = true;
                this.showExcel = false;
            }
            if (this.dataSourceTypeItem==="Excel") {
                this.showExcel = true;
                this.showMssql = false;
            }
        }
}
</script>

<style>
.ds-style {
    width: 570px;
}
.vl-style {
    width: 550px;
    padding: 0px;
    
}
.cs-style {
    width: 50%;
    
}
.col-style {
    width: 570px;
}
.txt-style {
    width: 695px;
    padding: 10px;
    
    align-self: start;
}
.tx-style {
    width: 50%;
    padding: 10px;
}
.p-fail {
    color: red;
}
.p-success {
    color:green;
}
</style>
