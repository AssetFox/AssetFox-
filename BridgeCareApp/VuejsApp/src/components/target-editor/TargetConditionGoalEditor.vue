<template>
  <v-layout column>
    <v-flex xs12>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onShowCreateTargetConditionGoalLibraryDialog(false)" class="ara-blue-bg white--text"
                 v-show="selectedScenarioId === uuidNIL">
            New Library
          </v-btn>
          <v-select :items="librarySelectItems"
                    label="Select a Target Condition Goal Library"
                    outline v-if="!hasSelectedLibrary || selectedScenarioId !== uuidNIL"
                    v-model="librarySelectItemValue">
          </v-select>
          <v-text-field label="Library Name" v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                        v-model="selectedTargetConditionGoalLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]">
            <template slot="append">
              <v-btn @click="librarySelectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL">
            Owner:
            {{ selectedTargetConditionGoalLibrary.owner ? selectedTargetConditionGoalLibrary.owner : "[ No Owner ]" }}
          </div>
          <v-checkbox class="sharing" label="Shared"
                      v-if="hasSelectedLibrary && selectedScenarioId === uuidNIL"
                      v-model="selectedTargetConditionGoalLibrary.shared"/>
        </v-flex>
      </v-layout>
      <v-flex v-show="hasSelectedLibrary" xs3>
        <v-btn @click="showCreateTargetConditionGoalDialog = true" class="ara-blue-bg white--text">Add</v-btn>
        <v-btn :disabled="selectedTargetConditionGoalIds.length === 0" @click="onRemoveTargetConditionGoals"
               class="ara-orange-bg white--text">
          Delete
        </v-btn>
      </v-flex>
    </v-flex>
    <v-flex v-show="hasSelectedLibrary" xs12>
      <div class="targets-data-table">
        <v-data-table :headers="targetConditionGoalGridHeaders" :items="targetConditionGoalGridData"
                      class="elevation-1 fixed-header v-table__overflow" item-key="id"
                      select-all v-model="selectedGridRows">
          <template slot="items" slot-scope="props">
            <td>
              <v-checkbox hide-details primary v-model="props.selected"></v-checkbox>
            </td>
            <td v-for="header in targetConditionGoalGridHeaders">
              <div>
                <v-edit-dialog v-if="header.value !== 'criterionLibrary'"
                               :return-value.sync="props.item[header.value]"
                               @save="onEditTargetConditionGoalProperty(props.item, header.value, props.item[header.value])"
                               large lazy persistent>
                  <v-text-field v-if="header.value === 'year'" readonly single-line class="sm-txt"
                                :value="props.item[header.value]"/>
                  <v-text-field v-else readonly single-line class="sm-txt" :value="props.item[header.value]"
                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                  <template slot="input">
                    <v-select v-if="header.value === 'attribute'" :items="numericAttributeNames"
                              label="Select an Attribute"
                              v-model="props.item.attribute"
                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <v-text-field v-if="header.value === 'year'" label="Edit" single-line
                                  :mask="'####'" v-model.number="props.item[header.value]"/>
                    <v-text-field v-if="header.value === 'target'" label="Edit" single-line
                                  :mask="'##########'" v-model.number="props.item[header.value]"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <v-text-field v-if="header.value === 'name'" label="Edit" single-line
                                  v-model="props.item[header.value]"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
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
    <v-flex v-show="hasSelectedLibrary && selectedScenarioId === uuidNIL" xs12>
      <v-layout justify-center>
        <v-flex xs6>
          <v-textarea label="Description" no-resize outline rows="4"
                      v-model="selectedTargetConditionGoalLibrary.description">
          </v-textarea>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-flex v-show="hasSelectedLibrary" xs12>
      <v-layout justify-end row>
        <v-btn @click="onUpsertTargetConditionGoalLibrary(selectedTargetConditionGoalLibrary, selectedScenarioId)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId !== uuidNIL" :disabled="disableCrudButton()">
          Save
        </v-btn>
        <v-btn @click="onUpsertTargetConditionGoalLibrary(selectedTargetConditionGoalLibrary, uuidNIL)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="disableCrudButton()">
          Update Library
        </v-btn>
        <v-btn @click="onShowCreateTargetConditionGoalLibraryDialog(true)" class="ara-blue-bg white--text"
               :disabled="disableCrudButton()">
          Create as New Library
        </v-btn>
        <v-btn @click="onShowConfirmDeleteAlert" class="ara-orange-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="!hasSelectedLibrary">
          Delete Library
        </v-btn>
        <v-btn @click="onDiscardChanges" class="ara-orange-bg white--text"
               v-show="selectedScenarioId !== uuidNIL">
          Discard Changes
        </v-btn>
      </v-layout>
    </v-flex>

    <ConfirmDeleteAlert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>

    <CreateTargetConditionGoalLibraryDialog :dialogData="createTargetConditionGoalLibraryDialogData"
                                            @submit="onUpsertTargetConditionGoalLibrary"/>

    <CreateTargetConditionGoalDialog :showDialog="showCreateTargetConditionGoalDialog"
                                     :currentNumberOfTargetConditionGoals="selectedTargetConditionGoalLibrary.targetConditionGoals.length"
                                     @submit="onAddTargetConditionGoal"/>

    <CriterionLibraryEditorDialog :dialogData="criterionLibraryEditorDialogData"
                                  @submit="onEditTargetConditionGoalCriterionLibrary"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Watch} from 'vue-property-decorator';
import {Action, Getter, State} from 'vuex-class';
import {
  emptyTargetConditionGoal,
  emptyTargetConditionGoalLibrary,
  TargetConditionGoal,
  TargetConditionGoalLibrary
} from '@/shared/models/iAM/target-condition-goal';
import {clone, contains, findIndex, isNil, prepend, propEq, update} from 'ramda';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import CreateTargetConditionGoalDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalDialog.vue';
import {getPropertyValues} from '@/shared/utils/getter-utils';
import {SelectItem} from '@/shared/models/vue/select-item';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';
import {
  CreateTargetConditionGoalLibraryDialogData,
  emptyCreateTargetConditionGoalLibraryDialogData
} from '@/shared/models/modals/create-target-condition-goal-library-dialog-data';
import CreateTargetConditionGoalLibraryDialog from '@/components/target-editor/target-editor-dialogs/CreateTargetConditionGoalLibraryDialog.vue';
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
    CriterionLibraryEditorDialog,
    CreateTargetConditionGoalLibraryDialog,
    CreateTargetConditionGoalDialog,
    ConfirmDeleteAlert: Alert
  }
})
export default class TargetConditionGoalEditor extends Vue {
  @State(state => state.targetConditionGoalModule.targetConditionGoalLibraries) stateTargetConditionGoalLibraries: TargetConditionGoalLibrary[];
  @State(state => state.targetConditionGoalModule.selectedTargetConditionGoalLibrary) stateSelectedTargetConditionLibrary: TargetConditionGoalLibrary;
  @State(state => state.attributeModule.numericAttributeNames) stateNumericAttributes: Attribute[];

  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('getTargetConditionGoalLibraries') getTargetConditionGoalLibrariesAction: any;
  @Action('selectTargetConditionGoalLibrary') selectTargetConditionGoalLibraryAction: any;
  @Action('upsertTargetConditionGoalLibrary') createTargetConditionGoalLibraryAction: any;
  @Action('deleteTargetConditionGoalLibrary') deleteTargetConditionGoalLibraryAction: any;
  @Action('getAttributes') getAttributesAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  @Getter('getNumericAttributes') getNumericAttributesGetter: any;

  selectedScenarioId: string = getBlankGuid();
  librarySelectItems: SelectItem[] = [];
  librarySelectItemValue: string | null = '';
  selectedTargetConditionGoalLibrary: TargetConditionGoalLibrary = clone(emptyTargetConditionGoalLibrary);
  hasSelectedLibrary: boolean = false;
  targetConditionGoalGridData: TargetConditionGoal[] = [];
  targetConditionGoalGridHeaders: DataTableHeader[] = [
    {text: 'Name', value: 'name', align: 'left', sortable: false, class: '', width: ''},
    {text: 'Attribute', value: 'attribute', align: 'left', sortable: false, class: '', width: ''},
    {text: 'Target', value: 'target', align: 'left', sortable: false, class: '', width: ''},
    {text: 'Year', value: 'year', align: 'left', sortable: false, class: '', width: ''},
    {text: 'Criteria', value: 'criterionLibrary', align: 'left', sortable: false, class: '', width: '50%'}
  ];
  numericAttributeNames: string[] = [];
  selectedGridRows: TargetConditionGoal[] = [];
  selectedTargetConditionGoalIds: string[] = [];
  selectedTargetConditionGoalForCriteriaEdit: TargetConditionGoal = clone(emptyTargetConditionGoal);
  showCreateTargetConditionGoalDialog: boolean = false;
  criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  createTargetConditionGoalLibraryDialogData: CreateTargetConditionGoalLibraryDialogData = clone(emptyCreateTargetConditionGoalLibraryDialogData);
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  rules: InputValidationRules = rules;
  uuidNIL: string = getBlankGuid();

  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('TargetConditionGoalEditor/Scenario') !== -1) {
        vm.selectedScenarioId = to.query.scenarioId;
        if (vm.selectedScenarioId === vm.uuidNIL) {
          vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
          vm.$router.push('/Scenarios/');
        }
      }

      vm.librarySelectItemValue = null;
      vm.getTargetConditionGoalLibrariesAction();
    });
  }

  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  @Watch('stateTargetConditionGoalLibraries')
  onStateTargetConditionGoalLibrariesChanged() {
    this.librarySelectItems = this.stateTargetConditionGoalLibraries.map((library: TargetConditionGoalLibrary) => ({
      text: library.name,
      value: library.id
    }));

    if (this.selectedScenarioId !== this.uuidNIL && hasAppliedLibrary(this.stateTargetConditionGoalLibraries, this.selectedScenarioId)) {
      this.librarySelectItemValue = getAppliedLibraryId(this.stateTargetConditionGoalLibraries, this.selectedScenarioId);
    }
  }

  @Watch('librarySelectItemValue')
  onLibrarySelectItemValueChanged() {
    this.selectTargetConditionGoalLibraryAction({libraryId: this.librarySelectItemValue});
  }

  @Watch('stateSelectedTargetConditionLibrary')
  onStateSelectedTargetConditionGoalLibraryChanged() {
    this.selectedTargetConditionGoalLibrary = clone(this.stateSelectedTargetConditionLibrary);
  }

  @Watch('selectedTargetConditionGoalLibrary')
  onSelectedTargetConditionGoalLibraryChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChangesCore(
          'target-condition-goal', this.selectedTargetConditionGoalLibrary, this.stateSelectedTargetConditionLibrary
      )
    });
    this.hasSelectedLibrary = this.selectedTargetConditionGoalLibrary.id !== this.uuidNIL;
    this.targetConditionGoalGridData = clone(this.selectedTargetConditionGoalLibrary.targetConditionGoals);
    if (this.numericAttributeNames.length === 0) {
      this.numericAttributeNames = getPropertyValues('name', this.getNumericAttributesGetter);
    }
  }

  @Watch('selectedGridRows')
  onSelectedGridRowsChanged() {
    this.selectedTargetConditionGoalIds = getPropertyValues('id', this.selectedGridRows) as string[];
  }

  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    this.numericAttributeNames = getPropertyValues('name', this.stateNumericAttributes);
  }

  onShowCreateTargetConditionGoalLibraryDialog(createAsNewLibrary: boolean) {
    this.createTargetConditionGoalLibraryDialogData = {
      showDialog: true,
      targetConditionGoals: createAsNewLibrary ? this.selectedTargetConditionGoalLibrary.targetConditionGoals : [],
      scenarioId: createAsNewLibrary ? this.selectedScenarioId : this.uuidNIL
    };
  }

  onAddTargetConditionGoal(newTargetConditionGoal: TargetConditionGoal) {
    this.showCreateTargetConditionGoalDialog = false;

    if (!isNil(newTargetConditionGoal)) {
      this.selectedTargetConditionGoalLibrary = {
        ...this.selectedTargetConditionGoalLibrary,
        targetConditionGoals: prepend(newTargetConditionGoal, this.selectedTargetConditionGoalLibrary.targetConditionGoals)
      };
    }
  }

  onEditTargetConditionGoalProperty(targetConditionGoal: TargetConditionGoal, property: string, value: any) {
    this.selectedTargetConditionGoalLibrary = {
      ...this.selectedTargetConditionGoalLibrary,
      targetConditionGoals: update(
          findIndex(propEq('id', targetConditionGoal.id), this.selectedTargetConditionGoalLibrary.targetConditionGoals),
          setItemPropertyValue(property, value, targetConditionGoal) as TargetConditionGoal,
          this.selectedTargetConditionGoalLibrary.targetConditionGoals
      )
    };
  }

  onShowCriterionLibraryEditorDialog(targetConditionGoal: TargetConditionGoal) {
    this.selectedTargetConditionGoalForCriteriaEdit = clone(targetConditionGoal);

    var fromScenario = false;
    if(this.selectedScenarioId != this.uuidNIL){
      fromScenario = true;
    }
    this.criterionLibraryEditorDialogData = {
      showDialog: true,
      libraryId: targetConditionGoal.criterionLibrary.id,
      isCallFromScenario: fromScenario
    };
  }

  onEditTargetConditionGoalCriterionLibrary(criterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary) && this.selectedTargetConditionGoalForCriteriaEdit.id !== this.uuidNIL) {
      this.selectedTargetConditionGoalLibrary = {
        ...this.selectedTargetConditionGoalLibrary,
        targetConditionGoals: update(
            findIndex(propEq('id', this.selectedTargetConditionGoalForCriteriaEdit.id), this.selectedTargetConditionGoalLibrary.targetConditionGoals),
            {...this.selectedTargetConditionGoalForCriteriaEdit, criterionLibrary: criterionLibrary},
            this.selectedTargetConditionGoalLibrary.targetConditionGoals
        )
      };
    }

    this.selectedTargetConditionGoalForCriteriaEdit = clone(emptyTargetConditionGoal);
  }

  onUpsertTargetConditionGoalLibrary(library: TargetConditionGoalLibrary, scenarioId: string) {
    this.createTargetConditionGoalLibraryDialogData = clone(emptyCreateTargetConditionGoalLibraryDialogData);

    if (!isNil(library)) {
      this.createTargetConditionGoalLibraryAction({library: library, scenarioId: scenarioId});
    }
  }

  onDiscardChanges() {
    this.librarySelectItemValue = null;
    setTimeout(() => {
      if (this.selectedScenarioId !== this.uuidNIL &&
          hasAppliedLibrary(this.stateTargetConditionGoalLibraries, this.selectedScenarioId)) {
        this.librarySelectItemValue = getAppliedLibraryId(this.stateTargetConditionGoalLibraries, this.selectedScenarioId);
      }
    });
  }

  onRemoveTargetConditionGoals() {
    this.selectedTargetConditionGoalLibrary = {
      ...this.selectedTargetConditionGoalLibrary,
      targetConditionGoals: this.selectedTargetConditionGoalLibrary.targetConditionGoals
          .filter((targetConditionGoal: TargetConditionGoal) => !contains(targetConditionGoal.id, this.selectedTargetConditionGoalIds))
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
      this.librarySelectItemValue = null;
      this.deleteTargetConditionGoalLibraryAction({libraryId: this.selectedTargetConditionGoalLibrary.id});
    }
  }

  disableCrudButton() {
    if (this.hasSelectedLibrary) {
      const allDataIsValid = this.selectedTargetConditionGoalLibrary.targetConditionGoals.every((t: TargetConditionGoal) => {
        return this.rules['generalRules'].valueIsNotEmpty(t.attribute) === true &&
            this.rules['generalRules'].valueIsNotEmpty(t.name) === true &&
            this.rules['generalRules'].valueIsNotEmpty(t.target) === true;
      });

      return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedTargetConditionGoalLibrary.name) === true &&
          allDataIsValid);
    }

    return true;
  }
}
</script>

<style>
.targets-data-table {
  height: 425px;
  overflow-y: auto;
  overflow-x: hidden;
}

.targets-data-table .v-menu--inline, .target-criteria-output {
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
