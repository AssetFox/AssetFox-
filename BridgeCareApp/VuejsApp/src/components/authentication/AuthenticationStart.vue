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
                        <v-btn id="AuthenticationStart-btn-goToLoginPage"
                               v-if="securityType === esecSecurityType" 
                               @click="onRedirect" class="v-btn theme--light ara-blue-bg white--text">
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
    import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import { hasValue } from '@/shared/utils/has-value-util';
    import { isAuthenticatedUser,
    } from '@/shared/utils/authentication-utils';

    @Component
    export default class AuthenticationStart extends Vue {
        @State(state => state.authenticationModule.authenticated) authenticated: boolean;
        @State(state => state.authenticationModule.hasRole) hasRole: boolean;
        @State(state => state.authenticationModule.checkedForRole) checkedForRole: boolean;
        @State(state => state.authenticationModule.securityType) securityType: string;
        @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;

        @Action('azureB2CLogin') azureB2CLoginAction: any;

        esecSecurityType: string = SecurityTypes.esec;
        b2cSecurityType: string = SecurityTypes.b2c;

        mounted() {
            const hasAuthInfo: boolean =
                this.securityType === SecurityTypes.esec
                    ? hasValue(localStorage.getItem('UserTokens'))
                    : hasValue(localStorage.getItem('LoggedInUser'));
            if (!this.authenticated && hasAuthInfo) {
                isAuthenticatedUser()
                    .then((isAuthenticated: boolean | void) => {
                        if (isAuthenticated) {
                            this.$router.push('/Scenarios/')
                        }
                    });
            } else if (this.authenticated && hasAuthInfo) {
                this.$router.push('/Scenarios/');
            } else {
                if (this.securityType === SecurityTypes.esec) {
                    this.onRedirect();
                } else if (this.securityType === SecurityTypes.b2c) {
                    this.azureB2CLoginAction();
                }
            }
        }

        onRedirect() { 
            let href: string = `${this.$config.authorizationEndpoint}?response_type=code&scope=openid&scope=BAMS`;
            href += `&client_id=${this.$config.clientId}`;
            href += `&redirect_uri=${this.$config.redirectUri}`;

            // The 'state' query parameter that is sent to ESEC will be sent back to
            // the /Authentication page of the iam-deploy app.
            if (process.env.VUE_APP_IS_PRODUCTION !== 'true') {
                href += '&state=localhost' + process.env.PORT;
            }

            window.location.href = href;
        }
    }
</script>
