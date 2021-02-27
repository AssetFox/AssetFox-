<template>
  <v-layout column>
    <v-flex xs12>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onCreateNewLibrary" class="ara-blue-bg white--text" v-show="selectedScenarioId === '0'">
            New Library
          </v-btn>
          <v-select :items="performanceCurveLibrarySelectItems"
                    label="Select a Performance Library"
                    outline v-if="!hasSelectedLibrary || selectedScenarioId !== '0'"
                    v-model="librarySelectItemValue">
          </v-select>
          <v-text-field label="Library Name"
                        v-if="hasSelectedLibrary && selectedScenarioId === '0'"
                        v-model="selectedPerformanceCurveLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]">
            <template slot="append">
              <v-btn @click="librarySelectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="hasSelectedLibrary && selectedScenarioId === '0'">
            Owner: {{ selectedPerformanceCurveLibrary.owner ? selectedPerformanceCurveLibrary.owner : "[ No Owner ]" }}
          </div>
          <v-checkbox class="sharing" label="Shared"
                      v-if="hasSelectedLibrary && selectedScenarioId === '0'"
                      v-model="selectedPerformanceCurveLibrary.shared"/>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedLibrary"></v-divider>
    <v-flex v-show="hasSelectedLibrary" xs12>
      <v-layout class="header-height" justify-center>
        <v-flex xs8>
          <v-btn @click="showCreatePerformanceCurveDialog = true" class="ara-blue-bg white--text">
            Add
          </v-btn>
        </v-flex>
      </v-layout>
      <v-layout class="data-table" justify-center>
        <v-flex xs8>
          <v-card>
            <v-card-title>
              Performance equation
              <v-spacer></v-spacer>
              <v-text-field append-icon="fas fa-search" hide-details lablel="Search"
                            single-line
                            v-model="gridSearchTerm">
              </v-text-field>
            </v-card-title>
            <v-data-table :headers="performanceCurveGridHeaders"
                          :items="performanceCurveGridData"
                          :search="gridSearchTerm"
                          class="elevation-1 fixed-header v-table__overflow"
                          item-key="performanceLibraryEquationId">
              <template slot="items" slot-scope="props">
                <td class="text-xs-center">
                  <v-edit-dialog
                      :return-value.sync="props.item.name"
                      @save="onEditPerformanceCurveProperty(props.item.id, 'name', props.item.name)"
                      large lazy persistent>
                    <v-text-field readonly single-line
                                  class="sm-txt equation-name-text-field-output"
                                  :value="props.item.name"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <template slot="input">
                      <v-text-field label="Edit" single-line v-model="props.item.name"
                                    :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    </template>
                  </v-edit-dialog>
                </td>
                <td class="text-xs-center">
                  <v-edit-dialog
                      :return-value.sync="props.item.attribute"
                      @save="onEditPerformanceCurveProperty(props.item.id, 'attribute', props.item.attribute)"
                      large lazy persistent>
                    <v-text-field readonly single-line class="sm-txt attribute-text-field-output"
                                  :value="props.item.attribute"
                                  :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    <template slot="input">
                      <v-select :items="attributeSelectItems" label="Edit"
                                v-model="props.item.attribute"
                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                    </template>
                  </v-edit-dialog>
                </td>
                <td class="text-xs-center">
                  <v-menu left min-height="500px" min-width="500px"
                          v-show="props.item.equation !== ''">
                    <template slot="activator">
                      <v-btn class="ara-blue" icon>
                        <v-icon>fas fa-eye</v-icon>
                      </v-btn>
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea class="sm-txt" :value="props.item.equation" full-width
                                    no-resize outline
                                    readonly
                                    rows="5"/>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <v-btn @click="onShowEquationEditorDialog(props.item.id)" class="edit-icon" icon>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </td>
                <td class="text-xs-center">
                  <v-menu min-height="500px" min-width="500px" right
                          v-show="props.item.criterion !== ''">
                    <template slot="activator">
                      <v-btn class="ara-blue" flat icon>
                        <v-icon>fas fa-eye</v-icon>
                      </v-btn>
                    </template>
                    <v-card>
                      <v-card-text>
                        <v-textarea class="sm-txt" :value="props.item.criterion" full-width
                                    no-resize outline
                                    readonly
                                    rows="5"/>
                      </v-card-text>
                    </v-card>
                  </v-menu>
                  <v-btn @click="onEditPerformanceCurveCriterionLibrary(props.item.id)" class="edit-icon" icon>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </td>
                <td class="text-xs-center">
                  <v-btn @click="onDeletePerformanceCurve(props.item.id)" class="ara-orange" icon>
                    <v-icon>fas fa-trash</v-icon>
                  </v-btn>
                </td>
              </template>
            </v-data-table>
          </v-card>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-divider v-show="hasSelectedLibrary"></v-divider>
    <v-flex v-show="hasSelectedLibrary && selectedScenarioId === uuidNIL"
            xs12>
      <v-layout justify-center>
        <v-flex xs6>
          <v-textarea label="Description" no-resize outline rows="4"
                      v-model="selectedPerformanceCurveLibrary.description"/>
        </v-flex>
      </v-layout>
    </v-flex>
    <v-flex xs12>
      <v-layout justify-end row v-show="hasSelectedLibrary">
        <v-btn :disabled="disableSubmitAction()"
               @click="onAddOrUpdatePerformanceCurveLibrary(selectedPerformanceCurveLibrary, selectedScenarioId)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId !== uuidNIL">
          Save
        </v-btn>
        <v-btn :disabled="disableSubmitAction()"
               @click="onAddOrUpdatePerformanceCurveLibrary(selectedPerformanceCurveLibrary, uuidNIL)"
               class="ara-blue-bg white--text"
               v-show="selectedScenarioId === uuidNIL">
          Update Library
        </v-btn>
        <v-btn :disabled="disableSubmitAction()" @click="onCreateAsNewLibrary"
               class="ara-blue-bg white--text">
          Create as New Library
        </v-btn>
        <v-btn @click="onDeletePerformanceCurveLibrary" class="ara-orange-bg white--text"
               v-show="selectedScenarioId === uuidNIL" :disabled="!hasSelectedLibrary">
          Delete Library
        </v-btn>
        <v-btn :disabled="!hasSelectedLibrary" @click="onDiscardChanges"
               class="ara-orange-bg white--text" v-show="selectedScenarioId !== uuidNIL">
          Discard Changes
        </v-btn>
      </v-layout>
    </v-flex>

    <ConfirmDeleteAlert :dialogData="confirmDeleteAlertData"
                        @submit="onSubmitConfirmDeleteAlertResult"/>

    <CreatePerformanceCurveLibraryDialog :dialogData="createPerformanceCurveLibraryDialogData"
                                         @submit="onAddOrUpdatePerformanceCurveLibrary"/>

    <CreatePerformanceCurveDialog :showDialog="showCreatePerformanceCurveDialog"
                                  @submit="onCreatePerformanceCurve"/>

    <EquationEditorDialog :dialogData="equationEditorDialogData" @submit="onSubmitEquationEditorDialogResult"/>

    <CriterionLibraryEditorDialog :dialogData="criterionLibraryEditorDialogData"
                                  @submit="onSubmitCriterionLibraryEditorDialogResult"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Watch} from 'vue-property-decorator';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import CreatePerformanceCurveLibraryDialog from './performance-curve-editor-dialogs/CreatePerformanceCurveLibraryDialog.vue';
import CreatePerformanceCurveDialog from './performance-curve-editor-dialogs/CreatePerformanceCurveDialog.vue';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import CriterionLibraryEditorDialog from '../../shared/modals/CriterionLibraryEditorDialog.vue';
import {
  emptyPerformanceCurve,
  emptyPerformanceCurveLibrary,
  PerformanceCurve,
  PerformanceCurveGridItem,
  PerformanceCurveLibrary
} from '@/shared/models/iAM/performance';
import {SelectItem} from '@/shared/models/vue/select-item';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {any, append, clone, find, findIndex, isNil, propEq, reject, update} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {
  CreatePerformanceCurveLibraryDialogData,
  emptyCreatePerformanceLibraryDialogData
} from '@/shared/models/modals/create-performance-curve-library-dialog-data';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import {
  emptyEquationEditorDialogData,
  EquationEditorDialogData
} from '@/shared/models/modals/equation-editor-dialog-data';
import {Attribute} from '@/shared/models/iAM/attribute';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {Equation} from '@/shared/models/iAM/equation';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';
import {getBlankGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibraryId, hasAppliedLibrary} from '@/shared/utils/library-utils';

@Component({
  components: {
    CreatePerformanceCurveLibraryDialog,
    CreatePerformanceCurveDialog,
    EquationEditorDialog,
    CriterionLibraryEditorDialog,
    ConfirmDeleteAlert: Alert
  }
})
export default class PerformanceCurveEditor extends Vue {
  @State(state => state.performanceCurvesEditor.performanceCurveLibraries) statePerformanceCurveLibraries: PerformanceCurveLibrary[];
  @State(state => state.performanceCurvesEditor.selectedPerformanceCurveLibrary) stateSelectedPerformanceCurveLibrary: PerformanceCurveLibrary;
  @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

  @Action('getPerformanceCurveLibraries') getPerformanceCurveLibrariesAction: any;
  @Action('selectPerformanceCurveLibrary') selectPerformanceCurveLibraryAction: any;
  @Action('addOrUpdatePerformanceCurveLibrary') addOrUpdatePerformanceCurveLibraryAction: any;
  @Action('deletePerformanceCurveLibrary') deletePerformanceCurveLibraryAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;

  gridSearchTerm = '';
  selectedPerformanceCurveLibrary: PerformanceCurveLibrary = clone(emptyPerformanceCurveLibrary);
  selectedScenarioId: string = getBlankGuid();
  hasSelectedLibrary: boolean = false;
  performanceCurveLibrarySelectItems: SelectItem[] = [];
  librarySelectItemValue: string | null = '';
  performanceCurveGridHeaders: DataTableHeader[] = [
    {text: 'Name', value: 'name', align: 'center', sortable: true, class: '', width: ''},
    {text: 'Attribute', value: 'attribute', align: 'center', sortable: true, class: '', width: ''},
    {text: 'Equation', value: 'equation', align: 'center', sortable: false, class: '', width: ''},
    {text: 'Criterion', value: 'criterion', align: 'center', sortable: false, class: '', width: ''},
    {text: '', value: '', align: 'center', sortable: false, class: '', width: ''}
  ];
  performanceCurveGridData: PerformanceCurveGridItem[] = [];
  attributeSelectItems: SelectItem[] = [];
  selectedPerformanceCurve: PerformanceCurve = clone(emptyPerformanceCurve);
  hasSelectedPerformanceCurve: boolean = false;
  createPerformanceCurveLibraryDialogData: CreatePerformanceCurveLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);
  equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
  criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  showCreatePerformanceCurveDialog = false;
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  rules: InputValidationRules = clone(rules);
  uuidNIL: string = getBlankGuid();

  /**
   * beforeRouteEnter => This event handler is used to set the selectedScenarioId object unset the librarySelectItemValue object,
   * trigger an action to get all performance curve libraries, and trigger an action to get the selected scenario's
   * performance curve library if it has one.
   */
  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      if (to.path.indexOf('PerformanceCurveEditor/Scenario') !== -1) {
        vm.selectedScenarioId = to.query.scenarioId;
        if (vm.selectedScenarioId === vm.uuidNIL) {
          vm.setErrorMessageAction({message: 'Unable to identify selected scenario.'});
          vm.$router.push('/Scenarios/');
        }
      }

      vm.librarySelectItemValue = null;
      vm.getPerformanceCurveLibrariesAction();
    });
  }

  /**
   * mounted => This event handler is used to trigger a function to set the attributeSelectItems object.
   */
  mounted() {
    this.setAttributeSelectItems();
  }

  /**
   * beforeDestroy => This event handler is used to trigger an action to reset the vuex hasUnsavedChanges object.
   */
  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  /**
   * onStatePerformanceCurveLibrariesChanged => This statePerformanceCurveLibraries watcher is used to set the
   * performanceCurveLibrarySelectItems object.
   */
  @Watch('statePerformanceCurveLibraries')
  onStatePerformanceCurveLibrariesChanged() {
    this.performanceCurveLibrarySelectItems = this.statePerformanceCurveLibraries
        .map((library: PerformanceCurveLibrary) => ({
          text: library.name,
          value: library.id
        }));

    if (this.selectedScenarioId !== this.uuidNIL &&
        hasAppliedLibrary(this.statePerformanceCurveLibraries, this.selectedScenarioId)) {
      this.librarySelectItemValue = getAppliedLibraryId(this.statePerformanceCurveLibraries, this.selectedScenarioId);
    }
  }

  /**
   * onLibrarySelectItemValueChanged => This librarySelectItemValue watcher is used to trigger an action to set
   * the vuex selectedPerformanceCurveLibrary object.
   */
  @Watch('librarySelectItemValue')
  onLibrarySelectItemValueChanged() {
    this.selectPerformanceCurveLibraryAction({libraryId: this.librarySelectItemValue});
  }

  /**
   * onStateSelectedPerformanceCurveLibraryChanged => This stateSelectedPerformanceCurveLibrary watcher is used to
   * set the selectedPerformanceCurveLibrary object.
   */
  @Watch('stateSelectedPerformanceCurveLibrary')
  onStateSelectedPerformanceCurveLibraryChanged() {
    this.selectedPerformanceCurveLibrary = clone(this.stateSelectedPerformanceCurveLibrary);
  }

  /**
   * onSelectedPerformanceCurveLibraryChanged => This selectedPerformanceCurveLibrary watcher is used to trigger
   * an action to set the vuex hasUnsavedChanges object, set the hasSelectedLibrary object, and set the
   * performanceCurveGridData object.
   */
  @Watch('selectedPerformanceCurveLibrary')
  onSelectedPerformanceCurveLibraryChanged() {
    this.setHasUnsavedChangesAction({
      value: hasUnsavedChangesCore(
          'performance-curves', this.selectedPerformanceCurveLibrary, this.stateSelectedPerformanceCurveLibrary
      )
    });

    this.hasSelectedLibrary = this.selectedPerformanceCurveLibrary.id !== this.uuidNIL;

    this.performanceCurveGridData = this.selectedPerformanceCurveLibrary.performanceCurves
        .map((item: PerformanceCurve) => ({
          id: item.id,
          name: item.name,
          attribute: item.attribute,
          equation: item.equation.expression,
          criterion: item.criterionLibrary.mergedCriteriaExpression
        }));

    if (this.librarySelectItemValue !== this.selectedPerformanceCurveLibrary.id) {
      this.librarySelectItemValue = this.selectedPerformanceCurveLibrary.id;
    }
  }

  /**
   * onStateNumericAttributesChanged => This stateNumericAttributes watcher triggers a function to set the
   * attributeSelectItems object.
   */
  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    this.setAttributeSelectItems();
  }

  /**
   * setAttributeSelectItems => This function is used to set the attributeSelectItems object.
   */
  setAttributeSelectItems() {
    if (hasValue(this.stateNumericAttributes)) {
      this.attributeSelectItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
        text: attribute.name,
        value: attribute.name
      }));
    }
  }

  /**
   * onCreateNewLibrary => This function is used to set the createPerformanceCurveLibraryDialogData object.
   */
  onCreateNewLibrary() {
    this.createPerformanceCurveLibraryDialogData = {...emptyCreatePerformanceLibraryDialogData, showDialog: true};
  }

  /**
   * onCreatePerformanceCurve => This function is used to append new performance curves to the
   * selectedPerformanceCurveLibrary object's performanceCurves property.
   */
  onCreatePerformanceCurve(newPerformanceCurve: PerformanceCurve) {
    this.showCreatePerformanceCurveDialog = false;

    if (!isNil(newPerformanceCurve)) {
      this.selectedPerformanceCurveLibrary = {
        ...this.selectedPerformanceCurveLibrary,
        performanceCurves: append(newPerformanceCurve, this.selectedPerformanceCurveLibrary.performanceCurves)
      };
    }
  }

  /**
   * onEditPerformanceCurveProperty => This function is used to modify a performance curve object's property within
   * the selectedPerformanceCurveLibrary object's performanceCurves property.
   */
  onEditPerformanceCurveProperty(id: string, property: string, value: any) {
    if (any(propEq('id', id), this.selectedPerformanceCurveLibrary.performanceCurves)) {
      const performanceCurve: PerformanceCurve = find(
          propEq('id', id), this.selectedPerformanceCurveLibrary.performanceCurves
      ) as PerformanceCurve;

      this.selectedPerformanceCurveLibrary = {
        ...this.selectedPerformanceCurveLibrary,
        performanceCurves: update(
            findIndex(propEq('id', performanceCurve.id), this.selectedPerformanceCurveLibrary.performanceCurves),
            setItemPropertyValue(property, value, performanceCurve) as PerformanceCurve,
            this.selectedPerformanceCurveLibrary.performanceCurves
        )
      };
    }
  }

  /**
   * onShowEquationEditorDialog => This function is used to set the selectedPerformanceCurve object and then set
   * the equationEditorDialogData object.
   */
  onShowEquationEditorDialog(performanceCurveId: string) {
    this.selectedPerformanceCurve = find(
        propEq('id', performanceCurveId), this.selectedPerformanceCurveLibrary.performanceCurves
    ) as PerformanceCurve;

    if (!isNil(this.selectedPerformanceCurve)) {
      this.hasSelectedPerformanceCurve = true;

      this.equationEditorDialogData = {showDialog: true, equation: this.selectedPerformanceCurve.equation};
    }
  }

  /**
   * onSubmitEquationEditorDialogResult => This function is used to modify the selectedPerformanceCurve object's
   * equation property within the selectedPerformanceCurveLibrary object's performanceCurves property.
   */
  onSubmitEquationEditorDialogResult(equation: Equation) {
    this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

    if (!isNil(equation) && this.hasSelectedPerformanceCurve) {
      this.selectedPerformanceCurveLibrary = {
        ...this.selectedPerformanceCurveLibrary,
        performanceCurves: update(
            findIndex(propEq('id', this.selectedPerformanceCurve.id),
                this.selectedPerformanceCurveLibrary.performanceCurves),
            {...this.selectedPerformanceCurve, equation: equation},
            this.selectedPerformanceCurveLibrary.performanceCurves
        )
      };
    }

    this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
    this.hasSelectedPerformanceCurve = false;
  }

  /**
   * onEditPerformanceCurveCriterionLibrary => This function is used to set the selectedPerformanceCurve object and set the
   * criterionLibraryEditorDialogData object.
   */
  onEditPerformanceCurveCriterionLibrary(performanceCurveId: string) {
    this.selectedPerformanceCurve = find(
        propEq('id', performanceCurveId), this.selectedPerformanceCurveLibrary.performanceCurves
    ) as PerformanceCurve;

    if (!isNil(this.selectedPerformanceCurve)) {
      this.hasSelectedPerformanceCurve = true;

      this.criterionLibraryEditorDialogData = {
        showDialog: true,
        libraryId: this.selectedPerformanceCurve.criterionLibrary.id
      };
    }
  }

  /**
   * onSubmitCriterionLibraryEditorDialogResult => This function is used to modify the selectedPerformanceCurve object's
   * criterionLibrary property within the selectedPerformanceCurveLibrary object's performanceCurves property.
   */
  onSubmitCriterionLibraryEditorDialogResult(criterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary) && this.hasSelectedPerformanceCurve) {
      this.selectedPerformanceCurveLibrary = {
        ...this.selectedPerformanceCurveLibrary,
        performanceCurves: update(
            findIndex(propEq('id', this.selectedPerformanceCurve.id),
                this.selectedPerformanceCurveLibrary.performanceCurves),
            {...this.selectedPerformanceCurve, criterionLibrary: criterionLibrary},
            this.selectedPerformanceCurveLibrary.performanceCurves
        )
      };
    }

    this.selectedPerformanceCurve = clone(emptyPerformanceCurve);
    this.hasSelectedPerformanceCurve = false;
  }

  /**
   * onDeletePerformanceCurve => This function is used to remove a performance curve object from the
   * selectedPerformanceCurveLibrary object's performanceCurves property.
   */
  onDeletePerformanceCurve(performanceCurveId: string) {
    this.selectedPerformanceCurveLibrary = {
      ...this.selectedPerformanceCurveLibrary,
      performanceCurves: reject(propEq('id', performanceCurveId),
          this.selectedPerformanceCurveLibrary.performanceCurves)
    };
  }

  /**
   * onCreateAsNewLibrary => This function is used to set the createPerformanceCurveLibraryDialogData object
   * using the selectedPerformanceCurveLibrary object's description and performanceCurves properties.
   */
  onCreateAsNewLibrary() {
    this.createPerformanceCurveLibraryDialogData = {
      showDialog: true,
      performanceCurves: this.selectedPerformanceCurveLibrary.performanceCurves,
      scenarioId: this.selectedScenarioId
    };
  }

  /**
   * onAddOrUpdatePerformanceCurveLibrary => This function is used to trigger an action that will add/update a performance
   * curve library object and its child objects in the backend database.
   */
  onAddOrUpdatePerformanceCurveLibrary(performanceCurveLibrary: PerformanceCurveLibrary, scenarioId: string) {
    this.createPerformanceCurveLibraryDialogData = clone(emptyCreatePerformanceLibraryDialogData);

    if (!isNil(performanceCurveLibrary) && performanceCurveLibrary.id !== this.uuidNIL) {
      this.addOrUpdatePerformanceCurveLibraryAction({library: performanceCurveLibrary, scenarioId: scenarioId});
    }
  }

  /**
   * onDiscardChanges => This function is used to unset the librarySelectItemValue object and triggers an action to set
   * the vuex selectedPerformanceCurveLibrary object if there is a selected scenario with one of the available libraries
   * applied to it.
   */
  onDiscardChanges() {
    this.librarySelectItemValue = null;
    setTimeout(() => {
      if (this.selectedScenarioId !== this.uuidNIL &&
          hasAppliedLibrary(this.statePerformanceCurveLibraries, this.selectedScenarioId)) {
        const libraryId: string = getAppliedLibraryId(this.statePerformanceCurveLibraries, this.selectedScenarioId);
        this.librarySelectItemValue = libraryId;
      }
    });
  }

  /**
   * onDeletePerformanceCurveLibrary => This function is used to set the confirmDeleteAlertData object.
   */
  onDeletePerformanceCurveLibrary() {
    this.confirmDeleteAlertData = {
      showDialog: true,
      heading: 'Warning',
      choice: true,
      message: 'Are you sure you want to delete?'
    };
  }

  /**
   * onSubmitConfirmDeleteAlertResult => This function is used to trigger an action to delete the
   * selectedPerformanceCurveLibrary object's data from the backend database.
   */
  onSubmitConfirmDeleteAlertResult(submit: boolean) {
    this.confirmDeleteAlertData = clone(emptyAlertData);

    if (submit) {
      this.librarySelectItemValue = null;
      this.deletePerformanceCurveLibraryAction({libraryId: this.selectedPerformanceCurveLibrary.id});
    }
  }

  /**
   * disableSubmitAction => This function is used to disable all submit action buttons if there is no library selected or
   * if the selectedPerformanceCurveLibrary object's data is invalid.
   */
  disableSubmitAction() {
    if (this.hasSelectedLibrary) {
      const allSubDataIsValid: boolean = this.selectedPerformanceCurveLibrary.performanceCurves
          .every((performanceCurve: PerformanceCurve) => {
            return this.rules['generalRules'].valueIsNotEmpty(performanceCurve.name) === true &&
                this.rules['generalRules'].valueIsNotEmpty(performanceCurve.attribute) === true;
          });

      return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedPerformanceCurveLibrary.name) === true &&
          allSubDataIsValid);
    }

    return true;
  }
}
</script>

<style>
.equation-name-text-field-output {
  margin-left: 10px;
}

.attribute-text-field-output {
  margin-left: 15px;
}

.header-height {
  height: 45px;
}

.sharing label {
  padding-top: 0.5em;
}

.sharing {
  padding-top: 0;
  margin: 0;
}
</style>
