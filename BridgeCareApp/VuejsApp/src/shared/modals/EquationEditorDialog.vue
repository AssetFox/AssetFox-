<template>
  <v-row>
    <v-dialog max-width="900px" persistent scrollable v-model="showDialogComputed">
      <v-card class="equation-container-card Montserrat-font-family">
        <v-card-title class="ghd-dialog-box-padding-top">
          <v-col cols = "12">
            <v-row justify="space-between" class="text-center" >
              <div style="margin-left: 347px" class="ghd-control-dialog-header">Equation Editor</div>
              <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                X
            </v-btn>
            </v-row>
          </v-col>
        </v-card-title>
         <v-card-text class="equation-content ghd-dialog-box-padding-center">
                <v-row justify="center">
                  <p class="invalid-message" v-if="invalidExpressionMessage !== ''">{{ invalidExpressionMessage }}</p>
                  <p id="EquationEditor-validEquatiionMsg-p" class="valid-message" v-if="validExpressionMessage !== ''">{{ validExpressionMessage }}</p>
                </v-row>
            <v-col cols = "12">
              <v-tabs v-model="selectedTab">
                <v-tab @click="isPiecewise = false">Equation</v-tab>
                <v-tab @click="isPiecewise = true">Piecewise</v-tab>
                <v-tab @click="isPiecewise = true">Time In Rating</v-tab>
              </v-tabs>
            </v-col>
                <v-window>
                    <v-window-item v-if="selectedTab === 0">
                      <div class="equation-container-div">
                            <v-row>
                              <v-col>
                                <v-subheader class="equation-list-subheader">Attributes: Click to add</v-subheader>
                                <div class="attributes-list-container">
                                  <template v-for="(attribute, index) in attributesList" :key="attribute">
                                    <v-list-tile 
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
                              </v-col>
                              <v-col>
                                <v-subheader class="equation-list-subheader">Formulas: Click to add</v-subheader>
                                <div class="formulas-list-container">
                                  <template v-for="(formula, index) in formulasList" :key="formula">
                                    <v-list-tile 
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
                              </v-col>
                            </v-row>
                            <v-row justify="center" style="padding-top: 10px"> 
                              <div class="math-buttons-container">
                                <v-btn @click="onAddValueToExpression('+')" class="math-button add circular-button" icon
                                      size="small">
                                  <span>+</span>
                                </v-btn>
                                <v-btn @click="onAddValueToExpression('-')" class="math-button subtract circular-button" icon
                                size="small">
                                  <span>-</span>
                                </v-btn>
                                <v-btn @click="onAddValueToExpression('*')" class="math-button multiply circular-button" icon
                                size="small">
                                  <span>*</span>
                                </v-btn>
                                <v-btn @click="onAddValueToExpression('/')" class="math-button divide circular-button" icon
                                size="small">
                                  <span>/</span>
                                </v-btn>
                                <v-btn @click="onAddValueToExpression('(')" class="math-button parentheses circular-button" icon
                                size="small">
                                  <span>(</span>
                                </v-btn>
                                <v-btn @click="onAddValueToExpression(')')" class="math-button parentheses circular-button" icon
                                size="small">
                                  <span>)</span>
                                </v-btn>
                              </div>
                            </v-row>

                            
                            <v-row style="padding-left: 15px; padding-top: 11px">
                              <v-textarea :rows="5" @blur="setCursorPosition" @focus="setTextareaCursorPosition"
                                          id="equation_textarea"
                                          ref="textareaInput"
                                          no-resize outline
                                          spellcheck="false"
                                          variant="outlined"
                                          density="compact"
                                          style="max-width: 825px; height: 2.5em"
                                          v-model="expression" class="ghd-text-field-border">
                              </v-textarea>
                            </v-row>
                      </div>
                    </v-window-item>
                    <v-window-item v-if="selectedTab === 1">
                      <div class="equation-container-div">
                        <v-row>
                          <v-col cols = "5" >
                            <div>                 
                              <div class="data-points-grid">
                                <v-data-table :headers="piecewiseGridHeaders" :items="piecewiseGridData" class="v-table__overflow ghd-table" hide-actions
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg">
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                      <tr>
                                        <td v-for="header in piecewiseGridHeaders">
                                          <div v-if="header.key !== ''">
                                            <div @click="onEditDataPoint(props.item, header.key)" justify="space-between" style="margin-left: 10px" class="edit-data-point-span">
                                              {{ props.item[header.key] }}
                                            </div>
                                          </div>
                                          <div v-else>
                                            <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ghd-blue" flat icon v-if="props.item.timeValue !== 0">
                                              <img :src="getUrl('assets/icons/trash-ghd-blue.svg')" />
                                            </v-btn>
                                          </div>
                                        </td>
                                      </tr>
                                    </template>
                                  </v-data-table>
                              </div>
                              <v-row justify="space-between" class="add-addmulti-container">
                                <v-btn @click="onAddTimeAttributeDataPoint"
                                variant = "flat"  class='ghd-blue ghd-button ghd-button-text'>
                                  Add
                                </v-btn>
                                <v-btn @click="showAddMultipleDataPointsPopup = true"
                                variant = "flat" class="ghd-blue ghd-button ghd-button-text">
                                  Add Multi
                                </v-btn>
                              </v-row>
                            </div>
                          </v-col>
                          <v-col cols = "7" >
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
                          </v-col>
                        </v-row>
                      </div>
                    </v-window-item>
                    <v-window-item v-if="selectedTab === 2">
                      <div class="equation-container-div">
                        <v-row>
                          <v-col cols = "5">
                            <div>
                              <div class="data-points-grid">
                                <div>
                                  <v-data-table :headers="timeInRatingGridHeaders" :items="timeInRatingGridData" class="v-table__overflow ghd-table" hide-actions
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg">
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                      <tr>
                                        <td v-for="header in timeInRatingGridHeaders">
                                          <div v-if="header.key !== ''">
                                            <div @click="onEditDataPoint(props.item, header.key)" justify="space-between" style="margin-left: 10px" class="edit-data-point-span">
                                              {{ props.item[header.key] }}
                                            </div>
                                          </div>
                                          <div v-else>
                                            <v-btn @click="onRemoveTimeAttributeDataPoint(props.item.id)" class="ghd-blue" icon v-if="props.item.timeValue !== 0">
                                              <img :src="getUrl('assets/icons/trash-ghd-blue.svg')" />
                                            </v-btn>
                                          </div>
                                        </td>
                                      </tr>
                                    </template>
                                  </v-data-table>
                                </div>
                             </div>
                              <v-row justify="space-between" class="add-addmulti-container">
                                <v-btn @click="onAddTimeAttributeDataPoint"
                                variant = "flat" class='ghd-blue ghd-button ghd-button-text' >
                                  Add
                                </v-btn>
                                <v-btn @click="showAddMultipleDataPointsPopup = true"
                                variant = "flat" class='ghd-blue ghd-button ghd-button-text'>
                                  Add Multi
                                </v-btn>
                              </v-row>
                            </div>
                          </v-col>
                          <v-col cols = "7" >
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
                          </v-col>
                        </v-row>
                      </div>
                    </v-window-item>
                </v-window>

        </v-card-text>
        <v-row justify="center" style="margin-bottom: 5px">
          <v-btn id="EquationEditor-checkEquation-btn" :disabled="disableEquationCheck()" @click="onCheckEquation" density="compact" variant = "flat" class="ghd-blue check-eq ghd-button ghd-button-text">Check Equation</v-btn>
        </v-row>
       <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-row>
            <v-col cols = "12">
                <v-row justify="center">
                  <v-btn @click="onSubmit(false)" variant = "outlined" class='ghd-blue ghd-button ghd-button-text' id="EquationEditorDialog-Cancel-Btn">Cancel</v-btn>
                  <v-btn :disabled="cannotSubmit" @click="onSubmit(true)" variant = "outlined" class='ghd-blue ghd-button ghd-button-text' id="EquationEditorDialog-Save-Btn">Save</v-btn>
                </v-row>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog max-width="250px" persistent v-model="showAddDataPointPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-row column justify-center>
            <div>
              <v-col cols = "12">
                <v-row justify-space-between >
                  <h6 class="header-title">Time Value</h6>
                </v-row>
              </v-col>
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            outline
                            type="number"
                            v-model="newDataPoint.timeValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
            <div>
              <v-col cols = "12">
                <v-row justify-space-between >
                  <h6 class="header-title">Condition Value</h6>
                </v-row>
              </v-col>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" outline
                            type="number" v-model="newDataPoint.conditionValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
          </v-row>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-row justify="center">
            <v-btn @click="onSubmitNewDataPoint(false)" variant = "flat" size="small" class="ghd-blue ghd-button ghd-button-text">Cancel</v-btn>
            <v-btn :disabled="disableNewDataPointSubmit()" @click="onSubmitNewDataPoint(true)"
            variant = "outlined"
            size="small" class="ghd-blue ghd-button ghd-button-text">
              Save
            </v-btn>            
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog max-width="400px" persistent v-model="showAddMultipleDataPointsPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-row column justify-center>
            <p>Data point entries must follow the format <strong>#,#</strong> (time,attribute) with each entry on a
              separate line.</p>
            <v-col cols = "12">
              <v-textarea
                  :rules="[multipleDataPointsFormIsNotEmpty, isCorrectMultipleDataPointsFormat, timeValueIsGreaterThanZero, multipleDataPointsAreNew]"
                  no-resize outline rows="10"
                  v-model="multipleDataPoints" class="ghd-text-field-border">
              </v-textarea>
            </v-col>

          </v-row>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-row justify="center">
            <v-btn @click="onSubmitNewDataPointMulti(false)" variant = "flat" size="small" class="ghd-blue ghd-button ghd-button-text">Cancel
            </v-btn>
            <v-btn :disabled="disableMultipleDataPointsSubmit()" @click="onSubmitNewDataPointMulti(true)"
            variant = "outlined"
            size="small" class="ghd-blue ghd-button ghd-button-text">
              Save
            </v-btn>           
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog max-width="250px" persistent v-model="showEditDataPointPopup">
      <v-card class="Montserrat-font-family">
        <v-card-text class="ghd-dialog-box-padding-top">
          <v-row column justify-center>
            <div>
              <v-col cols = "12">
                <v-row justify-space-between >
                  <h6 class="header-title">Time Value</h6>
                </v-row>
              </v-col>
              <v-text-field :rules="[timeValueIsNotEmpty, timeValueIsGreaterThanZero, timeValueIsNew]"
                            outline
                            type="number"
                            v-model="editedDataPoint.timeValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
            <div>
              <v-col cols = "12">
                <v-row justify-space-between >
                  <h6 class="header-title">Condition Value</h6>
                </v-row>
              </v-col>
              <v-text-field :rules="[conditionValueIsNotEmpty, conditionValueIsNew]" outline
                            type="number" v-model="editedDataPoint.conditionValue" class="ghd-text-field ghd-text-field-border">
              </v-text-field>
            </div>
          </v-row>
        </v-card-text>
        <v-card-actions class="ghd-dialog-box-padding-bottom">
          <v-row justify-center>
            <v-btn @click="onSubmitEditedDataPointValue(false)" variant = "flat" size="small" class="ghd-blue ghd-button-text">Cancel</v-btn>
            <v-btn :disabled="disableEditDataPointSubmit()" @click="onSubmitEditedDataPointValue(true)"
            variant = "outlined"
            size="small" class="ghd-blue ghd-button ghd-button-text">
              Save
            </v-btn>            
          </v-row>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script lang="ts" setup>
import { getUrl } from '@/shared/utils/get-url';
import Vue, {ShallowRef, computed, shallowRef} from 'vue';
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
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

let store = useStore();
const emit = defineEmits(['submit'])
const props = defineProps<{
  dialogData: EquationEditorDialogData,
  isFromPerformanceCurveEditor: Boolean
    }>()
    let showDialogComputed = computed(() => props.dialogData.showDialog);
    
let stateNumericAttributes = computed<Attribute[]>(() => store.state.attributeModule.numericAttributes);
async function getAttributesAction(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}

  let equation: Equation = {...emptyEquation, id: getNewGuid()};
  const attributesList = ref<string[]>([]);  
  const formulasList = ref<string[]>(formulas);
  let expression = shallowRef<string>('');
  const isPiecewise = ref<boolean>(false);
  const textareaInput: any = ref(null);
  let cursorPosition: number = 0;
  const cannotSubmit = ref<boolean>(true);
  const invalidExpressionMessage = ref('');
  const validExpressionMessage = ref('');
  const piecewiseGridHeaders = ref<any[]>([
    {title: 'Time', key: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
    {title: 'Condition', key: 'conditionValue', align: 'left', sortable: false, class: '', width: '10px'},
    {title: 'Action', key: '', align: 'left', sortable: false, class: '', width: '10px'}
  ]);
  const timeInRatingGridHeaders= ref<any[]>([
    {title: 'Condition', key: 'conditionValue', align: 'left', sortable: false, class: '', width: '10px'},
    {title: 'Time', key: 'timeValue', align: 'left', sortable: false, class: '', width: '10px'},
    {title: 'Action', key: '', align: 'left', sortable: false, class: '', width: '10px'}
  ]);
  const piecewiseGridData = ref<TimeConditionDataPoint[]>([]); 
  const timeInRatingGridData = ref<TimeConditionDataPoint[]>([]);
  const showAddDataPointPopup = ref<boolean>(false);
  const newDataPoint = ref<TimeConditionDataPoint>(clone(emptyTimeConditionDataPoint));
  let xAxisMax: number = 0;
  let yAxisMax: number = 0;
  const dataPointsSource = ref<number[][]>([]);
  const showAddMultipleDataPointsPopup = ref<boolean>(false);
  const multipleDataPoints = ref('');;
  const selectedTab = ref<number>(0);
  const showEditDataPointPopup = ref<boolean>(false);
  let editedDataPointProperty: string = '';
  const editedDataPoint = ref<TimeConditionDataPoint>(clone(emptyTimeConditionDataPoint));
  let piecewiseRegex: RegExp = /(\(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*\))/;
  let multipleDataPointsRegex: RegExp = /(\d+(\.{1}\d+)*,\d+(\.{1}\d+)*)/;
  let uuidNIL: string = getBlankGuid();
/**
   * mounted => event handler is used to set the textareaInput object, set the cursorPosition object, and to trigger
   * a function to set the attributesList object.
   */
  onMounted(()=>mounted())
   function mounted() {
    //  cursorPosition = textareaInput.selectionStart; 
    console.log(textareaInput.value) 
    if (hasValue(stateNumericAttributes.value)) {
      setAttributesList();
    }
  }

  /**
   * onStateNumericAttributesChanged => stateNumericAttributes watcher is used to trigger a function to set the
   * attributesList object.
   */
  watch(stateNumericAttributes,()=>onStateNumericAttributesChanged())
  function onStateNumericAttributesChanged() {
    if (hasValue(stateNumericAttributes.value)) {
      setAttributesList();
    }
  }

  /**
   * onDialogDataChanged => dialogData watcher is used to set the equation object, set the expression object, set
   * the isPiecewise object, set the selectedTab object (if expression is piecewise), and trigger a function to set
   * some of the piecewise chart properties (if expression is piecewise).
   */
  watch(()=>props.dialogData,()=>onDialogDataChanged())
  function onDialogDataChanged() {
    equation = {
      id: props.dialogData.equation.id === uuidNIL ? equation.id : props.dialogData.equation.id,
      expression: !isNil(props.dialogData.equation.expression) ? props.dialogData.equation.expression : ''
    };
    expression.value = equation.expression;

    if (piecewiseRegex.test(expression.value)) {
      isPiecewise.value = true;
      selectedTab.value = 1;
      onParsePiecewiseEquation();
    } else {
      isPiecewise.value = false;
    }
  }

  /**
   * onPiecewiseGridDataChanged => piecewiseGridData watcher is used to reset the cannotSubmit, invalidExpressionMessage,
   * and validExpressionMessage objects. It will also set the xAxisMax, yAxisMax, and dataPointsSource objects.
   */
  watch(piecewiseGridData,()=> onPiecewiseGridDataChanged())
  function onPiecewiseGridDataChanged() {
    cannotSubmit.value = true;
    invalidExpressionMessage.value = '';
    validExpressionMessage.value = '';

    let highestTimeValue: number = getLastPropertyValue('timeValue', piecewiseGridData.value);
    if (highestTimeValue % 2 !== 0) {
      highestTimeValue += 1;
    }
    xAxisMax = highestTimeValue;

    let highestConditionValue: number = getLastPropertyValue('conditionValue', piecewiseGridData.value);
    if (highestConditionValue % 2 !== 0) {
      highestConditionValue += 1;
    }
    yAxisMax = highestConditionValue;

    dataPointsSource.value = piecewiseGridData.value.map((dataPoint: TimeConditionDataPoint) =>
        [dataPoint.timeValue, dataPoint.conditionValue]);
  }

  /**
   * onExpressionChanged => expression watcher is used to reset the invalidExpressionMessage and validExpressionMessage
   * objects as well as set the cannotSubmit object.
   */
  watch(expression,()=>{
    invalidExpressionMessage.value = '';
    validExpressionMessage.value = '';
    cannotSubmit.value = !(expression.value === '' && !isPiecewise.value);
  })

  /**
   * onParsePiecewiseEquation => function is used to
   */
  function onParsePiecewiseEquation() {
    let dataPoints: TimeConditionDataPoint[] = [];

    const dataPointStrings: string[] = expression.value.split(piecewiseRegex)
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

    syncDataGridLists(dataPoints);
  }

  /**
   * setAttributesList => function is used to set the attributesList object.
   */
  function setAttributesList() {
    attributesList.value = getPropertyValues('name', stateNumericAttributes.value);
  }

  /**
   * Setter: cursorPosition
   */
   function setCursorPosition() {
    cursorPosition = textareaInput.value.selectionStart;
  }

  /**
   * One of the formula list items in the list of formulas has been clicked
   * @param formula The formula string to add to the expression string
   */
   function onAddFormulaToEquation(formula: string) {
    if (cursorPosition === 0) {
      expression.value = `${formula}${expression.value}`;
      cursorPosition = formula !== 'E' && formula !== 'PI'
          ? formula.indexOf('(') + 1
          : formula.length;
    } else if (cursorPosition === expression.value.length) {
      expression.value = `${expression.value}${formula}`;
      if (formula !== 'E' && formula !== 'PI') {
        let i = expression.value.length;
        while (expression.value.charAt(i) !== '(') {
          i--;
        }
        cursorPosition = i + 1;
      } else {
        cursorPosition = expression.value.length;
      }
    } else {
      const output = `${expression.value.substr(0, cursorPosition)}${formula}`;
      expression.value = `${output}${expression.value.substr(cursorPosition)}`;
      if (formula !== 'E' && formula !== 'PI') {
        let i = output.length;
        while (output.charAt(i) !== '(') {
          i--;
        }
        cursorPosition = i + 1;
      } else {
        cursorPosition = output.length;
      }
    }
    textareaInput.value.focus();
  }

  /**
   * onAddValueToExpression => function is used to add a string value to the expression object using the cursorPosition
   * object's value and then to reset the cursorPosition object's value after modifying the expression object. Finally,
   * the textareaInput object is put into focus.
   */
   function onAddValueToExpression(value: string) {
    if (cursorPosition === 0) {
      cursorPosition = value.length;
      expression.value = `${value}${expression.value}`;
    } else if (cursorPosition === expression.value.length) {
      expression.value = `${expression.value}${value}`;
      cursorPosition = expression.value.length;
    } else {
      const output = `${expression.value.substr(0, cursorPosition)}${value}`;
      expression.value = `${output}${expression.value.substr(cursorPosition)}`;
      cursorPosition = output.length;
    }
    textareaInput.value.focus();
  }

  /**
   * setTextareaCursorPosition => function is used to set the textareaInput object's cursor position.
   */
   function setTextareaCursorPosition() {
    setTimeout(() =>
        textareaInput.value.setSelectionRange(cursorPosition, cursorPosition)
    );
  }

  /**
   * onAddTimeAttributeDataPoint => function is used to set the newDataPoint object's id property with a new uuid
   * and then set the showAddDataPointPopup object to 'true'.
   */
   function onAddTimeAttributeDataPoint() {
    newDataPoint.value = {
      ...newDataPoint.value,
      id: getNewGuid()
    };
    showAddDataPointPopup.value = true;
  }

  /**
   * onSubmitNewDataPoint => function is used to parse the newDataPoint object's timeValue and conditionValue properties
   * and then sync it with the other data point values between the piecewise and time-in-rating data grids/charts.
   */
   function onSubmitNewDataPoint(submit: boolean) {
    showAddDataPointPopup.value = false;

    if (submit) {
      const newParsedDataPoint: TimeConditionDataPoint = {
        ...newDataPoint.value,
        timeValue: parseInt(newDataPoint.value.timeValue.toString()),
        conditionValue: parseFloat(newDataPoint.value.conditionValue.toString())
      };

      const dataPoints: TimeConditionDataPoint[] = selectedTab.value === 1
          ? [...piecewiseGridData.value, newParsedDataPoint]
          : [...timeInRatingGridData.value, newParsedDataPoint];

      syncDataGridLists(dataPoints);
    }

    newDataPoint.value = clone(emptyTimeConditionDataPoint);
  }

  /**
   * syncDataGridLists => function is used to calculate and sync the data points between the piecewise and
   * time-in-rating data grids/charts.
   */
   function syncDataGridLists(dataPoints: TimeConditionDataPoint[]) {
    let piecewiseData: TimeConditionDataPoint[] = [];
    let timeInRatingData: TimeConditionDataPoint[] = [];

    if (selectedTab.value === 1) {
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

    piecewiseGridData.value = piecewiseData;
    timeInRatingGridData.value = timeInRatingData;
  }

  /**
   * onSubmitNewDataPointMulti => function is used to parse the Multiple Data Points Popup's 'submit' result and
   * then to sync the parsed data point values between the piecewise and time-in-rating data grids/charts.
   */
   function onSubmitNewDataPointMulti(submit: boolean) {
    if (submit) {
      const parsedMultiDataPoints: TimeConditionDataPoint[] = parseMultipleDataPoints();

      const dataPoints = selectedTab.value === 1
          ? [...piecewiseGridData.value, ...parsedMultiDataPoints]
          : [...timeInRatingGridData.value, ...parsedMultiDataPoints];

      syncDataGridLists(dataPoints);
    }

    showAddMultipleDataPointsPopup.value = false;
    multipleDataPoints.value = '';
  }

  /**
   * parseMultipleDataPoints => function is used to parse the multipleDataPoints string into a list of
   * TimeConditionDataPoint objects.
   */
   function parseMultipleDataPoints() {
    const splitDataPoints: string[] = multipleDataPoints.value
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
   * onEditDataPoint => function is used to set the objects editedDataPoint, editedDataPointProperty, and
   * showEditDataPointPopup.
   */
   function onEditDataPoint(dataPoint: TimeConditionDataPoint, property: string) {
    editedDataPoint.value = clone(dataPoint);
    editedDataPointProperty = property;
    showEditDataPointPopup.value = true;
  }

  /**
   * onSubmitEditedDataPointValue => function is used to update a data point that was edited via the
   * Edit Data Point Popup and then to sync the changes between the piecewise and time-in-rating data grids/charts.
   */
   function onSubmitEditedDataPointValue(submit: boolean) {
    if (submit) {
      let dataPoints = selectedTab.value === 1 ? clone(piecewiseGridData.value) : clone(timeInRatingGridData.value);
      var timeValue = parseFloat(editedDataPoint.value.timeValue.toString());
      var conditionValue = parseFloat(editedDataPoint.value.conditionValue.toString());
      if (!isNaN(timeValue) && !isNaN(conditionValue)) {
        editedDataPoint.value.timeValue = timeValue;
        editedDataPoint.value.conditionValue = conditionValue;
        dataPoints = update(
          findIndex(propEq('id', editedDataPoint.value.id), dataPoints), editedDataPoint.value, dataPoints
        );

        syncDataGridLists(dataPoints);
      }
    }

    editedDataPoint.value = clone(emptyTimeConditionDataPoint);
    editedDataPointProperty = '';
    showEditDataPointPopup.value = false;
  }

  /**
   * onRemoveTimeAttributeDataPoint => function is used to remove a TimeConditionDataPoint object from either the
   * piecewise data grid/chart list or the time-in-rating data grid/chart list and then to sync the data point values
   * between the piecewise and time-in-rating data grids/charts.
   */
   function onRemoveTimeAttributeDataPoint(id: string) {
    const dataPoints: TimeConditionDataPoint[] = selectedTab.value === 1
        ? piecewiseGridData.value.filter((dataPoint: TimeConditionDataPoint) => dataPoint.id !== id)
        : timeInRatingGridData.value.filter((dataPoint: TimeConditionDataPoint) => dataPoint.id !== id);

    syncDataGridLists(dataPoints);
  }

  function disableEquationCheck() {
        return isPiecewise.value ? !hasValue(onParseTimeAttributeDataPoints()) : !hasValue(expression.value);
    }

  /**
   * onCheckEquation => function is used to trigger a service function to make an HTTP request to the backend API
   * equation validation service in order to validate the current expression.
   */
   function onCheckEquation() {
    const equationValidationParameters: EquationValidationParameters = {
      expression: isPiecewise.value ? onParseTimeAttributeDataPoints() : expression.value,
      isPiecewise: isPiecewise.value,
      currentUserCriteriaFilter: {...emptyUserCriteriaFilter},
      networkId: getBlankGuid()
    };

    ValidationService.getEquationValidationResult(equationValidationParameters)
        .then((response: AxiosResponse) => {
          if (hasValue(response, 'data')) {
            const result: ValidationResult = response.data as ValidationResult;
            if (result.isValid) {
              validExpressionMessage.value = 'Equation is valid.';
              invalidExpressionMessage.value = '';
              cannotSubmit.value = false;
            } else {
              invalidExpressionMessage.value = result.validationMessage;
              validExpressionMessage.value = '';
              cannotSubmit.value = true;
            }
          }
        });
  }

  /**
   * onParseTimeAttributeDataPoints => function is used to parse a list of TimeAttributeDataPoints objects into a
   * string of (x,y) data points.
   */
   function onParseTimeAttributeDataPoints() {
    return piecewiseGridData.value.map((timeAttributeDataPoint: TimeConditionDataPoint) =>
        `(${timeAttributeDataPoint.timeValue},${timeAttributeDataPoint.conditionValue})`
    ).join('');
  }

  /**
   * onSubmit => function is used to emit the modified equation object back to the parent component.
   */
   function onSubmit(submit: boolean) {
    resetComponentCalculatedProperties();

    if (submit) {
      equation.expression = isPiecewise.value ? onParseTimeAttributeDataPoints() : expression.value;
      emit('submit', equation);
    } else {
      emit('submit', null);
    }

    piecewiseGridData.value = [];
    timeInRatingGridData.value = [];
    selectedTab.value = 0;
    equation = {...emptyEquation, id: getNewGuid()};
  }

  /**
   * resetComponentCalculatedProperties => function is used to reset the cursorPosition, invalidExpressionMessage,
   * and validExpressionMessage objects.
   */
   function resetComponentCalculatedProperties() {
    cursorPosition = 0;
    invalidExpressionMessage.value = '';
    validExpressionMessage.value = '';
  }

  /**
   * disableNewDataPointSubmit => function is used to disable the New Data Point Popup's 'submit' button if the data
   * point value is not valid.
   */
   function disableNewDataPointSubmit() {
    return timeValueIsNotEmpty(newDataPoint.value.timeValue.toString()) !== true ||
        timeValueIsGreaterThanZero(newDataPoint.value.timeValue.toString()) !== true ||
        timeValueIsNew(newDataPoint.value.timeValue.toString()) !== true ||
        conditionValueIsNotEmpty(newDataPoint.value.conditionValue.toString()) !== true ||
        conditionValueIsNew(newDataPoint.value.conditionValue.toString()) !== true;
  }

  /**
   * disableMultipleDataPointsSubmit => function is used to disable the Multiple Data Points Popup's 'submit' button
   * if the multiple data points' values are not valid.
   */
   function disableMultipleDataPointsSubmit() {
    return multipleDataPoints.value === '' ||
        isCorrectMultipleDataPointsFormat() !== true ||
        multipleDataPointsAreNew() !== true;
  }

  /**
   * disableEditDataPointSubmit => function is used to disable the Edit Data Point Popup's 'submit' button if the
   * data point's modified value is not valid.
   */
   function disableEditDataPointSubmit() {

      return (timeValueIsNotEmpty(editedDataPoint.value.timeValue.toString()) !== true ||
          timeValueIsGreaterThanZero(editedDataPoint.value.timeValue.toString()) !== true ||
          timeValueIsNew(editedDataPoint.value.timeValue.toString()) !== true) &&
       (conditionValueIsNotEmpty(editedDataPoint.value.conditionValue.toString()) !== true ||
          conditionValueIsNew(editedDataPoint.value.conditionValue.toString()) !== true);
    
  }

  /**
   * Rule: Checks if a given time value is > 0
   * @param value
   */
   function timeValueIsGreaterThanZero(value: string) {
    return parseInt(value) > 0 || 'Time values cannot be less than or equal to 0';
  }

  /**
   * Rule: Checks if a given time value is new
   * @param value
   */
   function timeValueIsNew(value: string) {
    if (selectedTab.value === 1) {
      const timeValues: number[] = getPropertyValues('timeValue', piecewiseGridData.value);

      return timeValues.indexOf(parseInt(value)) === -1 || 'Time value already exists';
    }

    return true;
  }

  /**
   * Rule: Checks if a given time value is not empty
   * @param value
   */
   function timeValueIsNotEmpty(value: string) {
    return hasValue(value) || 'A value must be entered';
  }

  /**
   * Rule: Checks if a given condition value is new
   * @param value
   */
   function conditionValueIsNew(value: string) {
    const conditionValues: number[] = selectedTab.value === 1
        ? getPropertyValues('conditionValue', piecewiseGridData.value)
        : getPropertyValues('conditionValue', timeInRatingGridData.value);

    return conditionValues.indexOf(parseFloat(value)) === -1 || 'Condition value already exists';
  }

  /**
   * Rule: Checks if a given condition value is not empty
   * @param value
   */
   function conditionValueIsNotEmpty(value: string) {
    return hasValue(value) || 'A value must be entered';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea is not empty
   */
   function multipleDataPointsFormIsNotEmpty() {
    return multipleDataPoints.value !== '' || 'Values must be entered';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea has correctly formatted data
   */
   function isCorrectMultipleDataPointsFormat() {
    const eachDataPointIsValid = multipleDataPoints.value
        .split(/\r?\n/).filter((dataPoints: string) => dataPoints !== '')
        .every((dataPoints: string) => {
          return multipleDataPointsRegex.test(dataPoints) &&
              dataPoints.split(',').every((value: string) => !isNaN(parseFloat(value)));
        });

    return eachDataPointIsValid || 'Incorrect format';
  }

  /**
   * Rule: Checks if the multiple data points popup's textarea data has all new values for times & conditions
   */
   function multipleDataPointsAreNew() {
    const dataPoints: TimeConditionDataPoint[] = parseMultipleDataPoints();
    const existingConditionValues: number[] = [];
    const existingTimeValues: number[] = [];

    const eachDataPointIsNew = dataPoints.every((dataPoint: TimeConditionDataPoint) => {
      const conditionEditorValueIsNew = conditionValueIsNew(dataPoint.conditionValue.toString()) === true;
      const timeEditorValueIsNew: boolean = timeValueIsNew(dataPoint.timeValue.toString()) === true;

      if (!conditionValueIsNew) {
        existingConditionValues.push(dataPoint.conditionValue);
      }

      if (selectedTab.value === 1 && !timeValueIsNew) {
        existingTimeValues.push(dataPoint.timeValue);
      }

      return selectedTab.value === 1
          ? conditionEditorValueIsNew && timeEditorValueIsNew
          : conditionEditorValueIsNew;
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
  padding-left: 100px;
  word-break: break-all; 
  word-wrap: break-word;
  z-index: 2!important;
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
