<template>
  <v-dialog persistent fullscreen v-model="dialogData.showDialog">
    <v-card>
      <v-card-text>
        <v-layout justify-center column>
          <div>
            <CriterionLibraryEditor />
          </div>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="stateSelectedCriterionLibrary.id === uuidNIL"
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
import {Action, State} from 'vuex-class';
import {CriterionLibraryEditorDialogData} from '../models/modals/criterion-library-editor-dialog-data';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';
import {any, propEq} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

@Component({
  components: {CriterionLibraryEditor}
})
export default class CriterionLibraryEditorDialog extends Vue {
  @Prop() dialogData: CriterionLibraryEditorDialogData;

  @State(state => state.criteriaEditor.criterionLibraries) stateCriterionLibraries: CriterionLibrary[];
  @State(state => state.criteriaEditor.selectedCriterionLibrary) stateSelectedCriterionLibrary: CriterionLibrary;

  @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
  @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;

  uuidNIL: string = getBlankGuid();

  /**
   *
   */
  mounted() {
    this.getCriterionLibrariesAction();
  }

  /**
   *
   */
  @Watch('dialogData')
  onDialogDataChanged() {
    if (hasValue(this.stateCriterionLibraries) && any(propEq('id', this.dialogData.libraryId), this.stateCriterionLibraries)) {
      this.selectCriterionLibraryAction({libraryId: this.dialogData.libraryId});
    }
  }

  /**
   *
   */
  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.stateSelectedCriterionLibrary);
    } else {
      this.$emit('submit', null);
    }
  }
}
</script>

<style>
.v-dialog:not(.v-dialog--fullscreen) {
  max-height: 100%;
  max-width: 75%;
}
</style>
