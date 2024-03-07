<template>
    <v-dialog max-width="450px" persistent v-model="dialogData.showDialog">
        <v-card elevation="5" 
         class="modal-pop-up-padding">
            <v-card-title>
                <v-row justify="space-between">
                <h3 class="dialog-header">
                    Clone scenario
                </h3>
                <v-btn @click="onSubmit(false)" flat>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-row>
            </v-card-title>
            <v-card-text>
              <v-select
                    :items="stateNetworks"
                    label="Select a network"
                    item-title="name"
                    v-model="networkMetaData"
                    menu-icon=custom:GhdDownSvg
                    return-object
                    @update:modelValue="selectedNetwork(`${networkMetaData.name}`, `${networkMetaData.id}`)"
                    density="compact"
                    variant="outlined"
                ></v-select>
                <v-text-field
                    id="CloneScenarioDialog-scenarioName-textField"
                    label="Scenario name"
                    variant="outlined"
                    density="compact"
                    v-model="dialogData.scenario.name"
                ></v-text-field>
                <v-checkbox v-model="shared" label="Share with all?" />
            </v-card-text>
            <v-card-actions>
                <v-row justify="center">
                    <v-btn
                        id="CloneScenarioDialog-cancel-btn"
                        @click="onSubmit(false)"
                        class="ghd-button ghd-blue"
                        >Cancel</v-btn
                    >
                    <v-btn
                        id="CloneScenarioDialog-save-btn"
                        :disabled="dialogData.scenario.name === '' || !isNetworkSelected"
                        variant="outlined"
                        @click="onSubmit(true)"
                        class="ghd-button ghd-blue"
                    >
                        Save
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import { toRefs, computed, ref, watch } from 'vue'; 
import { getUserName } from '@/shared/utils/get-user-info';
import { User } from '@/shared/models/iAM/user';
import {
    emptyScenario,
    Scenario,
    ScenarioUser,
    CloneScenario,
    emptyCloneScenario,
} from '@/shared/models/iAM/scenario';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { find, isNil, propEq, clone } from 'ramda';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import {CloneScenarioDialogData,CloneSimulationDialogData} from '@/shared/models/modals/clone-scenario-dialog-data';
import { useStore } from 'vuex'; 

    let store = useStore(); 
    const props = defineProps<{dialogData: CloneSimulationDialogData}>();
    const emit = defineEmits(['submit']);
    const { dialogData } = toRefs(props);

    const stateUsers =  computed<User[]>(() => store.state.userModule.users);
    const stateNetworks = computed<Network[]>(() => store.state.networkModule.networks);  
    let shared =  ref<boolean>(false);
    async function cloneScenarioWithDestinationNetworkAction(payload?:any): Promise<any>{await store.dispatch('cloneScenarioWithDestinationNetwork',payload)}
    async function getCompatibleNetworksAction(payload?: any): Promise<any>{await store.dispatch('getCompatibleNetworks',payload)}

    let newScenario: CloneScenario = { ...emptyCloneScenario, id: getNewGuid() };
    let selectedNetworkId: string = getBlankGuid();
    let isNetworkSelected = ref(false);
    let networkMetaData = ref<Network>({...emptyNetwork})
    let selectedNetworkName: string;
    let hasCompatibleNetworks: boolean = false;

    watch(dialogData,()=> {
        onModifyScenarioUserAccess();
    });

    watch(stateNetworks, ()=>  {
        selectedNetworkId = props.dialogData.scenario.networkId;
        selectedNetworkName = props.dialogData.scenario.networkName;
         networkMetaData.value =  stateNetworks.value.find(_ => _.id == props.dialogData.scenario.networkId) ||  networkMetaData.value;
         isNetworkSelected.value = true;
        hasCompatibleNetworks = true;
    });

    watch(shared, ()=> {
        onModifyScenarioUserAccess();
    });

    function selectedNetwork(networkName: string, networkId: string){
      selectedNetworkId = networkId;
      selectedNetworkName = networkName;
      if(networkId != '' && !isNil(networkId) && networkId != getBlankGuid()){
         isNetworkSelected.value = true;
      }
      else{
         isNetworkSelected.value = false;
      }
    }

    function onModifyScenarioUserAccess() {
        if (props.dialogData.showDialog) {
            const currentUser: User = find(
                propEq('username', getUserName()),
                 stateUsers.value,
            ) as User;
            const owner: ScenarioUser = {
                userId: currentUser.id,
                username: currentUser.username,
                canModify: true,
                isOwner: true,
            };

            newScenario = {
                ...props.dialogData.scenario,
                destinationNetworkId: selectedNetworkId,
                networkId: props.dialogData.scenario.networkId,
                users: shared.value
                    ? [
                          owner,
                          ... stateUsers.value
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
            newScenario.destinationNetworkId = selectedNetworkId;
            newScenario.networkId = props.dialogData.scenario.networkId
            newScenario.networkName = selectedNetworkName;      
            newScenario.name = props.dialogData.scenario.name;
            emit('submit', newScenario);
        } else {
            emit('submit', null);
        }

        newScenario = { ...emptyCloneScenario, id: getNewGuid() };
        shared = ref(false);
        hasCompatibleNetworks = false;
    }

</script>
