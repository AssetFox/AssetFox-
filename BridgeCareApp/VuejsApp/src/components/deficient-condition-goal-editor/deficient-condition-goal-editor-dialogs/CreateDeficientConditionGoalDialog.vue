<template>
  <v-layout>
    <v-dialog max-width="450px" persistent v-model="showDialog">
      <v-card>
        <v-card-title class="ghd-dialog-box-padding-top">
         <v-layout justify-space-between align-center>
            <div class="ghd-control-dialog-header">Add New Deficient Condition Goal</div>
            <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
              X
            </v-btn>
          </v-layout>
        </v-card-title>
        <v-card-text class="ghd-dialog-box-padding-center">
          <v-layout column>
            <v-flex>
              <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-name-vtextfield"
                            outline v-model="newDeficientConditionGoal.name"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                            class="ghd-text-field-border ghd-text-field"></v-text-field>
            </v-flex>
            <v-flex>
              <v-subheader class="ghd-md-gray ghd-control-label">Select Attribute</v-subheader>
              <v-select id="CreateDeficientConditionGoalDialog-attribute-vselect"
                        :items="numericAttributeNames"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="newDeficientConditionGoal.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-select ghd-text-field ghd-text-field-border">
              </v-select>
            </v-flex>
            <v-flex>
              <v-subheader class="ghd-md-gray ghd-control-label">Deficient Limit</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-limit-vtextfield"
                            outline
                            v-model.number="newDeficientConditionGoal.deficientLimit" :mask="'##########'"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                            class="ghd-text-field-border ghd-text-field"></v-text-field>
            </v-flex>
            <v-flex>
              <v-subheader class="ghd-md-gray ghd-control-label">Allowed Deficient Percentage</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-percentage-vtextfield"
                            outline
                            v-model.number="newDeficientConditionGoal.allowedDeficientPercentage"
                            :mask="'###'"
                            :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(newDeficientConditionGoal.allowedDeficientPercentage, [0, 100])]"
                            class="ghd-text-field-border ghd-text-field">
              </v-text-field>
            </v-flex>
          </v-layout>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-layout justify-center row>
            <v-btn id="CreateDeficientConditionGoalDialog-cancel-vbtn" @click="onSubmit(false)" flat class='ghd-blue ghd-button-text ghd-button'>
              Cancel
            </v-btn>
            <v-btn id="CreateDeficientConditionGoalDialog-save-vbtn" :disabled="disableSubmitBtn()" @click="onSubmit(true)" 
              outline class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
              Save
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
