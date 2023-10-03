<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
       <v-card-title class="ghd-dialog-padding-top-title">
        <v-layout justify-start>
          <div class="dialog-header"><h5>New Remaining Limit Library</h5></div>
        </v-layout>
        <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-layout column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field id="CreateRemainingLifeLimitLibraryDialog-nane-vtextarea"
                        outline v-model="newRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-control-text ghd-control-border"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
          <v-textarea id="CreateRemainingLifeLimitLibraryDialog-description-vtextarea"
                      no-resize outline rows="3" class="ghd-control-text ghd-control-border"
                      v-model="newRemainingLifeLimitLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-layout justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn id="CreateRemainingLifeLimitLibraryDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-button" outline>Cancel</v-btn>
          <v-btn id="CreateRemainingLifeLimitLibraryDialog-save-vbtn" :disabled="newRemainingLifeLimitLibrary.name === ''" @click="onSubmit(true)"
                 class="ghd-white-bg ghd-blue ghd-button" outline>
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
      this.$emit('submit', this.newRemainingLifeLimitLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newRemainingLifeLimitLibrary = {...emptyRemainingLifeLimitLibrary, id: getNewGuid()};
  }
}
</script>
