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
          <v-text-field label="Name" outline v-model="newNetwork.name"></v-text-field>
        </v-layout>
      </v-card-text>
      <v-card-actions>
        <v-layout justify-space-between row>
          <v-btn :disabled="newNetwork.name === ''" @click="onSubmit(true)"
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
import {Component, Prop} from 'vue-property-decorator';
import {emptyNetwork, Network} from '@/shared/models/iAM/network';
import {getNewGuid} from '@/shared/utils/uuid-utils';

@Component
export default class CreateNetworkDialog extends Vue {
  @Prop() showDialog: boolean;

  newNetwork: Network = {...emptyNetwork, id: getNewGuid()};

  onSubmit(submit: boolean) {
    if (submit) {
      this.$emit('submit', this.newNetwork);
    } else {
      this.$emit('submit', null);
    }

    this.newNetwork = {...emptyNetwork, id: getNewGuid()};
  }
}
</script>
