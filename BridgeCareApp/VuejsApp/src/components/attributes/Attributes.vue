<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader id="Attributes-headerText-vsubheader" class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                        <v-select :items='selectAttributeItems'
                            id="Attributes-selectAttribute-vselect"
                            variant="outlined"
                            append-icon=$vuetify.icons.ghd-down                           
                            v-model='selectAttributeItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                    <v-btn id="Attributes-addAttribute-vbtn" style="margin-top: 20px !important; margin-left: 20px !important" @click="addAttribute" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
                        Add Attribute
                    </v-btn>
                </v-layout>
            </v-flex>
        </v-flex>
        <v-divider v-if="hasSelectedAttribute" />
        <v-flex xs12 v-if="hasSelectedAttribute" class="ghd-constant-header" >
            <v-layout>
                <v-flex xs2> 
                    <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                    <v-text-field id="Attributes-attributeName-vtextfield" outline class="ghd-text-field-border ghd-text-field"
                        placeholder="Name" v-model='selectedAttribute.name'/>
                </v-flex>
                <v-flex xs2>
                    <v-subheader class="ghd-md-gray ghd-control-label">Data Type</v-subheader>
                    <v-select
                        id="Attributes-attributeDataType-vselect"
                        variant="outlined"
                        append-icon=$vuetify.icons.ghd-down                           
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='typeSelectValues'
                        v-model='selectedAttribute.type'>
                    </v-select>                           
                </v-flex>
                <v-flex xs4>
                    <v-subheader class="ghd-md-gray ghd-control-label">
                        Aggregation Rule
                    </v-subheader>
                    <v-select
                        id="Attributes-attributeAggregationRule-vselect"
                        variant="outlined"
                        append-icon=$vuetify.icons.ghd-down                           
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='aggregationRuleSelectValues'
                        v-model='selectedAttribute.aggregationRuleType'>
                    </v-select>                           
                </v-flex>
            </v-layout>
        </v-flex>
        <v-flex xs12 v-if="hasSelectedAttribute">
            <v-flex xs10>
                <v-layout>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Default Value</v-subheader>
                        <v-text-field id="Attributes-attributeDefaultString-vtextfield" v-if="selectedAttribute.type == 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            v-model='selectedAttribute.defaultValue'/>
                        <v-text-field id="Attributes-attributeDefaultNumber-vtextfield" v-if="selectedAttribute.type != 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            v-model.number='selectedAttribute.defaultValue'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Minimum Value</v-subheader>
                        <v-text-field id="Attributes-attributeMinimumValue-vtextfield" outline class="ghd-text-field-border ghd-text-field"                            
                            v-model.number='selectedAttribute.minimum'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs2>
                        <v-subheader class="ghd-md-gray ghd-control-label">Maximum Value</v-subheader>
                        <v-text-field id="Attributes-attributeMaximumValue-vtextfield" outline class="ghd-text-field-border ghd-text-field"
                            v-model.number='selectedAttribute.maximum'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs4 style="padding-top:50px;">
                        <v-layout>
                        <v-switch id="Attributes-attributeCalculated-vswitch" class='sharing header-text-content' label='Calculated' 
                            v-model='selectedAttribute.isCalculated'/>
                        <v-switch id="Attributes-attributeAscending-vswitch" class='sharing header-text-content' label='Ascending' 
                            v-model='selectedAttribute.isAscending'/>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </v-flex>
        </v-flex>
        <!-- Data source combobox -->
        <v-flex xs12 v-if="hasSelectedAttribute">
            <v-flex xs6>
                <v-layout column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Data Source</v-subheader>
                    <v-select
                        id="Attributes-attributeDataSource-vselect"
                        variant="outlined"
                        append-icon=$vuetify.icons.ghd-down  
                        v-model='selectDatasourceItemValue'
                        :items='selectDatasourceItems'                     
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>                           
                </v-layout>
            </v-flex>
        </v-flex>
        <!-- Command text area -->
        <v-flex xs12 v-if="hasSelectedAttribute && selectedAttribute.dataSource.type == 'SQL'">
            <v-layout justify-center>
                <v-flex >
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
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- Data source combobox -->
        <v-flex xs12 v-if="hasSelectedAttribute && selectedAttribute.dataSource.type == 'Excel'">
            <v-flex xs6>
                <v-layout column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Column Name</v-subheader>
                    <v-select
                        id="Attributes-attributeColumnName-vselect"
                        variant="outlined"
                        append-icon=$vuetify.icons.ghd-down                           
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        :items='selectExcelColumns'
                        v-model='selectedAttribute.command'>
                    </v-select>                           
                </v-layout>
            </v-flex>
        </v-flex>
        <!-- The Buttons  -->
        <v-flex xs12 v-if="hasSelectedAttribute">        
            <v-layout justify-center>
                <v-btn id="Attributes-cancel-vbtn" :disabled='!hasUnsavedChanges' @click='onDiscardChanges' flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn v-if="selectedAttribute.dataSource.type == 'SQL'" :disabled="selectedAttribute.dataSource.type != 'SQL'" 
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'
                    @click="CheckSqlCommand">
                    Test
                </v-btn>
                <v-btn id="Attributes-save-vbtn" @click='saveAttribute' :disabled='disableCrudButtons() || !hasUnsavedChanges' class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Save
                </v-btn>               
            </v-layout>
        </v-flex>
    </v-layout>
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
import Vue, { onBeforeMount, onBeforeUnmount, Ref, ref, shallowRef, ShallowRef, watch } from 'vue';
import { Console } from 'console';
import { useStore } from 'vuex';

    let store = useStore();
    let hasSelectedAttribute: boolean = false;
    let hasEmptyDataSource: boolean = true;
    let selectAttributeItemValue: ShallowRef<string | null> = shallowRef(null);
    let selectDatasourceItemValue: ShallowRef<string | null> = shallowRef(null);
    let selectAttributeItems: SelectItem[] = [];
    let selectDatasourceItems: SelectItem[] = [];
    let selectAggregationRuleTypeItems: SelectItem[] = [];
    let selectExcelColumns: SelectItem[] = [];
    let selectedAttribute: Ref<Attribute> = ref(clone(emptyAttribute));
    let selectedDataSource: Datasource | undefined = clone(emptyDatasource);
    let rules: InputValidationRules = validationRules;
    let validationErrorMessage: string = '';
    let ValidationSuccessMessage: string = '';
    let commandIsValid: boolean = true;
    let checkedCommand = '';

    let aggregationRuleSelectValues: SelectItem[] = []    
    let typeSelectValues: SelectItem[] = [
        {text: 'STRING', value: 'STRING'},
        {text: 'NUMBER', value: 'NUMBER'}
    ];

    let stateAttributes = shallowRef<Attribute[]>(store.state.attributeModule.attributes) ;
    let stateDataSources = shallowRef<Datasource[]>(store.state.datasourceModule.dataSources) ;    
    let stateAggregationRules = shallowRef<RuleDefinition[]>(store.state.attributeModule.aggregationRules) ;
    let stateAggregationRulesForType = shallowRef<string[]>(store.state.attributeModule.aggregationRulesForType) ;
    let stateAttributeDataSourceTypes = shallowRef<string[]>(store.state.attributeModule.attributeDataSourceTypes) ;
    let excelColumns = shallowRef<RawDataColumns>(store.state.datasourceModule.excelColumns) ;
    let stateSelectedAttribute = shallowRef<Attribute>(store.state.attributeModule.selectedAttribute) ;
    let hasUnsavedChanges = shallowRef<boolean>(store.state.unsavedChangesFlagModule.hasUnsavedChanges) ;
    let hasAdminAccess = shallowRef<boolean>(store.state.authenticationModule.hasAdminAccess) ;
    
    async function logOutAction(payload?: any): Promise<any> {await store.dispatch('logOut');}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
    async function getDataSourcesAction(payload?: any): Promise<any> {await store.dispatch('getDataSources');}
    async function getAttributeAggregationRulesAction(payload?: any): Promise<any> {await store.dispatch('getAttributeAggregationRules');}
    async function getAggregationRulesForTypeAction(payload?: any): Promise<any> {await store.dispatch('getAggregationRulesForType');}
    async function getAttributeDataSourceTypes(payload?: any): Promise<any> {await store.dispatch('getAttributeDataSourceTypes');}
    async function getExcelSpreadsheetColumnHeadersAction(payload?: any): Promise<any> {await store.dispatch('getExcelSpreadsheetColumnHeaders');}
    async function selectAttributeAction(payload?: any): Promise<any> {await store.dispatch('selectAttribute');}
    async function upsertAttributeAction(payload?: any): Promise<any> {await store.dispatch('upsertAttribute');}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
    async function getUserNameByIdGetter(payload?: any): Promise<any> {await store.dispatch('getUserNameById');}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}

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

    watch(stateAttributes, () => onStateAttributesChanged)
    function onStateAttributesChanged() {
        selectAttributeItems = stateAttributes.value.map((attribute: Attribute) => ({
            text: attribute.name,
            value: attribute.id,
        }));
    }

    watch(stateDataSources, () => onStateDataSourcesChanged)
    function onStateDataSourcesChanged() {
        selectDatasourceItems = stateDataSources.value.map((datasource: Datasource) => ({
            text: datasource.name + ' (' + datasource.type + ')',
            value: datasource.id,
        }));
    }

    watch(selectAttributeItemValue, () => onSelectAttributeItemValueChanged)
    function onSelectAttributeItemValueChanged() {
        selectAttributeAction(selectAttributeItemValue);
        hasSelectedAttribute = true;
        checkedCommand = "";
        commandIsValid = false;
        getAggregationRulesForTypeAction(selectedAttribute.value.type)
        aggregationRuleSelectValues = stateAggregationRulesForType.value.map((rule: string) => ({
            text: rule,
            value: rule,
        }));
    }
    
    watch(() => selectedAttribute.value.type, () => onSelectedAttributeTypeChanged)
    function onSelectedAttributeTypeChanged() {
        getAggregationRulesForTypeAction(selectedAttribute.value.type)
        aggregationRuleSelectValues = stateAggregationRulesForType.value.map((rule: string) => ({
            text: rule,
            value: rule,
        }));
    }

    watch(selectDatasourceItemValue, () => onSelectDatasourceItemValue)
    function onSelectDatasourceItemValue(){
        if (any(propEq('id', selectDatasourceItemValue), stateDataSources.value)) {
            let ds = find(
                propEq('id', selectDatasourceItemValue),
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
    }

    watch(excelColumns, () => onExcelColumnsChanged)
    function onExcelColumnsChanged(){
        selectExcelColumns = excelColumns.value.columnHeaders.map((header: string) => ({
            text: header,
            value: header,
        }));
    }

    watch(stateSelectedAttribute, () => onStateSelectedAttributeChanged)
    function onStateSelectedAttributeChanged() {
        selectedAttribute = clone(stateSelectedAttribute);
        if(isNil(selectedAttribute.value.dataSource)) {
            selectedAttribute.value.dataSource = clone(emptyDatasource);
        }
        selectDatasourceItemValue.value = selectedAttribute.value.dataSource.id;
    }

    watch(selectedAttribute, () =>onSelectedAttributeChanged )
    function onSelectedAttributeChanged() {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', selectedAttribute, stateSelectedAttribute);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    function addAttribute()
    {
        selectAttributeItemValue.value = getBlankGuid()
    }

    function onDiscardChanges() {
        selectedAttribute = clone(stateSelectedAttribute);
    }

    function saveAttribute(){
        let isInsert = false;
        if(selectedAttribute.value.id === getBlankGuid()){
            selectedAttribute.value.id = getNewGuid();
            isInsert = true;
        }
            
        upsertAttributeAction(selectedAttribute)
        
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
                checkedCommand === selectedAttribute.value.command &&
                commandIsValid;
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
            commandIsValid = result.isValid;
            checkedCommand = selectedAttribute.value.command;
            if (result.isValid) {
              ValidationSuccessMessage = result.validationMessage;
              validationErrorMessage = '';
            } else {
              validationErrorMessage = result.validationMessage;
              ValidationSuccessMessage = '';            
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