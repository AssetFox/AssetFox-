<template>
    <div class="text-center">
      <div
        style="cursor:pointer;"
        color="primary"
      >
        <slot></slot>
  
        <v-dialog
          v-model="dialog"
          activator="parent"
          max-width="290"
        >
          <v-card>
            <v-card-text>
              <v-col>
                <slot style="cursor:pointer;" name="input"></slot>          
                <v-row>
                  <v-col align="center">
                    <v-btn
                      variant="outlined"
                      class="pa-2 ma-2 ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
                      @click="onCancel"
                    >
                      Cancel
                    </v-btn>
                    <v-btn
                      variant="outlined"
                      class="pa-2 ma-2 ghd-blue ghd-button-text ghd-outline-button-padding ghd-button"
                      :disabled="hasValidationErrors"
                      @click="onSave"
                    >
                      Save
                    </v-btn>
                  </v-col>
                </v-row>
              </v-col>
            </v-card-text>
          </v-card>
        </v-dialog>
      </div>
    </div>
  </template>
<script setup lang="ts">
import { clone } from 'ramda';
import { computed, reactive, ref, watch } from 'vue';
import { InputValidationRules, rules as validationRules } from '@/shared/utils/input-validation-rules';

let rules: InputValidationRules = validationRules;
const emit = defineEmits(['save', 'open', 'update:returnValue']);
let dialog = ref(false);
let prevVal: any;
const scenarioNameErrors = ref<string[]>([]);
const props = defineProps<{
  returnValue: any;
  initialName: any;
}>();

watch(dialog, (newVal) => {
  if (newVal === true) {
    emit('open');
    prevVal = clone(props.initialName);
  }
});

const hasValidationErrors = computed(() => {
  // Validate that there are no special characters in the name
  scenarioNameErrors.value = [];

  const specialCharError = rules.generalRules.valueContainsNoSpecialCharacters(props.returnValue);
  if (specialCharError !== true) {
    scenarioNameErrors.value.push(specialCharError as string);
  }

  return scenarioNameErrors.value.length > 0;
});

function onSave() {
  emit('save');
  dialog.value = false;
}

function onCancel() {
    dialog.value = false;
}
</script>
