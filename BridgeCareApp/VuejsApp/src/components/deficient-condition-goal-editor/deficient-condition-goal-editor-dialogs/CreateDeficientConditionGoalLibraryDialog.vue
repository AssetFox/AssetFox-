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
        <v-text-field id="CreateDeficientConditionGoalLibraryDialog-nane-vtextarea" outline v-model="newDeficientConditionGoalLibrary.name"
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      class="ghd-text-field-border ghd-text-field"></v-text-field>
        <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
        <v-textarea id="CreateDeficientConditionGoalLibraryDialog-description-vtextarea" no-resize outline rows="3"
                    v-model="newDeficientConditionGoalLibrary.description"
                    class="ghd-text-field-border">
        </v-textarea>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-space-between row>
          <v-btn id="CreateDeficientConditionGoalLibraryDialog-cancel-vbtn" @click="onSubmit(false)" outline class='ghd-blue ghd-button-text ghd-button'>Cancel</v-btn>
          <v-btn id="CreateDeficientConditionGoalLibraryDialog-save-vbtn" :disabled="newDeficientConditionGoalLibrary.name === ''" @click="onSubmit(true)"
                 outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>         
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { watch } from 'vue';
import {CreateDeficientConditionGoalLibraryDialogData} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
import {
  DeficientConditionGoal,
  DeficientConditionGoalLibrary,
  emptyDeficientConditionGoalLibrary
} from '@/shared/models/iAM/deficient-condition-goal';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import { useStore } from 'vuex';

  let store = useStore();

  const props = defineProps<{
    dialogData: CreateDeficientConditionGoalLibraryDialogData
  }>()
  const emit = defineEmits(['submit'])

  let getIdByUserNameGetter: any = store.getters.getIdByUserName;

  let newDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = {
    ...emptyDeficientConditionGoalLibrary,
    id: getNewGuid()
  };
  let rules: InputValidationRules = validationRules;

  watch(() => props.dialogData, () => onDialogDataChanged)
  function onDialogDataChanged() {
    let currentUser: string = getUserName();

    newDeficientConditionGoalLibrary = {
      ...newDeficientConditionGoalLibrary,
      deficientConditionGoals: hasValue(props.dialogData.deficientConditionGoals)
          ? props.dialogData.deficientConditionGoals.map((deficientConditionGoal: DeficientConditionGoal) => ({
            ...deficientConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: getIdByUserNameGetter(currentUser),
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDeficientConditionGoalLibrary);
    } else {
      emit('submit', null);
    }

    newDeficientConditionGoalLibrary = {...emptyDeficientConditionGoalLibrary, id: getNewGuid()};
  }

</script>
