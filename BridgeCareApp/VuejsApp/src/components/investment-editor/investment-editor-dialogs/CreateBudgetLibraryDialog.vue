<template>
  <v-dialog width="50%" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
         <v-row justify="space-between">
            <div class="ghd-control-dialog-header"><h5>New Investment Library</h5></div>
            <XButton @click="onSubmit(false)"/>
          </v-row>
        </v-card-title>     

      <v-card-text class="ghd-dialog-box-padding-center">
        <v-row>
          <v-col>
            <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
            <v-text-field id="CreateBudgetLibraryDialog-name-textField"
                          v-model="newBudgetLibrary.name"
                          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].nameIsNotUnique(newBudgetLibrary.name, libraryNames)]"
                          class="ghd-text-field-border ghd-text-field" variant="outlined" density="compact"
                      />
          
            <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>        
            <v-textarea id="CreateBudgetLibraryDialog-description-textArea"
                        no-resize
                        rows="5"
                        v-model="newBudgetLibrary.description"
                        class="ghd-control-text ghd-control-border" variant="outlined" 
                        >
            </v-textarea>
        </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-row justify="center">  
          <CancelButton 
            @cancel="onSubmit(false)"
          />
          <SaveButton 
            @save="onSubmit(true)"
            :disabled="canDisableSave()"
          />           
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {contains} from 'ramda';
import {CreateBudgetLibraryDialogData} from '@/shared/models/modals/create-budget-library-dialog-data';
import {Budget, BudgetAmount, BudgetLibrary, emptyBudgetLibrary} from '@/shared/models/iAM/investment';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { getUserName } from '@/shared/utils/get-user-info';
import { ref, computed, toRefs, watch } from 'vue';
import { useStore } from 'vuex';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

let store = useStore();

const props = defineProps<{
          dialogData:CreateBudgetLibraryDialogData,
          libraryNames: string[];
        }>();
const { dialogData } = toRefs(props);
// let showDialogComputed = computed(() => props.dialogData.showDialog);;

const emit = defineEmits(['submit'])
let getIdByUserNameGetter: any = store.getters.getIdByUserName
let newBudgetLibrary = ref<BudgetLibrary>({...emptyBudgetLibrary, id: getNewGuid()});
let rules: InputValidationRules = validationRules;

watch(()=> props.dialogData,()=> {
  
    let currentUser: string = getUserName();
    newBudgetLibrary.value = {
      ...newBudgetLibrary.value,
      budgets: props.dialogData.budgets.map((budget: Budget) => ({
        ...budget,
        id: getNewGuid(),
        budgetAmounts: budget.budgetAmounts.map((budgetAmount: BudgetAmount) => ({
          ...budgetAmount,
          id: getNewGuid()
        }))
      })),
      owner: getIdByUserNameGetter(currentUser)
    };
  });

  function canDisableSave() : boolean {
    let check: boolean = false;
    if (newBudgetLibrary.value.name === '') return true;
    if (contains(newBudgetLibrary.value.name, props.libraryNames)) return true;
    return  check;
  }
  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newBudgetLibrary.value);
    } else {
      emit('submit', null);
    }
    newBudgetLibrary.value = {...emptyBudgetLibrary, id: getNewGuid()};
  }
</script>
