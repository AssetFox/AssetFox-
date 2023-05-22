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
                    <v-btn style="margin-top: 20px !important; margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="onSaveClick">
                        Save
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs6 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs3>
                        <v-subheader class="ghd-md-gray ghd-control-label">Key Field(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs3>
                        <v-subheader class="ghd-md-gray ghd-control-label">{{keyFieldsDelimited}}</v-subheader>  
                    </v-flex>                        
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="onEditKeyFieldsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs6 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs3>
                        <v-subheader class="ghd-md-gray ghd-control-label">Inventory Report(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs3>
                         <v-subheader class="ghd-md-gray ghd-control-label">{{inventoryReportsDelimited}}</v-subheader> 
                    </v-flex>                 
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="onEditInventoryReportsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>
            <v-flex xs6 class="ghd-constant-header">
                <v-layout>
                    <v-flex xs3>
                        <v-subheader class="ghd-md-gray ghd-control-label">Simulation Report(s): </v-subheader> 
                    </v-flex>
                    <v-flex xs3>
                        <v-subheader class="ghd-md-gray ghd-control-label">{{simulationReportsDelimited}}</v-subheader> 
                    </v-flex>                     
                    <v-btn style="margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="onEditSimulationReportsClick">
                        Edit
                    </v-btn>
                </v-layout>
            </v-flex>            
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


@Component({
    components:{
        EditAdminDataDialog
    }  
})
export default class Data extends Vue {
    @State(state => state.adminDataModule.simulationReportNames)
    stateSimulationReportNames: string[];
    @State(state => state.adminDataModule.inventoryReportNames)
    stateInventoryReportNames: string[];
    @State(state => state.adminDataModule.primaryNetwork)
    statePrimaryNetwork: Network;   
    @State(state => state.adminDataModule.keyFields)
    stateKeyFields: string[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

    @Action('getSimulationReports') getSimulationReportsAction: any;
    @Action('getInventoryReports') getInventoryReportsAction: any;
    @Action('getPrimaryNetwork') getPrimaryNetworkAction: any;
    @Action('getKeyFields') getKeyFieldsAction: any;
    @Action('setSimulationReports') setSimulationReportsAction: any;
    @Action('setInventoryReports') setInventoryReportsAction: any;
    @Action('setPrimaryNetwork') setPrimaryNetworkAction: any;
    @Action('setKeyFields') setKeyFieldsAction: any;
    @Action('getNetworks') getNetworks: any;
    @Action('getAttributes') getAttributes: any;

    selectPrimaryNetworkItems: SelectItem[] = [];
    selectPrimaryNetworkItemValue: string | null = '';

    editAdminDataDialogData: EditAdminDataDialogData = clone(emptyEditAdminDataDialogData)

    keyFields: string[] = [];
    simulationReports: string[] = [];
    inventoryReports: string[] = [];
    networks: Network[] = [];

    selectedKeyFields: string[] = [];
    selectedSimulationReports: string[] = [];
    selectedInventoryReports: string[] = [];
    primaryNetwork: Network = clone(emptyNetwork);

    keyFieldsDelimited: string = '';
    simulationReportsDelimited: string = '';
    inventoryReportsDelimited: string = '';

    keyFieldsName: string  = 'KeyFields';
    simulationReportsName: string = 'SimulationReports';
    inventoryReportsName: string = 'InventoryReports';

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectPrimaryNetworkItemValue = null;
            (async () => { 
                await vm.getAttributes();
                await vm.getNetworks();
                await vm.getSimulationReportsAction();
                await vm.getInventoryReportsAction();
                await vm.getPrimaryNetworkAction();
                await vm.getKeyFieldsAction();
            })();

            vm.primaryNetworks = [{...emptyNetwork, name: 'test 1', id: getNewGuid()}, 
                {...emptyNetwork, name: 'test 2', id: getNewGuid()},
                {...emptyNetwork, name: 'test 3', id: getNewGuid()}]

            vm.selectedKeyFields = ['test 1', 'test 2', 'test 3'];
            vm.selectedSimulationReports = ['test 1', 'test 2', 'test 3'];
            vm.selectedInventoryReports = ['test 1', 'test 2', 'test 3'];

            vm.keyFields = ['test 1', 'test 2', 'test 3'];
            vm.simulationReports = ['test 1', 'test 2', 'test 3'];
            vm.inventoryReports = ['test 1', 'test 2', 'test 3'];
        });
    }

    mounted() {
        const reports: string[] =  this.$config.reportType;
        this.simulationReports = clone(reports);
        this.inventoryReports = clone(reports);
    }

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

    @Watch('stateInventoryReportNames')
    onStateInventoryReportNamesChanged(){
        this.selectedInventoryReports = clone(this.stateInventoryReportNames);
    }

    @Watch('statePrimaryNetwork')
    onStatePrimaryNetworkChanged(){
        this.primaryNetwork = clone(this.statePrimaryNetwork)
    }

    @Watch('stateKeyFields')
    onStateKeyFieldsChanged(){
        this.selectedKeyFields = clone(this.stateKeyFields);
    }

    @Watch('networks')
    onNetworksChanged(){
        this.selectPrimaryNetworkItems = this.networks.map(_ => {
            return {text: _.name, value: _.id}
        });
    }

    @Watch('selectedKeyFields')
    onSelectedKeyFieldsChanged(){
        this.keyFieldsDelimited = '';
        for(let i = 0; i < this.selectedKeyFields.length; i++){
            this.keyFieldsDelimited += this.selectedKeyFields[i];
            if(i !== this.selectedKeyFields.length - 1){
                this.keyFieldsDelimited += ", ";
            }
        }
    }

    @Watch('selectedSimulationReports')
    onSelectedSimulationReportsChanged(){
        this.simulationReportsDelimited = '';
        for(let i = 0; i < this.selectedSimulationReports.length; i++){
            this.simulationReportsDelimited += this.selectedKeyFields[i];
            if(i !== this.selectedSimulationReports.length - 1){
                this.simulationReportsDelimited += ", ";
            }
        }
    }

    @Watch('selectedInventoryReports')
    onSelectedInventoryReportsChanged(){
        this.inventoryReportsDelimited = '';
        for(let i = 0; i < this.selectedInventoryReports.length; i++){
            this.inventoryReportsDelimited += this.selectedInventoryReports[i];
            if(i !== this.selectedInventoryReports.length - 1){
                this.inventoryReportsDelimited += ", ";
            }
        }
    }

    @Watch('primaryNetwork')
    onPrimaryNetworkChanged(){
        this.selectPrimaryNetworkItemValue = this.primaryNetwork.id;
    }

    onSaveClick(){

    }
    onEditKeyFieldsClick(){
        this.editAdminDataDialogData.showDialog = true;
        this.editAdminDataDialogData.selectedSettings = clone(this.selectedKeyFields);
        this.editAdminDataDialogData.settingName = this.keyFieldsName;
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
    onSubmitEditDataDialogResult(selectedSettings: string[]){
        if(selectedSettings !== null)
            switch(this.editAdminDataDialogData.settingName){
                case this.keyFieldsName:
                    this.selectedKeyFields = clone(selectedSettings);
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
}

</script>

<style>

</style>