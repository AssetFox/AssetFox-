<template>
  <v-dialog max-width="450px" persistent v-bind:show="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Investment Library</div>
            <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field id="CreateBudgetLibraryDialog-name-textField"
                        outline v-model="newBudgetLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].nameIsNotUnique(newBudgetLibrary.name, libraryNames)]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          <v-textarea id="CreateBudgetLibraryDialog-description-textArea"
                      no-resize outline rows="3"
                      v-model="newBudgetLibrary.description"
                      class="ghd-text-field-border">
          </v-textarea>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn id="CreateBudgetLibraryDialog-cancel-btn" @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' variant = "outlined">Cancel</v-btn>
          <v-btn id="CreateBudgetLibraryDialog-save-btn" :disabled="canDisableSave()" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
            Save
          </v-btn>          
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts"setup>
import Vue from 'vue';
import {contains} from 'ramda';
import {CreateBudgetLibraryDialogData} from '@/shared/models/modals/create-budget-library-dialog-data';
import {Budget, BudgetAmount, BudgetLibrary, emptyBudgetLibrary} from '@/shared/models/iAM/investment';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { getUserName } from '@/shared/utils/get-user-info';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
let store = useStore();
const props = defineProps<{

  dialogData:CreateBudgetLibraryDialogData,
  libraryNames: string[];
}>()
const emit = defineEmits(['submit'])
let getIdByUserNameGetter: any = store.getters.getIdByUserName
let newBudgetLibrary: BudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
let rules: InputValidationRules = validationRules;

watch(()=>props.dialogData,()=> onDialogDataChanged)
  function onDialogDataChanged() {
    let currentUser: string = getUserName();
    newBudgetLibrary = {
      ...newBudgetLibrary,
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
  }
  function canDisableSave() : boolean {
    let check: boolean = false;
    if (newBudgetLibrary.name === '') return true;
    if (contains(newBudgetLibrary.name, props.libraryNames)) return true;
    return  check;
  }
  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newBudgetLibrary);
    } else {
      emit('submit', null);
    }

    newBudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
  }
</script>
