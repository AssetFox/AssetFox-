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
import {Action} from 'vuex-class';
import {CriterionFilterEditorDialogData} from '../models/modals/criterion-filter-editor-dialog-data';
import {hasValue} from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {clone} from 'ramda';
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

  @Action('updateUserCriteriaFilter') updateUserCriteriaFilterAction: any;

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
    
    this.criteriaEditorData = {
        ...this.criteriaEditorData,
        mergedCriteriaExpression: this.dialogData.criteria,
        isLibraryContext: true
        };

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

    onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        this.canUpdateOrCreate = result.validated;

        if (result.validated) {

        var tempL : UserCriteriaFilter = {userId : this.dialogData.userId, 
                userName: this.dialogData.userName, 
                hasAccess: true, 
                hasCriteria: true, 
                criteria: result.criteria, 
                criteriaId: this.dialogData.criteriaId}; 
        this.resultForParentComponent = tempL;
        }
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
    };
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
