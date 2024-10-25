<template>
    <v-row>
        <v-dialog max-width='900px' persistent scrollable v-model ='dialogData.showDialog'>
            <v-card>
                <v-card-title class="ghd-dialog-box-padding-top">
                    <v-row justify-space-between align-center>
                        <div class="ghd-control-dialog-header">Edit Budget Criteria</div>
                        <v-spacer></v-spacer>
                        <XButton @click="onSubmit(false)"/>
                    </v-row>
                </v-card-title>
                <div style='height: 500px; max-width:900px; margin-top:20px;' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 435px; overflow-y:auto;'>
                        <v-data-table-server
                                  id="EditBudgetsDialog-budgets-dataTable"
                                  :headers='editBudgetsDialogGridHeaders'
                                  :items="editBudgetsDialogGridData"
                                  :items-length="editBudgetsDialogGridData.length"
                                  sort-asc-icon="custom:GhdTableSortAscSvg"
                                  sort-desc-icon="custom:GhdTableSortDescSvg"
                                  hide-actions
                                  item-key='id'
                                  v-model='selectedGridRows'                              
                                  class="ghd-table hide_table_scroll">
                                  <template #bottom></template>
                        <template slot='items' slot-scope='props' v-slot:item="props">
                         <tr>  
                            <td>
                                <v-text-field
                                    v-model="props.item.budgetOrder" 
                                    @change="reorderList(props.item)" 
                                    @mousedown="setCurrentOrder(props.item)" 
                                    variant="underlined" style="width: 30px;"
                                />
                            </td>
                            <td>
                                <v-row>
                                    <v-col>
                                        <v-btn class="ghd-blue" @click="swapItemOrder(props.item, 'up')" @mousedown="setCurrentOrder(props.item)" flat>
                                            <v-icon title="up"> fas fa-chevron-up
                                            </v-icon>
                                        </v-btn>
                                        <v-btn class="ghd-blue" @click="swapItemOrder(props.item, 'down')" @mousedown="setCurrentOrder(props.item)" flat>
                                            <v-icon title="down"> fas fa-chevron-down
                                            </v-icon>
                                        </v-btn>
                                    </v-col>
                                </v-row>
                                
                            </td>
                            <td>
                                <editDialog id="EditBudgetsDialog-budget-editDialog"
                                               v-model:return-value='props.item.name' persistent
                                               @save='onEditBudgetName(props.item)' size="large" lazy>
                                    <v-text-field id="EditBudgetsDialog-budget-textField"
                                        variant="underlined"
                                        readonly single-line class='sm-txt' :model-value='props.item.name'
                                        :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]" />
                                    <template v-slot:input>
                                        <v-text-field label='Edit' single-line v-model='props.item.name'
                                                      :rules="[rules['generalRules'].valueIsNotEmpty, rules['investmentRules'].budgetNameIsUnique(props.item, editBudgetsDialogGridData)]" />
                                    </template>
                                </editDialog>
                            </td>
                            <td>
                                <v-text-field
                                    readonly single-line class='sm-txt'
                                    variant="underlined"
                                    :model-value='props.item.criterionLibrary.mergedCriteriaExpression'>
                                    <template v-slot:append-inner>
                                        <v-btn id="EditBudgetsDialog-openCriteriaEditor-vbtn" @click="onShowCriterionLibraryEditorDialog(props.item)"  
                                            class="ghd-green" flat icon>
                                            <EditSvg />
                                        </v-btn>                                        
                                    </template>
                                </v-text-field>
                            </td>
                            <td>
                                <v-btn id="EditBudgetsDialog-removeBudget-btn" @click="onRemoveBudget(props.item.id)" @mousedown="setCurrentOrder(props.item)" 
                                    class="ghd-red" flat icon>
                                    <TrashCanSvg />
                                </v-btn>
                             
                            </td>
                        </tr>    
                        </template>
                    </v-data-table-server>
                    </div>
                    <v-row row align-end style="margin:0 !important">
                        <v-btn id="EditBudgetsDialog-add-btn" @click='onAddBudget' class='ghd-blue ghd-button' variant = "flat">
                            Add
                        </v-btn>
                    </v-row>
                </div>
                <v-card-actions class="ghd-dialog-box-padding-bottom">
                    <v-row justify="center">
                        <CancelButton @cancel="onSubmit(false)"/>
                        <SaveButton 
                            @save="onSubmit(true)"
                            :disabled='disableSubmitButton()'
                        />
                    </v-row>                    
                </v-card-actions>
            </v-card>
        </v-dialog>
        <GeneralCriterionEditorDialog :dialogData='criterionLibraryEditorDialogData'
                                      @submit='onSubmitCriterionLibraryEditorDialogResult' />
    </v-row>
</template>

<script setup lang='ts'>
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import { hasValue } from '@/shared/utils/has-value-util';
import { any, clone, isNil, update, findIndex, propEq, isEmpty } from 'ramda';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import {
    EditBudgetsDialogData, EmitedBudgetChanges, emptyEmitBudgetChanges,
} from '@/shared/models/modals/edit-budgets-dialog';
import { Budget, emptyBudget } from '@/shared/models/iAM/investment';
import { rules as validationRules, InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { emptyCriterionLibrary } from '@/shared/models/iAM/criteria';

import { ref, onMounted, onBeforeUnmount, toRefs, watch } from 'vue';
import { useStore } from 'vuex';
import { getUrl } from '@/shared/utils/get-url';
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';
import CancelButton from '@/shared/components/buttons/CancelButton.vue';
import SaveButton from '@/shared/components/buttons/SaveButton.vue';
import XButton from '@/shared/components/buttons/XButton.vue';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: EditBudgetsDialogData
}>()
const { dialogData } = toRefs(props);

function addErrorNotificationAction(payload?: any){ store.dispatch('addErrorNotification', payload);}

let editBudgetsDialogGridHeaders: any[] = [
        { title: 'Order', key: 'order', sortable: false, align: 'center', class: '', width: '5%' },
        { title: '', key: '', sortable: false, align: 'left', class: '', width: '' },
        { title: 'Budget', key: 'name', sortable: false, align: 'left', class: '', width: '30%' },
        { title: 'Criteria', key: 'criterionLibrary', sortable: false, align: 'left', class: '', width: '40%' },
        { title: 'Actions', key: 'actions', sortable: false, align: 'left', class: '', width: '10%' }
    ];
let editBudgetsDialogGridData = ref<Budget[]>([]);
let totalItems = ref<number>(0);
let selectedGridRows = ref<Budget[]>([]);    
let criterionLibraryEditorDialogData = ref<GeneralCriterionEditorDialogData>(clone(emptyGeneralCriterionEditorDialogData));
let selectedBudgetForCriteriaEdit = ref<Budget>(clone(emptyBudget));
let rules: InputValidationRules = validationRules;
let uuidNIL: string = getBlankGuid();
let Up: string = "up";
let budgetChanges = ref<EmitedBudgetChanges>(clone(emptyEmitBudgetChanges));
    
let originalOrder: number = 0;
let currentSelectedBudget = ref<Budget>(emptyBudget);

watch(dialogData,() => {
        budgetChanges.value.addedBudgets = [];
        budgetChanges.value.updatedBudgets = [];
        budgetChanges.value.deletionIds = [];

        editBudgetsDialogGridData.value = setDefaultBudgetOrder(props.dialogData.budgets.sort(compareOrder));
    });

    function setDefaultBudgetOrder(loadedBudgets: Budget[]): Budget[] {
        let inc = 1;
        let cloneBudgets = clone(loadedBudgets);
        // If there is a 0 in the ordering, we need to reset the
        // ordering.
        const budgetCheck = cloneBudgets.find(b => b.budgetOrder === 0);
        if(!isNil(budgetCheck) && budgetChanges.value.updatedBudgets.length === 0) {
            cloneBudgets.forEach(b => {
                b.budgetOrder = inc++;
                // Update order
                if(any(propEq('id', b.id), budgetChanges.value.updatedBudgets))
                    budgetChanges.value.updatedBudgets[budgetChanges.value.updatedBudgets.findIndex((budget => budget.id == b.id))] = b;
                else
                    budgetChanges.value.updatedBudgets.push(b);

            });
        }
        return cloneBudgets;
    }
    function onAddBudget() {
        const unnamedBudgets = editBudgetsDialogGridData.value
            .filter((budget: Budget) => budget.name.match(/Unnamed Budget/));
    
        const budget: Budget = {
            ...emptyBudget,
            id: getNewGuid(),
            budgetOrder: editBudgetsDialogGridData.value.length + 1,
            name: `Unnamed Budget ${editBudgetsDialogGridData.value.length + 1}`,
            criterionLibrary: clone(emptyCriterionLibrary),
        }
        editBudgetsDialogGridData.value.push(budget);
        budgetChanges.value.addedBudgets.push(budget);
    }
    function onEditBudgetName(budget: Budget) {
        editBudgetsDialogGridData.value = update(
            findIndex(propEq('id', budget.id), editBudgetsDialogGridData.value),
            clone(budget),
            editBudgetsDialogGridData.value,
        );
        const origBudget = props.dialogData.budgets.find((b) => b.id == budget.id)
        if(!isNil(origBudget)){
            if(origBudget.name !== budget.name){
                if(any(propEq('id', budget.id), budgetChanges.value.addedBudgets))
                    budgetChanges.value.addedBudgets[budgetChanges.value.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else if(any(propEq('id', budget.id), budgetChanges.value.updatedBudgets))
                    budgetChanges.value.updatedBudgets[budgetChanges.value.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                else
                    budgetChanges.value.updatedBudgets.push(budget);
            }
        }          
    }
    function onEditBudgetOrder(budget: Budget) {
        editBudgetsDialogGridData.value = update(
            findIndex(propEq('id', budget.id), editBudgetsDialogGridData.value),
            clone(budget),
            editBudgetsDialogGridData.value,
        );
        if(any(propEq('id', budget.id), budgetChanges.value.updatedBudgets))
            budgetChanges.value.updatedBudgets[budgetChanges.value.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
        else
            budgetChanges.value.updatedBudgets.push(budget);
    }
    function disableDeleteButton() {
        return !hasValue(selectedGridRows.value);
    }

    function onRemoveBudgets() {
        editBudgetsDialogGridData.value = editBudgetsDialogGridData.value
            .filter((budget: Budget) => !any(propEq('id', budget.id), selectedGridRows.value));
        selectedGridRows.value.forEach(budget => {
            removeBudget(budget.id)
        })
        selectedGridRows.value = [];
    }

    function onRemoveBudget(id: string){
        editBudgetsDialogGridData.value = editBudgetsDialogGridData.value
            .filter((budget: Budget) => budget.id != id);
        removeBudget(id);
        cleanReorderList();
    }

    function removeBudget(id: string){
        if(any(propEq('id', id), budgetChanges.value.addedBudgets)){
            budgetChanges.value.addedBudgets = budgetChanges.value.addedBudgets.filter((addBudge: Budget) => addBudge.id != id);
            budgetChanges.value.deletionIds.push(id);
        }              
        else if(any(propEq('id', id), budgetChanges.value.updatedBudgets)) {
            budgetChanges.value.updatedBudgets = budgetChanges.value.updatedBudgets.filter((upBudge: Budget) => upBudge.id != id);
            budgetChanges.value.deletionIds.push(id);
        }
        else
            budgetChanges.value.deletionIds.push(id);
    }

    function onShowCriterionLibraryEditorDialog(budget: Budget) {
        selectedBudgetForCriteriaEdit.value = clone(budget);

        criterionLibraryEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: budget.criterionLibrary.mergedCriteriaExpression
        };
    }

    function onSubmitCriterionLibraryEditorDialogResult(criterionExpression: string) {
        criterionLibraryEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedBudgetForCriteriaEdit.value.id !== uuidNIL) {
            selectedBudgetForCriteriaEdit.value.criterionLibrary.mergedCriteriaExpression = criterionExpression;           

            editBudgetsDialogGridData.value = update(
                findIndex(propEq('id', selectedBudgetForCriteriaEdit.value.id), editBudgetsDialogGridData.value),
                { ...selectedBudgetForCriteriaEdit.value, criterionLibrary: selectedBudgetForCriteriaEdit.value.criterionLibrary },
                editBudgetsDialogGridData.value,
            );

            const budget = selectedBudgetForCriteriaEdit.value;
            const origBudget = props.dialogData.budgets.find((b) => b.id == budget.id);

            if(!isNil(origBudget)){
                if(origBudget.criterionLibrary.mergedCriteriaExpression !== budget.criterionLibrary.mergedCriteriaExpression){                                                            
                    if(budgetChanges.value.addedBudgets.length !== 0){
                        budgetChanges.value.addedBudgets[budgetChanges.value.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else if(budgetChanges.value.updatedBudgets.length !== 0){                        
                        budgetChanges.value.updatedBudgets[budgetChanges.value.updatedBudgets.findIndex((b => b.id == budget.id))] = budget;
                    }
                    else
                    {
                        budgetChanges.value.updatedBudgets.push(budget);
                    }
                }
            }
            else{
                budgetChanges.value.addedBudgets[budgetChanges.value.addedBudgets.findIndex((b => b.id == budget.id))] = budget;
            }        

            selectedBudgetForCriteriaEdit.value = clone(emptyBudget);
        }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', budgetChanges.value);
        } else {
            emit('submit', null);
        }

        editBudgetsDialogGridData.value = [];
        selectedGridRows.value = [];
    }

    function disableSubmitButton() {
        const allDataIsValid: boolean = editBudgetsDialogGridData.value.every((budget: Budget) => {
            return rules['generalRules'].valueIsNotEmpty(budget.name) === true &&
                rules['investmentRules'].budgetNameIsUnique(budget, editBudgetsDialogGridData.value) === true;
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
            editBudgetsDialogGridData.value.forEach(element => {
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
            if (item.budgetOrder >= editBudgetsDialogGridData.value.length) return;
            let hold: number = item.budgetOrder;
            
            editBudgetsDialogGridData.value.forEach(element => {
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
        editBudgetsDialogGridData.value.sort(compareOrder);
        originalOrder = 0;
        currentSelectedBudget.value = emptyBudget;
    }
    function reorderList(item: Budget) {
        const original = originalOrder;
        const replacement = currentSelectedBudget.value.budgetOrder;
        if (isNil(replacement) || isEmpty(replacement) || original === 0) return;

        const diff = original - replacement;
        if (diff > 0) { // reorder up
            editBudgetsDialogGridData.value.forEach(element => {
                if (element === currentSelectedBudget.value) { onEditBudgetOrder(element); }
                else if (element.budgetOrder >=replacement && element.budgetOrder <= original) {
                    element.budgetOrder++;
                    onEditBudgetOrder(element);
                }
            });
        } else { // reorder down
            editBudgetsDialogGridData.value.forEach(element => {
                if (element === currentSelectedBudget.value) { onEditBudgetOrder(element); }
                else if (element.budgetOrder >=original && element.budgetOrder <= replacement) {
                    element.budgetOrder--;
                    onEditBudgetOrder(element);
                }
            });
        }
        editBudgetsDialogGridData.value.sort(compareOrder);
        originalOrder = 0;
        currentSelectedBudget.value = emptyBudget;
    }
    function cleanReorderList() {
        let count: number = 1;
        editBudgetsDialogGridData.value.forEach(element => {
            element.budgetOrder = count;
            onEditBudgetOrder(element);
            count++;
        });
    }
    function setCurrentOrder(item: Budget) {
        originalOrder = item.budgetOrder;
        currentSelectedBudget.value = item;
    }
</script>
<style>
.order_input {
    width: 45px;
    justify-content: center;
    padding: 5px;
}

.hide_table_scroll .v-table__wrapper{
    overflow: hidden;
}
</style>