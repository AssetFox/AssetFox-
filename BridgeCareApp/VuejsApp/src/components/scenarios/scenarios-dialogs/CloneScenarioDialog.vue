<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card elevation="5" outlined class="modal-pop-up-padding">
            <v-card-title>
                <h3 class="dialog-header">
                    Clone scenario
                </h3>
                <v-spacer></v-spacer>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>

            <v-card-text>
              <v-select
                    :items="stateCompatibleNetworks"
                    label="Select a compatible network"
                    item-text="name"
                    v-model="networkMetaData"
                    return-object
                    v-if="hasCompatibleNetworks"
                    v-on:change="selectedNetwork(`${networkMetaData.name}`, `${networkMetaData.id}`)"
                    dense
                    outline
                ></v-select>
                <v-text-field
                    label="Scenario name"
                    outline
                    v-model="newScenario.name"
                ></v-text-field>
                <v-checkbox v-model="shared" label="Share with all?" />
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        :disabled="newScenario.name === '' || !isNetworkSelected"
                        @click="onSubmit(true)"
                        class="ara-blue-bg white--text"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        @click="onSubmit(false)"
                        class="ara-orange-bg white--text"
                        >Cancel</v-btn
                    >
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop, Watch } from 'vue-property-decorator';
import { Action, State } from 'vuex-class';
import { getUserName } from '@/shared/utils/get-user-info';
import { User } from '@/shared/models/iAM/user';
import {
    emptyScenario,
    Scenario,
    ScenarioUser,
} from '@/shared/models/iAM/scenario';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { find, isNil, propEq, clone } from 'ramda';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import {CloneScenarioDialogData} from '@/shared/models/modals/clone-scenario-dialog-data';

@Component
export default class CloneScenarioDialog extends Vue {
    @Prop() dialogData: CloneScenarioDialogData;

    @State(state => state.userModule.users) stateUsers: User[];
    @State(state => state.networkModule.compatibleNetworks) stateCompatibleNetworks: Network[];

    @Action("getCompatibleNetworks") getCompatibleNetworksAction: any;

    newScenario: Scenario = { ...emptyScenario, id: getNewGuid() };
    shared: boolean = false;
    selectedNetworkId: string = getBlankGuid();
    isNetworkSelected: boolean = false;
    networkMetaData: Network = {...emptyNetwork}
    selectedNetworkName: string;
    hasCompatibleNetworks: boolean = false;

    @Watch('dialogData')
    onDialogDataChanged() {
        this.onModifyScenarioUserAccess();
    }

    @Watch('stateCompatibleNetworks')
    onStateCompatibleNetworksChanged() {

        this.selectedNetworkId = this.dialogData.scenario.networkId;
        this.selectedNetworkName = this.dialogData.scenario.networkName;
        this.networkMetaData = this.stateCompatibleNetworks.find(_ => _.id == this.dialogData.scenario.networkId) || this.networkMetaData;
        this.isNetworkSelected = true;


        this.hasCompatibleNetworks = true;
    }

    @Watch('shared')
    onSetPublic() {
        this.onModifyScenarioUserAccess();
    }

    selectedNetwork(networkName: string, networkId: string){
      this.selectedNetworkId = networkId;
      this.selectedNetworkName = networkName;
      if(networkId != '' && !isNil(networkId) && networkId != getBlankGuid()){
        this.isNetworkSelected = true;
      }
      else{
        this.isNetworkSelected = false;
      }
    }

    onModifyScenarioUserAccess() {
        if (this.dialogData.showDialog) {
            const currentUser: User = find(
                propEq('username', getUserName()),
                this.stateUsers,
            ) as User;
            const owner: ScenarioUser = {
                userId: currentUser.id,
                username: currentUser.username,
                canModify: true,
                isOwner: true,
            };

            this.newScenario = {
                ...this.dialogData.scenario,
                networkId: this.selectedNetworkId,
                users: this.shared
                    ? [
                          owner,
                          ...this.stateUsers
                              .filter(
                                  (user: User) =>
                                      user.username !== currentUser.username,
                              )
                              .map((user: User) => ({
                                  userId: user.id,
                                  username: user.username,
                                  canModify: false,
                                  isOwner: false,
                              })),
                      ]
                    : [owner]
            };

            this.getCompatibleNetworksAction({networkId: this.dialogData.scenario.networkId});
        }
    }

    onSubmit(submit: boolean) {
        if (submit) {
            this.newScenario.networkId = this.selectedNetworkId;
            this.newScenario.networkName = this.selectedNetworkName;
            this.$emit('submit', this.newScenario);
        } else {
            this.$emit('submit', null);
        }

        this.newScenario = { ...emptyScenario, id: getNewGuid() };
        this.shared = false;
        this.hasCompatibleNetworks = false;
    }
}
</script>
