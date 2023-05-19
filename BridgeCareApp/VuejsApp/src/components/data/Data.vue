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
        <EditDataDialog :DialogData='editDataDialogData'
                                     @submit='onSubmitEditDataDialogResult' />
    </v-layout>
</template>

<script lang='ts'>

import { SelectItem } from '@/shared/models/vue/select-item';
import Vue from 'vue';
import Component from 'vue-class-component';
import EditDataDialog from './EditDataDialog.vue';
import {EditDataDialogData, emptyEditDataDialogData} from '@/shared/models/modals/edit-data-dialog-data';
import { clone } from 'ramda';
import { watch } from 'fs';
import { Watch } from 'vue-property-decorator';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { getNewGuid } from '@/shared/utils/uuid-utils';


@Component({
    components:{
        EditDataDialog
    }  
})
export default class Data extends Vue {
    primaryNetworks: Network[] = [];

    selectPrimaryNetworkItems: SelectItem[] = [];
    selectPrimaryNetworkItemValue: string = '';

    editDataDialogData: EditDataDialogData = clone(emptyEditDataDialogData)

    keyFields: string[] = [];
    simulationReports: string[] = [];
    inventoryReports: string[] = [];

    selectedKeyFields: string[] = [];
    selectedSimulationReports: string[] = [];
    selectedInventoryReports: string[] = [];

    keyFieldsDelimited: string = '';
    simulationReportsDelimited: string = '';
    inventoryReportsDelimited: string = '';

    keyFieldsName: string  = 'KeyFields';
    simulationReportsName: string = 'SimulationReports';
    inventoryReportsName: string = 'InventoryReports';

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectPrimaryNetworkItemValue = null;

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

    @Watch('primaryNetworks')
    onPrimaryNetworksChanged(){
        this.selectPrimaryNetworkItems = this.primaryNetworks.map(_ => {
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

    onSaveClick(){

    }
    onEditKeyFieldsClick(){
        this.editDataDialogData.showDialog = true;
        this.editDataDialogData.selectedSettings = clone(this.selectedKeyFields);
        this.editDataDialogData.settingName = this.keyFieldsName;
        this.editDataDialogData.settingsList = clone(this.keyFields);
    }
    onEditInventoryReportsClick(){
        this.editDataDialogData.showDialog = true;
        this.editDataDialogData.selectedSettings = clone(this.selectedInventoryReports);
        this.editDataDialogData.settingName = this.inventoryReportsName;
        this.editDataDialogData.settingsList = clone(this.inventoryReports);
    }
    onEditSimulationReportsClick(){
        this.editDataDialogData.showDialog = true;
        this.editDataDialogData.selectedSettings = clone(this.selectedSimulationReports);
        this.editDataDialogData.settingName = this.simulationReportsName;
        this.editDataDialogData.settingsList = clone(this.simulationReports);
    }
    onSubmitEditDataDialogResult(selectedSettings: string[]){
        if(selectedSettings !== null)
            switch(this.editDataDialogData.settingName){
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

        this.editDataDialogData = clone(emptyEditDataDialogData);
    }
}

</script>

<style>

</style>