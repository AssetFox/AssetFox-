<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Attribute</v-subheader>
                        <v-select :items='selectAttributeItems'
                            outline                           
                            v-model='selectAttributeItemValue' class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                    <v-btn style="margin-top: 20px !important; margin-left: 20px !important" @click="addAttribute" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline>
                        Add Attribute
                    </v-btn>
                </v-layout>
            </v-flex>
        </v-flex>
        <v-divider v-if="hasSelectedAttribute" />
        <v-flex xs12 v-if="hasSelectedAttribute" class="ghd-constant-header" >
            <v-layout>
                <v-flex xs2> 
                    <v-text-field outline style="padding-top: 20px !important" class="ghd-text-field-border ghd-text-field"
                        placeholder="Name" v-model='selectedAttribute.name'/>
                </v-flex>
                <v-flex xs2>
                    <v-subheader class="ghd-md-gray ghd-control-label">Type</v-subheader>
                    <v-select
                        outline                           
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
                        outline                           
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
                        <v-text-field v-if="selectedAttribute.type == 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            placeholder="Default Value"
                            v-model='selectedAttribute.defaultValue'/>
                        <v-text-field v-if="selectedAttribute.type != 'STRING'" outline class="ghd-text-field-border ghd-text-field"
                            placeholder="Default Value"
                            v-model.number='selectedAttribute.defaultValue'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs2>
                        <v-text-field outline class="ghd-text-field-border ghd-text-field"
                            placeholder="Minimum Value"
                            v-model.number='selectedAttribute.minimum'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs2>
                        <v-text-field outline class="ghd-text-field-border ghd-text-field"
                            placeholder="Maximum Value"
                            v-model.number='selectedAttribute.maximum'
                            :mask="'#############'"/>
                    </v-flex>
                    <v-flex xs4>
                        <v-layout>
                        <v-switch class='sharing header-text-content' label='Calculated' 
                            v-model='selectedAttribute.isCalculated'/>
                        <v-switch class='sharing header-text-content' label='Ascending' 
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
                        outline  
                        v-model='currentDataSource'
                        :items='dataSourceSelectValues'                     
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>                           
                </v-layout>
            </v-flex>
        </v-flex>
        <!-- Command text area -->
        <v-flex xs12 v-if="hasSelectedAttribute && currentDataSource == 'MS SQL'">
            <v-layout justify-center>
                <v-flex >
                    <v-subheader class="ghd-subheader ">Command</v-subheader>
                    <v-textarea no-resize outline rows='4' class="ghd-text-field-border" v-model='selectedAttribute.command'>
                    </v-textarea>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- Data source combobox -->
        <v-flex xs12 v-if="hasSelectedAttribute && currentDataSource == 'Excel'">
            <v-flex xs6>
                <v-layout column>
                    <v-subheader class="ghd-md-gray ghd-control-label">Column Name</v-subheader>
                    <v-select
                        outline                           
                        class="ghd-select ghd-text-field ghd-text-field-border">
                    </v-select>                           
                </v-layout>
            </v-flex>
        </v-flex>
        <!-- The Buttons  -->
        <v-flex xs12 v-if="hasSelectedAttribute">        
            <v-layout justify-center>
                <v-btn :disabled='!hasUnsavedChanges' @click='onDiscardChanges' flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn v-if="currentDataSource != 'Excel'" :disabled="currentDataSource == 'None'" class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Test
                </v-btn>
                <v-btn @click='saveAttribute' :disabled='disableCrudButtons() || !hasUnsavedChanges' class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Save
                </v-btn>               
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script lang='ts'>
import { Attribute, emptyAttribute } from '@/shared/models/iAM/attribute';
import { SelectItem } from '@/shared/models/vue/select-item';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { clone, isNil } from 'ramda';
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, State } from 'vuex-class';

@Component({
    
})
export default class Attributes extends Vue {
    hasSelectedAttribute: boolean = false;
    selectAttributeItemValue: string | null = null;
    selectAttributeItems: SelectItem[] = [];
    selectedAttribute: Attribute = clone(emptyAttribute);
    currentDataSource: string  = 'None';
    rules: InputValidationRules = rules;

    aggregationRuleSelectValues: SelectItem[] = [
        {text: 'PREDOMINANT', value: 'PREDOMINANT'},
        {text: 'AVERAGE', value: 'AVERAGE'}
    ];
    typeSelectValues: SelectItem[] = [
        {text: 'STRING', value: 'STRING'},
        {text: 'NUMBER', value: 'NUMBER'}
    ];
    dataSourceSelectValues: SelectItem[] = [
        {text: 'MS SQL', value: 'MS SQL'},
        {text: 'Excel', value: 'Excel'},
        {text: 'None', value: 'None'}
    ];

    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.attributeModule.selectedAttribute) stateSelectedAttribute: Attribute;
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    
    @Action('getAttributes') getAttributes: any;
    @Action('selectAttribute') selectAttributeAction: any;
    @Action('upsertAttribute') upsertAttributeAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.librarySelectItemValue = null;
            vm.getAttributes();
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

    @Watch('selectAttributeItemValue')
    onSelectAttributeItemValueChanged() {
        this.selectAttributeAction(this.selectAttributeItemValue);
        this.hasSelectedAttribute = true;
    }

    @Watch('stateSelectedAttribute')
    onStateSelectedAttributeChanged() {
        this.selectedAttribute = clone(this.stateSelectedAttribute);
    }

    @Watch('selectedAttribute', {deep: true})
    onSelectedAttributeChanged() {
        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', this.selectedAttribute, this.stateSelectedAttribute)
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
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.command) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.defaultValue) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.isCalculated) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttribute.isAscending) === true
            if(this.selectedAttribute.type === 'NUMBER'){
                allValid = allValid && !isNaN(+this.selectedAttribute.defaultValue);
                this.selectedAttribute.defaultValue = '';
            }
                

        return !allValid;
    }
}

</script>

<style>

</style>