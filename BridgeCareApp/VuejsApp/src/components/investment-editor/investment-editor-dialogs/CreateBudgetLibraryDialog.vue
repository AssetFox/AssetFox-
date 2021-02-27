<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Investment Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newBudgetLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

          <v-textarea label="Description" no-resize outline rows="3"
                      v-model="newBudgetLibrary.description">
          </v-textarea>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newBudgetLibrary.name === ''" @click="onSubmit(true)"
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
import {CreateBudgetLibraryDialogData} from '@/shared/models/modals/create-budget-library-dialog-data';
import {Budget, BudgetAmount, BudgetLibrary, emptyBudgetLibrary} from '@/shared/models/iAM/investment';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateBudgetLibraryDialog extends Vue {
  @Prop() dialogData: CreateBudgetLibraryDialogData;

  newBudgetLibrary: BudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newBudgetLibrary = {
      ...this.newBudgetLibrary,
      budgets: this.dialogData.budgets.map((budget: Budget) => ({
        ...budget,
        id: getNewGuid(),
        budgetAmounts: budget.budgetAmounts.map((budgetAmount: BudgetAmount) => ({
          ...budgetAmount,
          id: getNewGuid()
        }))
      })),
      owner: getUserName()
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newBudgetLibrary, this.dialogData.scenarioId);
    } else {
      this.$emit('submit', null);
    }

    this.newBudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
  }
}
</script>
