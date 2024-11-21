<template>
    <v-row>
        <v-col cols="12">            
            <div style="margin-bottom: 10px;">
                <v-btn id="TreatmentConsequencesTab-AddConsequenceBtn" @click='onAddConsequence' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Consequence</v-btn>
                <v-data-table :headers='consequencesGridHeaders' :items='consequencesGridData'
                              id="ConsequencesTab-Consequences-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-asc-icon="custom:GhdTableSortAscSvg"
                              sort-desc-icon="custom:GhdTableSortDescSvg"
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <tr>
                        <td v-for='header in consequencesGridHeaders'>
                            <editDialog
                                v-if="header.key !== 'equation' && header.key !== 'criterionLibrary' && header.key !== ''"
                                v-model:return-value='props.item[header.key]'
                                @save='onEditConsequenceProperty(props.item, header.key, props.item[header.key])'
                                size="large" lazy persistent>
                                <v-text-field v-if="header.key === 'attribute'" variant="underlined" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field variant="underlined" v-if="header.key === 'changeValue'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.changeValue'
                                              :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                <template v-slot:input>
                                    <v-select v-if="header.key === 'attribute'" :items='attributeSelectItems'
                                        item-title="text"
                                        item-value="value"   
                                        menu-icon=custom:GhdDownSvg
                                        variant="underlined"
                                        label='Edit'
                                        v-model='props.item.attribute'
                                        :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field variant="underlined" v-if="header.key === 'changeValue'" label='Edit'
                                                  v-model='props.item.changeValue'
                                                  :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                </template>
                            </editDialog>

                            <v-menu
                                location="left"
                                v-if="header.key === 'equation'"
                            >
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props" id="TreatmentConsequencesTab-EquationView-btn" class="ghd-blue" flat icon>
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
                                            style = "min-width: 500px;min-height: 205px;"
                                        />
                                    </v-card-text>
                                </v-card>
                            </v-menu>     
                             <v-btn id="TreatmentConsequencesTab-EquationEditorBtn" v-if="header.key === 'equation'" @click='onShowConsequenceEquationEditorDialog(props.item)' class='edit-icon ghd-blue'
                                    flat icon>
                                    <img class='img-general img-shift' :src="getUrl('/assets/icons/edit.svg')"/>
                            </v-btn>                       

                            <v-menu
                                location="left"
                                v-if="header.key === 'criterionLibrary'"
                            >
                                <template v-slot:activator="{ props }">
                                    <v-btn v-bind="props" id="TreatmentConsequencesTab-CriteriaView-btn" class="ghd-blue" flat icon>
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
                                            style = "min-width: 500px;min-height: 205px;"
                                        />
                                    </v-card-text>
                                </v-card>
                            </v-menu>
                            <v-btn id="TreatmentConsequencesTab-CriteriaEditorBtn" v-if="header.key === 'criterionLibrary'" @click='onShowConsequenceCriterionEditorDialog(props.item)'
                                    class='edit-icon ghd-blue' flat icon>
                                    <img class='img-general img-shift' :src="getUrl('/assets/icons/edit.svg')"/>
                            </v-btn>

                            <v-row v-if="header.key === ''" align="start">
                                <v-btn id="TreatmentConquencesTab-DeleteCostBtn" @click='onRemoveConsequence(props.item.id)' 
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

        <EquationEditorDialog :dialogData='consequenceEquationEditorDialogData'
                                         :isFromPerformanceCurveEditor=false
                                         @submit='onSubmitConsequenceEquationEditorDialogResult' />

        <GeneralCriterionEditorDialog :dialogData='consequenceCriterionEditorDialogData'
                                                 @submit='onSubmitConsequenceCriterionEditorDialogResult' />
    </v-row>
</template>

<script setup lang='ts'>
import { ref, computed, shallowRef, watch, toRefs, onMounted } from 'vue';
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
import TrashCanSvg from '@/shared/icons/TrashCanSvg.vue';
import EditSvg from '@/shared/icons/EditSvg.vue';

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

    onMounted(() => {
        if(props.selectedTreatmentConsequences.length > 0)
        consequencesGridData.value = clone(props.selectedTreatmentConsequences);
    })

    watch(() => props.selectedTreatmentConsequences, () => {
        consequencesGridData.value = clone(selectedTreatmentConsequences.value);
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
