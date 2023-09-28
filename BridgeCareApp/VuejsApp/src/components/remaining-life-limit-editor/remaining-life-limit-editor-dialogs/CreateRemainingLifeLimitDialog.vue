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
          <v-btn id="CreateRemainingLifeLimitDialog-cancel-btn" @click="onSubmit(false)" class="ghd-button" variant = "flat">Cancel</v-btn>
          <v-btn id="CreateRemainingLifeLimitDialog-save-btn" :disabled="disableSubmitAction()" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button" variant = "outline">
            Save
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { computed, toRef, watch } from 'vue';
import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {hasValue} from '@/shared/utils/has-value-util';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';

  const props = defineProps<{
    dialogData: CreateRemainingLifeLimitDialogData
  }>();
  const emit = defineEmits(['submit']);

  let newRemainingLifeLimit: RemainingLifeLimit = {...emptyRemainingLifeLimit, id: getNewGuid()};
  let rules: InputValidationRules = clone(validationRules);
  
  watch((() => props.dialogData), onDialogDataChanged )
  function onDialogDataChanged() {
    newRemainingLifeLimit.attribute = hasValue(props.dialogData.numericAttributeSelectItems)
        ? props.dialogData.numericAttributeSelectItems[0].value.toString() : '';
  }

  function disableSubmitAction() {
    return rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.attribute) !== true ||
        rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.value) !== true;
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newRemainingLifeLimit);
    } else {
      emit('submit', null);
    }

    newRemainingLifeLimit = {...emptyRemainingLifeLimit, id: getNewGuid()};
  }

</script>
