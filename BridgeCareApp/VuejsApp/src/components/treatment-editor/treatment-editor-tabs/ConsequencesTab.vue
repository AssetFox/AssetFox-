<template>
    <v-row>
        <v-col cols="12">            
            <div style="margin-bottom: 10px;">
                <v-data-table :headers='consequencesGridHeaders' :items='consequencesGridData'
                              id="ConsequencesTab-Consequences-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <tr>
                        <td v-for='header in consequencesGridHeaders'>
                            <editDialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditConsequenceProperty(props.item, header.value, props.item[header.value])'
                                size="large" lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'changeValue'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.changeValue'
                                              :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                <template v-slot:input>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                        item-title="text"
                                        item-value="value"   
                                        append-icon=ghd-down
                                        label='Edit'
                                        v-model='props.item.attribute'
                                        :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'changeValue'" label='Edit'
                                                  v-model='props.item.changeValue'
                                                  :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                </template>
                            </editDialog>

                            <v-menu
                                location="left"
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'equation'"
                            >
                                <template v-slot:activator>
                                    <v-btn id="TreatmentConsequencesTab-EquationView-btn" class="ghd-blue" flat>
                                        <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea id="TreatmentConsequencesTab-EquationView-textarea"
                                            class="sm-txt"
                                            :model-value="
                                                props.item.equation.expression
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
                             <v-btn id="TreatmentConsequencesTab-EquationEditorBtn" v-if="header.value === 'equation'" @click='onShowConsequenceEquationEditorDialog(props.item)' class='edit-icon'
                                    icon>
                                <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                            </v-btn>                       

                            <v-menu
                                location="left"
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'criterionLibrary'"
                            >
                                <template v-slot:activator>
                                    <v-btn id="TreatmentConsequencesTab-CriteriaView-btn" class="ghd-blue" flat>
                                        <img class='img-general' :src="getUrl('assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            id="TreatmentConsequencesTab-CriteriaView-textarea"
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
                            <v-btn id="TreatmentConsequencesTab-CriteriaEditorBtn" v-if="header.value === 'criterionLibrary'" @click='onShowConsequenceCriterionEditorDialog(props.item)'
                                    class='edit-icon' icon>
                                <img class='img-general' :src="getUrl('assets/icons/edit.svg')"/>
                            </v-btn>

                            <v-row v-if="header.value === ''" align-start>
                                <v-btn id="TreatmentConquencesTab-DeleteCostBtn" @click='onRemoveConsequence(props.item.id)' icon>
                                    <img class='img-general' :src="getUrl('assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </v-row>
                        </td>
                    </tr>
                    </template>
                </v-data-table>
            </div>
            <v-btn id="TreatmentConsequencesTab-AddConsequenceBtn" @click='onAddConsequence' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Consequence</v-btn>
        </v-col>

        <EquationEditorDialog :dialogData='consequenceEquationEditorDialogData'
                                         :isFromPerformanceCurveEditor=false
                                         @submit='onSubmitConsequenceEquationEditorDialogResult' />

        <GeneralCriterionEditorDialog :dialogData='consequenceCriterionEditorDialogData'
                                                 @submit='onSubmitConsequenceCriterionEditorDialogResult' />
    </v-row>
</template>

<script setup lang='ts'>
import { ref, computed, shallowRef, watch, toRefs } from 'vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import { any, clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import { useStore } from 'vuex';
import { emptyConsequence, TreatmentConsequence } from '@/shared/models/iAM/treatment';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';

import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { Equation } from '@/shared/models/iAM/equation';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';
import GeneralCriterionEditorDialog from '@/shared/modals/GeneralCriterionEditorDialog.vue';
import { emptyGeneralCriterionEditorDialogData, GeneralCriterionEditorDialogData } from '@/shared/models/modals/general-criterion-editor-dialog-data';
import { getUrl } from '@/shared/utils/get-url';

const props = defineProps<{
         selectedTreatmentConsequences: TreatmentConsequence[],
         rules: InputValidationRules,
         callFromScenario: boolean,
         callFromLibrary: boolean  
    }>(); 
    const { selectedTreatmentConsequences, rules, callFromScenario, callFromLibrary } = toRefs(props);
    const emit = defineEmits(['submit', 'onAddConsequence', 'onModifyConsequence', 'onRemoveConsequence'])
    let store = useStore();

    const stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    

    const consequencesGridHeaders: any[] = [
        { title: 'Attribute', key: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { title: 'Change Value', key: 'changeValue', align: 'left', sortable: false, class: '', width: '125px' },
        { title: 'Equation', key: 'equation', align: 'left', sortable: false, class: '', width: '125px' },
        { title: 'Criteria', key: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '125px' },
        { title: 'Actions', key: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    const consequencesGridData = ref<TreatmentConsequence[]>();
    const consequenceEquationEditorDialogData = ref(clone(emptyEquationEditorDialogData));
    const consequenceCriterionEditorDialogData = ref(clone(emptyGeneralCriterionEditorDialogData));
    const selectedConsequenceForEquationOrCriteriaEdit = ref<TreatmentConsequence>(clone(emptyConsequence));
    const attributeSelectItems = ref<SelectItem[]>([]);
    let uuidNIL: string = getBlankGuid();

   created();
   function created() {
        setAttributeSelectItems();
    }

    watch(selectedTreatmentConsequences, () => {
        consequencesGridData.value = clone(props.selectedTreatmentConsequences);
    });

    watch(stateAttributes, () => {
        setAttributeSelectItems();
    });

    function setAttributeSelectItems() {
        if (hasValue(stateAttributes.value)) {
            attributeSelectItems.value = stateAttributes.value.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }));
        }
    }

    function onAddConsequence() {
        const newConsequence: TreatmentConsequence = { ...emptyConsequence, id: getNewGuid() };
        emit('onAddConsequence', newConsequence);
    }

    function onEditConsequenceProperty(consequence: TreatmentConsequence, property: string, value: any) {
        emit('onModifyConsequence', setItemPropertyValue(property, value, consequence));
    }

    function onShowConsequenceEquationEditorDialog(consequence: TreatmentConsequence) {
        selectedConsequenceForEquationOrCriteriaEdit.value = clone(consequence);

        consequenceEquationEditorDialogData.value = {
            showDialog: true,
            equation: clone(consequence.equation),
        };
    }

    function onSubmitConsequenceEquationEditorDialogResult(equation: Equation) {
        consequenceEquationEditorDialogData.value = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && selectedConsequenceForEquationOrCriteriaEdit.value.id !== uuidNIL) {
            emit('onModifyConsequence', setItemPropertyValue('equation', equation, selectedConsequenceForEquationOrCriteriaEdit.value));
        }

        selectedConsequenceForEquationOrCriteriaEdit.value = clone(emptyConsequence);
    }

    function onShowConsequenceCriterionEditorDialog(consequence: TreatmentConsequence) {
        selectedConsequenceForEquationOrCriteriaEdit.value = clone(consequence);

        consequenceCriterionEditorDialogData.value = {
            showDialog: true,
            CriteriaExpression: consequence.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitConsequenceCriterionEditorDialogResult(criterionExpression: string) {
        consequenceCriterionEditorDialogData.value = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedConsequenceForEquationOrCriteriaEdit.value.id !== uuidNIL) {
            if(selectedConsequenceForEquationOrCriteriaEdit.value.criterionLibrary.id === getBlankGuid())
                selectedConsequenceForEquationOrCriteriaEdit.value.criterionLibrary.id = getNewGuid();
            emit('onModifyConsequence', setItemPropertyValue('criterionLibrary', 
            {...selectedConsequenceForEquationOrCriteriaEdit.value.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary, 
            selectedConsequenceForEquationOrCriteriaEdit.value));
        }

        selectedConsequenceForEquationOrCriteriaEdit.value = clone(emptyConsequence);
    }

    function onRemoveConsequence(consequenceId: string) {
        emit('onRemoveConsequence', consequenceId);
    }

</script>

<style>
.consequences-tab-content {
    height: 185px;
}

.consequences-data-table {
    height: 215px;
    overflow-y: auto;
    font-family: 'Montserrat', sans-serif;
}
</style>
