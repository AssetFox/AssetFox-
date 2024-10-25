<template>
    <v-row column>
        <v-col cols = "12">
            <v-card elevation="5" >
                <v-tabs center-active v-model="tab">
                    <v-tab
                        v-for="item in tabItems"
                        :key="item.name"
                        :value="item.name"
                        class="tab-theme"
                        bg-color="primary"
                    >
                        <GhdQueueSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'General work queue'"/> 
                        <GhdShareSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'Shared with me'"/>  
                        <GhdStarSvg style="padding-right:10px"  class="icon-selected-tab" v-if="item.name === 'My scenarios'"/>  
                        {{ item.name }} ( {{ item.count }} )
                    </v-tab>
                    <v-spacer></v-spacer>
                    <v-col cols = "1"></v-col>
                </v-tabs>
                <v-window v-model="tab" style="background-color: #d9e7f2;">
                    <v-window-item value="My scenarios">
                         <v-col cols = "12">
                            <v-card elevation="5">
                                <v-card-title>
                                        <v-row style = "margin-right: -100px;">
                                            <v-col>
                                                <v-row style="margin: 0;">
                                                    <v-text-field
                                                id="Scenarios-searchScenarios-textField"
                                                type="text"
                                                placeholder="Search in scenarios"
                                                hide-details
                                                single-line
                                                v-model="searchMine"
                                                prepend-inner-icon=custom:GhdSearchSvg
                                                variant="outlined"
                                                density="compact"
                                                clearable
                                                @click:clear="onMineClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn id="Scenarios-performSearch-button" 
                                                style="margin-top: 2px;margin-left: 5px; margin-right: 5px;" 
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="onMineSearchClick()">
                                                Search
                                            </v-btn>
                                            <v-btn id="Scenarios-performFilter-button" 
                                                style="margin-top: 2px; margin-right: 5px;" 
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
                                                        
                                                        <img class='img-general' :src="getUrl('assets/icons/x-circle.svg')" @click="onMineFilterClearClick()"  >
                                                </v-chip>
                                                </div>
                                            </span>
                                                </v-row>
                                                
                                            </v-col>
                                            
                                            <v-spacer></v-spacer>
                                            <v-col class="d-flex justify-end" style="padding-right: 100px;">
                                                <v-btn
                                                    id="Scenarios-createScenario-btn"
                                                    @click="
                                                        showCreateScenarioDialog = true
                                                    "
                                                    class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                    variant="outlined"
                                                    style="justify-content: end;"
                                                >
                                                    Create new scenario
                                                </v-btn>
                                            </v-col>
                                            
                                        </v-row>

                                    <v-row class="flex-end xs2" style="justify-content: end; padding-right: 70px">
                                        
                                    </v-row>
                                </v-card-title>
                                <v-data-table-server
                                    :items="currentUserScenariosPage"                      
                                    :items-length="totalUserScenarios"
                                    :pagination.sync="userScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                                    v-model:sort-by="userScenariosPagination.sort"
                                    v-model:page="userScenariosPagination.page"
                                    v-model:items-per-page="userScenariosPagination.rowsPerPage"
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg"
                                    item-value="name"
                                    :hover="true"
                                    @update:options="onUserScenariosPagination"
                                >
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <tr>
                                        <td>
                                        
                                            <editScenarioNameDialog
                                                size="large"
                                                lazy
                                                persistent
                                                v-model:return-value="nameUpdate"
                                                :initial-name="nameUpdate"
                                                @save="
                                                    onEditScenarioName(
                                                        props.item,
                                                        nameUpdate,
                                                    )
                                                "
                                                @open="prepareForNameEdit( props.item.name,)"
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
                                                            rules[
                                                                'generalRules'
                                                            ].valueContainsNoSpecialCharacters,
                                                        ]"
                                                    />
                                                </template>
                                            </editScenarioNameDialog>
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
                                        <td>{{ props.item.runTime }}</td>
                                        <td>
                                            <v-menu  location="left">
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"

                                                >
                                                    <v-btn
                                                        id="Scenarios-actionMenu-vbtn"
                                                        color="text-green darken-1"
                                                        flat
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="getUrl('assets/icons/more-vertical.svg')"/>
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
                                                        class="menu-style"
                                                        :disabled="item.title === 'Reports' && !checkIfReportExists(props.item.id)">
                                                        <v-list-item-title icon>
                                                            <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                            <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon> 
                                                            {{item.title}}
                                                        </v-list-item-title>
                                                    </v-list-item>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                    </tr>
                                    </template>
                                    <!-- <v-alert
                                        :model-value="hasMineSearch()"
                                        class="assetFox-orange-bg"
                                        icon="fas fa-exclamation"
                                        slot="no-data"
                                    >
                                        Your search for "{{ currentSearchMine }}" found
                                        no results.
                                    </v-alert> -->
                                </v-data-table-server>
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
                                                density="compact"
                                                variant="outlined"
                                                hide-details
                                                single-line
                                                prepend-inner-icon=custom:GhdSearchSvg
                                                v-model="searchShared"
                                                clearable
                                                @click:clear="onSharedClearClick()"
                                                class="ghd-text-field-border ghd-text-field search-icon-general"
                                            >
                                            </v-text-field>
                                            <v-btn style="margin-top: 2px; margin-left: 5px; margin-right: 5px;" 
                                                id="Scenarios-shared-performSearch-button"
                                                class='ghd-blue ghd-button-text ghd-outline-button-padding ghd-button' 
                                                variant = "outlined" 
                                                @click="onSharedSearchClick()">
                                                Search
                                            </v-btn>
                                            <v-btn id="Scenarios-performFilter-button" 
                                                style="margin-top: 2px; margin-left: 5px;" 
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
                                                        
                                                        <img class='img-general' :src="getUrl('assets/icons/x-circle.svg')" @click="onSharedFilterClearClick()"  >
                                                </v-chip>
                                                </div>
                                            </span>
                                        </v-row>
                                        
                                    </v-col>
                                </v-card-title>
                                <v-data-table-server
                                    :items="currentSharedScenariosPage"                      
                                    :items-length="totalSharedScenarios"
                                    :pagination.sync="sharedScenariosPagination"
                                    :headers="scenarioGridHeaders"
                                    :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                                    :hover="true"
                                    v-model:sort-by="sharedScenariosPagination.sort"
                                    v-model:page="sharedScenariosPagination.page"
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg"
                                    v-model:items-per-page="sharedScenariosPagination.rowsPerPage"
                                    item-value="name"
                                    @update:options="onSharedScenariosPagination"
                                >
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                    <tr>
                                        <td>
                                            <editScenarioNameDialog
                                                size="large"
                                                lazy
                                                persistent
                                                v-model:return-value="nameUpdate"
                                                :initial-name="props.item.name"
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
                                                            rules[
                                                                'generalRules'
                                                            ].valueContainsNoSpecialCharacters,
                                                        ]"
                                                    />
                                                </template>
                                            </editScenarioNameDialog>
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
                                        <td>{{ props.item.runTime }}</td>
                                        <td>
                                            <v-menu >
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"
                                                >
                                                    <v-btn
                                                        id="Scenarios-shared-actionMenu-vbtn"
                                                        color="text-green darken-1"
                                                        flat
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="getUrl('assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-item v-for="(item,i) in actionItemsForSharedScenario"
                                                        :key="i"
                                                        @click="OnActionTaken(item.action,props.item.users,props.item,false)"
                                                        class="menu-style"
                                                        :disabled="item.title === 'Reports' && !checkIfReportExists(props.item.id)">
                                                                <v-list-item-title icon>                                                        
                                                                <img v-if="item.isCustomIcon" style="padding-right:5px" v-bind:src="item.icon"/>
                                                                <v-icon v-else class="action-icon-padding">{{ item.icon}}</v-icon>  
                                                                {{item.title}}
                                                            </v-list-item-title>
                                                    </v-list-item>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                        </tr>
                                    </template>                                                                 
                                </v-data-table-server>
                            </v-card>
                        </v-col>
                    </v-window-item>
                    <v-window-item value="General work queue">
                        <v-col cols = "12">
                            <v-card elevation="5">
                                <v-card-title class="ghd-dialog-padding-top-title">
                                    <v-row justify-start>
                                    <div class="dialog-header"><h5>Work Queue</h5></div>
                                    </v-row>

                                </v-card-title>
                                <v-data-table-server
                                    :headers="workQueueGridHeaders"
                                    :totalItems="totalQueuedSimulations"
                                    :pagination.sync="workQueuePagination"
                                    :items="currentWorkQueuePage"              
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg"        
                                    :items-length="totalQueuedSimulations"
                                    :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                                    v-model:sort-by="workQueuePagination.sort"
                                    v-model:page="workQueuePagination.page"
                                    v-model:items-per-page="workQueuePagination.rowsPerPage"
                                    item-value="name"
                                    @update:options="onWorkQueuePagination"
                                >                           
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <tr>
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
                                            <v-menu location="left">
                                                <template
                                                    v-slot:activator="{
                                                        props
                                                    }"
                                                >
                                                    <v-btn
                                                        color="text-green darken-1"
                                                        flat
                                                        v-bind="props"
                                                    >
                                                        <img class='img-general' :src="getUrl('assets/icons/more-vertical.svg')"/>
                                                    </v-btn>
                                                </template>

                                                <v-list>
                                                    <v-list-tile v-for="(item,i) in actionItemsForWorkQueue"
                                                        :key="i"
                                                        @click="OnWorkQueueActionTaken(item.action,props.item)"
                                                        class="menu-style">
                                                        <v-list-tile-title flat>                                                        
                                                            <img style="padding-right:5px" v-bind:src="item.icon"/>
                                                            {{item.title}}
                                                        </v-list-tile-title>
                                                    </v-list-tile>
                                                </v-list>
                                            </v-menu>
                                        </td>
                                        </tr>
                                    </template>                                         
                                    <template v-slot:no-data>
                                        {{ getEmptyWorkQueueMessage() }}
                                    </template>
                                </v-data-table-server>
                                <v-card-title class="ghd-dialog-padding-top-title">
                                    <v-row justify-start>
                                    <div class="dialog-header"><h5>Fast Queue</h5></div>
                                    </v-row>

                                </v-card-title>
                                <v-data-table-server
                                    :headers="workQueueGridHeaders"
                                    :items="currentFastWorkQueuePage"
                                    :totalItems="totalFastQueuedItems"
                                    :pagination.sync="fastWorkQueuePagination"                  
                                    :items-length="totalFastQueuedItems"
                                    sort-asc-icon="custom:GhdTableSortAscSvg"
                                    sort-desc-icon="custom:GhdTableSortDescSvg"
                                    :items-per-page-options="[
                                        {value: 5, title: '5'},
                                        {value: 10, title: '10'},
                                        {value: 25, title: '25'},
                                    ]"
                                    v-model:sort-by="fastWorkQueuePagination.sort"
                                    v-model:page="fastWorkQueuePagination.page"
                                    v-model:items-per-page="fastWorkQueuePagination.rowsPerPage"
                                    item-value="name"
                                    
                                >                           
                                    <template slot="items" slot-scope="props" v-slot:item="props">
                                        <tr>
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
                                                <v-menu  left>
                                                    <template
                                                        v-slot:activator="{
                                                            props
                                                        }"
                                                    >
                                                        <v-btn
                                                            color="text-green darken-1"
                                                            flat
                                                            v-bind="props"
                                                        >
                                                            <img class='img-general' :src="getUrl('assets/icons/more-vertical.svg')"/>
                                                        </v-btn>
                                                    </template>

                                                    <v-list>
                                                        <v-list-item v-for="(item,i) in actionItemsForFastWorkQueue"
                                                            :key="i"
                                                            @click="OnWorkQueueActionTaken(item.action,props.item)"
                                                            class="menu-style">
                                                            <v-list-item-title flat>                                                        
                                                                <img style="padding-right:5px" v-bind:src="item.icon"/>
                                                                {{item.title}}
                                                            </v-list-item-title>
                                                        </v-list-item>
                                                    </v-list>
                                                </v-menu>
                                            </td>
                                        </tr>
                                    </template>                                         
                                    <template v-slot:no-data>
                                        {{ getEmptyWorkQueueMessage() }}
                                    </template>
                                </v-data-table-server>
                            </v-card>
                        </v-col>
                    </v-window-item>
                </v-window>
            </v-card>
        </v-col>

         <AlertPreChecks
            :dialogDataPreChecks="confirmAnalysisPreCheckAlertData"
            @submit="onConfirmAnalysisPreCheckAlertSubmit"
        />
       
         <AlertWithButtons
            :dialogDataWithButtons="confirmAnalysisRunAlertData"
            @submit="onConfirmAnalysisRunAlertSubmit"
            />

        <Alert
            :dialogData="ConfirmConvertJsonToRelationalData"
            @submit="onConfirmConvertJsonToRelationalAlertSubmit"
        />

        <ReportsDownloaderDialog :dialogData="reportsDownloaderDialogData" />

        <ShareScenarioDialog
            :dialogData="shareScenarioDialogData"
            @submit="onShareScenarioDialogSubmit"
        />

        <Alert
            :dialogData="confirmCloneScenarioAlertData"
            @submit="onConfirmCloneScenarioAlertSubmit"
        />

        <Alert
            :dialogData="confirmDeleteAlertData"
            @submit="onConfirmDeleteAlertSubmit"
        />
       
        <Alert
            :dialogData="confirmCancelAlertData"
            @submit="onConfirmCancelAlertSubmit"
        />

        <Alert
            :dialogData="confirmDeleteReportsData"
            @submit="onDeleteAllGeneratedReports"
        />
        
        <CreateScenarioDialog
            :showDialog="showCreateScenarioDialog"
            @submit="onCreateScenarioDialogSubmit"
        />
        <FilterScenarioList
            :showDialog="showSharedFilterScenarioList"
            @submit="onFilterSharedScenarioListSubmit"
        />
        <FilterScenarioList
            :showDialog="showFilterScenarioList"
            @submit="onFilterScenarioListSubmit"
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
        <ImportExportCommittedProjectsDialog
            :showDialog="showImportExportCommittedProjectsDialog"
            @submit="onSubmitImportExportCommittedProjectsDialogResult"
            @delete="onDeleteCommittedProjects"
        />
    </v-row>
</template>

<script lang="ts" setup>
import { getUrl } from '@/shared/utils/get-url';
import { Ref, ref, shallowReactive, shallowRef, ShallowRef, watch, onBeforeUnmount, computed, reactive, inject } from 'vue'; 
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
CloneScenario,
} from '@/shared/models/iAM/scenario';
import { hasValue } from '@/shared/utils/has-value-util';
import { AlertData, AlertDataWithButtons, AlertPreChecksData, emptyAlertData, emptyAlertDataWithButtons, emptyAlertPreChecksData } from '@/shared/models/modals/alert-data';
import Alert from '@/shared/modals/Alert.vue';
import editDialog from '@/shared/modals/Edit-Dialog.vue'
import editScenarioNameDialog from '@/shared/modals/EditDialogForScenarioName.vue'
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
import mitt, { Emitter, EventType } from 'mitt';
import $vuetify from '@/plugins/vuetify';
import { onBeforeMount } from 'vue';
import { importCompletion } from '@/shared/models/iAM/ImportCompletion';
import GhdSearchSvg from '@/shared/icons/GhdSearchSvg.vue';
import { User } from '@/shared/models/iAM/user';
import router from '@/router';
import { useRoute } from 'vue-router';
import ReportsService from '@/services/reports.service';

    let store = useStore(); 
    const $router = useRouter();     
    const $emitter = inject('emitter') as Emitter<Record<EventType, unknown>>
    
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
    let simulationRunSettingId = computed(() => store.state.scenarioModule.simulationRunSettingId);

    let authenticated:boolean = (store.state.authenticationModule.authenticated);
    let userId: string = (store.state.authenticationModule.userId);
    let hasAdminAccess: boolean = (store.state.authenticationModule.hasAdminAccess) ; 
    let hasSimulationAccess:boolean = (store.state.authenticationModule.hasSimulationAccess) ; 

    function addSuccessNotificationAction(payload?: any){ store.dispatch('addSuccessNotification', payload)}
    function addWarningNotificationAction(payload?: any){ store.dispatch('addWarningNotification', payload)}
    function addErrorNotificationAction(payload?: any){ store.dispatch('addErrorNotification', payload)}
    function addInfoNotificationAction(payload?: any){ store.dispatch('addInfoNotification', payload)}
    async function getScenariosAction(payload?: any): Promise<any>{await store.dispatch('getScenarios', payload)}
    async function getSharedScenariosPageAction(payload?: any): Promise<any>{await store.dispatch('getSharedScenariosPage', payload)}
    async function createScenarioAction(payload?: any): Promise<any>{await store.dispatch('createScenario', payload)}
    async function cloneScenarioAction(payload?: any): Promise<any>{await store.dispatch('cloneScenario', payload)}
    async function cloneScenarioWithDestinationNetworkAction(payload?:any): Promise<any>{await store.dispatch('cloneScenarioWithDestinationNetwork',payload)}
    async function getScenarioSelectableTreatmentsAction(payload?: any): Promise<any> {return await store.dispatch('getScenarioSelectableTreatments', payload)}

    async function updateScenarioAction(payload?: any): Promise<any>{await store.dispatch('updateScenario', payload)}
    async function deleteScenarioAction(payload?: any): Promise<any>{await store.dispatch('deleteScenario', payload)}
    async function cancelWorkQueueItemAction(payload?: any): Promise<any>{await store.dispatch('cancelWorkQueueItem', payload)}
    async function cancelFastQueueItemAction(payload?: any): Promise<any>{await store.dispatch('cancelFastQueueItem', payload)}
    async function runSimulationAction(payload?: any): Promise<any>{await store.dispatch('runSimulation', payload)}

    async function migrateLegacySimulationDataAction(payload?: any): Promise<any>{await store.dispatch('migrateLegacySimulationData', payload)}
    async function updateSimulationAnalysisDetailAction(payload?: any): Promise<any>{await store.dispatch('updateSimulationAnalysisDetail', payload)}
    async function updateSimulationReportDetailAction(payload?: any): Promise<any>{await store.dispatch('updateSimulationReportDetail', payload)}
    async function updateNetworkRollupDetailAction(payload?: any): Promise<any>{await store.dispatch('updateNetworkRollupDetail', payload)}

    function selectScenarioAction(payload?: any){ store.dispatch('selectScenario', payload)} 
    async function upsertBenefitQuantifierAction(payload?: any): Promise<any>{await store.dispatch('upsertBenefitQuantifier', payload)} 
    async function aggregateNetworkDataAction(payload?: any): Promise<any>{await store.dispatch('aggregateNetworkData')} 
    async function getUserScenariosPageAction(payload?: any): Promise<any>{await store.dispatch('getUserScenariosPage', payload)}

    async function updateQueuedWorkStatusAction(payload?: any): Promise<any>{await store.dispatch('updateQueuedWorkStatus', payload)} 
    async function getWorkQueuePageAction(payload?: any): Promise<any>{await store.dispatch('getWorkQueuePage', payload)} 
    async function getFastWorkQueuePageAction(payload?: any): Promise<any>{await store.dispatch('getFastWorkQueuePage', payload)} 
    async function updateFastQueuedWorkStatusAction(payload?: any): Promise<any>{await store.dispatch('updateFastQueuedWorkStatus', payload)} 
    async function getAllUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('getAllUserCriteriaFilter',payload);}
    
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
            title: 'Run Time',
            key: 'runTime',
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
    let workQueueGridHeaders: any[] = [
        {
            title: 'Queue Position',
            key: 'queuePosition',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },        
        {
            title: 'Name',
            key: 'name',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Description',
            key: 'workDescription',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Queued By',
            key: 'queueingUser',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Queued Time',
            key: 'queueEntryTimestamp',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Start Time',
            key: 'workStartedTimestamp',
            align: 'left',
            sortable: true,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Current Run Time',
            key: 'currentRunTime',
            align: 'left',
            sortable: false,
            class: 'header-border',
            width: '',
        },
        {
            title: 'Previous Run Time',
            key: 'previousRunTime',
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

    let actionItems: ScenarioActions[] = [];
    let actionItemsForSharedScenario: ScenarioActions[] = [];
    let actionItemsForWorkQueue: ScenarioActions[] = [];
    let actionItemsForFastWorkQueue: ScenarioActions[] = [];
    const tabItems: TabItems[] = reactive([]);
    let tab = ref('');
    let availableActions: any;
    let availableSimulationActions: any;
    let nameUpdate = ref('');

    let scenarios: Scenario[] = [];
    let scenarioForReportDeletion = ref<Scenario | null>(null);
    let userScenarios: Scenario[] = [];
    let currentUserScenariosPage = ref<Scenario[]>([])
    const userScenariosPagination: Pagination = shallowReactive(clone(emptyPagination));

    let totalUserScenarios: ShallowRef<number> = shallowRef(0);

    let preCheckMessages: any;
    let emptyTreatmentBudgets: any;
    let preCheckHeading: string;
    let preCheckStatus: any;

    let sharedScenarios: Scenario[] = [];
    let currentSharedScenariosPage: Ref<Scenario[]> = ref([]);
    const sharedScenariosPagination: Pagination = shallowReactive(clone(emptyPagination));    
    let totalSharedScenarios = ref<number>(0);
    let initializing: boolean = true;
    
    let searchMine = ref('');
    let currentSearchMine = ref('')
    let sortedMineCategory: string = '';
    let sortedMineValue: string = '';
    let sortedCategory: string = '';
    let sortedValue: string = '';
    let searchShared = ref('');
    let currentSearchShared: string = '';
    //confirmRollupAlertData: AlertData = clone(emptyAlertData);
    //showCreateNetworkDialog: boolean = false;
    let reportsDownloaderDialogData= ref(clone(emptyReportsDownloadDialogData));

    let confirmAnalysisRunAlertData= ref(clone(emptyAlertDataWithButtons));
    let confirmAnalysisPreCheckAlertData= ref(clone(emptyAlertPreChecksData));
    let shareScenarioDialogData = ref(clone(emptyShareScenarioDialogData));
    
    let ConfirmConvertJsonToRelationalData = ref(clone(emptyAlertData));
    let confirmCloneScenarioAlertData  = ref(clone(emptyAlertData));
    let cloneScenarioDialogData = ref(clone(emptyCloneScenarioDialogData));
    let confirmDeleteAlertData = ref(clone(emptyAlertData));
    let confirmCancelAlertData = ref(clone(emptyAlertData));
    let confirmDeleteReportsData = ref(clone(emptyAlertData));
    let showCreateScenarioDialog = ref(false);
    let selectedScenario: Scenario = clone(emptyScenario);
    let runAnalysisScenario: Scenario = clone(emptyScenario);
    let networkDataAssignmentStatus: string = '';
    let rules: InputValidationRules = validationRules;
    let showMigrateLegacySimulationDialog: boolean = false;
    let showImportExportCommittedProjectsDialog: boolean = false;
    let alertDataForDeletingCommittedProjects = ref({ ...emptyAlertData });
    let selectedScenarioId: string = "";
    
    const currentWorkQueuePage = ref<QueuedWork[]>([]);
    const workQueuePagination = ref<Pagination>(clone(emptyPagination));
    let totalQueuedSimulations: ShallowRef<number> = shallowRef(0);
    let initializingWorkQueue: boolean = true;
    let selectedQueuedWork: QueuedWork = clone(emptyQueuedWork);

    const currentFastWorkQueuePage = ref<QueuedWork[]>([]);
    const fastWorkQueuePagination: Pagination = shallowReactive(clone(emptyPagination));
    let totalFastQueuedItems: ShallowRef<number> = shallowRef(0);
    let initializingFastWorkQueue: boolean = true;
    let selectedFastQueuedWork: QueuedWork = clone(emptyQueuedWork);

    let aggragateDialogData: any = { showDialog: false };
    let showFilterScenarioList = ref(false);
    let showSharedFilterScenarioList = ref(false)
    let availableScenarioIds = ref(['']);

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

            let stateUsers = computed<User[]>(()=>store.state.userModule.users);

            // Clone the stateSharedScenariosPage
            const clonedPage = clone(stateSharedScenariosPage.value);

            // Get the list of inactive users
            const inactiveUsers = stateUsers.value.filter(user => user.activeStatus === false);

            // Create a set of inactive usernames for quick lookup
            const inactiveUsernamesSet = new Set(inactiveUsers.map(user => user.username));

            // Append [Inactive] to creators' names if they are in the inactive list
            clonedPage.forEach(item => {
            if (item.creator && inactiveUsernamesSet.has(item.creator)) {
                item.creator += ' [Inactive]';
                item.owner += ' [Inactive]';
            }
            });

        // Update the currentSharedScenariosPage with the modified list
        currentSharedScenariosPage.value = clonedPage
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

        let stateUsers = computed<User[]>(()=>store.state.userModule.users);

        // Clone the stateUserScenariosPage
        const clonedPage = clone(stateUserScenariosPage.value);

        // Get the list of inactive users
        const inactiveUsers = stateUsers.value.filter(user => user.activeStatus === false);

        // Create a set of inactive usernames for quick lookup
        const inactiveUsernamesSet = new Set(inactiveUsers.map(user => user.username));

       // Append [Inactive] to creators' names if they are in the inactive list
       clonedPage.forEach(item => {
        if (item.creator && inactiveUsernamesSet.has(item.creator)) {
            item.creator += ' [Inactive]';
            item.owner += ' [Inactive]';
        }
    });
    
    // Update the currentUserScenariosPage with the modified list
    currentUserScenariosPage.value = clonedPage
    }

    watch(stateTotalUserScenarios, onStateTotalUserScenariosChanged) 
    function onStateTotalUserScenariosChanged(){
        totalUserScenarios.value = stateTotalUserScenarios.value;
    }
    watch(totalUserScenarios, onTotalUserScenariosChanged) 
    function onTotalUserScenariosChanged(){
        setTabTotals();
    }
    
    watch(stateWorkQueuePage, () => {
        currentWorkQueuePage.value = clone(stateWorkQueuePage.value);
    });

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
        currentFastWorkQueuePage.value = clone(stateFastWorkQueuePage.value);
    }

    watch(stateTotalFastQueuedItems, onStateTotalFastQueuedItemsChanged) 
    function onStateTotalFastQueuedItemsChanged(){
        totalFastQueuedItems.value = stateTotalFastQueuedItems.value;
    }
  
    watch(totalFastQueuedItems, onTotalFastQueuedItemsChanged) 
    function onTotalFastQueuedItemsChanged(){
        setTabTotals();
    }

    function onUserScenariosPagination() {
        if(initializing)
            return;
        const { sort, descending, page, rowsPerPage } = userScenariosPagination;

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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearchMine.value
        };
        if(hasValue(networks) )
            getUserScenariosPageAction(request); 
    }

    function onSharedScenariosPagination(pageEvent?: { page: number, itemsPerPage: number, sortBy: string[] }) {
        if(initializing)
            return;
        const { sort, descending, page, rowsPerPage } = sharedScenariosPagination;

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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearchShared
        };
        if(hasValue(networks) )
            getSharedScenariosPageAction(request); 
    }

    // Refresh both lists and counts(gets called when clone, delete, create operations are performed)
    function onScenariosPagination() {
        if(initializing)
            return;

        const { sort, descending, page, rowsPerPage } = userScenariosPagination;
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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: currentSearchMine.value
        };

        if(hasValue(networks))
            getUserScenariosPageAction(request).then(() => {
                onSharedScenariosPagination();
            });
    }

    function onWorkQueuePagination() {
        doWorkQueuePagination();
    }

    function onFastWorkQueuePagination() {
        doFastQueuePagination();
    }

    function doWorkQueuePagination() {
        if(initializingWorkQueue)
            return;
        const { sort, descending, page, rowsPerPage } = workQueuePagination.value;

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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: ""
        };
        getWorkQueuePageAction(workQueueRequest);    
    }

    function doFastQueuePagination() {
        if(initializingWorkQueue)
            return;
        const { sort, descending, page, rowsPerPage } = fastWorkQueuePagination;

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
            sortColumn: sort != null && !isNil(sort[0]) ? sort[0].key : '',
            isDescending: sort != null && !isNil(sort[0]) ? sort[0].order === 'desc' : false,
            search: ""
        };
        getFastWorkQueuePageAction(workQueueRequest);    
    }

    onBeforeMount(async () => {
        const route = useRoute();
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

        $emitter.on(
            Hub.BroadcastEventType.BroadcastSimulationDeletionCompletionEvent,
            importCompleted,
        );

        $emitter.on('SimulationRunSettingUpdated', () => {
            availableScenarioIds.value.push(simulationRunSettingId.value);
            checkIfReportExists(simulationRunSettingId);
        });

        availableActions = {
            runAnalysis: 'runAnalysis',
            reports: 'reports',
            deleteReports: 'deletedReports',
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
                title: 'Settings',
                action: availableActions.settings,
                icon: getUrl("assets/icons/gear.svg"),
                isCustomIcon: true
            },
            {
                title: 'Committed Projects',
                action: availableActions.commitedProjects,
                icon: getUrl("assets/icons/committed-projects.svg"),
                isCustomIcon: true
            },
            {
                title: 'Run Analysis',
                action: availableActions.runAnalysis,
                icon: getUrl("assets/icons/monitor.svg"),
                isCustomIcon: true
            },
            {
                title: 'Reports',
                action: availableActions.reports,
                icon: getUrl("assets/icons/clipboard.svg"),
                isCustomIcon: true
            },
            ...(hasAdminAccess ? [{
                title: 'Delete all generated Reports',
                action: availableActions.deleteReports,
                icon: getUrl("assets/icons/clipboard.svg"),
                isCustomIcon: true
            }] : []), 
            {
                title: 'Clone',
                action: availableActions.clone,
                icon: getUrl("assets/icons/copy.svg"),
                isCustomIcon: true
            },
            {
                title: 'Delete',
                action: availableActions.delete,
                icon: getUrl("assets/icons/trash.svg"),
                isCustomIcon: true
            }           
        ];
        actionItemsForWorkQueue = [
             {
                title: 'Cancel Work',
                action: availableSimulationActions.cancel,
                icon: getUrl("assets/icons/x-circle.svg"),
                isCustomIcon: true
            }             
        ];
        actionItemsForFastWorkQueue = [
             {
                title: 'Cancel Work',
                action: availableSimulationActions.fastCancel,
                icon: getUrl("assets/icons/x-circle.svg"),
                isCustomIcon: true
            }             
        ];
        actionItems = actionItemsForSharedScenario.slice();
        actionItems.splice(5, 0, {
            title: 'Share',
            action: availableActions.share,
            icon: getUrl("assets/icons/share-geometric.svg"),
                isCustomIcon: true
        });
        tabItems.push(
            { name: 'My scenarios', icon: getUrl("assets/icons/star-empty.svg"), count: totalUserScenarios.value },
            { name: 'Shared with me', icon: getUrl("assets/icons/share-empty.svg"), count: totalSharedScenarios.value },
            { name: 'General work queue', icon: getUrl("assets/icons/queue.svg"), count: totalQueuedSimulations.value },
        );
        tab.value = 'My scenarios';

        // Get the current tab name
        const tabName = route.query.tab;

        // Check if the tab has a query to go to a different tab
        if (typeof tabName === 'string' && tabItems.some(item => item.name === tabName))
        {
        tab.value = tabName;
        router.replace({ query: { ...route.query, tab: undefined } });
      } 
      else 
      {
        // If no valid tab query param, reset the query and set default tab
        router.replace({ query: { ...route.query, tab: undefined } });
        tab.value = 'My scenarios';
      }
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

        $emitter.off(
            Hub.BroadcastEventType.BroadcastSimulationDeletionCompletionEvent,
            importCompleted,
        );
    });

    async function initializeScenarioPages(){
        const { sort, descending, page, rowsPerPage } = sharedScenariosPagination;

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
        await getSharedScenariosPageAction(request)
        await getUserScenariosPageAction(request)
        await getWorkQueuePageAction(workQueueRequest)
        await getFastWorkQueuePageAction(workQueueRequest)
        initializing = false;
        initializingWorkQueue = false;
        initializingFastWorkQueue = false;
        totalUserScenarios.value = stateTotalUserScenarios.value;
        totalSharedScenarios.value = stateTotalSharedScenarios.value;
        totalQueuedSimulations.value = stateTotalQueuedSimulations.value;
        totalFastQueuedItems.value = stateTotalFastQueuedItems.value;
        currentUserScenariosPage.value = clone(stateUserScenariosPage.value);
        currentSharedScenariosPage.value = clone(stateSharedScenariosPage.value);
        currentWorkQueuePage.value = clone(stateWorkQueuePage.value);
        currentFastWorkQueuePage.value = clone(stateFastWorkQueuePage.value);
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
                if(tab.value == tabItems[0].name)
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
        nameUpdate.value = name;
    }

    function onShowConfirmAnalysisRunAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

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
        confirmAnalysisRunAlertData.value = clone(emptyAlertDataWithButtons);
        runAnalysisScenario = selectedScenario;

        if (submit == "pre-checks") {
            preCheckMessages = [];
                if (submit && selectedScenario.id !== getBlankGuid()) 
                {
                    await ScenarioService.upsertValidateSimulation(selectedScenario.networkId, selectedScenario.id).then((response: AxiosResponse) => {
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
                emptyTreatmentBudgets = await getScenarioSelectableTreatmentsAction({ scenarioId: selectedScenario.id });
                emptyTreatmentBudgets.forEach((treatment: { budgets: string | any[]; name: any; }) => {
                    if (!treatment.budgets || treatment.budgets.length === 0) {
                        preCheckMessages += `Treatment ${treatment.name} has no budgets.`
                    }
                });

                secondRunAnalysisModal();
        }
        else if(submit == "continue") {
            store.dispatch('updateSimulationRunSettingName', selectedScenario.name);
            store.dispatch('updateSimulationRunSettingId', selectedScenario.id);
            if (submit && selectedScenario.id !== getBlankGuid()) {
                runSimulationAction({
                    networkId: selectedScenario.networkId,
                    scenarioId: selectedScenario.id,
                }).then(() => (selectedScenario = clone(emptyScenario)));
            }
            
            //Add a small delay to avoid adding two items to the queue at the same time
            await new Promise(resolve => setTimeout(resolve, 500));
            tab.value = 'General work queue';
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

    async function onConfirmAnalysisPreCheckAlertSubmit(submit: boolean) {
        store.dispatch('updateSimulationRunSettingName', selectedScenario.name);
        store.dispatch('updateSimulationRunSettingId', selectedScenario.id);
        confirmAnalysisPreCheckAlertData.value = clone(emptyAlertPreChecksData);

        selectedScenario = runAnalysisScenario;

        if (submit && selectedScenario.id !== getBlankGuid()) {
            runSimulationAction({
                networkId: selectedScenario.networkId,
                scenarioId: selectedScenario.id,
            }).then(() => (selectedScenario = clone(emptyScenario)));
        }

        //Add a small delay to avoid adding two items to the queue at the same time
        await new Promise(resolve => setTimeout(resolve, 500));
        tab.value = 'General work queue';
    }

    function onShowConfirmConvertJsonToRelationalAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        ConfirmConvertJsonToRelationalData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message:
                'Converting the simulation output from json to a relational format can be a lengthy process.',
        };
    }

    function onConfirmConvertJsonToRelationalAlertSubmit(submit: boolean) {
        ConfirmConvertJsonToRelationalData.value = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            ScenarioService.ConvertSimulationOutputToRelational(selectedScenario.id);
        }
    }

    function onShowReportsDownloaderDialog(scenario: Scenario) {
        reportsDownloaderDialogData.value = {
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
    
    function onNavigateToReportsView(localScenario: Scenario) {
        selectScenarioAction({scenarioId: localScenario.id });
        $router.push({
            path: '/ReportsAndOutputs/Scenario/',
            query: {
                scenarioId: localScenario.id,
                networkId: localScenario.networkId,
                scenarioName: localScenario.name,
                networkName: localScenario.networkName,
            }
        });
    }

    function onDeleteReportsDialog(localScenario: Scenario)
    {
        confirmDeleteReportsData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete all generated reports for ' + localScenario.name + '?',
        };
        scenarioForReportDeletion.value = localScenario;
    }
    
    async function onDeleteAllGeneratedReports(submit: boolean) {
        confirmDeleteReportsData.value = clone(emptyAlertData);

        if(submit === true)
        {
            if(scenarioForReportDeletion.value)
            {
                deleteAllReports(scenarioForReportDeletion.value);
            }
        }
    }

    function onShowShareScenarioDialog(scenario: Scenario) {
        shareScenarioDialogData.value = {
            showDialog: true,
            scenario: clone(scenario),
        };
    }

    function onShareScenarioDialogSubmit(scenarioUsers: ScenarioUser[]) {
        const scenario: Scenario = {
            ...shareScenarioDialogData.value.scenario,
            users: [],
        };

        shareScenarioDialogData.value = clone(emptyShareScenarioDialogData);

        if (!isNil(scenarioUsers) && scenario.id !== getBlankGuid()) {
            updateScenarioAction({
                scenario: { ...scenario, users: scenarioUsers },
            });
        }
    }

    function onShowConfirmCloneScenarioAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        confirmCloneScenarioAlertData.value = {
            showDialog: true,
            heading: 'Clone Scenario',
            choice: true,
            message: 'Are you sure you want clone this scenario?',
        };
    }

    function onShowCloneScenarioDialog(scenario: Scenario) {
        selectedScenario = clone(scenario);

        cloneScenarioDialogData.value = {
            showDialog: true,
            scenario: selectedScenario
        };
    }

    function onConfirmCloneScenarioAlertSubmit(submit: boolean) {
        confirmCloneScenarioAlertData.value = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            cloneScenarioAction({
                scenarioId: selectedScenario.id,
            }).then(() => {
                selectedScenario = clone(emptyScenario)               
                if(tab.value == tabItems[0].name) {
                    onUserScenariosPagination();
                }
                else {
                    onSharedScenariosPagination();
                }
            });
        }
    }

    function onCloneScenarioDialogSubmit(scenario: CloneScenario) {
        cloneScenarioDialogData.value = clone(emptyCloneScenarioDialogData);

        if (!isNil(scenario)) {
            cloneScenarioWithDestinationNetworkAction(scenario).then(() => {
                selectedScenario = clone(emptyScenario)
                onScenariosPagination();
            });
        }
    }

    function onShowConfirmDeleteAlert(scenario: Scenario) {
        selectedScenario = clone(scenario);

        confirmDeleteAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to delete?',
        };
    }


    function onShowConfirmCancelAlert(simulation: QueuedWork) {
        selectedQueuedWork = clone(simulation);

        confirmCancelAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to cancel this process?',
        };
    }

    function onShowConfirmFastCancelAlert(simulation: QueuedWork) {
        selectedFastQueuedWork = clone(simulation);

        confirmCancelAlertData.value = {
            showDialog: true,
            heading: 'Warning',
            choice: true,
            message: 'Are you sure you want to cancel this process?',
        };
    }

    function onConfirmDeleteAlertSubmit(submit: boolean) {
        confirmDeleteAlertData.value = clone(emptyAlertData);

        if (submit && selectedScenario.id !== getBlankGuid()) {
            deleteScenarioAction({
                scenarioId: selectedScenario.id,
                scenarioName: selectedScenario.name,
            }).then(async () => {
                deleteAllReports(selectedScenario.id);
                selectedScenario = clone(emptyScenario); 
            });
        }
    }

    async function deleteAllReports(_scenario: any) {
        await ReportsService.deleteAllGeneratedReports(_scenario.id)
            .then((response: AxiosResponse<any>) => {
                if (hasValue(response, 'data')) {
                    if (!response.data.includes("No reports exist")) {
                        addSuccessNotificationAction({
                            message: 'All reports for ' + _scenario.name + ' have been deleted.',
                        });
                    }
                } else {
                    addErrorNotificationAction({
                        message: 'Failed to delete reports.',
                        longMessage: 'Failed to delete the associated reports. Please check if the scenario has generated reports.',
                    });
                }
            });
    }

    function onConfirmCancelAlertSubmit(submit: boolean) {
        confirmCancelAlertData.value = clone(emptyAlertData);

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
    function onFilterScenarioListSubmit(filterCategory:string, FilterValue:string) {
        showFilterScenarioList.value = false;
        sortedMineCategory = filterCategory;
        sortedMineValue = FilterValue;
        if ((filterCategory !=''&&!isNil(filterCategory)) && (FilterValue !=''&&!isNil(FilterValue)))
        {
            currentSearchMine.value = FilterValue;
            resetPageMine();
        }
        else if(!isNil(filterCategory)||!isNil(FilterValue))
        {
            sortedMineCategory = '';
            sortedMineValue = '';
            currentSearchMine.value = '';
            resetPageMine();
        }
    }
    function onFilterSharedScenarioListSubmit(filterCategory:string, FilterValue:string) {
        showSharedFilterScenarioList.value = false;
        sortedCategory = filterCategory;
        sortedValue = FilterValue;
        if ((filterCategory !=''&&!isNil(filterCategory)) && (FilterValue !=''&&!isNil(FilterValue)))
        {
            currentSearchShared = FilterValue;
            resetPageShared();
        }
        else if(!isNil(filterCategory)||!isNil(FilterValue))
        {
            sortedCategory = ''
            sortedValue = ''
            currentSearchShared = '';
            resetPageShared();
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

    function checkIfReportExists(scenarioId: any) 
    {
        return availableScenarioIds.value.includes(scenarioId);
    }

    function importCompleted(data: any){
        
        var workType = data.workType as WorkType
        if(workType  === WorkType.DeleteSimulation ){
            onScenariosPagination()
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
        showCreateScenarioDialog.value = false;

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
                onNavigateToReportsView(scenario);
                break;
            case availableActions.deleteReports:
            onDeleteReportsDialog(scenario);
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
        currentSearchMine.value = searchMine.value;
        resetPageMine()
    }

    function onMineClearClick(){
        searchMine.value = '';
        onMineSearchClick();
    }
    function onMineFilterClearClick(){
        sortedMineCategory = '';
        sortedMineValue = '';
        currentSearchMine.value= '';
        resetPageMine()
    }

    function resetPageMine(){
        userScenariosPagination.page = 1;
        onUserScenariosPagination();
    }

    function onSharedSearchClick(){
        currentSearchShared = searchShared.value;
        resetPageShared()
    }
    function onSharedFilterClearClick(){
        sortedCategory = '';
        sortedValue = '';
        currentSearchShared = '';
        resetPageShared()
    }

    function onSharedClearClick(){
        searchShared.value = '';
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
        return currentSearchMine.value.trim() !== '';
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
