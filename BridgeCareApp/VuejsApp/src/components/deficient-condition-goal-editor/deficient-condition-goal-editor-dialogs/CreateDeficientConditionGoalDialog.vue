<template>
  <v-layout>
    <v-dialog max-width="450px" persistent v-model="showDialog">
      <v-card>
        <v-card-title>
          <v-layout justify-center>
            <h3>New Deficient Condition Goal</h3>
          </v-layout>
        </v-card-title>
        <v-card-text class="new-deficient-card-text">
          <v-layout column>
            <v-flex>
              <v-text-field label="Name" outline v-model="newDeficientConditionGoal.name"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
            </v-flex>
            <v-flex>
              <v-select :items="numericAttributeNames" label="Select Attribute"
                        outline
                        v-model="newDeficientConditionGoal.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]">
              </v-select>
            </v-flex>
            <v-flex>
              <v-text-field label="Deficient Limit" outline
                            v-model.number="newDeficientConditionGoal.deficientLimit" :mask="'##########'"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"></v-text-field>
            </v-flex>
            <v-flex>
              <v-text-field label="Allowed Deficient Percentage" outline
                            v-model.number="newDeficientConditionGoal.allowedDeficientPercentage"
                            :mask="'###'"
                            :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(newDeficientConditionGoal.allowedDeficientPercentage, [0, 100])]">
              </v-text-field>
            </v-flex>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="disableSubmitBtn()" @click="onSubmit(true)" class="ara-blue-bg white--text">
              Save
            </v-btn>
            <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">
              Cancel
            </v-btn>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {State} from 'vuex-class';
import {DeficientConditionGoal, emptyDeficientConditionGoal} from '@/shared/models/iAM/deficient-condition-goal';
import {clone} from 'ramda';
import {Attribute} from '@/shared/models/iAM/attribute';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateDeficientConditionGoalDialog extends Vue {
  @Prop() showDialog: boolean;
  @Prop() currentNumberOfDeficientConditionGoals: number;

  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];

  newDeficientConditionGoal: DeficientConditionGoal = clone({...emptyDeficientConditionGoal, id: getNewGuid()});
  numericAttributeNames: string[] = [];
  rules: InputValidationRules = rules;

  mounted() {
    this.setNumericAttributeNames();
  }

  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    this.setNumericAttributeNames();
  }

  @Watch('showDialog')
  onShowDialogChanged() {
    if (this.showDialog) {
      this.setNewDeficientConditionGoalDefaultValues();
    }
  }

  @Watch('currentNumberOfDeficientConditionGoals')
  onCurrentNumberOfDeficientConditionGoalsChanged() {
    if (this.showDialog) {
      this.setNewDeficientConditionGoalDefaultValues();
    }
  }

  setNewDeficientConditionGoalDefaultValues() {
    this.newDeficientConditionGoal = {
      ...this.newDeficientConditionGoal,
      attribute: hasValue(this.numericAttributeNames) ? this.numericAttributeNames[0] : '',
      name: `Unnamed Deficient Condition Goal ${this.currentNumberOfDeficientConditionGoals + 1}`,
      deficientLimit: this.currentNumberOfDeficientConditionGoals > 0 ? this.currentNumberOfDeficientConditionGoals + 1 : 1
    };
  }

  setNumericAttributeNames() {
    if (hasValue(this.stateNumericAttributes)) {
      this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);

      if (this.showDialog) {
        this.setNewDeficientConditionGoalDefaultValues();
      }
    }
  }

  disableSubmitBtn() {
    return !(this.rules['generalRules'].valueIsNotEmpty(this.newDeficientConditionGoal.name) === true &&
        this.rules['generalRules'].valueIsNotEmpty(this.newDeficientConditionGoal.attribute) &&
        this.rules['generalRules'].valueIsNotEmpty(this.newDeficientConditionGoal.deficientLimit) &&
        this.rules['generalRules'].valueIsNotEmpty(this.newDeficientConditionGoal.allowedDeficientPercentage) &&
        this.rules['generalRules'].valueIsWithinRange(this.newDeficientConditionGoal.allowedDeficientPercentage, [0, 100]));
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newDeficientConditionGoal);
    } else {
      this.$emit('submit', null);
    }

    this.newDeficientConditionGoal = {...emptyDeficientConditionGoal, id: getNewGuid()};
  }
}
</script>
