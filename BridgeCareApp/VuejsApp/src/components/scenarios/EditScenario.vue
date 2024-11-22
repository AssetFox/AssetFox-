<template>
        <v-row>
            <v-col class="p-0">
                <v-card
                class="mx-auto ghd-sidebar-scenario"
                
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
                        :disabled="navigationTab.disabled"
                        v-for="navigationTab in visibleNavigationTabs()"
                    >
                        <v-list-item id="EditScenario-tabs-vListTile" :to="navigationTab.navigation" style="border-bottom: 1px solid #CCCCCC;">
                            <template v-slot:prepend>
                                    <TreatmentSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Treatment'"/>  
                                    <TargetConditionGoalSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Target Condition Goal'"/>  
                                    <RemainingLifeLimitSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Remaining Life Limit'"/>  
                                    <PerformanceCurveSvg style="height: 34px; width: 36px;"  class="scenario-icon" v-if="navigationTab.tabName === 'Deterioration Model'"/>  
                                    <DeficientConditionGoalSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Deficient Condition Goal'"/>  
                                    <InvestmentSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Investment'"/>  
                                    <CashFlowSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Cash Flow'"/>  
                                    <BudgetPrioritySvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Budget Priority'"/>  
                                    <AnalysisMethodSvg style="height: 38px; width: 34px"  class="scenario-icon" v-if="navigationTab.tabName === 'Analysis Method'"/>  
                                    <CalculatedAttributeSvg style="height: 32px; width: 32px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Calculated Attribute'"/>  
                                    <CommittedProjectSvg style="height: 32px; width: 32px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Committed Projects'"/>  
                                    <ReportsSvg style="height: 38px; width: 32px"  class="scenario-icon-stroke" v-if="navigationTab.tabName === 'Reports & Outputs'"/>  
                            </template>
                        <v-list-item-title style="display: flex; justify-content: space-between; align-items: center; padding-left: 5px; width: 100%;">
    <span style="font-size: 1.1rem !important;">{{ navigationTab.tabName }}</span>
    <i 
        :class="[navigationTab.validationIcon, { 
            'green-icon': navigationTab.validationIcon === 'fas fa-check-circle', 
            'red-icon': navigationTab.validationIcon === 'fas fa-times-circle', 
            'yellow-icon': navigationTab.validationIcon === 'fas fa-exclamation-circle' 
        }]" 
        v-if="navigationTab.validationIcon" 
        style="margin-left: 30px; font-size: 1.3rem;"
        v-b-tooltip.hover
        :title="getTooltipText(navigationTab.validationIcon, navigationTab.tabName)">
    </i>
</v-list-item-title>
                        </v-list-item>
                    </v-list-item>
                </v-list>
                <div style="margin: 10px;">
                    <v-btn
                        :class="{
                            'blue-run-icon ghd-button-text': !isBudgetPrioritySet,
                            'ghd-white-bg ghd-lt-gray ghd-button-text ghd-button-border': !isCommittedProjectsBudgetsUnset
                        }"
                        @click="onShowRunSimulationAlert"
                        :disabled="isBudgetPrioritySet || !isCommittedProjectsBudgetsUnset"
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

        <AlertPreChecks
            :dialogDataPreChecks="confirmAnalysisPreCheckAlertData"
            @submit="onConfirmAnalysisPreCheckAlertSubmit"
        />

        <AlertWithButtons
            :dialogDataWithButtons="confirmAnalysisRunAlertData"
            @submit="onConfirmAnalysisRunAlertSubmit"
        />

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
import Vue, { ref, shallowReactive, computed, watch, onMounted, onBeforeUnmount, onBeforeMount } from 'vue'; 
import { emptyScenario, Scenario, QueuedWork} from '@/shared/models/iAM/scenario';
import ImportExportCommittedProjectsDialog from '@/components/scenarios/scenarios-dialogs/ImportExportCommittedProjectsDialog.vue';
import { any, clone, isNil, propEq } from 'ramda';
import { AxiosResponse } from 'axios';
import CommittedProjectsService from '@/services/committed-projects.service';
import { Network } from '@/shared/models/iAM/network';
import FileDownload from 'js-file-download';
import { NavigationTab } from '@/shared/models/iAM/navigation-tab';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import { AlertData, AlertDataWithButtons, AlertPreChecksData, emptyAlertData, emptyAlertDataWithButtons, emptyAlertPreChecksData } from '@/shared/models/modals/alert-data';
import AlertPreChecks from '@/shared/modals/AlertPreChecks.vue';
import AlertWithButtons from '@/shared/modals/AlertWithButtons.vue';
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
import { SectionCommittedProject } from '@/shared/models/iAM/committed-projects';
import ReportsSvg from '@/shared/icons/ReportsSvg.vue';
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 
import ScenarioService from '@/services/scenario.service';
import InvestmentService from '@/services/investment.service';
import { InvestmentPagingRequestModel} from '@/shared/models/iAM/paging';
import PerformanceCurveService from '@/services/performance-curve.service';
import {
    PerformanceCurve,
} from '@/shared/models/iAM/performance';
import { PagingRequest } from '@/shared/models/iAM/paging';
import TreatmentService from '@/services/treatment.service';
import BudgetPriorityService from '@/services/budget-priority.service';
import { BudgetPriority } from '@/shared/models/iAM/budget-priority';
import { inject } from 'vue';
import mitt, { Emitter, EventType } from 'mitt';
import AnalysisMethodService from '@/services/analysis-method.service';
import CashFlowService from '@/services/cash-flow.service';

    let store = useStore(); 
    const router = useRouter(); 
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>

    const stateNetworks: Network[] = shallowReactive(store.state.networkModule.networks) ;
    const hasAdminAccess = computed<boolean>(() => store.state.authenticationModule.hasAdminAccess) ; 
    let hasSimulationAccess:boolean = (store.state.authenticationModule.hasSimulationAccess) ; 

    const stateSelectedScenario = computed<Scenario>(() => store.state.scenarioModule.selectedScenario) ;
    const stateSharedScenariosPage = computed<Scenario[]>(() => store.state.scenarioModule.currentSharedScenariosPage) ;
    const stateUserScenariosPage = computed<Scenario[]>(() =>store.state.scenarioModule.currentUserScenarioPage) ;
    let simulationRunSettingId = computed(() => store.state.scenarioModule.simulationRunSettingId);

    let userId = ref<string>(store.state.authenticationModule.userId);

    function addSuccessNotificationAction(payload?: any){ store.dispatch('addSuccessNotification',payload)}
    function addErrorNotificationAction(payload?: any){ store.dispatch('addErrorNotification',payload)}

    function selectScenarioAction(payload?: any){ store.dispatch('selectScenario',payload)} 

    async function runSimulationAction(payload?: any): Promise<any>{await store.dispatch('runSimulation', payload)}   
    async function runNewSimulationAction(payload?: any): Promise<any>{await store.dispatch('runNewSimulation',payload)}
    async function getScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {return await store.dispatch('getScenarioSelectableTreatments', payload)}
    async function getSimpleScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {await store.dispatch('getSimpleScenarioSelectableTreatments', payload);}
    async function getScenarioSimpleBudgetDetailsAction(payload?: any): Promise<any>{await store.dispatch('getScenarioSimpleBudgetDetails', payload)}
    async function getSimulationRunSettingAction(payload?: any): Promise<any> { await store.dispatch('getSimulationRunSetting', payload);}
 

    let selectedScenarioId: string = getBlankGuid();
    let showImportExportCommittedProjectsDialog: boolean = false;
    let networkId: string = getBlankGuid();
    let simulationName: string;
    let networkName: string = '';
    let selectedScenario: Scenario = clone(emptyScenario);
    let runAnalysisScenario: Scenario = clone(emptyScenario);
    let preCheckMessages: any;
    let preCheckHeading: string;
    let preCheckStatus: any;
    let emptyTreatmentBudgets: any;
    let confirmAnalysisRunAlertData= ref(clone(emptyAlertDataWithButtons));
    let confirmAnalysisPreCheckAlertData= ref(clone(emptyAlertPreChecksData));
    let isAnalysisMethodSet = ref(false);
    let isInvestmentSet = ref(false);
    let isDeteriorationModelSet = ref(false);
    let isTreatmentSet = ref(false);
    let isBudgetPrioritySet = ref(false);
    let isCashFlowSet = ref(false);
    let isCommittedProjectsSet = ref(false);
    let hasScenarioBeenRun = ref(false);
    let isCommittedProjectsBudgetsUnset = ref(true);

    let navigationTabs = ref<NavigationTab[]>([
    {
        tabName: 'Analysis Method',
        tabIcon: 'fas fa-chart-bar',
        validationIcon: '',
        navigation: {
            path: '/EditAnalysisMethod/',
        },
    },
    {
        tabName: 'Investment',
        tabIcon: 'fas fa-dollar-sign',
        validationIcon: '',
        navigation: {
            path: '/InvestmentEditor/Scenario/',
        },
        disabled: isAnalysisMethodSet.value,
    },
    {
        tabName: 'Deterioration Model',
        tabIcon: 'fas fa-chart-line',
        validationIcon: '',
        navigation: {
            path: '/PerformanceCurveEditor/Scenario/',
        },
        disabled: isInvestmentSet.value,
    },
    {
        tabName: 'Treatment',
        tabIcon: 'fas fa-tools',
        validationIcon: '',
        navigation: {
            path: '/TreatmentEditor/Scenario/',
        },
        disabled: isDeteriorationModelSet.value,
    },
    {
        tabName: 'Budget Priority',
        tabIcon: 'fas clipboard',
        validationIcon: '',
        navigation: {
            path: '/BudgetPriorityEditor/Scenario/',
        },
        disabled: isTreatmentSet.value,
    },
    {
        tabName: 'Calculated Attribute',
        tabIcon: 'fas fa-plus-square',
        navigation: {
            path: '/CalculatedAttributeEditor/Scenario/',
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
        validationIcon: '',
        navigation: {
            path: '/CashFlowEditor/Scenario/',
        },
        disabled: isBudgetPrioritySet.value,
    },
    {
        tabName: 'Committed Projects',
        validationIcon: '',
        tabIcon: 'fas fa-clipboard',
        navigation: {
            path: '/CommittedProjectsEditor/Scenario/',
        },
        disabled: isBudgetPrioritySet.value,
    },
    {
        tabName: 'Reports & Outputs',
        tabIcon: 'fas fa-clipboard',
        navigation: {
            path: '/ReportsAndOutputs/Scenario/',
        },
        disabled: hasScenarioBeenRun.value,
    },
    ]);

    const alertData = ref<AlertData>(clone(emptyAlertData));
    const alertDataForDeletingCommittedProjects = ref<AlertData>({ ...emptyAlertData });

    onMounted(() => {
        ScenarioSettingsUpdated()
        .then(() => getAnalysisMethod())
        .then(() => getTreatments())
        .then(() => getBudgetPriority())
        .then(() => getReportRunStatus())
        .then(() => getCashFlow())
        .then(() => getDeteriorationModel())
        .then(() => getInvestment())
        .then(() => getCommittedProjects())
    });
    
    onBeforeMount(() => {
        selectedScenarioId = router.currentRoute.value.query.scenarioId as string;
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
                navigationTabs.value = navigationTabs.value.map(
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
                            navigationTab['visible'] = hasAdminAccess.value;
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
                    navigationTabs.value,
                );
                // if no matching navigation path was found in the href, then route with path of first navigationTabs entry
                if (!hasChildPath) {
                    router.push(navigationTabs.value[0].navigation);
                }                
            }
    }

    watch(stateSelectedScenario, () => onStateSelectedScenarioChanged)
    function onStateSelectedScenarioChanged() {
        selectedScenario = clone(stateSelectedScenario.value);
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
        alertDataForDeletingCommittedProjects.value = {
            showDialog: true,
            heading: 'Are you sure?',
            message:
                "You are about to delete all of this scenario's committed projects.",
            choice: true,
        };
    }

    function onDeleteCommittedProjectsSubmit(doDelete: boolean) {
        alertDataForDeletingCommittedProjects.value = { ...emptyAlertData };

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
        return navigationTabs.value.filter(
            navigationTab =>
                navigationTab.visible === undefined || navigationTab.visible,
        );
    }

    async function ScenarioSettingsUpdated()
    {
        $emitter.on('AnalysisMethodUpdated', () => {
            // Update the disabled property of the Investment tab
            navigationTabs.value.forEach((tab) => {
                if (tab.tabName === 'Investment') {
                    tab.disabled = false;
                }
                else if(tab.tabName === 'Analysis Method')
                {
                    tab.validationIcon = 'fas fa-check-circle';
                }
            })
        });

        $emitter.on('InvestmentSettingsUpdated', () => {
            // Update the disabled property of the Deterioration Model tab
            navigationTabs.value.forEach((tab) => {
                if (tab.tabName === 'Deterioration Model') {
                    tab.disabled = false;
                }
                else if(tab.tabName === 'Investment')
                {
                    tab.validationIcon = 'fas fa-check-circle';
                }
            })
        });

        $emitter.on('DeteriorationModelSettingsUpdated', () => {                
            // Update the disabled property of the Treatment tab
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Treatment') {
                        tab.disabled = false;
                    }
                    else if(tab.tabName === 'Deterioration Model')
                    {
                        tab.validationIcon = 'fas fa-check-circle';
                    }
                });
        });

        $emitter.on('TreatmentSettingsUpdated', () => {
            // Update the disabled property of the Budget Priority tab
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Budget Priority') {
                        tab.disabled = false;
                    }
                    else if(tab.tabName === 'Treatment')
                    {
                        tab.validationIcon = 'fas fa-check-circle';
                    }
                });
        });

        $emitter.on('BudgetPriorityUpdated', () => {
            isBudgetPrioritySet.value = false;

            // Update the disabled property of the Committed Projects tab
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Committed Projects' || tab.tabName === 'Cash Flow') {
                        tab.disabled = false;
                    }
                    else if(tab.tabName === 'Budget Priority')
                    {
                        tab.validationIcon = 'fas fa-check-circle';
                    }
                });
        });

        $emitter.on('CashFlowUpdated', () => {
            isCashFlowSet.value = false;

            // Update the icon of the Cash Flow tab
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Cash Flow') {
                        tab.validationIcon = 'fas fa-check-circle';
                    }
                });
        });

        $emitter.on('CommittedProjectsUpdated', () => {
            isCashFlowSet.value = false;
            isCommittedProjectsBudgetsUnset.value = true;
            // Update the icon of the Committed Projects tab
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Committed Projects') {
                        tab.validationIcon = 'fas fa-check-circle';
                    }
                });
        });

        $emitter.on('SimulationRunSettingUpdated', () => {
            if(String(selectedScenarioId) === String(simulationRunSettingId.value))
            {
                hasScenarioBeenRun.value = false;

                // Update the disabled property of the Reports & Outputs tab
                navigationTabs.value.forEach((tab) => {
                        if (tab.tabName === 'Reports & Outputs') {
                            tab.disabled = false;
                        }
                    });
                navigationTabs.value = [...navigationTabs.value];
            }
        });

        $emitter.on('switchedToNewInvestmentLibrary', () => {
            navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Committed Projects') {
                        if(tab.validationIcon === 'fas fa-check-circle')
                        tab.validationIcon = 'fas fa-times-circle';
                    }
                });

                if(isCommittedProjectsSet.value === false)
                {
                    isCommittedProjectsBudgetsUnset.value = false;
                }
        });
        
    }
    
    /**
     * Shows the Alert
     */
    function onShowRunSimulationAlert() {
        confirmAnalysisRunAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Only one simulation can be run at a time. The model run you are about to queue will be ' +
                'executed in the order in which it was received.',
            buttons: []
        };
    }

    async function onConfirmAnalysisRunAlertSubmit(submit: string) {
        confirmAnalysisRunAlertData.value.showDialog = false;
        confirmAnalysisPreCheckAlertData.value = clone(emptyAlertPreChecksData);
        runAnalysisScenario = selectedScenario;

        if (submit == "pre-checks") {
            preCheckMessages = [];
                if (submit && selectedScenarioId !== getBlankGuid()) 
                {
                    await ScenarioService.upsertValidateSimulation(networkId, selectedScenarioId).then((response: AxiosResponse) => {
                        if (hasValue(response, 'status') && http2XX.test(response.status.toString())) {
                            addSuccessNotificationAction({message: "Simulation pre-checks completed",});
                            if(response.data.length > 0)
                            {
                                preCheckStatus = response.data[0].status;
                                for(const item of response.data)
                                {
                                    if (item.message != '') {
                                    preCheckMessages += item.message;
                                    }
                                }
                            }
                            else
                            {
                                preCheckStatus = 3;
                            }
                        }
                    });

                }
                // Check which treatments have no budgets and add them to the warning list
                emptyTreatmentBudgets = await getScenarioSelectableTreatmentsAction({ scenarioId: selectedScenarioId });
                emptyTreatmentBudgets.forEach((treatment: { budgets: string | any[]; name: any; }) => {
                    if (!treatment.budgets || treatment.budgets.length === 0) {
                        preCheckMessages += `Treatment ${treatment.name} has no budgets.`
                    }
                });

                secondRunAnalysisModal();
        }
        else if(submit == "continue") {
            store.dispatch('updateSimulationRunSettingName', simulationName);
            store.dispatch('updateSimulationRunSettingId', selectedScenarioId);
            if (submit && selectedScenarioId !== getBlankGuid()) {
                runSimulationAction({
                    networkId: networkId,
                    scenarioId: selectedScenarioId,
                }).then(() => (selectedScenario = clone(emptyScenario)));
            }
            router.push({ path: '/Scenarios/', query: { tab: 'General work queue' } }).catch(() => {});
        }

    }

    function secondRunAnalysisModal() {
        confirmAnalysisPreCheckAlertData.value = clone(emptyAlertPreChecksData);

            if(preCheckStatus == 0)
            {
                preCheckHeading = 'Error';
            }
            else if(preCheckStatus == 1)
            {
                preCheckHeading = 'Warning';
            }
            else if(preCheckStatus == 2)
            {
                preCheckHeading = 'Information';
            }
            else if(preCheckStatus == 3)
            {
                preCheckHeading = 'Success';
                preCheckMessages += 'No warnings have been returned.' + 'No errors have been returned';
            }

            if(preCheckStatus == 0)
            {
                (selectedScenario = clone(emptyScenario));
                confirmAnalysisPreCheckAlertData.value = {
                showDialog: true,
                heading: (preCheckHeading),
                choice: false,
                message:(preCheckMessages),
                }
            }
            else{
                (selectedScenario = clone(emptyScenario));
                confirmAnalysisPreCheckAlertData.value = {
                showDialog: true,
                heading: (preCheckHeading),
                choice: true,
                message:(preCheckMessages),
                }
            }
    }

    function onConfirmAnalysisPreCheckAlertSubmit(submit: boolean) {
        store.dispatch('updateSimulationRunSettingName', simulationName);
        store.dispatch('updateSimulationRunSettingId', selectedScenarioId);

        confirmAnalysisPreCheckAlertData.value = clone(emptyAlertPreChecksData);

        selectedScenario = runAnalysisScenario;

        if (submit && selectedScenarioId !== getBlankGuid()) {
            runSimulationAction({
                networkId: networkId,
                scenarioId: selectedScenarioId,
            }).then(() => (selectedScenario = clone(emptyScenario)));
            router.push({ path: '/Scenarios/', query: { tab: 'General work queue' } }).catch(() => {});
        }
    }


    /**
     * Takes in a boolean parameter from the AppPopupModal to determine if a scenario's simulation should be executed
     * @param runScenarioSimulation Alert result
     */
    function onSubmitAlertResult(runScenarioSimulation: boolean) {
        $emitter.emit('SimulationRunSettingUpdated', selectedScenario.id); 
        alertData.value = clone(emptyAlertData);

        if (runScenarioSimulation) {
            runSimulationAction({
                networkId: networkId,
                scenarioId: selectedScenarioId,
            });
        }
    }

    async function getAnalysisMethod()
    {
        await AnalysisMethodService.getSimulationAnalysisSetting(selectedScenarioId)
        .then((response: { data: any;}) => {
            if(typeof response.data === 'boolean')
            {
                isAnalysisMethodSet.value = response.data === false;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Analysis Method') {
                        if(isAnalysisMethodSet.value === true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-check-circle";
                    }
                });
                
                // Update the disabled property
                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Investment') {
                        tab.disabled = isAnalysisMethodSet.value;
                    }
                });
            }
        });
    }

    async function getInvestment()
    {
            const request: InvestmentPagingRequestModel = {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updatedBudgets: [],
                budgetsForDeletion: [],
                addedBudgets: [],
                deletionyears: [],
                updatedBudgetAmounts: {},
                Investment: null,
                addedBudgetAmounts: {},
                firstYearAnalysisBudgetShift: 0,
                isModified: false
            },
            sortColumn: 'year',
            isDescending: false,
            search: ''
        };

        await InvestmentService.getScenarioInvestmentPage(selectedScenarioId, request)
        .then((response: { data: any; }) => {
            if (response.data) {
                isInvestmentSet.value = response.data.firstYear == 0;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Investment') {
                        if(isInvestmentSet.value === true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-check-circle";
                    }
                });
                
                // Update the disabled property
                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Deterioration Model') {
                        tab.disabled = isInvestmentSet.value;
                    }
                });
            }
        });

    }

    async function getDeteriorationModel()
    {
        const request: PagingRequest<PerformanceCurve>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: "",
            isDescending: false,
            search: ""
        };

        store.dispatch('getIsDeteriorationModelApiRunning', true);
        await PerformanceCurveService.getPerformanceCurvePage(selectedScenarioId, request).then(response => {
            if (response.data) 
            {
                isDeteriorationModelSet.value = response.data.items.length == 0;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Deterioration Model') {
                        if(isDeteriorationModelSet.value === true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-check-circle";
                    }
                });

                
                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Treatment') {
                        tab.disabled = isDeteriorationModelSet.value;
                    }
                });
            }
        });
    }

    async function getTreatments()
    {
        await TreatmentService.getSimpleTreatmentsByScenarioId(selectedScenarioId).then(response => {
            if(response.data)
            {
                isTreatmentSet.value = response.data.length == 0;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Treatment') {
                        if(isTreatmentSet.value === true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-check-circle";
                    }
                });

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Budget Priority') {
                        tab.disabled = isTreatmentSet.value;
                    }
                });
            }   
        });
    }

    async function getBudgetPriority()
    {
        const request: PagingRequest<BudgetPriority>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: "",
            isDescending: false,
            search: ""
        };

        await BudgetPriorityService.getScenarioBudgetPriorityPage(selectedScenarioId, request).then(response => {
            if(response.data)
            {
                isBudgetPrioritySet.value = response.data.items.length == 0;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Budget Priority') {
                        if(isBudgetPrioritySet.value === true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-check-circle";
                    }
                });

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Committed Projects' || tab.tabName === 'Cash Flow') {
                        tab.disabled = isBudgetPrioritySet.value;
                    }
                });
            }   
        });
    }

    async function getCashFlow()
    {
        await CashFlowService.getScenarioCashFlowRules(selectedScenarioId).then(response => {
            if(response.data)
            {
                isCashFlowSet.value = response.data.length == 0;

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Cash Flow') {
                        if(isCashFlowSet.value === false)
                        {
                            tab.validationIcon = "fas fa-check-circle";
                        }
                        else
                        tab.validationIcon = "fas fa-exclamation-circle";
                    }
                });
            }   
        });
    }

    async function getCommittedProjects()
    {
        let hasUnsetBudgets = false;
        const request: PagingRequest<SectionCommittedProject>= {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: "",
            isDescending: false,
            search: ""
        };

        await CommittedProjectsService.getCommittedProjectsPage(selectedScenarioId, request).then(response => {
            if(response.data)
            {
                isCommittedProjectsSet.value = response.data.items.length == 0;

                if (response?.data?.items) {
                    response.data.items.forEach((item: { scenarioBudgetId: any; }) => {
                        if (!item.scenarioBudgetId || item.scenarioBudgetId === '') {
                            hasUnsetBudgets = true;
                        }
                    });
                }

                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Committed Projects') {
                        if(isCommittedProjectsSet.value === true)
                        {
                            tab.validationIcon = "fas fa-exclamation-circle";
                            isCommittedProjectsBudgetsUnset.value = true;
                        }
                        else if(hasUnsetBudgets == true)
                        {
                            tab.validationIcon = "fas fa-times-circle";
                            isCommittedProjectsBudgetsUnset.value = false;
                        }
                        else
                        {
                            tab.validationIcon = "fas fa-check-circle";
                            isCommittedProjectsBudgetsUnset.value = true;

                        }
                    }
                });
            }   

        })
    }

    function getTooltipText(validationIcon: string, tabName: string) 
    {
        // Map each name and message to a record
        const tabNameMessages: Record<string, string> = {
            'Analysis Method': 'Analysis Method must be saved before continuing',
            'Investment': 'Investment must be saved before continuing',
            'Deterioration Model': 'Deterioration Model must be saved before continuing',
            'Treatment': 'Treatment must be saved before continuing',
            'Budget Priority': 'Budget Priority must be saved before continuing',
            'Cash Flow': 'Warning: Cash flow has not been set',
            'Committed Projects': 'Warning: Committed Projects have not been set'
        };

        // Set the validation for each item
        if (validationIcon === 'fas fa-check-circle') {
            return 'All validations passed';
        } else if (validationIcon === 'fas fa-times-circle' && tabName in tabNameMessages) {
            return tabNameMessages[tabName];
        } else if (validationIcon === 'fas fa-exclamation-circle' && tabName in tabNameMessages) {
            return tabNameMessages[tabName];
        } else {
            return '';
        }
    }


    async function getReportRunStatus()
    {
        await ScenarioService.getSimulationRunSetting(selectedScenarioId).then(response => {
            if(typeof response.data === 'boolean')
            {
                hasScenarioBeenRun.value = response.data === false;
                
                // Update the disabled property
                navigationTabs.value.forEach((tab) => {
                    if (tab.tabName === 'Reports & Outputs') {
                        tab.disabled = hasScenarioBeenRun.value;
                    }
                });
            }  
        });

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

.blue-run-icon {
    background-color: #002E6C;
    color: white;
}

.red-icon {
  color: red;
}

.yellow-icon {
    color: #ffd32c;
}


</style>
