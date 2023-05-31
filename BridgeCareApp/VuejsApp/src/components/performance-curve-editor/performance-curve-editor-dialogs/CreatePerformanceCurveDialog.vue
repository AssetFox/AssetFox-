<template>
  <v-layout>
    <v-dialog v-model="showDialog"
              max-width="434px"
              persistent>
      <v-card  height="411px" class="ghd-dialog">
        <v-card-title class="ghd-dialog">
          <v-layout justify-left>
            <h3 class="ghd-dialog">Add New Deterioration Equation</h3>
          </v-layout>
        </v-card-title>
        <v-card-text class="ghd-dialog">
          <v-layout column>
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
              outline
            >
              <template v-slot:selection="{ item }">
                <span class="ghd-control-text">{{ item.text }}</span>
              </template>
              <template v-slot:item="{ item }">
                <v-list-item class="ghd-control-text" v-on="on" v-bind="attrs">
                  <v-list-item-content>
                    <v-list-item-title>
                      <v-row no-gutters align="center">
                      <span>{{ item.text }}</span>
                      </v-row>
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </template>
            </v-select>                      
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-center row>
            <v-btn
              id="CreatePerformanceCurveDialog-cancel-button"
              class="ghd-white-bg ghd-blue ghd-button-text"
              depressed
              @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn
              id="CreatePerformanceCurveDialog-save-button"
              :disabled="newPerformanceCurve.name === '' || newPerformanceCurve.attribute === ''"
              class="ghd-blue-bg ghd-white ghd-button-text"
              @click="onSubmit(true)"
              depressed>
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
import {emptyPerformanceCurve, PerformanceCurve} from '@/shared/models/iAM/performance';
import {SelectItem} from '@/shared/models/vue/select-item';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreatePerformanceCurveDialog extends Vue {
  @Prop() showDialog: boolean;

  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];

  attributeSelectItems: SelectItem[] = [];
  newPerformanceCurve: PerformanceCurve = {...emptyPerformanceCurve, id: getNewGuid()};
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
      this.$emit('submit', this.newPerformanceCurve);
    } else {
      this.$emit('submit', null);
    }

    this.newPerformanceCurve = {...emptyPerformanceCurve, id: getNewGuid()};
  }
}
</script>
