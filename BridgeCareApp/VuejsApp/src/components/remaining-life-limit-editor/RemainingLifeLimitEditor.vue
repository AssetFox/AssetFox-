<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="
                            onShowCreateRemainingLifeLimitLibraryDialog(false)
                        "
                        class="ara-blue-bg white--text"
                        v-show="!hasScenario"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        v-if="
                            !hasSelectedLibrary || hasScenario
                        "
                        :items="selectListItems"
                        label="Select a Remaining Life Limit Library"
                        outline
                        v-model="selectItemValue"
                    />
                    <v-text-field
                        v-if="
                            hasSelectedLibrary && !hasScenario
                        "
                        label="Library Name"
                        v-model="selectedRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                    >
                        <template slot="append">
                            <v-btn
                                @click="selectItemValue = null"
                                class="ara-orange"
                                icon
                            >
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                </v-flex>
            </v-layout>
            <v-flex
                xs3
                v-show="hasSelectedLibrary || hasScenario"
            >
                <v-btn
                    @click="onShowCreateRemainingLifeLimitDialog"
                    class="ara-blue-bg white--text"
                    >Add</v-btn
                >
            </v-flex>
        </v-flex>
        <v-flex
            xs12
            v-show="hasSelectedLibrary || hasScenario"
        >
            <div class="remaining-life-limit-data-table">
                <v-data-table
                    :headers="gridHeaders"
                    :items="remainingLifeLimits"
                    class="elevation-1 fixed-header v-table__overflow"
                >
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.attribute"
                                large
                                lazy
                                persistent
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'attribute',
                                        props.item.attribute,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.attribute"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-select
                                        :items="numericAttributeSelectItems"
                                        label="Select an Attribute"
                                        outline
                                        v-model="props.item.attribute"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-edit-dialog
                                :return-value.sync="props.item.value"
                                large
                                lazy
                                persistent
                                @save="
                                    onEditRemainingLifeLimitProperty(
                                        props.item,
                                        'value',
                                        props.item.value,
                                    )
                                "
                            >
                                <v-text-field
                                    readonly
                                    single-line
                                    class="sm-txt"
                                    :value="props.item.value"
                                    :rules="[
                                        rules['generalRules'].valueIsNotEmpty,
                                    ]"
                                />
                                <template slot="input">
                                    <v-text-field
                                        label="Edit"
                                        single-line
                                        :mask="'##########'"
                                        v-model.number="props.item.value"
                                        :rules="[
                                            rules['generalRules']
                                                .valueIsNotEmpty,
                                        ]"
                                    />
                                </template>
                            </v-edit-dialog>
                        </td>
                        <td>
                            <v-text-field
                                :value="
                                    props.item.criterionLibrary
                                        .mergedCriteriaExpression
                                "
                                readonly
                            >
                                <template slot="append-outer">
                                    <v-icon
                                        @click="
                                            onShowCriterionLibraryEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                    >
                                        fas fa-edit
                                    </v-icon>
                                </template>
                            </v-text-field>
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
                        v-model="selectedRemainingLifeLimitLibrary.description"
                        @input='selectedRemainingLifeLimitLibrary = {...selectedRemainingLifeLimitLibrary, description: $event}'
                    >
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12 v-show="hasSelectedLibrary || hasScenario">
            <v-layout justify-end row>
                <v-btn
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertScenarioRemainingLifeLimits"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario"
                >
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertRemainingLifeLimitLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                >
                    Update Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton()"
                    @click="onShowCreateRemainingLifeLimitLibraryDialog(true)"
                    class="ara-blue-bg white--text"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    v-show="!hasScenario"
                    class="ara-orange-bg white--text"
                    @click="onShowConfirmDeleteAlert"
                    :disabled="!hasSelectedLibrary"
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

        <CreateRemainingLifeLimitLibraryDialog
            :dialogData="createRemainingLifeLimitLibraryDialogData"
            @submit="onSubmitCreateRemainingLifeLimitLibraryDialogResult"
        />

        <CreateRemainingLifeLimitDialog
            :dialogData="createRemainingLifeLimitDialogData"
            @submit="onAddRemainingLifeLimit"
        />

        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onEditRemainingLifeLimitCriterionLibrary"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import { Watch } from 'vue-property-decorator';
import {
    emptyRemainingLifeLimit,
    emptyRemainingLifeLimitLibrary,
    RemainingLifeLimit,
    RemainingLifeLimitLibrary,
} from '@/shared/models/iAM/remaining-life-limit';
import { prepend, clone, findIndex, isNil, propEq, update } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    CreateRemainingLifeLimitLibraryDialogData,
    emptyCreateRemainingLifeLimitLibraryDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import CreateRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
import {
    CreateRemainingLifeLimitDialogData,
    emptyCreateRemainingLifeLimitDialogData,
} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';

@Component({
    components: {
        CreateRemainingLifeLimitLibraryDialog,
        CreateRemainingLifeLimitDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class RemainingLifeLimitEditor extends Vue {
    @State(state => state.remainingLifeLimitModule.remainingLifeLimitLibraries)
    stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[];
    @State(
        state =>
            state.remainingLifeLimitModule.selectedRemainingLifeLimitLibrary,
    )
    stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
    @State(state => state.attributeModule.numericAttributes)
    stateNumericAttributes: Attribute[];
    @State(state => state.remainingLifeLimitModule.scenarioRemainingLifeLimits)
    stateScenarioRemainingLifeLimits: RemainingLifeLimit[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;

    @Action('getRemainingLifeLimitLibraries')
    getRemainingLifeLimitLibrariesAction: any;
    @Action('upsertRemainingLifeLimitLibrary')
    upsertRemainingLifeLimitLibraryAction: any;
    @Action('deleteRemainingLifeLimitLibrary')
    deleteRemainingLifeLimitLibraryAction: any;
    @Action('selectRemainingLifeLimitLibrary')
    selectRemainingLifeLimitLibraryAction: any;
    @Action('setErrorMessage') setErrorMessageAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioRemainingLifeLimits')
    getScenarioRemainingLifeLimitsAction: any;
    @Action('upsertScenarioRemainingLifeLimits')
    upsertScenarioRemainingLifeLimitsAction: any;

    remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
    selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(
        emptyRemainingLifeLimitLibrary,
    );
    selectedScenarioId: string = getBlankGuid();
    selectItemValue: string | null = '';
    selectListItems: SelectItem[] = [];
    hasSelectedLibrary: boolean = false;
    gridHeaders: DataTableHeader[] = [
        {
            text: 'Remaining Life Attribute',
            value: 'attribute',
            align: 'left',
            sortable: true,
            class: '',
            width: '12.4%',
        },
        {
            text: 'Limit',
            value: 'value',
            align: 'left',
            sortable: true,
            class: '',
            width: '12.4%',
        },
        {
            text: 'Criteria',
            value: 'criteria',
            align: 'left',
            sortable: false,
            class: '',
            width: '75%',
        },
    ];
    remainingLifeLimits: RemainingLifeLimit[] = [];
    numericAttributeSelectItems: SelectItem[] = [];
    createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(
        emptyCreateRemainingLifeLimitDialogData,
    );
    selectedRemainingLifeLimit: RemainingLifeLimit = clone(
        emptyRemainingLifeLimit,
    );
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
        emptyCreateRemainingLifeLimitLibraryDialogData,
    );
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    hasScenario: boolean = false;
    currentUrl: string = window.location.href;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.selectItemValue = null;
            vm.getRemainingLifeLimitLibrariesAction();
            if (to.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;
                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.setErrorMessageAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }
                vm.hasScenario = true;
                vm.getScenarioRemainingLifeLimitsAction(vm.selectedScenarioId);
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateRemainingLifeLimitLibraries')
    onStateRemainingLifeLimitLibrariesChanged() {
        this.selectListItems = this.stateRemainingLifeLimitLibraries.map(
            (remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
                text: remainingLifeLimitLibrary.name,
                value: remainingLifeLimitLibrary.id,
            }),
        );
    }

    @Watch('selectItemValue')
    onSelectItemValueChanged() {
        this.selectRemainingLifeLimitLibraryAction({
            libraryId: this.selectItemValue,
        });
    }

    @Watch('stateSelectedRemainingLifeLimitLibrary')
    onStateSelectedRemainingLifeLimitLibraryChanged() {
        this.selectedRemainingLifeLimitLibrary = clone(
            this.stateSelectedRemainingLifeLimitLibrary,
        );
    }

    @Watch('selectedRemainingLifeLimitLibrary')
    onSelectedRemainingLifeLimitLibraryChanged() {
        this.hasSelectedLibrary =
            this.selectedRemainingLifeLimitLibrary.id !== this.uuidNIL;

        if (this.hasScenario) {
            this.remainingLifeLimits = this.selectedRemainingLifeLimitLibrary.remainingLifeLimits.map((remainingLifeLimit: RemainingLifeLimit) => ({
                ...remainingLifeLimit, id: getNewGuid()
            }));
        } else {
            this.remainingLifeLimits = clone(
                this.selectedRemainingLifeLimitLibrary.remainingLifeLimits,
            );
        }
    }

    @Watch('stateScenarioRemainingLifeLimits')
    onStateScenarioRemainingLifeLimitsChanged() {
        if (this.hasScenario) {
            this.remainingLifeLimits = clone(this.stateScenarioRemainingLifeLimits);
        }
    }
    @Watch('remainingLifeLimits')
    onGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.remainingLifeLimits, this.stateScenarioRemainingLifeLimits)
            : hasUnsavedChangesCore('',
                { ...clone(this.selectedRemainingLifeLimitLibrary), remainingLifeLimits: clone(this.remainingLifeLimits) },
                this.stateSelectedRemainingLifeLimitLibrary);

        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributesSelectListItems();
    }

    mounted() {
        this.setAttributesSelectListItems();
    }

    setAttributesSelectListItems() {
        if (hasValue(this.stateNumericAttributes)) {
            this.numericAttributeSelectItems = this.stateNumericAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }

    onShowCreateRemainingLifeLimitLibraryDialog(createAsNewLibrary: boolean) {
        this.createRemainingLifeLimitLibraryDialogData = {
            showDialog: true,
            remainingLifeLimits: createAsNewLibrary
                ? this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
                : [],
        };
    }

    onSubmitCreateRemainingLifeLimitLibraryDialogResult(library: RemainingLifeLimitLibrary) {
        this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

        if (!isNil(library)) {
            this.upsertRemainingLifeLimitLibraryAction({library: library});
        }
    }

    onShowCreateRemainingLifeLimitDialog() {
        this.createRemainingLifeLimitDialogData = {
            showDialog: true,
            numericAttributeSelectItems: this.numericAttributeSelectItems,
        };
    }

    onAddRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
        this.createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);

        if (!isNil(newRemainingLifeLimit)) {
            this.remainingLifeLimits = prepend(newRemainingLifeLimit, this.remainingLifeLimits);
        }
    }

    onEditRemainingLifeLimitProperty(remainingLifeLimit: RemainingLifeLimit, property: string, value: any) {
        this.remainingLifeLimits = update(
            findIndex(propEq('id', remainingLifeLimit.id), this.remainingLifeLimits),
            setItemPropertyValue(property, value, remainingLifeLimit),
            this.remainingLifeLimits,
        );
    }

    onShowCriterionLibraryEditorDialog(remainingLifeLimit: RemainingLifeLimit) {
        this.selectedRemainingLifeLimit = remainingLifeLimit;

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: remainingLifeLimit.criterionLibrary.id,
            isCallFromScenario: this.hasScenario,
            isCriterionForLibrary: !this.hasScenario,
        };
    }

    onEditRemainingLifeLimitCriterionLibrary(
        criterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedRemainingLifeLimit.id !== this.uuidNIL) {
            this.remainingLifeLimits = update(
                findIndex(
                    propEq('id', this.selectedRemainingLifeLimit.id),
                    this.remainingLifeLimits,
                ),
                {
                    ...this.selectedRemainingLifeLimit,
                    criterionLibrary: criterionLibrary,
                },
                this.remainingLifeLimits,
            );
        }

        this.selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
    }

    onUpsertRemainingLifeLimitLibrary() {
        const library: RemainingLifeLimitLibrary = {
            ...clone(this.selectedRemainingLifeLimitLibrary),
            remainingLifeLimits: clone(this.remainingLifeLimits)
        };
        this.upsertRemainingLifeLimitLibraryAction({ library: library, });
    }

    onUpsertScenarioRemainingLifeLimits() {
        this.upsertScenarioRemainingLifeLimitsAction({
            scenarioRemainingLifeLimits: this.remainingLifeLimits,
            scenarioId: this.selectedScenarioId,
        }).then(() => (this.selectItemValue = null));
    }

    onDiscardChanges() {
        this.selectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.remainingLifeLimits = clone(this.stateScenarioRemainingLifeLimits);
            }
        });
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
            this.selectItemValue = null;
            this.deleteRemainingLifeLimitLibraryAction({
                libraryId: this.selectedRemainingLifeLimitLibrary.id,
            });
        }
    }

    disableCrudButton() {
        const dataIsValid: boolean = this.remainingLifeLimits.every(
            (remainingLife: RemainingLifeLimit) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.value,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedRemainingLifeLimitLibrary.name,
                ) === true &&
                dataIsValid);
        }

        return !dataIsValid;
    }
}
</script>
