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
                        <CriterionLibraryEditor
                            :dialogLibraryId="dialogData.libraryId"
                            :dialogIsFromScenario="
                                dialogData.isCallFromScenario
                            "
                            :dialogIsFromLibrary="dialogData.isCriterionForLibrary"
                            @submit="onSubmitSelectedCriterionLibrary"
                        />
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
                        :disabled="(!dialogData.isCallFromScenario && !dialogData.isCriterionForLibrary) || !stateSelectedCriterionIsValid
                        "
                        class="ghd-blue-bg ghd-white ghd-button-text"
                        depressed
                        @click="onBeforeSubmit(true)"
                    >
                        Save
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>

        <HasUnsavedChangesAlert
            :dialogData="hasUnsavedChangesAlertData"
            @submit="onCloseHasUnsavedChangesAlert"
        />
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import { CriterionLibraryEditorDialogData } from '../models/modals/criterion-library-editor-dialog-data';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasValue } from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { clone, isNil } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';

@Component({
    components: { CriterionLibraryEditor, HasUnsavedChangesAlert: Alert },
})
export default class CriterionLibraryEditorDialog extends Vue {
    @Prop() dialogData: CriterionLibraryEditorDialogData;

    @State(state => state.criterionModule.criterionLibraries)
    stateCriterionLibraries: CriterionLibrary[];
    @State(state => state.criterionModule.selectedCriterionLibrary)
    stateSelectedCriterionLibrary: CriterionLibrary;
    @State(state => state.criterionModule.selectedCriterionIsValid)
    stateSelectedCriterionIsValid: boolean;
    @State(state => state.criterionModule.scenarioRelatedCriteria)
    stateScenarioRelatedCriteria: CriterionLibrary;

    @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
    @Action('setSelectedCriterionIsValid')
    setSelectedCriterionIsValidAction: any;
    @Action('selectScenarioRelatedCriterion')
    selectScenarioRelatedCriterionAction: any;
    @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
    @Action('upsertCriterionLibrary') upsertCriterionLibraryAction: any;

    criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary = clone(
        emptyCriterionLibrary,
    );
    uuidNIL: string = getBlankGuid();
    hasUnsavedChanges: boolean = false;
    hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

    @Watch('dialogData')
    onDialogDataChanged() {
        const htmlTag: HTMLCollection = document.getElementsByTagName(
            'html',
        ) as HTMLCollection;
        const criteriaEditorCard: HTMLCollection = document.getElementsByClassName(
            'criteria-editor-card',
        ) as HTMLCollection;

        if (this.dialogData.showDialog) {
            if (!hasValue(this.stateCriterionLibraries)) {
                this.getCriterionLibrariesAction().then(() => {
                    if (this.dialogData.isCallFromScenario || this.dialogData.isCriterionForLibrary) {
                        this.selectScenarioRelatedCriterionAction({
                            libraryId: this.dialogData.libraryId,
                        });
                    }
                });
            } else {
                if (this.dialogData.isCallFromScenario || this.dialogData.isCriterionForLibrary) {
                    this.selectScenarioRelatedCriterionAction({
                        libraryId: this.dialogData.libraryId,
                    });
                }
            }

            this.setSelectedCriterionIsValidAction({ isValid: false });

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
            this.selectScenarioRelatedCriterionAction({
                libraryId: this.uuidNIL,
            });
        }
    }

    @Watch('criterionLibraryEditorSelectedCriterionLibrary')
    onCriterionLibraryEditorSelectedCriterionLibraryChanged() {
        this.hasUnsavedChanges = hasUnsavedChangesCore(
            'criterion-library',
            this.criterionLibraryEditorSelectedCriterionLibrary,
            this.stateSelectedCriterionLibrary,
        );
    }

    onSubmitSelectedCriterionLibrary(
        criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorSelectedCriterionLibrary = clone(
            criterionLibraryEditorSelectedCriterionLibrary,
        );
    }

    onBeforeSubmit(submit: boolean) {
        if (this.hasUnsavedChanges && !this.dialogData.isCallFromScenario) {
            this.onShowHasUnsavedChangesAlert();
        } else {
            this.onSubmit(submit);
        }
    }

    onShowHasUnsavedChangesAlert() {
        this.hasUnsavedChangesAlertData = {
            showDialog: true,
            heading: 'Unsaved Changes',
            message:
                'The selected criterion library has unsaved changes. Click "Update Library" or "Create as New Library" to save changes.',
            choice: false,
        };
    }

    onCloseHasUnsavedChangesAlert() {
        this.hasUnsavedChangesAlertData = clone(emptyAlertData);
    }

    onSubmit(submit: boolean) {
        if (submit) {
            if (!isNil(this.stateScenarioRelatedCriteria)) {
                this.upsertCriterionLibraryAction({
                    library: this.stateScenarioRelatedCriteria,
                })
                .then((id: string) => {
                    this.stateScenarioRelatedCriteria.id = id;
                    this.$emit('submit', this.stateScenarioRelatedCriteria);
                });
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
