<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="
                            onShowCreateDeficientConditionGoalLibraryDialog(
                                false,
                            )
                        "
                        class="ara-blue-bg white--text"
                        v-show="!hasScenario"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        :items="librarySelectItems"
                        label="Select a Deficient Condition Goal Library"
                        outline
                        v-if="!hasSelectedLibrary || hasScenario"
                        v-model="librarySelectItemValue"
                    >
                    </v-select>
                    <v-text-field
                        label="Library Name"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedDeficientConditionGoalLibrary.name"
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
                    <div v-if='hasSelectedLibrary && !hasScenario'>
                        Owner: {{ getOwnerUserName() || '[ No Owner ]' }}
                    </div>
                    <v-checkbox
                        class="sharing"
                        label="Shared"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedDeficientConditionGoalLibrary.isShared"
                    />
                </v-flex>
            </v-layout>
            <v-flex v-show="hasSelectedLibrary || hasScenario" xs3>
                <v-btn
                    @click="showCreateDeficientConditionGoalDialog = true"
                    class="ara-blue-bg white--text"
                    >Add</v-btn
                >
                <v-btn
                    :disabled="selectedDeficientConditionGoalIds.length === 0"
                    @click="onRemoveSelectedDeficientConditionGoals"
                    class="ara-orange-bg white--text"
                >
                    Delete
                </v-btn>
            </v-flex>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <div class="deficients-data-table">
                <v-data-table
                    :headers="deficientConditionGoalGridHeaders"
                    :items="deficientConditionGoalGridData"
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
                        <td v-for="header in deficientConditionGoalGridHeaders">
                            <div>
                                <v-edit-dialog
                                    v-if="header.value !== 'criterionLibrary'"
                                    :return-value.sync="
                                        props.item[header.value]
                                    "
                                    @save="
                                        onEditDeficientConditionGoalProperty(
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
                                        v-if="
                                            header.value !==
                                                'allowedDeficientPercentage'
                                        "
                                        readonly
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />

                                    <v-text-field
                                        v-if="
                                            header.value ===
                                                'allowedDeficientPercentage'
                                        "
                                        readonly
                                        class="sm-txt"
                                        :value="props.item[header.value]"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                            rules[
                                                'generalRules'
                                            ].valueIsWithinRange(
                                                props.item[header.value],
                                                [0, 100],
                                            ),
                                        ]"
                                    />

                                    <template slot="input">
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

                                        <v-select
                                            v-if="header.value === 'attribute'"
                                            :items="numericAttributeNames"
                                            label="Select an Attribute"
                                            v-model="props.item[header.value]"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        >
                                        </v-select>

                                        <v-text-field
                                            v-if="
                                                header.value ===
                                                    'deficientLimit'
                                            "
                                            label="Edit"
                                            single-line
                                            v-model="props.item[header.value]"
                                            :mask="'##########'"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                            ]"
                                        />

                                        <v-text-field
                                            v-if="
                                                header.value ===
                                                    'allowedDeficientPercentage'
                                            "
                                            label="Edit"
                                            single-line
                                            v-model.number="
                                                props.item[header.value]
                                            "
                                            :mask="'###'"
                                            :rules="[
                                                rules['generalRules']
                                                    .valueIsNotEmpty,
                                                rules[
                                                    'generalRules'
                                                ].valueIsWithinRange(
                                                    props.item[header.value],
                                                    [0, 100],
                                                ),
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
                        v-model="selectedDeficientConditionGoalLibrary.description"
                        @input='selectedDeficientConditionGoalLibrary = {...selectedDeficientConditionGoalLibrary, description: $event}'
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout justify-end row>
                <v-btn
                    @click="onUpsertScenarioDeficientConditionGoals"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                >
                    Save
                </v-btn>
                <v-btn
                    @click="onUpsertDeficientConditionGoalLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                    :disabled="disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges"
                >
                    Update Library
                </v-btn>
                <v-btn
                    @click="onShowCreateDeficientConditionGoalLibraryDialog(true)"
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
                <v-btn
                    @click="onDiscardChanges"
                    class="ara-orange-bg white--text"
                    v-show="hasSelectedLibrary || hasScenario"
                    :disabled="!hasUnsavedChanges"
                >
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmBeforeDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onSubmitConfirmDeleteAlertResult"
        />

        <CreateDeficientConditionGoalLibraryDialog
            :dialogData="createDeficientConditionGoalLibraryDialogData"
            @submit="onSubmitCreateDeficientConditionGoalLibraryDialogResult"
        />

        <CreateDeficientConditionGoalDialog
            :showDialog="showCreateDeficientConditionGoalDialog"
            :currentNumberOfDeficientConditionGoals="
                selectedDeficientConditionGoalLibrary.deficientConditionGoals
                    .length
            "
            @submit="onAddDeficientConditionGoal"
        />

        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onEditDeficientConditionGoalCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, State } from 'vuex-class';
import {
    DeficientConditionGoal,
    DeficientConditionGoalLibrary,
    emptyDeficientConditionGoal,
    emptyDeficientConditionGoalLibrary,
} from '@/shared/models/iAM/deficient-condition-goal';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
    clone,
    contains,
    findIndex,
    isNil,
    prepend,
    propEq,
    update,
} from 'ramda';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import CreateDeficientConditionGoalDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalDialog.vue';
import {
    CreateDeficientConditionGoalLibraryDialogData,
    emptyCreateDeficientConditionGoalLibraryDialogData,
} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import CreateDeficientConditionGoalLibraryDialog from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalLibraryDialog.vue';
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
    getAppliedLibraryId,
    hasAppliedLibrary,
} from '@/shared/utils/library-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { getUserName } from '@/shared/utils/get-user-info';

@Component({
    components: {
        CreateDeficientConditionGoalLibraryDialog,
        CreateDeficientConditionGoalDialog,
        CriterionLibraryEditorDialog,
        ConfirmBeforeDeleteAlert: Alert,
    },
})
export default class DeficientConditionGoalEditor extends Vue {
    @State(
        state =>
            state.deficientConditionGoalModule.deficientConditionGoalLibraries,
    )
    stateDeficientConditionGoalLibraries: DeficientConditionGoalLibrary[];
    @State(
        state =>
            state.deficientConditionGoalModule
                .selectedDeficientConditionGoalLibrary,
    )
    stateSelectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary;
    @State(state => state.attributeModule.numericAttributeNames)
    stateNumericAttributes: Attribute[];
    @State(
        state =>
            state.deficientConditionGoalModule.scenarioDeficientConditionGoals,
    )
    stateScenarioDeficientConditionGoals: DeficientConditionGoal[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('getDeficientConditionGoalLibraries')
    getDeficientConditionGoalLibrariesAction: any;
    @Action('selectDeficientConditionGoalLibrary')
    selectDeficientConditionGoalLibraryAction: any;
    @Action('upsertDeficientConditionGoalLibrary')
    upsertDeficientConditionGoalLibraryAction: any;
    @Action('deleteDeficientConditionGoalLibrary')
    deleteDeficientConditionGoalLibraryAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioDeficientConditionGoals')
    getScenarioDeficientConditionGoalsAction: any;
    @Action('upsertScenarioDeficientConditionGoals')
    upsertScenarioDeficientConditionGoalsAction: any;

    @Getter('getNumericAttributes') getNumericAttributesGetter: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;

    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = null;
    selectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = clone(
        emptyDeficientConditionGoalLibrary,
    );
    hasSelectedLibrary: boolean = false;
    deficientConditionGoalGridHeaders: DataTableHeader[] = [
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '15%',
        },
        {
            text: 'Attribute',
            value: 'attribute',
            align: 'left',
            sortable: false,
            class: '',
            width: '12%',
        },
        {
            text: 'Deficient Limit',
            value: 'deficientLimit',
            align: 'left',
            sortable: false,
            class: '',
            width: '8%',
        },
        {
            text: 'Allowed Deficient Percentage',
            value: 'allowedDeficientPercentage',
            align: 'left',
            sortable: false,
            class: '',
            width: '11%',
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
    deficientConditionGoalGridData: DeficientConditionGoal[] = [];
    numericAttributeNames: string[] = [];
    selectedGridRows: DeficientConditionGoal[] = [];
    selectedDeficientConditionGoalIds: string[] = [];
    selectedDeficientConditionGoalForCriteriaEdit: DeficientConditionGoal = clone(
        emptyDeficientConditionGoal,
    );
    showCreateDeficientConditionGoalDialog: boolean = false;
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    createDeficientConditionGoalLibraryDialogData: CreateDeficientConditionGoalLibraryDialogData = clone(
        emptyCreateDeficientConditionGoalLibraryDialogData,
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
            vm.getDeficientConditionGoalLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.DeficientConditionGoal) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioDeficientConditionGoalsAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateDeficientConditionGoalLibraries')
    onStateDeficientConditionGoalLibrariesChanged() {
        this.librarySelectItems = this.stateDeficientConditionGoalLibraries.map(
            (library: DeficientConditionGoalLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }

    @Watch('librarySelectItemValue')
    onSelectItemValueChanged() {
        this.selectDeficientConditionGoalLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
    }

    @Watch('stateSelectedDeficientConditionGoalLibrary')
    onStateSelectedDeficientConditionGoalLibraryChanged() {
        this.selectedDeficientConditionGoalLibrary = clone(
            this.stateSelectedDeficientConditionGoalLibrary,
        );
    }

    @Watch('selectedDeficientConditionGoalLibrary', {deep: true})
    onSelectedDeficientConditionGoalLibraryChanged() {
        this.hasSelectedLibrary = this.selectedDeficientConditionGoalLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

        if (this.hasScenario) {
            this.deficientConditionGoalGridData = this.selectedDeficientConditionGoalLibrary.deficientConditionGoals
                .map((deficientGoal: DeficientConditionGoal) => ({
                    ...deficientGoal,
                    id: getNewGuid(),
                }));
        } else {
            this.deficientConditionGoalGridData = clone(this.selectedDeficientConditionGoalLibrary.deficientConditionGoals);
        }
        /*if (this.numericAttributeNames.length === 0) {
            this.numericAttributeNames = getPropertyValues(
                'name',
                this.getNumericAttributesGetter,
            );
        }*/
    }

    @Watch('selectedGridRows')
    onSelectedDeficientRowsChanged() {
        this.selectedDeficientConditionGoalIds = getPropertyValues('id', this.selectedGridRows,) as string[];
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
    }

    @Watch('stateScenarioDeficientConditionGoals')
    onStateScenarioDeficientConditionGoalsChanged() {
        if (this.hasScenario) {
            this.deficientConditionGoalGridData = clone(this.stateScenarioDeficientConditionGoals);
        }
    }

    @Watch('deficientConditionGoalGridData')
    onDeficientConditionGoalGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.deficientConditionGoalGridData, this.stateScenarioDeficientConditionGoals)
            : hasUnsavedChangesCore('',
                {...clone(this.selectedDeficientConditionGoalLibrary), deficientConditionGoals: clone(this.deficientConditionGoalGridData)},
                this.stateSelectedDeficientConditionGoalLibrary);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedDeficientConditionGoalLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedDeficientConditionGoalLibrary.owner) == getUserName();
    }

    onShowCreateDeficientConditionGoalLibraryDialog(createExistingLibraryAsNew: boolean) {
        this.createDeficientConditionGoalLibraryDialogData = {
            showDialog: true,
            deficientConditionGoals: createExistingLibraryAsNew
                ? this.deficientConditionGoalGridData
                : [],
        };
    }

    onSubmitCreateDeficientConditionGoalLibraryDialogResult(library: DeficientConditionGoalLibrary,) {
        this.createDeficientConditionGoalLibraryDialogData = clone(emptyCreateDeficientConditionGoalLibraryDialogData,);

        if (!isNil(library)) {
            this.upsertDeficientConditionGoalLibraryAction({ library: library});
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = library.name;
        }
    }

    onAddDeficientConditionGoal(newDeficientConditionGoal: DeficientConditionGoal) {
        this.showCreateDeficientConditionGoalDialog = false;

        if (!isNil(newDeficientConditionGoal)) {
            this.deficientConditionGoalGridData = prepend(
                newDeficientConditionGoal,
                this.deficientConditionGoalGridData,
            );
        }
    }

    onEditDeficientConditionGoalProperty(deficientConditionGoal: DeficientConditionGoal, property: string, value: any) {
        this.deficientConditionGoalGridData = update(
            findIndex(
                propEq('id', deficientConditionGoal.id),
                this.deficientConditionGoalGridData,
            ),
            setItemPropertyValue(
                property,
                value,
                deficientConditionGoal,
            ) as DeficientConditionGoal,
            this.deficientConditionGoalGridData,
        );
    }

    onShowCriterionLibraryEditorDialog(deficientConditionGoal: DeficientConditionGoal,) {
        this.selectedDeficientConditionGoalForCriteriaEdit = clone(
            deficientConditionGoal,
        );

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: deficientConditionGoal.criterionLibrary.id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario,
        };
    }

    onEditDeficientConditionGoalCriterionLibrary(criterionLibrary: CriterionLibrary,) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedDeficientConditionGoalForCriteriaEdit.id !== this.uuidNIL) {
            this.deficientConditionGoalGridData = update(
                findIndex(propEq('id', this.selectedDeficientConditionGoalForCriteriaEdit.id), this.deficientConditionGoalGridData),
                { ...this.selectedDeficientConditionGoalForCriteriaEdit, criterionLibrary: criterionLibrary},
                this.deficientConditionGoalGridData,
            );
        }

        this.selectedDeficientConditionGoalForCriteriaEdit = clone(
            emptyDeficientConditionGoal,
        );
    }

    onUpsertDeficientConditionGoalLibrary() {
        const library: DeficientConditionGoalLibrary = {
            ...clone(this.selectedDeficientConditionGoalLibrary),
            deficientConditionGoals: clone(this.deficientConditionGoalGridData)
        };
        this.upsertDeficientConditionGoalLibraryAction({library: library});
    }

    onUpsertScenarioDeficientConditionGoals() {
        this.upsertScenarioDeficientConditionGoalsAction({
            scenarioDeficientConditionGoals: this.deficientConditionGoalGridData,
            scenarioId: this.selectedScenarioId,
        }).then(() => this.librarySelectItemValue = null);
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.deficientConditionGoalGridData = clone(this.stateScenarioDeficientConditionGoals);
            }
        });
    }

    onRemoveSelectedDeficientConditionGoals() {
        this.deficientConditionGoalGridData = this.deficientConditionGoalGridData
            .filter((goal: DeficientConditionGoal) => !contains(goal.id, this.selectedDeficientConditionGoalIds));
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
            this.deleteDeficientConditionGoalLibraryAction({
                libraryId: this.selectedDeficientConditionGoalLibrary.id,
            });
        }
    }

    disableCrudButtons() {
        const dataIsValid: boolean = this.deficientConditionGoalGridData.every(
            (deficientGoal: DeficientConditionGoal) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        deficientGoal.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedDeficientConditionGoalLibrary.name,
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
.deficients-data-table {
    height: 425px;
    overflow-y: auto;
    overflow-x: hidden;
}

.deficients-data-table .v-menu--inline,
.deficient-criteria-output {
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
