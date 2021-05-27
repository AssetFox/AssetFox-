<template>
  <v-dialog max-width="500px"
            scrollable
            v-model="dialogData.showModal">
    <v-card>
      <v-card-title primary-title>
        <v-layout column>
          <v-flex>
            <v-layout justify-center>
              <h3 class="grey--text">Available Reports</h3>
            </v-layout>
          </v-flex>
          <v-progress-linear :indeterminate="true" v-if="isDownloading"></v-progress-linear>
          <v-flex>
            <span class="grey--text" v-if="isDownloading">Downloading... You can close this window, it will not stop the report generation</span>
          </v-flex>
        </v-layout>
      </v-card-title>
      <v-divider></v-divider>
      <v-card-text>
          <v-layout align-start row>
            <!-- <v-checkbox :label="item"
                        :value="item"
                        color="primary lighten-1"
                        v-model="selectedReports">
            </v-checkbox> -->
            <v-chip color='ara-blue-bg' text-color='white' @click="onDownloadSummaryReport(true)">
                            Summary Report
                        </v-chip>
            <v-chip color='ara-blue-bg' text-color='white' @click="onDownloadSimulationLog(true)">
                            Simulation Log
                        </v-chip>
          </v-layout>
        </v-list-tile>
        <v-alert :value="errorMessage !== ''"
                 color="error"
                 icon="warning"
                 outline>
          {{ errorMessage }}
        </v-alert>
      </v-card-text>
      <v-divider></v-divider>
      <v-card-actions>
        <!-- <v-layout justify-space-between row>
          <v-btn :disabled="isBusy"
                 @click="onDownloadReports(true)"
                 class="ara-blue-bg white--text">
            Download
          </v-btn>
          <v-btn :disabled="isBusy"
                 @click="onDownloadReports(false)"
                 class="ara-orange-bg white--text">
            Close
          </v-btn>
        </v-layout> -->
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {ReportsDownloaderDialogData} from '@/shared/models/modals/reports-downloader-dialog-data';
import FileDownload from 'js-file-download';
import ReportsService from '@/services/reports.service';
import {AxiosResponse} from 'axios';
import {hasValue} from '@/shared/utils/has-value-util';

@Component({})
export default class ReportsDownloaderDialog extends Vue {
  @Prop() dialogData: ReportsDownloaderDialogData;

  @State(state => state.busyModule.isBusy) isBusy: boolean;

  @Action('setSuccessMessage') setSuccessMessageAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;

  reports: string[] = [/*'Detailed Report', */'Summary Report', 'Simulation Log'];
  errorMessage: string = '';
  isDownloading: boolean = false;

  async onDownloadLog() {
    await ReportsService.downloadSimulationLog(this.dialogData.networkId, this.dialogData.scenarioId)
       .then((response: AxiosResponse<any>) => {
          this.isDownloading = false;
             if (hasValue(response, 'data')) {
               this.setSuccessMessageAction({message: 'Log downloaded'});
               FileDownload(response.data, `Simulation log ${this.dialogData.name}.txt`);
             } else {
               this.setErrorMessageAction({
               message: 'Failed to download simulation log. Please try running the simulation and downloading the log again.'
             });
           }
          });
              
  }

  async onDownloadSummaryReport(download: boolean) {
    if (download) {
      this.errorMessage = '';
      this.isDownloading = true;
      this.dialogData.showModal = false;
      await ReportsService.downloadSummaryReport(this.dialogData.networkId, this.dialogData.scenarioId)
         .then((response: AxiosResponse<any>) => {
         this.isDownloading = false;
         if (hasValue(response, 'data')) {
           this.setSuccessMessageAction({message: 'Report downloaded'});
           FileDownload(response.data, `Summary Report ${this.dialogData.name}.xlsx`);
         } else {
           this.setErrorMessageAction({
             message: 'Failed to download summary report. Please try generating and downloading the report again.'
           });
         }
        });
      }
    else {
      this.dialogData.showModal = false;
    }
  }

  async onDownloadSimulationLog(download: boolean) {
    if (download) {
      this.errorMessage = '';
      this.isDownloading = true;
      this.dialogData.showModal = false;
      await ReportsService.downloadSimulationLog(this.dialogData.networkId, this.dialogData.scenarioId)
         .then((response: AxiosResponse<any>) => {
         this.isDownloading = false;
         if (hasValue(response, 'data')) {
           this.setSuccessMessageAction({message: 'Report downloaded'});
           FileDownload(response.data, `Simulation Log ${this.dialogData.name}.xlsx`);
         } else {
           this.setErrorMessageAction({
             message: 'Failed to download simulation log. Please try generating and downloading the log again.'
           });
         }
        });
      }
    else {
      this.dialogData.showModal = false;
    }
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
