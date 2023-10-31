<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Budget Priority</div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-subheader class="ghd-md-gray ghd-control-label">Priority Level</v-subheader>
          <v-text-field id="CreateBudgetPriorityDialog-priorityLevel-vtextfield" outline v-model.number="newBudgetPriority.priorityLevel"
                        :mask="'##########'" :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Year</v-subheader>
          <v-text-field id="CreateBudgetPriorityDialog-year-vtextfield" outline v-model.number="newBudgetPriority.year"
                        :mask="'####'" class="ghd-text-field-border ghd-text-field"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-center row>
          <v-btn id="CreateBudgetPriorityDialog-cancel-vbtn" @click="onSubmit(false)" variant = "flat" class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn >
          <v-btn id="CreateBudgetPriorityDialog-save-vbtn" :disabled="disableSubmitButton()" @click="onSubmit(true)" variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>         
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { computed } from 'vue';
import {BudgetPriority, emptyBudgetPriority} from '@/shared/models/iAM/budget-priority';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

  const props = defineProps({
    showDialog: Boolean
  })
  let showDialogComputed = computed(() => props.showDialog);
  const emit = defineEmits(['submit'])

  let newBudgetPriority: BudgetPriority = {...emptyBudgetPriority, id: getNewGuid()};
  let rules: InputValidationRules = validationRules;

  function disableSubmitButton() {
    return !(rules['generalRules'].valueIsNotEmpty(newBudgetPriority.priorityLevel) === true);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newBudgetPriority);
    } else {
      emit('submit', null);
    }

    newBudgetPriority = {...emptyBudgetPriority, id: getNewGuid()};
  }
</script>
