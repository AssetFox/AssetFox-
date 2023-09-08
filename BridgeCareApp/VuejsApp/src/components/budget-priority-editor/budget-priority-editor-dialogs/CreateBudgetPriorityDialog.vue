<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Budget Priority</div>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Priority Level</v-subheader>
          <v-text-field id="CreateBudgetPriorityDialog-priorityLevel-vtextfield" outline v-model.number="newBudgetPriority.priorityLevel"
                        :mask="'##########'" :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Year</v-subheader>
          <v-text-field id="CreateBudgetPriorityDialog-year-vtextfield" outline v-model.number="newBudgetPriority.year"
                        :mask="'####'" class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn id="CreateBudgetPriorityDialog-cancel-vbtn" @click="onSubmit(false)" flat class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn >
          <v-btn id="CreateBudgetPriorityDialog-save-vbtn" :disabled="disableSubmitButton()" @click="onSubmit(true)" outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
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
