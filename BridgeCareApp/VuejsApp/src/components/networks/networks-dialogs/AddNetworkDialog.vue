<template>
  <v-dialog max-width="450px" persistent v-model ="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Network</div>
          </v-row>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-text-field outline 
            label="Name"
            id="AddNetworkDialog-NetworkName-vtextfield"
            v-model="networkName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-center row>
          <v-btn id="AddNetworkDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button'  style="margin-right:auto; margin-left:auto;" variant = "flat">Cancel</v-btn>
          <v-btn id="AddNetworkDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' style="margin-right:auto; margin-left:auto;" variant = "outlined">
            Save
          </v-btn>          
        </v-row>
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