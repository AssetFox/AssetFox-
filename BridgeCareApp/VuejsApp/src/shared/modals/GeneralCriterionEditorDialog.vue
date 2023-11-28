<template>
    <v-dialog
        persistent
        v-model="dialogData.showDialog"
        class="criterion-library-editor-dialog" width="auto"
    >
        <v-card>
            <v-card-text>
                <v-row>
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
                    @click="onSubmit(false)"
                >
                    Cancel
                </v-btn>
                <v-btn
                    :disabled="!canUpdateOrCreate"
                    class="ghd-blue-bg ghd-white ghd-button-text"
                    flat
                    style="margin-left: 5px;"                    
                    @click="onSubmit(true)"
                >
                    Save
                </v-btn>
            </v-row>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { GeneralCriterionEditorDialogData } from '../models/modals/general-criterion-editor-dialog-data';
import {
  CriteriaEditorData,
    CriteriaEditorResult,
    CriterionLibrary,
    emptyCriteriaEditorData,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { isNil } from 'ramda';
import CriteriaEditor from '../components/CriteriaEditor.vue';
import { toRefs, ref, watch} from 'vue';
import { useStore } from 'vuex';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: GeneralCriterionEditorDialogData
    }>()
const { dialogData } = toRefs(props);
const criteriaEditorData = ref<CriteriaEditorData>({
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  });
const canUpdateOrCreate = ref<boolean>(false);

let CriteriaExpressionToReturn: string | null = "";

  watch(dialogData,()=> {
        // const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
        // const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;
        if (dialogData.value.showDialog) {    
            criteriaEditorData.value = {
                    ...criteriaEditorData.value,
                    mergedCriteriaExpression: dialogData.value.CriteriaExpression,
                    isLibraryContext: true
                };

            canUpdateOrCreate.value = false;

            // if (hasValue(htmlTag)) {
            //     htmlTag[0].setAttribute('style', 'overflow:hidden;');
            // }

            // if (hasValue(criteriaEditorCard)) {
            //     criteriaEditorCard[0].setAttribute('style', 'height:100%');
            // }
            // } else {
            // if (hasValue(htmlTag)) {
            //     htmlTag[0].setAttribute('style', 'overflow:auto;');
            // }
        }
    });

    function onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        canUpdateOrCreate.value = result.validated;

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
            dialogData.value.showDialog = false
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
