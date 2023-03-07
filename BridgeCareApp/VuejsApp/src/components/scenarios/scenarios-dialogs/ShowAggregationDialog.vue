<template>
    <v-dialog
        scrollable
        persistent
        v-model="dialogData.showDialog"
        max-width="60%"
        transition="dialog-bottom-transition"
    >
        <v-card elevation="5" outlined >
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
            <v-flex xs10>
                <v-layout>
                    <div class="network-min-width">
                        <v-data-table
                            :headers="networkGridHeaders"
                            sort-icon=$vuetify.icons.ghd-table-sort
                            :items="networks"
                            :items-per-page="5"
                            class="elevation-1"
                            hide-actions
                        >
                            <template slot="items" slot-scope="props">
                                <td>{{ props.item.name }}</td>
                                <td>{{ props.item.createdDate }}</td>
                                <td class="text-xs-center">
                                    <v-menu
                                        left
                                        min-height="500px"
                                        min-width="500px"
                                    >
                                        <template slot="activator">
                                            <v-btn class="ara-blue" icon>
                                                <v-icon>fas fa-eye</v-icon>
                                            </v-btn>
                                        </template>
                                        <v-card>
                                            <v-card-text>
                                                <v-textarea
                                                    class="sm-txt"
                                                    :value="
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
                                    <v-layout row wrap>
                                        <v-flex class="play-button-center">
                                            <v-btn
                                                @click="
                                                    onShowConfirmDataAggregationAlert(
                                                        props.item.id,
                                                    )
                                                "
                                                class="green--text darken-1"
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
                                        </v-flex>
                                    </v-layout>
                                </td>
                            </template>
                        </v-data-table>
                    </div>
                </v-layout>
                <EquationEditorDialog
                    :dialogData="equationEditorDialogData"
                    @submit="onSubmitEquationEditorDialogSubmit"
                />
                <ConfirmDataAssignmentAlert
                    :dialogData="confirmDataAggregationAlertData"
                    @submit="onConfirmDataAggregationAlertSubmit"
                />
            </v-flex>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
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
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import { Hub } from '@/connectionHub';

@Component({
    components: {
        EquationEditorDialog,
        ConfirmDataAssignmentAlert: Alert,
    },
})
export default class ShowAggregationDialog extends Vue {
    @Prop() dialogData: any;
    @State(state => state.networkModule.networks) stateNetworks: Network[];

    @Action('aggregateNetworkData') aggregateNetworkDataAction: any;
    @Action('upsertBenefitQuantifier') upsertBenefitQuantifierAction: any;

    equationEditorDialogData: EquationEditorDialogData = clone(
        emptyEquationEditorDialogData,
    );
    confirmDataAggregationAlertData: AlertData = clone(emptyAlertData);

    networkDataAssignmentStatus: string = '';
    networkDataAssignmentPercentage = 0;
    networkGridHeaders: DataTableHeader[] = [
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
    networks: Network[] = [];
    selectedNetworkId: string = '';

    @Watch('stateNetworks')
    onStateNetworksChanged() {
        this.networks = clone(this.stateNetworks);
    }
    mounted() {
        this.networks = clone(this.stateNetworks);

        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            this.getDataAggregationStatus,
        );
    }

    onShowEquationEditorDialog(equation: Equation, networkId: string) {
        this.selectedNetworkId = networkId;
        this.equationEditorDialogData = {
            showDialog: true,
            equation: equation,
        };
    }
    onSubmitEquationEditorDialogSubmit(equation: Equation) {
        this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

        if (!isNil(equation) && hasValue(this.networks)) {
            var localNetworkObj = this.networks.find(_ => _.id == this.selectedNetworkId);

            if(isNil(localNetworkObj)){
                return `no network found with networkId ${this.selectedNetworkId}`;
            }
            this.upsertBenefitQuantifierAction({
                benefitQuantifier: {
                    ...localNetworkObj.benefitQuantifier,
                    equation: equation,
                },
            });
        }
    }

    onShowConfirmDataAggregationAlert(networkId: string) {
        this.selectedNetworkId = networkId;
        this.confirmDataAggregationAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'The data aggregation operation can take around 1 hour to finish. ' +
                'Are you sure that you want to continue?',
        };
    }

    onConfirmDataAggregationAlertSubmit(response: boolean) {
        this.confirmDataAggregationAlertData = clone(emptyAlertData);

        if (response) {
            this.aggregateNetworkDataAction({
                networkId: this.selectedNetworkId,
            });
        }
    }

    getDataAggregationStatus(data: any) {
        const networkRollupDetail: NetworkRollupDetail = data.networkRollupDetail as NetworkRollupDetail;
        if (any(propEq('id', networkRollupDetail.networkId), this.networks)) {
            const updatedNetwork: Network = find(
                propEq('id', networkRollupDetail.networkId),
                this.networks,
            ) as Network;
            updatedNetwork.status = networkRollupDetail.status;
            updatedNetwork.networkDataAssignmentPercentage = data.percentage as number;

            this.networks = update(
                findIndex(propEq('id', updatedNetwork.id), this.networks),
                updatedNetwork,
                this.networks,
            );
        }
    }
    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
            this.getDataAggregationStatus,
        );
    }
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
