<template>
    <v-dialog width="768px" height="540px" persistent v-model='showDialogComputed'>
        <v-card class="div-padding">
            <v-card-title class="pa-2">
                <v-row justify="space-between">
                    <v-col class="pt-0">
                        <h3  style="padding-top: 10px;" class="Montserrat-font-family">Performance Curves Upload</h3>
                    </v-col>
                    <v-spacer/>
                    <v-col class="pt-0" align="end">
                        <XButton @click="onSubmit(false)"/>
                    </v-col> 
                </v-row>              
            </v-card-title>
            <v-card-text >
                <v-row column>
                    <PerformanceCurvesFileSelector :closed='closed' @submit='onFileSelectorChange' />                    
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
</template>

<script lang='ts' setup>
import Vue, { computed } from 'vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportPerformanceCurvesDialogResult } from '@/shared/models/modals/import-export-performance-curves-dialog-result';
import {clone} from 'ramda';
import PerformanceCurvesFileSelector from '@/shared/components/FileSelector.vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import UploadButton from '@/shared/components/buttons/UploadButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

const emit = defineEmits(['submit'])
let store = useStore();
const props = defineProps<{
    showDialog: boolean
    }>()
    let showDialogComputed = computed(() => props.showDialog);
function addErrorNotificationAction(payload?: any) {store.dispatch('addErrorNotification');}

    let PerformanceCurvesFile: File | null = null;
    let overwriteBudgets: boolean = true;
    let closed: boolean = false;

    watch(()=>props.showDialog,()=>onShowDialogChanged())
    function onShowDialogChanged() {
        if (props.showDialog) {
            closed = false;
        } else {
            PerformanceCurvesFile = null;
            closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */

    function onFileSelectorChange(file: File) {
        PerformanceCurvesFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportPerformanceCurvesDialogResult = {
                file: PerformanceCurvesFile as File,
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