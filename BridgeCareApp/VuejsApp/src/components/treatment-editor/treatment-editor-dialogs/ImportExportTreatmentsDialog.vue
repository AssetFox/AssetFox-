<template>
    <v-dialog width="768px" height="540px" persistent v-model='showDialog'>
        <v-card class="div-padding">
            <v-card-title class="pa-2">
                <v-layout justify-start>
                    <h3 class="Montserrat-font-family">Treatments Upload</h3>
                </v-layout>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>
            <v-card-text class="pa-0">
                <v-layout column>
                    <TreatmentsFileSelector :closed='closed' @submit='onFileSelectorChange' />                    
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-center>
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "flat">Cancel</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outlined">Upload</v-btn>                    
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang='ts' setup>
import Vue from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportTreatmentsDialogResult } from '@/shared/models/modals/import-export-treatments-dialog-result';
import {clone} from 'ramda';
import { useStore } from 'vuex';
import TreatmentsFileSelector from '@/shared/components/FileSelector.vue';

    const props = defineProps<{showDialog: Boolean}>()
    const showDialog = props.showDialog;

    async function addErrorNotificationAction(payload?: any): Promise<any> {
        await store.dispatch('addErrorNotification');
    }

    async function setIsBusyAction(payload?: any): Promise<any> {
        await store.dispatch('setIsBusy');
    }   

    let TreatmentsFile: File | null = null;
    let overwriteBudgets: boolean = true;
    let closed: boolean = false;
    let store = useStore();
    const emit = defineEmits(['submit'])

    watch(showDialog, () => onShowDialogChanged)
    async function onShowDialogChanged() {
        if (showDialog) {
            closed = false;
        } else {
            TreatmentsFile = null;
            closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */
    function onFileSelectorChange(file: File) {
        TreatmentsFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportTreatmentsDialogResult = {
                file: TreatmentsFile as File,
                isExport: isExport
            };
            emit('submit', result);
        } else {
            emit('submit', null);
        }
    }

</script>
<style scoped>
.div-padding {
    padding: 30px;
}
</style>