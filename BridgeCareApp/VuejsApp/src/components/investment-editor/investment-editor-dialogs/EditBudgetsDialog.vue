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
                    <v-data-table id="EditBudgetsDialog-budgets-dataTable"
                                  :headers='editBudgetsDialogGridHeaders'
                                  :items='editBudgetsDialogGridData'
                                  sort-icon=$vuetify.icons.ghd-table-sort
                                  hide-actions
                                  item-key='id'
                                  v-model='selectedGridRows'
                                  class="ghd-table">
                        <template slot='items' slot-scope='props'>
                            <td>
                                <v-layout row>
                                <v-text-field v-model="props.item.budgetOrder" @change="reorderList(props.item)" @mousedown="setCurrentOrder(props.item)" class='order_input'/>
                                <v-btn class="ghd-blue" icon>
                                    <v-layout column>
                                    <v-icon title="up" @click="swapItemOrder(props.item, 'up')" @mousedown="setCurrentOrder(props.item)"> fas fa-chevron-up
                                    </v-icon>
                                    <v-icon title="down" @click="swapItemOrder(props.item, 'down')" @mousedown="setCurrentOrder(props.item)"> fas fa-chevron-down
                                    </v-icon>
                                    </v-layout>
                                </v-btn>
                                </v-layout>
                            </td>
                            <td>
                                <v-edit-dialog id="EditBudgetsDialog-budget-editDialog"
                                               :return-value.sync='props.item.name' persistent
                                               @save='onEditBudgetName(props.item)' large lazy>
                                    <v-text-field id="EditBudgetsDialog-budget-textField"
                                                  readonly single-line class='sm-txt' :value='props.item.name'
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
                                        <v-btn id="EditBudgetsDialog-openCriteriaEditor-vbtn" @click="onShowCriterionLibraryEditorDialog(props.item)"  class="ghd-blue" icon style="margin-top:-6px;">
                                            <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                                        </v-btn>                                        
                                    </template>
                                </v-text-field>
                            </td>
                            <td>
                                <v-btn @click="onRemoveBudget(props.item.id)" @mousedown="setCurrentOrder(props.item)" class="ghd-blue" icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </td>
                        </template>
                    </v-data-table>
                    </div>
                    <v-layout row align-end style="margin:0 !important">
                        <v-btn id="EditBudgetsDialog-add-btn" @click='onAddBudget' class='ghd-blue ghd-button' flat>
                            Add
                        </v-btn>
                    </v-layout>
                </div>
                
                <v-card-actions class="ghd-dialog-box-padding-bottom">
                    <v-layout justify-center>
                        <v-btn id="EditBudgetsDialog-cancel-btn" @click='onSubmit(false)' class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>Cancel</v-btn>
                        <v-btn id="EditBudgetsDialog-save-btn" @click='onSubmit(true)' class='ghd-blue hd-button-text ghd-button' flat
                               :disabled='disableSubmitButton()'>
                            Save
                        </v-btn>                        
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <GeneralCriterionEditorDialog :dialogData='criterionLibraryEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { hasValue } from '@/shared/utils/has-value-util';
import { Action } from 'vuex-class';
import { any, clone, isNil, update, findIndex, propEq, isEmpty } from 'ramda';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import {
    EditBudgetsDialogData, EmitedBudgetChanges, emptyEmitBudgetChanges,
} from '@/shared/models/modals/edit-budgets-dialog';
import { Budget, emptyBudget } from '@/shared/models/iAM/investment';
import { rules, InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { emptyCriterionLibrary } from '@/shared/models/iAM/criteria';
import { isNull, isNullOrUndefined } from 'util';

@Component({
    components: {
        GeneralCriterionEditorDialog,
    },
})
export default class EditBudgetsDialog extends Vue {
    @Prop() dialogData: EditBudgetsDialogData;

    @Action('addErrorNotification') addErrorNotificationAction: any;

    editBudgetsDialogGridHeaders: DataTableHeader[] = [
        { text: 'Order', value: 'order', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Budget', value: 'name', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Criteria', value: 'criterionLibrary', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Actions', value: 'actions', sortable: false, align: 'left', class: '', width: '' }
    ];
    editBudgetsDialogGridData: Budget[] = [];
    selectedGridRows: Budget[] = [];    
    criterionLibraryEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
    selectedBudgetForCriteriaEdit: Budget = clone(emptyBudget);
    rules: InputValidationRules = rules;
    uuidNIL: string = getBlankGuid();
    Up: string = "up";
    budgetChanges: EmitedBudgetChanges = clone(emptyEmitBudgetChanges);
    
    originalOrder: number = 0;
    currentSelectedBudget: Budget = emptyBudget;

    @Watch('dialogData')
    onDialogDataChanged() {
        this.budgetChanges.addedBudgets = [];
        this.budgetChanges.updatedBudgets = [];
        this.budgetChanges.deletionIds = [];

        this.editBudgetsDialogGridData = this.setDefaultBudgetOrder(this.dialogData.budgets.sort(this.compareOrder));
    }
    setDefaultBudgetOrder(loadedBudgets: Budget[]): Budget[] {
        let inc = 1;
        let cloneBudgets = clone(loadedBudgets);
        // If there is a 0 in the ordering, we need to reset the
        // ordering.
        const budgetCheck = cloneBudgets.find(b => b.budgetOrder === 0);
        if(!isNil(budgetCheck) && this.budgetChanges.updatedBudgets.length === 0) {
            cloneBudgets.forEach(b => {
                b.budgetOrder = inc++;
                // Update order
                if(any(propEq('id', b.id), this.budgetChanges.updatedBudgets))
                    this.budgetChanges.updatedBudgets[this.budgetChanges.updatedBudgets.findIndex((budget => budget.id == b.id))] = b;
                else
                    this.budgetChanges.updatedBudgets.push(b);

            });
        }
        return cloneBudgets;
    }
    onAddBudget() {
        const unnamedBudgets = this.editBudgetsDialogGridData
            .filter((budget: Budget) => budget.name.match(/Unnamed Budget/));
    
        const budget: Budget = {
            ...emptyBudget,
            id: getNewGuid(),
            budgetOrder: this.editBudgetsDialogGridData.length + 1,
            name: `Unnamed Budget ${this.editBudgetsDialogGridData.length + 1}`,
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
    onEditBudgetOrder(budget: Budget) {
        this.editBudgetsDialogGridData = update(
            findIndex(propEq('id', budget.id), this.editBudgetsDialogGridData),
            clone(budget),
            this.editBudgetsDialogGridData,
        );
        if(any(propEq('id', budget.id), this.budgetChanges.updatedBudgets))
            this.budgetChanges.updatedBudgets[this.budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
        else
            this.budgetChanges.updatedBudgets.push(budget);
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
        this.removeBudget(id);
        this.cleanReorderList();
    }

    removeBudget(id: string){
        if(any(propEq('id', id), this.budgetChanges.addedBudgets)){
            this.budgetChanges.addedBudgets = this.budgetChanges.addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            this.budgetChanges.deletionIds.push(id);
        }              
        else if(any(propEq('id', id), this.budgetChanges.updatedBudgets)) {
            this.budgetChanges.updatedBudgets = this.budgetChanges.updatedBudgets.filter((upBudge: Budget) => upBudge.id != id);
            this.budgetChanges.deletionIds.push(id);
        }
        else
            this.budgetChanges.deletionIds.push(id);
    }

    onShowCriterionLibraryEditorDialog(budget: Budget) {
        this.selectedBudgetForCriteriaEdit = clone(budget);

        this.criterionLibraryEditorDialogData = {
            showDialog: true,
            CriteriaExpression: budget.criterionLibrary.mergedCriteriaExpression
        };
    }

    onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        this.criterionLibraryEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && this.selectedBudgetForCriteriaEdit.id !== this.uuidNIL) {
            this.selectedBudgetForCriteriaEdit.criterionLibrary.mergedCriteriaExpression = criterionExpression;           

            this.editBudgetsDialogGridData = update(
                findIndex(propEq('id', this.selectedBudgetForCriteriaEdit.id), this.editBudgetsDialogGridData),
                { ...this.selectedBudgetForCriteriaEdit, criterionLibrary: this.selectedBudgetForCriteriaEdit.criterionLibrary },
                this.editBudgetsDialogGridData,
            );

            const budget = this.selectedBudgetForCriteriaEdit;
            const origBudget = this.dialogData.budgets.find((b) => b.id == budget.id);

            if(!isNil(origBudget)){
                if(origBudget.criterionLibrary.mergedCriteriaExpression !== budget.criterionLibrary.mergedCriteriaExpression){                                                            
                    if(this.budgetChanges.addedBudgets.length !== 0){
                        this.budgetChanges.addedBudgets[this.budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else if(this.budgetChanges.updatedBudgets.length !== 0){                        
                        this.budgetChanges.updatedBudgets[this.budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else
                    {
                        this.budgetChanges.updatedBudgets.push(budget);
                    }
                }
            }
            else{
                this.budgetChanges.addedBudgets[this.budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
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
    compareOrder(b1: Budget, b2: Budget) {
        return b1.budgetOrder - b2.budgetOrder;
    }
    swapItemOrder(item:Budget, direction: string) {
        
        if (isNil(direction) || isNil(item)) return;

        if (direction.toLowerCase() === this.Up) {    
            if (item.budgetOrder <= 1) return;
            this.editBudgetsDialogGridData.forEach(element => {
                if( element.budgetOrder === (item.budgetOrder-1)) {
                    element.budgetOrder = item.budgetOrder;
                    this.onEditBudgetOrder(element);
                }
                else if (element.budgetOrder === item.budgetOrder) {
                    element.budgetOrder = item.budgetOrder -1;
                    this.onEditBudgetOrder(element);
                }
            });
        } else {
            if (item.budgetOrder >= this.editBudgetsDialogGridData.length) return;
            let hold: number = item.budgetOrder;
            
            this.editBudgetsDialogGridData.forEach(element => {
                if( element.budgetOrder === (hold)) {
                    element.budgetOrder = item.budgetOrder + 1;
                    this.onEditBudgetOrder(element);
                }
                else if (element.budgetOrder === (hold + 1)) {
                    element.budgetOrder = hold;
                    this.onEditBudgetOrder(element);
                }
            });
        }
        // sort after the reorder
        this.editBudgetsDialogGridData.sort(this.compareOrder);
        this.originalOrder = 0;
        this.currentSelectedBudget = emptyBudget;
    }
    reorderList(item: Budget) {
        const original = this.originalOrder;
        const replacement = this.currentSelectedBudget.budgetOrder;
        if (isNil(replacement) || isEmpty(replacement) || original === 0) return;

        const diff = original - replacement;
        if (diff > 0) { // reorder up
            this.editBudgetsDialogGridData.forEach(element => {
                if (element === this.currentSelectedBudget) { this.onEditBudgetOrder(element); }
                else if (element.budgetOrder >=replacement && element.budgetOrder <= original) {
                    element.budgetOrder++;
                    this.onEditBudgetOrder(element);
                }
            });
        } else { // reorder down
            this.editBudgetsDialogGridData.forEach(element => {
                if (element === this.currentSelectedBudget) { this.onEditBudgetOrder(element); }
                else if (element.budgetOrder >=original && element.budgetOrder <= replacement) {
                    element.budgetOrder--;
                    this.onEditBudgetOrder(element);
                }
            });
        }
        this.editBudgetsDialogGridData.sort(this.compareOrder);
        this.originalOrder = 0;
        this.currentSelectedBudget = emptyBudget;
    }
    cleanReorderList() {
        let count: number = 1;
        this.editBudgetsDialogGridData.forEach(element => {
            element.budgetOrder = count;
            this.onEditBudgetOrder(element);
            count++;
        });
    }
    setCurrentOrder(item: Budget) {
        this.originalOrder = item.budgetOrder;
        this.currentSelectedBudget = item;
    }
}
</script>
<style>
.order_input {
    width: 15px;
    justify-content: center;
    padding: 5px;
}
</style>