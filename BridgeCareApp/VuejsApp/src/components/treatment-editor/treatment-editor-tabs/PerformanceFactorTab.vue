<template>
    <v-layout class='factor-tab-content'>
        <v-flex xs12>            
            <div class='factor-data-table'>
                <v-data-table :headers='factorGridHeaders' :items='factorGridData'
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=$vuetify.icons.ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <td v-for='header in factorGridHeaders'>
                            <v-edit-dialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditPerformanceFactorProperty(props.item, header.value, props.item[header.value])'
                                size="large" lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'performanceFactor'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='parseFloat(props.item.performanceFactor).toFixed(2)'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template v-slot:input>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                             append-icon=$vuetify.icons.ghd-down
                                              label='Edit'
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'performanceFactor'" label='Edit' single-line maxLength="5"
                                                  v-model='props.item.performanceFactor'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                </template>
                            </v-edit-dialog>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>
    </v-layout>
</template>

<script lang='ts' setup>
import Vue, { onMounted, ref, shallowRef } from 'vue';
import { TreatmentAttributeFactor, TreatmentPerformanceFactor, Treatment } from '@/shared/models/iAM/treatment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';

import {
    PerformanceCurve,
} from '@/shared/models/iAM/performance';
import { isNullOrUndefined } from 'util';
import { any, clone } from 'ramda';
import { useStore } from 'vuex';
import { inject, reactive, onBeforeUnmount, watch, Ref} from 'vue';

    let selectedTreatmentPerformanceFactors: TreatmentPerformanceFactor[];
    let selectedTreatment = shallowRef<Treatment>(); 
    let scenarioId: string;
    let rules: InputValidationRules;
    let callFromScenario: boolean;
    let callFromLibrary: boolean;

    const emit = defineEmits(['submit', 'onAddConsequence', 'onModifyConsequence', 'onRemoveConsequence','onModifyPerformanceFactor'])
    let store = useStore();

    let stateAttributes = ref<Attribute[]>(store.state.attributeModule.attributes);
    let stateScenarioPerformanceCurves = ref<PerformanceCurve[]>(store.state.performanceCurveModule.scenarioPerformanceCurves)

    let factorGridHeaders: DataTableHeader[] = [
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { text: 'Performance Factor', value: 'performanceFactor', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    let factorGridData: TreatmentPerformanceFactor[] = [];
    let attributeSelectItems: SelectItem[] = [];
    let uuidNIL: string = getBlankGuid();


    onMounted(() => mounted());
    function mounted() {
        setAttributeSelectItems();
   }

   watch(stateAttributes, () => onStateAttributesChanged)
    function onStateAttributesChanged() {
        setAttributeSelectItems();
    }

    watch(selectedTreatment, () => onSelectedTreatmentChanged)
    function onSelectedTreatmentChanged() {
        if (selectedTreatmentPerformanceFactors.length <= 0) {
           buildDataFromCurves(); 
           factorGridData.forEach(_ => emit('onModifyPerformanceFactor', clone(_)))
        }
        else{
            factorGridData.forEach(data => {
                let found = false;
                selectedTreatmentPerformanceFactors.forEach(factors => {
                    if (factors.attribute === data.attribute) {
                        data.id = factors.id;
                        data.performanceFactor = factors.performanceFactor;
                        found = true
                    }
                });
                if(!found)
                    emit('onModifyPerformanceFactor', clone(data))
            });
        }
    }
    watch(stateScenarioPerformanceCurves, () => onStatePerformanceCurvesChanged)
   function onStatePerformanceCurvesChanged() {
        buildDataFromCurves();
    }
    function setAttributeSelectItems() {
        if (hasValue(stateAttributes)) {
            attributeSelectItems = stateAttributes.value.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }));
        }
    }
    function onEditPerformanceFactorProperty(performancefactor: TreatmentPerformanceFactor, property: string, value: any) {
        performancefactor.performanceFactor = value;
        emit('onModifyPerformanceFactor', setItemPropertyValue(property, value, performancefactor));
    }
    function buildDataFromCurves() {
        if(callFromLibrary)
            return;
        let testStateAttributes:Attribute[] = [];
        stateScenarioPerformanceCurves.value.forEach(curve => {
            stateAttributes.value.forEach(state => {
                if (state.name === curve.attribute) {
                    if (testStateAttributes.findIndex((o) => { return o.name === curve.attribute }) === -1){
                        testStateAttributes.push(state);
                    }
                }
            });
        });

        factorGridData = testStateAttributes.map(_ => ({
            id: getNewGuid(),
            attribute: _.name,
            performanceFactor: 1.0
        }));
    }
</script>

<style>
.factor-tab-content {
    height: 185px;
}

.factor-data-table {
    height: 415px;
    overflow-y: auto;
    font-family: 'Montserrat', sans-serif;
}
</style>
