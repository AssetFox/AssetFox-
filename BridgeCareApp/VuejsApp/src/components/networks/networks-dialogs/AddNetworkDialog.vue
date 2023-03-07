<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Network</div>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline 
            v-model="networkName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' flat>Cancel</v-btn>
          <v-btn @click="onSubmit(true)"
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
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { clone } from 'ramda';
import { AddNetworkDialogData } from '@/shared/models/modals/add-network-dialog-data';

@Component
export default class AddNetworkDialog extends Vue {
  @Prop() dialogData: AddNetworkDialogData;

  newNetwork: Network = clone(emptyNetwork);
  rules: InputValidationRules = rules;
  networkName: string = 'New Network';


  @Watch('networkName')
    onNetworkNameChanged() {
        this.newNetwork.name = this.networkName;
    }
  @Watch('dialogData')
  onDialogDataChanged() {
    this.newNetwork = {
      ...this.newNetwork,
      name: this.networkName,
    }
    
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newNetwork);
    } else {
      this.$emit('submit', null);
    }

    this.newNetwork = clone(emptyNetwork);
    this.dialogData.showDialog = false;
  }
}
</script>