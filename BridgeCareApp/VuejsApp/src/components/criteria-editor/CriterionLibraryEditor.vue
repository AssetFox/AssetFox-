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
                        outline
                    >
                    </v-select>
                    <v-text-field
                        v-if="hasSelectedCriterionLibrary"
                        v-model="selectedCriterionLibrary.name"
                        @change="canUpdateOrCreate = true"
                    >
                        <template slot="append">
                            <v-btn
                                @click="librarySelectItemValue = null"
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
                <v-btn outline
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

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Action, State, Getter } from 'vuex-class';
import { Prop, Watch } from 'vue-property-decorator';
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
import Alert from '@/shared/modals/Alert.vue';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { hasValue } from '@/shared/utils/has-value-util';
import { getUserName } from '@/shared/utils/get-user-info';

@Component({
    components: {
        ConfirmDeleteAlert: Alert,
        CreateCriterionLibraryDialog,
        CriteriaEditor,
    },
})
export default class CriterionLibraryEditor extends Vue {
    @Prop() dialogLibraryId: string;
    @Prop() dialogIsFromScenario: boolean;
    @Prop() dialogIsFromLibrary: boolean;

    @State(state => state.criterionModule.criterionLibraries)
    stateCriterionLibraries: CriterionLibrary[];
    @State(state => state.criterionModule.selectedCriterionLibrary)
    stateSelectedCriterionLibrary: CriterionLibrary;
    @State(state => state.criterionModule.scenarioRelatedCriteria)
    scenarioRelatedCriteria: CriterionLibrary;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.criterionModule.hasPermittedAccess) hasPermittedAccess: boolean;
    @Action('getHasPermittedAccess') getHasPermittedAccessAction: any;
    @Action('getCriterionLibraries') getCriterionLibrariesAction: any;
    @Action('upsertCriterionLibrary') upsertCriterionLibraryAction: any;
    @Action('selectCriterionLibrary') selectCriterionLibraryAction: any;
    @Action('deleteCriterionLibrary') deleteCriterionLibraryAction: any;
    @Action('setSelectedCriterionIsValid')
    setSelectedCriterionIsValidAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('selectScenarioRelatedCriterion')
    selectScenarioRelatedCriterionAction: any;
    @Action('upsertSelectedScenarioRelatedCriterion')
    upsertSelectedScenarioRelatedCriterionAction: any;
    @Action('getSelectedCriterionLibrary') getSelectedCriterionLibraryAction: any;

    @Getter('getUserNameById') getUserNameByIdGetter: any;

    hasSelectedCriterionLibrary: boolean = false;
    criterionLibrarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = null;
    selectedCriterionLibrary: CriterionLibrary = clone(emptyCriterionLibrary);
    criteriaEditorData: CriteriaEditorData = {
        ...emptyCriteriaEditorData,
        isLibraryContext: true,
    };
    isLibraryContext: boolean = true;
    createCriterionLibraryDialogData: CreateCriterionLibraryDialogData = clone(
        emptyCreateCriterionLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    canUpdateOrCreate: boolean = false;
    uuidNIL: string = getBlankGuid();
    callFromScenario: boolean = false;
    callFromLibraryToEditCriterion: boolean = false;
    criteriaForScenario: string | null = null;
    selectedScenarioRelatedCriteria: CriterionLibrary = clone(
        emptyCriterionLibrary,
    );
    hasCreatedLibrary: boolean = false;
    hasLibraryEditPermission: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.getHasPermittedAccessAction();

            if (to.path.indexOf('CriterionLibraryEditor/Library') !== -1) {
                vm.librarySelectItemValue = null;
                vm.getCriterionLibrariesAction();
            } else {
                vm.isLibraryContext = false;
            }
        });
    }

    beforeDestroy() {
        if (this.isLibraryContext) {
            this.setHasUnsavedChangesAction({ value: false });
        }
    }

    @Watch('stateCriterionLibraries')
    onStateCriterionLibrariesChanged() {
        this.criterionLibrarySelectItems = this.stateCriterionLibraries
            .filter((lib: CriterionLibrary) => lib.isSingleUse == false)
            .map((library: CriterionLibrary) => ({
                text: library.name,
                value: library.id,
            }));
        if (!this.isLibraryContext && hasValue(this.librarySelectItemValue)) {
            this.getSelectedCriterionLibraryAction({
                libraryId: this.librarySelectItemValue,
            });
        }
    }

    @Watch('dialogLibraryId')
    onDialogLibraryIdChanged() {
        if (
            (!this.dialogIsFromScenario && !this.dialogIsFromLibrary) ||
            this.dialogLibraryId == this.uuidNIL
        ) {
            this.librarySelectItemValue = this.dialogLibraryId;
        }
    }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.getSelectedCriterionLibraryAction({
                libraryId: this.librarySelectItemValue,
            });
    }

    @Watch('stateSelectedCriterionLibrary')
    onStateSelectedCriterionLibraryChanged() {
        this.canUpdateOrCreate = false;
        this.selectedCriterionLibrary = clone(
            this.stateSelectedCriterionLibrary,
        );
    }

    @Watch('scenarioRelatedCriteria')
    onScenarioRelatedCriteria() {
        this.criteriaForScenario = this.dialogLibraryId;
        this.callFromScenario = this.dialogIsFromScenario;
        this.callFromLibraryToEditCriterion = this.dialogIsFromLibrary;
        this.selectedScenarioRelatedCriteria = clone(
            this.scenarioRelatedCriteria,
        );
        this.criteriaEditorData = {
            ...this.criteriaEditorData,
            mergedCriteriaExpression: this.selectedScenarioRelatedCriteria
                .mergedCriteriaExpression,
        };
    }

    @Watch('canUpdateOrCreate')
    onCanUpdateOrCreateChanged() {}

    @Watch('selectedCriterionLibrary', {deep: true})
    onSelectedCriterionLibraryChanged() {
        this.hasSelectedCriterionLibrary =
            this.selectedCriterionLibrary.id !== this.uuidNIL;

        if (this.hasSelectedCriterionLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (
            (this.callFromScenario || this.dialogIsFromLibrary) &&
            this.hasSelectedCriterionLibrary == false
        ) {
            this.criteriaEditorData = {
                ...this.criteriaEditorData,
                mergedCriteriaExpression: this.selectedScenarioRelatedCriteria
                    .mergedCriteriaExpression,
            };
        } else {
            this.criteriaEditorData = {
                ...this.criteriaEditorData,
                mergedCriteriaExpression: this.selectedCriterionLibrary
                    .mergedCriteriaExpression,
            };
        }

        if (this.isLibraryContext) {
            this.setHasUnsavedChangesAction({
                value: hasUnsavedChangesCore(
                    'criterion-library',
                    this.selectedCriterionLibrary,
                    this.stateSelectedCriterionLibrary,
                ),
            });
        } else {
            this.$emit('submit', this.selectedCriterionLibrary);
        }
    }

    mounted() {
        if (hasValue(this.stateCriterionLibraries)) {
            this.criterionLibrarySelectItems = this.stateCriterionLibraries
            .filter((lib: CriterionLibrary) => lib.isSingleUse == false)
            .map((library: CriterionLibrary) => ({
                text: library.name,
                value: library.id,
            }));

            if (
                (!this.isLibraryContext || !this.dialogIsFromLibrary) &&
                hasValue(this.librarySelectItemValue)
            ) {
                this.getSelectedCriterionLibraryAction({
                    libraryId: this.librarySelectItemValue,
            });
            }
        }
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.hasAdminAccess || (this.hasPermittedAccess && this.checkUserIsLibraryOwner());
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedCriterionLibrary.owner) == getUserName();
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedCriterionLibrary.owner);
        }
        
        return getUserName();
    }

    onShowCreateCriterionLibraryDialog(createAsNew: boolean) {
        this.createCriterionLibraryDialogData = {
            showDialog: true,
            mergedCriteriaExpression: createAsNew
                ? this.selectedCriterionLibrary.mergedCriteriaExpression != null
                    ? this.selectedCriterionLibrary.mergedCriteriaExpression
                    : ''
                : '',
        };
    }

    onSubmitCriteriaEditorResult(result: CriteriaEditorResult) {
        this.canUpdateOrCreate = result.validated;

        if (result.validated) {
            if (!this.dialogIsFromScenario && !this.dialogIsFromLibrary) {
                this.selectedCriterionLibrary = {
                    ...this.selectedCriterionLibrary,
                    mergedCriteriaExpression: result.criteria!,
                };
            } else {
                this.selectedScenarioRelatedCriteria = {
                    ...this.selectedScenarioRelatedCriteria,
                    mergedCriteriaExpression: result.criteria!,
                    isSingleUse: true,
                };
                this.upsertSelectedScenarioRelatedCriterionAction({
                    library: this.selectedScenarioRelatedCriteria,
                });
            }

            this.setSelectedCriterionIsValidAction({ isValid: true });
        } else {
            this.setSelectedCriterionIsValidAction({ isValid: false });
        }
    }

    onUpsertCriterionLibrary(criterionLibrary: CriterionLibrary) {
        this.createCriterionLibraryDialogData = clone(
            emptyCreateCriterionLibraryDialogData,
        );

        if (!isNil(criterionLibrary)) {
            // undefined dialogIsFromLibrary means, the call has come from none of the scenario related component
            if(isNil(this.dialogIsFromLibrary) || this.dialogIsFromLibrary){
                criterionLibrary.isSingleUse = false;
            }
            this.upsertCriterionLibraryAction({
                library: criterionLibrary,
            }).then(() => (this.librarySelectItemValue = criterionLibrary.id));

            this.hasCreatedLibrary = true;
        }
    }

    onShowConfirmDeleteAlert() {
        this.confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.deleteCriterionLibraryAction({
                libraryId: this.selectedCriterionLibrary.id,
            }).then(() => (this.librarySelectItemValue = null));
        }
    }
}
</script>
