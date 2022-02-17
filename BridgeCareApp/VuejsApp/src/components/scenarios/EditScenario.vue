<template>
    <v-layout column>
        <v-flex xs12>
            <v-layout fixed justify-space-between>
                <div>
                    <v-tabs>
                        <v-tab
                            :key="navigationTab.tabName"
                            :to="navigationTab.navigation"
                            v-for="navigationTab in visibleNavigationTabs()"
                        >
                            {{ navigationTab.tabName }}
                            <v-icon right>{{ navigationTab.tabIcon }}</v-icon>
                        </v-tab>
                    </v-tabs>
                </div>
                <div>
                    <v-layout>
                        <div v-if="$screen.xxl && !$screen.freeRealEstate">
                            <v-menu>
                                <template slot="activator">
                                    <v-btn icon>
                                        <v-icon>fas fa-bars</v-icon>
                                    </v-btn>
                                </template>
                                <v-list>
                                    <v-list-tile
                                        @click="onShowRunSimulationAlert"
                                    >
                                        <v-list-tile-action>
                                            <v-icon>fas fa-play</v-icon>
                                        </v-list-tile-action>
                                        <v-list-tile-title
                                            >Run Scenario</v-list-tile-title
                                        >
                                    </v-list-tile>
                                    <v-list-tile
                                        @click="
                                            showImportExportCommittedProjectsDialog = true
                                        "
                                    >
                                        <v-list-tile-action>
                                            <v-icon
                                                >fas fa-cloud-upload-alt</v-icon
                                            >
                                        </v-list-tile-action>
                                        <v-list-tile-title
                                            >Committed
                                            Projects</v-list-tile-title
                                        >
                                    </v-list-tile>
                                </v-list>
                            </v-menu>
                        </div>
                        <div
                            class="edit-scenario-btns-div"
                            v-if="$screen.freeRealEstate"
                        >
                            <div>
                                <v-btn
                                    @click="onShowRunSimulationAlert"
                                    class="ara-blue-bg white--text"
                                    >Run Scenario
                                    <v-icon class="white--text" right
                                        >fas fa-play</v-icon
                                    >
                                </v-btn>
                            </div>
                            <div>
                                <v-btn
                                    @click="
                                        showImportExportCommittedProjectsDialog = true
                                    "
                                    class="ara-blue-bg white--text"
                                >
                                    Committed Projects
                                    <v-icon class="white--text" right
                                        >fas fa-cloud-upload-alt</v-icon
                                    >
                                </v-btn>
                            </div>
                        </div>
                    </v-layout>
                </div>
            </v-layout>
        </v-flex>

        <v-flex xs12>
            <v-container fluid grid-list-xs>
                <router-view></router-view>
            </v-container>
        </v-flex>

        <Alert :dialogData="alertData" @submit="onSubmitAlertResult" />

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
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import { emptyScenario, Scenario } from '@/shared/models/iAM/scenario';
import ImportExportCommittedProjectsDialog from '@/components/scenarios/scenarios-dialogs/ImportExportCommittedProjectsDialog.vue';
import { any, clone, isNil, propEq } from 'ramda';
import { AxiosResponse } from 'axios';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Network } from '@/shared/models/iAM/network';
import FileDownload from 'js-file-download';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { http2XX } from '@/shared/utils/http-utils';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { FileInfo } from '@/shared/models/iAM/file-info';
import { convertBase64ToArrayBuffer } from '@/shared/utils/file-utils';

@Component({
    components: {
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        Alert,
    },
})
export default class EditScenario extends Vue {
    @State(state => state.breadcrumbModule.navigation) navigation: any[];
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @State(state => state.scenarioModule.selectedScenario)
    stateSelectedScenario: Scenario;
    @State(state => state.scenarioModule.scenarios) stateScenarios: Scenario[];
    @State(state => state.authenticationModule.userId) userId: string;

    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('selectScenario') selectScenarioAction: any;
    @Action('runSimulation') runSimulationAction: any;
    @Action('runNewSimulation') runNewSimulationAction: any;

    selectedScenarioId: string = getBlankGuid();
    showImportExportCommittedProjectsDialog: boolean = false;
    networkId: string = getBlankGuid();
    networkName: string = '';
    selectedScenario: Scenario = clone(emptyScenario);
    navigationTabs: NavigationTab[] = [
        {
            tabName: 'Analysis Method',
            tabIcon: 'fas fa-chart-bar',
            navigation: {
                path: '/EditAnalysisMethod/',
            },
        },
        {
            tabName: 'Investment',
            tabIcon: 'fas fa-dollar-sign',
            navigation: {
                path: '/InvestmentEditor/Scenario/',
            },
        },
        {
            tabName: 'Performance Curve',
            tabIcon: 'fas fa-chart-line',
            navigation: {
                path: '/PerformanceCurveEditor/Scenario/',
            },
        },
        {
            tabName: 'Calculated Attribute',
            tabIcon: 'fas fa-plus-square',
            navigation: {
                path: '/CalculatedAttributeEditor/Scenario/',
            },
        },
        {
            tabName: 'Treatment',
            tabIcon: 'fas fa-tools',
            navigation: {
                path: '/TreatmentEditor/Scenario/',
            },
        },
        {
            tabName: 'Budget Priority',
            tabIcon: 'fas fa-copy',
            navigation: {
                path: '/BudgetPriorityEditor/Scenario/',
            },
        },
        {
            tabName: 'Target Condition Goal',
            tabIcon: 'fas fa-bullseye',
            navigation: {
                path: '/TargetConditionGoalEditor/Scenario/',
            },
        },
        {
            tabName: 'Deficient Condition Goal',
            tabIcon: 'fas fa-level-down-alt',
            navigation: {
                path: '/DeficientConditionGoalEditor/Scenario/',
            },
        },
        {
            tabName: 'Remaining Life Limit',
            tabIcon: 'fas fa-business-time',
            navigation: {
                path: '/RemainingLifeLimitEditor/Scenario/',
            },
        },
        {
            tabName: 'Cash Flow',
            tabIcon: 'fas fa-money-bill-wave',
            navigation: {
                path: '/CashFlowEditor/Scenario/',
            },
        },
    ];
    alertData: AlertData = clone(emptyAlertData);
    alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };

    beforeRouteEnter(to: any, from: any, next: any) {
        next((vm: any) => {
            // set selectedScenarioId
            vm.selectedScenarioId = to.query.scenarioId;
            vm.networkId = to.query.networkId;
            vm.simulationName = to.query.scenarioName;
            vm.networkName = to.query.networkName;

            // check that selectedScenarioId is set
            if (vm.selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                vm.addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                vm.$router.push('/Scenarios/');
            } else {
                vm.navigationTabs = vm.navigationTabs.map(
                    (navTab: NavigationTab) => {
                        const navigationTab = {
                            ...navTab,
                            navigation: {
                                ...navTab.navigation,
                                query: {
                                    scenarioName: vm.simulationName,
                                    scenarioId: vm.selectedScenarioId,
                                    networkId: vm.networkId,
                                    networkName: vm.networkName,
                                },
                            },
                        };

                        if (navigationTab.tabName === 'Remaining Life Limit') {
                            navigationTab['visible'] = vm.isAdmin;
                        }

                        return navigationTab;
                    },
                );

                // get the window href
                const href = window.location.href;
                // check each NavigationTab object to see if it has a matching navigation path with the href
                const hasChildPath = any(
                    (navigationTab: NavigationTab) =>
                        href.indexOf(navigationTab.navigation.path) !== -1,
                    vm.navigationTabs,
                );
                // if no matching navigation path was found in the href, then route with path of first navigationTabs entry
                if (!hasChildPath) {
                    vm.$router.push(vm.navigationTabs[0].navigation);
                }
            }
        });
    }

    @Watch('stateSelectedScenario')
    onStateSelectedScenarioChanged() {
        this.selectedScenario = clone(this.stateSelectedScenario);
    }

    @Watch('stateScenarios')
    onStateScenariosChanged() {
        if (any(propEq('id', this.selectedScenario.id))) {
            this.selectScenarioAction({
                scenarioId: this.selectedScenario.id,
            });
        }
    }

    mounted() {
        if (this.selectedScenarioId !== getBlankGuid()) {
            this.selectScenarioAction({
                scenarioId: this.selectedScenarioId,
            });
        }
    }

    beforeDestroy() {
        this.selectScenarioAction({ scenarioId: getBlankGuid() });
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

    visibleNavigationTabs() {
        return this.navigationTabs.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }

    /**
     * Shows the Alert
     */
    onShowRunSimulationAlert() {
        this.alertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Only one simulation can be run at a time. The model run you are about to queue will be ' +
                'executed in the order in which it was received.',
        };
    }

    /**
     * Takes in a boolean parameter from the AppPopupModal to determine if a scenario's simulation should be executed
     * @param runScenarioSimulation Alert result
     */
    onSubmitAlertResult(runScenarioSimulation: boolean) {
        this.alertData = clone(emptyAlertData);

        if (runScenarioSimulation) {
            this.runSimulationAction({
                networkId: this.networkId,
                scenarioId: this.selectedScenarioId,
            });
        }
    }
}
</script>

<style>
.child-router-div {
    height: 100%;
    overflow: auto;
}

.edit-scenario-btns-div {
    display: flex;
}
</style>
