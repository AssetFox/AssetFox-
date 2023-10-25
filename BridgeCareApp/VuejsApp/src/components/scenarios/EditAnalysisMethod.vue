<template>
    <div>
        <v-row>
            <v-col id="EditAnalysisMethod-weightingParent-vflex" xs4>
        <v-subheader class="ghd-control-label ghd-md-gray">Weighting</v-subheader>
        <v-select
            id="EditAnalysisMethod-weighting-vselect"
            class="ghd-select ghd-control-border ghd-control-text"
            :items="weightingAttributes"
            variant="outlined"
            clearable
            item-title="text"
            item-value="value"
            v-model="analysisMethod.attribute"
            :disabled="!hasAdminAccess"
        >
        </v-select>
        </v-col>
        <v-col id="EditAnalysisMethod-optimizationStrategyParent-vflex" xs4>
        <v-subheader class="ghd-control-label ghd-md-gray">Optimization Strategy</v-subheader>
        <v-select 
            id="EditAnalysisMethod-optimizationStrategy-select"
            class="ghd-select ghd-control-border ghd-control-text"
            :items="optimizationStrategy"
            item-title="text"
            item-value="value"
            variant="outlined"
            v-model="analysisMethod.optimizationStrategy"
            :disabled="!hasAdminAccess"
        >
        </v-select>
        </v-col>    
        <v-col cols = "4" id="EditAnalysisMethod-spendingStrategyParent-vflex">
                            <v-subheader class="ghd-control-label ghd-md-gray">Spending Strategy</v-subheader>
                            <v-select
                                id="EditAnalysisMethod-spendingStrategy-select"
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="spendingStrategy"
                                variant="outlined"
                                item-title="text"
                                item-value="value"
                                v-model="analysisMethod.spendingStrategy"
                            >
                            </v-select>
                        </v-col>                      
                    </v-row>
                    <v-row>
                        <v-col cols = "4">
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Attribute</v-subheader>
                            <v-select
                                id="EditAnalysisMethod-benefitAttribute-select"
                                class="ghd-select ghd-control-text ghd-control-border"
                                :items="benefitAttributes"
                                item-title="text"
                                item-value="value"
                                variant="outlined"
                                v-model="analysisMethod.benefit.attribute"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-col>
                        <v-col cols = "4">
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Limit</v-subheader>
                            <v-text-field
                                id="EditAnalysisMethod-benefitLimit-textField"
                                style="margin:0px"
                                class="ghd-control-text ghd-control-border"
                                @update:model-value="onSetBenefitProperty('limit',$event)"
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
                        </v-col>
                        <v-col cols = "4" class="ghd-constant-header">
                            <v-switch style="margin-left:10px;margin-top:30px;"
                                id="EditAnalysisMethod-allowMultiBudgetFunding-switch"
                                class="ghd-checkbox"
                                label="Allow Multi Budget Funding"
                                :disabled="!hasAdminAccess"
                                v-model="analysisMethod.shouldUseExtraFundsAcrossBudgets"
                                @change='onSetAnalysisMethodProperty("shouldUseExtraFundsAcrossBudgets",$event)'/>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col cols = "6">
                            <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                            <v-textarea
                                id="EditAnalysisMethod-description-textArea"
                                class="ghd-control-text ghd-control-border"
                                @update:model-value="onSetAnalysisMethodProperty('description', $event)"
                                no-resize
                                variant="outlined"
                                rows="6"
                                :model-value="analysisMethod.description"
                            >
                            </v-textarea>
                        </v-col>
                        <v-col cols = "6">
                            <v-row style="height=12px;padding-bottom:0px;">
                                <v-col style="height=12px;padding-bottom:25px;padding-top:0px;">
                                    <v-btn
                                        id="EditAnalysisMethod-criteriaEditor-btn"
                                        style="padding-right:30px !important;"
                                        @click="
                                            onShowCriterionEditorDialog
                                        "
                                        class="edit-icon ghd-control-label"
                                        flat
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                    <v-subheader class="ghd-control-label ghd-md-gray">                             
                                        Criteria
                                    </v-subheader>

                                </v-col>
                            </v-row>
                            <v-textarea
                                id="EditAnalysisMethod-criteria-textArea"
                                class="ghd-control-text ghd-control-border"
                                style="padding-bottom: 0px; height: 100px;"
                                no-resize
                                variant="outlined"
                                readonly
                                :rows="criteriaRows()"
                                v-model="
                                    analysisMethod.criterionLibrary
                                        .mergedCriteriaExpression
                                "
                            >
                            </v-textarea>
                            <div style="padding-top: 20px;"></div>
                            <v-checkbox
                                id="EditAnalysisMethod-criteria-checkbox"
                                style="padding-top: 0px; margin-top: 4px;"
                                class="ghd-checkbox ghd-md-gray"
                                label="Criteria is intentionally empty (MUST check to Save)" 
                                v-model="criteriaIsIntentionallyEmpty"
                                v-show="criteriaIsEmpty()"
                            >
                            </v-checkbox>
                        </v-col>
                    </v-row>
                
                <v-row justify-center>
                    <v-btn
                        id="EditAnalysisMethod-cancel-btn"
                        @click="onDiscardChanges"
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
                        variant = "flat"
                        >Cancel</v-btn
                    >
                    <v-btn
                        id="EditAnalysisMethod-save-btn"
                        @click="onUpsertAnalysisMethod"
                        variant = "flat"
                        class="ghd-blue-bg ghd-white ghd-button-text ghd-button"
                        >Save</v-btn
                    >
                </v-row>
    </div>
                        <!-- <v-col id="EditAnalysisMethod-weightingParent-vflex" xs4>
                            <v-subheader class="ghd-control-label ghd-md-gray">Weighting</v-subheader>
                            <v-select
                                id="EditAnalysisMethod-weighting-vselect"
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="weightingAttributes"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'attribute',
                                        $event,
                                    )
                                "
                                variant="outlined"
                                clearable
                                :model-value="analysisMethod.attribute"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-col>
                        <v-col id="EditAnalysisMethod-optimizationStrategyParent-vflex" xs4>
                            <v-subheader class="ghd-control-label ghd-md-gray">Optimization Strategy</v-subheader>
                            <v-select 
                                id="EditAnalysisMethod-optimizationStrategy-select"
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="optimizationStrategy"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'optimizationStrategy',
                                        $event,
                                    )
                                "
                                variant="outlined"
                                :model-value="analysisMethod.optimizationStrategy"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-col>
                        <v-col cols = "4" id="EditAnalysisMethod-spendingStrategyParent-vflex">
                            <v-subheader class="ghd-control-label ghd-md-gray">Spending Strategy</v-subheader>
                            <v-select
                                id="EditAnalysisMethod-spendingStrategy-select"
                                class="ghd-select ghd-control-border ghd-control-text"
                                :items="spendingStrategy"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetAnalysisMethodProperty(
                                        'spendingStrategy',
                                        $event,
                                    )
                                "
                                variant="outlined"
                                :model-value="analysisMethod.spendingStrategy"
                            >
                            </v-select>
                        </v-col>                        
                    <v-row justify-center>
                        <v-spacer />
                         <v-col cols = "4">
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Attribute</v-subheader>
                            <v-select
                                id="EditAnalysisMethod-benefitAttribute-select"
                                class="ghd-select ghd-control-text ghd-control-border"
                                :items="benefitAttributes"
                                append-icon=$vuetify.icons.ghd-down
                                @change="
                                    onSetBenefitProperty('attribute', $event)
                                "
                                variant="outlined"
                                :model-value="analysisMethod.benefit.attribute"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-col>
                        <v-col cols = "4">
                            <v-subheader class="ghd-control-label ghd-md-gray">Benefit Limit</v-subheader>
                            <v-text-field
                                id="EditAnalysisMethod-benefitLimit-textField"
                                style="margin:0px"
                                class="ghd-control-text ghd-control-border"
                                @update:model-value="onSetBenefitProperty('limit',$event)"
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
                        </v-col>
                        <v-col cols = "4" class="ghd-constant-header">
                            <v-switch style="margin-left:10px;margin-top:30px;"
                                id="EditAnalysisMethod-allowMultiBudgetFunding-switch"
                                class="ghd-checkbox"
                                label="Allow Multi Budget Funding"
                                :disabled="!hasAdminAccess"
                                v-model="analysisMethod.shouldUseExtraFundsAcrossBudgets"
                                @change='onSetAnalysisMethodProperty("shouldUseExtraFundsAcrossBudgets",$event)'/>
                        </v-col>
                        <v-spacer />
                    </v-row>
                    <v-row justify-center>
                        <v-spacer></v-spacer>
                        <v-col cols = "6">
                            <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                            <v-textarea
                                id="EditAnalysisMethod-description-textArea"
                                class="ghd-control-text ghd-control-border"
                                @update:model-value="onSetAnalysisMethodProperty('description', $event)"
                                no-resize
                                outline
                                rows="6"
                                :model-value="analysisMethod.description"
                            >
                            </v-textarea>
                        </v-col>
                        <v-col cols = "6">
                            <v-row style="height=12px;padding-bottom:0px;">
                                <v-col cols = "12" style="height=12px;padding-bottom:0px">
                                    <v-subheader class="ghd-control-label ghd-md-gray">                             
                                        Criteria</v-subheader>
                                </v-col>
                                <v-col cols = "1" style="height=12px;padding-bottom:0px;padding-top:0px;">
                                    <v-btn
                                        id="EditAnalysisMethod-criteriaEditor-btn"
                                        style="padding-right:20px !important;"
                                        @click="
                                            onShowCriterionEditorDialog
                                        "
                                        class="edit-icon ghd-control-label"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-col>
                            </v-row>
                            <v-textarea
                                id="EditAnalysisMethod-criteria-textArea"
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
                                id="EditAnalysisMethod-criteria-checkbox"
                                style="padding-top: 0px; margin-top: 4px;"
                                class="ghd-checkbox ghd-md-gray"
                                label="Criteria is intentionally empty (MUST check to Save)" 
                                v-model="criteriaIsIntentionallyEmpty"
                                v-show="criteriaIsEmpty()"
                            >
                            </v-checkbox>
                        </v-col>
                        <v-spacer></v-spacer>
                    </v-row>
            <v-col cols = "6">
                <v-row justify-center row>
                    <v-btn
                        id="EditAnalysisMethod-cancel-btn"
                        @click="onDiscardChanges"
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
                        variant = "flat"
                        >Cancel</v-btn
                    >
                    <v-btn
                        id="EditAnalysisMethod-save-btn"
                        @click="onUpsertAnalysisMethod"
                        variant = "flat"
                        class="ghd-blue-bg ghd-white ghd-button-text ghd-button"
                        >Save</v-btn
                    >
                </v-row>
            </v-col> -->

            <GeneralCriterionEditorDialog
                :dialogData="criterionEditorDialogData"
                @submit="onCriterionEditorDialogSubmit"
            />
</template>

<script lang="ts" setup>
import Vue, { Ref, ref, shallowReactive, shallowRef, watch, onMounted, onBeforeUnmount, inject } from 'vue'; 
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
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 

    let store = useStore(); 
    const router = useRouter(); 
    // ToDo - verify if below is correct. Its used in onUpsertAnalysisMethod()
    const $refs = inject('$refs') as any

    let stateAnalysisMethod: AnalysisMethod = shallowRef(store.state.analysisMethodModule.analysisMethod) ;
    const stateNumericAttributes: Attribute[] = shallowReactive(store.state.attributeModule.numericAttributes) ;

    let hasAdminAccess: boolean = (store.state.authenticationModule.hasAdminAccess) ; 

    async function getAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('getAnalysisMethod')} 
    async function upsertAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('upsertAnalysisMethod')} 

    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification')}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges')} 

    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch('getCurrentUserOrSharedScenario')}
    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario')} 

    let selectedScenarioId: string = getBlankGuid();
    const analysisMethod = ref<AnalysisMethod>(clone(emptyAnalysisMethod));
    let optimizationStrategy: SelectItem[] = [
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
    let spendingStrategy: SelectItem[] = [
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
    // let benefitAttributes: SelectItem[] = [];
    const benefitAttributes = ref<SelectItem[]>([]);
    // let weightingAttributes: SelectItem[] = [{ text: '', value: '' }];
    const weightingAttributes = ref<SelectItem[]>([{ text: '', value: '' }]);
    // let simulationName: string;
    const simulationName = ref<string>();
    // let networkName: string = '';
    const networkName = ref<string>();
    // let criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
    //     emptyGeneralCriterionEditorDialogData,
    // );
    const criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));

    // let rules: InputValidationRules = validationRules;
    const rules = ref<InputValidationRules>(validationRules);
    // let valid: boolean = true;
    const valid = ref<boolean>(true);
    // let criteriaIsIntentionallyEmpty: boolean = false;
    const criteriaIsIntentionallyEmpty = ref<boolean>(false);

    //beforeRouteEnter(to: any, from: any, next: any) {
       //next((vm: any) => {
    created(); 
    function created() { 
            selectedScenarioId = router.currentRoute.value.query.scenarioId as string;
            simulationName.value = router.currentRoute.value.query.simulationName as string;
            networkName.value = router.currentRoute.value.query.networkName as string;
            if (selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                router.push('/Scenarios/');
            }

            // get the selected scenario's analysisMethod data
            getAnalysisMethodAction({ scenarioId: selectedScenarioId }).then(() => {                       
                getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId}).then(() => {         
                    selectScenarioAction({ scenarioId: selectedScenarioId });        
                });
            });
        //});
    }

    onMounted(() => {
        if (hasValue(stateNumericAttributes)) {
            setBenefitAndWeightingAttributes();
        }
    });

    onBeforeUnmount(() => beforeDestroy );
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(stateAnalysisMethod, () => onStateAnalysisChanged)
    function onStateAnalysisChanged() {
        analysisMethod.value = {
            ...stateAnalysisMethod,
            benefit: {
                ...stateAnalysisMethod.benefit,
                id:
                    stateAnalysisMethod.benefit.id === getBlankGuid()
                        ? getNewGuid()
                        : stateAnalysisMethod.benefit.id,
            },
        };
    }

    watch(analysisMethod, () => onAnalysisChanged)
    function onAnalysisChanged() {
        setHasUnsavedChangesAction({
            value:
                !equals(analysisMethod.value, stateAnalysisMethod),
        });

        setBenefitAttributeIfEmpty();        
    }

    watch(stateNumericAttributes, () => onStateNumericAttributesChanged)
    function onStateNumericAttributesChanged() {
        if (hasValue(stateNumericAttributes)) {
            setBenefitAndWeightingAttributes();
            setBenefitAttributeIfEmpty();
        }
    }

    function setBenefitAttributeIfEmpty() {
        if (
            !hasValue(analysisMethod.value.benefit.attribute) &&
            hasValue(benefitAttributes)
        ) {
            analysisMethod.value.benefit.attribute = benefitAttributes.value[0].value.toString();
        }
    }

    function onSetAnalysisMethodProperty(property: string, value: any) {
        analysisMethod.value = setItemPropertyValue(
            property,
            value,
            analysisMethod,
        );
    }

    function onSetBenefitProperty(property: string, value: any) {
        analysisMethod.value.benefit = setItemPropertyValue(
            property,
            value,
            analysisMethod.value.benefit,
        );
    }

    function setBenefitAndWeightingAttributes() {
        const numericAttributeSelectItems: SelectItem[] = stateNumericAttributes.map(
            (attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }),
        );
        benefitAttributes.value = [...numericAttributeSelectItems];
        weightingAttributes.value = [
            weightingAttributes.value[0],
            ...numericAttributeSelectItems,
        ];
    }

    function onShowCriterionEditorDialog() {
        criterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: analysisMethod.value.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onCriterionEditorDialogSubmit(criterionexpression: string) {
        criterionEditorDialogData.value = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (!isNil(criterionexpression)) {
            if(analysisMethod.value.criterionLibrary.id == getBlankGuid())
                analysisMethod.value.criterionLibrary.id = getNewGuid();
            analysisMethod.value = {
                ...analysisMethod.value,
                criterionLibrary: {...analysisMethod.value.criterionLibrary, mergedCriteriaExpression: criterionexpression} as CriterionLibrary,
            };
        }
    }

    function criteriaIsEmpty()
    {
        return (isNil(analysisMethod.value.criterionLibrary) ||
                isNil(analysisMethod.value.criterionLibrary.mergedCriteriaExpression) ||
                analysisMethod.value.criterionLibrary.mergedCriteriaExpression == ""
                );
    }

    function criteriaRows()
    {
        return criteriaIsEmpty() ? 4 : 6;
    }

    function criteriaIsInvalid() {
        return criteriaIsEmpty() && !criteriaIsIntentionallyEmpty;
    }    

    async function onUpsertAnalysisMethod() {
        const form: any = $refs.form;

        await(form.validate(), () =>{
            if(form.result.valid){
                upsertAnalysisMethodAction({
                analysisMethod: analysisMethod,
                scenarioId: selectedScenarioId,
            });
            }
        })
         
    }

    function onDiscardChanges() {
        analysisMethod.value = clone(stateAnalysisMethod);
    }

</script>
