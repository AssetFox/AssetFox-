<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs6 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Primary Network</v-subheader>
                        <v-select :items='selectPrimaryNetworkItems'
                            variant="outlined"
                            v-model='selectPrimaryNetworkItemValue'                         
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                </v-layout>
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Raw Data Network</v-subheader>
                        <v-select :items="selectRawDataNetworkItems"
                            variant="outlined"
                            v-model="selectRawdataNetworkItemValue"
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>
                    </v-layout>
                </v-layout>
            </v-flex>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Key Field(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs5>
                        <div class="ghd-md-gray ghd-control-label elipsisList">{{keyFieldsDelimited}}</div>  
                    </v-flex>                        
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditKeyFieldsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Raw Data Key Field(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs5>
                        <div class="ghd-md-gray ghd-control-label elipsisList">{{rawDataKeyFieldsDelimited}}</div>  
                    </v-flex>                        
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditRawDataKeyFieldsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Inventory Report(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs5>
                         <div class="ghd-md-gray ghd-control-label elipsisList">{{inventoryReportsDelimited}}</div> 
                    </v-flex>                 
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditInventoryReportsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Simulation Report(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs5>
                        <div class="ghd-md-gray ghd-control-label elipsisList">{{simulationReportsDelimited}}</div> 
                    </v-flex>                     
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onEditSimulationReportsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>            
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Constraint Type: </v-subheader> 
                    </v-flex>
                    <v-flex xs5>
                        <v-radio-group v-model="constraintTypeRadioGroup" row>
                            <v-radio class="admin-radio" label="OR" value="OR"></v-radio>
                            <v-radio class="admin-radio" label="AND" value="AND"></v-radio>
                        </v-radio-group>
                    </v-flex>                     
                </v-layout>
            </v-flex>     
        </v-flex>  
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs7>
                    <v-btn style="margin-top: 5px !important;" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onSaveClick" :disabled='disableCrudButtons() || !hasUnsavedChanges'>
                        Save
                    </v-btn>
                </v-flex>
            </v-layout>
        </v-flex>
        <EditAdminDataDialog :DialogData='editAdminDataDialogData'
                                     @submit='onSubmitEditAdminDataDialogResult' />
    </v-layout>
</template>

<script lang='ts' setup>

import { SelectItem } from '@/shared/models/vue/select-item';
import Vue, { shallowRef } from 'vue';

import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import EditAdminDataDialog from './EditAdminDataDialog.vue';
import {EditAdminDataDialogData, emptyEditAdminDataDialogData} from '@/shared/models/modals/edit-data-dialog-data';
import { clone } from 'ramda';

import { Network } from '@/shared/models/iAM/network';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    let stateAvailableReportNames = ref<string[]>(store.state.adminDataModule.availableReportNames);
    let stateSimulationReportNames = ref<string[]>(store.state.adminDataModule.stateSimulationReportNames);
    let stateInventoryReportNames = ref<string[]>(store.state.adminDataModule.stateInventoryReportNames);
    let statePrimaryNetwork = ref<string>(store.state.adminDataModule.statePrimaryNetwork);
    let stateKeyFields = ref<string[]>(store.state.adminDataModule.keyFields);
    let stateRawDataKeyFields = ref<string[]>(store.state.adminDataModule.rawDataKeyFields);
    let stateRawdataNetwork = ref<string>(store.state.adminDataModule.stateRawdataNetwork);
    let stateConstraintType = ref<string>(store.state.adminDataModule.stateConstraintType);
    let hasUnsavedChanges = ref<boolean>(store.state.adminDataModule.hasUnsavedChanges);
    let stateNetworks = ref<Network[]>(store.state.adminDataModule.stateNetworks);
    let stateAttributes = ref<Attribute[]>(store.state.adminDataModule.stateAttributes);
    async function getAvailableReportsAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}
    async function getSimulationReportsAction(payload?: any): Promise<any> {await store.dispatch('getSimulationReports');}
    async function getInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('getInventoryReports');}
    async function getPrimaryNetworkAction(payload?: any): Promise<any> {await store.dispatch('getPrimaryNetwork');}
    async function getRawdataNetworkAction(payload?: any): Promise<any> {await store.dispatch('getRawdataNetwork');}
    async function getKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields');}
    async function getRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getRawDataKeyFields');}
    async function getConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('getConstraintType');}
    async function setSimulationReportsAction(payload?: any): Promise<any> {await store.dispatch('setSimulationReports');}
    async function setInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('setInventoryReports');}
    async function setPrimaryNetworkAction(payload?: any): Promise<any> {await store.dispatch('setPrimaryNetwork');}
    async function setRawdataNetworkAction(payload?: any): Promise<any> {await store.dispatch('setRawdataNetwork');}
    async function setKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('setKeyFields');}
    async function setRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('setRawDataKeyFields');}
    async function setConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('setConstraintType');}
    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks');}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}

    let selectPrimaryNetworkItems: SelectItem[] = [];
    let selectRawDataNetworkItems: SelectItem[] = [];
    let selectPrimaryNetworkItemValue = shallowRef<string>('');
    let selectRawdataNetworkItemValue = shallowRef<string>('');

    const props = defineProps<{
    editAdminDataDialogData: EditAdminDataDialogData
    }>()

    let keyFields: string[] = [];
    let simulationReports: string[] = [];
    let inventoryReports: string[] = [];
    let networks: Network[] = [];

    let selectedKeyFields: string[] = [];
    let selectedRawDataKeyFields: string[] = []
    let selectedAvailableReports: string[] = [];
    let selectedSimulationReports: string[] = [];
    let selectedInventoryReports: string[] = [];
    let primaryNetwork = shallowRef<string>('');
    let rawdataNetwork: string = '';

    let keyFieldsDelimited: string = '';
    let rawDataKeyFieldsDelimited: string = '';
    let simulationReportsDelimited: string = '';
    let inventoryReportsDelimited: string = '';

    let keyFieldsName: string  = 'KeyFields';
    let rawDataKeyFieldsName: string = 'RawDataKeyFields';
    let simulationReportsName: string = 'SimulationReports';
    let inventoryReportsName: string = 'InventoryReports';

    let InputRules: InputValidationRules = rules;

    let constraintTypeRadioGroup = shallowRef<string>('');
    created();

    function created() {
        (() => {
            (async () => { 
                await getAttributes();
                await getNetworks();
                await getAvailableReportsAction();                
                await getSimulationReportsAction();
                await getInventoryReportsAction();
                await getPrimaryNetworkAction();
                await getRawdataNetworkAction();
                if(selectPrimaryNetworkItemValue === null)
                    onStatePrimaryNetworkChanged();
                if(selectRawdataNetworkItemValue === null)
                    onStateRawdataNetworkChanged();
                await getKeyFieldsAction();
                await getRawDataKeyFieldsAction();
                await getConstraintTypeAction();
                onStateConstraintTypeChanged();
            })();
            
        });
    }
    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    //watchers
    watch(stateNetworks, () => onStateNetworksChanged)
    function onStateNetworksChanged() {
        networks = clone(stateNetworks.value);
    }
    watch(stateAttributes,() => onStateAttributesChanged)
    function onStateAttributesChanged(){
        keyFields = clone(stateAttributes.value).map(_ =>_.name) //.map(_ => _.name)
    }
    watch(stateSimulationReportNames,() => onStateSimulationReportNamesChanged)
    function onStateSimulationReportNamesChanged(){
        selectedSimulationReports = clone(stateSimulationReportNames.value);
    }
    watch(stateAvailableReportNames,() => onStateAvailableReportNamesChanged)
    function onStateAvailableReportNamesChanged()
    {
        selectedAvailableReports = clone(stateAvailableReportNames.value);
        const reports: string[] = stateAvailableReportNames.value;
        simulationReports = clone(reports);
        inventoryReports = clone(reports);
    }
    watch(stateInventoryReportNames,() =>onStateInventoryReportNamesChanged)
    function onStateInventoryReportNamesChanged()
    {
        selectedInventoryReports = clone(stateInventoryReportNames.value);
    }
    watch(statePrimaryNetwork,() => onStatePrimaryNetworkChanged)
    function onStatePrimaryNetworkChanged(){
        selectPrimaryNetworkItemValue.value = statePrimaryNetwork.value;
    }
    watch(stateRawdataNetwork,() => onStateRawdataNetworkChanged)
    function onStateRawdataNetworkChanged() {
        selectRawdataNetworkItemValue.value = stateRawdataNetwork.value;
    }
    watch(stateKeyFields,() => onStateKeyFieldsChanged)
    function onStateKeyFieldsChanged(){
        selectedKeyFields = clone(stateKeyFields.value);
    }
    watch(stateRawDataKeyFields,() => onStateRawDataKeyFieldsChanged)
    function onStateRawDataKeyFieldsChanged(){
        selectedRawDataKeyFields = clone(stateRawDataKeyFields.value);
    }
    watch(stateConstraintType,() => onStateConstraintTypeChanged)
    function onStateConstraintTypeChanged(){
        constraintTypeRadioGroup.value = stateConstraintType.value
    }
    watch(networks,() => onNetworksChanged)
    function onNetworksChanged(){
        selectPrimaryNetworkItems = networks.map(_ => {
            return {text: _.name, value: _.name}
        });
        selectRawDataNetworkItems = networks.map(_ => {
            return {text: _.name, value: _.name}
        });
    }
    watch(selectedKeyFields,() => onSelectedKeyFieldsChanged)
    function onSelectedKeyFieldsChanged(){
        keyFieldsDelimited = '';
        keyFieldsDelimited = convertToDelimited(selectedKeyFields, ', ')
        checkHasUnsaved();       
    }
    watch(selectedRawDataKeyFields,() => onSelectedRawDataKeyFieldsChanged)
    function onSelectedRawDataKeyFieldsChanged(){
        rawDataKeyFieldsDelimited = '';
        rawDataKeyFieldsDelimited = convertToDelimited(selectedRawDataKeyFields, ', ')
        checkHasUnsaved();       
    }
    watch(selectedSimulationReports,() => onSelectedSimulationReportsChanged)
    function onSelectedSimulationReportsChanged(){
        simulationReportsDelimited = '';
        simulationReportsDelimited = convertToDelimited(selectedSimulationReports, ', ')
        checkHasUnsaved();       
    }

    watch(selectedInventoryReports,() => onSelectedInventoryReportsChanged)
    function onSelectedInventoryReportsChanged(){
        inventoryReportsDelimited = '';
        inventoryReportsDelimited = convertToDelimited(selectedInventoryReports, ', ')
        checkHasUnsaved();       
    }

    watch(selectPrimaryNetworkItemValue,() => onSelectPrimaryNetworkItemValueChanged)
    function onSelectPrimaryNetworkItemValueChanged(){
        if(selectPrimaryNetworkItemValue === null)
            primaryNetwork.value = ''
        else
            primaryNetwork.value = selectPrimaryNetworkItemValue.value;  
        checkHasUnsaved();       
    }

    watch(selectRawdataNetworkItemValue,() => onSelectRawdataNetworkItemValueChanged)
    function onSelectRawdataNetworkItemValueChanged() {
        if (selectRawdataNetworkItemValue === null) 
            rawdataNetwork = '';
        else 
            rawdataNetwork = selectRawdataNetworkItemValue.value;
        checkHasUnsaved();
    }

    watch(primaryNetwork,() => onPrimaryNetworkChanged)
    function onPrimaryNetworkChanged(){
        checkHasUnsaved();       
    }

    watch(constraintTypeRadioGroup,() => onConstraintTypeRadioGroupChanged)
    function onConstraintTypeRadioGroupChanged(){
        checkHasUnsaved();
    }

    //Buttons
    function onSaveClick(){
        (async () => {
            await setPrimaryNetworkAction(primaryNetwork); 
            await setRawdataNetworkAction(rawdataNetwork);
            await setConstraintTypeAction(constraintTypeRadioGroup);
            await setSimulationReportsAction(convertToDelimited(selectedSimulationReports, ','));
            await setInventoryReportsAction(convertToDelimited(selectedInventoryReports, ','));
            await setKeyFieldsAction(convertToDelimited(selectedKeyFields, ','));
            await setRawDataKeyFieldsAction(convertToDelimited(selectedRawDataKeyFields, ','));                      
        })();
    }
    function onEditKeyFieldsClick(){
        props.editAdminDataDialogData.showDialog = true;
        props.editAdminDataDialogData.selectedSettings = clone(selectedKeyFields);
        props.editAdminDataDialogData.settingName = keyFieldsName;
        props.editAdminDataDialogData.settingsList = clone(keyFields);
    }
    function onEditRawDataKeyFieldsClick(){
        props.editAdminDataDialogData.showDialog = true;
        props.editAdminDataDialogData.selectedSettings = clone(  selectedRawDataKeyFields);
        props.editAdminDataDialogData.settingName =   rawDataKeyFieldsName;
        props.editAdminDataDialogData.settingsList = clone(  keyFields);
    }
    function onEditInventoryReportsClick(){
        props.editAdminDataDialogData.showDialog = true;
        props.editAdminDataDialogData.selectedSettings = clone(  selectedInventoryReports);
        props.editAdminDataDialogData.settingName =   inventoryReportsName;
        props.editAdminDataDialogData.settingsList = clone(  inventoryReports);
    }
    function onEditSimulationReportsClick(){
        props.editAdminDataDialogData.showDialog = true;
        props.editAdminDataDialogData.selectedSettings = clone(  selectedSimulationReports);
        props.editAdminDataDialogData.settingName =   simulationReportsName;
        props.editAdminDataDialogData.settingsList = clone(  simulationReports);
    }
    function onSubmitEditAdminDataDialogResult(selectedSettings: string[]){
        if(selectedSettings !== null)
            switch(  props.editAdminDataDialogData.settingName){
                case   keyFieldsName:
                      selectedKeyFields = clone(selectedSettings);
                    break;
                case   rawDataKeyFieldsName:
                      selectedRawDataKeyFields = clone(selectedSettings);
                    break;
                case   inventoryReportsName:
                      selectedInventoryReports = clone(selectedSettings);
                    break;
                case   simulationReportsName:
                      selectedSimulationReports = clone(selectedSettings);
                    break;
            }

            props.editAdminDataDialogData.selectedSettings = clone(emptyEditAdminDataDialogData.selectedSettings);
            props.editAdminDataDialogData.settingName = clone(emptyEditAdminDataDialogData.settingName);
            props.editAdminDataDialogData.settingsList = clone(emptyEditAdminDataDialogData.settingsList);
            props.editAdminDataDialogData.showDialog = clone(emptyEditAdminDataDialogData.showDialog);
    }

    //Subroutines
    function checkHasUnsaved(){
        const hasChanged = hasUnsavedChangesCore('', selectedInventoryReports, stateInventoryReportNames) ||
            hasUnsavedChangesCore('', selectedSimulationReports, stateSimulationReportNames) ||
            hasUnsavedChangesCore('', selectedKeyFields, stateKeyFields) ||
            hasUnsavedChangesCore('', selectedRawDataKeyFields, stateRawDataKeyFields) ||
            primaryNetwork.value != statePrimaryNetwork.value || rawdataNetwork != stateRawdataNetwork.value ||
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
        let allValid = rules['generalRules'].valueIsNotEmpty(selectedKeyFields) === true
            && rules['generalRules'].valueIsNotEmpty(selectedInventoryReports) === true
            && rules['generalRules'].valueIsNotEmpty(selectedSimulationReports) === true
            && rules['generalRules'].valueIsNotEmpty(primaryNetwork.value.trim()) === true
            && rules['generalRules'].valueIsNotEmpty(constraintTypeRadioGroup.value.trim()) === true

        return !allValid;
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