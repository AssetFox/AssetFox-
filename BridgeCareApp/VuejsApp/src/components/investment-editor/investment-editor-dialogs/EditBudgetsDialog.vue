<template>
    <v-layout>
        <v-dialog max-width='800px' persistent scrollable v-model='dialogData.showDialog'>
            <v-card>
                <v-card-title class="ghd-dialog-box-padding-top">
                    <v-layout justify-space-between align-center>
                        <div class="ghd-control-dialog-header">Edit Budget Criteria</div>
                        <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
                            X
                        </v-btn>
                    </v-layout>
                </v-card-title>
                <div style='height: 500px; max-width:800px' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 450px; overflow-y:auto;'>
                    <v-data-table :headers='editBudgetsDialogGridHeaders'
                                  :items='editBudgetsDialogGridData'
                                  sort-icon=$vuetify.icons.ghd-table-sort
                                  hide-actions
                                  item-key='id'
                                  v-model='selectedGridRows'
                                  class="ghd-table">
                        <template slot='items' slot-scope='props'>
                            <td>
                                <v-edit-dialog :return-value.sync='props.item.name' persistent
                                               @save='onEditBudgetName(props.item)' large lazy>
                                    <v-text-field readonly single-line class='sm-txt' :value='props.item.name'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]" />
                                    <template slot='input'>
                                        <v-text-field label='Edit' single-line v-model='props.item.name'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]" />
                                    </template>
                                </v-edit-dialog>
                            </td>
                            <td>
                                <v-text-field readonly single-line class='sm-txt'
                                              :value='props.item.criterionLibrary.mergedCriteriaExpression'>
                                    <template slot='append-outer'>
                                        <v-btn @click="onShowCriterionLibraryEditorDialog(props.item)"  class="ghd-blue" icon style="margin-top:-6px;">
                                            <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                        </v-btn>                                        
                                    </template>
                                </v-text-field>
                            </td>
                            <td>
                                <v-btn @click="onRemoveBudget(props.item.id)"  class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </td>
                        </template>
                    </v-data-table>
                    </div>
                    <v-layout row align-end style="margin:0 !important">
                        <v-btn @click='onAddBudget' class='ghd-blue ghd-button' flat>
                            Add
                        </v-btn>
                    </v-layout>
                </div>
                
                <v-card-actions class="ghd-dialog-box-padding-bottom">
                    <v-layout justify-center>
                        <v-btn @click='onSubmit(false)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>Cancel</v-btn>
                        <v-btn @click='onSubmit(true)' class='ghd-blue hd-button-text ghd-button' flat
                               :disabled='disableSubmitButton()'>
                            Save
                        </v-btn>                        
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <CriterionLibraryEditorDialog :dialogData='criterionLibraryEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { hasValue } from '@/shared/utils/has-value-util';
import { Action } from 'vuex-class';
import { any, clone, isNil, update, findIndex, propEq } from 'ramda';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
    EditBudgetsDialogData, EmitedBudgetChanges, emptyEmitBudgetChanges,
} from '@/shared/models/modals/edit-budgets-dialog';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import { Budget, emptyBudget } from '@/shared/models/iAM/investment';
import { rules, InputValidationRules } from '@/shared/utils/input-validation-rules';
import ObjectID from 'bson-objectid';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary, emptyCriterionLibrary } from '@/shared/models/iAM/criteria';

@Component({
    components: {
        CriterionLibraryEditorDialog,
    },
})
export default class EditBudgetsDialog extends Vue {
    @Prop() dialogData: EditBudgetsDialogData;

    @Action('addErrorNotification') addErrorNotificationAction: any;

    editBudgetsDialogGridHeaders: DataTableHeader[] = [
        { text: 'Budget', value: 'name', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Criteria', value: 'criterionLibrary', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Actions', value: 'actions', sortable: false, align: 'left', class: '', width: '' }
    ];
    editBudgetsDialogGridData: Budget[] = [];
    selectedGridRows: Budget[] = [];
    criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
    selectedBudgetForCriteriaEdit: Budget = clone(emptyBudget);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();

    budgetChanges: EmitedBudgetChanges = clone(emptyEmitBudgetChanges);

    @Watch('dialogData')
    onDialogDataChanged() {
        this.editBudgetsDialogGridData = clone(this.dialogData.budgets);
        this.budgetChanges.addedBudgets = [];
        this.budgetChanges.updatedBudgets = [];
        this.budgetChanges.deletionIds = [];
    }

    onAddBudget() {
        const unnamedBudgets = this.editBudgetsDialogGridData
            .filter((budget: Budget) => budget.name.match(/Unnamed Budget/));
        const budget: Budget = {
            ...emptyBudget,
            id: getNewGuid(),
            name: `Unnamed Budget ${unnamedBudgets.length + 1}`,
            criterionLibrary: clone(emptyCriterionLibrary),
        }
        this.editBudgetsDialogGridData.push(budget);
        this.budgetChanges.addedBudgets.push(budget);
    }

    onEditBudgetName(budget: Budget) {
        this.editBudgetsDialogGridData = update(
            findIndex(propEq('id', budget.id), this.editBudgetsDialogGridData),
            clone(budget),
            this.editBudgetsDialogGridData,
        );

        const origBudget = this.dialogData.budgets.find((b) => b.id == budget.id)
        if(!isNil(origBudget)){
            if(origBudget.name !== budget.name){
                if(any(propEq('id', budget.id), this.budgetChanges.addedBudgets))
                    this.budgetChanges.addedBudgets[this.budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else if(any(propEq('id', budget.id), this.budgetChanges.updatedBudgets))
                    this.budgetChanges.updatedBudgets[this.budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else
                    this.budgetChanges.updatedBudgets.push(budget);
            }
        }          
    }

    disableDeleteButton() {
        return !hasValue(this.selectedGridRows);
    }

    onRemoveBudgets() {
        this.editBudgetsDialogGridData = this.editBudgetsDialogGridData
            .filter((budget: Budget) => !any(propEq('id', budget.id), this.selectedGridRows));
        this.selectedGridRows.forEach(budget => {
            this.removeBudget(budget.id)
        })
        this.selectedGridRows = [];
    }

    onRemoveBudget(id: string){
        this.editBudgetsDialogGridData = this.editBudgetsDialogGridData
            .filter((budget: Budget) => budget.id != id);

        this.removeBudget(id)
    }

    removeBudget(id: string){
        if(any(propEq('id', id), this.budgetChanges.addedBudgets)){
            this.budgetChanges.addedBudgets = this.budgetChanges.addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            this.budgetChanges.deletionIds.push(id);
        }              
        else if(any(propEq('id', id), this.budgetChanges.updatedBudgets))
            this.budgetChanges.updatedBudgets = this.budgetChanges.updatedBudgets.filter((upBudge: Budget) => upBudge.id != id);
        else
            this.budgetChanges.deletionIds.push(id);
    }

    onShowCriterionLibraryEditorDialog(budget: Budget) {
        this.selectedBudgetForCriteriaEdit = clone(budget);

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: budget.criterionLibrary.id,
            isCallFromScenario: this.dialogData.scenarioId !== this.uuidNIL,
            isCriterionForLibrary: this.dialogData.scenarioId === this.uuidNIL
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
        this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedBudgetForCriteriaEdit.id !== this.uuidNIL) {
            this.editBudgetsDialogGridData = update(
                findIndex(propEq('id', this.selectedBudgetForCriteriaEdit.id), this.editBudgetsDialogGridData),
                { ...this.selectedBudgetForCriteriaEdit, criterionLibrary: criterionLibrary },
                this.editBudgetsDialogGridData,
            );

        const budget = this.selectedBudgetForCriteriaEdit;
        const origBudget = this.dialogData.budgets.find((b) => b.id == budget.id)
        if(!isNil(origBudget)){
            if(origBudget.criterionLibrary.mergedCriteriaExpression !== budget.criterionLibrary.mergedCriteriaExpression){
                if(any(propEq('id', budget.id), this.budgetChanges.addedBudgets))
                    this.budgetChanges.addedBudgets[this.budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else if(any(propEq('id', budget.id), this.budgetChanges.updatedBudgets))
                    this.budgetChanges.updatedBudgets[this.budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else
                    this.budgetChanges.updatedBudgets.push(budget);
            }
        }  

            this.selectedBudgetForCriteriaEdit = clone(emptyBudget);
        }
    }

    onSubmit(submit: boolean) {
        if (submit) {
            this.$emit('submit', this.budgetChanges);
        } else {
            this.$emit('submit', null);
        }

        this.editBudgetsDialogGridData = [];
        this.selectedGridRows = [];
    }

    disableSubmitButton() {
        const allDataIsValid: boolean = this.editBudgetsDialogGridData.every((budget: Budget) => {
            return this.rules['generalRules'].valueIsNotEmpty(budget.name) === true &&
                this.rules['investmentRules'].budgetNameIsUnique(budget, this.editBudgetsDialogGridData) === true;
        });

        return !allDataIsValid;
    }
}
</script>
