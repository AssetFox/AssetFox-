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
                                @save='onEditConsequenceProperty(props.item, header.value, props.item[header.value])'
                                large lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='ghd-control-text-sm'
                                              :value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'factor'" readonly single-line class='ghd-control-text-sm'
                                              :value='props.item.factor'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                                <template slot='input'>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                             append-icon=$vuetify.icons.ghd-down
                                              label='Edit'
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'factor'" label='Edit'
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
import { emptyConsequence, TreatmentConsequence } from '@/shared/models/iAM/treatment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import { SelectItem } from '@/shared/models/vue/select-item';
import { Attribute } from '@/shared/models/iAM/attribute';
import { setItemPropertyValue } from '@/shared/utils/setter-utils';
import { InputValidationRules } from '@/shared/utils/input-validation-rules';
import { Equation } from '@/shared/models/iAM/equation';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { CriterionLibrary } from '@/shared/models/iAM/criteria';

@Component({
    components: {
    },
})
export default class PerformanceFactorTab extends Vue {
    @Prop() selectedTreatmentConsequences: TreatmentConsequence[];
    @Prop() rules: InputValidationRules;
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

    factorGridHeaders: DataTableHeader[] = [
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { text: 'Performance Factor', value: 'factor', align: 'left', sortable: false, class: '', width: '125px' },
    ];
    factorGridData: TreatmentConsequence[] = [];
    attributeSelectItems: SelectItem[] = [];
    uuidNIL: string = getBlankGuid();

    mounted() {
        console.log("" + this.selectedTreatmentConsequences);
        this.setAttributeSelectItems();
    }

    @Watch('selectedTreatmentConsequences')
    onSelectedTreatmentConsequencesChanged() {
        console.log("inside");        
        this.factorGridData = clone(this.selectedTreatmentConsequences);
    }

    @Watch('stateAttributes')
    onStateAttributesChanged() {
        this.setAttributeSelectItems();
    }

    setAttributeSelectItems() {
        if (hasValue(this.stateAttributes)) {
            console.log("state attributes: " + this.stateAttributes.length);
            this.attributeSelectItems = this.stateAttributes.map((attribute: Attribute) => ({
                text: attribute.name,
                value: attribute.name,
            }));
        }
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
