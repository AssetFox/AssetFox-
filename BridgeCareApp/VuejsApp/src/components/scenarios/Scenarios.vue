<template>
    <v-row column>
        <v-col cols = "12">
            <v-card elevation="5" color="blue lighten-5">
                <v-tabs center-active v-model="tab">
                    <v-tab
                        v-for="item in tabItems"
                        :key="item.name"
                        :value="item.name"
                        class="tab-theme"
                    >
                        <GhdQueueSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'Work queue'"/> 
                        <GhdShareSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'Shared with me'"/>  
                        <GhdStarSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'My scenarios'"/>  
                        {{ item.name }} ( {{ item.count }} )
                    </v-tab>
                    <v-spacer></v-spacer>
                    <v-col cols = "1"></v-col>
                </v-tabs>
                <v-window v-model="tab" >
                    <v-window-item value="My scenarios">
                         <v-col cols = "12">
                            <v-card elevation="5">
                                <v-card-title>
                                    <v-col cols = "6">
                                        <v-row style = "margin-right: -100px;">
                                            <v-text-field
                                                id="Scenarios-searchScenarios-textField"
                                                type="text"
                                                placeholder="Search in scenarios"
                                                hide-details
                                                single-line
                                                v-model="searchMine"
                                                outline
                                                clearable
                                                @click:clear="onMineClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn id="Scenarios-performSearch-button" 
                                                style="margin-top: 2px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="onMineSearchClick()">
                                                Search
                                            </v-btn>
                                            <v-btn id="Scenarios-performFilter-button" 
                                                style="margin-top: 2px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="showFilterScenarioList = true">
                                                Filter
                                            </v-btn>
                                            <span class="Scenario-status"  >
                                                <div v-if = "(sortedMineCategory != '' && sortedMineValue!='')&&(sortedMineValue != undefined && sortedMineCategory != null)">
                                                     Current Filter: 
                                                     <v-chip
                                                        class="ma-2"
                                                        closeable
                                                        color=primary
                                                        text-color="white"
                                                        :model-value="true"
                                                        icon
                                                        >
                                                        {{sortedMineCategory}} by {{ sortedMineValue }}
                                                        
                                                        <img class='img-general' :src="require('@/assets/icons/x-circle.svg')" @click="onMineFilterClearClick()"  >
                                                </v-chip>
                                                </div>
                                            </span>
                                        </v-row>
                                    </v-col>
                                    <v-col cols = "4"></v-col>
                                    <v-row class="flex-end xs2" style="justify-content: end; padding-right: 70px">
                                        <v-btn
                                           id="Scenarios-createScenario-btn"
                                            @click="
                                                showCreateScenarioDialog = true
                                            "
                                            color="blue darken-2 text-white"
                                        >
                                            Create new scenario
                                        </v-btn>
                                    </v-row>
                                </v-card-title>
                                <v-data-table
                                    id="Scenarios-scenarios-datatable"
                                    :items="currentUserScenariosPage"                      
                                    :totalItems="totalUserScenarios"
                                    :pagination.sync="userScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    
                                    calculate-widths
                                >
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <td>
                                        
                                            <v-dialog
                                                size="large"
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
                                                <template v-slot:input>
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
                                            </v-dialog>
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
                                            <v-menu offset location="left">
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"

                                                >
                                                    <v-btn
                                                        id="Scenarios-actionMenu-vbtn"
                                                        color="text-green darken-1"
                                                        icon
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-item
                                                        v-for="(item,i) in actionItems"
                                                        :key="i"
                                                        @click="OnActionTaken(
                                                                item.action,
                                                                props.item.users,
                                                                props.item,
                                                                true) "
                                                        class="menu-style">
                                                        <v-list-item-title icon>
                                                            <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                            <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon> 
                                                            {{item.title}}
                                                        </v-list-item-title>
                                                    </v-list-item>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                    <!-- <v-alert
                                        :model-value="hasMineSearch()"
                                        class="ara-orange-bg"
                                        icon="fas fa-exclamation"
                                        slot="no-data"
                                    >
                                        Your search for "{{ currentSearchMine }}" found
                                        no results.
                                    </v-alert> -->
                                </v-data-table>
                            </v-card>
                        </v-col>
                    </v-window-item>
                    <v-window-item value="Shared with me">
                       <v-col cols = "12">
                            <v-card elevation="5">
                                <v-card-title>
                                    <v-col cols = "6">
                                        <v-row style = "margin-right:-200px">
                                            <v-text-field
                                                id="Scenarios-shared-searchScenarios-textField"
                                                label="Search"
                                                placeholder="Search in scenarios"
                                                outline
                                                hide-details
                                                single-line
                                                v-model="searchShared"
                                                clearable
                                                @click:clear="onSharedClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn style="margin-top: 2px;" 
                                                id="Scenarios-shared-performSearch-button"
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="onSharedSearchClick()">
                                                Search
                                            </v-btn>
                                            <v-btn id="Scenarios-performFilter-button" 
                                                style="margin-top: 2px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="showSharedFilterScenarioList = true">
                                                Filter
                                            </v-btn>
                                            <span class="Scenario-status"  >
                                                <div v-if = "(sortedCategory != '' && sortedValue!='')&&(sortedValue != undefined && sortedCategory != null)">
                                                     Current Filter: 
                                                     <v-chip
                                                        class="ma-2"
                                                        closeable
                                                        color=primary
                                                        text-color="white"
                                                        :model-value="true"
                                                        icon
                                                        >
                                                        {{sortedCategory}} by {{ sortedValue }}
                                                        
                                                        <img class='img-general' :src="require('@/assets/icons/x-circle.svg')" @click="onSharedFilterClearClick()"  >
                                                </v-chip>
                                                </div>
                                            </span>
                                        </v-row>
                                        
                                    </v-col>
                                </v-card-title>
                                <v-data-table
                                    :items="currentSharedScenariosPage"                      
                                    :totalItems="totalSharedScenarios"
                                    :pagination.sync="sharedScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    sort-icon=$vuetify.icons.ghd-table-sort
                                >
                                    <template slot="items" slot-scope="props" v-slot:item="props">

                                        <td>
                                            <v-edit-dialog
                                                size="large"
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
                                                <template v-slot:input>
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
                                            <v-menu offset location="left">
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"
                                                >
                                                    <v-btn
                                                        id="Scenarios-shared-actionMenu-vbtn"
                                                        color="text-green darken-1"
                                                        icon
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-item v-for="(item,i) in actionItemsForSharedScenario"
                                                        :key="i"
                                                        @click="OnActionTaken(item.action,props.item.users,props.item,false)"
                                                        class="menu-style">
                                                        <v-list-item-title icon>                                                        
                                                            <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                            <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon>  
                                                            {{item.title}}
                                                        </v-list-item-title>
                                                    </v-list-item>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </template>
                                                                 
                                </v-data-table>
                            </v-card>
                        </v-col>
                     two
                    </v-window-item>
                    <v-window-item value="Work queue">
                        <!-- <v-col cols = "12">
                            <v-card elevation="5">
                                <v-card-title class="ghd-dialog-padding-top-title">
                                    <v-row justify-start>
                                    <div class="dialog-header"><h5>Work Queue</h5></div>
                                    </v-row>

                                </v-card-title>
                                <v-data-table
                                    :headers="workQueueGridHeaders"
                                    :items="currentWorkQueuePage"
                                    :totalItems="totalQueuedSimulations"
                                    :pagination.sync="workQueuePagination"
                                    sort-icon=$vuetify.icons.ghd-table-sort
                                >                           
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <td>{{ props.item.queuePosition }}</td>
                                        <td>
                                            {{ props.item.name }}
                                        </td>
                                        <td>{{props.item.workDescription}}</td>
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
                                            <v-menu offset location="left">
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"
                                                >
                                                    <v-btn
                                                        color="text-green darken-1"
                                                        icon
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile v-for="(item,i) in actionItemsForWorkQueue"
                                                        :key="i"
                                                        @click="OnWorkQueueActionTaken(item.action,props.item)"
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
                                    <template v-slot:no-data>
                                        {{ getEmptyWorkQueueMessage() }}
                                    </template>
                                </v-data-table>
                                <v-card-title class="ghd-dialog-padding-top-title">
                                    <v-row justify-start>
                                    <div class="dialog-header"><h5>Fast Queue</h5></div>
                                    </v-row>

                                </v-card-title>
                                <v-data-table
                                    :headers="workQueueGridHeaders"
                                    :items="currentFastWorkQueuePage"
                                    :totalItems="totalFastQueuedItems"
                                    :pagination.sync="fastWorkQueuePagination"
                                    sort-icon=$vuetify.icons.ghd-table-sort
                                >                           
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <td>{{ props.item.queuePosition }}</td>
                                        <td>
                                            {{ props.item.name }}
                                        </td>
                                        <td>{{props.item.workDescription}}</td>
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
                                            <v-menu offset left>
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"
                                                >
                                                    <v-btn
                                                        color="text-green darken-1"
                                                        icon
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="require('@/assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile v-for="(item,i) in actionItemsForFastWorkQueue"
                                                        :key="i"
                                                        @click="OnWorkQueueActionTaken(item.action,props.item)"
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
                                    <template v-slot:no-data>
                                        {{ getEmptyWorkQueueMessage() }}
                                    </template>
                                </v-data-table>
                            </v-card>
                        </v-col> -->
                        three
                    </v-window-item>
                </v-window>
            </v-card>
        </v-col>
        <!-- missing implemtation
         <ConfirmAnalysisRunAlertPrehecks
            :dialogDataPreChecks="onSecondConfirmAnalysisRunAlertData"
            @submit="onSecondConfirmAnalysisRunAlertSubmit"
        />
        -->
         <ConfirmAnalysisRunAlertWithButtons :is="Alert" 
            :dialogDataWithButtons="confirmAnalysisRunAlertData"
            @submit="onConfirmAnalysisRunAlertSubmit"
            />

        <ConfirmConvertToRelationalAlert :is="Alert"
            :dialogData="ConfirmConvertJsonToRelationalData"
            @submit="onConfirmConvertJsonToRelationalAlertSubmit"
        />

        <ReportsDownloaderDialog :dialogData="reportsDownloaderDialogData" />

        <ShareScenarioDialog
            :dialogData="shareScenarioDialogData"
            @submit="onShareScenarioDialogSubmit"
        />

        <ConfirmCloneScenarioAlert :is="Alert"
            :dialogData="confirmCloneScenarioAlertData"
            @submit="onConfirmCloneScenarioAlertSubmit"
        />

        <ConfirmDeleteAlert :is="Alert"
            :dialogData="confirmDeleteAlertData"
            @submit="onConfirmDeleteAlertSubmit"
        />
       
        <ConfirmCancelAlert :is="Alert"
            :dialogData="confirmCancelAlertData"
            @submit="onConfirmCancelAlertSubmit"
        />
        
        <CreateScenarioDialog
            :showDialog="showCreateScenarioDialog"
            @submit="onCreateScenarioDialogSubmit"
        />
        <!-- missing implemtation
            <FilterScenarioList
            :showDialog="showSharedFilterScenarioList"
            @submit="onFilterSharedScenarioListSubmit"
        />
        <FilterScenarioList
            :showDialog="showFilterScenarioList"
            @submit="onFilterScenarioListSubmit"
        />
        -->

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
        <CommittedProjectsFileUploaderDialog :is="ImportExportCommittedProjectsDialog" 
            :showDialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
            @delete="onDeleteCommittedProjects"
        />
    </v-row>
</template>

<script lang="ts" setup>
import Vue, { Ref, ref, shallowReactive, shallowRef, ShallowRef, watch, onBeforeUnmount, onMounted, inject, readonly, computed, reactive } from 'vue'; 
import moment from 'moment';
import {
    emptyScenario,
    Scenario,
    ScenarioActions,
    TabItems,
    ScenarioUser,
    emptyQueuedWork,
    QueuedWork,
    WorkType,
} from '@/shared/models/iAM/scenario';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, AlertDataWithButtons, AlertPreChecksData, emptyAlertData, emptyAlertDataWithButtons, emptyAlertPreChecksData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import AlertWithButtons from '@/shared/modals/AlertWithButtons.vue';
import AlertPreChecks from '@/shared/modals/AlertPreChecks.vue';
import { emptyAlertButton } from '@/shared/models/modals/alert-data';
import ReportsDownloaderDialog from '@/components/scenarios/scenarios-dialogs/ReportsDownloaderDialog.vue';
import ShowAggregationDialog from '@/components/scenarios/scenarios-dialogs/ShowAggregationDialog.vue';
import {
    emptyReportsDownloadDialogData,
    ReportsDownloaderDialogData,
} from '@/shared/models/modals/reports-downloader-dialog-data';
import CloneScenarioDialog from '@/components/scenarios/scenarios-dialogs/CloneScenarioDialog.vue'
import { CloneScenarioDialogData, emptyCloneScenarioDialogData } from '@/shared/models/modals/clone-scenario-dialog-data'
import CreateScenarioDialog from '@/components/scenarios/scenarios-dialogs/CreateScenarioDialog.vue';
import FilterScenarioList from '@/components/scenarios/scenarios-dialogs/FilterScenarioList.vue';
import ShareScenarioDialog from '@/components/scenarios/scenarios-dialogs/ShareScenarioDialog.vue';
import { Network } from '@/shared/models/iAM/network';
import { any, clone, isNil } from 'ramda';
import { getUserName } from '@/shared/utils/get-user-info';
import {
    InputValidationRules,
    rules as validationRules,
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
import { queuedWorkStatusUpdate } from '@/shared/models/iAM/queuedWorkStatusUpdate';
import { ImportExportCommittedProjectsDialogResult } from '@/shared/models/modals/import-export-committed-projects-dialog-result';
import FileDownload from 'js-file-download';
import ImportExportCommittedProjectsDialog from './scenarios-dialogs/ImportExportCommittedProjectsDialog.vue';
import GhdStarSvg from '@/shared/icons/GhdStarSvg.vue';
import GhdShareSvg from '@/shared/icons/GhdShareSvg.vue';
import GhdQueueSvg from '@/shared/icons/GhdQueueSvg.vue';
import { emptyPagination, Pagination } from '@/shared/models/vue/pagination';
import { PagingRequest } from '@/shared/models/iAM/paging';
import ScenarioService from '@/services/scenario.service';
import { useStore } from 'vuex'; 
import { useRouter } from 'vue-router'; 
import mitt from 'mitt';
import $vuetify from '@/plugins/vuetify';
import { onBeforeMount } from 'vue';

    let store = useStore(); 
    const $router = useRouter();     
    const $emitter = mitt()

    const stateNetworks = computed<Network[]>(() => store.state.networkModule.networks) ;
    const stateScenarios = computed<Scenario[]>(() => store.state.scenarioModule.scenarios); 

    const stateSharedScenariosPage = computed<Scenario[]>(() => store.state.scenarioModule.currentSharedScenariosPage) ;
    const stateUserScenariosPage = computed<Scenario[]>(() => store.state.scenarioModule.currentUserScenarioPage) ;

    let stateTotalSharedScenarios = computed<number>(() => store.state.scenarioModule.totalSharedScenarios) ;
    let stateTotalUserScenarios = computed<number>(() => store.state.scenarioModule.totalUserScenarios) ;

    const stateWorkQueuePage = computed<QueuedWork[]>(() => store.state.scenarioModule.currentWorkQueuePage) ;
    let stateTotalQueuedSimulations = computed<number>(() => store.state.scenarioModule.totalQueuedSimulations) ;
    const stateFastWorkQueuePage = computed<QueuedWork[]>(() => store.state.scenarioModule.currentFastWorkQueuePage);
    let stateTotalFastQueuedItems = computed<number>(() => store.state.scenarioModule.totalFastQueuedItems);

    let authenticated:boolean = (store.state.authenticationModule.authenticated);
    let userId: string = (store.state.authenticationModule.userId);
    let hasAdminAccess: boolean = (store.state.authenticationModule.hasAdminAccess) ; 
    let hasSimulationAccess:boolean = (store.state.authenticationModule.hasSimulationAccess) ; 

    async function addSuccessNotificationAction(payload?: any): Promise<any>{await store.dispatch('addSuccessNotification')}
    async function addWarningNotificationAction(payload?: any): Promise<any>{await store.dispatch('addWarningNotification')}
    async function addErrorNotificationAction(payload?: any): Promise<any>{await store.dispatch('addErrorNotification')}
    async function addInfoNotificationAction(payload?: any): Promise<any>{await store.dispatch('addInfoNotification')}
    async function getScenariosAction(payload?: any): Promise<any>{await store.dispatch('getScenarios')}
    async function getSharedScenariosPageAction(payload?: any): Promise<any>{await store.dispatch('getSharedScenariosPage', payload)}
    async function createScenarioAction(payload?: any): Promise<any>{await store.dispatch('createScenario', payload)}
    async function cloneScenarioAction(payload?: any): Promise<any>{await store.dispatch('cloneScenario', payload)}

    async function updateScenarioAction(payload?: any): Promise<any>{await store.dispatch('updateScenario', payload)}
    async function deleteScenarioAction(payload?: any): Promise<any>{await store.dispatch('deleteScenario', payload)}
    async function cancelWorkQueueItemAction(payload?: any): Promise<any>{await store.dispatch('cancelWorkQueueItem')}
    async function cancelFastQueueItemAction(payload?: any): Promise<any>{await store.dispatch('cancelFastQueueItem')}
    async function runSimulationAction(payload?: any): Promise<any>{await store.dispatch('runSimulation')}

    async function migrateLegacySimulationDataAction(payload?: any): Promise<any>{await store.dispatch('migrateLegacySimulationData')}
    async function updateSimulationAnalysisDetailAction(payload?: any): Promise<any>{await store.dispatch('updateSimulationAnalysisDetail')}
    async function updateSimulationReportDetailAction(payload?: any): Promise<any>{await store.dispatch('updateSimulationReportDetail')}
    async function updateNetworkRollupDetailAction(payload?: any): Promise<any>{await store.dispatch('updateNetworkRollupDetail')}

    async function selectScenarioAction(payload?: any): Promise<any>{await store.dispatch('selectScenario', payload)} 
    async function upsertBenefitQuantifierAction(payload?: any): Promise<any>{await store.dispatch('upsertBenefitQuantifier')} 
    async function aggregateNetworkDataAction(payload?: any): Promise<any>{await store.dispatch('aggregateNetworkData')} 
    async function getUserScenariosPageAction(payload?: any): Promise<any>{await store.dispatch('getUserScenariosPage', payload)}

    async function updateQueuedWorkStatusAction(payload?: any): Promise<any>{await store.dispatch('updateQueuedWorkStatus', payload)} 
    async function getWorkQueuePageAction(payload?: any): Promise<any>{await store.dispatch('getWorkQueuePage', payload)} 
    async function getFastWorkQueuePageAction(payload?: any): Promise<any>{await store.dispatch('getFastWorkQueuePage', payload)} 
    async function updateFastQueuedWorkStatusAction(payload?: any): Promise<any>{await store.dispatch('updateFastQueuedWorkStatus', payload)} 
    
    let networks: Network[] = [];
    let scenarioGridHeaders: any = [
        {
            title: 'Scenario',
            key: 'name',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Creator',
            key: 'creator',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Owner',
            key: 'owner',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Network',
            key: 'network',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Date Created',
            key: 'createdDate',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Date Last Modified',
            key: 'lastModifiedDate',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Date Last Run',
            key: 'lastRun',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Status',
            key: 'status',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Run Time',
            key: 'runTime',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Report Status',
            key: 'reportStatus',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Action',
            key: 'actions',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            title: '',
            key: '',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
    ];
    let workQueueGridHeaders: DataTableHeader[] = [
        {
            text: 'Queue Position',
            value: 'queuePosition',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },        
        {
            text: 'Name',
            value: 'name',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            text: 'Description',
            value: 'workDescription',
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

    let actionItems: ScenarioActions[] = [];
    let actionItemsForSharedScenario: ScenarioActions[] = [];
    let actionItemsForWorkQueue: ScenarioActions[] = [];
    let actionItemsForFastWorkQueue: ScenarioActions[] = [];
    const tabItems: TabItems[] = reactive([]);
    let tab = ref('');
    let availableActions: any;
    let availableSimulationActions: any;
    let nameUpdate: string = '';

    let scenarios: Scenario[] = [];

    let userScenarios: Scenario[] = [];
    let currentUserScenariosPage: Scenario[] = []
    const userScenariosPagination: Pagination = shallowReactive(clone(emptyPagination));

    let totalUserScenarios: ShallowRef<number> = shallowRef(0);

    let sharedScenarios: Scenario[] = [];
    let currentSharedScenariosPage: Scenario[] = [];
    const sharedScenariosPagination: Pagination = shallowReactive(clone(emptyPagination));    
    let totalSharedScenarios = ref<number>(0);
    let initializing: boolean = true;
    
    let searchMine: string = '';
    let currentSearchMine: string = ''
    let sortedMineCategory: string = '';
    let sortedMineValue: string = '';
    let sortedCategory: string = '';
    let sortedValue: string = '';
    let searchShared: string = '';
    let currentSearchShared: string = '';
    //confirmRollupAlertData: AlertData = clone(emptyAlertData);
    //showCreateNetworkDialog: boolean = false;
    let reportsDownloaderDialogData: ReportsDownloaderDialogData = clone(emptyReportsDownloadDialogData,);

    let confirmAnalysisRunAlertData: AlertData = clone(emptyAlertData);
    let shareScenarioDialogData: ShareScenarioDialogData = clone(emptyShareScenarioDialogData,);
    
    let ConfirmConvertJsonToRelationalData: AlertData = clone(emptyAlertData);
    let confirmCloneScenarioAlertData: AlertData = clone(emptyAlertData);
    let cloneScenarioDialogData: CloneScenarioDialogData = clone(emptyCloneScenarioDialogData);
    let confirmDeleteAlertData: AlertData = clone(emptyAlertData);
    let confirmCancelAlertData: AlertData = clone(emptyAlertData);
    let showCreateScenarioDialog: boolean = false;
    let selectedScenario: Scenario = clone(emptyScenario);   
    let networkDataAssignmentStatus: string = '';
    let rules: InputValidationRules = validationRules;
    let showMigrateLegacySimulationDialog: boolean = false;
    let showImportExportCommittedProjectsDialog: boolean = false;
    let alertDataForDeletingCommittedProjects: AlertData = { ...emptyAlertData };
    let selectedScenarioId: string = "";
    
    let currentWorkQueuePage: QueuedWork[] = [];
    const workQueuePagination: Pagination = shallowReactive(clone(emptyPagination));
    let totalQueuedSimulations: ShallowRef<number> = shallowRef(0);
    let initializingWorkQueue: boolean = true;
    let selectedQueuedWork: QueuedWork = clone(emptyQueuedWork);

    let currentFastWorkQueuePage: QueuedWork[] = [];
    const fastWorkQueuePagination: Pagination = shallowReactive(clone(emptyPagination));
    let totalFastQueuedItems: ShallowRef<number> = shallowRef(0);
    let initializingFastWorkQueue: boolean = true;
    let selectedFastQueuedWork: QueuedWork = clone(emptyQueuedWork);

    let aggragateDialogData: any = { showDialog: false };

    watch(stateNetworks, onstateNetworksChanged) 
    function onstateNetworksChanged() {
        networks = clone(stateNetworks.value);
        if (hasValue(networks)) {
            initializeScenarioPages()
        }
    }
 
    watch(stateScenarios, onstateScenariosChanged) 
    function onstateScenariosChanged() {
        scenarios = clone(stateScenarios.value);
    }

    watch(stateSharedScenariosPage, onstateSharedScenariosPageChanged) 
    function onstateSharedScenariosPageChanged(){
        currentSharedScenariosPage = clone(stateSharedScenariosPage.value);
    }

    watch(stateTotalSharedScenarios, onStateTotalSharedScenariosChanged) 
    function onStateTotalSharedScenariosChanged(){
        totalSharedScenarios.value = stateTotalSharedScenarios.value;
    }
    watch(totalSharedScenarios, onTotalSharedScenariosChanged) 
    function onTotalSharedScenariosChanged(){
        setTabTotals();
    }

    watch(stateUserScenariosPage, onStateUserScenariosPageChanged) 
    function onStateUserScenariosPageChanged(){
        currentUserScenariosPage = clone(stateUserScenariosPage.value);
    }

    watch(stateTotalUserScenarios, onStateTotalUserScenariosChanged) 
    function onStateTotalUserScenariosChanged(){
        totalUserScenarios.value = stateTotalUserScenarios.value;
    }
    watch(totalUserScenarios, onTotalUserScenariosChanged) 
    function onTotalUserScenariosChanged(){
        setTabTotals();
    }
    
    watch(stateWorkQueuePage, onStateWorkQueuePageChanged) 
    function onStateWorkQueuePageChanged(){
        currentWorkQueuePage = clone(stateWorkQueuePage.value);
    }

    watch(stateTotalQueuedSimulations, onStateTotalQueuedSimulations) 
    function onStateTotalQueuedSimulations(){
        totalQueuedSimulations.value = stateTotalQueuedSimulations.value;
    }
    watch(totalQueuedSimulations, onTotalQueuedSimulationsChanged) 
    function onTotalQueuedSimulationsChanged(){
        setTabTotals();
    }

    watch(stateFastWorkQueuePage, onStateFastWorkQueuePageChanged) 
    function onStateFastWorkQueuePageChanged(){
        currentFastWorkQueuePage = clone(stateFastWorkQueuePage.value);
    }

    watch(stateTotalFastQueuedItems, onStateTotalFastQueuedItemsChanged) 
    function onStateTotalFastQueuedItemsChanged(){
        totalFastQueuedItems.value = stateTotalFastQueuedItems.value;
    }
  
    watch(totalFastQueuedItems, onTotalFastQueuedItemsChanged) 
    function onTotalFastQueuedItemsChanged(){
        setTabTotals();
    }

    watch(userScenariosPagination, onUserScenariosPagination) 
    function onUserScenariosPagination() {
        if(initializing)
            return;
        const { sortBy, descending, page, rowsPerPage } = userScenariosPagination;

        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: currentSearchMine
        };
        if(hasValue(networks) )
            getUserScenariosPageAction(request); 
    }

    watch(sharedScenariosPagination, onSharedScenariosPagination) 
    function onSharedScenariosPagination() {
        if(initializing)
            return;
        const { sortBy, descending, page, rowsPerPage } = sharedScenariosPagination;

        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: currentSearchShared
        };
        if(hasValue(networks) )
            getSharedScenariosPageAction(request); 
    }

    // Refresh both lists and counts(gets called when clone, delete, create operations are performed)
    function onScenariosPagination() {
        if(initializing)
            return;

        const { sortBy, descending, page, rowsPerPage } = userScenariosPagination;
        const request: PagingRequest<Scenario>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: currentSearchMine
        };

        if(hasValue(networks))
            getUserScenariosPageAction(request).then(() => {
                onSharedScenariosPagination();
            });
    }

    watch(workQueuePagination, onWorkQueuePagination) 
    function onWorkQueuePagination() {
        doWorkQueuePagination();
    }

    watch(fastWorkQueuePagination, onFastWorkQueuePagination) 
    function onFastWorkQueuePagination() {
        doFastQueuePagination();
    }

    function doWorkQueuePagination() {
        if(initializingWorkQueue)
            return;
        const { sortBy, descending, page, rowsPerPage } = workQueuePagination;

        const workQueueRequest: PagingRequest<QueuedWork>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: ""
        };
        getWorkQueuePageAction(workQueueRequest);    
    }

    function doFastQueuePagination() {
        if(initializingWorkQueue)
            return;
        const { sortBy, descending, page, rowsPerPage } = fastWorkQueuePagination;

        const workQueueRequest: PagingRequest<QueuedWork>= {
            page: page,
            rowsPerPage: rowsPerPage,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: sortBy != null ? sortBy : '',
            isDescending: descending != null ? descending : false,
            search: ""
        };
        getFastWorkQueuePageAction(workQueueRequest);    
    }

    onBeforeMount(() => {
        networks = clone(stateNetworks.value);
        if (hasValue(networks) ) {
            initializeScenarioPages();
        } 
        
        $emitter.on(
            Hub.BroadcastEventType.BroadcastDataMigrationEvent,
            getDataMigrationStatus,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent,
            getScenarioAnalysisDetailUpdate,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastWorkQueueUpdateEvent,
            updateWorkQueue,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastWorkQueueStatusUpdateEvent,
            getWorkQueueUpdate,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastFastWorkQueueUpdateEvent,
            updateFastWorkQueue,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastFastWorkQueueStatusUpdateEvent,
            getFastWorkQueueUpdate,
        );

        $emitter.on(
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            getReportStatus,
        );

        availableActions = {
            runAnalysis: 'runAnalysis',
            reports: 'reports',
            settings: 'settings',
            share: 'share',
            clone: 'clone',
            delete: 'delete',
            commitedProjects: 'commitedProjects',
            convert:'convert'
        };
        availableSimulationActions = {
            cancel: 'cancel',
            fastCancel: 'fastCancel'
        }
        actionItemsForSharedScenario = [
            {
                title: 'Run Analysis',
                action: availableActions.runAnalysis,
                icon: require("@/assets/icons/monitor.svg"),
                isCustomIcon: true
            },
            {
                title: 'Reports',
                action: availableActions.reports,
                icon: require("@/assets/icons/clipboard.svg"),
                isCustomIcon: true
            },
            {
                title: 'Settings',
                action: availableActions.settings,
                icon: require("@/assets/icons/gear.svg"),
                isCustomIcon: true
            },
            {
                title: 'Committed Projects',
                action: availableActions.commitedProjects,
                icon: require("@/assets/icons/committed-projects.svg"),
                isCustomIcon: true
            },
            {
                title: 'Convert Output from Json to Relational',
                action: availableActions.convert,
                icon: "fas fa-exchange-alt",
                isCustomIcon: false
            },
            {
                title: 'Clone',
                action: availableActions.clone,
                icon: require("@/assets/icons/copy.svg"),
                isCustomIcon: true
            },
            {
                title: 'Delete',
                action: availableActions.delete,
                icon: require("@/assets/icons/trash.svg"),
                isCustomIcon: true
            }           
        ];
        actionItemsForWorkQueue = [
             {
                title: 'Cancel Work',
                action: availableSimulationActions.cancel,
                icon: require("@/assets/icons/x-circle.svg"),
                isCustomIcon: true
            }             
        ];
        actionItemsForFastWorkQueue = [
             {
                title: 'Cancel Work',
                action: availableSimulationActions.fastCancel,
                icon: require("@/assets/icons/x-circle.svg"),
                isCustomIcon: true
            }             
        ];
        actionItems = actionItemsForSharedScenario.slice();
        actionItems.splice(4, 0, {
            title: 'Share',
            action: availableActions.share,
            icon: require("@/assets/icons/share-geometric.svg"),
                isCustomIcon: true
        });
        tabItems.push(
            { name: 'My scenarios', icon: require("@/assets/icons/star-empty.svg"), count: totalUserScenarios.value },
            { name: 'Shared with me', icon: require("@/assets/icons/share-empty.svg"), count: totalSharedScenarios.value },
            { name: 'General work queue', icon: require("@/assets/icons/queue.svg"), count: totalQueuedSimulations.value },
        );
        tab.value = 'My scenarios';
    });

    onBeforeUnmount(()=> {
        $emitter.off(
            Hub.BroadcastEventType.BroadcastDataMigrationEvent,
            getDataMigrationStatus,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastSimulationAnalysisDetailEvent,
            getScenarioAnalysisDetailUpdate,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastWorkQueueUpdateEvent,
            updateWorkQueue,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastWorkQueueStatusUpdateEvent,
            getWorkQueueUpdate,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastFastWorkQueueUpdateEvent,
            updateFastWorkQueue,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastFastWorkQueueStatusUpdateEvent,
            getFastWorkQueueUpdate,
        );
        $emitter.off(
            Hub.BroadcastEventType.BroadcastReportGenerationStatusEvent,
            getReportStatus,
        );
    }); 

    function initializeScenarioPages(){
        const { sortBy, descending, page, rowsPerPage } = sharedScenariosPagination;

        const request: PagingRequest<Scenario> = {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false,
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };
        const workQueueRequest: PagingRequest<QueuedWork> = {
            page: 1,
            rowsPerPage: 5,
            syncModel: {
                libraryId: null,
                updateRows: [],
                rowsForDeletion: [],
                addedRows: [],
                isModified: false
            },           
            sortColumn: '',
            isDescending: false,
            search: ''
        };        
        getSharedScenariosPageAction(request).then(() =>
        getUserScenariosPageAction(request).then(() =>
        getWorkQueuePageAction(workQueueRequest).then(() => 
        getFastWorkQueuePageAction(workQueueRequest).then(() => {
            initializing = false;
            initializingWorkQueue = false;
            initializingFastWorkQueue = false;
            totalUserScenarios.value = stateTotalUserScenarios.value;
            totalSharedScenarios.value = stateTotalSharedScenarios.value;
            totalQueuedSimulations.value = stateTotalQueuedSimulations.value;
            totalFastQueuedItems.value = stateTotalFastQueuedItems.value;
            currentUserScenariosPage = clone(stateUserScenariosPage.value);
            currentSharedScenariosPage = clone(stateSharedScenariosPage.value);
            currentWorkQueuePage = clone(stateWorkQueuePage.value);
            currentFastWorkQueuePage = clone(stateFastWorkQueuePage.value);
        })))); 
    }

    function formatDate(dateToFormat: Date) {
        return hasValue(dateToFormat)
            ? moment(dateToFormat).format('M/D/YYYY')
            : null;
    }

    function formatDateWithTime(dateToFormat: Date) {
    return hasValue(dateToFormat)
        ? moment(dateToFormat).format('M/D/YYYY hh:mm:ss')
        : null;
    }

    function canModifySharedScenario(scenarioUsers: ScenarioUser[]) {
        const currentUser: string = getUserName();
        const scenarioUserCanModify = (user: ScenarioUser) =>
            user.username === currentUser && user.canModify;
        return (
            hasAdminAccess ||
            hasSimulationAccess ||
            any(scenarioUserCanModify, scenarioUsers)
        );
    }

    // TODO: update to send no payload when API is modified to migrate ALL simulations
    function onStartDataMigration() {
        // the legacy scenario id is hardcoded to our test scenario "JML Run District 8"
        migrateLegacySimulationDataAction({
            simulationId: import.meta.env.VITE_APP_HARDCODED_SCENARIOID_FROM_LEGACY,
        }).then(() => initializeScenarioPages());
    }

    function onEditScenarioName(scenario: Scenario, name: string) {
        scenario.name = name;
        if (hasValue(scenario.name)) {
            updateScenarioAction({ scenario: scenario }).then(() => {
                if(tab.value == '0')
                    onUserScenariosPagination();
                else
                    onSharedScenariosPagination();
            });
        } else {
            scenarios = [];
            setTimeout(() => (scenarios = clone(stateScenarios.value)));
        }
    }

    function prepareForNameEdit(name: string) {
        nameUpdate = name;
    }

    function onShowConfirmAnalysisRunAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        confirmAnalysisRunAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Only one simulation can be run at a time. The model run you are about to queue will be ' +
                'executed in the order in which it was received.',
        };
    }

    function onConfirmAnalysisRunAlertSubmit(submit: boolean) {
        confirmAnalysisRunAlertData = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            runSimulationAction({
                networkId: selectedScenario.networkId,
                scenarioId: selectedScenario.id,
            }).then(() => (selectedScenario = clone(emptyScenario)));
        }
    }

    function onShowConfirmConvertJsonToRelationalAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        ConfirmConvertJsonToRelationalData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Converting the simulation output from json to a relational format can be a lengthy process.',
        };
    }

    function onConfirmConvertJsonToRelationalAlertSubmit(submit: boolean) {
        ConfirmConvertJsonToRelationalData = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            ScenarioService.ConvertSimulationOutputToRelational(selectedScenario.id);
        }
    }

    function onShowReportsDownloaderDialog(scenario: Scenario) {
        reportsDownloaderDialogData = {
            showModal: true,
            scenarioId: scenario.id,
            networkId: scenario.networkId,
            name: scenario.name,
        };
    }

    function onNavigateToEditScenarioView(localScenario: Scenario) {
        selectScenarioAction({ scenarioId: localScenario.id });

        $router.push({
            path: '/EditScenario/',
            query: {
                scenarioId: localScenario.id,
                networkId: localScenario.networkId,
                scenarioName: localScenario.name,
                networkName: localScenario.networkName,
            },
        });
    }

    function onNavigateToCommittedProjectView(localScenario: Scenario) {
        selectScenarioAction({ scenarioId: localScenario.id });

        $router.push({
            path: '/CommittedProjectsEditor/Scenario/',
            query: {
                scenarioId: localScenario.id,
                networkId: localScenario.networkId,
                scenarioName: localScenario.name,
                networkName: localScenario.networkName,
            },
        });
    }

    function onShowShareScenarioDialog(scenario: Scenario) {
        shareScenarioDialogData = {
            showDialog: true,
            scenario: clone(scenario),
        };
    }

    function onShareScenarioDialogSubmit(scenarioUsers: ScenarioUser[]) {
        const scenario: Scenario = {
            ...shareScenarioDialogData.scenario,
            users: [],
        };

        shareScenarioDialogData = clone(emptyShareScenarioDialogData);

        if (!isNil(scenarioUsers) && scenario.id !== getBlankGuid()) {
            updateScenarioAction({
                scenario: { ...scenario, users: scenarioUsers },
            });
        }
    }

    function onShowConfirmCloneScenarioAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        confirmCloneScenarioAlertData = {
            showDialog: true,
            heading: 'Clone Scenario',
            choice: true,
            message: 'Are you sure you want clone this scenario?',
        };
    }

    function onShowCloneScenarioDialog(scenario: Scenario) {
        selectedScenario = clone(scenario);

        cloneScenarioDialogData = {
            showDialog: true,
            scenario: selectedScenario
        };
    }

    function onConfirmCloneScenarioAlertSubmit(submit: boolean) {
        confirmCloneScenarioAlertData = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            cloneScenarioAction({
                scenarioId: selectedScenario.id,
            }).then(() => {
                selectedScenario = clone(emptyScenario)
                if(tab.value == '0')
                    onUserScenariosPagination();
                else
                    onSharedScenariosPagination();
            });
        }
    }

    function onCloneScenarioDialogSubmit(scenario: Scenario) {
        cloneScenarioDialogData = clone(emptyCloneScenarioDialogData);

        if (!isNil(scenario)) {
            cloneScenarioAction({
                scenarioId: scenario.id,
                networkId: scenario.networkId,
                scenarioName: scenario.name
            }).then(() => {
                selectedScenario = clone(emptyScenario)
                onScenariosPagination();
            });
        }
    }

    function onShowConfirmDeleteAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        confirmDeleteAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }


    function onShowConfirmCancelAlert(simulation: QueuedWork) {
        selectedQueuedWork = clone(simulation);

        confirmCancelAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to cancel this process?',
        };
    }

    function onShowConfirmFastCancelAlert(simulation: QueuedWork) {
        selectedFastQueuedWork = clone(simulation);

        confirmCancelAlertData = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to cancel this process?',
        };
    }

    function onConfirmDeleteAlertSubmit(submit: boolean) {
        confirmDeleteAlertData = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            deleteScenarioAction({
                scenarioId: selectedScenario.id,
                scenarioName: selectedScenario.name,
            }).then(async () => {
                selectedScenario = clone(emptyScenario); 
            });
        }
    }

    function onConfirmCancelAlertSubmit(submit: boolean) {
        confirmCancelAlertData = clone(emptyAlertData);

        if (submit && selectedQueuedWork.id !== getBlankGuid() && selectedQueuedWork.id.trim() != '') {
            cancelWorkQueueItemAction({
                simulationId: selectedQueuedWork.id,
            }).then(() => {
                selectedQueuedWork = clone(emptyQueuedWork);
            });
        }
        else if(submit && selectedFastQueuedWork.id !== getBlankGuid() && selectedFastQueuedWork.id.trim() != '') {
            cancelFastQueueItemAction(selectedFastQueuedWork.id).then(() => {
                selectedFastQueuedWork = clone(emptyQueuedWork);
            });
        }
    }

    function getDataMigrationStatus(data: any) {
        const status: any = data.status;
        if (status.indexOf('Error') !== -1) {
            addErrorNotificationAction({
                message: 'Data migration error.',
                longMessage: data.status,
            });
        } else {
            addInfoNotificationAction({
                message: 'Data migration info.',
                longMessage: data.status,
            });
        }
    }

    function delay(ms: number) {
        return new Promise( resolve => setTimeout(resolve, ms) );
    }

    function getScenarioAnalysisDetailUpdate(data: any) {
        updateSimulationAnalysisDetailAction({
            simulationAnalysisDetail: data.simulationAnalysisDetail,
        });
        const updatedQueueItem: queuedWorkStatusUpdate = {
            id: data.simulationAnalysisDetail.simulationId as string + WorkType[WorkType.SimulationAnalysis] ,
            status: data.simulationAnalysisDetail.status
        }
        updateQueuedWorkStatusAction({
            workQueueStatusUpdate: updatedQueueItem
        })                            
    }

    function getWorkQueueUpdate(data: any) {
            var updatedQueueItem = data.queueItem as queuedWorkStatusUpdate
            if(isNil(updatedQueueItem))
                return;
            var queueItem = stateWorkQueuePage.value.find(_ => _.id === updatedQueueItem.id)
            if(!isNil(queueItem)){
                updateQueuedWorkStatusAction({
                    workQueueStatusUpdate: updatedQueueItem
                })
            }                                
    }

    function updateWorkQueue(data: any) {
        (async () => { 
            await delay(1000);
                doWorkQueuePagination();
            })();
    }

    function getFastWorkQueueUpdate(data: any) {
            var updatedQueueItem = data.queueItem as queuedWorkStatusUpdate
            if(isNil(updatedQueueItem))
                return;
            var queueItem = stateFastWorkQueuePage.value.find(_ => _.id === updatedQueueItem.id)
            if(!isNil(queueItem)){
                updateFastQueuedWorkStatusAction({
                    workQueueStatusUpdate: updatedQueueItem
                })
            }                                
    }

    function updateFastWorkQueue(data: any) {
        (async () => { 
            await delay(1000);
                doFastQueuePagination();
            })();
    }

    function getReportStatus(data: any) {
        updateSimulationReportDetailAction({
            simulationReportDetail: data.simulationReportDetail,
        });
    }

    function onCreateScenarioDialogSubmit(scenario: Scenario) {
        showCreateScenarioDialog = false;

        if (!isNil(scenario)) {
            createScenarioAction({
                scenario: scenario,
                networkId: scenario.networkId,
            }).then(() => {
                onScenariosPagination();
            });
        }
    }

    function onMigrateLegacySimulationSubmit(legacySimulationId: number) {
        showMigrateLegacySimulationDialog = false;

        if (!isNil(legacySimulationId)) {
            migrateLegacySimulationDataAction({
                simulationId: legacySimulationId,
                networkId: 'D7B54881-DD44-4F93-8250-3D4A630A4D3B',
            }).then(() => initializeScenarioPages());
        }
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
            CommittedProjectsService.deleteSpecificCommittedProjects(
                [selectedScenarioId],
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

    function OnActionTaken(
        action: string,
        scenarioUsers: ScenarioUser[],
        scenario: Scenario,
        isOwner: boolean,
    ) {
        switch (action) {
            case availableActions.runAnalysis:
                if (canModifySharedScenario(scenarioUsers) || isOwner) {
                    onShowConfirmAnalysisRunAlert(scenario);
                } else {
                }
                break;
            case availableActions.reports:
                onShowReportsDownloaderDialog(scenario);
                break;
            case availableActions.settings:
                if (canModifySharedScenario(scenarioUsers) || isOwner) {
                    onNavigateToEditScenarioView(scenario);
                }
                break;
            case availableActions.share:
                onShowShareScenarioDialog(scenario);
                break;
            case availableActions.clone:
                onShowCloneScenarioDialog(scenario);
                break;
            case availableActions.delete:
                onShowConfirmDeleteAlert(scenario);
                break;
            case availableActions.commitedProjects:
                if (canModifySharedScenario(scenarioUsers) || isOwner) {
                    onNavigateToCommittedProjectView(scenario);
                }
                break;
            case availableActions.convert:
                onShowConfirmConvertJsonToRelationalAlert(scenario);
        }
    }

    function OnWorkQueueActionTaken(
        action: string,
        simulation: QueuedWork,
    ) {
        switch (action) {
            case availableSimulationActions.cancel:
                onShowConfirmCancelAlert(simulation);
                break;
            case availableSimulationActions.fastCancel:
                onShowConfirmFastCancelAlert(simulation);
                break;
        }
    }


    function onShowAggregatePopup() {
        aggragateDialogData = {
            showDialog: true,
        };
    }

    function onMineSearchClick(){
        currentSearchMine = searchMine;
        resetPageMine()
    }

    function onMineClearClick(){
        searchMine = '';
        onMineSearchClick();
    }
    function onMineFilterClearClick(){
        sortedMineCategory = '';
        sortedMineValue = '';
        currentSearchMine= '';
        resetPageMine()
    }

    function resetPageMine(){
        userScenariosPagination.page = 1;
        onUserScenariosPagination();
    }

    function onSharedSearchClick(){
        currentSearchShared = searchShared;
        resetPageShared()
    }
    function onSharedFilterClearClick(){
        sortedCategory = '';
        sortedValue = '';
        currentSearchShared = '';
        resetPageShared()
    }

    function onSharedClearClick(){
        searchShared = '';
        onSharedSearchClick();
    }

    function resetPageShared(){
        sharedScenariosPagination.page = 1;
        onSharedScenariosPagination();
    }

    function hasSharedSearch(): boolean{
        return currentSearchShared.trim() !== '';
    }

    function hasMineSearch(): boolean{
        return currentSearchMine.trim() !== '';
    }

    function setTabTotals(){
        tabItems.forEach(tab => {
            if(tab.name === 'Shared with me')
                tab.count = totalSharedScenarios.value;
            else if (tab.name === 'My scenarios')
                tab.count = totalUserScenarios.value;
            else
                tab.count = totalQueuedSimulations.value + totalFastQueuedItems.value;
        })
    }

    function getEmptyWorkQueueMessage()
    {
        if (totalSharedScenarios.value == 0 &&
            totalUserScenarios.value == 0 &&
            totalQueuedSimulations.value == 0 &&
            totalFastQueuedItems.value == 0){
            
            return "Retrieving data..."
        }
        else {
            return "No queued work"
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
.theme--light.v-datatable thead th.column.sortable.active {
    color: rgba(0,0,0,0.87);
    border-top:2px solid rgba(0,0,0,0.54);
    border-left:2px solid rgba(0,0,0,0.54);
    border-right:2px solid rgba(0,0,0,0.54);
}

</style>
