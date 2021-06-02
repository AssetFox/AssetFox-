<template>
  <v-dialog max-width="200px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>Set Number of Years to Delete</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-text-field type="number" :mask="'##########'" label="Edit" single-line
          v-model.number="range"
          :min="1"
          :max="maxRange"
          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(range, [1,maxRange])]"/>
        <label>{{rangeLabel}}</label>
      </v-card-text>
      <v-card-actions>
        <v-btn :disabled="range === 0 || range > maxRange" @click="onSubmit(true)" class="ara-blue-bg white--text">Save</v-btn>
        <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';

@Component
export default class SetRangeForDeletingBudgetYearsDialog extends Vue {
  @Prop() showDialog: boolean;
  @Prop() endYear : number;  
  @Prop() maxRange : number;  

  range: number = 1;

  rules: InputValidationRules = rules;

  get rangeLabel() {
    return 'Year Range: ' + (this.range <= 1 ? this.endYear : (this.endYear - this.range + 1) + '-' + this.endYear);
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
