<template>
    <v-row class="costs-tab-content">
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
                    <template slot="items" slot-scope="props" v-slot:item="props">
                        <tr style="border:none">
                            <td xs5>                            
                                <v-row xs6 align-center>                                
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
                                </v-row>
                                <v-row xs6 align-center>  
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
                                </v-row>                          
                            </td>
                            <td xs5>
                                <v-row xs6 align-center>
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
                                </v-row> 
                                <v-row xs6 align-center>              
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
                                </v-row>
                            </td>     
                            <td xs2>
                                <v-row align-start>
                                    <v-btn
                                        @click="onRemoveCost(props.item.id)"
                                        icon
                                    >
                                        <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                </v-row>                   
                            </td>
                        </tr>
                    </template>
                </v-data-table>
            </div>
            <v-btn @click="onAddCost" class="ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border" id="TreatmentCostsTab-AddCostBtn" >Add Cost</v-btn>
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
    </v-row>
</template>

<script lang="ts" setup>
import Vue, { shallowRef } from 'vue';
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
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';

    const emit = defineEmits(['submit', 'onAddCost', 'onModifyCost', 'onRemoveCost'])
    let store = useStore();
    let selectedTreatmentCosts = shallowRef<TreatmentCost[]>();
    let  callFromScenario: boolean;
    let  callFromLibrary: boolean;

    let costsGridHeaders: DataTableHeader[] = [
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
    let costsGridData: TreatmentCost[] | undefined = [];
    let costEquationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    let costCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );
    let selectedCostForEquationOrCriteriaEdit: TreatmentCost = clone(emptyCost);
    let uuidNIL: string = getBlankGuid();
    let alertData: AlertData = clone(emptyAlertData);

    watch(selectedTreatmentCosts, () => onSelectedTreatmentCostsChanged)
    function onSelectedTreatmentCostsChanged() {
        costsGridData = clone(selectedTreatmentCosts.value);
    }

    function onAddCost() {
        const newCost: TreatmentCost = { ...emptyCost, id: getNewGuid() };
        emit('onAddCost', newCost);
    }

    function onShowCostEquationEditorDialog(cost: TreatmentCost) {
        selectedCostForEquationOrCriteriaEdit = clone(cost);

        costEquationEditorDialogData = {
            showDialog: true,
            equation: clone(cost.equation),
        };
    }

    function onSubmitCostEquationEditorDialogResult(equation: Equation) {
        costEquationEditorDialogData = clone(
            emptyEquationEditorDialogData,
        );

        if (
            !isNil(equation) &&
            selectedCostForEquationOrCriteriaEdit.id !== uuidNIL
        ) {
            emit(
                'onModifyCost',
                setItemPropertyValue(
                    'equation',
                    equation,
                    selectedCostForEquationOrCriteriaEdit,
                ),
            );
        }

        selectedCostForEquationOrCriteriaEdit = clone(emptyCost);
    }

    function onShowCostCriterionEditorDialog(cost: TreatmentCost) {
        selectedCostForEquationOrCriteriaEdit = clone(cost);

        costCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: cost.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitCostCriterionEditorDialogResult(
        criterionExpression: string,
    ) {
        costCriterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        if (
            !isNil(criterionExpression) &&
            selectedCostForEquationOrCriteriaEdit.id !== uuidNIL
        ) {
            if(selectedCostForEquationOrCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedCostForEquationOrCriteriaEdit.criterionLibrary.id = getNewGuid();
            emit(
                'onModifyCost',
                setItemPropertyValue(
                    'criterionLibrary',
                    {...selectedCostForEquationOrCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary,
                    selectedCostForEquationOrCriteriaEdit,
                ),
            );
        }

        selectedCostForEquationOrCriteriaEdit = clone(emptyCost);
    }

    function onRemoveCost(costId: string) {
        emit('onRemoveCost', costId);
    }

    /**
     * Shows the Alert
     */
     function showExampleFunction() {
        alertData = {
            showDialog: true,
            heading: 'Example',
            choice: false,
            message:
                'To use a minimum of $1000 for a $5/sq ft treatment, ' +
                'use Max(5*[DECK_AERA],1000)',
        };
    }

    function onSubmitAlertResult(dummy: boolean) {
        alertData = clone(emptyAlertData);
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
