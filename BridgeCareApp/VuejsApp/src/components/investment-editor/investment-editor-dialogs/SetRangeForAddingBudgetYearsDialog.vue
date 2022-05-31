<template>
  <v-dialog max-width="200px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Set Number of Years to Add</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-text-field type="number" min=1 :mask="'##########'" label="Edit" outline v-model.number="range"/>
        <label>{{rangeLabel}}</label>
      </v-card-text>
      <v-card-actions>
        <v-btn :disabled="range === 0" @click="onSubmit(true)" class="ara-blue-bg white--text">Save</v-btn>
        <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';

@Component
export default class SetRangeForAddingBudgetYearsDialog extends Vue {
  @Prop() showDialog: boolean;
  @Prop() startYear : number;  

  range: number = 1;

  get rangeLabel() {
    return 'Year Range: ' + (this.range <= 1 ? this.startYear : this.startYear + '-' + (this.startYear + this.range - 1));
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.range);
    } else {
      this.$emit('submit', 0);
    }

    this.range = 1;
  }
}
</script>
