<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout justify-center>
                <v-flex xs3>
                    <v-btn
                        @click='onShowCreateTreatmentLibraryDialog(false)'
                        class='ara-blue-bg white--text'
                        v-show='!hasScenario'
                    >
                        New Library
                    </v-btn>
                    <v-select
                        :items='librarySelectItems'
                        class='treatment-library-select'
                        label='Select a Treatment Library'
                        outline
                        v-if='!hasSelectedLibrary || hasScenario'
                        v-model='librarySelectItemValue'
                    >
                    </v-select>
                    <v-text-field
                        v-if='hasSelectedLibrary && !hasScenario'
                        label='Treatment Library Name'
                        v-model='selectedTreatmentLibrary.name'
                        :rules="[rules['generalRules'].valueIsNotEmpty]"
                    >
                        <template slot='append'>
                            <v-btn
                                @click='librarySelectItemValue = null'
                                class='ara-orange'
                                icon
                            >
                                <v-icon>fas fa-caret-left</v-icon>
                            </v-btn>
                        </template>
                    </v-text-field>
                    <div v-if='hasSelectedLibrary && !hasScenario'>
                        Owner:
                        {{
                            selectedTreatmentLibrary.owner
                                ? selectedTreatmentLibrary.owner
                                : '[ No Owner ]'
                        }}
                    </div>
                    <v-checkbox
                        class='sharing'
                        label='Shared'
                        v-if='hasSelectedLibrary && !hasScenario'
                        v-model='selectedTreatmentLibrary.shared'
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <div class='treatments-div'>
                <v-layout justify-center row>
                    <v-flex xs3>
                        <v-btn
                            @click='showCreateTreatmentDialog = true'
                            class='ara-blue-bg white--text'
                        >
                            Add Treatment
                        </v-btn>
                        <v-select
                            :items='treatmentSelectItems'
                            label='Select a Treatment'
                            @change='keepActiveTab = false'
                            outline
                            v-model='treatmentSelectItemValue'
                            v-if='!hasSelectedTreatment'
                        >
                        </v-select>
                        <v-text-field
                            v-if='hasSelectedTreatment'
                            label='Treatment Name'
                            v-model='selectedTreatment.name'
                            :rules="[rules['generalRules'].valueIsNotEmpty]"
                        >
                            <template slot='append'>
                                <v-btn
                                    @click='treatmentSelectItemValue = null'
                                    class='ara-orange'
                                    icon
                                >
                                    <v-icon>fas fa-caret-left</v-icon>
                                </v-btn>
                            </template>
                        </v-text-field>
                    </v-flex>
                    <v-flex xs9>
                        <div v-show='selectedTreatment.id !== uuidNIL'>
                            <v-tabs v-model='activeTab'>
                                <v-tab
                                    :key='index'
                                    @click='activeTab = index'
                                    ripple
                                    v-for='(treatmentTab,
                                    index) in treatmentTabs'
                                >
                                    {{ treatmentTab }}
                                </v-tab>
                                <v-tabs-items v-model='activeTab'>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text
                                                class='card-tab-content'
                                            >
                                                <TreatmentDetailsTab
                                                    :selectedTreatmentDetails='selectedTreatmentDetails'
                                                    :rules='rules'
                                                    :callFromScenario='hasScenario'
                                                    :callFromLibrary='!hasScenario'
                                                    @onModifyTreatmentDetails='modifySelectedTreatmentDetails'
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text
                                                class='card-tab-content'
                                            >
                                                <CostsTab
                                                    :selectedTreatmentCosts='selectedTreatment.costs'
                                                    :callFromScenario='hasScenario'
                                                    :callFromLibrary='!hasScenario'
                                                    @onAddCost='addSelectedTreatmentCost'
                                                    @onModifyCost='modifySelectedTreatmentCost'
                                                    @onRemoveCost='removeSelectedTreatmentCost'
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text
                                                class='card-tab-content'
                                            >
                                                <ConsequencesTab
                                                    :selectedTreatmentConsequences='selectedTreatment.consequences'
                                                    :rules='rules'
                                                    :callFromScenario='hasScenario'
                                                    :callFromLibrary='!hasScenario'
                                                    @onAddConsequence='addSelectedTreatmentConsequence'
                                                    @onModifyConsequence='modifySelectedTreatmentConsequence'
                                                    @onRemoveConsequence='removeSelectedTreatmentConsequence'
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                    <v-tab-item>
                                        <v-card>
                                            <v-card-text class='card-tab-content'>
                                                <BudgetsTab :selectedTreatmentBudgets='selectedTreatment.budgetIds'
                                                            :isNewTreatment='selectedTreatment.isNew'
                                                            @onModifyBudgets='modifySelectedTreatmentBudgets' />
                                            </v-card-text>
                                        </v-card>
                                    </v-tab-item>
                                </v-tabs-items>
                            </v-tabs>
                        </div>
                    </v-flex>
                </v-layout>
            </div>
        </v-flex>
        <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
        <v-flex v-show='hasSelectedLibrary && !hasScenario' xs12>
            <v-layout justify-center>
                <v-flex xs6>
                    <v-textarea
                        label='Description'
                        no-resize
                        outline
                        rows='4'
                        v-model='selectedTreatmentLibrary.description'
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12>
            <v-layout justify-end row v-show='hasSelectedLibrary || hasScenario'>
                <v-btn
                    @click='onUpsertScenarioTreatments'
                    class='ara-blue-bg white--text'
                    v-show='hasScenario'
                    :disabled='disableCrudButton() || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn
                    @click='onUpsertTreatmentLibrary()'
                    class='ara-blue-bg white--text'
                    v-show='!hasScenario'
                    :disabled='disableCrudButton() || !hasUnsavedChanges'
                >
                    Update Library
                </v-btn>
                <v-btn
                    @click='onShowCreateTreatmentLibraryDialog(true)'
                    class='ara-blue-bg white--text'
                    :disabled='disableCrudButton() || !hasUnsavedChanges'
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click='onShowConfirmDeleteAlert'
                    class='ara-orange-bg white--text'
                    v-show='!hasScenario'
                    :disabled='!hasSelectedLibrary'
                >
                    Delete Library
                </v-btn>
                <v-btn :disabled='!hasUnsavedChanges'
                    @click='onDiscardChanges'
                    class='ara-orange-bg white--text'
                    v-show='hasScenario'
                >
                    Discard Changes
                </v-btn>
            </v-layout>
        </v-flex>

        <ConfirmDeleteAlert
            :dialogData='confirmBeforeDeleteAlertData'
            @submit='onSubmitConfirmDeleteAlertResult'
        />

        <CreateTreatmentLibraryDialog
            :dialogData='createTreatmentLibraryDialogData'
            @submit='onSubmitCreateTreatmentLibraryDialogResult'
        />

        <CreateTreatmentDialog
            :showDialog='showCreateTreatmentDialog'
            @submit='onAddTreatment'
        />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import CreateTreatmentLibraryDialog
    from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
import { SelectItem } from '@/shared/models/vue/select-item';
import {
    CreateTreatmentLibraryDialogData,
    emptyCreateTreatmentLibraryDialogData,
} from '@/shared/models/modals/create-treatment-library-dialog-data';
import {
    emptyConsequence,
    emptyTreatment,
    emptyTreatmentDetails,
    emptyTreatmentLibrary,
    Treatment,
    TreatmentConsequence,
    TreatmentCost,
    TreatmentDetails,
    TreatmentLibrary,
} from '@/shared/models/iAM/treatment';
import CreateTreatmentDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentDialog.vue';
import {
    any,
    append,
    clone,
    find,
    findIndex,
    isNil,
    prepend,
    propEq,
    reject,
    update,
} from 'ramda';
import TreatmentDetailsTab from '@/components/treatment-editor/treatment-editor-tabs/TreatmentDetailsTab.vue';
import CostsTab from '@/components/treatment-editor/treatment-editor-tabs/CostsTab.vue';
import ConsequencesTab from '@/components/treatment-editor/treatment-editor-tabs/ConsequencesTab.vue';
import BudgetsTab from '@/components/treatment-editor/treatment-editor-tabs/BudgetsTab.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { SimpleBudgetDetail } from '@/shared/models/iAM/investment';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';

@Component({
    components: {
        BudgetsTab,
        ConsequencesTab,
        CostsTab,
        TreatmentDetailsTab,
        CreateTreatmentDialog,
        CreateTreatmentLibraryDialog,
        ConfirmDeleteAlert: Alert,
    },
})
export default class TreatmentEditor extends Vue {
    @State(state => state.treatmentModule.treatmentLibraries)
    stateTreatmentLibraries: TreatmentLibrary[];
    @State(state => state.treatmentModule.selectedTreatmentLibrary)
    stateSelectedTreatmentLibrary: TreatmentLibrary;
    @State(state => state.treatmentModule.scenarioSelectableTreatments)
    stateScenarioSelectableTreatments: Treatment[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;

    @Action('getTreatmentLibraries') getTreatmentLibrariesAction: any;
    @Action('selectTreatmentLibrary') selectTreatmentLibraryAction: any;
    @Action('upsertTreatmentLibrary') upsertTreatmentLibraryAction: any;
    @Action('deleteTreatmentLibrary') deleteTreatmentLibraryAction: any;
    @Action('getScenarioSimpleBudgetDetails')
    getScenarioSimpleBudgetDetailsAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Action('getScenarioSelectableTreatments')
    getScenarioSelectableTreatmentsAction: any;
    @Action('upsertScenarioSelectableTreatments')
    upsertScenarioSelectableTreatmentsAction: any;

    selectedTreatmentLibrary: TreatmentLibrary = clone(emptyTreatmentLibrary);
    treatments: Treatment[] = [];
    selectedScenarioId: string = getBlankGuid();
    hasSelectedLibrary: boolean = false;
    librarySelectItems: SelectItem[] = [];
    librarySelectItemValue: string | null = null;
    treatmentSelectItems: SelectItem[] = [];
    treatmentSelectItemValue: string | null = null;
    selectedTreatment: Treatment = clone(emptyTreatment);
    selectedTreatmentDetails: TreatmentDetails = clone(emptyTreatmentDetails);
    activeTab: number = 0;
    treatmentTabs: string[] = ['details', 'costs', 'consequences'];
    createTreatmentLibraryDialogData: CreateTreatmentLibraryDialogData = clone(
        emptyCreateTreatmentLibraryDialogData,
    );
    showCreateTreatmentDialog: boolean = false;
    confirmBeforeDeleteAlertData: AlertData = clone(emptyAlertData);
    hasSelectedTreatment: boolean = false;
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    keepActiveTab: boolean = false;
    hasScenario: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getTreatmentLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.Treatment) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;
                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.setErrorMessageAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioSelectableTreatmentsAction(vm.selectedScenarioId);

                vm.treatmentTabs = [...vm.treatmentTabs, 'budgets'];
                vm.getScenarioSimpleBudgetDetailsAction({
                    scenarioId: vm.selectedScenarioId,
                });
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateTreatmentLibraries')
    onStateTreatmentLibrariesChanged() {
        this.librarySelectItems = this.stateTreatmentLibraries.map(
            (library: TreatmentLibrary) => ({
                text: library.name,
                value: library.id.toString(),
            }),
        );
    }

    @Watch('librarySelectItemValue')
    onLibrarySelectItemValueChanged() {
        this.selectTreatmentLibraryAction({
            libraryId: this.librarySelectItemValue,
        });
    }

    @Watch('stateSelectedTreatmentLibrary')
    onStateSelectedTreatmentLibraryChanged() {
        this.selectedTreatmentLibrary = clone(
            this.stateSelectedTreatmentLibrary,
        );
    }

    @Watch('selectedTreatmentLibrary')
    onSelectedTreatmentLibraryChanged() {
        this.hasSelectedLibrary = this.selectedTreatmentLibrary.id !== this.uuidNIL;

        if (this.hasScenario) {
            this.treatments = this.selectedTreatmentLibrary.treatments
                .map((treatment: Treatment) => ({
                    ...treatment,
                    id: getNewGuid(),
                    consequences: treatment.consequences.map((consequence: TreatmentConsequence) => ({
                        ...consequence,
                        id: getNewGuid(),
                    })),
                    costs: treatment.costs.map((cost: TreatmentCost) => ({
                        ...cost,
                        id: getNewGuid(),
                    })),
                    isNew: true
                }));
        } else {
            this.treatments = clone(this.selectedTreatmentLibrary.treatments);
        }
    }

    @Watch('stateScenarioSelectableTreatments')
    onStateScenarioSelectableTreatmentsChanged() {
        if (this.hasScenario) {
            this.treatments = clone(this.stateScenarioSelectableTreatments);
        }
    }

    @Watch('treatments')
    onSelectedScenarioTreatmentsChanged() {
        this.treatmentSelectItems = this.treatments.map((treatment: Treatment) => ({
            text: treatment.name,
            value: treatment.id,
        }));

        const hasUnsavedChanges: boolean = this.hasScenario
            ? hasUnsavedChangesCore('treatment', this.treatments, this.stateScenarioSelectableTreatments)
            : hasUnsavedChangesCore('treatment',
                {...clone(this.selectedTreatmentLibrary), treatments: clone(this.treatments)},
                this.stateSelectedTreatmentLibrary
            );
        this.setHasUnsavedChangesAction({value: hasUnsavedChanges})

        if (this.selectedTreatment.id !== this.uuidNIL &&
            any(propEq('id', this.selectedTreatment.id), this.treatments)) {
            this.selectedTreatment = find(propEq('id', this.selectedTreatment.id), this.treatments) as Treatment;
        } else {
            this.treatmentSelectItemValue = null;
        }
    }

    @Watch('treatmentSelectItemValue')
    onTreatmentSelectItemValueChanged() {
        this.selectedTreatment = any(propEq('id', this.treatmentSelectItemValue), this.treatments)
            ? find(propEq('id', this.treatmentSelectItemValue), this.treatments) as Treatment
            : clone(emptyTreatment);

        if (!this.keepActiveTab) {
            this.activeTab = 0;
        }
        this.keepActiveTab = true;
    }

    @Watch('selectedTreatment')
    onSelectedTreatmentChanged() {
        this.hasSelectedTreatment = this.selectedTreatment.id !== this.uuidNIL;

        this.selectedTreatmentDetails = {
            description: this.selectedTreatment.description,
            shadowForSameTreatment: this.selectedTreatment.shadowForSameTreatment,
            shadowForAnyTreatment: this.selectedTreatment.shadowForAnyTreatment,
            criterionLibrary: this.selectedTreatment.criterionLibrary
        };
    }

    onShowCreateTreatmentLibraryDialog(createAsNewLibrary: boolean) {
        this.createTreatmentLibraryDialogData = {
            showDialog: true,
            selectedTreatmentLibraryTreatments: createAsNewLibrary ? this.treatments : [],
        };
    }

    onSubmitCreateTreatmentLibraryDialogResult(library: TreatmentLibrary) {
        this.createTreatmentLibraryDialogData = clone(emptyCreateTreatmentLibraryDialogData,);

        if (!isNil(library)) {
            var localObject = clone(library);
            localObject.treatments = clone(this.selectedScenarioTreatments);
            this.upsertTreatmentLibraryAction({ library: localObject });
        }
    }

    onUpsertScenarioTreatments() {
        this.upsertScenarioSelectableTreatmentsAction({ scenarioSelectableTreatments: this.treatments, scenarioId: this.selectedScenarioId, })
            .then(() => this.librarySelectItemValue = null);
    }

    onUpsertTreatmentLibrary() {
        const treatmentLibrary: TreatmentLibrary = {
            ...clone(this.selectedTreatmentLibrary),
            treatments: clone(this.treatments)
        };
        this.upsertTreatmentLibraryAction({library: treatmentLibrary});
    }

    onAddTreatment(newTreatment: Treatment) {
        this.showCreateTreatmentDialog = false;

        if (!isNil(newTreatment)) {
            this.treatments = append(newTreatment, this.treatments,);
            setTimeout(() => (this.treatmentSelectItemValue = newTreatment.id));
        }
    }

    modifySelectedTreatmentDetails(treatmentDetails: TreatmentDetails) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                description: treatmentDetails.description,
                shadowForAnyTreatment: treatmentDetails.shadowForAnyTreatment,
                shadowForSameTreatment: treatmentDetails.shadowForSameTreatment,
                criterionLibrary: treatmentDetails.criterionLibrary,
            });
        }
    }

    addSelectedTreatmentCost(newCost: TreatmentCost) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: prepend(newCost, this.selectedTreatment.costs),
            });
        }
    }

    modifySelectedTreatmentCost(modifiedCost: TreatmentCost) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: update(
                    findIndex(propEq('id', modifiedCost.id), this.selectedTreatment.costs,),
                    modifiedCost,
                    this.selectedTreatment.costs,
                ),
            });
        }
    }

    removeSelectedTreatmentCost(costId: string) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                costs: reject(propEq('id', costId), this.selectedTreatment.costs,),
            });
        }
    }

    addSelectedTreatmentConsequence(newConsequence: TreatmentConsequence) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: prepend(newConsequence, this.selectedTreatment.consequences,),
            });
        }
    }

    modifySelectedTreatmentConsequence(modifiedConsequence: TreatmentConsequence,) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: update(
                    findIndex(propEq('id', modifiedConsequence.id), this.selectedTreatment.consequences,),
                    modifiedConsequence,
                    this.selectedTreatment.consequences,
                ),
            });
        }
    }

    removeSelectedTreatmentConsequence(consequenceId: string) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                consequences: reject(propEq('id', consequenceId), this.selectedTreatment.consequences,),
            });
        }
    }

    modifySelectedTreatmentBudgets(simpleBudgetDetails: SimpleBudgetDetail[]) {
        if (this.hasSelectedTreatment) {
            this.modifySelectedTreatment({
                ...clone(this.selectedTreatment),
                budgetIds: getPropertyValues('id', simpleBudgetDetails,) as string[],
            });
        }
    }

    modifySelectedTreatment(treatment: Treatment) {
        this.treatments = update(
            findIndex(propEq('id', this.selectedTreatment.id), this.treatments),
            treatment,
            this.treatments
        );
    }

    onDiscardChanges() {
        this.treatmentSelectItemValue = null;
        this.librarySelectItemValue = null;
        setTimeout(() => {
            if (this.hasScenario) {
                this.treatments = clone(this.stateScenarioSelectableTreatments);
            }
        });
    }

    onShowConfirmDeleteAlert() {
        this.confirmBeforeDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onSubmitConfirmDeleteAlertResult(submit: boolean) {
        this.confirmBeforeDeleteAlertData = clone(emptyAlertData);

        if (submit) {
            this.librarySelectItemValue = null;
            this.deleteTreatmentLibraryAction({ libraryId: this.selectedTreatmentLibrary.id, });
        }
    }

    disableCrudButton() {
        const allDataIsValid: boolean = this.treatments.every((treatment: Treatment) => {
            const allSubDataIsValid: boolean = treatment.consequences.every((consequence: TreatmentConsequence) => {
                    return (this.rules['generalRules'].valueIsNotEmpty(consequence.attribute,) === true &&
                        this.rules['treatmentRules']
                            .hasChangeValueOrEquation(consequence.changeValue, consequence.equation.expression,) === true
                    );
                },
            );

            return allSubDataIsValid && this.rules['generalRules'].valueIsNotEmpty(treatment.name) === true &&
                this.rules['generalRules'].valueIsNotEmpty(treatment.shadowForAnyTreatment) === true &&
                this.rules['generalRules'].valueIsNotEmpty(treatment.shadowForSameTreatment) === true;
        });

        if (this.hasSelectedLibrary) {
            return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedTreatmentLibrary.name) === true &&
                allDataIsValid);
        }

        return !allDataIsValid;
    }
}
</script>

<style>
.treatment-editor-container {
    height: 730px;
    overflow-x: hidden;
    overflow-y: auto;
}

.treatments-div {
    height: 355px;
}

.card-tab-content {
    height: 305px;
    overflow-x: hidden;
    overflow-y: auto;
}

.treatment-library-select {
    height: 60px;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}
</style>
