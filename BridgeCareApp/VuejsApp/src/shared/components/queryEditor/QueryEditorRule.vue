<template>
        <v-row :style="{borderLeft: 'solid ' + getBorderColor()}" class="rule-panel">
            <div style="width: 100%; margin: 10px;">
                <v-row style="margin: 0;">
                    <v-col cols="12">
                        <v-row v-if="criteriaRule!.isEquation" justify="center" style="margin: 0;">
                            <v-text-field class="ellipsis-text" density="compact" bg-color="white" variant="outlined" readonly
                                    v-model="selectedOperand"
                            style="margin-bottom: -15px;"></v-text-field>

                            
                            <v-menu
                                    location="left"
                                    v-show="selectedOperand !== ''"
                                >
                                    <template v-slot:activator="{ props }">
                                        <v-btn  v-bind="props" class="ghd-blue" icon variant="text">
                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')">
                                        </v-btn>
                                    </template>
                                    <v-card>
                                        <v-card-text>
                                            <v-textarea
                                                id="PerformanceCurveEditor-checkEquation-vtextarea"
                                                class="sm-txt Montserrat-font-family"
                                                :model-value="
                                                    selectedOperand
                                                "
                                                full-width
                                                no-resize
                                                outline
                                                readonly
                                                rows="5"
                                                style = "min-width: 500px;min-height: 205px;"
                                            />
                                        </v-card-text>
                                    </v-card>
                                </v-menu>
                                <v-btn 
                                    @click="onShowEquationEditorDialog(criteriaValue) "
                                    class="ghd-blue"
                                    variant="text"
                                    icon
                                >
                                    <img class='img-general' :src="getUrl('assets/icons/edit.svg')">
                                </v-btn>
                        </v-row>
                        <v-row v-else justify="center" style="margin: 0;">
                            <span>{{criteriaRule.selectedOperand }}</span>
                        </v-row>
                        
                        <v-row style="margin: 0;">
                            <div style="width: 25%; margin-right: 10px;">
                                <v-select density="compact" variant="outlined" style="margin-bottom: -15px; "
                                    :items='queryRule!.operators'
                                    v-model="selectedOperator"
                                    bg-color="white"></v-select>
                            </div>
                            <div style="width: 50%;">
                                <v-text-field v-if="queryRule!.type === 'STRING'" density="compact" bg-color="white" variant="outlined" 
                                    v-model="selectedValue"
                                    style="margin-bottom: -15px;"></v-text-field>
                                <v-text-field v-if="queryRule!.type === 'NUMBER'" density="compact" bg-color="white" 
                                     variant="outlined" style="margin-bottom: -15px;"
                                     type="number" v-maska:[mask]
                                     v-model="selectedValue"></v-text-field>
                                <v-select v-if="queryRule!.type === 'select'" density="compact" variant="outlined" style="margin-bottom: -15px; "
                                    :items='queryRule!.choices'
                                    item-title="text"
                                    item-value="value"
                                    v-model="selectedValue"
                                    bg-color="white"></v-select>
                            </div>
                            
                            <v-btn @click="onDeleteClick" style="padding-left: 10px;" class="ghd-red" variant="text">
                                <TrashCanSvg />
                            </v-btn>
                        </v-row>
                        
                    </v-col>
                </v-row>
                
            </div>

            <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            :isFromPerformanceCurveEditor=false
            @submit="onSubmitEquationEditorDialogResult"
        />

        </v-row>
</template>

<script setup lang="ts">
    import { toRefs, computed, ref, watch, onBeforeMount, reactive, shallowRef } from 'vue';
    import { getUrl } from '@/shared/utils/get-url';
import { SelectItem } from '@/shared/models/vue/select-item';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { CriteriaConfigRule, CriteriaRule } from '@/shared/models/iAM/criteria';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { clone, findIndex, isNil, propEq, update } from 'ramda';
import { useStore } from 'vuex';
import { AttributeSelectValues } from '@/shared/models/iAM/attribute';
import { hasValue } from '@/shared/utils/has-value-util';
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import { Equation } from '@/shared/models/iAM/equation';

    let store = useStore();
    async function getAttributeSelectValuesAction(payload?: any): Promise<any> {await store.dispatch('getAttributeSelectValues',payload);}
    const stateAttributesSelectValues = computed<AttributeSelectValues[]>(() => store.state.attributeModule.attributesSelectValues);
    const stateCheckedSelectAttributes = computed<string[]>(() => store.state.attributeModule.checkedSelectAttributes);
    const emit = defineEmits(['update:criteriaRule', 'delete'])
    let selectedOperator = ref<string | null>('=');
    let selectedValue = ref<string | null>(null);
    let selectedOperand = ref<string | null>(null)
    let queryRule = ref<any>({})
        let equationEditorDialogData = shallowRef(clone(
        emptyEquationEditorDialogData,
    ));
    const props = defineProps<{
        criteriaRule: CriteriaRule,
        depth: number,
        queryRules: CriteriaConfigRule[],
        id: string
    }>()

    const criteriaValue = computed({
      get() {
        return props.criteriaRule
      },
      set(value) {
        emit('update:criteriaRule', value)
      }
    })

    const mask = { mask: '##########' };

    onBeforeMount(async () => {
        if(!isNil(props.criteriaRule.selectedOperator))
            selectedOperator.value = clone(props.criteriaRule.selectedOperator)
        if(!isNil(props.criteriaRule.value))
            selectedValue.value = clone(props.criteriaRule.value)
        if(isNil(stateCheckedSelectAttributes.value.find(_ => _ === props.criteriaRule.selectedOperand))){
            await getAttributeSelectValuesAction({attributeNames: [props.criteriaRule.selectedOperand]});
        }
        if(props.criteriaRule.isEquation){
            queryRule.value = {
                operators: props.queryRules[0].operators,
                type: "NUMBER",

            } as CriteriaConfigRule
            selectedOperand.value = criteriaValue.value.selectedOperand;
        }
        else
            queryRule.value = props.queryRules.find(_ => _.label === props.criteriaRule.selectedOperand)!
        
        // Conditionally set the operator
        if (criteriaValue.value.selectedOperator === undefined) {
            criteriaValue.value.selectedOperator = '=';
        } else 
        {
            criteriaValue.value.selectedOperator = clone(props.criteriaRule.selectedOperator);
        }
    })

    function onShowEquationEditorDialog(criteriaRule: CriteriaRule) {      
        if (!isNil(criteriaRule)) {

            equationEditorDialogData.value = {
                showDialog: true,
                equation: {expression: criteriaRule.selectedOperand} as Equation,            
            };
        }
    }
    function onSubmitEquationEditorDialogResult(equation: Equation) {
        equationEditorDialogData.value = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) ) {
            selectedOperand.value = equation.expression;
        }
    }

    watch(selectedOperator, (newVal, oldVal) =>{
        criteriaValue.value = setItemPropertyValue(
                    'selectedOperator',
                    newVal,
                    criteriaValue.value
                ) as CriteriaRule
    })

    watch(selectedValue, (newVal, oldVal) =>{
        criteriaValue.value = setItemPropertyValue(
                    'value',
                    newVal,
                    criteriaValue.value
                ) as CriteriaRule
    })

    watch(selectedOperand, (newVal, oldVal) =>{
        criteriaValue.value = setItemPropertyValue(
                    'selectedOperand',
                    newVal,
                    criteriaValue.value
                ) as CriteriaRule
    })

    function onDeleteClick(){    
        emit("delete", props.id)
    }

    function getBorderColor(){
        
        let depthMod = props.depth % 3

        switch(depthMod){
            case 0:{
                return 'blue'
            }
            case 1: {
                return 'green'
            }
            case 2: {
                return 'red'
            }
        }
    }

    
</script>
<style>
    .rule-panel{
        margin: 0px !important;
        border-bottom: 1px solid #ddd;
        background-color: #f5f5f5;
        border-top: 1px solid #ddd;
        border-top-left-radius: 3px;
        border-top-right-radius: 3px;
        border-bottom-left-radius: 3px;
        border-bottom-right-radius: 3px;
        border-left: 1px solid #ddd;
        border-right: 1px solid #ddd;
    }

    .ellipsis-text {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
    }
    
</style>