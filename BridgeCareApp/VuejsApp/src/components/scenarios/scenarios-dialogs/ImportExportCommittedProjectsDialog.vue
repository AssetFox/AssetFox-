<template>
    <v-dialog persistent v-model="showDialog">
        <!-- <v-card class="div-padding">
            <v-card-title class="pa-2"> -->
                <v-row>
                    <h3 class="Montserrat-font-family">Committed Projects</h3>
                    <v-btn @click="onSubmit(false)" flat>
                    <i class="fas fa-times fa-2x">dfdf</i>
                </v-btn>
                </v-row>
            <!-- </v-card-title> -->
            <v-card-text class="pa-0">
                <v-row column>
                    <CommittedProjectsFileSelector :closed='closed' useTreatment="true" @treatment='onTreatmentChanged' @submit='onSubmitFileSelectorFile' />
                    <span class="div-warning-border">
                        <v-row align-start>
                            <img style="padding-right:5px; height:30px; " :src="require('@/assets/icons/urgent-info.svg')"/>
                            <h3 class="h3-color">Warning</h3>
                        </v-row>
                        <p class="Montserrat-font-family">
                            Uploading new committed projects will override ALL previous commitments.
                            Committed projects may take a few minutes to process. You will receive an email when this process is complete.
                        </p>
                    </span>
                </v-row>
            </v-card-text>
            <v-card-actions>
                <v-row justify-center row>
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue Montserrat-font-family' variant = "flat">Cancel</v-btn>
                    <v-btn @click='onSubmit(true, true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outlined">Export</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outlined">Upload</v-btn>
                </v-row>
            </v-card-actions>
        <!-- </v-card> -->
    </v-dialog>
</template>

<script setup lang='ts'>
import { ref, toRefs, watch } from 'vue'; 

import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import CommittedProjectsFileSelector from '@/shared/components/FileSelector.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { clone } from 'ramda';
import { useStore } from 'vuex'; 

    let store = useStore(); 
    const props = defineProps<{showDialog: boolean}>();
    const emit = defineEmits(['submit','delete']);
    const { showDialog } = toRefs(props);
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification', payload)}
    async function setIsBusyAction(payload?: any): Promise<any>{await store.dispatch('setIsBusy', payload)}

    const committedProjectsFile = ref<File | null>(null);
    const applyNoTreatment = ref< boolean >(true);
    const closed = ref< boolean >(false);

    watch(showDialog,()=> {
        if (showDialog) {
            closed.value = false;
        } else {
            committedProjectsFile.value = null;
            closed.value = true;
        }
    });

    /**
     * FileSelector submit event handler
     */
    function onSubmitFileSelectorFile(file: File, treatment: boolean) {
        committedProjectsFile.value = hasValue(file) ? clone(file) : null;
        applyNoTreatment.value = treatment;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportCommittedProjectsDialogResult = {
                applyNoTreatment: applyNoTreatment.value,
                file: committedProjectsFile.value as File,
                isExport: isExport,
            };
            emit('submit', result);
        } else {
            emit('submit', null);
        }
    }
    /**
     * Apply no treatment event handler
     */
    function onTreatmentChanged(treatment: boolean) {
        applyNoTreatment.value = treatment;
    }
    /**
     * Dialog delete event handler
     */
    function onDelete() {
        emit('delete');
    }

</script>
<style scoped>
.div-warning-border {
    border: solid;
    border-color: #F00;
    border-radius: 4px;
    border-width: 1px;
    padding: 10px;
}
.div-padding {
    padding: 30px;
}
.h3-color {
    color: red;
    font-family: Montserrat-font-family;
}
.icon-color {
    color: red;
}
</style>