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
    import {InventoryItem, InventoryItemDetail, LabelValue, NbiLoadRating} from '@/shared/models/iAM/inventory';
    import {find, groupBy, propEq, uniq} from 'ramda';
    import {hasValue} from '@/shared/utils/has-value-util';
    import {DataTableHeader} from '@/shared/models/vue/data-table-header';
    import {DataTableRow} from '@/shared/models/vue/data-table-row';

    @Component
    export default class Inventory extends Vue {
        @State(state => state.inventoryModule.inventoryItems) inventoryItems: InventoryItem[];
        @State(state => state.inventoryModule.inventoryItemDetail) inventoryItemDetail: InventoryItemDetail;
        @State(state => state.inventoryModule.lastFiveBmsIdSearches) stateLastFiveBmsIdSearches: string[];
        @State(state => state.inventoryModule.lastFiveBrKeySearches) stateLastFiveBrKeySearches: number[];
        @State(state => state.inventoryModule.staticHTMLForInventory) staticHTMLForInventory: any;

        @Action('getInventory') getInventoryAction: any;
        @Action('getStaticInventoryHTML') getStaticInventoryHTMLAction: any; 
        @Action('appendBmsIdSearchString') appendBmsIdSearchStringAction: any;
        @Action('appendBrKeySearchNumber') appendBrKeySearchNumberAction: any;
        @Action('setIsBusy') setIsBusyAction: any;
        @Action('clearInventoryItemDetail') clearInventoryItemDetailAction: any;
        
        lastFiveBmsIdSearches: any[] = [];
        bmsIdsSelectList: any[] = [];
        selectedBmsId: string = '';
        lastFiveBrKeySearches: any[] = [];
        brKeysSelectList: number[] = [];
        selectedBrKey: string = '';
        conditionTableHeaders: DataTableHeader[] = [
            {text: '', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Condition', value: '', align: 'center', sortable: false, class: '', width: ''},
            {text: 'Duration (years)', value: '', align: 'center', sortable: false, class: '', width: ''}
        ];
        nbiLoadRatingTableHeaders: DataTableHeader[] = [];
        nbiLoadRatingTableRows: DataTableRow[] = [];
        postingTableHeaders: DataTableHeader[] = [];
        postingTableRows: DataTableRow[] = [];
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

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('stateLastFiveBmsIdSearches')
        onLastFiveBmsIdSearchesChanged() {
            if (hasValue(this.stateLastFiveBmsIdSearches)) {
                this.lastFiveBmsIdSearches = this.setLastFiveSearchesForInventorySelectList(this.stateLastFiveBmsIdSearches);
                this.setupSelectLists();
            }
        }

        /**
         * Calls the setInventorySelectLists function to set both inventory type select lists
         */
        @Watch('stateLastFiveBrKeySearches')
        onLastFiveBrKeySearchesChanged() {
            if (hasValue(this.stateLastFiveBrKeySearches)) {
                this.lastFiveBrKeySearches = this.setLastFiveSearchesForInventorySelectList(this.stateLastFiveBrKeySearches);
                this.setupSelectLists();
            }

        }

        @Watch('staticHTMLForInventory')
        onStaticHTMLForInventory(){
            this.sanitizedHTML = this.$sanitize(this.staticHTMLForInventory);
        }

        @Watch('inventoryItemDetail')
        onInventoryItemDetailChanged(inventoryItemDetail: InventoryItemDetail) {
            if (inventoryItemDetail.nbiLoadRatings.length > 0) {
                // get the nbiLoadRating column names using the inventoryItemDetail.nbiLoadRatings 1st entry
                const nbiLoadRatingColumns: string[] = uniq(inventoryItemDetail.nbiLoadRatings[0].nbiLoadRatingRow
                    .map((labelValue: LabelValue) => labelValue.label) as string[]
                );
                // set nbiLoadRatingTableHeaders using nbiLoadRatingColumns
                this.nbiLoadRatingTableHeaders = nbiLoadRatingColumns.map((columnName: string) => ({
                    text: columnName, value: columnName, align: 'center', sortable: false, class: '', width: ''
                }) as DataTableHeader);
                // set the nbiLoadRatingTableRows
                this.nbiLoadRatingTableRows = this.createDataTableRowFromNbiLoadRatingGrouping(inventoryItemDetail.nbiLoadRatings);
            } else {
                this.nbiLoadRatingTableRows = [];
            }

            // get the posting column names using the inventoryItemDetail.posting LabelValue list
            const postingColumns: string[] = uniq(inventoryItemDetail.posting
                .map((labelValue: LabelValue) => labelValue.label) as string[]
            );
            // set postingTableHeaders using postingColumns
            this.postingTableHeaders = postingColumns.map((columnName: string) => ({
                text: columnName, value: columnName, align: 'center', sortable: false, class: '', width: ''
            }) as DataTableHeader);
            // set the postingTableRows using the createDataTableRowFromGrouping func.
            this.postingTableRows = this.createDataTableRowFromGrouping(inventoryItemDetail.posting);

        }

        /**
         * Vue component has been mounted
         */
        mounted() {                        
            const inventoryDetail = ["BMSID","BRKEY_"]; // TODO: Implementation based setting for keyAttributes should be defined and used here
            this.getInventoryAction(inventoryDetail);
        }

        created() {
            this.inventorySelectListsWorker = this.$worker.create(
                [
                    {
                        message: 'setInventorySelectLists', func: (data: any) => {
                            if (data) {
                                const inventoryItems = data.inventoryItems;
                                const stateLastFiveBmsIdSearches = data.stateLastFiveBmsIdSearches;
                                const stateLastFiveBrKeySearches = data.stateLastFiveBrKeySearches;
                                const lastFiveBmsIdSearches = data.lastFiveBmsIdSearches;
                                const lastFiveBrKeySearches = data.lastFiveBrKeySearches;

                                const bmsIds: any[] = [];
                                const brKeys: any[] = [];

                                inventoryItems.forEach((item: InventoryItem, index: number) => {
                                    if (index === 0) { 
                                        // TODO: headers to be populated based on number of key attributes display names from setting
                                        bmsIds.push({header: 'BMS Ids'});
                                        brKeys.push({header: 'BR Keys'});
                                    }
                                    
                                    if (stateLastFiveBmsIdSearches.indexOf(item.keyProperties[0]) === -1) {
                                        bmsIds.push({
                                            identifier: item.keyProperties[0],
                                            group: 'BMS Ids'
                                        });
                                    }

                                    if (stateLastFiveBrKeySearches.indexOf(item.keyProperties[1]) === -1) {
                                        brKeys.push({
                                            identifier: item.keyProperties[1],
                                            group: 'BR Keys'
                                        });
                                    }
                                });

                                const bmsIdsSelectList = lastFiveBmsIdSearches.concat(bmsIds);
                                const brKeysSelectList = lastFiveBrKeySearches.concat(brKeys);

                                return {bmsIdsSelectList: bmsIdsSelectList, brKeysSelectList: brKeysSelectList};
                            }

                            return {bmsIdsSelectList: [], brKeysSelectList: []};
                        }
                    }
                ]
            );
        }

        beforeDestroy() {
            this.clearInventoryItemDetailAction();
        }

        setupSelectLists() {
            const data: any = {
                inventoryItems: this.inventoryItems,
                stateLastFiveBmsIdSearches: this.stateLastFiveBmsIdSearches,
                stateLastFiveBrKeySearches: this.stateLastFiveBrKeySearches,
                lastFiveBmsIdSearches: this.lastFiveBmsIdSearches,
                lastFiveBrKeySearches: this.lastFiveBrKeySearches,
            };
            this.inventorySelectListsWorker.postMessage('setInventorySelectLists', [data])
                .then((result: any) => {
                    this.bmsIdsSelectList = result.bmsIdsSelectList;
                    this.brKeysSelectList = result.brKeysSelectList;
                });
        }

        setLastFiveSearchesForInventorySelectList(searchData: any[]) {
            const lastFiveSearches: any[] = [];

            searchData.forEach((searchValue: any, index: number) => {
                if (index === 0) {
                    lastFiveSearches.push({header: 'Last Five Searches'});
                }

                lastFiveSearches.push({
                    identifier: searchValue,
                    group: 'Last Five Searches'
                });

                if (index === searchData.length - 1) {
                    lastFiveSearches.push({divider: true});
                }
            });

            return lastFiveSearches;
        }

        createDataTableRowFromGrouping(labelValueList: LabelValue[]) {
            // group the LabelValue list by the label prop
            const groups = groupBy((labelValue: LabelValue) => labelValue.label, labelValueList);
            // get the list of group keys
            const keys = Object.keys(groups);
            // get the length of the first LabelValue group using the first key in keys if keys has a value
            const groupsLength = hasValue(keys) ? groups[keys[0]].length : 0;
            // create a DataTableRow list
            const dataTableRows: DataTableRow[] = [];
            // use a for loop to create a DataTableRow to add to dataTableRows
            for (let i = 0; i < groupsLength; i++) {
                // create an empty DataTableRow object
                const dataTableRow: DataTableRow = {};
                // loop over each postingGroups key, adding the key as a property to postingTableRow
                // and then getting the value of the LabelValue object at the current iteration for the current group
                Object.keys(groups).forEach((key: string) => dataTableRow[key] = hasValue(groups[key][i]) ? groups[key][i].value : '');
                // push the created postingTableRow to postingTableRows
                dataTableRows.push(dataTableRow);
            }
            return dataTableRows;
        }

        createDataTableRowFromNbiLoadRatingGrouping(nbiLoadRatingList: NbiLoadRating[]) {
            // create a DataTableRow list
            const dataTableRows: DataTableRow[] = [];
            for (let index = 0; index < nbiLoadRatingList.length; index++) {
                // group the LabelValue list by the label prop
                const groups = groupBy((labelValue: LabelValue) => labelValue.label, nbiLoadRatingList[index].nbiLoadRatingRow);
                // get the list of group keys
                const keys = Object.keys(groups);
                // get the length of the first LabelValue group using the first key in keys if keys has a value
                const groupsLength = hasValue(keys) ? groups[keys[0]].length : 0;

                // use a for loop to create a DataTableRow to add to dataTableRows
                for (let i = 0; i < groupsLength; i++) {
                    // create an empty DataTableRow object
                    const dataTableRow: DataTableRow = {};
                    // loop over each postingGroups key, adding the key as a property to postingTableRow
                    // and then getting the value of the LabelValue object at the current iteration for the current group
                    Object.keys(groups).forEach((key: string) => dataTableRow[key] = hasValue(groups[key][i]) ? groups[key][i].value : '');
                    // push the created postingTableRow to postingTableRows
                    dataTableRows.push(dataTableRow);
                }
            }
            return dataTableRows;
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

        getGMapsUrl() {
            var url = `https://maps.google.com/maps?q=${this.inventoryItemDetail.name}&t=&z=15&ie=UTF8&iwloc=&output=embed`;
            return encodeURI(url);
        }    
    }
</script>

<style>
    @import "../assets/css/inventory.css"
</style>
