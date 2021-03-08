<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Remaining Limit Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-textarea label="Description" no-resize outline rows="3"
                      v-model="newRemainingLifeLimitLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newRemainingLifeLimitLibrary.name === ''" @click="onSubmit(true)"
                 class="ara-blue-bg white--text">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateRemainingLifeLimitLibraryDialogData} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import {
  emptyRemainingLifeLimitLibrary,
  RemainingLifeLimit,
  RemainingLifeLimitLibrary
} from '@/shared/models/iAM/remaining-life-limit';
import {rules, InputValidationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

@Component
export default class CreateRemainingLifeLimitLibraryDialog extends Vue {
  @Prop() dialogData: CreateRemainingLifeLimitLibraryDialogData;

  newRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  rules: InputValidationRules = {...rules};

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newRemainingLifeLimitLibrary = {
      ...this.newRemainingLifeLimitLibrary,
      remainingLifeLimits: hasValue(this.dialogData.remainingLifeLimits)
          ? this.dialogData.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
            ...remainingLifeLimit,
            id: getNewGuid()
          }))
          : []
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newRemainingLifeLimitLibrary, this.dialogData.scenarioId);
    } else {
      this.$emit('submit', null);
    }

    this.newRemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  }
}
</script>
