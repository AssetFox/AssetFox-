<template>
  <v-dialog max-width="45%" v-model="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify="space-between">
          <div class="dialog-header"><h5>Add New Target Condition Goal</h5></div>
          <v-btn @click="onSubmit(false)" flat>
            <i class="fas fa-times fa-2x"></i>
          </v-btn>
        </v-row>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row column>
          <v-col>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>
          <v-text-field 
            id="CreateTargetConditionGoalDialog-name-vtextfield"
            variant="outlined" 
            v-model="newTargetConditionGoal.name"
            class="ghd-control-text ghd-control-border"
            density="compact"
            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Select Attribute</v-subheader>
          <v-select id="CreateTargetConditionGoalDialog-attribute-vselect"
                    :items="numericAttributeNames"
                    density="compact"
                    class="ghd-select ghd-control-text ghd-text-field ghd-text-field-border"
                    variant="outlined" v-model="newTargetConditionGoal.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Year</v-subheader>
          <v-text-field 
            id="CreateTargetConditionGoalDialog-year-vtextfield" 
            :mask="'####'" 
            class="ghd-control-text ghd-control-border" 
            variant="outlined"
            density="compact" 
            v-model.number="newTargetConditionGoal.year"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Target</v-subheader>
          <v-text-field 
            id="CreateTargetConditionGoalDialog-target-vtextfield" 
            variant="outlined"
            density="compact"
            :mask="'##########'" 
            v-model.number="newTargetConditionGoal.target"
            class="ghd-control-text ghd-control-border"
            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions style="margin-left: 10px; padding-left: 10px;">
        <v-row justify="center" class="ghd-dialog-padding-bottom-buttons">
          <v-btn 
            id="CreateTargetConditionGoalDialog-cancel-vbtn" 
            @click="onSubmit(false)" 
            class="ghd-white-bg ghd-blue" 
            variant = "outlined"
            rounded="0">
            Cancel
          </v-btn>
          <v-btn 
            id="CreateTargetConditionGoalDialog-save-vbtn" 
            :disabled="disableSubmitButton()" 
            @click="onSubmit(true)" 
            class="ghd-white-bg ghd-blue" 
            variant = "outlined"
            rounded="0">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch, toRefs } from 'vue';

import {emptyTargetConditionGoal, TargetConditionGoal} from '@/shared/models/iAM/target-condition-goal';
import {Attribute} from '@/shared/models/iAM/attribute';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules as validationRules,} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {isEqual} from '@/shared/utils/has-unsaved-changes-helper';
import { useStore } from 'vuex';

  const props = defineProps<{
          showDialog: boolean,
          currentNumberOfTargetConditionGoals:number
        }>();
        
  const { showDialog } = toRefs(props);

  let store = useStore();
  const emit = defineEmits(['submit'])
  const stateNumericAttributes = computed<Attribute[]>(() => store.state.attributeModule.numericAttributes);

  let newTargetConditionGoal = ref<TargetConditionGoal>({...emptyTargetConditionGoal, id: getNewGuid()});
  let numericAttributeNames = ref<string[]>([]);
  const rules = ref<InputValidationRules>(validationRules);

  onMounted(() => mounted);
  function mounted() {
    setNumericAttributeNames();
  }

  watch(stateNumericAttributes,()=> {
    setNumericAttributeNames();
  });

  watch(numericAttributeNames,()=> {
    setNewTargetConditionGoalDefaultValues();
  });

  watch(() => props.showDialog,()=> {
    setNewTargetConditionGoalDefaultValues();
  });

  watch(() => props.currentNumberOfTargetConditionGoals,()=> onCurrentNumberOfDeficientConditionGoalsChanged)
  function onCurrentNumberOfDeficientConditionGoalsChanged() {
    setNewTargetConditionGoalDefaultValues();
  }

  function setNewTargetConditionGoalDefaultValues() {
    if (showDialog) {
      newTargetConditionGoal.value = {
        ...newTargetConditionGoal.value,
        attribute: hasValue(numericAttributeNames.value) ? numericAttributeNames.value[0] : '',
        name: `Unnamed Target Condition Goal ${props.currentNumberOfTargetConditionGoals + 1}`,
        target: props.currentNumberOfTargetConditionGoals > 0 ? props.currentNumberOfTargetConditionGoals + 1 : 1
      };
    }
  }

  function setNumericAttributeNames() {
    if (hasValue(stateNumericAttributes) && !isEqual(numericAttributeNames.value, stateNumericAttributes)) {
      numericAttributeNames.value = getPropertyValues('name', stateNumericAttributes.value);
    }
  }

  function disableSubmitButton() {
    return !(rules.value['generalRules'].valueIsNotEmpty(newTargetConditionGoal.value.attribute) === true &&
        rules.value['generalRules'].valueIsNotEmpty(newTargetConditionGoal.value.target) === true &&
        rules.value['generalRules'].valueIsNotEmpty(newTargetConditionGoal.value.name) === true);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newTargetConditionGoal.value);
    } else {
      emit('submit', null);
    }

    newTargetConditionGoal.value = {...emptyTargetConditionGoal, id: getNewGuid()};
  }

</script>
