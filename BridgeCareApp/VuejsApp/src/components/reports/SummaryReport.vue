<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Summary Report</v-subheader>
                    <v-select
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                </v-flex>
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { clone, contains } from 'ramda';
import { State } from 'vuex-class';
import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getPropertyValues } from '@/shared/utils/getter-utils';

@Component
export default class SummaryReports extends Vue {

    initializedBudgets: boolean = false;
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
