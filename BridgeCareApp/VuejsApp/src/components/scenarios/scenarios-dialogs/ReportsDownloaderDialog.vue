<template>
  <v-dialog max-width="500px" scrollable v-model="showDialogComputed">
    <v-card>
      <v-card-title primary-title>
        <v-row column>
          <v-col>
            <v-row justify-center>
              <h3 class="text-grey">Available Reports</h3>
            </v-row>
          </v-col>
          <v-progress-linear :indeterminate="true" v-if="isDownloading"></v-progress-linear>
          <v-col>
            <span class="text-grey" v-if="isDownloading">Downloading... You can close this window, it will not stop the report generation</span>
          </v-col>
        </v-row>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
          <v-row align-start row>
            <v-select style="padding:0px !important"
                :items='reports'
                v-model='selectedReport'>
            </v-select>
            <v-chip
                round
                color="ara-blue-bg"
                text-color="white"
                @click="onGenerateReport(true)"
                :disabled="selectedReport === ''"
            >
                Generate Report
            </v-chip>
            <v-divider vertical></v-divider>
            <v-chip color='ara-blue-bg' text-color='white' @click="onDownloadSimulationLog(true)">
                Simulation Log
            </v-chip>
          </v-row>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
          <v-chip
                 @click="onDownloadReport()"
                 class="green darken-2 text-white">
            Download report
          </v-chip>
      </v-card-actions>
    </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { Ref, ref, shallowReactive, shallowRef, watch, onMounted, computed } from 'vue'; 

import {ReportsDownloaderDialogData} from '@/shared/models/modals/reports-downloader-dialog-data';
import ReportsService from '@/services/reports.service';
import {AxiosResponse} from 'axios';
import { FileInfo } from '@/shared/models/iAM/file-info';
import FileDownload from 'js-file-download';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone } from 'ramda';
import { useStore } from 'vuex'; 

    let store = useStore(); 

    const props = defineProps<{dialogData: ReportsDownloaderDialogData}>();
    let showDialogComputed = computed(() => props.dialogData.showModal);
    let isBusy = ref<boolean>(store.state.busyModule.isBusy);

    let stateSimulationReportNames: string[] = (store.state.adminDataModule.simulationReportNames)

    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification')}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification')}
    async function getSimulationReportsAction(payload?: any): Promise<any>{await store.dispatch('getSimulationReports')}

    let reports: SelectItem[] = [];   
    let selectedReport: string = ''; 
    let isDownloading: boolean = false;
    let reportIndexID: string = getBlankGuid();

    onMounted(()=> mounted)
    function mounted() {
        getSimulationReportsAction().then(() => {
            //const reports: string[] = clone(stateSimulationReportNames)
            let reports: string[] = clone(stateSimulationReportNames)
            // ToDo - Had to Comment the below and change above from const to variable
            // reports = reports.map(rep => {
            //     return {text: rep, value: rep}
            // })

            if(reports.length > 0)
                selectedReport = reports[0];
        })       
    }

    async function onGenerateReport(download: boolean) {
        if (download) {            
            isDownloading = true;
            props.dialogData.showModal = false;
            await ReportsService.generateReport(
                props.dialogData.scenarioId, selectedReport
            ).then((response: AxiosResponse<any>) => {
                isDownloading = false;
                if (response.status == 200) {
                    if (hasValue(response, 'data')) {
                        const resultId: string = response.data as string;
                        //reportIndexID = resultId;
                    }

                    addSuccessNotificationAction({
                        message: selectedReport +  ' report generation started for ' + props.dialogData.name + '.',
                    });
                } else {
                    addErrorNotificationAction({
                        message: 'Failed to generate apricot for ' + props.dialogData.name + '.',
                        longMessage:
                            'Failed to generate the report. Make sure the scenario has been run',
                    });
                }
            });
        } else {
            props.dialogData.showModal = false;
        }
    }

    async function onDownloadReport() {        
        isDownloading = true;
        props.dialogData.showModal = false;        
        await ReportsService.downloadReport(
            props.dialogData.scenarioId, selectedReport
        ).then((response: AxiosResponse<any>) => {
            isDownloading = false;
            if (hasValue(response, 'data')) {
                const fileInfo: FileInfo = response.data as FileInfo;
                FileDownload(convertBase64ToArrayBuffer(fileInfo.fileData), fileInfo.fileName, fileInfo.mimeType);
            } else {
                addErrorNotificationAction({
                    message: 'Failed to download report.',
                    longMessage:
                        'Failed to download the summary report. Make sure the scenario has been run',
                });
            }
        });
    }

    async function onDownloadSimulationLog(download: boolean) {
        if (download) {            
            isDownloading = true;
            props.dialogData.showModal = false;
            await ReportsService.downloadSimulationLog(
                props.dialogData.networkId,
                props.dialogData.scenarioId,
            ).then((response: AxiosResponse<any>) => {
                isDownloading = false;
                if (hasValue(response, 'data')) {
                    addSuccessNotificationAction({
                        message: 'Report downloaded',
                    });
                    FileDownload(
                        response.data,
                        `Simulation Log ${props.dialogData.name}.txt`,
                    );
                } else {
                    addErrorNotificationAction({
                        message: 'Failed to download simulation log.',
                        longMessage:
                            'Failed to download simulation log. Please try generating and downloading the log again.',
                    });
                }
            });
        } else {
            props.dialogData.showModal = false;
        }
    }

</script>

<style>
.missing-attributes-card-text {
    max-height: 300px;
    max-width: 300px;
    overflow-y: auto;
}
</style>
