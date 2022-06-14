<template>
    <v-layout column class="Montserrat-font-family pa-2">
        <!-- <v-flex xs6> -->
            <v-layout align-center class="vl-style" row>
                <v-layout column>
                    <v-subheader class="ghd-control-label ghd-md-gray">Data Source</v-subheader>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border ds-style"
                      :items="DataSourceType"
                      v-model="dataSourceTypeItem"
                      outline
                      outlined
                    >
                    </v-select>
                </v-layout>
                <v-btn class="ghd-white-bg ghd-blue" outline>Add Data Source</v-btn>
            </v-layout>
        <!-- </v-flex> -->
        <v-layout column>
            <v-subheader class="ghd-control-label ghd-md-gray">Source Type</v-subheader>
            <v-select
              class="ghd-select ghd-text-field ghd-text-field-border ds-style"
              :items="SourceType"
              v-model="sourceTypeItem"
              outline
              outlined
            >
            </v-select>
        </v-layout>
        <v-layout column class="cs-style">
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray">FileName</v-subheader>
            <v-layout row>
                <v-text-field
                    v-show="showExcel"
                    class="ghd-control-text ghd-control-border"
                    outlined
                ></v-text-field>
                <v-btn v-show="showExcel" class="ghd-white-bg ghd-blue" outline>Add</v-btn>
            </v-layout>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray">Location Column</v-subheader>
            <v-select
              v-show="showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border"
            >
            </v-select>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray ds-style">Date Column</v-subheader>
            <v-select
              v-show="showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border"
            >
            </v-select>
        </v-layout>
        <v-layout column>
            <v-subheader v-show="!showExcel" class="ghd-control-label ghd-md-gray">Connection String</v-subheader>
            <v-layout justify-start>
                    <v-flex xs6>
                        <v-textarea
                          v-show="!showExcel"
                          label="Description"
                          no-resize
                          outline
                        >
                        </v-textarea>
                        <p>Message</p>
                    </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-center> 
            <v-flex xs6>
                <v-btn class="ghd-white-bg ghd-blue" flat>Cancel</v-btn>
                <v-btn class="ghd-blue-bg ghd-white ghd-button-text">Test</v-btn>
                <v-btn class="ghd-blue-bg ghd-white ghd-button-text">Save</v-btn>
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
    sourceTypeItem: string | null = '';
    dataSourceTypeItem: string | null = '';
    showMessage: boolean = false;
    showExcel: boolean = false;
    mounted() {
    }
    @Watch('librarySelectItemValue')
        onLibrarySelectItemValueChanged() {
            //set toggle for active bams or excel
    }
    @Watch('sourceTypeItem')
        onSourceTypeChanged() {
            this.showExcel = !this.showExcel;
        }
    @Watch('dataSourceTypeItem') 
        onDataSourceTypeChanged() {
            // do something...?
        }
}
</script>

<style>
.ds-style {
    width: 50%;
}
.vl-style {
    width: 800px;
}
.cs-style {
    width: 50%;
    align-content: flex-start;
}
</style>
