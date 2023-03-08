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
                            <img style="padding-right:5px; height:30px; " :src="require('@/assets/icons/urgent-info.svg')"/>
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
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue Montserrat-font-family' flat>Cancel</v-btn>
                    <v-btn @click='onSubmit(true, true)' class='ghd-white-bg ghd-blue ghd-button Montserrat-font-family' outline>Export</v-btn>
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
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import FileSelector from '@/shared/components/FileSelector.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { clone } from 'ramda';

@Component({
    components: { CommittedProjectsFileSelector: FileSelector },
})
export default class ImportExportCommittedProjectsDialog extends Vue {
    @Prop() showDialog: boolean;

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setIsBusy') setIsBusyAction: any;

    committedProjectsFile: File | null = null;
    applyNoTreatment: boolean = true;
    closed: boolean = false;

    @Watch('showDialog')
    onShowDialogChanged() {
        if (this.showDialog) {
            this.closed = false;
        } else {
            this.committedProjectsFile = null;
            this.closed = true;
        }
    }

    /**
     * FileSelector submit event handler
     */
    onSubmitFileSelectorFile(file: File, treatment: boolean) {
        this.committedProjectsFile = hasValue(file) ? clone(file) : null;
        this.applyNoTreatment = treatment;
    }

    /**
     * Dialog submit event handler
     */
    onSubmit(submit: boolean, isExport: boolean = false) {
        if (submit) {
            const result: ImportExportCommittedProjectsDialogResult = {
                applyNoTreatment: this.applyNoTreatment,
                file: this.committedProjectsFile as File,
                isExport: isExport,
            };
            this.$emit('submit', result);
        } else {
            this.$emit('submit', null);
        }
    }
    /**
     * Apply no treatment event handler
     */
    onTreatmentChanged(treatment: boolean) {
        this.applyNoTreatment = treatment;
    }
    /**
     * Dialog delete event handler
     */
    onDelete() {
        this.$emit('delete');
    }
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