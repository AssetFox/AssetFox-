<template>
    <v-dialog  width="768px" height="540px" persistent v-model='showDialog'>
        <v-card class="div-padding">
            <v-card-title class="pa-2">
                <v-layout justify-start>
                    <h3 class="Montserrat-font-family">Committed Projects</h3>
                </v-layout>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>
            <v-card-text class="pa-0">
                <v-layout column>
                    <CommittedProjectsFileSelector :closed='closed' useTreatment='true' @treatment='onTreatmentChanged' @submit='onSubmitFileSelectorFile' />
                    <span class="div-warning-border">
                        <v-layout align-start>
                            <v-icon class="px-2 icon-color">fas fa-exclamation-triangle</v-icon>
                            <h3 class="h3-color">Warning</h3>
                        </v-layout>
                        <p class="Montserrat-font-family">
                            Uploading new committed projects will override ALL previous commitments.
                            Committed projects may take a few minutes to process. You will receive an email when this process is complete.
                        </p>
                    </span>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-center row>
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue Montserrat-font-family' variant = "flat">Cancel</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' variant = "outline">Upload</v-btn>
                    <!-- <v-tooltip top>
                        <template slot='activator'>
                            <v-btn @click='onDelete' class='ara-orange-bg white--text'>Delete</v-btn>
                        </template>
                        <span>Delete scenario's current committed projects</span>
                    </v-tooltip> -->
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang='ts' setup>
import {watch, ref} from 'vue';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import FileSelector from '@/shared/components/FileSelector.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { clone } from 'ramda';
import { useStore } from 'vuex';


    let store = useStore();
    const props = defineProps({showDialog: Boolean});
    let showDialog = ref<boolean>(props.showDialog);
    const emit = defineEmits(['submit', 'delete']);

    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification'); }
    async function setIsBusyAction(payload?: any): Promise<any> { await store.dispatch('setIsBusy'); }

    let committedProjectsFile: File | null = null;
    let applyNoTreatment: boolean = true;
    let closed: boolean = false;

    watch(showDialog, () => onShowDialogChanged)
    function onShowDialogChanged() {
        if (showDialog) {
            closed = false;
        } else {
            committedProjectsFile = null;
            closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */
    function onSubmitFileSelectorFile(file: File, treatment: boolean) {
        committedProjectsFile = hasValue(file) ? clone(file) : null;
        applyNoTreatment = treatment;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            //todo: get rid of is export portion
            const result: ImportExportCommittedProjectsDialogResult = {
                applyNoTreatment: applyNoTreatment,
                file: committedProjectsFile as File,
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
        applyNoTreatment = treatment;
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