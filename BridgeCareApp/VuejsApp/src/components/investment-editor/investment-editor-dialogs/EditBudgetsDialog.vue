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
                                              :value='props.criterionLibrary.mergedCriteriaExpression'>
                                    <template slot='append-inner'>
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

<script lang='ts' setup>
import Vue from 'vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { any, clone, isNil, update, findIndex, propEq, isEmpty } from 'ramda';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import {
    EditBudgetsDialogData, EmitedBudgetChanges, emptyEmitBudgetChanges,
} from '@/shared/models/modals/edit-budgets-dialog';
import { Budget, emptyBudget } from '@/shared/models/iAM/investment';
import { rules as validationRules, InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { emptyCriterionLibrary } from '@/shared/models/iAM/criteria';
import { isNull, isNullOrUndefined } from 'util';
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: EditBudgetsDialogData
}>()
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}
let ditBudgetsDialogGridHeaders: DataTableHeader[] = [
        { text: 'Order', value: 'order', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Budget', value: 'name', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Criteria', value: 'criterionLibrary', sortable: false, align: 'left', class: '', width: '' },
        { text: 'Actions', value: 'actions', sortable: false, align: 'left', class: '', width: '' }
    ];
let editBudgetsDialogGridData: Budget[] = [];
let selectedGridRows: Budget[] = [];    
let criterionLibraryEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
let selectedBudgetForCriteriaEdit: Budget = clone(emptyBudget);
let rules: InputValidationRules = validationRules;
let uuidNIL: string = getBlankGuid();
let Up: string = "up";
let budgetChanges: EmitedBudgetChanges = clone(emptyEmitBudgetChanges);
    
let originalOrder: number = 0;
let currentSelectedBudget: Budget = emptyBudget;

watch(()=>props.dialogData,()=> onDialogDataChanged)
    function onDialogDataChanged() {
        budgetChanges.addedBudgets = [];
        budgetChanges.updatedBudgets = [];
        budgetChanges.deletionIds = [];

        editBudgetsDialogGridData = setDefaultBudgetOrder(props.dialogData.budgets.sort(compareOrder));
    }
    function setDefaultBudgetOrder(loadedBudgets: Budget[]): Budget[] {
        let inc = 1;
        let cloneBudgets = clone(loadedBudgets);
        // If there is a 0 in the ordering, we need to reset the
        // ordering.
        const budgetCheck = cloneBudgets.find(b => b.budgetOrder === 0);
        if(!isNil(budgetCheck) && budgetChanges.updatedBudgets.length === 0) {
            cloneBudgets.forEach(b => {
                b.budgetOrder = inc++;
                // Update order
                if(any(propEq('id', b.id), budgetChanges.updatedBudgets))
                    budgetChanges.updatedBudgets[budgetChanges.updatedBudgets.findIndex((budget => budget.id == b.id))] = b;
                else
                    budgetChanges.updatedBudgets.push(b);

            });
        }
        return cloneBudgets;
    }
    function onAddBudget() {
        const unnamedBudgets = editBudgetsDialogGridData
            .filter((budget: Budget) => budget.name.match(/Unnamed Budget/));
    
        const budget: Budget = {
            ...emptyBudget,
            id: getNewGuid(),
            budgetOrder: editBudgetsDialogGridData.length + 1,
            name: `Unnamed Budget ${editBudgetsDialogGridData.length + 1}`,
            criterionLibrary: clone(emptyCriterionLibrary),
        }
        editBudgetsDialogGridData.push(budget);
        budgetChanges.addedBudgets.push(budget);
    }
    function onEditBudgetName(budget: Budget) {
        editBudgetsDialogGridData = update(
            findIndex(propEq('id', budget.id), editBudgetsDialogGridData),
            clone(budget),
            editBudgetsDialogGridData,
        );
        const origBudget = props.dialogData.budgets.find((b) => b.id == budget.id)
        if(!isNil(origBudget)){
            if(origBudget.name !== budget.name){
                if(any(propEq('id', budget.id), budgetChanges.addedBudgets))
                    budgetChanges.addedBudgets[budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else if(any(propEq('id', budget.id), budgetChanges.updatedBudgets))
                    budgetChanges.updatedBudgets[budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else
                    budgetChanges.updatedBudgets.push(budget);
            }
        }          
    }
    function onEditBudgetOrder(budget: Budget) {
        editBudgetsDialogGridData = update(
            findIndex(propEq('id', budget.id), editBudgetsDialogGridData),
            clone(budget),
            editBudgetsDialogGridData,
        );
        if(any(propEq('id', budget.id), budgetChanges.updatedBudgets))
            budgetChanges.updatedBudgets[budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
        else
            budgetChanges.updatedBudgets.push(budget);
    }
    function disableDeleteButton() {
        return !hasValue(selectedGridRows);
    }

    function onRemoveBudgets() {
        editBudgetsDialogGridData = editBudgetsDialogGridData
            .filter((budget: Budget) => !any(propEq('id', budget.id), selectedGridRows));
        selectedGridRows.forEach(budget => {
            removeBudget(budget.id)
        })
        selectedGridRows = [];
    }

    function onRemoveBudget(id: string){
        editBudgetsDialogGridData = editBudgetsDialogGridData
            .filter((budget: Budget) => budget.id != id);
        removeBudget(id);
        cleanReorderList();
    }

    function removeBudget(id: string){
        if(any(propEq('id', id), budgetChanges.addedBudgets)){
            budgetChanges.addedBudgets = budgetChanges.addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            budgetChanges.deletionIds.push(id);
        }              
        else if(any(propEq('id', id), budgetChanges.updatedBudgets)) {
            budgetChanges.updatedBudgets = budgetChanges.updatedBudgets.filter((upBudge: Budget) => upBudge.id != id);
            budgetChanges.deletionIds.push(id);
        }
        else
            budgetChanges.deletionIds.push(id);
    }

    function onShowCriterionLibraryEditorDialog(budget: Budget) {
        selectedBudgetForCriteriaEdit = clone(budget);

        criterionLibraryEditorDialogData = {
            showDialog: true,
            CriteriaExpression: budget.criterionLibrary.mergedCriteriaExpression
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        criterionLibraryEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedBudgetForCriteriaEdit.id !== uuidNIL) {
            selectedBudgetForCriteriaEdit.criterionLibrary.mergedCriteriaExpression = criterionExpression;           

            editBudgetsDialogGridData = update(
                findIndex(propEq('id', selectedBudgetForCriteriaEdit.id), editBudgetsDialogGridData),
                { ...selectedBudgetForCriteriaEdit, criterionLibrary: selectedBudgetForCriteriaEdit.criterionLibrary },
                editBudgetsDialogGridData,
            );

            const budget = selectedBudgetForCriteriaEdit;
            const origBudget = props.dialogData.budgets.find((b) => b.id == budget.id);

            if(!isNil(origBudget)){
                if(origBudget.criterionLibrary.mergedCriteriaExpression !== budget.criterionLibrary.mergedCriteriaExpression){                                                            
                    if(budgetChanges.addedBudgets.length !== 0){
                        budgetChanges.addedBudgets[budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else if(budgetChanges.updatedBudgets.length !== 0){                        
                        budgetChanges.updatedBudgets[budgetChanges.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else
                    {
                        budgetChanges.updatedBudgets.push(budget);
                    }
                }
            }
            else{
                budgetChanges.addedBudgets[budgetChanges.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
            }        

            selectedBudgetForCriteriaEdit = clone(emptyBudget);
        }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', budgetChanges);
        } else {
            emit('submit', null);
        }

        editBudgetsDialogGridData = [];
        selectedGridRows = [];
    }

    function disableSubmitButton() {
        const allDataIsValid: boolean = editBudgetsDialogGridData.every((budget: Budget) => {
            return rules['generalRules'].valueIsNotEmpty(budget.name) === true &&
                rules['investmentRules'].budgetNameIsUnique(budget, editBudgetsDialogGridData) === true;
        });

        return !allDataIsValid;
    }
    function compareOrder(b1: Budget, b2: Budget) {
        return b1.budgetOrder - b2.budgetOrder;
    }
    function swapItemOrder(item:Budget, direction: string) {
        
        if (isNil(direction) || isNil(item)) return;

        if (direction.toLowerCase() === Up) {    
            if (item.budgetOrder <= 1) return;
            editBudgetsDialogGridData.forEach(element => {
                if( element.budgetOrder === (item.budgetOrder-1)) {
                    element.budgetOrder = item.budgetOrder;
                    onEditBudgetOrder(element);
                }
                else if (element.budgetOrder === item.budgetOrder) {
                    element.budgetOrder = item.budgetOrder -1;
                    onEditBudgetOrder(element);
                }
            });
        } else {
            if (item.budgetOrder >= editBudgetsDialogGridData.length) return;
            let hold: number = item.budgetOrder;
            
            editBudgetsDialogGridData.forEach(element => {
                if( element.budgetOrder === (hold)) {
                    element.budgetOrder = item.budgetOrder + 1;
                    onEditBudgetOrder(element);
                }
                else if (element.budgetOrder === (hold + 1)) {
                    element.budgetOrder = hold;
                    onEditBudgetOrder(element);
                }
            });
        }
        // sort after the reorder
        editBudgetsDialogGridData.sort(compareOrder);
        originalOrder = 0;
        currentSelectedBudget = emptyBudget;
    }
    function reorderList(item: Budget) {
        const original = originalOrder;
        const replacement = currentSelectedBudget.budgetOrder;
        if (isNil(replacement) || isEmpty(replacement) || original === 0) return;

        const diff = original - replacement;
        if (diff > 0) { // reorder up
            editBudgetsDialogGridData.forEach(element => {
                if (element === currentSelectedBudget) { onEditBudgetOrder(element); }
                else if (element.budgetOrder >=replacement && element.budgetOrder <= original) {
                    element.budgetOrder++;
                    onEditBudgetOrder(element);
                }
            });
        } else { // reorder down
            editBudgetsDialogGridData.forEach(element => {
                if (element === currentSelectedBudget) { onEditBudgetOrder(element); }
                else if (element.budgetOrder >=original && element.budgetOrder <= replacement) {
                    element.budgetOrder--;
                    onEditBudgetOrder(element);
                }
            });
        }
        editBudgetsDialogGridData.sort(compareOrder);
        originalOrder = 0;
        currentSelectedBudget = emptyBudget;
    }
    function cleanReorderList() {
        let count: number = 1;
        editBudgetsDialogGridData.forEach(element => {
            element.budgetOrder = count;
            onEditBudgetOrder(element);
            count++;
        });
    }
    function setCurrentOrder(item: Budget) {
        originalOrder = item.budgetOrder;
        currentSelectedBudget = item;
    }
</script>
<style>
.order_input {
    width: 15px;
    justify-content: center;
    padding: 5px;
}
</style>