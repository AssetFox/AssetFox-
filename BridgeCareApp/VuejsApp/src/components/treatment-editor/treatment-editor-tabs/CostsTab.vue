<template>
    <v-layout class="costs-tab-content">
        <v-flex xs12>              
            <div class="costs-data-table">                
                <v-data-table
                    hide-default-header             
                    :headers="costsGridHeaders"
                    sort-icon=$vuetify.icons.ghd-table-sort
                    :items="costsGridData"
                    class="elevation-1 v-table__overflow ghd-padding-top"
                    hide-actions
                >
                    <template slot="items" slot-scope="props">
                        <tr style="border:none">
                            <td xs5>                            
                                <v-layout xs6 align-center>                                
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="width:95%">Equation</v-subheader>
                                    <v-btn
                                        @click="
                                            onShowCostEquationEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>                                
                                </v-layout>
                                <v-layout xs6 align-center>  
                                    <v-textarea
                                        class="ghd-control-border ghd-control-text-sm"
                                        full-width
                                        no-resize
                                        outline
                                        readonly
                                        rows="3"
                                        v-model="props.item.equation.expression"
                                    >                                
                                    </v-textarea>  
                                </v-layout>                          
                            </td>
                            <td xs5>
                                <v-layout xs6 align-center>
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="width:95%">Criteria</v-subheader>
                                    <v-btn
                                        @click="
                                            onShowCostCriterionEditorDialog(
                                                props.item,
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                    </v-btn>
                                </v-layout> 
                                <v-layout xs6 align-center>              
                                    <v-textarea
                                        class="ghd-control-border ghd-control-text-sm"
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
                                    </v-textarea>
                                </v-layout>
                            </td>     
                            <td xs2>
                                <v-layout align-start>
                                    <v-btn
                                        @click="onRemoveCost(props.item.id)"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </v-layout>                   
                            </td>
                        </tr>
                    </template>
                </v-data-table>
            </div>
            <v-btn @click="onAddCost" class="ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border" >Add Cost</v-btn>
            <v-chip class="ma-2 ara-blue" @click="showExampleFunction">
                Equation - Use Max(,) to enforce minimum costs
            </v-chip>            
        </v-flex>

        <CostEquationEditorDialog
            :dialogData="costEquationEditorDialogData"
            :isFromPerformanceCurveEditor=false
            @submit="onSubmitCostEquationEditorDialogResult"
        />

        <GeneralCriterionEditorDialog
            :dialogData="costCriterionEditorDialogData"
            @submit="onSubmitCostCriterionEditorDialogResult"
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
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import { Equation } from '@/shared/models/iAM/equation';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';

@Component({
    components: {
        GeneralCriterionEditorDialog,
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
            text: '',
            value: 'equation',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: '',
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
    costCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
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

    onShowCostCriterionEditorDialog(cost: TreatmentCost) {
        this.selectedCostForEquationOrCriteriaEdit = clone(cost);

        this.costCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: cost.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onSubmitCostCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        this.costCriterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (
            !isNil(criterionExpression) &&
            this.selectedCostForEquationOrCriteriaEdit.id !== this.uuidNIL
        ) {
            if(this.selectedCostForEquationOrCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedCostForEquationOrCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.$emit(
                'onModifyCost',
                setItemPropertyValue(
                    'criterionLibrary',
                    {...this.selectedCostForEquationOrCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary,
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
    min-width: 1100px;
}

.costs-data-table {
    overflow-y: auto;
    border: 1px solid #999999 !important;
    min-width: 1000px;
}

.costs-data-table table.v-table thead tr{
    height: 0px !important;
    border: none !important;
}
</style>
