<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn @click='onShowCreatePerformanceCurveLibraryDialog(false)' class='ara-blue-bg white--text'
                           v-show="!hasScenario">
                        New Library
                    </v-btn>
                    <v-select :items='librarySelectItems'
                              label='Select a Performance Library'
                              outline v-if="!hasSelectedLibrary || hasScenario"
                              v-model='librarySelectItemValue'>
                    </v-select>
                    <v-text-field label='Library Name'
                                  v-if="hasSelectedLibrary && !hasScenario"
                                  v-model='selectedPerformanceCurveLibrary.name'
                                  :rules="[rules['generalRules'].valueIsNotEmpty]">
                        <template slot='append'>
                            <v-btn @click='librarySelectItemValue = null' class='ara-orange' icon>
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if="hasSelectedLibrary && selectedScenarioId === '0'">
                        Owner:
                        {{ selectedPerformanceCurveLibrary.owner ? selectedPerformanceCurveLibrary.owner : '[ No Owner ]'
                        }}
                    </div>
                    <v-checkbox class='sharing' label='Shared'
                                v-if="hasSelectedLibrary && selectedScenarioId === '0'"
                                v-model='selectedPerformanceCurveLibrary.shared' />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <v-layout class='header-height' justify-center>
                <v-flex xs8>
                    <v-btn @click='showCreatePerformanceCurveDialog = true' class='ara-blue-bg white--text'>
                        Add
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout class='data-table' justify-center>
                <v-flex xs8>
                    <v-card>
                        <v-card-title>
                            Performance equation
                            <v-spacer></v-spacer>
                            <v-text-field append-icon='fas fa-search' hide-details lablel='Search'
                                          single-line
                                          v-model='gridSearchTerm'>
                            </v-text-field>
                        </v-card-title>
                        <v-data-table :headers='performanceCurveGridHeaders'
                                      :items='performanceCurveGridData'
                                      :search='gridSearchTerm'
                                      class='elevation-1 fixed-header v-table__overflow'
                                      item-key='performanceLibraryEquationId'>
                            <template slot='items' slot-scope='props'>
                                <td class='text-xs-center'>
                                    <v-edit-dialog
                                        :return-value.sync='props.item.name'
                                        @save="onEditPerformanceCurveProperty(props.item.id, 'name', props.item.name)"
                                        large lazy persistent>
                                        <v-text-field readonly single-line
                                                      class='sm-txt equation-name-text-field-output'
                                                      :value='props.item.name'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                        <template slot='input'>
                                            <v-text-field label='Edit' single-line v-model='props.item.name'
                                                          :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class='text-xs-center'>
                                    <v-edit-dialog
                                        :return-value.sync='props.item.attribute'
                                        @save="onEditPerformanceCurveProperty(props.item.id, 'attribute', props.item.attribute)"
                                        large lazy persistent>
                                        <v-text-field readonly single-line class='sm-txt attribute-text-field-output'
                                                      :value='props.item.attribute'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                        <template slot='input'>
                                            <v-select :items='attributeSelectItems' label='Edit'
                                                      v-model='props.item.attribute'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                        </template>
                                    </v-edit-dialog>
                                </td>
                                <td class='text-xs-center'>
                                    <v-menu left min-height='500px' min-width='500px'
                                            v-show="props.item.equation.expression !== ''">
                                        <template slot='activator'>
                                            <v-btn class='ara-blue' icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea class='sm-txt' :value='props.item.equation.expression' full-width
                                                            no-resize outline
                                                            readonly
                                                            rows='5' />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click='onShowEquationEditorDialog(props.item.id)' class='edit-icon' icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class='text-xs-center'>
                                    <v-menu min-height='500px' min-width='500px' right
                                            v-show="props.item.criterionLibrary.mergedCriteriaExpression !== ''">
                                        <template slot='activator'>
                                            <v-btn class='ara-blue' flat icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea class='sm-txt' :value='props.item.criterionLibrary.mergedCriteriaExpression' full-width
                                                            no-resize outline
                                                            readonly
                                                            rows='5' />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn @click='onEditPerformanceCurveCriterionLibrary(props.item.id)'
                                           class='edit-icon' icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class='text-xs-center'>
                                    <v-btn @click='onRemovePerformanceCurve(props.item.id)' class='ara-orange' icon>
                                        <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                </td>
                            </template>
                        </v-data-table>
                    </v-card>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
        <v-flex v-show='hasSelectedLibrary && !hasScenario'
                xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea label='Description' no-resize outline rows='4'
                                v-model='selectedPerformanceCurveLibrary.description' />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show='hasSelectedLibrary || hasScenario'>
                <v-btn :disabled='disableCrudButton() || !hasUnsavedChanges'
                       @click='onUpsertScenarioPerformanceCurves'
                       class='ara-blue-bg white--text'
                       v-show='hasScenario'>
                    Save
                </v-btn>
                <v-btn :disabled='disableCrudButton() || !hasUnsavedChanges'
                       @click='onUpsertPerformanceCurveLibrary'
                       class='ara-blue-bg white--text'
                       v-show='!hasScenario'>
                    Update Library
                </v-btn>
                <v-btn :disabled='disableCrudButton() || !hasUnsavedChanges' @click='onShowCreatePerformanceCurveLibraryDialog(true)'
                       class='ara-blue-bg white--text'>
                    Create as New Library
                </v-btn>
                <v-btn @click='onShowConfirmDeleteAlert' class='ara-orange-bg white--text'
                       v-show='!hasScenario' :disabled='!hasSelectedLibrary'>
                    Delete Library
                </v-btn>
                <v-btn :disabled='!hasUnsavedChanges' @click='onDiscardChanges' class='ara-orange-bg white--text' v-show='hasScenario'>
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData'
                            @submit='onSubmitConfirmDeleteAlertResult' />

        <CreatePerformanceCurveLibraryDialog :dialogData='createPerformanceCurveLibraryDialogData'
                                             @submit='onSubmitCreatePerformanceCurveLibraryDialogResult' />

        <CreatePerformanceCurveDialog :showDialog='showCreatePerformanceCurveDialog'
                                      @submit='onSubmitCreatePerformanceCurveDialogResult' />

        <EquationEditorDialog :dialogData='equationEditorDialogData' @submit='onSubmitEquationEditorDialogResult' />

        <CriterionLibraryEditorDialog :dialogData='criterionLibraryEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Watch } from 'vue-property-decorator';
import Component from 'vue-class-component';
import { Action, State } from 'vuex-class';
import CreatePerformanceCurveLibraryDialog
    from './performance-curve-editor-dialogs/CreatePerformanceCurveLibraryDialog.vue';
import CreatePerformanceCurveDialog from './performance-curve-editor-dialogs/CreatePerformanceCurveDialog.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../shared/modals/CriterionLibraryEditorDialog.vue';
import {
    emptyPerformanceCurve,
    emptyPerformanceCurveLibrary,
    PerformanceCurve,
    PerformanceCurveLibrary,
} from '@/shared/models/iAM/performance';
import { SelectItem } from '@/shared/models/vue/select-item';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { any, prepend, clone, find, findIndex, isNil, propEq, reject, update } from 'ramda';
import { hasValue } from '@/shared/utils/has-value-util';
import {
    CreatePerformanceCurveLibraryDialogData,
    emptyCreatePerformanceLibraryDialogData,
} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Attribute } from '@/shared/models/iAM/attribute';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';

@Component({
    components: {
        CreatePerformanceCurveLibraryDialog,
        CreatePerformanceCurveDialog,
        EquationEditorDialog,
        CriterionLibraryEditorDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class PerformanceCurveEditor extends Vue {
    @State(state => state.performanceCurveModule.performanceCurveLibraries) statePerformanceCurveLibraries: PerformanceCurveLibrary[];
    @State(state => state.performanceCurveModule.selectedPerformanceCurveLibrary) stateSelectedPerformanceCurveLibrary: PerformanceCurveLibrary;
    @State(state => state.performanceCurveModule.scenarioPerformanceCurves) stateScenarioPerformanceCurves: PerformanceCurve[];
    @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;

    @Action('getPerformanceCurveLibraries') getPerformanceCurveLibrariesAction: any;
    @Action('selectPerformanceCurveLibrary') selectPerformanceCurveLibraryAction: any;
    @Action('upsertPerformanceCurveLibrary') upsertPerformanceCurveLibraryAction: any;
    @Action('deletePerformanceCurveLibrary') deletePerformanceCurveLibraryAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('updatePerformanceCurvesCriterionLibraries') updatePerformanceCurveCriterionLibrariesAction: any;
    @Action('getScenarioPerformanceCurves') getScenarioPerformanceCurvesAction: any;
    @Action('upsertScenarioPerformanceCurves') upsertScenarioPerformanceCurvesAction: any;

    gridSearchTerm = '';
    selectedPerformanceCurveLibrary: PerformanceCurveLibrary = clone(emptyPerformanceCurveLibrary);
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    hasScenario: boolean = false;
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = '';
    performanceCurveGridHeaders: DataTableHeader[] = [
        { text: 'Name', value: 'name', align: 'center', sortable: true, class: '', width: '' },
        { text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: '' },
        { text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: '' },
        { text: 'Criterion', value: 'criterionLibrary', align: 'center', sortable: false, class: '', width: '' },
        { text: '', value: '', align: 'center', sortable: false, class: '', width: '' },
    ];
    performanceCurveGridData: PerformanceCurve[] = [];
    attributeSelectItems: SelectItem[] = [];
    selectedPerformanceCurve: PerformanceCurve = clone(emptyPerformanceCurve);
    hasSelectedPerformanceCurve: boolean = false;
    createPerformanceCurveLibraryDialogData: CreatePerformanceCurveLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);
    equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
    showCreatePerformanceCurveDialog = false;
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    rules: InputValidationRules = clone(rules);
    uuidNIL: string = getBlankGuid();
    currentUrl: string = window.location.href;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getPerformanceCurveLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.PerformanceCurve) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;

                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.setErrorMessageAction({ message: 'Unable to identify selected scenario.' });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioPerformanceCurvesAction(vm.selectedScenarioId);
            }
        });
    }

    mounted() {
        this.setAttributeSelectItems();
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('statePerformanceCurveLibraries')
    onStatePerformanceCurveLibrariesChanged() {
        this.librarySelectItems = this.statePerformanceCurveLibraries
            .map((library: PerformanceCurveLibrary) => ({
                text: library.name,
                value: library.id,
            }));
    }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectPerformanceCurveLibraryAction(this.librarySelectItemValue);
    }

    @Watch('stateSelectedPerformanceCurveLibrary')
    onStateSelectedPerformanceCurveLibraryChanged() {
        this.selectedPerformanceCurveLibrary = clone(this.stateSelectedPerformanceCurveLibrary);
    }

    @Watch('selectedPerformanceCurveLibrary')
    onSelectedPerformanceCurveLibraryChanged() {
        this.hasSelectedLibrary = this.selectedPerformanceCurveLibrary.id !== this.uuidNIL;

        if (this.hasScenario) {
            this.performanceCurveGridData = this.selectedPerformanceCurveLibrary.performanceCurves
                .map((performanceCurve: PerformanceCurve) => ({
                    ...performanceCurve,
                    id: getNewGuid(),
                }));
        } else {
            this.performanceCurveGridData = clone(this.selectedPerformanceCurveLibrary.performanceCurves);
        }
    }

    @Watch('stateNumericAttributes')
    onStateNumericAttributesChanged() {
        this.setAttributeSelectItems();
    }

    @Watch('stateScenarioPerformanceCurves')
    onStateScenarioPerformanceCurvesChanged() {
        if (this.hasScenario) {
            this.performanceCurveGridData = clone(this.stateScenarioPerformanceCurves);
        }
    }

    @Watch('performanceCurveGridData')
    onPerformanceCurveGridDataChanged() {
        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('', this.performanceCurveGridData, this.stateScenarioPerformanceCurves)
            : hasUnsavedChangesCore('',
                {...clone(this.selectedPerformanceCurveLibrary), performanceCurves: clone(this.performanceCurveGridData)},
                this.stateSelectedPerformanceCurveLibrary);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    setAttributeSelectItems() {
        if (hasValue(this.stateNumericAttributes)) {
            this.attributeSelectItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }));
        }
    }

    onShowCreatePerformanceCurveLibraryDialog(createAsNewLibrary: boolean) {
        this.createPerformanceCurveLibraryDialogData = {
            showDialog: true,
            performanceCurves: createAsNewLibrary ? this.performanceCurveGridData : [],
        };
    }

    onSubmitCreatePerformanceCurveLibraryDialogResult(performanceCurveLibrary: PerformanceCurveLibrary) {
        this.createPerformanceCurveLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);

        if (!isNil(performanceCurveLibrary)) {
            this.upsertPerformanceCurveLibraryAction(performanceCurveLibrary);
        }
    }

    onSubmitCreatePerformanceCurveDialogResult(newPerformanceCurve: PerformanceCurve) {
        this.showCreatePerformanceCurveDialog = false;

        if (!isNil(newPerformanceCurve)) {
            this.performanceCurveGridData = prepend(newPerformanceCurve, this.performanceCurveGridData);
        }
    }

    onEditPerformanceCurveProperty(id: string, property: string, value: any) {
        if (any(propEq('id', id), this.performanceCurveGridData)) {
            const performanceCurve: PerformanceCurve = find(
                propEq('id', id), this.performanceCurveGridData,
            ) as PerformanceCurve;

            this.performanceCurveGridData = update(
                findIndex(propEq('id', performanceCurve.id), this.performanceCurveGridData),
                setItemPropertyValue(property, value, performanceCurve) as PerformanceCurve,
                this.performanceCurveGridData,
            );
        }
    }

    onShowEquationEditorDialog(performanceCurveId: string) {
        this.selectedPerformanceCurve = find(propEq('id', performanceCurveId), this.performanceCurveGridData) as PerformanceCurve;

        if (!isNil(this.selectedPerformanceCurve)) {
            this.hasSelectedPerformanceCurve = true;

            this.equationEditorDialogData = { showDialog: true, equation: this.selectedPerformanceCurve.equation };
        }
    }

    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.hasSelectedPerformanceCurve) {
            this.performanceCurveGridData = update(
                findIndex(propEq('id', this.selectedPerformanceCurve.id), this.performanceCurveGridData),
                { ...this.selectedPerformanceCurve, equation: equation },
                this.performanceCurveGridData,
            );
        }

        this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
        this.hasSelectedPerformanceCurve = false;
    }

    onEditPerformanceCurveCriterionLibrary(performanceCurveId: string) {
        this.selectedPerformanceCurve = find(
            propEq('id', performanceCurveId), this.performanceCurveGridData,
        ) as PerformanceCurve;

        if (!isNil(this.selectedPerformanceCurve)) {
            this.hasSelectedPerformanceCurve = true;

            this.criterionLibraryEditorDialogData = {
                showDialog: true,
                libraryId: this.selectedPerformanceCurve.criterionLibrary.id,
                isCallFromScenario: this.hasScenario,
                isCriterionForLibrary: !this.hasScenario
            };
        }
    }

    onSubmitCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.hasSelectedPerformanceCurve) {
            this.performanceCurveGridData = update(
                findIndex(propEq('id', this.selectedPerformanceCurve.id),
                    this.performanceCurveGridData),
                { ...this.selectedPerformanceCurve, criterionLibrary: criterionLibrary },
                this.performanceCurveGridData,
            );
        }

        this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
        this.hasSelectedPerformanceCurve = false;
    }

    onRemovePerformanceCurve(performanceCurveId: string) {
        this.performanceCurveGridData = reject(propEq('id', performanceCurveId),
            this.performanceCurveGridData);
    }

    onUpsertScenarioPerformanceCurves() {
        this.upsertScenarioPerformanceCurvesAction({scenarioPerformanceCurves: this.performanceCurveGridData, scenarioId: this.selectedScenarioId})
        .then(() => this.librarySelectItemValue = null);
    }

    onUpsertPerformanceCurveLibrary() {
        const performanceCurveLibrary: PerformanceCurveLibrary = {
            ...clone(this.selectedPerformanceCurveLibrary),
            performanceCurves: clone(this.performanceCurveGridData)
        };
        this.upsertPerformanceCurveLibraryAction(performanceCurveLibrary);
    }

    onDiscardChanges() {
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.performanceCurveGridData = clone(this.stateScenarioPerformanceCurves);
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
            this.librarySelectItemValue = null;
            this.deletePerformanceCurveLibraryAction(this.selectedPerformanceCurveLibrary.id);
        }
    }

    disableCrudButton() {
        const dataIsValid: boolean = this.performanceCurveGridData
            .every((performanceCurve: PerformanceCurve) => {
                return this.rules['generalRules'].valueIsNotEmpty(performanceCurve.name) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(performanceCurve.attribute) === true;
            });

        if (this.hasSelectedLibrary) {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedPerformanceCurveLibrary.name) === true &&
                dataIsValid && this.hasUnsavedChanges);
        }

        return !(dataIsValid && this.hasUnsavedChanges);
    }
}
</script>

<style>
.equation-name-text-field-output {
    margin-left: 10px;
}

.attribute-text-field-output {
    margin-left: 15px;
}

.header-height {
    height: 45px;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
