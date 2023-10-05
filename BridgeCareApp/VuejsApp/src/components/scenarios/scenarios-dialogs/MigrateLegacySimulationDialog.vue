<template>
  <Dialog max-width="450px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Id of Legacy Simulation to Migrate</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Legacy Simulation Id" outline v-model="legacySimulationId"></v-text-field>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="!(legacySimulationId > 0)" @click="onSubmit(true)"
                 class="ara-blue-bg text-white">
            Migrate
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg text-white">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  <!-- </v-dialog> -->
  </Dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue'; 
import Dialog from 'primevue/dialog';

  const emit = defineEmits(['submit'])
  const props = defineProps<{showDialog: boolean}>();


  let legacySimulationId: number = 0;

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', legacySimulationId);
    } else {
      emit('submit', null);
    }

    legacySimulationId = 0;
  }

</script>
