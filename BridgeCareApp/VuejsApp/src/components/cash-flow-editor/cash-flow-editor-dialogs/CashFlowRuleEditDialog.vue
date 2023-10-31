<template>
    <v-dialog max-width="850px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify-space-between align-center>
          <div class="ghd-control-dialog-header">Cash Flow Rule Settings: {{selectedCashFlowRule.name}}</div>
          <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
        </v-row>
      </v-card-title>
            <div style='height: 500px; max-width:850px' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 450px; overflow-y:auto;'>
            <v-data-table
                :headers="cashFlowRuleDistributionGridHeaders"
                :items="cashFlowDistributionRuleGridData"
                sort-icon=$vuetify.icons.ghd-table-sort
                hide-actions
                class="ghd-table v-table__overflow">
                <template v-slot:item ="{item}" slot="items" slot-scope="props">
                    <td>
                        <editDialog
                            :return-value.sync="item.value.durationInYears"
                            @save="onEditSelectedLibraryListData(item,'durationInYears')"
                            full-width
                            size="large"
                            lazy
                            persistent>
                            <v-text-field
                                id="CashFlowRuleEditDialog-yearReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :model-value="item.value.durationInYears"
                                :rules="[
                                    rules['generalRules'].valueIsNotEmpty,
                                    rules[
                                        'cashFlowRules'
                                    ].isDurationGreaterThanPreviousDuration(item,selectedCashFlowRule)]"/>
                            <template v-slot:input>
                                <v-text-field
                                    id="CashFlowRuleEditDialog-yearEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    v-model.number="item.value.durationInYears"
                                    :rules="[
                                        rules[
                                            'generalRules'
                                        ].valueIsNotEmpty,
                                        rules[
                                            'cashFlowRules'
                                        ].isDurationGreaterThanPreviousDuration(
                                            item,
                                            selectedCashFlowRule
                                        )
                                    ]"/>
                            </template>
                        </editDialog>
                    </td>
                    <td>
                        <editDialog
                            :return-value.sync="item.value.costCeiling"
                            size="large"
                            lazy
                            persistent
                            full-width
                            @save="onEditSelectedLibraryListData(item,'costCeiling')"
                            @open="onOpenCostCeilingEditDialog(item.value.id)">
                            <v-text-field
                                id="CashFlowRuleEditDialog-dollarReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :model-value="
                                    formatAsCurrency(item.value.costCeiling)"
                                :rules="[
                                    rules['generalRules']
                                        .valueIsNotEmpty,
                                    rules[
                                        'cashFlowRules'
                                    ].isAmountGreaterThanOrEqualToPreviousAmount(
                                        item,
                                        selectedCashFlowRule
                                    )
                                ]"/>
                            <template v-slot:input>
                                <v-text-field
                                    name="CashFlowRuleEditDialog-dollarEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    :id="item.value.id"
                                    v-model="item.value.costCeiling"
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
                                            item,
                                            selectedCashFlowRule
                                        )
                                    ]"/>
                            </template>
                        </editDialog>
                    </td>
                    <td>
                        <editDialog
                            :return-value.sync="item.value.yearlyPercentages"
                            @save="onEditSelectedLibraryListData(item,'yearlyPercentages')"
                            full-width
                            size="large"
                            lazy
                            persistent>
                            <v-text-field
                                id="CashFlowRuleEditDialog-distributionReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :model-value="item.value.yearlyPercentages"
                                :rules="[
                                    rules['generalRules']
                                        .valueIsNotEmpty,
                                    rules['cashFlowRules']
                                        .doesTotalOfPercentsEqualOneHundred
                                ]"/>
                            <template v-slot:input>
                                <v-text-field
                                    id="CashFlowRuleEditDialog-distributionEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    v-model="item.value.yearlyPercentages"
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
                        </editDialog>
                    </td>
                    <td>
                        <v-btn
                            @click="onDeleteCashFlowDistributionRule(item.value.id)"
                            class="ghd-blue"
                            icon>
                            <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                </template>
            </v-data-table>
                </div>
                <v-btn @click="onAddCashFlowDistributionRule" class='ghd-blue ghd-button' variant = "flat" id="CashFlowRuleEditDialog-addDistributionRule-btn">
                    Add Distribution Rule
                </v-btn>
            </div>
                     

                            
      <v-card-actions>
        <v-row justify-center row>
            <v-btn @click="onSubmit(false)" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined" id="CashFlowRuleEditDialog-cancel-btn">
            Cancel
          </v-btn>
          <v-btn @click="onSubmit(true)"
                 :disabled="!hasUnsavedChanges || !isDataValid"
                 id="CashFlowRuleEditDialog-submit-btn"
                 class='ghd-blue hd-button-text ghd-button' variant = "flat">
            Submit
          </v-btn>         
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { nextTick, ref, watch, Ref } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
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
//import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';

  const props = defineProps<{showDialog: boolean, selectedCashFlowRule: CashFlowRule}>()
  const emit = defineEmits(['submit']);

  let hasUnsavedChanges: boolean = false;
  let isDataValid: boolean = true;

  let cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = []
  let processedGridData: CashFlowDistributionRule[] = [];

  let cashFlowRuleDistributionGridHeaders: DataTableHeader[] = [
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
  let newCashFlowRuleLibrary: CashFlowRuleLibrary = {...emptyCashFlowRuleLibrary, id: getNewGuid()};
  let inputRules: InputValidationRules = clone(rules);

  function onSubmit(submit: boolean) {
    if (submit) {
      hasUnsavedChanges = false;
      emit('submit', processedGridData);
    } else {
      onSelectedSplitTreatmentIdChanged()
      emit('submit', null);      
    }
  }

function onEditSelectedLibraryListData(data: any, property: string) {
        let changedRule = data as CashFlowDistributionRule
        let rule = processedGridData.find(o => o.id === changedRule.id)
        if(!isNil(rule))
            switch (property) {
                case 'durationInYears':
                    rule.durationInYears = changedRule.durationInYears;
                    break;
                case 'costCeiling':
                    rule.costCeiling = unFormatAsCurrency(changedRule.costCeiling);
                    break;
                case 'yearlyPercentages':
                    rule.yearlyPercentages = changedRule.yearlyPercentages;
                    break;
            }
            onCashFlowDistributionRuleGridDataChanged()
    }

    function onAddCashFlowDistributionRule() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = modifyNewCashFlowDistributionRuleDefaultValues();
        processedGridData.push(clone(newCashFlowDistributionRule))
        cashFlowDistributionRuleGridData.push(newCashFlowDistributionRule);
    }

    function modifyNewCashFlowDistributionRuleDefaultValues() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = {
            ...emptyCashFlowDistributionRule,
            id: getNewGuid(),
        };

        if (cashFlowDistributionRuleGridData.length === 0) {
            return newCashFlowDistributionRule;
        } else {
            const durationInYears: number =
                getLastPropertyValue(
                    'durationInYears',
                    cashFlowDistributionRuleGridData,
                ) + 1;
            const costCeiling: number = getLastPropertyValue(
                'costCeiling',
                processedGridData,
            );
            const yearlyPercentages = getNewCashFlowDistributionRuleYearlyPercentages(
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

    function getNewCashFlowDistributionRuleYearlyPercentages(durationInYears: number) {
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

    function onOpenCostCeilingEditDialog(distributionRuleId: string) {
        nextTick(() => {
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

    watch(props.selectedCashFlowRule, () => onSelectedSplitTreatmentIdChanged)
    function onSelectedSplitTreatmentIdChanged() {
        cashFlowDistributionRuleGridData = hasValue(props.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(props.selectedCashFlowRule.cashFlowDistributionRules) : [];
        
        processedGridData = hasValue(props.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(props.selectedCashFlowRule.cashFlowDistributionRules) : [];
    }

    watch(processedGridData, () => onCashFlowDistributionRuleGridDataChanged)
    function onCashFlowDistributionRuleGridDataChanged() {
        if(checkIsDataValid())
            hasUnsavedChanges = 
                hasUnsavedChangesCore(
                    '',
                    processedGridData,
                    props.selectedCashFlowRule.cashFlowDistributionRules,
                )
    }

    function checkIsDataValid() : boolean
    {
        let rule =  clone(emptyCashFlowRule)
        rule.cashFlowDistributionRules = processedGridData;
        isDataValid = processedGridData.every((
                        distributionRule: CashFlowDistributionRule,
                        index: number,
                    ) => {
                        let isValid: boolean =
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.durationInYears,
                            ) === true &&
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.costCeiling,
                            ) === true &&
                            inputRules['generalRules'].valueIsNotEmpty(
                                distributionRule.yearlyPercentages,
                            ) === true &&
                            inputRules[
                                'cashFlowRules'
                            ].doesTotalOfPercentsEqualOneHundred(
                                distributionRule.yearlyPercentages,
                            ) === true;

                        if (index !== 0) {
                            isValid =
                                isValid &&
                                inputRules[
                                    'cashFlowRules'
                                ].isDurationGreaterThanPreviousDuration(
                                    distributionRule,
                                    rule,
                                ) === true &&
                                inputRules[
                                    'cashFlowRules'
                                ].isAmountGreaterThanOrEqualToPreviousAmount(
                                    distributionRule,
                                    rule,
                                ) === true;
                        }

                        return isValid;
                    },);
        return isDataValid;     
    }

    function formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    function unFormatAsCurrency(value: any) : number {
        if (hasValue(value)) {
            let num = Number(value.toString().replace(/[^0-9.-]+/g,""));
            return num;
        }
        return 0;
    }

    function onDeleteCashFlowDistributionRule(id: string) {
        cashFlowDistributionRuleGridData = cashFlowDistributionRuleGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
        processedGridData = processedGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
    }
</script>