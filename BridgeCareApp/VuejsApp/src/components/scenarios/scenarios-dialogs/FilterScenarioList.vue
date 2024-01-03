<template>
    <v-dialog max-width="450px" persistent v-model="showDialogComputed">
        <v-card elevation="5"  class="modal-pop-up-padding">
            
            <v-card-title>
                <v-row justify="space-between">
                <h3 class="dialog-header">
                    Filter For Scenarios
                </h3>
                <v-btn @click="onSubmit(false)" flat>
                    <i class="fas fa-times fa-2x"></i>
                </v-btn>
            </v-row>
            </v-card-title>

            <v-card-text>
              <v-select
                    id="FilterScenarioList-selectAFilter-select"
                    :items="filters"
                    label="Select a filter"
                    item-title="name"
                    v-model="FilterCategory"
                    return-object
                    menu-icon=custom:GhdDownSvg
                    @update:modelValue="selectedFilter(`${FilterCategory}`, `${FilterValue}`)"
                    variant="outlined"
                    density="compact"
                ></v-select>
                <v-text-field
                    id="FilterScenarioList-scenarioName-textField"
                    label="Filter Value"
                    variant="outlined"
                    density="compact"
                    v-model="FilterValue"
                ></v-text-field>
            </v-card-text>
            <v-card-actions>
                <v-row justify="center">
                    <v-btn
                        id="FilterScenarioList-save-btn"
                        @click="onSubmit(true)"
                        class="ghd-button ghd-blue"
                    >
                        Filter
                    </v-btn>
                    <v-btn
                        id="FilterScenarioList-cancel-btn"
                        @click="onSubmit(false)"
                        class="ghd-button ghd-blue"
                        variant="outlined"
                        >Cancel</v-btn
                    >
                </v-row>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed, ref, watch } from 'vue'; 
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
  let showDialogComputed = computed(() => props.showDialog);
    let stateUsers = ref<User[]>(store.state.userModule.users);
    let shared = ref<boolean>(false);

    let filters: string[] = [
        "Scenario",
        "Owner",
        "Creator",
        "Network"
    ]
    let FilterCategory = ref('');
    let FilterValue = ref('');
    let isNetworkSelected: boolean = false;

    watch(()=> props.showDialog,()=> onShowDialogChanged)
    function onShowDialogChanged() {
         FilterCategory.value = '';
        FilterValue.value = '';
    }

    watch(shared, ()=> onSetPublic)
    function onSetPublic() {
        // ToDo - commented the below line as the function is missing
        //onModifyScenarioUserAccess();
    }

    function selectedFilter(localFilterCategory: string, localFilterValue: string){
       FilterCategory.value = localFilterCategory;
       FilterValue.value =  FilterValue.value;
      if( FilterCategory.value != '' && !isNil( FilterCategory.value)){
        isNetworkSelected = true;
      }
      else{
        isNetworkSelected = false;
      }
    }

    function onSubmit(submit: boolean) {
        if (submit) {
            emit('submit',  FilterCategory.value, FilterValue.value);
        } else {
            emit('submit', null);
        }

    }

</script>
