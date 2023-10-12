<template>
    <Dialog width="768px" height="540px" persistent v-bind:show='showDialog'>
        <v-card class="div-padding">
            <v-card-title class="pa-2">
                <v-row justify-start>
                    <h3 class="Montserrat-font-family">Treatments Upload</h3>
                </v-row>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>
            <v-card-text class="pa-0">
                <v-row column>
                    <TreatmentsFileSelector :closed='closed' @submit='onFileSelectorChange' />                    
                </v-row>
            </v-card-text>
            <v-card-actions>
                <v-row justify-center>
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "flat">Cancel</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outlined">Upload</v-btn>                    
                </v-row>
            </v-card-actions>
        </v-card>
    </Dialog>
</template>

<script lang='ts' setup>
import Vue from 'vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportNewTreatmentDialogResult } from '@/shared/models/modals/import-new-treatment-dialog-result';
import {clone} from 'ramda';
import TreatmentsFileSelector from '@/shared/components/FileSelector.vue';
import Dialog from 'primevue/dialog';

    const props = defineProps<{showDialog: Boolean}>()
    const showDialog = reactive(props.showDialog);

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

    function onFileSelectorChange(file: File) {
        TreatmentsFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportNewTreatmentDialogResult = {
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