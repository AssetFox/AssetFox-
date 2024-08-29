<template>
    <div>
        <v-row v-if="!isNil(criteria)" style="margin: 0px;">
            <v-col style="padding: 0;">
                <v-row :style="{borderLeft: 'solid ' + getBorderColor()}" class="pannel-top">
                    <div style="width: 35%; margin: 10px !important;">
                        <v-row style="margin: 0;">
                            <v-select bg-color="white" density="compact" variant="outlined" style="margin-bottom: -15px;"
                            :items='matchTypes'
                            v-model="selectedMatch"
                            item-title="text"
                            item-value="value"></v-select>
                            <v-btn @click="onDeleteClick" v-if="depth !== 0" style="padding-left: 0;" class="ghd-blue" variant="text">
                                <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')" />
                            </v-btn>
                        </v-row>
                        
                    </div>
                </v-row>
                <v-row :style="{borderLeft: 'solid ' + getBorderColor()}" class="pannel-bottom">
                    <v-col style="padding: 0;">
                        <div style="width:75%; margin: 10px;">
                            <v-row style="margin: 0;">
                                <v-combobox bg-color="white" density="compact" variant="outlined"
                                :items='queryRules'
                                v-model="selectedQueryRule"
                                item-title="label"
                                item-value="label"
                                return-object></v-combobox>
                                <v-btn @click="onAddRuleClick"
                                     variant = "outlined" class='ghd-blue ghd-button-text ghd-button button-spacing' :disabled="selectedQueryRule === null">Add Rule</v-btn>
                                <v-btn @click="onAddGroupClick" v-if="depth + 1 < maxDepth" variant = "outlined" class='ghd-blue ghd-button-text ghd-button'>Add Group</v-btn>
                            </v-row>
                        </div>
                        <div v-if="!isNil(criteriaValue!.children)" >
                            <div v-for="child in criteriaValue!.children!" :key="child.id">
                                <QueryEditorRule v-if="child.type === queryBuilderTypes.QueryBuilderRule" v-model:criteria-rule="(child.query as CriteriaRule)" 
                                :query-rules="queryRules" :depth="depth + 1" 
                                :id="child.id!"
                                style="margin: 10px !important;"
                                @delete='deleteChild'></QueryEditorRule>
                                <self v-if="child.type === queryBuilderTypes.QueryBuilderGroup" style="margin: 10px !important;" 
                                    :query-rules="queryRules" 
                                    :max-depth="maxDepth" 
                                    v-model:criteria="(child.query as Criteria)"  
                                    :depth="depth + 1" 
                                    :id="child.id!"
                                    @delete='deleteChild'></self>
                            </div>
                            
                        </div>
                    </v-col>
                </v-row>
                
            </v-col>
        </v-row>
    </div>
</template>

<script setup lang="ts">
import { SelectItem } from '@/shared/models/vue/select-item';
import { toRefs, computed, ref, onBeforeMount, watch, shallowRef } from 'vue';
import QueryEditorRule from './QueryEditorRule.vue';
import { getUrl } from '@/shared/utils/get-url';
import self from '../queryEditor/QueryEditorGroup.vue'
import { doesNotMatch } from 'assert';
import { Criteria, CriteriaConfigRule, CriteriaRule, CriteriaType } from '@/shared/models/iAM/criteria'
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { clone, isNil } from 'ramda';
import { getNewGuid } from '@/shared/utils/uuid-utils';
import { getuid } from 'process';
    
const emit = defineEmits(['update:criteria', 'delete'])
    const matchTypes = ref<SelectItem[]>([{text: 'AND', value:'AND'}, {text: 'OR', value: 'OR'}]);
    let selectedQueryRule = ref<CriteriaConfigRule | null>(null)
    let passingRule: CriteriaConfigRule | null = null;
    let selectedMatch = ref('');

    const queryBuilderTypes: any = {
        QueryBuilderRule: 'query-builder-rule',
        QueryBuilderGroup: 'query-builder-group',
    };
    
    const props = defineProps<{
        criteria: Criteria | null,
        depth: number,
        queryRules: CriteriaConfigRule[],
        maxDepth: number,
        id?: string
    }>()

    const criteriaValue = computed({
      get() {
        return props.criteria
      },
      set(value) {
        emit('update:criteria', value)
      }
    })

    onBeforeMount(() => {
        if(!isNil(props.criteria))
            selectedMatch.value = props.criteria?.logicalOperator
    })

    watch(selectedMatch, (newVal, oldVal) => {
        criteriaValue.value = setItemPropertyValue(
                    'logicalOperator',
                    newVal,
                    criteriaValue.value
                ) as Criteria
    })

    watch(() => props.criteria, () => {
        if(!isNil(props.criteria))
            selectedMatch.value = props.criteria?.logicalOperator
    })

    watch(selectedQueryRule, () => {
        passingRule = selectedQueryRule.value;
    })

    function onAddRuleClick(){
        
        let newRule: CriteriaRule = {
            rule: selectedQueryRule.value!.label,
            selectedOperand: selectedQueryRule.value!.label,
        }
        let newCriteriaType: CriteriaType = {
            type: queryBuilderTypes.QueryBuilderRule,
            query: newRule,
            id: getNewGuid()
        }
        if(isNil(criteriaValue.value!.children)){
            criteriaValue.value = setItemPropertyValue(
                    'children',
                    [newCriteriaType],
                    criteriaValue.value
                ) as Criteria
        }
        else
            criteriaValue.value!.children!.push(newCriteriaType)
        
    }

    function onAddGroupClick(){
        let newCriteria: Criteria = {
            logicalOperator: "AND",
        }
        let newCriteriaType: CriteriaType = {
            type: queryBuilderTypes.QueryBuilderGroup,
            query: newCriteria,
            id: getNewGuid()
        }
        if(isNil(criteriaValue.value!.children)){
            criteriaValue.value = setItemPropertyValue(
                    'children',
                    [newCriteriaType],
                    criteriaValue.value
                ) as Criteria
        }
        else
            criteriaValue.value!.children!.push(newCriteriaType)
    }

    function onDeleteClick(){
        emit("delete", props.id)
    }

    function deleteChild(id: string){
        criteriaValue.value!.children = criteriaValue.value!.children!.filter((child: CriteriaType) =>
                !(child.id == id));
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
    .pannel-top{
        margin: 0px;
        border-bottom: 1px solid #ddd;
        background-color: #f5f5f5;
        border-top: 1px solid #ddd;
        border-top-left-radius: 3px;
        border-top-right-radius: 3px;
        border-left: 1px solid #ddd;
        border-right: 1px solid #ddd;
    }

    .pannel-bottom{
        margin: 0px !important;
        border-left: 1px solid #ddd;
        border-right: 1px solid #ddd;
        border-bottom: 1px solid #ddd;
        border-bottom-left-radius: 3px;
        border-bottom-right-radius: 3px;
    }

    .pannel-div{
        padding: 10px;
    }
    
    .button-spacing{
        margin-left: 5px;
        margin-right: 5px;
    }
</style>
