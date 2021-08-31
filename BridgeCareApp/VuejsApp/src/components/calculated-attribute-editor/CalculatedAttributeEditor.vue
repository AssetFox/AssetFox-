<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click="
                            onShowCreateCalculatedAttributeLibraryDialog(false)
                        "
                        class="ara-blue-bg white--text"
                        v-show="!hasScenario"
                    >
                        New Library
                    </v-btn>
                    <v-select
                        :items="librarySelectItems"
                        label="Select a Calculated attribute Library"
                        outline
                        v-if="!hasSelectedLibrary || hasScenario"
                        v-model="librarySelectItemValue"
                    >
                    </v-select>
                    <v-text-field
                        label="Library Name"
                        v-if="hasSelectedLibrary && !hasScenario"
                        v-model="selectedCalculatedAttributeLibrary.name"
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
                    <v-checkbox v-show="!hasScenario" class="sharing" label="Default Calculations" />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedLibrary || hasScenario"></v-divider>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout class="data-table" justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-card-title>
                            <v-select
                                :items="attributeSelectItems"
                                label="Attribute"
                                outline
                                v-model="attributeSelectItemValue"
                            >
                            </v-select>
                            <v-select
                                :items="attributeTimingSelectItems"
                                label="Timing"
                                outline
                                v-model="attributeTimingSelectItemValue"
                            >
                            </v-select>
                            <v-spacer></v-spacer>
                            <v-text-field
                                append-icon="fas fa-search"
                                hide-details
                                lablel="Search"
                                single-line
                                v-model="gridSearchTerm"
                            >
                            </v-text-field>
                        </v-card-title>
                        <v-data-table
                            :headers="calculatedAttributeGridHeaders"
                            :items="calculatedAttributeGridData"
                            :search="gridSearchTerm"
                            class="elevation-1 fixed-header v-table__overflow"
                            item-key="calculatedAttributeLibraryEquationId"
                        >
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-center">
                                    <v-btn
                                        @click="
                                            onShowEquationEditorDialog(
                                                props.item.id,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-btn
                                        @click="
                                            onEditCalculatedAttributeCriterionLibrary(
                                                props.item.id,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="text-xs-center">
                                    <v-btn
                                        @click="
                                            onRemoveCalculatedAttribute(
                                                props.item.id,
                                            )
                                        "
                                        class="ara-orange"
                                        icon
                                    >
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedLibrary || hasScenario"></v-divider>
        <v-flex v-show="hasSelectedLibrary && !hasScenario" xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea
                        label="Description"
                        no-resize
                        outline
                        rows="4"
                        v-model="selectedCalculatedAttributeLibrary.description"
                        @input="
                            selectedCalculatedAttributeLibrary = {
                                ...selectedCalculatedAttributeLibrary,
                                description: $event,
                            }
                        "
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout
                justify-end
                row
                v-show="hasSelectedLibrary || hasScenario"
            >
                <v-btn
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertScenarioCalculatedAttribute"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario"
                >
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton() || !hasUnsavedChanges"
                    @click="onUpsertCalculatedAttributeLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                >
                    Update Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton()"
                    @click="onShowCreateCalculatedAttributeLibraryDialog(true)"
                    class="ara-blue-bg white--text"
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click="onShowConfirmDeleteAlert"
                    class="ara-orange-bg white--text"
                    v-show="!hasScenario"
                    :disabled="!hasSelectedLibrary"
                >
                    Delete Library
                </v-btn>
                <v-btn
                    :disabled="!hasUnsavedChanges"
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
        <CreateCalculatedAttributeLibraryDialog
            :dialogData="createCalculatedAttributeLibraryDialogData"
            @submit="onSubmitCreateCalculatedAttributeLibraryDialogResult"
        />

        <CreateCalculatedAttributeDialog
            :showDialog="showCreateCalculatedAttributeDialog"
            @submit="onSubmitCreateCalculatedAttributeDialogResult"
        />
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            @submit="onSubmitEquationEditorDialogResult"
        />
        <CriterionLibraryEditorDialog
            :dialogData="criterionLibraryEditorDialogData"
            @submit="onSubmitCriterionLibraryEditorDialogResult"
        />
    </v-layout>
</template>
<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import Alert from '@/shared/modals/Alert.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../shared/modals/CriterionLibraryEditorDialog.vue';
import CreateCalculatedAttributeLibraryDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeLibraryDialog.vue';
import CreateCalculatedAttributeDialog from './calculated-attribute-editor-dialogs/CreateCalculatedAttributeDialog.vue';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { clone, find, findIndex, isNil, prepend, propEq, update } from 'ramda';
import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
} from '@/shared/models/iAM/calculated-attribute';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { Attribute } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import {
    CreateCalculatedAttributeLibraryDialogData,
    emptyCreateCalculatedAttributeLibraryDialogData,
} from '@/shared/models/modals/create-calculated-attribute-library-dialog-data';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';

@Component({
    components: {
        CreateCalculatedAttributeLibraryDialog,
        CreateCalculatedAttributeDialog,
        EquationEditorDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class CalculatedAttributeEditor extends Vue {
    @State(
        state => state.calculatedAttributeModule.calculatedAttributeLibraries,
    )
    stateCalculatedAttributeLibraries: CalculatedAttributeLibrary[];
    @State(
        state =>
            state.calculatedAttributeModule.selectedCalculatedAttributeLibrary,
    )
    stateSelectedCalculatedAttributeLibrary: CalculatedAttributeLibrary;

    @State(state => state.calculatedAttributeModule.scenarioCalculatedAttribute)
    stateScenarioCalculatedAttribute: CalculatedAttribute[];
    @State(state => state.attributeModule.numericAttributes)
    stateNumericAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;

    @Action('upsertScenarioCalculatedAttribute')
    upsertScenarioCalculatedAttributeAction: any;
    @Action('upsertCalculatedAttributeLibrary')
    upsertCalculatedAttributeLibraryAction: any;
    @Action('deleteCalculatedAttributeLibrary')
    deleteCalculatedAttributeLibraryAction: any;
    @Action('getCalculatedAttributeLibraries')
    getCalculatedAttributeLibrariesAction: any;
    @Action('getScenarioCalculatedAttribute')
    getScenarioCalculatedAttributeAction: any;
    @Action('selectCalculatedAttributeLibrary')
    selectCalculatedAttributeLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

    hasSelectedLibrary: boolean = false;
    hasScenario: boolean = false;
    rules: InputValidationRules = clone(rules);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    showCreateCalculatedAttributeDialog = false;
    hasSelectedCalculatedAttribute: boolean = false;
    selectedCalculatedAttribute: CalculatedAttribute = clone(
        emptyCalculatedAttribute,
    );
    selectedScenarioId: string = getBlankGuid();
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = '';
    attributeSelectItems: SelectItem[] = [];
    attributeSelectItemValue: string | null = '';
    attributeTimingSelectItems: SelectItem[] = [];
    attributeTimingSelectItemValue: string | null = '';

    gridSearchTerm = '';
    selectedCalculatedAttributeLibrary: CalculatedAttributeLibrary = clone(
        emptyCalculatedAttributeLibrary,
    );
    createCalculatedAttributeLibraryDialogData: CreateCalculatedAttributeLibraryDialogData = clone(
        emptyCreateCalculatedAttributeLibraryDialogData,
    );
    equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    calculatedAttributeGridData: CalculatedAttribute[] = [];

    calculatedAttributeGridHeaders: DataTableHeader[] = [
        {
            text: 'Equation',
            value: 'equation',
            align: 'center',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Criterion',
            value: 'criterionLibrary',
            align: 'center',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: '',
            value: '',
            align: 'center',
            sortable: false,
            class: '',
            width: '',
        },
    ];

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getCalculatedAttributeLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.CalculatedAttribute) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.setErrorMessageAction({
                        message: 'Unable to identify selected scenario.',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioCalculatedAttributeAction(vm.selectedScenarioId);
            }
        });
    }
    mounted() {
        this.setAttributeSelectItems();
    }
    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributeSelectItems();
    }
    setAttributeSelectItems() {
        if (hasValue(this.stateNumericAttributes)) {
            this.attributeSelectItems = this.stateNumericAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );
        }
    }
    @Watch('stateCalculatedAttributeLibraries')
    onStateCalculatedAttributeLibrariesChanged() {
        this.librarySelectItems = this.stateCalculatedAttributeLibraries.map(
            (library: CalculatedAttributeLibrary) => ({
                text: library.name,
                value: library.id,
            }),
        );
    }
    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectCalculatedAttributeLibraryAction(
            this.librarySelectItemValue,
        );
    }

    @Watch('stateSelectedCalculatedAttributeLibrary')
    onStateSelectedCalculatedAttributeLibraryChanged() {
        this.selectedCalculatedAttributeLibrary = clone(
            this.stateSelectedCalculatedAttributeLibrary,
        );
    }
    @Watch('stateScenarioCalculatedAttribute')
    onStateScenarioCalculatedAttributeChanged() {
        if (this.hasScenario) {
            this.calculatedAttributeGridData = clone(
                this.stateScenarioCalculatedAttribute,
            );
        }
    }
    @Watch('calculatedAttributeGridData')
    onCalculatedAttributeGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore(
                  '',
                  this.calculatedAttributeGridData,
                  this.stateScenarioCalculatedAttribute,
              )
            : hasUnsavedChangesCore(
                  '',
                  {
                      ...clone(this.selectedCalculatedAttributeLibrary),
                      calculatedAttributes: clone(
                          this.calculatedAttributeGridData,
                      ),
                  },
                  this.stateSelectedCalculatedAttributeLibrary,
              );
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }
    @Watch('selectedCalculatedAttributeLibrary')
    onSelectedCalculatedAttributeLibraryChanged() {
        this.hasSelectedLibrary =
            this.selectedCalculatedAttributeLibrary.id !== this.uuidNIL;

        if (this.hasScenario) {
            this.calculatedAttributeGridData = this.selectedCalculatedAttributeLibrary.calculatedAttributes.map(
                (calculatedAttribute: CalculatedAttribute) => ({
                    ...calculatedAttribute,
                    id: getNewGuid(),
                }),
            );
        } else {
            this.calculatedAttributeGridData = clone(
                this.selectedCalculatedAttributeLibrary.calculatedAttributes,
            );
        }
    }
    onUpsertScenarioCalculatedAttribute() {
        this.upsertScenarioCalculatedAttributeAction({
            scenarioCalculatedAttribute: this.calculatedAttributeGridData,
            scenarioId: this.selectedScenarioId,
        }).then(() => (this.librarySelectItemValue = null));
    }

    onUpsertCalculatedAttributeLibrary() {
        const CalculatedAttributeLibrary: CalculatedAttributeLibrary = {
            ...clone(this.selectedCalculatedAttributeLibrary),
            calculatedAttributes: clone(this.calculatedAttributeGridData),
        };
        this.upsertCalculatedAttributeLibraryAction(CalculatedAttributeLibrary);
    }
    onSubmitCreateCalculatedAttributeLibraryDialogResult(
        calculatedAttributeLibrary: CalculatedAttributeLibrary,
    ) {
        this.createCalculatedAttributeLibraryDialogData = clone(
            emptyCreateCalculatedAttributeLibraryDialogData,
        );

        if (!isNil(calculatedAttributeLibrary)) {
            this.upsertCalculatedAttributeLibraryAction(
                calculatedAttributeLibrary,
            );
        }
    }
    onSubmitCreateCalculatedAttributeDialogResult(
        newCalculatedAttribute: CalculatedAttribute,
    ) {
        this.showCreateCalculatedAttributeDialog = false;

        if (!isNil(newCalculatedAttribute)) {
            this.calculatedAttributeGridData = prepend(
                newCalculatedAttribute,
                this.calculatedAttributeGridData,
            );
        }
    }
    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedCalculatedAttribute) {
            this.calculatedAttributeGridData = update(
                findIndex(
                    propEq('id', this.selectedCalculatedAttribute.id),
                    this.calculatedAttributeGridData,
                ),
                { ...this.selectedCalculatedAttribute, equation: equation },
                this.calculatedAttributeGridData,
            );
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }

    disableCrudButton() {
        const dataIsValid: boolean = this.calculatedAttributeGridData.every(
            (calculatedAttribute: CalculatedAttribute) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        calculatedAttribute.name,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        calculatedAttribute.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedCalculatedAttributeLibrary.name,
                ) === true && dataIsValid
            );
        }

        return !dataIsValid;
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
            this.deleteCalculatedAttributeLibraryAction(
                this.selectedCalculatedAttributeLibrary.id,
            );
        }
    }
    onEditCalculatedAttributeCriterionLibrary(calculatedAttributeId: string) {
        this.selectedCalculatedAttribute = find(
            propEq('id', calculatedAttributeId),
            this.calculatedAttributeGridData,
        ) as CalculatedAttribute;

        if (!isNil(this.selectedCalculatedAttribute)) {
            this.hasSelectedCalculatedAttribute = true;

            this.criterionLibraryEditorDialogData = {
                showDialog: true,
                libraryId: this.selectedCalculatedAttribute.criterionLibrary.id,
                isCallFromScenario: this.hasScenario,
                isCriterionForLibrary: !this.hasScenario,
            };
        }
    }

    onSubmitCriterionLibraryEditorDialogResult(
        criterionLibrary: CriterionLibrary,
    ) {
        this.criterionLibraryEditorDialogData = clone(
            emptyCriterionLibraryEditorDialogData,
        );

        if (!isNil(criterionLibrary) && this.hasSelectedCalculatedAttribute) {
            this.calculatedAttributeGridData = update(
                findIndex(
                    propEq('id', this.selectedCalculatedAttribute.id),
                    this.calculatedAttributeGridData,
                ),
                {
                    ...this.selectedCalculatedAttribute,
                    criterionLibrary: criterionLibrary,
                },
                this.calculatedAttributeGridData,
            );
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.calculatedAttributeGridData = clone(
                    this.stateScenarioCalculatedAttribute,
                );
            }
        });
    }
}
</script>
