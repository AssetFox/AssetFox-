<template>
  <v-layout>
    <v-dialog max-width="450px" persistent v-model="showDialog">
      <v-card class="ghd-padding">
        <v-card-title>
            <v-layout justify-left>
              <h3 class="ghd-title">Create New Treatment</h3>
            </v-layout>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>          
        </v-card-title>
        <v-card-text>
          <v-layout column>
            <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
            <v-text-field id="CreateTreatmentDialog-name-textField" class="ghd-control-border ghd-control-text ghd-control-width-lg" outline v-model="newTreatment.name"></v-text-field>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout row justify-center>
            <v-btn
              id="CreateTreatmentDialog-cancel-btn"
              @click="onSubmit(false)" class="ghd-white-bg ghd-blue ghd-button-text" depressed>Cancel
            </v-btn>
            <v-btn 
              id="CreateTreatmentDialog-save-btn"
              :disabled="newTreatment.name === ''" 
              @click="onSubmit(true)" 
              class="ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding">Save
            </v-btn>            
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

  newTreatment: Treatment = {...emptyTreatment, id: getNewGuid(), addTreatment: false};

  onSubmit(submit: boolean) {
    if (submit) {
      this.newTreatment.addTreatment = true;
      this.$emit('submit', this.newTreatment);
    } else {
      this.$emit('submit', null);
    }

    this.newTreatment = {...emptyTreatment, id: getNewGuid(), addTreatment: false};
  }
}
</script>
