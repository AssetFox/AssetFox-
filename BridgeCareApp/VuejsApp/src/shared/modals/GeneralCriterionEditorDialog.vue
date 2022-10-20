<template>
    <v-dialog
        persistent
        fullscreen
        v-model="dialogData.showDialog"
        class="criterion-library-editor-dialog"
    >
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
                <v-layout justify-center>
                    <v-btn
                        class="ghd-white-bg ghd-blue ghd-button-text ghd-outline-button-padding ghd-button ghd-button-border"
                        depressed
                        @click="onSubmit(false)"
                    >
                        Cancel
                    </v-btn>
                    <v-btn
                        :disabled="!canUpdateOrCreate"
                        class="ghd-blue-bg ghd-white ghd-button-text"
                        depressed
                        @click="onSubmit(true)"
                    >
                        Save
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
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

@Component({
    components: { CriterionLibraryEditor, HasUnsavedChangesAlert: Alert, CriteriaEditor },
})
export default class GeneralCriterionEditorDialog extends Vue {
    @Prop() dialogData: GeneralCriterionEditorDialogData;

    @State(state => state.criterionModule.criterionLibraries)
    stateCriterionLibraries: CriterionLibrary[];
    @State(state => state.criterionModule.selectedCriterionLibrary)
    stateSelectedCriterionLibrary: CriterionLibrary;
    @State(state => state.criterionModule.selectedCriterionIsValid)
    stateSelectedCriterionIsValid: boolean;

    criteriaEditorData: CriteriaEditorData = {
    ...emptyCriteriaEditorData,
    isLibraryContext: true
  };

  uuidNIL: string = getBlankGuid();
  canUpdateOrCreate: boolean = false;

  CriteriaExpressionToReturn: string | null = "";

  @Watch('dialogData')
    onDialogDataChanged() {
        const htmlTag: HTMLCollection = document.getElementsByTagName('html') as HTMLCollection;
        const criteriaEditorCard: HTMLCollection = document.getElementsByClassName('criteria-editor-card') as HTMLCollection;

        if (this.dialogData.showDialog) {    
            this.criteriaEditorData = {
                    ...this.criteriaEditorData,
                    mergedCriteriaExpression: this.dialogData.CriteriaExpression,
                    isLibraryContext: true
                };

            this.canUpdateOrCreate = false;

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
            this.CriteriaExpressionToReturn = result.criteria
        }
    }

    onSubmit(submit: boolean) {
        if (submit) {
            if (!isNil(this.CriteriaExpressionToReturn)) {
                this.$emit('submit', this.CriteriaExpressionToReturn);
            }
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
