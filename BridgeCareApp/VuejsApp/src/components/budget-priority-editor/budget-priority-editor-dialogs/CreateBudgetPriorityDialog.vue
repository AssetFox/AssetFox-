<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-space-between align-center>
          <h3>New Budget Priority</h3>
          <v-btn @click="onSubmit(false)" flat class="header-cancel">
                <h4>X</h4>
          </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-subheader class="ghd-subheader">Priority Level</v-subheader>
          <v-text-field outline v-model.number="newBudgetPriority.priorityLevel"
                        :mask="'##########'" :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ara-text-field-border ara-text-field"/>
          <v-subheader class="ghd-subheader">Year</v-subheader>
          <v-text-field outline v-model.number="newBudgetPriority.year"
                        :mask="'####'" class="ara-text-field-border ara-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-center row>
          <v-btn @click="onSubmit(false)" flat color="#2A578D">
            Cancel
          </v-btn >
          <v-btn :disabled="disableSubmitButton()" @click="onSubmit(true)" outline color="#2A578D">
            Save
          </v-btn>         
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop} from 'vue-property-decorator';
import {BudgetPriority, emptyBudgetPriority} from '@/shared/models/iAM/budget-priority';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreatePriorityDialog extends Vue {
  @Prop() showDialog: boolean;

  newBudgetPriority: BudgetPriority = {...emptyBudgetPriority, id: getNewGuid()};
  rules: InputValidationRules = rules;

  disableSubmitButton() {
    return !(this.rules['generalRules'].valueIsNotEmpty(this.newBudgetPriority.priorityLevel) === true);
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newBudgetPriority);
    } else {
      this.$emit('submit', null);
    }

    this.newBudgetPriority = {...emptyBudgetPriority, id: getNewGuid()};
  }
}
</script>
