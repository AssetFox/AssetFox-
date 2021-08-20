<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Cash Flow Rule Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newCashFlowRuleLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

          <v-textarea label="Description" no-resize outline rows="3"
                      v-model="newCashFlowRuleLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newCashFlowRuleLibrary.name === ''" @click="onSubmit(true)"
                 class="ara-blue-bg white--text">
            Submit
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
import Component from 'vue-class-component';
import {Prop, Watch} from 'vue-property-decorator';
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

@Component
export default class CreateCashFlowRuleLibraryDialog extends Vue {
  @Prop() dialogData: CreateCashFlowRuleLibraryDialogData;

  newCashFlowRuleLibrary: CashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newCashFlowRuleLibrary = {
      ...this.newCashFlowRuleLibrary,
      cashFlowRules: hasValue(this.dialogData.cashFlowRules)
          ? this.dialogData.cashFlowRules.map((cashFlowRule: CashFlowRule) => ({
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
      owner: getUserName()
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newCashFlowRuleLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newCashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  }
}
</script>
