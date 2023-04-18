<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">Network</v-subheader>
                        <v-select :items='selectNetworkItems'
                            outline  
                            v-model='selectNetworkItemValue'                         
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                    <v-btn style="margin-top: 20px !important; margin-left: 20px !important" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="onAddNetworkDialog">
                        Add Network
                    </v-btn>
                </v-layout>
            </v-flex>
        </v-flex>
        <v-divider />
        <v-flex xs12 class="ghd-constant-header" v-show="hasSelectedNetwork">
            <v-layout justify-space-between>
                <v-flex xs6>
                    <v-layout column>
                        <v-subheader class="ghd-md-gray ghd-control-label">
                            Key Attribute
                        </v-subheader>
                        <v-select
                            outline                           
                            class="ghd-select ghd-text-field ghd-text-field-border"
                            :disabled="!isNewNetwork"
                            v-model="selectedKeyAttributeItem"
                            :items='selectKeyAttributeItems'>
                        </v-select>  
                    </v-layout>                         
                </v-flex>
                <v-flex xs5>
                <v-layout v-show="!isNewNetwork">
                    <v-select
                        outline 
                        :items="selectDataSourceItems"  
                        style="margin-top: 18px !important;"                  
                        class="ghd-select ghd-text-field ghd-text-field-border shifted-label"
                        label="Data Source"
                        v-model="selectDataSourceId">
                    </v-select>  
                    <v-btn style="margin-top: 20px !important;" 
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' outline
                        @click="selectAllFromSource">
                        Select All From Source
                    </v-btn>                            
                </v-layout>  
                </v-flex>       
            </v-layout>
        </v-flex>
        <!-- Data source combobox -->
        <v-flex xs12 v-show="hasSelectedNetwork">
            <v-layout justify-space-between>
                <v-flex xs5 >
                    <v-layout column>
                        <v-layout style="height=12px;padding-bottom:0px;">
                                <v-flex xs12 class="ghd-constant-header" style="height=12px;padding-bottom:0px">
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="padding-top: 14px !important">                             
                                        Spatial Weighting Equation</v-subheader>
                                </v-flex>
                                <v-flex xs1 style="height=12px;padding-bottom:0px;padding-top:0px;">
                                    <v-btn
                                        style="padding-right:20px !important;"
                                        class="edit-icon ghd-control-label"
                                        icon
                                        :disabled="!isNewNetwork"
                                        @click="onShowEquationEditorDialog">
                                        <v-icon class="ghd-blue">fas fa-edit</v-icon>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                        <v-text-field outline class="ghd-text-field-border ghd-text-field" 
                           :disabled="!isNewNetwork" v-model="spatialWeightingEquationValue.expression"/>                         
                    </v-layout>
                    <v-layout v-show="hasStartedAggregation">
                        <v-flex>
                            <v-subheader class="ghd-control-label ara-black" v-text="networkDataAssignmentStatus" ></v-subheader>
                            <v-progress-linear
                                            v-model="
                                                networkDataAssignmentPercentage
                                            "
                                            height="25"
                                            striped
                                        >
                                            <strong
                                                >{{
                                                    Math.ceil(
                                                        networkDataAssignmentPercentage,
                                                    )
                                                }}%</strong
                                            >
                                        </v-progress-linear>
                        </v-flex>
                    </v-layout>
                </v-flex>
                <v-flex xs5>
                    <v-layout column>
                        <div class='priorities-data-table' v-show="!isNewNetwork">
                            <v-layout justify-center>
                                <v-btn flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onAddAll">
                                    Add All
                                </v-btn>
                                <v-divider class="investment-divider" inset vertical>
                                </v-divider>
                                <v-btn flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onRemoveAll">
                                    Remove All
                                </v-btn>
                            </v-layout>
                            <v-data-table :headers='dataSourceGridHeaders' :items='attributeRows'
                                class='v-table__overflow ghd-table' item-key='id' select-all
                                v-model="selectedAttributeRows"
                                :must-sort='true'
                                hide-actions
                                :pagination.sync="pagination">
                                <template slot='items' slot-scope='props'>
                                    <td>
                                        <v-checkbox hide-details primary v-model='props.selected'></v-checkbox>
                                    </td>
                                    <td>{{ props.item.name }}</td> 
                                    <td>{{ props.item.dataSource.type }}</td> 
                                </template>
                            </v-data-table>    
                            <div class="text-xs-center pt-2">
                                <v-pagination class="ghd-pagination ghd-button-text" 
                                    v-model="pagination.page" 
                                    :length="pages()"
                                    ></v-pagination>
                            </div>
                        </div>               
                    </v-layout>
                </v-flex>
            </v-layout>
        </v-flex>
        <!-- The Buttons  -->
        <v-flex xs12 v-show="hasSelectedNetwork">        
            <v-layout justify-center style="padding-top: 30px !important">
                <v-btn :disabled='!hasUnsavedChanges' @click='onDiscardChanges'
                    flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn @click='aggregateNetworkData' :disabled='disableCrudButtonsAggregate() || isNewNetwork' v-show="!isNewNetwork" class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Aggregate
                </v-btn>
                <v-btn @click='onDeleteClick' :disabled='isNewNetwork' v-show="!isNewNetwork" class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Delete
                </v-btn>
                <v-btn @click='createNetwork' :disabled='disableCrudButtonsCreate() || !isNewNetwork'
                    v-show="isNewNetwork"
                    class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Create
                </v-btn>            
            </v-layout>
        </v-flex>
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            :isFromPerformanceCurveEditor=false
            @submit="onSubmitEquationEditorDialogResult"
        />
        <AddNetworkDialog :dialogData='addNetworkDialogData'
                                @submit='addNetwork' />
    </v-layout>
</template>

<script lang='ts'>
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { SelectItem } from '@/shared/models/vue/select-item';
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, Getter, State } from 'vuex-class';
import EquationEditorDialog from '../../shared/modals/EquationEditorDialog.vue';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Attribute, emptyAttribute } from '@/shared/models/iAM/attribute';
import { Datasource } from '@/shared/models/iAM/data-source';
import { clone, isNil, propEq, any } from 'ramda';
import { hasUnsavedChangesCore } from '@/shared/utils/has-unsaved-changes-helper';
import { emptyEquation, Equation } from '@/shared/models/iAM/equation';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import  AddNetworkDialog from '@/components/networks/networks-dialogs/AddNetworkDialog.vue';
import { AddNetworkDialogData, emptyAddNetworkDialogData } from '@/shared/models/modals/add-network-dialog-data';
import { Hub } from '@/connectionHub';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';
import { getBlankGuid } from '@/shared/utils/uuid-utils';

@Component({
    components: {
        EquationEditorDialog,
        AddNetworkDialog
    },
})
export default class Networks extends Vue {
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.networkModule.selectedNetwork) stateSelectedNetwork: Network;
    @State(state => state.attributeModule.attributes) stateAttributes: Attribute[];
    @State(state => state.datasourceModule.dataSources) stateDataSources: Datasource[];
    @State(state => state.unsavedChangesFlagModule.hasUnsavedChanges) hasUnsavedChanges: boolean;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    
    @Action('getNetworks') getNetworks: any;
    @Action('getDataSources') getDataSources: any;
    @Action('getAttributes') getAttributes: any;
    @Action('selectNetwork') selectNetworkAction: any;
    @Action('createNetwork') createNetworkAction: any;
    @Action('deleteNetwork') deleteNetworkAction: any;
    @Action('aggregateNetworkData') aggregateNetworkAction: any;
    @Action('setHasUnsavedChanges') setHasUnsavedChangesAction: any;
    @Getter('getUserNameById') getUserNameByIdGetter: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;

    rules: InputValidationRules = rules;

    dataSourceGridHeaders: DataTableHeader[] = [
        { text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '' },
        { text: 'Data Source', value: 'data source', align: 'left', sortable: true, class: '', width: '' },
    ];

    addNetworkDialogData: AddNetworkDialogData = clone(emptyAddNetworkDialogData);
    pagination: Pagination = emptyPagination;
    selectNetworkItems: SelectItem[] = [];
    selectKeyAttributeItems: SelectItem[] = [];
    selectDataSourceItems: SelectItem[] = [];
    attributeRows: Attribute[] =[];
    cleanAttributes: Attribute[] = [];
    attributes: Attribute[] = [];
    selectedAttributeRows: Attribute[] = [];
    dataSourceSelectValues: SelectItem[] = [
        {text: 'SQL', value: 'SQL'},
        {text: 'Excel', value: 'Excel'},
        {text: 'None', value: 'None'}
    ]; 

    networkDataAssignmentPercentage = 0;
    networkDataAssignmentStatus: string = 'Waiting on server.';

    selectedKeyAttributeItem: string = '';
    selectedKeyAttribute: Attribute = clone(emptyAttribute);
    selectedNetwork: Network = clone(emptyNetwork);
    selectNetworkItemValue: string = '';
    selectDataSourceId: string = '';
    hasSelectedNetwork: boolean = false;
    isNewNetwork: boolean = false;
    hasStartedAggregation: boolean = false;
    isKeyPropertySelectedAttribute: boolean = false;
    spatialWeightingEquationValue: Equation = clone(emptyEquation); //placeholder until network dto and api changes
    equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            vm.getAttributes();
            vm.getNetworks();
            vm.getDataSources();
        });
    }

    mounted() {
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            this.getDataAggregationStatus,
        );
    }
    
    @Watch('stateNetworks')
    onStateNetworksChanged() {
        this.selectNetworkItems = this.stateNetworks.map((network: Network) => ({
            text: network.name,
            value: network.id,
        }));
    }
    @Watch('stateAttributes')
    onStateAttributesChanged() {
        this.attributeRows = clone(this.stateAttributes);
        this.selectKeyAttributeItems = this.stateAttributes.map((attribute: Attribute) => ({
            text: attribute.name,
            value: attribute.id,
        }));
    }
    @Watch('stateDataSources')
    onStateDataSourcesChanges() {
        this.selectDataSourceItems = this.stateDataSources.map((dataSource: Datasource) => ({
            text: dataSource.name,
            value: dataSource.id,
        }));
    }
    @Watch('selectNetworkItemValue')
    onSelectNetworkItemValueChanged() {
        this.selectNetworkAction(this.selectNetworkItemValue);
        if(this.selectNetworkItemValue != getBlankGuid() || this.isNewNetwork)
            this.hasSelectedNetwork = true;
        else
            this.hasSelectedNetwork = false;
    }
    @Watch('selectedAttributeRows')
    onSelectedAttributeRowsChanged()
    {
        if(any(propEq('id', this.selectedNetwork.keyAttribute), this.selectedAttributeRows)) {
            this.isKeyPropertySelectedAttribute = true;
        }
        else {
            this.isKeyPropertySelectedAttribute = false;
        }
    }
    
    @Watch('stateSelectedNetwork')
    onStateSelectedNetworkChanged() {
        if (!this.isNewNetwork) {
            this.selectedNetwork = clone(this.stateSelectedNetwork);
        }
    }
    @Watch('selectedNetwork', {deep: true})
    onSelectedNetworkChanged() {
        this.selectedAttributeRows = [];
        this.hasStartedAggregation = false;
        this.selectNetworkItemValue = this.selectedNetwork.id;
        this.selectedKeyAttributeItem = this.selectedNetwork.keyAttribute;
        this.spatialWeightingEquationValue.expression = this.selectedNetwork.defaultSpatialWeighting;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', this.selectedNetwork, this.stateSelectedNetwork);
        this.setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }
    @Watch('selectedKeyAttributeItem')
    onSelectedKeyAttributeItemChanged()
    {
        this.selectedKeyAttribute = this.attributeRows.find((attr: Attribute) => attr.id === this.selectedKeyAttributeItem) || clone(emptyAttribute);
    }
    onAddNetworkDialog() {
        this.addNetworkDialogData = {
            showDialog: true,
        }
    }

    addNetwork(network: Network)
    {
        this.selectNetworkItems.push({
            text: network.name,
            value: network.id
        });
        
        this.isNewNetwork = true;
        this.selectNetworkItemValue = network.id;
        this.selectedNetwork = clone(network);
        this.hasSelectedNetwork = true;
    }
    onDiscardChanges() {
        this.selectedNetwork = clone(this.stateSelectedNetwork);
    }
    onSubmitEquationEditorDialogResult(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation)) {
            this.spatialWeightingEquationValue = clone(equation)
        }
    }
    onShowEquationEditorDialog() {
        this.equationEditorDialogData = {
            showDialog: true,
            equation: this.spatialWeightingEquationValue,
        };      
    }
    selectAllFromSource(){
        this.selectedAttributeRows = clone(this.stateAttributes.filter((attr: Attribute) => attr.dataSource.id == this.selectDataSourceId));
    }
    onAddAll(){
        this.selectedAttributeRows = clone(this.attributeRows)
    }
    onRemoveAll(){
        this.selectedAttributeRows = [];
    }
    aggregateNetworkData(){
        this.aggregateNetworkAction({
            attributes: this.selectedAttributeRows,
            networkId: this.selectNetworkItemValue
        });

        this.hasStartedAggregation = true;
    }

    onDeleteClick(){
        this.deleteNetworkAction(this.selectedNetwork.id).then(() => {
            this.hasSelectedNetwork = false;
            this.selectNetworkItemValue = "";
            this.selectedNetwork = clone(emptyNetwork)
        })       
    }
    disableCrudButtonsCreate() {

        let allValid = this.rules['generalRules'].valueIsNotEmpty(this.selectedNetwork.name) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.spatialWeightingEquationValue.expression) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedKeyAttributeItem) === true;


        return !allValid;
    }
    disableCrudButtonsAggregate() {
        let isKeyPropertySelectedAttribute: Boolean = any(propEq('id', this.selectedNetwork.KeyAttribute), this.selectedAttributeRows);

        let allValid = this.rules['generalRules'].valueIsNotEmpty(this.selectedNetwork.name) === true
            && this.rules['generalRules'].valueIsNotEmpty(this.selectedAttributeRows) === true
            && this.isKeyPropertySelectedAttribute === true
            && this.hasStartedAggregation === false;


        return !allValid;
    }
    createNetwork(){
        this.isNewNetwork = false;

        this.createNetworkAction({
            network: this.selectedNetwork,
            parameters: {
                defaultEquation: this.spatialWeightingEquationValue.expression,
                networkDefinitionAttribute: this.selectedKeyAttribute
            }
        })

    }

    getDataAggregationStatus(data: any) {
        const networkRollupDetail: NetworkRollupDetail = data.networkRollupDetail as NetworkRollupDetail;
        if (networkRollupDetail.networkId === this.selectedNetwork.id){
            this.networkDataAssignmentStatus = networkRollupDetail.status;
            this.networkDataAssignmentPercentage = data.percentage as number;
        }
    }
    
    pages() {
        this.pagination.totalItems = this.attributeRows.length
        if (this.pagination.rowsPerPage == null || this.pagination.totalItems == null) 
            return 0

        return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
    }

    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            this.getDataAggregationStatus,
        );
    }
}
</script>

<style>
    .shifted-label label{
        font-family: 'Montserrat', sans-serif !important;
        font-size: 14px !important;
        padding-top: 0px !important;
        font-weight: normal;
        top: 10px !important
    }
</style>