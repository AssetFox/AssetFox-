<template>
    <v-dialog width="768px" height="540px" persistent v-model="showDialog">
        <v-card class="div-padding">
            <v-row justify="space-between" style="margin-bottom: 10px;">
                <h3 class="Montserrat-font-family">Supersede Upload</h3>
                <v-btn @click="onSubmit(false)" variant="flat">
                <i class="fas fa-times fa-2x"></i>
            </v-btn>
            </v-row>
            <v-row >
                <FileSelector :closed='closed' :useTreatment="false" @submit='onFileSelectorChange' />
            </v-row>
            <v-row justify="center">
                <CancelButton @cancel="onSubmit(false)"/>
                <UploadButton @upload="onSubmit(true)"/>                     
            </v-row>
        </v-card>
    </v-dialog>
</template>

<script setup lang='ts'>
import { ref, toRefs, watch } from 'vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportSupersedeDialogResult } from '@/shared/models/modals/import-supersede-dialog-result';
import {clone} from 'ramda';
import { useStore } from 'vuex';
import FileSelector from '@/shared/components/FileSelector.vue';
import UploadButton from '@/shared/components/buttons/UploadButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';

    const props = defineProps<{showDialog: boolean}>()
    const { showDialog } = toRefs(props);

    function addErrorNotificationAction(payload?: any) {
      store.dispatch('addErrorNotification', payload);
    }

    const SupersedeRulesFile = ref<File | null>(null);
    const closed = ref<boolean>(false);
    let store = useStore();
    const emit = defineEmits(['submit'])

    watch(showDialog, () => {
        if (showDialog.value) {
            closed.value = false;
        } else {
            SupersedeRulesFile.value = null;
            closed.value = true;
        }
    });

    /**
     * FileSelector submit event handler
     */
    function onFileSelectorChange(file: File) {
        SupersedeRulesFile.value = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    function onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportSupersedeDialogResult = {
                file: SupersedeRulesFile.value as File,
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