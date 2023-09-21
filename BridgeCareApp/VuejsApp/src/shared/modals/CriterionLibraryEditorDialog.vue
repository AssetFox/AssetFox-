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

<script lang="ts" setup>
import Vue from 'vue';
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
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: CriterionLibraryEditorDialogData
    }>()

    let stateCriterionLibraries = ref<CriterionLibrary[]>(store.state.criterionModule.criterionLibraries);
    let stateSelectedCriterionLibrary = ref<CriterionLibrary>(store.state.criterionModule.selectedCriterionLibrary);
    let stateSelectedCriterionIsValid = ref<boolean>(store.state.criterionModule.selectedCriterionIsValid);
    let stateScenarioRelatedCriteria = ref<CriterionLibrary>(store.state.criterionModule.scenarioRelatedCriteria);
    async function getCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCriterionLibraries');}
    async function setSelectedCriterionIsValidAction(payload?: any): Promise<any> {await store.dispatch('setSelectedCriterionIsValid');}
    async function selectScenarioRelatedCriterionAction(payload?: any): Promise<any> {await store.dispatch('selectScenarioRelatedCriterion');}
    async function selectCriterionLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectCriterionLibrary');}
    async function upsertCriterionLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertCriterionLibrary');}
    let criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary = clone(
        emptyCriterionLibrary,
    );
    let uuidNIL: string = getBlankGuid();
    let hasUnsavedChanges: boolean = false;
    let hasUnsavedChangesAlertData: AlertData = clone(emptyAlertData);

    watch(()=>props.dialogData,()=>onDialogDataChanged())
    function onDialogDataChanged() {
        const htmlTag: HTMLCollection = document.getElementsByTagName(
            'html',
        ) as HTMLCollection;
        const criteriaEditorCard: HTMLCollection = document.getElementsByClassName(
            'criteria-editor-card',
        ) as HTMLCollection;

        if (props.dialogData.showDialog) {
            if (!hasValue(stateCriterionLibraries)) {
                getCriterionLibrariesAction().then(() => {
                    if (props.dialogData.isCallFromScenario || props.dialogData.isCriterionForLibrary) {
                        selectScenarioRelatedCriterionAction({
                            libraryId: props.dialogData.libraryId,
                        });
                    }
                });
            } else {
                if (props.dialogData.isCallFromScenario || props.dialogData.isCriterionForLibrary) {
                    selectScenarioRelatedCriterionAction({
                        libraryId: props.dialogData.libraryId,
                    });
                }
            }

            setSelectedCriterionIsValidAction({ isValid: false });

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
            selectScenarioRelatedCriterionAction({
                libraryId: uuidNIL,
            });
        }
    }

    watch(criterionLibraryEditorSelectedCriterionLibrary,()=> onCriterionLibraryEditorSelectedCriterionLibraryChanged())
    function onCriterionLibraryEditorSelectedCriterionLibraryChanged() {
        hasUnsavedChanges = hasUnsavedChangesCore(
            'criterion-library',
            criterionLibraryEditorSelectedCriterionLibrary,
            stateSelectedCriterionLibrary,
        );
    }

    function onSubmitSelectedCriterionLibrary(
        criterionLibraryEditorSelectedCriterionLibrary: CriterionLibrary,
    ) {
        criterionLibraryEditorSelectedCriterionLibrary = clone(
            criterionLibraryEditorSelectedCriterionLibrary,
        );
    }

   function  onBeforeSubmit(submit: boolean) {
        if (hasUnsavedChanges && !props.dialogData.isCallFromScenario) {
            onShowHasUnsavedChangesAlert();
        } else {
            onSubmit(submit);
        }
    }

    function onShowHasUnsavedChangesAlert() {
        hasUnsavedChangesAlertData = {
            showDialog: true,
            heading: 'Unsaved Changes',
            message:
                'The selected criterion library has unsaved changes. Click "Update Library" or "Create as New Library" to save changes.',
            choice: false,
        };
    }

    function onCloseHasUnsavedChangesAlert() {
        hasUnsavedChangesAlertData = clone(emptyAlertData);
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            if (!isNil(stateScenarioRelatedCriteria)) {
                upsertCriterionLibraryAction({
                    library: stateScenarioRelatedCriteria,
                })
                .then((id: string) => {
                    stateScenarioRelatedCriteria.value.id = id;
                    emit('submit', stateScenarioRelatedCriteria);
                });
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
