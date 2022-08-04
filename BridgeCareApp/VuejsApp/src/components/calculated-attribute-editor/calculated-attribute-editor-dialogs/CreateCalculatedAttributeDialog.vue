<template>
  <v-layout>
    <v-dialog v-model="showDialog"
              max-width="250px"
              persistent>
      <v-card>
        <v-card-title>
          <v-layout justify-center>
            <h3>New Equation</h3>
          </v-layout>
        </v-card-title>
        <v-card-text>
          <v-layout column>
            <v-text-field v-model="newCalculatedAttribute.name"
                          :rules="[rules['generalRules'].valueIsNotEmpty]"
                          label="Name"
                          outline/>
            <v-select v-model="newCalculatedAttribute.attribute"
                      :items="attributeSelectItems"
                      append-icon=$vuetify.icons.ghd-down
                      :rules="[rules['generalRules'].valueIsNotEmpty]"
                      label="Select Attribute"
                      outline/>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="newCalculatedAttribute.name === '' || newCalculatedAttribute.attribute === ''"
                   class="ara-blue-bg white--text"
                   @click="onSubmit(true)">
              Save
            </v-btn>
            <v-btn class="ara-orange-bg white--text"
                   @click="onSubmit(false)">
              Cancel
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
import {SelectItem} from '@/shared/models/vue/select-item';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import { CalculatedAttribute, emptyCalculatedAttribute } from '@/shared/models/iAM/calculated-attribute';

@Component
export default class CreateCalculatedAttributeDialog extends Vue {
@Prop() showDialog: boolean;

  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];

  attributeSelectItems: SelectItem[] = [];
  newCalculatedAttribute: CalculatedAttribute[] = [];
  rules: InputValidationRules = clone(rules);

  mounted() {
    if (hasValue(this.stateNumericAttributes)) {
      this.setAttributeSelectItems();
    }
  }

  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    if (hasValue(this.stateNumericAttributes)) {
      this.setAttributeSelectItems();
    }
  }

  setAttributeSelectItems() {
    this.attributeSelectItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
      text: attribute.name,
      value: attribute.name
    }));
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newCalculatedAttribute);
    } else {
      this.$emit('submit', null);
    }

    this.newCalculatedAttribute = [] as CalculatedAttribute[];
  }
}
</script>