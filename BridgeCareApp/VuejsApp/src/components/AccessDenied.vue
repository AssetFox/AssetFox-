<template>
    <v-container fluid grid-list-xl>
        <v-row>
            <v-col cols = "12">
                <v-row justify="center">
                    <v-card class="access-denied-card">
                        <div class="bridgecare-logo-img-div">
                            <v-img :src="getUrl('assets/images/logos/Banner-logo.jpg')"
                                   class="bridgecare-logo-img">
                            </v-img>
                        </div>
                        <v-card-title style="justify-content: center; text-align: center">
                            <h3>Access Denied</h3>
                        </v-card-title>
                        <v-card-text style="justify-content: center; text-align: center">
                            <p>This account has been deactivated. Please contact your administrator for more information.</p>
                        </v-card-text>
                    </v-card>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script lang="ts" setup>
    import Vue from 'vue';
    import { hasValue } from '@/shared/utils/has-value-util';
    import { SecurityTypes } from '@/shared/utils/security-types';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    import { getUrl } from '@/shared/utils/get-url';

    const $router = useRouter();
    let store = useStore();
    let securityType = ref<string>(store.state.authenticationModule.securityType);
    onMounted(()=>mounted())
    function mounted() {
            if (securityType === SecurityTypes.esec) {
                /*
                 * The /iAM/ pages of the penndot deployments fail to set the cookie until they have been refreshed.
                 */
                if (!window.location.hash) {
                    window.location.hash = 'refreshed';
                    window.location.reload();
                }

                if (!hasValue($router.currentRoute.value.query.host)) {
                    return;
                }
                /*
                 * In order to log out properly, the browser must visit the landing page of a penndot deployment, as iam-deploy.com cannot
                 * modify browser cookies for penndot.gov. So, if the browser was sent here from another host, redirect back to the landing
                 * page of that host without the 'host' query string.
                 */
                const host: string = $router.currentRoute.value.query.host as string;
                if (host !== window.location.host) {
                    window.location.href = 'http://' + host + '/iAM';
                }
            }
        }
</script>

<style>
    .access-denied-card {
        width: 45%;
    }

    .bridgecare-logo-img-div {
        width: 100%;
    }

    .bridgecare-logo-img {
        width: 100%;
        border-bottom-style: solid;
        border-color: #008FCA;
    }
</style>
