<template>
  <v-row>
    <v-dialog v-model="showDialogComputed" max-width="250px" persistent>
      <v-card>
        <v-card-title>
          <v-row justify-center>
            <h3>New Equation</h3>
          </v-row>
        </v-card-title>
        <v-card-text>
          <v-row column>
            <v-text-field v-model="newCalculatedAttribute.name"
                          :rules="[rules['generalRules'].valueIsNotEmpty]"
                          label="Name"
                          variant="outlined"/>
            <v-select v-model="newCalculatedAttribute.attribute"
                      :items="attributeSelectItems"
                      menu-icon=custom:GhdDownSvg
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      label="Select Attribute"
                      variant="outlined" />
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-row justify-space-between row>
            <v-btn :disabled="newCalculatedAttribute.name === '' || newCalculatedAttribute.attribute === ''"
                   class="assetFox-blue-bg text-white"
                   @click="onSubmit(true)">
              Save
            </v-btn>
            <v-btn class="assetFox-orange-bg text-white"
                   @click="onSubmit(false)">
              Cancel
            </v-btn>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
import {SelectItem} from '@/shared/models/vue/select-item';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules as validationRules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { CalculatedAttribute, emptyCalculatedAttribute } from '@/shared/models/iAM/calculated-attribute';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

  let showDialog : boolean = false;
  let store = useStore();
  let stateNumericAttributes = ref<Attribute[]>(store.state.attributeModule.numericAttributes);
  let attributeSelectItems: SelectItem[] = [];
  let newCalculatedAttribute: CalculatedAttribute[] = [];
  let rules: InputValidationRules = validationRules;
  const emit = defineEmits(['submit'])
  const props = defineProps<{
    showDialog: boolean
  }>()
  let showDialogComputed = computed(() => props.showDialog);
  watch(stateNumericAttributes,() => onStateNumericAttributesChanged)
  function onStateNumericAttributesChanged() {
    if (hasValue(stateNumericAttributes)) {
      setAttributeSelectItems();
    }
  }

  onMounted(() => mounted());
    function mounted() {
      if (hasValue(stateNumericAttributes)) {
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
      emit('submit', newCalculatedAttribute);
    } else {
      emit('submit', null);
    }

    newCalculatedAttribute = [] as CalculatedAttribute[];
  }

</script>