<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row>
          <div class="dialog-header">Create New Target Condition Goal Library</div>
          <v-spacer></v-spacer>
          <v-btn id="CreateRemainingLifeLimitDialog-x-btn" @click="onSubmit(false)" icon>
                      <i class="fas fa-times fa-2x"></i>
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row>
          <v-subheader class="ghd-control-label ghd-md-gray">Select an Attribute</v-subheader>
        </v-row>
        <v-row>
          <v-select id="CreateRemainingLifeLimitDialog-selectAnAttribute-select"
                    :items="dialogData.numericAttributeSelectItems"
                    append-icon=ghd-down
                    item-title="text"
                    item-value="value"
                    v-model="newRemainingLifeLimit.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                    variant="outlined"
                    class="ghd-select ghd-control-text ghd-text-field ghd-text-field-border"/>
        </v-row>
        <v-row>
          <v-subheader class="ghd-control-label ghd-md-gray">Limit</v-subheader>
        </v-row>
        <v-row>
          <v-text-field id="CreateRemainingLifeLimitDialog-limit-textField"
                        outline :mask="'##########'"
                        v-model.number="newRemainingLifeLimit.value"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-control-text ghd-control-border"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn id="CreateRemainingLifeLimitDialog-cancel-btn" @click="onSubmit(false)" class="ghd-button" variant = "flat">Cancel</v-btn>
          <v-btn id="CreateRemainingLifeLimitDialog-save-btn" :disabled="disableSubmitAction()" @click="onSubmit(true)" class="ghd-white-bg ghd-blue ghd-button" variant = "outlined">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { computed, ref, toRefs, watch } from 'vue';
import {emptyRemainingLifeLimit, RemainingLifeLimit} from '@/shared/models/iAM/remaining-life-limit';
import {CreateRemainingLifeLimitDialogData} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {hasValue} from '@/shared/utils/has-value-util';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';

  const props = defineProps<{
    dialogData: CreateRemainingLifeLimitDialogData
  }>();
  const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit']);

  let newRemainingLifeLimit = ref<RemainingLifeLimit>({...emptyRemainingLifeLimit, id: getNewGuid()});
  let rules: InputValidationRules = clone(validationRules);
  
  watch(()=> props.dialogData,()=> {
    newRemainingLifeLimit.value.attribute = hasValue(props.dialogData.numericAttributeSelectItems)
        ? props.dialogData.numericAttributeSelectItems[0].value.toString() : '';
  });

  function disableSubmitAction() {
    return rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.value.attribute) !== true ||
        rules['generalRules'].valueIsNotEmpty(newRemainingLifeLimit.value) !== true;
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newRemainingLifeLimit.value);
    } else {
      emit('submit', null);
    }

    newRemainingLifeLimit.value = {...emptyRemainingLifeLimit, id: getNewGuid()};
  }

</script>
