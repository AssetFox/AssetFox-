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
                    id="CloneScenarioDialog-scenarioName-textField"
                    label="Scenario name"
                    outline
                    v-model="dialogData.scenario.name"
                ></v-text-field>
                <v-checkbox v-model="shared" label="Share with all?" />
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        id="CloneScenarioDialog-save-btn"
                        :disabled="dialogData.scenario.name === '' || !isNetworkSelected"
                        @click="onSubmit(true)"
                        class="ara-blue-bg white--text"
                    >
                        Save
                    </v-btn>
                    <v-btn
                        id="CloneScenarioDialog-cancel-btn"
                        @click="onSubmit(false)"
                        class="ara-orange-bg white--text"
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
import { find, isNil, propEq, clone } from 'ramda';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import {CloneScenarioDialogData} from '@/shared/models/modals/clone-scenario-dialog-data';
import { useStore } from 'vuex'; 

    let store = useStore(); 
    const props = defineProps<{dialogData: CloneScenarioDialogData}>();
    const emit = defineEmits(['submit']);

    const stateUsers =  shallowReactive<User[]>(store.state.userModule.users);
    const stateCompatibleNetworks = shallowReactive<Network[]>(store.state.networkModule.compatibleNetworks);  
    let shared =  ref<boolean>(false);

    async function getCompatibleNetworksAction(payload?: any): Promise<any>{await store.dispatch('getCompatibleNetworks')}

    let newScenario: Scenario = { ...emptyScenario, id: getNewGuid() };
    let selectedNetworkId: string = getBlankGuid();
    let isNetworkSelected: boolean = false;
    let networkMetaData: Network = {...emptyNetwork}
    let selectedNetworkName: string;
    let hasCompatibleNetworks: boolean = false;

    watch(()=> props.dialogData,()=> onDialogDataChanged)
    function onDialogDataChanged() {
        onModifyScenarioUserAccess();
    }

    watch(stateCompatibleNetworks, ()=> onStateCompatibleNetworksChanged)
    function onStateCompatibleNetworksChanged() {

        selectedNetworkId = props.dialogData.scenario.networkId;
        selectedNetworkName = props.dialogData.scenario.networkName;
        networkMetaData = stateCompatibleNetworks.find(_ => _.id == props.dialogData.scenario.networkId) || networkMetaData;
        isNetworkSelected = true;
        hasCompatibleNetworks = true;
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
        if (props.dialogData.showDialog) {
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
                ...props.dialogData.scenario,
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

            getCompatibleNetworksAction({networkId: props.dialogData.scenario.networkId});
        }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            newScenario.networkId = selectedNetworkId;
            newScenario.networkName = selectedNetworkName;      
            newScenario.name = props.dialogData.scenario.name;
            emit('submit', newScenario);
        } else {
            emit('submit', null);
        }

        newScenario = { ...emptyScenario, id: getNewGuid() };
        shared = ref(false);
        hasCompatibleNetworks = false;
    }

</script>
