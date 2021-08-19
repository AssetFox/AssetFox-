<template>
    <v-layout class="costs-tab-content">
        <v-flex xs12>
            <v-btn @click="onAddCost" class="ara-blue-bg white--text"
                >Add Cost</v-btn
            >
            <div class="costs-data-table">
                <v-chip class="ma-2 ara-blue" @click="showExampleFunction">
                    Equation - Use Max(,) to enforce minimum costs
                </v-chip>
                <v-data-table
                    :headers="costsGridHeaders"
                    :items="costsGridData"
                    class="elevation-1 fixed-header v-table__overflow"
                    hide-actions
                >
                    <template slot="items" slot-scope="props">
                        <td>
                            <v-textarea
                                full-width
                                no-resize
                                outline
                                readonly
                                rows="3"
                                v-model="props.item.equation.expression"
                            >
                                <template slot="append-outer">
                                    <v-btn
                                        @click="
                                            onShowCostEquationEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-textarea
                                full-width
                                no-resize
                                outline
                                readonly
                                rows="3"
                                v-model="
                                    props.item.criterionLibrary
                                        .mergedCriteriaExpression
                                "
                            >
                                <template slot="append-outer">
                                    <v-btn
                                        @click="
                                            onShowCostCriterionLibraryEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>
                        </td>
                        <td>
                            <v-layout align-start>
                                <v-btn
                                    @click="onRemoveCost(props.item.id)"
                                    class="ara-orange"
                                    icon
                                >
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <CostEquationEditorDialog
            :dialogData="costEquationEditorDialogData"
            @submit="onSubmitCostEquationEditorDialogResult"
        />

        <CostCriterionLibraryEditorDialog
            :dialogData="costCriterionLibraryEditorDialogData"
            @submit="onSubmitCostCriterionLibraryEditorDialogResult"
        />

        <Alert :dialogData="alertData" @submit="onSubmitAlertResult" />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { emptyCost, TreatmentCost } from '@/shared/models/iAM/treatment';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../../shared/modals/CriterionLibraryEditorDialog.vue';
import { Equation } from '@/shared/models/iAM/equation';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';

@Component({
    components: {
        CostCriterionLibraryEditorDialog: CriterionLibraryEditorDialog,
        CostEquationEditorDialog: EquationEditorDialog,
        Alert
    },
})
export default class CostsTab extends Vue {
    @Prop() selectedTreatmentCosts: TreatmentCost[];
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    costsGridHeaders: DataTableHeader[] = [
        {
            text: 'Equation',
            value: 'equation',
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
            width: '',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '100px',
        },
    ];
    costsGridData: TreatmentCost[] = [];
    costEquationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    costCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(
        emptyCriterionLibraryEditorDialogData,
    );
    selectedCostForEquationOrCriteriaEdit: TreatmentCost = clone(emptyCost);
    uuidNIL: string = getBlankGuid();
    alertData: AlertData = clone(emptyAlertData);

    @Watch('selectedTreatmentCosts')
    onSelectedTreatmentCostsChanged() {
        this.costsGridData = clone(this.selectedTreatmentCosts);
    }

    onAddCost() {
        const newCost: TreatmentCost = { ...emptyCost, id: getNewGuid() };
        this.$emit('onAddCost', newCost);
    }

    onShowCostEquationEditorDialog(cost: TreatmentCost) {
        this.selectedCostForEquationOrCriteriaEdit = clone(cost);

        this.costEquationEditorDialogData = {
            showDialog: true,
            equation: clone(cost.equation),
        };
    }

    onSubmitCostEquationEditorDialogResult(equation: Equation) {
        this.costEquationEditorDialogData = clone(
            emptyEquationEditorDialogData,
        );

        if (
            !isNil(equation) &&
            this.selectedCostForEquationOrCriteriaEdit.id !== this.uuidNIL
        ) {
            this.$emit(
                'onModifyCost',
                setItemPropertyValue(
                    'equation',
                    equation,
                    this.selectedCostForEquationOrCriteriaEdit,
                ),
            );
        }

        this.selectedCostForEquationOrCriteriaEdit = clone(emptyCost);
    }

    onShowCostCriterionLibraryEditorDialog(cost: TreatmentCost) {
        this.selectedCostForEquationOrCriteriaEdit = clone(cost);

        this.costCriterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: cost.criterionLibrary.id,
            isCallFromScenario: this.callFromScenario,
            isCriterionForLibrary: this.callFromLibrary
        };
    }

    onSubmitCostCriterionLibraryEditorDialogResult(
        criterionLibrary: CriterionLibrary,
    ) {
        this.costCriterionLibraryEditorDialogData = clone(
            emptyCriterionLibraryEditorDialogData,
        );

        if (
            !isNil(criterionLibrary) &&
            this.selectedCostForEquationOrCriteriaEdit.id !== this.uuidNIL
        ) {
            this.$emit(
                'onModifyCost',
                setItemPropertyValue(
                    'criterionLibrary',
                    criterionLibrary,
                    this.selectedCostForEquationOrCriteriaEdit,
                ),
            );
        }

        this.selectedCostForEquationOrCriteriaEdit = clone(emptyCost);
    }

    onRemoveCost(costId: string) {
        this.$emit('onRemoveCost', costId);
    }

    /**
     * Shows the Alert
     */
    showExampleFunction() {
        this.alertData = {
            showDialog: true,
            heading: 'Example',
            choice: false,
            message:
                'To use a minimum of $1000 for a $5/sq ft treatment, ' +
                'use Max(5*[DECK_AERA],1000)',
        };
    }

    onSubmitAlertResult(dummy: boolean) {
        this.alertData = clone(emptyAlertData);
    }
}
</script>

<style>
.costs-tab-content {
    height: 185px;
}

.costs-data-table {
    height: 215px;
    overflow-y: auto;
}
</style>
