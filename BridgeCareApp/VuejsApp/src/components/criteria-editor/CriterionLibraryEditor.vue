<template>
    <v-layout column>
        <v-flex v-if="false">
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="onShowCreateCriterionLibraryDialog(false)"
                        class="ara-blue-bg white--text"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        v-model="librarySelectItemValue"
                        v-if="!hasSelectedCriterionLibrary"
                        :items="criterionLibrarySelectItems"
                        label="Select a Criteria Library"
                        variant="outlined"
                    >
                    </v-select>
                    <v-text-field
                        v-if="hasSelectedCriterionLibrary"
                        v-model="selectedCriterionLibrary.name"
                        @change="canUpdateOrCreate = true"
                    >
                        <template slot="append-inner">
                            <v-btn
                                @click="librarySelectItemValue = ''"
                                class="ara-orange"
                                icon
                            >
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedCriterionLibrary">
                        Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                    </div>
                    <v-checkbox
                        v-if="hasSelectedCriterionLibrary"
                        v-model="selectedCriterionLibrary.isShared"
                        class="sharing"
                        label="Shared"
                        @change="canUpdateOrCreate = true"
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedCriterionLibrary || callFromScenario || callFromLibraryToEditCriterion" />
        <v-flex v-show="hasSelectedCriterionLibrary || callFromScenario || callFromLibraryToEditCriterion">
            <v-layout justify-center>
                <v-flex xs10>
                    <CriteriaEditor
                        :criteriaEditorData="criteriaEditorData"
                        @submitCriteriaEditorResult="
                            onSubmitCriteriaEditorResult
                        "
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedCriterionLibrary || callFromScenario || callFromLibraryToEditCriterion" />
        <v-flex v-show="hasSelectedCriterionLibrary">
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea
                        v-model="selectedCriterionLibrary.description"
                        label="Description"
                        no-resize
                        outline
                        rows="4"
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex>
            <v-layout justify-end row v-show="hasSelectedCriterionLibrary">
                <v-btn
                    @click="onUpsertCriterionLibrary(selectedCriterionLibrary)"
                    class="ara-blue-bg white--text"
                    :disabled="!canUpdateOrCreate || !hasLibraryEditPermission"
                >
                    Update Library
                </v-btn>
                <v-btn
                    @click="onShowCreateCriterionLibraryDialog(true)"
                    class="ara-blue-bg white--text"
                    :disabled="!canUpdateOrCreate"
                >
                    Create as New Library
                </v-btn>
                <v-btn variant = "outlined"
                    @click="onShowConfirmDeleteAlert"
                    class="ara-orange-bg white--text"
                    :disabled="!hasLibraryEditPermission"
                >
                    Delete Library
                </v-btn>
            </v-layout>
        </v-flex>

        <CreateCriterionLibraryDialog
            :dialogData="createCriterionLibraryDialogData"
            @submit="onUpsertCriterionLibrary"
        />

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />
    </v-layout>
</template>

<script lang="ts" setup>
import Vue, { shallowRef } from 'vue';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import CriteriaEditor from '@/shared/components/CriteriaEditor.vue';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    CriteriaEditorData,
    CriteriaEditorResult,
    CriterionLibrary,
    emptyCriteriaEditorData,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';

import { clone, isNil } from 'ramda';
import {
    hasUnsavedChanges,
    hasUnsavedChangesCore,
} from '@/shared/utils/has-unsaved-changes-helper';
import {
    CreateCriterionLibraryDialogData,
    emptyCreateCriterionLibraryDialogData,
} from '@/shared/models/modals/create-criterion-library-dialog-data';
import CreateCriterionLibraryDialog from '@/components/criteria-editor/criteria-editor-dialogs/CreateCriterionLibraryDialog.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import ConfirmDeleteAlert from '@/shared/modals/Alert.vue';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { getUserName } from '@/shared/utils/get-user-info';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let dialogLibraryId = shallowRef<string>('');
    let dialogIsFromScenario = shallowRef<boolean>();
    let dialogIsFromLibrary: boolean;
    let store = useStore();

    let stateCriterionLibraries = ref<CriterionLibrary[]>(store.state.criterionModule.criterionLibraries)
    let stateSelectedCriterionLibrary = ref<CriterionLibrary>(store.state.criterionModule.selectedCriterionLibrary)
    let scenarioRelatedCriteria = ref<CriterionLibrary>(store.state.criterionModule.scenarioRelatedCriteria)
    let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess)
    let hasPermittedAccess = ref<boolean>(store.state.criterionModule.hasPermittedAccess)

    async function getHasPermittedAccessAction(payload?: any): Promise<any> {
        await store.dispatch('getHasPermittedAccess');
    }

    async function getCriterionLibrariesAction(payload?: any): Promise<any> {
        await store.dispatch('getCriterionLibraries');
    }

    async function upsertCriterionLibraryAction(payload?: any): Promise<any> {
        await store.dispatch('upsertCriterionLibrary');
    }

    async function selectCriterionLibraryAction(payload?: any): Promise<any> {
        await store.dispatch('selectCriterionLibrary');
    }

    async function deleteCriterionLibraryAction(payload?: any): Promise<any> {
        await store.dispatch('deleteCriterionLibrary');
    }

    async function setSelectedCriterionIsValidAction(payload?: any): Promise<any> {
        await store.dispatch('setSelectedCriterionIsValid');
    }

    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {
        await store.dispatch('setHasUnsavedChanges');
    }

    async function selectScenarioRelatedCriterionAction(payload?: any): Promise<any> {
        await store.dispatch('selectScenarioRelatedCriterion');
    }

    async function upsertSelectedScenarioRelatedCriterionAction(payload?: any): Promise<any> {
        await store.dispatch('upsertSelectedScenarioRelatedCriterion');
    }

    async function getSelectedCriterionLibraryAction(payload?: any): Promise<any> {
        await store.dispatch('getSelectedCriterionLibrary');
    }

    async function getUserNameByIdGetter(payload?: any): Promise<any> {
        await store.dispatch('getUserNameById');
    }


    let hasSelectedCriterionLibrary: boolean = false;
    let criterionLibrarySelectItems: SelectItem[] = [];
    let librarySelectItemValue = shallowRef<string>('');
    let selectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
    let criteriaEditorData: CriteriaEditorData = {
        ...emptyCriteriaEditorData,
        isLibraryContext: true,
    };
    let isLibraryContext: boolean = true;
    const props = defineProps<{createCriterionLibraryDialogData: CreateCriterionLibraryDialogData}>()
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let canUpdateOrCreate = shallowRef<boolean>(false);
    let uuidNIL: string = getBlankGuid();
    let callFromScenario = shallowRef<boolean>();
    let callFromLibraryToEditCriterion: boolean = false;
    let criteriaForScenario = shallowRef<string>('');
    let selectedScenarioRelatedCriteria: CriterionLibrary = clone(
        emptyCriterionLibrary,
    );
    let hasCreatedLibrary = shallowRef<boolean>(false);
    let hasLibraryEditPermission = shallowRef<boolean>(false);
    const emit = defineEmits(['submit'])
    const $vuetify = inject('$vuetify') as any
    const $router = useRouter();
    const $statusHub = inject('$statusHub') as any
    const $config = inject('$config') as any

   created();
   function created() {
         ((vm: any) => {
            vm.getHasPermittedAccessAction();
            if ($router.currentRoute.value.path.indexOf('CriterionLibraryEditor/Library') !== -1) {
                vm.librarySelectItemValue = null;
                vm.getCriterionLibrariesAction();
            } else {
                vm.isLibraryContext = false;
            }
        });
    }

    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        if (isLibraryContext) {
            setHasUnsavedChangesAction({ value: false });
        }
    }


    watch(stateCriterionLibraries, () => onStateCriterionLibrariesChanged)
    function onStateCriterionLibrariesChanged() {
        criterionLibrarySelectItems = stateCriterionLibraries.value
            .filter((lib: CriterionLibrary) => lib.isSingleUse == false)
            .map((library: CriterionLibrary) => ({
                text: library.name,
                value: library.id,
            }));
        if (!isLibraryContext && hasValue(librarySelectItemValue)) {
            getSelectedCriterionLibraryAction({
                libraryId: librarySelectItemValue,
            });
        }
    }

    watch(dialogLibraryId, () => onDialogLibraryIdChanged) 
    function onDialogLibraryIdChanged(){
        if (
            (!dialogIsFromScenario && !dialogIsFromLibrary) ||
            dialogLibraryId.value == uuidNIL
        ) {
            librarySelectItemValue = dialogLibraryId;
        }
    }

    watch(librarySelectItemValue, () => onLibrarySelectItemValueChanged) 
    function onLibrarySelectItemValueChanged(){
            getSelectedCriterionLibraryAction({
                libraryId: librarySelectItemValue.value
            });
    }

    watch(stateSelectedCriterionLibrary, () => onStateSelectedCriterionLibraryChanged)
    function onStateSelectedCriterionLibraryChanged(){
        canUpdateOrCreate.value = false;
        selectedCriterionLibrary = clone(
            stateSelectedCriterionLibrary.value
        );
    }

    watch(scenarioRelatedCriteria, onScenarioRelatedCriteria)
    function onScenarioRelatedCriteria() {
        criteriaForScenario = dialogLibraryId;
        callFromScenario = dialogIsFromScenario;
        callFromLibraryToEditCriterion = dialogIsFromLibrary;
        selectedScenarioRelatedCriteria = clone(
            scenarioRelatedCriteria.value
        );
        criteriaEditorData = {
            ...criteriaEditorData,
            mergedCriteriaExpression: selectedScenarioRelatedCriteria
                .mergedCriteriaExpression,
        };
    }

    watch(canUpdateOrCreate, () => onCanUpdateOrCreateChanged) 
    function onCanUpdateOrCreateChanged(){


    }

    watch(selectedCriterionLibrary, () => onSelectedCriterionLibraryChanged)
    function onSelectedCriterionLibraryChanged() {
        hasSelectedCriterionLibrary =
            selectedCriterionLibrary.id !== uuidNIL;

        if (hasSelectedCriterionLibrary) {
            checkLibraryEditPermission();
            hasCreatedLibrary.value = false;
        }

        if (
            (callFromScenario || dialogIsFromLibrary) &&
            hasSelectedCriterionLibrary == false
        ) {
            criteriaEditorData = {
                ...criteriaEditorData,
                mergedCriteriaExpression: selectedScenarioRelatedCriteria
                    .mergedCriteriaExpression,
            };
        } else {
            criteriaEditorData = {
                ...criteriaEditorData,
                mergedCriteriaExpression: selectedCriterionLibrary
                    .mergedCriteriaExpression,
            };
        }

        if (isLibraryContext) {
            setHasUnsavedChangesAction({
                value: hasUnsavedChangesCore(
                    'criterion-library',
                    selectedCriterionLibrary,
                    stateSelectedCriterionLibrary,
                ),
            });
        } else {
           emit('submit', selectedCriterionLibrary);
        }
    }

   onMounted(() => mounted());
   function mounted() {
        if (hasValue(stateCriterionLibraries)) {
            criterionLibrarySelectItems = stateCriterionLibraries.value
            .filter((lib: CriterionLibrary) => lib.isSingleUse == false)
            .map((library: CriterionLibrary) => ({
                text: library.name,
                value: library.id,
            }));

            if (
                (!isLibraryContext || !dialogIsFromLibrary) &&
                hasValue(librarySelectItemValue)
            ) {
                getSelectedCriterionLibraryAction({
                    libraryId: librarySelectItemValue
            });
            }
        }
    }

    function checkLibraryEditPermission() {
        hasLibraryEditPermission = hasAdminAccess || (hasPermittedAccess && checkUserIsLibraryOwner());
    }

    async function checkUserIsLibraryOwner() {
        return await getUserNameByIdGetter(selectedCriterionLibrary.owner) == getUserName();
    }

    async function getOwnerUserName(): Promise<string> {

        if (!hasCreatedLibrary) {
        return await getUserNameByIdGetter(selectedCriterionLibrary.owner);
        }
        
        return getUserName();
    }

    function onShowCreateCriterionLibraryDialog(createAsNew: boolean) {

        props.createCriterionLibraryDialogData.showDialog = true;
        props.createCriterionLibraryDialogData.mergedCriteriaExpression = createAsNew ? selectedCriterionLibrary.mergedCriteriaExpression != null
                    ? selectedCriterionLibrary.mergedCriteriaExpression
                    : ''
                : '';
    }

    function onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        canUpdateOrCreate.value = result.validated;

        if (result.validated) {
            if (!dialogIsFromScenario && !dialogIsFromLibrary) {
                selectedCriterionLibrary = {
                    ...selectedCriterionLibrary,
                    mergedCriteriaExpression: result.criteria!,
                };
            } else {
                selectedScenarioRelatedCriteria = {
                    ...selectedScenarioRelatedCriteria,
                    mergedCriteriaExpression: result.criteria!,
                    isSingleUse: true,
                };
                upsertSelectedScenarioRelatedCriterionAction({
                    library: selectedScenarioRelatedCriteria,
                });
            }

            setSelectedCriterionIsValidAction({ isValid: true });
        } else {
            setSelectedCriterionIsValidAction({ isValid: false });
        }
    }

    function onUpsertCriterionLibrary(criterionLibrary: CriterionLibrary) {
        props.createCriterionLibraryDialogData.showDialog = clone(emptyCreateCriterionLibraryDialogData.showDialog);

        if (!isNil(criterionLibrary)) {
            // undefined dialogIsFromLibrary means, the call has come from none of the scenario related component
            if(isNil(dialogIsFromLibrary) || dialogIsFromLibrary){
                criterionLibrary.isSingleUse = false;
            }
            upsertCriterionLibraryAction({
                library: criterionLibrary,
            }).then(() => (librarySelectItemValue.value = criterionLibrary.id));

            hasCreatedLibrary.value = true;
        }
    }

    function onShowConfirmDeleteAlert() {
        confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    function onSubmitConfirmDeleteAlertResult(submit: boolean) {
        confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            deleteCriterionLibraryAction({
                libraryId: selectedCriterionLibrary.id,
            }).then(() => (librarySelectItemValue.value = ''));
        }
    }

</script>
