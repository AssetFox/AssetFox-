<template>
    <v-layout column>
        <v-flex x12>
            <v-card elevation="5" color="blue lighten-5">
                <v-tabs center-active v-model="tab">
                    <v-tabs-slider color="blue"></v-tabs-slider>
                    <v-tab
                        v-for="item in tabItems"
                        :key="item.name"
                        class="tab-theme"
                    >
                        <v-icon left>{{ item.icon }}</v-icon>
                        {{ item.name }}
                        ( {{ item.count }} )</v-tab
                    >
                    <v-spacer></v-spacer>
                    <v-btn v-if="isAdmin"
                        class="green darken-2 white--text"
                        @click="onShowAggregatePopup"
                    >
                        Aggregate Data
                    </v-btn>
                    <v-flex xs1></v-flex>
                </v-tabs>
                <v-tabs-items v-model="tab">
                    <v-tab-item>
                        <v-flex x12>
                            <v-card elevation="5">
                                <v-card-title>
                                    <v-flex xs6>
                                        <v-text-field
                                            type="text"
                                            placeholder="Search in scenarios"
                                            append-icon="fas fa-search"
                                            hide-details
                                            single-line
                                            v-model="searchMine"
                                            outline
                                        >
                                        </v-text-field>
                                    </v-flex>
                                    <v-flex xs4></v-flex>
                                    <v-flex class="justify-end xs2">
                                        <v-btn
                                            @click="
                                                showCreateScenarioDialog = true
                                            "
                                            color="blue darken-2 white--text"
                                        >
                                            Create new scenario
                                        </v-btn>
                                    </v-flex>
                                </v-card-title>
                                <v-data-table
                                    :headers="scenarioGridHeaders"
                                    :items="userScenarios"
                                    :search="searchMine"
                                    calculate-widths
                                >
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-edit-dialog
                                                large
                                                lazy
                                                persistent
                                                :return-value.sync="
                                                    props.item.name
                                                "
                                                @save="
                                                    onEditScenarioName(
                                                        props.item,
                                                        nameUpdate,
                                                    )
                                                "
                                                @open="
                                                    prepareForNameEdit(
                                                        props.item.name,
                                                    )
                                                "
                                            >
                                                {{ props.item.name }}
                                                <template slot="input">
                                                    <v-text-field
                                                        label="Edit"
                                                        single-line
                                                        v-model="nameUpdate"
                                                        :rules="[
                                                            rules[
                                                                'generalRules'
                                                            ].valueIsNotEmpty,
                                                        ]"
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            {{
                                                props.item.creator
                                                    ? props.item.creator
                                                    : '[ Unknown ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                props.item.owner
                                                    ? props.item.owner
                                                    : '[ No Owner ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                props.item.networkName
                                                    ? props.item.networkName
                                                    : '[ Unknown ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                formatDate(
                                                    props.item.createdDate,
                                                )
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                formatDate(
                                                    props.item.lastModifiedDate,
                                                )
                                            }}
                                        </td>
                                        <td>
                                            {{ formatDate(props.item.lastRun) }}
                                        </td>
                                        <td>{{ props.item.status }}</td>
                                        <td>{{ props.item.runTime }}</td>
                                        <td>{{ props.item.reportStatus }}</td>
                                        <td>
                                            <v-menu offset-x left>
                                                <template
                                                    v-slot:activator="{
                                                        on,
                                                        attrs,
                                                    }"
                                                >
                                                    <v-btn
                                                        color="green--text darken-1"
                                                        icon
                                                        v-bind="attrs"
                                                        v-on="on"
                                                    >
                                                        <i
                                                            class="fas fa-ellipsis-v"
                                                        ></i>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile
                                                        v-for="(item,
                                                        i) in actionItems"
                                                        :key="i"
                                                        @click="
                                                            OnActionTaken(
                                                                item.action,
                                                                props.item
                                                                    .users,
                                                                props.item,
                                                                true,
                                                            )
                                                        "
                                                        class="menu-style"
                                                    >
                                                        <v-list-tile-title icon>
                                                            <v-icon
                                                                class="action-icon-padding"
                                                                >{{
                                                                    item.icon
                                                                }}</v-icon
                                                            >
                                                            {{
                                                                item.title
                                                            }}</v-list-tile-title
                                                        >
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                    <v-alert
                                        :value="true"
                                        class="ara-orange-bg"
                                        icon="fas fa-exclamation"
                                        slot="no-results"
                                    >
                                        Your search for "{{ searchMine }}" found
                                        no results.
                                    </v-alert>
                                </v-data-table>
                            </v-card>
                        </v-flex>
                    </v-tab-item>
                    <v-tab-item>
                        <v-flex xs12>
                            <v-card elevation="5">
                                <v-card-title>
                                    <v-flex xs6>
                                        <v-text-field
                                            label="Search"
                                            placeholder="Search in scenarios"
                                            outline
                                            append-icon="fas fa-search"
                                            hide-details
                                            single-line
                                            v-model="searchShared"
                                        >
                                        </v-text-field>
                                    </v-flex>
                                </v-card-title>
                                <v-data-table
                                    :headers="scenarioGridHeaders"
                                    :items="sharedScenarios"
                                    :search="searchShared"
                                >
                                    <template slot="items" slot-scope="props">
                                        <td>
                                            <v-edit-dialog
                                                large
                                                lazy
                                                persistent
                                                :return-value.sync="
                                                    props.item.name
                                                "
                                                @save="
                                                    onEditScenarioName(
                                                        props.item,
                                                        nameUpdate,
                                                    )
                                                "
                                                @open="
                                                    prepareForNameEdit(
                                                        props.item.name,
                                                    )
                                                "
                                            >
                                                {{ props.item.name }}
                                                <template slot="input">
                                                    <v-text-field
                                                        label="Edit"
                                                        single-line
                                                        v-model="nameUpdate"
                                                        :rules="[
                                                            rules[
                                                                'generalRules'
                                                            ].valueIsNotEmpty,
                                                        ]"
                                                    />
                                                </template>
                                            </v-edit-dialog>
                                        </td>
                                        <td>
                                            {{
                                                props.item.creator
                                                    ? props.item.creator
                                                    : '[ Unknown ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                props.item.owner
                                                    ? props.item.owner
                                                    : '[ No Owner ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                props.item.networkName
                                                    ? props.item.networkName
                                                    : '[ Unknown ]'
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                formatDate(
                                                    props.item.createdDate,
                                                )
                                            }}
                                        </td>
                                        <td>
                                            {{
                                                formatDate(
                                                    props.item.lastModifiedDate,
                                                )
                                            }}
                                        </td>
                                        <td>
                                            {{ formatDate(props.item.lastRun) }}
                                        </td>
                                        <td>{{ props.item.status }}</td>
                                        <td>{{ props.item.runTime }}</td>
                                        <td>{{ props.item.reportStatus }}</td>
                                        <td>
                                            <v-menu offset-x left>
                                                <template
                                                    v-slot:activator="{
                                                        on,
                                                        attrs,
                                                    }"
                                                >
                                                    <v-btn
                                                        color="green--text darken-1"
                                                        icon
                                                        v-bind="attrs"
                                                        v-on="on"
                                                    >
                                                        <i
                                                            class="fas fa-ellipsis-v"
                                                        ></i>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile
                                                        v-for="(item,
                                                        i) in actionItemsForSharedScenario"
                                                        :key="i"
                                                        @click="
                                                            OnActionTaken(
                                                                item.action,
                                                                props.item
                                                                    .users,
                                                                props.item,
                                                                false,
                                                            )
                                                        "
                                                        class="menu-style"
                                                    >
                                                        <v-list-tile-title icon>
                                                            <v-icon
                                                                class="action-icon-padding"
                                                                >{{
                                                                    item.icon
                                                                }}</v-icon
                                                            >
                                                            {{
                                                                item.title
                                                            }}</v-list-tile-title
                                                        >
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                    <v-alert
                                        :value="true"
                                        class="ara-orange-bg"
                                        icon="fas fa-exclamation"
                                        slot="no-results"
                                    >
                                        Your search for "{{ searchShared }}"
                                        found no results.
                                    </v-alert>
                                </v-data-table>
                            </v-card>
                        </v-flex>
                    </v-tab-item>
                </v-tabs-items>
            </v-card>
        </v-flex>

        <!--    <CreateNetworkDialog :showDialog="showCreateNetworkDialog" @submit="onCreateNetworkDialogSubmit"/>-->

        <!--    <ConfirmRollupAlert :dialogData="confirmRollupAlertData" @submit="onConfirmRollupAlertSubmit"/>-->

        <ConfirmAnalysisRunAlert
            :dialogData="confirmAnalysisRunAlertData"
            @submit="onConfirmAnalysisRunAlertSubmit"
        />

        <ReportsDownloaderDialog :dialogData="reportsDownloaderDialogData" />

        <ShareScenarioDialog
            :dialogData="shareScenarioDialogData"
            @submit="onShareScenarioDialogSubmit"
        />

        <ConfirmCloneScenarioAlert
            :dialogData="confirmCloneScenarioAlertData"
            @submit="onConfirmCloneScenarioAlertSubmit"
        />

        <ConfirmDeleteAlert
            :dialogData="confirmDeleteAlertData"
            @submit="onConfirmDeleteAlertSubmit"
        />

        <CreateScenarioDialog
            :showDialog="showCreateScenarioDialog"
            @submit="onCreateScenarioDialogSubmit"
        />

        <CloneScenarioDialog
            :dialogData="cloneScenarioDialogData"
            @submit="onCloneScenarioDialogSubmit"
        />

        <MigrateLegacySimulationDialog
            :showDialog="showMigrateLegacySimulationDialog"
            @submit="onMigrateLegacySimulationSubmit"
        />
        <ShowAggregationDialog :dialogData="aggragateDialogData" />
        <Alert
            :dialogData="alertDataForDeletingCommittedProjects"
            @submit="onDeleteCommittedProjectsSubmit"
        />

        <CommittedProjectsFileUploaderDialog
            :showDialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
            @delete="onDeleteCommittedProjects"
        />
    </v-layout>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import moment from 'moment';
import {
    emptyScenario,
    Scenario,
    ScenarioActions,
    TabItems,
    ScenarioUser,
} from '@/shared/models/iAM/scenario';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import ReportsDownloaderDialog from '@/components/scenarios/scenarios-dialogs/ReportsDownloaderDialog.vue';
import ShowAggregationDialog from '@/components/scenarios/scenarios-dialogs/ShowAggregationDialog.vue';
import {
    emptyReportsDownloadDialogData,
    ReportsDownloaderDialogData,
} from '@/shared/models/modals/reports-downloader-dialog-data';
import CloneScenarioDialog from '@/components/scenarios/scenarios-dialogs/CloneScenarioDialog.vue'
import { CloneScenarioDialogData, emptyCloneScenarioDialogData } from '@/shared/models/modals/clone-scenario-dialog-data'
import CreateScenarioDialog from '@/components/scenarios/scenarios-dialogs/CreateScenarioDialog.vue';
import ShareScenarioDialog from '@/components/scenarios/scenarios-dialogs/ShareScenarioDialog.vue';
import { Network } from '@/shared/models/iAM/network';
import { any, clone, find, findIndex, isNil, propEq, update } from 'ramda';
import { getUserName } from '@/shared/utils/get-user-info';
import {
    InputValidationRules,
    rules,
} from '@/shared/utils/input-validation-rules';
import CreateNetworkDialog from '@/components/scenarios/scenarios-dialogs/CreateNetworkDialog.vue';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
    emptyShareScenarioDialogData,
    ShareScenarioDialogData,
} from '@/shared/models/modals/share-scenario-dialog-data';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import MigrateLegacySimulationDialog from '@/components/scenarios/scenarios-dialogs/MigrateLegacySimulationDialog.vue';
import { Hub } from '@/connectionHub';
import CommittedProjectsService from '@/services/committed-projects.service';
import { AxiosResponse } from 'axios';
import { http2XX } from '@/shared/utils/http-utils';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';
import { FileInfo } from '@/shared/models/iAM/file-info';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import FileDownload from 'js-file-download';
import ImportExportCommittedProjectsDialog from './scenarios-dialogs/ImportExportCommittedProjectsDialog.vue';

@Component({
    components: {
        MigrateLegacySimulationDialog,
        ConfirmCloneScenarioAlert: Alert,
        ConfirmDeleteAlert: Alert,
        ConfirmRollupAlert: Alert,
        ConfirmAnalysisRunAlert: Alert,
        ReportsDownloaderDialog,
        CreateScenarioDialog,
        CloneScenarioDialog,
        CreateNetworkDialog,
        ShareScenarioDialog,
        ShowAggregationDialog,
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        Alert
    },
})
export default class Scenarios extends Vue {
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.scenarioModule.scenarios) stateScenarios: Scenario[];

    @State(state => state.breadcrumbModule.navigation) navigation: any[];

    @State(state => state.authenticationModule.authenticated)
    authenticated: boolean;
    @State(state => state.authenticationModule.userId) userId: string;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @State(state => state.authenticationModule.isCWOPA) isCWOPA: boolean;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
    @Action('getScenarios') getScenariosAction: any;
    @Action('createScenario') createScenarioAction: any;
    @Action('cloneScenario') cloneScenarioAction: any;
    @Action('updateScenario') updateScenarioAction: any;
    @Action('deleteScenario') deleteScenarioAction: any;
    @Action('runSimulation') runSimulationAction: any;
    @Action('migrateLegacySimulationData')
    migrateLegacySimulationDataAction: any;
    @Action('updateSimulationAnalysisDetail')
    updateSimulationAnalysisDetailAction: any;
    @Action('updateSimulationReportDetail')
    updateSimulationReportDetailAction: any;
    @Action('updateNetworkRollupDetail') updateNetworkRollupDetailAction: any;
    @Action('selectScenario') selectScenarioAction: any;

    //@Action('rollupNetwork') rollupNetworkAction: any;
    //@Action('createNetwork') createNetworkAction: any;
    @Action('upsertBenefitQuantifier') upsertBenefitQuantifierAction: any;
    @Action('aggregateNetworkData') aggregateNetworkDataAction: any;

    networks: Network[] = [];
    scenarioGridHeaders: DataTableHeader[] = [
        {
            text: 'Scenario',
            value: 'name',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Creator',
            value: 'creator',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Owner',
            value: 'owner',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Network',
            value: 'network',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Date Created',
            value: 'createdDate',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Date Last Modified',
            value: 'lastModifiedDate',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Date Last Run',
            value: 'lastRun',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Status',
            value: 'status',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Run Time',
            value: 'runTime',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Report Status',
            value: 'reportStatus',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Action',
            value: 'actions',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: '',
            value: '',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
    ];

    actionItems: ScenarioActions[] = [];
    actionItemsForSharedScenario: ScenarioActions[] = [];
    tabItems: TabItems[] = [];
    tab: string = '';
    availableActions: any;
    nameUpdate: string = '';
    scenarios: Scenario[] = [];
    userScenarios: Scenario[] = [];
    sharedScenarios: Scenario[] = [];
    searchMine = '';
    searchShared = '';
    //confirmRollupAlertData: AlertData = clone(emptyAlertData);
    //showCreateNetworkDialog: boolean = false;
    reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(
        emptyReportsDownloadDialogData,
    );
    confirmAnalysisRunAlertData: AlertData = clone(emptyAlertData);
    shareScenarioDialogData: ShareScenarioDialogData = clone(
        emptyShareScenarioDialogData,
    );
    confirmCloneScenarioAlertData: AlertData = clone(emptyAlertData);
    cloneScenarioDialogData: CloneScenarioDialogData = clone(emptyCloneScenarioDialogData);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    showCreateScenarioDialog: boolean = false;
    selectedScenario: Scenario = clone(emptyScenario);
    networkDataAssignmentStatus: string = '';
    rules: InputValidationRules = rules;
    showMigrateLegacySimulationDialog: boolean = false;
    showImportExportCommittedProjectsDialog: boolean = false;
    alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    selectedScenarioId: string = "";

    aggragateDialogData: any = { showDialog: false };

    @Watch('stateNetworks')
    onStateNetworksChanged() {
        this.networks = clone(this.stateNetworks);
        if (hasValue(this.networks)) {
            this.getScenariosAction();
        }
    }

    @Watch('stateScenarios')
    onStateScenariosChanged() {
        this.scenarios = clone(this.stateScenarios);
    }

    @Watch('scenarios')
    onScenariosChanged() {
        const username: string = getUserName();
        // filter scenarios that are the user's
        this.userScenarios = this.scenarios.filter(
            (scenario: Scenario) => scenario.owner === username,
        );
        // filter scenarios that are shared with the user
        const scenarioUserCanModify = (user: ScenarioUser) =>
            user.username === username;
        const sharedScenarioFilter = (scenario: Scenario) =>
            scenario.owner !== username &&
            (this.isAdmin ||
                this.isCWOPA ||
                any(scenarioUserCanModify, scenario.users));
        this.sharedScenarios = this.scenarios.filter(sharedScenarioFilter);

        this.tabItems[0].count = this.userScenarios.length;
        this.tabItems[1].count = this.sharedScenarios.length;
    }

    mounted() {
        this.networks = clone(this.stateNetworks);
        if (hasValue(this.networks) && !hasValue(this.stateScenarios)) {
            this.getScenariosAction();
        } else {
            this.scenarios = clone(this.stateScenarios);
        }

        // this.$statusHub.$on(
        //     Hub.BroadcastEventType.BroadcastAssignDataStatusEvent,
        //     this.getDataAggregationStatus,
        // );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastDataMigrationEvent,
            this.getDataMigrationStatus,
        );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent,
            this.getScenarioAnalysisDetailUpdate,
        );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastSummaryReportGenerationStatusEvent,
            this.getSummaryReportStatus,
        );

        this.availableActions = {
            runAnalysis: 'runAnalysis',
            reports: 'reports',
            settings: 'settings',
            share: 'share',
            clone: 'clone',
            delete: 'delete',
            commitedProjects: 'commitedProjects'
        };
        this.actionItemsForSharedScenario = [
            {
                title: 'Run Analysis',
                action: this.availableActions.runAnalysis,
                icon: 'fas fa-play',
            },
            {
                title: 'Reports',
                action: this.availableActions.reports,
                icon: 'fas fa-chart-line',
            },
            {
                title: 'Settings',
                action: this.availableActions.settings,
                icon: 'fas fa-edit',
            },
            {
                title: 'Clone',
                action: this.availableActions.clone,
                icon: 'fas fa-paste',
            },
            {
                title: 'Delete',
                action: this.availableActions.delete,
                icon: 'fas fa-trash',
            },
            {
                title: 'Committed Projects',
                action: this.availableActions.commitedProjects,
                icon: 'fas fa-trash',
            }
        ];
        this.actionItems = this.actionItemsForSharedScenario.slice();
        this.actionItems.splice(3, 0, {
            title: 'Share',
            action: this.availableActions.share,
            icon: 'fas fa-users',
        });
        this.tabItems.push(
            { name: 'My scenarios', icon: 'star', count: 0 },
            { name: 'Shared with me', icon: 'share', count: 0 },
        );
        this.tab = 'My scenarios';
    }

    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastDataMigrationEvent,
            this.getDataMigrationStatus,
        );
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent,
            this.getScenarioAnalysisDetailUpdate,
        );
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastSummaryReportGenerationStatusEvent,
            this.getSummaryReportStatus,
        );
    }

    formatDate(dateToFormat: Date) {
        return hasValue(dateToFormat)
            ? moment(dateToFormat).format('M/D/YYYY')
            : null;
    }

    canModifySharedScenario(scenarioUsers: ScenarioUser[]) {
        const currentUser: string = getUserName();
        const scenarioUserCanModify = (user: ScenarioUser) =>
            user.username === currentUser && user.canModify;
        return (
            this.isAdmin ||
            this.isCWOPA ||
            any(scenarioUserCanModify, scenarioUsers)
        );
    }

    /*onShowConfirmRollupAlert() {
      this.confirmRollupAlertData = {
        showDialog: true,
        heading: 'Warning',
        choice: true,
        message: 'The rollup can take around 5 minutes to finish. Continue?'
      }
    }

    onConfirmRollupAlertSubmit(submit: boolean) {
      this.confirmRollupAlertData = clone(emptyAlertData);

      if (submit) {
        this.rollupNetworkAction({
          networkId: this.networks[0].id,
        });
      }
    }*/

    /*onCreateNetworkDialogSubmit(network: Network) {
      this.showCreateNetworkDialog = false;

      if (!isNil(network)) {
        this.createNetworkAction({network: network});
      }
    }*/

    // TODO: update to send no payload when API is modified to migrate ALL simulations
    onStartDataMigration() {
        // the legacy scenario id is hardcoded to our test scenario "JML Run District 8"
        this.migrateLegacySimulationDataAction({
            simulationId: process.env.VUE_APP_HARDCODED_SCENARIOID_FROM_LEGACY,
        });
    }

    onEditScenarioName(scenario: Scenario, name: string) {
        scenario.name = name;
        if (hasValue(scenario.name)) {
            this.updateScenarioAction({ scenario: scenario });
        } else {
            this.scenarios = [];
            setTimeout(() => (this.scenarios = clone(this.stateScenarios)));
        }
    }

    prepareForNameEdit(name: string) {
        this.nameUpdate = name;
    }

    onShowConfirmAnalysisRunAlert(scenario: Scenario) {
        this.selectedScenario = clone(scenario);

        this.confirmAnalysisRunAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Only one simulation can be run at a time. The model run you are about to queue will be ' +
                'executed in the order in which it was received.',
        };
    }

    onConfirmAnalysisRunAlertSubmit(submit: boolean) {
        this.confirmAnalysisRunAlertData = clone(emptyAlertData);

        if (submit && this.selectedScenario.id !== getBlankGuid()) {
            this.runSimulationAction({
                networkId: this.selectedScenario.networkId,
                scenarioId: this.selectedScenario.id,
            }).then(() => (this.selectedScenario = clone(emptyScenario)));
        }
    }

    onShowReportsDownloaderDialog(scenario: Scenario) {
        console.log(scenario.networkId);
        this.reportsDownloaderDialogData = {
            showModal: true,
            scenarioId: scenario.id,
            networkId: scenario.networkId,
            name: scenario.name,
        };
    }

    onNavigateToEditScenarioView(localScenario: Scenario) {
        this.selectScenarioAction({ scenarioId: localScenario.id });

        this.$router.push({
            path: '/EditScenario/',
            query: {
                scenarioId: localScenario.id,
                networkId: localScenario.networkId,
                scenarioName: localScenario.name,
                networkName: localScenario.networkName,
            },
        });
    }

    onShowShareScenarioDialog(scenario: Scenario) {
        this.shareScenarioDialogData = {
            showDialog: true,
            scenario: clone(scenario),
        };
    }

    onShareScenarioDialogSubmit(scenarioUsers: ScenarioUser[]) {
        const scenario: Scenario = {
            ...this.shareScenarioDialogData.scenario,
            users: [],
        };

        this.shareScenarioDialogData = clone(emptyShareScenarioDialogData);

        if (!isNil(scenarioUsers) && scenario.id !== getBlankGuid()) {
            this.updateScenarioAction({
                scenario: { ...scenario, users: scenarioUsers },
            });
        }
    }

    onShowConfirmCloneScenarioAlert(scenario: Scenario) {
        this.selectedScenario = clone(scenario);

        this.confirmCloneScenarioAlertData = {
            showDialog: true,
            heading: 'Clone Scenario',
            choice: true,
            message: 'Are you sure you want clone this scenario?',
        };
    }

    onShowCloneScenarioDialog(scenario: Scenario) {
        this.selectedScenario = clone(scenario);

        this.cloneScenarioDialogData = {
            showDialog: true,
            scenario: this.selectedScenario
        };
    }

    onConfirmCloneScenarioAlertSubmit(submit: boolean) {
        this.confirmCloneScenarioAlertData = clone(emptyAlertData);

        if (submit && this.selectedScenario.id !== getBlankGuid()) {
            this.cloneScenarioAction({
                scenarioId: this.selectedScenario.id,
            }).then(() => (this.selectedScenario = clone(emptyScenario)));
        }
    }

    onCloneScenarioDialogSubmit(scenario: Scenario) {
        this.cloneScenarioDialogData = clone(emptyCloneScenarioDialogData);

        if (!isNil(scenario)) {
            this.cloneScenarioAction({
                scenarioId: scenario.id,
                networkId: scenario.networkId,
                scenarioName: scenario.name
            }).then(() => (this.selectedScenario = clone(emptyScenario)));
        }
    }

    onShowConfirmDeleteAlert(scenario: Scenario) {
        this.selectedScenario = clone(scenario);

        this.confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }

    onConfirmDeleteAlertSubmit(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit && this.selectedScenario.id !== getBlankGuid()) {
            this.deleteScenarioAction({
                scenarioId: this.selectedScenario.id,
            }).then(() => (this.selectedScenario = clone(emptyScenario)));
        }
    }

    getDataMigrationStatus(data: any) {
        const status: any = data.status;
        if (status.indexOf('Error') !== -1) {
            this.addErrorNotificationAction({
                message: 'Data migration error.',
                longMessage: data.status,
            });
        } else {
            this.addInfoNotificationAction({
                message: 'Data migration info.',
                longMessage: data.status,
            });
        }
    }

    getScenarioAnalysisDetailUpdate(data: any) {
        this.updateSimulationAnalysisDetailAction({
            simulationAnalysisDetail: data.simulationAnalysisDetail,
        });
    }

    getSummaryReportStatus(data: any) {
        this.updateSimulationReportDetailAction({
            simulationReportDetail: data.simulationReportDetail,
        });
    }

    onCreateScenarioDialogSubmit(scenario: Scenario) {
        this.showCreateScenarioDialog = false;

        if (!isNil(scenario)) {
            this.createScenarioAction({
                scenario: scenario,
                networkId: scenario.networkId,
            });
        }
    }

    onMigrateLegacySimulationSubmit(legacySimulationId: number) {
        this.showMigrateLegacySimulationDialog = false;

        if (!isNil(legacySimulationId)) {
            this.migrateLegacySimulationDataAction({
                simulationId: legacySimulationId,
                networkId: 'D7B54881-DD44-4F93-8250-3D4A630A4D3B',
            });
        }
    }

    onSubmitImportExportCommittedProjectsDialogResult(
        result: ImportExportCommittedProjectsDialogResult,
    ) {
        this.showImportExportCommittedProjectsDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {
                CommittedProjectsService.exportCommittedProjects(
                    this.selectedScenarioId,
                ).then((response: AxiosResponse) => {
                    if (hasValue(response, 'data')) {
                        const fileInfo: FileInfo = response.data as FileInfo;
                        FileDownload(
                            convertBase64ToArrayBuffer(fileInfo.fileData),
                            fileInfo.fileName,
                            fileInfo.mimeType,
                        );
                    }
                });
            } else {
                if (hasValue(result.file)) {
                    CommittedProjectsService.importCommittedProjects(
                        result.file,
                        result.applyNoTreatment,
                        this.selectedScenarioId,
                    ).then((response: AxiosResponse) => {
                        if (
                            hasValue(response, 'status') &&
                            http2XX.test(response.status.toString())
                        ) {
                            this.addSuccessNotificationAction({
                                message: 'Successful upload.',
                                longMessage:
                                    'Successfully uploaded committed projects.',
                            });
                        }
                    });
                } else {
                    this.addErrorNotificationAction({
                        message: 'No file selected.',
                        longMessage:
                            'No file selected to upload the committed projects.',
                    });
                }
            }
        }
    }

    onDeleteCommittedProjects() {
        this.alertDataForDeletingCommittedProjects = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }

    onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        this.alertDataForDeletingCommittedProjects = { ...emptyAlertData };

        if (doDelete) {
            CommittedProjectsService.deleteCommittedProjects(
                this.selectedScenarioId,
            ).then((response: AxiosResponse) => {
                if (
                    hasValue(response) &&
                    http2XX.test(response.status.toString())
                ) {
                    this.addSuccessNotificationAction({
                        message: 'Committed projects have been deleted.',
                    });
                }
            });
        }
    }

    OnActionTaken(
        action: string,
        scenarioUsers: ScenarioUser[],
        scenario: Scenario,
        isOwner: boolean,
    ) {
        switch (action) {
            case this.availableActions.runAnalysis:
                if (this.canModifySharedScenario(scenarioUsers) || isOwner) {
                    this.onShowConfirmAnalysisRunAlert(scenario);
                } else {
                }
                break;
            case this.availableActions.reports:
                this.onShowReportsDownloaderDialog(scenario);
                break;
            case this.availableActions.settings:
                if (this.canModifySharedScenario(scenarioUsers) || isOwner) {
                    this.onNavigateToEditScenarioView(scenario);
                }
                break;
            case this.availableActions.share:
                this.onShowShareScenarioDialog(scenario);
                break;
            case this.availableActions.clone:
                this.onShowCloneScenarioDialog(scenario);
                break;
            case this.availableActions.delete:
                this.onShowConfirmDeleteAlert(scenario);
                break;
            case this.availableActions.commitedProjects:
                this.selectedScenarioId = scenario.id;
                this.showImportExportCommittedProjectsDialog = true;
                break;
        }
    }
    onShowAggregatePopup() {
        this.aggragateDialogData = {
            showDialog: true,
        };
    }
}
</script>

<style>
.pad-button {
    padding-top: 33px;
}

.network-min-width {
    min-width: 1000px;
}

.status-min-width {
    min-width: 300px;
}

.menu-style {
    border-bottom: inset;
    padding: 2px;
    padding-right: 15px;
}

.tab-theme {
    border: ridge;
    border-width: 2px;
}
.action-icon-padding {
    padding-right: 14px;
}
.header-border {
  border-bottom: 2px solid black;
}
</style>
