<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card>
            <v-card-title>
                <v-layout justify-center>
                    <h3>New Network</h3>
                </v-layout>
            </v-card-title>
            <v-card-text>
                <v-layout column>
                    <v-text-field label="Name" outline v-model="createNetworkData.name"></v-text-field>
                </v-layout>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn :disabled="createNetworkData.name === ''" @click="onSubmit(true)"
                           class="ara-blue-bg white--text">
                        Save
                    </v-btn>
                    <v-btn @click="onSubmit(false)" class="ara-orange-bg white--text">Cancel</v-btn>
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
    import Vue from 'vue';
    import {Component, Prop, Watch} from 'vue-property-decorator';
    import { emptyCreateNetworkData, NetworkCreationData } from '@/shared/models/iAM/networkCore';
    import {clone} from 'ramda';
    import {getUserName} from '../../../shared/utils/get-user-info';

    @Component
    export default class CreateScenarioDialog extends Vue {
        @Prop() showDialog: boolean;

        createNetworkData: NetworkCreationData = clone(emptyCreateNetworkData);
        public: boolean = false;

        mounted() {
            this.createNetworkData.creator = getUserName();
            this.createNetworkData.owner = this.createNetworkData.creator;
        }

        @Watch('public')
        onSetPublic() {
            this.createNetworkData.owner = this.public ? undefined : getUserName();
        }

        /**
         * 'Submit' button has been clicked
         */
        onSubmit(submit: boolean) {
            if (submit) {
                this.$emit('submit', this.createNetworkData);
            } else {
                this.$emit('submit', null);
            }

            this.createNetworkData = {...emptyCreateNetworkData};
            this.createNetworkData.creator = getUserName();
            this.createNetworkData.owner = this.createNetworkData.creator;
            this.public = false;
        }
    }
</script>
