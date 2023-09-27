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
                        variant="outlined"
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

<script setup lang="ts">
import Vue, { onMounted, shallowRef, watch } from 'vue';
import {DeficientConditionGoal, emptyDeficientConditionGoal} from '@/shared/models/iAM/deficient-condition-goal';
import {clone} from 'ramda';
import {Attribute} from '@/shared/models/iAM/attribute';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { useStore } from 'vuex';

  let store = useStore();

  const props = defineProps<{
    currentNumberOfDeficientConditionGoals: number,
    showDialog: boolean
  }>()
  const emit = defineEmits(['submit'])

  let stateNumericAttributes = shallowRef<Attribute[]>(store.state.attributeModule.numericAttributes);

  let  newDeficientConditionGoal: DeficientConditionGoal = clone({...emptyDeficientConditionGoal, id: getNewGuid()});
  let numericAttributeNames: string[] = [];
  let rules: InputValidationRules = validationRules;

  onMounted(() => mounted); 
  function mounted() {
    setNumericAttributeNames();
  }

  watch(stateNumericAttributes, () => onStateNumericAttributesChanged)
  function onStateNumericAttributesChanged() {
    setNumericAttributeNames();
  }

  watch(() => props.showDialog, () => onShowDialogChanged)
  function onShowDialogChanged() {
    if (props.showDialog) {
      setNewDeficientConditionGoalDefaultValues();
    }
  }

  watch(() => props.currentNumberOfDeficientConditionGoals, () => onCurrentNumberOfDeficientConditionGoalsChanged)
  function onCurrentNumberOfDeficientConditionGoalsChanged() {
    if (props.showDialog) {
      setNewDeficientConditionGoalDefaultValues();
    }
  }

  function setNewDeficientConditionGoalDefaultValues() {
    newDeficientConditionGoal = {
      ...newDeficientConditionGoal,
      attribute: hasValue(numericAttributeNames) ? numericAttributeNames[0] : '',
      name: `Unnamed Deficient Condition Goal ${props.currentNumberOfDeficientConditionGoals + 1}`,
      deficientLimit: props.currentNumberOfDeficientConditionGoals > 0 ? props.currentNumberOfDeficientConditionGoals + 1 : 1
    };
  }

  function setNumericAttributeNames() {
    if (hasValue(stateNumericAttributes)) {
      numericAttributeNames = getPropertyValues('name', stateNumericAttributes.value);

      if (props.showDialog) {
        setNewDeficientConditionGoalDefaultValues();
      }
    }
  }

  function disableSubmitBtn() {
    return !(rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.name) === true &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.attribute) &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.deficientLimit) &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.allowedDeficientPercentage) &&
        rules['generalRules'].valueIsWithinRange(newDeficientConditionGoal.allowedDeficientPercentage, [0, 100]));
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDeficientConditionGoal);
    } else {
      emit('submit', null);
    }

    newDeficientConditionGoal = {...emptyDeficientConditionGoal, id: getNewGuid()};
  }
</script>
