<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Data Source</div>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline id="CreateDataSourceDialog-Name-vtextField"
            v-model="datasourceName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn id="CreateDataSourceDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' flat>Cancel</v-btn>
          <v-btn id="CreateDataSourceDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
            Save
          </v-btn>          
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { Datasource, emptyDatasource, DSSQL } from '../../../shared/models/iAM/data-source';
import { CreateDataSourceDialogData} from '@/shared/models/modals/data-source-dialog-data'
import { Getter } from 'vuex-class';
import { getUserName } from '@/shared/utils/get-user-info';

@Component
export default class CreateDataSourceDialog extends Vue {
  @Prop() dialogData: CreateDataSourceDialogData;
  
  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newDataSource: Datasource = emptyDatasource;
  rules: InputValidationRules = rules;
  datasourceName: string = 'New Data Source';


  @Watch('datasourceName')
    onDataSourceNameChanged() {
        this.newDataSource.name = this.datasourceName;
    }
  @Watch('dialogData')
  onDialogDataChanged() {
    this.newDataSource = {
        id: getNewGuid(),
        createdBy: this.getIdByUserNameGetter(getUserName()),
        name: this.datasourceName,
        type: DSSQL,
        connectionString: '',
        dateColumn: '',
        locationColumn: '',
        secure: false
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newDataSource);
    } else {
      this.$emit('submit', null);
    }

    this.newDataSource = emptyDatasource;
    this.dialogData.showDialog = false;
  }
}
</script>