<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-row>
            <div class="ghd-control-dialog-header">New Investment Library</div>
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
          <v-text-field id="CreateBudgetLibraryDialog-name-textField"
                        outline v-model="newBudgetLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].nameIsNotUnique(newBudgetLibrary.name, libraryNames)]"
                     />
        </v-row>
        <v-row>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
        </v-row>
        <v-row>
          <v-textarea id="CreateBudgetLibraryDialog-description-textArea"
                      no-resize outline rows="3"
                      v-model="newBudgetLibrary.description"
                      >
          </v-textarea>
        </v-row>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-spacer></v-spacer>   
          <v-btn id="CreateBudgetLibraryDialog-cancel-btn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' variant = "outlined">Cancel</v-btn>
          <v-btn id="CreateBudgetLibraryDialog-save-btn" :disabled="canDisableSave()" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
            Save
          </v-btn>
        <v-spacer></v-spacer>            
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import Vue from 'vue';
import {contains} from 'ramda';
import {CreateBudgetLibraryDialogData} from '@/shared/models/modals/create-budget-library-dialog-data';
import {Budget, BudgetAmount, BudgetLibrary, emptyBudgetLibrary} from '@/shared/models/iAM/investment';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { getUserName } from '@/shared/utils/get-user-info';
import { ref, computed, toRefs, watch } from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

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
