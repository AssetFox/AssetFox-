<template>
  <v-dialog max-width="450px" persistent v-model ="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center >
          <div class="ghd-control-dialog-header">New Budget Priority Library</div>
          <v-spacer></v-spacer>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
            X
          </v-btn>
        </v-row>

      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
        </v-row>
        <v-row>
          <v-text-field id="CreateBudgetPriorityLibraryDialog-name-vtextfield" outline v-model="newBudgetPriorityLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
        </v-row>
        <v-row>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
        </v-row>
        <v-row>
          <v-textarea id="CreateBudgetPriorityLibraryDialog-description-vtextfield" no-resize outline :rows="5"
                      v-model="newBudgetPriorityLibrary.description"
                      class="ghd-text-field-border"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row align-content="center" justify="center" row>       
          <v-btn id="CreateBudgetPriorityLibraryDialog-cancel-vbtn" @click="onSubmit(false)" variant = "outlined" class='ghd-blue ghd-button-text ghd-button'>Cancel </v-btn>
          <v-btn id="CreateBudgetPriorityLibraryDialog-save-vbtn" :disabled="newBudgetPriorityLibrary.name === ''" @click="onSubmit(true)"
          variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue, { ref, toRefs, watch } from 'vue';
import {CreateBudgetPriorityLibraryDialogData} from '@/shared/models/modals/create-budget-priority-library-dialog-data';
import {
  BudgetPercentagePair,
  BudgetPriority,
  BudgetPriorityLibrary,
  emptyBudgetPriorityLibrary
} from '@/shared/models/iAM/budget-priority';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { nextTick } from 'process';
import { useStore } from 'vuex';

  let store = useStore();
  const props = defineProps<{
    dialogData: CreateBudgetPriorityLibraryDialogData
  }>();
const { dialogData } = toRefs(props);

  const emit = defineEmits(['submit'])
  let getIdByUserNameGetter: any = store.getters.getIdByUserName;

  let newBudgetPriorityLibrary = ref<BudgetPriorityLibrary>({...emptyBudgetPriorityLibrary, id: getNewGuid()});
  let rules: InputValidationRules = validationRules;
  
  watch(()=> props.dialogData, ()=> {
    let currentUser: string = getUserName();

    newBudgetPriorityLibrary.value = {
      ...newBudgetPriorityLibrary.value,
      budgetPriorities: props.dialogData.budgetPriorities.map((budgetPriority: BudgetPriority) => ({
        ...budgetPriority, id: getNewGuid(),
        budgetPercentagePairs: budgetPriority.budgetPercentagePairs.map((budgetPercentagePair: BudgetPercentagePair) => ({
          ...budgetPercentagePair, id: getNewGuid()
        }))
      })),
      owner: getIdByUserNameGetter(currentUser),
    };
  });

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newBudgetPriorityLibrary.value);
    } else {
      emit('submit', null);
    }

    newBudgetPriorityLibrary.value = {...emptyBudgetPriorityLibrary, id: getNewGuid()};
  }
</script>
