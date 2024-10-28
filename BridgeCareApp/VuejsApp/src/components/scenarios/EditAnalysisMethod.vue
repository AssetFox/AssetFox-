Line 164 Delete,

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
            menu-icon=custom:GhdDownSvg
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
            menu-icon=custom:GhdDownSvg
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
                                menu-icon=custom:GhdDownSvg
                                v-model="analysisMethod.spendingStrategy"
                                @update:model-value="onSetAnalysisMethodProperty('spendingStrategy',$event)">
                            </v-select>
                        </v-col>                      
                    </v-row>
                    <v-row>
                        <v-col cols = "3">
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
                                menu-icon=custom:GhdDownSvg
                                @update:model-value="onSetBenefitProperty('attribute', $event)"
                                :disabled="!hasAdminAccess"
                            >
                            </v-select>
                        </v-col>
                        <v-col cols = "3">
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
                    </v-row>
                    <v-row>
                        <v-col cols = "3">
                            <v-switch 
                            id="EditAnalysisMethod-allowMultiBudgetFunding-switch"
                            class="ghd-checkbox"
                            color="#2A578D"
                            label="Allow Multi Budget Funding"
                            :disabled="!hasAdminAccess"
                            v-model="analysisMethod.shouldUseExtraFundsAcrossBudgets"
                            @update:model-value='onSetAnalysisMethodProperty("shouldUseExtraFundsAcrossBudgets",$event)'/>
                            
                        </v-col>
                        <v-col cols = "3">
                            <v-switch 
                            id="EditAnalysisMethod-allowMultipleTreatments-switch"
                            class="ghd-checkbox"
                            color="#2A578D"
                            label="Allow Multiple Treatments"
                            :disabled="!hasAdminAccess"
                            v-model="analysisMethod.shouldAllowMultipleTreatments"
                            @update:model-value='onSetAnalysisMethodProperty("shouldAllowMultipleTreatments",$event)'/>
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
                                    class="edit-icon ghd-control-label ghd-blue"
                                    icon
                                    flat
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/> 
                                </v-btn>
                                </v-row>
                                <v-textarea
                                    id="EditAnalysisMethod-criteria-textArea"
                                    class="ghd-control-text ghd-control-border"
                                    style="padding-bottom: 0px; height: 250px;"
                                    no-resize
                                    variant="outlined"
                                    readonly
                                    :rows="criteriaRows()"
                                    v-model="analysisMethod.criterionLibrary.mergedCriteriaExpression"
                                >
                                </v-textarea>
                            </v-col>
                        </v-row>
                    </v-row>
                <v-row justify="center">
                    <v-row justify="center">
                        <CancelButton 
                            :disabled="!hasUnsavedChanges"
                            @cancel="onDiscardChanges"
                        />
                        <SaveButton 
                            :disabled="!valid || !hasUnsavedChanges"
                            @save="onUpsertAnalysisMethod"
                        />
                    </v-row>
                </v-row>
    </v-card>
            <GeneralCriterionEditorDialog
                :dialogData="criterionEditorDialogData"
                @submit="onCriterionEditorDialogSubmit"
            />
            <Alert
            :dialogData="ConfirmEmptyCriteria"
            @submit="onConfirmEmptyCriteriaAlertSubmit"
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
import {emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 
import { getUrl } from '@/shared/utils/get-url';
import ConfirmDialog from 'primevue/confirmdialog';
import EditSvg from '@/shared/icons/EditSvg.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import mitt, { Emitter, EventType } from 'mitt';
import AnalysisMethodService from '@/services/analysis-method.service';

    let store = useStore(); 
    const router = useRouter(); 
    // ToDo - verify if below is correct. Its used in onUpsertAnalysisMethod()
    const $refs = inject('$refs') as any

    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>

    const stateAnalysisMethod = computed<AnalysisMethod>(() => store.state.analysisMethodModule.analysisMethod) ;
    const stateNumericAttributes = computed<Attribute[]>( ()=> store.state.attributeModule.numericAttributes) ;
    const stateSimulationAnalysisSetting = computed<boolean>( ()=> store.state.analysisMethodModule.simulationAnalysisSetting);

    const hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess) ; 

    async function getAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('getAnalysisMethod', payload)} 
    async function upsertAnalysisMethodAction(payload?: any): Promise<any>{await store.dispatch('upsertAnalysisMethod', payload)} 
    async function getSimulationAnalysisSettingAction(payload?: any): Promise<any>{await store.dispatch('getSimulationAnalysisSetting', payload)}

    function addErrorNotificationAction(payload?: any){ store.dispatch('addErrorNotification', payload)}
    function setHasUnsavedChangesAction(payload?: any){ store.dispatch('setHasUnsavedChanges', payload)} 

    async function getCurrentUserOrSharedScenarioAction(payload?: any): Promise<any>{await store.dispatch('getCurrentUserOrSharedScenario', payload)}
    function selectScenarioAction(payload?: any){ store.dispatch('selectScenario', payload)} 

    const selectedScenarioId = ref<string>(getBlankGuid());
    const analysisMethod = ref<AnalysisMethod>(clone(emptyAnalysisMethod));
    let benefit = computed<Benefit>(() => analysisMethod.value.benefit)//
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
    let hasUnsavedChanges = ref<boolean>(false);
    let ConfirmEmptyCriteria = ref(clone(emptyAlertData));

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

// Get the selected scenario's analysis method data
getAnalysisMethodAction({ scenarioId: selectedScenarioId.value })
    .then(() => {
        // Get simulation analysis setting
        return getSimulationAnalysisSettingAction({ scenarioId: selectedScenarioId.value });
    })
    .then(() => {
        // Get current user or shared scenario info
        return getCurrentUserOrSharedScenarioAction({ simulationId: selectedScenarioId.value });
    })
    .then(() => {
        // Select scenario
        selectScenarioAction({ scenarioId: selectedScenarioId.value });
    })
    .catch(error => {
        console.error('Error in action chain:', error);
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
            },
        };
    });

    // watch(benefit, onAnalysisChanged)
    watch(analysisMethod, onAnalysisChanged)
     async function onAnalysisChanged() {
        hasUnsavedChanges.value = !equals(analysisMethod.value, stateAnalysisMethod.value);

        setHasUnsavedChangesAction({
            value:
                hasUnsavedChanges.value
        });

         setBenefitAttributeIfEmpty();
    }

    watch(stateSimulationAnalysisSetting, (newVal) => {
        if(newVal === false)
        {
            hasUnsavedChanges.value = true;
        }
    });

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
        onAnalysisChanged();
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
        return criteriaIsEmpty() && !criteriaIsIntentionallyEmpty.value;
    }    

    async function onUpsertAnalysisMethod() {
        if(criteriaIsEmpty())
        {
            onConfirmEmptyCriteriaAlert();
        }
        else
        {
            if(analysisMethod.value.benefit.id === getBlankGuid())
            analysisMethod.value.benefit.id = getNewGuid();
        upsertAnalysisMethodAction({
                analysisMethod: analysisMethod.value,
                scenarioId: selectedScenarioId.value,
            });
            $emitter.emit('AnalysisMethodUpdated');              
        }
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

    function onConfirmEmptyCriteriaAlert() {
        ConfirmEmptyCriteria.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Warning: Criteria field is blank. A blank Criteria field will default to your user assigned assets.',
        };
    }

    function onConfirmEmptyCriteriaAlertSubmit(submit: boolean) {
        ConfirmEmptyCriteria.value = clone(emptyAlertData);

        if (submit) {
            if(analysisMethod.value.benefit.id === getBlankGuid())
            analysisMethod.value.benefit.id = getNewGuid();
        upsertAnalysisMethodAction({
                analysisMethod: analysisMethod.value,
                scenarioId: selectedScenarioId.value,
            });
            $emitter.emit('AnalysisMethodUpdated');              
        } 
    }


    function onDiscardChanges() {
        analysisMethod.value = clone(stateAnalysisMethod.value);
    }



</script>
