<template>
  <v-layout column>
    <v-layout justify-center v-show="selectedScenarioId !== uuidNIL">
      <v-flex xs12>
        <v-layout justify-space-between row>
          <v-spacer></v-spacer>
          <v-flex xs2>
            <v-text-field label="First Year of Analysis Period" readonly outline
                          v-model="investmentPlan.firstYearOfAnalysisPeriod"/>
          </v-flex>
          <v-flex xs2>
            <v-text-field label="Number of Years in Analysis Period" readonly outline
                          v-model="investmentPlan.numberOfYearsInAnalysisPeriod"/>
          </v-flex>
          <v-spacer></v-spacer>
        </v-layout>

        <v-layout justify-space-between row>
          <v-spacer></v-spacer>
          <v-flex xs2>
            <v-text-field label="Minimum Project Cost Limit" outline id="min-proj-cost-limit-txt"
                          v-model="investmentPlan.minimumProjectCostLimit"
                          @input="syncAllBudgetAmountsWithMinimumProjectCostLimit"
                          v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                          :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].minCostLimitGreaterThanZero(investmentPlan.minimumProjectCostLimit)]"/>
          </v-flex>
          <v-flex xs2>
            <v-text-field label="Inflation Rate Percentage" outline v-model="investmentPlan.inflationRatePercentage"
                          :mask="'###'"
                          :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(investmentPlan.inflationRatePercentage, [0,100])]"/>
          </v-flex>
          <v-spacer></v-spacer>
        </v-layout>
      </v-flex>
    </v-layout>
    <v-divider v-if="selectedScenarioId !== uuidNIL"/>
    <v-flex xs12>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onShowCreateBudgetLibraryDialog(false)" class="ara-blue-bg white--text"
                 v-show="selectedScenarioId === uuidNIL">
            New Library
          </v-btn>
          <v-select :items="librarySelectItems"
                    label="Select an Investment library"
                    outline v-if="!hasSelectedLibrary || selectedScenarioId !== uuidNIL"
                    v-model="librarySelectItemValue">
          </v-select>
          <v-text-field label="Library Name" v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                        v-model="selectedBudgetLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]">
            <template slot="append">
              <v-btn @click="librarySelectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL">
            Owner: {{ selectedBudgetLibrary.owner ? selectedBudgetLibrary.owner : "[ No Owner ]" }}
          </div>
          <v-checkbox class="sharing" label="Shared"
                      v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                      v-model="selectedBudgetLibrary.shared"/>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedLibrary"></v-divider>
    <v-flex v-show="hasSelectedLibrary" xs12>
      <v-layout justify-center>
        <v-flex xs6>
          <v-layout justify-space-between>
            <v-btn @click="onShowEditBudgetsDialog" class="ara-blue-bg white--text">
              Edit Budgets
            </v-btn>
            <v-btn :disabled="selectedBudgetLibrary.budgets.length === 0" @click="onAddBudgetYear"
                   class="ara-blue-bg white--text">
              Add Year
            </v-btn>
            <v-btn :disabled="selectedBudgetLibrary.budgets.length === 0"
                   @click="showSetRangeForAddingBudgetYearsDialog = true"
                   class="ara-blue-bg white--text">
              Add Years by Range
            </v-btn>
            <v-btn :disabled="selectedGridRows.length === 0" @click="onRemoveBudgetYears"
                   class="ara-orange-bg white--text">
              Delete Budget Year(s)
            </v-btn>
          </v-layout>
        </v-flex>
      </v-layout>
      <v-layout justify-center>
        <v-flex xs8>
          <v-card>
            <v-data-table :headers="budgetYearsGridHeaders" :items="budgetYearsGridData"
                          class="elevation-1 v-table__overflow" item-key="year" select-all
                          v-model="selectedGridRows">
              <template slot="items" slot-scope="props">
                <td>
                  <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
                </td>
                <td v-for="header in budgetYearsGridHeaders">
                  <div v-if="header.value !== 'year'">
                    <v-edit-dialog :return-value.sync="props.item[header.value]"
                                   @save="onEditBudgetYearValue(props.item.year, header.value, props.item[header.value])"
                                   large lazy persistent>
                      <v-text-field readonly single-line class="sm-txt"
                                    :value="formatAsCurrency(props.item[header.value])"
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                      <template slot="input">
                        <v-text-field label="Edit" single-line
                                      v-model.number="props.item[header.value]"
                                      v-currency="{currency: {prefix: '$', suffix: ''}, locale: 'en-US', distractionFree: false}"
                                      :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                      </template>
                    </v-edit-dialog>
                  </div>
                  <div v-else>
                    <span class="sm-txt">{{ props.item.year }}</span>
                  </div>
                </td>
              </template>
            </v-data-table>
          </v-card>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedLibrary"></v-divider>
    <v-flex
        v-show="hasSelectedLibrary && selectedScenarioId === uuidNIL"
        xs12>
      <v-layout justify-center>
        <v-flex xs6>
          <v-textarea label="Description" no-resize outline rows="4"
                      v-model="selectedBudgetLibrary.description">
          </v-textarea>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-flex xs12>
      <v-layout justify-end row v-show="hasSelectedLibrary">
        <v-btn :disabled="disableCrudButton()"
               @click="onAddOrUpdateInvestment(selectedBudgetLibrary, selectedScenarioId)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId !== uuidNIL">
          Save
        </v-btn>
        <v-btn :disabled="disableCrudButton()"
               @click="onAddOrUpdateInvestment(selectedBudgetLibrary, uuidNIL)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId === uuidNIL">
          Update Library
        </v-btn>
        <v-btn :disabled="disableCrudButton()" @click="onShowCreateBudgetLibraryDialog(true)"
               class="ara-blue-bg white--text">
          Create as New Library
        </v-btn>
        <v-btn @click="onShowConfirmDeleteAlert" class="ara-orange-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="!hasSelectedLibrary">
          Delete Library
        </v-btn>
        <v-btn :disabled="!hasSelectedLibrary" @click="onDiscardChanges"
               class="ara-orange-bg white--text" v-show="selectedScenarioId !== uuidNIL">
          Discard Changes
        </v-btn>
      </v-layout>
    </v-flex>

    <ConfirmDeleteAlert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>

    <CreateBudgetLibraryDialog :dialogData="createBudgetLibraryDialogData"
                               @submit="onAddOrUpdateInvestment"/>

    <SetRangeForAddingBudgetYearsDialog :showDialog="showSetRangeForAddingBudgetYearsDialog"
                                        @submit="onSubmitBudgetYearRange"/>

    <EditBudgetsDialog :dialogData="editBudgetsDialogData" @submit="onSubmitEditBudgetsDialogResult"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Watch} from 'vue-property-decorator';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import SetRangeForAddingBudgetYearsDialog from './investment-editor-dialogs/SetRangeForAddingBudgetYearsDialog.vue';
import EditBudgetsDialog from './investment-editor-dialogs/EditBudgetsDialog.vue';
import {
  Budget,
  BudgetAmount,
  BudgetLibrary,
  BudgetYearsGridData,
  emptyBudgetLibrary,
  emptyInvestmentPlan,
  InvestmentPlan
} from '@/shared/models/iAM/investment';
import {any, append, clone, contains, find, findIndex, groupBy, isNil, keys, propEq, update} from 'ramda';
import {SelectItem} from '@/shared/models/vue/select-item';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {hasValue} from '@/shared/utils/has-value-util';
import moment from 'moment';
import {
  CreateBudgetLibraryDialogData,
  emptyCreateBudgetLibraryDialogData
} from '@/shared/models/modals/create-budget-library-dialog-data';
import {EditBudgetsDialogData, emptyEditBudgetsDialogData} from '@/shared/models/modals/edit-budgets-dialog';
import {getLastPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
import {formatAsCurrency} from '@/shared/utils/currency-formatter';
import Alert from '@/shared/modals/Alert.vue';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibraryId, hasAppliedLibrary} from '@/shared/utils/library-utils';
import {sorter} from '@/shared/utils/sorter-utils';
import CreateBudgetLibraryDialog
  from '@/components/investment-editor/investment-editor-dialogs/CreateBudgetLibraryDialog.vue';

@Component({
  components: {
    CreateBudgetLibraryDialog,
    SetRangeForAddingBudgetYearsDialog,
    EditBudgetsDialog,
    ConfirmDeleteAlert: Alert
  }
})
export default class InvestmentEditor extends Vue {
  @State(state => state.investment.budgetLibraries) stateBudgetLibraries: BudgetLibrary[];
  @State(state => state.investment.selectedBudgetLibrary) stateSelectedBudgetLibrary: BudgetLibrary;
  @State(state => state.investment.investmentPlan) stateInvestmentPlan: InvestmentPlan;

  @Action('getInvestment') getInvestmentAction: any;
  @Action('selectBudgetLibrary') selectBudgetLibraryAction: any;
  @Action('addOrUpdateInvestment') addOrUpdateInvestmentAction: any;
  @Action('deleteBudgetLibrary') deleteBudgetLibraryAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  selectedBudgetLibrary: BudgetLibrary = clone(emptyBudgetLibrary);
  investmentPlan: InvestmentPlan = clone(emptyInvestmentPlan);
  selectedScenarioId: string = getBlankGuid();
  hasSelectedLibrary: boolean = false;
  librarySelectItems: SelectItem[] = [];
  librarySelectItemValue: string | null = '';
  budgetYearsGridHeaders: DataTableHeader[] = [
    {text: 'Year', value: 'year', sortable: true, align: 'left', class: '', width: ''}
  ];
  budgetYearsGridData: BudgetYearsGridData[] = [];
  selectedGridRows: BudgetYearsGridData[] = [];
  selectedBudgetYears: number[] = [];
  createBudgetLibraryDialogData: CreateBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);
  editBudgetsDialogData: EditBudgetsDialogData = clone(emptyEditBudgetsDialogData);
  showSetRangeForAddingBudgetYearsDialog: boolean = false;
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  uuidNIL: string = getBlankGuid();
  rules: InputValidationRules = clone(rules);

  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('InvestmentEditor/Scenario') !== -1) {
        vm.selectedScenarioId = to.query.scenarioId;
        if (vm.selectedScenarioId === vm.uuidNIL) {
          vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
          vm.$router.push('/Scenarios/');
        }
      }

      vm.selectItemValue = null;
      vm.getInvestmentAction({scenarioId: vm.selectedScenarioId});
    });
  }

  mounted() {
    this.$nextTick(() => {
      const minimumProjectCostLimitTextField: HTMLElement = document
          .getElementById('min-proj-cost-limit-txt') as HTMLElement;
      if (hasValue(minimumProjectCostLimitTextField)) {
        setTimeout(() => {
          minimumProjectCostLimitTextField.click();
          setTimeout(() => minimumProjectCostLimitTextField.blur());
        });
      }
    });
  }

  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  @Watch('stateBudgetLibraries')
  onStateBudgetLibrariesChanged() {
    this.librarySelectItems = this.stateBudgetLibraries
        .map((library: BudgetLibrary) => ({
          text: library.name,
          value: library.id
        }));

    if (this.selectedScenarioId !== this.uuidNIL && this.selectedBudgetLibrary.id === this.uuidNIL &&
        hasAppliedLibrary(this.stateBudgetLibraries, this.selectedScenarioId)) {
      this.librarySelectItemValue = getAppliedLibraryId(this.stateBudgetLibraries, this.selectedScenarioId);
    }
  }

  @Watch('librarySelectItemValue')
  onLibrarySelectItemValueChanged() {
    this.selectBudgetLibraryAction({libraryId: this.librarySelectItemValue});
  }

  @Watch('stateSelectedBudgetLibrary')
  onStateSelectedBudgetLibraryChanged() {
    this.selectedBudgetLibrary = clone(this.stateSelectedBudgetLibrary);
  }

  @Watch('selectedBudgetLibrary')
  onSelectedBudgetLibraryChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChangesCore(
          'budget-library', this.selectedBudgetLibrary, this.stateSelectedBudgetLibrary
      )
    });

    this.selectedGridRows = [];
    this.hasSelectedLibrary = this.selectedBudgetLibrary.id !== this.uuidNIL;

    this.setGridHeaders();
    this.setGridData();

    this.syncInvestmentPlanWithSelectedBudgetLibrary();

    if (!this.hasBudgetsThatMeetMinimumProjectCostLimit()) {
      this.syncAllBudgetAmountsWithMinimumProjectCostLimit();
    }
  }

  @Watch('stateInvestmentPlan')
  onStateInvestmentPlanChanged() {
    this.investmentPlan = clone(this.stateInvestmentPlan);
  }

  @Watch('investmentPlan')
  onInvestmentPlanChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChangesCore('investment-plan',
          {
            ...this.investmentPlan, minimumProjectCostLimit: hasValue(this.investmentPlan.minimumProjectCostLimit)
                ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                : 0
          },
          {
            ...this.stateInvestmentPlan,
            minimumProjectCostLimit: hasValue(this.stateInvestmentPlan.minimumProjectCostLimit)
                ? parseFloat(this.stateInvestmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
                : 0
          })
    });

    this.syncInvestmentPlanWithSelectedBudgetLibrary();

    if (!this.hasBudgetsThatMeetMinimumProjectCostLimit()) {
      this.syncAllBudgetAmountsWithMinimumProjectCostLimit();
    }
  }

  @Watch('selectedGridRows')
  onSelectedGridRowsChanged() {
    this.selectedBudgetYears = getPropertyValues('year', this.selectedGridRows) as number[];
  }

  setGridHeaders() {
    const budgetNames: string[] = sorter(getPropertyValues('name', this.selectedBudgetLibrary.budgets)) as string[];
    const budgetHeaders: DataTableHeader[] = budgetNames
        .map((name: string) => ({
          text: name,
          value: name,
          sortable: true,
          align: 'left',
          class: '',
          width: ''
        }) as DataTableHeader);
    this.budgetYearsGridHeaders = [this.budgetYearsGridHeaders[0], ...budgetHeaders];
  }

  setGridData() {
    this.budgetYearsGridData = [];

    const budgetAmounts: BudgetAmount[] = this.selectedBudgetLibrary.budgets
        .flatMap((budget: Budget) => budget.budgetAmounts);
    const groupBudgetAmountsByYear = groupBy((budgetAmount: BudgetAmount) => budgetAmount.year.toString());
    const groupedBudgetAmounts = groupBudgetAmountsByYear(budgetAmounts);

    keys(groupedBudgetAmounts).forEach((year: any) => {
      const gridDataRow: BudgetYearsGridData = {year: parseInt(year)};

      const budgetAmounts: BudgetAmount[] = groupedBudgetAmounts[year];

      const budgetNames: string[] = sorter(getPropertyValues('name', this.selectedBudgetLibrary.budgets)) as string[];
      for (let i = 0; i < budgetNames.length; i++) {
        const budgetAmount: BudgetAmount = budgetAmounts
            .find((ba: BudgetAmount) => ba.budgetName === budgetNames[i]) as BudgetAmount;

        gridDataRow[budgetNames[i]] = hasValue(budgetAmount) ? budgetAmount.value : null;
      }

      this.budgetYearsGridData.push(gridDataRow);
    });
  }

  syncInvestmentPlanWithSelectedBudgetLibrary() {
    if (this.selectedScenarioId !== this.uuidNIL) {
      const allBudgetAmounts: BudgetAmount[] = this.selectedBudgetLibrary.budgets
          .flatMap((budget: Budget) => budget.budgetAmounts);
      const allBudgetYears: number[] = sorter(getPropertyValues('year', allBudgetAmounts)) as number[];

      this.investmentPlan.firstYearOfAnalysisPeriod = hasValue(allBudgetYears)
          ? allBudgetYears[0] : this.investmentPlan.firstYearOfAnalysisPeriod;
      this.investmentPlan.numberOfYearsInAnalysisPeriod = hasValue(allBudgetYears)
          ? allBudgetYears.length : 1;
    }
  }

  hasBudgetsThatMeetMinimumProjectCostLimit() {
    if (this.selectedScenarioId !== this.uuidNIL) {
      const minProjectCostLimit: number = hasValue(this.investmentPlan.minimumProjectCostLimit)
          ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
          : 0;

      return this.selectedBudgetLibrary.budgets
          .flatMap((budget: Budget) => budget.budgetAmounts)
          .every((budgetAmount: BudgetAmount) => {
            const budgetAmountValue = hasValue(budgetAmount.value)
                ? parseFloat(budgetAmount.value.toString().replace(/(\$*)(\,*)/g, ''))
                : 0;

            return budgetAmountValue >= minProjectCostLimit;
          });
    }

    return true;
  }

  syncAllBudgetAmountsWithMinimumProjectCostLimit() {
    const minProjectCostLimit: number = hasValue(this.investmentPlan.minimumProjectCostLimit)
        ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
        : 0;

    if (minProjectCostLimit > 0 && this.selectedScenarioId !== this.uuidNIL) {
      this.selectedBudgetLibrary = {
        ...this.selectedBudgetLibrary,
        budgets: this.selectedBudgetLibrary.budgets.map((budget: Budget) => ({
          ...budget,
          budgetAmounts: budget.budgetAmounts.map((budgetAmount: BudgetAmount) => {
            const budgetAmountValue = parseFloat(budgetAmount.value.toString().replace(/(\$*)(\,*)/g, ''));
            if (budgetAmountValue < minProjectCostLimit) {
              budgetAmount.value = minProjectCostLimit;
            }
            return budgetAmount;
          })
        }))
      };
    }
  }

  onShowCreateBudgetLibraryDialog(createAsNewLibrary: boolean) {
    this.createBudgetLibraryDialogData = {
      showDialog: true,
      budgets: createAsNewLibrary ? this.selectedBudgetLibrary.budgets : [],
      scenarioId: createAsNewLibrary ? this.selectedScenarioId : this.uuidNIL
    };
  }

  onAddBudgetYear() {
    const latestYear: number = getLastPropertyValue('year', this.budgetYearsGridData);
    const nextYear = hasValue(latestYear) ? latestYear + 1 : moment().year();

    const budgetLibrary: BudgetLibrary = clone(this.selectedBudgetLibrary);

    budgetLibrary.budgets.forEach((budget: Budget) => {
      const newBudgetAmount: BudgetAmount = {
        id: getNewGuid(),
        budgetName: budget.name,
        year: nextYear,
        value: this.selectedScenarioId !== this.uuidNIL ? this.investmentPlan.minimumProjectCostLimit : 0
      };
      budget.budgetAmounts = append(newBudgetAmount, budget.budgetAmounts);
    });

    this.selectedBudgetLibrary = clone(budgetLibrary);
  }

  onSubmitBudgetYearRange(range: number) {
    this.showSetRangeForAddingBudgetYearsDialog = false;

    if (range > 0) {
      const latestYear: number = getLastPropertyValue('year', this.budgetYearsGridData);
      const startYear: number = hasValue(latestYear) ? latestYear + 1 : moment().year();
      const endYear = moment().year(startYear).add(range, 'years').year();

      const budgetLibrary: BudgetLibrary = clone(this.selectedBudgetLibrary);

      for (let currentYear = startYear; currentYear < endYear; currentYear++) {
        budgetLibrary.budgets.forEach((budget: Budget) => {
          budget.budgetAmounts.push({
            id: getNewGuid(),
            budgetName: budget.name,
            year: currentYear,
            value: this.selectedScenarioId !== this.uuidNIL ? this.investmentPlan.minimumProjectCostLimit : 0
          });
        });
      }

      this.selectedBudgetLibrary = clone(budgetLibrary);
    }
  }

  onRemoveBudgetYears() {
    const budgetLibrary: BudgetLibrary = clone(this.selectedBudgetLibrary);

    budgetLibrary.budgets.forEach((budget: Budget) => {
      budget.budgetAmounts = budget.budgetAmounts.filter((budgetAmount: BudgetAmount) =>
          !contains(budgetAmount.year, this.selectedBudgetYears));
    });

    this.selectedBudgetLibrary = clone(budgetLibrary);
  }

  onShowEditBudgetsDialog() {
    this.editBudgetsDialogData = {
      showDialog: true,
      budgets: this.selectedBudgetLibrary.budgets
    };
  }

  onSubmitEditBudgetsDialogResult(budgets: Budget[]) {
    this.editBudgetsDialogData = clone(emptyEditBudgetsDialogData);

    if (!isNil(budgets)) {
      this.selectedBudgetLibrary = {
        ...this.selectedBudgetLibrary,
        budgets: budgets
      };
    }
  }

  onEditBudgetYearValue(year: number, budgetName: string, value: number) {
    if (any(propEq('name', budgetName), this.selectedBudgetLibrary.budgets)) {
      const budget: Budget = find(propEq('name', budgetName), this.selectedBudgetLibrary.budgets) as Budget;

      if (any(propEq('year', year), budget.budgetAmounts)) {
        const budgetAmount: BudgetAmount = find(propEq('year', year), budget.budgetAmounts) as BudgetAmount;
        budget.budgetAmounts = update(
            findIndex(propEq('id', budgetAmount.id), budget.budgetAmounts),
            {
              ...budgetAmount,
              value: hasValue(value)
                  ? parseFloat(value.toString().replace(/(\$*)(\,*)/g, ''))
                  : 0
            }, budget.budgetAmounts);

        this.selectedBudgetLibrary = {
          ...this.selectedBudgetLibrary,
          budgets: update(findIndex(propEq('id', budget.id), this.selectedBudgetLibrary.budgets),
              {...budget}, this.selectedBudgetLibrary.budgets)
        };
      }
    }
  }

  onAddOrUpdateInvestment(library: BudgetLibrary, scenarioId: string) {
    this.createBudgetLibraryDialogData = clone(emptyCreateBudgetLibraryDialogData);

    if (!isNil(library)) {
      this.addOrUpdateInvestmentAction({
        library: {
          ...library,
          budgets: library.budgets.map((budget: Budget) => ({
            ...budget,
            budgetAmounts: budget.budgetAmounts.map((budgetAmount: BudgetAmount) => ({
              ...budgetAmount,
              value: parseFloat(budgetAmount.value.toString().replace(/(\$*)(\,*)/g, ''))
            }))
          }))
        },
        investmentPlan: {
          ...this.investmentPlan,
          minimumProjectCostLimit: hasValue(this.investmentPlan.minimumProjectCostLimit)
              ? parseFloat(this.investmentPlan.minimumProjectCostLimit.toString().replace(/(\$*)(\,*)/g, ''))
              : 500000
        },
        scenarioId: scenarioId
      });
    }
  }

  onDiscardChanges() {
    this.librarySelectItemValue = null;
    setTimeout(() => {
      this.investmentPlan = clone(this.stateInvestmentPlan);
      setTimeout(() => {
        if (this.selectedScenarioId !== this.uuidNIL &&
            hasAppliedLibrary(this.stateBudgetLibraries, this.selectedScenarioId)) {
          this.librarySelectItemValue = getAppliedLibraryId(this.stateBudgetLibraries, this.selectedScenarioId);
        }
      });
    });
  }

  formatAsCurrency(value: any) {
    if (hasValue(value)) {
      return formatAsCurrency(value);
    }

    return null;
  }

  onShowConfirmDeleteAlert() {
    this.confirmDeleteAlertData = {
      showDialog: true,
      heading: 'Warning',
      choice: true,
      message: 'Are you sure you want to delete?'
    };
  }

  onSubmitConfirmDeleteAlertResult(submit: boolean) {
    this.confirmDeleteAlertData = clone(emptyAlertData);

    if (submit) {
      this.librarySelectItemValue = null;
      this.deleteBudgetLibraryAction({libraryId: this.selectedBudgetLibrary.id});
    }
  }

  disableCrudButton() {
    if (this.hasSelectedLibrary) {
      const allBudgetLibraryDataIsValid: boolean = this.selectedBudgetLibrary.budgets.every((budget: Budget) => {
        return budget.budgetAmounts
            .every((budgetAmount: BudgetAmount) => this.rules['generalRules'].valueIsNotEmpty(budgetAmount.year) &&
                this.rules['generalRules'].valueIsNotEmpty(budgetAmount.value) === true);
      });

      if (this.selectedScenarioId !== this.uuidNIL) {
        const allInvestmentPlanDataIsValid: boolean = this.rules['generalRules'].valueIsNotEmpty(this.investmentPlan.minimumProjectCostLimit) === true &&
            this.rules['investmentRules'].minCostLimitGreaterThanZero(this.investmentPlan.minimumProjectCostLimit) === true &&
            this.rules['generalRules'].valueIsNotEmpty(this.investmentPlan.inflationRatePercentage) === true &&
            this.rules['generalRules'].valueIsWithinRange(this.investmentPlan.inflationRatePercentage, [0, 100]);

        return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedBudgetLibrary.name) === true &&
            allBudgetLibraryDataIsValid && allInvestmentPlanDataIsValid);
      } else {
        return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedBudgetLibrary.name) === true &&
            allBudgetLibraryDataIsValid);
      }
    }

    return true;
  }
}
</script>

<style>
.sharing label {
  padding-top: 0.5em;
}

.sharing {
  padding-top: 0;
  margin: 0;
}

.budget-year-amount-input {
  border: 1px solid;
  width: 100%;
}
</style>
