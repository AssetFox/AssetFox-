<template>
  <v-dialog width="50%"  persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="dialog-header"><h5>Create New Target Condition Goal Library</h5></div>
          <v-btn @click="onSubmit(false)" 
                    id="CreateTargetConditionGoalLibraryDialog-Close-btn"
                    flat>
                    <i class="fas fa-times fa-2x"></i>
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text  class="ghd-dialog-text-field-padding">
        <v-col>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field  
            variant="outlined"
            density="compact" 
            id="CreateTargetConditionGoalLibraryDialog-Name-textField"
            v-model="newTargetConditionGoalLibrary.name"
            class="ghd-control-text ghd-control-border"
            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
        </v-col>
        <v-col>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
          <v-textarea 
            no-resize 
            variant="outlined" 
            id="CreateTargetConditionGoalLibraryDialog-Description-textarea"
            rows="3"
            class="ghd-control-text ghd-control-border" height="100px"
            v-model="newTargetConditionGoalLibrary.description"/>
        </v-col>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newTargetConditionGoalLibrary.name ===''" 
          />
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang='ts'>
import { ref, toRefs, watch } from 'vue';

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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';

  let store = useStore();
  const emit = defineEmits(['submit'])
  const props = defineProps<{dialogData: CreateTargetConditionGoalLibraryDialogData}>()
  const { dialogData } = toRefs(props);
  let getIdByUserNameGetter = store.getters.getIdByUserName;

  const newTargetConditionGoalLibrary = ref<TargetConditionGoalLibrary>({...emptyTargetConditionGoalLibrary, id: getNewGuid()});
  const rules = ref<InputValidationRules>(validationRules);

  watch(dialogData, ()=>  {
    let currentUser: string = getUserName();

     newTargetConditionGoalLibrary.value = {
      ...newTargetConditionGoalLibrary.value,
      targetConditionGoals: hasValue(props.dialogData.targetConditionGoals)
          ? props.dialogData.targetConditionGoals.map((targetConditionGoal: TargetConditionGoal) => ({
            ...targetConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: getIdByUserNameGetter(currentUser),
    };
  })

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newTargetConditionGoalLibrary.value);
    } else {
      emit('submit', null);
    }

    newTargetConditionGoalLibrary.value = {...emptyTargetConditionGoalLibrary, id: getNewGuid()};
  }
</script>
