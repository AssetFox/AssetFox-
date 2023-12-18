<template> 
     <div v-if="stateInventoryReportNames.length > 1" style="width: 300px; margin-left:650px">
        <v-autocomplete
            v-model="inventoryReportName" 
            :items="stateInventoryReportNames"
            variant="outlined"
            density="compact"
            class="ghd-select ghd-text-field ghd-text-field-border">
        </v-autocomplete>
     </div>
    <v-layout>
        <v-row>
            <v-row justify="space-between"></v-row>
            <v-row justify="space-between">
                <v-row style="display: flex; align-items: center; justify-content: center">
                    <div class="flex xs4" v-for="(key, index) in inventoryDetails">
                        <v-autocomplete
                        style="margin-top: 50px; width: 250px; margin-right: 20px"
                        class="ghd-select ghd-text-field ghd-text-field-border ghd-button-text"
                        :items="reactiveData[index]"
                        v-model="selectedInventoryIndex[index]"
                        :label="`Select by ${key}`"
                        :disabled="isDisabled(index)"
                        variant="outlined"
                        density="compact"
                        ></v-autocomplete>
                    </div>
                </v-row>
                <v-spacer></v-spacer>
                    <v-btn style="margin-right: 75px; margin-top: 40px" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                    variant ="outlined" 
                    @click="resetDropdowns()">
                    Reset Key Fields
                    </v-btn>
           </v-row>
            <v-divider></v-divider>
        </v-row>
    </v-layout>
    <div style="margin: auto; margin-top: 25px; display: flex; align-items: center; justify-content: center" v-html="staticHTMLForInventory"></div>
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
    async function getInventoryReportsAction(payload?: any): Promise<any> {await store.dispatch('getInventoryReports',payload);}
    async function getKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields', payload);}
    async function getConstraintTypeAction(payload?: any): Promise<any> {await store.dispatch('getConstraintType',payload);}
    async function getQueryAction(payload?: any): Promise<any> {await store.dispatch('getQuery',payload);}
    async function getRawDataKeyFieldsAction(payload?: any): Promise<any> {await store.dispatch('getRawDataKeyFields',payload);}
    async function getKeyPropertiesAction(payload?: any): Promise<any> {await store.dispatch('getKeyFields',payload);}
        
    
    const keyAttributeValues = ref<string[][]>([]);
    const reactiveData: Ref<[string[], string[]]> = ref([[], []]);
    let inventoryItem: any[][] = [];

    let selectedKeys: string[] = [];

    const selectedInventoryIndex = ref([]);

    let htmlResponse = ref();

    let queryLength: number;
    let reactValue: any[] = [];

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

    const beforeRouteLeave = () => {
    // Reset staticHTMLForInventory when leaving the route
    store.state.inventoryModule.staticHTMLForInventory = null; // Set to null or an initial value
    }


    const InventoryStateKeys = computed<InventoryItem[]>(()=> store.state.inventoryModule.inventoryItems)

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        watch(inventoryItems,async ()=>{
            keyAttributeValues.value = await setupSelectLists();
        });

        watch(selectedInventoryIndex, (newValues: any, oldValues) => {
            let index: number;
            if(reactValue[0] != newValues[0] && newValues[0] != undefined){
                reactValue[0] = newValues[0];
                index = 0;
            }
            else
            {
                reactValue[1] = newValues[1];
                index = 1;
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
                reactiveData.value[0].push(keyAttributeValues.value[i][j])
            }

            for(let i = 0; i < keyAttributeValues.value.length; i++)
            {
                let j = 1;
                reactiveData.value[1].push(keyAttributeValues.value[i][j])
            }

        })

        watch(staticHTMLForInventory,()=>{
           // sanitizedHTML = $sanitize(staticHTMLForInventory.value);
        })

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

                getInventoryAction(inventoryDetails.value);
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
            reactiveData.value = [[],[]];
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
                    reactiveData.value[i] = queryValue;
                }
        });
        /**
         * Vue component has been mounted
         */
        onMounted(() => {
            (async () => { 
                await getConstraintTypeAction();
                await getInventoryReportsAction();
                await getKeyFieldsAction(); 
                await getRawDataKeyFieldsAction();
                await getConstraintTypeAction();
            })();
        })

        onBeforeUnmount(() => {
            const router = useRouter();
            router.beforeEach(beforeRouteLeave);
        });

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
        }

        async function resetDropdowns() {
            selectedInventoryIndex.value = [];
            reactiveData.value = [[],[]];
            //Reset selected key fields
            selectedInventoryIndex.value.forEach((value: string, index: number, array: string[]) => {
                selectedKeys[index] = '';
            })
            selectedInventoryIndex.value = [];
            selectedKeys[0] = "";
            keyAttributeValues.value = await setupSelectLists();
        }

        function onSelectInventoryItem(index: number){
            constraintDetails = stateConstraintType.value;
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
                    if(selectedInventoryIndex.value[i]) {
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
            selectedKeys = selectedInventoryIndex.value;
            let key = selectedInventoryIndex.value[index];
                let data: InventoryParam = {
                keyProperties: {},
                values: function (values: any): unknown {
                throw new Error('Function not implemented.');
                }
                };

            if(reportType === 'P')
            {
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
            }

                 //Create a dictionary of the selected key fields
                let dictionary: Record<string, string> = {};

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
                for(let i = 0; i < selectedInventoryIndex.value.length; i++) 
                    {
                        if(selectedInventoryIndex.value[i] !== '') 
                        {
                            if(!querySelectedData.includes(inventoryDetails.value[i]) && !querySelectedData.includes(selectedInventoryIndex.value[i]))
                            {
                                querySelectedData.push(inventoryDetails.value[i]);
                                querySelectedData.push(selectedInventoryIndex.value[i]);
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
