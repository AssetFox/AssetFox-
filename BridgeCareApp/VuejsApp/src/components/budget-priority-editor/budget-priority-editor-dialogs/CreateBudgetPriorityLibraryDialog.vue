<template>
  <v-dialog width="50%" persistent v-model ="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="ghd-control-dialog-header"><h5>New Budget Priority Library</h5></div>
          <XButton @click="onSubmit(false)"/>
        </v-row>
      </v-card-title>

      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          
            <v-text-field id="CreateBudgetPriorityLibraryDialog-name-vtextfield"
                          v-model="newBudgetPriorityLibrary.name"
                          :rules="[rules['generalRules'].valueIsNotEmpty]"
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"/>
          
            <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          
            <v-textarea id="CreateBudgetPriorityLibraryDialog-description-vtextfield"
                        no-resize
                        :rows="5"
                        v-model="newBudgetPriorityLibrary.description"
                        class="ghd-control-text ghd-control-border" variant="outlined"  />
            </v-col>
        </v-row>
      </v-card-text>
      
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center"> 
          <CancelButton @cancel="onSubmit(false)"/>
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="newBudgetPriorityLibrary.name === ''"
          />
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
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

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
