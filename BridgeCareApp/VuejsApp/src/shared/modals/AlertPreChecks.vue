<template>
    <v-row>
        <v-dialog style="overflow-y: auto" max-width='800px' persistent scrollable v-model="showDialogComputed">
            <v-card>
                <v-card-title class="ghd-dialog-box-padding-top">
                    <v-row justify-space-between align-center>
                        <div style="margin-left: 250px" class="ghd-control-dialog-header">Interactive pre-checks</div>
                        <v-btn style='margin-left:241px; font-size: 17px' @click="onSubmit(false)" variant = "flat">
                            X
                        </v-btn>
                    </v-row>
                </v-card-title>
                    <div justify-center style="font-weight: 500; margin-left:50px; margin-top: 0px">
                        {{dialogDataPreChecks.heading}}
                    </div>
                <div style='height: 100%; max-width:100%' class="ghd-dialog-box-padding-center">
                    <div style='max-height: 450px; overflow-y:auto;'>
                        <v-card-text style="border:1px solid black;" class="px-4">
                            <ul>
                                <li class="text--primary" v-for="(key, index) in filteredMessages" :key="index">
                                    {{ key }}                                
                                </li>
                            </ul>
                         </v-card-text>
                    </div>
                </div>
                <v-card-actions>
                    <v-row style="margin-left:20px" justify-center row v-if="dialogDataPreChecks.choice">
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
                    <v-row v-if="!dialogDataPreChecks.choice" style="padding-left: 35px">
                        <v-btn @click="onSubmit(false)" class="assetFox-blue-bg text-white">
                            OK
                        </v-btn>
                    </v-row>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-row>
</template>

<script lang="ts" setup>
    import Vue, { computed } from 'vue';
    import {AlertPreChecksData} from '../models/modals/alert-data';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    
    const props = defineProps<{
        dialogDataPreChecks: AlertPreChecksData
    }>()
    let showDialogComputed = computed(() => props.dialogDataPreChecks.showDialog);
    const emit = defineEmits(['submit'])


    const filteredMessages = computed(() => {
    const message = props.dialogDataPreChecks.message;

    if (props.dialogDataPreChecks.message) {
        // Split the message by periods and filter out empty strings
        const sentences = message.split('.');
        
        var filtered = sentences.filter(function (el) {
            return el != null && el != '';
        });

        return filtered;
    }
    return [];
});        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        function onSubmit(submit: boolean) {
            emit('submit', submit);
        }
    
</script>
