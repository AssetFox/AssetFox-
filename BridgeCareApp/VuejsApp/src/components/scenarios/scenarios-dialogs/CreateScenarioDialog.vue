<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card elevation="5" outlined class="modal-pop-up-padding">
            <v-card-title>
                <h3 class="dialog-header">
                    Create new scenario
                </h3>
                <v-spacer></v-spacer>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>

            <v-card-text>
              <v-select
                    :items="stateNetworks"
                    label="Select a network"
                    item-text="name"
                    v-model="networkMetaData"
                    return-object
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
import { State } from 'vuex-class';
import { getUserName } from '@/shared/utils/get-user-info';
import { User } from '@/shared/models/iAM/user';
import {
    emptyScenario,
    Scenario,
    ScenarioUser,
} from '@/shared/models/iAM/scenario';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { find, isNil, propEq } from 'ramda';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';

@Component
export default class CreateScenarioDialog extends Vue {
    @Prop() showDialog: boolean;

    @State(state => state.userModule.users) stateUsers: User[];
    @State(state => state.networkModule.networks) stateNetworks: Network[];

    newScenario: Scenario = { ...emptyScenario, id: getNewGuid() };
    shared: boolean = false;
    selectedNetworkId: string = getBlankGuid();
    isNetworkSelected: boolean = false;
    networkMetaData: Network = {...emptyNetwork}
    selectedNetworkName: string;

    @Watch('showDialog')
    onShowDialogChanged() {
        this.onModifyScenarioUserAccess();
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
        if (this.showDialog) {
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
                ...this.newScenario,
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
    }
}
</script>
