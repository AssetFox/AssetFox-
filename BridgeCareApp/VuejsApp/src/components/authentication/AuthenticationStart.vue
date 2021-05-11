<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Beginning Authentication</h3>
                        </v-card-title>
                        <v-card-text v-if="securityType === 'ESEC'">
                            You should be redirected to the PennDOT login page shortly. If you are not redirected within
                            5 seconds, press the button below.
                        </v-card-text>
                        <v-btn v-if="securityType === 'ESEC'" @click="onRedirect" class="v-btn theme--light ara-blue-bg white--text">
                            Go to login page
                        </v-btn>
                        <v-card-text v-if="securityType === 'B2C'">
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
    import {Component, Watch} from 'vue-property-decorator';
    import {State, Action} from 'vuex-class';
    import oidcConfig from '@/oidc-config';

    @Component
    export default class AuthenticationStart extends Vue {
        @State(state => state.authenticationModule.authenticated) authenticated: boolean;
        @State(state => state.authenticationModule.hasRole) hasRole: boolean;
        @State(state => state.authenticationModule.checkedForRole) checkedForRole: boolean;
        @State(state => state.authenticationModule.securityType) securityType: string;

        @Action('azureB2CLogin') azureB2CLoginAction: any;

        @Watch('checkedForRole')
        onCheckedRole() {
            if (this.checkedForRole && this.securityType === 'ESEC') {
                if (this.hasRole) {
                    this.$router.push('/Scenarios/');
                } else {
                    this.$router.push('/NoRole/');
                }
            }
        }

        @Watch('authenticated')
        onAuthenticatedChanged() {
            if (this.authenticated) {
                this.$router.push('/Scenarios/');
            }
        }

        mounted() {
            if (this.securityType === 'ESEC') {
                this.onRedirect();
            } else if (this.securityType === 'B2C') {
                this.onAzureLogin();
            }
        }

        onRedirect() {
            if (!this.authenticated) {
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
        }

        onAzureLogin() {
            if (!this.authenticated) {
                this.azureB2CLoginAction();
            } else if (this.authenticated) {
                this.$router.push('/Scenarios/');
            }
        }
    }
</script>
