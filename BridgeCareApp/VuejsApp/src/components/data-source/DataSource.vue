<template>
    <v-row>
        <v-col cols = "12">
            <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label" style="margin-bottom:1%; margin-left:1%;" >Data Source</v-subheader>
            </v-row>
            <v-col cols = "8">
                <v-row> 
                    <v-select
                      id="DataSource-DataSourceSelect-vselect"
                      class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family"
                      :items="dsItems"
                      v-model="sourceTypeItem"
                      outline
                      variant = "outlined"
                    >
                </v-select>
                <v-btn style="margin-top: 10px !important; margin-left: 20px !important" id="DataSource-AddDataSource-vbtn" class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button Montserrat-font-family" variant = "outlined" @click="onShowCreateDataSourceDialog" outline>Add Data Source</v-btn>
                </v-row>
              </v-col>
             </v-col>
            <v-divider ></v-divider>
            <v-row>
                <div v-show="showMssql && !isNewDataSource" style="margin-top:5px;margin-bottom:12px;" class="ghd-control-label ghd-md-gray"
                > 
                    Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                </div>
            </v-row>
     <v-col cols="4">
        <v-row>
                <v-subheader class="ghd-md-gray ghd-control-label" style="margin-left:3%;" >Source Type</v-subheader>
            </v-row>
        <v-row>
            <v-select
              id="DataSource-SourceType-vselect"
              class="ghd-select ghd-text-field ghd-text-field-border ds-style Montserrat-font-family"
              :items="dsTypeItems"
              style="padding-right:20%; margin-left:3%;"
              v-model="dataSourceTypeItem"
              outline
              variant = "outlined"
            >
            </v-select>
        </v-row>
    </v-col>
        <v-row>
            <div>
                <v-row>               
                <v-subheader  class="ghd-control-label ghd-md-gray Montserrat-font-family" style="margin-left:2%;">FileName</v-subheader>
                </v-row> 
                <v-row>
                    <v-text-field
                        id="DataSource-fileName-vtextfield"
                        class="ghd-control-text ghd-control-border Montserrat-font-family"
                        v-model="fileName"
                        style="margin-left:2%;"
                        outline
                        variant = "outlined"
                    ></v-text-field>
                    <v-btn id="DataSource-AddFile-vbtn" class="ghd-blue ghd-button-text ghd-outline-button-padding ghd-button Montserrat-font-family" style="margin-left:2%; margin-top: 1%;;" variant = "outlined" @click="chooseFiles()">Add File</v-btn>
                    <input @change="onSelect($event.target.files)" id="file-select" type="file" hidden />
                </v-row>
                <v-subheader  class="ghd-control-label ghd-md-gray Montserrat-font-family">Location Column</v-subheader>
                <v-select
                id="DataSource-Location-vselect"
                :items="locColumns"
                v-model="currentExcelLocationColumn"
               
                class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
                outline
                variant = "outlined"
                >
                </v-select>
                <v-subheader class="ghd-control-label ghd-md-gray Montserrat-font-family">Date Column</v-subheader>
                <v-select
                id="DataSource-Date-vselect"
                :items="datColumns"
                
                v-model="currentExcelDateColumn"
                class="ghd-select ghd-text-field ghd-text-field-border Montserrat-font-family col-style"
                outline
                variant = "outlined"
                >
                </v-select>
            </div>
        </v-row>
        <v-row>
            <v-subheader v-show="showMssql"  class="ghd-control-label ghd-md-gray Montserrat-font-family">Connection String            
            </v-subheader>
            <v-row>
                    <v-col cols = "8">
                        <v-textarea
                          id="DataSource-ConnectionString-vtextarea"
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
                    </v-col>
            </v-row>
        </v-row>

            <v-col cols = "6" sm="4" justify-center>
                <v-row>
                <v-btn id="DataSource-Cancel-vbtn"  @click="resetDataSource" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center' flat>Cancel</v-btn>
                <v-btn id="DataSource-Test-vbtn"  @click="checkSQLConnection" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'>Test</v-btn>
                <v-btn id="DataSource-Save-vbtn"  :disabled="disableCrudButtons() || !hasUnsavedChanges" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center' @click="onSaveDatasource">Save</v-btn>
                <v-btn id="DataSource-Load-vbtn"  :disabled="isNewDataSource" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center' @click="onLoadExcel">Load</v-btn>
                <v-btn id="DataSource-Delete-vbtn"  :disabled="isNewDataSource" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center' @click="onDeleteClick">Delete</v-btn>
                </v-row>
            </v-col>

        <CreateDataSourceDialog :dialogData='createDataSourceDialogData'
                                @submit='onCreateNewDataSource' />
    </v-row>
</template>

<script setup lang='ts'>
import Vue, { onMounted, Ref, ref, ShallowRef, shallowRef, watch } from 'vue';
import { clone, isNil, prop } from 'ramda';
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
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import DataSourceService from '@/services/data-source.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { useStore } from 'vuex';

    let store = useStore();
    const emit = defineEmits(['submit'])
    let dataSources = shallowRef<Datasource[]>(store.state.datasourceModule.dataSources) ;
    let dataSourceTypes = shallowRef<string[]>(store.state.datasourceModule.dataSourceTypes) ;
    let excelColumns = shallowRef<RawDataColumns>(store.state.datasourceModule.excelColumns) ;
    let sqlCommandResponse = shallowRef<SqlCommandResponse>(store.state.datasourceModule.sqlCommandResponse) ;
    let hasUnsavedChanges = shallowRef<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges) ;

    async function getDataSourcesAction(payload?: any): Promise<any> {await store.dispatch('getDataSources');}
    async function getDataSourceTypesAction(payload?: any): Promise<any> {await store.dispatch('getDataSourceTypes');}
    async function upsertSqlDataSourceAction(payload?: any): Promise<any> {await store.dispatch('upsertSqlDataSource');}
    async function upsertExcelDataSourceAction(payload?: any): Promise<any> {await store.dispatch('upsertExcelDataSource');}
    async function deleteDataSourceAction(payload?: any): Promise<any> {await store.dispatch('deleteDataSource');}
    async function importExcelSpreadsheetFileAction(payload?: any): Promise<any> {await store.dispatch('importExcelSpreadsheetFile');}
    async function getExcelSpreadsheetColumnHeadersAction(payload?: any): Promise<any> {await store.dispatch('getExcelSpreadsheetColumnHeaders');}
    async function checkSqlCommandAction(payload?: any): Promise<any> {await store.dispatch('checkSqlCommand');}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification');} 

    let getUserNameByIdGetter: any = store.getters.getUserNameById;
    let getIdByUserNameGetter: any = store.getters.getIdByUserName ;

    let dsTypeItems: string[] = [];
    let dsItems: any = [];
    
    let assetNumber: number = 0;
    let invalidColumn: string = '';
    let sqlResponse: string | null = '';
    let sqlValid: boolean = false;

    const sourceTypeItem = ref<string>('');
    let dataSourceTypeItem: ShallowRef<string | null> = shallowRef('');
    let datasourceNames: string[] = [];
    let dataSourceExcelColumns: DataSourceExcelColumns = { locationColumn: [], dateColumn: []};

    let currentExcelLocationColumn: ShallowRef<string> = shallowRef('');
    let currentExcelDateColumn: ShallowRef<string> = shallowRef('');
    let currentDatasource: Ref<Datasource> = ref(clone(emptyDatasource));
    let unmodifiedDatasource: Ref<Datasource> = ref(clone(emptyDatasource));
    let createDataSourceDialogData: Ref<CreateDataSourceDialogData> = ref(clone(emptyCreateDataSourceDialogData));

    let selectedConnection: ShallowRef<string> = shallowRef('');
    let showMssql: boolean = false;
    let showExcel: boolean = false;
    let showSqlMessage: boolean = false;
    let showSaveMessage: boolean = false;
    let isNewDataSource: boolean = false;
    let allowSaveData: boolean = false;
        
    let fileName: string | null = '';
    let fileSelect: HTMLInputElement = {} as HTMLInputElement;
    let files: File[] = [];
    let file: ShallowRef<File | null> = shallowRef(null);   

    let locColumns: string[] =[];
    let datColumns: string[] =[];

    let connectionStringPlaceHolderMessage: string = '';    
    
    onMounted(() => mounted)
    function mounted() {

        fileSelect = document.getElementById('file-select') as HTMLInputElement;   

        getDataSourcesAction();
        getDataSourceTypesAction();
        showSqlMessage = false;
    }

    watch(excelColumns, () => onExcelColumnsChanged)
    function onExcelColumnsChanged() {
        dataSourceExcelColumns = {
            locationColumn: excelColumns ? excelColumns.value.columnHeaders ? excelColumns.value.columnHeaders.length > 0 ? excelColumns.value.columnHeaders : [] : [] : [],
            dateColumn: excelColumns ? excelColumns.value.columnHeaders ? excelColumns.value.columnHeaders.length > 0 ? excelColumns.value.columnHeaders : [] : [] : []
        };
        if (dataSourceExcelColumns.locationColumn.length > 0) {
            locColumns = dataSourceExcelColumns.locationColumn;
            currentExcelLocationColumn.value = currentDatasource ? currentDatasource.value.locationColumn : '';
        }
        if (dataSourceExcelColumns.dateColumn.length > 0) {
            datColumns = dataSourceExcelColumns.dateColumn; 
            currentExcelDateColumn.value = currentDatasource ? currentDatasource.value.dateColumn : '';
        }
    }

    watch(dataSources, () => onGetDataSources)
    function onGetDataSources() {
        if (dataSources != null || dataSources != undefined) {
            let filteredSources = dataSources.value.filter((ds: Datasource) => ds.type != 'None');  
            dsItems = dataSources.value.length > 0 ? filteredSources.map(
                (ds: Datasource) => ({
                    text: ds.name,
                    value: ds.name
                }),
            ) : []
        }
    }

    watch(dataSourceTypes, () => onGetDataSourceTypes)
    function onGetDataSourceTypes() {
        dsTypeItems = clone(dataSourceTypes.value);
    }

    watch(dataSourceTypeItem, () => onSourceTypeChanged)
    function onSourceTypeChanged() {
        if (dataSourceTypeItem.value===DSSQL) {
            showMssql = true;
            showExcel = false;
            currentDatasource.value.type = "SQL";
        }
        if (dataSourceTypeItem.value===DSEXCEL) {
            showExcel = true;
            showMssql = false;
            currentDatasource.value.type = "Excel";
            
            if(!isNewDataSource) {
                getExcelSpreadsheetColumnHeadersAction(currentDatasource.value.id);
                currentExcelDateColumn.value = currentDatasource.value.dateColumn;
                currentExcelLocationColumn.value = currentDatasource.value.locationColumn;
            }
            
        }
    }

    watch(sourceTypeItem, () => onsourceTypeItemChanged)
    function onsourceTypeItemChanged() {
        // get the current data source object
        let currentDatasource = clone(dataSources.value.length>0 ? dataSources.value.find(f => f.name === sourceTypeItem.value) : clone(emptyDatasource));
        currentDatasource ? currentDatasource = clone(currentDatasource) : currentDatasource = clone(emptyDatasource);

        if(isNil(currentDatasource.connectionString)) {
            currentDatasource.connectionString = '';
        }

        unmodifiedDatasource.value = clone(currentDatasource);

        // update the source type droplist
        dataSourceTypeItem.value = currentDatasource.type;
        currentExcelDateColumn.value = currentDatasource.dateColumn;
        currentExcelLocationColumn.value = currentDatasource.locationColumn;
        selectedConnection.value = isOwner() ? currentDatasource.connectionString : '';
        connectionStringPlaceHolderMessage = currentDatasource.connectionString != ''? "Replacement connection string" : 'New connection string';
        showSqlMessage = false; showSaveMessage = false;
        if(!isNewDataSource) {
                getExcelSpreadsheetColumnHeadersAction(currentDatasource.id);
                currentExcelDateColumn.value = currentDatasource.dateColumn;
                currentExcelLocationColumn.value = currentDatasource.locationColumn;
            }
    }

    watch(selectedConnection, () => onSelectedConnectionChanged)
    function onSelectedConnectionChanged() {
        if(selectedConnection.value != '')
        {
            currentDatasource.value.connectionString = selectedConnection.value;
        }
    }

    watch(sqlCommandResponse, () => onSqlCommandResponseChanged)
    function onSqlCommandResponseChanged() {
        sqlCommandResponse ? sqlResponse = sqlCommandResponse.value.validationMessage : '';
    }

    watch(file, () => onFileChanged)
    function onFileChanged() {
        files = hasValue(file.value) ? [file.value as File] : [];                                   
        emit('submit', file.value);
        file.value ? fileName = file.value.name : fileName = '';

        (<HTMLInputElement>document.getElementById('file-select')!).value = '';
    }

    watch(currentDatasource, () => onCurrentDataSourceChanged)
    function onCurrentDataSourceChanged() {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', currentDatasource, unmodifiedDatasource);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    watch(unmodifiedDatasource, () => onUnmodifiedDatasourceChanged)
    function onUnmodifiedDatasourceChanged(){
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', currentDatasource, unmodifiedDatasource);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    watch(currentExcelDateColumn, () => onCurrentExcelDateColumnChanged)
    function onCurrentExcelDateColumnChanged() {
        currentDatasource.value.dateColumn = currentExcelDateColumn.value;
    }

    watch(currentExcelLocationColumn, () => onCurrentExcelLocationColumnChanged)
    function onCurrentExcelLocationColumnChanged() {
        currentDatasource.value.locationColumn = currentExcelLocationColumn.value;
    }

    function onLoadExcel() {
        if ( hasValue(file.value)) {
            importExcelSpreadsheetFileAction({
            file: file.value,
            id: currentDatasource.value.id
        }).then((response: any) => {
            getExcelSpreadsheetColumnHeadersAction(currentDatasource.value.id);
        });
        }
    }
    function onSaveDatasource() {
        if (dataSourceTypeItem.value === DSSQL) {
            let sqldat : SqlDataSource = {
                    id: currentDatasource.value.id,
                    name: currentDatasource.value.name,
                    connectionString: currentDatasource.value.connectionString,
                    type: currentDatasource.value.type,
                    secure: currentDatasource.value.secure,
                    createdBy: currentDatasource.value.createdBy
            };
            upsertSqlDataSourceAction(sqldat).then(() => {
                showSqlMessage = false;
                showSaveMessage = true;
                if(isNewDataSource)
                {
                    currentDatasource.value.createdBy = getIdByUserNameGetter(getUserName());
                    isNewDataSource = false;
                }
                selectedConnection.value = isOwner() ? currentDatasource.value.connectionString : '';
                connectionStringPlaceHolderMessage = currentDatasource.value.connectionString!='' ? 'Replacement connection string' : 'New connection string';
                getDataSourcesAction();
                unmodifiedDatasource = clone(currentDatasource);
            });
        } else {
            let exldat : ExcelDataSource = {
            id: currentDatasource.value.id,
            name: currentDatasource.value.name,
            locationColumn: currentExcelLocationColumn.value,
            dateColumn: currentExcelDateColumn.value,
            type: currentDatasource.value.type,
            secure: currentDatasource.value.secure,
            createdBy: currentDatasource.value.createdBy
            }
            upsertExcelDataSourceAction(exldat).then(() => {
                if (!isNewDataSource) {
                    showSaveMessage = true;
                }
                getDataSourcesAction().then(() => {
                    isNewDataSource = false;
                });
                unmodifiedDatasource = clone(currentDatasource);
            });
        }
    }

    function onDeleteClick(){
        deleteDataSourceAction(currentDatasource.value.id).then(() => {
            resetDataSource();
        })
    }
    function onShowCreateDataSourceDialog() {
        createDataSourceDialogData.showDialog = true;
        //createDataSourceDialogData = {
            //showDialog: true,
        //}
    }
    function onCreateNewDataSource(datasource: Datasource) {
        // add to the state
        if (datasource != null || datasource != undefined) {
        dataSources.value.push(datasource);
        currentDatasource.value = datasource;
        isNewDataSource = true;
        sourceTypeItem.value = datasource.name;
        dataSourceTypeItem.value = datasource.type;
        selectedConnection.value = datasource.connectionString;        
        connectionStringPlaceHolderMessage = 'New connection string';
        datColumns = [];
        locColumns = [];       
        }
    }
    function allowSave(): boolean {
        let result: boolean = false;
        if (dataSources == undefined) return false;
        if (dataSourceTypeItem.value===DSEXCEL) {
            if (currentExcelDateColumn.value !== '' || currentExcelLocationColumn.value !== '') {
                return true;
            }
        }
        return result;
    }
    function resetDataSource() {
        currentDatasource.value = emptyDatasource;
        sourceTypeItem.value = '';
        dataSourceTypeItem.value = '';
        datColumns = [];
        locColumns = [];
        showMssql = false;
        showExcel = false;
        showSqlMessage = false;
        showSaveMessage = false;
        selectedConnection.value = '';
        connectionStringPlaceHolderMessage = 'New connection string';        
    }
    function chooseFiles(){
        if(document != null)
        {
            document.getElementById('file-select')!.click();
        }
    }
    function onSelect(fileList: FileList) {
        if (hasValue(fileList)) {
            const fileName: string = prop('name', fileList[0]) as string;

            if (fileName.indexOf('xlsx') === -1) {
                addErrorNotificationAction({
                    message: 'Only .xlsx file types are allowed',
                });
            }

            file.value = clone(fileList[0]);
        }

        fileSelect.value = '';
    }
    function checkSQLConnection() {
        if (currentDatasource != undefined) {
            showSqlMessage = false;
            showSaveMessage = false;
            let connStr: string = currentDatasource.value.connectionString;

            let testConnection: TestStringData = {testString: connStr};

            checkSqlCommandAction(testConnection).then(() => {
                sqlValid = sqlCommandResponse.value.isValid;
                sqlResponse = sqlCommandResponse.value.validationMessage;
                showSqlMessage = true;
            });
        }
    }
    function getOwnerUserName(): string {
        if(currentDatasource.value.createdBy != NIL && currentDatasource.value.createdBy != undefined)
        {
            return getUserNameByIdGetter(currentDatasource.value.createdBy);
        }
        return "Unknown";
    }
    function isOwner() {
        return currentDatasource.value.createdBy == getIdByUserNameGetter(getUserName());
    }
    function disableCrudButtons(): boolean {
        if(currentDatasource.value.type == "SQL")
        {
            return !sqlValid;
        }

        if(currentDatasource.value.type == "Excel" && !isNewDataSource)
        {
            return !allowSave();
        }

        return false;
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
