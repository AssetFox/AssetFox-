<template>
    <v-layout>
        <v-dialog style="overflow-y: auto" max-width='800px' persistent scrollable v-model="dialogDataPreChecks.showDialog">
            <v-card>
                <v-card-title class="ghd-dialog-box-padding-top">
                    <v-layout justify-space-between align-center>
                        <div class="ghd-control-dialog-header">Interactive pre-checks</div>
                        <v-btn @click="onSubmit(false)" variant = "flat" class="ghd-close-button">
                            X
                        </v-btn>
                    </v-layout>
                </v-card-title>
                    <v-layout justify-center style="font-weight: 500">
                        {{dialogDataPreChecks.heading}}
                    </v-layout>
                <div style='height: 100%; max-width:100%' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 450px; overflow-y:auto;'>
                        <v-card-text style="border:1px solid black;" class="px-4">
                            <ul>
                                <li class="text--primary" v-for="(key, index) in dialogDataPreChecks.message" :key="index">
                                {{key}}
                                </li>
                            </ul>
                         </v-card-text>
                    </div>
                </div>
                <v-card-actions>
                    <v-layout justify-center row v-if="dialogDataPreChecks.choice">
                        <v-btn 
                        id="Alert-Cancel-vbtn"
                        @click="onSubmit(false)" 
                        class="ghd-blue ghd-button" variant = "outline">
                            Cancel
                        </v-btn>
                        <v-btn 
                        id="Alert-Proceed-vbtn"
                        @click="onSubmit(true)" 
                        class="ghd-blue-bg ghd-white ghd-button">
                            Proceed
                        </v-btn>
                    </v-layout>
                    <v-layout justify-center v-if="!dialogDataPreChecks.choice">
                        <v-btn @click="onSubmit(true)" class="ara-blue-bg white--text">
                            OK
                        </v-btn>
                    </v-layout>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-layout>
</template>

<script lang="ts" setup>
    import Vue from 'vue';
    import {AlertPreChecksData} from '../models/modals/alert-data';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';

    const props = defineProps<{
        dialogDataPreChecks: AlertPreChecksData
    }>()

const emit = defineEmits(['submit'])

        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        function onSubmit(submit: boolean) {
            emit('submit', submit);
        }
    
</script>
