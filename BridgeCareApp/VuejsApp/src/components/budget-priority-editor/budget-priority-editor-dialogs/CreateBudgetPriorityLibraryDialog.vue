<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Budget Priority Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newBudgetPriorityLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-textarea label="Description" no-resize outline rows="3"
                      v-model="newBudgetPriorityLibrary.description"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newBudgetPriorityLibrary.name === ''" @click="onSubmit(true)"
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
import {Component, Prop, Watch} from 'vue-property-decorator';
import {Getter} from 'vuex-class';
import {CreateBudgetPriorityLibraryDialogData} from '@/shared/models/modals/create-budget-priority-library-dialog-data';
import {
  BudgetPercentagePair,
  BudgetPriority,
  BudgetPriorityLibrary,
  emptyBudgetPriorityLibrary
} from '@/shared/models/iAM/budget-priority';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreatePriorityLibraryDialog extends Vue {
  @Prop() dialogData: CreateBudgetPriorityLibraryDialogData;

  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newBudgetPriorityLibrary: BudgetPriorityLibrary = {...emptyBudgetPriorityLibrary, id: getNewGuid()};
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

    this.newBudgetPriorityLibrary = {
      ...this.newBudgetPriorityLibrary,
      budgetPriorities: this.dialogData.budgetPriorities.map((budgetPriority: BudgetPriority) => ({
        ...budgetPriority, id: getNewGuid(),
        budgetPercentagePairs: budgetPriority.budgetPercentagePairs.map((budgetPercentagePair: BudgetPercentagePair) => ({
          ...budgetPercentagePair, id: getNewGuid()
        }))
      })),
      owner: this.getIdByUserNameGetter(currentUser),
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newBudgetPriorityLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newBudgetPriorityLibrary = {...emptyBudgetPriorityLibrary, id: getNewGuid()};
  }
}
</script>
