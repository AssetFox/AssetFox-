<template>
    <v-layout>
        <v-flex xs12>
            <v-layout justify-space-between row>
                <div class="flex xs2 justify-content: end">
                        <button class="ghd-outline-button-padding ghd-button"  
                        style="border: 1px solid black; 
                        border-radius: 4px;
                        padding: 2px" @click="resetVals()">Reset Key Fields</button>
                </div>
                <v-spacer></v-spacer>
                <v-layout>
                    <div class="flex xs4" v-for="(key, index) in inventoryDetails">
                        <v-autocomplete :items="keyAttirbuteValues[index]" @change="onSelectInventoryItem(index)" item-text="identifier" item-value="identifier"
                                        :label="`Select by ${key} Key`" outline
                                        v-model="selectedKeys[index]">
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
                    <div v-if="stateInventoryReportNames.length > 1" class="flex xs2 justify-content: end">
                        <v-select 
                            v-model="inventoryReportName" 
                            :items="stateInventoryReportNames">
                        </v-select>
                    </div>
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
        @State(state => state.inventoryModule.querySet) querySet: string[];
        @State(state => state.adminDataModule.keyFields) stateKeyFields: string[];
        @State(state => state.adminDataModule.inventoryReportNames) stateInventoryReportNames: string[];
        @State(state => state.adminDataModule.constraintType) stateConstraintType: string;


        @Action('getInventory') getInventoryAction: any;
        @Action('getStaticInventoryHTML') getStaticInventoryHTMLAction: any; 
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('getInventoryReports') getInventoryReportsAction: any;
        @Action('getKeyFields') getKeyFieldsAction: any;
        @Action('getConstraintType') getConstraintTypeAction: any;
        @Action('getQuery') getQueryAction: any;

        keyAttirbuteValues: string[][] = [];

        inventoryItem: any[][] = [];

        selectedKeys: string[] = [];

        inventoryDetails: string[] = [];
        constraintDetails: string = '';

        resetCounter = 0;

        inventorySelectListsWorker: any = null;

        inventoryData: any  = null;
        sanitizedHTML: any = null;
  
        inventoryReportName: string = '';
        DictionaryString : any = [];

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('inventoryItems')
        onInventoryItemsChanged() {
            this.setupSelectLists();
        }

        @Watch('staticHTMLForInventory')
        onStaticHTMLForInventory(){
            this.sanitizedHTML = this.$sanitize(this.staticHTMLForInventory);
        }
        
        @Watch('stateKeyFields')
        onStateKeyFieldsChanged(){
            this.inventoryDetails = clone(this.stateKeyFields);
            this.inventoryDetails.forEach(_ => this.selectedKeys.push(""));
            this.getInventoryAction(this.inventoryDetails);
        }

        @Watch('stateInventoryReportNames')
        onStateInventoryReportNamesChanged(){
            if(this.stateInventoryReportNames.length > 0)
                this.inventoryReportName = this.stateInventoryReportNames[0]
        }

        @Watch('stateConstraintType')
        onStateConstraintTypeChanged(){
            this.constraintDetails = this.stateConstraintType;
        }

        @Watch('querySet')
        onQuerySetChanged(){
            //console.log(this.querySet[0].values[0]);
            //this.querySet.forEach(data => {
            //})
        }

        /**
         * Vue component has been mounted
         */
        mounted() {
            (async () => { 
                await this.getConstraintTypeAction();
                await this.getInventoryReportsAction();
                await this.getKeyFieldsAction(); 
                //await this.getQueryAction();
                this.onStateConstraintTypeChanged();
            })();
        }

        created() {
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

        setupSelectLists() {
            const data: any = {
                inventoryItems: this.inventoryItems,
                inventoryDetails: this.inventoryDetails
            };
            this.inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])
                .then((result: any) => {
                    if(result.keys.length > 0){
                        this.bmsIdsSelectList = result.keys[0];
                        this.brKeysSelectList = result.keys[1];
                        for(let i = 0; i < this.inventoryDetails.length; i++){
                            this.keyAttirbuteValues[i] = result.keys[i];
                        }
                    }  
                });
        }

        resetVals() {
            this.selectedKeys.forEach(item => {
                item = '';
                console.log(item);
            })
        }

        onSelectInventoryItem(index: number){
            let selectedCounter = 0;
            if(this.constraintDetails == 'OR')
            {
                let key = this.selectedKeys[index];
                let data: InventoryParam = {keyProperties: {}};

                for(let i = 0; i < this.inventoryDetails.length; i++){
                    if(i === index){
                        data.keyProperties[i] = key;
                        continue;
                    }
                    let inventoryItem = this.inventoryItems.filter(function(item: { keyProperties: string | any[]; }){if(item.keyProperties.indexOf(key) !== -1) return item;})[0]; 
                    let otherKeyValue = inventoryItem.keyProperties[i]; 
                    this.selectedKeys[i] = otherKeyValue;
                    data.keyProperties[i] = otherKeyValue;
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

                this.getStaticInventoryHTMLAction({reportType: this.inventoryReportName, filterData: data});  
            }
            else if(this.constraintDetails == 'AND') {
                //Get the first use selected key field and it's selection and put it in a dictionary

                let queryDict: string[] = [];
                let queryData: string[] = [];
                let queryFirst: string[] = [];

                for(let i = 0; i < this.inventoryDetails.length && Object.keys(queryDict).length < 1; i++) 
                {
                    if(this.selectedKeys[i] !== '' && Object.keys(queryDict).length < 1) 
                    {
                        let dictNames: any = this.inventoryDetails[i];
                        let dictValues: any = this.selectedKeys[i];

                        queryFirst = [this.inventoryDetails[i], this.selectedKeys[i]];
                        queryDict[dictNames] = dictValues;                     
                        i++;
                    }  
                }              
                queryData = queryFirst;
                this.getQueryAction({querySet: queryData});  

                //Check if any dropdowns are empty
                for(let i = 0; i < this.inventoryDetails.length; i++) {
                    if(this.selectedKeys[i] !== '') {
                        selectedCounter++;
                    }
                }
                
                if(selectedCounter === this.inventoryDetails.length)
                {
                    let key = this.selectedKeys[index];
                    let data: InventoryParam = {keyProperties: {}};

                    for(let i = 0; i < this.inventoryDetails.length; i++){
                        if(i === index){
                            data.keyProperties[i] = key;
                            continue;
                        }
                        let inventoryItem = this.inventoryItems.filter(function(item: { keyProperties: any | any[]; }){if(item.keyProperties.indexOf(key) !== -1) return item;})[0]; 
                        let otherKeyValue = inventoryItem.keyProperties[i]; 
                        this.selectedKeys[i] = otherKeyValue;
                        data.keyProperties[i] = otherKeyValue;
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
                    this.getStaticInventoryHTMLAction({reportType: this.inventoryReportName, filterData: data});
                 }
            }       
        }    }
</script>

<style>
    @import "../assets/css/inventory.css"
</style>
