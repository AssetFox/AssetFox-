<template>
  <v-dialog max-width="450px" persistent v-model="showDialogComputed">
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
                 class="assetFox-blue-bg text-white">
            Save
          </v-btn>
          <v-btn @click="onSubmit(false)" class="assetFox-orange-bg text-white">Cancel</v-btn>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import Vue, { computed } from 'vue';
import {emptyNetwork, Network} from '@/shared/models/iAM/network';
import {getNewGuid} from '@/shared/utils/uuid-utils';

  const props = defineProps<{showDialog: boolean}>();
  const emit = defineEmits(['submit'])

  let showDialogComputed = computed(() => props.showDialog);

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
