<template>
  <v-dialog persistent fullscreen v-model="dialogData.showDialog" class="criterion-library-editor-dialog">
    <v-card>
      <v-card-text>
        <v-layout justify-center column>
          <div>
            <CriterionLibraryEditor :dialogLibraryId="dialogData.libraryId"
                                    @submit="onSubmitSelectedCriterionLibrary" />
          </div>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="stateSelectedCriterionLibrary.id === uuidNIL || !stateSelectedCriterionIsValid"
                 class="ara-blue-bg white--text"
                 @click="onBeforeSubmit(true)">
            Save
          </v-btn>
          <v-btn class="ara-orange-bg white--text"
                 @click="onSubmit(false)">
            Cancel
          </v-btn>
        </v-layout>
      </v-card-actions>
    </v-card>

    <HasUnsavedChangesAlert :dialogData="hasUnsavedChangesAlertData" @submit="onCloseHasUnsavedChangesAlert"/>
  </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {CriterionLibraryEditorDialogData} from '../models/modals/criterion-library-editor-dialog-data';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {hasValue} from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import Alert from '@/shared/modals/Alert.vue';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';

@Component({
  components: {CriterionLibraryEditor, HasUnsavedChangesAlert: Alert}
})
export default class CriterionLibraryEditorDialog extends Vue {
  @Prop() dialogData: CriterionLibraryEditorDialogData;

  @State(state => state.criterionModule.criterionLibraries) stateCriterionLibraries: CriterionLibrary[];
  @State(state => state.criterionModule.selectedCriterionLibrary) stateSelectedCriterionLibrary: CriterionLibrary;
  @State(state => state.criterionModule.selectedCriterionIsValid) stateSelectedCriterionIsValid: boolean;

  @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
  @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
  @Action('setSelectedCriterionIsValid') setSelectedCriterionIsValidAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
  uuidNIL: string = getBlankGuid();
  hasUnsavedChanges: boolean = false;
  hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

  @Watch('dialogData')
  onDialogDataChanged() {
    const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
    const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

    if (this.dialogData.showDialog) {
      if (!hasValue(this.stateCriterionLibraries)) {
        this.getCriterionLibrariesAction();
      }

      this.setSelectedCriterionIsValidAction({isValid: false});

      if (hasValue(htmlTag)) {
        htmlTag[0].setAttribute('style', 'overflow:hidden;');
      }

      if (hasValue(criteriaEditorCard)) {
        criteriaEditorCard[0].setAttribute('style', 'height:100%');
      }
    } else {
      if (hasValue(htmlTag)) {
        htmlTag[0].setAttribute('style', 'overflow:auto;');
      }
    }
  }

  @Watch('criterionLibraryEditorSelectedCriterionLibrary')
  onCriterionLibraryEditorSelectedCriterionLibraryChanged() {
    this.hasUnsavedChanges = hasUnsavedChangesCore(
        'criterion-library', this.criterionLibraryEditorSelectedCriterionLibrary, this.stateSelectedCriterionLibrary
    );
  }

  onSubmitSelectedCriterionLibrary(criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorSelectedCriterionLibrary = clone(criterionLibraryEditorSelectedCriterionLibrary);
  }

  onBeforeSubmit(submit: boolean) {
    if (this.hasUnsavedChanges) {
      this.onShowHasUnsavedChangesAlert();
    } else {
      this.onSubmit(submit);
    }
  }

  onShowHasUnsavedChangesAlert() {
    this.hasUnsavedChangesAlertData = {
      showDialog: true,
        heading: 'Unsaved Changes',
        message: 'The selected criterion library has unsaved changes. Click "Update Library" or "Create as New Library" to save changes.',
        choice: false
    }
  }

  onCloseHasUnsavedChangesAlert() {
    this.hasUnsavedChangesAlertData = clone(emptyAlertData);
  }

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
