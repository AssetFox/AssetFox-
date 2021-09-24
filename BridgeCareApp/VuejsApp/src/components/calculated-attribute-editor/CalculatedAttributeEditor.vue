<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center v-show="isAdmin">
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
                        v-if="!hasSelectedLibrary"
                        v-model="librarySelectItemValue"
                    >
                    </v-select>
                    <v-text-field
                        label="Library Name"
                        v-if="hasSelectedLibrary"
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
                    <v-checkbox
                        v-if="hasSelectedLibrary && !hasScenario"
                        class="sharing"
                        label="Default Calculation"
                        v-model="selectedCalculatedAttributeLibrary.isDefault"
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show="hasSelectedLibrary || hasScenario"></v-divider>
        <v-flex v-show="hasSelectedLibrary || hasScenario" xs12>
            <v-layout class="header-height" justify-center>
                <v-flex xs8>
                    <v-btn
                        @click="onAddCriterionEquationSet()"
                        class="ara-blue-bg white--text"
                        v-if="isAdmin"
                        :disabled="
                            attributeSelectItemValue == null ||
                                attributeSelectItemValue == ''
                        "
                    >
                        Add
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout class="data-table" justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-card-title>
                            <v-select
                                :items="attributeSelectItems"
                                v-if="!isAttributeSelectedItemValue"
                                label="Attribute"
                                outline
                                v-model="attributeSelectItemValue"
                            >
                            </v-select>
                            <v-text-field
                                label="Attribute name"
                                v-if="isAttributeSelectedItemValue"
                                v-model="attributeSelectItemValue"
                            >
                                <template slot="append">
                                    <v-btn
                                        @click="attributeSelectItemValue = null"
                                        class="ara-orange"
                                        icon
                                    >
                                        <v-icon>fas fa-caret-left</v-icon>
                                    </v-btn>
                                </template>
                            </v-text-field>
                            <v-select
                                :items="attributeTimingSelectItems"
                                v-if="!isTimingSelectedItemValue"
                                label="Timing"
                                outline
                                v-model="attributeTimingSelectItemValue"
                                :disabled="!isAdmin"
                                v-on:change="setTiming"
                            >
                            </v-select>
                            <v-text-field
                                label="Timing"
                                v-if="isTimingSelectedItemValue"
                                v-model="attributeTimingSelectItemValue"
                                :disabled="!isAdmin"
                            >
                                <template slot="append">
                                    <v-btn
                                        @click="
                                            attributeTimingSelectItemValue = null
                                        "
                                        class="ara-orange"
                                        icon
                                    >
                                        <v-icon>fas fa-caret-left</v-icon>
                                    </v-btn>
                                </template>
                            </v-text-field>
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
                            :items="selectedGridItem.equations"
                            :search="gridSearchTerm"
                            class="elevation-1 fixed-header v-table__overflow"
                            item-key="calculatedAttributeLibraryEquationId"
                        >
                            <template slot="items" slot-scope="props">
                                <td class="text-xs-center">
                                    <v-text-field
                                        readonly
                                        class="sm-txt"
                                        :value="props.item.equation.expression"
                                        :disabled="!isAdmin"
                                    >
                                        <template slot="append-outer">
                                            <v-btn
                                                @click="
                                                    onShowEquationEditorDialog(
                                                        props.item.id,
                                                    )
                                                "
                                                class="edit-icon"
                                                icon
                                                v-if="isAdmin"
                                            >
                                                <v-icon>fas fa-edit</v-icon>
                                            </v-btn>
                                        </template>
                                    </v-text-field>
                                </td>
                                <td class="text-xs-center">
                                    <v-text-field
                                        readonly
                                        class="sm-txt"
                                        :value="
                                            props.item.criteriaLibrary
                                                .mergedCriteriaExpression
                                        "
                                        :disabled="!isAdmin"
                                    >
                                        <template slot="append-outer">
                                            <v-btn
                                                @click="
                                                    onEditCalculatedAttributeCriterionLibrary(
                                                        props.item.id,
                                                    )
                                                "
                                                class="edit-icon"
                                                icon
                                                v-if="isAdmin"
                                            >
                                                <v-icon>fas fa-edit</v-icon>
                                            </v-btn>
                                        </template>
                                    </v-text-field>
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
                                        :disabled="!isAdmin"
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
                    @click="onUpsertScenarioCalculatedAttribute"
                    class="ara-blue-bg white--text"
                    v-show="hasScenario && isAdmin"
                >
                    Save
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton()"
                    @click="onUpsertCalculatedAttributeLibrary"
                    class="ara-blue-bg white--text"
                    v-show="!hasScenario"
                >
                    Update Library
                </v-btn>
                <v-btn
                    :disabled="disableCrudButton()"
                    v-if="isAdmin"
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
                    v-if="isAdmin"
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
import {
    clone,
    find,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import {
    CalculatedAttribute,
    CalculatedAttributeLibrary,
    CriterionAndEquationSet,
    emptyCalculatedAttribute,
    emptyCalculatedAttributeLibrary,
    emptyCriterionAndEquationSet,
    Timing,
    TimingMap,
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
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {
    CriterionLibrary,
    emptyCriteria,
    emptyCriterionLibrary,
} from '@/shared/models/iAM/criteria';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SelectItem } from '@/shared/models/vue/select-item';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { emptySelectItem } from '@/shared/models/vue/select-item';

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

    @State(
        state => state.calculatedAttributeModule.scenarioCalculatedAttributes,
    )
    stateScenarioCalculatedAttributes: CalculatedAttribute[];
    @State(state => state.calculatedAttributeModule.calculatedAttributes)
    stateCalculatedAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

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
    @Action('getCalculatedAttributes') getCalculatedAttributesAction: any;
    @Action('setErrorMessage') setErrorMessageAction: any;

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
    isAttributeSelectedItemValue: boolean = false;
    isTimingSelectedItemValue: boolean = false;
    attributeTimingSelectItems: SelectItem[] = [];
    attributeTimingSelectItemValue: string | null = '';
    currentCriteriaEquationSetSelectedId: string | null = '';

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
    activeCalculatedAttributeId: string = getBlankGuid();
    selectedGridItem: CalculatedAttribute = clone(emptyCalculatedAttribute);

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
            vm.attributeSelectItemValue = null;

            vm.getCalculatedAttributesAction();
            vm.getCalculatedAttributeLibrariesAction();

            vm.setAttributeSelectItems();
            vm.setAttributeTimingSelectItems();

            if (
                to.path.indexOf(ScenarioRoutePaths.CalculatedAttribute) !== -1
            ) {
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
    beforeDestroy() {
        this.calculatedAttributeGridData = [] as CalculatedAttribute[];
        this.selectedGridItem = clone(emptyCalculatedAttribute);
    }

    @Watch('stateCalculatedAttributes')
    onStateCalculatedAttributesChanged() {
        this.setAttributeSelectItems();
    }
    setAttributeSelectItems() {
        if (hasValue(this.stateCalculatedAttributes)) {
            this.attributeSelectItems = this.stateCalculatedAttributes.map(
                (attribute: Attribute) => ({
                    text: attribute.name,
                    value: attribute.name,
                }),
            );

            this.attributeSelectItems.forEach(_ => {
                var tempItem: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: _.text,
                    name: _.text,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                if (this.calculatedAttributeGridData == undefined) {
                    this.calculatedAttributeGridData = [] as CalculatedAttribute[];
                }
                this.calculatedAttributeGridData.push(tempItem);
            });
        }
    }
    setAttributeTimingSelectItems() {
        this.attributeTimingSelectItems = [
            { text: 'Pre Simulation', value: Timing.PreSimulation },
            { text: 'Post Simulation', value: Timing.PostSimulation },
            { text: 'On Demand', value: Timing.OnDemand },
        ];
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
    @Watch('attributeSelectItemValue')
    onAttributeSelectItemValueChanged() {
        // selection change in calculated attribute multi select
        if (
            isNil(this.attributeSelectItemValue) ||
            this.attributeSelectItemValue == ''
        ) {
            this.isAttributeSelectedItemValue = false;
            this.isTimingSelectedItemValue = false;
            this.selectedGridItem = clone(emptyCalculatedAttribute);
        } else {
            this.isAttributeSelectedItemValue = true;
            this.isTimingSelectedItemValue = true;
            var item = this.calculatedAttributeGridData.find(
                _ => _.attribute == this.attributeSelectItemValue,
            );
            if (item != undefined) {
                item.equations.forEach(_ => {
                    if (isNil(_.criteriaLibrary)) {
                        _.criteriaLibrary = clone(emptyCriterionLibrary);
                    }
                    if (isNil(_.equation)) {
                        _.equation = clone(emptyEquation);
                    }
                });
                this.activeCalculatedAttributeId = item.id;
                this.selectedGridItem = item;
                this.setTimingsMultiSelect(item.calculationTiming);
            } else {
                // if the selected Calculated attribute data is not present in the grid
                // Add a new object for it. Because we cannot loop over a object, which is null
                var newAttributeObject: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: this.attributeSelectItemValue,
                    name: this.attributeSelectItemValue,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                this.calculatedAttributeGridData.push(newAttributeObject);
                this.activeCalculatedAttributeId = newAttributeObject.id;
                this.selectedGridItem = newAttributeObject;
                this.setTimingsMultiSelect(Timing.OnDemand);
            }
        }
    }
    @Watch('attributeTimingSelectItemValue')
    onAttributeTimingSelectItemValue() {
        // Change in timings select box
        if (
            isNil(this.attributeTimingSelectItemValue) ||
            this.attributeTimingSelectItemValue == ''
        ) {
            this.isTimingSelectedItemValue = false;
        } else {
            this.isTimingSelectedItemValue = true;
            var localTiming = TimingMap[this.attributeTimingSelectItemValue];
            var item = this.calculatedAttributeGridData.find(
                _ => _.attribute == this.attributeSelectItemValue,
            );
            if (item != undefined) {
                item.calculationTiming = localTiming;
                this.selectedGridItem = item;
            }
        }
    }

    @Watch('stateSelectedCalculatedAttributeLibrary')
    onStateSelectedCalculatedAttributeLibraryChanged() {
        this.selectedCalculatedAttributeLibrary = clone(
            this.stateSelectedCalculatedAttributeLibrary,
        );
    }
    @Watch('stateScenarioCalculatedAttributes')
    onStateScenarioCalculatedAttributeChanged() {
        if (this.hasScenario) {
            if (
                !isNil(this.stateScenarioCalculatedAttributes) &&
                this.stateScenarioCalculatedAttributes.length > 0
            ) {
                this.activeCalculatedAttributeId = this.stateScenarioCalculatedAttributes[0].id;
            }
            this.resetGridData();
        }
    }
    @Watch('calculatedAttributeGridData')
    onCalculatedAttributeGridDataChanged() {
        if (this.isAdmin) {
            const hasUnsavedChanges: boolean = this.hasScenario
                ? hasUnsavedChangesCore(
                      '',
                      this.calculatedAttributeGridData,
                      this.stateScenarioCalculatedAttributes,
                  )
                : hasUnsavedChangesCore(
                      '',
                      {
                          ...clone(this.selectedCalculatedAttributeLibrary),
                          calculatedAttribute: clone(
                              this.calculatedAttributeGridData,
                          ),
                      },
                      this.stateSelectedCalculatedAttributeLibrary,
                  );
            this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
        }
    }
    @Watch('selectedCalculatedAttributeLibrary')
    onSelectedCalculatedAttributeLibraryChanged() {
        // change in library multiselect
        if (
            this.selectedCalculatedAttributeLibrary.id !== this.uuidNIL &&
            this.selectedCalculatedAttributeLibrary.id != getBlankGuid()
        ) {
            this.hasSelectedLibrary = true;
        } else {
            this.hasSelectedLibrary = false;
        }

        // If grid data is null, then add dummy objects on the fly, to show the rows to users.
        // So that, the users can add criteria and equations
        if (
            this.calculatedAttributeGridData == undefined ||
            this.calculatedAttributeGridData.length == 0
        ) {
            this.attributeSelectItems.forEach(_ => {
                var tempItem: CalculatedAttribute = {
                    id: getNewGuid(),
                    attribute: _.text,
                    name: _.text,
                    calculationTiming: Timing.OnDemand,
                    equations: [] as CriterionAndEquationSet[],
                };
                this.calculatedAttributeGridData.push(tempItem);
                this.setDefaultAttributeOnLoad(
                    this.calculatedAttributeGridData[0],
                );
            });
        }

        if (this.hasScenario && this.hasSelectedLibrary) {
            // we need new ids for the object which is assigned to a scenario.
            this.calculatedAttributeGridData = this.selectedCalculatedAttributeLibrary.calculatedAttributes.map(
                (value: CalculatedAttribute) => ({
                    ...value,
                    id: getNewGuid(),
                }),
            );
            // If a user in scenario page, then selecting a library should generate new Ids.
            // Because this data will be saved against the scenario
            this.calculatedAttributeGridData.forEach(att => {
                att.equations.map((value: CriterionAndEquationSet) => ({
                    ...value,
                    id: getNewGuid(),
                }));
                att.equations.forEach(eq => {
                    if (isNil(eq.criteriaLibrary)) {
                        eq.criteriaLibrary = clone(emptyCriterionLibrary);
                        eq.criteriaLibrary.id = getNewGuid();
                        eq.criteriaLibrary.isSingleUse = true;
                    } else {
                        eq.criteriaLibrary.id = getNewGuid();
                    }
                    eq.equation.id = getNewGuid();
                });
            });

            // Set the default values in Calculated attribute multi select, if we have data in calculatedAttributeGridData
            if (
                this.calculatedAttributeGridData != undefined &&
                this.calculatedAttributeGridData.length > 0
            ) {
                this.setDefaultAttributeOnLoad(
                    this.calculatedAttributeGridData[0],
                );
            } else {
                this.isAttributeSelectedItemValue = false;
            }
        } else if (this.hasScenario && !this.hasSelectedLibrary) {
            // If a user un select a Library, then reset the grid data from the scenario calculated attribute state
            this.resetGridData();
        } else if (!this.hasScenario) {
            // If a user is in Lirabry page
            this.calculatedAttributeGridData = clone(
                this.selectedCalculatedAttributeLibrary.calculatedAttributes,
            );
            if (
                this.calculatedAttributeGridData != undefined &&
                this.calculatedAttributeGridData.length > 0
            ) {
                this.attributeSelectItemValue = clone(
                    this.calculatedAttributeGridData[0].attribute,
                );
                this.isAttributeSelectedItemValue = true;

                this.setTimingsMultiSelect(
                    this.calculatedAttributeGridData[0].calculationTiming,
                );
                this.activeCalculatedAttributeId = this.calculatedAttributeGridData[0].id;
                this.selectedGridItem =
                    this.calculatedAttributeGridData[0] != undefined
                        ? this.calculatedAttributeGridData[0]
                        : this.selectedGridItem;
            } else {
                this.isAttributeSelectedItemValue = false;
                this.attributeSelectItemValue = null;
                this.attributeTimingSelectItemValue = null;
                this.isTimingSelectedItemValue = false;
            }
        }
    }
    setTiming(selectedItem: number) {
        this.setTimingsMultiSelect(selectedItem);
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
    onShowCreateCalculatedAttributeLibraryDialog(createAsNewLibrary: boolean) {
        var localCalculatedAttributes = [] as CalculatedAttribute[];
        if (createAsNewLibrary) {
            // if library is getting created from a scenario. Assign new Ids
            localCalculatedAttributes = this.calculatedAttributeGridData;
            localCalculatedAttributes.forEach(val => {
                val.id = getNewGuid();
                val.equations.forEach(eq => {
                    eq.id = getNewGuid();
                    if (!isNil(eq.criteriaLibrary)) {
                        eq.criteriaLibrary.id = getNewGuid();
                        eq.criteriaLibrary.isSingleUse = false;
                    }
                    eq.equation.id = getNewGuid();
                });
            });
        }
        this.createCalculatedAttributeLibraryDialogData = {
            showDialog: true,
            calculatedAttributes: createAsNewLibrary
                ? localCalculatedAttributes
                : ([] as CalculatedAttribute[]),
            attributeSelectItems: this.attributeSelectItems,
        };
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
            this.librarySelectItemValue = calculatedAttributeLibrary.id;
        }
    }
    onSubmitCreateCalculatedAttributeDialogResult(
        newCalculatedAttribute: CalculatedAttribute[],
    ) {
        this.showCreateCalculatedAttributeDialog = false;

        if (!isNil(newCalculatedAttribute)) {
            this.calculatedAttributeGridData = clone(newCalculatedAttribute);
        }
    }

    disableCrudButton() {
        if (this.calculatedAttributeGridData == undefined) {
            return false;
        }
        const dataIsValid = this.calculatedAttributeGridData.every(_ =>
            _.equations.every((set: CriterionAndEquationSet) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(set.id) === true
                );
            }),
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
    onAddCriterionEquationSet() {
        var newSet = clone(emptyCriterionAndEquationSet);
        newSet.id = getNewGuid();
        newSet.criteriaLibrary.id = getNewGuid();
        newSet.equation.id = getNewGuid();
        newSet.criteriaLibrary.isSingleUse = true;

        if (this.selectedGridItem.equations == undefined) {
            this.selectedGridItem.equations = [];
        }
        this.selectedGridItem.equations.push(newSet);
        this.calculatedAttributeGridData = update(
            findIndex(
                propEq('id', this.selectedGridItem.id),
                this.calculatedAttributeGridData,
            ),
            { ...this.selectedGridItem },
            this.calculatedAttributeGridData,
        );
    }
    onEditCalculatedAttributeCriterionLibrary(criterionEquationSetId: string) {
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        var currentCriteria = currItem.equations.find(
            _ => _.id == criterionEquationSetId,
        )!;
        this.currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (currentCriteria.criteriaLibrary.id == getBlankGuid()) {
            currentCriteria.criteriaLibrary = {
                id: getNewGuid(),
                name: '',
                mergedCriteriaExpression: '',
                description: '',
                isSingleUse: true,
            };
        }
        if (!isNil(currentCriteria)) {
            this.hasSelectedCalculatedAttribute = true;

            this.criterionLibraryEditorDialogData = {
                showDialog: true,
                libraryId: currentCriteria.criteriaLibrary.id,
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

        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        if (!isNil(criterionLibrary) && this.hasSelectedCalculatedAttribute) {
            currItem.equations.map(item => {
                item.id == this.currentCriteriaEquationSetSelectedId
                    ? (item.criteriaLibrary.mergedCriteriaExpression =
                          criterionLibrary.mergedCriteriaExpression)
                    : item.criteriaLibrary;
            });

            this.calculatedAttributeGridData = update(
                findIndex(
                    propEq('id', currItem.id),
                    this.calculatedAttributeGridData,
                ),
                { ...currItem },
                this.calculatedAttributeGridData,
            );
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onShowEquationEditorDialog(criterionEquationSetId: string) {
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        var currentEquation = currItem.equations.find(
            _ => _.id == criterionEquationSetId,
        );
        this.currentCriteriaEquationSetSelectedId = criterionEquationSetId;
        if (!isNil(currentEquation)) {
            this.hasSelectedCalculatedAttribute = true;

            this.equationEditorDialogData = {
                showDialog: true,
                equation: currentEquation.equation,
            };
        }
    }
    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedCalculatedAttribute) {
            var currItem = this.calculatedAttributeGridData.find(
                _ => _.id == this.activeCalculatedAttributeId,
            )!;
            currItem.equations.map(item => {
                item.id == this.currentCriteriaEquationSetSelectedId
                    ? (item.equation = equation)
                    : item.equation;
            });
        }

        this.selectedCalculatedAttribute = clone(emptyCalculatedAttribute);
        this.hasSelectedCalculatedAttribute = false;
    }
    onRemoveCalculatedAttribute(criterionEquationSetId: string) {
        var currItem = this.calculatedAttributeGridData.find(
            _ => _.id == this.activeCalculatedAttributeId,
        )!;
        currItem.equations = reject(
            propEq('id', criterionEquationSetId),
            currItem.equations,
        );
        this.selectedGridItem.equations = reject(
            propEq('id', criterionEquationSetId),
            this.selectedGridItem.equations,
        );
    }
    onDiscardChanges() {
        this.librarySelectItemValue = null;
        this.selectedCalculatedAttributeLibrary = clone(
            emptyCalculatedAttributeLibrary,
        );
        setTimeout(() => {
            if (this.hasScenario) {
                this.resetGridData();
            }
        });
    }

    resetGridData() {
        this.calculatedAttributeGridData = clone(
            this.stateScenarioCalculatedAttributes,
        );

        if (this.calculatedAttributeGridData != undefined) {
            var currItem = this.calculatedAttributeGridData.find(
                _ => _.id == this.activeCalculatedAttributeId,
            )!;

            if (currItem != undefined) {
                this.attributeSelectItemValue = clone(currItem.attribute);
                this.isAttributeSelectedItemValue = true;

                this.setTimingsMultiSelect(currItem.calculationTiming);
                this.selectedGridItem = currItem;
                // Setting up default values for null object, because API is sending it as null.
                this.selectedGridItem.equations.forEach(_ => {
                    if (isNil(_.criteriaLibrary)) {
                        _.criteriaLibrary = clone(emptyCriterionLibrary);
                        _.criteriaLibrary.id = getNewGuid();
                        _.criteriaLibrary.isSingleUse = true;
                    }
                });
            } else if (this.calculatedAttributeGridData.length > 0) {
                this.attributeSelectItemValue = this.calculatedAttributeGridData[0].attribute;
                this.isAttributeSelectedItemValue = true;
                this.setTimingsMultiSelect(
                    this.calculatedAttributeGridData[0].calculationTiming,
                );
            } else {
                this.attributeSelectItemValue = null;
                this.isAttributeSelectedItemValue = false;
            }
        }
    }
    setTimingsMultiSelect(selectedItem: number) {
        if (selectedItem == undefined) {
            selectedItem = Timing.OnDemand;
        }
        var localTiming = this.attributeTimingSelectItems.find(
            _ => _.value == selectedItem,
        )!.text;
        this.attributeTimingSelectItemValue = localTiming;
        this.isTimingSelectedItemValue = true;
    }
    setDefaultAttributeOnLoad(localCalculatedAttribute: CalculatedAttribute) {
        this.attributeSelectItemValue = clone(
            localCalculatedAttribute.attribute,
        );
        this.isAttributeSelectedItemValue = true;

        this.setTimingsMultiSelect(localCalculatedAttribute.calculationTiming);
        this.activeCalculatedAttributeId = localCalculatedAttribute.id;
        this.selectedGridItem =
            localCalculatedAttribute != undefined
                ? localCalculatedAttribute
                : this.selectedGridItem;
    }
}
</script>
