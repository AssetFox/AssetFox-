<template>
    <v-dialog max-width="350" persistent v-model="showDialogComputed">
        <v-card>
            <v-card-title class="headline">
                <v-row justify-center>
                    {{dialogDataWithButtons.heading}}
                </v-row>
            </v-card-title>
            <v-card-text class="px-4">
                <div class="text--primary">
                {{dialogDataWithButtons.message}}
                </div>
            </v-card-text>
            <v-card-actions>
                <v-row justify-center row v-if="dialogDataWithButtons.choice">
                    <v-btn 
                    id="Alert-Cancel-vbtn"
                    @click="onReturn('cancel')" 
                    class="ghd-blue ghd-button" variant = "outlined">
                        Cancel
                    </v-btn>
                    <v-btn 
                    id="Alert-Proceed-vbtn"
                    @click="onReturn('pre-checks')" 
                    class="ghd-blue-bg ghd-white ghd-button">
                        Run Pre-Checks
                    </v-btn>
                    <v-btn 
                    id="Alert-Cancel-vbtn"
                    @click="onReturn('continue')" 
                    class="ghd-blue ghd-button" variant = "outlined">
                        Continue
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
    import Vue, { computed } from 'vue';
    import {AlertDataWithButtons} from '../models/modals/alert-data';
    import {inject, reactive, ref, onMounted, onBeforeUnmount, watch, Ref} from 'vue';
    import { useStore } from 'vuex';
    import { useRouter } from 'vue-router';
    
    const props = defineProps<{
        dialogDataWithButtons: AlertDataWithButtons
    }>()
    let showDialogComputed = computed(() => props.dialogDataWithButtons.showDialog);
const emit = defineEmits(['submit'])


        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        function onReturn(submit: string) {
            emit('submit', submit);
        }
    
</script>
