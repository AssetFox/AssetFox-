<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">New Cash Flow Rule Library</div>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Rule Name</v-subheader>
          <v-text-field outline v-model="newCashRule.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(false)" flat class='ghd-blue ghd-button-text ghd-button'>
            Cancel
          </v-btn>
          <v-btn :disabled="newCashRule.name === ''" @click="onSubmit(true)"
                 outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Submit
          </v-btn>        
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Prop} from 'vue-property-decorator';
import {
  CashFlowRule,
  emptyCashFlowRule,
} from '@/shared/models/iAM/cash-flow';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class AddCashFlowDialog extends Vue {
    @Prop() showDialog: boolean;

  newCashRule: CashFlowRule = {...emptyCashFlowRule, id: getNewGuid()};
  rules: InputValidationRules = rules;

  disableSubmitButton() {
    return !(this.rules['generalRules'].valueIsNotEmpty(this.newCashRule.name) === true);
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newCashRule);
    } else {
      this.$emit('submit', null);
    }

    this.newCashRule = {...emptyCashFlowRule, id: getNewGuid()};
  }
}
</script>