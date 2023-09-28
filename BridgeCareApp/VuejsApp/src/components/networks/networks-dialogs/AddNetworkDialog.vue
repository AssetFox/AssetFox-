<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Network</div>
            <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline 
            id="AddNetworkDialog-NetworkName-vtextfield"
            v-model="networkName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn id="AddNetworkDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' variant = "flat">Cancel</v-btn>
          <v-btn id="AddNetworkDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outline">
            Save
          </v-btn>          
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { clone } from 'ramda';
import { AddNetworkDialogData } from '@/shared/models/modals/add-network-dialog-data';
import { ref, Ref, watch } from 'vue';

  const props = defineProps<{
    dialogData: AddNetworkDialogData
  }>()
  const emit = defineEmits(['submit'])


  let newNetwork: Network = clone(emptyNetwork);
  let rules: InputValidationRules = validationRules;
  let networkName: Ref<string> = ref('New Network');

  watch(networkName, () => onNetworkNameChanged)
  function onNetworkNameChanged() {
      newNetwork.name = networkName.value;
  }

   watch(() => props.dialogData, () => onDialogDataChanged)
  function onDialogDataChanged() {
    newNetwork = {
      ...newNetwork,
      name: networkName.value,
    }
    
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newNetwork);
    } else {
      emit('submit', null);
    }

    newNetwork = clone(emptyNetwork);
    props.dialogData.showDialog = false;
  }

</script>