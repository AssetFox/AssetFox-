<template>
    <v-layout>
        <v-flex xs12>
            <v-layout justify-space-between row>
                <v-spacer></v-spacer>
                <v-layout>
                    <div class="flex xs2" v-for="(key, index) in inventoryDetails">
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
    import {InventoryItem, KeyProperty} from '@/shared/models/iAM/inventory';
    import {find, propEq} from 'ramda';

    @Component
    export default class Inventory extends Vue {
        @State(state => state.inventoryModule.inventoryItems) inventoryItems: InventoryItem[];
        @State(state => state.inventoryModule.staticHTMLForInventory) staticHTMLForInventory: any;

        @Action('getInventory') getInventoryAction: any;
        @Action('getStaticInventoryHTML') getStaticInventoryHTMLAction: any; 
        @Action('setIsBusy') setIsBusyAction: any;

        keyAttirbuteValues: string[][] = [];

        inventoryItem: any[][] = [];

        selectedKeys: string[] = [];

        inventoryDetails: string[] = [];
                       
        inventorySelectListsWorker: any = null;

        inventoryData: any  = null;
        sanitizedHTML: any = null;
        implementationName = 'BAMS'; // TODO: get from implementation name setting
        // FYI BAMS inventoy report name is now changed to BAMSInventoryLookup
        // TODO: if PAMS, build inventoryReportName as PAMSInventoryLookupSegments or PAMSInventoryLookupSections        
        inventoryReportName: string = this.implementationName + 'InventoryLookup';

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

        /**
         * Vue component has been mounted
         */
        mounted() {
            const keys = ["BMSID","BRKEY_"]; // TODO: Implementation based setting for keyAttributes should be defined and used here

            this.inventoryDetails = keys;
            this.inventoryDetails.forEach(_ => this.selectedKeys.push(""));
           
            this.getInventoryAction(this.inventoryDetails);
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

        onSelectInventoryItem(index: number){
            const key = this.selectedKeys[index];
            let data: InventoryItem = {keyProperties: []};

            for(let i = 0; i < this.inventoryDetails.length; i++){
                if(i === index){
                    data.keyProperties[i] = key;
                    continue;
                }
                const inventoryItem = this.inventoryItems.filter(function(item){if(item.keyProperties.indexOf(key) !== -1) return item;})[0]; 
                const otherKeyValue = inventoryItem.keyProperties[i]; 
                this.selectedKeys[i] = otherKeyValue;
                data.keyProperties[i] = otherKeyValue;
            }
            this.getStaticInventoryHTMLAction({reportType: this.inventoryReportName, filterData: data});           
        }
    }
</script>

<style>
    @import "../assets/css/inventory.css"
</style>
