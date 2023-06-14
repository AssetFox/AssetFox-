<template>
  <v-dialog max-width="850px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-layout justify-space-between align-center>
          <div class="ghd-control-dialog-header">Cash Flow Rule Settings: {{this.selectedCashFlowRule.name}}</div>
          <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
        </v-layout>
      </v-card-title>

            
            <div style='height: 500px; max-width:850px' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 450px; overflow-y:auto;'>
            <v-data-table
                :headers="cashFlowRuleDistributionGridHeaders"
                :items="cashFlowDistributionRuleGridData"
                sort-icon=$vuetify.icons.ghd-table-sort
                hide-actions
                class="ghd-table v-table__overflow">
                <template slot="items" slot-scope="props">
                    <td>
                        <v-edit-dialog
                            :return-value.sync="props.item.durationInYears"
                            @save="onEditSelectedLibraryListData(props.item,'durationInYears')"
                            full-width
                            large
                            lazy
                            persistent>
                            <v-text-field
                                id="CashFlowRuleEditDialog-yearReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :value="props.item.durationInYears"
                                :rules="[
                                    rules['generalRules'].valueIsNotEmpty,
                                    rules[
                                        'cashFlowRules'
                                    ].isDurationGreaterThanPreviousDuration(props.item,selectedCashFlowRule)]"/>
                            <template slot="input">
                                <v-text-field
                                    id="CashFlowRuleEditDialog-yearEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    v-model.number="props.item.durationInYears"
                                    :rules="[
                                        rules[
                                            'generalRules'
                                        ].valueIsNotEmpty,
                                        rules[
                                            'cashFlowRules'
                                        ].isDurationGreaterThanPreviousDuration(
                                            props.item,
                                            selectedCashFlowRule
                                        )
                                    ]"/>
                            </template>
                        </v-edit-dialog>
                    </td>
                    <td>
                        <v-edit-dialog
                            :return-value.sync="props.item.costCeiling"
                            large
                            lazy
                            persistent
                            full-width
                            @save="onEditSelectedLibraryListData(props.item,'costCeiling')"
                            @open="onOpenCostCeilingEditDialog(props.item.id)">
                            <v-text-field
                                id="CashFlowRuleEditDialog-dollarReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :value="
                                    formatAsCurrency(props.item.costCeiling)"
                                :rules="[
                                    rules['generalRules']
                                        .valueIsNotEmpty,
                                    rules[
                                        'cashFlowRules'
                                    ].isAmountGreaterThanOrEqualToPreviousAmount(
                                        props.item,
                                        selectedCashFlowRule
                                    )
                                ]"/>
                            <template slot="input">
                                <v-text-field
                                    name="CashFlowRuleEditDialog-dollarEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    :id="props.item.id"
                                    v-model="props.item.costCeiling"
                                    v-currency="{
                                        currency: {
                                            prefix: '$',
                                            suffix: ''
                                        },
                                        locale: 'en-US',
                                        distractionFree: false
                                    }"
                                    :rules="[
                                        rules[
                                            'generalRules'
                                        ].valueIsNotEmpty,
                                        rules[
                                            'cashFlowRules'
                                        ].isAmountGreaterThanOrEqualToPreviousAmount(
                                            props.item,
                                            selectedCashFlowRule
                                        )
                                    ]"/>
                            </template>
                        </v-edit-dialog>
                    </td>
                    <td>
                        <v-edit-dialog
                            :return-value.sync="props.item.yearlyPercentages"
                            @save="onEditSelectedLibraryListData(props.item,'yearlyPercentages')"
                            full-width
                            large
                            lazy
                            persistent>
                            <v-text-field
                                id="CashFlowRuleEditDialog-distributionReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :value="props.item.yearlyPercentages"
                                :rules="[
                                    rules['generalRules']
                                        .valueIsNotEmpty,
                                    rules['cashFlowRules']
                                        .doesTotalOfPercentsEqualOneHundred
                                ]"/>
                            <template slot="input">
                                <v-text-field
                                    id="CashFlowRuleEditDialog-distributionEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    v-model="props.item.yearlyPercentages"
                                    :rules="[
                                        rules[
                                            'generalRules'
                                        ].valueIsNotEmpty,
                                        rules[
                                            'cashFlowRules'
                                        ]
                                            .doesTotalOfPercentsEqualOneHundred
                                    ]"/>
                            </template>
                        </v-edit-dialog>
                    </td>
                    <td>
                        <v-btn
                            @click="onDeleteCashFlowDistributionRule(props.item.id)"
                            class="ghd-blue"
                            icon>
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
                </div>
                <v-btn @click="onAddCashFlowDistributionRule" class='ghd-blue ghd-button' flat id="CashFlowRuleEditDialog-addDistributionRule-btn">
                    Add Distribution Rule
                </v-btn>
            </div>
                     

                            
      <v-card-actions>
        <v-layout justify-center row>
            <v-btn @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline id="CashFlowRuleEditDialog-cancel-btn">
            Cancel
          </v-btn>
          <v-btn @click="onSubmit(true)"
                 :disabled="!hasUnsavedChanges || !isDataValid"
                 id="CashFlowRuleEditDialog-submit-btn"
                 class='ghd-blue hd-button-text ghd-button' flat>
            Submit
          </v-btn>         
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Prop, Watch} from 'vue-property-decorator';
import {
  CashFlowDistributionRule,
  CashFlowRule,
  CashFlowRuleLibrary,
  emptyCashFlowDistributionRule,
  emptyCashFlowRule,
  emptyCashFlowRuleLibrary
} from '@/shared/models/iAM/cash-flow';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone, isNil} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';

@Component
export default class CashFlowRuleEditDialog extends Vue {
  @Prop() selectedCashFlowRule: CashFlowRule;
  @Prop() showDialog: boolean;

  hasUnsavedChanges: boolean = false;
  isDataValid: boolean = true;

  cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = []
  processedGridData: CashFlowDistributionRule[] = [];

    cashFlowRuleDistributionGridHeaders: DataTableHeader[] = [
        {
            text: 'Duration (yr)',
            value: 'durationInYears',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Cost Ceiling',
            value: 'costCeiling',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: 'Yearly Distribution (%)',
            value: 'yearlyPercentages',
            align: 'left',
            sortable: false,
            class: '',
            width: '31.6%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '4.2%',
        },
    ];
  newCashFlowRuleLibrary: CashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  onSubmit(submit: boolean) {
    if (submit) {
      this.hasUnsavedChanges = false;
      this.$emit('submit', this.processedGridData);
    } else {
      this.onSelectedSplitTreatmentIdChanged()
      this.$emit('submit', null);      
    }
  }

  onEditSelectedLibraryListData(data: any, property: string) {
        let changedRule = data as CashFlowDistributionRule
        let rule = this.processedGridData.find(o => o.id === changedRule.id)
        if(!isNil(rule))
            switch (property) {
                case 'durationInYears':
                    rule.durationInYears = changedRule.durationInYears;
                    break;
                case 'costCeiling':
                    rule.costCeiling = this.unFormatAsCurrency(changedRule.costCeiling);
                    break;
                case 'yearlyPercentages':
                    rule.yearlyPercentages = changedRule.yearlyPercentages;
                    break;
            }
            this.onCashFlowDistributionRuleGridDataChanged()
    }

    onAddCashFlowDistributionRule() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = this.modifyNewCashFlowDistributionRuleDefaultValues();
        this.processedGridData.push(clone(newCashFlowDistributionRule))
        this.cashFlowDistributionRuleGridData.push(newCashFlowDistributionRule);
    }

    modifyNewCashFlowDistributionRuleDefaultValues() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = {
            ...emptyCashFlowDistributionRule,
            id: getNewGuid(),
        };

        if (this.cashFlowDistributionRuleGridData.length === 0) {
            return newCashFlowDistributionRule;
        } else {
            const durationInYears: number =
                getLastPropertyValue(
                    'durationInYears',
                    this.cashFlowDistributionRuleGridData,
                ) + 1;
            const costCeiling: number = getLastPropertyValue(
                'costCeiling',
                this.cashFlowDistributionRuleGridData,
            );
            const yearlyPercentages = this.getNewCashFlowDistributionRuleYearlyPercentages(
                durationInYears,
            );

            return {
                ...newCashFlowDistributionRule,
                durationInYears: durationInYears,
                costCeiling:
                    newCashFlowDistributionRule.costCeiling! < costCeiling
                        ? costCeiling
                        : newCashFlowDistributionRule.costCeiling,
                yearlyPercentages: yearlyPercentages,
            };
        }
    }

    getNewCashFlowDistributionRuleYearlyPercentages(durationInYears: number) {
        const percentages: number[] = [];
        let percentage = 100 / durationInYears;

        if (100 % durationInYears !== 0) {
            percentage = Math.floor(percentage);

            for (let i = 0; i < durationInYears; i++) {
                if (i === durationInYears - 1) {
                    const sumCurrentPercentages: number = percentages.reduce(
                        (x, y) => x + y,
                    );
                    percentages.push(100 - sumCurrentPercentages);
                } else {
                    percentages.push(percentage);
                }
            }
        } else {
            for (let i = 0; i < durationInYears; i++) {
                percentages.push(percentage);
            }
        }

        return percentages.join('/');
    }

    onOpenCostCeilingEditDialog(distributionRuleId: string) {
        this.$nextTick(() => {
            const editDialogInputElement: HTMLElement = document.getElementById(
                distributionRuleId,
            ) as HTMLElement;
            if (hasValue(editDialogInputElement)) {
                setTimeout(() => {
                    editDialogInputElement.blur();
                    setTimeout(() => editDialogInputElement.click());
                }, 250);
            }
        });
    }

    @Watch('selectedCashFlowRule')
    onSelectedSplitTreatmentIdChanged() {
        this.cashFlowDistributionRuleGridData = hasValue(this.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(this.selectedCashFlowRule.cashFlowDistributionRules) : [];
        
        this.processedGridData = hasValue(this.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(this.selectedCashFlowRule.cashFlowDistributionRules) : [];
    }

    @Watch('processedGridData')
    onCashFlowDistributionRuleGridDataChanged() {
        if(this.checkIsDataValid())
            this.hasUnsavedChanges = 
                hasUnsavedChangesCore(
                    '',
                    this.processedGridData,
                    this.selectedCashFlowRule.cashFlowDistributionRules,
                )
    }

    checkIsDataValid() : boolean
    {
        let rule =  clone(emptyCashFlowRule)
        rule.cashFlowDistributionRules = this.processedGridData;
        this.isDataValid = this.processedGridData.every((
                        distributionRule: CashFlowDistributionRule,
                        index: number,
                    ) => {
                        let isValid: boolean =
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.durationInYears,
                            ) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.costCeiling,
                            ) === true &&
                            this.rules['generalRules'].valueIsNotEmpty(
                                distributionRule.yearlyPercentages,
                            ) === true &&
                            this.rules[
                                'cashFlowRules'
                            ].doesTotalOfPercentsEqualOneHundred(
                                distributionRule.yearlyPercentages,
                            ) === true;

                        if (index !== 0) {
                            isValid =
                                isValid &&
                                this.rules[
                                    'cashFlowRules'
                                ].isDurationGreaterThanPreviousDuration(
                                    distributionRule,
                                    rule,
                                ) === true &&
                                this.rules[
                                    'cashFlowRules'
                                ].isAmountGreaterThanOrEqualToPreviousAmount(
                                    distributionRule,
                                    rule,
                                ) === true;
                        }

                        return isValid;
                    },);
        return this.isDataValid;     
    }

    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    unFormatAsCurrency(value: any) : number {
        if (hasValue(value)) {
            let num = Number(value.toString().replace(/[^0-9.-]+/g,""));
            return num;
        }
        return 0;
    }

    onDeleteCashFlowDistributionRule(id: string) {
        this.cashFlowDistributionRuleGridData = this.cashFlowDistributionRuleGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
        this.processedGridData = this.processedGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
    }
}
</script>