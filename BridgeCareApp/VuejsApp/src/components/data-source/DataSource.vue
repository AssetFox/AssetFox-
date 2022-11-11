<template>
    <v-layout column class="Montserrat-font-family ma-0">
            <v-layout align-center class="vl-style">
                <v-flex xs12>
                <v-layout column>
                    <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Data Source</v-subheader>
                    <v-select
                      class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family"
                      :items="dsItems"
                      append-icon=$vuetify.icons.ghd-down
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
                <div v-show="showMssql && !isNewDataSource" style="margin-top:5px;margin-bottom:12px;" class="ghd-control-label ghd-md-gray"
                > 
                    Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                </div>
            </v-layout>
        <v-layout column>
            <v-subheader v-show="showMssql || showExcel" class="ghd-control-label ghd-md-gray Montserrat-font-family">Source Type</v-subheader>
            <v-select
              v-show="showMssql || showExcel"
              class="ghd-select ghd-text-field ghd-text-field-border ds-style Montserrat-font-family"
              :items="dsTypeItems"
              append-icon=$vuetify.icons.ghd-down
              v-model="dataSourceTypeItem"
              outline
              outlined
            >
            </v-select>
        </v-layout>
        <v-layout column class="cs-style">
            <div v-show="!isNewDataSource">                
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
                append-icon=$vuetify.icons.ghd-down
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
                append-icon=$vuetify.icons.ghd-down
                v-show="showExcel"
                v-model="currentExcelDateColumn"
                class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
                outline
                outlined
                >
                </v-select>
            </div>
        </v-layout>
        <v-layout column>
            <v-subheader v-show="showMssql" class="ghd-control-label ghd-md-gray Montserrat-font-family">Connection String            
            </v-subheader>
            <v-layout justify-start>
                    <v-flex xs8>
                        <v-textarea
                          class="ghd-control-border Montserrat-font-family"
                          :placeholder=connectionStringPlaceHolderMessage
                          v-show="showMssql"
                          v-model="selectedConnection"
                          no-resize
                          outline
                        >
                        </v-textarea>
                        <p class="p-success Montserrat-font-family" v-show="sqlValid && showSqlMessage">Test Connection: {{sqlResponse}}</p>
                        <p class="p-fail Montserrat-font-family" v-show="!sqlValid && showSqlMessage">Test Connection: {{sqlResponse}}</p>
                        <p class="p-success Montserrat-font-family" v-show="showSaveMessage">Successfully saved.</p>
                        <p class="ara-blue Montserrat-font-family" v-show="isNewDataSource && showExcel">Save new data source before loading file.</p>
                        <p class="p-fail Montserrat-font-family" v-show="false">Error! {{invalidColumn}} Column is invalid</p>
                    </v-flex>
            </v-layout>
        </v-layout>
        <v-layout justify-center> 
            <v-flex xs6>
                <v-btn v-show="showMssql || showExcel" @click="resetDataSource" class="ghd-white-bg ghd-blue" flat>Cancel</v-btn>
                <v-btn v-show="showMssql" @click="checkSQLConnection" class="ghd-blue-bg ghd-white ghd-button-text">Test</v-btn>
                <v-btn v-show="showMssql || showExcel" class="ghd-blue-bg ghd-white ghd-button-text" @click="onSaveDatasource">Save</v-btn>
                <v-btn v-show="showExcel" :disabled="isNewDataSource" class="ghd-blue-bg ghd-white ghd-button-text" @click="onLoadExcel">Load</v-btn>
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
import { Action, State, Getter, Mutation } from 'vuex-class';
import {
    Datasource, 
    emptyDatasource, 
    DSEXCEL, DSSQL, 
    DataSourceExcelColumns, 
    RawDataColumns, 
    SqlDataSource, 
    ExcelDataSource,
    SqlCommandResponse
} from '@/shared/models/iAM/data-source';
import { TestStringData } from '@/shared/models/iAM/test-string';
import {hasValue} from '@/shared/utils/has-value-util';
import {
    CreateDataSourceDialogData,
    emptyCreateDataSourceDialogData
} from '@/shared/models/modals/data-source-dialog-data';
import CreateDataSourceDialog from '@/components/data-source/data-source-dialogs/CreateDataSourceDialog.vue';
import { getUserName } from '@/shared/utils/get-user-info';
import { NIL } from 'uuid';

@Component({
    components: {
        CreateDataSourceDialog
    },
})
export default class DataSource extends Vue {
    
    @State(state => state.datasourceModule.dataSources) dataSources: Datasource[];
    @State(state => state.datasourceModule.dataSourceTypes) dataSourceTypes: string[];
    @State(state => state.datasourceModule.excelColumns) excelColumns: RawDataColumns;
    @State(state => state.datasourceModule.sqlCommandResponse) sqlCommandResponse: SqlCommandResponse;

    @Action('getDataSources') getDataSourcesAction: any;
    @Action('getDataSourceTypes') getDataSourceTypesAction: any;
    @Action('upsertSqlDataSource') upsertSqlDataSourceAction: any;
    @Action('upsertExcelDataSource') upsertExcelDataSourceAction: any;

    @Action('importExcelSpreadsheetFile') importExcelSpreadsheetFileAction: any;
    @Action('getExcelSpreadsheetColumnHeaders') getExcelSpreadsheetColumnHeadersAction: any;
    @Action('checkSqlCommand') checkSqlCommandAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @Getter('getIdByUserName') getIdByUserNameGetter: any;

    dsTypeItems: string[] = [];
    dsItems: any = [];
    
    assetNumber: number = 0;
    invalidColumn: string = '';
    sqlResponse: string | null = '';
    sqlValid: boolean = false;

    sourceTypeItem: string | null = '';
    dataSourceTypeItem: string | null = '';
    datasourceNames: string[] = [];
    dataSourceExcelColumns: DataSourceExcelColumns = { locationColumn: [], dateColumn: []};

    currentExcelLocationColumn: string = '';
    currentExcelDateColumn: string = '';
    currentDatasource: Datasource = emptyDatasource;
    createDataSourceDialogData: CreateDataSourceDialogData = clone(emptyCreateDataSourceDialogData);

    selectedConnection: string = '';
    showMssql: boolean = false;
    showExcel: boolean = false;
    showSqlMessage: boolean = false;
    showSaveMessage: boolean = false;
    isNewDataSource: boolean = false;
    allowSaveData: boolean = false;
        
    fileName: string | null = '';
    fileSelect: HTMLInputElement = {} as HTMLInputElement;
    files: File[] = [];
    file: File | null = null;   

    locColumns: string[] =[];
    datColumns: string[] =[];

    connectionStringPlaceHolderMessage: string = '';    

    mounted() {

        this.fileSelect = document.getElementById('file-select') as HTMLInputElement;   

        this.getDataSourcesAction();
        this.getDataSourceTypesAction();
        this.showSqlMessage = false;
    }
    @Watch('excelColumns')
        onExcelColumnsChanged() {
            this.dataSourceExcelColumns = {
                locationColumn: this.excelColumns ? this.excelColumns.columnHeaders ? this.excelColumns.columnHeaders.length > 0 ? this.excelColumns.columnHeaders : [] : [] : [],
                dateColumn: this.excelColumns ? this.excelColumns.columnHeaders ? this.excelColumns.columnHeaders.length > 0 ? this.excelColumns.columnHeaders : [] : [] : []
            };
            if (this.dataSourceExcelColumns.locationColumn.length > 0) {
                this.locColumns = this.dataSourceExcelColumns.locationColumn;
                this.currentExcelLocationColumn = this.currentDatasource ? this.currentDatasource.locationColumn : '';
            }
            if (this.dataSourceExcelColumns.dateColumn.length > 0) {
                this.datColumns = this.dataSourceExcelColumns.dateColumn; 
                this.currentExcelDateColumn = this.currentDatasource ? this.currentDatasource.dateColumn : '';
            }
        }
    @Watch('dataSources')
        onGetDataSources() {
            if (this.dataSources != null || this.dataSources != undefined) {
                let filteredSources = this.dataSources.filter((ds: Datasource) => ds.type != 'None');  
                this.dsItems = this.dataSources.length > 0 ? filteredSources.map(
                    (ds: Datasource) => ({
                        text: ds.name,
                        value: ds.name
                    }),
                ) : []
            }
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
                this.currentExcelDateColumn = this.currentDatasource.dateColumn;
                this.currentExcelLocationColumn = this.currentDatasource.locationColumn;
            }
        }
        @Watch('sourceTypeItem') 
        onsourceTypeItemChanged() {
            // get the current data source object
            let currentDatasource = this.dataSources.length>0 ? this.dataSources.find(f => f.name === this.sourceTypeItem) : emptyDatasource;
            currentDatasource ? this.currentDatasource = currentDatasource : this.currentDatasource = emptyDatasource;

            // update the source type droplist
            this.dataSourceTypeItem = this.currentDatasource.type;
            this.currentExcelDateColumn = this.currentDatasource.dateColumn;
            this.currentExcelLocationColumn = this.currentDatasource.locationColumn;
            this.selectedConnection = this.isOwner() ? this.currentDatasource.connectionString : '';
            this.connectionStringPlaceHolderMessage = this.currentDatasource.connectionString != ''? "Replacement connection string" : 'New connection string';
            this.showSqlMessage = false; this.showSaveMessage = false;
        }
        @Watch('selectedConnection')
        onSelectedConnectionChanged() {
            if(this.selectedConnection != '')
            {
                this.currentDatasource.connectionString = this.selectedConnection;
            }
        }
        @Watch('sqlCommandResponse')
        onSqlCommandResponseChanged() {
            this.sqlCommandResponse ? this.sqlResponse = this.sqlCommandResponse.validationMessage : '';
        }
    @Watch('file')
    onFileChanged() {
        this.files = hasValue(this.file) ? [this.file as File] : [];                                   
        this.$emit('submit', this.file);
        this.file ? this.fileName = this.file.name : this.fileName = '';

        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    }
    onLoadExcel() {
        if ( hasValue(this.file)) {
            this.importExcelSpreadsheetFileAction({
            file: this.file,
            id: this.currentDatasource.id
        }).then((response: any) => {
            this.showImportMessage = true;
            this.getExcelSpreadsheetColumnHeadersAction(this.currentDatasource.id);
        });
        }
    }
    onSaveDatasource() {
        if (this.dataSourceTypeItem === DSSQL) {
            let sqldat : SqlDataSource = {
                    id: this.currentDatasource.id,
                    name: this.currentDatasource.name,
                    connectionString: this.currentDatasource.connectionString,
                    type: this.currentDatasource.type,
                    secure: this.currentDatasource.secure,
                    createdBy: this.currentDatasource.createdBy
            };
            this.upsertSqlDataSourceAction(sqldat).then(() => {
                this.showSqlMessage = false;
                this.showSaveMessage = true;
                if(this.isNewDataSource)
                {
                    this.currentDatasource.createdBy = this.getIdByUserNameGetter(getUserName());
                    this.isNewDataSource = false;
                }
                this.selectedConnection = this.isOwner() ? this.currentDatasource.connectionString : '';
                this.connectionStringPlaceHolderMessage = this.currentDatasource.connectionString!='' ? 'Replacement connection string' : 'New connection string';
                this.getDataSourcesAction();
            });
        } else {
            let exldat : ExcelDataSource = {
            id: this.currentDatasource.id,
            name: this.currentDatasource.name,
            locationColumn: this.currentExcelLocationColumn,
            dateColumn: this.currentExcelDateColumn,
            type: this.currentDatasource.type,
            secure: this.currentDatasource.secure
            }
            this.upsertExcelDataSourceAction(exldat).then(() => {
                if (!this.isNewDataSource) {
                    this.showSaveMessage = true;
                }
                this.getDataSourcesAction().then(() => {
                    this.isNewDataSource = false;
                });
            });
        }
    }
    onShowCreateDataSourceDialog() {
        this.createDataSourceDialogData = {
            showDialog: true,
        }
    }
    onCreateNewDataSource(datasource: Datasource) {
        // add to the state
        if (datasource != null || datasource != undefined) {
        this.dataSources.push(datasource);
        this.currentDatasource = datasource;
        this.sourceTypeItem = datasource.name;
        this.dataSourceTypeItem = datasource.type;
        this.selectedConnection = datasource.connectionString;        
        this.connectionStringPlaceHolderMessage = 'New connection string';
        this.datColumns = [];
        this.locColumns = [];
        this.isNewDataSource = true;
        }
    }
    allowSave(): boolean {
        let result: boolean = false;
        if (this.dataSources == undefined) return false;
        if (this.dataSourceTypeItem===DSEXCEL) {
            if (this.datColumns.length === 0 && this.locColumns.length === 0) {
                return true;
            }
        }
        return result;
    }
    resetDataSource() {
        this.currentDatasource = emptyDatasource;
        this.sourceTypeItem = '';
        this.dataSourceTypeItem = '';
        this.datColumns = [];
        this.locColumns = [];
        this.showMssql = false;
        this.showExcel = false;
        this.showSqlMessage = false;
        this.showSaveMessage = false;
        this.selectedConnection = '';
        this.connectionStringPlaceHolderMessage = 'New connection string';        
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
    checkSQLConnection() {
        if (this.currentDatasource != undefined) {
            this.showSqlMessage = false;
            this.showSaveMessage = false;
            let connStr: string = this.currentDatasource.connectionString;
            //const regex1 = new RegExp(/\\/,'g');
            //connStr = connStr.replace(regex1, "%5C");

            let testConnection: TestStringData = {testString: connStr};

            this.checkSqlCommandAction(testConnection).then(() => {
                this.sqlValid = this.sqlCommandResponse.isValid;
                this.sqlResponse = this.sqlCommandResponse.validationMessage;
                this.showSqlMessage = true;
            });
        }
    }
    getOwnerUserName(): string {
        if(this.currentDatasource.createdBy != NIL && this.currentDatasource.createdBy != undefined)
        {
            return this.getUserNameByIdGetter(this.currentDatasource.createdBy);
        }
    }
    isOwner() {
        return this.currentDatasource.createdBy == this.getIdByUserNameGetter(getUserName());
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
