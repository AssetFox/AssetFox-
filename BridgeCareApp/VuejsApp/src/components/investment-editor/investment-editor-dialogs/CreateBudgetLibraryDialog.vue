<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">New Investment Library</div>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>           
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column>
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline v-model="newBudgetLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
<!-- , rules['investmentRules'].budgetNameIsUnique(newBudgetLibrary.name,budgetLibraries) -->
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          <v-textarea no-resize outline rows="3"
                      v-model="newBudgetLibrary.description"
                      class="ghd-text-field-border">
          </v-textarea>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row>
          <v-btn @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-button' flat>Cancel</v-btn>
          <v-btn :disabled="newBudgetLibrary.name === ''" @click="onSubmit(true)"
                 class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
            Save
          </v-btn>          
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {CreateBudgetLibraryDialogData} from '@/shared/models/modals/create-budget-library-dialog-data';
import {Budget, BudgetAmount, BudgetLibrary, emptyBudgetLibrary} from '@/shared/models/iAM/investment';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { getUserName } from '@/shared/utils/get-user-info';

@Component
export default class CreateBudgetLibraryDialog extends Vue {
  @Prop() dialogData: CreateBudgetLibraryDialogData;
  @Prop() budgetLibraries: BudgetLibrary[];
  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newBudgetLibrary: BudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
  rules: InputValidationRules = rules;

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();
    console.log("budgetLibraries: " + this.budgetLibraries.length);
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
      owner: this.getIdByUserNameGetter(currentUser)
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newBudgetLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newBudgetLibrary = {...emptyBudgetLibrary, id: getNewGuid()};
  }
}
</script>
