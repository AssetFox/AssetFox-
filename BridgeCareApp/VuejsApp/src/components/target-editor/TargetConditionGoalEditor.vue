<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="
                            onShowCreateTargetConditionGoalLibraryDialog(false)
                        "
                        class="ara-blue-bg white--text"
                        v-show="!hasScenario"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        :items="librarySelectItems"
                        label="Select a Target Condition Goal Library"
                        outline
                        v-if="!hasSelectedLibrary || hasScenario"
                        v-model="librarySelectItemValue"
                    >
                    </v-select>
                    <v-text-field
                        label="Library Name"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedTargetConditionGoalLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
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
                    <div v-if="hasSelectedLibrary && !hasScenario">
                        Owner:
                        {{
                            selectedTargetConditionGoalLibrary.owner
                                ? selectedTargetConditionGoalLibrary.owner
                                : '[ No Owner ]'
                        }}
                    </div>
                    <v-checkbox
                        class="sharing"
                        label="Shared"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedTargetConditionGoalLibrary.isShared"
                    />
                </v-flex>
            </v-layout>
            <v-flex v-show="hasSelectedLibrary || hasScenario" xs3>
                <v-btn
                    @click="showCreateTargetConditionGoalDialog = true"
                    class="ara-blue-bg white--text"
                    >Add</v-btn
                >
                <v-btn
                    :disabled="selectedTargetConditionGoalIds.length === 0"
                    @click="onRemoveTargetConditionGoals"
                    class="ara-orange-bg white--text"
                >
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <div class="targets-data-table">
                <v-data-table
                    :headers="targetConditionGoalGridHeaders"
                    :items="targetConditionGoalGridData"
                    class="elevation-1 fixed-header v-table__overflow"
                    item-key="id"
                    select-all
                    v-model="selectedGridRows"
                >
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-checkbox
                                hide-details
                                primary
                                v-model="props.selected"
                            ></v-checkbox>
                        </td>
                        <td v-for="header in targetConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog
                                    v-if="header.value !== 'criterionLibrary'"
                                    :return-value.sync="
                                        props.item[header.value]
                                    "
                                    @save="
                                        onEditTargetConditionGoalProperty(
                                            props.item,
                                            header.value,
                                            props.item[header.value],
                                        )
                                    "
                                    large
                                    lazy
                                    persistent
                                >
                                    <v-text-field
                                        v-if="header.value === 'year'"
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                    />
                                    <v-text-field
                                        v-else
                                        readonly
                                        single-line
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                    <template slot="input">
                                        <v-select
                                            v-if="header.value === 'attribute'"
                                            :items="numericAttributeNames"
                                            label="Select an Attribute"
                                            v-model="props.item.attribute"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <v-text-field
                                            v-if="header.value === 'year'"
                                            label="Edit"
                                            single-line
                                            :mask="'####'"
                                            v-model.number="
                                                props.item[header.value]
                                            "
                                        />
                                        <v-text-field
                                            v-if="header.value === 'target'"
                                            label="Edit"
                                            single-line
                                            :mask="'##########'"
                                            v-model.number="
                                                props.item[header.value]
                                            "
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                        <v-text-field
                                            v-if="header.value === 'name'"
                                            label="Edit"
                                            single-line
                                            v-model="props.item[header.value]"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />
                                    </template>
                                </v-edit-dialog>

                                <v-layout
                                    v-else
                                    align-center
                                    row
                                    style="flex-wrap:nowrap"
                                >
                                    <v-menu
                                        bottom
                                        min-height="500px"
                                        min-width="500px"
                                    >
                                        <template slot="activator">
                                            <v-text-field
                                                readonly
                                                class="sm-txt"
                                                :value="
                                                    props.item.criterionLibrary
                                                        .mergedCriteriaExpression
                                                "
                                            />
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    :value="
                                                        props.item
                                                            .criterionLibrary
                                                            .mergedCriteriaExpression
                                                    "
                                                    full-width
                                                    no-resize
                                                    outline
                                                    readonly
                                                    rows="5"
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                                        @click="
                                            onShowCriterionLibraryEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </v-layout>
                            </div>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea
                        label="Description"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedTargetConditionGoalLibrary.description"
                        @input='selectedTargetConditionGoalLibrary = {...selectedTargetConditionGoalLibrary, description: $event}'
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout justify-end row>
                <v-btn
                    @click="onUpsertScenarioTargetConditionGoals"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                >
                    Save
                </v-btn>
                <v-btn
                    @click="onUpsertTargetConditionGoalLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                >
                    Update Library
                </v-btn>
                <v-btn
                    @click="onShowCreateTargetConditionGoalLibraryDialog(true)"
                    class="ara-blue-bg white--text"
                    :disabled="disableCrudButtons()"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onShowConfirmDeleteAlert"
                    class="ara-orange-bg white--text"
                    v-show="!hasScenario"
                    :disabled="!hasLibraryEditPermission"
                >
                    Delete Library
                </v-btn>
                <v-btn :disabled='!hasUnsavedChanges'
                    @click="onDiscardChanges"
                    class="ara-orange-bg white--text"
                    v-show="hasSelectedLibrary || hasScenario"
                >
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateTargetConditionGoalLibraryDialog
            :dialogData="createTargetConditionGoalLibraryDialogData"
            @submit="onSubmitCreateTargetConditionGoalLibraryDialogResult"
        />

        <CreateTargetConditionGoalDialog
            :showDialog="showCreateTargetConditionGoalDialog"
            :currentNumberOfTargetConditionGoals="
                selectedTargetConditionGoalLibrary.targetConditionGoals.length
            "
            @submit="onAddTargetConditionGoal"
        />

        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onEditTargetConditionGoalCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, State } from 'vuex-class';
import {
    emptyTargetConditionGoal,
    emptyTargetConditionGoalLibrary,
    TargetConditionGoal,
    TargetConditionGoalLibrary,
} from '@/shared/models/iAM/target-condition-goal';
import {
    clone,
    contains,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import CreateTargetConditionGoalDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalDialog.vue';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import {
    CreateTargetConditionGoalLibraryDialogData,
    emptyCreateTargetConditionGoalLibraryDialogData,
} from '@/shared/models/modals/create-target-condition-goal-library-dialog-data';
import CreateTargetConditionGoalLibraryDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalLibraryDialog.vue';
import { Attribute } from '@/shared/models/iAM/attribute';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import {
    CriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';

@Component({
    components: {
        CriterionLibraryEditorDialog,
        CreateTargetConditionGoalLibraryDialog,
        CreateTargetConditionGoalDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class TargetConditionGoalEditor extends Vue {
    @State(
        state => state.targetConditionGoalModule.targetConditionGoalLibraries,
    )
    stateTargetConditionGoalLibraries: TargetConditionGoalLibrary[];
    @State(
        state =>
            state.targetConditionGoalModule.selectedTargetConditionGoalLibrary,
    )
    stateSelectedTargetConditionLibrary: TargetConditionGoalLibrary;
    @State(state => state.attributeModule.numericAttributeNames)
    stateNumericAttributes: Attribute[];
    @State(
        state => state.targetConditionGoalModule.scenarioTargetConditionGoals,
    )
    stateScenarioTargetConditionGoals: TargetConditionGoal[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getTargetConditionGoalLibraries') getTargetConditionGoalLibrariesAction: any;
    @Action('selectTargetConditionGoalLibrary') selectTargetConditionGoalLibraryAction: any;
    @Action('upsertTargetConditionGoalLibrary') upsertTargetConditionGoalLibraryAction: any;
    @Action('deleteTargetConditionGoalLibrary') deleteTargetConditionGoalLibraryAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioTargetConditionGoals') getScenarioTargetConditionGoalsAction: any;
    @Action('upsertScenarioTargetConditionGoals') upsertScenarioTargetConditionGoalsAction: any;

    @Getter('getNumericAttributes') getNumericAttributesGetter: any;

    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = '';
    selectedTargetConditionGoalLibrary: TargetConditionGoalLibrary = clone(
        emptyTargetConditionGoalLibrary,
    );
    hasSelectedLibrary: boolean = false;
    targetConditionGoalGridData: TargetConditionGoal[] = [];
    targetConditionGoalGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Target',
            value: 'target',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Year',
            value: 'year',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Criteria',
            value: 'criterionLibrary',
            align: 'left',
            sortable: false,
            class: '',
            width: '50%',
        },
    ];
    numericAttributeNames: string[] = [];
    selectedGridRows: TargetConditionGoal[] = [];
    selectedTargetConditionGoalIds: string[] = [];
    selectedTargetConditionGoalForCriteriaEdit: TargetConditionGoal = clone(
        emptyTargetConditionGoal,
    );
    showCreateTargetConditionGoalDialog: boolean = false;
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    createTargetConditionGoalLibraryDialogData: CreateTargetConditionGoalLibraryDialogData = clone(
        emptyCreateTargetConditionGoalLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    currentUrl: string = window.location.href;
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getTargetConditionGoalLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.TargetConditionGoal) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioTargetConditionGoalsAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateTargetConditionGoalLibraries')
    onStateTargetConditionGoalLibrariesChanged() {
        this.librarySelectItems = this.stateTargetConditionGoalLibraries.map(
            (library: TargetConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectTargetConditionGoalLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
    }

    @Watch('stateSelectedTargetConditionLibrary')
    onStateSelectedTargetConditionGoalLibraryChanged() {
        this.selectedTargetConditionGoalLibrary = clone(
            this.stateSelectedTargetConditionLibrary,
        );
    }

    @Watch('selectedTargetConditionGoalLibrary', {deep: true})
    onSelectedTargetConditionGoalLibraryChanged() {
        this.hasSelectedLibrary = this.selectedTargetConditionGoalLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (this.hasScenario) {
            this.targetConditionGoalGridData = this.selectedTargetConditionGoalLibrary.targetConditionGoals
                .map((goal: TargetConditionGoal) => ({
                    ...goal,
                    id: getNewGuid(),
                }));
        } else {
            this.targetConditionGoalGridData = clone(this.selectedTargetConditionGoalLibrary.targetConditionGoals);
        }

        /*if (this.numericAttributeNames.length === 0) {
            this.numericAttributeNames = getPropertyValues(
                'name',
                this.getNumericAttributesGetter,
            );
        }*/
    }

    @Watch('selectedGridRows')
    onSelectedGridRowsChanged() {
        this.selectedTargetConditionGoalIds = getPropertyValues('id', this.selectedGridRows,) as string[];
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
    }

    @Watch('stateScenarioTargetConditionGoals')
    onStateScenarioTargetConditionGoalsChanged() {
        if (this.hasScenario) {
            this.targetConditionGoalGridData = clone(this.stateScenarioTargetConditionGoals);
        }
    }

    @Watch('targetConditionGoalGridData')
    onTargetConditionGoalGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.targetConditionGoalGridData, this.stateScenarioTargetConditionGoals)
            : hasUnsavedChangesCore('',
                {...clone(this.selectedTargetConditionGoalLibrary), targetConditionGoals: clone(this.targetConditionGoalGridData)},
                this.stateSelectedTargetConditionLibrary);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedTargetConditionGoalLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedTargetConditionGoalLibrary.owner) == getUserName();
    }

    onShowCreateTargetConditionGoalLibraryDialog(createAsNewLibrary: boolean) {
        this.createTargetConditionGoalLibraryDialogData = {
            showDialog: true,
            targetConditionGoals: createAsNewLibrary
                ? this.targetConditionGoalGridData
                : [],
        };
    }

    onSubmitCreateTargetConditionGoalLibraryDialogResult(library: TargetConditionGoalLibrary) {
        this.createTargetConditionGoalLibraryDialogData = clone(emptyCreateTargetConditionGoalLibraryDialogData);

        if (!isNil(library)) {
            this.upsertTargetConditionGoalLibraryAction({ library: library });
        }
    }

    onAddTargetConditionGoal(newTargetConditionGoal: TargetConditionGoal) {
        this.showCreateTargetConditionGoalDialog = false;

        if (!isNil(newTargetConditionGoal)) {
            this.targetConditionGoalGridData = prepend(
                newTargetConditionGoal,
                this.targetConditionGoalGridData,
            );
        }
    }

    onEditTargetConditionGoalProperty(
        targetConditionGoal: TargetConditionGoal,
        property: string,
        value: any,
    ) {
        this.targetConditionGoalGridData = update(
            findIndex(
                propEq('id', targetConditionGoal.id),
                this.targetConditionGoalGridData,
            ),
            setItemPropertyValue(
                property,
                value,
                targetConditionGoal,
            ) as TargetConditionGoal,
            this.targetConditionGoalGridData,
        );
    }

    onShowCriterionLibraryEditorDialog(targetConditionGoal: TargetConditionGoal) {
        this.selectedTargetConditionGoalForCriteriaEdit = clone(
            targetConditionGoal,
        );

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: targetConditionGoal.criterionLibrary.id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario,
        };
    }

    onEditTargetConditionGoalCriterionLibrary(criterionLibrary: CriterionLibrary,) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedTargetConditionGoalForCriteriaEdit.id !== this.uuidNIL) {
            this.targetConditionGoalGridData = update(
                findIndex(propEq('id', this.selectedTargetConditionGoalForCriteriaEdit.id), this.targetConditionGoalGridData),
                {...this.selectedTargetConditionGoalForCriteriaEdit, criterionLibrary: criterionLibrary},
                this.targetConditionGoalGridData);
        }

        this.selectedTargetConditionGoalForCriteriaEdit = clone(emptyTargetConditionGoal);
    }

    onUpsertTargetConditionGoalLibrary() {
        const library: TargetConditionGoalLibrary = {
            ...clone(this.selectedTargetConditionGoalLibrary), targetConditionGoals: clone(this.targetConditionGoalGridData)
        };
        var localObject = clone(library);
            localObject.targetConditionGoals = clone(this.targetConditionGoalGridData);
            this.upsertTargetConditionGoalLibraryAction({ library: localObject });
    }

    onUpsertScenarioTargetConditionGoals() {
        this.upsertScenarioTargetConditionGoalsAction({
            scenarioTargetConditionGoals: this.targetConditionGoalGridData,
            scenarioId: this.selectedScenarioId,
        }).then(() => (this.librarySelectItemValue = null));
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.targetConditionGoalGridData = clone(this.stateScenarioTargetConditionGoals);
            }
        });
    }

    onRemoveTargetConditionGoals() {
        this.targetConditionGoalGridData = this.targetConditionGoalGridData.filter((goal: TargetConditionGoal) =>
            !contains(goal.id, this.selectedTargetConditionGoalIds),
        );
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
            this.librarySelectItemValue = null;
            this.deleteTargetConditionGoalLibraryAction({
                libraryId: this.selectedTargetConditionGoalLibrary.id,
            });
        }
    }

    disableCrudButtons() {
        const dataIsValid: boolean = this.targetConditionGoalGridData.every(
            (targetGoal: TargetConditionGoal) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        targetGoal.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        targetGoal.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedTargetConditionGoalLibrary.name,
                ) === true &&
                dataIsValid
            );
        }

        this.disableCrudButtonsResult = !dataIsValid;
        return !dataIsValid;
    }
}
</script>

<style>
.targets-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.targets-data-table .v-menu--inline,
.target-criteria-output {
    width: 100%;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
