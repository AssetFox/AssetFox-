<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Beginning Authentication</h3>
                        </v-card-title>
                        <v-card-text v-if="securityType === esecSecurityType">
                            You should be redirected to the PennDOT login page shortly. If you are not redirected within
                            5 seconds, press the button below.
                        </v-card-text>
                        <v-btn v-if="securityType === esecSecurityType" @click="onRedirect" class="v-btn theme--light ara-blue-bg white--text">
                            Go to login page
                        </v-btn>
                        <v-card-text v-if="securityType === b2cSecurityType">
                            Please click 'Login' if the login pop-up does not show within ~5 seconds.
                        </v-card-text>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import oidcConfig from '@/oidc-config';
    import { emptyUserCriteriaFilter, UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import { isEqual } from '@/shared/utils/has-unsaved-changes-helper';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import { hasValue } from '@/shared/utils/has-value-util';

    @Component
    export default class AuthenticationStart extends Vue {
        @State(state => state.authenticationModule.authenticated) authenticated: boolean;
        @State(state => state.authenticationModule.hasRole) hasRole: boolean;
        @State(state => state.authenticationModule.checkedForRole) checkedForRole: boolean;
        @State(state => state.authenticationModule.securityType) securityType: string;
        @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;

        @Action('azureB2CLogin') azureB2CLoginAction: any;
        @Action('getUserInfo') getUserInfoAction: any;
        @Action('getUserCriteriaFilter') getUserCriteriaFilterAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('azureB2CLogout') azureB2CLogoutAction: any;
        @Action('logOut') logOutAction: any;
        @Action('checkBrowserTokens') checkBrowserTokensAction: any;

        esecSecurityType: string = SecurityTypes.esec;
        b2cSecurityType: string = SecurityTypes.b2c;

        mounted() {
            const hasAuthInfo: boolean =
                this.securityType === SecurityTypes.esec
                    ? hasValue(localStorage.getItem('UserTokens'))
                    : hasValue(localStorage.getItem('LoggedInUser'));
            if (!this.authenticated && hasAuthInfo) {
                if (this.securityType === SecurityTypes.esec) {
                    this.onCheckEsecToken();
                } else {
                    this.onCheckAzureToken();
                }
            } else if (this.authenticated && hasAuthInfo) {
                this.$router.push('/Home/');
            } else {
                if (this.securityType === SecurityTypes.esec) {
                    this.onRedirect();
                } else if (this.securityType === SecurityTypes.b2c) {
                    this.azureB2CLoginAction();
                }
            }
        }

        onCheckEsecToken() {
            this.checkBrowserTokensAction()
                .then(() =>
                    this.getUserInfoAction().then(() =>
                        this.getUserCriteriaFilterAction().then(() => {
                            if (this.authenticated) {
                                this.$router.push('/Home/');
                            } else {
                                throw new Error(
                                    'Failed to authenticate',
                                );
                            }
                        }),
                    ),
                )
                .catch((error: any) => {
                    this.setErrorMessageAction({ message: error });
                    this.onLogout();
                });
        }

        onCheckAzureToken() {

        }

        onRedirect() {
            let href: string = `${oidcConfig.authorizationEndpoint}?response_type=code&scope=openid&scope=BAMS`;
            href += `&client_id=${oidcConfig.clientId}`;
            href += `&redirect_uri=${oidcConfig.redirectUri}`;

            // The 'state' query parameter that is sent to ESEC will be sent back to
            // the /Authentication page of the iam-deploy app.
            if (process.env.VUE_APP_IS_PRODUCTION !== 'true') {
                href += '&state=localhost8080';
            }

            window.location.href = href;
        }

        /**
         * Dispatches an action that will revoke all user tokens, prevents token refresh attempts,
         * and redirects users to the landing page
         */
        onLogout() {
            this.logOutAction().then(() => {
                if (window.location.host.toLowerCase().indexOf('penndot.gov') === -1) {
                    /*
                     * In order to log out properly, the browser must visit the /iAM page of a penndot deployment, as iam-deploy.com cannot
                     * modify browser cookies for penndot.gov. So, the current host is sent as part of the query to the penndot site
                     * to allow the landing page to redirect the browser to the original host.
                     */
                    window.location.href = 'http://bamssyst.penndot.gov/iAM?host=' + encodeURI(window.location.host);
                } else {
                    this.$router.push('/iAM/');
                }
            });
        }

        onAzureLogout() {
            this.azureB2CLogoutAction()
                .then(() => this.onLogout());
        }
    }
</script>
