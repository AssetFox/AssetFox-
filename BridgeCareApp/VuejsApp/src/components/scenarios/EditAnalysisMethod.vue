<template>
    <v-form ref="form" v-model="valid" lazy-validation>
        <v-layout column>
            <v-flex xs6>
                <v-layout column>
                    <v-layout justify-center>
                        <v-flex xs4>
                            <v-subheader class="ghd-control-label ghd-md-gray">Weighting</v-subheader>
                            <v-select
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="weightingAttributes"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'attribute',
                                        $event,
                                    )
                                "
                                outline
                                clearable
                                :value="analysisMethod.attribute"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-flex>
                        <v-flex xs4>
                            <v-subheader class="ghd-control-label ghd-md-gray">Optimization Strategy</v-subheader>
                            <v-select class="ghd-select ghd-control-border ghd-control-text"
                                :items="optimizationStrategy"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'optimizationStrategy',
                                        $event,
                                    )
                                "
                                outline
                                :value="analysisMethod.optimizationStrategy"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-flex>
                        <v-flex xs4>
                            <v-subheader class="ghd-control-label ghd-md-gray">Spending Strategy</v-subheader>
                            <v-select
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="spendingStrategy"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'spendingStrategy',
                                        $event,
                                    )
                                "
                                outline
                                :value="analysisMethod.spendingStrategy"
                            >
                            </v-select>
                        </v-flex>                        
                    </v-layout>
                    <v-layout justify-center>
                        <v-spacer />
                         <v-flex xs6>
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Attribute</v-subheader>
                            <v-select
                                class="ghd-select ghd-control-text ghd-control-border"
                                :items="benefitAttributes"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetBenefitProperty('attribute', $event)
                                "
                                outline
                                :value="analysisMethod.benefit.attribute"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-flex>
                        <v-flex xs6>
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Limit</v-subheader>
                            <v-text-field
                                style="margin:0px"
                                class="ghd-control-text ghd-control-border"
                                @input="onSetBenefitProperty('limit', $event)"
                                outline
                                type="number"
                                min="0"
                                :value.number="analysisMethod.benefit.limit"
                                :rules="[
                                    rules['generalRules'].valueIsNotEmpty,
                                    rules['generalRules'].valueIsNotNegative(
                                        analysisMethod.benefit.limit,
                                    ),
                                ]"
                                :disabled="!hasAdminAccess"
                            >
                            </v-text-field>
                        </v-flex>
                        <v-spacer />
                    </v-layout>
                    <v-layout justify-center>
                        <v-spacer></v-spacer>
                        <v-flex xs6>
                            <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                            <v-textarea
                                class="ghd-control-text ghd-control-border"
                                @input="
                                    onSetAnalysisMethodProperty(
                                        'description',
                                        $event,
                                    )
                                "
                                no-resize
                                outline
                                rows="6"
                                :value="analysisMethod.description"
                            >
                            </v-textarea>
                        </v-flex>
                        <v-flex xs6>
                            <v-layout style="height=12px;padding-bottom:0px;">
                                <v-flex xs12 style="height=12px;padding-bottom:0px">
                                    <v-subheader class="ghd-control-label ghd-md-gray">                             
                                        Criteria</v-subheader>
                                </v-flex>
                                <v-flex xs1 style="height=12px;padding-bottom:0px;padding-top:0px;">
                                    <v-btn
                                        style="padding-right:20px !important;"
                                        @click="
                                            onShowCriterionEditorDialog
                                        "
                                        class="edit-icon ghd-control-label"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                            <v-textarea
                                class="ghd-control-text ghd-control-border"
                                style="padding-bottom: 0px; height: 90px;"
                                no-resize
                                outline
                                readonly
                                :rows="criteriaRows()"
                                v-model="
                                    analysisMethod.criterionLibrary
                                        .mergedCriteriaExpression
                                "
                            >
                            </v-textarea>
                            <v-checkbox
                              style="padding-top: 0px; margin-top: 4px;"
                              class="ghd-checkbox ghd-md-gray"
                              label="Criteria is intentionally empty (MUST check to Save)" 
                              v-model="criteriaIsIntentionallyEmpty"
                              v-show="criteriaIsEmpty()"
                            >
                            </v-checkbox>
                        </v-flex>
                        <v-spacer></v-spacer>
                    </v-layout>
                </v-layout>
            </v-flex>

            <v-flex xs6>
                <v-layout justify-center row>
                    <v-btn
                        @click="onDiscardChanges"
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
                        depressed
                        >Cancel</v-btn
                    >
                    <v-btn
                        @click="onUpsertAnalysisMethod"
                        :disabled="criteriaIsInvalid() || !valid"
                        depressed
                        class="ghd-blue-bg ghd-white ghd-button-text ghd-button"
                        >Save</v-btn
                    >
                </v-layout>
            </v-flex>

            <GeneralCriterionEditorDialog
                :dialogData="criterionEditorDialogData"
                @submit="onCriterionEditorDialogSubmit"
            />
        </v-layout>
    </v-form>
</template>

<script lang="ts">
import Vue from 'vue';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import { clone, equals, isNil } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import {
    AnalysisMethod,
    emptyAnalysisMethod,
    OptimizationStrategy,
    SpendingStrategy,
} from '@/shared/models/iAM/analysis-method';
import { SelectItem } from '@/shared/models/vue/select-item';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';

@Component({
    components: { GeneralCriterionEditorDialog },
})
export default class EditAnalysisMethod extends Vue {
    @State(state => state.analysisMethodModule.analysisMethod)
    stateAnalysisMethod: AnalysisMethod;
    @State(state => state.attributeModule.numericAttributes)
    stateNumericAttributes: Attribute[];
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;

    @Action('getAnalysisMethod') getAnalysisMethodAction: any;
    @Action('upsertAnalysisMethod') upsertAnalysisMethodAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

    selectedScenarioId: string = getBlankGuid();
    analysisMethod: AnalysisMethod = clone(emptyAnalysisMethod);
    optimizationStrategy: SelectItem[] = [
        { text: 'Benefit', value: OptimizationStrategy.Benefit },
        {
            text: 'Benefit-to-Cost Ratio',
            value: OptimizationStrategy.BenefitToCostRatio,
        },
        { text: 'Remaining Life', value: OptimizationStrategy.RemainingLife },
        {
            text: 'Remaining life-to-Cost Ratio',
            value: OptimizationStrategy.RemainingLifeToCostRatio,
        },
    ];
    spendingStrategy: SelectItem[] = [
        { text: 'No Spending', value: SpendingStrategy.NoSpending },
        {
            text: 'Unlimited Spending',
            value: SpendingStrategy.UnlimitedSpending,
        },
        {
            text: 'Until Target & Deficient Condition Goals Met',
            value: SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet,
        },
        {
            text: 'Until Target Condition Goals Met',
            value: SpendingStrategy.UntilTargetConditionGoalsMet,
        },
        {
            text: 'Until Deficient Condition Goals Met',
            value: SpendingStrategy.UntilDeficientConditionGoalsMet,
        },
        { text: 'As Budget Permits', value: SpendingStrategy.AsBudgetPermits },
    ];
    benefitAttributes: SelectItem[] = [];
    weightingAttributes: SelectItem[] = [{ text: '', value: '' }];
    simulationName: string;
    networkName: string = '';
    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    rules: InputValidationRules = rules;
    valid: boolean = true;
    criteriaIsIntentionallyEmpty: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectedScenarioId = to.query.scenarioId;
            vm.simulationName = to.query.simulationName;
            vm.networkName = to.query.networkName;
            if (vm.selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                vm.addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            }

            // get the selected scenario's analysisMethod data
            vm.getAnalysisMethodAction({ scenarioId: vm.selectedScenarioId });
        });
    }

    mounted() {
        if (hasValue(this.stateNumericAttributes)) {
            this.setBenefitAndWeightingAttributes();
        }
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateAnalysisMethod')
    onStateAnalysisChanged() {
        this.analysisMethod = {
            ...this.stateAnalysisMethod,
            benefit: {
                ...this.stateAnalysisMethod.benefit,
                id:
                    this.stateAnalysisMethod.benefit.id === getBlankGuid()
                        ? getNewGuid()
                        : this.stateAnalysisMethod.benefit.id,
            },
        };
    }

    @Watch('analysisMethod')
    onAnalysisChanged() {
        this.setHasUnsavedChangesAction({
            value:
                !equals(this.analysisMethod, emptyAnalysisMethod) &&
                !equals(this.analysisMethod, this.stateAnalysisMethod),
        });

        this.setBenefitAttributeIfEmpty();
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        if (hasValue(this.stateNumericAttributes)) {
            this.setBenefitAndWeightingAttributes();
            this.setBenefitAttributeIfEmpty();
        }
    }

    setBenefitAttributeIfEmpty() {
        if (
            !hasValue(this.analysisMethod.benefit.attribute) &&
            hasValue(this.benefitAttributes)
        ) {
            this.analysisMethod.benefit.attribute = this.benefitAttributes[0].value.toString();
        }
    }

    onSetAnalysisMethodProperty(property: string, value: any) {
        this.analysisMethod = setItemPropertyValue(
            property,
            value,
            this.analysisMethod,
        );
    }

    onSetBenefitProperty(property: string, value: any) {
        this.analysisMethod.benefit = setItemPropertyValue(
            property,
            value,
            this.analysisMethod.benefit,
        );
    }

    setBenefitAndWeightingAttributes() {
        const numericAttributeSelectItems: SelectItem[] = this.stateNumericAttributes.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }),
        );
        this.benefitAttributes = [...numericAttributeSelectItems];
        this.weightingAttributes = [
            this.weightingAttributes[0],
            ...numericAttributeSelectItems,
        ];
    }

    onShowCriterionEditorDialog() {
        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.analysisMethod.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onCriterionEditorDialogSubmit(criterionexpression: string) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionexpression)) {
            if(this.analysisMethod.criterionLibrary.id == getBlankGuid())
                this.analysisMethod.criterionLibrary.id = getNewGuid();
            this.analysisMethod = {
                ...this.analysisMethod,
                criterionLibrary: {...this.analysisMethod.criterionLibrary, mergedCriteriaExpression: criterionexpression} as CriterionLibrary,
            };
        }
    }

    criteriaIsEmpty()
    {
        return (isNil(this.analysisMethod.criterionLibrary) ||
                isNil(this.analysisMethod.criterionLibrary.mergedCriteriaExpression) ||
                this.analysisMethod.criterionLibrary.mergedCriteriaExpression == ""
                );
    }

    criteriaRows()
    {
        return this.criteriaIsEmpty() ? 4 : 6;
    }

    criteriaIsInvalid() {
        return this.criteriaIsEmpty() && !this.criteriaIsIntentionallyEmpty;
    }    

    onUpsertAnalysisMethod() {
        const form: any = this.$refs.form;

        if (form.validate()) {
            this.upsertAnalysisMethodAction({
                analysisMethod: this.analysisMethod,
                scenarioId: this.selectedScenarioId,
            });
        }
    }

    onDiscardChanges() {
        this.analysisMethod = clone(this.stateAnalysisMethod);
    }
}
</script>
