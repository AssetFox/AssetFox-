<template>
  <v-dialog max-width="450px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title class="ghd-dialog-padding-top-title">
        <v-row justify-start>
          <div class="dialog-header"><h5>Add New Target Condition Goal</h5></div>
        </v-row>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
        </v-btn>
      </v-card-title>
      <v-card-text class="ghd-dialog-text-field-padding">
        <v-row column>
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
                    variant="outlined" v-model="newTargetConditionGoal.attribute"
                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Year</v-subheader>
          <v-text-field id="CreateTargetConditionGoalDialog-year-vtextfield" :mask="'####'" class="ghd-control-text ghd-control-border" outline v-model.number="newTargetConditionGoal.year"/>
          <v-subheader class="ghd-control-label ghd-md-gray">Target</v-subheader>
          <v-text-field id="CreateTargetConditionGoalDialog-target-vtextfield" outline :mask="'##########'" v-model.number="newTargetConditionGoal.target"
                        class="ghd-control-text ghd-control-border"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"/>
        </v-row>
      </v-card-text>
      <v-card-actions class="py-0">
        <v-row justify-center row class="ghd-dialog-padding-bottom-buttons">
          <v-btn id="CreateTargetConditionGoalDialog-cancel-vbtn" @click="onSubmit(false)" class="ghd-white-bg ghd-blue" variant = "outlined">
            Cancel
          </v-btn>
          <v-btn id="CreateTargetConditionGoalDialog-save-vbtn" :disabled="disableSubmitButton()" @click="onSubmit(true)" class="ghd-white-bg ghd-blue" variant = "outlined">
            Save
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';

import {emptyTargetConditionGoal, TargetConditionGoal} from '@/shared/models/iAM/target-condition-goal';
import {Attribute} from '@/shared/models/iAM/attribute';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules as validationRules,} from '@/shared/utils/input-validation-rules';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {isEqual} from '@/shared/utils/has-unsaved-changes-helper';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import Dialog from 'primevue/dialog';

  const props = defineProps<{
          showDialog: boolean,
          currentNumberOfTargetConditionGoals:number
        }>();

  let store = useStore();
  const emit = defineEmits(['submit'])
  let stateNumericAttributes = ref<Attribute[]>(store.state.attributeModule.numericAttributes);

  let newTargetConditionGoal: TargetConditionGoal = {...emptyTargetConditionGoal, id: getNewGuid()};
  let numericAttributeNames = ref<string[]>([]);
  let rules: InputValidationRules = validationRules;

  onMounted(() => mounted);
  function mounted() {
    setNumericAttributeNames();
  }

  watch(stateNumericAttributes,()=> onStateNumericAttributesChanged)
  function onStateNumericAttributesChanged() {
    setNumericAttributeNames();
  }

  watch(numericAttributeNames,()=> onNumericAttributeNamesChanged)
  function onNumericAttributeNamesChanged() {
    setNewTargetConditionGoalDefaultValues();
  }

  watch(() => props.showDialog,()=> onShowDialogChanged)
  function onShowDialogChanged() {
    setNewTargetConditionGoalDefaultValues();
  }

  watch(() => props.currentNumberOfTargetConditionGoals,()=> onCurrentNumberOfDeficientConditionGoalsChanged)
  function onCurrentNumberOfDeficientConditionGoalsChanged() {
    setNewTargetConditionGoalDefaultValues();
  }

  function setNewTargetConditionGoalDefaultValues() {
    if (props.showDialog) {
      newTargetConditionGoal = {
        ...newTargetConditionGoal,
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
    return !(rules['generalRules'].valueIsNotEmpty(newTargetConditionGoal.attribute) === true &&
        rules['generalRules'].valueIsNotEmpty(newTargetConditionGoal.target) === true &&
        rules['generalRules'].valueIsNotEmpty(newTargetConditionGoal.name) === true);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newTargetConditionGoal);
    } else {
      emit('submit', null);
    }

    newTargetConditionGoal = {...emptyTargetConditionGoal, id: getNewGuid()};
  }

</script>
