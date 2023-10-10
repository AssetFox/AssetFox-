<template>
    <v-dialog
        persistent
        fullscreen
        v-model="dialogData.showDialog"
        class="criterion-library-editor-dialog"
    >
        <v-card>
            <v-card-text>
                <v-row justify-center column>
                    <div>
                        <v-row justify-center>
                            <v-col cols = "10">
                            <CriteriaEditor :criteriaEditorData="criteriaEditorData"
                                            @submitCriteriaEditorResult="onSubmitCriteriaEditorResult"/>
                            </v-col>
                        </v-row>
                    </div>
                </v-row>
            </v-card-text>
            <v-card-actions>
                <v-row justify-center>
                    <v-btn
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                        variant = "flat"
                        @click="onSubmit(false)"
                    >
                        Cancel
                    </v-btn>
                    <v-btn
                        :disabled="!canUpdateOrCreate"
                        class="ghd-blue-bg ghd-white ghd-button-text"
                        variant = "flat"
                        @click="onSubmit(true)"
                    >
                        Save
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import { GeneralCriterionEditorDialogData } from '../models/modals/general-criterion-editor-dialog-data';
import {
  CriteriaEditorData,
    CriteriaEditorResult,
    CriterionLibrary,
    emptyCriteriaEditorData,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasValue } from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone, isNil } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import CriteriaEditor from '../components/CriteriaEditor.vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: GeneralCriterionEditorDialogData
    }>()
let stateCriterionLibraries = ref<CriterionLibrary[]>(store.state.criterionModule.criterionLibraries);
let stateSelectedCriterionLibrary = ref<CriterionLibrary>(store.state.criterionModule.selectedCriterionLibrary);
let stateSelectedCriterionIsValid = ref<boolean>(store.state.criterionModule.selectedCriterionIsValid);

let criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };

  let uuidNIL: string = getBlankGuid();
  let canUpdateOrCreate: boolean = false;

  let CriteriaExpressionToReturn: string | null = "";

  watch(()=>props.dialogData,()=>onDialogDataChanged())
    function onDialogDataChanged() {
        const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
        const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

        if (props.dialogData.showDialog) {    
            criteriaEditorData = {
                    ...criteriaEditorData,
                    mergedCriteriaExpression: props.dialogData.CriteriaExpression,
                    isLibraryContext: true
                };

            canUpdateOrCreate = false;

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
        canUpdateOrCreate = result.validated;

        if (result.validated) {
            CriteriaExpressionToReturn = result.criteria
        }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            if (!isNil(CriteriaExpressionToReturn)) {
                emit('submit', CriteriaExpressionToReturn);
            }
        } else {
            emit('submit', null);
        }
    }
</script>

<style>
.v-dialog:not(.v-dialog--fullscreen) {
    max-height: 100%;
    max-width: 75%;
}
</style>
