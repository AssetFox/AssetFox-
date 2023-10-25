import {QueryResponse, InventoryParam, emptyInventoryParam, InventoryItem, MappedInventoryItem, emptyQueryResponse} from '@/shared/models/iAM/inventory';
import InventoryService from '@/services/inventory.service';
import {append, clone, contains} from 'ramda';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

const state = {
    inventoryItems: [] as InventoryItem[],
    staticHTMLForInventory: '' as any,
    querySet: [] as string[],
};

const mutations = {
    inventoryItemsMutator(state: any, inventoryItems: InventoryItem[]) {
        state.inventoryItems = clone(inventoryItems);
    },
    inventoryStaticHTMLMutator(state: any, staticHTMLPage: any){
        state.staticHTMLForInventory = clone(staticHTMLPage);
    },
    inventoryQueryMutator(state: any, queries: string[]) {
        state.querySet = clone(queries);
    }
};

const actions = {
    async getInventory({commit}: any, keyProperties: any) {
            const response = await InventoryService.getInventory(keyProperties);
            if (hasValue(response, 'data')) {
              const mappedItems = response.data.map((resp: { keyProperties: any; }) => ({
                keyProperties: resp.keyProperties,
              }));
              commit('inventoryItemsMutator', mappedItems);
            }
        },

    async getStaticInventoryHTML({commit}: any, payload: any){
        await InventoryService.getStaticInventoryHTML(payload.reportType, payload.filterData)
        .then((response: AxiosResponse<any>) => {
            if(hasValue(response, 'data')){
                commit('inventoryStaticHTMLMutator', response.data);
            }
        });
    },

        async getQuery({commit}: any, payload: any){
        await InventoryService.getQuery(payload.querySet)
        .then((response: AxiosResponse<any>) => {
            if(hasValue(response, 'data')){
                commit('inventoryQueryMutator', response.data);
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
