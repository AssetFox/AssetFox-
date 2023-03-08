<template>
    <v-layout>
        <v-dialog width="768px" height="540px" persistent v-model='showDialog'>
            <v-card class="div-padding">
                <v-card-title class="pa-2">
                    <v-layout justify-start>
                        <h4 class="Montserrat-font-family">Investment Budgets Upload</h4>
                    </v-layout>
                    <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
                </v-card-title>
                <v-card-text class="pa-0">
                    <v-layout column>
                        <InvestmentBudgetsFileSelector :closed='closed' @submit='onFileSelectorChange' />
                        <v-flex xs12>
                            <v-layout justify-start>
                                <v-checkbox class="Montserrat-font-family" label='Overwrite budgets' v-model='overwriteBudgets'></v-checkbox>
                            </v-layout>
                        </v-flex>
                    </v-layout>
                </v-card-text>
                <v-card-actions>
                    <v-layout justify-center>
                        <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' flat>Cancel</v-btn>
                        <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' outline>Upload</v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>

        <v-dialog max-width='400px' persistent v-model='isSuccessfulImport'>
            <v-card>
                <v-card-title class="title-padding">
                    <v-layout justify-center>
                        <h6 class="ghd-control-label">Budgets have been replaced.  Please update budget priorities</h6>
                    </v-layout>
                </v-card-title>
                <v-card-actions class="bottom-portion-padding">
                    <v-layout justify-space-between row>
                        <v-btn @click="flipVisible()" outline class="ghd-blue ghd-button-text">Ok</v-btn>
                    </v-layout>
                </v-card-actions>
                
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, Mutation, State } from 'vuex-class';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportInvestmentBudgetsDialogResult } from '@/shared/models/modals/import-export-investment-budgets-dialog-result';
import {clone} from 'ramda';
import InvestmentBudgetsFileSelector from '@/shared/components/FileSelector.vue';

@Component({
    components: { InvestmentBudgetsFileSelector }
})
export default class ImportExportInvestmentBudgetsDialog extends Vue {
    @Prop() showDialog: boolean;

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setIsBusy') setIsBusyAction: any;
    @Mutation('isSuccessfulImportMutator') isSuccessfulImportMutator: any;
    @State(state => state.investmentModule.isSuccessfulImport) isSuccessfulImport: boolean

    investmentBudgetsFile: File | null = null;
    overwriteBudgets: boolean = true;
    closed: boolean = false;

    flipVisible(){
        this.isSuccessfulImportMutator(!this.isSuccessfulImport)
    }

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
<style>
    .title-padding{
        padding-top: 30px;
        padding-left: 30px;
        padding-right: 30px;
    }

    .bottom-portion-padding{
        padding-bottom: 30px;
    }
    .div-padding {
    padding: 30px;
    }
</style>
