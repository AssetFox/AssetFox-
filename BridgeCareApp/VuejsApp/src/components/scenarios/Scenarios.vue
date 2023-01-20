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
                        <!-- <img class="icon-selected-tab" style="padding-right:10px" v-bind:src="item.icon"/> -->
                        <GhdQueueSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'Simulation queue'"/> 
                        <GhdShareSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'Shared with me'"/>  
                        <GhdStarSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'My scenarios'"/>  
                        {{ item.name }} ( {{ item.count }} )
                    </v-tab>
                    <v-spacer></v-spacer>
                    <!--<v-btn v-if="isAdmin"
                        class="green darken-2 white--text"
                        @click="onShowAggregatePopup"
                    >
                        Aggregate Data
                    </v-btn>-->
                    <v-flex xs1></v-flex>
                </v-tabs>
                <v-tabs-items v-model="tab">
                    <v-tab-item>
                        <v-flex x12>
                            <v-card elevation="5">
                                <v-card-title>
                                    <v-flex xs6>
                                        <v-layout>
                                            <v-text-field
                                                type="text"
                                                placeholder="Search in scenarios"
                                                prepend-inner-icon=$vuetify.icons.ghd-search
                                                hide-details
                                                single-line
                                                v-model="searchMine"
                                                outline
                                                clearable
                                                @click:clear="onMineClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn style="margin-top: 2px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                outline 
                                                @click="onMineSearchClick()">
                                                Search
                                            </v-btn>
                                        </v-layout>
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
                                    :items="currentUserScenariosPage"                      
                                    :totalItems="totalUserScenarios"
                                    :pagination.sync="userScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    sort-icon=$vuetify.icons.ghd-table-sort

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
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile
                                                        v-for="(item,i) in actionItems"
                                                        :key="i"
                                                        @click="OnActionTaken(
                                                                item.action,
                                                                props.item.users,
                                                                props.item,
                                                                true) "
                                                        class="menu-style">
                                                        <v-list-tile-title icon>
                                                            <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                            <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon> 
                                                            {{item.title}}
                                                        </v-list-tile-title>
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                    <v-alert
                                        :value="hasMineSearch()"
                                        class="ara-orange-bg"
                                        icon="fas fa-exclamation"
                                        slot="no-data"
                                    >
                                        Your search for "{{ currentSearchMine }}" found
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
                                        <v-layout>
                                            <v-text-field
                                                label="Search"
                                                placeholder="Search in scenarios"
                                                outline
                                                prepend-inner-icon=$vuetify.icons.ghd-search
                                                hide-details
                                                single-line
                                                v-model="searchShared"
                                                clearable
                                                @click:clear="onSharedClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn style="margin-top: 2px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                outline 
                                                @click="onSharedSearchClick()">
                                                Search
                                            </v-btn>
                                        </v-layout>
                                    </v-flex>
                                </v-card-title>
                                <v-data-table
                                    :items="currentSharedScenariosPage"                      
                                    :totalItems="totalSharedScenarios"
                                    :pagination.sync="sharedScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    sort-icon=$vuetify.icons.ghd-table-sort
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
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile v-for="(item,i) in actionItemsForSharedScenario"
                                                        :key="i"
                                                        @click="OnActionTaken(item.action,props.item.users,props.item,false)"
                                                        class="menu-style">
                                                        <v-list-tile-title icon>                                                        
                                                            <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                            <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon>  
                                                            {{item.title}}
                                                        </v-list-tile-title>
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                    <template v-slot:no-data v-if="hasSharedSearch()">
                                        <v-alert
                                            :value="true"
                                            class="ara-orange-bg"
                                            icon="fas fa-exclamation">
                                            Your search for "{{ currentSearchShared }}"
                                            found no results.
                                        </v-alert>
                                    </template>                                 
                                </v-data-table>
                            </v-card>
                        </v-flex>
                    </v-tab-item>
                    <v-tab-item>
                        <v-flex xs12>
                            <v-card elevation="5">
                                <v-data-table
                                    :headers="simulationQueueGridHeaders"
                                    :items="currentSimulationQueuePage"
                                    :totalItems="totalQueuedSimulations"
                                    :pagination.sync="simulationQueuePagination"
                                    sort-icon=$vuetify.icons.ghd-table-sort
                                >                           
                                    <template slot="items" slot-scope="props">
                                        <td>{{ props.item.queuePosition }}</td>
                                        <td>
                                            {{ props.item.name }}
                                        </td>
                                        <td>
                                            {{
                                                props.item.queueingUser
                                                    ? props.item.queueingUser
                                                    : '[ Unknown ]'
                                            }}
                                        </td>
                                        <td>
                                            {{ formatDateWithTime(props.item.queueEntryTimestamp) }}
                                        </td>
                                        <td>
                                            {{ formatDateWithTime(props.item.workStartedTimestamp) }}
                                        </td>
                                        <td>{{ props.item.currentRunTime }}</td>
                                        <td>{{ props.item.previousRunTime }}</td>
                                        <td>{{ props.item.status }}</td>
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
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile v-for="(item,i) in actionItemsForSimulationQueue"
                                                        :key="i"
                                                        @click="OnSimulationQueueActionTaken(item.action,props.item)"
                                                        class="menu-style">
                                                        <v-list-tile-title icon>                                                        
                                                            <img style="padding-right:5px" v-bind:src="item.icon"/>
                                                            {{item.title}}
                                                        </v-list-tile-title>
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>                                         
                                    <template slot="no-data">
                                        {{ getEmptySimulationQueueMessage() }}
                                    </template>
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

        <ConfirmConvertToRelationalAlert
            :dialogData="ConfirmConvertJsonToRelationalData"
            @submit="onConfirmConvertJsonToRelationalAlertSubmit"
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
       
        <ConfirmCancelAlert
            :dialogData="confirmCancelAlertData"
            @submit="onConfirmCancelAlertSubmit"
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
    emptySimulation,
    QueuedSimulation,
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
import GhdStarSvg from '@/shared/icons/GhdStarSvg.vue';
import GhdShareSvg from '@/shared/icons/GhdShareSvg.vue';
import GhdQueueSvg from '@/shared/icons/GhdQueueSvg.vue';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { PagingPage, PagingRequest } from '@/shared/models/iAM/paging';
import ScenarioService from '@/services/scenario.service';

@Component({
    components: {
        MigrateLegacySimulationDialog,
        ConfirmCloneScenarioAlert: Alert,
        ConfirmDeleteAlert: Alert,
        ConfirmCancelAlert: Alert,
        ConfirmRollupAlert: Alert,
        ConfirmAnalysisRunAlert: Alert,
        ConfirmConvertToRelationalAlert: Alert,
        ReportsDownloaderDialog,
        CreateScenarioDialog,
        CloneScenarioDialog,
        CreateNetworkDialog,
        ShareScenarioDialog,
        ShowAggregationDialog,
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        Alert,
        GhdShareSvg,
        GhdStarSvg,
        GhdQueueSvg
    },
})
export default class Scenarios extends Vue {
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.scenarioModule.scenarios) stateScenarios: Scenario[];
    @State(state => state.scenarioModule.simulationQueue) stateSimulationQueue: QueuedSimulation[];

    @State(state => state.scenarioModule.currentSharedScenariosPage) stateSharedScenariosPage: Scenario[];
    @State(state => state.scenarioModule.currentUserScenarioPage) stateUserScenariosPage: Scenario[];
    @State(state => state.scenarioModule.currentSimulationQueuePage) stateSimulationQueuePage: QueuedSimulation[];

    @State(state => state.scenarioModule.totalSharedScenarios) stateTotalSharedScenarios: number;
    @State(state => state.scenarioModule.totalUserScenarios) stateTotalUserScenarios: number;
    @State(state => state.scenarioModule.totalQueuedSimulations) stateTotalQueuedSimulations: number;


    @State(state => state.breadcrumbModule.navigation) navigation: any[];

    @State(state => state.authenticationModule.authenticated)
    authenticated: boolean;
    @State(state => state.authenticationModule.userId) userId: string;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.authenticationModule.hasSimulationAccess) hasSimulationAccess: boolean;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
    @Action('getScenarios') getScenariosAction: any;
    @Action('getSharedScenariosPage') getSharedScenariosPageAction: any;
    @Action('getSimulationQueuePage') getSimulationQueuePageAction: any;
    @Action('getUserScenariosPage') getUserScenariosPageAction: any;
    @Action('createScenario') createScenarioAction: any;
    @Action('cloneScenario') cloneScenarioAction: any;
    @Action('updateScenario') updateScenarioAction: any;
    @Action('deleteScenario') deleteScenarioAction: any;
    @Action('cancelSimulation') cancelSimulationAction: any;
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
    simulationQueueGridHeaders: DataTableHeader[] = [
        {
            text: 'Queue Position',
            value: 'queuePosition',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },        
        {
            text: 'Scenario',
            value: 'name',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Queued By',
            value: 'queueingUser',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Queued Time',
            value: 'queueEntryTimestamp',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Start Time',
            value: 'workStartedTimestamp',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Current Run Time',
            value: 'currentRunTime',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Previous Run Time',
            value: 'previousRunTime',
            align: 'left',
            sortable: false,
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
    actionItemsForSimulationQueue: ScenarioActions[] = [];
    tabItems: TabItems[] = [];
    tab: string = '';
    availableActions: any;
    availableSimulationActions: any;
    nameUpdate: string = '';

    scenarios: Scenario[] = [];

    userScenarios: Scenario[] = [];
    currentUserScenariosPage: Scenario[] = []
    userScenariosPagination: Pagination = clone(emptyPagination);
    totalUserScenarios: number = 0;

    sharedScenarios: Scenario[] = [];
    currentSharedScenariosPage: Scenario[] = [];
    sharedScenariosPagination:  Pagination = clone(emptyPagination);    
    totalSharedScenarios: number = 0;

    simulationQueue: QueuedSimulation[] = [];
    currentSimulationQueuePage: QueuedSimulation[] = [];
    simulationQueuePagination: Pagination = clone(emptyPagination);
    totalQueuedSimulations: number = 0;

    initializing: boolean = true;
    initializingSimulationQueue: boolean = true;
    searchMine: string = '';
    currentSearchMine: string = '';
    searchShared: string = '';
    currentSearchShared: string = '';
    //confirmRollupAlertData: AlertData = clone(emptyAlertData);
    //showCreateNetworkDialog: boolean = false;
    reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(
        emptyReportsDownloadDialogData,
    );
    confirmAnalysisRunAlertData: AlertData = clone(emptyAlertData);
    shareScenarioDialogData: ShareScenarioDialogData = clone(
        emptyShareScenarioDialogData,
    );
    
    ConfirmConvertJsonToRelationalData: AlertData = clone(emptyAlertData);
    confirmCloneScenarioAlertData: AlertData = clone(emptyAlertData);
    cloneScenarioDialogData: CloneScenarioDialogData = clone(emptyCloneScenarioDialogData);
    confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    confirmCancelAlertData: AlertData = clone(emptyAlertData);
    showCreateScenarioDialog: boolean = false;
    selectedScenario: Scenario = clone(emptyScenario);
    selectedSimulation: QueuedSimulation = clone(emptySimulation);
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
            this.initializeScenarioPages()
        }
    }

    @Watch('stateScenarios', {deep: true})
    onStateScenariosChanged() {
        this.scenarios = clone(this.stateScenarios);
    }

    @Watch('stateSimulationQueue', {deep: true})
    onStateSimulationQueueChanged() {
        this.simulationQueue = clone(this.stateSimulationQueue);
    }

    @Watch('stateSharedScenariosPage', {deep: true}) onStateSharedScenariosPageChanged(){
        this.currentSharedScenariosPage = clone(this.stateSharedScenariosPage);
    }
    @Watch('stateTotalSharedScenarios') onStateTotalSharedScenariosChanged(){
        this.totalSharedScenarios = this.stateTotalSharedScenarios;
    }
    @Watch('totalSharedScenarios') onTotalSharedScenariosChanged(){
        this.setTabTotals();
    }
    
    @Watch('stateUserScenariosPage', {deep: true}) onStateUserScenariosPageChanged(){
        this.currentUserScenariosPage = clone(this.stateUserScenariosPage);
    }
    @Watch('stateTotalUserScenarios') onStateTotalUserScenariosChanged(){
        this.totalUserScenarios = this.stateTotalUserScenarios;
    }
    @Watch('totalUserScenarios') onTotalUserScenariosChanged(){
        this.setTabTotals();
    }
    
    @Watch('stateSimulationQueuePage', {deep: true}) onStateSimulationQueuePageChanged(){
        this.currentSimulationQueuePage = clone(this.stateSimulationQueuePage);
    }
    @Watch('stateTotalQueuedSimulations') onStateTotalQueuedSimulations(){
        this.totalQueuedSimulations = this.stateTotalQueuedSimulations;
    }
    @Watch('totalQueuedSimulations') onTotalQueuedSimulationsChanged(){
        this.setTabTotals();
    }

    @Watch('userScenariosPagination') onUserScenariosPagination() {
        if(this.initializing)
            return;
        const { sortBy, descending, page, rowsPerPage } = this.userScenariosPagination;

        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: this.currentSearchMine
        };
        if(hasValue(this.networks) )
            this.getUserScenariosPageAction(request); 
    }

    @Watch('sharedScenariosPagination') onSharedScenariosPagination() {
        if(this.initializing)
            return;
        const { sortBy, descending, page, rowsPerPage } = this.sharedScenariosPagination;

        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: this.currentSearchShared
        };
        if(hasValue(this.networks) )
            this.getSharedScenariosPageAction(request); 
    }

    // Refresh both lists and counts(gets called when clone, delete, create operations are performed)
    onScenariosPagination() {
        if(this.initializing)
            return;

        const { sortBy, descending, page, rowsPerPage } = this.userScenariosPagination;
        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: this.currentSearchMine
        };

        if(hasValue(this.networks))
            this.getUserScenariosPageAction(request).then(() => {
                this.onSharedScenariosPagination();
            });
    }

    @Watch('simulationQueuePagination') onSimulationQueuePagination() {
        this.doSimulationQueuePagination();
    }

    doSimulationQueuePagination() {
        if(this.initializingSimulationQueue)
            return;
        const { sortBy, descending, page, rowsPerPage } = this.simulationQueuePagination;

        const simulationQueueRequest: PagingRequest<QueuedSimulation>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: ""
        };
        this.getSimulationQueuePageAction(simulationQueueRequest);    
    }

    mounted() {
        this.networks = clone(this.stateNetworks);
        if (hasValue(this.networks) ) {
            this.initializeScenarioPages();
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
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            this.getReportStatus,
        );

        this.availableActions = {
            runAnalysis: 'runAnalysis',
            reports: 'reports',
            settings: 'settings',
            share: 'share',
            clone: 'clone',
            delete: 'delete',
            commitedProjects: 'commitedProjects',
            convert:'convert'
        };
        this.availableSimulationActions = {
            cancel: 'cancel'
        }
        this.actionItemsForSharedScenario = [
            {
                title: 'Run Analysis',
                action: this.availableActions.runAnalysis,
                icon: require("@/assets/icons/monitor.svg"),
                isCustomIcon: true
            },
            {
                title: 'Reports',
                action: this.availableActions.reports,
                icon: require("@/assets/icons/clipboard.svg"),
                isCustomIcon: true
            },
            {
                title: 'Settings',
                action: this.availableActions.settings,
                icon: require("@/assets/icons/gear.svg"),
                isCustomIcon: true
            },
            {
                title: 'Committed Projects',
                action: this.availableActions.commitedProjects,
                icon: require("@/assets/icons/committed-projects.svg"),
                isCustomIcon: true
            },
            {
                title: 'Convert Output from Json to Relational',
                action: this.availableActions.convert,
                icon: "fas fa-exchange-alt",
                isCustomIcon: false
            },
            {
                title: 'Clone',
                action: this.availableActions.clone,
                icon: require("@/assets/icons/copy.svg"),
                isCustomIcon: true
            },
            {
                title: 'Delete',
                action: this.availableActions.delete,
                icon: require("@/assets/icons/trash.svg"),
                isCustomIcon: true
            }           
        ];
        this.actionItemsForSimulationQueue = [
             {
                title: 'Cancel Analysis',
                action: this.availableSimulationActions.cancel,
                icon: require("@/assets/icons/x-circle.svg"),
                isCustomIcon: true
            }             
        ];
        this.actionItems = this.actionItemsForSharedScenario.slice();
        this.actionItems.splice(4, 0, {
            title: 'Share',
            action: this.availableActions.share,
            icon: require("@/assets/icons/share-geometric.svg"),
                isCustomIcon: true
        });
        this.tabItems.push(
            { name: 'My scenarios', icon: require("@/assets/icons/star-empty.svg"), count: this.totalUserScenarios },
            { name: 'Shared with me', icon: require("@/assets/icons/share-empty.svg"), count: this.totalSharedScenarios },
            { name: 'Simulation queue', icon: require("@/assets/icons/queue.svg"), count: this.totalQueuedSimulations },
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
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            this.getReportStatus,
        );
    }

    initializeScenarioPages(){
        const { sortBy, descending, page, rowsPerPage } = this.sharedScenariosPagination;

        const request: PagingRequest<Scenario> = {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        const simulationQueueRequest: PagingRequest<QueuedSimulation> = {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };        
        this.getSharedScenariosPageAction(request).then(() =>
        this.getUserScenariosPageAction(request).then(() =>
        this.getSimulationQueuePageAction(simulationQueueRequest).then(() => {
            this.initializing = false;
            this.initializingSimulationQueue = false;
            this.totalUserScenarios = this.stateTotalUserScenarios;
            this.totalSharedScenarios = this.stateTotalSharedScenarios;
            this.totalQueuedSimulations = this.stateTotalQueuedSimulations;
            this.currentUserScenariosPage = clone(this.stateUserScenariosPage);
            this.currentSharedScenariosPage = clone(this.stateSharedScenariosPage);
            this.currentSimulationQueuePage = clone(this.stateSimulationQueuePage);
        }))); 
    }

    formatDate(dateToFormat: Date) {
        return hasValue(dateToFormat)
            ? moment(dateToFormat).format('M/D/YYYY')
            : null;
    }

    formatDateWithTime(dateToFormat: Date) {
    return hasValue(dateToFormat)
        ? moment(dateToFormat).format('M/D/YYYY hh:mm:ss')
        : null;
    }

    canModifySharedScenario(scenarioUsers: ScenarioUser[]) {
        const currentUser: string = getUserName();
        const scenarioUserCanModify = (user: ScenarioUser) =>
            user.username === currentUser && user.canModify;
        return (
            this.hasAdminAccess ||
            this.hasSimulationAccess ||
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
        }).then(() => this.initializeScenarioPages());
    }

    onEditScenarioName(scenario: Scenario, name: string) {
        scenario.name = name;
        if (hasValue(scenario.name)) {
            this.updateScenarioAction({ scenario: scenario }).then(() => {
                if(this.tab == '0')
                    this.onUserScenariosPagination();
                else
                    this.onSharedScenariosPagination();
            });
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

    onShowConfirmConvertJsonToRelationalAlert(scenario: Scenario) {
        this.selectedScenario = clone(scenario);

        this.ConfirmConvertJsonToRelationalData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Converting the simulation output from json to a relational format can be a lengthy process.',
        };
    }

    onConfirmConvertJsonToRelationalAlertSubmit(submit: boolean) {
        this.ConfirmConvertJsonToRelationalData = clone(emptyAlertData);

        if (submit && this.selectedScenario.id !== getBlankGuid()) {
            ScenarioService.ConvertSimulationOutputToRelational(this.selectedScenario.id);
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

    onNavigateToCommittedProjectView(localScenario: Scenario) {
        this.selectScenarioAction({ scenarioId: localScenario.id });

        this.$router.push({
            path: '/CommittedProjectsEditor/Scenario/',
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
            }).then(() => {
                this.selectedScenario = clone(emptyScenario)
                if(this.tab == '0')
                    this.onUserScenariosPagination();
                else
                    this.onSharedScenariosPagination();
            });
        }
    }

    onCloneScenarioDialogSubmit(scenario: Scenario) {
        this.cloneScenarioDialogData = clone(emptyCloneScenarioDialogData);

        if (!isNil(scenario)) {
            this.cloneScenarioAction({
                scenarioId: scenario.id,
                networkId: scenario.networkId,
                scenarioName: scenario.name
            }).then(() => {
                this.selectedScenario = clone(emptyScenario)
                this.onScenariosPagination();
            });
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


    onShowConfirmCancelAlert(simulation: QueuedSimulation) {
        this.selectedSimulation = clone(simulation);

        this.confirmCancelAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to cancel the analysis?',
        };
    }

    onConfirmDeleteAlertSubmit(submit: boolean) {
        this.confirmDeleteAlertData = clone(emptyAlertData);

        if (submit && this.selectedScenario.id !== getBlankGuid()) {
            this.deleteScenarioAction({
                scenarioId: this.selectedScenario.id,
                scenarioName: this.selectedScenario.name,
            }).then(() => {
                this.selectedScenario = clone(emptyScenario);              
                this.onScenariosPagination();
            });
        }
    }

    onConfirmCancelAlertSubmit(submit: boolean) {
        this.confirmCancelAlertData = clone(emptyAlertData);

        if (submit && this.selectedSimulation.id !== getBlankGuid()) {
            this.cancelSimulationAction({
                simulationId: this.selectedSimulation.id,
            }).then(() => {
                this.selectedSimulation = clone(emptySimulation);
                //this.onSimulationQueuePagination();
            });
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

    delay(ms: number) {
        return new Promise( resolve => setTimeout(resolve, ms) );
    }

    getScenarioAnalysisDetailUpdate(data: any) {
        this.updateSimulationAnalysisDetailAction({
            simulationAnalysisDetail: data.simulationAnalysisDetail,
        });
        (async () => { 
            if ((data.simulationAnalysisDetail.status == "Queued to run.") ||
                (data.simulationAnalysisDetail.status == "Getting simulation analysis network") ||
                (data.simulationAnalysisDetail.status == "Simulation complete. 100%") ||
                (data.simulationAnalysisDetail.status == "Canceled"))
            {
                await this.delay(1000);
                this.doSimulationQueuePagination();
            }
        })();                            
    }

    getReportStatus(data: any) {
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
            }).then(() => {
                this.onScenariosPagination();
            });
        }
    }

    onMigrateLegacySimulationSubmit(legacySimulationId: number) {
        this.showMigrateLegacySimulationDialog = false;

        if (!isNil(legacySimulationId)) {
            this.migrateLegacySimulationDataAction({
                simulationId: legacySimulationId,
                networkId: 'D7B54881-DD44-4F93-8250-3D4A630A4D3B',
            }).then(() => this.initializeScenarioPages());
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
            CommittedProjectsService.deleteSpecificCommittedProjects(
                [this.selectedScenarioId],
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
                if (this.canModifySharedScenario(scenarioUsers) || isOwner) {
                    this.onNavigateToCommittedProjectView(scenario);
                }
                // this.selectedScenarioId = scenario.id;
                // this.showImportExportCommittedProjectsDialog = true;
                break;
            case this.availableActions.convert:
                this.onShowConfirmConvertJsonToRelationalAlert(scenario);
        }
    }

    OnSimulationQueueActionTaken(
        action: string,
        simulation: QueuedSimulation,
    ) {
        switch (action) {
            case this.availableSimulationActions.cancel:
                this.onShowConfirmCancelAlert(simulation);
                break;
        }
    }


    onShowAggregatePopup() {
        this.aggragateDialogData = {
            showDialog: true,
        };
    }

    onMineSearchClick(){
        this.currentSearchMine = this.searchMine;
        this.resetPageMine()
    }

    onMineClearClick(){
        this.searchMine = '';
        this.onMineSearchClick();
    }

    resetPageMine(){
        this.userScenariosPagination.page = 1;
        this.onUserScenariosPagination();
    }

    onSharedSearchClick(){
        this.currentSearchShared = this.searchShared;
        this.resetPageShared()
    }

    onSharedClearClick(){
        this.searchShared = '';
        this.onSharedSearchClick();
    }

    resetPageShared(){
        this.sharedScenariosPagination.page = 1;
        this.onSharedScenariosPagination();
    }

    hasSharedSearch(): boolean{
        return this.currentSearchShared.trim() !== '';
    }

    hasMineSearch(): boolean{
        return this.currentSearchMine.trim() !== '';
    }

    setTabTotals(){
        this.tabItems.forEach(tab => {
            if(tab.name === 'Shared with me')
                tab.count = this.totalSharedScenarios;
            else if (tab.name === 'My scenarios')
                tab.count = this.totalUserScenarios;
            else
                tab.count = this.totalQueuedSimulations;
        })
    }

    getEmptySimulationQueueMessage()
    {
        if (this.totalSharedScenarios == 0 &&
            this.totalUserScenarios == 0 &&
            this.totalQueuedSimulations == 0){
            
            return "Retrieving data..."
        }
        else {
            return "No running simulations"
        }
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

.v-tabs__item--active{
    fill:#002E6C !important;
    color: #002E6C !important;
}
.icon-selected-tab{
    fill:#2A578D
}
</style>
