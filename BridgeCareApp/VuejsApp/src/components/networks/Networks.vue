<template>
    <v-layout column class="Montserrat-font-family">
        <v-flex xs12>
            <v-flex xs8 class="ghd-constant-header">
                <v-layout>
                    <v-layout column>
                        <v-subheader id="Networks-headerText-vsubheader" class="ghd-md-gray ghd-control-label">Network</v-subheader>
                        <v-select :items='selectNetworkItems'
                            id="Networks-selectNetwork-vselect"
                            outline  
                            v-model='selectNetworkItemValue'                         
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    </v-layout>
                    <v-btn style="margin-top: 20px !important; margin-left: 20px !important" 
                        id="Networks-addNetwork-vbtn"
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
                            id="Networks-KeyAttribute-vselect"
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
                        id="Networks-DataSource-vselect"
                        outline 
                        :items="selectDataSourceItems"  
                        style="margin-top: 18px !important;"                  
                        class="ghd-select ghd-text-field ghd-text-field-border shifted-label"
                        label="Data Source"
                        v-model="selectDataSourceId">
                    </v-select>  
                    <v-btn style="margin-top: 20px !important;" 
                        id="Networks-SelectAllFromSource-vbtn"
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
                                        id="Networks-EditSpatialWeightingEquation-vbtn"
                                        style="padding-right:20px !important;"
                                        class="edit-icon ghd-control-label"
                                        icon
                                        :disabled="!isNewNetwork"
                                        @click="onShowEquationEditorDialog">
                                        <v-icon class="ghd-blue">fas fa-edit</v-icon>
                                    </v-btn>
                                </v-flex>
                            </v-layout>
                        <v-text-field id="Networks-EditSpatialWeightingEquation-vtextfield" outline class="ghd-text-field-border ghd-text-field" 
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
                                <v-btn id="Networks-AddAll-vbtn" flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onAddAll">
                                    Add All
                                </v-btn>
                                <v-divider class="investment-divider" inset vertical>
                                </v-divider>
                                <v-btn id="Networks-RemoveAll-vbtn" flat class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onRemoveAll">
                                    Remove All
                                </v-btn>
                            </v-layout>
                            <v-data-table id="Networks-Attributes-vdatatable" :headers='dataSourceGridHeaders' :items='attributeRows'
                                class='v-table__overflow ghd-table' item-key='id' select-all
                                v-model="selectedAttributeRows"
                                :must-sort='true'
                                hide-actions
                                :pagination.sync="pagination">
                                <template slot='items' slot-scope='props' v-slot:item="{item}">
                                    <td>
                                        <v-checkbox id="Networks-SelectAttribute-vcheckbox" hide-details primary v-model='item.raw.selected'></v-checkbox>
                                    </td>
                                    <td>{{ item.name }}</td> 
                                    <td>{{ item.dataSource.type }}</td> 
                                </template>
                            </v-data-table>    
                            <div class="text-xs-center pt-2">
                                <v-pagination id="Networks-ChangeTablePage-vpagination" class="ghd-pagination ghd-button-text" 
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
                <v-btn id="Networks-Cancel-vbtn" :disabled='!hasUnsavedChanges' @click='onDiscardChanges'
                    flat class='ghd-blue ghd-button-text ghd-button'>
                    Cancel
                </v-btn>  
                <v-btn id="Networks-Aggregate-vbtn" @click='aggregateNetworkData' :disabled='disableCrudButtonsAggregate() || isNewNetwork' v-show="!isNewNetwork" class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Aggregate
                </v-btn>
                <v-btn id="Networks-Delete-vbtn" @click='onDeleteClick' :disabled='isNewNetwork' v-show="!isNewNetwork" class='ghd-blue-bg white--text ghd-button-text ghd-button'>
                    Delete
                </v-btn>
                <v-btn id="Networks-Create-vbtn" @click='createNetwork' :disabled='disableCrudButtonsCreate() || !isNewNetwork'
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

<script setup lang='ts'>
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { SelectItem } from '@/shared/models/vue/select-item';
import Vue, { inject, onBeforeUnmount, onMounted, Ref, ref, ShallowRef, shallowRef, watch } from 'vue';
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
import { InputValidationRules, rules as validationRules } from '@/shared/utils/input-validation-rules';
import  AddNetworkDialog from '@/components/networks/networks-dialogs/AddNetworkDialog.vue';
import { AddNetworkDialogData, emptyAddNetworkDialogData } from '@/shared/models/modals/add-network-dialog-data';
import { Hub } from '@/connectionHub';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { useStore } from 'vuex';

    let store = useStore();
    let stateNetworks: ShallowRef<Network[]> = (store.state.networkModule.networks) ;
    let stateSelectedNetwork: ShallowRef<Network> = (store.state.networkModule.selectedNetwork) ;
    let stateAttributes: ShallowRef<Attribute[]> = (store.state.attributeModule.attributes) ;
    let stateDataSources: ShallowRef<Datasource[]> = (store.state.datasourceModule.dataSources) ;
    let hasUnsavedChanges: boolean = (store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let isAdmin: boolean = (store.state.authenticationModule.isAdmin) ;
    
    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks');}
    async function getDataSources(payload?: any): Promise<any> {await store.dispatch('getDataSources');}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes');}
    async function selectNetworkAction(payload?: any): Promise<any> {await store.dispatch('selectNetwork');}
    async function createNetworkAction(payload?: any): Promise<any> {await store.dispatch('createNetwork');}
    async function deleteNetworkAction(payload?: any): Promise<any> {await store.dispatch('deleteNetwork');}
    async function aggregateNetworkAction(payload?: any): Promise<any> {await store.dispatch('aggregateNetworkData');}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges');}
    async function getUserNameByIdGetter(payload?: any): Promise<any> {await store.dispatch('getUserNameById');}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}

    let rules: InputValidationRules = validationRules;

    let dataSourceGridHeaders: DataTableHeader[] = [
        { text: 'Name', value: 'name', align: 'left', sortable: true, class: '', width: '' },
        { text: 'Data Source', value: 'data source', align: 'left', sortable: true, class: '', width: '' },
    ];

    let addNetworkDialogData: AddNetworkDialogData = clone(emptyAddNetworkDialogData);
    let pagination: Pagination = emptyPagination;
    let selectNetworkItems: SelectItem[] = [];
    let selectKeyAttributeItems: SelectItem[] = [];
    let selectDataSourceItems: SelectItem[] = [];
    let attributeRows: Attribute[] =[];
    let cleanAttributes: Attribute[] = [];
    let attributes: Attribute[] = [];
    let selectedAttributeRows: ShallowRef<Attribute[]> = shallowRef([]);
    let dataSourceSelectValues: SelectItem[] = [
        {text: 'SQL', value: 'SQL'},
        {text: 'Excel', value: 'Excel'},
        {text: 'None', value: 'None'}
    ]; 

    let networkDataAssignmentPercentage = 0;
    let networkDataAssignmentStatus: string = 'Waiting on server.';

    let selectedKeyAttributeItem: Ref<string> = ref('');
    let selectedKeyAttribute: Attribute = clone(emptyAttribute);
    let selectedNetwork: Ref<Network> = ref(clone(emptyNetwork));
    let selectNetworkItemValue: Ref<string> = ref('');
    let selectDataSourceId: string = '';
    let hasSelectedNetwork: boolean = false;
    let isNewNetwork: boolean = false;
    let hasStartedAggregation: boolean = false;
    let isKeyPropertySelectedAttribute: boolean = false;
    let spatialWeightingEquationValue: Equation = clone(emptyEquation); //placeholder until network dto and api changes
    let equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );

    const $statusHub = inject('$statusHub') as any

    created();
    function created() {
        getAttributes();
        getNetworks();
        getDataSources();
    }
    onMounted(() => mounted); 
    function mounted() {
        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }

    onBeforeUnmount(() => beforeDestroy)
    function beforeDestroy() {
        $statusHub.$off(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }
    
    watch(stateNetworks, () => onStateNetworksChanged)
    function onStateNetworksChanged() {
        selectNetworkItems = stateNetworks.value.map((network: Network) => ({
            text: network.name,
            value: network.id,
        }));
    }

    watch(stateAttributes, () => onStateAttributesChanged)
    function onStateAttributesChanged() {
        attributeRows = clone(stateAttributes.value);
        selectKeyAttributeItems = stateAttributes.value.map((attribute: Attribute) => ({
            text: attribute.name,
            value: attribute.id,
        }));
    }

    watch(stateDataSources, () => onStateDataSourcesChanges)
    function onStateDataSourcesChanges() {
        selectDataSourceItems = stateDataSources.value.map((dataSource: Datasource) => ({
            text: dataSource.name,
            value: dataSource.id,
        }));
    }

    watch(selectNetworkItemValue, () => onSelectNetworkItemValueChanged)
    function onSelectNetworkItemValueChanged() {
        selectNetworkAction(selectNetworkItemValue);
        if(selectNetworkItemValue.value != getBlankGuid() || isNewNetwork)
            hasSelectedNetwork = true;
        else
            hasSelectedNetwork = false;
    }

    watch(selectedAttributeRows, () => onSelectedAttributeRowsChanged)
    function onSelectedAttributeRowsChanged()
    {
        if(any(propEq('id', selectedNetwork.value.keyAttribute), selectedAttributeRows.value)) {
            isKeyPropertySelectedAttribute = true;
        }
        else {
            isKeyPropertySelectedAttribute = false;
        }
    }
    
    watch(stateSelectedNetwork, () => onStateSelectedNetworkChanged)
    function onStateSelectedNetworkChanged() {
        if (!isNewNetwork) {
            selectedNetwork = clone(stateSelectedNetwork);
        }
    }

    watch(selectedNetwork, () => onSelectedNetworkChanged)
    function onSelectedNetworkChanged() {
        selectedAttributeRows.value = [];
        hasStartedAggregation = false;
        selectNetworkItemValue.value = selectedNetwork.value.id;
        selectedKeyAttributeItem.value = selectedNetwork.value.keyAttribute;
        spatialWeightingEquationValue.expression = selectedNetwork.value.defaultSpatialWeighting;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', selectedNetwork, stateSelectedNetwork);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    }

    watch(selectedKeyAttributeItem, () => onSelectedKeyAttributeItemChanged)
    function onSelectedKeyAttributeItemChanged()
    {
        selectedKeyAttribute = attributeRows.find((attr: Attribute) => attr.id === selectedKeyAttributeItem.value) || clone(emptyAttribute);
    }

    function onAddNetworkDialog() {
        addNetworkDialogData = {
            showDialog: true,
        }
    }

    function addNetwork(network: Network)
    {
        selectNetworkItems.push({
            text: network.name,
            value: network.id
        });
        
        isNewNetwork = true;
        selectNetworkItemValue.value = network.id;
        selectedNetwork.value = clone(network);
        hasSelectedNetwork = true;
    }
    function onDiscardChanges() {
        selectedNetwork = clone(stateSelectedNetwork);
    }
    function onSubmitEquationEditorDialogResult(equation: Equation) {
        equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation)) {
            spatialWeightingEquationValue = clone(equation)
        }
    }
    function onShowEquationEditorDialog() {
        equationEditorDialogData = {
            showDialog: true,
            equation: spatialWeightingEquationValue,
        };      
    }
    function selectAllFromSource(){
        selectedAttributeRows.value = clone(stateAttributes.value.filter((attr: Attribute) => attr.dataSource.id == selectDataSourceId));
    }
    function onAddAll(){
        selectedAttributeRows.value = clone(attributeRows)
    }
    function onRemoveAll(){
        selectedAttributeRows.value = [];
    }
    function aggregateNetworkData(){
        aggregateNetworkAction({
            attributes: selectedAttributeRows.value,
            networkId: selectNetworkItemValue.value
        });

        hasStartedAggregation = true;
    }

    function onDeleteClick(){
        deleteNetworkAction(selectedNetwork.value.id).then(() => {
            hasSelectedNetwork = false;
            selectNetworkItemValue.value = "";
            selectedNetwork.value = clone(emptyNetwork)
        })       
    }
    function disableCrudButtonsCreate() {

        let allValid = rules['generalRules'].valueIsNotEmpty(selectedNetwork.value.name) === true
            && rules['generalRules'].valueIsNotEmpty(spatialWeightingEquationValue.expression) === true
            && rules['generalRules'].valueIsNotEmpty(selectedKeyAttributeItem) === true;


        return !allValid;
    }
    function disableCrudButtonsAggregate() {
        let isKeyPropertySelectedAttribute: Boolean = any(propEq('id', selectedNetwork.value.KeyAttribute), selectedAttributeRows.value);

        let allValid = rules['generalRules'].valueIsNotEmpty(selectedNetwork.value.name) === true
            && rules['generalRules'].valueIsNotEmpty(selectedAttributeRows) === true
            && isKeyPropertySelectedAttribute === true
            && hasStartedAggregation === false;


        return !allValid;
    }
    function createNetwork(){
        isNewNetwork = false;

        createNetworkAction({
            network: selectedNetwork.value,
            parameters: {
                defaultEquation: spatialWeightingEquationValue.expression,
                networkDefinitionAttribute: selectedKeyAttribute
            }
        })

    }

    function getDataAggregationStatus(data: any) {
        const networkRollupDetail: NetworkRollupDetail = data.networkRollupDetail as NetworkRollupDetail;
        if (networkRollupDetail.networkId === selectedNetwork.value.id){
            networkDataAssignmentStatus = networkRollupDetail.status;
            networkDataAssignmentPercentage = data.percentage as number;
        }
    }
    
    function pages() {
        pagination.totalItems = attributeRows.length
        if (pagination.rowsPerPage == null || pagination.totalItems == null) 
            return 0

        return Math.ceil(pagination.totalItems / pagination.rowsPerPage)
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