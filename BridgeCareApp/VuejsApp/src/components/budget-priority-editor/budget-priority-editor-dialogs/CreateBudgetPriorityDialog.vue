<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Budget Priority</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Priority Level" outline v-model.number="newBudgetPriority.priorityLevel"
                        :mask="'##########'" :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-text-field label="Year" outline v-model.number="newBudgetPriority.year"
                        :mask="'####'"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="disableSubmitButton()" @click="onSubmit(true)" class="ara-blue-bg white--text">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
            Cancel
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
