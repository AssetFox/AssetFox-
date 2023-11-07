<template>
        <v-row>
            <v-col class="p-0">
                <v-card
                class="mx-auto ghd-sidebar-scenario"
                height="100%"
                elevation="0"
                style="border-top-left-radius: 5px; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px; border: 1px solid #999999;"
            >
                <v-list
                    id = "EditScenario-navigation-vList"
                    class="ghd-navigation-list">
                    <v-list-item
                        id = "EditScenario-navigation-vlistItemGroup"
                        class="settings-list ghd-control-text"
                        :key="navigationTab.tabName"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                        <v-list-item id="EditScenario-tabs-vListTile" :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <template v-slot:prepend>
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
                                    <ReportsSvg style="height: 38px; width: 22px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Reports & Outputs'"/>  
                            </template>
                            <v-list-item-title style="width: auto;" v-text="navigationTab.tabName"></v-list-item-title>
                        </v-list-item>
                    </v-list-item>
                </v-list>
                <div style="margin: auto; width: 85%;">
                    <v-btn
                        class="ghd-white-bg ghd-lt-gray ghd-button-text ghd-button-border"
                        @click="onShowRunSimulationAlert"
                        
                        block
                        variant = "outlined">
                        Run Scenario
                    </v-btn>
                </div>
            </v-card>
            </v-col>
            
            <v-col class=" p-0" cols="10">
                <v-container class="p-0" fluid grid-list-xs >
                    <router-view></router-view>
                </v-container>
            </v-col>
        </v-row>
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

</template>

<script lang="ts" setup>
import Vue, { Ref, ref, shallowReactive, shallowRef, watch, onMounted, onBeforeUnmount, onBeforeMount } from 'vue'; 
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
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 

    let store = useStore(); 
    const router = useRouter(); 

    const stateNetworks: Network[] = shallowReactive(store.state.networkModule.networks) ;
    let hasAdminAccess: boolean = (store.state.authenticationModule.hasAdminAccess) ; 
    let hasSimulationAccess:boolean = (store.state.authenticationModule.hasSimulationAccess) ; 

    const stateSelectedScenario: Scenario = shallowReactive(store.state.scenarioModule.selectedScenario) ;
    const stateSharedScenariosPage: Scenario[] = shallowReactive(store.state.scenarioModule.currentSharedScenariosPage) ;
    const stateUserScenariosPage: Scenario[] = shallowReactive(store.state.scenarioModule.currentUserScenarioPage) ;

    let userId = ref<string>(store.state.authenticationModule.userId);

    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification')}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification')}

    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario')} 

    async function runSimulationAction(payload?: any): Promise<any>{await store.dispatch('runSimulation')}   
    async function runNewSimulationAction(payload?: any): Promise<any>{await store.dispatch('runNewSimulation')}

    let selectedScenarioId: string = getBlankGuid();
    let showImportExportCommittedProjectsDialog: boolean = false;
    let networkId: string = getBlankGuid();
    let simulationName: string;
    let networkName: string = '';
    let selectedScenario: Scenario = clone(emptyScenario);
    let navigationTabs: NavigationTab[] = [
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
    let alertData: AlertData = clone(emptyAlertData);
    let alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };

    onMounted(() => {
    });
    onBeforeMount(() => {
        selectedScenarioId = router.currentRoute.value.path;  //router.currentRoute.value.query.scenarioId as string;
        let temp: string = router.currentRoute.value.query.scenarioId as string;
    });
    created();
    function created() { 
            // set selectedScenarioId
            selectedScenarioId = router.currentRoute.value.query.scenarioId as string;
            networkId = router.currentRoute.value.query.networkId as string;
            simulationName = router.currentRoute.value.query.scenarioName as string;
            networkName = router.currentRoute.value.query.networkName as string;

            // check that selectedScenarioId is set
            if (selectedScenarioId === getBlankGuid()) {
                // set 'no selected scenario' error message, then redirect user to Scenarios UI
                addErrorNotificationAction({
                    message: 'Found no selected scenario for edit',
                });
                router.push('/Scenarios/');
            } else {                
                navigationTabs = navigationTabs.map(
                    (navTab: NavigationTab) => {
                        const navigationTab = {
                            ...navTab,
                            navigation: {
                                ...navTab.navigation,
                                query: {
                                    scenarioName: simulationName,
                                    scenarioId: selectedScenarioId,
                                    networkId: networkId,
                                    networkName: networkName,
                                },
                            },
                        };

                        if (navigationTab.tabName === 'Remaining Life Limit' 
                            || navigationTab.tabName === 'Target Condition Goal' 
                            || navigationTab.tabName === 'Deficient Condition Goal' 
                            || navigationTab.tabName === 'Calculated Attribute') {
                            navigationTab['visible'] = hasAdminAccess;
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
                    navigationTabs,
                );
                // if no matching navigation path was found in the href, then route with path of first navigationTabs entry
                if (!hasChildPath) {
                    router.push(navigationTabs[0].navigation);
                }                
            }
    }

    watch(stateSelectedScenario, () => onStateSelectedScenarioChanged)
    function onStateSelectedScenarioChanged() {
        selectedScenario = clone(stateSelectedScenario);
    }

    watch(stateSharedScenariosPage, () => onStateSharedScenariosPageChanged)
    function onStateSharedScenariosPageChanged() {
        if (any(propEq('id', selectedScenario.id))) {
            selectScenarioAction({
                scenarioId: selectedScenario.id,
            });
        }
    }

    watch(stateUserScenariosPage, () => onStateUserScenariosPagePageChanged)
    function onStateUserScenariosPagePageChanged() {
        if (any(propEq('id', selectedScenario.id))) {
            selectScenarioAction({
                scenarioId: selectedScenario.id,
            });
        }
    }

    onMounted(() => mounted);
    function mounted() {
        if (selectedScenarioId !== getBlankGuid()) {            
            selectScenarioAction({
                scenarioId: selectedScenarioId,
            });
        }
    }

    onBeforeUnmount(() => beforeDestroy); 
    function beforeDestroy() {
        selectScenarioAction({ scenarioId: getBlankGuid() });
    }

    function onSubmitImportExportCommittedProjectsDialogResult(
        result: ImportExportCommittedProjectsDialogResult,
    ) {
        showImportExportCommittedProjectsDialog = false;

        if (hasValue(result)) {
            if (result.isExport) {
                CommittedProjectsService.exportCommittedProjects(
                    selectedScenarioId,
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
                        selectedScenarioId,
                    ).then((response: AxiosResponse) => {
                        if (
                            hasValue(response, 'status') &&
                            http2XX.test(response.status.toString())
                        ) {
                            addSuccessNotificationAction({
                                message: 'Successful upload.',
                                longMessage:
                                    'Successfully uploaded committed projects.',
                            });
                        }
                    });
                } else {
                    addErrorNotificationAction({
                        message: 'No file selected.',
                        longMessage:
                            'No file selected to upload the committed projects.',
                    });
                }
            }
        }
    }

    function onDeleteCommittedProjects() {
        alertDataForDeletingCommittedProjects = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }

    function onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        alertDataForDeletingCommittedProjects = { ...emptyAlertData };

        if (doDelete) {
            CommittedProjectsService.deleteSimulationCommittedProjects(
                selectedScenarioId,
            ).then((response: AxiosResponse) => {
                if (
                    hasValue(response) &&
                    http2XX.test(response.status.toString())
                ) {
                    addSuccessNotificationAction({
                        message: 'Committed projects have been deleted.',
                    });
                }
            });
        }
    }

    function visibleNavigationTabs() {
        return navigationTabs.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }

    /**
     * Shows the Alert
     */
    function onShowRunSimulationAlert() {
        alertData = {
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
    function onSubmitAlertResult(runScenarioSimulation: boolean) {
        alertData = clone(emptyAlertData);

        if (runScenarioSimulation) {
            runSimulationAction({
                networkId: networkId,
                scenarioId: selectedScenarioId,
            });
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


.text-primary .scenario-icon{
    fill: #FFFFFF !important;
}

.scenario-icon {
    fill: #999999 !important;
}

.text-primary .scenario-icon-stroke{
    stroke: #FFFFFF !important;
}

.scenario-icon-stroke {
    stroke: #999999 !important;
}

</style>
