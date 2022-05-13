<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="onShowCreateCashFlowRuleLibraryDialog(false)"
                        class="ara-blue-bg white--text"
                        v-show="!hasScenario"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        :items="librarySelectItems"
                        label="Select a Cash Flow Library"
                        outline
                        v-if="!hasSelectedLibrary || hasScenario"
                        v-model="librarySelectItemValue"
                    >
                    </v-select>
                    <v-text-field
                        label="Library Name"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedCashFlowRuleLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                    >
                        <template slot="append">
                            <v-btn
                                @click="librarySelectItemValue = null"
                                class="ara-orange"
                                icon
                            >
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if='hasSelectedLibrary && !hasScenario'>
                        Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                    </div>
                    <v-checkbox
                        class="sharing"
                        label="Shared"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedCashFlowRuleLibrary.isShared"
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <div class="cash-flow-library-tables">
                <v-layout justify-center row>
                    <v-flex xs8>
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddCashFlowRule">
                                    <v-icon class="plus-icon" left
                                        >fas fa-plus
                                    </v-icon>
                                    Add Cash Flow Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table
                                    :headers="cashFlowRuleGridHeaders"
                                    :items="cashFlowRuleGridData"
                                    class="elevation-1 v-table__overflow"
                                    item-key="id"
                                >
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-radio-group
                                                :mandatory="false"
                                                class="cash-flow-radio-group"
                                                v-model="
                                                    cashFlowRuleRadioBtnValue
                                                "
                                            >
                                                <v-radio
                                                    :value="props.item.id"
                                                ></v-radio>
                                            </v-radio-group>
                                        </td>
                                        <td>
                                            <v-edit-dialog
                                                :return-value.sync="
                                                    props.item.name
                                                "
                                                large
                                                lazy
                                                persistent
                                                @save="
                                                    onEditSelectedLibraryListData(
                                                        props.item,
                                                        'description',
                                                    )
                                                "
                                            >
                                                <v-text-field
                                                    readonly
                                                    single-line
                                                    class="sm-txt"
                                                    :value="props.item.name"
                                                    :rules="[
                                                        rules.generalRules
                                                            .valueIsNotEmpty,
                                                    ]"
                                                />
                                                <template slot="input">
                                                    <v-textarea
                                                        label="Description"
                                                        no-resize
                                                        outline
                                                        rows="5"
                                                        :rules="[
                                                            rules.generalRules
                                                                .valueIsNotEmpty,
                                                        ]"
                                                        v-model="
                                                            props.item.name
                                                        "
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-menu
                                                bottom
                                                min-height="500px"
                                                min-width="500px"
                                            >
                                                <template slot="activator">
                                                    <v-text-field
                                                        readonly
                                                        single-line
                                                        class="sm-txt"
                                                        :value="
                                                            props.item
                                                                .criterionLibrary
                                                                .mergedCriteriaExpression
                                                        "
                                                    />
                                                </template>
                                                <v-card>
                                                    <v-card-text>
                                                        <v-textarea
                                                            :value="
                                                                props.item
                                                                    .criterionLibrary
                                                                    .mergedCriteriaExpression
                                                            "
                                                            full-width
                                                            no-resize
                                                            outline
                                                            readonly
                                                            rows="5"
                                                        />
                                                    </v-card-text>
                                                </v-card>
                                            </v-menu>

                                            <v-btn
                                                @click="
                                                    onEditCashFlowRuleCriterionLibrary(
                                                        props.item,
                                                    )
                                                "
                                                class="edit-icon"
                                                icon
                                            >
                                                <v-icon>fas fa-edit</v-icon>
                                            </v-btn>
                                        </td>
                                        <td>
                                            <v-btn
                                                @click="
                                                    onDeleteCashFlowRule(
                                                        props.item.id,
                                                    )
                                                "
                                                class="ara-orange"
                                                icon
                                            >
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </td>
                                    </template>
                                </v-data-table>
                            </v-card-text>
                        </v-card>
                    </v-flex>
                    <v-flex v-if="selectedCashFlowRule.id !== uuidNIL" xs4>
                        <v-card>
                            <v-card-title>
                                <v-btn @click="onAddCashFlowDistributionRule">
                                    <v-icon class="plus-icon" left
                                        >fas fa-plus
                                    </v-icon>
                                    Add Distribution Rule
                                </v-btn>
                            </v-card-title>
                            <v-card-text class="cash-flow-library-card">
                                <v-data-table
                                    :headers="
                                        cashFlowRuleDistributionGridHeaders
                                    "
                                    :items="cashFlowDistributionRuleGridData"
                                    class="elevation-1 v-table__overflow"
                                >
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-edit-dialog
                                                :return-value.sync="
                                                    props.item.durationInYears
                                                "
                                                @save="
                                                    onEditSelectedLibraryListData(
                                                        props.item,
                                                        'durationInYears',
                                                    )
                                                "
                                                full-width
                                                large
                                                lazy
                                                persistent
                                            >
                                                <v-text-field
                                                    readonly
                                                    single-line
                                                    class="sm-txt"
                                                    :value="
                                                        props.item
                                                            .durationInYears
                                                    "
                                                    :rules="[
                                                        rules['generalRules']
                                                            .valueIsNotEmpty,
                                                        rules[
                                                            'cashFlowRules'
                                                        ].isDurationGreaterThanPreviousDuration(
                                                            props.item,
                                                            selectedCashFlowRule,
                                                        ),
                                                    ]"
                                                />
                                                <template slot="input">
                                                    <v-text-field
                                                        label="Edit"
                                                        single-line
                                                        v-model.number="
                                                            props.item
                                                                .durationInYears
                                                        "
                                                        :rules="[
                                                            rules[
                                                                'generalRules'
                                                            ].valueIsNotEmpty,
                                                            rules[
                                                                'cashFlowRules'
                                                            ].isDurationGreaterThanPreviousDuration(
                                                                props.item,
                                                                selectedCashFlowRule,
                                                            ),
                                                        ]"
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog
                                                :return-value.sync="
                                                    props.item.costCeiling
                                                "
                                                large
                                                lazy
                                                persistent
                                                full-width
                                                @open="
                                                    onOpenCostCeilingEditDialog(
                                                        props.item.id,
                                                    )
                                                "
                                                @save="
                                                    onEditSelectedLibraryListData(
                                                        props.item,
                                                        'costCeiling',
                                                    )
                                                "
                                            >
                                                <v-text-field
                                                    readonly
                                                    single-line
                                                    class="sm-txt"
                                                    :value="
                                                        formatAsCurrency(
                                                            props.item
                                                                .costCeiling,
                                                        )
                                                    "
                                                    :rules="[
                                                        rules['generalRules']
                                                            .valueIsNotEmpty,
                                                        rules[
                                                            'cashFlowRules'
                                                        ].isAmountGreaterThanOrEqualToPreviousAmount(
                                                            props.item,
                                                            selectedCashFlowRule,
                                                        ),
                                                    ]"
                                                />
                                                <template slot="input">
                                                    <v-text-field
                                                        label="Edit"
                                                        single-line
                                                        :id="props.item.id"
                                                        v-model="
                                                            props.item
                                                                .costCeiling
                                                        "
                                                        v-currency="{
                                                            currency: {
                                                                prefix: '$',
                                                                suffix: '',
                                                            },
                                                            locale: 'en-US',
                                                            distractionFree: false,
                                                        }"
                                                        :rules="[
                                                            rules[
                                                                'generalRules'
                                                            ].valueIsNotEmpty,
                                                            rules[
                                                                'cashFlowRules'
                                                            ].isAmountGreaterThanOrEqualToPreviousAmount(
                                                                props.item,
                                                                selectedCashFlowRule,
                                                            ),
                                                        ]"
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-edit-dialog
                                                :return-value.sync="
                                                    props.item.yearlyPercentages
                                                "
                                                @save="
                                                    onEditSelectedLibraryListData(
                                                        props.item,
                                                        'yearlyPercentages',
                                                    )
                                                "
                                                full-width
                                                large
                                                lazy
                                                persistent
                                            >
                                                <v-text-field
                                                    readonly
                                                    single-line
                                                    class="sm-txt"
                                                    :value="
                                                        props.item
                                                            .yearlyPercentages
                                                    "
                                                    :rules="[
                                                        rules['generalRules']
                                                            .valueIsNotEmpty,
                                                        rules['cashFlowRules']
                                                            .doesTotalOfPercentsEqualOneHundred,
                                                    ]"
                                                />
                                                <template slot="input">
                                                    <v-text-field
                                                        label="Edit"
                                                        single-line
                                                        v-model="
                                                            props.item
                                                                .yearlyPercentages
                                                        "
                                                        :rules="[
                                                            rules[
                                                                'generalRules'
                                                            ].valueIsNotEmpty,
                                                            rules[
                                                                'cashFlowRules'
                                                            ]
                                                                .doesTotalOfPercentsEqualOneHundred,
                                                        ]"
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            <v-btn
                                                @click="
                                                    onDeleteCashFlowDistributionRule(
                                                        props.item.id,
                                                    )
                                                "
                                                class="ara-orange"
                                                icon
                                            >
                                                <v-icon>fas fa-trash</v-icon>
                                            </v-btn>
                                        </td>
                                    </template>
                                </v-data-table>
                            </v-card-text>
                        </v-card>
                    </v-flex>
                </v-layout>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea
                        label="Description"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedCashFlowRuleLibrary.description"
                        @input="
                            selectedCashFlowRuleLibrary = {
                                ...selectedCashFlowRuleLibrary,
                                description: $event,
                            }
                        "
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout
                justify-end
                row
                v-show="hasSelectedLibrary || hasScenario"
            >
                <v-btn
                    :disabled="disableCrudButtonsResult || !hasUnsavedChanges"
                    @click="onUpsertScenarioCashFlowRules"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario"
                >
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                    @click="onUpsertCashFlowRuleLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                >
                    Update Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButtons()"
                    @click="onShowCreateCashFlowRuleLibraryDialog(true)"
                    class="ara-blue-bg white--text"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onDeleteCashFlowRuleLibrary"
                    class="ara-orange-bg white--text"
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                >
                    Delete Library
                </v-btn>
                <v-btn
                    @click="onDiscardChanges"
                    class="ara-orange-bg white--text"
                    v-show="hasSelectedLibrary || hasScenario"
                    :disabled="!hasUnsavedChanges"
                >
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateCashFlowRuleLibraryDialog
            :dialogData="createCashFlowRuleLibraryDialogData"
            @submit="onSubmitCreateCashFlowRuleLibraryDialogResult"
        />

        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onSubmitCriterionLibraryEditorDialogResult"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State, Getter } from 'vuex-class';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    append,
    clone,
    find,
    findIndex,
    isNil,
    prepend,
    propEq,
    update,
    reject,
} from 'ramda';
import {
    CashFlowDistributionRule,
    CashFlowRule,
    CashFlowRuleLibrary,
    emptyCashFlowDistributionRule,
    emptyCashFlowRule,
    emptyCashFlowRuleLibrary,
} from '@/shared/models/iAM/cash-flow';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {
    CreateCashFlowRuleLibraryDialogData,
    emptyCreateCashFlowLibraryDialogData,
} from '@/shared/models/modals/create-cash-flow-rule-library-dialog-data';
import CreateCashFlowRuleLibraryDialog from '@/components/cash-flow-editor/cash-flow-editor-dialogs/CreateCashFlowRuleLibraryDialog.vue';
import { formatAsCurrency } from '@/shared/utils/currency-formatter';
import { hasValue } from '@/shared/utils/has-value-util';
import { getLastPropertyValue } from '@/shared/utils/getter-utils';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';

@Component({
    components: {
        CreateCashFlowRuleLibraryDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class CashFlowEditor extends Vue {
    @State(state => state.cashFlowModule.cashFlowRuleLibraries)
    stateCashFlowRuleLibraries: CashFlowRuleLibrary[];
    @State(state => state.cashFlowModule.selectedCashFlowRuleLibrary)
    stateSelectedCashRuleFlowLibrary: CashFlowRuleLibrary;
    @State(state => state.cashFlowModule.scenarioCashFlowRules)
    stateScenarioCashFlowRules: CashFlowRule[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('getCashFlowRuleLibraries') getCashFlowRuleLibrariesAction: any;
    @Action('selectCashFlowRuleLibrary') selectCashFlowRuleLibraryAction: any;
    @Action('upsertCashFlowRuleLibrary') upsertCashFlowRuleLibraryAction: any;
    @Action('deleteCashFlowRuleLibrary') deleteCashFlowRuleLibraryAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioCashFlowRules') getScenarioCashFlowRulesAction: any;
    @Action('upsertScenarioCashFlowRules')

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    upsertScenarioCashFlowRulesAction: any;
    hasSelectedLibrary: boolean = false;
    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = null;
    selectedCashFlowRuleLibrary: CashFlowRuleLibrary = clone(
        emptyCashFlowRuleLibrary,
    );
    cashFlowRuleGridHeaders: DataTableHeader[] = [
        {
            text: 'Select',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '5%',
        },
        {
            text: 'Rule Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '25%',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '65%',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '5%',
        },
    ];
    cashFlowRuleGridData: CashFlowRule[] = [];
    cashFlowRuleRadioBtnValue: string = '';
    selectedCashFlowRule: CashFlowRule = clone(emptyCashFlowRule);
    selectedCashFlowRuleForCriteriaEdit: CashFlowRule = clone(
        emptyCashFlowRule,
    );
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
    cashFlowDistributionRuleGridData: CashFlowDistributionRule[] = [];
    createCashFlowRuleLibraryDialogData: CreateCashFlowRuleLibraryDialogData = clone(
        emptyCreateCashFlowLibraryDialogData,
    );
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = clone(rules);
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getCashFlowRuleLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.CashFlow) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Unable to identify selected scenario.',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioCashFlowRulesAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateCashFlowRuleLibraries')
    onStateCashFlowRuleLibrariesChanged() {
        this.librarySelectItems = this.stateCashFlowRuleLibraries.map(
            (library: CashFlowRuleLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectCashFlowRuleLibraryAction(this.librarySelectItemValue);
    }

    @Watch('stateSelectedCashRuleFlowLibrary')
    onStateSelectedCashFlowRuleLibraryChanged() {
        this.selectedCashFlowRuleLibrary = clone(
            this.stateSelectedCashRuleFlowLibrary,
        );
        console.log('message');
    }

    @Watch('selectedCashFlowRuleLibrary', {deep: true})
    onSelectedCashFlowRuleLibraryChanged() {
        this.hasSelectedLibrary =
            this.selectedCashFlowRuleLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (this.hasScenario) {
            this.cashFlowRuleGridData = this.selectedCashFlowRuleLibrary.cashFlowRules.map(
                (cashFlowRule: CashFlowRule) => ({
                    ...cashFlowRule,
                    id: getNewGuid(),
                    cashFlowDistributionRules: cashFlowRule.cashFlowDistributionRules.map(
                        (distributionRule: CashFlowDistributionRule) => ({
                            ...distributionRule,
                            id: getNewGuid(),
                        }),
                    ),
                }),
            );
        } else {
            this.cashFlowRuleGridData = clone(
                this.selectedCashFlowRuleLibrary.cashFlowRules,
            );
        }
    }

    @Watch('stateScenarioCashFlowRules')
    onStateScenarioCashFlowRulesChanged() {
        if (this.hasScenario) {
            this.cashFlowRuleGridData = clone(this.stateScenarioCashFlowRules);
        }
    }

    @Watch('cashFlowRuleGridData')
    onCashFlowRuleGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore(
                  '',
                  this.cashFlowRuleGridData,
                  this.stateScenarioCashFlowRules,
              )
            : hasUnsavedChangesCore(
                  '',
                  {
                      ...clone(this.selectedCashFlowRuleLibrary),
                      cashFlowRules: clone(this.cashFlowRuleGridData),
                  },
                  this.stateSelectedCashRuleFlowLibrary,
              );
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        this.onSelectCashFlowRule();
    }

    @Watch('cashFlowRuleRadioBtnValue')
    onSplitTreatmentRadioValueChanged() {
        this.onSelectCashFlowRule();
    }

    @Watch('selectedCashFlowRule')
    onSelectedSplitTreatmentIdChanged() {
        this.cashFlowDistributionRuleGridData = hasValue(
            this.selectedCashFlowRule.cashFlowDistributionRules,
        )
            ? clone(this.selectedCashFlowRule.cashFlowDistributionRules)
            : [];
    }

    onSelectCashFlowRule() {
        const cashFlowRule: CashFlowRule = find(
            propEq('id', this.cashFlowRuleRadioBtnValue),
            this.cashFlowRuleGridData,
        ) as CashFlowRule;

        if (hasValue(cashFlowRule)) {
            this.selectedCashFlowRule = clone(cashFlowRule);
        } else {
            this.selectedCashFlowRule = clone(emptyCashFlowRule);
        }
    }

    onShowCreateCashFlowRuleLibraryDialog(createAsNewLibrary: boolean) {
        this.createCashFlowRuleLibraryDialogData = {
            showDialog: true,
            cashFlowRules: createAsNewLibrary ? this.cashFlowRuleGridData : [],
        };
    }

    onSubmitCreateCashFlowRuleLibraryDialogResult(
        cashFlowRuleLibrary: CashFlowRuleLibrary,
    ) {
        this.createCashFlowRuleLibraryDialogData = clone(
            emptyCreateCashFlowLibraryDialogData,
        );

        if (!isNil(cashFlowRuleLibrary)) {
            this.upsertCashFlowRuleLibraryAction(cashFlowRuleLibrary);
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = cashFlowRuleLibrary.name;
        }
    }

    onAddCashFlowRule() {
        const newCashFlowRule: CashFlowRule = {
            ...emptyCashFlowRule,
            name: `Unnamed Rule ${this.cashFlowRuleGridData.length + 1}`,
            id: getNewGuid(),
        };

        this.cashFlowRuleGridData = prepend(
            newCashFlowRule,
            this.cashFlowRuleGridData,
        );
    }

    onDeleteCashFlowRule(cashFlowRuleId: string) {
        this.cashFlowRuleGridData = reject(
            propEq('id', cashFlowRuleId),
            this.cashFlowRuleGridData,
        );
    }

    onAddCashFlowDistributionRule() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = this.modifyNewCashFlowDistributionRuleDefaultValues();

        this.cashFlowRuleGridData = update(
            findIndex(
                propEq('id', this.selectedCashFlowRule.id),
                this.cashFlowRuleGridData,
            ),
            {
                ...this.selectedCashFlowRule,
                cashFlowDistributionRules: append(
                    newCashFlowDistributionRule,
                    this.selectedCashFlowRule.cashFlowDistributionRules,
                ),
            },
            this.cashFlowRuleGridData,
        );
    }

    modifyNewCashFlowDistributionRuleDefaultValues() {
        const newCashFlowDistributionRule: CashFlowDistributionRule = {
            ...emptyCashFlowDistributionRule,
            id: getNewGuid(),
        };

        if (this.selectedCashFlowRule.cashFlowDistributionRules.length === 0) {
            return newCashFlowDistributionRule;
        } else {
            const durationInYears: number =
                getLastPropertyValue(
                    'durationInYears',
                    this.selectedCashFlowRule.cashFlowDistributionRules,
                ) + 1;
            const costCeiling: number = getLastPropertyValue(
                'costCeiling',
                this.selectedCashFlowRule.cashFlowDistributionRules,
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

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedCashFlowRuleLibrary.owner) == getUserName();
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedCashFlowRuleLibrary.owner);
        }
        
        return getUserName();
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

    onDeleteCashFlowDistributionRule(cashFlowDistributionRuleId: string) {
        this.cashFlowRuleGridData = update(
            findIndex(
                propEq('id', this.selectedCashFlowRule),
                this.cashFlowRuleGridData,
            ),
            {
                ...this.selectedCashFlowRule,
                cashFlowDistributionRules: reject(
                    propEq('id', cashFlowDistributionRuleId),
                    this.selectedCashFlowRule.cashFlowDistributionRules,
                ),
            },
            this.cashFlowRuleGridData,
        );
    }

    onEditCashFlowRuleCriterionLibrary(cashFlowRule: CashFlowRule) {
        this.selectedCashFlowRuleForCriteriaEdit = clone(cashFlowRule);

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: this.selectedCashFlowRuleForCriteriaEdit.criterionLibrary
                .id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario,
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(
        criterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorDialogData = clone(
            emptyCriterionLibraryEditorDialogData,
        );

        if (
            !isNil(criterionLibrary) &&
            this.selectedCashFlowRuleForCriteriaEdit.id !== this.uuidNIL
        ) {
            this.cashFlowRuleGridData = update(
                findIndex(
                    propEq('id', this.selectedCashFlowRuleForCriteriaEdit.id),
                    this.cashFlowRuleGridData,
                ),
                {
                    ...this.selectedCashFlowRuleForCriteriaEdit,
                    criterionLibrary: criterionLibrary,
                },
                this.cashFlowRuleGridData,
            );

            this.selectedCashFlowRuleForCriteriaEdit = clone(emptyCashFlowRule);
        }
    }

    onEditSelectedLibraryListData(data: any, property: string) {
        switch (property) {
            case 'description':
                this.cashFlowRuleGridData = update(
                    findIndex(propEq('id', data.id), this.cashFlowRuleGridData),
                    data as CashFlowRule,
                    this.cashFlowRuleGridData,
                );
                break;
            case 'durationInYears':
            case 'costCeiling':
            case 'yearlyPercentages':
                this.cashFlowRuleGridData = update(
                    findIndex(
                        propEq('id', this.selectedCashFlowRule.id),
                        this.cashFlowRuleGridData,
                    ),
                    {
                        ...this.selectedCashFlowRule,
                        cashFlowDistributionRules: update(
                            findIndex(
                                propEq('id', data.id),
                                this.selectedCashFlowRule
                                    .cashFlowDistributionRules,
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
                            this.selectedCashFlowRule.cashFlowDistributionRules,
                        ),
                    },
                    this.cashFlowRuleGridData,
                );
        }
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

    onUpsertScenarioCashFlowRules() {
        this.upsertScenarioCashFlowRulesAction({
            scenarioCashFlowRules: this.cashFlowRuleGridData,
            scenarioId: this.selectedScenarioId,
        });
    }

    onUpsertCashFlowRuleLibrary() {
        const cashFlowRuleLibrary: CashFlowRuleLibrary = {
            ...clone(this.selectedCashFlowRuleLibrary),
            cashFlowRules: clone(this.cashFlowRuleGridData),
        };

        this.upsertCashFlowRuleLibraryAction(cashFlowRuleLibrary);
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.cashFlowRuleGridData = clone(
                    this.stateScenarioCashFlowRules,
                );
            }
        });
    }

    formatAsCurrency(value: any) {
        if (hasValue(value)) {
            return formatAsCurrency(value);
        }
        return null;
    }

    disableCrudButtons() {
        const allDataIsValid = this.cashFlowRuleGridData.every(
            (rule: CashFlowRule) => {
                const allSubDataIsValid = rule.cashFlowDistributionRules.every(
                    (
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
                    },
                );

                return (
                    this.rules['generalRules'].valueIsNotEmpty(rule.name) ===
                        true && allSubDataIsValid
                );
            },
        );

        if (!this.hasScenario && this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedCashFlowRuleLibrary.name,
                ) === true && allDataIsValid
            );
        }
        this.disableCrudButtonsResult = !allDataIsValid;
        return !allDataIsValid;
    }

    onDeleteCashFlowRuleLibrary() {
        this.confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.librarySelectItemValue = null;
            this.deleteCashFlowRuleLibraryAction(
                this.selectedCashFlowRuleLibrary.id,
            );
        }
    }
}
</script>

<style>
.cash-flow-library-tables {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.cash-flow-library-tables .v-menu--inline {
    width: 85%;
}

.cash-flow-library-tables .v-menu__activator a,
.cash-flow-library-tables .v-menu--inline input {
    width: 100%;
}

.cash-flow-radio-group .v-input--radio-group__input {
    padding-top: 25px;
}

.output {
    border-bottom: 1px solid;
}

.cash-flow-library-card {
    height: 330px;
    overflow-y: auto;
    overflow-x: hidden;
}

.invalid-input {
    color: red;
}

.amount-div {
    width: 208px;
}

.split-treatment-limit-currency-input {
    border: 1px solid;
    width: 100%;
}

.split-treatment-limit-amount-rule-span {
    font-size: 0.8em;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
