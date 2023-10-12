<template>
    <v-dialog max-width="290" persistent v-bind:show="dialogData.showDialog">
        <v-card>
            <v-card-title class="headline">
                <v-row justify-center>
                    {{dialogData.heading}}
                </v-row>
            </v-card-title>
            <v-card-text class="px-4">
                <div class="text--primary">
                {{dialogData.message}}
                </div>
            </v-card-text>
            <v-card-actions>
                <v-row justify-center row v-if="dialogData.choice">
                    <v-btn 
                    id="Alert-Cancel-vbtn"
                    @click="onSubmit(false)" 
                    class="ghd-blue ghd-button" variant = "outlined">
                        Cancel
                    </v-btn>
                    <v-btn 
                    id="Alert-Proceed-vbtn"
                    @click="onSubmit(true)" 
                    class="ghd-blue-bg ghd-white ghd-button">
                        Proceed
                    </v-btn>
                </v-row>
                <v-row justify-center v-if="!dialogData.choice">
                    <v-btn @click="onSubmit(true)" class="ara-blue-bg text-white">
                        OK
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
    import Vue from 'vue';
    import {AlertData} from '../models/modals/alert-data';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';

const emit = defineEmits(['submit'])
const props = defineProps<{
    dialogData: AlertData
    }>()

        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        function onSubmit(submit: boolean) {
            emit('submit', submit);
        }
    
</script>
