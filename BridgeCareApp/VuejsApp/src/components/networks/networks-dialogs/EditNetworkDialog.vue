<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify-space-between align-center>
            <div class="ghd-control-dialog-header">Edit Network Name</div>
          </v-row>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-text-field label="Network Name" id="EditNetworkDialog-Name-vtextField"
            v-model="networkName"
            class="ghd-text-field-border ghd-text-field"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-center row>
          <v-btn @click="onSubmit(false)" class="cancel-btn" style="margin-right:auto; margin-left:auto;">Cancel</v-btn>
          <v-btn @click="onSubmit(true)" class="save-btn" style="margin-right:auto; margin-left:auto;">
            Save
          </v-btn>          
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';

const props = defineProps<{
  dialogData: { showDialog: boolean }
  initialNetworkName: string
}>();
const emit = defineEmits(['submit']);

let showDialog = computed(() => props.dialogData.showDialog);
let networkName = ref<string>(props.initialNetworkName);

// Watch for changes to the network name and update networkName
watch(() => props.initialNetworkName, (newName) => {
  networkName.value = newName;
});

// Submit function
function onSubmit(submit: boolean) {
  if (submit) {
    emit('submit', networkName.value); // Emit the new network name
  } else {
    networkName = ref<string>(props.initialNetworkName);
    emit('submit', null); // Cancel action
  }
  props.dialogData.showDialog = false; // Close the dialog
}
</script>
