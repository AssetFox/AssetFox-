<template>
    <v-dialog max-width='500px' persistent v-model='showDialog'>
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Investment Budgets Import/Export</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <InvestmentBudgetsFileSelector :closed='closed' @submit='onFileSelectorChange' />
                    <v-flex xs12>
                        <v-layout justify-start>
                            <v-checkbox label='Overwrite budgets' v-model='overwriteBudgets'></v-checkbox>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn @click='onSubmit(true)' class='ara-blue-bg white--text'>Upload</v-btn>
                    <v-btn @click='onSubmit(true, true)' class='ara-blue-bg white--text'>Export</v-btn>
                    <v-btn @click='onSubmit(false)' class='ara-orange-bg white--text'>Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action } from 'vuex-class';
import FileSelector from '@/shared/components/FileSelector.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportInvestmentBudgetsDialogResult } from '@/shared/models/modals/import-export-investment-budgets-dialog-result';
import {clone} from 'ramda';

@Component({
    components: { InvestmentBudgetsFileSelector: FileSelector },
})
export default class ImportExportInvestmentBudgetsDialog extends Vue {
    @Prop() showDialog: boolean;

    @Action('setErrorMessage') setErrorMessageAction: any;
    @Action('setIsBusy') setIsBusyAction: any;

    investmentBudgetsFile: File | null = null;
    overwriteBudgets: boolean = true;
    closed: boolean = false;

    @Watch('showDialog')
    onShowDialogChanged() {
        if (this.showDialog) {
            this.closed = false;
        } else {
            this.investmentBudgetsFile = null;
            this.closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */
    onFileSelectorChange(file: File) {
        this.investmentBudgetsFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportInvestmentBudgetsDialogResult = {
                overwriteBudgets: this.overwriteBudgets,
                file: this.investmentBudgetsFile as File,
                isExport: isExport
            };
            this.$emit('submit', result);
        } else {
            this.$emit('submit', null);
        }
    }
}
</script>
