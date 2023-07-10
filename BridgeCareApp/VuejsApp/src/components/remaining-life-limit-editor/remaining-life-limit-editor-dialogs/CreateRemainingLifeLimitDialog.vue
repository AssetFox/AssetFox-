<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
            <v-card-title class="ghd-dialog-padding-top-title">
        <v-layout justify-start>
          <div class="dialog-header"><h5>Create New Target Condition Goal Library</h5></div>
        </v-layout>
        <v-btn id="CreateRemainingLifeLimitDialog-x-btn" @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-layout column>
          <v-subheader class="ghd-control-label ghd-md-gray">Select an Attribute</v-subheader>
          <v-select id="CreateRemainingLifeLimitDialog-selectAnAttribute-select"
                    :items="dialogData.numericAttributeSelectItems"
                    append-icon=$vuetify.icons.ghd-down
                    outline v-model="newRemainingLifeLimit.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                    class="ghd-select ghd-control-text ghd-text-field ghd-text-field-border"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Limit</v-subheader>
          <v-text-field id="CreateRemainingLifeLimitDialog-limit-textField"
                        outline :mask="'##########'"
                        v-model.number="newRemainingLifeLimit.value"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-control-text ghd-control-border"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-layout justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn @click="onSubmit(false)" class="ghd-button" flat>Cancel</v-btn>
          <v-btn :disabled="disableSubmitAction()" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button" outline>
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {hasValue} from '@/shared/utils/has-value-util';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';


@Component
export default class CreateRemainingLifeLimitDialog extends Vue {
  @Prop() dialogData: CreateRemainingLifeLimitDialogData;

  newRemainingLifeLimit: RemainingLifeLimit = {...emptyRemainingLifeLimit, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newRemainingLifeLimit.attribute = hasValue(this.dialogData.numericAttributeSelectItems)
        ? this.dialogData.numericAttributeSelectItems[0].value.toString() : '';
  }

  disableSubmitAction() {
    return this.rules['generalRules'].valueIsNotEmpty(this.newRemainingLifeLimit.attribute) !== true ||
        this.rules['generalRules'].valueIsNotEmpty(this.newRemainingLifeLimit.value) !== true;
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newRemainingLifeLimit);
    } else {
      this.$emit('submit', null);
    }

    this.newRemainingLifeLimit = {...emptyRemainingLifeLimit, id: getNewGuid()};
  }
}
</script>
