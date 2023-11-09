<template>
  <v-dialog persistent
        v-model="dialogData.showDialog"
        class="criterion-library-editor-dialog">
    <v-card>
      <v-card-text>
                <v-row >
                    <div>
                      <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                                      @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
                    </div>
                </v-row>
            </v-card-text>
            <v-row justify="center" style="padding: 10px; margin: 0px;">
                <v-btn
                    class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                    flat
                    style="margin-right: 5px;"
                    @click="onBeforeSubmit(false)"
                >
                    Cancel
                </v-btn>
                <v-btn
                    class="ghd-blue-bg ghd-white ghd-button-text"
                    flat
                    style="margin-left: 5px;"                    
                    @click="onSubmit(true)"
                >
                    Save
                </v-btn>
            </v-row>
    </v-card>
    <HasUnsavedChangesAlert :dialogData="hasUnsavedChangesAlertData" @submit="onCloseHasUnsavedChangesAlert"/>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed, toRefs } from 'vue';
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
    const { dialogData } = toRefs(props);
let showDialogComputed = computed(() => props.dialogData.showDialog);
async function getAvailableReportsAction(payload?: any): Promise<any> {await store.dispatch('getAvailableReports');}

  let uuidNIL: string = getBlankGuid();
  let hasUnsavedChanges: boolean = false;
  let hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

  let resultForParentComponent: UserCriteriaFilter;

   let criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };

  watch(dialogData,()=> {
    const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
    const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

    if (dialogData.value.showDialog) {
    
    criteriaEditorData = {
        ...criteriaEditorData,
        mergedCriteriaExpression: dialogData.value.criteria,
        isLibraryContext: true
        };

      
    }
  })

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
