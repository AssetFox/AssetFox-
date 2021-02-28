<template>
  <v-layout>
    <v-dialog max-width="900px" persistent scrollable v-model="dialogData.showDialog">
      <v-card class="equation-container-card">
        <v-card-title>
          <v-layout justify-center>
            <h3>Equation Editor</h3>
          </v-layout>
        </v-card-title>
        <v-card-text>
          <v-layout column>
            <v-flex xs12>
              <div class="validation-message-div">
                <v-layout justify-center>
                  <p class="invalid-message" v-if="invalidExpressionMessage !== ''">{{ invalidExpressionMessage }}</p>
                  <p class="valid-message" v-if="validExpressionMessage !== ''">{{ validExpressionMessage }}</p>
                </v-layout>
              </div>
            </v-flex>
            <v-flex xs12>
              <v-tabs v-model="selectedTab">
                <v-tab :key="0" @click="isPiecewise = false">Equation</v-tab>
                <v-tab :key="1" @click="isPiecewise = true">Piecewise</v-tab>
                <v-tab :key="2" @click="isPiecewise = true">Time In Rating</v-tab>
                <v-tab-item>
                  <div class="equation-container-div">
                    <v-layout column>
                      <div>
                        <v-layout justify-space-between row>
                          <div>
                            <v-list>
                              <template>
                                <v-subheader>Attributes: Click to add</v-subheader>
                                <div class="attributes-list-container">
                                  <v-list-tile :key="attribute"
                                               @click="onAddValueToExpression(`[${attribute}]`)" class="list-tile"
                                               ripple
                                               v-for="attribute in attributesList">
                                    <v-list-tile-content>
                                      <v-list-tile-title>{{ attribute }}
                                      </v-list-tile-title>
                                    </v-list-tile-content>
                                  </v-list-tile>
                                </div>
                              </template>
                            </v-list>
                          </div>
                          <div>
                            <v-list>
                              <template>
                                <v-subheader>Formulas: Click to add</v-subheader>
                                <div class="formulas-list-container">
                                  <v-list-tile :key="formula"
                                               @click="onAddFormulaToEquation(formula)" class="list-tile"
                                               ripple
                                               v-for="formula in formulasList">
                                    <v-list-tile-content>
                                      <v-list-tile-title>{{ formula }}
                                      </v-list-tile-title>
                                    </v-list-tile-content>
                                  </v-list-tile>
                                </div>
                              </template>
                            </v-list>
                          </div>
                        </v-layout>
                      </div>
                      <div>
                        <v-layout justify-center>
                          <div class="math-buttons-container">
                            <v-layout justify-space-between row>
                              <v-btn @click="onAddValueToExpression('+')" class="math-button add" fab
                                     small>
                                <span>+</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('-')" class="math-button subtract" fab
                                     small>
                                <span>-</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('*')" class="math-button multiply" fab
                                     small>
                                <span>*</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('/')" class="math-button divide" fab
                                     small>
                                <span>/</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('(')" class="math-button parentheses" fab
                                     small>
                                <span>(</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression(')')" class="math-button parentheses" fab
                                     small>
                                <span>)</span>
                              </v-btn>
                            </v-layout>
                          </div>
                        </v-layout>
                      </div>
                      <div>
                        <v-layout justify-center>
                          <v-textarea :rows="5" @blur="setCursorPosition" @focus="setTextareaCursorPosition" full-width
                                      id="equation_textarea"
                                      no-resize outline
                                      spellcheck="false"
                                      v-model="expression">
                          </v-textarea>
                        </v-layout>
                      </div>
                    </v-layout>
                  </div>
                </v-tab-item>
                <v-tab-item>
                  <div class="equation-container-div">
                    <v-layout row>
                      <v-flex xs4>
                        <div>
                          <v-layout justify-space-between row>
                            <v-btn @click="onAddTimeAttributeDataPoint"
                                   class="ara-blue-bg white--text">
                              Add
                            </v-btn>
                            <v-btn @click="showAddMultipleDataPointsPopup = true"
                                   class="ara-blue-bg white--text">
                              Add Multi
                            </v-btn>
                          </v-layout>
                          <div class="data-points-grid">
                            <v-data-table :headers="piecewiseGridHeaders"
                                          :items="piecewiseGridData"
                                          class="elevation-1 v-table__overflow"
                                          hide-actions>
                              <template slot="items" slot-scope="props">
                                <td v-for="header in piecewiseGridHeaders">
                                  <div v-if="header.value !== ''">
                                    <div v-if="props.item.timeValue === 0">
                                      {{ props.item[header.value] }}
                                    </div>
                                    <div @click="onEditDataPoint(props.item, header.value)" class="edit-data-point-span"
                                         v-else>
                                      {{ props.item[header.value] }}
                                    </div>
                                  </div>
                                  <div v-else>
                                    <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ara-orange"
                                           icon
                                           v-if="props.item.timeValue !== 0">
                                      <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                  </div>
                                </td>
                              </template>
                            </v-data-table>
                          </div>
                        </div>
                      </v-flex>
                      <v-flex xs8>
                        <div class="kendo-chart-container">
                          <kendo-chart :data-source="piecewiseGridData"
                                       :pannable-lock="'y'"
                                       :series-defaults-style="'smooth'"
                                       :series-defaults-type="'scatterLine'"
                                       :theme="'sass'"
                                       :tooltip-format="'({0},{1})'"
                                       :tooltip-visible="true"
                                       :x-axis-max="xAxisMax"
                                       :x-axis-min="0"
                                       :x-axis-title-text="'Time'"
                                       :y-axis-max="yAxisMax"
                                       :y-axis-min="0"
                                       :y-axis-title-text="'Condition'"
                                       :zoomable-mousewheel-lock="'y'"
                                       :zoomable-selection-lock="'y'">
                            <kendo-chart-series-item :data="dataPointsSource"
                                                     :markers-visible="false">
                            </kendo-chart-series-item>
                          </kendo-chart>
                        </div>
                      </v-flex>
                    </v-layout>
                  </div>
                </v-tab-item>
                <v-tab-item>
                  <div class="equation-container-div">
                    <v-layout row>
                      <v-flex xs4>
                        <div>
                          <v-layout justify-space-between row>
                            <v-btn @click="onAddTimeAttributeDataPoint"
                                   class="ara-blue-bg white--text">
                              Add
                            </v-btn>
                            <v-btn @click="showAddMultipleDataPointsPopup = true"
                                   class="ara-blue-bg white--text">
                              Add Multi
                            </v-btn>
                          </v-layout>
                          <div class="data-points-grid">
                            <v-data-table :headers="timeInRatingGridHeaders"
                                          :items="timeInRatingGridData"
                                          class="elevation-1 v-table__overflow"
                                          hide-actions>
                              <template slot="items" slot-scope="props">
                                <td v-for="header in timeInRatingGridHeaders">
                                  <div v-if="header.value !== ''">
                                    <div @click="onEditDataPoint(props.item, header.value)"
                                         class="edit-data-point-span">
                                      {{ props.item[header.value] }}
                                    </div>
                                  </div>
                                  <div v-else>
                                    <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ara-orange"
                                           icon>
                                      <v-icon>fas fa-trash</v-icon>
                                    </v-btn>
                                  </div>
                                </td>
                              </template>
                            </v-data-table>
                          </div>
                        </div>
                      </v-flex>
                      <v-flex xs8>
                        <div class="kendo-chart-container">
                          <kendo-chart :data-source="piecewiseGridData"
                                       :pannable-lock="'y'"
                                       :series-defaults-style="'smooth'"
                                       :series-defaults-type="'scatterLine'"
                                       :theme="'sass'"
                                       :tooltip-format="'({0},{1})'"
                                       :tooltip-visible="true"
                                       :x-axis-max="xAxisMax"
                                       :x-axis-min="0"
                                       :x-axis-title-text="'Time'"
                                       :y-axis-max="yAxisMax"
                                       :y-axis-min="0"
                                       :y-axis-title-text="'Condition'"
                                       :zoomable-mousewheel-lock="'y'"
                                       :zoomable-selection-lock="'y'">
                            <kendo-chart-series-item :data="dataPointsSource"
                                                     :markers-visible="false">
                            </kendo-chart-series-item>
                          </kendo-chart>
                        </div>
                      </v-flex>
                    </v-layout>
                  </div>
                </v-tab-item>
              </v-tabs>
            </v-flex>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout>
            <v-flex xs12>
              <div>
                <v-layout justify-space-between row>
                  <v-btn @click="onCheckEquation" class="ara-blue-bg white--text">Check</v-btn>
                  <v-btn :disabled="cannotSubmit" @click="onSubmit(true)"
                         class="ara-blue-bg white--text">Save
                  </v-btn>
                  <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
              </div>
            </v-flex>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="250px" persistent v-model="showAddDataPointPopup">
      <v-card>
        <v-card-text>
          <v-layout column justify-center>
            <div>
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            label="Time Value"
                            outline
                            type="number"
                            v-model="newDataPoint.timeValue">
              </v-text-field>
            </div>
            <div>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" label="Condition Value" outline
                            type="number" v-model="newDataPoint.conditionValue">
              </v-text-field>
            </div>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="disableNewDataPointSubmit()" @click="onSubmitNewDataPoint(true)"
                   class="ara-blue-bg white--text">
              Save
            </v-btn>
            <v-btn @click="onSubmitNewDataPoint(false)" class="ara-orange-bg white--text">Cancel</v-btn>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="200px" persistent v-model="showAddMultipleDataPointsPopup">
      <v-card>
        <v-card-text>
          <v-layout column justify-center>
            <p>Data point entries must follow the format <span
                class="format-span"><strong>#,#</strong></span> (time,attribute) with each entry on a
              separate line.</p>
            <v-flex xs2>
              <v-textarea
                  :rules="[multipleDataPointsFormIsNotEmpty, isCorrectMultipleDataPointsFormat, timeValueIsGreaterThanZero, multipleDataPointsAreNew]"
                  no-resize outline rows="20"
                  v-model="multipleDataPoints">
              </v-textarea>
            </v-flex>

          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="disableMultipleDataPointsSubmit()" @click="onSubmitNewDataPointMulti(true)"
                   class="ara-blue-bg white--text">
              Save
            </v-btn>
            <v-btn @click="onSubmitNewDataPointMulti(false)" class="ara-orange-bg white--text">Cancel
            </v-btn>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="250px" persistent v-model="showEditDataPointPopup">
      <v-card>
        <v-card-text>
          <v-layout justify-center>
            <div v-if="editedDataPointProperty === 'timeValue'">
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            label="Time Value"
                            outline
                            type="number"
                            v-model="editedDataPoint.timeValue">
              </v-text-field>
            </div>
            <div v-else>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" label="Attribute Value" outline
                            type="number" v-model="editedDataPoint.conditionValue">
              </v-text-field>
            </div>
          </v-layout>
        </v-card-text>
        <v-card-actions>
          <v-layout justify-space-between row>
            <v-btn :disabled="disableEditDataPointSubmit()" @click="onSubmitEditedDataPointValue(true)"
                   class="ara-blue-bg white--text">
              Save
            </v-btn>
            <v-btn @click="onSubmitEditedDataPointValue(false)" class="ara-orange-bg white--text">Cancel
            </v-btn>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import {Component, Prop, Watch} from 'vue-property-decorator';
import {Action, State} from 'vuex-class';
import {EquationEditorDialogData} from '@/shared/models/modals/equation-editor-dialog-data';
import EquationService from '@/services/equation.service';
import {formulas} from '@/shared/utils/formulas';
import {AxiosResponse} from 'axios';
import {getLastPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {emptyEquation, Equation, EquationValidationResult} from '@/shared/models/iAM/equation';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {emptyTimeConditionDataPoint, TimeConditionDataPoint} from '@/shared/models/iAM/time-condition-data-point';
import {add, clone, findIndex, insert, isEmpty, propEq, reverse, update, isNil} from 'ramda';
import {sortByProperty} from '@/shared/utils/sorter-utils';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class EquationEditorDialog extends Vue {
  @Prop() dialogData: EquationEditorDialogData;

  @State(state => state.attribute.numericAttributes) stateNumericAttributes: Attribute[];

  @Action('getAttributes') getAttributesAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;

  equation: Equation = {...emptyEquation, id: getNewGuid()};
  attributesList: string[] = [];
  formulasList: string[] = formulas;
  expression: string = '';
  isPiecewise: boolean = false;
  textareaInput: HTMLTextAreaElement = {} as HTMLTextAreaElement;
  cursorPosition: number = 0;
  cannotSubmit: boolean = true;
  invalidExpressionMessage: string = '';
  validExpressionMessage: string = '';
  piecewiseGridHeaders: DataTableHeader[] = [
    {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: 'Condition', value: 'conditionValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
  ];
  timeInRatingGridHeaders: DataTableHeader[] = [
    {text: 'Condition', value: 'conditionValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: '', value: '', align: 'left', sortable: false, class: '', width: '10px'}
  ];
  piecewiseGridData: TimeConditionDataPoint[] = [];
  timeInRatingGridData: TimeConditionDataPoint[] = [];
  showAddDataPointPopup: boolean = false;
  newDataPoint: TimeConditionDataPoint = clone(emptyTimeConditionDataPoint);
  xAxisMax: number = 0;
  yAxisMax: number = 0;
  dataPointsSource: number[][] = [];
  showAddMultipleDataPointsPopup: boolean = false;
  multipleDataPoints: string = '';
  selectedTab: number = 0;
  showEditDataPointPopup: boolean = false;
  editedDataPointProperty: string = '';
  editedDataPoint: TimeConditionDataPoint = clone(emptyTimeConditionDataPoint);
  piecewiseRegex: RegExp = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))/;
  multipleDataPointsRegex: RegExp = /(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*)/
  uuidNIL: string = getBlankGuid();

  /**
   * mounted => This event handler is used to set the textareaInput object, set the cursorPosition object, and to trigger
   * a function to set the attributesList object.
   */
  mounted() {
    this.textareaInput = document.getElementById('equation_textarea') as HTMLTextAreaElement;
    this.cursorPosition = this.textareaInput.selectionStart;
    if (hasValue(this.stateNumericAttributes)) {
      this.setAttributesList();
    }
  }

  /**
   * onStateNumericAttributesChanged => This stateNumericAttributes watcher is used to trigger a function to set the
   * attributesList object.
   */
  @Watch('stateNumericAttributes')
  onStateNumericAttributesChanged() {
    if (hasValue(this.stateNumericAttributes)) {
      this.setAttributesList();
    }
  }

  /**
   * onDialogDataChanged => This dialogData watcher is used to set the equation object, set the expression object, set
   * the isPiecewise object, set the selectedTab object (if expression is piecewise), and trigger a function to set
   * some of the piecewise chart properties (if expression is piecewise).
   */
  @Watch('dialogData')
  onDialogDataChanged() {
    this.equation = {
      id: this.dialogData.equation.id === this.uuidNIL ? this.equation.id : this.dialogData.equation.id,
      expression: !isNil(this.dialogData.equation.expression) ? this.dialogData.equation.expression : ''
    };
    this.expression = this.equation.expression;

    if (this.piecewiseRegex.test(this.expression)) {
      this.isPiecewise = true;
      this.selectedTab = 1;
      this.onParsePiecewiseEquation();
    } else {
      this.isPiecewise = false;
    }
  }

  /**
   * onPiecewiseGridDataChanged => This piecewiseGridData watcher is used to reset the cannotSubmit, invalidExpressionMessage,
   * and validExpressionMessage objects. It will also set the xAxisMax, yAxisMax, and dataPointsSource objects.
   */
  @Watch('piecewiseGridData')
  onPiecewiseGridDataChanged() {
    this.cannotSubmit = true;
    this.invalidExpressionMessage = '';
    this.validExpressionMessage = '';

    let highestTimeValue: number = getLastPropertyValue('timeValue', this.piecewiseGridData);
    if (highestTimeValue % 2 !== 0) {
      highestTimeValue += 1;
    }
    this.xAxisMax = highestTimeValue;

    let highestConditionValue: number = getLastPropertyValue('conditionValue', this.piecewiseGridData);
    if (highestConditionValue % 2 !== 0) {
      highestConditionValue += 1;
    }
    this.yAxisMax = highestConditionValue;

    this.dataPointsSource = this.piecewiseGridData.map((dataPoint: TimeConditionDataPoint) =>
        [dataPoint.timeValue, dataPoint.conditionValue]);
  }

  /**
   * onExpressionChanged => This expression watcher is used to reset the invalidExpressionMessage and validExpressionMessage
   * objects as well as set the cannotSubmit object.
   */
  @Watch('expression')
  onExpressionChanged() {
    this.invalidExpressionMessage = '';
    this.validExpressionMessage = '';
    this.cannotSubmit = !(this.expression === '' && !this.isPiecewise);
  }

  /**
   * onParsePiecewiseEquation => This function is used to
   */
  onParsePiecewiseEquation() {
    let dataPoints: TimeConditionDataPoint[] = [];

    const dataPointStrings: string[] = this.expression.split(this.piecewiseRegex)
        .filter((dataPoint: string) => hasValue(dataPoint) && dataPoint.indexOf(',') !== -1);

    dataPointStrings.forEach((dataPoint: string) => {
      const splitDataPoint = dataPoint
          .replace('(', '')
          .replace(')', '')
          .split(',');

      dataPoints.push({
        id: getNewGuid(),
        timeValue: parseInt(splitDataPoint[0]),
        conditionValue: parseFloat(splitDataPoint[1])
      });
    });

    dataPoints = sortByProperty('timeValue', dataPoints);

    this.syncDataGridLists(dataPoints);
  }

  /**
   * setAttributesList => This function is used to set the attributesList object.
   */
  setAttributesList() {
    this.attributesList = getPropertyValues('name', this.stateNumericAttributes);
  }

  /**
   * Setter: cursorPosition
   */
  setCursorPosition() {
    this.cursorPosition = this.textareaInput.selectionStart;
  }

  /**
   * One of the formula list items in the list of formulas has been clicked
   * @param formula The formula string to add to the expression string
   */
  onAddFormulaToEquation(formula: string) {
    if (this.cursorPosition === 0) {
      this.expression = `${formula}${this.expression}`;
      this.cursorPosition = formula !== 'E' && formula !== 'PI'
          ? formula.indexOf('(') + 1
          : formula.length;
    } else if (this.cursorPosition === this.expression.length) {
      this.expression = `${this.expression}${formula}`;
      if (formula !== 'E' && formula !== 'PI') {
        let i = this.expression.length;
        while (this.expression.charAt(i) !== '(') {
          i--;
        }
        this.cursorPosition = i + 1;
      } else {
        this.cursorPosition = this.expression.length;
      }
    } else {
      const output = `${this.expression.substr(0, this.cursorPosition)}${formula}`;
      this.expression = `${output}${this.expression.substr(this.cursorPosition)}`;
      if (formula !== 'E' && formula !== 'PI') {
        let i = output.length;
        while (output.charAt(i) !== '(') {
          i--;
        }
        this.cursorPosition = i + 1;
      } else {
        this.cursorPosition = output.length;
      }
    }
    this.textareaInput.focus();
  }

  /**
   * onAddValueToExpression => This function is used to add a string value to the expression object using the cursorPosition
   * object's value and then to reset the cursorPosition object's value after modifying the expression object. Finally,
   * the textareaInput object is put into focus.
   */
  onAddValueToExpression(value: string) {
    if (this.cursorPosition === 0) {
      this.cursorPosition = value.length;
      this.expression = `${value}${this.expression}`;
    } else if (this.cursorPosition === this.expression.length) {
      this.expression = `${this.expression}${value}`;
      this.cursorPosition = this.expression.length;
    } else {
      const output = `${this.expression.substr(0, this.cursorPosition)}${value}`;
      this.expression = `${output}${this.expression.substr(this.cursorPosition)}`;
      this.cursorPosition = output.length;
    }
    this.textareaInput.focus();
  }

  /**
   * setTextareaCursorPosition => This function is used to set the textareaInput object's cursor position.
   */
  setTextareaCursorPosition() {
    setTimeout(() =>
        this.textareaInput.setSelectionRange(this.cursorPosition, this.cursorPosition)
    );
  }

  /**
   * onAddTimeAttributeDataPoint => This function is used to set the newDataPoint object's id property with a new uuid
   * and then set the showAddDataPointPopup object to 'true'.
   */
  onAddTimeAttributeDataPoint() {
    this.newDataPoint = {
      ...this.newDataPoint,
      id: getNewGuid()
    };
    this.showAddDataPointPopup = true;
  }

  /**
   * onSubmitNewDataPoint => This function is used to parse the newDataPoint object's timeValue and conditionValue properties
   * and then sync it with the other data point values between the piecewise and time-in-rating data grids/charts.
   */
  onSubmitNewDataPoint(submit: boolean) {
    this.showAddDataPointPopup = false;

    if (submit) {
      const newParsedDataPoint: TimeConditionDataPoint = {
        ...this.newDataPoint,
        timeValue: parseInt(this.newDataPoint.timeValue.toString()),
        conditionValue: parseFloat(this.newDataPoint.conditionValue.toString())
      };

      const dataPoints: TimeConditionDataPoint[] = this.selectedTab === 1
          ? [...this.piecewiseGridData, newParsedDataPoint]
          : [...this.timeInRatingGridData, newParsedDataPoint];

      this.syncDataGridLists(dataPoints);
    }

    this.newDataPoint = clone(emptyTimeConditionDataPoint);
  }

  /**
   * syncDataGridLists => This function is used to calculate and sync the data points between the piecewise and
   * time-in-rating data grids/charts.
   */
  syncDataGridLists(dataPoints: TimeConditionDataPoint[]) {
    let piecewiseData: TimeConditionDataPoint[] = [];
    let timeInRatingData: TimeConditionDataPoint[] = [];

    if (this.selectedTab === 1) {
      piecewiseData = sortByProperty('timeValue', dataPoints)
          .filter((dataPoint: TimeConditionDataPoint) => dataPoint.timeValue !== 0);

      piecewiseData.forEach((dataPoint: TimeConditionDataPoint, index: number) => {
        timeInRatingData.push({
          ...dataPoint,
          timeValue: index === 0
              ? piecewiseData[index].timeValue
              : Math.abs(piecewiseData[index - 1].timeValue - piecewiseData[index].timeValue)
        });
      });

      timeInRatingData = reverse(sortByProperty('conditionValue', timeInRatingData));

      if (hasValue(timeInRatingData)) {
        const n1: TimeConditionDataPoint = {
          id: getNewGuid(),
          timeValue: 0,
          conditionValue: Math.trunc(add(1, timeInRatingData[0].conditionValue))
        };
        piecewiseData = insert(0, n1, piecewiseData);
      }
    } else {
      timeInRatingData = reverse(sortByProperty('conditionValue', dataPoints));

      let cumulativeTimeValue: number = 0;
      timeInRatingData.forEach((dataPoint: TimeConditionDataPoint) => {
        const timeValue: number = add(cumulativeTimeValue, dataPoint.timeValue);
        cumulativeTimeValue = timeValue;

        piecewiseData.push({
          ...dataPoint,
          timeValue: timeValue
        });
      });

      piecewiseData = sortByProperty('timeValue', piecewiseData);

      if (hasValue(timeInRatingData)) {
        const n1: TimeConditionDataPoint = {
          id: getNewGuid(),
          timeValue: 0,
          conditionValue: Math.trunc(add(1, timeInRatingData[0].conditionValue))
        };
        piecewiseData = insert(0, n1, piecewiseData);
      }
    }

    this.piecewiseGridData = piecewiseData;
    this.timeInRatingGridData = timeInRatingData;
  }

  /**
   * onSubmitNewDataPointMulti => This function is used to parse the Multiple Data Points Popup's 'submit' result and
   * then to sync the parsed data point values between the piecewise and time-in-rating data grids/charts.
   */
  onSubmitNewDataPointMulti(submit: boolean) {
    if (submit) {
      const parsedMultiDataPoints: TimeConditionDataPoint[] = this.parseMultipleDataPoints();

      const dataPoints = this.selectedTab === 1
          ? [...this.piecewiseGridData, ...parsedMultiDataPoints]
          : [...this.timeInRatingGridData, ...parsedMultiDataPoints];

      this.syncDataGridLists(dataPoints);
    }

    this.showAddMultipleDataPointsPopup = false;
    this.multipleDataPoints = '';
  }

  /**
   * parseMultipleDataPoints => This function is used to parse the multipleDataPoints string into a list of
   * TimeConditionDataPoint objects.
   */
  parseMultipleDataPoints() {
    const splitDataPoints: string[] = this.multipleDataPoints
        .split(/\r?\n/).filter((dataPoints: string) => dataPoints !== '');

    if (hasValue(splitDataPoints)) {
      const dataPoints: TimeConditionDataPoint[] = splitDataPoints.map((dataPoints: string) => {
        const splitValues: string[] = dataPoints.split(',');

        return {
          id: getNewGuid(),
          timeValue: parseInt(splitValues[0]),
          conditionValue: parseFloat(splitValues[1])
        };
      });

      return dataPoints;
    }

    return [];
  }

  /**
   * onEditDataPoint => This function is used to set the objects editedDataPoint, editedDataPointProperty, and
   * showEditDataPointPopup.
   */
  onEditDataPoint(dataPoint: TimeConditionDataPoint, property: string) {
    this.editedDataPoint = clone(dataPoint);
    this.editedDataPointProperty = property;
    this.showEditDataPointPopup = true;
  }

  /**
   * onSubmitEditedDataPointValue => This function is used to update a data point that was edited via the
   * Edit Data Point Popup and then to sync the changes between the piecewise and time-in-rating data grids/charts.
   */
  onSubmitEditedDataPointValue(submit: boolean) {
    if (submit) {
      let dataPoints = this.selectedTab === 1 ? clone(this.piecewiseGridData) : clone(this.timeInRatingGridData);
      dataPoints = update(
          findIndex(propEq('id', this.editedDataPoint.id), dataPoints), this.editedDataPoint, dataPoints
      );

      this.syncDataGridLists(dataPoints);
    }

    this.editedDataPoint = clone(emptyTimeConditionDataPoint);
    this.editedDataPointProperty = '';
    this.showEditDataPointPopup = false;
  }

  /**
   * onRemoveTimeAttributeDataPoint => This function is used to remove a TimeConditionDataPoint object from either the
   * piecewise data grid/chart list or the time-in-rating data grid/chart list and then to sync the data point values
   * between the piecewise and time-in-rating data grids/charts.
   */
  onRemoveTimeAttributeDataPoint(id: string) {
    const dataPoints: TimeConditionDataPoint[] = this.selectedTab === 1
        ? this.piecewiseGridData.filter((dataPoint: TimeConditionDataPoint) => dataPoint.id !== id)
        : this.timeInRatingGridData.filter((dataPoint: TimeConditionDataPoint) => dataPoint.id !== id);

    this.syncDataGridLists(dataPoints);
  }

  /**
   * onCheckEquation => This function is used to trigger a service function to make an HTTP request to the backend API
   * equation validation service in order to validate the current expression.
   */
  onCheckEquation() {
    //TODO : replace with actual Equation DTO when .net core API is setup
    const equation: any = {
      equation: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.expression,
      isFunction: !this.isPiecewise,
      isPiecewise: this.isPiecewise
    };

    EquationService.checkEquationValidity(equation)
        .then((response: AxiosResponse<EquationValidationResult>) => {
          if (hasValue(response, 'data')) {
            const validationResult: EquationValidationResult = response.data;
            if (validationResult.isValid) {
              this.validExpressionMessage = 'Equation is valid.';
              this.invalidExpressionMessage = '';
              this.cannotSubmit = false;
            } else {
              this.invalidExpressionMessage = validationResult.message;
              this.validExpressionMessage = '';
              this.cannotSubmit = true;
            }
          }
        });
  }

  /**
   * onParseTimeAttributeDataPoints => This function is used to parse a list of TimeAttributeDataPoints objects into a
   * string of (x,y) data points.
   */
  onParseTimeAttributeDataPoints() {
    return this.piecewiseGridData.map((timeAttributeDataPoint: TimeConditionDataPoint) =>
        `(${timeAttributeDataPoint.timeValue},${timeAttributeDataPoint.conditionValue})`
    ).join('');
  }

  /**
   * onSubmit => This function is used to emit the modified equation object back to the parent component.
   */
  onSubmit(submit: boolean) {
    this.resetComponentCalculatedProperties();

    if (submit) {
      /*const result: EquationEditorDialogResult = {
        equation: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.expression,
        isPiecewise: this.isPiecewise,
        isFunction: false
      };*/
      this.equation.expression = this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.expression;
      this.$emit('submit', this.equation);
    } else {
      this.$emit('submit', null);
    }

    this.piecewiseGridData = [];
    this.timeInRatingGridData = [];
    this.selectedTab = 0;
    this.equation = {...emptyEquation, id: getNewGuid()};
  }

  /**
   * resetComponentCalculatedProperties => This function is used to reset the cursorPosition, invalidExpressionMessage,
   * and validExpressionMessage objects.
   */
  resetComponentCalculatedProperties() {
    this.cursorPosition = 0;
    this.invalidExpressionMessage = '';
    this.validExpressionMessage = '';
  }

  /**
   * disableNewDataPointSubmit => This function is used to disable the New Data Point Popup's 'submit' button if the data
   * point value is not valid.
   */
  disableNewDataPointSubmit() {
    return this.timeValueIsNotEmpty(this.newDataPoint.timeValue.toString()) !== true ||
        this.timeValueIsGreaterThanZero(this.newDataPoint.timeValue.toString()) !== true ||
        this.timeValueIsNew(this.newDataPoint.timeValue.toString()) !== true ||
        this.conditionValueIsNotEmpty(this.newDataPoint.conditionValue.toString()) !== true ||
        this.conditionValueIsNew(this.newDataPoint.conditionValue.toString()) !== true;
  }

  /**
   * disableMultipleDataPointsSubmit => This function is used to disable the Multiple Data Points Popup's 'submit' button
   * if the multiple data points' values are not valid.
   */
  disableMultipleDataPointsSubmit() {
    return this.multipleDataPoints === '' ||
        this.isCorrectMultipleDataPointsFormat() !== true ||
        this.multipleDataPointsAreNew() !== true;
  }

  /**
   * disableEditDataPointSubmit => This function is used to disable the Edit Data Point Popup's 'submit' button if the
   * data point's modified value is not valid.
   */
  disableEditDataPointSubmit() {
    if (this.editedDataPointProperty === 'timeValue') {
      return this.timeValueIsNotEmpty(this.editedDataPoint.timeValue.toString()) !== true ||
          this.timeValueIsGreaterThanZero(this.editedDataPoint.timeValue.toString()) !== true ||
          this.timeValueIsNew(this.editedDataPoint.timeValue.toString()) !== true;
    } else {
      return this.conditionValueIsNotEmpty(this.editedDataPoint.conditionValue.toString()) !== true ||
          this.conditionValueIsNew(this.editedDataPoint.conditionValue.toString()) !== true;
    }
  }

  /**
   * Rule: Checks if a given time value is > 0
   * @param value
   */
  timeValueIsGreaterThanZero(value: string) {
    return parseInt(value) > 0 || 'Time values cannot be less than or equal to 0';
  }

  /**
   * Rule: Checks if a given time value is new
   * @param value
   */
  timeValueIsNew(value: string) {
    if (this.selectedTab === 1) {
      const timeValues: number[] = getPropertyValues('timeValue', this.piecewiseGridData);

      return timeValues.indexOf(parseInt(value)) === -1 || 'Time value already exists';
    }

    return true;
  }

  /**
   * Rule: Checks if a given time value is not empty
   * @param value
   */
  timeValueIsNotEmpty(value: string) {
    return hasValue(value) || 'A value must be entered';
  }

  /**
   * Rule: Checks if a given condition value is new
   * @param value
   */
  conditionValueIsNew(value: string) {
    const conditionValues: number[] = this.selectedTab === 1
        ? getPropertyValues('conditionValue', this.piecewiseGridData)
        : getPropertyValues('conditionValue', this.timeInRatingGridData);

    return conditionValues.indexOf(parseFloat(value)) === -1 || 'Condition value already exists';
  }

  /**
   * Rule: Checks if a given condition value is not empty
   * @param value
   */
  conditionValueIsNotEmpty(value: string) {
    return hasValue(value) || 'A value must be entered';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea is not empty
   */
  multipleDataPointsFormIsNotEmpty() {
    return this.multipleDataPoints !== '' || 'Values must be entered';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea has correctly formatted data
   */
  isCorrectMultipleDataPointsFormat() {
    const eachDataPointIsValid = this.multipleDataPoints
        .split(/\r?\n/).filter((dataPoints: string) => dataPoints !== '')
        .every((dataPoints: string) => {
          return this.multipleDataPointsRegex.test(dataPoints) &&
              dataPoints.split(',').every((value: string) => !isNaN(parseFloat(value)));
        });

    return eachDataPointIsValid || 'Incorrect format';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea data has all new values for times & conditions
   */
  multipleDataPointsAreNew() {
    const dataPoints: TimeConditionDataPoint[] = this.parseMultipleDataPoints();
    const existingConditionValues: number[] = [];
    const existingTimeValues: number[] = [];

    const eachDataPointIsNew = dataPoints.every((dataPoint: TimeConditionDataPoint) => {
      const conditionValueIsNew = this.conditionValueIsNew(dataPoint.conditionValue.toString()) === true;
      const timeValueIsNew: boolean = this.timeValueIsNew(dataPoint.timeValue.toString()) === true;

      if (!conditionValueIsNew) {
        existingConditionValues.push(dataPoint.conditionValue);
      }

      if (this.selectedTab === 1 && !timeValueIsNew) {
        existingTimeValues.push(dataPoint.timeValue);
      }

      return this.selectedTab === 1
          ? conditionValueIsNew && timeValueIsNew
          : conditionValueIsNew;
    });

    let conditionValuesAlreadyExistsMessage: string = '';
    if (!isEmpty(existingConditionValues)) {
      conditionValuesAlreadyExistsMessage = 'The following condition values already exist: ';

      existingConditionValues.forEach((value: number, index: number) => {
        conditionValuesAlreadyExistsMessage += index > 0 ? `, ${value}` : `${value}`;
      });
    }

    let timeValuesAlreadyExistsMessage: string = '';
    if (!isEmpty(existingTimeValues)) {
      timeValuesAlreadyExistsMessage = 'The following time values already exist: ';

      existingTimeValues.forEach((value: number, index: number) => {
        timeValuesAlreadyExistsMessage += index > 0 ? `, ${value}` : `${value}`;
      });
    }

    return eachDataPointIsNew || `${conditionValuesAlreadyExistsMessage}\n${timeValuesAlreadyExistsMessage}`;
  }
}
</script>

<style>
.equation-container-card {
  height: 750px;
  overflow-y: auto;
  overflow-x: hidden;
}

.validation-message-div {
  height: 21px;
}

.invalid-message {
  color: red;
}

.attributes-list-container, .formulas-list-container {
  width: 205px;
  height: 250px;
  overflow: auto;
}

.list-tile {
  cursor: pointer;
}

.math-button {
  border: 1px solid black;
  font-size: 1.5em;
}

.parentheses {
  font-size: 1.25em;
}

.add, .divide {
  font-size: 1.5em;
}

.multiply {
  font-size: 1.75em;
}

.subtract {
  font-size: 2em;
}

.valid-message {
  color: green;
}

.data-points-grid {
  width: 300px;
  height: 308px;
  overflow: auto;
}

.rows-per-page-select .v-input__slot {
  width: 30%;
}

.equation-container-div {
  height: 505px;
}

.format-span {
  color: red;
}

.edit-data-point-span {
  cursor: pointer;
}
</style>
