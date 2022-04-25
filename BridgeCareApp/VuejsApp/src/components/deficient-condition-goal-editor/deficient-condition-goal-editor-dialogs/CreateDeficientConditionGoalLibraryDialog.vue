<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Deficient Condition Goal Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-text-field label="Name" outline v-model="newDeficientConditionGoalLibrary.name"
                      :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
        <v-textarea label="Description" no-resize outline rows="3"
                    v-model="newDeficientConditionGoalLibrary.description">
        </v-textarea>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newDeficientConditionGoalLibrary.name === ''" @click="onSubmit(true)"
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
