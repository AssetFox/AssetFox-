<template>
    <v-container fluid grid-list-xl>
        <v-row>
            <v-col cols = "12">
                <v-row justify-center>
                    <v-card class="under-construction-card">
                        <div class="under-construction-img-div">
                            <v-img :src="getUrl('@/assets/images/under-construction.png')"
                                   class="under-construction-img">
                            </v-img>
                        </div>
                        <v-card-title>
                            <h3>Page Not Yet Ready</h3>
                        </v-card-title>
                        <v-card-text>
                            <p>The page you are viewing has not yet been developed, but we are hard at work on it.</p>
                        </v-card-text>
                    </v-card>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script lang="ts" setup>
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
import { useStore } from 'vuex';
import { useRouter } from 'vue-router';
import { getUrl } from '@/shared/utils/get-url';

</script>

<style>
    .under-construction-card {
        width: 800px;
    }

    .under-construction-img-div {
        width: 100%;
    }

    .under-construction-img {
        width: 75%;
        margin: 0 auto;
    }
</style>
