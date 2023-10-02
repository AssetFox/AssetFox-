<template>
    <v-layout class='consequences-tab-content'>
        <v-flex xs12>            
            <div class='consequences-data-table'>
                <v-data-table :headers='consequencesGridHeaders' :items='consequencesGridData'
                              id="ConsequencesTab-Consequences-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=$vuetify.icons.ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:items="props">
                        <td v-for='header in consequencesGridHeaders'>
                            <v-edit-dialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditConsequenceProperty(props.item, header.value, props.item[header.value])'
                                size="large" lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'changeValue'" readonly single-line class='ghd-control-text-sm'
                                              :value='props.item.changeValue'
                                              :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                <template slot='input'>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                             append-icon=$vuetify.icons.ghd-down
                                              label='Edit'
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'changeValue'" label='Edit'
                                                  v-model='props.item.changeValue'
                                                  :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                </template>
                            </v-edit-dialog>

                            <v-menu
                                location="left"
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'equation'"
                            >
                                <template slot="activator">
                                    <v-btn class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            class="sm-txt"
                                            :value="
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
                             <v-btn v-if="header.value === 'equation'" @click='onShowConsequenceEquationEditorDialog(props.item)' class='edit-icon'
                                    icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>                       

                            <v-menu
                                location="left"
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'criterionLibrary'"
                            >
                                <template slot="activator">
                                    <v-btn class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            class="sm-txt"
                                            :value="
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
                            <v-btn v-if="header.value === 'criterionLibrary'" @click='onShowConsequenceCriterionEditorDialog(props.item)'
                                    class='edit-icon' icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>

                            <v-layout v-if="header.value === ''" align-start>
                                <v-btn @click='onRemoveConsequence(props.item.id)' icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
            <v-btn @click='onAddConsequence' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Consequence</v-btn>
        </v-flex>

        <ConsequenceEquationEditorDialog :dialogData='consequenceEquationEditorDialogData'
                                         :isFromPerformanceCurveEditor=false
                                         @submit='onSubmitConsequenceEquationEditorDialogResult' />

        <GeneralCriterionEditorDialog :dialogData='consequenceCriterionEditorDialogData'
                                                 @submit='onSubmitConsequenceCriterionEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts' setup>
import Vue, { shallowRef } from 'vue';
import { any, clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import { inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { emptyConsequence, TreatmentConsequence } from '@/shared/models/iAM/treatment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
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

    let selectedTreatmentConsequences = shallowRef<TreatmentConsequence[]>();
    let rules: InputValidationRules;
    let callFromScenario: boolean;
    let callFromLibrary: boolean;
    const emit = defineEmits(['submit', 'onAddConsequence', 'onModifyConsequence', 'onRemoveConsequence'])
    let store = useStore();

    let stateAttributes = ref<Attribute[]>(store.state.attributeModule.attributes);
    

    let consequencesGridHeaders: DataTableHeader[] = [
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { text: 'Change Value', value: 'changeValue', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Criteria', value: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Actions', value: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    let consequencesGridData = shallowRef<TreatmentConsequence[]>();
    let consequenceEquationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
    let consequenceCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
    let selectedConsequenceForEquationOrCriteriaEdit: TreatmentConsequence = clone(emptyConsequence);
    let attributeSelectItems: SelectItem[] = [];
    let uuidNIL: string = getBlankGuid();

   created();
   function created() {
        setAttributeSelectItems();
    }

    watch(selectedTreatmentConsequences, () => onSelectedTreatmentConsequencesChanged)
     async function onSelectedTreatmentConsequencesChanged() {
        consequencesGridData = clone(selectedTreatmentConsequences);
    }

    watch(stateAttributes, () => onStateAttributesChanged)
     async function onStateAttributesChanged() {
        setAttributeSelectItems();
    }

    function setAttributeSelectItems() {
        if (hasValue(stateAttributes)) {
            attributeSelectItems = stateAttributes.value.map((attribute: Attribute) => ({
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
        selectedConsequenceForEquationOrCriteriaEdit = clone(consequence);

        consequenceEquationEditorDialogData = {
            showDialog: true,
            equation: clone(consequence.equation),
        };
    }

    function onSubmitConsequenceEquationEditorDialogResult(equation: Equation) {
        consequenceEquationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && selectedConsequenceForEquationOrCriteriaEdit.id !== uuidNIL) {
            emit('onModifyConsequence', setItemPropertyValue('equation', equation, selectedConsequenceForEquationOrCriteriaEdit));
        }

        selectedConsequenceForEquationOrCriteriaEdit = clone(emptyConsequence);
    }

    function onShowConsequenceCriterionEditorDialog(consequence: TreatmentConsequence) {
        selectedConsequenceForEquationOrCriteriaEdit = clone(consequence);

        consequenceCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: consequence.criterionLibrary.mergedCriteriaExpression,
        };
    }

    function onSubmitConsequenceCriterionEditorDialogResult(criterionExpression: string) {
        consequenceCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && selectedConsequenceForEquationOrCriteriaEdit.id !== uuidNIL) {
            if(selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary.id === getBlankGuid())
                selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary.id = getNewGuid();
            emit('onModifyConsequence', setItemPropertyValue('criterionLibrary', 
            {...selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary, 
            selectedConsequenceForEquationOrCriteriaEdit));
        }

        selectedConsequenceForEquationOrCriteriaEdit = clone(emptyConsequence);
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
