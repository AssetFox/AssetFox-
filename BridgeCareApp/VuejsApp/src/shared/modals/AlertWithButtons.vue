<template>
    <v-dialog max-width="350" persistent v-model="dialogDataWithButtons.showDialog">
        <v-card>
            <v-card-title class="headline">
                <v-layout justify-center>
                    {{dialogDataWithButtons.heading}}
                </v-layout>
            </v-card-title>
            <v-card-text class="px-4">
                <div class="text--primary">
                {{dialogDataWithButtons.message}}
                </div>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-center row v-if="dialogDataWithButtons.choice">
                    <v-btn 
                    id="Alert-Cancel-vbtn"
                    @click="onReturn('cancel')" 
                    class="ghd-blue ghd-button" outline>
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
                    class="ghd-blue ghd-button" outline>
                        Continue
                    </v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import {AlertDataWithButtons} from '../models/modals/alert-data';
    
    @Component
    export default class AlertWithButtons extends Vue {
        @Prop() dialogDataWithButtons: AlertDataWithButtons;

        /**
         * Emits a boolean result to the parent component
         * @param submit
         */
        onReturn(submit: string) {
            this.$emit('submit', submit);
        }
    }
</script>
