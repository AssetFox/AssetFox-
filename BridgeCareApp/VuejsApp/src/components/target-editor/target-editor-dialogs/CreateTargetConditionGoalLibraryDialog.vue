<template>
  <v-dialog width="444px" height="437px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-layout justify-start>
          <div class="dialog-header"><h5>Create New Target Condition Goal Library</h5></div>
        </v-layout>
        <v-btn @click="onSubmit(false)" 
                    id="CreateTargetConditionGoalLibraryDialog-Close-btn"
                    icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text  class="ghd-dialog-text-field-padding">
        <v-layout column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field  outline 
                        id="CreateTargetConditionGoalLibraryDialog-Name-textField"
                        v-model="newTargetConditionGoalLibrary.name"
                        class="ghd-control-text ghd-control-border"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
          <v-textarea no-resize outline 
                      id="CreateTargetConditionGoalLibraryDialog-Description-textarea"
                      rows="3"
                      class="ghd-control-text ghd-control-border" height="100px"
                      v-model="newTargetConditionGoalLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-layout justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn @click="onSubmit(false)"
           id="CreateTargetConditionGoalLibraryDialog-Cancel-btn"
           class="ghd-white-bg ghd-blue" outline>Cancel</v-btn>
          <v-btn :disabled="newTargetConditionGoalLibrary.name ===''" @click="onSubmit(true)" outline
                 class="ghd-white-bg ghd-blue"
                 id="CreateTargetConditionGoalLibraryDialog-Save-btn">
            Save
          </v-btn>

        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang='ts'>
import Vue from 'vue';
import {Getter} from 'vuex-class';
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

  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newTargetConditionGoalLibrary: TargetConditionGoalLibrary = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

    this.newTargetConditionGoalLibrary = {
      ...this.newTargetConditionGoalLibrary,
      targetConditionGoals: hasValue(this.dialogData.targetConditionGoals)
          ? this.dialogData.targetConditionGoals.map((targetConditionGoal: TargetConditionGoal) => ({
            ...targetConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: this.getIdByUserNameGetter(currentUser),
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
