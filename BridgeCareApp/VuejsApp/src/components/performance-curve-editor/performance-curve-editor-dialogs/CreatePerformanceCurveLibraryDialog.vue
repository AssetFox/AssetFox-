<template>
  <v-dialog v-model="dialogData.showDialog"
            max-width="444px"
            persistent>
    <v-card height="437px" class="ghd-dialog">
      <v-card-title class="ghd-dialog">
        <v-layout justify-left>
          <h3 class="ghd-dialog">Create New<br/>Deterioration Model Library</h3>
        </v-layout>
      </v-card-title>
      <v-card-text class="ghd-dialog">
        <v-layout column>
          <v-subheader class="ghd-control-label ghd-md-gray">Name</v-subheader>             
          <v-text-field class="ghd-control-text ghd-control-border"
                        v-model="newPerformanceCurveLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                        outline/>
          <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>             
          <v-textarea class="ghd-control-text ghd-control-border"
                      v-model="newPerformanceCurveLibrary.description"
                      no-resize
                      outline
                      rows="3"/>
        </v-layout>
      </v-card-text>
      <v-card-actions>
          <v-layout justify-center row>
            <v-btn outline
                   class="ghd-white-bg ghd-blue ghd-button-text"
                   depressed
                   @click="onSubmit(false)">
              Cancel
            </v-btn>
            <v-btn :disabled="newPerformanceCurveLibrary.name === ''"
                   class="ghd-blue-bg ghd-white ghd-button-text"
                   @click="onSubmit(true)"
                   depressed                   
                   >
              Save
            </v-btn>
          </v-layout>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Getter} from 'vuex-class';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {emptyPerformanceCurveLibrary, PerformanceCurve, PerformanceCurveLibrary} from '@/shared/models/iAM/performance';
import {CreatePerformanceCurveLibraryDialogData} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {getUserName} from '@/shared/utils/get-user-info';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {clone} from 'ramda';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreatePerformanceCurveLibraryDialog extends Vue {
  @Prop() dialogData: CreatePerformanceCurveLibraryDialogData;

  @Getter('getIdByUserName') getIdByUserNameGetter: any;

  newPerformanceCurveLibrary: PerformanceCurveLibrary = {...emptyPerformanceCurveLibrary, id: getNewGuid()};
  rules: InputValidationRules = clone(rules);

  @Watch('dialogData')
  onDialogDataChanged() {
    let currentUser: string = getUserName();

    this.newPerformanceCurveLibrary = {
      ...this.newPerformanceCurveLibrary,
      performanceCurves: this.dialogData.performanceCurves.map((performanceCurve: PerformanceCurve) => {
        performanceCurve.id = getNewGuid();
        if (performanceCurve.equation.id !== getBlankGuid()) {
          performanceCurve.equation.id = getNewGuid();
        }
        return performanceCurve;
      }),
      owner: this.getIdByUserNameGetter(currentUser),
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
