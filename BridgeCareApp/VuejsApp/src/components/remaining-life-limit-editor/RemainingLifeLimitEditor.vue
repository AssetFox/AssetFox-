<template>
  <v-layout column>
    <v-flex xs12>
      <v-layout justify-center>
        <v-flex xs3>
          <v-btn @click="onShowCreateRemainingLifeLimitLibraryDialog(false)"
                 class="ara-blue-bg white--text" v-show="!hasScenario">
            New Library
          </v-btn>
          <v-select v-if="!hasSelectedRemainingLifeLimitLibrary || hasScenario"
                    :items="selectListItems"
                    label="Select a Remaining Life Limit Library" outline v-model="selectItemValue"/>
          <v-text-field v-if="hasSelectedRemainingLifeLimitLibrary && !hasScenario"
                        label="Library Name"
                        v-model="selectedRemainingLifeLimitLibrary.name"
                        :rules="[rules['generalRules'].valueIsNotEmpty]">
            <template slot="append">
              <v-btn @click="selectItemValue = null" class="ara-orange" icon>
                <v-icon>fas fa-caret-left</v-icon>
              </v-btn>
            </template>
          </v-text-field>
        </v-flex>
      </v-layout>
      <v-flex xs3 v-show="hasSelectedRemainingLifeLimitLibrary || hasScenario">
        <v-btn @click="onShowCreateRemainingLifeLimitDialog" class="ara-blue-bg white--text">Add</v-btn>
      </v-flex>
    </v-flex>
    <v-flex xs12 v-show="hasSelectedRemainingLifeLimitLibrary || hasScenario">
      <div class="remaining-life-limit-data-table">
        <v-data-table :headers="gridHeaders" :items="gridData"
                      class="elevation-1 fixed-header v-table__overflow">
          <template slot="items" slot-scope="props">
            <td>
              <v-edit-dialog :return-value.sync="props.item.attribute" large lazy persistent
                             @save="onEditRemainingLifeLimitProperty(props.item, 'attribute', props.item.attribute)">
                <v-text-field readonly single-line class="sm-txt" :value="props.item.attribute"
                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                <template slot="input">
                  <v-select :items="numericAttributeSelectItems" label="Select an Attribute"
                            outline v-model="props.item.attribute"
                            :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                </template>
              </v-edit-dialog>
            </td>
            <td>
              <v-edit-dialog :return-value.sync="props.item.value" large lazy persistent
                             @save="onEditRemainingLifeLimitProperty(props.item, 'value', props.item.value)">
                <v-text-field readonly single-line class="sm-txt" :value="props.item.value"
                              :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                <template slot="input">
                  <v-text-field label="Edit" single-line :mask="'##########'"
                                v-model.number="props.item.value"
                                :rules="[rules['generalRules'].valueIsNotEmpty]"/>
                </template>
              </v-edit-dialog>
            </td>
            <td>
              <v-text-field :value="props.item.criterionLibrary.mergedCriteriaExpression" readonly>
                <template slot="append-outer">
                  <v-icon @click="onShowCriterionLibraryEditorDialog(props.item)"
                          class="edit-icon">
                    fas fa-edit
                  </v-icon>
                </template>
              </v-text-field>
            </td>
          </template>
        </v-data-table>
      </div>
    </v-flex>
    <v-flex xs12>
      <v-layout justify-end row >
        <v-btn :disabled="disableCrudButton()"
               @click="onSaveScenarioRemainingLifeLimit(selectedScenarioId)"
               class="ara-blue-bg white--text"
               v-show="hasScenario">
          Save
        </v-btn>
        <v-btn :disabled="disableCrudButton()"
               @click="onUpsertRemainingLifeLimitLibrary(selectedRemainingLifeLimitLibrary)"
               class="ara-blue-bg white--text"
               v-show="hasSelectedRemainingLifeLimitLibrary || !hasScenario">
          Update Library
        </v-btn>
        <v-btn :disabled="disableCrudButton()" @click="onShowCreateRemainingLifeLimitLibraryDialog(true)"
               class="ara-blue-bg white--text">
          Create as New Library
        </v-btn>
        <v-btn v-show="!hasScenario" class="ara-orange-bg white--text"
               @click="onShowConfirmDeleteAlert"
               :disabled="!hasSelectedRemainingLifeLimitLibrary">
          Delete Library
        </v-btn>
        <v-btn @click="onDiscardChanges"
               class="ara-orange-bg white--text"
               v-show="hasSelectedRemainingLifeLimitLibrary || hasScenario">
          Discard Changes
        </v-btn>
      </v-layout>
    </v-flex>

    <ConfirmDeleteAlert :dialogData="confirmDeleteAlertData" @submit="onSubmitConfirmDeleteAlertResult"/>

    <CreateRemainingLifeLimitLibraryDialog :dialogData="createRemainingLifeLimitLibraryDialogData"
                                           @submit="onUpsertRemainingLifeLimitLibrary"/>

    <CreateRemainingLifeLimitDialog :dialogData="createRemainingLifeLimitDialogData"
                                    @submit="onAddRemainingLifeLimit"/>

    <CriterionLibraryEditorDialog :dialogData="criterionLibraryEditorDialogData"
                                  @submit="onEditRemainingLifeLimitCriterionLibrary"/>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import {Action, State} from 'vuex-class';
import {Watch} from 'vue-property-decorator';
import {
  emptyRemainingLifeLimit,
  emptyRemainingLifeLimitLibrary,
  RemainingLifeLimit,
  RemainingLifeLimitLibrary
} from '@/shared/models/iAM/remaining-life-limit';
import {append, clone, findIndex, isNil, propEq, update} from 'ramda';
import {hasValue} from '@/shared/utils/has-value-util';
import {SelectItem} from '@/shared/models/vue/select-item';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {Attribute} from '@/shared/models/iAM/attribute';
import CreateRemainingLifeLimitDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitDialog.vue';
import {
  CriterionLibraryEditorDialogData,
  emptyCriterionLibraryEditorDialogData
} from '@/shared/models/modals/criterion-library-editor-dialog-data';
import CriterionLibraryEditorDialog from '@/shared/modals/CriterionLibraryEditorDialog.vue';
import {
  CreateRemainingLifeLimitLibraryDialogData,
  emptyCreateRemainingLifeLimitLibraryDialogData
} from '@/shared/models/modals/create-remaining-life-limit-library-dialog-data';
import CreateRemainingLifeLimitLibraryDialog from '@/components/remaining-life-limit-editor/remaining-life-limit-editor-dialogs/CreateRemainingLifeLimitLibraryDialog.vue';
import {
  CreateRemainingLifeLimitDialogData,
  emptyCreateRemainingLifeLimitDialogData
} from '@/shared/models/modals/create-remaining-life-limit-dialog-data';
import {AlertData, emptyAlertData} from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import {hasUnsavedChangesCore} from '@/shared/utils/has-unsaved-changes-helper';
import {InputValidationRules, rules} from '@/shared/utils/input-validation-rules';
import {setItemPropertyValue} from '@/shared/utils/setter-utils';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import {getAppliedLibraryId, hasAppliedLibrary} from '@/shared/utils/library-utils';
import {CriterionLibrary} from '@/shared/models/iAM/criteria';
import { ScenarioRoutePaths } from '@/shared/utils/route-paths';

@Component({
  components: {
    CreateRemainingLifeLimitLibraryDialog,
    CreateRemainingLifeLimitDialog,
    CriterionLibraryEditorDialog,
    ConfirmDeleteAlert: Alert
  }
})
export default class RemainingLifeLimitEditor extends Vue {
  @State(state => state.remainingLifeLimitModule.remainingLifeLimitLibraries) stateRemainingLifeLimitLibraries: RemainingLifeLimitLibrary[];
  @State(state => state.remainingLifeLimitModule.selectedRemainingLifeLimitLibrary) stateSelectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary;
  @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];
  @State(
        state => state.remainingLifeLimitModule.scenarioRemainingLifeLimits,
    )
    stateScenarioRemainingLifeLimits: RemainingLifeLimit[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges)
    hasUnsavedChanges: boolean;

  @Action('getRemainingLifeLimitLibraries') getRemainingLifeLimitLibrariesAction: any;
  @Action('upsertRemainingLifeLimitLibrary') upsertRemainingLifeLimitLibraryAction: any;
  @Action('deleteRemainingLifeLimitLibrary') deleteRemainingLifeLimitLibraryAction: any;
  @Action('selectRemainingLifeLimitLibrary') selectRemainingLifeLimitLibraryAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
  @Action('getScenariotRemainingLifeLimit')
  getScenariotRemainingLifeLimitAction: any;
  @Action('upsertScenarioRemainingLifeLimit')
  upsertScenarioRemainingLifeLimitAction: any;

  remainingLifeLimitLibraries: RemainingLifeLimitLibrary[] = [];
  selectedRemainingLifeLimitLibrary: RemainingLifeLimitLibrary = clone(emptyRemainingLifeLimitLibrary);
  selectedScenarioId: string = getBlankGuid();
  selectItemValue: string | null = '';
  selectListItems: SelectItem[] = [];
  hasSelectedRemainingLifeLimitLibrary: boolean = false;
  gridHeaders: DataTableHeader[] = [
    {
      text: 'Remaining Life Attribute',
      value: 'attribute',
      align: 'left',
      sortable: true,
      class: '',
      width: '12.4%'
    },
    {text: 'Limit', value: 'value', align: 'left', sortable: true, class: '', width: '12.4%'},
    {text: 'Criteria', value: 'criteria', align: 'left', sortable: false, class: '', width: '75%'}
  ];
  gridData: RemainingLifeLimit[] = [];
  numericAttributeSelectItems: SelectItem[] = [];
  createRemainingLifeLimitDialogData: CreateRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);
  selectedRemainingLifeLimit: RemainingLifeLimit = clone(emptyRemainingLifeLimit);
  criterionLibraryEditorDialogData: CriterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);
  createRemainingLifeLimitLibraryDialogData: CreateRemainingLifeLimitLibraryDialogData = clone(
      emptyCreateRemainingLifeLimitLibraryDialogData
  );
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  rules: InputValidationRules = rules;
  uuidNIL: string = getBlankGuid();
  hasScenario: boolean = false;
  currentUrl: string = window.location.href;

  beforeRouteEnter(to: any, from: any, next: any) {
    next((vm: any) => {
      vm.selectItemValue = null;
      vm.getRemainingLifeLimitLibrariesAction();
      if (to.path.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !== -1) {
        vm.selectedScenarioId = to.query.scenarioId;
        if (vm.selectedScenarioId === vm.uuidNIL) {
          vm.setErrorMessageAction({message: 'Found no selected scenario for edit'});
          vm.$router.push('/Scenarios/');
        }
        vm.hasScenario = true;
        vm.getScenariotRemainingLifeLimitAction(vm.selectedScenarioId);
      }
    });
  }

  beforeDestroy() {
    this.setHasUnsavedChangesAction({value: false});
  }

  @Watch('stateRemainingLifeLimitLibraries')
  onStateRemainingLifeLimitLibrariesChanged() {
    this.selectListItems = this.stateRemainingLifeLimitLibraries
        .map((remainingLifeLimitLibrary: RemainingLifeLimitLibrary) => ({
          text: remainingLifeLimitLibrary.name,
          value: remainingLifeLimitLibrary.id
        }));

    // if (this.selectedScenarioId !== this.uuidNIL && hasAppliedLibrary(this.stateRemainingLifeLimitLibraries, this.selectedScenarioId)) {
    //   this.selectItemValue = getAppliedLibraryId(this.stateRemainingLifeLimitLibraries, this.selectedScenarioId);
    // }
  }

  @Watch('selectItemValue')
  onSelectItemValueChanged() {
    this.selectRemainingLifeLimitLibraryAction({libraryId: this.selectItemValue});
  }

  @Watch('stateSelectedRemainingLifeLimitLibrary')
  onStateSelectedRemainingLifeLimitLibraryChanged() {
    this.selectedRemainingLifeLimitLibrary = clone(this.stateSelectedRemainingLifeLimitLibrary);
  }

  @Watch('selectedRemainingLifeLimitLibrary')
  onSelectedRemainingLifeLimitLibraryChanged() {
    // this.setHasUnsavedChangesAction({
    //   value: hasUnsavedChangesCore(
    //       'remaining-life-limit', this.selectedRemainingLifeLimitLibrary, this.stateSelectedRemainingLifeLimitLibrary
    //   )
    // });
    this.hasSelectedRemainingLifeLimitLibrary = this.selectedRemainingLifeLimitLibrary.id !== this.uuidNIL;
    this.gridData = clone(this.selectedRemainingLifeLimitLibrary.remainingLifeLimits);
  }

  @Watch('stateScenarioRemainingLifeLimits')
    onStateScenarioRemainingLifeLimitsChanged() {
        if (
            this.currentUrl.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !==
            -1
        ) {
            this.gridData = clone(
                this.stateScenarioRemainingLifeLimits,
            );
        }
    }
    @Watch('gridData')
    onGridDataChanged() {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore(
            'remaining-life-limit',
            this.gridData,
            this.currentUrl.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !==
                -1
                ? this.stateScenarioRemainingLifeLimits
                : this.stateSelectedRemainingLifeLimitLibrary.remainingLifeLimits,
        );

        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    this.setAttributesSelectListItems();
  }

  mounted() {
    this.setAttributesSelectListItems();
  }

  setAttributesSelectListItems() {
    if (hasValue(this.stateNumericAttributes)) {
      this.numericAttributeSelectItems = this.stateNumericAttributes.map((attribute: Attribute) => ({
        text: attribute.name,
        value: attribute.name
      }));
    }
  }

  onShowCreateRemainingLifeLimitLibraryDialog(createExistingLibraryAsNew: boolean) {
    this.createRemainingLifeLimitLibraryDialogData = {
      ...this.createRemainingLifeLimitDialogData,
      showDialog: true,
      remainingLifeLimits: createExistingLibraryAsNew
          ? this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
          : [],
      scenarioId: createExistingLibraryAsNew ? this.selectedScenarioId : this.uuidNIL
    };
  }

  onShowCreateRemainingLifeLimitDialog() {
    this.createRemainingLifeLimitDialogData = {
      showDialog: true,
      numericAttributeSelectItems: this.numericAttributeSelectItems
    };
  }

  onAddRemainingLifeLimit(newRemainingLifeLimit: RemainingLifeLimit) {
    this.createRemainingLifeLimitDialogData = clone(emptyCreateRemainingLifeLimitDialogData);

    if (!isNil(newRemainingLifeLimit)) {
      this.gridData = append(
            newRemainingLifeLimit, this.gridData
        )
    }
  }

  onEditRemainingLifeLimitProperty(remainingLifeLimit: RemainingLifeLimit, property: string, value: any) {
    this.gridData =  update(
          findIndex(propEq('id', remainingLifeLimit.id), this.gridData),
          setItemPropertyValue(property, value, remainingLifeLimit),
          this.gridData
      )
  }

  onShowCriterionLibraryEditorDialog(remainingLifeLimit: RemainingLifeLimit) {
    this.selectedRemainingLifeLimit = remainingLifeLimit;

    let fromScenario = false;
        let criterionForLibrary = false;
        if (
            this.currentUrl.indexOf(ScenarioRoutePaths.RemainingLifeLimit) !==
            -1
        ) {
            fromScenario = true;
        } else {
            criterionForLibrary = true;
        }
    this.criterionLibraryEditorDialogData = {
      showDialog: true,
      libraryId: remainingLifeLimit.criterionLibrary.id,
      isCallFromScenario: fromScenario,
      isCriterionForLibrary: criterionForLibrary,
    };
  }

  onEditRemainingLifeLimitCriterionLibrary(criterionLibrary: CriterionLibrary) {
    this.criterionLibraryEditorDialogData = clone(emptyCriterionLibraryEditorDialogData);

    if (!isNil(criterionLibrary) && this.selectedRemainingLifeLimit.id !== this.uuidNIL) {
      this.selectedRemainingLifeLimitLibrary = {
        ...this.selectedRemainingLifeLimitLibrary,
        remainingLifeLimits: update(
            findIndex(propEq('id', this.selectedRemainingLifeLimit.id), this.selectedRemainingLifeLimitLibrary.remainingLifeLimits),
            {...this.selectedRemainingLifeLimit, criterionLibrary: criterionLibrary},
            this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
        )
      };
    }

    this.selectedRemainingLifeLimit = clone(emptyRemainingLifeLimit);
  }

  onUpsertRemainingLifeLimitLibrary(remainingLifeLimitLibrary: RemainingLifeLimitLibrary) {
    this.createRemainingLifeLimitLibraryDialogData = clone(emptyCreateRemainingLifeLimitLibraryDialogData);

    if (!isNil(remainingLifeLimitLibrary)) {
      var localObject = clone(remainingLifeLimitLibrary);
      localObject.remainingLifeLimits = clone(this.gridData);
      this.upsertRemainingLifeLimitLibraryAction({library: localObject});
    }
  }
  onSaveScenarioRemainingLifeLimit(scenarioId: string) {
        if (!isNil(this.gridData)) {
          var localCopy: RemainingLifeLimit[];
          if(this.hasSelectedRemainingLifeLimitLibrary){
              localCopy = this.gridData.map((lifeLimit: RemainingLifeLimit) =>({
                ...lifeLimit,
                id: getNewGuid(),

              }));
          }
          else{
            localCopy = clone(this.gridData);
          }
            this.upsertScenarioRemainingLifeLimitAction({
              scenarioRemainingLifeLimits: localCopy,
                scenarioId: scenarioId,
            }).then(() => (this.selectItemValue = null));
        }
    }

  onDiscardChanges() {
    this.selectItemValue = null;
    setTimeout(() => {
      if (
                this.currentUrl.indexOf(
                    ScenarioRoutePaths.RemainingLifeLimit,
                ) !== -1
            ) {
                this.gridData = clone(
                    this.stateScenarioRemainingLifeLimits,
                );
            }
    });
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
      this.deleteRemainingLifeLimitLibraryAction({libraryId: this.selectedRemainingLifeLimitLibrary.id});
    }
  }

  disableCrudButton() {
    const dataIsValid: boolean = this.gridData.every(
            (remainingLife: RemainingLifeLimit) => {
                return (
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.value,
                    ) === true &&
                    this.rules['generalRules'].valueIsNotEmpty(
                        remainingLife.attribute,
                    ) === true
                );
            },
        );

        if (this.hasSelectedRemainingLifeLimitLibrary) {
            return !(
                this.rules['generalRules'].valueIsNotEmpty(
                    this.selectedRemainingLifeLimitLibrary.name,
                ) === true &&
                dataIsValid &&
                this.hasUnsavedChanges
            );
        }

        return !(dataIsValid && this.hasUnsavedChanges);

    // if (this.hasSelectedRemainingLifeLimitLibrary) {
    //   const allDataIsValid = this.selectedRemainingLifeLimitLibrary.remainingLifeLimits
    //       .every((rml: RemainingLifeLimit) => {
    //         return this.rules['generalRules'].valueIsNotEmpty(rml.attribute) === true &&
    //             this.rules['generalRules'].valueIsNotEmpty(rml.value) === true;
    //       });

    //   return !(this.rules['generalRules'].valueIsNotEmpty(this.selectedRemainingLifeLimitLibrary.name) === true && allDataIsValid);
    // }

    // return true;
  }
}
</script>
