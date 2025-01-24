<template>
    <v-row>
        <v-col cols="12">            
            <div style="margin-bottom: 10px;">
                <v-btn id="TreatmentSupersedeRulesTab-AddSupersedeRuleBtn" @click='onAddSupersedeRule' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Supersede</v-btn>
                <v-data-table :headers='supersedeRulesGridHeaders' :items='supersedeRulesGridData'
                              id="SupersedeRulesTab-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-asc-icon="custom:GhdTableSortAscSvg"
                              sort-desc-icon="custom:GhdTableSortDescSvg"
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <tr>
                            <td v-for='header in supersedeRulesGridHeaders'>                                
                                <v-select style="margin-top: 15px;"
                                    id="TreatmentSupersedeRulesTab-treatment-select"
                                    :items='treatmentSelectItems'
                                    menu-icon=custom:GhdDownSvg
                                    class='ghd-control-border ghd-control-text ghd-control-width-dd ghd-select'
                                    label='Select a Treatment'
                                    variant="outlined"
                                    density="compact"
                                    item-title="text"
                                    item-value="value"                                    
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"
                                    v-if="header.key === 'treatment'"
                                    v-model='props.item.treatment.name'                                    
                                    @update:model-value='onChangeSupersedeRuleTreatment(props.item, props.item.treatment.name)'
                                >
                                </v-select>

                                <v-menu
                                    location="left"
                                    v-if="header.key === 'criterionLibrary'"
                                >
                                    <template v-slot:activator="{ props }">
                                        <v-btn v-bind="props" id="TreatmentSupersedeRulesTab-CriteriaView-btn" class="ghd-blue" flat icon>
                                            <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')"/>
                                        </v-btn>
                                    </template>
                                    <v-card>
                                        <v-card-text>
                                            <v-textarea
                                                id="TreatmentSupersedeRulesTab-CriteriaView-textarea"
                                                class="sm-txt"
                                                :model-value="
                                                    props.item.criterionLibrary.mergedCriteriaExpression
                                                "
                                                full-width
                                                no-resize
                                                outline
                                                readonly
                                                rows="3"
                                                style = "min-width: 500px;min-height: 205px;"
                                            />
                                        </v-card-text>
                                    </v-card>
                                </v-menu>

                                <v-btn id="TreatmentSupersedeRulesTab-CriteriaEditorBtn" v-if="header.key === 'criterionLibrary'" @click='onShowSupersedeRuleCriterionEditorDialog(props.item)'
                                        class='edit-icon ghd-blue' flat icon>
                                        <img class='img-general img-shift' :src="getUrl('/assets/icons/edit.svg')"/>
                                </v-btn>
                                
                                <v-row v-if="header.key === ''" align="start">
                                    <v-btn id="TreatmentSupersedeTab-DeleteCostBtn" @click='onRemoveSupersedeRule(props.item.id)'
                                        class="ghd-red" flat icon>
                                        <TrashCanSvg />
                                    </v-btn>
                                </v-row>
                            </td>
                        </tr>
                    </template>
                </v-data-table>
            </div>            
        </v-col>

        <GeneralCriterionEditorDialog :dialogData='supersedeCriterionEditorDialogData'
                                                 @submit='onSubmitSupersedeRuleCriterionEditorDialogResult' />
    </v-row>
</template>

<script setup lang='ts'>
import { ref, watch, toRefs, onMounted } from 'vue';
import { any, clone, isNil } from 'ramda';
import { emptySupersedeRule, SimpleTreatment, TreatmentSupersedeRule } from '@/shared/models/iAM/treatment';
import { SelectItem } from '@/shared/models/vue/select-item';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { getUrl } from '@/shared/utils/get-url';
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';

    const props = defineProps<{
        selectedTreatmentSupersedeRules: TreatmentSupersedeRule[],
        treatmentSelectItems: SelectItem[],
        rules: InputValidationRules,
        callFromScenario: boolean,
        callFromLibrary: boolean  
    }>();
    const { selectedTreatmentSupersedeRules, treatmentSelectItems, rules, callFromScenario, callFromLibrary } = toRefs(props);
    const emit = defineEmits(['submit', 'onAddSupersedeRule', 'onModifySupersedeRule', 'onRemoveSupersedeRule'])        
    const supersedeRulesGridHeaders: any[] = [
        { title: 'Treatment', key: 'treatment', align: 'left', sortable: false, class: '', width: '175px' },       
        { title: 'Criteria', key: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '125px' },
        { title: 'Actions', key: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    const supersedeRulesGridData = ref<TreatmentSupersedeRule[]>();
    const supersedeCriterionEditorDialogData = ref(clone(emptyGeneralCriterionEditorDialogData));
    const selectedSupersedeRuleForCriteriaEdit = ref<TreatmentSupersedeRule>(clone(emptySupersedeRule));
    let uuidNIL: string = getBlankGuid();

   created();
   function created() {
    }

    onMounted(() => {
        if(props.selectedTreatmentSupersedeRules != undefined && props.selectedTreatmentSupersedeRules.length > 0)
        supersedeRulesGridData.value = clone(props.selectedTreatmentSupersedeRules);
    })

    watch(() => props.selectedTreatmentSupersedeRules, () => {
        supersedeRulesGridData.value = clone(selectedTreatmentSupersedeRules.value);
    });
    
    function onAddSupersedeRule() {
        const newSupersedeRule: TreatmentSupersedeRule = { ...emptySupersedeRule, id: getNewGuid() };
        emit('onAddSupersedeRule', newSupersedeRule);
    }

    // function onEditSupersedeRuleProperty(supersedeRule: TreatmentSupersedeRule, property: string, value: any) {

    //     emit('onModifySupersedeRule', setItemPropertyValue(property, value, supersedeRule));
    // }

    function onChangeSupersedeRuleTreatment(supersedeRule: TreatmentSupersedeRule, value: string) {
        if (!isNil(value)) {            
            emit('onModifySupersedeRule', setItemPropertyValue('treatment', 
            {...supersedeRule.treatment, id: value} as SimpleTreatment, 
            supersedeRule));
        }
    }

    function onShowSupersedeRuleCriterionEditorDialog(supersedeRule: TreatmentSupersedeRule) {
        selectedSupersedeRuleForCriteriaEdit.value = clone(supersedeRule);

        supersedeCriterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: supersedeRule.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitSupersedeRuleCriterionEditorDialogResult(criterionExpression: string) {
        supersedeCriterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedSupersedeRuleForCriteriaEdit.value.id !== uuidNIL) {
            if(selectedSupersedeRuleForCriteriaEdit.value.criterionLibrary.id === getBlankGuid())
                selectedSupersedeRuleForCriteriaEdit.value.criterionLibrary.id = getNewGuid();
            emit('onModifySupersedeRule', setItemPropertyValue('criterionLibrary', 
            {...selectedSupersedeRuleForCriteriaEdit.value.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary, 
            selectedSupersedeRuleForCriteriaEdit.value));
        }

        selectedSupersedeRuleForCriteriaEdit.value = clone(emptySupersedeRule);
    }

    function onRemoveSupersedeRule(supersedeRuleId: string) {
        emit('onRemoveSupersedeRule', supersedeRuleId);
    }
</script>

<style>
.supersedeRules-tab-content {
    height: 185px;
}

.supersedeRules-data-table {
    height: 215px;
    overflow-y: auto;
    font-family: 'Montserrat', sans-serif;
}
</style>
