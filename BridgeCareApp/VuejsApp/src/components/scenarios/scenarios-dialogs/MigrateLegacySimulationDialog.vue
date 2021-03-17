<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
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
                 class="ara-blue-bg white--text">
            Migrate
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';

@Component
export default class MigrateLegacySimulationDialog extends Vue {
  @Prop() showDialog: boolean;

  legacySimulationId: number = 0;

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.legacySimulationId);
    } else {
      this.$emit('submit', null);
    }

    this.legacySimulationId = 0;
  }
}
</script>
