<template>
    <v-app class="paper-white-bg">
        <v-content>
            <v-toolbar app class="paper-white-bg">
                <v-toolbar-title>
                    <img v-bind:src="agencyLogo" @click="onNavigate('/Scenarios/')" class="pointer-for-image" /> 
                    <v-divider class="mx-2 navbar-divider" vertical color="#798899"/>
                    <img v-bind:src="productLogo" @click="onNavigate('/Scenarios/')" class="pointer-for-image" />
                    <v-divider class="mx-2 navbar-divider" vertical color="#798899"/>
                </v-toolbar-title>
                <v-toolbar-items>
                    <v-btn
                        id="App-scenarios-btn"
                        @click="onNavigate('/Scenarios/')"
                        class="ara-blue-pantone-281"
                        flat                        
                    >
                        Scenarios
                    </v-btn>                   
                     <v-btn
                        id="App-libraries-btn"
                        @click="onNavigate('/EditLibrary/')"
                        class="ara-blue-pantone-281"
                        flat
                    >
                        Libraries
                    </v-btn>
                    <v-btn
                        id="App-rawData-btn"
                        @click="onNavigate('/EditRawData/')"
                        class="ara-blue-pantone-281"
                        flat
                        v-if="hasAdminAccess"
                    >
                        Raw Data
                    </v-btn>
                    <v-btn
                        id="App-administration-btn"
                        @click="onNavigate('/EditAdmin/')"
                        class="ara-blue-pantone-281"
                        flat
                        v-if="hasAdminAccess"
                    >
                        Administration
                    </v-btn>
                    <v-btn
                    v-if="stateInventoryReportNames.length > 0"
                        id="App-inventory-btn"
                        @click="onNavigate('/Inventory/')"
                        class="ara-blue-pantone-281"
                        flat
                    >
                        Inventory
                    </v-btn>
                     <v-btn
                        id="App-news-btn"
                        @click="onShowNewsDialog()"
                        class="ara-blue-pantone-281"
                        flat
                    >
                        News
                        <v-icon v-if="hasUnreadNewsItem" size="13" class="news-notification">fas fa-exclamation-circle</v-icon>
                    </v-btn>
                </v-toolbar-items>
                <v-spacer></v-spacer>
                <v-toolbar-title class="white--text">
                    <v-menu
                        offset-y
                        min-width="20%"
                        max-width="20%"
                        max-height="75%"
                        :close-on-content-click="false"
                    >
                        <template v-slot:activator="{ on, attrs }">
                            <button
                                v-on="on"
                                v-bind="attrs"
                                @click="onNotificationMenuSelect"
                                class="notification-icon"
                                icon
                            >
                                <img style="position:absolute; top:20px; height:25px;" :src="require('@/assets/icons/bell.svg')"/>
                                <notification-bell
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
                                />
                            </button>
                        </template>
                        <v-card class="mx-auto" max-width="100%">
                            <v-toolbar color="#002E6C" dark>
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
                                        <v-list-tile>
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
                    <span>{{ username }}</span>
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
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogout"
                        class="mx-2"
                        icon
                        color="#002E6C"
                    >
                        <v-icon small color="white">fas fa-sign-out-alt</v-icon>
                    </v-btn>
                    <v-btn
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
                        <div>BridgeCare &copy; 2021</div>
                        <div>{{ packageVersion }}</div>
                    </div>
                </v-flex>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner />
            <Alert :dialog-data="alertDialogData" @submit="onAlertResult" />
            <NewsDialog :showDialog="showNewsDialog" @close="onCloseNewsDialog()" />
        </v-content>
    </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import NotificationBell from 'vue-notification-bell';
import { Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
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
import { clone } from 'ramda';
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

@Component({
    components: { Alert, Spinner, NotificationBell, NewsDialog },
})
export default class AppComponent extends Vue {
    @State(state => state.authenticationModule.authenticated)
    authenticated: boolean;
    @State(state => state.authenticationModule.hasRole) hasRole: boolean;
    @State(state => state.authenticationModule.username) username: string;
    @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
    @State(state => state.authenticationModule.refreshing) refreshing: boolean;
    @State(state => state.breadcrumbModule.navigation) navigation: any[];
    @State(state => state.notificationModule.notifications)
    notifications: Notification[];
    @State(state => state.notificationModule.counter)
    notificationCounter: number;
    @State(state => state.scenarioModule.selectedScenario)
    stateSelectedScenario: Scenario;
    @State(state => state.announcementModule.packageVersion)
    packageVersion: string;
    @State(state => state.authenticationModule.securityType)
    securityType: string;
    @State(state => state.announcementModule.announcements) announcements: Announcement[];
    @State(state => state.userModule.currentUser) currentUser: User;
    @State(state => state.adminSiteSettingsModule.agencyLogo) agencyLogoBase64: string;
    @State(state => state.adminSiteSettingsModule.productLogo) productLogoBase64: string;
    @State(state => state.adminDataModule.inventoryReportNames) stateInventoryReportNames: string[];
    
    @Action('logOut') logOutAction: any;
    @Action('setIsBusy') setIsBusyAction: any;
    @Action('getNetworks') getNetworksAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('getAnnouncements') getAnnouncementsAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
    @Action('addTaskCompletedNotification') addTaskCompletedNotificationAction: any;
    @Action('removeNotification') removeNotificationAction: any;
    @Action('clearNotificationCounter') clearNotificationCounterAction: any;
    @Action('generatePollingSessionId') generatePollingSessionIdAction: any;
    @Action('getAllUsers') getAllUsersAction: any;
    @Action('getUserCriteriaFilter') getUserCriteriaFilterAction: any;
    @Action('loadNotifications') loadNotificationsActions: any;
    @Action('azureB2CLogin') azureB2CLoginAction: any;
    @Action('azureB2CLogout') azureB2CLogoutAction: any;
    @Action('getCurrentUserByUserName') getCurrentUserByUserNameAction: any;
    @Action('updateUserLastNewsAccessDate') updateUserLastNewsAccessDateAction: any;
    @Action('getAgencyLogo') getAgencyLogoAction: any;
    @Action('getProductLogo') getProductLogoAction: any;
    @Action('getInventoryReports') getInventoryReportsAction: any;

    drawer: boolean = false;
    latestNewsDate: string = '0001-01-01';
    currentUserLastNewsAccessDate: string = '0001-01-01';
    alertDialogData: AlertData = clone(emptyAlertData);
    pushRouteUpdate: boolean = false;
    route: any = {};
    selectedScenario: Scenario = clone(emptyScenario);
    hasSelectedScenario: boolean = false;
    selectedScenarioHasStatus: boolean = false;
    ignoredAPIs: string[] = [
        'SynchronizeLegacySimulation',
        'RunSimulation',
        'GenerateReport',
        'AggregateNetworkData',
        'RefreshToken',
    ];
    esecSecurityType: string = SecurityTypes.esec;
    b2cSecurityType: string = SecurityTypes.b2c;
    showNewsDialog: boolean = false;
    hasUnreadNewsItem: boolean = false;
    currentURL: any = '';
    unauthorizedError: string = '';
    agencyLogo: string = '';
    productLogo: string = '';
    inventoryReportName: string = '';

    get container() {
        const container: any = {};

        if (this.$vuetify.breakpoint.xs) {
            container['grid-list-xs'] = true;
        }

        if (this.$vuetify.breakpoint.sm) {
            container['grid-list-sm'] = true;
        }

        if (this.$vuetify.breakpoint.md) {
            container['grid-list-md'] = true;
        }

        if (this.$vuetify.breakpoint.lg) {
            container['grid-list-lg'] = true;
        }

        if (this.$vuetify.breakpoint.xl) {
            container['grid-list-xl'] = true;
        }

        return container;
    }

    get authenticatedWithRole() {
        return this.authenticated && this.hasRole;
    }

    @Watch('stateSelectedScenario')
    onStateSelectedScenarioChanged() {
        this.selectedScenario = clone(this.stateSelectedScenario);
        this.hasSelectedScenario = this.selectedScenario.id !== getBlankGuid();
        this.selectedScenarioHasStatus = hasValue(this.selectedScenario.status);
    }

    @Watch('authenticated')
    onAuthenticationChange() {
        if (this.authenticated) {
            this.onAuthenticate();
        }
    }

    @Watch('announcements')
    onAnnouncementsChange() {
        this.latestNewsDate = getDateOnly(this.announcements[0].createdDate.toString()); 
    }

    @Watch('currentUser')
    onCurrentUserChange() {
        this.currentUserLastNewsAccessDate = getDateOnly(this.currentUser.lastNewsAccessDate);
        this.checkLastNewsAccessDate();
    }

    @Watch('agencyLogoBase64')
    onAgencyLogoBase64Change() {
        this.agencyLogo = this.agencyLogoBase64;
    }

    @Watch('productLogoBase64')
    onProductLogoBase64Change() {
        this.productLogo = this.productLogoBase64;
    }

    @Watch('stateInventoryReportNames')
        onStateInventoryReportNamesChanged(){
            if(this.stateInventoryReportNames.length > 0)
                this.inventoryReportName = this.stateInventoryReportNames[0]
        }

    created() {
        // create a request handler
        async function requestHandler(
            app: AppComponent,
            request: AxiosRequestConfig,
        ) {
            request.headers = setContentTypeCharset(request.headers);
            if (app.refreshing) {
                await new Promise(_ => setTimeout(_, 5000));
            }

            request.headers = setAuthHeader(request.headers);
            app.setIsBusyAction({
                isBusy: app.ignoredAPIs.every(
                    (ignored: string) => request.url!.indexOf(ignored) === -1,
                ),
            });
            return request;
        }

        // set axios request interceptor to use request handler
        axiosInstance.interceptors.request.use((request: any) =>
            requestHandler(this, request),
        );
        // set nodejs axios request interceptor to use request handler
        nodejsAxiosInstance.interceptors.request.use((request: any) =>
            requestHandler(this, request),
        );
        // set bridge care core axios request interceptor to use request handler
        coreAxiosInstance.interceptors.request.use((request: any) =>
            requestHandler(this, request),
        );
        // create a success & error handler
        const successHandler = (response: AxiosResponse) => {
            response.headers = setContentTypeCharset(response.headers);
            this.setIsBusyAction({ isBusy: false });
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
            this.setIsBusyAction({ isBusy: false });            
            this.unauthorizedError = hasValue(this.unauthorizedError) ? error.response!.data : "User is not authorized!";
            if (error.response!.status === 500) return;
            
            this.addErrorNotificationAction({
                message: error.response!.status === 403 ? "Authorization Failed" : "HTTP Error",
                longMessage: error.response!.status === 403 ? this.unauthorizedError : getErrorMessage(error),
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
            this.securityType === SecurityTypes.esec &&
            UnsecuredRoutePathNames.indexOf(
                this.$router.currentRoute.name as string,
            ) === -1
        ) {
            // Upon opening the page, and every 30 seconds, check if authentication data
            // has been changed by another tab or window
            clearRefreshIntervalID();
            setRefreshIntervalID();
        }
    }

    mounted() {
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            this.onAddErrorNotification,
        );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastWarningEvent,
            this.onAddWarningNotification,
        );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastInfoEvent,
            this.onAddInfoNotification,
        );
        this.$statusHub.$on(
            Hub.BroadcastEventType.BroadcastTaskCompletedEvent,
            this.onAddTaskCompletedNotification
        );
        
        this.currentURL = this.$router.currentRoute.name;

        if(this.$config.agencyLogo.trim() === "")
            this.agencyLogo = require(`@/assets/images/PennDOTLogo.svg`)
        else
            this.agencyLogo = this.$config.agencyLogo

        if(this.$config.productLogo.trim() === "")
            this.productLogo = require(`@/assets/images/BridgeCareLogo.svg`)
        else
            this.productLogo = this.$config.productLogo
    }

    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            this.onAddErrorNotification,
        );
    }

    onAddErrorNotification(data: any) {
        let errorNotification:string = data.error.toString();
        let spl = errorNotification.split('::');
        if (spl.length > 0 ) {
            this.addErrorNotificationAction( {
                message: spl[0],
                longMessage: spl.length>1 ? spl[1] : 'Unknown Error'
            });
        } else {
            this.addErrorNotificationAction( {
                message: 'Server Error',
                longMessage: data.error
            });
        }
    }

    onAddInfoNotification(data: any) {
        this.addInfoNotificationAction({
            message: 'Server Update',
            longMessage: data.info
        });
    }

    onAddWarningNotification(data: any) {
        this.addWarningNotificationAction({
            message: 'Server Warning',
            longMessage: data.info,
        });
    }

    onAddTaskCompletedNotification(data: any) {
        this.addTaskCompletedNotificationAction({
            message: 'Task Completed',
            longMessage: data.task
        });
    }


    onAlertResult(submit: boolean) {
        this.alertDialogData = clone(emptyAlertData);

        if (submit) {
            this.pushRouteUpdate = true;
            this.onNavigate(this.route);
        }
    }

    onAzureLogin() {
        if (this.$router.currentRoute.name === 'AuthenticationStart') {
            this.azureB2CLoginAction();
        } else {
            this.$router.push('/AuthenticationStart');
        }
    }

    onAzureLogout() {
        this.azureB2CLogoutAction().then(() => this.onLogout());
    }
    
    /**
     * Sets up a recurring attempt at refreshing user tokens, and fetches network and attribute data
     */
    onAuthenticate() {
        this.$forceUpdate();
        this.getNetworksAction();
        this.getAttributesAction();
        this.getAllUsersAction();
        this.getAnnouncementsAction();
        this.getUserCriteriaFilterAction();
        if (this.username != null && this.username != '') {
            this.getCurrentUserByUserNameAction(this.username);
        }

        //If these gets are placed before authorization, GetUserInformation() in EsecSecurity.cs will throw an error, as its HttpRequest will have no Authorization header!
        this.getAgencyLogoAction();
        this.getProductLogoAction();
    }

    /**
     * Dispatches an action that will revoke all user tokens, prevents token refresh attempts,
     * and redirects users to the landing page
     */
    onLogout() {
        this.logOutAction().then(() => {
            clearRefreshIntervalID(); 
            if (window.location.host.toLowerCase().indexOf('penndot.gov') === -1) {
                /*
                 * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
                 * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
                 * to allow the landing page to redirect the browser to the original host.
                 */
                window.location.href =
                    'http://www.bamssyst.penndot.gov/iAM?host=' +
                    encodeURI(window.location.host);
            } else {
                this.onNavigate('/iAM/');
            }
        });
    }

    /**
     * Navigates a user to a page using the specified routeName
     * @param route The route name to use when navigating a user
     */
    onNavigate(route: any) {
        if (this.$router.currentRoute.path !== route.path) {
            this.$router.push(route);
        }
    }

    onNotificationMenuSelect() {
        this.clearNotificationCounterAction();
    }

    onRemoveNotification(id: number) {
        this.removeNotificationAction(id);
    }

    onShowNewsDialog() {
        this.showNewsDialog = true;
        this.updateUserLastNewsAccessDateAction({id: this.currentUser.id, accessDate: this.latestNewsDate});
        this.hasUnreadNewsItem = false;
    }

    onCloseNewsDialog() {
        this.showNewsDialog = false;
    }

    checkLastNewsAccessDate () {
        this.hasUnreadNewsItem = newsAccessDateComparison(this.latestNewsDate, this.currentUserLastNewsAccessDate);
    }
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
