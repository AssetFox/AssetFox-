<template>
    <v-container fluid grid-list-xl>
        <v-row>
            <v-flex xs12>
                <v-row justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authenticating...</h3>
                        </v-card-title>
                    </v-card>
                </v-row>
            </v-flex>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
    import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import Vue, { onMounted, ref } from 'vue';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    let authenticated : boolean = store.state.authenticationModule.authenticated;  
    let hasRole: boolean = store.state.authenticationModule.hasRole;
    let hasAdminAccess: boolean = store.state.authenticationModule.hasAdminAccess;
    let currentUserCriteriaFilter: UserCriteriaFilter = store.state.userModule.currentUserCriteriaFilter;
    let securityType: string = store.state.authenticationModule.securityType;

    async function setSuccessMessageAction(payload?: any): Promise<any> {await store.dispatch('setSuccessMessage');}
    async function setErrorMessageAction(payload?: any): Promise<any> {await store.dispatch('setErrorMessage');}
    async function getUserTokensAction(payload?: any): Promise<any> {await store.dispatch('getUserTokens');}
    async function getUserInfoAction(payload?: any): Promise<any> {await store.dispatch('getUserInfo');}
    async function getAzureAccountDetailsAction(payload?: any): Promise<any> {await store.dispatch('getAzureAccountDetails');}
    async function getUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('getUserCriteriaFilter');}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification');}

    const $router = useRouter();

    onMounted(() => mounted);
    function mounted() {
        const code: string = $router.currentRoute.value.query.code as string;
        const state: string = $router.currentRoute.value.query.state as string;

        if (securityType === SecurityTypes.esec) {
            // The ESEC login will always redirect the browser to the iam-deploy site.
            // If the state is set, we know the authentication was started by a local client,
            // and so we should send the browser back to that client.
            if (state === 'localhost' + import.meta.env.PORT) {
                window.location.href = `http://localhost:${import.meta.env.PORT}/Authentication/?code=${code}`;
                return;
            }

            getUserTokensAction(code).then(() => {
                if (!authenticated) {
                    onAuthenticationFailure();
                } else {
                    getUserInfoAction().then(() => {
                        getUserCriteriaFilterAction().then(() => {
                            if (!hasRole || (!currentUserCriteriaFilter.hasAccess && !hasAdminAccess)) {
                                onRoleFailure();
                            } else {
                                onAuthenticationSuccess();
                            }
                        });
                    });
                }
            });
        }

        if (securityType === SecurityTypes.b2c) {
            getAzureAccountDetailsAction();
            if (!authenticated) {
                onAuthenticationFailure();
            } else {
                onAuthenticationSuccess();
            }
        }
    }

    function onAuthenticationSuccess() {
        $router.push('/Scenarios/');
    }

    function onAuthenticationFailure() {
        addErrorNotificationAction({ message: 'Authentication failed.' });
        $router.push('/AuthenticationFailure/');
    }

    function onRoleFailure() {
        $router.push('/NoRole/');
    }
</script>
