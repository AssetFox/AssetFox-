<template>
    <v-dialog max-width="450px" v-model="showDialogComputed">
        <v-card elevation="5"  class="modal-pop-up-padding">
            <v-card-title>
                <v-row justify="space-between">
                <h3 class="dialog-header">
                    Create new scenario
                </h3>                
                <v-btn @click="onSubmit(false)" flat>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-row>
            </v-card-title>

            <v-card-text>
              <v-select
                    id="CreateScenarioDialog-selectANetwork-select"
                    :items="stateNetworks"
                    label="Select a network"
                    item-title="name"
                    menu-icon=custom:GhdDownSvg
                    v-model="networkMetaData"
                    return-object
                    @update:modelValue="selectedNetwork(`${networkMetaData.name}`, `${networkMetaData.id}`)"
                    density="default"
                    variant="outlined"
                ></v-select>
                <v-text-field
                    id="CreateScenarioDialog-scenarioName-textField"
                    label="Scenario name"
                    variant="outlined"
                    v-model="newScenario.name"
                    :error-messages="scenarioNameErrors"
                    @input="validateScenarioName"
                ></v-text-field>
                <v-checkbox v-model="shared" label="Share with all?" />
            </v-card-text>
            <v-card-actions>
                <v-row justify-space-between row>
                    <v-btn
                        variant="text"
                        id="CreateScenarioDialog-cancel-btn"
                        @click="onSubmit(false)"
                        class="ghd-white-bg ghd-blue ghd-button-text"
                    >
                        Cancel
                    </v-btn>
                    <v-btn
                        id="CreateScenarioDialog-save-btn"
                        :disabled="isSaveButtonDisabled"
                        @click="onSubmit(true)"
                        class="ghd-blue ghd-button-text"
                        variant="outlined"
                    >
                        Save
                    </v-btn>
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { computed, ref, shallowReactive, watch } from 'vue'; 
import { getUserName } from '@/shared/utils/get-user-info';
import { User } from '@/shared/models/iAM/user';
import {
    emptyScenario,
    Scenario,
    ScenarioUser,
} from '@/shared/models/iAM/scenario';
import { getBlankGuid, getNewGuid } from '@/shared/utils/uuid-utils';
import { emptyNetwork, Network } from '@/shared/models/iAM/network';
import { useStore } from 'vuex'; 
import { validate } from 'uuid';
import {
    InputValidationRules,
    rules as validationRules,
} from '@/shared/utils/input-validation-rules';
import { find, isNil, propEq } from 'ramda';

  let store = useStore(); 
  async function getNetworksAction(payload?: any): Promise<any> { await store.dispatch('getNetworks', payload);}

  let rules: InputValidationRules = validationRules;
  const scenarioNameErrors = ref<string[]>([]);
  const props = defineProps<{showDialog: boolean}>();
  const emit = defineEmits(['submit'])
  let showDialogComputed = computed(() => props.showDialog);

    const stateUsers = computed<User[]>(() => store.state.userModule.users);
    let shared = ref<boolean>(false);
    let stateNetworks = computed<Network[]>(() => store.state.networkModule.networks) ;

    let newScenario = ref<Scenario>({ ...emptyScenario, id: getNewGuid() });

    let selectedNetworkId = ref(getBlankGuid());
    let isNetworkSelected = ref(false);
    let networkMetaData = ref<Network>({...emptyNetwork})
    let selectedNetworkName: string;

    watch(() => props.showDialog,()=> onShowDialogChanged())
    function onShowDialogChanged() {
        onModifyScenarioUserAccess();
    }

    watch(shared, ()=> onSetPublic())
    function onSetPublic() {
        onModifyScenarioUserAccess();
    }

    function selectedNetwork(networkName: string, networkId: string){
      selectedNetworkId.value = networkId;
      selectedNetworkName = networkName;
      if(networkId != '' && !isNil(networkId) && networkId != getBlankGuid()){
        isNetworkSelected.value = true;
      }
      else{
        isNetworkSelected.value = false;
      }
    }

    function onModifyScenarioUserAccess() {
        if (props.showDialog) {
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

            newScenario.value = {
                ...newScenario.value,
                networkId: selectedNetworkId.value,
                users: shared.value
                    ? [
                          owner,
                          ...stateUsers.value
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

    function validateScenarioName() {
        scenarioNameErrors.value = [];

        if (!newScenario.value.name) {
            scenarioNameErrors.value.push('This field is required');
        }

        const specialCharError = rules.generalRules.valueContainsNoSpecialCharacters(newScenario.value.name);
        if (specialCharError !== true) {
            scenarioNameErrors.value.push(specialCharError as string);
        }
    }

    const hasValidationErrors = computed(() => scenarioNameErrors.value.length > 0);

    const isSaveButtonDisabled = computed(() => 
        newScenario.value.name === '' || !isNetworkSelected.value || hasValidationErrors.value
    );

    // Trigger validation on name change
    watch(() => newScenario.value.name, validateScenarioName);

    function onSubmit(submit: boolean) {
        stateNetworks = store.state.networkModule.networks;
        if (submit) {
            newScenario.value.networkId = selectedNetworkId.value;
            newScenario.value.networkName = selectedNetworkName;
            emit('submit', newScenario.value);
        } else {
            emit('submit', null);
        }

        newScenario.value = { ...emptyScenario, id: getNewGuid() };
        shared = ref(false);
    }
</script>
