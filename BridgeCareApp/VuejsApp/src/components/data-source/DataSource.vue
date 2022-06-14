<template>
    <v-layout column class="Montserrat-font-family ma-0">
            <v-layout align-center class="vl-style">
                <v-flex xs12>
                <v-layout column>
                    <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Data Source</v-subheader>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family"
                      :items="DataSourceType"
                      v-model="dataSourceTypeItem"
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
              :items="SourceType"
              v-model="sourceTypeItem"
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
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';

@Component({

})
export default class DataSource extends Vue {
    DataSourceType = [
        {text: "BAMS SQL", value: "BAMS SQL"},
        {text: "Excel", value: "EXCEL"}
    ];
    SourceType = [
        {text: "MS SQL", value: "MS SQL"},
        {text: "Excel", value: "Excel"}
    ];
    assetNumber: number = 0;
    invalidColumn: string = '';
    sourceTypeItem: string | null = '';
    dataSourceTypeItem: string | null = '';
    showMssql: boolean = false;
    showExcel: boolean = false;
    mounted() {
    }
    @Watch('librarySelectItemValue')
        onLibrarySelectItemValueChanged() {
            //set toggle for active bams or excel
    }
    @Watch('sourceTypeItem')
        onSourceTypeChanged() {
            if (this.sourceTypeItem==="MS SQL") {
                this.showMssql = true;
                this.showExcel = false;
            }
            if (this.sourceTypeItem==="Excel") {
                this.showExcel = true;
                this.showMssql = false;
            }
        }
    @Watch('dataSourceTypeItem') 
        onDataSourceTypeChanged() {
            // do something...?
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
