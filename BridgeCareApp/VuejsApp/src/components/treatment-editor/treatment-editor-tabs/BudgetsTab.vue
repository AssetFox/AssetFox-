<template>
    <v-container fluid grid-list-xl>
        <v-row class="budgets-tab-content">
            <v-flex xs12>
                <v-row>
                    <v-flex xs10>
                        <v-row column v-if='budgets?.values.length === 0'>
                            <h3>Investment Library Not Found</h3>
                            <div>
                                No investmentModule library data was found for the selected scenario.
                            </div>
                            <div>
                                To add investmentModule library data, go to the scenario's investmentModule plan editor.
                            </div>
                        </v-row>
                        <v-row v-else>
                            <v-data-table :headers='budgetHeaders' :items='budgets'
                                          class='elevation-1 v-table__overflow budgets-data-table ghd-control-text'
                                          sort-icon=$vuetify.icons.ghd-table-sort
                                          hide-actions
                                          item-key='id' select-all
                                          v-model='selectedBudgets'>
                                <template slot='items' slot-scope='props'  v-slot:item="props">
                                    <td>
                                        <v-checkbox hide-details primary v-model='props.item.selected' />
                                    </td>
                                    <td style="width:400px;">
                                        {{ props.item.name }}
                                    </td>
                                    <td></td>
                                </template>
                            </v-data-table>
                        </v-row>
                    </v-flex>
                </v-row>
            </v-flex>
        </v-row>
    </v-container>
</template>

<script lang='ts' setup>
import Vue, { shallowRef } from 'vue';
import { clone, contains } from 'ramda';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { useStore } from 'vuex';

import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getPropertyValues } from '@/shared/utils/getter-utils';


    const emit = defineEmits(['submit', 'onModifyBudget'])
    let store = useStore();
    let stateScenarioSimpleBudgetDetails = ref<SimpleBudgetDetail[]>(store.state.adminDataModule.stateAttributes);

    let selectedTreatmentBudgets = shallowRef<string[]>();
    let addTreatment: boolean;
    let fromLibrary: boolean;    

    let initializedBudgets: boolean = false;
    let budgetHeaders: DataTableHeader[] = [        
        { text: 'Budget', value: 'name', align: 'left', sortable: true, class: '', width: '300' },
    ];
    let budgets = shallowRef<SimpleBudgetDetail[]>();
    let selectedBudgets = shallowRef<SimpleBudgetDetail[]>();

   

    watch(stateScenarioSimpleBudgetDetails, () => onStateScenarioInvestmentLibraryChanged)
     async function onStateScenarioInvestmentLibraryChanged() {
        budgets.value = clone(stateScenarioSimpleBudgetDetails.value);
    }

    
    watch(selectedTreatmentBudgets, () => onBudgetsTabDataChanged)
     async function onBudgetsTabDataChanged() {   
        if ((addTreatment || fromLibrary) && !initializedBudgets) {        
            selectedBudgets.value = budgets.value;
            initializedBudgets = true;
        } else {
            selectedBudgets.value = budgets.value!
                .filter((simpleBudgetDetail: SimpleBudgetDetail) => contains(simpleBudgetDetail.id));
        }
    }

    watch(selectedBudgets, () => onSelectedBudgetsChanged)
     async function onSelectedBudgetsChanged() { 
        const selectedBudgetIds: string[] = getPropertyValues('id', selectedBudgets.value!) as string[];
        if (!isEqual(selectedTreatmentBudgets, selectedBudgetIds)) {
            emit('onModifyBudget', selectedBudgets.value);
        }
    }

    watch(budgets, () => onBudgetsChanged)
     async function onBudgetsChanged(){ 
        selectedBudgets = clone(budgets);
    }

</script>

<style>
.budgets-tab-content{
   min-width: 1000px;  
}

.budgets-data-table {
    min-width: 800px;
    width: 800px;
    height: 295px !important;
    overflow-y: auto;
}

.budgets-data-table .v-table tbody tr td{
    font-size: 14px !important;
    min-width: 80px;
}

.budgets-data-table .v-table thead tr th{
    width: 20px !important;
}

.budgets-data-table .v-table thead tr th .v-input {
    max-width: 40px !important;
} 
</style>
