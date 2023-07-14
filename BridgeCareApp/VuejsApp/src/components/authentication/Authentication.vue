<template>
    <v-container fluid grid-list-xl>
        <v-layout>
            <v-flex xs12>
                <v-layout justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authenticating...</h3>
                        </v-card-title>
                    </v-card>
                </v-layout>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
    import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import Vue from 'vue';
    import {Component} from 'vue-property-decorator';
    import {Action, State} from 'vuex-class';
    import { SecurityTypes } from '@/shared/utils/security-types';

    @Component
    export default class Authentication extends Vue {
        @State(state => state.authenticationModule.authenticated) authenticated: boolean;
        @State(state => state.authenticationModule.hasRole) hasRole: boolean;
        @State(state => state.authenticationModule.hasAdminAccess) hasAdminAccess: boolean;
        @State(state => state.userModule.currentUserCriteriaFilter) currentUserCriteriaFilter: UserCriteriaFilter;
        @State(state => state.authenticationModule.securityType) securityType: string;

        @Action('setSuccessMessage') setSuccessMessageAction: any;
        @Action('setErrorMessage') setErrorMessageAction: any;
        @Action('getUserTokens') getUserTokensAction: any;
        @Action('getUserInfo') getUserInfoAction: any;
        @Action('getAzureAccountDetails') getAzureAccountDetailsAction: any;
        @Action('getUserCriteriaFilter') getUserCriteriaFilterAction: any;
        @Action('addErrorNotification') addErrorNotificationAction: any;

        mounted() {
            const code: string = this.$route.query.code as string;
            const state: string = this.$route.query.state as string;

            if (this.securityType === SecurityTypes.esec) {
                // The ESEC login will always redirect the browser to the iam-deploy site.
                // If the state is set, we know the authentication was started by a local client,
                // and so we should send the browser back to that client.
                if (state === 'localhost' + process.env.PORT) {
                    window.location.href = `http://localhost:${process.env.PORT}/Authentication/?code=${code}`;
                    return;
                }

                this.getUserTokensAction(code).then(() => {
                    if (!this.authenticated) {
                        this.onAuthenticationFailure();
                    } else {
                        this.getUserInfoAction().then(() => {
                            this.getUserCriteriaFilterAction().then(() => {
                                if (!this.hasRole || (!this.currentUserCriteriaFilter.hasAccess && !this.hasAdminAccess)) {
                                    this.onRoleFailure();
                                } else {
                                    this.onAuthenticationSuccess();
                                }
                            });
                        });
                    }
                });
            }

        if (this.securityType === SecurityTypes.b2c) {
            this.getAzureAccountDetailsAction();
            if (!this.authenticated) {
                this.onAuthenticationFailure();
            } else {
                this.onAuthenticationSuccess();
            }
        }
    }

    onAuthenticationSuccess() {
        this.$router.push('/Scenarios/');
    }

    onAuthenticationFailure() {
        this.addErrorNotificationAction({ message: 'Authentication failed.' });
        this.$router.push('/AuthenticationFailure/');
    }

    onRoleFailure() {
        this.$router.push('/NoRole/');
    }
}
</script>
