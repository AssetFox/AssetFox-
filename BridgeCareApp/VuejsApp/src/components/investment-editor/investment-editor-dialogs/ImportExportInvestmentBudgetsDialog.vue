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

<script lang='ts' setup>
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportInvestmentBudgetsDialogResult } from '@/shared/models/modals/import-export-investment-budgets-dialog-result';
import {clone} from 'ramda';
import InvestmentBudgetsFileSelector from '@/shared/components/FileSelector.vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref,shallowRef} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
let showDialog=shallowRef<boolean>(false);
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}
async function setIsBusyAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}
function isSuccessfulImportMutator(payload:any){store.commit('isSuccessfulImportMutator');}
let isSuccessfulImport = ref<boolean>(store.state.investmentModule.isSuccessfulImport);

let investmentBudgetsFile: File | null = null;
let overwriteBudgets: boolean = true;
let closed: boolean = false;

    function flipVisible(){
        isSuccessfulImportMutator(!isSuccessfulImport.value)
    }

    watch(showDialog,()=> onShowDialogChanged)
    function onShowDialogChanged() {
        if (showDialog) {
            closed = false;
        } else {
            investmentBudgetsFile = null;
            closed = true;
        }
    }   

    /**
     * FileSelector submit event handler
     */

    function onFileSelectorChange(file: File) {
        investmentBudgetsFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportInvestmentBudgetsDialogResult = {
                overwriteBudgets: overwriteBudgets,
                file: investmentBudgetsFile as File,
                isExport: isExport
            };
            emit('submit', result);
        } else {
            emit('submit', null);
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
