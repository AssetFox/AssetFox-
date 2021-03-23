<template>
  <v-dialog persistent fullscreen v-model="dialogData.showDialog" class="criterion-library-editor-dialog">
    <v-card>
      <v-card-text>
        <v-layout justify-center column>
          <div>
            <v-layout justify-center>
        <v-flex xs10>
          <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                          @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
        </v-flex>
      </v-layout>
          </div>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <!-- <v-btn :disabled="stateSelectedCriterionLibrary.id === uuidNIL || !stateSelectedCriterionIsValid" -->
            <v-btn
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
import {CriterionFilterEditorDialogData} from '../models/modals/criterion-filter-editor-dialog-data';
import {CriterionLibrary, emptyCriterionLibrary} from '@/shared/models/iAM/criteria';
import {hasValue} from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import Alert from '@/shared/modals/Alert.vue';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
import {
  CriteriaEditorData,
  CriteriaEditorResult,
  emptyCriteriaEditorData
} from '@/shared/models/iAM/criteria';
import { UserCriteriaFilter } from '../models/iAM/user-criteria-filter';

@Component({
  components: {CriterionLibraryEditor, HasUnsavedChangesAlert: Alert, CriteriaEditor}
})
export default class CriterionFilterEditorDialog extends Vue {
  @Prop() dialogData: CriterionFilterEditorDialogData;

  //@State(state => state.criterionModule.criterionLibraries) stateCriterionLibraries: CriterionLibrary[];
  //@State(state => state.criterionModule.selectedCriterionLibrary) stateSelectedCriterionLibrary: CriterionLibrary;
  @State(state => state.criterionModule.selectedCriterionIsValid) stateSelectedCriterionIsValid: boolean;
  //@State(state => state.)

  //@Action('getCriterionLibraries') getCriterionLibrariesAction: any;
  //@Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
  //@Action('setSelectedCriterionIsValid') setSelectedCriterionIsValidAction: any;
  //@Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
  @Action('updateUserCriteriaFilter') updateUserCriteriaFilterAction: any;

  //criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
  uuidNIL: string = getBlankGuid();
  hasUnsavedChanges: boolean = false;
  hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

  resultForParentComponent: UserCriteriaFilter;

   criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };

  @Watch('dialogData')
  onDialogDataChanged() {
    const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
    const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

    if (this.dialogData.showDialog) {
    //   if (!hasValue(this.stateCriterionLibraries)) {
    //     this.getCriterionLibrariesAction();
    //   }
    
    this.criteriaEditorData = {
        ...this.criteriaEditorData,
        mergedCriteriaExpression: this.dialogData.criteria,
        isLibraryContext: true
        };
      //this.setSelectedCriterionIsValidAction({isValid: false});

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

//   @Watch('criterionLibraryEditorSelectedCriterionLibrary')
//   onCriterionLibraryEditorSelectedCriterionLibraryChanged() {
//     this.hasUnsavedChanges = hasUnsavedChangesCore(
//         'criterion-library', this.criterionLibraryEditorSelectedCriterionLibrary, this.stateSelectedCriterionLibrary
//     );
//   }

//   @Watch('selectedCriterionLibrary')
//   onSelectedCriterionLibraryChanged() {
//     this.hasSelectedCriterionLibrary = this.selectedCriterionLibrary.id !== this.uuidNIL;

//     this.criteriaEditorData = {
//       ...this.criteriaEditorData,
//       mergedCriteriaExpression: this.selectedCriterionLibrary.mergedCriteriaExpression
//     };

//     if (this.isLibraryContext) {
//       this.setHasUnsavedChangesAction({
//         value: hasUnsavedChangesCore(
//             'criterion-library', this.selectedCriterionLibrary, this.stateSelectedCriterionLibrary
//         )
//       });
//     } else {
//       this.$emit('submit', this.selectedCriterionLibrary);
//     }
//   }

//   onSubmitSelectedCriterionLibrary(criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary) {
//     this.criterionLibraryEditorSelectedCriterionLibrary = clone(criterionLibraryEditorSelectedCriterionLibrary);
//   }

    onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        this.canUpdateOrCreate = result.validated;

// TODO: if critera editor is sending a valid criteria, then update the userCriteria Table
        if (result.validated) {
        // this.selectedCriterionLibrary = {
        //     ...this.selectedCriterionLibrary,
        //     mergedCriteriaExpression: result.criteria!
        // };

        var tempL : UserCriteriaFilter = {userId : this.dialogData.userId, 
                userName: this.dialogData.userName, 
                hasAccess: true, 
                hasCriteria: true, 
                criteria: result.criteria, 
                criteriaId: this.dialogData.criteriaId}; 
        this.resultForParentComponent = tempL;

        //this.updateUserCriteriaFilterAction({userCriteriaFilter : tempL})
        //this.setSelectedCriterionIsValidAction({isValid: true});
        } else {
        //this.setSelectedCriterionIsValidAction({isValid: false});
        }
        // this.$emit('submitCriteriaEditorDialogResult', {criteriaId: this.dialogData.criteriaId, userId: this.dialogData.userId,
        // userName: this.dialogData.userName, hasCriteria: this.dialogData.hasCriteria, hasAccess: this.dialogData.hasAccess,
        // criteria: result.criteria});
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
      this.dialogData.showDialog = false;
      this.$emit('submitCriteriaEditorDialogResult', this.resultForParentComponent);
    } else {
      this.dialogData.showDialog = false;
      this.$emit('submitCriteriaEditorDialogResult', null);
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
