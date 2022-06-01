<template>
    <v-dialog max-width='500px' persistent v-model='showDialog'>
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Treatments Upload</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <TreatmentsFileSelector :closed='closed' @submit='onFileSelectorChange' />                    
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between>
                    <v-btn @click='onSubmit(true)' class='ara-blue-bg white--text'>Upload</v-btn>                    
                    <v-btn @click='onSubmit(false)' class='ara-orange-bg white--text'>Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action } from 'vuex-class';
import { hasValue } from '@/shared/utils/has-value-util';
import { ImportExportTreatmentsDialogResult } from '@/shared/models/modals/import-export-treatments-dialog-result';
import {clone} from 'ramda';
import TreatmentsFileSelector from '@/shared/components/FileSelector.vue';

@Component({
    components: { TreatmentsFileSelector }
})
export default class ImportExportTreatmentsDialog extends Vue {
    @Prop() showDialog: boolean;

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setIsBusy') setIsBusyAction: any;

    TreatmentsFile: File | null = null;
    overwriteBudgets: boolean = true;
    closed: boolean = false;

    @Watch('showDialog')
    onShowDialogChanged() {
        if (this.showDialog) {
            this.closed = false;
        } else {
            this.TreatmentsFile = null;
            this.closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */
    onFileSelectorChange(file: File) {
        this.TreatmentsFile = hasValue(file) ? clone(file) : null;
    }

    /**
     * Dialog submit event handler
     */
    onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportTreatmentsDialogResult = {
                file: this.TreatmentsFile as File,
                isExport: isExport
            };
            this.$emit('submit', result);
        } else {
            this.$emit('submit', null);
        }
    }
}
</script>
