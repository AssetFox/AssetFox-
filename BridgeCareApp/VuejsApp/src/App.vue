<template>
    <v-app class="paper-white-bg">
        <v-main>
            <v-toolbar app class="paper-white-bg">
                <v-toolbar-title>
                    <img v-bind:src="agencyLogo" @click="onNavigate('/Scenarios/')" class="pointer-for-image" /> 
                    <img v-bind:src="productLogo" @click="onNavigate('/Scenarios/')" class="pointer-for-image" />
                </v-toolbar-title>
                <v-toolbar-items>
                    <v-btn
                        id="App-scenarios-btn"
                        @click="onNavigate('/Scenarios/')"
                        class="ara-blue-pantone-281"
                        variant = "flat"                     
                    >
                        Scenarios
                    </v-btn>                   
                     <v-btn
                        id="App-libraries-btn"
                        @click="onNavigate('/EditLibrary/')"
                        class="ara-blue-pantone-281"
                        variant = "flat"
                    >
                        Libraries
                    </v-btn>
                    <v-btn
                        id="App-rawData-btn"
                        @click="onNavigate('/EditRawData/')"
                        class="ara-blue-pantone-281"
                        variant = "flat"
                        v-if="hasAdminAccess"
                    >
                        Raw Data
                    </v-btn>
                    <v-btn
                        id="App-administration-btn"
                        @click="onNavigate('/EditAdmin/')"
                        class="ara-blue-pantone-281"
                        variant = "flat"
                        v-if="hasAdminAccess"
                    >
                        Administration
                    </v-btn>
                    <v-btn
                        id="App-inventory-btn"
                        @click="onNavigate('/Inventory/')"
                        class="ara-blue-pantone-281"
                        variant = "flat"
                    >
                        Inventory
                    </v-btn>
                     <v-btn
                        id="App-news-btn"
                        @click="onShowNewsDialog()"
                        class="ara-blue-pantone-281"
                        variant = "flat"
                    >
                        News
                        <v-icon v-if="hasUnreadNewsItem" size="13" class="news-notification">fas fa-exclamation-circle</v-icon>
                    </v-btn>
                </v-toolbar-items>
                <v-spacer></v-spacer>
                <v-toolbar-title class="white--text">
                    <v-menu
                        offset-
                        min-width="20%"
                        max-width="20%"
                        max-height="75%"
                        :close-on-content-click="false"
                    >
                        <template v-slot:activator="{ on, attrs }">
                            <button
                                id="App-notification-button"
                                v-on="on"
                                v-bind="attrs"
                                @click="onNotificationMenuSelect"
                                class="notification-icon"
                                icon
                            >
                                <img style="position:absolute; top:20px; height:25px;" :src="require('@/assets/icons/bell.svg')"/>
                                <!-- <notification-bell
                                    :size="30"
                                    :count="notificationCounter"
                                    :upperLimit="50"
                                    left="8px"
                                    top="8px"
                                    counterPadding="2px"
                                    fontSize="10px"
                                    counterStyle="roundRectangle"
                                    counterBackgroundColor="#FF0000"
                                    counterTextColor="#FFFFFF"
                                    iconColor="#002E6C"
                                    class="hide-bell-svg"
                                /> -->
                            </button>
                        </template>
                        <v-card class="mx-auto" max-width="100%">
                            <v-toolbar 
                                id = "App-notification-toolbar"
                                color="#002E6C" dark>
                                <v-app-bar-nav-icon></v-app-bar-nav-icon>

                                <v-toolbar-title>Notifications</v-toolbar-title>

                                <v-spacer></v-spacer>
                            </v-toolbar>
                            <v-list>
                                <v-list-group
                                    v-for="notification in notifications"
                                    :key="notification.id"
                                    v-model="notification.active"
                                    append-icon=""
                                    class="notification-message"
                                    style="border-bottom: 1px solid;"
                                >
                                    <v-icon
                                        slot="prependIcon"
                                        :color="notification.iconColor"
                                        >{{ notification.icon }}</v-icon
                                    >
                                    <template v-slot:activator>
                                        <v-list-tile
                                            id="App-notification-vListTile">
                                            <v-list-tile-content
                                                style="font-size: 85%"
                                                v-text="
                                                    notification.shortMessage
                                                "
                                            ></v-list-tile-content>
                                            <v-btn icon small right absolute>
                                                <v-icon
                                                    small
                                                    @click="
                                                        onRemoveNotification(
                                                            notification.id,
                                                        )
                                                    "
                                                    >fas fa-times-circle</v-icon
                                                >
                                            </v-btn>
                                        </v-list-tile>
                                    </template>
                                    <v-list-item>
                                        <v-list-item-content>
                                            <v-list-item-title
                                                class="notification-long-message"
                                                v-text="
                                                    notification.longMessage
                                                "
                                            ></v-list-item-title>
                                        </v-list-item-content>
                                    </v-list-item>
                                    <v-spacer></v-spacer>
                                </v-list-group>
                            </v-list>
                        </v-card>
                    </v-menu>
                </v-toolbar-title>
                <v-toolbar-title>
                    <v-divider class="mx-1 navbar-divider" vertical color="#798899"/>
                </v-toolbar-title>
                <v-toolbar-title style="margin-left:2px !important" class="navbar-gray" v-if="authenticated">
                    <img style="height:40px; position:relative; top:2px" :src="require('@/assets/icons/user-no-circle.svg')"/>
                    <span
                      id="App-username-span"
                    >{{ username }}</span>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="!authenticated">
                    <v-btn
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogin"
                        class="mx-2"
                        icon
                        color="#002E6C"
                    >
                        <v-icon small color="white">fas fa-sign-in-alt</v-icon>
                    </v-btn>
                    <v-btn
                        v-if="securityType === esecSecurityType && currentURL != 'AuthenticationStart'"
                        @click="onNavigate('/AuthenticationStart/')"
                        class="mx-2"
                        icon
                        color="#002E6C"
                    >
                        <v-icon small color="white">fas fa-sign-in-alt</v-icon>
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <v-btn
                        id="App-b2cLogout-vbtn"
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogout"
                        class="mx-2"
                        icon
                        color="#002E6C"
                    >
                        <v-icon small color="white">fas fa-sign-out-alt</v-icon>
                    </v-btn>
                    <v-btn
                        id="App-esecLogout-vbtn"
                        v-if="securityType === esecSecurityType"
                        @click="onLogout"
                        class="mx-2"
                        icon                        
                        color="#002E6C"
                    >
                        <v-icon small color="white">fas fa-sign-out-alt</v-icon>
                    </v-btn>
                </v-toolbar-title>
            </v-toolbar>
                <v-alert
                v-model='alert'
                type="info">
                    {{stateAlertMessage}}
                </v-alert>
                <div class="scenario-status" v-if="hasSelectedScenario">
                        <br>
                        <span>Scenario: </span>
                            <span id = 'App-scenarioName-span' style="font-weight: normal;">{{ selectedScenario.name }}</span>
                            {{ "\xA0" }}
                            <span v-if="selectedScenarioHasStatus && hasSelectedScenario">
                            <hr color="#798899" class="mx-1 navbar-divider v-divider v-divider--vertical theme--light">
                            </span>
                            {{ "\xA0" }}
                            <span v-if="selectedScenarioHasStatus">
                            Status:
                            </span>
                            <span style="font-weight: normal;"
                             v-if="selectedScenarioHasStatus">{{
                                 selectedScenario.status
                             }}</span>
                    </div>
            <v-container fluid v-bind="container">
                <router-view></router-view>
            </v-container>
            <v-footer app class="ara-blue-pantone-289-bg white--text" fixed>
                <v-spacer></v-spacer>
                <v-flex xs2>
                    <div class="dev-and-ver-div">
                        <div class="font-weight-light">iAM</div>
                        <div>{{implementationName}}</div>
                        <div>{{ packageVersion }}</div>
                    </div>
                </v-flex>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner />
            <Alert :dialog-data="alertDialogData" @submit="onAlertResult" />
            <NewsDialog :showDialog="showNewsDialog" @close="onCloseNewsDialog()" />
        </v-main>
    </v-app>
</template>

<script setup lang="ts">
import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import NotificationBell from 'vue-notification-bell';
import Spinner from './shared/modals/Spinner.vue';
import { hasValue } from '@/shared/utils/has-value-util';
import { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import {
    axiosInstance,
    coreAxiosInstance,
    nodejsAxiosInstance,
} from '@/shared/utils/axios-instance';
import {
    getErrorMessage,
    setAuthHeader,
    setContentTypeCharset,
} from '@/shared/utils/http-utils';
import Alert from '@/shared/modals/Alert.vue';
import { AlertData, emptyAlertData } from '@/shared/models/modals/alert-data';
import { bind, clone } from 'ramda';
import { emptyScenario, Scenario } from '@/shared/models/iAM/scenario';
import { getBlankGuid } from '@/shared/utils/uuid-utils';
import { newsAccessDateComparison, getDateOnly, getCurrentDateOnly } from '@/shared/utils/date-utils';
import { Hub } from '@/connectionHub';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
import { Notification } from '@/shared/models/iAM/notifications';
import { User, emptyUser } from '@/shared/models/iAM/user';
import { SecurityTypes } from '@/shared/utils/security-types';
import {
    clearRefreshIntervalID,
    setRefreshIntervalID,
} from '@/shared/utils/refresh-interval-id';
import {
    isAuthenticatedUser,
    onHandleLogout,
} from '@/shared/utils/authentication-utils';
import { UnsecuredRoutePathNames } from '@/shared/utils/route-paths';
import NewsDialog from '@/components/NewsDialog.vue'
import { Announcement, emptyAnnouncement } from '@/shared/models/iAM/announcement';
import { useStore } from 'vuex';
import { useRouter, onBeforeRouteLeave } from 'vue-router';


    let store = useStore();
    let authenticated = ref<boolean>(store.state.authenticationModule.authenticated);
    let hasRole = ref<boolean>(store.state.authenticationModule.hasRole);
    let username = ref<string>(store.state.authenticationModule.username);
    let hasAdminAccess = ref<boolean>(store.state.authenticationModule.hasAdminAccess);
    let refreshing = ref<boolean>(store.state.authenticationModule.refreshing);
    //let navigation = ref<any[]>(store.state.breadcrumbModule.navigation);
    let notifications = ref<Notification[]>(store.state.notificationModule.notifications);
    let notificationCounter = ref<number>(store.state.notificationModule.counter);
    let stateSelectedScenario = ref<Scenario>(store.state.scenarioModule.selectedScenario);
    let packageVersion = ref<string>(store.state.announcementModule.packageVersion);
    let securityType = ref<string>(store.state.authenticationModule.securityType);
    let announcements = ref<Announcement[]>(store.state.announcementModule.announcements);
    let currentUser = ref<User>(store.state.userModule.currentUser);
    let stateImplementationName = ref<string>(store.state.adminSiteSettingsModule.implementationName);
    let agencyLogoBase64 = ref<string>(store.state.adminSiteSettingsModule.agencyLogo);
    let productLogoBase64 = ref<string>(store.state.adminSiteSettingsModule.productLogo);
    let stateInventoryReportNames = ref<string[]>(store.state.adminDataModule.inventoryReportNames);
    let stateAlertMessage = ref<string>(store.state.alertModule.alertMessage);
    let stateAlert = ref<boolean>(store.state.alertModule.alert);
    async function logOutAction(payload?: any): Promise<any> {await store.dispatch('logOut');}
    async function setIsBusyAction(payload?: any): Promise<any> { await store.dispatch('setIsBusy');}
    async function getNetworksAction(payload?: any): Promise<any> { await store.dispatch('getNetworks');}
    async function getAttributesAction(payload?: any): Promise<any> { await store.dispatch('getAttributes');}
    async function getAnnouncementsAction(payload?: any): Promise<any> { await store.dispatch('getAnnouncements');}
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification');}
    async function addWarningNotificationAction(payload?: any): Promise<any> { await store.dispatch('addWarningNotification');}
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification');} 
    async function addInfoNotificationAction(payload?: any): Promise<any> { await store.dispatch('addInfoNotification');} 
    async function addTaskCompletedNotificationAction(payload: any): Promise<any> { await store.dispatch('addTaskCompletedNotification', payload)}
    async function removeNotificationAction(payload?: any): Promise<any> { await store.dispatch('removeNotification');}
    async function clearNotificationCounterAction(payload?: any): Promise<any> { await store.dispatch('clearNotificationCounter');} 
    async function generatePollingSessionIdAction(payload?: any): Promise<any> { await store.dispatch('generatePollingSessionId');}
    async function getAllUsersAction(payload?: any): Promise<any> { await store.dispatch('getAllUsers');}
    async function getUserCriteriaFilterAction(payload?: any): Promise<any> { await store.dispatch('getUserCriteriaFilter');} 
    async function loadNotificationsActions(payload?: any): Promise<any> { await store.dispatch('loadNotifications');} 
    async function azureB2CLoginAction(payload?: any): Promise<any> { await store.dispatch('azureB2CLogin');} 
    async function azureB2CLogoutAction(payload?: any): Promise<any> { await store.dispatch('azureB2CLogout');} 
    async function getCurrentUserByUserNameAction(payload?: any): Promise<any> { await store.dispatch('getCurrentUserByUserName');}
    async function updateUserLastNewsAccessDateAction(payload?: any): Promise<any> { await store.dispatch('updateUserLastNewsAccessDate');}
    async function getImplementationNameAction(payload?: any): Promise<any> { await store.dispatch('getImplementationName');}
    async function getAgencyLogoAction(payload?: any): Promise<any> { await store.dispatch('getAgencyLogo');} 
    async function getProductLogoAction(payload?: any): Promise<any> { await store.dispatch('getProductLogo');} 
    async function getInventoryReportsAction(payload?: any): Promise<any> { await store.dispatch('getInventoryReports');} 
    async function setAlertMessageAction(payload?: any): Promise<any> { await store.dispatch('setAlertMessage');} 

    let drawer: boolean = false;
    let latestNewsDate: string = '0001-01-01';
    let currentUserLastNewsAccessDate: string = '0001-01-01';
    let alertDialogData: AlertData = clone(emptyAlertData);
    let pushRouteUpdate: boolean = false;
    let route: any = {};
    let selectedScenario: Scenario = clone(emptyScenario);
    let hasSelectedScenario: boolean = false;
    let selectedScenarioHasStatus: boolean = false;
    let ignoredAPIs: string[] = [
        'SynchronizeLegacySimulation',
        'RunSimulation',
        'GenerateReport',
        'AggregateNetworkData',
        'RefreshToken',
    ];
    let esecSecurityType: string = SecurityTypes.esec;
    let b2cSecurityType: string = SecurityTypes.b2c;
    let showNewsDialog: boolean = false;
    let hasUnreadNewsItem: boolean = false;
    let currentURL: any = '';
    let unauthorizedError: string = '';
    let implementationName: string = '';
    let agencyLogo: string = '';
    let productLogo: string = '';
    let inventoryReportName: string = '';
    let alert: Ref<boolean> = ref(false);

    const $vuetify = inject('$vuetify') as any
    const $router = useRouter();
    const $statusHub = inject('$statusHub') as any
    const $config = inject('$config') as any

    created();

    function container() {
        const container: any = {};

        if ($vuetify.breakpoint.xs) {
            container['grid-list-xs'] = true;
        }

        if ($vuetify.breakpoint.sm) {
            container['grid-list-sm'] = true;
        }

        if ($vuetify.breakpoint.md) {
            container['grid-list-md'] = true;
        }

        if ($vuetify.breakpoint.lg) {
            container['grid-list-lg'] = true;
        }

        if ($vuetify.breakpoint.xl) {
            container['grid-list-xl'] = true;
        }

        return container;
    }

    function authenticatedWithRole() {
        return authenticated && hasRole;
    }
    watch(stateSelectedScenario, () => onStateSelectedScenarioChanged)
    function onStateSelectedScenarioChanged() {
        selectedScenario = clone(stateSelectedScenario.value);
        hasSelectedScenario = selectedScenario.id !== getBlankGuid();
        selectedScenarioHasStatus = hasValue(selectedScenario.status);
    }

    watch(authenticated, () => onAuthenticationChange)
    function onAuthenticationChange() {
        if (authenticated) {
            onAuthenticate();
        }
    }

    watch(announcements, () => onAnnouncementsChange)
    function onAnnouncementsChange() {
        latestNewsDate = getDateOnly(announcements.value[0].createdDate.toString()); 
    }

    watch(currentUser, () => onCurrentUserChange)
    function onCurrentUserChange() {
        currentUserLastNewsAccessDate = getDateOnly(currentUser.value.lastNewsAccessDate);
        checkLastNewsAccessDate();
    }

    watch(stateImplementationName, () => onimplementationNameChange)
    function onimplementationNameChange() {
        implementationName = stateImplementationName.value;
    }

    watch(agencyLogoBase64, () => onAgencyLogoBase64Change)
    function onAgencyLogoBase64Change() {
        agencyLogo = agencyLogoBase64.value;
    }

    watch(productLogoBase64, () => onProductLogoBase64Change)
    function onProductLogoBase64Change() {
        productLogo = productLogoBase64.value;
    }

    watch(stateInventoryReportNames, () => onStateInventoryReportNamesChanged)
    function onStateInventoryReportNamesChanged(){
        if(stateInventoryReportNames.value.length > 0)
            inventoryReportName = stateInventoryReportNames.value[0]
    }

    watch(stateAlertMessage, () => onStateAlertMessageChanged)
    function onStateAlertMessageChanged(){
        if(stateAlertMessage.value.trim() !== ''){
            alert.value = true;
        }
        else
            alert.value = false;
    }

    watch(alert, () => onAlertChanged)
    function onAlertChanged(){
        if(!alert){
            setAlertMessageAction('');
        }
    }
    
    function created() {
        // create a request handler
        async function requestHandler(
            request: AxiosRequestConfig,
        ) {
            request.headers = setContentTypeCharset(request.headers);
            if (refreshing) {
                await new Promise(_ => setTimeout(_, 5000));
            }

            request.headers = setAuthHeader(request.headers);
            setIsBusyAction({
                isBusy: ignoredAPIs.every(
                    (ignored: string) => request.url!.indexOf(ignored) === -1,
                ),
            });
            return request;
        }

        // set axios request interceptor to use request handler
        axiosInstance.interceptors.request.use((request: any) =>
            requestHandler(request),
        );
        // set nodejs axios request interceptor to use request handler
        nodejsAxiosInstance.interceptors.request.use((request: any) =>
            requestHandler(request),
        );
        // set bridge care core axios request interceptor to use request handler
        coreAxiosInstance.interceptors.request.use((request: any) =>
            requestHandler(request),
        );
        // create a success & error handler
        const successHandler = (response: AxiosResponse) => {
            response.headers = setContentTypeCharset(response.headers);
            setIsBusyAction({ isBusy: false });
            return response;
        };
        const errorHandler = (error: AxiosError) => {
            if (error.request) {
                error.request.headers = setContentTypeCharset(
                    error.request.headers,
                );
            }
            if (error.response) {
                error.response.headers = setContentTypeCharset(
                    error.response.headers,
                );
            }
            setIsBusyAction({ isBusy: false });            
            unauthorizedError = hasValue(unauthorizedError) ? error.response!.data : "User is not authorized!";
            if (error.response!.status === 500) return;
            
            addErrorNotificationAction({
                message: error.response!.status === 403 ? "Authorization Failed" : "HTTP Error",
                longMessage: error.response!.status === 403 ? unauthorizedError : getErrorMessage(error),
            });
            if (
                hasValue(error, 'response') &&
                error.response!.status.toString() === '401'
            ) {
                isAuthenticatedUser().then(
                    (isAuthenticated: boolean | void) => {
                        if (!isAuthenticated) {
                            setTimeout(() => onHandleLogout(), 3000);
                        }
                    },
                );
            }
        };
        // set axios response handler to use success & error handlers
        axiosInstance.interceptors.response.use(
            (response: any) => successHandler(response),
            (error: any) => errorHandler(error),
        );
        // set nodejs axios response handler to user success & error handlers
        nodejsAxiosInstance.interceptors.response.use(
            (response: any) => successHandler(response),
            (error: any) => errorHandler(error),
        );
        // set bridge care core axios response handler to use success & error handlers
        coreAxiosInstance.interceptors.response.use(
            (response: any) => successHandler(response),
            (error: any) => errorHandler(error),
        );

        if (
            securityType === SecurityTypes.esec &&
            UnsecuredRoutePathNames.indexOf(
                $router.currentRoute.value.name as string,
            ) === -1
        ) {
            // Upon opening the page, and every 30 seconds, check if authentication data
            // has been changed by another tab or window
            clearRefreshIntervalID();
            setRefreshIntervalID();
        }
    }

    
    onMounted(() => mounted());
    function mounted() {

        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            onAddErrorNotification,
        );
        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastWarningEvent,
            onAddWarningNotification,
        );
        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastInfoEvent,
            onAddInfoNotification,
        );
        $statusHub.$on(
            Hub.BroadcastEventType.BroadcastTaskCompletedEvent,
            onAddTaskCompletedNotification
        );
        
        currentURL = $router.currentRoute.value.name;

        if($config.agencyLogo.trim() === "")
            agencyLogo = require(`@/assets/images/generic/IAM_Main.jpg`)
        else
            agencyLogo = $config.agencyLogo

        if($config.productLogo.trim() === "")
            productLogo = require(`@/assets/images/generic/IAM_Banner.jpg`)
        else
            productLogo = $config.productLogo

        if(implementationName === "")
            implementationName = "BridgeCare"
        else
            implementationName = $config.implementationName
    }

    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        $statusHub.$off(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            onAddErrorNotification,
        );
    }

    function onAddErrorNotification(data: any) {
        let errorNotification:string = data.error.toString();
        let spl = errorNotification.split('::');
        if (spl.length > 0 ) {
            addErrorNotificationAction( {
                message: spl[0],
                longMessage: spl.length>1 ? spl[1] : 'Unknown Error'
            });
        } else {
            addErrorNotificationAction( {
                message: 'Server Error',
                longMessage: data.error
            });
        }
    }

    function onAddInfoNotification(data: any) {
        addInfoNotificationAction({
            message: 'Server Update',
            longMessage: data.info
        });
    }

    function onAddWarningNotification(data: any) {
        let warningNotification:string = data.warning.toString();
        let spl = warningNotification.split('::');
        if (spl.length > 0) {
            addWarningNotificationAction({
                message: spl[0],
                longMessage: spl.length > 1 ? spl[1] : ''
            });
        } else {
            addWarningNotificationAction({
                message: 'Server Warning',
                longMessage: data.warning,
            });
        }
    }

    function onAddTaskCompletedNotification(data: any) {
        addTaskCompletedNotificationAction({
            message: 'Task Completed',
            longMessage: data.task
        });
    }


    function onAlertResult(submit: boolean) {
        alertDialogData = clone(emptyAlertData);

        if (submit) {
            pushRouteUpdate = true;
            onNavigate(route);
        }
    }

    function onAzureLogin() {
        if ($router.currentRoute.value.name === 'AuthenticationStart') {
            azureB2CLoginAction();
        } else {
            $router.push('/AuthenticationStart');
        }
    }

    function onAzureLogout() {
        azureB2CLogoutAction().then(() => onLogout());
    }
    
    /**
     * Sets up a recurring attempt at refreshing user tokens, and fetches network and attribute data
     */
    function onAuthenticate() {
        $forceUpdate();
        getNetworksAction().then(() =>
        getAttributesAction().then(() =>
        getAllUsersAction().then(() =>
        getAnnouncementsAction().then(() =>
        getUserCriteriaFilterAction().then(() =>{
            if (username != null && username.value != '') {
                getCurrentUserByUserNameAction(username.value);
            }
        }).then(() =>
        //If these gets are placed before authorization, GetUserInformation() in EsecSecurity.cs will throw an error, as its HttpRequest will have no Authorization header!
        getImplementationNameAction().then(() =>
        getAgencyLogoAction().then(() => {getProductLogoAction();}
        )))))));
    }
  function $forceUpdate() {
    throw new Error('Method not implemented.');
  }

    /**
     * Dispatches an action that will revoke all user tokens, prevents token refresh attempts,
     * and redirects users to the landing page
     */
    function onLogout() {
        logOutAction().then(() => {
            clearRefreshIntervalID(); 
            if (window.location.host.toLowerCase().indexOf('penndot.gov') === -1 && securityType.value === esecSecurityType) {
                /*
                 * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
                 * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
                 * to allow the landing page to redirect the browser to the original host.
                 */                                
                window.location.href =
                    'http://www.bamssyst.penndot.gov/iAM?host=' +
                    encodeURI(window.location.host);
            } else {
                onNavigate('/iAM/');
            }
        });
    }

    /**
     * Navigates a user to a page using the specified routeName
     * @param route The route name to use when navigating a user
     */
    function onNavigate(route: any) {
        if ($router.currentRoute.value.path !== route.path) {
            $router.push(route).catch(() => {});
        }
    }

    function onNotificationMenuSelect() {
        clearNotificationCounterAction();
    }

    function onRemoveNotification(id: number) {
        removeNotificationAction(id);
    }

    function onShowNewsDialog() {
        showNewsDialog = true;
        updateUserLastNewsAccessDateAction({id: currentUser.value.id, accessDate: latestNewsDate});
        hasUnreadNewsItem = false;
    }

    function onCloseNewsDialog() {
        showNewsDialog = false;
    }

    function checkLastNewsAccessDate () {
        hasUnreadNewsItem = newsAccessDateComparison(latestNewsDate, currentUserLastNewsAccessDate);
    }

</script>

<style>
html {
    overflow: auto;
    overflow-x: hidden;
    overflow-y: scroll !important;
}

.navbar-divider {
    display: inline !important;
    line-height: 100% !important;
}

.navbar-gray {
    color: #2c2c2c !important;
}

.navbar-user-icon {
    margin-right: 10px;
}

.news-notification {
    margin-bottom: 15px;
}

.notification-icon {
    margin-top: 5px;
}

.v-list__group__header__prepend-icon .v-icon {
    color: #798899;
}

.v-list__group__header__prepend-icon .primary--text .v-icon {
    color: #008fca;
}

.dev-and-ver-div {
    display: flex;
    justify-content: space-evenly;
}

.pointer-for-image{
        cursor: pointer;
    }

.hide-bell-svg svg{
    visibility: collapse;
}

</style>
