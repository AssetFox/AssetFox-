<template>
    <v-dialog persistent maximizable v-model="showDialogComputed" class="criterion-library-editor-dialog">
        <v-card>
            <v-card-text>
                <v-row justify-center column>
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
                        :disabled="(!dialogData.isCallFromScenario && !dialogData.isCriterionForLibrary) || !stateSelectedCriterionIsValid
                        "
                        class="ghd-blue-bg ghd-white ghd-button-text"
                        variant = "flat"
                        @click="onBeforeSubmit(true)"
                    >
                        Save
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>

        <HasUnsavedChangesAlert
            :dialogData="hasUnsavedChangesAlertData"
            @submit="onCloseHasUnsavedChangesAlert"
        />
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
import { CriterionLibraryEditorDialogData } from '../models/modals/criterion-library-editor-dialog-data';
import {
    CriterionLibrary,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasValue } from '@/shared/utils/has-value-util';
import CriterionLibraryEditor from '@/components/criteria-editor/CriterionLibraryEditor.vue';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { ap, clone, isNil } from 'ramda';
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
    let showDialogComputed = computed(() => props.dialogData.showDialog);
    let stateCriterionLibraries = computed<CriterionLibrary[]>(() => store.state.criterionModule.criterionLibraries);
    let stateSelectedCriterionLibrary = computed<CriterionLibrary>(() => store.state.criterionModule.selectedCriterionLibrary);
    let stateSelectedCriterionIsValid = computed<boolean>(() => store.state.criterionModule.selectedCriterionIsValid);
    let stateScenarioRelatedCriteria = computed<CriterionLibrary>(() => store.state.criterionModule.scenarioRelatedCriteria);
    async function getCriterionLibrariesAction(payload?: any): Promise<any> {await store.dispatch('getCriterionLibraries', payload);}
    async function setSelectedCriterionIsValidAction(payload?: any): Promise<any> {await store.dispatch('setSelectedCriterionIsValid', payload);}
    async function selectScenarioRelatedCriterionAction(payload?: any): Promise<any> {await store.dispatch('selectScenarioRelatedCriterion', payload);}
    async function selectCriterionLibraryAction(payload?: any): Promise<any> {await store.dispatch('selectCriterionLibrary', payload);}
    async function upsertCriterionLibraryAction(payload?: any): Promise<any> {await store.dispatch('upsertCriterionLibrary', payload);}
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
            stateSelectedCriterionLibrary.value,
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
            if (!isNil(stateScenarioRelatedCriteria.value)) {
                upsertCriterionLibraryAction({
                    library: stateScenarioRelatedCriteria.value,
                })
                .then((id: string) => {
                    stateScenarioRelatedCriteria.value.id = id;
                    emit('submit', stateScenarioRelatedCriteria.value);
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
