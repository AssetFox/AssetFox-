
<template>
    <v-row column class="Montserrat-font-family">
        <v-col cols="12">
            <v-col cols="6" class="ghd-constant-header">
                <v-row style="margin-bottom: 20px;">
                    <v-row column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Primary Network</v-subheader>
                        <v-select :items="selectPrimaryNetworkItems"  id="AdminData-PrimaryNetwork-select"                       
                            variant="outlined"
                            item-title="text"  
                            item-value="value"
                            v-model="selectPrimaryNetworkItemValue"
                            style="padding-left: 17px;"                         
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-row>
                </v-row>
                <v-row style="margin-bottom: 20px;">
                    <v-row column>
                        <v-subheader  class="ghd-md-gray ghd-control-label">Raw Data Network</v-subheader>
                        <v-select :items="selectRawDataNetworkItems"
                            variant="outlined"
                            item-title="text"
                            style="margin-left: 5px;"
                            item-value="value"
                            v-model="selectRawdataNetworkItemValue"
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>
                    </v-row>
                </v-row>
            </v-col>
            <v-col cols="8" class="ghd-constant-header">
                <v-row style="margin-bottom: 5px;">
                    <v-col cols="2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Key Field(s): </v-subheader> 
                    </v-col>
                    <v-col cols="5" style="margin-top:5px">
                        <div id="AdminData-keyFields-div" class="ghd-md-gray ghd-control-label elipsisList">{{keyFieldsDelimited}}</div>  
                    </v-col>                        
                    <v-btn style="margin-left: 20px;margin-top:10px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditKeyFieldsClick">
                        Edit
                    </v-btn>
                </v-row>
            </v-col>
            <v-col cols = "8" class="ghd-constant-header">
                <v-row style="margin-bottom: 5px;">
                    <v-col cols = "2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Raw Data Key Field(s): </v-subheader> 
                    </v-col>
                    <v-col cols="5" style="margin-top: 5px;">
                        <div class="ghd-md-gray ghd-control-label elipsisList">{{rawDataKeyFieldsDelimited}}</div>  
                    </v-col>                        
                    <v-btn style="margin-left: 20px; margin-top:13px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditRawDataKeyFieldsClick">
                        Edit
                    </v-btn>
                </v-row>
            </v-col>
            <v-col cols = "8" class="ghd-constant-header">
                <v-row style="margin-bottom: 5px;">
                    <v-col cols = "2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Inventory Report(s): </v-subheader> 
                    </v-col>
                    <v-col cols="5">
                         <div class="ghd-md-gray ghd-control-label elipsisList">{{inventoryReportsDelimited}}</div> 
                    </v-col>                 
                    <v-btn style="margin-left: 20px;margin-top:13px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditInventoryReportsClick">
                        Edit
                    </v-btn>
                </v-row>
            </v-col>
            <v-col cols = "8" class="ghd-constant-header">
                <v-row style="margin-bottom: 5px;">
                    <v-col cols = "2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Simulation Report(s): </v-subheader> 
                    </v-col>
                    <v-col cols="5">
                        <div class="ghd-md-gray ghd-control-label elipsisList">{{simulationReportsDelimited}}</div> 
                    </v-col>                     
                    <v-btn style="margin-left: 20px;margin-top:13px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditSimulationReportsClick">
                        Edit
                    </v-btn>
                </v-row>
            </v-col>            
            <v-col cols = "8" class="ghd-constant-header">
                <v-row>
                    <v-col cols = "2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Constraint Type: </v-subheader> 
                    </v-col>
                    <v-col cols="4">
                        <input type ="radio" v-model="constraintTypeRadioGroup" value ="OR"/>
                        <label style="margin-right: 10px;">OR</label>
                        <input type ="radio" v-model="constraintTypeRadioGroup" value ="AND"/>
                        <label>AND</label>
                    </v-col>                     
                </v-row>
            </v-col>     
        </v-col>  
        <v-col cols = "12">
            <v-row justify-center>
                <v-col cols="7">
                    <v-btn style="margin-top: -30px !important;" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onSaveClick" :disabled='!hasUnsavedChanges || !disableCrudButtons()' >
                        Save
                    </v-btn>
                </v-col>
            </v-row>
        </v-col>
        <EditAdminDataDialog :DialogData='editAdminDataDialogData'
                                     @submit='onSubmitEditAdminDataDialogResult' />
    </v-row>
</template>

<script lang='ts' setup>

import { SelectItem } from '@/shared/models/vue/select-item';
import Vue, { computed, shallowRef, } from 'vue';

import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import EditAdminDataDialog from './EditAdminDataDialog.vue';
import {EditAdminDataDialogData, emptyEditAdminDataDialogData} from '@/shared/models/modals/edit-data-dialog-data';
import { clone } from 'ramda';
import Dropdown from 'primevue/dropdown';
import { Network } from '@/shared/models/iAM/network';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { Console } from 'console';

    let store = useStore();
    let stateAvailableReportNames = computed<string[]>(()=>store.state.adminDataModule.availableReportNames);
    let stateSimulationReportNames = computed<string[]>(()=>store.state.adminDataModule.simulationReportNames);
    let stateInventoryReportNames = computed<string[]>(()=>store.state.adminDataModule.inventoryReportNames);
    let statePrimaryNetwork = computed<string>(()=>store.state.adminDataModule.primaryNetwork);
    let stateKeyFields = computed<string[]>(()=>store.state.adminDataModule.keyFields);
    let stateRawDataKeyFields = computed<string[]>(()=>store.state.adminDataModule.rawDataKeyFields);
    let stateRawdataNetwork = computed<string>(()=>store.state.adminDataModule.rawdataNetwork);
    let stateConstraintType = computed<string>(()=>store.state.adminDataModule.constraintType);
    let hasUnsavedChanges = computed<boolean>(()=>store.state.unsavedChangesFlagModule.hasUnsavedChanges) ;
    let stateNetworks = computed<Network[]>(()=>store.state.networkModule.networks);
    let stateAttributes = computed<Attribute[]>(()=>store.state.attributeModule.attributes);
        async function setIsBusyAction(payload?: any): Promise<any> { await store.dispatch('setIsBusy', payload);}
    async function getAvailableReportsAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports',payload);}
    async function getSimulationReportsAction(payload?: any): Promise<any> {await store.dispatch('getSimulationReports',payload);}
    async function getInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('getInventoryReports',payload);}
    async function getPrimaryNetworkAction(payload?: any): Promise<any> {await store.dispatch('getPrimaryNetwork',payload);}
    async function getRawdataNetworkAction(payload?: any): Promise<any> {await store.dispatch('getRawdataNetwork',payload);}
    async function getKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields',payload);}
    async function getRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getRawDataKeyFields',payload);}
    async function getConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('getConstraintType',payload);}
    async function setSimulationReportsAction(payload?: any): Promise<any> {await store.dispatch('setSimulationReports',payload);}
    async function setInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('setInventoryReports',payload);}
    async function setPrimaryNetworkAction(payload?: any): Promise<any> {await store.dispatch('setPrimaryNetwork',payload);}
    async function setRawdataNetworkAction(payload?: any): Promise<any> {await store.dispatch('setRawdataNetwork',payload);}
    async function setKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('setKeyFields',payload);}
    async function setRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('setRawDataKeyFields',payload);}
    async function setConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('setConstraintType',payload);}
    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks',payload);}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes',payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges',payload);}

    const selectPrimaryNetworkItems= ref<SelectItem[]>([]);
    const selectRawDataNetworkItems=  ref<SelectItem[]> ([]);
    const selectPrimaryNetworkItemValue =  ref<string>('');
    const selectRawdataNetworkItemValue = ref<string>('');

   
    const editAdminDataDialogData= reactive<EditAdminDataDialogData>(emptyEditAdminDataDialogData);
 

    let keyFields = reactive<string[]> ([]);
    const keyFieldsForDialog = ref<string[]> ([]);
    let simulationReports: string[] = [];
    let inventoryReports: string[] = [];
    let networks = reactive<Network[]>([]);

    const selectedKeyFields= ref<string[]> ([]);
    const selectedRawDataKeyFields= ref<string[]>([]);
    let selectedAvailableReports:string[]=[];
    const selectedSimulationReports =ref<string[]>([]);
    const selectedInventoryReports= ref<string[]>([]);
    const primaryNetwork = ref<string>('');
    const rawdataNetwork= ref<string> ('');

    const keyFieldsDelimited= ref<string>('');
    const rawDataKeyFieldsDelimited =ref<string>('');
    const simulationReportsDelimited= ref<string>('');
    const inventoryReportsDelimited=  ref<string>('');

    let keyFieldsName: string  = 'KeyFields';
    let rawDataKeyFieldsName: string = 'RawDataKeyFields';
    let simulationReportsName: string = 'SimulationReports';
    let inventoryReportsName: string = 'InventoryReports';

    let InputRules: InputValidationRules = rules;

    let constraintTypeRadioGroup = shallowRef<string>('');
    created();
    function created() {
                
                 getPrimaryNetworkAction();
                 getRawdataNetworkAction();
                 getKeyFieldsAction();
                 getRawDataKeyFieldsAction();
                 getConstraintTypeAction();
                onStateConstraintTypeChanged();
                getAttributes();
                getNetworks();
                getAvailableReportsAction();
                 getSimulationReportsAction();
                getInventoryReportsAction();
                
        (() => {
            
            (async () => { 
                
                 
                 
            })();
            
        });
    }
    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    //watchers
    watch(stateNetworks, () => {
        networks = clone(stateNetworks.value);
        stateNetworks.value.forEach(_ => {
            selectPrimaryNetworkItems.value.push({text:_.name,value:_.name})
            selectRawDataNetworkItems.value.push({text:_.name,value:_.name})
        });
    })
    watch(stateAttributes,() => {
        
        keyFields = clone(stateAttributes.value).map(_ =>_.name) //.map(_ => _.name)
        stateAttributes.value.forEach(function (value) {
            keyFieldsForDialog.value.push(value.name);
        });
    })
    watch(stateSimulationReportNames,() => {
        selectedSimulationReports.value = clone(stateSimulationReportNames.value);
    })
    watch(stateAvailableReportNames,() => 
    {
        selectedAvailableReports = clone(stateAvailableReportNames.value);

        const reports = ref<string[]>([])
        selectedAvailableReports.forEach(function(value){
            reports.value.push(value);
        });
        
        simulationReports = clone(reports.value);
        inventoryReports = clone(reports.value);
    })
    watch(stateInventoryReportNames,() =>
    {
        selectedInventoryReports.value = clone(stateInventoryReportNames.value);
    })
    watch(statePrimaryNetwork,() => {
        selectPrimaryNetworkItemValue.value = statePrimaryNetwork.value;
    })
    function onStatePrimaryNetworkChanged()
    {
        selectPrimaryNetworkItemValue.value = statePrimaryNetwork.value;
    }
    watch(stateRawdataNetwork,() =>  {
        selectRawdataNetworkItemValue.value = stateRawdataNetwork.value;
    })
    function  onStateRawdataNetworkChanged()
    {
        selectPrimaryNetworkItemValue.value = statePrimaryNetwork.value;
    }
    watch(stateKeyFields,() => {
        selectedKeyFields.value = clone(stateKeyFields.value);
    })
    watch(stateRawDataKeyFields,() =>{
        selectedRawDataKeyFields.value = clone(stateRawDataKeyFields.value);
    })
    watch(stateConstraintType,() => {
        constraintTypeRadioGroup.value = stateConstraintType.value
    })
    function onStateConstraintTypeChanged()
    {
        constraintTypeRadioGroup.value = stateConstraintType.value
    }
    //watch(networks,() =>{
      //  selectPrimaryNetworkItems = networks.map(_ => {
       //     return {text: _.name, value: _.name}
      //  });
      //  selectRawDataNetworkItems = networks.map(_ => {
      //      return {text: _.name, value: _.name}
       // });
    //})
    watch(selectedKeyFields,() => {
        keyFieldsDelimited.value = '';
        keyFieldsDelimited.value = convertToDelimited(selectedKeyFields.value, ', ')
        checkHasUnsaved();       
    })
    watch(selectedRawDataKeyFields,() => {
        rawDataKeyFieldsDelimited.value = '';
        rawDataKeyFieldsDelimited.value = convertToDelimited(selectedRawDataKeyFields.value, ', ')
        checkHasUnsaved();       
    })
    watch(selectedSimulationReports,() => {
        simulationReportsDelimited.value = '';
        simulationReportsDelimited.value = convertToDelimited(selectedSimulationReports.value, ', ')
        checkHasUnsaved();       
    })

    watch(selectedInventoryReports,() => {
        inventoryReportsDelimited.value = '';
        inventoryReportsDelimited.value = convertToDelimited(selectedInventoryReports.value, ', ')
        checkHasUnsaved();       
    })

    watch(selectPrimaryNetworkItemValue,() => {
        if(selectPrimaryNetworkItemValue === null)
            primaryNetwork.value = ''
        else
            primaryNetwork.value = selectPrimaryNetworkItemValue.value;  
        checkHasUnsaved();       
    })

    watch(selectRawdataNetworkItemValue,() =>  {
        if (selectRawdataNetworkItemValue === null) 
            rawdataNetwork.value = '';
        else 
            rawdataNetwork.value = selectRawdataNetworkItemValue.value;
        checkHasUnsaved();
    })

    watch(primaryNetwork,() => {
        checkHasUnsaved();       
    })

    watch(constraintTypeRadioGroup,() => {
        checkHasUnsaved();
    })

    //Buttons
    function onSaveClick(){
        
        (async () => {
            await setPrimaryNetworkAction(primaryNetwork.value); 
            await setRawdataNetworkAction(rawdataNetwork.value);
            await setConstraintTypeAction(constraintTypeRadioGroup.value);
            await setSimulationReportsAction(convertToDelimited(selectedSimulationReports.value, ','));
            await setInventoryReportsAction(convertToDelimited(selectedInventoryReports.value, ','));
            await setKeyFieldsAction(convertToDelimited(selectedKeyFields.value, ','));
            await setRawDataKeyFieldsAction(convertToDelimited(selectedRawDataKeyFields.value, ','));                      
        })();
    }
    function onEditKeyFieldsClick(){
        editAdminDataDialogData.selectedItem=''
        editAdminDataDialogData.AddedItems = selectedKeyFields.value.map(str=>
        {
            return {value:str, networkType:''}
        })
        editAdminDataDialogData.selectedSettings = clone(selectedKeyFields.value);
        editAdminDataDialogData.settingName = keyFieldsName;
        editAdminDataDialogData.settingsList = clone(keyFieldsForDialog.value);
        editAdminDataDialogData.showDialog = true;
    }
    function onEditRawDataKeyFieldsClick(){
        editAdminDataDialogData.selectedItem=''
        editAdminDataDialogData.AddedItems = selectedRawDataKeyFields.value.map(str=>
        {
            return {value:str, networkType:''}
        })
        editAdminDataDialogData.showDialog = true;
        editAdminDataDialogData.selectedSettings = clone( selectedRawDataKeyFields.value);
        editAdminDataDialogData.settingName =   rawDataKeyFieldsName;
        editAdminDataDialogData.settingsList = clone(  keyFieldsForDialog.value);
    }
    function onEditInventoryReportsClick(){
        editAdminDataDialogData.selectedItem=''
        editAdminDataDialogData.AddedItems = selectedInventoryReports.value.map(str=>
        {
            return {value:str, networkType:''}
        })
        
        editAdminDataDialogData.showDialog = true;
        editAdminDataDialogData.selectedSettings = clone(  selectedInventoryReports.value);
        editAdminDataDialogData.settingName =   inventoryReportsName;
        editAdminDataDialogData.settingsList = clone(inventoryReports);
    }
    function onEditSimulationReportsClick(){
        editAdminDataDialogData.selectedItem=''
        editAdminDataDialogData.AddedItems = selectedSimulationReports.value.map(str=>
        {
            return {value:str, networkType:''}
        })
        editAdminDataDialogData.showDialog = true;
        editAdminDataDialogData.selectedSettings = clone(  selectedSimulationReports.value);
        editAdminDataDialogData.settingName =   simulationReportsName;
        editAdminDataDialogData.settingsList = clone( simulationReports);
    }
    function onSubmitEditAdminDataDialogResult(selectedSettings: string[]){
        if(selectedSettings !== null)

            switch(  editAdminDataDialogData.settingName){
                case keyFieldsName:
                    keyFieldsDelimited.value=''
                    selectedKeyFields.value=[];
                    selectedSettings.forEach(function(value){
                        selectedKeyFields.value.push(value);   
                                     }     )     
                    const temp = convertToDelimited(selectedKeyFields.value,',');
                    keyFieldsDelimited.value = temp;              
                    break;
                case   rawDataKeyFieldsName:
                rawDataKeyFieldsDelimited.value=''
                    selectedRawDataKeyFields.value=[];
                    selectedSettings.forEach(function(value){
                        selectedRawDataKeyFields.value.push(value);   
                                     }     )     
                    const delimited = convertToDelimited(selectedRawDataKeyFields.value,',');
                    rawDataKeyFieldsDelimited.value = delimited; 
                    break;
                case   inventoryReportsName:
                    inventoryReportsDelimited.value=''
                    selectedInventoryReports.value=[];
                    selectedSettings.forEach(function(value){
                        selectedInventoryReports.value.push(value);   
                                     }     )     
                    const inventory = convertToDelimited(selectedInventoryReports.value,',');
                    inventoryReportsDelimited.value = inventory; 
                    break;
                case   simulationReportsName:
                simulationReportsDelimited.value=''
                selectedSimulationReports.value=[];
                    selectedSettings.forEach(function(value){
                        selectedSimulationReports.value.push(value);   
                                     }     )     
                    const simulation = convertToDelimited(selectedSimulationReports.value,',');
                    simulationReportsDelimited.value = simulation;
                    break;
            }

            editAdminDataDialogData.selectedSettings = [];
            editAdminDataDialogData.settingName = clone(emptyEditAdminDataDialogData.settingName);
            editAdminDataDialogData.settingsList = clone(emptyEditAdminDataDialogData.settingsList);
            editAdminDataDialogData.showDialog = false;
            editAdminDataDialogData.selectedItem ='';
    }

    //Subroutines
    function checkHasUnsaved(){
        const hasChanged = hasUnsavedChangesCore('', selectedInventoryReports.value, stateInventoryReportNames.value) ||
            hasUnsavedChangesCore('', selectedSimulationReports.value, stateSimulationReportNames.value) ||
            hasUnsavedChangesCore('', selectedKeyFields.value, stateKeyFields.value) ||
            hasUnsavedChangesCore('', selectedRawDataKeyFields.value, stateRawDataKeyFields.value) ||
            primaryNetwork.value != statePrimaryNetwork.value || rawdataNetwork.value != stateRawdataNetwork.value ||
            constraintTypeRadioGroup.value != stateConstraintType.value
        setHasUnsavedChangesAction({ value: hasChanged });
    }
    function convertToDelimited(listToBeDelimited: string[], delimitValue: string){
        let delimitedList = '';
        for(let i = 0; i < listToBeDelimited.length; i++){
            delimitedList += listToBeDelimited[i];
            if(i !== listToBeDelimited.length - 1){
                delimitedList += delimitValue;
            }
        }

        return delimitedList
    }
    function disableCrudButtons() {
        let allValid = rules['generalRules'].valueIsNotEmpty(selectedKeyFields.value) === true
            && rules['generalRules'].valueIsNotEmpty(selectedInventoryReports.value) === true
            && rules['generalRules'].valueIsNotEmpty(selectedSimulationReports.value) === true
            && rules['generalRules'].valueIsNotEmpty(primaryNetwork.value) === true
            && rules['generalRules'].valueIsNotEmpty(rawdataNetwork.value) === true
            && rules['generalRules'].valueIsNotEmpty(constraintTypeRadioGroup.value) === true

        return allValid;
    }



</script>

<style>
    .elipsisList { 
        white-space: nowrap !important;
        overflow: hidden !important;
        text-overflow: ellipsis !important;
    }

    .v-input--radio-group {
        padding-top: 0 !important;
        margin-top: 0 !important;
    }

    .admin-radio label{
        margin-bottom: 0 !important;
        font-weight: 500 !important;
    }

</style>