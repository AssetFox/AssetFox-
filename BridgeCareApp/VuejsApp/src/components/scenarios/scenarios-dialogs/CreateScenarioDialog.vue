<template>
    <v-dialog max-width="450px" persistent v-bind:show="showDialog">
        <v-card elevation="5" variant = "outlined" class="modal-pop-up-padding">
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
                    id="CreateScenarioDialog-selectANetwork-select"
                    :items="stateNetworks"
                    label="Select a network"
                    item-title="name"
                    v-model="networkMetaData"
                    return-object
                    v-on:change="selectedNetwork(`${networkMetaData.name}`, `${networkMetaData.id}`)"
                    density="default"
                    variant="outlined"
                ></v-select>
                <v-text-field
                    id="CreateScenarioDialog-scenarioName-textField"
                    label="Scenario name"
                    outline
                    v-model="newScenario.name"
                ></v-text-field>
                <v-checkbox v-model="shared" label="Share with all?" />
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        id="CreateScenarioDialog-save-btn"
                        :disabled="newScenario.name === '' || !isNetworkSelected"
                        @click="onSubmit(true)"
                        class="ara-blue-bg text-white"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        id="CreateScenarioDialog-cancel-btn"
                        @click="onSubmit(false)"
                        class="ara-orange-bg text-white"
                        >Cancel</v-btn
                    >
                </v-layout>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { ref, shallowReactive, watch } from 'vue'; 
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
import { useStore } from 'vuex'; 

  let store = useStore(); 

  const props = defineProps<{showDialog: boolean}>();
  const emit = defineEmits(['submit'])

    const stateUsers: User[] = shallowReactive(store.state.userModule.users);
    let shared = ref<boolean>(false);
    let stateNetworks = ref<Network[]>(store.state.networkModule.networks) ;

    let newScenario: Scenario = { ...emptyScenario, id: getNewGuid() };
    
    let selectedNetworkId: string = getBlankGuid();
    let isNetworkSelected: boolean = false;
    let networkMetaData: Network = {...emptyNetwork}
    let selectedNetworkName: string;

    watch(()=> props.showDialog,()=> onShowDialogChanged)
    function onShowDialogChanged() {
        onModifyScenarioUserAccess();
    }

    watch(shared, ()=> onSetPublic)
    function onSetPublic() {
        onModifyScenarioUserAccess();
    }

    function selectedNetwork(networkName: string, networkId: string){
      selectedNetworkId = networkId;
      selectedNetworkName = networkName;
      if(networkId != '' && !isNil(networkId) && networkId != getBlankGuid()){
        isNetworkSelected = true;
      }
      else{
        isNetworkSelected = false;
      }
    }

    function onModifyScenarioUserAccess() {
        if (props.showDialog) {
            const currentUser: User = find(
                propEq('username', getUserName()),
                stateUsers,
            ) as User;
            const owner: ScenarioUser = {
                userId: currentUser.id,
                username: currentUser.username,
                canModify: true,
                isOwner: true,
            };

            newScenario = {
                ...newScenario,
                networkId: selectedNetworkId,
                users: shared
                    ? [
                          owner,
                          ...stateUsers
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

    function onSubmit(submit: boolean) {
        if (submit) {
            newScenario.networkId = selectedNetworkId;
            newScenario.networkName = selectedNetworkName;
            emit('submit', newScenario);
        } else {
            emit('submit', null);
        }

        newScenario = { ...emptyScenario, id: getNewGuid() };
        shared = ref(false);
    }

</script>
