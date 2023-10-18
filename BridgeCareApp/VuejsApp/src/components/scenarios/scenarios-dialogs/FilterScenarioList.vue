<template>
    <v-dialog max-width="450px" persistent v-bind:show="showDialog">
        <v-card elevation="5" variant = "outlined" class="modal-pop-up-padding">
            <v-card-title>
                <h3 class="dialog-header">
                    Filter For Scenarios
                </h3>
                <v-spacer></v-spacer>
                <v-btn @click="onSubmit(false)" icon>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-card-title>

            <v-card-text>
              <v-select
                    id="FilterScenarioList-selectAFilter-select"
                    :items="filters"
                    label="Select a filter"
                    item-title="name"
                    v-model="FilterCategory"
                    return-object
                    v-on:change="selectedFilter(`${FilterCategory}`, `${FilterValue}`)"
                    density="default"
                    variant = "outlined"
                ></v-select>
                <v-text-field
                    id="CreateScenarioDialog-scenarioName-textField"
                    label="Filter Value"
                    outline
                    v-model="FilterValue"
                ></v-text-field>
            </v-card-text>
            <v-card-actions>
                <v-row justify-space-between row>
                    <v-btn
                        id="CreateScenarioDialog-save-btn"
                        @click="onSubmit(true)"
                        class="ara-blue-bg text-white"
                    >
                        Filter
                    </v-btn>
                    <v-btn
                        id="CreateScenarioDialog-cancel-btn"
                        @click="onSubmit(false)"
                        class="ara-orange-bg text-white"
                        >Cancel</v-btn
                    >
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { ref, watch } from 'vue'; 
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
import Dialog from 'primevue/dialog';

  let store = useStore(); 

  const props = defineProps<{showDialog: boolean}>();
  const emit = defineEmits(['submit'])

    let stateUsers = ref<User[]>(store.state.userModule.users);
    let shared = ref<boolean>(false);

    let filters: string[] = [
        "Scenario",
        "Owner",
        "Creator",
        "Network"
    ]
    let FilterCategory = '';
    let FilterValue = '';
    let isNetworkSelected: boolean = false;

    watch(()=> props.showDialog,()=> onShowDialogChanged)
    function onShowDialogChanged() {
        FilterCategory = '';
        FilterValue = '';
    }

    watch(shared, ()=> onSetPublic)
    function onSetPublic() {
        // ToDo - commented the below line as the function is missing
        //onModifyScenarioUserAccess();
    }

    function selectedFilter(FilterCategory: string, FilterValue: string){
      FilterCategory = FilterCategory;
      FilterValue = FilterValue;
      if(FilterCategory != '' && !isNil(FilterCategory)){
        isNetworkSelected = true;
      }
      else{
        isNetworkSelected = false;
      }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit', FilterCategory,FilterValue);
        } else {
            emit('submit', null);
        }

    }

</script>
