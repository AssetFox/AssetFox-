<template>
    <v-dialog max-width='500px' width="768px" height="540px" persistent v-model='showDialog'>
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>Committed Projects</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <CommittedProjectsFileSelector :closed='closed' @submit='onSubmitFileSelectorFile' />
                    <v-flex xs12>
                        <v-layout justify-start>
                            <v-checkbox label='No Treatment' v-model='applyNoTreatment'></v-checkbox>
                        </v-layout>
                    </v-flex>
                    <span class="div-warning-border">
                        <v-layout align-center><v-icon>fas fa-exclamation-triangle</v-icon><h3>Warning</h3></v-layout>
                        <p>
                            Uploading new committed projects will override ALL previous commitments.
                            Committed projects may take a few minutes to process. You will receive an email when this process is complete.
                        </p>
                    </span>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn @click='onSubmit(false)' class='ghd-white-bg ghd-blue' flat>Cancel</v-btn>
                    <v-btn @click='onSubmit(true, true)' class='ghd-white-bg ghd-blue' outline>Export</v-btn>
                    <v-btn @click='onSubmit(true)' class='ghd-white-bg ghd-blue' outline>Upload</v-btn>
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
    onSubmitFileSelectorFile(file: File) {
        this.committedProjectsFile = hasValue(file) ? clone(file) : null;
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
</style>