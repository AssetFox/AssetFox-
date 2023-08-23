<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-layout justify-space-between>
                <v-flex xs4 class="ghd-constant-header">
                    <v-subheader class="ghd-md-gray ghd-control-label">Select a Summary Report</v-subheader>
                    <v-select
                        :items="librarySelectItems"
                        append-icon=$vuetify.icons.ghd-down
                        outline
                        v-model="librarySelectItemValue"
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>
                </v-flex>
                <v-flex>
                    <v-btn>Generate Report</v-btn>
                    <v-btn>Download Report</v-btn>
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs6>
            <v-layout style="height=12px;padding-bottom:0px;">
                <v-flex xs12 style="height=12px;padding-bottom:0px">
                    <v-subheader class="ghd-control-label ghd-md-gray">                             
                        Criteria
                    </v-subheader>
                </v-flex>
                <v-flex xs1 style="height=12px;padding-bottom:0px;padding-top:0px;">
                    <v-btn
                        id="SummaryReport-criteriaEditor-btn"
                        style="padding-right:20px !important;"
                        @click="
                            onShowCriterionEditorDialog
                        "
                        class="edit-icon ghd-control-label"
                        icon
                    >
                        <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-textarea
                id="SummaryReport-criteria-textArea"
                class="ghd-control-text ghd-control-border"
                style="padding-bottom: 0px; height: 90px;"
                no-resize
                outline
                readonly
                :rows=6
                v-model=mergedCriteriaExpression
            >
            </v-textarea>
            <!-- <v-checkbox
                id="SummaryReport-criteria-checkbox"
                style="padding-top: 0px; margin-top: 4px;"
                class="ghd-checkbox ghd-md-gray"
                label="Criteria is intentionally empty (MUST check to Save)" 
                v-model="criteriaIsIntentionallyEmpty"
                v-show="criteriaIsEmpty()"
            >
            </v-checkbox> -->
        </v-flex>
        <v-flex>
            <v-btn>Simulation Log</v-btn>
        </v-flex>
        <GeneralCriterionEditorDialog
            :dialogData="criterionEditorDialogData"
            @submit="onCriterionEditorDialogSubmit"
        />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { clone, contains } from 'ramda';
import { State } from 'vuex-class';
import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
import { getPropertyValues } from '@/shared/utils/getter-utils';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';

@Component({
    components: { GeneralCriterionEditorDialog },
})
export default class SummaryReports extends Vue {

    initializedBudgets: boolean = false;
    mergedCriteriaExpression: string = "";

    criterionEditorDialogData: GeneralCriterionEditorDialogData = clone(
        emptyGeneralCriterionEditorDialogData,
    );

    onShowCriterionEditorDialog() {
        this.criterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: this.mergedCriteriaExpression,
        };
    }
    onCriterionEditorDialogSubmit(criterionexpression: string) {
        this.criterionEditorDialogData = clone(
            emptyGeneralCriterionEditorDialogData,
        );

        // if (!isNil(criterionexpression)) {
        //     if(this.analysisMethod.criterionLibrary.id == getBlankGuid())
        //         this.analysisMethod.criterionLibrary.id = getNewGuid();
        //     this.analysisMethod = {
        //         ...this.analysisMethod,
        //         criterionLibrary: {...this.analysisMethod.criterionLibrary, mergedCriteriaExpression: criterionexpression} as CriterionLibrary,
        //     };
        // }
    }
}
</script>

<style>
.budgets-tab-content{
   min-width: 1000px;  
}

.budgets-data-table {
    min-width: 800px;
    width: 800px;
    height: 295px !important;
    overflow-y: auto;
}

.budgets-data-table .v-table tbody tr td{
    font-size: 14px !important;
    min-width: 80px;
}

.budgets-data-table .v-table thead tr th{
    width: 20px !important;
}

.budgets-data-table .v-table thead tr th .v-input {
    max-width: 40px !important;
} 
</style>
