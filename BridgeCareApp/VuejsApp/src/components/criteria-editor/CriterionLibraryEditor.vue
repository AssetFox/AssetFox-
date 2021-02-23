<template>
  <v-layout column>
    <v-flex>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onCreateNewLibrary" class="ara-blue-bg white--text">
            New Library
          </v-btn>
          <v-select v-if="!hasSelectedCriterionLibrary" v-model="librarySelectItemValue"
                    :items="criterionLibrarySelectItems" label="Select a Criteria Library" outline>
          </v-select>
          <v-text-field v-if="hasSelectedCriterionLibrary"
                        v-model="selectedCriterionLibrary.name" @change="canUpdateOrCreate = true">
            <template slot="append">
              <v-btn @click="librarySelectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="hasSelectedCriterionLibrary">
            Owner: {{ selectedCriterionLibrary.owner ? selectedCriterionLibrary.owner : '[ No Owner ]' }}
          </div>
          <v-checkbox v-if="hasSelectedCriterionLibrary"
                      v-model="selectedCriterionLibrary.shared"
                      class="sharing" label="Shared" @change="canUpdateOrCreate = true"/>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedCriterionLibrary"/>
    <v-flex v-show="hasSelectedCriterionLibrary">
      <v-layout justify-center>
        <v-flex xs10>
          <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                          @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedCriterionLibrary"/>
    <v-flex v-show="hasSelectedCriterionLibrary">
      <v-layout justify-center>
        <v-flex xs6>
          <v-textarea v-model="selectedCriterionLibrary.description" label="Description" no-resize outline
                      rows="4">
          </v-textarea>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-flex>
      <v-layout v-show="hasSelectedCriterionLibrary" justify-end row>
        <v-btn @click="onAddOrUpdateCriterionLibrary" class="ara-blue-bg white--text" :disabled="!canUpdateOrCreate">
          Update Library
        </v-btn>
        <v-btn @click="onCreateAsNewLibrary" class="ara-blue-bg white--text" :disabled="!canUpdateOrCreate">
          Create as New Library
        </v-btn>
        <v-btn @click="onDeleteCriterionLibrary" class="ara-orange-bg white--text">
          Delete Library
        </v-btn>
      </v-layout>
    </v-flex>

    <CreateCriterionLibraryDialog :dialogData="createCriterionLibraryDialogData"
                                  @submit="onAddOrUpdateCriterionLibrary"/>

    <Alert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import {Watch} from 'vue-property-decorator';
import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
import {SelectItem} from '@/shared/models/vue/select-item';
import {
  CriteriaEditorData,
  CriteriaEditorResult,
  CriterionLibrary,
  emptyCriteriaEditorData,
  emptyCriterionLibrary
} from '@/shared/models/iAM/criteria';

import {clone, isNil} from 'ramda';
import {hasUnsavedChanges} from '@/shared/utils/has-unsaved-changes-helper';
import {
  CreateCriterionLibraryDialogData,
  emptyCreateCriterionLibraryDialogData
} from '@/shared/models/modals/create-criterion-library-dialog-data';
import CreateCriterionLibraryDialog from '@/components/criteria-editor/criteria-editor-dialogs/CreateCriterionLibraryDialog.vue';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';

@Component({
  components: {Alert, CreateCriterionLibraryDialog, CriteriaEditor}
})
export default class CriterionLibraryEditor extends Vue {
  @State(state => state.criteriaEditor.criterionLibraries) stateCriterionLibraries: CriterionLibrary[];
  @State(state => state.criteriaEditor.selectedCriterionLibrary) stateSelectedCriterionLibrary: CriterionLibrary;

  @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
  @Action('addOrUpdateCriterionLibrary') addOrUpdateCriterionLibraryAction: any;
  @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
  @Action('deleteCriterionLibrary') deleteCriterionLibraryAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  hasSelectedCriterionLibrary: boolean = false;
  criterionLibrarySelectItems: SelectItem[] = [];
  librarySelectItemValue: string | null = null;
  selectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
  criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };
  createCriterionLibraryDialogData: CreateCriterionLibraryDialogData = clone(emptyCreateCriterionLibraryDialogData);
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  canUpdateOrCreate: boolean = false;
  uuidNIL: string = getBlankGuid();

  /**
   * beforeRouteEnter => This event handler is used to unset the librarySelectItemValue object and to trigger an action
   * to call a service function to create an HTTP request for all criterion libraries.
   */
  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('CriterionLibraryEditor/Library') !== -1) {
        vm.librarySelectItemValue = null;
        vm.getCriterionLibrariesAction();
      }
    });
  }

  /**
   * beforeDestroy => This event handler is used to trigger an action to modify the vuex hasUnsavedChanges object.
   */
  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  /**
   * onStateCriterionLibrariesChanged => This stateCriterionLibraries watcher is used to set the criterionLibrarySelectItems
   * object.
   */
  @Watch('stateCriterionLibraries')
  onStateCriterionLibrariesChanged() {
    this.criterionLibrarySelectItems = this.stateCriterionLibraries.map((library: CriterionLibrary) => ({
      text: library.name,
      value: library.id
    }));
  }

  /**
   * onLibrarySelectItemValueChanged => This librarySelectItemValue watcher is used to trigger an action to modify the
   * vuex selectedCriterionLibrary object.
   */
  @Watch('librarySelectItemValue')
  onLibrarySelectItemValueChanged() {
    this.selectCriterionLibraryAction({libraryId: this.librarySelectItemValue});
  }

  /**
   * onStateSelectedCriterionLibraryChanged => This stateSelectedCriterionLibrary watcher is used to reset the
   * canUpdateOrCreate object and to set the selectedCriterionLibrary object.
   */
  @Watch('stateSelectedCriterionLibrary')
  onStateSelectedCriterionLibraryChanged() {
    this.canUpdateOrCreate = false;
    this.selectedCriterionLibrary = clone(this.stateSelectedCriterionLibrary);
  }

  /**
   * onSelectedCriterionLibraryChanged => This selectedCriterionLibrary watcher is used to trigger an action to modify
   * the vuex hasUnsavedChanges object, set the hasSelectedCriterionLibrary object, and set the criteriaEditorData object.
   */
  @Watch('selectedCriterionLibrary')
  onSelectedCriterionLibraryChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChanges(
          'criteria', this.selectedCriterionLibrary, this.stateSelectedCriteriaLibrary, null
      )
    });

    this.hasSelectedCriterionLibrary = this.selectedCriterionLibrary.id !== this.uuidNIL;

    this.criteriaEditorData = {
      ...this.criteriaEditorData,
      mergedCriteriaExpression: this.selectedCriterionLibrary.mergedCriteriaExpression
    };
  }

  /**
   * onCreateNewLibrary => This function is used to set the createCriterionLibraryDialogData object.
   */
  onCreateNewLibrary() {
    this.createCriterionLibraryDialogData = {
      ...this.createCriterionLibraryDialogData,
      showDialog: true
    };
  }

  /**
   * onSubmitCriteriaEditorResult => This function is used to set the canUpdateOrCreate object and set the selectedCriterionLibrary
   * object.
   */
  onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
    this.canUpdateOrCreate = result.validated;

    if (result.validated) {
      this.selectedCriterionLibrary = {
        ...this.selectedCriterionLibrary,
        mergedCriteriaExpression: result.criteria!
      };
    }
  }

  /**
   * onCreateAsNewLibrary => This function is used to set the createCriterionLibraryDialogData object with context data
   * from the selectedCriterionLibrary object.
   */
  onCreateAsNewLibrary() {
    this.createCriterionLibraryDialogData = {
      showDialog: true,
      criteria: this.selectedCriterionLibrary.mergedCriteriaExpression,
      description: this.selectedCriterionLibrary.description
    };
  }

  /**
   * onAddOrUpdateCriterionLibrary => This function is used to reset the createCriterionLibraryDialogData object and to
   * trigger an action to call a service function to create an HTTP request to add/update the criterionLibrary object
   * data in the database.
   */
  onAddOrUpdateCriterionLibrary(criterionLibrary: CriterionLibrary) {
    this.createCriterionLibraryDialogData = clone(emptyCreateCriterionLibraryDialogData);

    if (!isNil(criterionLibrary) && criterionLibrary.id !== this.uuidNIL) {
      this.addOrUpdateCriterionLibraryAction({library: criterionLibrary})
          .then(() => this.librarySelectItemValue = criterionLibrary.id);
    }
  }

  /**
   * onDeleteCriterionLibrary => This function is used to set the confirmDeleteAlertData object.
   */
  onDeleteCriterionLibrary() {
    this.confirmDeleteAlertData = {
      showDialog: true,
      heading: 'Warning',
      choice: true,
      message: 'Are you sure you want to delete?'
    };
  }

  /**
   * onSubmitConfirmDeleteAlertResult => This function is used to reset the confirmDeleteAlertData object and to trigger
   * an action to call a service function to create an HTTP request to delete the selectedCriterionLibrary object in the
   * database.
   */
  onSubmitConfirmDeleteAlertResult(submit: boolean) {
    this.confirmDeleteAlertData = clone(emptyAlertData);

    if (submit) {
      this.deleteCriterionLibraryAction({libraryId: this.selectedCriterionLibrary.id})
          .then(() => this.librarySelectItemValue = null);
    }
  }
}
</script>