<template>
    <v-layout class='consequences-tab-content'>
        <v-flex xs12>            
            <div class='consequences-data-table'>
                <v-data-table :headers='consequencesGridHeaders' :items='consequencesGridData'
                              id="ConsequencesTab-Consequences-vDataTable"
                              class='elevation-1 fixed-header v-table__overflow'
                              sort-icon=$vuetify.icons.ghd-table-sort
                              hide-actions>
                    <template slot='items' slot-scope='props'>
                        <td v-for='header in consequencesGridHeaders'>
                            <v-edit-dialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditConsequenceProperty(props.item, header.value, props.item[header.value])'
                                large lazy persistent>
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
                                left
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'equation'"
                            >
                                <template slot="activator">
                                    <v-btn 
                                        id="TreatmentConsequencesTab-EquationView-btn"
                                        class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            class="sm-txt"
                                            id="TreatmentConsequencesTab-EquationView-textarea"
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
                             <v-btn id="TreatmentConsequencesTab-EquationEditorBtn" v-if="header.value === 'equation'" @click='onShowConsequenceEquationEditorDialog(props.item)' class='edit-icon'
                                    icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>                       

                            <v-menu
                                left
                                min-height="500px"
                                min-width="500px"
                                v-show="header.value === 'criterionLibrary'"
                            >
                                <template slot="activator">
                                    <v-btn id="TreatmentConsequencesTab-CriteriaView-btn" class="ghd-blue" icon>
                                        <img class='img-general' :src="require('@/assets/icons/eye-ghd-blue.svg')"/>
                                    </v-btn>
                                </template>
                                <v-card>
                                    <v-card-text>
                                        <v-textarea
                                            id="TreatmentConsequencesTab-CriteriaView-textarea"
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
                            <v-btn id="TreatmentConsequencesTab-CriteriaEditorBtn" v-if="header.value === 'criterionLibrary'" @click='onShowConsequenceCriterionEditorDialog(props.item)'
                                    class='edit-icon' icon>
                                <img class='img-general' :src="require('@/assets/icons/edit.svg')"/>
                            </v-btn>

                            <v-layout v-if="header.value === ''" align-start>
                                <v-btn id="TreatmentConquencesTab-DeleteCostBtn" @click='onRemoveConsequence(props.item.id)' icon>
                                    <img class='img-general' :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
            <v-btn id="TreatmentConsequencesTab-AddConsequenceBtn" @click='onAddConsequence' class='ghd-white-bg ghd-blue ghd-button-text-sm ghd-blue-border ghd-text-padding'>Add Consequence</v-btn>
        </v-flex>

        <ConsequenceEquationEditorDialog :dialogData='consequenceEquationEditorDialogData'
                                         :isFromPerformanceCurveEditor=false
                                         @submit='onSubmitConsequenceEquationEditorDialogResult' />

        <GeneralCriterionEditorDialog :dialogData='consequenceCriterionEditorDialogData'
                                                 @submit='onSubmitConsequenceCriterionEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { State } from 'vuex-class';
import { clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
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

@Component({
    components: {
        GeneralCriterionEditorDialog,
        ConsequenceEquationEditorDialog: EquationEditorDialog,
    },
})
export default class ConsequencesTab extends Vue {
    @Prop() selectedTreatmentConsequences: TreatmentConsequence[];
    @Prop() rules: InputValidationRules;
    @Prop() callFromScenario: boolean;
    @Prop() callFromLibrary: boolean;

    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];

    consequencesGridHeaders: DataTableHeader[] = [
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '175px' },
        { text: 'Change Value', value: 'changeValue', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Criteria', value: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Actions', value: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    consequencesGridData: TreatmentConsequence[] = [];
    consequenceEquationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
    consequenceCriterionEditorDialogData: GeneralCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);
    selectedConsequenceForEquationOrCriteriaEdit: TreatmentConsequence = clone(emptyConsequence);
    attributeSelectItems: SelectItem[] = [];
    uuidNIL: string = getBlankGuid();

    mounted() {
        this.setAttributeSelectItems();
    }

    @Watch('selectedTreatmentConsequences')
    onSelectedTreatmentConsequencesChanged() {
        this.consequencesGridData = clone(this.selectedTreatmentConsequences);
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
        }
    }

    onAddConsequence() {
        const newConsequence: TreatmentConsequence = { ...emptyConsequence, id: getNewGuid() };
        this.$emit('onAddConsequence', newConsequence);
    }

    onEditConsequenceProperty(consequence: TreatmentConsequence, property: string, value: any) {
        this.$emit('onModifyConsequence', setItemPropertyValue(property, value, consequence));
    }

    onShowConsequenceEquationEditorDialog(consequence: TreatmentConsequence) {
        this.selectedConsequenceForEquationOrCriteriaEdit = clone(consequence);

        this.consequenceEquationEditorDialogData = {
            showDialog: true,
            equation: clone(consequence.equation),
        };
    }

    onSubmitConsequenceEquationEditorDialogResult(equation: Equation) {
        this.consequenceEquationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && this.selectedConsequenceForEquationOrCriteriaEdit.id !== this.uuidNIL) {
            this.$emit('onModifyConsequence', setItemPropertyValue('equation', equation, this.selectedConsequenceForEquationOrCriteriaEdit));
        }

        this.selectedConsequenceForEquationOrCriteriaEdit = clone(emptyConsequence);
    }

    onShowConsequenceCriterionEditorDialog(consequence: TreatmentConsequence) {
        this.selectedConsequenceForEquationOrCriteriaEdit = clone(consequence);

        this.consequenceCriterionEditorDialogData = {
            showDialog: true,
            CriteriaExpression: consequence.criterionLibrary.mergedCriteriaExpression,
        };
    }

    onSubmitConsequenceCriterionEditorDialogResult(criterionExpression: string) {
        this.consequenceCriterionEditorDialogData = clone(emptyGeneralCriterionEditorDialogData);

        if (!isNil(criterionExpression) && this.selectedConsequenceForEquationOrCriteriaEdit.id !== this.uuidNIL) {
            if(this.selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary.id === getBlankGuid())
                this.selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary.id = getNewGuid();
            this.$emit('onModifyConsequence', setItemPropertyValue('criterionLibrary', 
            {...this.selectedConsequenceForEquationOrCriteriaEdit.criterionLibrary, mergedCriteriaExpression: criterionExpression} as CriterionLibrary, 
            this.selectedConsequenceForEquationOrCriteriaEdit));
        }

        this.selectedConsequenceForEquationOrCriteriaEdit = clone(emptyConsequence);
    }

    onRemoveConsequence(consequenceId: string) {
        this.$emit('onRemoveConsequence', consequenceId);
    }
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
