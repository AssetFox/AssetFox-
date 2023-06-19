import {InventoryParam, InventoryItem, MappedInventoryItem} from '@/shared/models/iAM/inventory';
import InventoryService from '@/services/inventory.service';
import {append, clone, contains} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    inventoryItems: [] as InventoryItem[],
    staticHTMLForInventory: '' as any
};

const mutations = {
    inventoryItemsMutator(state: any, inventoryItems: InventoryItem[]) {
        state.inventoryItems = clone(inventoryItems);
    },
    inventoryStaticHTMLMutator(state: any, staticHTMLPage: any){
        state.staticHTMLForInventory = clone(staticHTMLPage);
    },
};

const actions = {
    async getInventory({commit}: any, payload: any) {
        await InventoryService.getInventory(payload)
            .then((response: AxiosResponse<MappedInventoryItem[]>) => {
                if (hasValue(response, 'data')) {
                    var mappedItems: InventoryItem[] = [];
                    var r = response.data;
                    r.forEach(resp => {
                        var mappedItem: InventoryItem = {keyProperties:[]};
                        mappedItem.keyProperties = resp.keyProperties;
                        mappedItems.push(mappedItem);
                    });
                    commit('inventoryItemsMutator', mappedItems);
                }
            });
    },

    async getStaticInventoryHTML({commit}: any, payload: any){
        await InventoryService.getStaticInventoryHTML(payload.reportType, payload.filterData)
        .then((response: AxiosResponse<any>) => {
            if(hasValue(response, 'data')){
                commit('inventoryStaticHTMLMutator', response.data);
            }
        });
    }
};

const getters = {};

export default {
    state,
    getters,
    actions,
    mutations
};
