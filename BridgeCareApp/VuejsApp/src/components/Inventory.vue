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
                        <v-autocomplete :items="keyAttributeValues[index]" @change="onSelectInventoryItem(index)" item-title="identifier" item-value="identifier"
                                        :label="`Select by ${key} Key`" outline
                                        v-model="selectedKeys[index]"
                                        :disabled = "isDisabled(index)">
                            <template v-slot:item="data" slot="item" slot-scope="data">
                                <template v-if="typeof data.item !== 'object'">
                                    <v-list-tile-content v-text="data.item"></v-list-tile-content>
                                </template>
                                <template v-else>
                                    <v-list-tile-content>
                                        <v-list-tile-title v-html="data.item.value"></v-list-tile-title>
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

<script lang="ts" setup>
    import Vue from 'vue';
    import {QueryResponse, InventoryParam, emptyInventoryParam, InventoryItem, KeyProperty} from '@/shared/models/iAM/inventory';
    import {clone, empty, find, forEach, propEq} from 'ramda';
    import InventoryService from '@/services/inventory.service'
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';

    let store = useStore();
    const emit = defineEmits(['submit'])
    let inventoryItems = ref<InventoryItem[]>(store.state.inventoryModule.inventoryItems);
    let staticHTMLForInventory = ref<any>(store.state.inventoryModule.staticHTMLForInventory);
    let querySet = ref<InventoryParam[]>(store.state.inventoryModule.querySet);
    let stateKeyFields = ref<string[]>(store.state.adminDataModule.keyFields);
    let stateRawDataKeyFields = ref<string[]>(store.state.adminDataModule.rawDataKeyFields);
    let stateInventoryReportNames = ref<string[]>(store.state.adminDataModule.inventoryReportNames);
    let stateConstraintType = ref<string>(store.state.adminDataModule.constraintType);
    
    async function getInventoryAction(payload?: any): Promise<any> {await store.dispatch('getInventory');}
    async function getStaticInventoryHTMLAction(payload?: any): Promise<any> {await store.dispatch('getStaticInventoryHTML');}
    async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('setIsBusy');}
    async function getInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('getInventoryReports');}
    async function getKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields');}
    async function getConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('getConstraintType');}
    async function getQueryAction(payload?: any): Promise<any> {await store.dispatch('getQuery');}
    async function getRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getRawDataKeyFields');}
        
let keyAttributeValues: string[][] = [];

let inventoryItem: any[][] = [];

let selectedKeys: string[] = [];

let inventoryDetails: string[] = [];
let constraintDetails: string = '';
let lastThreeLetters: string = '';
let reportType: string = '';

let queryValue: any;
let querySelectedData: string[] = [];

let inventorySelectListsWorker: any = null;

let inventoryData: any  = null;
let sanitizedHTML: any = null;
const $sanitize = inject('$sanitize') as any
const $worker = inject('$worker') as any
let inventoryReportName: string = '';

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        watch(inventoryItems,()=>onInventoryItemsChanged())
        async function onInventoryItemsChanged() {
            keyAttributeValues = await setupSelectLists();
        }

        watch(staticHTMLForInventory,()=>onStaticHTMLForInventory())
        function onStaticHTMLForInventory(){
            sanitizedHTML = $sanitize(staticHTMLForInventory.value);
        }
        
        watch(stateKeyFields,()=>onStateKeyFieldsChanged())
        function onStateKeyFieldsChanged(){
            if(reportType === 'P') {
                inventoryDetails = clone(stateKeyFields.value);
                inventoryDetails.forEach(_ => selectedKeys.push(""));

                getInventoryAction(inventoryDetails);
            }
        }

        watch(stateRawDataKeyFields,()=>onStateRawKeyFieldsChanged())
        function onStateRawKeyFieldsChanged(){
            if(reportType === 'R') {
                inventoryDetails = clone(stateRawDataKeyFields.value);

                inventoryDetails.forEach(_ => selectedKeys.push(""));
                getInventoryAction([inventoryDetails[0]]);
            } 

        }

        watch(stateInventoryReportNames,()=>onStateInventoryReportNamesChanged())
        function onStateInventoryReportNamesChanged(){
            if(stateInventoryReportNames.value.length > 0)
                inventoryReportName = stateInventoryReportNames.value[0]
            
            lastThreeLetters = inventoryReportName.slice(-3);
            reportType = lastThreeLetters[1];
        }

        watch(stateConstraintType,()=> onStateConstraintTypeChanged())
        function onStateConstraintTypeChanged(){
            constraintDetails = stateConstraintType.value;
        }

        watch(querySet,()=> onQuerySetChanged())
        function onQuerySetChanged(){
                keyAttributeValues = [];
                for(let i = 0; i < inventoryDetails.length; i++) 
                {
                    queryValue = querySet.value[i].values;
                    queryValue.sort((a: string | number, b: string | number) => 
                    {
                        if (typeof a === "number" && typeof b === "number") {
                            return a - b; // Sort numbers in descending order
                        } else if (typeof a === "string" && typeof b === "string") {
                            return a.localeCompare(b); // Sort strings in alphabetical order
                        } else {
                            return 0; // Preserve the original order if the data types are different
                        }
                    });
                    keyAttributeValues[i] = queryValue;
                }
        }
        /**
         * Vue component has been mounted
         */
        onMounted(()=>mounted())
         function mounted() {
            (async () => { 
                await getConstraintTypeAction();
                await getInventoryReportsAction();
                await getKeyFieldsAction(); 
                await getRawDataKeyFieldsAction();
                onStateConstraintTypeChanged();
            })();
        }
        created();
        function created() {
            initializeLists();
        }

        async function setupSelectLists() {
            querySelectedData = [];
            const data: any = {
                inventoryItems: inventoryItems,
                inventoryDetails: inventoryDetails
            };
            let toReturn: string[][] = [];
            let result = await inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])  
            if(result.keys.length > 0){
                for(let i = 0; i < inventoryDetails.length; i++){
                    toReturn[i] = clone(result.keys[i]);
                }
            }
            return toReturn;                  
        }

        function initializeLists() {
            inventorySelectListsWorker = $worker.create(
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

        async function resetDropdowns() {
            keyAttributeValues = [];
            //Reset selected key fields
            selectedKeys.forEach((value: string, index: number, array: string[]) => {
                selectedKeys[index] = '';
            })
            selectedKeys[0] = "";
            keyAttributeValues = await setupSelectLists();
        }

        function onSelectInventoryItem(index: number){
            if(constraintDetails == 'OR')
            {
                HandleSelectedItems(index);
            }
            else if(constraintDetails == 'AND') {
                let selectedCounter = 0;
                //Get the first user selected key field and it's selection and put it in a dictionary
                QueryAccess();

                //Check if any dropdowns are empty
                for(let i = 0; i < inventoryDetails.length; i++) {
                    if(selectedKeys[i] !== '') {
                        selectedCounter++;
                    }
                }
                
                if(selectedCounter === inventoryDetails.length)
                {
                    HandleSelectedItems(index);
                 }
            }       
        }

        function HandleSelectedItems(index: number) {
            let key = selectedKeys[index];
                let data: InventoryParam = {
                keyProperties: {},
                values: function (values: any): unknown {
                throw new Error('Function not implemented.');
                }
                };

                for(let i = 0; i < inventoryDetails.length; i++){
                    if(i === index){
                        data.keyProperties[i] = key;
                        continue;
                    }
                    let inventoryItem = inventoryItems.value.filter(function(item: { keyProperties: string | any[]; }){if(item.keyProperties.indexOf(key) !== -1) return item;})[0]; 
                    let otherKeyValue = inventoryItem.keyProperties[i]; 
                    selectedKeys[i] = otherKeyValue;
                    data.keyProperties[i] = otherKeyValue;
                }

                 //Create a dictionary of the selected key fields
                let dictionary: Record<string, string> = {};

                for(let i = 0; i < inventoryDetails.length; i++) {
                    let dictNames: any = inventoryDetails[i];
                    let dictValues: any = selectedKeys[i];
                    dictionary[dictNames] = dictValues;                     
                }
    
                //Set the data equal to the dictionary
                data.keyProperties = dictionary;

                getStaticInventoryHTMLAction({reportType: inventoryReportName, filterData: data.keyProperties}); 
        }

        function QueryAccess() {
             //Get the first user selected key field and it's selection and put it in a dictionary

            if(querySelectedData.length === 0 || querySelectedData.length === 2) 
            {
                for(let i = 0; i < inventoryDetails.length - 1; i++) 
                    {
                        if(selectedKeys[i] !== '') 
                        {
                            if(!querySelectedData.includes(inventoryDetails[i]) && !querySelectedData.includes(selectedKeys[i]))
                            {
                                querySelectedData.push(inventoryDetails[i]);
                                querySelectedData.push(selectedKeys[i]);
                            }
                        }
                    }
                    //Send to back end to recieve dropdown lists for the other key fields                     
                    getQueryAction({querySet: querySelectedData});           
            }
        }

        function isDisabled(index: number) {
            if(querySelectedData.length < index * 2 && constraintDetails == "AND") {
                return true;
            }
            return false;
        }
</script>

<style>
    @import "../assets/css/inventory.css"
</style>
