<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" flat 
              id="CreateCashFlowRuleLibraryDialog-Close-vbtn"
              class="ghd-close-button">
              X
            </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
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
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(false)" outline 
                 id="CreateCashFlowRuleLibraryDialog-Cancel-vbtn"
                 class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn>
          <v-btn :disabled="newCashFlowRuleLibrary.name === ''" @click="onSubmit(true)"
                 outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'
                 id="CreateCashFlowRuleLibraryDialog-Create-vbtn">
            Submit
          </v-btn>        
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
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
  
  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newCashFlowRuleLibrary: CashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

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
        owner: this.getIdByUserNameGetter(currentUser),
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
