<template>
  <v-row>
    <v-dialog max-width="450px" persistent v-model="showDialog">
      <v-card>
        <v-card-title class="ghd-dialog-box-padding-top">
         <v-row justify="space-between" align="center">
            <div class="ghd-control-dialog-header">Add New Deficient Condition Goal</div>
            <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
              X
            </v-btn>
          </v-row>
        </v-card-title>
        <v-card-text class="ghd-dialog-box-padding-center">
          <v-row>
            <v-col>
              <v-subheader class="ghd-md-gray ghd-control-label">Name</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-name-vtextfield"
                            variant="outlined" 
                            v-model="newDeficientConditionGoal.name"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                            class="ghd-text-field-border ghd-text-field"></v-text-field>
              <v-subheader class="ghd-md-gray ghd-control-label">Select Attribute</v-subheader>
              <v-select id="CreateDeficientConditionGoalDialog-attribute-vselect"
                        :items="numericAttributeNames"
                        variant="outlined"
                        v-model="newDeficientConditionGoal.attribute" :rules="[rules['generalRules'].valueIsNotEmpty]"
                        class="ghd-select ghd-text-field ghd-text-field-border">
              </v-select>
              <v-subheader class="ghd-md-gray ghd-control-label">Deficient Limit</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-limit-vtextfield"
                            variant="outlined"
                            v-model.number="newDeficientConditionGoal.deficientLimit" :mask="'##########'"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                            class="ghd-text-field-border ghd-text-field"></v-text-field>
              <v-subheader class="ghd-md-gray ghd-control-label">Allowed Deficient Percentage</v-subheader>
              <v-text-field id="CreateDeficientConditionGoalDialog-percentage-vtextfield"
                            variant="outlined"
                            v-model.number="newDeficientConditionGoal.allowedDeficientPercentage"
                            :mask="'###'"
                            :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(newDeficientConditionGoal.allowedDeficientPercentage, [0, 100])]"
                            class="ghd-text-field-border ghd-text-field">
              </v-text-field>
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-row justify="center">
            <v-btn 
              id="CreateDeficientConditionGoalDialog-cancel-vbtn" 
              @click="onSubmit(false)" 
              variant = "flat" 
              class='ghd-blue ghd-button-text ghd-button'>
              Cancel
            </v-btn>
            <v-btn 
              id="CreateDeficientConditionGoalDialog-save-vbtn" 
              :disabled="disableSubmitBtn()" 
              @click="onSubmit(true)" 
              variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button'>
              Save
            </v-btn>           
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script setup lang="ts">
import { onMounted, computed, watch, ref, toRefs } from 'vue';
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
  const { showDialog, currentNumberOfDeficientConditionGoals } = toRefs(props);
  const emit = defineEmits(['submit'])

  const stateNumericAttributes = computed<Attribute[]>(() =>store.state.attributeModule.numericAttributes);

  const newDeficientConditionGoal = ref<DeficientConditionGoal>(clone({...emptyDeficientConditionGoal, id: getNewGuid()}));
  const numericAttributeNames = ref<string[]>([]);
  let rules: InputValidationRules = validationRules;

  onMounted(() => {
    setNumericAttributeNames();
  });

  watch(stateNumericAttributes, () => {
    setNumericAttributeNames();
  });

  watch(showDialog, () => {
    if (showDialog) {
      setNewDeficientConditionGoalDefaultValues();
    }
  });

  watch(currentNumberOfDeficientConditionGoals, () => {
    if (showDialog) {
      setNewDeficientConditionGoalDefaultValues();
    }
  });

  function setNewDeficientConditionGoalDefaultValues() {
    newDeficientConditionGoal.value = {
      ...newDeficientConditionGoal.value,
      attribute: hasValue(numericAttributeNames.value) ? numericAttributeNames.value[0] : '',
      name: `Unnamed Deficient Condition Goal ${props.currentNumberOfDeficientConditionGoals + 1}`,
      deficientLimit: currentNumberOfDeficientConditionGoals.value > 0 ? currentNumberOfDeficientConditionGoals.value + 1 : 1
    };
  }

  function setNumericAttributeNames() {
    if (hasValue(stateNumericAttributes)) {
      numericAttributeNames.value = getPropertyValues('name', stateNumericAttributes.value);

      if (showDialog) {
        setNewDeficientConditionGoalDefaultValues();
      }
    }
  }

  function disableSubmitBtn() {
    return !(rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.value.name) === true &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.value.attribute) &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.value.deficientLimit) &&
        rules['generalRules'].valueIsNotEmpty(newDeficientConditionGoal.value.allowedDeficientPercentage) &&
        rules['generalRules'].valueIsWithinRange(newDeficientConditionGoal.value.allowedDeficientPercentage, [0, 100]));
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newDeficientConditionGoal.value);
    } else {
      emit('submit', null);
    }

    newDeficientConditionGoal.value = {...emptyDeficientConditionGoal, id: getNewGuid()};
  }
</script>
