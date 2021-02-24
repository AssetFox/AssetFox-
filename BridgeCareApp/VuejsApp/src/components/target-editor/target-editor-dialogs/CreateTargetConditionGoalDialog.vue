<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Target Condition Goal</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field label="Name" outline v-model="newTargetConditionGoal.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>

          <v-select :items="numericAttributeNames" label="Select Attribute"
                    outline v-model="newTargetConditionGoal.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>

          <v-text-field :mask="'####'" label="Year" outline v-model.number="newTargetConditionGoal.year"/>

          <v-text-field label="Target" outline :mask="'##########'" v-model.number="newTargetConditionGoal.target"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="disableSubmitButton()" @click="onSubmit(true)" class="ara-blue-bg white--text">
            Save
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
import {Component, Prop, Watch} from 'vue-property-decorator';
import {State} from 'vuex-class';
import {emptyTargetConditionGoal, TargetConditionGoal} from '@/shared/models/iAM/target-condition-goal';
import {Attribute} from '@/shared/models/iAM/attribute';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateTargetConditionGoalDialog extends Vue {
  @Prop() showDialog: boolean;
  @Prop() currentNumberOfTargetConditionGoals: number;

  @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

  newTargetConditionGoal: TargetConditionGoal = {...emptyTargetConditionGoal, id: getNewGuid()};
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
    this.setNewTargetConditionGoalDefaultValues();
  }

  @Watch('numericAttributeNames')
  onNumericAttributeNamesChanged() {
    this.setNumericAttributeNames();
  }

  @Watch('currentNumberOfTargetConditionGoals')
  onCurrentNumberOfDeficientConditionGoalsChanged() {
    if (this.showDialog) {
      this.setNewTargetConditionGoalDefaultValues();
    }
  }

  setNewTargetConditionGoalDefaultValues() {
    this.newTargetConditionGoal = {
      ...this.newTargetConditionGoal,
      attribute: hasValue(this.numericAttributeNames) ? this.numericAttributeNames[0] : '',
      name: `Unnamed Target Condition Goal ${this.currentNumberOfTargetConditionGoals + 1}`,
      target: this.currentNumberOfTargetConditionGoals > 0 ? this.currentNumberOfTargetConditionGoals + 1 : 1
    };
  }

  setNumericAttributeNames() {
    if (hasValue(this.stateNumericAttributes)) {
      this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);

      if (this.showDialog) {
        this.setNewTargetConditionGoalDefaultValues();
      }
    }
  }

  disableSubmitButton() {
    return !(this.rules['generalRules'].valueIsNotEmpty(this.newTargetConditionGoal.attribute) === true &&
        this.rules['generalRules'].valueIsNotEmpty(this.newTargetConditionGoal.target) === true &&
        this.rules['generalRules'].valueIsNotEmpty(this.newTargetConditionGoal.name) === true);
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newTargetConditionGoal);
    } else {
      this.$emit('submit', null);
    }

    this.newTargetConditionGoal = {...emptyTargetConditionGoal, id: getNewGuid()};
  }
}
</script>
