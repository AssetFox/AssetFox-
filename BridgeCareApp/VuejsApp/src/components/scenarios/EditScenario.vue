<template>
    <v-layout column>
        <v-layout>
            <v-card
                class="mx-auto ghd-sidebar-scenario"
                height="100%"
                elevation="0"
                style="border-top-left-radius: 5px; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px; border: 1px solid #999999;"
            >
                <v-list 
                    id = "EditScenario-navigation-vList"
                    class="ghd-navigation-list">
                    <v-list-item-group
                        id = "EditScenario-navigation-vlistItemGroup"
                        class="settings-list ghd-control-text"
                        :key="navigationTab.tabName"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                        <v-list-tile id="EditScenario-tabs-vListTile" :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <v-list-tile-action>
                                <v-list-tile-icon>
                                    <TreatmentSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Treatment'"/>  
                                    <TargetConditionGoalSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Target Condition Goal'"/>  
                                    <RemainingLifeLimitSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Remaining Life Limit'"/>  
                                    <PerformanceCurveSvg style="height: 34px; width: 36px"  class="scenario-icon" v-if="navigationTab.tabName === 'Deterioration Model'"/>  
                                    <DeficientConditionGoalSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Deficient Condition Goal'"/>  
                                    <InvestmentSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Investment'"/>  
                                    <CashFlowSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Cash Flow'"/>  
                                    <BudgetPrioritySvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Budget Priority'"/>  
                                    <AnalysisMethodSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Analysis Method'"/>  
                                    <CalculatedAttributeSvg style="height: 32px; width: 32px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Calculated Attribute'"/>  
                                    <CommittedProjectSvg style="height: 32px; width: 24px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Committed Projects'"/>  
                                    <ReportsSvg style="height: 38px; width: 30px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Reports & Outputs'"/>  
                                </v-list-tile-icon>
                            </v-list-tile-action>
                            <v-list-tile-content>
                                <v-list-tile-title style="text-decoration: none">{{navigationTab.tabName}}</v-list-tile-title>
                            </v-list-tile-content>
                        </v-list-tile>
                    </v-list-item-group>
                </v-list>
                <div style="margin: auto; width: 85%;">
                    <v-btn
                        class="ghd-white-bg ghd-lt-gray ghd-button-text ghd-button-border"
                        @click="onShowRunSimulationAlert"
                        depressed
                        block
                        variant = "outline">
                        Run Scenario
                    </v-btn>
                </div>
            </v-card>
            <v-flex xs12 class="ghd-content">
                <v-container fluid grid-list-xs style="padding-left:20px;padding-right:20px;">
                    <router-view></router-view>
                </v-container>
            </v-flex>
        </v-layout>
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
import AnalysisMethodSvg from '@/shared/icons/AnalysisMethodSvg.vue';
import BudgetPrioritySvg from '@/shared/icons/BudgetPrioritySvg.vue';
import CashFlowSvg from '@/shared/icons/CashFlowSvg.vue';
import InvestmentSvg from '@/shared/icons/InvestmentSvg.vue';
import DeficientConditionGoalSvg from '@/shared/icons/DeficientConditionGoalSvg.vue';
import PerformanceCurveSvg from '@/shared/icons/PerformanceCurveSvg.vue';
import RemainingLifeLimitSvg from '@/shared/icons/RemainingLifeLimitSvg.vue';
import TargetConditionGoalSvg from '@/shared/icons/TargetConditionGoalSvg.vue';
import TreatmentSvg from '@/shared/icons/TreatmentSvg.vue';
import CalculatedAttributeSvg from '@/shared/icons/CalculatedAttributeSvg.vue';
import CommittedProjectSvg from '@/shared/icons/CommittedProjectSvg.vue';
import ReportsSvg from '@/shared/icons/ReportsSvg.vue';

@Component({
    components: {
        CommittedProjectsFileUploaderDialog: ImportExportCommittedProjectsDialog,
        Alert,
        TreatmentSvg, 
        TargetConditionGoalSvg,
        RemainingLifeLimitSvg,
        PerformanceCurveSvg,
        DeficientConditionGoalSvg,
        InvestmentSvg,
        CashFlowSvg,
        BudgetPrioritySvg,
        AnalysisMethodSvg,
        CalculatedAttributeSvg,
        CommittedProjectSvg,
        ReportsSvg
    },
})
export default class EditScenario extends Vue {
    @State(state => state.networkModule.networks) stateNetworks: Network[];
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.authenticationModule.hasSimulationAccess) hasSimulationAccess: boolean;
    @State(state => state.scenarioModule.selectedScenario) stateSelectedScenario: Scenario;
    @State(state => state.scenarioModule.currentSharedScenariosPage) stateSharedScenariosPage: Scenario[];
    @State(state => state.scenarioModule.currentUserScenarioPage) stateUserScenariosPage: Scenario[];
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
            tabName: 'Deterioration Model',
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
            tabIcon: 'fas fa-balance-scale',
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
            tabIcon: 'fas fa-battery-half',
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
        {
            tabName: 'Committed Projects',
            tabIcon: 'fas fa-clipboard',
            navigation: {
                path: '/CommittedProjectsEditor/Scenario/',
            },
        },
        {
            tabName: 'Reports & Outputs',
            tabIcon: 'fas fa-clipboard,',
            navigation: {
                path: '/ReportsAndOutputs/Scenario/',
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

                        if (navigationTab.tabName === 'Remaining Life Limit' 
                            || navigationTab.tabName === 'Target Condition Goal' 
                            || navigationTab.tabName === 'Deficient Condition Goal' 
                            || navigationTab.tabName === 'Calculated Attribute') {
                            navigationTab['visible'] = vm.hasAdminAccess;
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

    @Watch('stateSharedScenariosPage')
    onStateSharedScenariosPageChanged() {
        if (any(propEq('id', this.selectedScenario.id))) {
            this.selectScenarioAction({
                scenarioId: this.selectedScenario.id,
            });
        }
    }

     @Watch('stateUserScenariosPage')
    onStateUserScenariosPagePageChanged() {
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
            CommittedProjectsService.deleteSimulationCommittedProjects(
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
.settings-list a:hover {
    text-decoration: none;
}


.primary--text .scenario-icon{
    fill: #FFFFFF !important;
}

.scenario-icon {
    fill: #999999 !important;
}

.primary--text .scenario-icon-stroke{
    stroke: #FFFFFF !important;
}

.scenario-icon-stroke {
    stroke: #999999 !important;
}

</style>
