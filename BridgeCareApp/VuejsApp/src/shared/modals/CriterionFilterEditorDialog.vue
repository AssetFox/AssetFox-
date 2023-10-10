<template>
  <v-dialog persistent fullscreen v-model="dialogData.showDialog" class="criterion-library-editor-dialog">
    <v-card>
      <v-card-text>
        <v-row justify-center column>
          <div>
            <v-row justify-center>
        <v-flex xs10>
          <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                          @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
        </v-flex>
      </v-row>
          </div>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-row justify-space-between row>
            <v-btn
                 class="ara-blue-bg text-white"
                 @click="onBeforeSubmit(true)">
            Save
          </v-btn>
          <v-btn class="ara-orange-bg text-white"
                 @click="onSubmit(false)">
            Cancel
          </v-btn>
        </v-row>
      </v-card-actions>
    </v-card>

    <HasUnsavedChangesAlert :dialogData="hasUnsavedChangesAlertData" @submit="onCloseHasUnsavedChangesAlert"/>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
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
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submitCriteriaEditorDialogResult'])
const props = defineProps<{
  dialogData: CriterionFilterEditorDialogData
    }>()

async function getAvailableReportsAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}

  let uuidNIL: string = getBlankGuid();
  let hasUnsavedChanges: boolean = false;
  let hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

  let resultForParentComponent: UserCriteriaFilter;

   let criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };

  watch(()=>props.dialogData,()=>onDialogDataChanged())
  function onDialogDataChanged() {
    const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
    const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

    if (props.dialogData.showDialog) {
    
    criteriaEditorData = {
        ...criteriaEditorData,
        mergedCriteriaExpression: props.dialogData.criteria,
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

    function onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        const canUpdateOrCreate = result.validated;

        if (result.validated) {

        var tempL : UserCriteriaFilter = {userId : props.dialogData.userId, 
                userName: props.dialogData.userName, 
                description: props.dialogData.description,
                hasAccess: true, 
                name: props.dialogData.name,
                hasCriteria: true, 
                criteria: result.criteria, 
                criteriaId: props.dialogData.criteriaId}; 
        resultForParentComponent = tempL;
        }
    }
    
  function onBeforeSubmit(submit: boolean) {
    if (hasUnsavedChanges) {
      onShowHasUnsavedChangesAlert();
    } else {
      onSubmit(submit);
    }
  }

  function onShowHasUnsavedChangesAlert() {
    hasUnsavedChangesAlertData = {
      showDialog: true,
        heading: 'Unsaved Changes',
        message: 'The selected criterion library has unsaved changes. Click "Update Library" or "Create as New Library" to save changes.',
        choice: false
    };
  }

  function onCloseHasUnsavedChangesAlert() {
    hasUnsavedChangesAlertData = clone(emptyAlertData);
  }

  function onSubmit(submit: boolean) {
    if (submit) {
      props.dialogData.showDialog = false;
      emit('submitCriteriaEditorDialogResult', resultForParentComponent);
    } else {
      props.dialogData.showDialog = false;
      emit('submitCriteriaEditorDialogResult', null);
    }
  }
</script>

<style>
.v-dialog:not(.v-dialog--fullscreen) {
  max-height: 100%;
  max-width: 75%;
}
</style>
