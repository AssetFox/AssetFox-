<template>
    <v-container fluid grid-list-xl>
        <v-layout class='budgets-tab-content'>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-flex xs6>
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
                                          class='elevation-1 fixed-header v-table__overflow budgets-data-table'
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

    budgetHeaders: DataTableHeader[] = [
        { text: 'Budget', value: 'name', align: 'left', sortable: true, class: '', width: '300px' },
    ];
    budgets: SimpleBudgetDetail[] = [];
    selectedBudgets: SimpleBudgetDetail[] = [];

    @Watch('stateScenarioSimpleBudgetDetails')
    onStateScenarioInvestmentLibraryChanged() {
        this.budgets = clone(this.stateScenarioSimpleBudgetDetails);
    }

    @Watch('selectedTreatmentBudgets')
    onBudgetsTabDataChanged() {        
        if (this.addTreatment) {        
            this.selectedBudgets = this.budgets;
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
    height: 245px !important;
    overflow-y: auto;
}
</style>
