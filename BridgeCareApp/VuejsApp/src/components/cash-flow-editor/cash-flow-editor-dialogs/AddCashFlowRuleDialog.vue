<template>
  <Dialog max-width="450px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Rule Name</v-subheader>
          <v-text-field outline v-model="newCashRule.name"
                        id="AddCashFlowRuleDialog-ruleName-vtextfield"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(false)" variant = "flat" class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn>
          <v-btn :disabled="newCashRule.name === ''" @click="onSubmit(true)"
                  id="AddCashFlowRuleDialog-submit-btn"
                  variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Submit
          </v-btn>        
        </v-layout>
      </v-card-actions>
    </v-card>
  </Dialog>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import {
  CashFlowRule,
  emptyCashFlowRule,
} from '@/shared/models/iAM/cash-flow';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import Dialog from 'primevue/dialog';

const props = defineProps<{showDialog: boolean}>()
const emit = defineEmits(['submit']);
let newCashRule: CashFlowRule = {...emptyCashFlowRule, id: getNewGuid()};
let inputRules: InputValidationRules = rules;

function disableSubmitButton() {
  return !(inputRules['generalRules'].valueIsNotEmpty(newCashRule.name) === true);
}
function onSubmit(submit: boolean) {
  if (submit) {
    emit('submit', newCashRule);
  } else {
    emit('submit', null);
  }
  newCashRule = {...emptyCashFlowRule, id: getNewGuid()};
}
</script>