<template>
    <v-layout>
        <v-flex xs12>
            <v-layout justify-space-between row>
                <v-spacer></v-spacer>
                <v-flex xs2>
                <!--TODO: lists should be dynamically created based on no. of implementation based keyAttributes-->
                    <v-autocomplete :items="bmsIdsSelectList" @change="onSelectInventoryItemByBMSId" item-text="identifier" item-value="identifier"
                                    label="Select by BMS Id" outline
                                    v-model="selectedBmsId">
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
                </v-flex>
                <v-flex xs2>
                    <v-autocomplete :items="brKeysSelectList" @change="onSelectInventoryItemsByBRKey" item-text="identifier" item-value="identifier"
                                    label="Select by BR Key" outline
                                    v-model="selectedBrKey">
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
                </v-flex>
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

        brKeysSelectList: number[] = [];
        bmsIdsSelectList: any[] = [];
        inventoryItem: any[][] = [];
        selectedBmsId: string = '';
        selectedBrKey: string = '0';

        inventoryDetails: KeyProperty[];       
        
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
            const foo = ["BMSID","BRKEY_"]; // TODO: Implementation based setting for keyAttributes should be defined and used here

            this.inventoryDetails = this.$config.inventoryKeyProperties;
            var inventoryDetail = {
                key1: this.inventoryDetails[0].name,
                key2: this.inventoryDetails[1].name
            };
            this.getInventoryAction(foo);
        }

        created() {
            this.inventorySelectListsWorker = this.$worker.create(
                [
                    {
                        message: 'setInventorySelectLists', func: (data: any) => {
                            if (data) {
                                
                                const inventoryItems = data.inventoryItems;

                                const bmsIds: any[] = [];
                                const brKeys: any[] = [];

                                inventoryItems.forEach((item: InventoryItem, index: number) => {
                                    if (index === 0) { 
                                        // TODO: headers to be populated based on number of key attributes display names from setting
                                        bmsIds.push({header: 'BMS Ids'});
                                        brKeys.push({header: 'BR Keys'});
                                     }
                                   
                                        bmsIds.push({
                                            identifier: item.keyProperties[0],
                                            group: 'BMS Ids'
                                        });

                                        brKeys.push({
                                            identifier: item.keyProperties[1],
                                            group: 'BR Keys'
                                        });
                                    
                                });

                                const bmsIdsSelectList = bmsIds;
                                const brKeysSelectList = brKeys;

                                return {bmsIdsSelectList: bmsIdsSelectList, brKeysSelectList: brKeysSelectList};
                            }

                            return {bmsIdsSelectList: [], brKeysSelectList: []};
                        }
                    }
                ]
            );
        }


        setupSelectLists() {
            const data: any = {
                inventoryItems: this.inventoryItems,
            };
            this.inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])
                .then((result: any) => {
                    this.bmsIdsSelectList = result.bmsIdsSelectList;
                    this.brKeysSelectList = result.brKeysSelectList;
                });
        }

        /**
         * BMS id has been selected
         */
        onSelectInventoryItemByBMSId(bmsId: string) {
            var key : string = '-1';
            var data : InventoryItem = { keyProperties: [
                    bmsId,
                    key
                ]
            };            
            this.selectedBmsId = bmsId;
            const inventoryItem: InventoryItem = this.inventoryItems.filter(function(item){if(item.keyProperties.indexOf(bmsId) !== -1) return item;})[0];
            this.selectedBrKey = inventoryItem.keyProperties[1];
            this.getStaticInventoryHTMLAction(({reportType: this.inventoryReportName, filterData: data}));
        }

        /**
         * BR key has been selected
         */
        onSelectInventoryItemsByBRKey(brKey: string) {
            var data : InventoryItem = { keyProperties: [
                    '',
                    brKey
                ],
            };
            this.selectedBrKey = brKey;
            const inventoryItem = this.inventoryItems.filter(function(item){if(item.keyProperties.indexOf(brKey) !== -1) return item;})[0];
            this.selectedBmsId = inventoryItem.keyProperties[0];
            this.getStaticInventoryHTMLAction({reportType: this.inventoryReportName, filterData: data});
        }
    }
</script>

<style>
    @import "../assets/css/inventory.css"
</style>
