<template>
    <v-layout class='factor-tab-content'>
        <v-flex xs12>            
            <div class='factor-data-table'>
                <v-data-table :headers='factorGridHeaders' :items='factorGridData'
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=$vuetify.icons.ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props'>
                        <td v-for='header in factorGridHeaders'>
                            <v-edit-dialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditPerformanceFactorProperty(props.item, header.value, props.item[header.value])'
                                large lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'factor'" readonly single-line class='ghd-control-text-sm'
                                              :value='parseFloat(props.item.factor).toFixed(2)'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template slot='input'>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                             append-icon=$vuetify.icons.ghd-down
                                              label='Edit'
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'factor'" label='Edit' single-line maxLength="5"
                                                  v-model='props.item.factor'
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

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { State } from 'vuex-class';
import { clone, isNil } from 'ramda';
import { TreatmentAttributeFactor, TreatmentPerformanceFactor, Treatment } from '@/shared/models/iAM/treatment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';

@Component({
    components: {
    },
})
export default class PerformanceFactorTab extends Vue {
    @Prop() treatmentAttributeFactor: TreatmentPerformanceFactor[];
    @Prop() selectedTreatment: Treatment;
    @Prop() rules: InputValidationRules;
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

    factorGridHeaders: DataTableHeader[] = [
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { text: 'Performance Factor', value: 'factor', align: 'left', sortable: false, class: '', width: '125px' },
    ];
    factorGridData: TreatmentAttributeFactor[] = [];
    attributeSelectItems: SelectItem[] = [];
    uuidNIL: string = getBlankGuid();

    mounted() {
        console.log("" + this.selectedTreatmentPerformanceFactors);
        this.setAttributeSelectItems();
    }

    @Watch('selectedTreatmentPerformanceFactors')
    onSelectedTreatmentPerformanceFactorsChanged() {
        console.log("selected treatment: " + this.selectedTreatment.name);
        this.factorGridData = clone(this.selectedTreatmentPerformanceFactors);
    }

    @Watch('stateAttributes')
    onStateAttributesChanged() {
        this.setAttributeSelectItems();
    }

    setAttributeSelectItems() {
        if (hasValue(this.stateAttributes)) {
            this.attributeSelectItems = this.stateAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }));
  
            this.factorGridData = this.attributeSelectItems.map(_ => ({
                attribute : _.value,
                factor : 1.0,
            }));
        }
    }
    onEditPerformanceFactorProperty(performancefactor: TreatmentPerformanceFactor, property: string, value: any) {
        this.$emit('onModifyPerformanceFactor', setItemPropertyValue(property, value, performancefactor));
    }

    onModifyPerformanceFactor() {
        // emit the attribute/factor object
        //this.$emit('onAddConsequence', newConsequence);
    }
}
</script>

<style>
.factor-tab-content {
    height: 185px;
}

.factor-data-table {
    height: 215px;
    overflow-y: auto;
    font-family: 'Montserrat', sans-serif;
}
</style>
