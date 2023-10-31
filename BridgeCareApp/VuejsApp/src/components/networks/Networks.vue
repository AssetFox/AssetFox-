<template>
    <v-row column class="Montserrat-font-family">
        <v-col cols = "12">
            <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label" style="margin-bottom:1%; margin-left:1%;">Network</v-subheader>
            </v-row>
            <v-col cols = "8" >
                <v-row>
                        <v-select :items="selectNetworkItems"
                            id="Networks-selectNetwork-vselect"
                            variant="outlined"
                            item-title = "text"
                            item-value = "value"
                            v-model="selectNetworkItemValue"          
                            class="ghd-select ghd-text-field ghd-text-field-border">
                        </v-select>                           
                    <v-btn style="margin-top: 10px !important; margin-left: 20px !important" 
                        id="Networks-addNetwork-vbtn"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="onAddNetworkDialog">
                        Add Network
                    </v-btn>
                </v-row>
            </v-col>
        </v-col>
        <v-divider />
        <v-col cols = "12" class="ghd-constant-header" v-show="hasSelectedNetwork">
            <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label" >Key Attribute</v-subheader>
            </v-row>
            <v-row justify-space-between>
                <v-col cols = "6" sm="5">
                    <v-row column>
                        <v-select
                            item-title="text"
                            item-value="value"
                            id="Networks-KeyAttribute-vselect"
                            variant="outlined"
                            class="ghd-select ghd-text-field ghd-text-field-border"
                            :disabled="!isNewNetwork"
                            append-icon="@/assets/icons/down.svg"
                            v-model="selectedKeyAttributeItem"
                            :items='selectKeyAttributeItems'>
                        </v-select>  
                    </v-row>                         
                </v-col>
                <v-col cols = "5" style="align-items: right;" v-show="!isNewNetwork">
                    <v-row>
                    <v-subheader class="ghd-md-gray ghd-control-label" >Data Source</v-subheader>
                    </v-row>
                <v-row>
                    <v-select
                        id="Networks-DataSource-vselect"
                        variant="outlined"
                        item-title="text"
                        item-value="value"
                        :items="selectDataSourceItems"                       
                        class="ghd-select ghd-text-field ghd-text-field-border shifted-label"
                        v-model="selectDataSourceId">
                    </v-select>  
                    <v-btn style="margin-top: 10px !important; margin-left: 20px;"  
                        id="Networks-SelectAllFromSource-vbtn"
                        class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined"
                        @click="selectAllFromSource">
                        Select All From Source
                    </v-btn>                            
                </v-row>  
                </v-col>       
            </v-row>
        </v-col>
        <!-- Data source combobox -->
        <v-col cols = "12" v-show="hasSelectedNetwork">
            <v-row justify-space-between>
                <v-col cols = "5" >
                    <v-row column>
                        <v-row style="height=12px;padding-bottom:0px;">
                                <v-col cols = "12" class="ghd-constant-header" style="height=12px;padding-bottom:0px">
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="padding-top: 14px !important">                             
                                        Spatial Weighting Equation</v-subheader>
                                </v-col>
                                <v-col xs1 style="height=12px;">
                                    <!-- <v-btn
                                        id="Networks-EditSpatialWeightingEquation-vbtn"
                                        style="padding-right:20px !important;"
                                        class="edit-icon ghd-control-label"
                                        :disabled="!isNewNetwork"
                                        append-icon="ghd-blue"
                                        @click="onShowEquationEditorDialog">
                                        <v-icon class="ghd-blue">fas fa-edit</v-icon> 
                                    </v-btn> -->
                                    <v-row style="width: 70% !important; margin-bottom: 5px; margin-left: 1px;" >
                                <v-text-field id="Networks-EditSpatialWeightingEquation-vtextfield" outline class="ghd-text-field-border ghd-text-field" 
                                     v-model="spatialWeightingEquationValue.expression"/>
                                </v-row>
                                </v-col>
                          
                            </v-row>
                            
                                                 
                    </v-row>
                    <v-row v-show="hasStartedAggregation">
                        <v-col>
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
                        </v-col>
                    </v-row>
                </v-col>
                <v-col cols = "5">
                    <v-row column>
                        <div class='priorities-data-table' v-show="!isNewNetwork">
                            <v-row justify-center>
                                <v-btn id="Networks-AddAll-vbtn" variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onAddAll">
                                    Add All
                                </v-btn>
                                <v-divider class="investment-divider" inset vertical>
                                </v-divider>
                                <v-btn id="Networks-RemoveAll-vbtn" variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
                                    @click="onRemoveAll">
                                    Remove All
                                </v-btn>
                            </v-row>
                            <v-data-table id="Networks-Attributes-vdatatable" :header='dataSourceGridHeaders' :items='attributeRows'
                                class='v-table__overflow ghd-table' item-key='id' select-all
                                v-model="selectedAttributeRows"
                                :must-sort='true'
                                hide-actions
                                :pagination.sync="pagination">
                                <template slot='items' slot-scope='props' v-slot:item="{item}">
                                    <tr>
                                    <td>
                                        <v-checkbox id="Networks-SelectAttribute-vcheckbox" hide-details primary></v-checkbox>
                                    </td>
                                    <td>
                                        {{
                                            item.name 
                                        }}
                                    </td> 

                                    <td>{{ item.dataSource.type}}</td> 
                                </tr>
                                </template>
                            </v-data-table>    
                        </div>               
                    </v-row>
                </v-col>
            </v-row>
        </v-col>
        <!-- The Buttons  -->
        <v-col cols = "12"  v-show="hasSelectedNetwork">        
            <v-row justify-center style="padding-top: 30px !important">
                <v-btn id="Networks-Cancel-vbtn" :disabled='!hasUnsavedChanges' @click='onDiscardChanges'
                variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'>
                    Cancel
                </v-btn>  
                <v-btn id="Networks-Aggregate-vbtn" @click='aggregateNetworkData' :disabled='disableCrudButtonsAggregate() || isNewNetwork'  class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Aggregate
                </v-btn>
                <v-btn id="Networks-Delete-vbtn" @click='onDeleteClick' :disabled='isNewNetwork'  class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Delete
                </v-btn>
                <v-btn id="Networks-Create-vbtn" @click='createNetwork' :disabled='disableCrudButtonsCreate() || !isNewNetwork'
                   
                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Create
                </v-btn>            
            </v-row>
        </v-col>
        <EquationEditorDialog
            :dialogData="equationEditorDialogData"
            :isFromPerformanceCurveEditor=false
            @submit="onSubmitEquationEditorDialogResult"
        />
        <AddNetworkDialog :dialogData='addNetworkDialogData'
                                @submit='addNetwork' />
    </v-row>
</template>

<script setup lang='ts'>
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { SelectItem } from '@/shared/models/vue/select-item';
import Vue, { computed, DeepReadonly, inject, onBeforeUnmount, onMounted, reactive, Ref, ref, ShallowRef, shallowRef, watch } from 'vue';
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
import mitt from 'mitt';

    let store = useStore();
    let stateNetworks = computed<Network[]>(()=>store.state.networkModule.networks);
    let stateSelectedNetwork = computed<Network> (() => store.state.networkModule.selectedNetwork) ;
    let stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    let stateDataSources = computed<Datasource[]>(() => store.state.datasourceModule.dataSources) ;
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



    const addNetworkDialogData = reactive<AddNetworkDialogData>(emptyAddNetworkDialogData);
    let pagination: Pagination = emptyPagination;
    const selectNetworkItems = ref<SelectItem[]>([]);
    let selectKeyAttributeItems = ref<SelectItem[]>([]);
    let selectDataSourceItems = ref<SelectItem[]>([]);
    let attributeRows = ref<Attribute[]>([]);
    let cleanAttributes: Attribute[] = [];
    let attributes: Attribute[] = [];
    let selectedAttributeRows = ref<Attribute[]>([]);
    let dataSourceSelectValues: SelectItem[] = [
        {text: 'SQL', value: 'SQL'},
        {text: 'Excel', value: 'Excel'},
        {text: 'None', value: 'None'}
    ]; 

    let networkDataAssignmentPercentage = 0;
    let networkDataAssignmentStatus: string = 'Waiting on server.';

    let selectedKeyAttributeItem = ref<string>('');
    let selectedKeyAttribute = ref<Attribute>(clone(emptyAttribute));
    let selectedNetwork = ref<Network>(clone(emptyNetwork));
    let selectNetworkItemValue = ref<string>('');
    let selectDataSourceId: string = '';
    let hasSelectedNetwork = ref<boolean>(false);
    let isNewNetwork: boolean = false;
    let hasStartedAggregation: boolean = false;
    let isKeyPropertySelectedAttribute: boolean = false;
    let spatialWeightingEquationValue: Equation = clone(emptyEquation); //placeholder until network dto and api changes
    let equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    
    const $emitter = mitt()

    created();
    function created() {
        getAttributes();
        getNetworks();
        getDataSources();
    }
    onMounted(() => mounted); 
    function mounted() {
        $emitter.on(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }
    onBeforeUnmount(() => beforeDestroy)
    function beforeDestroy() {
        $emitter.off(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }
    
    watch(stateNetworks, () =>  {
        stateNetworks.value.forEach(_ => {
        selectNetworkItems.value.push({text:_.name,value:_.name})
        });
    })

    watch(stateAttributes, () => { 
        attributeRows.value = clone(stateAttributes.value);
        stateAttributes.value.forEach(_ => {
        selectKeyAttributeItems.value.push({text:_.name,value:_.name})
        })
        });

    watch(stateDataSources, () => {  
        stateDataSources.value.forEach(_ => {
            selectDataSourceItems.value.push({text:_.name,value:_.id})
        })
    })

    watch(selectNetworkItemValue, () =>  {
        selectNetworkAction(selectNetworkItemValue);
        console.log(attributeRows);
        if(selectNetworkItemValue.value != getBlankGuid() || isNewNetwork)
            hasSelectedNetwork.value = true;
        else
            hasSelectedNetwork.value = false;
    })

    watch(selectedAttributeRows, () => 
    {
        if(any(propEq('id', selectedNetwork.value.keyAttribute), selectedAttributeRows.value)) {
            isKeyPropertySelectedAttribute = true;
        }
        else {
            isKeyPropertySelectedAttribute = false;
        }
    })
    
    watch(stateSelectedNetwork, () => {
        if (!isNewNetwork) {
            selectedNetwork = clone(stateSelectedNetwork);
        }
    })

    watch(selectedNetwork, () => { 
        selectedAttributeRows.value = [];
        hasStartedAggregation = false;
        selectNetworkItemValue.value = selectedNetwork.value.id;
        selectedKeyAttributeItem.value = selectedNetwork.value.keyAttribute;
        spatialWeightingEquationValue.expression = selectedNetwork.value.defaultSpatialWeighting;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', selectedNetwork, stateSelectedNetwork);
        setHasUnsavedChangesAction({ value: hasUnsavedChanges });
    })

    watch(selectedKeyAttributeItem, () => 
    {
        selectedKeyAttribute.value = attributeRows.value.find((attr: Attribute) => attr.id === selectedKeyAttributeItem.value) || clone(emptyAttribute);
    })

    function onAddNetworkDialog() {
        addNetworkDialogData.showDialog = true;
    }

    function addNetwork(network: Network)
    {
        selectNetworkItems.value.push({
            text: network.name,
            value: network.id
        });
        
        isNewNetwork = true;
        selectNetworkItemValue.value = network.id;
        selectedNetwork.value = clone(network);
        hasSelectedNetwork.value = true;
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
        selectedAttributeRows.value = clone(attributeRows.value)
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
            hasSelectedNetwork.value = false;
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
        pagination.totalItems = attributeRows.value.length
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