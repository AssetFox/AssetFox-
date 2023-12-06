<template>
    <v-dialog persistent style="width: 70%; height: 80%; padding: 1em;" v-model="showDialog">
        <v-card class="div-padding">
            <v-row class="pa-2">
                <!-- <v-col> -->
                    <v-row justify="space-between">
                        <h3 class="Montserrat-font-family">Committed Projects</h3>
                        <v-btn @click="onSubmit(false)" flat>
                        <i class="fas fa-times fa-2x"></i>
                        </v-btn>
                    </v-row>
                        <div style="margin: 50px;">
                        <CommittedProjectsFileSelector :closed='closed' useTreatment='true' @treatment='onTreatmentChanged' @submit='onSubmitFileSelectorFile' />
                        </div>
                        <span class="div-warning-border" style="margin: 10px;">
                            <v-row align="start" style="padding:5px;">
                                <v-icon class="px-2 icon-color">fas fa-exclamation-triangle</v-icon>
                                <h3 class="h3-color">Warning</h3>
                            </v-row>
                            <p class="Montserrat-font-family">
                                Uploading new committed projects will override ALL previous commitments.
                                Committed projects may take a few minutes to process. You will receive an email when this process is complete.
                            </p>
                        </span>
                    <v-row justify="center" style="margin: 10px;">
                        <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue Montserrat-font-family' variant = "flat">Cancel</v-btn>
                        <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outlined">Upload</v-btn>
                    </v-row>
            </v-row>
        </v-card>
    </v-dialog>
</template>

<script setup lang='ts'>
import {watch, ref, computed, toRefs } from 'vue';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import CommittedProjectsFileSelector from '@/shared/components/FileSelector.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { clone } from 'ramda';
import { useStore } from 'vuex';

    let store = useStore();
    const props = defineProps<{showDialog: boolean}>();
    const { showDialog } = toRefs(props);
    const emit = defineEmits(['submit', 'delete']);

    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification',payload); }

    const committedProjectsFile = ref< File | null > ( null );
    const applyNoTreatment = ref<boolean>(true);
    const closed = ref<boolean>(false);

    watch(showDialog, () => {
        if (showDialog.value) {
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
            //todo: get rid of is export portion
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