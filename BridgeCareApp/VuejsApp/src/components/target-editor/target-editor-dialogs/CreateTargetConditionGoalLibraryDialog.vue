<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Target Condition Goal Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newTargetConditionGoalLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-textarea label="Description" no-resize outline rows="3"
                      v-model="newTargetConditionGoalLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newTargetConditionGoalLibrary.name ===''" @click="onSubmit(true)"
                 class="ara-blue-bg white--text">
            Submit
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang='ts'>
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateTargetConditionGoalLibraryDialogData} from '@/shared/models/modals/create-target-condition-goal-library-dialog-data';
import {
  emptyTargetConditionGoalLibrary,
  TargetConditionGoal,
  TargetConditionGoalLibrary
} from '@/shared/models/iAM/target-condition-goal';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

@Component
export default class CreateTargetConditionGoalLibraryDialog extends Vue {
  @Prop() dialogData: CreateTargetConditionGoalLibraryDialogData;

  newTargetConditionGoalLibrary: TargetConditionGoalLibrary = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newTargetConditionGoalLibrary = {
      ...this.newTargetConditionGoalLibrary,
      targetConditionGoals: hasValue(this.dialogData.targetConditionGoals)
          ? this.dialogData.targetConditionGoals.map((targetConditionGoal: TargetConditionGoal) => ({
            ...targetConditionGoal,
            id: getNewGuid()
          }))
          : [],
      owner: getUserName()
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newTargetConditionGoalLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newTargetConditionGoalLibrary = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  }
}
</script>
