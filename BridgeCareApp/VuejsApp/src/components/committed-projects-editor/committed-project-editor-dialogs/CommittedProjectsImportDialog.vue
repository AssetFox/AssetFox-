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

                        <div style="margin: 40px;">
                        <CommittedProjectsFileSelector :closed='closed' useTreatment='true' @submit='onSubmitFileSelectorFile' />
                        </div>                    
                    </v-row>
                    <v-row justify="center" style="margin: 5px;width: 100%;">
                        <span class="div-warning-border" style="margin: 5px;">
                            <v-row align="start" style="padding:8px;">
                                <v-icon class="px-2 icon-color">fas fa-exclamation-triangle</v-icon>
                                <h3 class="h3-color">Warning</h3>
                            </v-row>
                            <p style="font-family: roboto;">
                                Uploading new committed projects will override ALL previous commitments.
                                Committed projects may take a few minutes to process.
                            </p>
                        </span>
                    </v-row>
                    <v-row justify="center" style="margin: 5px;">
                        <CancelButton @cancel="onSubmit(false)"/>
                        <UploadButton @upload="onSubmit(true)"/>
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
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import UploadButton from '@/shared/components/buttons/UploadButton.vue';

    let store = useStore();
    const props = defineProps<{showDialog: boolean}>();
    const { showDialog } = toRefs(props);
    const emit = defineEmits(['submit', 'delete']);

    function addErrorNotificationAction(payload?: any) { store.dispatch('addErrorNotification',payload); }

    const committedProjectsFile = ref< File | null > ( null );
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
    function onSubmitFileSelectorFile(file: File) {
        committedProjectsFile.value = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            //todo: get rid of is export portion
            const result: ImportExportCommittedProjectsDialogResult = {
                file: committedProjectsFile.value as File,
                isExport: isExport,
            };
            emit('submit', result);
        } else {
            emit('submit', null);
        }
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