<template>
  <v-dialog max-width="450px" persistent v-model="computedShowDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="ghd-control-dialog-header"><h5>New Cash Flow Rule Library</h5></div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>

      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Rule Name</v-subheader>
            <v-text-field variant="underlined" v-model="newCashRule.name"
                          id="AddCashFlowRuleDialog-ruleName-vtextfield"
                          :rules="[rules['generalRules'].valueIsNotEmpty]"
                          class="ghd-text-field-border ghd-text-field"/>
            </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newCashRule.name === ''"
          />       
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';
import {
  CashFlowRule,
  emptyCashFlowRule,
} from '@/shared/models/iAM/cash-flow';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue'; 

const props = defineProps<{showDialog: boolean}>()
const emit = defineEmits(['submit']);
let newCashRule  = ref({...emptyCashFlowRule, id: getNewGuid()});
let inputRules: InputValidationRules = rules;
let computedShowDialog = computed(() => props.showDialog)
function disableSubmitButton() {
  return !(inputRules['generalRules'].valueIsNotEmpty(newCashRule.value.name) === true);
}
function onSubmit(submit: boolean) {
  if (submit) {
    emit('submit', newCashRule.value);
  } else {
    emit('submit', null);
  }
  newCashRule.value = {...emptyCashFlowRule, id: getNewGuid()};
}
</script>