<template>
  <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center >
          <div class="ghd-control-dialog-header">New Budget Priority Library</div>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
            X
          </v-btn>
        </v-layout>

      </v-card-title>
      <v-card-text class="ghd-dialog-box-padding-center">
        <v-layout column >
          <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
          <v-text-field outline v-model="newBudgetPriorityLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-text-field-border ghd-text-field"/>
          <v-subheader class="ghd-md-gray ghd-control-label">Description</v-subheader>
          <v-textarea no-resize outline :rows="5"
                      v-model="newBudgetPriorityLibrary.description"
                      class="ghd-text-field-border"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="ghd-dialog-box-padding-bottom">
        <v-layout justify-center row >       
          <v-btn @click="onSubmit(false)" outline class='ghd-blue ghd-button-text ghd-button'>Cancel </v-btn>
          <v-btn :disabled="newBudgetPriorityLibrary.name === ''" @click="onSubmit(true)"
                 outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
            Save
          </v-btn>
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
