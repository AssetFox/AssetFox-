<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Deficient Condition Goal Library</div>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
        <v-text-field outline v-model="newDeficientConditionGoalLibrary.name"
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      class="ghd-text-field-border ghd-text-field"></v-text-field>
        <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
        <v-textarea no-resize outline rows="3"
                    v-model="newDeficientConditionGoalLibrary.description"
                    class="ghd-text-field-border">
        </v-textarea>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(false)" outline class='ghd-blue ghd-button-text ghd-button'>Cancel</v-btn>
          <v-btn :disabled="newDeficientConditionGoalLibrary.name === ''" @click="onSubmit(true)"
                 outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>         
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateDeficientConditionGoalLibraryDialogData} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
import {
  DeficientConditionGoal,
  DeficientConditionGoalLibrary,
  emptyDeficientConditionGoalLibrary
} from '@/shared/models/iAM/deficient-condition-goal';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

@Component
export default class CreateDeficientConditionGoalLibraryDialog extends Vue {
  @Prop() dialogData: CreateDeficientConditionGoalLibraryDialogData;

  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = {
    ...emptyDeficientConditionGoalLibrary,
    id: getNewGuid()
  };
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

    this.newDeficientConditionGoalLibrary = {
      ...this.newDeficientConditionGoalLibrary,
      deficientConditionGoals: hasValue(this.dialogData.deficientConditionGoals)
          ? this.dialogData.deficientConditionGoals.map((deficientConditionGoal: DeficientConditionGoal) => ({
            ...deficientConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: this.getIdByUserNameGetter(currentUser),
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newDeficientConditionGoalLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newDeficientConditionGoalLibrary = {...emptyDeficientConditionGoalLibrary, id: getNewGuid()};
  }
}
</script>
