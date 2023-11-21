<template>
    <v-row>
        <v-col cols="12">            
            <div style="margin-bottom: 10px;">
                <v-data-table :headers='supersedeRulesGridHeaders' :items='supersedeRulesGridData'
                              id="supersedeRulesTab-supersedeRules-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <tr>
                        <td v-for='header in supersedeRulesGridHeaders'>
                            <!-- todo treatment select list -->

                            <v-menu
                                location="left"
                                min-height="500px"
                                min-width="500px"
                                v-if="header.key === 'criterionLibrary'"
                            >
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props" variant="flat" id="TreatmentsupersedeRulesTab-CriteriaView-btn" class="ghd-blue" flat>
                                        <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            id="TreatmentsupersedeRulesTab-CriteriaView-textarea"
                                            class="sm-txt"
                                            :model-value="
                                                props.item.criterionLibrary.mergedCriteriaExpression
                                            "
                                            full-width
                                            no-resize
                                            outline
                                            readonly
                                            rows="3"
                                        />
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                            <v-btn id="TreatmentsupersedeRulesTab-CriteriaEditorBtn" v-if="header.key === 'criterionLibrary'" @click='onShowSupersedeRuleCriterionEditorDialog(props.item)'
                                    variant="flat" class='edit-icon' icon>
                                <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                            </v-btn>

                            <v-row v-if="header.key === ''" align="start">
                                <v-btn variant="flat" id="TreatmentConquencesTab-DeleteCostBtn" @click='onRemoveSupersedeRule(props.item.id)' icon>
                                    <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </v-row>
                        </td>
                    </tr>
                    </template>
                </v-data-table>
            </div>
            <v-btn id="TreatmentsupersedeRulesTab-AddSupersedeRuleBtn" @click='onAddSupersedeRule' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Supersede</v-btn>
        </v-col>

        <GeneralCriterionEditorDialog :dialogData='supersedeCriterionEditorDialogData'
                                                 @submit='onSubmitSupersedeRuleCriterionEditorDialogResult' />
    </v-row>
</template>

<script setup lang='ts'>
import { ref, computed, shallowRef, watch, toRefs, onMounted } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import { any, clone, isNil } from 'ramda';
import { useStore } from 'vuex';
import { emptySupersedeRule, TreatmentSupersedeRule } from '@/shared/models/iAM/treatment';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { getUrl } from '@/shared/utils/get-url';

const props = defineProps<{
         selectedTreatmentsupersedeRules: TreatmentSupersedeRule[],
         rules: InputValidationRules,
         callFromScenario: boolean,
         callFromLibrary: boolean  
    }>(); 
    const { selectedTreatmentsupersedeRules, rules, callFromScenario, callFromLibrary } = toRefs(props);
    const emit = defineEmits(['submit', 'onAddSupersedeRule', 'onModifySupersedeRule', 'onRemoveSupersedeRule'])
    let store = useStore();

    const supersedeRulesGridHeaders: any[] = [
        { title: 'Treatment', key: 'treatment', align: 'left', sortable: false, class: '', width: '175px' },       
        { title: 'Criteria', key: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '125px' },
        { title: 'Actions', key: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    const supersedeRulesGridData = ref<TreatmentSupersedeRule[]>();
    const supersedeCriterionEditorDialogData = ref(clone(emptyGeneralCriterionEditorDialogData));
    const selectedCriteriaEdit = ref<TreatmentSupersedeRule>(clone(emptySupersedeRule));
    const attributeSelectItems = ref<SelectItem[]>([]);
    let uuidNIL: string = getBlankGuid();

   created();
   function created() {
    }

    onMounted(() => {
        if(props.selectedTreatmentsupersedeRules.length > 0)
        supersedeRulesGridData.value = clone(props.selectedTreatmentsupersedeRules);
    })

    watch(() => props.selectedTreatmentsupersedeRules, () => {
        supersedeRulesGridData.value = clone(selectedTreatmentsupersedeRules.value);
    });
    
    function onAddSupersedeRule() {
        const newSupersedeRule: TreatmentSupersedeRule = { ...emptySupersedeRule, id: getNewGuid() };
        emit('onAddSupersedeRule', newSupersedeRule);
    }

    function onEditSupersedeRuleProperty(supersedeRule: TreatmentSupersedeRule, property: string, value: any) {
        emit('onModifySupersedeRule', setItemPropertyValue(property, value, supersedeRule));
    }

    function onShowSupersedeRuleCriterionEditorDialog(supersedeRule: TreatmentSupersedeRule) {
        selectedCriteriaEdit.value = clone(supersedeRule);

        supersedeCriterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: supersedeRule.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitSupersedeRuleCriterionEditorDialogResult(criterionExpression: string) {
        supersedeCriterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedCriteriaEdit.value.id !== uuidNIL) {
            if(selectedCriteriaEdit.value.criterionLibrary.id === getBlankGuid())
                selectedCriteriaEdit.value.criterionLibrary.id = getNewGuid();
            emit('onModifySupersedeRule', setItemPropertyValue('criterionLibrary', 
            {...selectedCriteriaEdit.value.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary, 
            selectedCriteriaEdit.value));
        }

        selectedCriteriaEdit.value = clone(emptySupersedeRule);
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
