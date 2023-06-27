<template>
  <v-dialog max-width="450px" persistent v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-layout justify-start>
          <div class="dialog-header"><h5>Add New Target Condition Goal</h5></div>
        </v-layout>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-layout column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field id="CreateTargetConditionGoalDialog-name-vtextfield"
                        outline v-model="newTargetConditionGoal.name"
                        class="ghd-control-text ghd-control-border"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Select Attribute</v-subheader>
          <v-select id="CreateTargetConditionGoalDialog-attribute-vselect"
                    :items="numericAttributeNames"
                    append-icon=$vuetify.icons.ghd-down
                    class="ghd-select ghd-control-text ghd-text-field ghd-text-field-border"
                    outline v-model="newTargetConditionGoal.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Year</v-subheader>
          <v-text-field id="CreateTargetConditionGoalDialog-year-vtextfield" :mask="'####'" class="ghd-control-text ghd-control-border" outline v-model.number="newTargetConditionGoal.year"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Target</v-subheader>
          <v-text-field id="CreateTargetConditionGoalDialog-target-vtextfield" outline :mask="'##########'" v-model.number="newTargetConditionGoal.target"
                        class="ghd-control-text ghd-control-border"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
        </v-layout>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-layout justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn id="CreateTargetConditionGoalDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue" flat>
            Cancel
          </v-btn>
          <v-btn id="CreateTargetConditionGoalDialog-save-vbtn" :disabled="disableSubmitButton()" @click="onSubmit(true)" class="ghd-white-bg ghd-blue" outline>
            Save
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
import {isEqual} from '@/shared/utils/has-unsaved-changes-helper';

@Component
export default class CreateTargetConditionGoalDialog extends Vue {
  @Prop() showDialog: boolean;
  @Prop() currentNumberOfTargetConditionGoals: number;

  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];

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

  @Watch('numericAttributeNames')
  onNumericAttributeNamesChanged() {
    this.setNewTargetConditionGoalDefaultValues();
  }

  @Watch('showDialog')
  onShowDialogChanged() {
    this.setNewTargetConditionGoalDefaultValues();
  }

  @Watch('currentNumberOfTargetConditionGoals')
  onCurrentNumberOfDeficientConditionGoalsChanged() {
    this.setNewTargetConditionGoalDefaultValues();
  }

  setNewTargetConditionGoalDefaultValues() {
    if (this.showDialog) {
      this.newTargetConditionGoal = {
        ...this.newTargetConditionGoal,
        attribute: hasValue(this.numericAttributeNames) ? this.numericAttributeNames[0] : '',
        name: `Unnamed Target Condition Goal ${this.currentNumberOfTargetConditionGoals + 1}`,
        target: this.currentNumberOfTargetConditionGoals > 0 ? this.currentNumberOfTargetConditionGoals + 1 : 1
      };
    }
  }

  setNumericAttributeNames() {
    if (hasValue(this.stateNumericAttributes) && !isEqual(this.numericAttributeNames, this.stateNumericAttributes)) {
      this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
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
