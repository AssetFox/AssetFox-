<template>
    <v-card height="800px" class="elevation-0 vcard-main-layout">
        <v-row>
            <v-col id="EditAnalysisMethod-weightingParent-vflex">
        <v-subheader class="ghd-control-label ghd-md-gray">Weighting</v-subheader>
        <v-select
            id="EditAnalysisMethod-weighting-vselect"
            class="ghd-select ghd-control-border ghd-control-text"
            :items="weightingAttributes"
            variant="outlined"
            clearable
            item-title="text"
            item-value="value"
            density="compact"
            v-model="analysisMethod.attribute"
            :disabled="!hasAdminAccess"
            @update:model-value="onSetAnalysisMethodProperty('attribute',$event)"
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
            density="compact"
            v-model="analysisMethod.optimizationStrategy"
            @update:model-value="onSetAnalysisMethodProperty('optimizationStrategy',$event)"
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
                                density="compact"
                                v-model="analysisMethod.spendingStrategy"
                                @update:model-value="onSetAnalysisMethodProperty('spendingStrategy',$event)"
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
                                density="compact"
                                v-model="benefit.attribute"
                                @update:model-value="onSetBenefitProperty('attribute', $event)"
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
                                variant="outlined"
                                density="compact"
                                type="number"
                                min="0"
                                :value.number="benefit.limit"
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
                                color="#2A578D"
                                label="Allow Multi Budget Funding"
                                :disabled="!hasAdminAccess"
                                v-model="analysisMethod.shouldUseExtraFundsAcrossBudgets"
                                @update:model-value='onSetAnalysisMethodProperty("shouldUseExtraFundsAcrossBudgets",$event)'/>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-row justify="space-between" style="padding-left: 10px;">
                            <v-col>
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
                            <v-col>
                                <v-row justify="space-between" style="padding: 10px;">
                                <v-subheader class="ghd-control-label ghd-md-gray">                             
                                    Criteria
                                </v-subheader>
                                <v-btn
                                    id="EditAnalysisMethod-criteriaEditor-btn"
                                    style="padding-right:30px; padding-bottom: 5px; margin-bottom: 5px; !important;"
                                    @click="
                                        onShowCriterionEditorDialog
                                    "
                                    class="edit-icon ghd-control-label"
                                    flat
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                                </v-btn>
                                </v-row>
                                <v-textarea
                                    id="EditAnalysisMethod-criteria-textArea"
                                    class="ghd-control-text ghd-control-border"
                                    style="padding-bottom: 0px; height: 100px;"
                                    no-resize
                                    variant="outlined"
                                    readonly
                                    :rows="criteriaRows()"
                                    v-model="analysisMethod.criterionLibrary.mergedCriteriaExpression"
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
                    </v-row>
                <v-row justify="center">
                    <v-btn
                        id="EditAnalysisMethod-cancel-btn"
                        @click="onDiscardChanges"
                        :disabled="!hasUnsavedChanges"
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-button"
                        variant = "flat"
                        >Cancel</v-btn
                    >
                    <v-btn
                        id="EditAnalysisMethod-save-btn"
                        :disabled="(criteriaIsInvalid() || !valid) || !hasUnsavedChanges || !criteriaIsIntentionallyEmpty"
                        @click="onUpsertAnalysisMethod"
                        variant = "flat"
                        class="ghd-blue-bg ghd-white ghd-button-text ghd-button"
                        >Save</v-btn
                    >
                </v-row>
    </v-card>
            <GeneralCriterionEditorDialog
                :dialogData="criterionEditorDialogData"
                @submit="onCriterionEditorDialogSubmit"
            />
            <ConfirmDialog></ConfirmDialog>
</template>

<script lang="ts" setup>
import Vue, { computed, ref, shallowReactive, shallowRef, watch, onMounted, onBeforeUnmount, inject } from 'vue'; 
import { clone, equals, isNil } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import {
    AnalysisMethod,
    Benefit,
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
import { getUrl } from '@/shared/utils/get-url';
import ConfirmDialog from 'primevue/confirmdialog';

    let store = useStore(); 
    const router = useRouter(); 
    // ToDo - verify if below is correct. Its used in onUpsertAnalysisMethod()
    const $refs = inject('$refs') as any

    const stateAnalysisMethod = computed<AnalysisMethod>(() => store.state.analysisMethodModule.analysisMethod) ;
    const stateNumericAttributes = computed<Attribute[]>( ()=> store.state.attributeModule.numericAttributes) ;

    const hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess) ; 

    async function getAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('getAnalysisMethod', payload)} 
    async function upsertAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('upsertAnalysisMethod', payload)} 

    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification', payload)}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any>{await store.dispatch('setHasUnsavedChanges', payload)} 

    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch('getCurrentUserOrSharedScenario', payload)}
    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario', payload)} 

    const selectedScenarioId = ref<string>(getBlankGuid());
    const analysisMethod = ref<AnalysisMethod>(clone(emptyAnalysisMethod));
    let benefit = computed<Benefit>(() => analysisMethod.value.benefit)
    const optimizationStrategy: SelectItem[] = [
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
    const spendingStrategy: SelectItem[] = [
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
    const benefitAttributes = ref<SelectItem[]>([]);
    const weightingAttributes = ref<SelectItem[]>([{ text: '', value: '' }]);
    const simulationName = ref<string>();
    const networkName = ref<string>();
    const criterionEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
    const rules = ref<InputValidationRules>(validationRules);
    const valid = ref<boolean>(true);
    const criteriaIsIntentionallyEmpty = ref<boolean>(false);
    let hasUnsavedChanges = ref<boolean>(false)

    //beforeRouteEnter(to: any, from: any, next: any) {
       //next((vm: any) => {
    created(); 
    function created() { 
    }

    onMounted(() => {
        selectedScenarioId.value = router.currentRoute.value.query.scenarioId as string;
            simulationName.value = router.currentRoute.value.query.simulationName as string;
            networkName.value = router.currentRoute.value.query.networkName as string;
            if (selectedScenarioId.value === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                router.push('/Scenarios/');
            }

            // get the selected scenario's analysisMethod data
            getAnalysisMethodAction({ scenarioId: selectedScenarioId.value }).then(() => {                       
                getCurrentUserOrSharedScenarioAction({simulationId: selectedScenarioId.value}).then(() => {         
                    selectScenarioAction({ scenarioId: selectedScenarioId.value });        
                });
            });

        if (hasValue(stateNumericAttributes.value)) {
            setBenefitAndWeightingAttributes();
        }
    });

    onBeforeUnmount(() => {
        setHasUnsavedChangesAction({ value: false });
    });

    watch(stateAnalysisMethod, () => {
        analysisMethod.value = {
            ...stateAnalysisMethod.value,
            benefit: {
                ...stateAnalysisMethod.value.benefit,
                id:
                    stateAnalysisMethod.value.benefit.id === getBlankGuid()
                        ? getNewGuid()
                        : stateAnalysisMethod.value.benefit.id,
            },
        };
    });

    // watch(benefit, onAnalysisChanged)
    watch(analysisMethod, onAnalysisChanged)
    function onAnalysisChanged() {
        hasUnsavedChanges.value = !equals(analysisMethod.value, stateAnalysisMethod.value)
        setHasUnsavedChangesAction({
            value:
                hasUnsavedChanges.value
        });

        setBenefitAttributeIfEmpty();        
    }

    watch(stateNumericAttributes, () => {
        if (hasValue(stateNumericAttributes.value)) {
            setBenefitAndWeightingAttributes();
            setBenefitAttributeIfEmpty();
        }
    });

    function setBenefitAttributeIfEmpty() {
        if (
            !hasValue(analysisMethod.value.benefit.attribute) &&
            hasValue(benefitAttributes.value)
        ) {
            analysisMethod.value.benefit.attribute = benefitAttributes.value[0].value.toString();
        }
    }

    function onSetAnalysisMethodProperty(property: string, value: any) {
        analysisMethod.value = setItemPropertyValue(
            property,
            value,
            analysisMethod.value,
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
        const numericAttributeSelectItems: SelectItem[] = stateNumericAttributes.value.map(
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

        upsertAnalysisMethodAction({
                analysisMethod: analysisMethod.value,
                scenarioId: selectedScenarioId.value,
            });

        // const form: any = $refs.form;

        // await(form.validate(), () =>{
        //     if(form.result.valid){
        //         upsertAnalysisMethodAction({
        //         analysisMethod: analysisMethod,
        //         scenarioId: selectedScenarioId,
        //     });
        //     }
        // })
         
    }

    function onDiscardChanges() {
        analysisMethod.value = clone(stateAnalysisMethod.value);
    }



</script>
