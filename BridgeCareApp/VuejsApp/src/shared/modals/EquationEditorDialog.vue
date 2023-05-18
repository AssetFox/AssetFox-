<template>
  <v-layout>
    <v-dialog max-width="900px" persistent scrollable v-model="dialogData.showDialog">
      <v-card class="equation-container-card Montserrat-font-family">
        <v-card-title class="ghd-dialog-box-padding-top">
          <v-flex xs12>
            <v-layout justify-space-between >
              <div class="ghd-control-dialog-header">Equation Editor</div>
              <v-btn @click="onSubmit(false)" flat class="ghd-close-button">
                X
            </v-btn>
            </v-layout>
          </v-flex>
        </v-card-title>
        <v-card-text class="equation-content ghd-dialog-box-padding-center">
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
                <v-tab :key="1" @click="isPiecewise = true" :hidden="!isFromPerformanceCurveEditor">Piecewise</v-tab>
                <v-tab :key="2" @click="isPiecewise = true" :hidden="!isFromPerformanceCurveEditor">Time In Rating</v-tab>
                <v-tab-item>
                  <div class="equation-container-div">
                    <v-layout column>
                      <div>
                        <v-layout justify-space-between row>
                          <div>
                            <v-list>
                              <template>
                                <v-subheader class="equation-list-subheader">Attributes: Click to add</v-subheader>
                                <div class="attributes-list-container">
                                  <template v-for="(attribute, index) in attributesList">
                                    <v-list-tile :key="attribute"
                                                @click="onAddValueToExpression(`[${attribute}]`)" class="list-tile"
                                                ripple>
                                      <v-list-tile-content>
                                        <v-list-tile-title>{{ attribute }}
                                        </v-list-tile-title>
                                      </v-list-tile-content>
                                    </v-list-tile>
                                     <v-divider v-if="index + 1 < attributesList.length" :key="`divider-${index}`"></v-divider>
                                  </template>
                                </div>
                              </template>
                            </v-list>
                          </div>
                          <div>
                            <v-list>
                              <template>
                                <v-subheader class="equation-list-subheader">Formulas: Click to add</v-subheader>
                                <div class="formulas-list-container">
                                  <template v-for="(formula, index) in formulasList">
                                    <v-list-tile :key="formula"
                                                @click="onAddFormulaToEquation(formula)" class="list-tile"
                                                ripple
                                                >
                                      <v-list-tile-content>
                                        <v-list-tile-title>{{ formula }}
                                        </v-list-tile-title>
                                      </v-list-tile-content>
                                    </v-list-tile>
                                    <v-divider v-if="index + 1 < formulasList.length" :key="`divider-${index}`"></v-divider>
                                  </template>
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
                              <v-btn @click="onAddValueToExpression('+')" class="math-button add circular-button" fab
                                     small>
                                <span>+</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('-')" class="math-button subtract circular-button" fab
                                     small>
                                <span>-</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('*')" class="math-button multiply circular-button" fab
                                     small>
                                <span>*</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('/')" class="math-button divide circular-button" fab
                                     small>
                                <span>/</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression('(')" class="math-button parentheses circular-button" fab
                                     small>
                                <span>(</span>
                              </v-btn>
                              <v-btn @click="onAddValueToExpression(')')" class="math-button parentheses circular-button" fab
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
                                      v-model="expression" class="ghd-text-field-border">
                          </v-textarea>
                        </v-layout>
                      </div>
                    </v-layout>
                  </div>
                </v-tab-item>
                <v-tab-item>
                  <div class="equation-container-div">
                    <v-layout>
                      <v-flex xs5 >
                        <div>                 
                          <div class="data-points-grid">
                            <v-data-table :headers="piecewiseGridHeaders"
                                          :items="piecewiseGridData"
                                          sort-icon=$vuetify.icons.ghd-table-sort
                                          class="v-table__overflow ghd-table"
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
                                    <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ghd-blue"
                                           icon
                                           v-if="props.item.timeValue !== 0">
                                      <img :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                  </div>
                                </td>
                              </template>
                            </v-data-table>
                          </div>
                          <v-layout justify-space-between class="add-addmulti-container">
                            <v-btn @click="onAddTimeAttributeDataPoint"
                                    flat  class='ghd-blue ghd-button ghd-button-text'>
                              Add
                            </v-btn>
                            <v-btn @click="showAddMultipleDataPointsPopup = true"
                                    flat class="ghd-blue ghd-button ghd-button-text">
                              Add Multi
                            </v-btn>
                          </v-layout>
                        </div>
                      </v-flex>
                      <v-flex xs7 >
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
                    <v-layout>
                      <v-flex xs5>
                        <div>
                          <div class="data-points-grid">
                            <v-data-table :headers="timeInRatingGridHeaders"
                                          :items="timeInRatingGridData"
                                          sort-icon=$vuetify.icons.ghd-table-sort
                                          class="v-table__overflow ghd-table"
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
                                    <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ghd-blue"
                                           icon>
                                      <img :src="require('@/assets/icons/trash-ghd-blue.svg')"/>
                                    </v-btn>
                                  </div>
                                </td>
                              </template>
                            </v-data-table>
                          </div>
                          <v-layout justify-space-between class="add-addmulti-container">
                            <v-btn @click="onAddTimeAttributeDataPoint"
                                    flat class='ghd-blue ghd-button ghd-button-text' >
                              Add
                            </v-btn>
                            <v-btn @click="showAddMultipleDataPointsPopup = true"
                                    flat class='ghd-blue ghd-button ghd-button-text'>
                              Add Multi
                            </v-btn>
                          </v-layout>
                        </div>
                      </v-flex>
                      <v-flex xs7 >
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
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-layout>
            <v-flex xs12>
              <div>
                 <v-layout justify-center row>
                  <v-btn :disabled="disableEquationCheck()" @click="onCheckEquation" flat class="ghd-blue check-eq ghd-button ghd-button-text">Check Equation</v-btn>
                </v-layout>
                <v-layout justify-center row>
                  <v-btn @click="onSubmit(false)" outline class='ghd-blue ghd-button ghd-button-text' id="EquationEditorDialog-Cancel-Btn">Cancel</v-btn>
                  <v-btn :disabled="cannotSubmit" @click="onSubmit(true)"
                         class="white--text ghd-blue ghd-button ghd-button-text">Save
                  </v-btn>                  
                </v-layout>
              </div>
            </v-flex>
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="250px" persistent v-model="showAddDataPointPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-layout column justify-center>
            <div>
              <v-flex xs12>
                <v-layout justify-space-between >
                  <h6 class="header-title">Time Value</h6>
                </v-layout>
              </v-flex>
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            outline
                            type="number"
                            v-model="newDataPoint.timeValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
            <div>
              <v-flex xs12>
                <v-layout justify-space-between >
                  <h6 class="header-title">Condition Value</h6>
                </v-layout>
              </v-flex>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" outline
                            type="number" v-model="newDataPoint.conditionValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
          </v-layout>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-layout justify-center row>
            <v-btn @click="onSubmitNewDataPoint(false)" flat small class="ghd-blue ghd-button ghd-button-text">Cancel</v-btn>
            <v-btn :disabled="disableNewDataPointSubmit()" @click="onSubmitNewDataPoint(true)"
                   outline
                   small class="ghd-blue ghd-button ghd-button-text">
              Save
            </v-btn>            
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="400px" persistent v-model="showAddMultipleDataPointsPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-layout column justify-center>
            <p>Data point entries must follow the format <strong>#,#</strong> (time,attribute) with each entry on a
              separate line.</p>
            <v-flex xs2>
              <v-textarea
                  :rules="[multipleDataPointsFormIsNotEmpty, isCorrectMultipleDataPointsFormat, timeValueIsGreaterThanZero, multipleDataPointsAreNew]"
                  no-resize outline rows="10"
                  v-model="multipleDataPoints" class="ghd-text-field-border">
              </v-textarea>
            </v-flex>

          </v-layout>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-layout justify-center row>
            <v-btn @click="onSubmitNewDataPointMulti(false)" flat small class="ghd-blue ghd-button ghd-button-text">Cancel
            </v-btn>
            <v-btn :disabled="disableMultipleDataPointsSubmit()" @click="onSubmitNewDataPointMulti(true)"
                   outline
                   small class="ghd-blue ghd-button ghd-button-text">
              Save
            </v-btn>           
          </v-layout>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog max-width="250px" persistent v-model="showEditDataPointPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-layout column justify-center>
            <div>
              <v-flex xs12>
                <v-layout justify-space-between >
                  <h6 class="header-title">Time Value</h6>
                </v-layout>
              </v-flex>
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            outline
                            type="number"
                            v-model="editedDataPoint.timeValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
            <div>
              <v-flex xs12>
                <v-layout justify-space-between >
                  <h6 class="header-title">Condition Value</h6>
                </v-layout>
              </v-flex>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" outline
                            type="number" v-model="editedDataPoint.conditionValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
          </v-layout>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-layout justify-center row>
            <v-btn @click="onSubmitEditedDataPointValue(false)" flat small class="ghd-blue ghd-button-text">Cancel</v-btn>
            <v-btn :disabled="disableEditDataPointSubmit()" @click="onSubmitEditedDataPointValue(true)"
                   outline
                   small class="ghd-blue ghd-button ghd-button-text">
              Save
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
import {formulas} from '@/shared/utils/formulas';
import {AxiosResponse} from 'axios';
import {getLastPropertyValue, getPropertyValues} from '@/shared/utils/getter-utils';
import {Attribute} from '@/shared/models/iAM/attribute';
import {hasValue} from '@/shared/utils/has-value-util';
import {emptyEquation, Equation} from '@/shared/models/iAM/equation';
import {DataTableHeader} from '@/shared/models/vue/data-table-header';
import {emptyTimeConditionDataPoint, TimeConditionDataPoint} from '@/shared/models/iAM/time-condition-data-point';
import {add, clone, findIndex, insert, isEmpty, isNil, propEq, reverse, update} from 'ramda';
import {sortByProperty} from '@/shared/utils/sorter-utils';
import {getBlankGuid, getNewGuid} from '@/shared/utils/uuid-utils';
import ValidationService from '@/services/validation.service';
import {EquationValidationParameters, ValidationResult} from '@/shared/models/iAM/expression-validation';
import { emptyUserCriteriaFilter } from '../models/iAM/user-criteria-filter';

@Component
export default class EquationEditorDialog extends Vue {
    @Prop() dialogData: EquationEditorDialogData;
    @Prop() isFromPerformanceCurveEditor: Boolean;

    @State(state => state.attributeModule.numericAttributes) stateNumericAttributes: Attribute[];

    @Action('getAttributes') getAttributesAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

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
    {text: 'Action', value: '', align: 'left', sortable: false, class: '', width: '10px'}
  ];
  timeInRatingGridHeaders: DataTableHeader[] = [
    {text: 'Condition', value: 'conditionValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: 'Time', value: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
    {text: 'Action', value: '', align: 'left', sortable: false, class: '', width: '10px'}
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
  multipleDataPointsRegex: RegExp = /(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*)/;
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
      var timeValue = parseFloat(this.editedDataPoint.timeValue.toString());
      var conditionValue = parseFloat(this.editedDataPoint.conditionValue.toString());
      if (!isNaN(timeValue) && !isNaN(conditionValue)) {
        this.editedDataPoint.timeValue = timeValue;
        this.editedDataPoint.conditionValue = conditionValue;
        dataPoints = update(
          findIndex(propEq('id', this.editedDataPoint.id), dataPoints), this.editedDataPoint, dataPoints
        );

        this.syncDataGridLists(dataPoints);
      }
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

    disableEquationCheck() {
        return this.isPiecewise ? !hasValue(this.onParseTimeAttributeDataPoints()) : !hasValue(this.expression);
    }

  /**
   * onCheckEquation => This function is used to trigger a service function to make an HTTP request to the backend API
   * equation validation service in order to validate the current expression.
   */
  onCheckEquation() {
    const equationValidationParameters: EquationValidationParameters = {
      expression: this.isPiecewise ? this.onParseTimeAttributeDataPoints() : this.expression,
      isPiecewise: this.isPiecewise,
      currentUserCriteriaFilter: {...emptyUserCriteriaFilter},
      networkId: getBlankGuid()
    };

    ValidationService.getEquationValidationResult(equationValidationParameters)
        .then((response: AxiosResponse) => {
          if (hasValue(response, 'data')) {
            const result: ValidationResult = response.data as ValidationResult;
            if (result.isValid) {
              this.validExpressionMessage = 'Equation is valid.';
              this.invalidExpressionMessage = '';
              this.cannotSubmit = false;
            } else {
              this.invalidExpressionMessage = result.validationMessage;
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

      return (this.timeValueIsNotEmpty(this.editedDataPoint.timeValue.toString()) !== true ||
          this.timeValueIsGreaterThanZero(this.editedDataPoint.timeValue.toString()) !== true ||
          this.timeValueIsNew(this.editedDataPoint.timeValue.toString()) !== true) &&
       (this.conditionValueIsNotEmpty(this.editedDataPoint.conditionValue.toString()) !== true ||
          this.conditionValueIsNew(this.editedDataPoint.conditionValue.toString()) !== true);
    
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
  width: 400px;
  height: 250px;
  overflow: auto;
  border: thin solid rgba(0,0,0,.12);
  border-radius: 5px;
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

.add-addmulti-container{
  width:300px;
}

.equation-list-subheader{
  padding-left: 0 !important;
  padding-top: 25px !important;
}

.attributes-list-container .v-divider, .formulas-list-container .v-divider{
  width: 90%;
  position: relative;
  left: 5%;
  margin-top: 5px;
  margin-bottom: 5px;
}

.circular-button{
  border-radius: 50% !important;
  height: 30px !important;
  width: 30px !important;
  font-size: 1.25em !important;
}

.header-cancel{
  padding-top: 8px;
}

.header-title{
  padding-top: 12px;
}

.check-eq{
  margin-bottom: 5px !important;
}

.equation-content{
  overflow: hidden !important;
}

</style>
