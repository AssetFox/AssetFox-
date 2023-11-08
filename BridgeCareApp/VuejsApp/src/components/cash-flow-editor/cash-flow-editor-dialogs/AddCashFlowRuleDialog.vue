<template>
  <v-dialog max-width="450px" persistent v-model="computedShowDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-subheader class="ghd-md-gray ghd-control-label">Rule Name</v-subheader>
          <v-text-field variant="underlined" v-model="newCashRule.name"
                        id="AddCashFlowRuleDialog-ruleName-vtextfield"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-space-between row>
          <v-btn @click="onSubmit(false)" variant = "flat" class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn>
          <v-btn :disabled="newCashRule.name === ''" @click="onSubmit(true)"
                  id="AddCashFlowRuleDialog-submit-btn"
                  variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Submit
          </v-btn>        
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