<template>
    <v-layout class='consequences-tab-content'>
        <v-flex xs12>
            <v-btn @click='onAddConsequence' class='ara-blue-bg white--text'>Add Consequence</v-btn>
            <div class='consequences-data-table'>
                <v-data-table :headers='consequencesGridHeaders' :items='consequencesGridData'
                              class='elevation-1 fixed-header v-table__overflow'
                              hide-actions>
                    <template slot='items' slot-scope='props'>
                        <td v-for='header in consequencesGridHeaders'>
                            <v-edit-dialog
                                v-if="header.value !== 'equation' && header.value !== 'criterionLibrary' && header.value !== ''"
                                :return-value.sync='props.item[header.value]'
                                @save='onEditConsequenceProperty(props.item, header.value, props.item[header.value])'
                                large lazy persistent>
                                <v-text-field v-if="header.value === 'attribute'" readonly single-line class='sm-txt'
                                              :value='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                <v-text-field v-if="header.value === 'changeValue'" readonly single-line class='sm-txt'
                                              :value='props.item.changeValue'
                                              :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                <template slot='input'>
                                    <v-select v-if="header.value === 'attribute'" :items='attributeSelectItems'
                                              label='Edit'
                                              v-model='props.item.attribute'
                                              :rules="[rules['generalRules'].valueIsNotEmpty]" />
                                    <v-text-field v-if="header.value === 'changeValue'" label='Edit'
                                                  v-model='props.item.changeValue'
                                                  :rules="[rules['treatmentRules'].hasChangeValueOrEquation(props.item.changeValue, props.item.equation.expression)]" />
                                </template>
                            </v-edit-dialog>

                            <v-textarea v-if="header.value === 'equation'" full-width no-resize outline readonly
                                        rows='3'
                                        v-model='props.item.equation.expression'>
                                <template slot='append-outer'>
                                    <v-btn @click='onShowConsequenceEquationEditorDialog(props.item)' class='edit-icon'
                                           icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>

                            <v-textarea v-if="header.value === 'criterionLibrary'" full-width no-resize outline readonly
                                        rows='3'
                                        v-model='props.item.criterionLibrary.mergedCriteriaExpression'>
                                <template slot='append-outer'>
                                    <v-btn @click='onShowConsequenceCriterionLibraryEditorDialog(props.item)'
                                           class='edit-icon' icon>
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </template>
                            </v-textarea>

                            <v-layout v-if="header.value === ''" align-start>
                                <v-btn @click='onRemoveConsequence(props.item.id)' class='ara-orange' icon>
                                    <v-icon>fas fa-trash</v-icon>
                                </v-btn>
                            </v-layout>
                        </td>
                    </template>
                </v-data-table>
            </div>
        </v-flex>

        <ConsequenceEquationEditorDialog :dialogData='consequenceEquationEditorDialogData'
                                         :isFromPerformanceCurveEditor=false
                                         @submit='onSubmitConsequenceEquationEditorDialogResult' />

        <ConsequenceCriterionLibraryEditorDialog :dialogData='consequenceCriterionLibraryEditorDialogData'
                                                 @submit='onSubmitConsequenceCriterionLibraryEditorDialogResult' />
    </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { State } from 'vuex-class';
import { clone, isNil } from 'ramda';
import EquationEditorDialog from '../../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../../shared/modals/CriterionLibraryEditorDialog.vue';
import { emptyConsequence, TreatmentConsequence } from '@/shared/models/iAM/treatment';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import {
    CriterionLibraryEditorDialogData,
    emptyCriterionLibraryEditorDialogData,
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
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
        ConsequenceCriterionLibraryEditorDialog: CriterionLibraryEditorDialog,
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
        { text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '200px' },
        { text: 'Change Value', value: 'changeValue', align: 'left', sortable: false, class: '', width: '125px' },
        { text: 'Equation', value: 'equation', align: 'left', sortable: false, class: '', width: '' },
        { text: 'Criteria', value: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '' },
        { text: '', value: '', align: 'left', sortable: false, class: '', width: '100px' },
    ];
    consequencesGridData: TreatmentConsequence[] = [];
    consequenceEquationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
    consequenceCriterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
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

    onShowConsequenceCriterionLibraryEditorDialog(consequence: TreatmentConsequence) {
        this.selectedConsequenceForEquationOrCriteriaEdit = clone(consequence);

        this.consequenceCriterionLibraryEditorDialogData = {
            showDialog: true,
            libraryId: consequence.criterionLibrary.id,
            isCallFromScenario: this.callFromScenario,
            isCriterionForLibrary: this.callFromLibrary,
        };
    }

    onSubmitConsequenceCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
        this.consequenceCriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

        if (!isNil(criterionLibrary) && this.selectedConsequenceForEquationOrCriteriaEdit.id !== this.uuidNIL) {
            this.$emit('onModifyConsequence', setItemPropertyValue('criterionLibrary', criterionLibrary, this.selectedConsequenceForEquationOrCriteriaEdit));
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
}
</style>
