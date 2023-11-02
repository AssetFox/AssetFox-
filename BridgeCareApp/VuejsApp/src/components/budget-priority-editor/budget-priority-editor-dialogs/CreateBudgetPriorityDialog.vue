<template>
  <v-dialog width="50%" persistent v-model="showDialog">
    <v-card>    
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="ghd-control-dialog-header"><h5>New Budget Priority</h5></div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
          </v-btn>
        </v-row>
      </v-card-title>

      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Priority Level</v-subheader>       
            <v-text-field id="CreateBudgetPriorityDialog-priorityLevel-vtextfield"
                          v-model.number="newBudgetPriority.priorityLevel" 
                          :mask="'##########'" :rules="[rules['generalRules'].valueIsNotEmpty]"
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"/>
          
            <v-subheader class="ghd-md-gray ghd-control-label">Year</v-subheader>
          
            <v-text-field id="CreateBudgetPriorityDialog-year-vtextfield" 
                          v-model.number="newBudgetPriority.year"
                          :mask="'####'"
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"/>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <v-btn id="CreateBudgetPriorityDialog-cancel-vbtn"
                 @click="onSubmit(false)"
                 class='ghd-blue ghd-button-text ghd-button' variant = "outlined">
            Cancel
          </v-btn >
          <v-btn id="CreateBudgetPriorityDialog-save-vbtn"
                 :disabled="disableSubmitButton()"
                 @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-button' variant = "outlined" >
            Save
          </v-btn>         
        </v-row>
      </v-card-actions>

    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { toRefs, ref } from 'vue';
import {BudgetPriority, emptyBudgetPriority} from '@/shared/models/iAM/budget-priority';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

  const props = defineProps({
    showDialog: Boolean
  })
  const { showDialog } = toRefs(props);

  const emit = defineEmits(['submit'])

  let newBudgetPriority = ref<BudgetPriority>({...emptyBudgetPriority, id: getNewGuid()});
  let rules: InputValidationRules = validationRules;

  function disableSubmitButton() {
    return !(rules['generalRules'].valueIsNotEmpty(newBudgetPriority.value.priorityLevel) === true);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newBudgetPriority.value);
    } else {
      emit('submit', null);
    }

    newBudgetPriority.value = {...emptyBudgetPriority, id: getNewGuid()};
  }
</script>
