<template>
  <v-form ref="form"
          v-model="valid"
          lazy-validation>
    <v-layout column>
      <v-flex xs12>
        <v-layout column>
          <v-layout justify-center>
            <v-flex xs2>
              <v-select :items="weightingAttributes"
                        @change="onSetAnalysisMethodProperty('attribute', $event)" label="Weighting"
                        outline
                        clearable
                        :value="analysisMethod.attribute" :disabled="!isAdmin">
              </v-select>
            </v-flex>
            <v-flex xs2>
              <v-select :items="optimizationStrategy"
                        @change="onSetAnalysisMethodProperty('optimizationStrategy', $event)"
                        label="Optimization Strategy" outline
                        :value="analysisMethod.optimizationStrategy" :disabled="!isAdmin">
              </v-select>
            </v-flex>
            <v-flex xs2>
              <v-select :items="spendingStrategy" @change="onSetAnalysisMethodProperty('spendingStrategy', $event)"
                        label="Spending Strategy" outline
                        :value="analysisMethod.spendingStrategy">
              </v-select>
            </v-flex>
          </v-layout>
          <v-layout justify-center>
            <v-spacer/>
            <v-flex xs2>
              <v-select :items="benefitAttributes" @change="onSetBenefitProperty('attribute', $event)"
                        label="Benefit Attribute"
                        outline
                        :value="analysisMethod.benefit.attribute" :disabled="!isAdmin">
              </v-select>
            </v-flex>
            <v-flex xs2>
              <v-text-field @input="onSetBenefitProperty('limit', $event)" label="Benefit limit"
                            outline
                            type="number"
                            min=0
                            :value.number="analysisMethod.benefit.limit"
                            :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsNotNegative(analysisMethod.benefit.limit)]"
                            :disabled="!isAdmin">
              </v-text-field>
            </v-flex>
            <v-spacer/>
          </v-layout>
          <v-layout justify-center>
            <v-spacer></v-spacer>
            <v-flex xs6>
              <v-textarea @input="onSetAnalysisMethodProperty('description', $event)" label="Description" no-resize
                          outline rows="5"
                          :value="analysisMethod.description">
              </v-textarea>
            </v-flex>
            <v-spacer></v-spacer>
          </v-layout>
          <v-layout justify-center>
            <v-spacer></v-spacer>
            <v-flex xs6>
              <v-textarea label="Criteria" no-resize outline readonly rows="5"
                          v-model="analysisMethod.criterionLibrary.mergedCriteriaExpression">
                <template slot="append-outer">
                  <v-btn @click="onShowCriterionLibraryEditorDialog" class="edit-icon" icon>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </template>
              </v-textarea>
            </v-flex>
            <v-spacer></v-spacer>
          </v-layout>
        </v-layout>
      </v-flex>

      <v-flex xs12>
        <v-layout justify-end row>
          <v-btn @click="onUpsertAnalysisMethod" :disabled="!valid" class="ara-blue-bg white--text">Save</v-btn>
          <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text">Discard Changes</v-btn>
        </v-layout>
      </v-flex>

      <CriterionLibraryEditorDialog :dialogData="criterionLibraryEditorDialogData"
                                    @submit="onCriterionLibraryEditorDialogSubmit"/>
    </v-layout>
  </v-form>  
</template>

<script lang="ts">
import Vue from 'vue';
import {Watch} from 'vue-property-decorator';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {clone, equals, isNil} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {Attribute} from '@/shared/models/iAM/attribute';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import {
  AnalysisMethod,
  emptyAnalysisMethod,
  OptimizationStrategy,
  SpendingStrategy
} from '@/shared/models/iAM/analysis-method';
import {SelectItem} from '@/shared/models/vue/select-item';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';

@Component({
  components: {CriterionLibraryEditorDialog}
})
export default class EditAnalysisMethod extends Vue {
  @State(state => state.analysisMethodModule.analysisMethod) stateAnalysisMethod: AnalysisMethod;
  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];
  @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

  @Action('getAnalysisMethod') getAnalysisMethodAction: any;
  @Action('upsertAnalysisMethod') upsertAnalysisMethodAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
  
  selectedScenarioId: string = getBlankGuid();
  analysisMethod: AnalysisMethod = clone(emptyAnalysisMethod);
  optimizationStrategy: SelectItem[] = [
    {text: 'Benefit', value: OptimizationStrategy.Benefit},
    {text: 'Benefit-to-Cost Ratio', value: OptimizationStrategy.BenefitToCostRatio},
    {text: 'Remaining Life', value: OptimizationStrategy.RemainingLife},
    {text: 'Remaining life-to-Cost Ratio', value: OptimizationStrategy.RemainingLifeToCostRatio}
  ];
  spendingStrategy: SelectItem[] = [
    {text: 'No Spending', value: SpendingStrategy.NoSpending},
    {text: 'Unlimited Spending', value: SpendingStrategy.UnlimitedSpending},
    {
      text: 'Until Target & Deficient Condition Goals Met',
      value: SpendingStrategy.UntilTargetAndDeficientConditionGoalsMet
    },
    {text: 'Until Target Condition Goals Met', value: SpendingStrategy.UntilTargetConditionGoalsMet},
    {text: 'Until Deficient Condition Goals Met', value: SpendingStrategy.UntilDeficientConditionGoalsMet},
    {text: 'As Budget Permits', value: SpendingStrategy.AsBudgetPermits}
  ];
  benefitAttributes: SelectItem[] = [];
  weightingAttributes: SelectItem[] = [{text: '', value: ''}];
  simulationName: string;
  criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  rules: InputValidationRules = rules;
  valid: boolean = true;
  
  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      vm.selectedScenarioId = to.query.scenarioId;
      vm.simulationName = to.query.simulationName;
      
      if (vm.selectedScenarioId === getBlankGuid()) {
        // set 'no selected scenario' error message, then redirect user to Scenarios UI
        vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
        vm.$router.push('/Scenarios/');
      }

      // get the selected scenario's analysisMethod data
      vm.getAnalysisMethodAction({scenarioId: vm.selectedScenarioId});
    });
  }

  mounted() {
    if (hasValue(this.stateNumericAttributes)) {
      this.setBenefitAndWeightingAttributes();
    }
  }

  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }  

  @Watch('stateAnalysisMethod')
  onStateAnalysisChanged() {
    this.analysisMethod = {
      ...this.stateAnalysisMethod,
      benefit: {...this.stateAnalysisMethod.benefit,
        id: this.stateAnalysisMethod.benefit.id === getBlankGuid() ? getNewGuid() : this.stateAnalysisMethod.benefit.id}
    };
  }

  @Watch('analysisMethod')
  onAnalysisChanged() {
    this.setHasUnsavedChangesAction({
      value: !equals(this.analysisMethod, emptyAnalysisMethod) && !equals(this.analysisMethod, this.stateAnalysisMethod)
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
    if (!hasValue(this.analysisMethod.benefit.attribute) && hasValue(this.benefitAttributes)) {
      this.analysisMethod.benefit.attribute = this.benefitAttributes[0].value.toString();
    }
  }

  onSetAnalysisMethodProperty(property: string, value: any) {
    this.analysisMethod = setItemPropertyValue(property, value, this.analysisMethod);
  }

  onSetBenefitProperty(property: string, value: any) {
      this.analysisMethod.benefit = setItemPropertyValue(property, value, this.analysisMethod.benefit);
  }

  setBenefitAndWeightingAttributes() {
    const numericAttributeSelectItems: SelectItem[] = this.stateNumericAttributes.map((attribute: Attribute) => ({
      text: attribute.name,
      value: attribute.name
    }));
    this.benefitAttributes = [...numericAttributeSelectItems];
    this.weightingAttributes = [this.weightingAttributes[0], ...numericAttributeSelectItems];
  }

  onShowCriterionLibraryEditorDialog() {
    this.criterionLibraryEditorDialogData = {
      showDialog: true,
      libraryId: this.analysisMethod.criterionLibrary.id,
      isCallFromScenario: true,
        isCriterionForLibrary: false
    };
  }

  onCriterionLibraryEditorDialogSubmit(criterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary)) {
      this.analysisMethod = {...this.analysisMethod, criterionLibrary: criterionLibrary};
    }
  }

  onUpsertAnalysisMethod() {
    const form: any = this.$refs.form;

    if (form.validate()) {
      this.upsertAnalysisMethodAction({analysisMethod: this.analysisMethod, scenarioId: this.selectedScenarioId});
    }
  }

  onDiscardChanges() {
    this.analysisMethod = clone(this.stateAnalysisMethod);
  }
}
</script>
