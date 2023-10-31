<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" variant = "flat" 
              id="CreateCashFlowRuleLibraryDialog-Close-vbtn"
              class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline 
                        id="CreateCashFlowRuleLibraryDialog-Name-vTextField"
                        v-model="newCashFlowRuleLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          <v-textarea no-resize outline rows="3"
                      id="CreateCashFlowRuleLibraryDialog-Description-vtextarea"
                      v-model="newCashFlowRuleLibrary.description"
                      class="ghd-text-field-border"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify-space-between row>
          <v-btn @click="onSubmit(false)" variant = "outlined" 
                 id="CreateCashFlowRuleLibraryDialog-Cancel-vbtn"
                 class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn>
          <v-btn :disabled="newCashFlowRuleLibrary.name === ''" @click="onSubmit(true)"
          variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                 id="CreateCashFlowRuleLibraryDialog-Create-vbtn">
            Submit
          </v-btn>        
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed, reactive, watch } from 'vue';
import {CreateCashFlowRuleLibraryDialogData} from '@/shared/models/modals/create-cash-flow-rule-library-dialog-data';
import {
  CashFlowDistributionRule,
  CashFlowRule,
  CashFlowRuleLibrary,
  emptyCashFlowRuleLibrary
} from '@/shared/models/iAM/cash-flow';
import {hasValue} from '@/shared/utils/has-value-util';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { useStore } from 'vuex';

  let store = useStore();

  let getIdByUserNameGetter: any = store.getters.getIdByUserName;

  const props = defineProps<{dialogData: CreateCashFlowRuleLibraryDialogData}>()
  let showDialogComputed = computed(() => props.dialogData.showDialog);
  const emit = defineEmits(['submit']);

  let newCashFlowRuleLibrary: CashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  let inputRules: InputValidationRules = clone(rules);
  const dialogData = reactive(props.dialogData);

  watch(() => props.dialogData, () => onDialogDataChanged)
  function onDialogDataChanged() {
    let currentUser: string = getUserName();

    newCashFlowRuleLibrary = {
      ...newCashFlowRuleLibrary,
      cashFlowRules: hasValue(dialogData.cashFlowRules)
          ? dialogData.cashFlowRules.map((cashFlowRule: CashFlowRule) => ({
            ...cashFlowRule,
            id: getNewGuid(),
            cashFlowDistributionRules: hasValue(cashFlowRule.cashFlowDistributionRules)
                ? cashFlowRule.cashFlowDistributionRules.map((distributionRule: CashFlowDistributionRule) => ({
                  ...distributionRule,
                  id: getNewGuid()
                }))
                : []
          }))
          : [],
        owner: getIdByUserNameGetter(currentUser),
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newCashFlowRuleLibrary);
    } else {
      emit('submit', null);
    }

    newCashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  }
</script>
