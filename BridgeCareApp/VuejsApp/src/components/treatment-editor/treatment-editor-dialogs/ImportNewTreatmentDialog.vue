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
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' flat>Cancel</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' outline>Upload</v-btn>                    
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
import { ImportNewTreatmentDialogResult } from '@/shared/models/modals/import-new-treatment-dialog-result';
import {clone} from 'ramda';
import TreatmentsFileSelector from '@/shared/components/FileSelector.vue';

@Component({
    components: { TreatmentsFileSelector }
})

export default class ImportNewTreatmentDialog extends Vue {
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
            const result: ImportNewTreatmentDialogResult = {
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
<style scoped>
.div-padding {
    padding: 30px;
}
</style>