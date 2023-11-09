<template>
    <v-row column class="Montserrat-font-family">
        <v-col cols="12">
            <v-col cols ="8" class="ghd-constant-header">
                <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label" style="margin-bottom:1%; margin-right:1%;">Attribute</v-subheader>
                </v-row>
                <v-row>
                    <v-row column>
                        <v-select :items='selectAttributeItems'
                            item-title="text"
                            item-value="value"
                            id="Attributes-selectAttribute-vselect"
                            variant="outlined"
                            style="margin-left:1%;"
                            append-icon="@/assets/icons/down.svg"
                            v-model='selectAttributeItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-row>
                    <v-btn id="Attributes-addAttribute-vbtn" @click="addAttribute" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                        Add Attribute
                    </v-btn>
                </v-row>
            </v-col>
        </v-col>
        <v-divider style="background-color: #798899 !important;" v-if="hasSelectedAttribute"/>
        <v-col cols="12" v-if="hasSelectedAttribute" class="ghd-constant-header" >
            <v-row>
                <v-col cols="2"> 
                    <v-subheader class="ghd-md-gray ghd-control-label" style="margin-left:2%;">Attribute</v-subheader>
                    <v-text-field id="Attributes-attributeName-vtextfield" outline class="ghd-text-field-border ghd-text-field"
                         v-model='selectedAttribute.name'/>
                </v-col>
                <v-col cols="2">
                    <v-subheader class="ghd-md-gray ghd-control-label">Data Type</v-subheader>
                    <v-select
                          item-title="text"
                            item-value="value"
                        id="Attributes-attributeDataType-vselect"
                        variant="outlined"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='typeSelectValues'
                        v-model='selectedAttribute.type'>
                    </v-select>                           
                </v-col>
                <v-col cols="4">
                    <v-subheader class="ghd-md-gray ghd-control-label">
                        Aggregation Rule
                    </v-subheader>
                    <v-select
                            item-title="text"
                            item-value="value"
                        id="Attributes-attributeAggregationRule-vselect"
                        variant="outlined"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='aggregationRuleSelectValues'
                        v-model='selectedAttribute.aggregationRuleType'>
                    </v-select>                           
                </v-col>
            </v-row>
        </v-col>
        <v-col cols="12" v-if="hasSelectedAttribute">
            <v-col cols="10">
                <v-row>
                    <v-col cols="2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Default Value</v-subheader>
                        <v-text-field id="Attributes-attributeDefaultString-vtextfield" v-if="selectedAttribute.type == 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            v-model='selectedAttribute.defaultValue'/>
                        <v-text-field id="Attributes-attributeDefaultNumber-vtextfield" v-if="selectedAttribute.type != 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            v-model='selectedAttribute.defaultValue'
                            />
                    </v-col>
                    <v-col cols="2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Minimum Value</v-subheader>
                        <v-text-field id="Attributes-attributeMinimumValue-vtextfield" outline class="ghd-text-field-border ghd-text-field"                            
                            v-model.number='selectedAttribute.minimum'
                            />
                    </v-col>
                    <v-col cols="2">
                        <v-subheader class="ghd-md-gray ghd-control-label">Maximum Value</v-subheader>
                        <v-text-field id="Attributes-attributeMaximumValue-vtextfield" outline class="ghd-text-field-border ghd-text-field"
                            v-model.number='selectedAttribute.maximum'
                            />
                    </v-col>
                    <v-col cols="4" style="padding-top:50px;">
                        <v-row>
                        <v-switch id="Attributes-attributeCalculated-vswitch" class='sharing header-text-content'  label='Calculated' 
                            v-model='selectedAttribute.isCalculated'/>
                        <v-switch id="Attributes-attributeAscending-vswitch" class='sharing header-text-content'  label='Ascending' 
                            v-model='selectedAttribute.isAscending'/>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-col>
        <!-- Data source combobox -->
        <v-col cols="12" v-if="hasSelectedAttribute">
            <v-col cols="6">
                <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label">Data Source</v-subheader>
                </v-row>
                <v-row column>
                    <v-select
                            item-title="text"
                            item-value="value"
                        id="Attributes-attributeDataSource-vselect"
                        variant="outlined"
                        v-model='selectDatasourceItemValue'
                        :items='selectDatasourceItems'                     
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>                           
                </v-row>
            </v-col>
        </v-col>
        <!-- Command text area -->
        <v-col cols="12" v-if="hasSelectedAttribute && selectedAttribute.dataSource.type == 'SQL'">
            <v-row justify-center>
                <v-col >
                    <v-subheader class="ghd-subheader ">Command</v-subheader>
                    <v-textarea no-resize outline rows='4' class="ghd-text-field-border" v-model='selectedAttribute.command'>
                    </v-textarea>
                    <v-subheader
                        v-if="validationErrorMessage != ''" class="ghd-subheader "
                        style="top: -24px; position: relative; color: red">
                        {{validationErrorMessage}}
                    </v-subheader>
                    <v-subheader
                        v-if="ValidationSuccessMessage != ''" class="ghd-subheader "
                        style="top: -24px; position: relative; color: green">
                        {{ValidationSuccessMessage}}
                    </v-subheader>
                </v-col>
            </v-row>
        </v-col>
        <!-- Data source combobox -->
        <v-col cols="12" v-if="hasSelectedAttribute && selectedAttribute.dataSource.type == 'Excel'">
            <v-row>
                <v-subheader class="ghd-md-gray ghd-control-label" style="margin-left:1%; margin-bottom:0.75%;">Column Name</v-subheader>
            </v-row>
            <v-col cols="6">
                <v-row column>
                    <v-select
                        id="Attributes-attributeColumnName-vselect"
                        variant="outlined"
                        item-title="text"
                        item-value="value"
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='selectExcelColumns'
                        v-model='selectedAttribute.command'>
                    </v-select>                           
                </v-row>
            </v-col>
        </v-col>
        <!-- The Buttons  -->
        <v-col cols="12" v-if="hasSelectedAttribute">        
            <v-row justify-center>
                <v-btn id="Attributes-cancel-vbtn" :disabled='!hasUnsavedChanges' @click='onDiscardChanges' variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'>
                    Cancel
                </v-btn>  
                <v-btn
                variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'
                    @click="CheckSqlCommand">
                    Test
                </v-btn>
                <v-btn id="Attributes-save-vbtn" @click='saveAttribute'  variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'>
                    Save
                </v-btn>               
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang='ts'>
import AttributeService from '@/services/attribute.service';
import { Attribute, emptyAttribute, RuleDefinition } from '@/shared/models/iAM/attribute';
import { Datasource, emptyDatasource, RawDataColumns, noneDatasource } from '@/shared/models/iAM/data-source';
import { ValidationResult } from '@/shared/models/iAM/expression-validation';
import { SelectItem } from '@/shared/models/vue/select-item';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { hasValue } from '@/shared/utils/has-value-util';
import { InputValidationRules, rules as validationRules } from '@/shared/utils/input-validation-rules';
import { TestStringData } from '@/shared/models/iAM/test-string';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { AxiosResponse } from 'axios';
import { any, clone, find, isNil, propEq } from 'ramda';
import Vue, { computed, onBeforeMount, onBeforeUnmount, Ref, ref, shallowRef, ShallowRef, watch } from 'vue';
import { Console } from 'console';
import { useStore } from 'vuex';

    let store = useStore();
    let hasSelectedAttribute = ref<boolean>(false);
    let hasEmptyDataSource: boolean = true;
    let selectAttributeItemValue = ref<string | null>(null);
    let selectDatasourceItemValue = ref<string | null>(null);
    let selectAggregationRuleTypeItems: SelectItem[] = [];
    let selectExcelColumns = ref<SelectItem[]>([]);
    let selectedAttribute = ref<Attribute>(clone(emptyAttribute));
    let selectedDataSource: Datasource | undefined = clone(emptyDatasource);
    let rules: InputValidationRules = validationRules;
    let validationErrorMessage = ref<string>('');
    let ValidationSuccessMessage = ref<string>('');
    let commandIsValid = ref<boolean>(true);
    let checkedCommand = ref<string>('');

    let aggregationRuleSelectValues = ref<SelectItem[]>([]);    
    let typeSelectValues = ref<SelectItem[]>([
        {text: 'STRING', value: 'STRING'},
        {text: 'NUMBER', value: 'NUMBER'}
    ]);

    let stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes) ;
    let stateDataSources = computed<Datasource[]>(() => store.state.datasourceModule.dataSources) ;    
    let stateAggregationRules = computed<RuleDefinition[]>(() => store.state.attributeModule.aggregationRules) ;
    let stateAggregationRulesForType = computed<string[]>(() => store.state.attributeModule.aggregationRulesForType) ;
    let stateAttributeDataSourceTypes = computed<string[]>(() => store.state.attributeModule.attributeDataSourceTypes) ;
    let excelColumns = computed<RawDataColumns>(() => store.state.datasourceModule.excelColumns) ;
    let stateSelectedAttribute = computed<Attribute>(() => store.state.attributeModule.selectedAttribute) ;
    let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges) ;
    let hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess) ;
    let selectAttributeItems = ref<SelectItem[]>([]);
    let selectDatasourceItems: SelectItem[] = [];
    
    async function logOutAction(payload?: any): Promise<any> {await store.dispatch('logOut', payload);}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes', payload);}
    async function getDataSourcesAction(payload?: any): Promise<any> {await store.dispatch('getDataSources', payload);}
    async function getAttributeAggregationRulesAction(payload?: any): Promise<any> {await store.dispatch('getAttributeAggregationRules', payload);}
    async function getAggregationRulesForTypeAction(payload?: any): Promise<any> {await store.dispatch('getAggregationRulesForType', payload);}
    async function getAttributeDataSourceTypes(payload?: any): Promise<any> {await store.dispatch('getAttributeDataSourceTypes', payload);}
    async function getExcelSpreadsheetColumnHeadersAction(payload?: any): Promise<any> {await store.dispatch('getExcelSpreadsheetColumnHeaders', payload);}
    async function selectAttributeAction(payload?: any): Promise<any> {await store.dispatch('selectAttribute', payload);}
    async function upsertAttributeAction(payload?: any): Promise<any> {await store.dispatch('upsertAttribute', payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges', payload);}
    async function getUserNameByIdGetter(payload?: any): Promise<any> {await store.dispatch('getUserNameById', payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}

    created()
    function created() {
        getAttributes();
        getAttributeAggregationRulesAction();
        getAttributeDataSourceTypes();
        getDataSourcesAction();
    }

    onBeforeUnmount(() => beforeDestroy)
    function beforeDestroy() {
        setHasUnsavedChangesAction({ value: false });
    }

    watch(stateAttributes, () =>  {

            let temp = clone(stateAttributes.value)
            temp.forEach(_ => {
                selectAttributeItems.value.push({text: _.name, value: _.id})
            })
    })

    watch(stateDataSources, () => {
        selectDatasourceItems = stateDataSources.value.map((datasource: Datasource) => ({
            text: datasource.name + ' (' + datasource.type + ')',
            value: datasource.id,
        }));
    })

    watch(selectAttributeItemValue, () =>  {
        selectAttributeAction(selectAttributeItemValue.value);
        hasSelectedAttribute.value = true;
        checkedCommand.value = "";
        commandIsValid.value = false;

        getAggregationRulesForTypeAction(selectedAttribute.value.type)
        let temp = stateAggregationRulesForType.value.map((rule: string) => ({
            text: rule,
            value: rule,
        }));

        temp.forEach(_ => {
            aggregationRuleSelectValues.value.push(_)
        })
    })
    
    watch(selectedAttribute, () =>  {
       getAggregationRulesForTypeAction(selectedAttribute.value.type)

        aggregationRuleSelectValues.value = stateAggregationRulesForType.value.map((rule: string) => ({
            text: rule,
            value: rule,
        }));
    })

    watch(selectDatasourceItemValue, () => {
        if (any(propEq('id', selectDatasourceItemValue.value), stateDataSources.value)) {
            let ds = find(
                propEq('id', selectDatasourceItemValue.value),
                stateDataSources.value,
            )
            if(!isNil(ds))
            {
                selectedAttribute.value.dataSource = ds
                if(selectedAttribute.value.dataSource.type === "Excel"){
                    getExcelSpreadsheetColumnHeadersAction(selectedAttribute.value.dataSource.id)
                } 
            }
            else
                selectedAttribute.value.dataSource = clone(emptyDatasource)
        } else {
            selectedAttribute.value.dataSource = clone(emptyDatasource)
        }
    })

    watch(excelColumns, () => {

        let temp = clone(excelColumns.value.columnHeaders);

        temp.forEach(_ =>{
            selectExcelColumns.value.push({text: _, value: _});
        });

        //selectExcelColumns.value = excelColumns.value.columnHeaders.map((header: string) => ({
          //  text: header,
           // value: header,
       // }));
    })

    watch(stateSelectedAttribute, () =>  {
        selectedAttribute.value = clone(stateSelectedAttribute.value);
        if(isNil(selectedAttribute.value.dataSource)) {
            selectedAttribute.value.dataSource = clone(emptyDatasource);
        }
        selectDatasourceItemValue.value = selectedAttribute.value.dataSource.id;
    })

    watch(selectedAttribute, () => {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', selectedAttribute.value, stateSelectedAttribute.value);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    })

    function addAttribute()
    {
        selectAttributeItemValue.value = getBlankGuid()
    }

    function onDiscardChanges() {
        selectedAttribute.value = clone(stateSelectedAttribute.value);
    }

    function saveAttribute(){
        let isInsert = false;
        if(selectedAttribute.value.id === getBlankGuid()){
            selectedAttribute.value.id = getNewGuid();
            isInsert = true;
        }
            
        upsertAttributeAction(selectedAttribute.value)
        
        if(isInsert)
            selectAttributeItemValue.value = selectedAttribute.value.id;
    }

    function disableCrudButtons() {
        let allValid = rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.name) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.type) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.aggregationRuleType) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.defaultValue) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.isCalculated) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.isAscending) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.dataSource.type) === true
            if(selectedAttribute.value.type === 'NUMBER'){
                if(isNaN(+selectedAttribute.value.defaultValue)){
                    allValid = false;
                    selectedAttribute.value.defaultValue = '';
                }               
            }
            //when the parameter and ui sync an empty string is assigned to the parameter instead of null if the text box is empty
            if(!isNil(selectedAttribute.value.maximum))  
                selectedAttribute.value.maximum = null;
            if(!isNil(selectedAttribute.value.minimum))  
                selectedAttribute.value.minimum = null;

            if(selectedAttribute.value.dataSource.type === 'SQL'){
                allValid = allValid &&
                rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.command) === true &&
                checkedCommand.value === selectedAttribute.value.command &&
                commandIsValid.value;
            }
            else if(selectedAttribute.value.dataSource.type === 'Excel'){
                allValid = allValid &&
                rules['generalRules'].valueIsNotEmpty(selectedAttribute.value.command) === true;
            }

        return !allValid;
    }

    function CheckSqlCommand(){
        let commandData: TestStringData = {testString: selectedAttribute.value.command};
        AttributeService.CheckCommand(commandData)
            .then((response: AxiosResponse) => {
          if (hasValue(response, 'data')) {
            const result: ValidationResult = response.data as ValidationResult;
            commandIsValid.value = result.isValid;
            checkedCommand.value = selectedAttribute.value.command;
            if (result.isValid) {
              ValidationSuccessMessage.value = result.validationMessage;
              validationErrorMessage.value = '';
            } else {
              validationErrorMessage.value = result.validationMessage;
              ValidationSuccessMessage.value = '';            
            }
          }
        });
    }


</script>

<style>
    .sharing {
    padding-top: 0 !important;
}
.sharing .v-input__slot{
    top: -10px !important;
}

.sharing .v-label{
    margin-bottom: 0;
    padding-top: 0;
}
</style>