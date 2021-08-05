<template>
  <v-dialog v-model="dialogData.showDialog"
            max-width="450px"
            persistent>
    <v-card>
      <v-card-title>
        <v-layout justify-center>
          <h3>New Performance Curve Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text>
        <v-layout column>
          <v-text-field v-model="newPerformanceCurveLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        label="Name"
                        outline/>
          <v-textarea v-model="newPerformanceCurveLibrary.description"
                      label="Description"
                      no-resize
                      outline
                      rows="3"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newPerformanceCurveLibrary.name === ''"
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
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {emptyPerformanceCurveLibrary, PerformanceCurve, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {CreatePerformanceCurveLibraryDialogData} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

@Component
export default class CreatePerformanceCurveLibraryDialog extends Vue {
  @Prop() dialogData: CreatePerformanceCurveLibraryDialogData;

  newPerformanceCurveLibrary: PerformanceCurveLibrary = {...emptyPerformanceCurveLibrary, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  @Watch('dialogData')
  onDialogDataChanged() {
    this.newPerformanceCurveLibrary = {
      ...this.newPerformanceCurveLibrary,
      performanceCurves: this.dialogData.performanceCurves.map((performanceCurve: PerformanceCurve) => {
        performanceCurve.id = getNewGuid();
        if (performanceCurve.equation.id !== getBlankGuid()) {
          performanceCurve.equation.id = getNewGuid();
        }
        return performanceCurve;
      }),
      owner: getUserName()
    };
  }

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newPerformanceCurveLibrary);
    } else {
      this.$emit('submit', null);
    }

    this.newPerformanceCurveLibrary = {...emptyPerformanceCurveLibrary, id: getNewGuid()};
  }
}
</script>
