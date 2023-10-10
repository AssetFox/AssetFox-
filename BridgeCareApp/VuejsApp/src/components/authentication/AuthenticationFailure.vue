<template>
    <v-container fluid grid-list-xl>
        <v-row>
            <v-flex xs12>
                <v-row justify-center>
                    <v-card>
                        <v-card-title>
                            <h3>Authentication Failed</h3>
                        </v-card-title>
                        <v-btn @click="onAttemptAuthentication" class="v-btn theme--light ara-blue-bg text-white">
                            Try Again
                        </v-btn>
                    </v-card>
                </v-row>
            </v-flex>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
    import Vue from 'vue';
import { useRouter } from 'vue-router';
    const $router = useRouter();
    function onAttemptAuthentication() {
        $router.push('/AuthenticationStart/');
    }
    
</script>
