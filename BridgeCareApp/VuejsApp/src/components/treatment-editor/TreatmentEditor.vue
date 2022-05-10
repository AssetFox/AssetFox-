<template>
    <v-layout column>
        <v-flex>
            <v-layout>
                <v-flex xs3>
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment Library</v-subheader>
                    <v-select
                        :items='librarySelectItems'
                        class='ghd-control-border ghd-control-text ghd-control-width'
                        label='Select a Treatment Library'
                        outline                        
                        v-model='librarySelectItemValue'
                    >
                    </v-select>
                </v-flex>                    
                <!-- <v-flex>
                    <v-text-field xs3
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
                </v-flex> -->
                <v-flex xs3>                       
                    <v-subheader class="ghd-control-label ghd-md-gray">Treatment</v-subheader>
                    <v-select
                        :items='treatmentSelectItems'
                        class='ghd-control-border ghd-control-text ghd-control-width'
                        label='Select'
                        outline                        
                        v-model='treatmentSelectItemValue'
                    >
                    </v-select>
                </v-flex>
                <v-flex xs2 class="ara-text-field">
                    <div v-if='hasSelectedLibrary && !hasScenario'>
                        Owner: <v-label class="bold">{{ getOwnerUserName() || '[ No Owner ]' }}</v-label>
                    </div>
                </v-flex>
                <v-flex xs2>
                    <v-checkbox
                        class='sharing'
                        label='Shared'
                        v-if='hasSelectedLibrary && !hasScenario'
                        v-model='selectedTreatmentLibrary.isShared'
                    />                    
                </v-flex>
                <v-flex xs2>
                    <v-btn
                        @click='onShowCreateTreatmentLibraryDialog(false)'
                        class='ara-blue-bg white--text'
                        v-show='!hasScenario'
                    >
                        New Library
                    </v-btn>  
                </v-flex>
                <v-flex xs2>
                    <v-btn
                        @click='showCreateTreatmentDialog = true'
                        depressed
                        class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding'
                    >
                        Add Treatment
                    </v-btn>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-divider v-show='hasSelectedLibrary || hasScenario'></v-divider>
        <v-flex v-show='hasSelectedLibrary || hasScenario' xs12>
            <div class='treatments-div'>
                <v-layout row>
                    <v-flex xs12>
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
                                        <v-card style="border:none;">
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
                                                            :addTreatment='selectedTreatment.addTreatment'
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
                <v-flex xs12>
                    <v-subheader class="ghd-control-label ghd-md-gray">Description</v-subheader>
                    <v-textarea                        
                        class='ghd-control-border ghd-control-text'
                        no-resize
                        outline
                        rows='4'
                        :value='selectedTreatmentLibrary.description'
                        @input='selectedTreatmentLibrary = {...selectedTreatmentLibrary, description: $event}'
                    />
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs9>
            <v-layout justify-center row v-show='(hasSelectedLibrary || hasScenario) && hasSelectedTreatment'>
                <v-btn :disabled='!hasUnsavedChanges'
                    @click='onDiscardChanges'
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    depressed
                    v-show='hasScenario'
                >
                    Cancel
                </v-btn>
                <v-btn
                    @click='onShowConfirmDeleteAlert'
                    class='ghd-white-bg ghd-blue ghd-button-text'
                    depressed
                    v-show='!hasScenario'
                    :disabled='!hasLibraryEditPermission'
                >
                    Delete Library
                </v-btn>
                <v-btn
                    @click='onShowCreateTreatmentLibraryDialog(true)'
                    class='ghd-white-bg ghd-blue ghd-button-text ghd-blue-border ghd-text-padding'
                    :disabled='disableCrudButtons()'
                >
                    Create as New Library
                </v-btn>
                <v-btn
                    @click='onUpsertScenarioTreatments'
                    class='ghd-blue-bg ghd-white ghd-button-text'
                    depressed
                    v-show='hasScenario'
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'>
                    Save
                </v-btn>
                <v-btn
                    @click='onUpsertTreatmentLibrary'
                    class='ghd-blue-bg ghd-white ghd-button-text  ghd-text-padding'
                    depressed
                    v-show='!hasScenario'
                    :disabled='disableCrudButtonsResult || !hasLibraryEditPermission || !hasUnsavedChanges'
                >
                    Update Library
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
import { Action, State, Getter } from 'vuex-class';
import CreateTreatmentLibraryDialog from '@/components/treatment-editor/treatment-editor-dialogs/CreateTreatmentLibraryDialog.vue';
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
import { hasUnsavedChangesCore, isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getUserName } from '@/shared/utils/get-user-info';

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
    @State(state => state.investmentModule.scenarioSimpleBudgetDetails) stateScenarioSimpleBudgetDetails: SimpleBudgetDetail[];
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
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

    @Getter('getUserNameById') getUserNameByIdGetter: any;

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
    treatmentTabs: string[] = ['Treatment Details', 'Costs', 'Consequences'];
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
    budgets: SimpleBudgetDetail[] = [];
    hasCreatedLibrary: boolean = false;
    disableCrudButtonsResult: boolean = false;
    hasLibraryEditPermission: boolean = false;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getTreatmentLibrariesAction();

            if (to.path.indexOf(ScenarioRoutePaths.Treatment) !== -1) {
                vm.selectedScenarioId = to.query.scenarioId;
                if (vm.selectedScenarioId === vm.uuidNIL) {
                    vm.addErrorNotificationAction({
                        message: 'Found no selected scenario for edit',
                    });
                    vm.$router.push('/Scenarios/');
                }

                vm.hasScenario = true;
                vm.getScenarioSelectableTreatmentsAction(vm.selectedScenarioId);

                vm.treatmentTabs = [...vm.treatmentTabs, 'Budgets'];
                vm.getScenarioSimpleBudgetDetailsAction({
                    scenarioId: vm.selectedScenarioId,
                });
            }
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateScenarioSimpleBudgetDetails')
    onStateScenarioInvestmentLibraryChanged() {
        this.budgets = clone(this.stateScenarioSimpleBudgetDetails);
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

    @Watch('selectedTreatmentLibrary', {deep: true})
    onSelectedTreatmentLibraryChanged() {
        this.hasSelectedLibrary = this.selectedTreatmentLibrary.id !== this.uuidNIL;

        if (this.hasSelectedLibrary) {
            this.checkLibraryEditPermission();
            this.hasCreatedLibrary = false;
        }

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
                    budgetIds: getPropertyValues('id', this.budgets) as string[],                    
                    addTreatment: false
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
        this.setHasUnsavedChangesAction({value: hasUnsavedChanges});

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
            criterionLibrary: this.selectedTreatment.criterionLibrary,
            category: this.selectedTreatment.category,
            assetType: this.selectedTreatment.assetType,
        };
    }

    isSelectedTreatmentItem(treatmentId: string | number) {
        return isEqual(this.treatmentSelectItemValue, treatmentId.toString());
    }

    getOwnerUserName(): string {

        if (!this.hasCreatedLibrary) {
        return this.getUserNameByIdGetter(this.selectedTreatmentLibrary.owner);
        }
        
        return getUserName();
    }

    checkLibraryEditPermission() {
        this.hasLibraryEditPermission = this.isAdmin || this.checkUserIsLibraryOwner();
    }

    checkUserIsLibraryOwner() {
        return this.getUserNameByIdGetter(this.selectedTreatmentLibrary.owner) == getUserName();
    }

    onSetTreatmentSelectItemValue(treatmentId: string | number) {
        if (!isEqual(this.treatmentSelectItemValue, treatmentId.toString())) {
            this.treatmentSelectItemValue = treatmentId.toString();
        } else {
            this.treatmentSelectItemValue = null;
        }
    }

    onDeleteTreatment(treatmentId: string | number) {
        if (any(propEq('id', treatmentId.toString()), this.treatments)) {
            this.treatments = reject(propEq('id', treatmentId.toString()), this.treatments);
        }
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
            this.upsertTreatmentLibraryAction({ library: library });
            this.hasCreatedLibrary = true;
            this.librarySelectItemValue = library.name;
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
                category: treatmentDetails.category,
                assetType: treatmentDetails.assetType,
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

    disableCrudButtons() {
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

        this.disableCrudButtonsResult = !allDataIsValid;
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
    height: 550px;
}

.card-tab-content {
    height: 500px;
    overflow-x: hidden;
    overflow-y: auto;
    border: none;
}

.sharing label {
    padding-top: 0.5em;
}

.sharing {
    padding-top: 0;
    margin: 0;
}

.treatments-list {
    height: 308px;
    overflow-y: auto;
}

.selected-treatment-item {
    background: lightblue;
}
</style>
