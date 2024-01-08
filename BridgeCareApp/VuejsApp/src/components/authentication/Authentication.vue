<template>
    <v-container fluid grid-list-xl>
        <v-row>
            <v-col cols="12">
                <v-row justify="center">
                    <v-card>
                        <v-card-title>
                            <h3>Authenticating...</h3>
                        </v-card-title>
                    </v-card>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
    import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import Vue, { computed, onMounted, ref } from 'vue';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    let authenticated = computed<boolean>(() => store.state.authenticationModule.authenticated);  
    let hasRole= computed<boolean>(() => store.state.authenticationModule.hasRole);
    let hasAdminAccess= computed<boolean>(() => store.state.authenticationModule.hasAdminAccess);
    let currentUserCriteriaFilter= computed<UserCriteriaFilter>(() => store.state.userModule.currentUserCriteriaFilter);
    let securityType: string = store.state.authenticationModule.securityType;

    async function setSuccessMessageAction(payload?: any): Promise<any> {await store.dispatch('setSuccessMessage');}
    async function setErrorMessageAction(payload?: any): Promise<any> {await store.dispatch('setErrorMessage');}
    async function getUserTokensAction(payload?: any): Promise<any> {await store.dispatch('getUserTokens', payload);}
    async function getUserInfoAction(payload?: any): Promise<any> {await store.dispatch('getUserInfo');}
    async function getAzureAccountDetailsAction(payload?: any): Promise<any> {await store.dispatch('getAzureAccountDetails');}
    async function getUserCriteriaFilterAction(payload?: any): Promise<any> {await store.dispatch('getUserCriteriaFilter');}
    async function addErrorNotificationAction(payload?: any): Promise<any> {await store.dispatch('addErrorNotification', payload);}

    const $router = useRouter();

    onMounted(() => mounted());
    function mounted() {
        const code: string = $router.currentRoute.value.query.code as string;
        const state: string = $router.currentRoute.value.query.state as string;

        if (securityType === SecurityTypes.esec) {
            // The ESEC login will always redirect the browser to the iam-deploy site.
            // If the state is set, we know the authentication was started by a local client,
            // and so we should send the browser back to that client.
            if (state === 'localhost8080') {
                window.location.href = `http://localhost:8080/Authentication/?code=${code}`;
                return;
            }

            getUserTokensAction({ code: code }).then(() => {
                if (!authenticated.value) {
                    onAuthenticationFailure();
                } else {
                    getUserInfoAction().then(() => {
                        getUserCriteriaFilterAction().then(() => {
                            if (!hasRole.value || (!currentUserCriteriaFilter.value.hasAccess && !hasAdminAccess.value)) {
                                onRoleFailure();
                            } else {
                                onAuthenticationSuccess();
                            }
                        });
                    });
                }
            });
        }

        //Is this deprecated???
        if (securityType === SecurityTypes.b2c) {
            getAzureAccountDetailsAction();
            if (!authenticated.value) {
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
