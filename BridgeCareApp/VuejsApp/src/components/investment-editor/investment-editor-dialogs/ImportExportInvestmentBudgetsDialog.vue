<template>
    <v-row>
        <v-dialog width="768px" height="540px" persistent v-model ='showDialog'>
            <v-card class="div-padding">
                    <v-row justify="space-between" style="margin-bottom: 10px;">
                        <h4 style="padding-top: 10px; padding-left: 10px;" class="Montserrat-font-family">Investment Budgets Upload</h4>
                        <XButton @click="onSubmit(false)"/>
                    </v-row>
                    
                <v-card-text >
                    <v-row column>
                        <InvestmentBudgetsFileSelector :closed='closed' @submit='onFileSelectorChange' />
                        <v-col cols = "12">
                            <v-row justify-start>
                                <v-checkbox class="Montserrat-font-family" label='Overwrite budgets' v-model='overwriteBudgets'></v-checkbox>
                            </v-row>
                        </v-col>
                    </v-row>
                </v-card-text>
                <v-card-actions>
                    <v-row justify="center">
                        <CancelButton @cancel="onSubmit(false)"/>
                        <UploadButton @upload="onSubmit(true)"/>
                    </v-row>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog max-width='450px' persistent v-model='showSuccessDialog'>
            <v-card>
                <v-card-title class="title-padding">
                    <v-row justify="center">
                        <h6 class="ghd-control-label">Budgets have been replaced.  Please update budget priorities</h6>
                    </v-row>
                </v-card-title>
                <v-card-actions>
                    <v-row justify="center">
                        <OkButton @ok="successSubmit()"/>
                    </v-row>
                </v-card-actions>
                
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script lang='ts' setup>
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportInvestmentBudgetsDialogResult } from '@/shared/models/modals/import-export-investment-budgets-dialog-result';
import {clone} from 'ramda';
import InvestmentBudgetsFileSelector from '@/shared/components/FileSelector.vue';
import {inject, reactive, ref, toRefs, onMounted, onBeforeUnmount, watch, computed, Ref,shallowRef} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import UploadButton from '@/shared/components/buttons/UploadButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';
import OkButton from '@/shared/components/buttons/OkButton.vue';

let store = useStore();
const emit = defineEmits(['submit', 'submitSuccessImport'])
const props = defineProps<{
    showDialog: boolean,
    showSuccessDialog: boolean
    }>()
const { showDialog, showSuccessDialog } = toRefs(props);

async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}
function isSuccessfulImportMutator(payload:any){store.commit('isSuccessfulImportMutator', payload);}
let isSuccessfulImport = computed<boolean>(() => store.state.investmentModule.isSuccessfulImport);
const stateIsSuccessfuImport = computed<boolean>(() => store.state.investmentModule.isSuccessfulImport);
const hasAdminAccess = computed<boolean>(()=> (store.state.authenticationModule.hasAdminAccess)); 

let investmentBudgetsFile = ref<File | null>(null);
let overwriteBudgets = ref(true);
let closed = ref<boolean>(false);

    function successSubmit(){    
        emit('submitSuccessImport', false)
    }

    watch(()=>props.showDialog,() => {
        if (props.showDialog) {
            closed.value = false;
        } else {
            investmentBudgetsFile.value = null;
            closed.value = true;
        }
    }); 

    /**
     * FileSelector submit event handler
     */

    function onFileSelectorChange(file: File) {
        investmentBudgetsFile.value = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportInvestmentBudgetsDialogResult = {
                overwriteBudgets: overwriteBudgets.value,
                file: investmentBudgetsFile.value as File,
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
