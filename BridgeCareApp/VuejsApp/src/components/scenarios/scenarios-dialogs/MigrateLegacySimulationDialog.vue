<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>Id of Legacy Simulation to Migrate</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-row column>
          <v-text-field label="Legacy Simulation Id" outline v-model="legacySimulationId"></v-text-field>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-row justify-space-between row>
          <v-btn :disabled="!(legacySimulationId > 0)" @click="onSubmit(true)"
                 class="ara-blue-bg text-white">
            Migrate
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg text-white">Cancel</v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  <!-- </v-dialog> -->
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue'; 

  const emit = defineEmits(['submit'])
  const props = defineProps<{showDialog: boolean}>();
  let showDialogComputed = computed(() => props.showDialog);

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
