<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout>
                    <v-flex xs11>
                        <v-layout column v-if='budgets.length === 0'>
                            <h3>Investment Library Not Found</h3>
                            <div>
                                No investmentModule library data was found for the selected scenario.
                            </div>
                            <div>
                                To add investmentModule library data, go to the scenario's investmentModule plan editor.
                            </div>
                        </v-layout>
                        <v-layout v-else>
                            <v-data-table :headers='budgetHeaders' :items='budgets'
                                          class='elevation-1 v-table__overflow budgets-data-table ghd-control-text'
                                          sort-icon=$vuetify.icons.ghd-table-sort
                                          hide-actions
                                          item-key='id' select-all
                                          v-model='selectedBudgets'>
                                <template slot='items' slot-scope='props'>
                                    <td>
                                        <v-checkbox hide-details primary v-model='props.selected' />
                                    </td>
                                    <td>
                                        {{ props.item.name }}
                                    </td>
                                    <td></td>
                                </template>
                            </v-data-table>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { clone, contains } from 'ramda';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { State } from 'vuex-class';
import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getPropertyValues } from '@/shared/utils/getter-utils';

@Component
export default class BudgetsTab extends Vue {
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];

    @Prop() selectedTreatmentBudgets: string[];
    @Prop() addTreatment: boolean;
    @Prop() fromLibrary: boolean;    

    initializedBudgets: boolean = false;
    budgetHeaders: DataTableHeader[] = [        
        { text: 'Budget', value: 'name', align: 'left', sortable: true, class: '', width: '300' },
    ];
    budgets: SimpleBudgetDetail[] = [];
    selectedBudgets: SimpleBudgetDetail[] = [];

    @Watch('stateScenarioSimpleBudgetDetails')
    onStateScenarioInvestmentLibraryChanged() {
        this.budgets = clone(this.stateScenarioSimpleBudgetDetails);
    }

    @Watch('selectedTreatmentBudgets')
    onBudgetsTabDataChanged() {        
        if ((this.addTreatment || this.fromLibrary) && !this.initializedBudgets) {        
            this.selectedBudgets = this.budgets;
            this.initializedBudgets = true;
        } else {
            this.selectedBudgets = this.budgets
                .filter((simpleBudgetDetail: SimpleBudgetDetail) => contains(simpleBudgetDetail.id, this.selectedTreatmentBudgets));
        }
    }

    @Watch('selectedBudgets')
    onSelectedBudgetsChanged() {
        const selectedBudgetIds: string[] = getPropertyValues('id', this.selectedBudgets) as string[];
        if (!isEqual(this.selectedTreatmentBudgets, selectedBudgetIds)) {
            this.$emit('onModifyBudgets', this.selectedBudgets);
        }
    }

    @Watch('budgets')
    onBudgetsChanged() {
        this.selectedBudgets = clone(this.budgets);
    }
}
</script>

<style>
.budgets-data-table {
    height: 295px !important;
    overflow-y: auto;
}

.budgets-data-table .v-table tbody tr td{
    font-size: 14px !important;
}

.budgets-data-table .v-table thead tr th{
    width: 20px !important;
}

.budgets-data-table .v-table thead tr th .v-input {
    width: 20px !important;
} 
</style>
