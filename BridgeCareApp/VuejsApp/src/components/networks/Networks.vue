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
                        class="ghd-select ghd-text-field ghd-text-field-border"
                        density="compact">
                    </v-select>                           
                    <v-btn style="margin-top: 2px !important; margin-left: 20px !important" 
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
                            :items='selectKeyAttributeItems'
                            density="compact">
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
                        v-model="selectDataSourceId"
                        density="compact">
                    </v-select>  
                    <v-btn style="margin-top: 2px !important; margin-left: 20px;"  
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
                                    <v-subheader class="ghd-control-label ghd-md-gray" style="padding-top: 14px !important;">                             
                                        Spatial Weighting Equation</v-subheader>
                                </v-col>
                                <v-row style="width: 70% !important; margin-bottom: 5px; margin-left: 1px;; padding-top: 10px" >
                                    <v-text-field style="margin-left: 10px; margin-right: 10px" 
                                    :disabled="!isNewNetwork" density="compact" 
                                    id="Networks-EditSpatialWeightingEquation-vtextfield"
                                    variant="outlined" 
                                    class="ghd-text-field-border ghd-text-field" 
                                    v-model="spatialWeightingEquationValue.expression"/>

                                    <btn id="Networks-EditSpatialWeightingEquation-vbtn"
                                        style="margin-top: 10px; margin-right: 32px; cursor: pointer;"
                                        class="edit-icon ghd-control-label" 
                                        :disabled="!isNewNetwork"
                                        append-icon="ghd-blue"
                                        @click="onShowEquationEditorDialog">
                                        <v-icon v-if="isNewNetwork" class="ghd-blue" variant = "outlined">fas fa-edit</v-icon>
                                    </btn>
                                </v-row>
                            </v-row>
                    </v-row>
                    <v-row v-show="hasStartedAggregation">
                        <v-col>
                            <v-subheader class="ghd-control-label ara-black" v-text="networkDataAssignmentStatus" ></v-subheader>
                            <v-progress-linear
                                            v-model="networkDataAssignmentPercentage"
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
                                <v-btn style="margin-left: 130px" id="Networks-AddAll-vbtn" variant = "flat" class='ghd-blue ghd-button-text ghd-separated-button ghd-button'
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
                            <v-data-table id="Networks-Attributes-vdatatable" :headers='dataSourceGridHeaders' :items='attributeRows'
                                class='v-table__overflow ghd-table' item-key='id'
                                v-model="selectedAttributeRows"
                                :must-sort='true'
                                hide-actions
                                :pagination.sync="pagination">
                                <template slot='items' slot-scope='props' v-slot:item="{item}">
                                    <tr>
                                        <td>
                                            <v-checkbox v-model="selectedAttributeRows"
                                            :value="item" 
                                            id="Networks-SelectAttribute-vcheckbox" hide-details primary></v-checkbox>
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
            <v-row justify="center" style="padding-top: 30px !important">
                <v-btn id="Networks-Cancel-vbtn" :disabled='!hasUnsavedChanges' @click='onDiscardChanges'
                variant = "outlined" class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button vertical-center'>
                    Cancel
                </v-btn>  
                <p>&nbsp;&nbsp;&nbsp;</p>
                <v-btn v-show="!isNewNetwork" id="Networks-Aggregate-vbtn" @click='aggregateNetworkData' :disabled='disableCrudButtonsAggregate()'  class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Aggregate
                </v-btn>
                <p>&nbsp;&nbsp;&nbsp;</p>
                <v-btn v-show="!isNewNetwork" id="Networks-Delete-vbtn" @click='onDeleteClick' :disabled='isNewNetwork'  class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' variant = "outlined">
                    Delete
                </v-btn>
                <p>&nbsp;&nbsp;&nbsp;</p>
                <v-btn v-show="isNewNetwork" id="Networks-Create-vbtn" @click='createNetwork'  :disabled='disableCrudButtonsCreate()'
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
        <ConfirmDialog></ConfirmDialog>
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
import ConfirmDialog from 'primevue/confirmdialog';

    let store = useStore();
    let stateNetworks = computed<Network[]>(()=>store.state.networkModule.networks);
    let stateSelectedNetwork = computed<Network>(()=>store.state.networkModule.selectedNetwork) ;
    let stateAttributes = computed<Attribute[]>(() => store.state.attributeModule.attributes);
    let stateDataSources = computed<Datasource[]>(() => store.state.datasourceModule.dataSources) ;
    let hasUnsavedChanges = computed<boolean>(() => store.state.unsavedChangesFlagModule.hasUnsavedChanges);
    let isAdmin: boolean = (store.state.authenticationModule.isAdmin) ;
    
    async function getNetworks(payload?: any): Promise<any> {await store.dispatch('getNetworks', payload);}
    async function getDataSources(payload?: any): Promise<any> {await store.dispatch('getDataSources', payload);}
    async function getAttributes(payload?: any): Promise<any> {await store.dispatch('getAttributes', payload);}
    async function selectNetworkAction(payload?: any): Promise<any> {await store.dispatch('selectNetwork', payload);}
    async function createNetworkAction(payload?: any): Promise<any> {await store.dispatch('createNetwork', payload);}
    async function deleteNetworkAction(payload?: any): Promise<any> {await store.dispatch('deleteNetwork', payload);}
    async function aggregateNetworkAction(payload?: any): Promise<any> {await store.dispatch('aggregateNetworkData', payload);}
    async function setHasUnsavedChangesAction(payload?: any): Promise<any> {await store.dispatch('setHasUnsavedChanges', payload);}
    async function getUserNameByIdGetter(payload?: any): Promise<any> {await store.dispatch('getUserNameById', payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}

    let rules = ref<InputValidationRules>(validationRules);

    let dataSourceGridHeaders: any[] = [
        { title: 'Name', key: 'name', align: 'left', sortable: true, class: '', width: '' },
        { title: 'Data Source', key: 'data source', align: 'left', sortable: true, class: '', width: '' },
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

    let networkDataAssignmentPercentage = ref<number>(0);
    let networkDataAssignmentStatus = ref<string>('Waiting on server.');

    const selectedKeyAttributeItem = ref<string>('');
    const selectedKeyAttribute = ref<Attribute>(clone(emptyAttribute));
    const selectedNetwork = ref<Network>(clone(emptyNetwork));
    const selectNetworkItemValue = ref<string>('');
    const selectDataSourceId = ref<string>('');
    const hasSelectedNetwork = ref<boolean>(false);
    const isNewNetwork = ref<boolean>(false);
    const hasStartedAggregation = ref<boolean>(false);
    const isKeyPropertySelectedAttribute = ref<boolean>(false);
    const spatialWeightingEquationValue = ref<Equation>(clone(emptyEquation)); //placeholder until network dto and api changes
    const equationEditorDialogData = ref(clone(
        emptyEquationEditorDialogData,
    ));
    
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
        selectNetworkItems.value.push({text:_.name,value:_.id})
        });
    })

    watch(stateAttributes, () => { 
        attributeRows.value = clone(stateAttributes.value);
        stateAttributes.value.forEach(_ => {
        selectKeyAttributeItems.value.push({text:_.name,value:_.id})
        })
        });

    watch(stateDataSources, () => {  
        stateDataSources.value.forEach(_ => {
            selectDataSourceItems.value.push({text:_.name,value:_.id})
        })
    })

    watch(selectNetworkItemValue, () =>  {
        selectNetworkAction(selectNetworkItemValue.value);
        if(selectNetworkItemValue.value != getBlankGuid() || isNewNetwork.value)
            hasSelectedNetwork.value = true;
        else
            hasSelectedNetwork.value = false;
    })

    watch(selectedAttributeRows, () => 
    {
        if(any(propEq('id', selectedNetwork.value.keyAttribute), selectedAttributeRows.value)) {
            isKeyPropertySelectedAttribute.value = true;
        }
        else {
            isKeyPropertySelectedAttribute.value  = false;
        }
    })
    
    watch(stateSelectedNetwork, () => {
        if (isNewNetwork) {
            selectedNetwork.value = clone(stateSelectedNetwork.value);
        }
    })

    watch(selectedNetwork, () => { 
        selectedAttributeRows.value = [];
        hasStartedAggregation.value  = false;
        selectNetworkItemValue.value = selectedNetwork.value.id;
        selectedKeyAttributeItem.value = selectedNetwork.value.keyAttribute;
        spatialWeightingEquationValue.value.expression = selectedNetwork.value.defaultSpatialWeighting;

        const hasUnsavedChanges: boolean = hasUnsavedChangesCore('', selectedNetwork.value, stateSelectedNetwork.value);
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
        addNetworkDialogData.showDialog = false;
        selectNetworkItems.value.push({
            text: network.name,
            value: network.id
        });
        
        isNewNetwork.value  = true;
        selectNetworkItemValue.value = network.id;
        selectedNetwork.value = clone(network);
        hasSelectedNetwork.value = true;
    }
    function onDiscardChanges() {
        selectedNetwork.value = clone(stateSelectedNetwork.value);
    }
    function onSubmitEquationEditorDialogResult(equation: Equation) {
        equationEditorDialogData.value = clone(emptyEquationEditorDialogData);

        if (!isNil(equation)) {
            spatialWeightingEquationValue.value.expression  = clone(equation.expression);
        }
    }
    function onShowEquationEditorDialog() {
        equationEditorDialogData.value = {
            showDialog: true,
            equation: spatialWeightingEquationValue.value,
        };      
    }
    function selectAllFromSource(){
        selectedAttributeRows.value = clone(stateAttributes.value);
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

        hasStartedAggregation.value = true;
    }

    function onDeleteClick(){
        deleteNetworkAction(selectedNetwork.value.id).then(() => {
            hasSelectedNetwork.value = false;
            selectNetworkItemValue.value = "";
            selectedNetwork.value = clone(emptyNetwork)
        })       
    }
    function disableCrudButtonsCreate() {

        let allValid = rules.value['generalRules'].valueIsNotEmpty(selectedNetwork.value.name) === true
            && rules.value['generalRules'].valueIsNotEmpty(spatialWeightingEquationValue.value.expression) === true
            && rules.value['generalRules'].valueIsNotEmpty(selectedKeyAttributeItem.value) === true;


        return !allValid;
    }
    function disableCrudButtonsAggregate() {
        let isKeyPropertySelectedAttribute: Boolean = any(propEq('id', selectedNetwork.value.keyAttribute), selectedAttributeRows.value);
        let allValid = rules.value['generalRules'].valueIsNotEmpty(selectedNetwork.value.name) === true
            && rules.value['generalRules'].valueIsNotEmpty(selectedAttributeRows.value) === true
            && isKeyPropertySelectedAttribute === true
            && hasStartedAggregation.value === false;


        return !allValid;
    }
    function createNetwork(){
        isNewNetwork.value = false;

        createNetworkAction({
            network: selectedNetwork.value.id,
            parameters: {
                defaultEquation: spatialWeightingEquationValue.value.expression,
                networkDefinitionAttribute: selectedKeyAttribute.value
            }
        })

    }

    function getDataAggregationStatus(data: any) {
        const networkRollupDetail: NetworkRollupDetail = data.networkRollupDetail as NetworkRollupDetail;
        if (networkRollupDetail.networkId === selectedNetwork.value.id){
            networkDataAssignmentStatus.value = networkRollupDetail.status;
            networkDataAssignmentPercentage.value = data.percentage as number;
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