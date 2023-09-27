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
                        <v-btn id="AuthenticationStart-goToLoginPage-btn"
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

<script setup lang="ts">
    import Vue, { inject, onMounted } from 'vue';
    import { UserCriteriaFilter } from '@/shared/models/iAM/user-criteria-filter';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import { hasValue } from '@/shared/utils/has-value-util';
    import { isAuthenticatedUser,
    } from '@/shared/utils/authentication-utils';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';

    let store = useStore();
    let authenticated: boolean = (store.state.authenticationModule.authenticated);
    let hasRole: boolean = (store.state.authenticationModule.hasRole) ;
    let checkedForRole: boolean = (store.state.authenticationModule.checkedForRole) ;
    let securityType: string = (store.state.authenticationModule.securityType) ;
    let currentUserCriteriaFilter: UserCriteriaFilter = (store.state.userModule.currentUserCriteriaFilter) ;

    async function azureB2CLoginAction(payload?: any): Promise<any> { await store.dispatch('azureB2CLogin');} 

    let esecSecurityType: string = SecurityTypes.esec;
    let b2cSecurityType: string = SecurityTypes.b2c;

    const $router = useRouter();
    const $config = inject('$config') as any

    onMounted(() => mounted)
    function mounted() {
        const hasAuthInfo: boolean =
            securityType === SecurityTypes.esec
                ? hasValue(localStorage.getItem('UserTokens'))
                : hasValue(localStorage.getItem('LoggedInUser'));
        if (!authenticated && hasAuthInfo) {
            isAuthenticatedUser()
                .then((isAuthenticated: boolean | void) => {
                    if (isAuthenticated) {
                        $router.push('/Scenarios/')
                    }
                });
        } else if (authenticated && hasAuthInfo) {
            $router.push('/Scenarios/');
        } else {
            if (securityType === SecurityTypes.esec) {
                onRedirect();
            } else if (securityType === SecurityTypes.b2c) {
                azureB2CLoginAction();
            }
        }
    }

    function onRedirect() { 
        let href: string = `${$config.authorizationEndpoint}?response_type=code&scope=openid&scope=BAMS`;
        href += `&client_id=${$config.clientId}`;
        href += `&redirect_uri=${$config.redirectUri}`;

        // The 'state' query parameter that is sent to ESEC will be sent back to
        // the /Authentication page of the iam-deploy app.
        if (process.env.VUE_APP_IS_PRODUCTION !== 'true') {
            href += '&state=localhost' + process.env.PORT;
        }

        window.location.href = href;
    }
</script>
