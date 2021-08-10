<template>
  <v-layout>
    <v-dialog max-width="250px" persistent v-model="showDialog">
      <v-card>
        <v-card-title>
          <v-layout justify-center>
            <h3>New Treatment</h3>
          </v-layout>
        </v-card-title>
        <v-card-text>
          <v-layout>
            <v-text-field label="Name" outline v-model="newTreatment.name"></v-text-field>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="newTreatment.name === ''" @click="onSubmit(true)" color="info">Save
            </v-btn>
            <v-btn @click="onSubmit(false)" color="error">Cancel</v-btn>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {emptyTreatment, Treatment} from '@/shared/models/iAM/treatment';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateTreatmentDialog extends Vue {
  @Prop() showDialog: boolean;

  newTreatment: Treatment = {...emptyTreatment, id: getNewGuid(), isNew: true};

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newTreatment);
    } else {
      this.$emit('submit', null);
    }

    this.newTreatment = {...emptyTreatment, id: getNewGuid(), isNew: true};
  }
}
</script>
