<template>
    <v-container >
        <v-row >
            <v-row column v-if='budgets?.length === 0'>
                <h3 id="ButgetsTab-InvestmentLibraryNotFound-Header">Investment Library Not Found</h3>
                <div>
                    No investmentModule library data was found for the selected scenario.
                </div>
                <div>
                    To add investmentModule library data, go to the scenario's investmentModule plan editor.
                </div>
            </v-row>
            <v-row v-else>
                <v-data-table-virtual :headers='budgetHeaders' :items='budgets'
                              id="BudgetsTab-Budgets-datatable"
                              class='elevation-1 v-table__overflow budgets-data-table ghd-control-text'
                              sort-icon=ghd-table-sort
                              hide-actions
                              item-key='id' show-select
                              return-object 
                              v-model='selectedBudgets'>
                    <template slot='items' slot-scope='props'  v-slot:item="props">
                        <tr>
                            <td>
                                <v-checkbox id="BudgetsTab-Budget-checkbox" hide-details primary v-model="selectedBudgets" :value="props.item" />
                            </td>
                            <td style="width:400px;">
                                {{ props.item.name }}
                            </td>
                            <td></td>
                        </tr>
                    </template>
                </v-data-table-virtual>
            </v-row>
        </v-row>
    </v-container>
</template>

<script setup lang='ts'>
import { ref, toRefs, watch, shallowRef, computed, onMounted } from 'vue';
import { clone, contains } from 'ramda';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { useStore } from 'vuex';

import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getPropertyValues } from '@/shared/utils/getter-utils';

    const emit = defineEmits(['submit', 'onModifyBudget'])
    let store = useStore();
    
    const stateScenarioSimpleBudgetDetails = computed<SimpleBudgetDetail[]>(() => store.state.investmentModule.scenarioSimpleBudgetDetails);

    const props = defineProps<{
         selectedTreatmentBudgets: string[],
         addTreatment: boolean,
         fromLibrary: boolean  
    }>();  

    let initializedBudgets: boolean = false;
    const budgetHeaders: any[] = [        
        { title: 'Budget', key: 'name', align: 'left', sortable: true, class: '', width: '300' },
    ];
    const budgets = ref<SimpleBudgetDetail[]>([]);
    const selectedBudgets = ref<SimpleBudgetDetail[]>([]);

    onMounted(() => {  
        if(stateScenarioSimpleBudgetDetails.value.length > 0){
            budgets.value = clone(stateScenarioSimpleBudgetDetails.value);
            selectedBudgets.value = clone(budgets.value);
        }   
        onSelectedTreatmentBudgetsChanged();
    })

    watch(stateScenarioSimpleBudgetDetails, () => {
        budgets.value = clone(stateScenarioSimpleBudgetDetails.value);
    });

    watch(() => props.selectedTreatmentBudgets, onSelectedTreatmentBudgetsChanged);

    function onSelectedTreatmentBudgetsChanged(){
        if ((props.addTreatment || props.fromLibrary) && !initializedBudgets) {        
            selectedBudgets.value = budgets.value;
            initializedBudgets = true;
        } else {
            selectedBudgets.value = budgets.value!
                .filter((simpleBudgetDetail: SimpleBudgetDetail) => contains(simpleBudgetDetail.id));
        }
    }

    watch(selectedBudgets, () => { 
        const selectedBudgetIds: string[] = getPropertyValues('id', selectedBudgets.value!) as string[];
        if (!isEqual(props.selectedTreatmentBudgets, selectedBudgetIds)) {
            emit('onModifyBudget', selectedBudgets.value);
        }
    });

    watch(budgets, () => { 
        selectedBudgets.value = clone(budgets.value);
    });

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
