<template>
  <v-row>
    <v-dialog v-model="showDialogComputed" max-width="434px" persistent>
      <v-card  height="411px" class="ghd-dialog">
        <v-card-title class="ghd-dialog">
          <v-row justify="start">
            <h3 class="ghd-dialog">Add New Deterioration Equation</h3>
          </v-row>
        </v-card-title>
        <v-card-text >
          <v-row>
            <v-col>
            <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>            
            <v-text-field
              id="CreatePerformanceCurveDialog-name-text"
              class="ghd-control-text ghd-control-border"
              v-model="newPerformanceCurve.name"
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              variant="outlined"/>
            <v-subheader class="ghd-control-label ghd-md-gray">Select Attribute</v-subheader>            
            <v-select
              id="CreatePerformanceCurveDialog-attribute-select"
              menu-icon=custom:GhdDownSvg
              class="ghd-select ghd-control-text ghd-control-border"
              v-model="newPerformanceCurve.attribute"
              :items="attributeSelectItems"
              append-icon=ghd-down
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              variant="outlined"
              item-title="text"
              item-value="value"
            >
            </v-select>   
            </v-col>                   
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-row justify="center" >
            <v-btn
              id="CreatePerformanceCurveDialog-cancel-button"
              class="ghd-white-bg ghd-blue ghd-button-text"
              variant = "flat"
              @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn
              id="CreatePerformanceCurveDialog-save-button"
              :disabled="newPerformanceCurve.name === '' || newPerformanceCurve.attribute === ''"
              class="ghd-blue-bg ghd-white ghd-button-text"
              @click="onSubmit(true)"
              variant = "flat">
              Save
            </v-btn>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
import {emptyPerformanceCurve, PerformanceCurve} from '@/shared/models/iAM/performance';
import {SelectItem} from '@/shared/models/vue/select-item';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { on } from 'events';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    showDialog: boolean
    }>()
    let showDialogComputed = computed(() => props.showDialog);
    let stateNumericAttributes = computed<Attribute[]>(() => store.state.attributeModule.numericAttributes);
    let attributeSelectItems = ref<SelectItem[]>([]);
    let newPerformanceCurve = ref<PerformanceCurve>({...emptyPerformanceCurve, id: getNewGuid()});
    let rules: InputValidationRules = validationRules;

  onMounted(()=>mounted())
  function mounted() {
    if (hasValue(stateNumericAttributes.value)) {
      setAttributeSelectItems();
    }
  }

  watch(stateNumericAttributes,()=>onStateNumericAttributesChanged())
  function onStateNumericAttributesChanged() {
    if (hasValue(stateNumericAttributes.value)) {
      setAttributeSelectItems();
    }
  }

  function setAttributeSelectItems() {
    attributeSelectItems.value = stateNumericAttributes.value.map((attribute: Attribute) => ({
      text: attribute.name,
      value: attribute.name
    }));
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newPerformanceCurve.value);
    } else {
      emit('submit', null);
    }

    newPerformanceCurve.value = {...emptyPerformanceCurve, id: getNewGuid()};
  }
</script>
