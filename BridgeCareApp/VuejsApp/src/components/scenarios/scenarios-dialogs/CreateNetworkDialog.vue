<template>
  <Dialog max-width="450px" persistent v-bind:show="showDialog">
    <v-card>
      <v-card-title>
        <v-row justify-center>
          <h3>New Network</h3>
        </v-row>
      </v-card-title>
      <v-card-text>
        <v-row column>
          <v-text-field label="Name" outline v-model="newNetwork.name"></v-text-field>
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-row justify-space-between row>
          <v-btn :disabled="newNetwork.name === ''" @click="onSubmit(true)"
                 class="ara-blue-bg text-white">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="ara-orange-bg text-white">Cancel</v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </Dialog>
</template>

<script lang="ts" setup>
import Vue from 'vue';
import {emptyNetwork, Network} from '@/shared/models/iAM/network';
import {getNewGuid} from '@/shared/utils/uuid-utils';
import Dialog from 'primevue/dialog';

  const props = defineProps<{showDialog: boolean}>();
  const emit = defineEmits(['submit'])

  let newNetwork: Network = {...emptyNetwork, id: getNewGuid()};

  function onSubmit(submit: boolean) {
    if (submit) {
      emit('submit', newNetwork);
    } else {
      emit('submit', null);
    }

    newNetwork = {...emptyNetwork, id: getNewGuid()};
  }

</script>
