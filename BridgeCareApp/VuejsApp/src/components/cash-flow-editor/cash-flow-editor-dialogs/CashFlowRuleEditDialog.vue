<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Cash Flow Rule Library</h3>
        </v-layout>
      </v-card-title>
        <v-card-text class="cash-flow-library-card">
            <v-btn @click="onAddCashFlowDistributionRule">
                <v-icon class="plus-icon" left
                    >fas fa-plus
                </v-icon>
                Add Distribution Rule
            </v-btn>
            <v-data-table
                :headers="cashFlowRuleDistributionGridHeaders"
                :items="cashFlowDistributionRuleGridData"
                class="elevation-1 v-table__overflow">
                <template slot="items" slot-scope="props">
                    <td>
                        <v-edit-dialog
                            :return-value.sync="props.item.durationInYears"
                            
                            full-width
                            large
                            lazy
                            persistent>
                            <v-text-field
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
                            @open="onOpenCostCeilingEditDialog(props.item.id)">
                            <v-text-field
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
                            class="ara-orange"
                            icon>
                            <v-icon>fas fa-trash</v-icon>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
        </v-card-text>                          

                            
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn @click="onSubmit(true)"
                 class="ara-blue-bg white--text">
            Submit
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
            Cancel
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import Component from 'vue-class-component';
import {Prop, Watch} from 'vue-property-decorator';
import {
  CashFlowDistributionRule,
  CashFlowRule,
  CashFlowRuleLibrary,
  emptyCashFlowDistributionRule,
  emptyCashFlowRuleLibrary
} from '@/shared/models/iAM/cash-flow';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {append, clone, find, findIndex, propEq, update} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';

@Component
export default class CashFlowRuleEditDialog extends Vue {
  @Prop() selectedCashFlowRule: CashFlowRule;
  @Prop() showDialog: boolean;
  cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = []
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
      this.$emit('submit', this.cashFlowDistributionRuleGridData);
    } else {
      this.onSelectedSplitTreatmentIdChanged()
      this.$emit('submit', null);      
    }
  }

  onEditSelectedLibraryListData(data: any, property: string) {
        switch (property) {
            case 'durationInYears':
            case 'costCeiling':
            case 'yearlyPercentages':
                this.cashFlowDistributionRuleGridData = update(
                            findIndex(
                                propEq('id', data.id),
                                this.cashFlowDistributionRuleGridData,
                            ),
                            {
                                ...data,
                                costCeiling: hasValue(data.costCeiling)
                                    ? parseFloat(
                                          data.costCeiling
                                              .toString()
                                              .replace(/(\$*)(,*)/g, ''),
                                      )
                                    : null,
                            } as CashFlowDistributionRule,
                            this.cashFlowDistributionRuleGridData)
        }
    }

    onAddCashFlowDistributionRule() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = this.modifyNewCashFlowDistributionRuleDefaultValues();

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
    }

    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    onDeleteCashFlowDistributionRule(id: string) {
        this.cashFlowDistributionRuleGridData = this.cashFlowDistributionRuleGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
    }
}
</script>