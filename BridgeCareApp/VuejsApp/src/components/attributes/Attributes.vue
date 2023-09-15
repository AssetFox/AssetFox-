<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader id="Attributes-headerText-vsubheader" class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                        <v-select :items='selectAttributeItems'
                            id="Attributes-selectAttribute-vselect"
                            outline
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
                        outline
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
                        outline
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
                        outline
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
                        outline
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

<script lang='ts'>
import AttributeService from '@/services/attribute.service';
import { Attribute, emptyAttribute, RuleDefinition } from '@/shared/models/iAM/attribute';
import { Datasource, emptyDatasource, RawDataColumns, noneDatasource } from '@/shared/models/iAM/data-source';
import { ValidationResult } from '@/shared/models/iAM/expression-validation';
import { SelectItem } from '@/shared/models/vue/select-item';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { hasValue } from '@/shared/utils/has-value-util';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { TestStringData } from '@/shared/models/iAM/test-string';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { AxiosResponse } from 'axios';
import { any, clone, find, isNil, propEq } from 'ramda';
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, State } from 'vuex-class';
import { Console } from 'console';

@Component({
    
})
export default class Attributes extends Vue {
    hasSelectedAttribute: boolean = false;
    hasEmptyDataSource: boolean = true;
    selectAttributeItemValue: string | null = null;
    selectDatasourceItemValue: string | null = null;
    selectAttributeItems: SelectItem[] = [];
    selectDatasourceItems: SelectItem[] = [];
    selectAggregationRuleTypeItems: SelectItem[] = [];
    selectExcelColumns: SelectItem[] = [];
    selectedAttribute: Attribute = clone(emptyAttribute);
    selectedDataSource: Datasource | undefined = clone(emptyDatasource);
    rules: InputValidationRules = rules;
    validationErrorMessage: string = '';
    ValidationSuccessMessage: string = '';
    commandIsValid: boolean = true;
    checkedCommand = '';

    aggregationRuleSelectValues: SelectItem[] = []    
    typeSelectValues: SelectItem[] = [
        {text: 'STRING', value: 'STRING'},
        {text: 'NUMBER', value: 'NUMBER'}
    ];

    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.datasourceModule.dataSources) stateDataSources: Datasource[];    
    @State(state => state.attributeModule.aggregationRules) stateAggregationRules: RuleDefinition[];
    @State(state => state.attributeModule.aggregationRulesForType) stateAggregationRulesForType: string[];
    @State(state => state.attributeModule.attributeDataSourceTypes) stateAttributeDataSourceTypes: string[];
    @State(state => state.datasourceModule.excelColumns) excelColumns: RawDataColumns;
    @State(state => state.attributeModule.selectedAttribute) stateSelectedAttribute: Attribute;
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    
    @Action('getAttributes') getAttributes: any;
    @Action('getDataSources') getDataSourcesAction: any;    
    @Action('getAttributeAggregationRules') getAttributeAggregationRulesAction: any;    
    @Action('getAggregationRulesForType') getAggregationRulesForTypeAction: any;    
    @Action('getAttributeDataSourceTypes') getAttributeDataSourceTypes: any;
    @Action('getExcelSpreadsheetColumnHeaders') getExcelSpreadsheetColumnHeadersAction: any;
    @Action('selectAttribute') selectAttributeAction: any;
    @Action('upsertAttribute') upsertAttributeAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.getAttributes();
            vm.getAttributeAggregationRulesAction();
            vm.getAttributeDataSourceTypes();
            vm.getDataSourcesAction();
        });
    }

    beforeDestroy() {
        this.setHasUnsavedChangesAction({ value: false });
    }

    @Watch('stateAttributes')
    onStateAttributesChanged() {
        this.selectAttributeItems = this.stateAttributes.map((attribute: Attribute) => ({
            text: attribute.name,
            value: attribute.id,
        }));
    }

    @Watch('stateDataSources')
    onStateDataSourcesChanged() {
        this.selectDatasourceItems = this.stateDataSources.map((datasource: Datasource) => ({
            text: datasource.name + ' (' + datasource.type + ')',
            value: datasource.id,
        }));
    }

    @Watch('selectAttributeItemValue')
    onSelectAttributeItemValueChanged() {
        this.selectAttributeAction(this.selectAttributeItemValue);
        this.hasSelectedAttribute = true;
        this.checkedCommand = "";
        this.commandIsValid = false;
        this.getAggregationRulesForTypeAction(this.selectedAttribute.type)
        this.aggregationRuleSelectValues = this.stateAggregationRulesForType.map((rule: string) => ({
            text: rule,
            value: rule,
        }));
    }
    
    @Watch('selectedAttribute.type')
    onSelectedAttributeTypeChanged() {
        this.getAggregationRulesForTypeAction(this.selectedAttribute.type)
        this.aggregationRuleSelectValues = this.stateAggregationRulesForType.map((rule: string) => ({
            text: rule,
            value: rule,
        }));
    }
    @Watch('selectDatasourceItemValue')
    onSelectDatasourceItemValue(){
        if (any(propEq('id', this.selectDatasourceItemValue), this.stateDataSources)) {
            let ds = find(
                propEq('id', this.selectDatasourceItemValue),
                this.stateDataSources,
            )
            if(!isNil(ds))
            {
                this.selectedAttribute.dataSource = ds
                if(this.selectedAttribute.dataSource.type === "Excel"){
                    this.getExcelSpreadsheetColumnHeadersAction(this.selectedAttribute.dataSource.id)
                } 
            }
            else
                this.selectedAttribute.dataSource = clone(emptyDatasource)
        } else {
            this.selectedAttribute.dataSource = clone(emptyDatasource)
        }
    }

    @Watch('excelColumns')
    onExcelColumnsChanged(){
        this.selectExcelColumns = this.excelColumns.columnHeaders.map((header: string) => ({
            text: header,
            value: header,
        }));
    }

    @Watch('stateSelectedAttribute')
    onStateSelectedAttributeChanged() {
        this.selectedAttribute = clone(this.stateSelectedAttribute);
        if(isNil(this.selectedAttribute.dataSource)) {
            this.selectedAttribute.dataSource = clone(emptyDatasource);
        }
        this.selectDatasourceItemValue = this.selectedAttribute.dataSource.id;
    }

    @Watch('selectedAttribute', {deep: true})
    onSelectedAttributeChanged() {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', this.selectedAttribute, this.stateSelectedAttribute);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    addAttribute()
    {
        this.selectAttributeItemValue = getBlankGuid()
    }

    onDiscardChanges() {
        this.selectedAttribute = clone(this.stateSelectedAttribute);
    }

    saveAttribute(){
        let isInsert = false;
        if(this.selectedAttribute.id === getBlankGuid()){
            this.selectedAttribute.id = getNewGuid();
            isInsert = true;
        }
            
        this.upsertAttributeAction(this.selectedAttribute)
        
        if(isInsert)
            this.selectAttributeItemValue = this.selectedAttribute.id;
    }

    disableCrudButtons() {
        let allValid = this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.name) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.type) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.aggregationRuleType) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.defaultValue) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.isCalculated) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.isAscending) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.dataSource.type) === true
            if(this.selectedAttribute.type === 'NUMBER'){
                if(isNaN(+this.selectedAttribute.defaultValue)){
                    allValid = false;
                    this.selectedAttribute.defaultValue = '';
                }               
            }
            //when the parameter and ui sync an empty string is assigned to the parameter instead of null if the text box is empty
            if(!isNil(this.selectedAttribute.maximum))  
                this.selectedAttribute.maximum = null;
            if(!isNil(this.selectedAttribute.minimum))  
                this.selectedAttribute.minimum = null;

            if(this.selectedAttribute.dataSource.type === 'SQL'){
                allValid = allValid &&
                this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.command) === true &&
                this.checkedCommand === this.selectedAttribute.command &&
                this.commandIsValid;
            }
            else if(this.selectedAttribute.dataSource.type === 'Excel'){
                allValid = allValid &&
                this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.command) === true;
            }

        return !allValid;
    }

    CheckSqlCommand(){
        let commandData: TestStringData = {testString: this.selectedAttribute.command};
        AttributeService.CheckCommand(commandData)
            .then((response: AxiosResponse) => {
          if (hasValue(response, 'data')) {
            const result: ValidationResult = response.data as ValidationResult;
            this.commandIsValid = result.isValid;
            this.checkedCommand = this.selectedAttribute.command;
            if (result.isValid) {
              this.ValidationSuccessMessage = result.validationMessage;
              this.validationErrorMessage = '';
            } else {
              this.validationErrorMessage = result.validationMessage;
              this.ValidationSuccessMessage = '';            
            }
          }
        });
    }
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