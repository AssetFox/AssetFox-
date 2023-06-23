<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs6 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Primary Network</v-subheader>
                        <v-select :items='selectPrimaryNetworkItems'
                            outline  
                            v-model='selectPrimaryNetworkItemValue'                         
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
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
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
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
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
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
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
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
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
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
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

<script lang='ts'>

import { SelectItem } from '@/shared/models/vue/select-item';
import Vue from 'vue';
import Component from 'vue-class-component';
import EditAdminDataDialog from './EditAdminDataDialog.vue';
import {EditAdminDataDialogData, emptyEditAdminDataDialogData} from '@/shared/models/modals/edit-data-dialog-data';
import { clone } from 'ramda';
import { watch } from 'fs';
import { Watch } from 'vue-property-decorator';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { getNewGuid } from '@/shared/utils/uuid-utils';
import { Action, Getter, Mutation, State } from 'vuex-class';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { VMenu } from 'vuetify/lib';


@Component({
    components:{
        EditAdminDataDialog
    }  
})
export default class Data extends Vue {
    @State(state => state.adminDataModule.availableReportNames) stateAvailableReportNames: string[];
    @State(state => state.adminDataModule.simulationReportNames) stateSimulationReportNames: string[];
    @State(state => state.adminDataModule.inventoryReportNames) stateInventoryReportNames: string[];
    @State(state => state.adminDataModule.primaryNetwork) statePrimaryNetwork: string;   
    @State(state => state.adminDataModule.keyFields) stateKeyFields: string[];
    @State(state => state.adminDataModule.rawDataKeyFields) stateRawDataKeyFields: string[];
    @State(state => state.adminDataModule.constraintType) stateConstraintType: string;
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

    @Action('getAvailableReports') getAvailableReportsAction: any;
    @Action('getSimulationReports') getSimulationReportsAction: any;
    @Action('getInventoryReports') getInventoryReportsAction: any;
    @Action('getPrimaryNetwork') getPrimaryNetworkAction: any;
    @Action('getKeyFields') getKeyFieldsAction: any;
    @Action('getRawDataKeyFields') getRawDataKeyFieldsAction: any;
    @Action('getConstraintType') getConstraintTypeAction: any;
    @Action('setSimulationReports') setSimulationReportsAction: any;
    @Action('setInventoryReports') setInventoryReportsAction: any;
    @Action('setPrimaryNetwork') setPrimaryNetworkAction: any;
    @Action('setKeyFields') setKeyFieldsAction: any;
    @Action('setRawDataKeyFields') setRawDataKeyFieldsAction: any;
    @Action('setConstraintType') setConstraintTypeAction: any;
    @Action('getNetworks') getNetworks: any;
    @Action('getAttributes') getAttributes: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

    selectPrimaryNetworkItems: SelectItem[] = [];
    selectPrimaryNetworkItemValue: string | null = null;

    editAdminDataDialogData: EditAdminDataDialogData = clone(emptyEditAdminDataDialogData)

    keyFields: string[] = [];
    simulationReports: string[] = [];
    inventoryReports: string[] = [];
    networks: Network[] = [];

    selectedKeyFields: string[] = [];
    selectedRawDataKeyFields: string[] = []
    selectedAvailableReports: string[] = [];
    selectedSimulationReports: string[] = [];
    selectedInventoryReports: string[] = [];
    primaryNetwork: string = '';

    keyFieldsDelimited: string = '';
    rawDataKeyFieldsDelimited: string = '';
    simulationReportsDelimited: string = '';
    inventoryReportsDelimited: string = '';

    keyFieldsName: string  = 'KeyFields';
    rawDataKeyFieldsName: string = 'RawDataKeyFields';
    simulationReportsName: string = 'SimulationReports';
    inventoryReportsName: string = 'InventoryReports';

    rules: InputValidationRules = rules;

    constraintTypeRadioGroup: string = '';

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            (async () => { 
                await vm.getAttributes();
                await vm.getNetworks();
                await vm.getAvailableReportsAction();                
                await vm.getSimulationReportsAction();
                await vm.getInventoryReportsAction();
                await vm.getPrimaryNetworkAction();
                if(vm.selectPrimaryNetworkItemValue === null)
                    vm.onStatePrimaryNetworkChanged();
                await vm.getKeyFieldsAction();
                await vm.getRawDataKeyFields();
                await vm.getConstraintTypeAction();
                vm.onStateConstraintTypeChanged();
            })();
            
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }
    //Watches
    @Watch('stateNetworks')
    onStateNetworksChanged(){
        this.networks = clone(this.stateNetworks);
    }

    @Watch('stateAttributes')
    onStateAttributesChanged(){
        this.keyFields = clone(this.stateAttributes).map(_ => _.name)
    }

    @Watch('stateSimulationReportNames')
    onStateSimulationReportNamesChanged(){
        this.selectedSimulationReports = clone(this.stateSimulationReportNames);
    }
    @Watch('stateAvailableReportNames')
    onStateAvailableReportNamesChanged(){
        this.selectedAvailableReports = clone(this.stateAvailableReportNames);
        const reports: string[] = this.stateAvailableReportNames;
        this.simulationReports = clone(reports);
        this.inventoryReports = clone(reports);
    }

    @Watch('stateInventoryReportNames')
    onStateInventoryReportNamesChanged(){
        this.selectedInventoryReports = clone(this.stateInventoryReportNames);
    }

    @Watch('statePrimaryNetwork')
    onStatePrimaryNetworkChanged(){
        this.selectPrimaryNetworkItemValue = this.statePrimaryNetwork
    }

    @Watch('stateKeyFields')
    onStateKeyFieldsChanged(){
        this.selectedKeyFields = clone(this.stateKeyFields);
    }

    @Watch('stateRawDataKeyFields')
    onStateRawDataKeyFieldsChanged(){
        this.selectedRawDataKeyFields = clone(this.stateRawDataKeyFields);
    }

    @Watch('stateConstraintType')
    onStateConstraintTypeChanged(){
        this.constraintTypeRadioGroup = this.stateConstraintType
    }

    @Watch('networks')
    onNetworksChanged(){
        this.selectPrimaryNetworkItems = this.networks.map(_ => {
            return {text: _.name, value: _.name}
        });
    }

    @Watch('selectedKeyFields')
    onSelectedKeyFieldsChanged(){
        this.keyFieldsDelimited = '';
        this.keyFieldsDelimited = this.convertToDelimited(this.selectedKeyFields, ', ')
        this.checkHasUnsaved();       
    }

    @Watch('selectedRawDataKeyFields')
    onSelectedRawDataKeyFieldsChanged(){
        this.rawDataKeyFieldsDelimited = '';
        this.rawDataKeyFieldsDelimited = this.convertToDelimited(this.selectedRawDataKeyFields, ', ')
        this.checkHasUnsaved();       
    }

    @Watch('selectedSimulationReports')
    onSelectedSimulationReportsChanged(){
        this.simulationReportsDelimited = '';
        this.simulationReportsDelimited = this.convertToDelimited(this.selectedSimulationReports, ', ')
        this.checkHasUnsaved();       
    }

    @Watch('selectedInventoryReports')
    onSelectedInventoryReportsChanged(){
        this.inventoryReportsDelimited = '';
        this.inventoryReportsDelimited = this.convertToDelimited(this.selectedInventoryReports, ', ')
        this.checkHasUnsaved();       
    }

    @Watch('selectPrimaryNetworkItemValue')
    onSelectPrimaryNetworkItemValueChanged(){
        if(this.selectPrimaryNetworkItemValue === null)
            this.primaryNetwork = ''
        else
            this.primaryNetwork = this.selectPrimaryNetworkItemValue;  
        this.checkHasUnsaved();       
    }

    @Watch('primaryNetwork')
    onPrimaryNetworkChanged(){
        this.checkHasUnsaved();       
    }

    @Watch('constraintTypeRadioGroup')
    onConstraintTypeRadioGroupChanged(){
        this.checkHasUnsaved();
    }
    //Buttons
    onSaveClick(){
        (async () => {
            await this.setPrimaryNetworkAction(this.primaryNetwork); 
            await this.setConstraintTypeAction(this.constraintTypeRadioGroup);
            await this.setSimulationReportsAction(this.convertToDelimited(this.selectedSimulationReports, ','));
            await this.setInventoryReportsAction(this.convertToDelimited(this.selectedInventoryReports, ','));
            await this.setKeyFieldsAction(this.convertToDelimited(this.selectedKeyFields, ','));
            await this.setRawDataKeyFieldsAction(this.convertToDelimited(this.selectedRawDataKeyFields, ','));                      
        })();
    }

    onEditKeyFieldsClick(){
        this.editAdminDataDialogData.showDialog = true;
        this.editAdminDataDialogData.selectedSettings = clone(this.selectedKeyFields);
        this.editAdminDataDialogData.settingName = this.keyFieldsName;
        this.editAdminDataDialogData.settingsList = clone(this.keyFields);
    }
    onEditRawDataKeyFieldsClick(){
        this.editAdminDataDialogData.showDialog = true;
        this.editAdminDataDialogData.selectedSettings = clone(this.selectedRawDataKeyFields);
        this.editAdminDataDialogData.settingName = this.rawDataKeyFieldsName;
        this.editAdminDataDialogData.settingsList = clone(this.keyFields);
    }
    onEditInventoryReportsClick(){
        this.editAdminDataDialogData.showDialog = true;
        this.editAdminDataDialogData.selectedSettings = clone(this.selectedInventoryReports);
        this.editAdminDataDialogData.settingName = this.inventoryReportsName;
        this.editAdminDataDialogData.settingsList = clone(this.inventoryReports);
    }
    onEditSimulationReportsClick(){
        this.editAdminDataDialogData.showDialog = true;
        this.editAdminDataDialogData.selectedSettings = clone(this.selectedSimulationReports);
        this.editAdminDataDialogData.settingName = this.simulationReportsName;
        this.editAdminDataDialogData.settingsList = clone(this.simulationReports);
    }
    onSubmitEditAdminDataDialogResult(selectedSettings: string[]){
        if(selectedSettings !== null)
            switch(this.editAdminDataDialogData.settingName){
                case this.keyFieldsName:
                    this.selectedKeyFields = clone(selectedSettings);
                    break;
                case this.rawDataKeyFieldsName:
                    this.selectedRawDataKeyFields = clone(selectedSettings);
                    break;
                case this.inventoryReportsName:
                    this.selectedInventoryReports = clone(selectedSettings);
                    break;
                case this.simulationReportsName:
                    this.selectedSimulationReports = clone(selectedSettings);
                    break;
            }

        this.editAdminDataDialogData = clone(emptyEditAdminDataDialogData);
    }
    //Subroutines
    checkHasUnsaved(){
        const hasChanged = hasUnsavedChangesCore('', this.selectedInventoryReports, this.stateInventoryReportNames) ||
            hasUnsavedChangesCore('', this.selectedSimulationReports, this.stateSimulationReportNames) ||
            hasUnsavedChangesCore('', this.selectedKeyFields, this.stateKeyFields) ||
            hasUnsavedChangesCore('', this.selectedRawDataKeyFields, this.stateRawDataKeyFields) ||
            this.primaryNetwork != this.statePrimaryNetwork ||
            this.constraintTypeRadioGroup != this.stateConstraintType
        this.setHasUnsavedChangesAction({ value: hasChanged });
    }
    convertToDelimited(listToBeDelimited: string[], delimitValue: string){
        let delimitedList = '';
        for(let i = 0; i < listToBeDelimited.length; i++){
            delimitedList += listToBeDelimited[i];
            if(i !== listToBeDelimited.length - 1){
                delimitedList += delimitValue;
            }
        }

        return delimitedList
    }

    disableCrudButtons() {
        let allValid = this.rules['generalRules'].valueIsNotEmpty(this.selectedKeyFields) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedInventoryReports) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedSimulationReports) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.primaryNetwork.trim()) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.constraintTypeRadioGroup.trim()) === true

        return !allValid;
    }
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