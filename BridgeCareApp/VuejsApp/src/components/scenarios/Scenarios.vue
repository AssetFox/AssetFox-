<template>
  <v-layout column>
    <v-flex xs12>
      <v-card elevation='5'>
        <v-flex xs10>
          <v-layout>
            <div class='network-min-width'>
              <v-data-table :headers='networkGridHeaders'
                            :items='networks'
                            :items-per-page='5'
                            class='elevation-1'
                            hide-actions>
                <template slot='items' slot-scope='props'>
                  <td>{{ props.item.name }}</td>
                  <td>{{ props.item.createdDate }}</td>
                  <td class='text-xs-center'>
                    <v-menu left min-height='500px' min-width='500px'>
                      <template slot='activator'>
                        <v-btn class='ara-blue' icon>
                          <v-icon>fas fa-eye</v-icon>
                        </v-btn>
                      </template>
                      <v-card>
                        <v-card-text>
                          <v-textarea class='sm-txt' :value='props.item.benefitQuantifier.equation.expression'
                                      full-width no-resize outline readonly rows='5' />
                        </v-card-text>
                      </v-card>
                    </v-menu>
                    <v-btn @click='onShowEquationEditorDialog(props.item.benefitQuantifier.equation)' class='edit-icon'
                           icon>
                      <v-icon>fas fa-edit</v-icon>
                    </v-btn>
                  </td>
                  <td class='status-min-width'>
                    {{ networkDataAssignmentStatus }}
                    <v-progress-linear v-model='networkDataAssignmentPercentage'
                                       color='light-green darken-1'
                                       height='25'
                                       striped>
                      <strong>{{ Math.ceil(networkDataAssignmentPercentage) }}%</strong>
                    </v-progress-linear>
                  </td>
                  <td>
                    <v-layout row wrap>
                      <v-flex>
                        <v-btn @click='onShowConfirmDataAggregationAlert(props.item.id)' class='green--text darken-1'
                               :disabled="props.item.benefitQuantifier.equation.expression === ''"
                               :title="props.item.benefitQuantifier.equation.expression === '' ? 'Add Benefit Quantifier to Aggregate' : 'Aggregate'"
                               icon>
                          <v-icon>fas fa-play</v-icon>
                        </v-btn>
                      </v-flex>
                    </v-layout>
                  </td>
                  <!--                   <td>
                                        <v-layout row wrap>
                                            <v-flex>
                                                <v-btn @click="onShowConfirmRollupAlert" class="green&#45;&#45;text darken-2"
                                                       icon>
                                                    <v-icon>fas fa-play</v-icon>
                                                </v-btn>
                                            </v-flex>
                                        </v-layout>
                                    </td>-->
                </template>
              </v-data-table>
            </div>
            <!-- <div class="pad-button" v-if="isAdmin">
                <v-btn @click="showCreateNetworkDialog = true" color="green darken-2 white--text" round>Create network
                </v-btn>
            </div> -->
          </v-layout>
        </v-flex>
      </v-card>
    </v-flex>
    <v-flex x12>
      <v-card elevation='5' color='blue lighten-5'>
        <v-card-title>
          <v-flex xs2>
            <v-chip color='ara-blue-bg' text-color='white'>
              My Scenarios
              <v-icon right>star</v-icon>
            </v-chip>
          </v-flex>
          <v-flex xs6>
            <v-text-field append-icon='fas fa-search'
                          hide-details
                          lablel='Search'
                          single-line
                          v-model='searchMine'>
            </v-text-field>
          </v-flex>
          <v-flex xs4 v-if='isAdmin'>
            <v-btn @click='showMigrateLegacySimulationDialog = true'
                   class='ara-light-gray-bg'
                   round>
              Migrate Legacy Simulation
            </v-btn>
          </v-flex>
        </v-card-title>
        <v-data-table :headers='scenarioGridHeaders'
                      :items='userScenarios'
                      :search='searchMine'>
          <template slot='items' slot-scope='props'>
            <td>
              <v-edit-dialog large
                             lazy
                             persistent
                             :return-value.sync='props.item.name'
                             @save='onEditScenarioName(props.item)'>
                {{ props.item.name }}
                <template slot='input'>
                  <v-text-field label='Edit'
                                single-line
                                v-model='props.item.name'
                                :rules="[rules['generalRules'].valueIsNotEmpty]" />
                </template>
              </v-edit-dialog>
            </td>
            <td>
              {{ props.item.creator ? props.item.creator : '[ Unknown ]' }}
            </td>
            <td>
              {{ props.item.owner ? props.item.owner : '[ No Owner ]' }}
            </td>
            <td>{{ formatDate(props.item.createdDate) }}</td>
            <td>{{ formatDate(props.item.lastModifiedDate) }}</td>
            <td>{{ formatDate(props.item.lastRun) }}</td>
            <td>{{ props.item.status }}</td>
            <td>{{ props.item.runTime }}</td>
            <td>{{ props.item.reportStatus }}</td>
            <td>
              <v-layout nowrap row>
                <v-flex>
                  <v-btn @click='onShowConfirmAnalysisRunAlert(props.item)'
                         class='ara-blue'
                         icon
                         title='Run Analysis'>
                    <v-icon>fas fa-play</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowReportsDownloaderDialog(props.item)'
                         class='ara-blue'
                         icon
                         title='Reports'>
                    <v-icon>fas fa-chart-line</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onNavigateToEditScenarioView(props.item.id, props.item.name)'
                         class='edit-icon'
                         icon
                         title='Settings'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowShareScenarioDialog(props.item)'
                         class='ara-blue'
                         icon
                         title='Share'>
                    <v-icon>fas fa-users</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowConfirmCloneScenarioAlert(props.item)'
                         class='ara-blue'
                         icon
                         title='Clone'>
                    <v-icon>fas fa-paste</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowConfirmDeleteAlert(props.item)'
                         class='ara-orange'
                         icon
                         title='Delete'>
                    <v-icon>fas fa-trash</v-icon>
                  </v-btn>
                </v-flex>
              </v-layout>
            </td>
          </template>
          <v-alert :value='true'
                   class='ara-orange-bg'
                   icon='fas fa-exclamation'
                   slot='no-results'>
            Your search for "{{ searchMine }}" found no results.
          </v-alert>
        </v-data-table>
        <v-card-actions color='white'>
          <div style='width:2em' />
          <v-btn @click='showCreateScenarioDialog = true'
                 color='green darken-2 white--text'>
            Create new scenario
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>

    <v-flex xs12>
      <v-card elevation='5' color='blue lighten-3'>
        <v-card-title>
          <v-flex xs4>
            <v-chip class='ara-blue-bg white--text'>
              Shared with Me
              <v-icon right>share</v-icon>
            </v-chip>
          </v-flex>
          <v-spacer />
          <v-flex xs6>
            <v-text-field append-icon='fas fa-search'
                          hide-details
                          lablel='Search'
                          single-line
                          v-model='searchShared'>
            </v-text-field>
          </v-flex>
        </v-card-title>
        <v-data-table :headers='scenarioGridHeaders'
                      :items='sharedScenarios'
                      :search='searchShared'>
          <template slot='items' slot-scope='props'>
            <td>
              <v-edit-dialog large
                             lazy
                             persistent
                             :return-value.sync='props.item.name'
                             @save='onEditScenarioName(props.item)'>
                {{ props.item.name }}
                <template slot='input'>
                  <v-text-field label='Edit'
                                single-line
                                v-model='props.item.name'
                                :rules="[rules['generalRules'].valueIsNotEmpty]" />
                </template>
              </v-edit-dialog>
            </td>
            <td>{{ props.item.creator ? props.item.creator : '[ Unknown ]' }}
            </td>
            <td>{{ props.item.owner ? props.item.owner : '[ No Owner ]' }}
            </td>
            <td>{{ formatDate(props.item.createdDate) }}</td>
            <td>{{ formatDate(props.item.lastModifiedDate) }}</td>
            <td>{{ formatDate(props.item.lastRun) }}</td>
            <td>{{ props.item.status }}</td>
            <td>{{ props.item.runTime }}</td>
            <td>{{ props.item.reportStatus }}</td>
            <td>
              <v-layout nowrap row>
                <v-flex>
                  <v-btn v-if='canModifySharedScenario(props.item.users)'
                         @click='onShowConfirmAnalysisRunAlert(props.item)'
                         class='ara-blue'
                         flat
                         icon
                         title='Run Analysis'>
                    <v-icon>fas fa-play</v-icon>
                  </v-btn>
                  <v-btn v-else :disabled='true'
                         class='ara-light-gray'
                         flat
                         icon
                         title='Not authorized to run analysis'>
                    <v-icon>fas fa-play</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowReportsDownloaderDialog(props.item)'
                         class='ara-blue'
                         flat
                         icon
                         title='Reports'>
                    <v-icon>fas fa-chart-line</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn v-if='canModifySharedScenario(props.item.users)'
                         @click='onNavigateToEditScenarioView(props.item.id, props.item.name)'
                         class='edit-icon'
                         flat
                         icon
                         title='Settings'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                  <v-btn v-else :disabled='true'
                         class='ara-light-gray'
                         flat
                         icon
                         title='Not authorized to edit'>
                    <v-icon>fas fa-edit</v-icon>
                  </v-btn>
                </v-flex>
                <v-flex>
                  <v-btn @click='onShowConfirmCloneScenarioAlert(props.item)'
                         class='ara-blue'
                         icon
                         title='Clone'>
                    <v-icon>fas fa-paste</v-icon>
                  </v-btn>
                </v-flex>
              </v-layout>
            </td>
          </template>
          <v-alert :value='true'
                   class='ara-orange-bg'
                   icon='fas fa-exclamation'
                   slot='no-results'>
            Your search for "{{ searchShared }}" found no results.
          </v-alert>
        </v-data-table>
      </v-card>
    </v-flex>

    <!--    <CreateNetworkDialog :showDialog="showCreateNetworkDialog" @submit="onCreateNetworkDialogSubmit"/>-->

    <!--    <ConfirmRollupAlert :dialogData="confirmRollupAlertData" @submit="onConfirmRollupAlertSubmit"/>-->

    <ConfirmDataAssignmentAlert :dialogData='confirmDataAggregationAlertData'
                                @submit='onConfirmDataAggregationAlertSubmit' />

    <ConfirmAnalysisRunAlert :dialogData='confirmAnalysisRunAlertData' @submit='onConfirmAnalysisRunAlertSubmit' />

    <ReportsDownloaderDialog :dialogData='reportsDownloaderDialogData' />

    <ShareScenarioDialog :dialogData='shareScenarioDialogData' @submit='onShareScenarioDialogSubmit' />

    <ConfirmCloneScenarioAlert :dialogData='confirmCloneScenarioAlertData'
                               @submit='onConfirmCloneScenarioAlertSubmit' />

    <ConfirmDeleteAlert :dialogData='confirmDeleteAlertData' @submit='onConfirmDeleteAlertSubmit' />

    <CreateScenarioDialog :showDialog='showCreateScenarioDialog' @submit='onCreateScenarioDialogSubmit' />

    <MigrateLegacySimulationDialog :showDialog='showMigrateLegacySimulationDialog'
                                   @submit='onMigrateLegacySimulationSubmit' />

    <EquationEditorDialog :dialogData='equationEditorDialogData' @submit='onSubmitEquationEditorDialogSubmit' />
  </v-layout>
</template>

<script lang='ts'>
import Vue from 'vue';
import { Component, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import moment from 'moment';
import { emptyScenario, Scenario, ScenarioUser } from '@/shared/models/iAM/scenario';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import ReportsDownloaderDialog from '@/components/scenarios/scenarios-dialogs/ReportsDownloaderDialog.vue';
import {
  emptyReportsDownloadDialogData,
  ReportsDownloaderDialogData,
} from '@/shared/models/modals/reports-downloader-dialog-data';
import CreateScenarioDialog from '@/components/scenarios/scenarios-dialogs/CreateScenarioDialog.vue';
import ShareScenarioDialog from '@/components/scenarios/scenarios-dialogs/ShareScenarioDialog.vue';
import { Network } from '@/shared/models/iAM/network';
import { any, clone, isNil } from 'ramda';
import { getUserName } from '@/shared/utils/get-user-info';
import { InputValidationRules, rules } from '@/shared/utils/input-validation-rules';
import CreateNetworkDialog from '@/components/scenarios/scenarios-dialogs/CreateNetworkDialog.vue';
import { DataTableHeader } from '@/shared/models/vue/data-table-header';
import {
  emptyShareScenarioDialogData,
  ShareScenarioDialogData,
} from '@/shared/models/modals/share-scenario-dialog-data';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import MigrateLegacySimulationDialog from '@/components/scenarios/scenarios-dialogs/MigrateLegacySimulationDialog.vue';
import { Equation } from '@/shared/models/iAM/equation';
import EquationEditorDialog from '@/shared/modals/EquationEditorDialog.vue';
import {
  emptyEquationEditorDialogData,
  EquationEditorDialogData,
} from '@/shared/models/modals/equation-editor-dialog-data';
import { Hub } from '@/connectionHub';

@Component({
  components: {
    MigrateLegacySimulationDialog,
    ConfirmCloneScenarioAlert: Alert,
    ConfirmDeleteAlert: Alert,
    ConfirmRollupAlert: Alert,
    ConfirmDataAssignmentAlert: Alert,
    ConfirmAnalysisRunAlert: Alert,
    ReportsDownloaderDialog,
    CreateScenarioDialog,
    CreateNetworkDialog,
    ShareScenarioDialog,
    EquationEditorDialog,
  },
})
export default class Scenarios extends Vue {
  @State(state => state.networkModule.networks) stateNetworks: Network[];
  @State(state => state.scenarioModule.scenarios) stateScenarios: Scenario[];

  @State(state => state.breadcrumbModule.navigation) navigation: any[];

  //@State(state => state.rollup.rollups) rollups: Rollup[];
  @State(state => state.authenticationModule.authenticated) authenticated: boolean;
  @State(state => state.authenticationModule.userId) userId: string;
  @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
  @State(state => state.authenticationModule.isCWOPA) isCWOPA: boolean;

  @Action('getScenarios') getScenariosAction: any;
  @Action('createScenario') createScenarioAction: any;
  @Action('cloneScenario') cloneScenarioAction: any;
  @Action('updateScenario') updateScenarioAction: any;
  @Action('deleteScenario') deleteScenarioAction: any;
  @Action('runSimulation') runSimulationAction: any;
  @Action('migrateLegacySimulationData') migrateLegacySimulationDataAction: any;
  @Action('updateSimulationAnalysisDetail') updateSimulationAnalysisDetailAction: any;
  @Action('updateSimulationReportDetail') updateSimulationReportDetailAction: any;
  @Action('selectScenario') selectScenarioAction: any;

  /*@Action('rollupNetwork') rollupNetworkAction: any;
  @Action('createNetwork') createNetworkAction: any;*/
  @Action('upsertBenefitQuantifier') upsertBenefitQuantifierAction: any;
  @Action('aggregateNetworkData') aggregateNetworkDataAction: any;

  @Action('setSuccessMessage') setSuccessMessageAction: any;
  @Action('setWarningMessage') setWarningMessageAction: any;
  @Action('setErrorMessage') setErrorMessageAction: any;
  @Action('setInfoMessage') setInfoMessageAction: any;

  networkGridHeaders: DataTableHeader[] = [
    { text: 'Network', value: 'name', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Date Created', value: 'createdDate', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Benefit Quantifier', value: '', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Status', value: 'status', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Aggregate Data', value: '', align: 'left', sortable: false, class: '', width: '' },
  ];
  networks: Network[] = [];
  scenarioGridHeaders: DataTableHeader[] = [
    { text: 'Scenario', value: 'name', align: 'left', sortable: true, class: '', width: '' },
    { text: 'Creator', value: 'creator', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Owner', value: 'owner', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Date Created', value: 'createdDate', align: 'left', sortable: true, class: '', width: '' },
    { text: 'Date Last Modified', value: 'lastModifiedDate', align: 'left', sortable: true, class: '', width: '' },
    { text: 'Date Last Run', value: 'lastRun', align: 'left', sortable: true, class: '', width: '' },
    { text: 'Status', value: 'status', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Run Time', value: 'runTime', align: 'left', sortable: false, class: '', width: '' },
    { text: 'Report Status', value: 'reportStatus', align: 'left', sortable: false, class: '', width: '' },
    { text: '', value: '', align: 'left', sortable: false, class: '', width: '' },
  ];
  scenarios: Scenario[] = [];
  userScenarios: Scenario[] = [];
  sharedScenarios: Scenario[] = [];
  searchMine = '';
  searchShared = '';
  //confirmRollupAlertData: AlertData = clone(emptyAlertData);
  equationEditorDialogData: EquationEditorDialogData = clone(emptyEquationEditorDialogData);
  confirmDataAggregationAlertData: AlertData = clone(emptyAlertData);
  //showCreateNetworkDialog: boolean = false;
  reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(emptyReportsDownloadDialogData);
  confirmAnalysisRunAlertData: AlertData = clone(emptyAlertData);
  shareScenarioDialogData: ShareScenarioDialogData = clone(emptyShareScenarioDialogData);
  confirmCloneScenarioAlertData: AlertData = clone(emptyAlertData);
  confirmDeleteAlertData: AlertData = clone(emptyAlertData);
  showCreateScenarioDialog: boolean = false;
  selectedScenario: Scenario = clone(emptyScenario);
  networkDataAssignmentStatus: string = '';
  networkDataAssignmentPercentage = 0;
  rules: InputValidationRules = rules;
  showMigrateLegacySimulationDialog: boolean = false;

  @Watch('stateNetworks')
  onStateNetworksChanged() {
    this.networks = clone(this.stateNetworks);
      if (hasValue(this.networks)) {
          this.getScenariosAction({ networkId: this.networks[0].id });
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
    this.userScenarios = this.scenarios.filter((scenario: Scenario) => scenario.owner === username);
    // filter scenarios that are shared with the user
    const scenarioUserCanModify = (user: ScenarioUser) => user.username === username;
    const sharedScenarioFilter = (scenario: Scenario) =>
        scenario.owner !== username && (this.isAdmin || this.isCWOPA || any(scenarioUserCanModify, scenario.users));
    this.sharedScenarios = this.scenarios.filter(sharedScenarioFilter);
  }

  mounted() {
    this.networks = clone(this.stateNetworks);
    if (hasValue(this.stateScenarios)) {
        this.scenarios = clone(this.stateScenarios);
    } else if (hasValue(this.networks)) {
        this.getScenariosAction({ networkId: this.networks[0].id });
    }


    this.$statusHub.$on(Hub.BroadcastEventType.BroadcastAssignDataStatusEvent, this.getDataAggregationStatus);
    this.$statusHub.$on(Hub.BroadcastEventType.BroadcastDataMigrationEvent, this.getDataMigrationStatus);
    this.$statusHub.$on(Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent, this.getScenarioAnalysisDetailUpdate);
    this.$statusHub.$on(Hub.BroadcastEventType.BroadcastSummaryReportGenerationStatusEvent, this.getSummaryReportStatus);
  }

  beforeDestroy() {
    this.$statusHub.$off(Hub.BroadcastEventType.BroadcastAssignDataStatusEvent, this.getDataAggregationStatus);
    this.$statusHub.$off(Hub.BroadcastEventType.BroadcastDataMigrationEvent, this.getDataMigrationStatus);
    this.$statusHub.$off(Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent, this.getScenarioAnalysisDetailUpdate);
    this.$statusHub.$off(Hub.BroadcastEventType.BroadcastSummaryReportGenerationStatusEvent, this.getSummaryReportStatus);
  }

  formatDate(dateToFormat: Date) {
    return hasValue(dateToFormat) ? moment(dateToFormat).format('M/D/YYYY') : null;
  }

  canModifySharedScenario(scenarioUsers: ScenarioUser[]) {
    const currentUser: string = getUserName();
    const scenarioUserCanModify = (user: ScenarioUser) => user.username === currentUser && user.canModify;
    return this.isAdmin || this.isCWOPA || any(scenarioUserCanModify, scenarioUsers);
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

  onShowEquationEditorDialog(equation: Equation) {
    this.equationEditorDialogData = {
      showDialog: true,
      equation: equation,
    };
  }

  onSubmitEquationEditorDialogSubmit(equation: Equation) {
    this.equationEditorDialogData = clone(emptyEquationEditorDialogData);

    if (!isNil(equation) && hasValue(this.networks)) {
      this.upsertBenefitQuantifierAction({
        benefitQuantifier: {
          ...this.networks[0].benefitQuantifier,
          equation: equation,
        },
      });
    }
  }

  onShowConfirmDataAggregationAlert() {
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
        networkId: this.networks[0].id,
      });
    }
  }

  // TODO: update to send no payload when API is modified to migrate ALL simulations
  onStartDataMigration() {
    // the legacy scenario id is hardcoded to our test scenario "JML Run District 8"
    this.migrateLegacySimulationDataAction({ simulationId: process.env.VUE_APP_HARDCODED_SCENARIOID_FROM_LEGACY });
  }

  onEditScenarioName(scenario: Scenario) {
    if (hasValue(scenario.name)) {
      this.updateScenarioAction({ scenario: scenario });
    } else {
      this.scenarios = [];
      setTimeout(() => (this.scenarios = clone(this.stateScenarios)));
    }
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
        networkId: this.networks[0].id,
        scenarioId: this.selectedScenario.id,
      }).then(() => this.selectedScenario = clone(emptyScenario));
    }
  }

  onShowReportsDownloaderDialog(scenario: Scenario) {
    this.reportsDownloaderDialogData = {
      showModal: true,
      scenarioId: scenario.id,
      networkId: this.networks[0].id,
    };
  }

  onNavigateToEditScenarioView(id: string, name: string) {
    this.selectScenarioAction({ scenarioId: id });

    this.$router.push({
      path: '/EditScenario/',
      query: {
        scenarioId: id,
        scenarioName: name,
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

  onConfirmCloneScenarioAlertSubmit(submit: boolean) {
    this.confirmCloneScenarioAlertData = clone(emptyAlertData);

    if (submit && this.selectedScenario.id !== getBlankGuid()) {
      this.cloneScenarioAction({ scenarioId: this.selectedScenario.id })
          .then(() => this.selectedScenario = clone(emptyScenario));
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
      }).then(() => this.selectedScenario = clone(emptyScenario));
    }
  }

  getDataAggregationStatus(data: any) {
    this.networkDataAssignmentStatus = data.status;
    this.networkDataAssignmentPercentage = data.percentage;
  }

  getDataMigrationStatus(data: any) {
    const status: any = data.status;
    if (status.indexOf('Error') !== -1) {
      this.setErrorMessageAction({ message: data.status });
    } else {
      this.setInfoMessageAction({ message: data.status });
    }
  }

  getScenarioAnalysisDetailUpdate(data: any) {
    this.updateSimulationAnalysisDetailAction({ simulationAnalysisDetail: data.simulationAnalysisDetail });
  }

  getSummaryReportStatus(data: any) {
    this.updateSimulationReportDetailAction({ simulationReportDetail: data.simulationReportDetail });
  }

  onCreateScenarioDialogSubmit(scenario: Scenario) {
    this.showCreateScenarioDialog = false;

    if (!isNil(scenario)) {
      this.createScenarioAction({
        scenario: scenario,
        networkId: this.networks[0].id,
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
</style>
