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
                <v-btn class="ghd-white-bg ghd-blue Montserrat-font-family" @click="onShowCreateDataSourceDialog" outline>Add Data Source</v-btn>
            </v-layout>
            <v-divider v-show="showMssql || showExcel"></v-divider>
        <v-layout column>
            <v-subheader v-show="showMssql || showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Source Type</v-subheader>
            <v-select
              v-show="showMssql || showExcel"
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
                    v-model="fileName"
                    outline
                    outlined
                ></v-text-field>
                <v-btn v-show="showExcel" class="ghd-white-bg ghd-blue Montserrat-font-family" @click="chooseFiles()">Add File</v-btn>
                <input @change="onSelect($event.target.files)" id="file-select" type="file" hidden />
            </v-layout>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Location Column</v-subheader>
            <v-select
              :items="locColumns"
              v-model="currentExcelLocationColumn"
              v-show="showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
              outline
              outlined
            >
            </v-select>
            <v-subheader v-show="showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Date Column</v-subheader>
            <v-select
              :items="datColumns"
              v-show="showExcel"
              v-model="currentExcelDateColumn"
              class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
              outline
              outlined
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
                          v-model="selectedConnection"
                          no-resize
                          outline
                        >
                        </v-textarea>
                        <p class="p-success Montserrat-font-family" v-show="false">Connection Successful - Lorem ipsum dolor sit amet</p>
                        <p class="p-fail Montserrat-font-family" v-show="false">Connnection Failed - Lorem ipsum dolor sit amet</p>
                        <p class="p-success Montserrat-font-family" v-show="false">Success! {{assetNumber}} Number of Assets Added</p>
                        <p class="p-fail Montserrat-font-family" v-show="false">Error! {{invalidColumn}} Column is invalid</p>
                    </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-center> 
            <v-flex xs6>
                <v-btn v-show="showMssql || showExcel" @click="resetDataSource" class="ghd-white-bg ghd-blue" flat>Cancel</v-btn>
                <v-btn v-show="showMssql" class="ghd-blue-bg ghd-white ghd-button-text">Test</v-btn>
                <v-btn v-show="showMssql" class="ghd-blue-bg ghd-white ghd-button-text" @click="onSaveDatasource">Save</v-btn>
                <v-btn v-show="showExcel" class="ghd-blue-bg ghd-white ghd-button-text" @click="onLoadExcel">Load</v-btn>
            </v-flex>
        </v-layout>

        <CreateDataSourceDialog :dialogData='createDataSourceDialogData'
                                @submit='onCreateNewDataSource' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { clone, prop } from 'ramda';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import {Datasource, emptyDatasource, DSEXCEL, DSSQL, DataSourceExcelColumns, RawDataColumns, SqlDataSource, ExcelDataSource} from '@/shared/models/iAM/data-source';
import {hasValue} from '@/shared/utils/has-value-util';
import {
    CreateDataSourceDialogData,
    emptyCreateDataSourceDialogData
} from '@/shared/models/modals/data-source-dialog-data';
import CreateDataSourceDialog from '@/components/data-source/data-source-dialogs/CreateDataSourceDialog.vue';

@Component({
    components: {
        CreateDataSourceDialog
    },
})
export default class DataSource extends Vue {
    
    @State(state => state.datasourceModule.dataSources) dataSources: Datasource[];
    @State(state => state.datasourceModule.dataSourceTypes) dataSourceTypes: string[];
    @State(state => state.datasourceModule.excelColumns) excelColumns: RawDataColumns;

    @Action('getDataSources') getDataSourcesAction: any;
    @Action('getDataSourceTypes') getDataSourceTypesAction: any;
    @Action('upsertSqlDataSource') upsertSqlDataSourceAction: any;
    @Action('upsertExcelDataSource') upsertExcelDataSourceAction: any;

    @Action('importExcelSpreadsheetFile') importExcelSpreadsheetFileAction: any;
    @Action('getExcelSpreadsheetColumnHeaders') getExcelSpreadsheetColumnHeadersAction: any;

    dsTypeItems: string[] = [];
    dsItems: any = [];
    
    assetNumber: number = 0;
    invalidColumn: string = '';

    sourceTypeItem: string | null = '';
    dataSourceTypeItem: string | null = '';
    datasourceNames: string[] = [];
    dataSourceExcelColumns: DataSourceExcelColumns = { locationColumn: '', dateColumn: ''};

    currentExcelLocationColumn: string = '';
    currentExcelDateColumn: string = '';
    currentDatasource: Datasource = emptyDatasource;
    createDataSourceDialogData: CreateDataSourceDialogData = clone(emptyCreateDataSourceDialogData);

    selectedConnection: string = 'test';
    showMssql: boolean = false;
    showExcel: boolean = false;
    showImportMessage: boolean = false;

    fileName: string | null = '';
    fileSelect: HTMLInputElement = {} as HTMLInputElement;
    files: File[] = [];
    file: File | null = null;   

    locColumns: string[] =[];
    datColumns: string[] =[];

    mounted() {

        this.fileSelect = document.getElementById('file-select') as HTMLInputElement;   

        this.getDataSourcesAction();
        this.getDataSourceTypesAction();
    }
    @Watch('excelColumns')
        onExcelColumnsChanged() {
            this.dataSourceExcelColumns = {
                locationColumn: this.excelColumns.columnHeaders ? this.excelColumns.columnHeaders[0] : '',
                dateColumn: this.excelColumns.columnHeaders? this.excelColumns.columnHeaders[this.excelColumns.columnHeaders.length-1] : ''
            };
            if (this.dataSourceExcelColumns.locationColumn != '') {
                this.locColumns.push(this.dataSourceExcelColumns.locationColumn);
                this.currentExcelLocationColumn = this.currentDatasource ? this.currentDatasource.locationColumn : '';
            }
            if (this.dataSourceExcelColumns.dateColumn !='') {
                this.datColumns.push(this.dataSourceExcelColumns.dateColumn);
                this.currentExcelDateColumn = this.currentDatasource ? this.currentDatasource.dateColumn : '';
            }
        }
    @Watch('dataSources')
        onGetDataSources() {
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
            if (this.dataSourceTypeItem===DSSQL) {
                this.showMssql = true;
                this.showExcel = false;
            }
            if (this.dataSourceTypeItem===DSEXCEL) {
                this.showExcel = true;
                this.showMssql = false;
                
                this.getExcelSpreadsheetColumnHeadersAction(this.currentDatasource.id);

            }
        }
        @Watch('sourceTypeItem') 
        onsourceTypeItemChanged() {
            // get the current data source object
            let currentDatasource = this.dataSources.find(f => f.name === this.sourceTypeItem);
            currentDatasource ? this.currentDatasource = currentDatasource : this.currentDatasource = emptyDatasource;

            // update the source type droplist
            this.dataSourceTypeItem = this.currentDatasource.type;
        }
        @Watch('selectedConnection')
        onSelectedConnectionChanged() {
            this.currentDatasource.connectionString = this.selectedConnection;
        }
    @Watch('file')
    onFileChanged() {
        this.files = hasValue(this.file) ? [this.file as File] : [];                                   
        this.$emit('submit', this.file);
        this.file ? this.fileName = this.file.name : this.fileName = '';

        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    }
    onLoadExcel() {

        let exldat : ExcelDataSource = {
            id: this.currentDatasource.id,
            name: this.currentDatasource.name,
            locationColumn: this.currentExcelLocationColumn,
            dateColumn: this.currentExcelDateColumn,
            type: this.currentDatasource.type,
            secure: this.currentDatasource.secure
        }
        this.upsertExcelDataSourceAction(exldat).then(() => {
            if ( hasValue(this.file)) {
            this.importExcelSpreadsheetFileAction({
                file: this.file,
                id: this.currentDatasource.id
            }).then((response: any) => {
                this.showImportMessage = true;
            });
            }
        });



    }
    onSaveDatasource() {
        if (this.dataSourceTypeItem ===DSSQL) {
            let sqldat : SqlDataSource = {
                    id: this.currentDatasource.id,
                    name: this.currentDatasource.name,
                    connectionString: this.currentDatasource.connectionString,
                    type: this.currentDatasource.type,
                    secure: this.currentDatasource.secure
            };
            this.upsertSqlDataSourceAction(sqldat).then(() => (this.currentDatasource = emptyDatasource));
        } else {
        }
    }
    onShowCreateDataSourceDialog() {
        this.createDataSourceDialogData = {
            showDialog: true,
        }
    }
    onCreateNewDataSource(datasource: Datasource) {
        // add to the state
        this.dataSources.push(datasource);
    }
    resetDataSource() {
        this.currentDatasource = emptyDatasource;
        this.sourceTypeItem = '';
        this.dataSourceTypeItem = '';
        this.showMssql = false;
        this.showExcel = false;
    }
    chooseFiles(){
        if(document != null)
        {
            document.getElementById('file-select')!.click();
        }
    }
    onSelect(fileList: FileList) {
        if (hasValue(fileList)) {
            const fileName: string = prop('name', fileList[0]) as string;

            if (fileName.indexOf('xlsx') === -1) {
                this.addErrorNotificationAction({
                    message: 'Only .xlsx file types are allowed',
                });
            }

            this.file = clone(fileList[0]);
        }

        this.fileSelect.value = '';
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
