<template>  
    <v-layout>
        <v-flex xs12>
            <v-layout justify-space-between row>
                <v-subheader v-if="stateInventoryReportNames.length > 1" class="ghd-select ghd-text-field ghd-text-field-border">
                    <v-select
                        v-model="inventoryReportName" 
                        :items="stateInventoryReportNames"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                </v-subheader>
           </v-layout>
            <v-layout justify-space-between row>
                <v-spacer></v-spacer>
                <v-layout>
                    <div class="flex xs4" v-for="(key, index) in inventoryDetails">
                        <v-autocomplete :items="keyAttributeValues[index]" @change="onSelectInventoryItem(index)" item-text="identifier" item-value="identifier"
                                        :label="`Select by ${key} Key`" outline
                                        v-model="selectedKeys[index]"
                                        :disabled = "isDisabled(index)">
                            <template slot="item" slot-scope="data">
                                <template v-if="typeof data.item !== 'object'">
                                    <v-list-tile-content v-text="data.item"></v-list-tile-content>
                                </template>
                                <template v-else>
                                    <v-list-tile-content>
                                        <v-list-tile-title v-html="data.item.identifier"></v-list-tile-title>
                                    </v-list-tile-content>
                                </template>
                            </template>
                        </v-autocomplete>
                    </div>
                </v-layout>
                <v-spacer></v-spacer>
                    <v-btn style="padding-top: 15px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                    outline 
                    @click="resetDropdowns()">
                    Reset Key Fields
                    </v-btn>
           </v-layout>
            <v-divider></v-divider>
            <div class="container" v-html="sanitizedHTML"></div>
        </v-flex>
    </v-layout>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Watch} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import {QueryResponse, InventoryParam, emptyInventoryParam, InventoryItem, KeyProperty} from '@/shared/models/iAM/inventory';
    import {clone, empty, find, forEach, propEq} from 'ramda';
    import InventoryService from '@/services/inventory.service'

    @Component
    export default class Inventory extends Vue {
        @State(state => state.inventoryModule.inventoryItems) inventoryItems: InventoryItem[];
        @State(state => state.inventoryModule.staticHTMLForInventory) staticHTMLForInventory: any;
        @State(state => state.inventoryModule.querySet) querySet: InventoryParam[];
        @State(state => state.adminDataModule.keyFields) stateKeyFields: string[];
        @State(state => state.adminDataModule.rawDataKeyFields) stateRawDataKeyFields: string[];
        @State(state => state.adminDataModule.inventoryReportNames) stateInventoryReportNames: string[];
        @State(state => state.adminDataModule.constraintType) stateConstraintType: string;


        @Action('getInventory') getInventoryAction: any;
        @Action('getStaticInventoryHTML') getStaticInventoryHTMLAction: any; 
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInventoryReports') getInventoryReportsAction: any;
        @Action('getKeyFields') getKeyFieldsAction: any;
        @Action('getConstraintType') getConstraintTypeAction: any;
        @Action('getQuery') getQueryAction: any;
        @Action('getRawDataKeyFields') getRawDataKeyFieldsAction: any;

        keyAttributeValues: string[][] = [];

        inventoryItem: any[][] = [];

        selectedKeys: string[] = [];

        inventoryDetails: string[] = [];
        constraintDetails: string = '';
        lastThreeLetters: string = '';
        reportType: string = '';

        queryValue: any;
        querySelectedData: string[] = [];

        inventorySelectListsWorker: any = null;

        inventoryData: any  = null;
        sanitizedHTML: any = null;
  
        inventoryReportName: string = '';

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('inventoryItems')
        async onInventoryItemsChanged() {
            this.keyAttributeValues = await this.setupSelectLists();
        }

        @Watch('staticHTMLForInventory')
        onStaticHTMLForInventory(){
            this.sanitizedHTML = this.$sanitize(this.staticHTMLForInventory);
        }
        
        @Watch('stateKeyFields')
        onStateKeyFieldsChanged(){
            if(this.reportType === 'P') {
                this.inventoryDetails = clone(this.stateKeyFields);
                this.inventoryDetails.forEach(_ => this.selectedKeys.push(""));

                this.getInventoryAction(this.inventoryDetails);
            }
        }

        @Watch('stateRawDataKeyFields')
        onStateRawKeyFieldsChanged(){
            if(this.reportType === 'R') {
                this.inventoryDetails = clone(this.stateRawDataKeyFields);

                this.inventoryDetails.forEach(_ => this.selectedKeys.push(""));
                this.getInventoryAction([this.inventoryDetails[0]]);
            } 

        }

        @Watch('stateInventoryReportNames')
        onStateInventoryReportNamesChanged(){
            if(this.stateInventoryReportNames.length > 0)
                this.inventoryReportName = this.stateInventoryReportNames[0]
            
            this.lastThreeLetters = this.inventoryReportName.slice(-3);
            this.reportType = this.lastThreeLetters[1];
        }

        @Watch('stateConstraintType')
        onStateConstraintTypeChanged(){
            this.constraintDetails = this.stateConstraintType;
        }

        @Watch('querySet')
        onQuerySetChanged(){
                this.keyAttributeValues = [];
                for(let i = 0; i < this.inventoryDetails.length; i++) 
                {
                    this.queryValue = this.querySet[i].values;
                    this.queryValue.sort((a: string | number, b: string | number) => 
                    {
                        if (typeof a === "number" && typeof b === "number") {
                            return a - b; // Sort numbers in descending order
                        } else if (typeof a === "string" && typeof b === "string") {
                            return a.localeCompare(b); // Sort strings in alphabetical order
                        } else {
                            return 0; // Preserve the original order if the data types are different
                        }
                    });
                    this.keyAttributeValues[i] = this.queryValue;
                }
        }

        /**
         * Vue component has been mounted
         */
        mounted() {
            (async () => { 
                await this.getConstraintTypeAction();
                await this.getInventoryReportsAction();
                await this.getKeyFieldsAction(); 
                await this.getRawDataKeyFieldsAction();
                this.onStateConstraintTypeChanged();
            })();
        }

        created() {
            this.initializeLists();
        }

        async setupSelectLists() {
            this.querySelectedData = [];
            const data: any = {
                inventoryItems: this.inventoryItems,
                inventoryDetails: this.inventoryDetails
            };
            let toReturn: string[][] = [];
            let result = await this.inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])  
            if(result.keys.length > 0 && this.reportType === 'P'){
                for(let i = 0; i < this.inventoryDetails.length; i++){
                    toReturn[i] = clone(result.keys[i]);
                }
            }
            else{
                for(let i = 0; i < 1; i++){
                    toReturn[i] = clone(result.keys[i]);
                }
            }
            return toReturn;                  
        }

        initializeLists() {
            if(this.reportType === 'P') {

            this.inventorySelectListsWorker = this.$worker.create(
                [
                    {
                        message: 'setInventorySelectLists', func: (data: any) => {
                            if (data) {
                                
                                const inventoryItems = data.inventoryItems;

                                const keys: any[][] = []
                                inventoryItems.forEach((item: InventoryItem, index: number) => {
                                    if (index === 0) { 
                                        for(let i = 0; i < data.inventoryDetails.length; i++){
                                            keys.push([])
                                            keys[i].push({header: `${data.inventoryDetails[i]}'s`})
                                        }
                                    }                           
                                    
                                    for(let i = 0; i < data.inventoryDetails.length; i++){
                                        keys[i].push({
                                            identifier: item.keyProperties[i],
                                            group: data.inventoryDetails[i]
                                        })
                                    }
                                });
                           
                                return {keys: keys};
                            }
                            return  {keys: []};
                        }
                    }
                ]
            );
        }
        else
        this.initializeRawLists();
    }


        initializeRawLists() {
            this.inventorySelectListsWorker = this.$worker.create(
                [
                    {
                        message: 'setInventorySelectLists', func: (data: any) => {
                            if (data) {
                                
                                const inventoryItems = data.inventoryItems;

                                const keys: any[][] = []
                                inventoryItems.forEach((item: InventoryItem, index: number) => {
                                    if (index === 0) { 
                                        for(let i = 0; i < 1; i++){
                                            keys.push([])
                                            keys[i].push({header: `${data.inventoryDetails[i]}'s`})
                                        }
                                    }                           
                                    
                                    for(let i = 0; i < 1; i++){
                                        keys[i].push({
                                            identifier: item.keyProperties[i],
                                            group: data.inventoryDetails[i]
                                        })
                                    }
                                });
                           
                                return {keys: keys};
                            }
                            return  {keys: []};
                        }
                    }
                ]
            );
        }


        async resetDropdowns() {
            this.keyAttributeValues = [];
            //Reset selected key fields
            this.selectedKeys.forEach((value: string, index: number, array: string[]) => {
                this.selectedKeys[index] = '';
            })
            this.selectedKeys[0] = "";
            this.keyAttributeValues = await this.setupSelectLists();
        }

        onSelectInventoryItem(index: number){
            if(this.constraintDetails == 'OR')
            {
                this.HandleSelectedItems(index);
            }
            else if(this.constraintDetails == 'AND') {
                let selectedCounter = 0;
                //Get the first user selected key field and it's selection and put it in a dictionary
                this.QueryAccess();

                //Check if any dropdowns are empty
                for(let i = 0; i < this.inventoryDetails.length; i++) {
                    if(this.selectedKeys[i] !== '') {
                        selectedCounter++;
                    }
                }
                
                if(selectedCounter === this.inventoryDetails.length)
                {
                    this.HandleSelectedItems(index);
                 }
            }       
        }

        HandleSelectedItems(index: number) {
            let key = this.selectedKeys[index];
                let data: InventoryParam = {
                keyProperties: {},
                values: function (values: any): unknown {
                throw new Error('Function not implemented.');
                }
                };

                for(let i = 0; i < this.inventoryDetails.length; i++){
                    if(i === index){
                        data.keyProperties[i] = key;
                        continue;
                    }
                    if(this.constraintDetails == 'OR'){
                    let inventoryItem = this.inventoryItems.filter(function(item: { keyProperties: string | any[]; }){if(item.keyProperties.indexOf(key) !== -1) return item;})[0];
                    let otherKeyValue = inventoryItem.keyProperties[i]; 
                    this.selectedKeys[i] = otherKeyValue;
                    data.keyProperties[i] = otherKeyValue;
                    }
                }
                 //Create a dictionary of the selected key fields
                let dictionary: Record<string, string> = {};

                for(let i = 0; i < this.inventoryDetails.length; i++) {
                    let dictNames: any = this.inventoryDetails[i];
                    let dictValues: any = this.selectedKeys[i];
                    dictionary[dictNames] = dictValues;                     
                }

                //Set the data equal to the dictionary
                data.keyProperties = dictionary;

                this.getStaticInventoryHTMLAction({reportType: this.inventoryReportName, filterData: data.keyProperties}); 
        }

        QueryAccess() {
             //Get the first user selected key field and it's selection and put it in a dictionary

            if(this.querySelectedData.length === 0 || this.querySelectedData.length === 2) 
            {
                for(let i = 0; i < this.inventoryDetails.length - 1; i++) 
                    {
                        if(this.selectedKeys[i] !== '') 
                        {
                            if(!this.querySelectedData.includes(this.inventoryDetails[i]) && !this.querySelectedData.includes(this.selectedKeys[i]))
                            {
                                this.querySelectedData.push(this.inventoryDetails[i]);
                                this.querySelectedData.push(this.selectedKeys[i]);
                            }
                        }
                    }
                    //Send to backend to recieve dropdown lists for the other key fields                     
                    this.getQueryAction({querySet: this.querySelectedData});           
            }
        }

        isDisabled(index: number) {
            if(this.querySelectedData.length < index * 2 && this.constraintDetails == "AND") {
                return true;
            }
            return false;
        }
    }

</script>

<style>
    @import "../assets/css/inventory.css"
</style>
