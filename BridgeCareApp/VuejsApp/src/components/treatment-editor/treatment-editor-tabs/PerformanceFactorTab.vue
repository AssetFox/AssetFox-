<template>
    <v-row>
        <v-col>            
            <div class='factor-data-table'>
                <v-data-table :headers='factorGridHeaders' :items='factorGridData'
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props' v-slot:item="props">
                        <td v-for='header in factorGridHeaders'>
                            <editDialog
                                v-if="header.key !== 'equation' && header.key !== 'criterionLibrary' && header.key !== ''"
                                :return-value.sync='props.item[header.key]'
                                @save='onEditPerformanceFactorProperty(props.item, header.key, props.item[header.key])'
                                size="large" lazy persistent>
                                <v-text-field v-if="header.key === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.key === 'performanceFactor'" readonly single-line class='ghd-control-text-sm'
                                              :model-value='parseFloat(props.item.performanceFactor).toFixed(2)'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template v-slot:input>
                                    <v-select v-if="header.key === 'attribute'" :items='attributeSelectItems'
                                             append-icon=ghd-down
                                              label='Edit'
                                              item-title="text"
                                              item-value="value" 
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.key === 'performanceFactor'" label='Edit' single-line maxLength="5"
                                                  v-model='props.item.performanceFactor'
                                                  :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                </template>
                            </editDialog>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-col>
    </v-row>
</template>

<script setup lang='ts'>
import { watch, toRefs, computed, onMounted, ref, shallowRef } from 'vue';
import { TreatmentAttributeFactor, TreatmentPerformanceFactor, Treatment } from '@/shared/models/iAM/treatment';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import {
    PerformanceCurve,
} from '@/shared/models/iAM/performance';
import { clone } from 'ramda';
import { useStore } from 'vuex';

    const props = defineProps<{
        selectedTreatmentPerformanceFactors: TreatmentPerformanceFactor[],
        selectedTreatment:Treatment,
        scenarioId: string,
        rules: InputValidationRules,
        callFromScenario: boolean,
        callFromLibrary: boolean
    }>(); 
    const { selectedTreatmentPerformanceFactors, selectedTreatment, scenarioId, rules, callFromScenario, callFromLibrary } = toRefs(props);
    const emit = defineEmits(['submit', 'onAddConsequence', 'onModifyConsequence', 'onRemoveConsequence','onModifyPerformanceFactor']);
    let store = useStore();

    const stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    const stateScenarioPerformanceCurves = computed<PerformanceCurve[]>(() => store.state.performanceCurveModule.scenarioPerformanceCurves)

    const factorGridHeaders: any[] = [
        { title: 'Attribute', key: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { title: 'Performance Factor', key: 'performanceFactor', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    const factorGridData = ref<TreatmentPerformanceFactor[]>([]);
    const attributeSelectItems = ref<SelectItem[]>([]);
    let uuidNIL: string = getBlankGuid();


    onMounted(() =>  {
        setAttributeSelectItems();

        onTreatmentChange();
    });

    watch(stateAttributes, () =>  {
        setAttributeSelectItems();
    });

    watch(selectedTreatment, onTreatmentChange);

    function onTreatmentChange(){
        if (selectedTreatmentPerformanceFactors.value.length <= 0) {
           buildDataFromCurves(); 
           factorGridData.value.forEach(_ => emit('onModifyPerformanceFactor', clone(_)))
        }
        else{
            factorGridData.value.forEach(data => {
                let found = false;
                selectedTreatmentPerformanceFactors.value.forEach(factors => {
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

    watch(stateScenarioPerformanceCurves, () => {
        buildDataFromCurves();
    });

    function setAttributeSelectItems() {
        if (hasValue(stateAttributes.value)) {
            attributeSelectItems.value = stateAttributes.value.map((attribute: Attribute) => ({
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
        if(callFromLibrary.value)
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

        factorGridData.value = testStateAttributes.map(_ => ({
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
