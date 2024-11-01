<template>
    <v-btn
      class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
      @click="onDownloadSimulationLog"
      variant="flat"
    >
      Simulation Log
    </v-btn>
  </template>
  
  <script setup lang="ts">
  import { useStore } from 'vuex';
  import { downloadSimulationLog } from '@/shared/utils/simulation-log-utils';
  
  const props = defineProps({
    networkId: {
      type: String,
      required: true,
    },
    selectedScenarioId: {
      type: String,
      required: true,
    },
    simulationName: {
      type: String,
      required: true,
    },
  });
  
  const emit = defineEmits(['success', 'error']);
  const store = useStore();

  function addSuccessNotificationAction(payload: any) {
    store.dispatch('addSuccessNotification', payload);
  }

  function addErrorNotificationAction(payload: any) {
    store.dispatch('addErrorNotification', payload);
  }
  
  async function onDownloadSimulationLog() {
    await downloadSimulationLog(
      props.networkId,
      props.selectedScenarioId,
      props.simulationName,
      addSuccessNotificationAction,
      addErrorNotificationAction,
    );
}
  </script>
  
  <style scoped>
  /* Include any specific styles here */
  </style>
  