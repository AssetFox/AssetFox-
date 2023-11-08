<template>
    <v-dialog max-width="850px" v-model="showDialogComputed">
    <v-card>
      <v-card-title class="ghd-dialog-box-padding-top">
        <v-row justify="space-between" align-center>
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
                sort-icon=ghd-table-sort
                hide-actions
                class="ghd-table v-table__overflow">
                <template v-slot:item ="item" slot="items" slot-scope="props">
                    <tr>
                    <td>
                        <editDialog
                            :return-value.sync="item.item.durationInYears"
                            @save="onEditSelectedLibraryListData(item,'durationInYears')"
                            full-width
                            size="large"
                            lazy
                            persistent>
                            <v-text-field
                                id="CashFlowRuleEditDialog-yearReadOnly-vtextfield"
                                readonly
                                single-line
                                variant="underlined"
                                class="sm-txt"
                                :model-value="item.item.durationInYears"
                                :rules="[
                                    rules['generalRules'].valueIsNotEmpty,
                                    rules[
                                        'cashFlowRules'
                                    ].isDurationGreaterThanPreviousDuration(item.item,selectedCashFlowRule)]"/>
                            <template v-slot:input>
                                <v-text-field
                                    id="CashFlowRuleEditDialog-yearEdit-vtextfield"
                                    label="Edit"
                                    single-line
                                    variant="underlined"
                                    v-model.number="item.item.durationInYears"
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
                            :return-value.sync="item.item.costCeiling"
                            size="large"
                            lazy
                            persistent
                            full-width
                            @save="onEditSelectedLibraryListData(item.item,'costCeiling')"
                            @open="onOpenCostCeilingEditDialog(item.item.id)">
                            <v-text-field
                                id="CashFlowRuleEditDialog-dollarReadOnly-vtextfield"
                                readonly
                                single-line
                                variant="underlined"
                                class="sm-txt"
                                :model-value="
                                    formatAsCurrencyLocaal(item.item.costCeiling)"
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
                                    variant="underlined"
                                    :id="item.item.id"
                                    v-model="item.item.costCeiling"
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
                            :return-value.sync="item.item.yearlyPercentages"
                            @save="onEditSelectedLibraryListData(item,'yearlyPercentages')"
                            full-width
                            size="large"
                            lazy
                            persistent>
                            <v-text-field
                            variant="underlined"
                                id="CashFlowRuleEditDialog-distributionReadOnly-vtextfield"
                                readonly
                                single-line
                                class="sm-txt"
                                :model-value="item.item.yearlyPercentages"
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
                                    variant="underlined"
                                    v-model="item.item.yearlyPercentages"
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
                            @click="onDeleteCashFlowDistributionRule(item.item.id)"
                            class="ghd-blue"
                            variant="flat"
                            icon>
                            <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                        </v-btn>
                    </td>
                    </tr>
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
import { nextTick, ref, watch, Ref, computed } from 'vue';
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
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getUrl } from '@/shared/utils/get-url';

  const props = defineProps<{showDialog: boolean, selectedCashFlowRule: CashFlowRule}>()
  let showDialogComputed = computed(() => props.showDialog);
  const emit = defineEmits(['submit']);

  let hasUnsavedChanges = ref(false);
  let isDataValid = ref(true);

  let cashFlowDistributionRuleGridData = ref<CashFlowDistributionRule[]>([])
  let processedGridData: CashFlowDistributionRule[] = [];

  let cashFlowRuleDistributionGridHeaders: any[] = [
    {
        title: 'Duration (yr)',
        key: 'durationInYears',
        align: 'left',
        sortable: false,
        class: '',
        width: '31.6%',
    },
    {
        title: 'Cost Ceiling',
        key: 'costCeiling',
        align: 'left',
        sortable: false,
        class: '',
        width: '31.6%',
    },
    {
        title: 'Yearly Distribution (%)',
        key: 'yearlyPercentages',
        align: 'left',
        sortable: false,
        class: '',
        width: '31.6%',
    },
    {
        title: '',
        key: '',
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
      hasUnsavedChanges.value = false;
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
        cashFlowDistributionRuleGridData.value.push(newCashFlowDistributionRule);
    }

    function modifyNewCashFlowDistributionRuleDefaultValues() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = {
            ...emptyCashFlowDistributionRule,
            id: getNewGuid(),
        };

        if (cashFlowDistributionRuleGridData.value.length === 0) {
            return newCashFlowDistributionRule;
        } else {
            const durationInYears: number =
                getLastPropertyValue(
                    'durationInYears',
                    cashFlowDistributionRuleGridData.value,
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

    watch(() => props.selectedCashFlowRule, () => onSelectedSplitTreatmentIdChanged())
    function onSelectedSplitTreatmentIdChanged() {
        cashFlowDistributionRuleGridData.value = hasValue(props.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(props.selectedCashFlowRule.cashFlowDistributionRules) : [];
        
        processedGridData = hasValue(props.selectedCashFlowRule.cashFlowDistributionRules)
            ? clone(props.selectedCashFlowRule.cashFlowDistributionRules) : [];
    }

    watch(processedGridData, () => onCashFlowDistributionRuleGridDataChanged())
    function onCashFlowDistributionRuleGridDataChanged() {
        if(checkIsDataValid())
            hasUnsavedChanges.value = 
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
        isDataValid.value = processedGridData.every((
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
        return isDataValid.value;     
    }

    function formatAsCurrencyLocaal(value: any) {
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
        cashFlowDistributionRuleGridData.value = cashFlowDistributionRuleGridData.value.filter((rule: CashFlowDistributionRule) => rule.id !== id)
        processedGridData = processedGridData.filter((rule: CashFlowDistributionRule) => rule.id !== id)
    }
</script>