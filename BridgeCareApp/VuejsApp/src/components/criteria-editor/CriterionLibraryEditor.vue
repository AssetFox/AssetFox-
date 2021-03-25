<template>
  <v-layout column>
    <v-flex>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onShowCreateCriterionLibraryDialog(false)" class="ara-blue-bg white--text">
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
        <v-btn @click="onUpsertCriterionLibrary(selectedCriterionLibrary)" class="ara-blue-bg white--text"
               :disabled="!canUpdateOrCreate">
          Update Library
        </v-btn>
        <v-btn @click="onShowCreateCriterionLibraryDialog(true)" class="ara-blue-bg white--text"
               :disabled="!canUpdateOrCreate">
          Create as New Library
        </v-btn>
        <v-btn @click="onShowConfirmDeleteAlert" class="ara-orange-bg white--text">
          Delete Library
        </v-btn>
      </v-layout>
    </v-flex>

    <CreateCriterionLibraryDialog :dialogData="createCriterionLibraryDialogData"
                                  @submit="onUpsertCriterionLibrary"/>

    <ConfirmDeleteAlert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import {Prop, Watch} from 'vue-property-decorator';
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
import {hasUnsavedChanges, hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import {
  CreateCriterionLibraryDialogData,
  emptyCreateCriterionLibraryDialogData
} from '@/shared/models/modals/create-criterion-library-dialog-data';
import CreateCriterionLibraryDialog
  from '@/components/criteria-editor/criteria-editor-dialogs/CreateCriterionLibraryDialog.vue';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {hasValue} from '@/shared/utils/has-value-util';

@Component({
  components: {ConfirmDeleteAlert: Alert, CreateCriterionLibraryDialog, CriteriaEditor}
})
export default class CriterionLibraryEditor extends Vue {
  @Prop() dialogLibraryId: string;

  @State(state => state.criterionModule.criterionLibraries) stateCriterionLibraries: CriterionLibrary[];
  @State(state => state.criterionModule.selectedCriterionLibrary) stateSelectedCriterionLibrary: CriterionLibrary;

  @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
  @Action('upsertCriterionLibrary') upsertCriterionLibraryAction: any;
  @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
  @Action('deleteCriterionLibrary') deleteCriterionLibraryAction: any;
  @Action('setSelectedCriterionIsValid') setSelectedCriterionIsValidAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  hasSelectedCriterionLibrary: boolean = false;
  criterionLibrarySelectItems: SelectItem[] = [];
  librarySelectItemValue: string | null = null;
  selectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
  criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };
  isLibraryContext: boolean = false;
  createCriterionLibraryDialogData: CreateCriterionLibraryDialogData = clone(emptyCreateCriterionLibraryDialogData);
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  canUpdateOrCreate: boolean = false;
  uuidNIL: string = getBlankGuid();

  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('CriterionLibraryEditor/Library') !== -1) {
        vm.librarySelectItemValue = null;
        vm.getCriterionLibrariesAction();
      } else {
        vm.isLibraryContext = false;
      }
    });
  }

  beforeDestroy() {
    if (this.isLibraryContext) {
      this.setHasUnsavedChangesAction({value: false});
    }
  }

  @Watch('stateCriterionLibraries')
  onStateCriterionLibrariesChanged() {
    this.criterionLibrarySelectItems = this.stateCriterionLibraries.map((library: CriterionLibrary) => ({
      text: library.name,
      value: library.id
    }));

    if (!this.isLibraryContext && hasValue(this.librarySelectItemValue)) {
      this.selectCriterionLibraryAction({libraryId: this.librarySelectItemValue});
    }
  }

  @Watch('dialogLibraryId')
  onDialogLibraryIdChanged() {
    this.librarySelectItemValue = this.dialogLibraryId;
  }

  @Watch('librarySelectItemValue')
  onLibrarySelectItemValueChanged() {
    this.selectCriterionLibraryAction({libraryId: this.librarySelectItemValue});
  }

  @Watch('stateSelectedCriterionLibrary')
  onStateSelectedCriterionLibraryChanged() {
    this.canUpdateOrCreate = false;
    this.selectedCriterionLibrary = clone(this.stateSelectedCriterionLibrary);
  }

  @Watch('canUpdateOrCreate')
  onCanUpdateOrCreateChanged() {

  }

  @Watch('selectedCriterionLibrary')
  onSelectedCriterionLibraryChanged() {
    this.hasSelectedCriterionLibrary = this.selectedCriterionLibrary.id !== this.uuidNIL;

    this.criteriaEditorData = {
      ...this.criteriaEditorData,
      mergedCriteriaExpression: this.selectedCriterionLibrary.mergedCriteriaExpression
    };

    if (this.isLibraryContext) {
      this.setHasUnsavedChangesAction({
        value: hasUnsavedChangesCore(
            'criterion-library', this.selectedCriterionLibrary, this.stateSelectedCriterionLibrary
        )
      });
    } else {
      this.$emit('submit', this.selectedCriterionLibrary);
    }
  }

  onShowCreateCriterionLibraryDialog(createAsNew: boolean) {
    this.createCriterionLibraryDialogData = {
      showDialog: true,
      mergedCriteriaExpression: createAsNew ? 
      (this.selectedCriterionLibrary.mergedCriteriaExpression != null ? this.selectedCriterionLibrary.mergedCriteriaExpression : '')
       : ''
    };
  }

  onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
    this.canUpdateOrCreate = result.validated;

    if (result.validated) {
      this.selectedCriterionLibrary = {
        ...this.selectedCriterionLibrary,
        mergedCriteriaExpression: result.criteria!
      };

      this.setSelectedCriterionIsValidAction({isValid: true});
    } else {
      this.setSelectedCriterionIsValidAction({isValid: false});
    }
  }

  onUpsertCriterionLibrary(criterionLibrary: CriterionLibrary) {
    this.createCriterionLibraryDialogData = clone(emptyCreateCriterionLibraryDialogData);

    if (!isNil(criterionLibrary)) {
      this.upsertCriterionLibraryAction({library: criterionLibrary})
          .then(() => this.librarySelectItemValue = criterionLibrary.id);
    }
  }

  onShowConfirmDeleteAlert() {
    this.confirmDeleteAlertData = {
      showDialog: true,
      heading: 'Warning',
      choice: true,
      message: 'Are you sure you want to delete?'
    };
  }

  onSubmitConfirmDeleteAlertResult(submit: boolean) {
    this.confirmDeleteAlertData = clone(emptyAlertData);

    if (submit) {
      this.deleteCriterionLibraryAction({libraryId: this.selectedCriterionLibrary.id})
          .then(() => this.librarySelectItemValue = null);
    }
  }
}
</script>