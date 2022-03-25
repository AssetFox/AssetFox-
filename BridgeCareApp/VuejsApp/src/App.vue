<template>
    <v-app class="paper-white-bg">
        <v-content>
            <v-toolbar app class="paper-white-bg">
                <v-toolbar-title>
                    <img :src="require('@/assets/images/PennDOTLogo.svg')" />
                    <v-divider class="mx-2 navbar-divider" vertical color="#798899"/>
                    <img :src="require('@/assets/images/BridgeCareLogo.svg')" />
                    <v-divider class="mx-2 navbar-divider" vertical color="#798899"/>
                </v-toolbar-title>
                <v-toolbar-items>
                    <v-btn
                        @click="onNavigate('/Scenarios/')"
                        flat
                        class="ara-blue-pantone-281"
                    >
                        Scenarios
                    </v-btn>
                    <v-menu offset-y>
                            <template v-slot:activator="{ on, attrs }">
                                <v-btn flat v-on="on" v-bind="attrs" class="ara-blue-pantone-281">
                                    Libraries
                                </v-btn>
                            </template>
                        <v-list>    
                            <v-list-item-group class="ara-blue-pantone-281">
                                <v-list-tile
                                    @click="onNavigate('/InvestmentEditor/Library/')"
                                >
                                    <v-list-tile-title>Investment</v-list-tile-title>
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate('/PerformanceCurveEditor/Library/')
                                    "
                                >
                                    <v-list-tile-title
                                        >Performance Curve</v-list-tile-title
                                    >
                                </v-list-tile>
                                <v-list-tile
                                    @click="onNavigate('/TreatmentEditor/Library/')"
                                >
                                    <v-list-tile-title>Treatment</v-list-tile-title>
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate('/BudgetPriorityEditor/Library/')
                                    "
                                >
                                    <v-list-tile-title
                                        >Budget Priority</v-list-tile-title
                                    >
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate(
                                            '/TargetConditionGoalEditor/Library/',
                                        )
                                    "
                                >
                                    <v-list-tile-title
                                        >Target Condition Goal</v-list-tile-title
                                    >
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate(
                                            '/DeficientConditionGoalEditor/Library/',
                                        )
                                    "
                                >
                                    <v-list-tile-title
                                        >Deficient Condition Goal</v-list-tile-title
                                    >
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate('/RemainingLifeLimitEditor/Library/')
                                    "
                                    v-if="isAdmin"
                                >
                                    <v-list-tile-title
                                        >Remaining Life Limit</v-list-tile-title
                                    >
                                </v-list-tile>
                                <v-list-tile
                                    @click="onNavigate('/CashFlowEditor/Library/')"
                                >
                                    <v-list-tile-title>Cash Flow</v-list-tile-title>
                                </v-list-tile>
                                <v-list-tile
                                    @click="
                                        onNavigate('/CriterionLibraryEditor/Library/')
                                    "
                                >
                                    <v-list-tile-title>Criterion</v-list-tile-title>
                                </v-list-tile>
                                <v-list-tile
                                    v-show="isAdmin"
                                    @click="
                                        onNavigate(
                                            '/CalculatedAttributeEditor/Library/',
                                        )
                                    "
                                >
                                    <v-list-tile-title
                                        >Calculated Attribute</v-list-tile-title
                                    >
                                </v-list-tile>
                            </v-list-item-group>
                        </v-list> 
                    </v-menu>
                    <v-btn
                        @click="onNavigate('/UserCriteria/')"
                        class="ara-blue-pantone-281"
                        flat
                        v-if="isAdmin"
                    >
                        Security
                    </v-btn>
                    <v-btn
                        @click="onNavigate('/Inventory/')"
                        class="ara-blue-pantone-281"
                        flat
                    >
                        Inventory
                    </v-btn>
                     <v-btn
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
                            >
                                <notification-bell
                                    :size="30"
                                    :count="notificationCounter"
                                    :upperLimit="50"
                                    counterLocation="right"
                                    fontSize="10px"
                                    counterStyle="roundRectangle"
                                    counterBackgroundColor="#FF0000"
                                    counterTextColor="#FFFFFF"
                                    iconColor="#002E6C"
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
                <v-toolbar-title class="navbar-gray" v-if="authenticated">
                    <v-icon class="navbar-user-icon">fas fa-user</v-icon>
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
                        v-if="securityType === esecSecurityType"
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
            <div class="ara-blue-pantone-281 scenario-status" v-if="hasSelectedScenario">
                <span class="font-weight-light">Scenario: </span>
                    <span>{{ selectedScenario.name }}</span>
                    <span
                        v-if="selectedScenarioHasStatus"
                        class="font-weight-light"
                    >
                        => Status:
                    </span>
                    <span v-if="selectedScenarioHasStatus">{{
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
//import ReportsService from './services/reports.service';
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
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
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

    @Action('logOut') logOutAction: any;
    @Action('setIsBusy') setIsBusyAction: any;
    @Action('getNetworks') getNetworksAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('getAnnouncements') getAnnouncementsAction: any;
    @Action('addSuccessNotification') addSuccessNotificationAction: any;
    @Action('addWarningNotification') addWarningNotificationAction: any;
    @Action('addErrorNotification') addErrorNotificationAction: any;
    @Action('addInfoNotification') addInfoNotificationAction: any;
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
        'GenerateSummaryReport',
        'AggregateNetworkData',
        'RefreshToken',
    ];
    esecSecurityType: string = SecurityTypes.esec;
    b2cSecurityType: string = SecurityTypes.b2c;
    showNewsDialog: boolean = false;
    hasUnreadNewsItem: boolean = false;

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
        } /* else if (
            !this.authenticated &&
            this.securityType === SecurityTypes.esec
        ) {
            this.onLogout();
        } else if (
            !this.authenticated &&
            this.securityType === SecurityTypes.b2c
        ) {
            this.onAzureLogout();
        }*/
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
            this.addErrorNotificationAction({
                message: 'HTTP Error',
                longMessage: getErrorMessage(error),
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
    }

    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            this.onAddErrorNotification,
        );
    }

    onAddErrorNotification(data: any) {
        this.addErrorNotificationAction({
            message: 'Server Error.',
            longMessage: data.error,
        });
    }

    onAddWarningNotification(data: any) {
        this.addWarningNotificationAction({
            message: 'Server Warning.',
            longMessage: data.warning,
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
        this.getCurrentUserByUserNameAction(this.username);
    }

    /**
     * Dispatches an action that will revoke all user tokens, prevents token refresh attempts,
     * and redirects users to the landing page
     */
    onLogout() {
        this.logOutAction().then(() => {
            clearRefreshIntervalID();
            if (
                window.location.host.toLowerCase().indexOf('penndot.gov') === -1
            ) {
                /*
                 * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
                 * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
                 * to allow the landing page to redirect the browser to the original host.
                 */
                window.location.href =
                    'http://bamssyst.penndot.gov/iAM?host=' +
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

</style>
