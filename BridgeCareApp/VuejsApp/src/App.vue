<template>
    <v-app class="paper-white-bg">
        <v-main style="font-family: roboto;">
            <v-toolbar app class="paper-white-bg elevation-2">
                <v-toolbar-title  >
                    <v-row  >
                        <v-col><v-img v-bind:src="agencyLogo" @click="onNavigate('/Scenarios/')"/></v-col>                    
                    </v-row>
                </v-toolbar-title>

                <v-toolbar-title >
                    <v-row justify="start">
                        <v-col><img v-bind:src="productLogo" @click="onNavigate('/Scenarios/')"/></v-col>
                    </v-row>
                </v-toolbar-title>
                <v-toolbar-items >
                    <v-btn-toggle style="height: 100%;">
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
                    </v-btn-toggle>
                </v-toolbar-items>
                
                <v-spacer></v-spacer>
                <v-spacer></v-spacer>
                
                    
                <v-toolbar-title class="white--text">
                    
                    <v-menu
                        offset-
                        min-width="21%"
                        max-width="21%"
                        max-height="500%"
                        :close-on-content-click="false"
                    >
                    
                        <template v-slot:activator="{ props }">
                            <button
                                style="margin-left: 170%;"  
                                id="App-notification-button"
                                v-bind="props"
                                @click="onNotificationMenuSelect"
                                class="notification-icon"
                                icon
                            >
                                <img style="position:absolute; top:20px; height:25px;" :src="getUrl('assets/icons/bell.svg')"/>
                                <v-badge
                                    v-if="notificationCounter > 0"
                                    overlap
                                    style="position: absolute;"
                                    :size="30"
                                    :content="notificationCounter"
                                    :animated="true"
                                    fontSize="12px"
                                    counterStyle="round"
                                    counterLocation="upperRight"
                                    counterBackgroundColor="#FF0000"
                                    counterTextColor="#FFFFFF"
                                    color="#002E6C"
                                    class="hide-bell-svg"
                                   
                                > 
                                <v-icon style="bottom: 50%;" size="small"></v-icon>
                                </v-badge>
                            </button>
                        </template>            
                        <v-card class="mx-auto" style="width: 1950%; min-height: 80vh; left: -170px; top: 25px;">
                            <v-toolbar 
                                id = "App-notification-toolbar"
                                color="#002E6C" dark>


                                <v-toolbar-title>Notifications</v-toolbar-title>

                            </v-toolbar>
                            <v-list class="h-100">
                                <v-list-group
                                    v-for="(notification,index) in notifications"
                                    :key="notification.id"
                                    
                                    v-model="notification.active"
                                    append-icon=""
                                    class="notification-message"
                                    style="border-bottom: 1px solid; padding:5%;"
                                    @click="toggleExpand(notification.active,index)"
                                >
                               
                                    <template v-slot:activator justify-end   >
                                        
                                        <v-list-tile 
                                            id="App-notification-vListTile">
                                            <v-row justify="end">
                                            <v-col cols ="9">
                                                <v-icon class="notificationIcon"
                                        :color="notification.iconColor"
                                        >{{ notification.icon }}</v-icon
                                    >
                                            
                                            <v-list-item-content
                                                style="margin-bottom: 10px;"
                                                v-text="
                                                    notification.shortMessage
                                                "
                                            >
                                            
                                        </v-list-item-content>
                                    </v-col>
                                    <v-col> 
                                        <v-btn icon size="16" justify-end position="absolute" style="margin-left:15%;">
                                                <v-icon
                                                    size="small"
                                                    @click="
                                                        onRemoveNotification(
                                                            notification.id,
                                                        )
                                                    "
                                                    >fas fa-times-circle</v-icon
                                                >
                                        </v-btn>
                                    </v-col>
                                    </v-row>
                                        </v-list-tile>
                                        <v-list-tile class="notification-long-message" v-if="notification.active">
                                            <v-list-item-title
                                                class="text-wrap"
                                                v-text="
                                                    notification.longMessage
                                                "
                                            ></v-list-item-title>
                                        </v-list-tile>
                                    </template>
                                    
                                    <v-spacer></v-spacer>
                                </v-list-group>
                            </v-list>         
                        </v-card>
                    </v-menu>
                </v-toolbar-title>
                <v-toolbar-title>
                    <v-divider class="mx-1 navbar-divider" vertical style="background-color: #798899; margin-left:90% !important;"/>
                </v-toolbar-title>
                <v-toolbar-title style="margin-left:2px !important" class="navbar-gray" v-if="authenticated">
                    <img style="height:40px; position:relative; top:2px" :src="getUrl('assets/icons/user-no-circle.svg')"/>
                    <span
                      id="App-username-span"
                    >{{ username }}</span>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="!authenticated">
                    <v-btn
                        style="background-color: #002E6C;"
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogin"
                        class="mx-2"
                        icon
                        color="#FFFFFF"
                    >
                        <v-icon size="small" color="white">fas fa-sign-in-alt</v-icon>
                    </v-btn>
                    <v-btn
                        style="background-color: #002E6C;"
                        v-if="securityType === esecSecurityType && currentURL != 'AuthenticationStart'"
                        @click="onNavigate('/AuthenticationStart/')"
                        class="mx-2"
                        icon
                        color="#FFFFFF"
                    >
                        <font-awesome-icon :icon="['fas','sign-in-alt']"/>
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <v-btn
                        style="background-color: #002E6C;"
                        id="App-b2cLogout-vbtn"
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogout"
                        class="mx-2"
                        icon
                        color="#ffffff"
                    >
                    <font-awesome-icon :icon="['fas', 'sign-out-alt']"/>
                    </v-btn>
                    <v-btn
                        style="background-color: #002E6C;"  
                        id="App-esecLogout-vbtn"
                        v-if="securityType === esecSecurityType"
                        @click="onLogout"
                        class="mx-2"
                        icon                        
                        color="#ffffff"
                    >
                        <font-awesome-icon :icon="['fas', 'sign-out-alt']"/>
                    </v-btn>
                </v-toolbar-title>
            </v-toolbar>
                <v-alert
                v-model='alert'
                type="info">
                    {{stateAlertMessage}}
                </v-alert>
                <div class="scenario-status" v-if="hasSelectedScenario" style="margin-bottom: 20px; height: auto;">
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
                <div v-else style="margin-bottom: 20px; height: auto;">
                    <br>
                </div>
            <v-container fluid v-bind="container">
                <router-view></router-view>
            </v-container>
            <v-footer app color ="#00204B"  fixed>
                <v-spacer></v-spacer>
                <v-col cols = "1">
                    <div class="dev-and-ver-div" >
                        <div>iAM</div> &nbsp;
                        <div>{{implementationName}}</div> &nbsp;
                        <div>{{packageVersionEnv}}</div>
                    </div>
                </v-col>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner />
            <Alert :dialog-data="alertDialogData" @submit="onAlertResult" />
            <NewsDialog :showDialog="showNewsDialog" @close="onCloseNewsDialog()" />
        </v-main>
    </v-app>
</template>

<script setup lang="ts">
import {inject, reactive, computed, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import NotificationBell from 'vue-notification-bell';
import Notifications from '@kyvg/vue3-notification'
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
import router from './router';
import mitt from 'mitt'
import vuetify from '@/plugins/vuetify';
import config from '../public/config.json';
import { getUrl } from './shared/utils/get-url';

    let store = useStore();
    let authenticated = computed(() => store.state.authenticationModule.authenticated);
    let hasRole = computed<boolean>(() => store.state.authenticationModule.hasRole);
    let username = computed<string>(() => store.state.authenticationModule.username);
    let hasAdminAccess = computed(() => store.state.authenticationModule.hasAdminAccess);
    const refreshing = computed<boolean>(() => store.state.authenticationModule.refreshing);
    //let navigation = ref<any[]>(store.state.breadcrumbModule.navigation);
    const notifications = computed<Notification[]>(() => store.state.notificationModule.notifications);
    const notificationCounter = computed<number>(() => store.state.notificationModule.counter);
    const stateSelectedScenario = computed<Scenario>(() => store.state.scenarioModule.selectedScenario);
    const packageVersion = computed<string>(() => store.state.announcementModule.packageVersion);
    const securityType = computed<string>(() => store.state.authenticationModule.securityType);
    const announcements = computed(() => store.state.announcementModule.announcements);
    const currentUser = computed<User>(() =>store.state.userModule.currentUser);
    const stateImplementationName = computed<string>(()=>store.state.adminSiteSettingsModule.implementationName);
    const agencyLogoBase64 = computed(() => store.state.adminSiteSettingsModule.agencyLogo);
    const productLogoBase64 = computed(() => store.state.adminSiteSettingsModule.productLogo);
    const stateInventoryReportNames = computed<string[]>(() => store.state.adminDataModule.inventoryReportNames);
    const stateAlertMessage = computed<string>(() => store.state.alertModule.alertMessage);
    const stateAlert = ref<boolean>(store.state.alertModule.alert);
    async function logOutAction(payload?: any): Promise<any> {await store.dispatch('logOut', payload);}
    async function setIsBusyAction(payload?: any): Promise<any> { await store.dispatch('setIsBusy', payload);}
    async function getNetworksAction(payload?: any): Promise<any> { await store.dispatch('getNetworks', payload);}
    async function getAttributesAction(payload?: any): Promise<any> { await store.dispatch('getAttributes', payload);}
    async function getAnnouncementsAction(payload?: any): Promise<any> { await store.dispatch('getAnnouncements', payload);}
    async function addSuccessNotificationAction(payload?: any): Promise<any> { await store.dispatch('addSuccessNotification', payload);}
    async function addWarningNotificationAction(payload?: any): Promise<any> { await store.dispatch('addWarningNotification', payload);}
    async function addErrorNotificationAction(payload?: any): Promise<any> { await store.dispatch('addErrorNotification', payload);} 
    async function addInfoNotificationAction(payload?: any): Promise<any> { await store.dispatch('addInfoNotification', payload);} 
    async function addTaskCompletedNotificationAction(payload: any): Promise<any> { await store.dispatch('addTaskCompletedNotification', payload)}
    async function removeNotificationAction(payload?: any): Promise<any> { await store.dispatch('removeNotification', payload);}
    async function clearNotificationCounterAction(payload?: any): Promise<any> { await store.dispatch('clearNotificationCounter', payload);} 
    async function generatePollingSessionIdAction(payload?: any): Promise<any> { await store.dispatch('generatePollingSessionId', payload);}
    async function getAllUsersAction(payload?: any): Promise<any> { await store.dispatch('getAllUsers', payload);}
    async function getUserCriteriaFilterAction(payload?: any): Promise<any> { await store.dispatch('getUserCriteriaFilter', payload);} 
    async function loadNotificationsActions(payload?: any): Promise<any> { await store.dispatch('loadNotifications', payload);} 
    async function azureB2CLoginAction(payload?: any): Promise<any> { await store.dispatch('azureB2CLogin', payload);} 
    async function azureB2CLogoutAction(payload?: any): Promise<any> { await store.dispatch('azureB2CLogout', payload);} 
    async function getCurrentUserByUserNameAction(payload?: any): Promise<any> { await store.dispatch('getCurrentUserByUserName', payload);}
    async function updateUserLastNewsAccessDateAction(payload?: any): Promise<any> { await store.dispatch('updateUserLastNewsAccessDate', payload);}
    async function getImplementationNameAction(payload?: any): Promise<any> { await store.dispatch('getImplementationName', payload);}
    async function getAgencyLogoAction(payload?: any): Promise<any> { await store.dispatch('getAgencyLogo', payload);} 
    async function getProductLogoAction(payload?: any): Promise<any> { await store.dispatch('getProductLogo', payload);} 
    async function getInventoryReportsAction(payload?: any): Promise<any> { await store.dispatch('getInventoryReports', payload);} 
    async function setAlertMessageAction(payload?: any): Promise<any> { await store.dispatch('setAlertMessage', payload);} 

    let drawer: boolean = false;
    let latestNewsDate: string = '0001-01-01';
    let currentUserLastNewsAccessDate: string = '0001-01-01';
    let alertDialogData: AlertData = clone(emptyAlertData);
    let pushRouteUpdate: boolean = false;
    let route: any = {};
    const selectedScenario = ref<Scenario>(clone(emptyScenario));
    const hasSelectedScenario = ref<boolean>(false);
    const selectedScenarioHasStatus = ref<boolean>(false);
    let ignoredAPIs: string[] = [
        'SynchronizeLegacySimulation',
        'RunSimulation',
        'GenerateReport',
        'AggregateNetworkData',
        'RefreshToken',
    ];
    let esecSecurityType: string = SecurityTypes.esec;
    let b2cSecurityType: string = SecurityTypes.b2c;
    
    let showNewsDialog= ref(false);

    let hasUnreadNewsItem: boolean = false;
    let currentURL: any = '';
    let unauthorizedError: string = '';
    const implementationName =ref<string>('');
    let packageVersionEnv = ref<string>('');
    const agencyLogo= ref<string>('');
    const productLogo= ref<string>('');
    let inventoryReportName: string = '';
    let alert: Ref<boolean> = ref(false);

    const $emitter = mitt()
    
    created();

    function container() {
        const container: any = {};

        if (vuetify.display.xs) {
            container['grid-list-xs'] = true;
        }

        if (vuetify.display.sm) {
            container['grid-list-sm'] = true;
        }

        if (vuetify.display.md) {
            container['grid-list-md'] = true;
        }

        if (vuetify.display.lg) {
            container['grid-list-lg'] = true;
        }

        if (vuetify.display.xl) {
            container['grid-list-xl'] = true;
        }

        return container;
    }

    function authenticatedWithRole() {
        return authenticated && hasRole;
    }
    
    watch(stateSelectedScenario, () => {
        selectedScenario.value = clone(stateSelectedScenario.value);
        hasSelectedScenario.value= selectedScenario.value.id !== getBlankGuid();
        selectedScenarioHasStatus.value = hasValue(selectedScenario.value.status);
    })

    watch(authenticated, () => {
        if (authenticated) {
            onAuthenticate();
        }
    });

    watch(announcements, () => {
        latestNewsDate = getDateOnly(announcements.value[0].createdDate.toString()); 
    });

    watch(currentUser, () => {
        currentUserLastNewsAccessDate = getDateOnly(currentUser.value.lastNewsAccessDate);
        checkLastNewsAccessDate();
    });

    watch(stateImplementationName, () =>  {
        implementationName.value = stateImplementationName.value;
    })

    watch(agencyLogoBase64, () => {
        agencyLogo.value = agencyLogoBase64.value;
    })

    watch(productLogoBase64, () =>  {
        productLogo.value = productLogoBase64.value;
    })

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
        packageVersionEnv.value = import.meta.env.VITE_APP_VERSION // declared in .env files

        // create a request handler
        async function requestHandler(
            request: AxiosRequestConfig,
        ) {
            request.headers = setContentTypeCharset(request.headers);
            if (refreshing.value) {
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
                router.currentRoute.value.name as string,
            ) === -1
        ) {
            // Upon opening the page, and every 30 seconds, check if authentication data
            // has been changed by another tab or window
            clearRefreshIntervalID();
            setRefreshIntervalID();
        }
    }

    
    onMounted(() => {

        $emitter.on(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            onAddErrorNotification,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastWarningEvent,
            onAddWarningNotification,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastInfoEvent,
            onAddInfoNotification,
        );
        $emitter.on(
            Hub.BroadcastEventType.BroadcastTaskCompletedEvent,
            onAddTaskCompletedNotification
        );
        
        currentURL = router.currentRoute.value.name;

        if(config.agencyLogo.trim() === "")
            agencyLogo.value = new URL(`assets/images/generic/IAM_Main.jpg`, import.meta.url).href;
        else
            agencyLogo.value = agencyLogoBase64.value

        if(config.productLogo.trim() === "")
            productLogo.value = new URL(`assets/images/generic/IAM_Banner.jpg`, import.meta.url).href;
        else
            productLogo.value = productLogoBase64.value

        if(implementationName.value === "")
            implementationName.value = "BridgeCare"
        else
            implementationName.value = stateImplementationName.value
    });

    onBeforeUnmount(() => beforeDestroy());
    function beforeDestroy() {
        $emitter.off(
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
        if (router.currentRoute.value.name === 'AuthenticationStart') {
            azureB2CLoginAction();
        } else {
            router.push('/AuthenticationStart');
        }
    }

    function onAzureLogout() {
        azureB2CLogoutAction().then(() => onLogout());
    }
    
    /**
     * Sets up a recurring attempt at refreshing user tokens, and fetches network and attribute data
     */
    function onAuthenticate() {
        //$forceUpdate();
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
        if (router.currentRoute.value.path !== route.path) {
            hasSelectedScenario.value = false;
            router.push(route).catch(() => {});
        }
    }

    function onNotificationMenuSelect() {
        clearNotificationCounterAction();
    }

    function onRemoveNotification(id: number) {
        removeNotificationAction(id);
    }

    function onShowNewsDialog() {
        showNewsDialog.value = true;
        if (currentUser.value.id != getBlankGuid()) {
            updateUserLastNewsAccessDateAction({id: currentUser.value.id, accessDate: latestNewsDate});
        }
        hasUnreadNewsItem = false;
    }

    function onCloseNewsDialog() {
        showNewsDialog.value = false;
    }

    function checkLastNewsAccessDate () {
        hasUnreadNewsItem = newsAccessDateComparison(latestNewsDate, currentUserLastNewsAccessDate);
    }

    const toggleExpand = (active:boolean,index:number) => {notifications.value[index].active=!active};
</script>

<style>
html {
    overflow: auto;
    overflow-x: hidden;
    overflow-y: scroll !important;
}
.mx-auto{
    max-height: 300px;
    overflow-y: auto;
}
.notificationIcon{
    margin-right: 15px;
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
.image{
    width:350px;
    height:60px;
    margin-right: 10px;
    display: inline-block;
}
.custom-toolbar{
   width: 800px ; 
}
.scroll-container {
    max-height: 100%;
    overflow: hidden;
  }
</style>
