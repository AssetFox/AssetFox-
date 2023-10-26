<template> 
     <div v-if="stateInventoryReportNames.length > 1" style="width: 250px; margin-left:700px">
        <v-select
            v-model="inventoryReportName" 
            :items="stateInventoryReportNames"
            variant="outlined"
            class="ghd-select ghd-text-field ghd-text-field-border">
        </v-select>
     </div>
    <v-layout>
        <v-row>
            <v-row justify-space-between row>
           </v-row>
            <v-row justify-space-between row>
                <v-row>
                    <div class="flex xs4" v-for="(key, index) in inventoryDetails">
                        <!-- <v-autocomplete :items="reactiveData[index]" @change="onSelectInventoryItem(index)"
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
                        </v-autocomplete> -->
                        <v-select
                        style="margin-top: 50px; width: 250px; margin-right: 30px"
                        class="ghd-select ghd-text-field ghd-text-field-border ghd-button-text"
                        :items="reactiveData[index]"
                        v-model="selectedInventoryIndex[index]"
                        :label="`Select by ${key}`"
                        outline
                        ></v-select>
                    </div>
                </v-row>
                <v-spacer></v-spacer>
                    <v-btn style="margin-right: 75px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                    variant ="outlined" 
                    @click="resetDropdowns()">
                    Reset Key Fields
                    </v-btn>
           </v-row>
            <v-divider></v-divider>
        </v-row>
    </v-layout>
</template>

<script lang="ts" setup>
    import Vue from 'vue';
    import {QueryResponse, InventoryParam, emptyInventoryParam, InventoryItem, KeyProperty} from '@/shared/models/iAM/inventory';
    import {clone, empty, find, forEach, propEq} from 'ramda';
    import InventoryService from '@/services/inventory.service'
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref, computed} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { coreAxiosInstance } from '@/shared/utils/axios-instance';

    let store = useStore();
    const emit = defineEmits(['submit'])
    const inventoryItems = computed<InventoryItem[]>(()=>store.state.inventoryModule.inventoryItems);
    const staticHTMLForInventory = computed<any>(()=>store.state.inventoryModule.staticHTMLForInventory);
    const querySet = computed<InventoryParam[]>(()=>store.state.inventoryModule.querySet);
    const stateKeyFields = computed<string[]>(()=>store.state.adminDataModule.keyFields);
    const stateRawDataKeyFields = computed<string[]>(()=>store.state.adminDataModule.rawDataKeyFields);
    const stateInventoryReportNames = computed<string[]>(()=>store.state.adminDataModule.inventoryReportNames);
    const stateConstraintType = computed<string>(()=>store.state.adminDataModule.constraintType);
    
    async function getInventoryAction(payload?: any): Promise<any> {await store.dispatch('getInventory',payload);}
    async function getStaticInventoryHTMLAction(payload?: any): Promise<any> {await store.dispatch('getStaticInventoryHTML',payload);}
    async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('setIsBusy',payload);}
    async function getInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('getInventoryReports',payload);}
    async function getKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields', payload);}
    async function getConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('getConstraintType',payload);}
    async function getQueryAction(payload?: any): Promise<any> {await store.dispatch('getQuery',payload);}
    async function getRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getRawDataKeyFields',payload);}
    async function getKeyPropertiesAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields',payload);}
        
    
    const keyAttributeValues = ref<string[][]>([]);
    const reactiveData: Ref<[string[], string[]]> = ref([[], []]);
    let inventoryItem: any[][] = [];

    const selectedKeys: string[] = [];

    const selectedInventoryIndex = ref([]);

    const inventoryDetails = ref<string[]>([]);
    let constraintDetails: string = '';
    let lastThreeLetters: string = '';
    let reportType: string = '';

    let queryValue: any;
    let querySelectedData: string[] = [];

    let inventorySelectListsWorker: any = null;

    let inventoryData: any  = null;
    let sanitizedHTML: any = null;
    let inventoryReportName: string = '';


    const InventoryStateKeys = computed<InventoryItem[]>(()=> store.state.inventoryModule.inventoryItems)

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        watch(inventoryItems,async ()=>{
            keyAttributeValues.value = await setupSelectLists();
        });

        watch(selectedInventoryIndex, (newValues, oldValues) => {
            let index: number;
            if(newValues.length < 5){
                index = reactiveData.value[1].indexOf(newValues[0]);
            }
            else (newValues.length < 5)
            {
                index = reactiveData.value[0].indexOf(newValues[0]);
            }
        onSelectInventoryItem(index);
        }, { deep: true });

        watch(InventoryStateKeys, ()=>{
            InventoryStateKeys.value.forEach(element => {
                keyAttributeValues.value.push(element.keyProperties)
            });

            for(let i = 0; i < keyAttributeValues.value.length; i++)
            {
                let j = 0;
                reactiveData.value[1].push(keyAttributeValues.value[i][j])
            }

            for(let i = 0; i < keyAttributeValues.value.length; i++)
            {
                let j = 1;
                reactiveData.value[0].push(keyAttributeValues.value[i][j])
            }

/*                 let index = 1;
                onSelectInventoryItem(index);
 */ 

        })

        watch(staticHTMLForInventory,()=>onStaticHTMLForInventory())
        function onStaticHTMLForInventory(){
           // sanitizedHTML = $sanitize(staticHTMLForInventory.value);
        }

        watch(stateInventoryReportNames,()=>{
            if(stateInventoryReportNames.value.length > 0)
                inventoryReportName = stateInventoryReportNames.value[0]
            
            lastThreeLetters = inventoryReportName.slice(-3);
            reportType = lastThreeLetters[1];
        });
        
        watch(stateKeyFields,()=>{
            if(reportType === 'P') {
                inventoryDetails.value = clone(stateKeyFields.value);
                inventoryDetails.value.forEach(_ => selectedKeys.push(""));

                getInventoryAction(inventoryDetails);
            }
        });

        watch(stateRawDataKeyFields,()=>{
            if(reportType === 'R') {
                inventoryDetails.value = clone(stateRawDataKeyFields.value);

                inventoryDetails.value.forEach(_ => selectedKeys.push(""));
                getInventoryAction([inventoryDetails.value[0]]);
            } 
        });

        watch(stateConstraintType,()=>{
            constraintDetails = stateConstraintType.value;
        });

        watch(querySet,()=>{
                keyAttributeValues.value = [];
                for(let i = 0; i < inventoryDetails.value.length; i++) 
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
                    keyAttributeValues.value[i] = queryValue;
                }
        });
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
                await getConstraintTypeAction();
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
                inventoryDetails: inventoryDetails.value
            };
            let toReturn: string[][] = [];
            var keyProperties = data.inventoryDetails;
             const response = await InventoryService.getInventory(keyProperties);
               keyAttributeValues.value = response.data;
               response.data.forEach((element: string[]) => {
                    toReturn.push(element);
                });

            return toReturn; 
                  
        }
        
        async function initializeLists() {
            const keys: any[][] = [];

            const response = await coreAxiosInstance.get("/api/Inventory/GetKeyProperties");
            inventoryDetails.value = response.data;

            getInventoryAction(inventoryDetails.value);
            //setupSelectLists();


/*             console.log(inventoryDetails);
            if (inventoryItems.value && inventoryItems.value.length > 0) {
                const data = inventoryItems.value;
                
                for (let i = 0; i < inventoryDetails.length; i++) {
                    keys.push([]);
                    keys[i].push({ header: `${inventoryDetails[i]}'s` });
                }

                for (const item of data) {
                    for (let i = 0; i < inventoryDetails.length; i++) {
                        keys[i].push({
                            identifier: item.keyProperties[i],
                            group: inventoryDetails[i],
                        });
                    }
                }
            }
            return { keys };
 */        }

        async function resetDropdowns() {
            keyAttributeValues.value = [];
            //Reset selected key fields
            selectedKeys.forEach((value: string, index: number, array: string[]) => {
                selectedKeys[index] = '';
            })
            selectedKeys[0] = "";
            keyAttributeValues.value = await setupSelectLists();
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
                for(let i = 0; i < inventoryDetails.value.length; i++) {
                    if(selectedKeys[i] !== '') {
                        selectedCounter++;
                    }
                }
                
                if(selectedCounter === inventoryDetails.value.length)
                {
                    HandleSelectedItems(index);
                 }
            }       
        }

        function HandleSelectedItems(index: number) {
            selectedKeys[index] = "2";
            let key = selectedKeys[index];
                let data: InventoryParam = {
                keyProperties: {},
                values: function (values: any): unknown {
                throw new Error('Function not implemented.');
                }
                };

                for(let i = 0; i < inventoryDetails.value.length; i++){
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

                for(let i = 0; i < inventoryDetails.value.length; i++)
                {
                    let temp = inventoryDetails.value[1];
                    inventoryDetails.value[1] = inventoryDetails.value[0];
                    inventoryDetails.value[0] = temp;
                }

                for(let i = 0; i < inventoryDetails.value.length; i++) {
                    let dictNames: any = inventoryDetails.value[i];
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
                for(let i = 0; i < inventoryDetails.value.length - 1; i++) 
                    {
                        if(selectedKeys[i] !== '') 
                        {
                            if(!querySelectedData.includes(inventoryDetails.value[i]) && !querySelectedData.includes(selectedKeys[i]))
                            {
                                querySelectedData.push(inventoryDetails.value[i]);
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
