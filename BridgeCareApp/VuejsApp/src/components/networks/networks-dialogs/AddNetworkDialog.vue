<template>
  <v-dialog max-width="450px" persistent v-model ="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify="space-between" align="center">
            <div class="ghd-control-dialog-header">New Network</div>
          </v-row>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>      
            <v-text-field variant="outlined" 

            id="AddNetworkDialog-NetworkName-vtextfield"
            v-model="networkName"
            class="ghd-text-field-border ghd-text-field"/>
          </v-col>
          
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <v-btn id="AddNetworkDialog-Cancel-vbtn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button'   variant = "flat">Cancel</v-btn>
          <v-btn id="AddNetworkDialog-Save-vbtn" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'  variant = "outlined">
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


  let newNetwork = ref<Network>(clone(emptyNetwork));
  let rules: InputValidationRules = validationRules;
  let networkName = ref<string>('New Network');

  watch(networkName, () =>  {
      newNetwork.value.name = networkName.value;
  })

   watch(() => props.dialogData, () =>  {
    newNetwork.value = {
      ...newNetwork.value,
      name: networkName.value,
    }
  })

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newNetwork.value);
    } else {
      emit('submit', null);
    }

    newNetwork.value = clone(emptyNetwork);
    props.dialogData.showDialog = false;
  }

</script>