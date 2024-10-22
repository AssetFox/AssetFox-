<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify="space-between" align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" variant = "flat" 
              id="CreateCashFlowRuleLibraryDialog-Close-vbtn"
              class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline 
                        id="CreateCashFlowRuleLibraryDialog-Name-vTextField"
                        variant="underlined"
                        v-model="newCashFlowRuleLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          <v-textarea no-resize outline rows="3"
                      variant="outlined"
                      id="CreateCashFlowRuleLibraryDialog-Description-vtextarea"
                      v-model="newCashFlowRuleLibrary.description"
                      class="ghd-text-field-border"/>
          </v-col>       
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newCashFlowRuleLibrary.name === ''"  
          />       
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed, reactive, ref, watch } from 'vue';
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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue'; 

  let store = useStore();

  let getIdByUserNameGetter: any = store.getters.getIdByUserName;

  const props = defineProps<{dialogData: CreateCashFlowRuleLibraryDialogData}>()
  let showDialogComputed = computed(() => props.dialogData.showDialog);
  const emit = defineEmits(['submit']);

  let newCashFlowRuleLibrary = ref<CashFlowRuleLibrary>({...emptyCashFlowRuleLibrary, id: getNewGuid()});
  let inputRules: InputValidationRules = clone(rules);
  const dialogData = reactive(props.dialogData);

  watch(() => props.dialogData, () => onDialogDataChanged)
  function onDialogDataChanged() {
    let currentUser: string = getUserName();

    newCashFlowRuleLibrary.value = {
      ...newCashFlowRuleLibrary.value,
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
      emit('submit', newCashFlowRuleLibrary.value);
    } else {
      emit('submit', null);
    }

    newCashFlowRuleLibrary.value = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  }
</script>
