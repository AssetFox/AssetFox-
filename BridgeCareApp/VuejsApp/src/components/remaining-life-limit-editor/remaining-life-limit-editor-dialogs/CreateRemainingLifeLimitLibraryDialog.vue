<template>
  <v-dialog max-width="450px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
       <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify-start>
          <div class="dialog-header"><h5>New Remaining Limit Library</h5></div>
        </v-row>
        <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field outline v-model="newRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-control-text ghd-control-border"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
          <v-textarea no-resize outline rows="3" class="ghd-control-text ghd-control-border"
                      v-model="newRemainingLifeLimitLibrary.description"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn @click="onSubmit(false)" class="ghd-button" variant = "outlined">Cancel</v-btn>
          <v-btn :disabled="newRemainingLifeLimitLibrary.name === ''" @click="onSubmit(true)"
                 class="ghd-white-bg ghd-blue ghd-button" variant = "outlined">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { watch } from 'vue';
import {CreateRemainingLifeLimitLibraryDialogData} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import {
  emptyRemainingLifeLimitLibrary,
  RemainingLifeLimit,
  RemainingLifeLimitLibrary
} from '@/shared/models/iAM/remaining-life-limit';
import {rules as validationRules, InputValidationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

  const props = defineProps<{
    dialogData: CreateRemainingLifeLimitLibraryDialogData
  }>();
  const emit = defineEmits(['submit']);

  let newRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  let rules: InputValidationRules = {...validationRules};

  watch((() => props.dialogData), onDialogDataChanged )
  function onDialogDataChanged() {
    newRemainingLifeLimitLibrary = {
      ...newRemainingLifeLimitLibrary,
      remainingLifeLimits: hasValue(props.dialogData.remainingLifeLimits)
          ? props.dialogData.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
            ...remainingLifeLimit,
            id: getNewGuid()
          }))
          : []
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newRemainingLifeLimitLibrary);
    } else {
      emit('submit', null);
    }

    newRemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  }
</script>
