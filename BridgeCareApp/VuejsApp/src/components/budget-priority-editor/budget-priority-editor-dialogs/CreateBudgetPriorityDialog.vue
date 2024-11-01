<template>
  <v-dialog width="50%" persistent v-model="showDialog">
    <v-card>    
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="ghd-control-dialog-header"><h5>New Budget Priority</h5></div>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>

      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Priority Level</v-subheader>       
            <v-text-field id="CreateBudgetPriorityDialog-priorityLevel-vtextfield"
                          v-model="newBudgetPriority.priorityLevel" 
                          v-maska:[priorityMask] :rules="[rules['generalRules'].valueIsNotEmpty]"
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"/>
          
            <v-subheader class="ghd-md-gray ghd-control-label">Year</v-subheader>
          
            <v-text-field id="CreateBudgetPriorityDialog-year-vtextfield" 
                          v-model="newBudgetPriority.year"
                          v-maska:[yearMask]
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"/>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="disableSubmitButton()"
          />        
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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  const props = defineProps({
    showDialog: Boolean
  })
  const { showDialog } = toRefs(props);

  const emit = defineEmits(['submit'])

  const priorityMask = { mask: '##########' };
  const yearMask = { mask: '####' };

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
