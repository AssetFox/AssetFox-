<template>
  <v-row>
    <v-dialog v-bind:show="showDialog" max-width="434px" persistent>
      <v-card  height="411px" class="ghd-dialog">
        <v-card-title class="ghd-dialog">
          <v-row justify-left>
            <h3 class="ghd-dialog">Add New Deterioration Equation</h3>
          </v-row>
        </v-card-title>
        <v-card-text class="ghd-dialog">
          <v-row column>
            <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>            
            <v-text-field
              id="CreatePerformanceCurveDialog-name-text"
              class="ghd-control-text ghd-control-border"
              v-model="newPerformanceCurve.name"
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              outline/>
            <v-subheader class="ghd-control-label ghd-md-gray">Select Attribute</v-subheader>            
            <v-select
              id="CreatePerformanceCurveDialog-attribute-select"
              class="ghd-select ghd-control-text ghd-control-border"
              v-model="newPerformanceCurve.attribute"
              :items="attributeSelectItems"
              append-icon=$vuetify.icons.ghd-down
              :rules="[rules['generalRules'].valueIsNotEmpty]"
              variant="outlined"
            >
              <template v-slot:selection="{ item }">
                <span class="ghd-control-text">{{ item.raw.text }}</span>
              </template>
              <template v-slot:item="{ item }">
                <v-list-item class="ghd-control-text" v-bind="props">
                    <v-list-item-title>
                      <v-row no-gutters align="center">
                      <span>{{ item.raw.text }}</span>
                      </v-row>
                    </v-list-item-title>
                </v-list-item>
              </template>
            </v-select>                      
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-row justify-center row>
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
import Vue from 'vue';
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
import Dialog from 'primevue/dialog';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    showDialog: boolean
    }>()

    let stateNumericAttributes = ref<Attribute[]>(store.state.attributeModule.numericAttributes);
    let attributeSelectItems: SelectItem[] = [];
    let newPerformanceCurve: PerformanceCurve = {...emptyPerformanceCurve, id: getNewGuid()};
    let rules: InputValidationRules = validationRules;

  onMounted(()=>mounted())
  function mounted() {
    if (hasValue(stateNumericAttributes.value)) {
      setAttributeSelectItems();
    }
  }

  watch(stateNumericAttributes,()=>onStateNumericAttributesChanged)
  function onStateNumericAttributesChanged() {
    if (hasValue(stateNumericAttributes.value)) {
      setAttributeSelectItems();
    }
  }

  function setAttributeSelectItems() {
    attributeSelectItems = stateNumericAttributes.value.map((attribute: Attribute) => ({
      text: attribute.name,
      value: attribute.name
    }));
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newPerformanceCurve);
    } else {
      emit('submit', null);
    }

    newPerformanceCurve = {...emptyPerformanceCurve, id: getNewGuid()};
  }
</script>
