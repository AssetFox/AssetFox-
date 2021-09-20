<template>
    <v-app class="paper-white-bg">
        <v-content>
            <v-navigation-drawer
                :disable-resize-watcher="true"
                app
                class="paper-white-bg"
                v-if="authenticatedWithRole"
                v-model="drawer"
            >
                <v-list class="pt-0" dense>
                    <v-list-tile
                        @click="
                            drawer = false;
                            onNavigate('/Home/');
                        "
                    >
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray"
                                >fas fa-newspaper</v-icon
                            >
                        </v-list-tile-action>
                        <v-list-tile-title>Home</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Inventory/')">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray"
                                >fas fa-archive</v-icon
                            >
                        </v-list-tile-action>
                        <v-list-tile-title>Inventory</v-list-tile-title>
                    </v-list-tile>
                    <v-list-tile @click="onNavigate('/Scenarios/')">
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray"
                                >fas fa-project-diagram</v-icon
                            >
                        </v-list-tile-action>
                        <v-list-tile-title>Scenarios</v-list-tile-title>
                    </v-list-tile>
                    <v-list-group prepend-icon="fas fa-book">
                        <template slot="activator">
                            <v-list-tile>
                                <v-list-tile-title>Libraries</v-list-tile-title>
                            </v-list-tile>
                        </template>
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
                    </v-list-group>
                    <v-list-tile
                        @click="onNavigate('/UserCriteria/')"
                        v-if="isAdmin"
                    >
                        <v-list-tile-action>
                            <v-icon class="ara-dark-gray">fas fa-lock</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-title>Security</v-list-tile-title>
                    </v-list-tile>
                </v-list>
            </v-navigation-drawer>
            <v-toolbar app class="ara-blue-pantone-289-bg">
                <v-toolbar-side-icon
                    @click="drawer = !drawer"
                    class="white--text"
                    v-if="
                        authenticatedWithRole &&
                            $router.currentRoute.name !== 'Home'
                    "
                ></v-toolbar-side-icon>
                <v-toolbar-title
                    class="white--text"
                    v-if="
                        authenticatedWithRole &&
                            $router.currentRoute.name === 'Home'
                    "
                >
                    <v-btn
                        @click="onNavigate('/Inventory/')"
                        class="ara-blue-bg white--text"
                        round
                    >
                        <v-icon style="padding-right: 12px"
                            >fas fa-archive</v-icon
                        >
                        Inventory Lookup
                    </v-btn>
                    <v-btn
                        @click="onNavigate('/Scenarios/')"
                        class="ara-blue-bg white--text"
                        round
                    >
                        <v-icon style="padding-right: 12px"
                            >fas fa-project-diagram</v-icon
                        >
                        BridgeCare Analysis
                    </v-btn>
                    <v-btn
                        @click="onNavigate('/UserCriteria/')"
                        class="ara-blue-bg white--text"
                        round
                        v-if="isAdmin"
                    >
                        <v-icon style="padding-right: 12px">fas fa-lock</v-icon>
                        Security
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="hasSelectedScenario">
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
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <span class="font-weight-light">Hello, </span>
                    <span>{{ username }}</span>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="!authenticated">
                    <v-btn
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogin"
                        class="ara-blue-bg white--text"
                        round
                    >
                        Log In
                    </v-btn>
                    <v-btn
                        v-if="securityType === esecSecurityType"
                        @click="onNavigate('/AuthenticationStart/')"
                        class="ara-blue-bg white--text"
                        round
                    >
                        Log In
                    </v-btn>
                </v-toolbar-title>
                <v-toolbar-title class="white--text" v-if="authenticated">
                    <v-btn
                        v-if="securityType === b2cSecurityType"
                        @click="onAzureLogout"
                        class="ara-blue-bg white--text"
                        round
                    >
                        Log Out
                    </v-btn>
                    <v-btn
                        v-if="securityType === esecSecurityType"
                        @click="onLogout"
                        class="ara-blue-bg white--text"
                        round
                    >
                        Log Out
                    </v-btn>
                </v-toolbar-title>
            </v-toolbar>
            <v-container fluid v-bind="container">
                <router-view></router-view>
            </v-container>
            <v-footer app class="ara-blue-pantone-289-bg white--text" fixed>
                <v-spacer></v-spacer>
                <v-flex xs2>
                    <div class="dev-and-ver-div">
                        <div class="font-weight-light">iAM</div>
                        <div>BridgeCare &copy; 2019</div>
                        <div>{{ packageVersion }}</div>
                    </div>
                </v-flex>
                <v-spacer></v-spacer>
            </v-footer>
            <Spinner />
            <Alert :dialog-data="alertDialogData" @submit="onAlertResult" />
        </v-content>
    </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import Spinner from './shared/modals/Spinner.vue';
import iziToast from 'izitoast';
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
import { Hub } from '@/connectionHub';
import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
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

@Component({
    components: { Alert, Spinner },
})
export default class AppComponent extends Vue {
    @State(state => state.authenticationModule.authenticated)
    authenticated: boolean;
    @State(state => state.authenticationModule.hasRole) hasRole: boolean;
    @State(state => state.authenticationModule.username) username: string;
    @State(state => state.authenticationModule.isAdmin) isAdmin: boolean;
    @State(state => state.authenticationModule.refreshing) refreshing: boolean;
    @State(state => state.breadcrumbModule.navigation) navigation: any[];
    @State(state => state.toastrModule.successMessage) successMessage: string;
    @State(state => state.toastrModule.warningMessage) warningMessage: string;
    @State(state => state.toastrModule.errorMessage) errorMessage: string;
    @State(state => state.toastrModule.infoMessage) infoMessage: string;
    @State(state => state.scenarioModule.selectedScenario)
    stateSelectedScenario: Scenario;
    @State(state => state.announcementModule.packageVersion)
    packageVersion: string;
    @State(state => state.authenticationModule.securityType)
    securityType: string;

    @Action('logOut') logOutAction: any;
    @Action('setIsBusy') setIsBusyAction: any;
    @Action('getNetworks') getNetworksAction: any;
    @Action('getAttributes') getAttributesAction: any;
    @Action('setSuccessMessage') setSuccessMessageAction: any;
    @Action('setWarningMessage') setWarningMessageAction: any;
    @Action('setErrorMessage') setErrorMessageAction: any;
    @Action('setInfoMessage') setInfoMessageAction: any;
    @Action('generatePollingSessionId') generatePollingSessionIdAction: any;
    @Action('getAllUsers') getAllUsersAction: any;
    @Action('getUserCriteriaFilter') getUserCriteriaFilterAction: any;

    @Action('azureB2CLogin') azureB2CLoginAction: any;
    @Action('azureB2CLogout') azureB2CLogoutAction: any;

    drawer: boolean = false;
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

    @Watch('successMessage')
    onSuccessMessageChanged() {
        if (hasValue(this.successMessage)) {
            iziToast.success({
                title: 'Success',
                message: this.successMessage,
                position: 'topRight',
                closeOnClick: true,
                timeout: 3000,
            });
            this.setSuccessMessageAction({ message: '' });
        }
    }

    @Watch('warningMessage')
    onWarningMessageChanged() {
        if (hasValue(this.warningMessage)) {
            iziToast.warning({
                title: 'Warning',
                message: this.warningMessage,
                position: 'topRight',
                closeOnClick: true,
                timeout: 5000,
            });
            this.setWarningMessageAction({ message: '' });
        }
    }

    @Watch('errorMessage')
    onErrorMessageChanged() {
        if (hasValue(this.errorMessage)) {
            iziToast.error({
                title: 'Error',
                message: this.errorMessage,
                position: 'topRight',
                closeOnClick: true,
                timeout: false,
            });
            this.setErrorMessageAction({ message: '' });
        }
    }

    @Watch('infoMessage')
    onInfoMessageChanged() {
        if (hasValue(this.infoMessage)) {
            iziToast.info({
                title: 'Info',
                message: this.infoMessage,
                position: 'topRight',
                closeOnClick: true,
                timeout: 5000,
            });
            this.setInfoMessageAction({ message: '' });
        }
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
            this.setErrorMessageAction({ message: getErrorMessage(error) });
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
            this.onSetErrorMessage,
        );
    }

    beforeDestroy() {
        this.$statusHub.$off(
            Hub.BroadcastEventType.BroadcastErrorEvent,
            this.onSetErrorMessage,
        );
    }

    onSetErrorMessage(data: any) {
        this.setErrorMessageAction({ message: data.error });
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
        this.getUserCriteriaFilterAction();
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
}
</script>

<style>
html {
    overflow: auto;
    overflow-x: hidden;
    overflow-y: scroll !important;
}

.v-list__group__header__prepend-icon .v-icon {
    color: #798899 !important;
}

.v-list__group__header__prepend-icon .primary--text .v-icon {
    color: #008fca;
}

.dev-and-ver-div {
    display: flex;
    justify-content: space-evenly;
}
</style>
