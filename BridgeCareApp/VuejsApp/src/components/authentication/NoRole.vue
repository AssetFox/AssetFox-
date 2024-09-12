<template>
    <v-container fluid grid-list-xl>
        <v-row justify="center" class="mt-10"> 
            <v-col cols="12" md="6">  
                <v-row justify="center">
                    <v-card class="no-role-container">
                        <div class="bridgecare-logo-img-div">
                            <v-img :src="getUrl('assets/images/logos/Banner-logo.jpg')" class="bridgecare-logo-img">
                            </v-img>
                        </div>
                        <v-card-title class="text-center">  
                            <h3>Service Not Available</h3>
                        </v-card-title>
                        <v-card-text style="justify-content: center; text-align: center">
                            <p>You do not have access to the iAM PAMS App.</p>
                            <p>You can request access at:</p>
                            <a :href="'mailto:' + adminContactEmail">{{ adminContactEmail }}</a>
                        </v-card-text>
                    </v-card>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>


<script setup lang="ts">
import { getUrl } from '@/shared/utils/get-url';
import { computed, onMounted } from 'vue';
import { useStore } from 'vuex';

const store = useStore();
const adminContactEmail = computed(() => store.state.adminSiteSettingsModule.adminContactEmail);

onMounted(() => {
    store.dispatch('getAdminContactEmail');
});

</script>

<style>
    .no-role-container {
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
