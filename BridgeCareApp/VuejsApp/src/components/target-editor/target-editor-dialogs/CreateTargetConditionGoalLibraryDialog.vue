<template>
  <Dialog width="444px" height="437px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify-start>
          <div class="dialog-header"><h5>Create New Target Condition Goal Library</h5></div>
        </v-row>
        <v-btn @click="onSubmit(false)" 
                    id="CreateTargetConditionGoalLibraryDialog-Close-btn"
                    icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text  class="ghd-dialog-text-field-padding">
        <v-row column>
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
        </v-row>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn @click="onSubmit(false)"
           id="CreateTargetConditionGoalLibraryDialog-Cancel-btn"
           class="ghd-white-bg ghd-blue" variant = "outlined">Cancel</v-btn>
          <v-btn :disabled="newTargetConditionGoalLibrary.name ===''" @click="onSubmit(true)" variant = "outlined"
                 class="ghd-white-bg ghd-blue"
                 id="CreateTargetConditionGoalLibraryDialog-Save-btn">
            Save
          </v-btn>

        </v-row>
      </v-card-actions>
    </v-card>
  </Dialog>
</template>

<script lang='ts' setup>
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';

import {CreateTargetConditionGoalLibraryDialogData} from '@/shared/models/modals/create-target-condition-goal-library-dialog-data';
import {
  emptyTargetConditionGoalLibrary,
  TargetConditionGoal,
  TargetConditionGoalLibrary
} from '@/shared/models/iAM/target-condition-goal';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules as validationRules,} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Dialog from 'primevue/dialog';

  let store = useStore();
  const emit = defineEmits(['submit'])
  const props = defineProps<{dialogData: CreateTargetConditionGoalLibraryDialogData}>()

  let getIdByUserNameGetter = store.getters.getIdByUserName;

  let newTargetConditionGoalLibrary: TargetConditionGoalLibrary = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  let rules: InputValidationRules = validationRules;

  watch(()=> props.dialogData, ()=> onDialogDataChanged)
  function onDialogDataChanged() {
    let currentUser: string = getUserName();

     newTargetConditionGoalLibrary = {
      ...newTargetConditionGoalLibrary,
      targetConditionGoals: hasValue(props.dialogData.targetConditionGoals)
          ? props.dialogData.targetConditionGoals.map((targetConditionGoal: TargetConditionGoal) => ({
            ...targetConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: getIdByUserNameGetter(currentUser),
    };
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newTargetConditionGoalLibrary);
    } else {
      emit('submit', null);
    }

    newTargetConditionGoalLibrary = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  }
</script>
