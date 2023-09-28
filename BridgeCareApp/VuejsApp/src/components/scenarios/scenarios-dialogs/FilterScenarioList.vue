<template>
    <v-dialog max-width="450px" persistent v-model="showDialog">
        <v-card elevation="5" outlined class="modal-pop-up-padding">
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
                    item-text="name"
                    v-model="FilterCategory"
                    return-object
                    v-on:change="selectedFilter(`${FilterCategory}`, `${FilterValue}`)"
                    dense
                    variant = "outline"
                ></v-select>
                <v-text-field
                    id="CreateScenarioDialog-scenarioName-textField"
                    label="Filter Value"
                    outline
                    v-model="FilterValue"
                ></v-text-field>
            </v-card-text>
            <v-card-actions>
                <v-layout justify-space-between row>
                    <v-btn
                        id="CreateScenarioDialog-save-btn"
                        @click="onSubmit(true)"
                        class="ara-blue-bg white--text"
                    >
                        Filter
                    </v-btn>
                    <v-btn
                        id="CreateScenarioDialog-cancel-btn"
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
export default class FilterScenarioList extends Vue {
    @Prop() showDialog: boolean;

    @State(state => state.userModule.users) stateUsers: User[];
    filters: string[] = [
        "Scenario",
        "Owner",
        "Creator",
        "Network"
    ]
    FilterCategory = '';
    FilterValue = '';

    @Watch('showDialog')
    onShowDialogChanged() {
        this.FilterCategory = '';
        this.FilterValue = '';
    }

    @Watch('shared')
    onSetPublic() {
        this.onModifyScenarioUserAccess();
    }

    selectedFilter(FilterCategory: string, FilterValue: string){
      this.FilterCategory = FilterCategory;
      this.FilterValue = FilterValue;
      if(FilterCategory != '' && !isNil(FilterCategory)){
        this.isNetworkSelected = true;
      }
      else{
        this.isNetworkSelected = false;
      }
    }


    onSubmit(submit: boolean) {
        if (submit) {
            this.$emit('submit', this.FilterCategory,this.FilterValue);
        } else {
            this.$emit('submit', null);
        }

    }
}
</script>
