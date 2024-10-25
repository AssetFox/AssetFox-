<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify="space-between" align="center">
            <div class="ghd-control-dialog-header">New Deficient Condition Goal Library</div>
            <XButton @click="onSubmit(false)"/>
          </v-row>
        </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
        <v-text-field id="CreateDeficientConditionGoalLibraryDialog-nane-vtextarea" outline v-model="newDeficientConditionGoalLibrary.name"
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      variant = "outlined"
                      class="ghd-text-field-border ghd-text-field"></v-text-field>
        <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
        <v-textarea id="CreateDeficientConditionGoalLibraryDialog-description-vtextarea" no-resize outline rows="3"
                    v-model="newDeficientConditionGoalLibrary.description"
                    variant="outlined"
                    class="ghd-text-field-border">
        </v-textarea>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newDeficientConditionGoalLibrary.name === ''"
          />        
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, toRefs, watch } from 'vue';
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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

  let store = useStore();

  const props = defineProps<{
    dialogData: CreateDeficientConditionGoalLibraryDialogData
  }>()
  const { dialogData } = toRefs(props);
  const emit = defineEmits(['submit'])

  let getIdByUserNameGetter: any = store.getters.getIdByUserName;

  const newDeficientConditionGoalLibrary = ref<DeficientConditionGoalLibrary>({ ...emptyDeficientConditionGoalLibrary,id: getNewGuid() });
  let rules: InputValidationRules = validationRules;

  watch(dialogData, () => {
    let currentUser: string = getUserName();

    newDeficientConditionGoalLibrary.value = {
      ...newDeficientConditionGoalLibrary.value,
      deficientConditionGoals: hasValue(dialogData.value.deficientConditionGoals)
          ? dialogData.value.deficientConditionGoals.map((deficientConditionGoal: DeficientConditionGoal) => ({
            ...deficientConditionGoal,
            id: getNewGuid()
          }))
          : [],
        owner: getIdByUserNameGetter(currentUser),
    };
  });

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDeficientConditionGoalLibrary.value);
    } else {
      emit('submit', null);
    }

    newDeficientConditionGoalLibrary.value = {...emptyDeficientConditionGoalLibrary, id: getNewGuid()};
  }

</script>
