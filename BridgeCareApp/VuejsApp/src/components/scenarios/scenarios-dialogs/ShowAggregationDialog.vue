<template>
    <Dialog         
        scrollable
        persistent
        v-bind:show="dialogData.showDialog"
        max-width="60%"
        transition="dialog-bottom-transition"
    >
        <v-card elevation="5" variant = "outlined" >
            <v-card-title>
                <h3 class="dialog-header">
                    Aggregate Data
                </h3>
                <v-spacer></v-spacer>
                <v-btn
                    @click="dialogData.showDialog = false"
                    
                    icon
                >
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>
            <v-col cols = "10">
                <v-row>
                    <div class="network-min-width">
                        <v-data-table
                            :headers="networkGridHeaders"
                            sort-icon=$vuetify.icons.ghd-table-sort
                            :items="networks"
                            :items-per-page="5"
                            class="elevation-1"
                            hide-actions
                        >
                            <template slot="items" slot-scope="props" v-slot:item="props">
                                <td>{{ props.item.name }}</td>
                                <td>{{ props.item.createdDate }}</td>
                                <td class="text-xs-center">
                                    <v-menu
                                        left
                                        min-height="500px"
                                        min-width="500px"
                                    >
                                        <template v-slot:activator>
                                            <v-btn class="ara-blue" icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    class="sm-txt"
                                                    :model-value="
                                                        props.item
                                                            .benefitQuantifier
                                                            .equation.expression
                                                    "
                                                    full-width
                                                    no-resize
                                                    outline
                                                    readonly
                                                    rows="5"
                                                />
                                            </v-card-text>
                                        </v-card>
                                    </v-menu>
                                    <v-btn
                                        @click="
                                            onShowEquationEditorDialog(
                                                props.item.benefitQuantifier
                                                    .equation,
                                                    props.item.id
                                            )
                                        "
                                        class="edit-icon"
                                        icon
                                    >
                                        <v-icon>fas fa-edit</v-icon>
                                    </v-btn>
                                </td>
                                <td class="status-min-width">
                                    {{ props.item.status }}
                                    <v-progress-linear
                                        v-model="
                                            props.item.networkDataAssignmentPercentage
                                        "
                                        color="light-green darken-1"
                                        height="25"
                                        striped
                                    >
                                        <strong
                                            >{{
                                                Math.ceil(
                                                    props.item.networkDataAssignmentPercentage,
                                                )
                                            }}%</strong
                                        >
                                    </v-progress-linear>
                                </td>
                                <td>
                                    <v-row row wrap>
                                        <v-col class="play-button-center">
                                            <v-btn
                                                @click="
                                                    onShowConfirmDataAggregationAlert(
                                                        props.item.id,
                                                    )
                                                "
                                                class="text-green darken-1"
                                                :disabled="
                                                    props.item.benefitQuantifier
                                                        .equation.expression ===
                                                        ''
                                                "
                                                :title="
                                                    props.item.benefitQuantifier
                                                        .equation.expression ===
                                                    ''
                                                        ? 'Add Benefit Quantifier to Aggregate'
                                                        : 'Aggregate'
                                                "
                                                icon
                                            >
                                                <v-icon>fas fa-play</v-icon>
                                            </v-btn>
                                        </v-col>
                                    </v-row>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-row>
                <EquationEditorDialog
                    :dialogData="equationEditorDialogData"
                    @submit="onSubmitEquationEditorDialogSubmit"
                />
                <ConfirmDataAssignmentAlert :is="Alert"
                    :dialogData="confirmDataAggregationAlertData"
                    @submit="onConfirmDataAggregationAlertSubmit"
                />
            </v-col>
        </v-card>
    </Dialog>
</template>

<script lang="ts" setup>
import Alert from '@/shared/modals/Alert.vue';
import EquationEditorDialog from '@/shared/modals/EquationEditorDialog.vue';
import { Equation } from '@/shared/models/iAM/equation';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { NetworkRollupDetail } from '@/shared/models/iAM/network-rollup-detail';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import {
    emptyEquationEditorDialogData,
    EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import { hasValue } from '@/shared/utils/has-value-util';
import { any, clone, find, findIndex, isNil, propEq, update } from 'ramda';
import { Hub } from '@/connectionHub';
import Vue, { Ref, ref, shallowReactive, shallowRef, watch, onMounted, onBeforeUnmount, inject } from 'vue'; 
import { useStore } from 'vuex'; 
import Dialog from 'primevue/dialog';
import mitt from 'mitt';

    let store = useStore();     
    const $emitter = mitt()

    const props = defineProps<{dialogData: any}>();

    const stateNetworks: Network[] = shallowReactive(store.state.networkModule.networks);

    async function aggregateNetworkDataAction(payload?: any): Promise<any>{await store.dispatch('aggregateNetworkData')}
    async function upsertBenefitQuantifierAction(payload?: any): Promise<any>{await store.dispatch('upsertBenefitQuantifier')} 

    let equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    let confirmDataAggregationAlertData: AlertData = clone(emptyAlertData);
    
    let networkDataAssignmentStatus: string = '';
    let networkDataAssignmentPercentage = 0;
    let networkGridHeaders: DataTableHeader[] = [
        {
            text: 'Network',
            value: 'name',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Date Created',
            value: 'createdDate',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Benefit Quantifier',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Status',
            value: 'status',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
        {
            text: 'Aggregate Data',
            value: '',
            align: 'left',
            sortable: false,
            class: '',
            width: '',
        },
    ];
    let networks: Network[] = [];
    let selectedNetworkId: string = '';

    watch(stateNetworks, ()=> onStateNetworksChanged)
    function onStateNetworksChanged() {
        networks = clone(stateNetworks);
    }

    onMounted(() => mounted)
    function mounted() {
        networks = clone(stateNetworks);

        $emitter.on(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }

    function onShowEquationEditorDialog(equation: Equation, networkId: string) {
        selectedNetworkId = networkId;
        equationEditorDialogData = {
            showDialog: true,
            equation: equation,
        };
    }
    function onSubmitEquationEditorDialogSubmit(equation: Equation) {
        equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && hasValue(networks)) {
            var localNetworkObj = networks.find(_ => _.id == selectedNetworkId);

            if(isNil(localNetworkObj)){
                return `no network found with networkId ${selectedNetworkId}`;
            }
            upsertBenefitQuantifierAction({
                benefitQuantifier: {
                    ...localNetworkObj.benefitQuantifier,
                    equation: equation,
                },
            });
        }
    }

    function onShowConfirmDataAggregationAlert(networkId: string) {
        selectedNetworkId = networkId;
        confirmDataAggregationAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'The data aggregation operation can take around 1 hour to finish. ' +
                'Are you sure that you want to continue?',
        };
    }

    function onConfirmDataAggregationAlertSubmit(response: boolean) {
        confirmDataAggregationAlertData = clone(emptyAlertData);

        if (response) {
            aggregateNetworkDataAction({
                networkId: selectedNetworkId,
            });
        }
    }

    function getDataAggregationStatus(data: any) {
        const networkRollupDetail: NetworkRollupDetail = data.networkRollupDetail as NetworkRollupDetail;
        if (any(propEq('id', networkRollupDetail.networkId), networks)) {
            const updatedNetwork: Network = find(
                propEq('id', networkRollupDetail.networkId),
                networks,
            ) as Network;
            updatedNetwork.status = networkRollupDetail.status;
            updatedNetwork.networkDataAssignmentPercentage = data.percentage as number;

            networks = update(
                findIndex(propEq('id', updatedNetwork.id), networks),
                updatedNetwork,
                networks,
            );
        }
    }

    onBeforeUnmount(() => beforeDestroy); 
    function beforeDestroy() {
        $emitter.off(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            getDataAggregationStatus,
        );
    }

</script>

<style scoped>
.network-min-width {
    min-width: 1140px;
}

.play-button-center {
    text-align: -webkit-center;
}
.agg-pop-height {min-height: 300px}
</style>
