<template>
  <v-layout column>
    <v-flex xs12>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onShowCreateDeficientConditionGoalLibraryDialog(false)" class="ara-blue-bg white--text"
                 v-show="selectedScenarioId === uuidNIL">
            New Library
          </v-btn>
          <v-select :items="librarySelectItems"
                    label="Select a DeficientConditionGoal Library"
                    outline v-if="!hasSelectedLibrary || selectedScenarioId !== uuidNIL"
                    v-model="selectItemValue">
          </v-select>
          <v-text-field label="Library Name" v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                        v-model="selectedDeficientConditionGoalLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]">
            <template slot="append">
              <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL">
            Owner: {{
              selectedDeficientConditionGoalLibrary.owner ? selectedDeficientConditionGoalLibrary.owner : "[ No Owner ]"
            }}
          </div>
          <v-checkbox class="sharing" label="Shared"
                      v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                      v-model="selectedDeficientConditionGoalLibrary.shared"/>
        </v-flex>
      </v-layout>
      <v-flex v-show="hasSelectedLibrary" xs3>
        <v-btn @click="showCreateDeficientConditionGoalDialog = true" class="ara-blue-bg white--text">Add</v-btn>
        <v-btn :disabled="selectedDeficientConditionGoalIds.length === 0"
               @click="onRemoveSelectedDeficientConditionGoals"
               class="ara-orange-bg white--text">
          Delete
        </v-btn>
      </v-flex>
    </v-flex>
    <v-flex xs12 v-show="hasSelectedLibrary">
      <div class="deficients-data-table">
        <v-data-table :headers="deficientConditionGoalGridHeaders" :items="deficientConditionGoalGridData"
                      class="elevation-1 fixed-header v-table__overflow"
                      item-key="id" select-all v-model="selectedGridRows">
          <template slot="items" slot-scope="props">
            <td>
              <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
            </td>
            <td v-for="header in deficientConditionGoalGridHeaders">
              <div>
                <v-edit-dialog v-if="header.value !== 'criterionLibrary'"
                    :return-value.sync="props.item[header.value]"
                    @save="onEditDeficientConditionGoalProperty(props.item, header.value, props.item[header.value])"
                    large lazy persistent>
                  <v-text-field v-if="header.value !== 'allowedDeficientPercentage'" readonly class="sm-txt" :value="props.item[header.value]"
                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>

                  <v-text-field v-if="header.value === 'allowedDeficientPercentage'" readonly class="sm-txt" :value="props.item[header.value]"
                                :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [0, 100])]"/>

                  <template v-if="header.value === 'name'" slot="input">
                    <v-text-field label="Edit" single-line v-model="props.item[header.value]"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                  </template>

                  <template v-if="header.value === 'attribute'" slot="input">
                    <v-select :items="numericAttributeNames" label="Select an Attribute"
                              v-model="props.item[header.value]"
                              :rules="[rules['generalRules'].valueIsNotEmpty]">
                    </v-select>
                  </template>

                  <template v-if="header.value === 'deficientLimit'" slot="input">
                    <v-text-field label="Edit" single-line v-model="props.item[header.value]"
                                  :mask="'##########'"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                  </template>

                  <template v-if="header.value === 'allowedDeficientPercentage'" slot="input">
                    <v-text-field label="Edit" single-line v-model.number="props.item[header.value]"
                                  :mask="'###'"
                                  :rules="[rules['generalRules'].valueIsNotEmpty, rules['generalRules'].valueIsWithinRange(props.item[header.value], [0, 100])]"/>
                  </template>
                </v-edit-dialog>

                <v-layout v-else align-center row style="flex-wrap:nowrap">
                  <v-menu bottom min-height="500px" min-width="500px">
                    <template slot="activator">
                      <v-text-field readonly class="sm-txt"
                                    :value="props.item.criterionLibrary.mergedCriteriaExpression"/>
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea :value="props.item.criterionLibrary.mergedCriteriaExpression" full-width no-resize
                                    outline
                                    readonly
                                    rows="5"/>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <v-btn @click="onShowCriterionLibraryEditorDialog(props.item)" class="edit-icon" icon>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-layout>
              </div>
            </td>
          </template>
        </v-data-table>
      </div>
    </v-flex>
    <v-flex v-show="hasSelectedLibrary && selectedScenarioId === uuidNIL"
            xs12>
      <v-layout justify-center>
        <v-flex xs6>
          <v-textarea label="Description" no-resize outline rows="4"
                      v-model="selectedDeficientConditionGoalLibrary.description">
          </v-textarea>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-flex v-show="hasSelectedLibrary" xs12>
      <v-layout justify-end row>
        <v-btn
            @click="onAddOrUpdateDeficientConditionGoalLibrary(selectedDeficientConditionGoalLibrary, selectedScenarioId)"
            class="ara-blue-bg white--text"
            v-show="selectedScenarioId !== uuidNIL" :disabled="disableCrudButton()">
          Save
        </v-btn>
        <v-btn @click="onAddOrUpdateDeficientConditionGoalLibrary(selectedDeficientConditionGoalLibrary, uuidNIL)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="disableCrudButton()">
          Update Library
        </v-btn>
        <v-btn @click="onShowCreateDeficientConditionGoalLibraryDialog" class="ara-blue-bg white--text"
               :disabled="disableCrudButton()">
          Create as New Library
        </v-btn>
        <v-btn @click="onShowConfirmDeleteAlert" class="ara-orange-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="!hasSelectedLibrary">
          Delete Library
        </v-btn>
        <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text"
               v-show="selectedScenarioId !== uuidNIL" :disabled="!hasSelectedLibrary">
          Discard Changes
        </v-btn>
      </v-layout>
    </v-flex>

    <ConfirmBeforeDeleteAlert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>

    <CreateDeficientConditionGoalLibraryDialog :dialogData="createDeficientConditionGoalLibraryDialogData"
                                               @submit="onAddOrUpdateDeficientConditionGoalLibrary"/>

    <CreateDeficientConditionGoalDialog :showDialog="showCreateDeficientConditionGoalDialog"
                                        :currentNumberOfDeficientConditionGoals="selectedDeficientConditionGoalLibrary.deficientConditionGoals.length"
                                        @submit="onAddDeficientConditionGoal"/>

    <CriterionLibraryEditorDialog :dialogData="criterionLibraryEditorDialogData"
                                  @submit="onEditDeficientConditionGoalCriterionLibrary"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Watch} from 'vue-property-decorator';
import {Action, Getter, State} from 'vuex-class';
import {
  DeficientConditionGoal,
  DeficientConditionGoalLibrary,
  emptyDeficientConditionGoal,
  emptyDeficientConditionGoalLibrary
} from '@/shared/models/iAM/deficient-condition-goal';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {clone, contains, findIndex, isNil, prepend, propEq, update} from 'ramda';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import CreateDeficientConditionGoalDialog
  from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalDialog.vue';
import {
  CreateDeficientConditionGoalLibraryDialogData,
  emptyCreateDeficientConditionGoalLibraryDialogData
} from '@/shared/models/modals/create-deficient-condition-goal-library-dialog-data';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {SelectItem} from '@/shared/models/vue/select-item';
import CreateDeficientConditionGoalLibraryDialog
  from '@/components/deficient-condition-goal-editor/deficient-condition-goal-editor-dialogs/CreateDeficientConditionGoalLibraryDialog.vue';
import {Attribute} from '@/shared/models/iAM/attribute';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibraryId, hasAppliedLibrary} from '@/shared/utils/library-utils';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';

@Component({
  components: {
    CreateDeficientConditionGoalLibraryDialog,
    CreateDeficientConditionGoalDialog,
    CriterionLibraryEditorDialog,
    ConfirmBeforeDeleteAlert: Alert
  }
})
export default class DeficientConditionGoalEditor extends Vue {
  @State(state => state.deficientEditor.deficientConditionGoalLibraries) stateDeficientConditionGoalLibraries: DeficientConditionGoalLibrary[];
  @State(state => state.deficientEditor.selectedDeficientConditionGoalLibrary) stateSelectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary;
  @State(state => state.attribute.numericAttributeNames) stateNumericAttributes: Attribute[];

  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('getDeficientConditionGoalLibraries') getDeficientConditionGoalLibrariesAction: any;
  @Action('selectDeficientConditionGoalLibrary') selectDeficientConditionGoalLibraryAction: any;
  @Action('addOrUpdateDeficientConditionGoalLibrary') addOrUpdateDeficientConditionGoalLibraryAction: any;
  @Action('deleteDeficientConditionGoalLibrary') deleteDeficientConditionGoalLibraryAction: any;
  @Action('getAttributes') getAttributesAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  @Getter('getNumericAttributes') getNumericAttributesGetter: any;

  selectedScenarioId: string = getBlankGuid();
  librarySelectItems: SelectItem[] = [];
  selectItemValue: string | null = null;
  selectedDeficientConditionGoalLibrary: DeficientConditionGoalLibrary = clone(emptyDeficientConditionGoalLibrary);
  hasSelectedLibrary: boolean = false;
  deficientConditionGoalGridHeaders: DataTableHeader[] = [
    {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: '15%'},
    {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: '12%'},
    {text: 'Deficient Limit', value: 'deficientLimit', align: 'left', sortable: false, class: '', width: '8%'},
    {
      text: 'Allowed Deficient Percentage',
      value: 'allowedDeficientPercentage',
      align: 'left',
      sortable: false,
      class: '',
      width: '11%'
    },
    {text: 'Criteria', value: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '50%'}
  ];
  deficientConditionGoalGridData: DeficientConditionGoal[] = [];
  numericAttributeNames: string[] = [];
  selectedGridRows: DeficientConditionGoal[] = [];
  selectedDeficientConditionGoalIds: string[] = [];
  selectedDeficientConditionGoalForCriteriaEdit: DeficientConditionGoal = clone(emptyDeficientConditionGoal);
  showCreateDeficientConditionGoalDialog: boolean = false;
  criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  createDeficientConditionGoalLibraryDialogData: CreateDeficientConditionGoalLibraryDialogData = clone(emptyCreateDeficientConditionGoalLibraryDialogData);
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  rules: InputValidationRules = rules;
  uuidNIL: string = getBlankGuid();

  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('DeficientConditionGoalEditor/Scenario') !== -1) {
        vm.selectedScenarioId = to.query.scenarioId;
        if (vm.selectedScenarioId === vm.uuidNIL) {
          vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
          vm.$router.push('/Scenarios/');
        }
      }

      vm.selectItemValue = null;
      vm.getDeficientConditionGoalLibrariesAction();
    });
  }

  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  @Watch('stateDeficientConditionGoalLibraries')
  onStateDeficientConditionGoalLibrariesChanged() {
    this.librarySelectItems = this.stateDeficientConditionGoalLibraries.map((library: DeficientConditionGoalLibrary) => ({
      text: library.name,
      value: library.id
    }));

    if (this.selectedScenarioId !== this.uuidNIL && hasAppliedLibrary(this.stateDeficientConditionGoalLibraries, this.selectedScenarioId)) {
      this.selectItemValue = getAppliedLibraryId(this.stateDeficientConditionGoalLibraries, this.selectedScenarioId);
    }
  }

  @Watch('selectItemValue')
  onSelectItemValueChanged() {
    this.selectDeficientConditionGoalLibraryAction({libraryId: this.selectItemValue});
  }

  @Watch('stateSelectedDeficientConditionGoalLibrary')
  onStateSelectedDeficientConditionGoalLibraryChanged() {
    this.selectedDeficientConditionGoalLibrary = clone(this.stateSelectedDeficientConditionGoalLibrary);
  }

  @Watch('selectedDeficientConditionGoalLibrary')
  onSelectedDeficientConditionGoalLibraryChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChangesCore(
          'deficient', this.selectedDeficientConditionGoalLibrary, this.stateSelectedDeficientConditionGoalLibrary)
    });
    this.hasSelectedLibrary = this.selectedDeficientConditionGoalLibrary.id !== this.uuidNIL;
    this.deficientConditionGoalGridData = clone(this.selectedDeficientConditionGoalLibrary.deficientConditionGoals);
    if (this.numericAttributeNames.length === 0) {
      this.numericAttributeNames = getPropertyValues('name', this.getNumericAttributesGetter);
    }
  }

  @Watch('selectedGridRows')
  onSelectedDeficientRowsChanged() {
    this.selectedDeficientConditionGoalIds = getPropertyValues('id', this.selectedGridRows) as string[];
  }

  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
  }

  onShowCreateDeficientConditionGoalLibraryDialog(createExistingLibraryAsNew: boolean) {
    this.createDeficientConditionGoalLibraryDialogData = {
      showDialog: true,
      deficientConditionGoals: createExistingLibraryAsNew
          ? this.selectedDeficientConditionGoalLibrary.deficientConditionGoals
          : [],
      scenarioId: createExistingLibraryAsNew ? this.selectedScenarioId : this.uuidNIL
    };
  }

  onAddDeficientConditionGoal(newDeficientConditionGoal: DeficientConditionGoal) {
    this.showCreateDeficientConditionGoalDialog = false;

    if (!isNil(newDeficientConditionGoal)) {
      this.selectedDeficientConditionGoalLibrary = {
        ...this.selectedDeficientConditionGoalLibrary,
        deficientConditionGoals: prepend(newDeficientConditionGoal, this.selectedDeficientConditionGoalLibrary.deficientConditionGoals)
      };
    }
  }

  onEditDeficientConditionGoalProperty(deficientConditionGoal: DeficientConditionGoal, property: string, value: any) {
    this.selectedDeficientConditionGoalLibrary = {
      ...this.selectedDeficientConditionGoalLibrary,
      deficientConditionGoals: update(
          findIndex(propEq('id', deficientConditionGoal.id), this.selectedDeficientConditionGoalLibrary.deficientConditionGoals),
          setItemPropertyValue(property, value, deficientConditionGoal) as DeficientConditionGoal,
          this.selectedDeficientConditionGoalLibrary.deficientConditionGoals
      )
    };
  }

  onShowCriterionLibraryEditorDialog(deficientConditionGoal: DeficientConditionGoal) {
    this.selectedDeficientConditionGoalForCriteriaEdit = clone(deficientConditionGoal);

    this.criterionLibraryEditorDialogData = {
      showDialog: true,
      libraryId: deficientConditionGoal.criterionLibrary.id
    };
  }

  onEditDeficientConditionGoalCriterionLibrary(criterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary)) {
      this.selectedDeficientConditionGoalLibrary = {
        ...this.selectedDeficientConditionGoalLibrary,
        deficientConditionGoals: update(
            findIndex(propEq('id', this.selectedDeficientConditionGoalForCriteriaEdit.id), this.selectedDeficientConditionGoalLibrary.deficientConditionGoals),
            {...this.selectedDeficientConditionGoalForCriteriaEdit, criterionLibrary: criterionLibrary},
            this.selectedDeficientConditionGoalLibrary.deficientConditionGoals
        )
      };
    }

    this.selectedDeficientConditionGoalForCriteriaEdit = clone(emptyDeficientConditionGoal);
  }

  onAddOrUpdateDeficientConditionGoalLibrary(library: DeficientConditionGoalLibrary, scenarioId: string) {
    this.createDeficientConditionGoalLibraryDialogData = clone(emptyCreateDeficientConditionGoalLibraryDialogData);

    if (!isNil(library)) {
      this.addOrUpdateDeficientConditionGoalLibraryAction({library: library, scenarioId: scenarioId});
    }
  }

  onDiscardChanges() {
    this.selectItemValue = null;
    setTimeout(() => {
      if (this.selectedScenarioId !== this.uuidNIL &&
          hasAppliedLibrary(this.stateDeficientConditionGoalLibraries, this.selectedScenarioId)) {
        this.selectItemValue = getAppliedLibraryId(this.stateDeficientConditionGoalLibraries, this.selectedScenarioId);
      }
    });
  }

  onRemoveSelectedDeficientConditionGoals() {
    this.selectedDeficientConditionGoalLibrary = {
      ...this.selectedDeficientConditionGoalLibrary,
      deficientConditionGoals: this.selectedDeficientConditionGoalLibrary.deficientConditionGoals
          .filter((deficientConditionGoal: DeficientConditionGoal) => !contains(deficientConditionGoal.id, this.selectedDeficientConditionGoalIds))
    };
  }

  onShowConfirmDeleteAlert() {
    this.confirmDeleteAlertData = {
      showDialog: true,
      heading: 'Warning',
      choice: true,
      message: 'Are you sure you want to delete?'
    };
  }

  onSubmitConfirmDeleteAlertResult(submit: boolean) {
    this.confirmDeleteAlertData = clone(emptyAlertData);

    if (submit) {
      this.selectItemValue = null;
      this.deleteDeficientConditionGoalLibraryAction({libraryId: this.selectedDeficientConditionGoalLibrary.id});
    }
  }

  disableCrudButton() {
    if (this.hasSelectedLibrary) {
      const allDataIsValid: boolean = this.selectedDeficientConditionGoalLibrary.deficientConditionGoals.every((d: DeficientConditionGoal) => {
        return this.rules['generalRules'].valueIsNotEmpty(d.attribute) === true &&
            this.rules['generalRules'].valueIsNotEmpty(d.name) === true &&
            this.rules['generalRules'].valueIsNotEmpty(d.deficientLimit) === true &&
            this.rules['generalRules'].valueIsNotEmpty(d.allowedDeficientPercentage) === true &&
            this.rules['generalRules'].valueIsWithinRange(d.allowedDeficientPercentage, [0, 100]) === true;
      });

      return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedDeficientConditionGoalLibrary.name) === true &&
          allDataIsValid);
    }

    return true;
  }
}
</script>

<style>
.deficients-data-table {
  height: 425px;
  overflow-y: auto;
  overflow-x: hidden;
}

.deficients-data-table .v-menu--inline, .deficient-criteria-output {
  width: 100%;
}

.sharing label {
  padding-top: 0.5em;
}

.sharing {
  padding-top: 0;
  margin: 0;
}
</style>
